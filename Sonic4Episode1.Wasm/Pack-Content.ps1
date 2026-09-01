#!/usr/bin/env pwsh
# Makes one tar.gz per content profile for the web head. The tree itself comes from
# `s4content build <profile>`; this only selects, tars and hashes.
#
# The exclusions are the whole reason this is separate from s4content: they are what the *browser*
# does not need, not what the content is. Audio sizes and why each is safe to drop are in
# docs/web-content-delivery.md.

param(
    [string[]]$Profiles = @(
        "mobile-sd-png", "mobile-sd-astc",
        "mobile-hd-dxt", "mobile-hd-astc",
        "desktop-hd-dxt", "desktop-hd-astc"
    ),
    [string]$ContentBuild = "$PSScriptRoot/../Content.Build",
    [string]$Shaders = "$PSScriptRoot/../Shaders/FNA",
    [string]$Output = "$PSScriptRoot/wwwroot/sets"
)

$ErrorActionPreference = "Stop"
$Shaders = (Resolve-Path $Shaders).Path

# music is served as ogg/m4a straight from wwwroot; the wav are MGCB inputs and the runtime reads
# the xnb beside them; nothing reads the CSB
function Test-Excluded {
    param([string]$Path)

    if ($Path -like "SOUND/*/*") { return $Path -notlike "*.xnb" }
    if ($Path -like "SOUND/*.wav") { return $true }
    if ($Path -like "SOUND/*.CSB") { return $true }
    if ($Path -eq ".notshipped") { return $true }

    return $false
}

New-Item -ItemType Directory -Force $Output | Out-Null
$sha = [System.Security.Cryptography.SHA256]::Create()

foreach ($profile in $Profiles) {
    $tree = Join-Path $ContentBuild $profile
    if (-not (Test-Path $tree)) {
        throw "profile '$profile' has not been built. Run: dotnet run --project Sonic4Episode1.Content -- build $profile"
    }

    Write-Host "packing $profile"

    # (root, relative path) pairs, so more than one source tree can land in one archive
    $root = (Resolve-Path $tree).Path
    $included = [System.Collections.Generic.List[object]]::new()
    foreach ($file in Get-ChildItem -LiteralPath $root -File -Recurse) {
        $relative = [IO.Path]::GetRelativePath($root, $file.FullName).Replace('\', '/')
        if (-not (Test-Excluded $relative)) {
            $included.Add([pscustomobject]@{ Root = $root; Path = $relative })
        }
    }

    # only the wasm head lacks Shaders.targets, so nothing else assembles these into its Content
    # directory. Without them the NN stdshader never loads and console geometry draws untextured.
    foreach ($file in Get-ChildItem -LiteralPath $Shaders -File -Recurse) {
        $relative = [IO.Path]::GetRelativePath($Shaders, $file.FullName).Replace('\', '/')
        $included.Add([pscustomobject]@{ Root = $Shaders; Path = "Shaders/$relative" })
    }

    $included = $included | Sort-Object -Property Path -CaseSensitive

    $archive = Join-Path $Output "$profile.tar.gz"
    $staged = Join-Path ([IO.Path]::GetTempPath()) "s4pack-$profile"
    if (Test-Path $staged) { Remove-Item -LiteralPath $staged -Recurse -Force }

    # tar takes one -C per invocation, so the shaders are staged next to a link-free copy rather
    # than appended: 286 small files, cheaper than the alternative of two passes over 200 MB
    New-Item -ItemType Directory -Force (Join-Path $staged "Shaders") | Out-Null
    Copy-Item -Path (Join-Path $Shaders "*") -Destination (Join-Path $staged "Shaders") -Recurse -Force

    $listFile = [IO.Path]::GetTempFileName()
    Set-Content -LiteralPath $listFile -Value ($included | Where-Object { $_.Root -eq $root } | ForEach-Object { $_.Path }) -Encoding ASCII

    $tarFile = [IO.Path]::GetTempFileName()
    tar -C $root -cf $tarFile -T $listFile
    if ($LASTEXITCODE -ne 0) { throw "tar failed for $profile" }
    tar -C $staged -rf $tarFile "Shaders"
    if ($LASTEXITCODE -ne 0) { throw "tar failed appending shaders for $profile" }
    Remove-Item -LiteralPath $listFile
    Remove-Item -LiteralPath $staged -Recurse -Force

    $tarStream = [IO.File]::OpenRead($tarFile)
    $gzFile = [IO.File]::Create($archive)
    $gzip = [IO.Compression.GZipStream]::new($gzFile, [IO.Compression.CompressionLevel]::Optimal)
    $tarStream.CopyTo($gzip)
    $gzip.Dispose(); $gzFile.Dispose(); $tarStream.Dispose()
    Remove-Item -LiteralPath $tarFile

    # over file contents rather than the tar bytes: gzip embeds a timestamp, so hashing the archive
    # would change the version on every run and force a redownload of identical content
    $digest = [System.IO.MemoryStream]::new()
    foreach ($entry in $included) {
        $bytes = [Text.Encoding]::UTF8.GetBytes($entry.Path)
        $digest.Write($bytes, 0, $bytes.Length)
        $source = if ($entry.Root -eq $root) { Join-Path $root $entry.Path }
                  else { Join-Path $Shaders ($entry.Path -replace '^Shaders/', '') }
        $stream = [IO.File]::OpenRead($source)
        $fileHash = $sha.ComputeHash($stream)
        $stream.Dispose()
        $digest.Write($fileHash, 0, $fileHash.Length)
    }

    $digest.Position = 0
    $version = [BitConverter]::ToString($sha.ComputeHash($digest)).Replace("-", "").ToLowerInvariant()
    $digest.Dispose()

    # the count is the extractor's progress denominator, which was a hardcoded 1629 before
    Set-Content -LiteralPath (Join-Path $Output "$profile.sha256") `
        -Value "$version $($included.Count)" -Encoding ASCII -NoNewline

    $size = (Get-Item $archive).Length / 1MB
    Write-Host ("  {0} files, {1:N1} MB, {2}" -f $included.Count, $size, $version.Substring(0, 12))
}
