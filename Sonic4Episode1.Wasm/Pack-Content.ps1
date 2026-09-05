#!/usr/bin/env pwsh
# Lays out the web head's content: a boot bundle the game extracts before it starts, plus one
# bundle per group of archives that amFs fetches when a level asks for them.
#
# The exclusions are the whole reason this is separate from s4content: they are what the *browser*
# does not need, not what the content is.

param(
    [string[]]$Profiles = @(
        "mobile-sd-png", "mobile-sd-astc",
        "mobile-hd-dxt", "mobile-hd-astc",
        "desktop-hd-dxt", "desktop-hd-astc"
    ),
    [string]$ContentBuild = "$PSScriptRoot/../Content.Build",
    [string]$Shaders = "$PSScriptRoot/../Shaders/FNA",
    [string]$Output = "$PSScriptRoot/wwwroot/sets",
    [string]$Streamed = "$PSScriptRoot/wwwroot/Content",
    # a two segment group smaller than this is not worth its own request
    [int]$BundleThresholdBytes = 8MB
)

$ErrorActionPreference = "Stop"
$Shaders = (Resolve-Path $Shaders).Path

# music is served as ogg straight from wwwroot; the wav are MGCB inputs and the runtime reads
# the xnb beside them; nothing reads the CSB
function Test-Excluded {
    param([string]$Path)

    if ($Path -like "SOUND/*/*") { return $Path -notlike "*.xnb" }
    if ($Path -like "SOUND/*.wav") { return $true }
    if ($Path -like "SOUND/*.CSB") { return $true }
    if ($Path -eq ".notshipped") { return $true }

    return $false
}

# Everything a synchronous reader can touch has to be on the filesystem before the game starts, so
# only archives amFs reads in the background are worth bundling. SHADER.AMB is an archive but
# mppLoadStdShader reads it at draw time with nowhere to await, so it boots with the rest
function Test-Bundled {
    param([string]$Path)

    if ($Path -eq "NNSTDSHADER/SHADER.AMB") { return $false }
    return $Path -like "*.AMB"
}

# BGM is deliberately not in any bundle: CriAuBgmSource fetches a track at a time, gated by the
# preload at the level load barrier. The oggs are byte identical across every profile, so they are
# laid out once beside the sets rather than duplicated into each one. Additive: anything already
# here that these do not replace is left alone
function Copy-StreamedAudio {
    param([string]$Tree, [string]$Destination)

    $from = Join-Path $Tree "SOUND"
    if (-not (Test-Path $from)) { throw "no SOUND directory in $Tree" }

    $copied = 0
    $bytes = 0
    foreach ($file in Get-ChildItem -LiteralPath $from -File -Recurse -Filter *.ogg) {
        $relative = [IO.Path]::GetRelativePath($from, $file.FullName)
        # only the per track subdirectories; a loose ogg at the SOUND root is not a BGM part
        if ($relative -eq [IO.Path]::GetFileName($relative)) { continue }

        $target = Join-Path (Join-Path $Destination "SOUND") $relative
        New-Item -ItemType Directory -Force (Split-Path $target -Parent) | Out-Null

        $existing = Get-Item -LiteralPath $target -ErrorAction SilentlyContinue
        if ($existing -and $existing.Length -eq $file.Length) { continue }

        Copy-Item -LiteralPath $file.FullName -Destination $target -Force
        $copied++
        $bytes += $file.Length
    }

    Write-Host ("streamed audio: {0} files copied, {1:N1} MB" -f $copied, ($bytes / 1MB))
}

# one gzipped tar from a set of (Source, Path) entries. tar takes a single -C, so anything that does
# not live under the content tree is staged at its archive path and appended
function Write-Archive {
    param([object[]]$Entries, [string]$Root, [string]$Target)

    $fromTree = @($Entries | Where-Object { $_.FromTree })
    $appended = @($Entries | Where-Object { -not $_.FromTree })

    $tarFile = [IO.Path]::GetTempFileName()
    Remove-Item -LiteralPath $tarFile

    if ($fromTree.Count -gt 0) {
        $listFile = [IO.Path]::GetTempFileName()
        Set-Content -LiteralPath $listFile -Value ($fromTree | ForEach-Object { $_.Path }) -Encoding ASCII
        tar -C $Root -cf $tarFile -T $listFile
        if ($LASTEXITCODE -ne 0) { throw "tar failed for $Target" }
        Remove-Item -LiteralPath $listFile
    }

    if ($appended.Count -gt 0) {
        $staged = Join-Path ([IO.Path]::GetTempPath()) ("s4pack-" + [IO.Path]::GetFileNameWithoutExtension($Target))
        if (Test-Path $staged) { Remove-Item -LiteralPath $staged -Recurse -Force }

        foreach ($entry in $appended) {
            $destination = Join-Path $staged $entry.Path
            New-Item -ItemType Directory -Force (Split-Path $destination -Parent) | Out-Null
            Copy-Item -LiteralPath $entry.Source -Destination $destination -Force
        }

        $stagedList = [IO.Path]::GetTempFileName()
        Set-Content -LiteralPath $stagedList -Value ($appended | ForEach-Object { $_.Path }) -Encoding ASCII

        $create = if (Test-Path $tarFile) { "-rf" } else { "-cf" }
        tar -C $staged $create $tarFile -T $stagedList
        if ($LASTEXITCODE -ne 0) { throw "tar failed appending to $Target" }

        Remove-Item -LiteralPath $stagedList
        Remove-Item -LiteralPath $staged -Recurse -Force
    }

    $tarStream = [IO.File]::OpenRead($tarFile)
    $gzFile = [IO.File]::Create($Target)
    $gzip = [IO.Compression.GZipStream]::new($gzFile, [IO.Compression.CompressionLevel]::Optimal)
    $tarStream.CopyTo($gzip)
    $gzip.Dispose(); $gzFile.Dispose(); $tarStream.Dispose()
    Remove-Item -LiteralPath $tarFile

    return (Get-Item -LiteralPath $Target).Length
}

New-Item -ItemType Directory -Force $Output | Out-Null
$sha = [System.Security.Cryptography.SHA256]::Create()
$laidOutStreamedAudio = $false

foreach ($profile in $Profiles) {
    $tree = Join-Path $ContentBuild $profile
    if (-not (Test-Path $tree)) {
        throw "profile '$profile' has not been built. Run: dotnet run --project Sonic4Episode1.Content -- build $profile"
    }

    if (-not $laidOutStreamedAudio) {
        Copy-StreamedAudio -Tree $tree -Destination $Streamed
        $laidOutStreamedAudio = $true
    }

    Write-Host "packing $profile"

    # (source, tar path) pairs, so more than one source tree can land in one archive
    $root = (Resolve-Path $tree).Path
    $included = [System.Collections.Generic.List[object]]::new()
    foreach ($file in Get-ChildItem -LiteralPath $root -File -Recurse) {
        $relative = [IO.Path]::GetRelativePath($root, $file.FullName).Replace('\', '/')
        if (-not (Test-Excluded $relative)) {
            $included.Add([pscustomobject]@{
                Source = $file.FullName; Path = $relative; Length = $file.Length; FromTree = $true })
        }
    }

    # only the wasm head lacks Shaders.targets, so nothing else assembles these into its Content
    # directory. Without them the NN stdshader never loads and console geometry draws untextured.
    # The permutations ship as NNSTDSHADER/SHADER.AMB, so the loose blobs it was packed from do not
    foreach ($file in Get-ChildItem -LiteralPath $Shaders -File -Recurse) {
        $relative = [IO.Path]::GetRelativePath($Shaders, $file.FullName).Replace('\', '/')
        if ($relative -like "nnstd/*") { continue }

        $path = if ($relative -like "NNSTDSHADER/*") { $relative } else { "Shaders/$relative" }
        $included.Add([pscustomobject]@{
            Source = $file.FullName; Path = $path; Length = $file.Length; FromTree = $false })
    }

    $included = @($included | Sort-Object -Property Path -CaseSensitive)

    # size the two segment groups first, because whether a path lands in one depends on how big the
    # whole group turned out
    $groupBytes = @{}
    foreach ($entry in $included) {
        if (-not (Test-Bundled $entry.Path)) { continue }

        $parts = $entry.Path.Split('/')
        $key = if ($parts.Count -ge 2) { $parts[0] + '/' + $parts[1] } else { $parts[0] }
        if (-not $groupBytes.ContainsKey($key)) { $groupBytes[$key] = [long]0 }
        $groupBytes[$key] += $entry.Length
    }

    $boot = [System.Collections.Generic.List[object]]::new()
    $bundles = [ordered]@{}
    foreach ($entry in $included) {
        if (-not (Test-Bundled $entry.Path)) {
            $boot.Add($entry)
            continue
        }

        $parts = $entry.Path.Split('/')
        $two = if ($parts.Count -ge 2) { $parts[0] + '/' + $parts[1] } else { $parts[0] }
        $key = if ($groupBytes[$two] -ge $BundleThresholdBytes) { $two } else { $parts[0] }
        $name = $key.Replace('/', '_').ToLowerInvariant()

        if (-not $bundles.Contains($name)) { $bundles[$name] = [System.Collections.Generic.List[object]]::new() }
        $bundles[$name].Add($entry)
    }

    $setDir = Join-Path $Output $profile
    New-Item -ItemType Directory -Force $setDir | Out-Null
    Get-ChildItem -LiteralPath $setDir -File | Remove-Item -Force

    $bootBytes = Write-Archive -Entries $boot -Root $root -Target (Join-Path $setDir "boot.tar.gz")

    # over file contents rather than the tar bytes: gzip embeds a timestamp, so hashing the archive
    # would change the version on every run and force a redownload of identical content
    $digest = [System.IO.MemoryStream]::new()
    foreach ($entry in $included) {
        $bytes = [Text.Encoding]::UTF8.GetBytes($entry.Path)
        $digest.Write($bytes, 0, $bytes.Length)
        $stream = [IO.File]::OpenRead($entry.Source)
        $fileHash = $sha.ComputeHash($stream)
        $stream.Dispose()
        $digest.Write($fileHash, 0, $fileHash.Length)
    }

    $digest.Position = 0
    $version = [BitConverter]::ToString($sha.ComputeHash($digest)).Replace("-", "").ToLowerInvariant()
    $digest.Dispose()

    # a path list rather than a prefix rule the runtime would have to reimplement and keep in step.
    # 500-odd paths is a few KB, and it cannot silently misroute
    $manifest = [System.Collections.Generic.List[string]]::new()
    $manifest.Add("version $version")
    $manifest.Add("boot boot.tar.gz $bootBytes")

    $bundleBytes = 0L
    foreach ($name in $bundles.Keys) {
        $entries = $bundles[$name]
        $size = Write-Archive -Entries $entries -Root $root -Target (Join-Path $setDir "$name.tar.gz")
        $bundleBytes += $size

        $manifest.Add("bundle $name $size")
        foreach ($entry in $entries) { $manifest.Add($entry.Path) }
    }

    Set-Content -LiteralPath (Join-Path $setDir "manifest.txt") -Value $manifest -Encoding ASCII

    Write-Host ("  boot {0:N1} MB, {1} bundles {2:N1} MB, {3} files, {4}" -f `
        ($bootBytes / 1MB), $bundles.Count, ($bundleBytes / 1MB), $included.Count, $version.Substring(0, 12))
}
