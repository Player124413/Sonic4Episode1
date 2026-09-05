<#
.SYNOPSIS
Builds SDL2, FNA3D and FAudio for UWP and stages them in Dependencies\FNA\lib-uwp.

.DESCRIPTION
The UWP head P/Invokes these three plus d3dcompiler_47.dll, which mojoshader links against
statically on WinRT. None of them are in the repo; Dependencies\ is gitignored.

SDL2 comes from a checkout you point at with -SdlDir. FNA3D and FAudio build out of
Submodules\FNA\lib, which is where their UWP vcxproj files live.

.EXAMPLE
.\Build-Natives.ps1 -SdlDir C:\src\SDL

.EXAMPLE
.\Build-Natives.ps1 -SdlDir C:\src\SDL -Configuration Debug -Platforms x64
#>
[CmdletBinding()]
param(
    [string] $SdlDir = "$env:USERPROFILE\Documents\GitHub\SDL",
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',
    [ValidateSet('x86', 'x64', 'arm64')]
    [string[]] $Platforms = @('x86', 'x64', 'arm64'),
    [switch] $SkipSdl
)

$ErrorActionPreference = 'Stop'

$root = Resolve-Path (Join-Path $PSScriptRoot '..')
$fnaLib = Join-Path $root 'Submodules\FNA\lib'
$fna3dProject = Join-Path $fnaLib 'FNA3D\visualc-winrt\FNA3D-UWP.vcxproj'
$faudioProject = Join-Path $fnaLib 'FAudio\visualc-winrt\FAudio-UWP.vcxproj'
$sdlProject = Join-Path $SdlDir 'VisualC-WinRT\SDL-UWP.vcxproj'
$redistRoot = Join-Path ${env:ProgramFiles(x86)} 'Windows Kits\10\Redist\D3D'

# MSBuild platform names differ from the ones the appx and PlatformTarget use, and SDL-UWP
# writes Win32 output with no platform folder at all.
$msbuildPlatform = @{ 'x86' = 'Win32'; 'x64' = 'x64'; 'arm64' = 'ARM64' }

function Resolve-MSBuild
{
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (-not (Test-Path $vswhere))
    {
        throw "vswhere.exe not found. Visual Studio 2019 or later is required."
    }

    $installs = & $vswhere -products * -requires Microsoft.Component.MSBuild -property installationPath
    foreach ($install in $installs)
    {
        $candidate = Join-Path $install 'MSBuild\Current\Bin\MSBuild.exe'
        if (Test-Path $candidate)
        {
            return $candidate
        }
    }

    throw "No MSBuild.exe found in any Visual Studio install."
}

function Invoke-Build([string] $project, [string] $platform, [string[]] $properties)
{
    $arguments = @(
        $project,
        "-p:Configuration=$Configuration",
        "-p:Platform=$platform",
        # SDL-UWP pins v142 on every config except x64, which fails on a VS2022-only box.
        '-p:PlatformToolset=v143',
        '-nologo',
        '-v:m'
    ) + $properties

    & $msbuild @arguments
    if ($LASTEXITCODE -ne 0)
    {
        throw "Build failed: $(Split-Path $project -Leaf) $Configuration|$platform"
    }
}

function Copy-Artifact([string] $source, [string] $destination)
{
    if (-not (Test-Path $source))
    {
        throw "Expected build output missing: $source"
    }

    Copy-Item $source $destination -Force
    Write-Host "    $(Split-Path $source -Leaf)"
}

if (-not (Test-Path $sdlProject))
{
    throw "SDL2 checkout not found at '$SdlDir'. Pass -SdlDir with the path to an SDL2 (not SDL3) checkout."
}

$msbuild = Resolve-MSBuild
Write-Host "MSBuild: $msbuild"
Write-Host "SDL2:    $SdlDir"

foreach ($platform in $Platforms)
{
    $target = $msbuildPlatform[$platform]
    Write-Host ""
    Write-Host "=== $Configuration | $platform (MSBuild '$target') ===" -ForegroundColor Cyan

    if (-not $SkipSdl)
    {
        Invoke-Build $sdlProject $target @()
    }

    Invoke-Build $fna3dProject $target @("-p:FNA3DSDL2Dir=$SdlDir")
    Invoke-Build $faudioProject $target @("-p:FAudioSDL2Dir=$SdlDir")

    # SDL-UWP: Win32 lands in VisualC-WinRT\Debug\, everything else in VisualC-WinRT\<Platform>\Debug\
    $sdlPlatformDir = if ($target -eq 'Win32') { '' } else { "$target\" }
    $sdlOut = Join-Path $SdlDir "VisualC-WinRT\$sdlPlatformDir$Configuration\SDL-UWP"
    $fna3dOut = Join-Path $fnaLib "FNA3D\visualc-winrt\$target\$Configuration"
    $faudioOut = Join-Path $fnaLib "FAudio\visualc-winrt\$target\$Configuration"

    $destination = Join-Path $root "Dependencies\FNA\lib-uwp\$platform"
    New-Item -ItemType Directory -Force -Path $destination | Out-Null
    Write-Host "  -> $destination"

    Copy-Artifact (Join-Path $sdlOut 'SDL2.dll') $destination
    Copy-Artifact (Join-Path $fna3dOut 'FNA3D.dll') $destination
    Copy-Artifact (Join-Path $faudioOut 'FAudio.dll') $destination

    $compiler = Join-Path $redistRoot "$platform\d3dcompiler_47.dll"
    if (Test-Path $compiler)
    {
        Copy-Artifact $compiler $destination
    }
    else
    {
        Write-Warning "d3dcompiler_47.dll not found at '$compiler'. mojoshader links it statically, so the app will fail to start without it."
    }
}

Write-Host ""
Write-Host "Done." -ForegroundColor Green
