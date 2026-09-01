<#
.SYNOPSIS
Compiles nnstdshader permutations, one per NN shader key.

.DESCRIPTION
Each key is the 26 digit base 32 name NN gives a shader profile (see
docs/nn-stdshader-ubershader.md). This decodes a key back into NNS_STDSHADER_PROFILE, turns the
profile into the NND_OPT_* defines nnMakeShaderHeader would have emitted, and compiles one blob.

Output is named by the key, so the runtime can compute a material's key and load it directly rather
than consulting a hand written table.

The FNA backend compiles a D3D9 effect with fxc into Shaders/FNA/nnstd/*.fxb. The MonoGame backend
compiles MGFX with mgfxc into Shaders/MonoGame/nnstd/*.mgfxo, because MonoGame's Effect ctor will
not take a D3D9 blob. Same keys, same defines, same shader source.

The default key list is every distinct name in the retail NNSTDSHADER/SHADER.AMB, 285 of them.
That takes a few minutes, which is why this is a script and not part of the normal build.

.EXAMPLE
./Build-Permutations.ps1
./Build-Permutations.ps1 -Backend MonoGame
./Build-Permutations.ps1 -Keys 0000000000000RDMQC0000M1C4 -Verbose
#>
[CmdletBinding()]
param(
    [ValidateSet('FNA', 'MonoGame')]
    [string] $Backend = 'FNA',
    # explicit keys, or a file of them one per line; defaults to the committed retail key list
    [string[]] $Keys,
    [string] $KeyFile = "$PSScriptRoot\..\..\Sonic4Episode1.Tests\Fixtures\nnstdshader-keys.txt",
    [string] $OutDir = "$PSScriptRoot\..\$Backend\nnstd",
    [string] $Fxc,
    [string] $Mgfxc = "$PSScriptRoot\..\mgfx\tool\mgfxc.exe",
    [int] $Jobs = [Environment]::ProcessorCount,
    [switch] $Force
)

$ErrorActionPreference = 'Stop'

$Extension = if ($Backend -eq 'MonoGame') { '.mgfxo' } else { '.fxb' }

if ($Backend -eq 'FNA') {
    if (-not $Fxc) {
        $Fxc = (Get-ChildItem "${env:ProgramFiles(x86)}\Windows Kits\10\bin\*\x64\fxc.exe" -ErrorAction SilentlyContinue |
            Select-Object -Last 1).FullName
    }
    if (-not $Fxc -or -not (Test-Path $Fxc)) { throw "fxc.exe not found; install the Windows SDK or pass -Fxc" }
}
else {
    # a stock mgfxc cannot compile nnstdshader at all, its MojoShader rejects struct uniforms,
    # see docs/monogame-mojoshader-ctab.md
    if (-not (Test-Path $Mgfxc)) {
        throw "patched mgfxc not found at $Mgfxc; run Shaders\mgfx\Build-Mgfxc.ps1"
    }
    $Mgfxc = (Resolve-Path $Mgfxc).Path
}

if (-not $Keys) { $Keys = Get-Content $KeyFile | Where-Object { $_.Trim() } }

# {bias, width} per field, must stay in step with AppMain/Nn/NnStdShaderName.cs
$FieldNames = @(
    'bVtxCol','bVtxColSpe','bVtxNrm','bVtxObjSpaceNrm','bVtxReverseTan','VtxTfmMode',
    'bLighting','bSpecular','bTwoSidedLighting','nFragParallelLight','nFragPointLight',
    'NormalMapType','bBaseMap','nDecalMap','bSpecularMap','bShininessMap','bDualParaboloidMap',
    'bEnvMaskMap','bModulateMap','bAddMap','bOpacityMap',
    'bUser1Map','bUser2Map','bUser3Map','bUser4Map','bUser5Map','bUser6Map','bUser7Map','bUser8Map',
    'nShadowMap','bUserSampler2D1','bUserSampler2D2','bUserSampler3D1','bUserSampler3D2',
    'bUserSamplerCube1','bUserSamplerCube2',
    'TexCoord0','TexCoord1','TexCoord2','TexCoord3','TexCoord4','TexCoord5','TexCoord6','TexCoord7',
    'UserProfile','UserProfileDrawobj')

$FieldWidth = @(1,1,1,1,1,2,1,1,1,2,2,2,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,3,3,3,3,3,3,3,3,6,8)
$FieldBias  = @(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-3,-3,-3,-3,-3,-3,-3,-3,0,0)

$Base32 = '0123456789ABCDEFGHIJKLMNOPQRSTUV'

function ConvertFrom-ShaderKey([string] $key) {
    if ($key.Length -ne 26) { throw "'$key' is not 26 digits" }

    $v = [System.Numerics.BigInteger]::Zero
    foreach ($c in $key.ToCharArray()) {
        $d = $Base32.IndexOf([char]::ToUpper($c))
        if ($d -lt 0) { throw "'$c' is not a base 32 digit in '$key'" }
        $v = $v * 32 + $d
    }

    $p = @{}
    for ($i = 0; $i -lt $FieldNames.Count; $i++) {
        $mask = ([System.Numerics.BigInteger]1 -shl $FieldWidth[$i]) - 1
        $p[$FieldNames[$i]] = [int]($v -band $mask) + $FieldBias[$i]
        $v = $v -shr $FieldWidth[$i]
    }
    return $p
}

# order nnstdshader allocates texture matrices, sampler registers and UV slots in
$MapOrder = @(
    @{ opt = 'NORMAL';         present = { param($p) $p.NormalMapType -ne 0 };  texcoord = $true }
    @{ opt = 'BASE';           present = { param($p) $p.bBaseMap -ne 0 };       texcoord = $true }
    @{ opt = 'DECAL';          present = { param($p) $p.nDecalMap -ge 1 };      texcoord = $true }
    @{ opt = 'DECAL2';         present = { param($p) $p.nDecalMap -ge 2 };      texcoord = $true }
    @{ opt = 'DECAL3';         present = { param($p) $p.nDecalMap -ge 3 };      texcoord = $true }
    @{ opt = 'SPECULAR';       present = { param($p) $p.bSpecularMap -ne 0 };   texcoord = $true }
    @{ opt = 'SHININESS';      present = { param($p) $p.bShininessMap -ne 0 };  texcoord = $true }
    @{ opt = 'DUALPARABOLOID'; present = { param($p) $p.bDualParaboloidMap -ne 0 }; texcoord = $false }
    @{ opt = 'ENVMASK';        present = { param($p) $p.bEnvMaskMap -ne 0 };    texcoord = $true }
    @{ opt = 'MODULATE';       present = { param($p) $p.bModulateMap -ne 0 };   texcoord = $true }
    @{ opt = 'ADD';            present = { param($p) $p.bAddMap -ne 0 };        texcoord = $true }
    @{ opt = 'OPACITY';        present = { param($p) $p.bOpacityMap -ne 0 };    texcoord = $true }
    @{ opt = 'USER1';          present = { param($p) $p.bUser1Map -ne 0 };      texcoord = $true }
    @{ opt = 'USER2';          present = { param($p) $p.bUser2Map -ne 0 };      texcoord = $true }
    @{ opt = 'USER3';          present = { param($p) $p.bUser3Map -ne 0 };      texcoord = $true }
    @{ opt = 'USER4';          present = { param($p) $p.bUser4Map -ne 0 };      texcoord = $true }
    @{ opt = 'USER5';          present = { param($p) $p.bUser5Map -ne 0 };      texcoord = $true }
    @{ opt = 'USER6';          present = { param($p) $p.bUser6Map -ne 0 };      texcoord = $true }
    @{ opt = 'USER7';          present = { param($p) $p.bUser7Map -ne 0 };      texcoord = $true }
    @{ opt = 'USER8';          present = { param($p) $p.bUser8Map -ne 0 };      texcoord = $true }
    @{ opt = 'SHADOW';         present = { param($p) $p.nShadowMap -ge 1 };     texcoord = $false }
    @{ opt = 'SHADOW2';        present = { param($p) $p.nShadowMap -ge 2 };     texcoord = $false }
    @{ opt = 'SAMPLER2D1';     present = { param($p) $p.bUserSampler2D1 -ne 0 }; texcoord = $false }
    @{ opt = 'SAMPLER2D2';     present = { param($p) $p.bUserSampler2D2 -ne 0 }; texcoord = $false }
    @{ opt = 'SAMPLER3D1';     present = { param($p) $p.bUserSampler3D1 -ne 0 }; texcoord = $false }
    @{ opt = 'SAMPLER3D2';     present = { param($p) $p.bUserSampler3D2 -ne 0 }; texcoord = $false }
    @{ opt = 'SAMPLERCUBE1';   present = { param($p) $p.bUserSamplerCube1 -ne 0 }; texcoord = $false }
    @{ opt = 'SAMPLERCUBE2';   present = { param($p) $p.bUserSamplerCube2 -ne 0 }; texcoord = $false }
)

function ConvertTo-Defines([hashtable] $p) {
    $d = [ordered]@{}

    $d['NND_OPT_VTX_COLOR'] = $p.bVtxCol
    $d['NND_OPT_VTX_COLOR_SPECULAR'] = $p.bVtxColSpe
    $d['NND_OPT_VTX_NORMAL'] = $p.bVtxNrm
    $d['NND_OPT_VTX_OBJECT_SPACE_NORMAL'] = $p.bVtxObjSpaceNrm
    $d['NND_OPT_VTX_REVERSE_TANGENT'] = $p.bVtxReverseTan
    $d['NND_OPT_VTX_TFM_MODE'] = $p.VtxTfmMode

    $d['NND_OPT_LIGHTING'] = $p.bLighting
    $d['NND_OPT_SPECULAR'] = $p.bSpecular
    $d['NND_OPT_TWOSIDED_LIGHTING'] = $p.bTwoSidedLighting
    $d['NND_OPT_NUM_FRAG_PARALLEL_LIGHT'] = $p.nFragParallelLight
    $d['NND_OPT_NUM_FRAG_POINT_LIGHT'] = $p.nFragPointLight
    $d['NND_OPT_FRAG_LIGHTING'] = [int](($p.nFragParallelLight + $p.nFragPointLight) -gt 0)

    # NormalMapType 0 none, 1 RGB->XYZ, 2 AXLY
    $d['NND_OPT_TANGENT_SPACE'] = [int]($p.NormalMapType -ne 0)
    $d['NND_OPT_NORMALMAP_AXLY'] = [int]($p.NormalMapType -eq 2)

    # slot and texcoord indices are assigned in MapOrder order, independently of each other
    $slot = 0
    $coord = 0
    $maxTexCoord = 0
    foreach ($m in $MapOrder) {
        $present = & $m.present $p
        if (-not $present) {
            $d["NND_OPT_TEX_$($m.opt)"] = -1
            if ($m.texcoord) { $d["NND_OPT_TEXCOORD_$($m.opt)"] = -1 }
            continue
        }

        $d["NND_OPT_TEX_$($m.opt)"] = $slot
        $slot++

        if ($m.texcoord) {
            $c = $p["TexCoord$coord"]
            $coord++
            $d["NND_OPT_TEXCOORD_$($m.opt)"] = $c
            if ($c -ge 0 -and ($c + 1) -gt $maxTexCoord) { $maxTexCoord = $c + 1 }
        }
    }

    $d['NND_OPT_MAX_TEXMAP'] = $slot
    $d['NND_OPT_MAX_TEXCOORD'] = $maxTexCoord

    # nnMakeShaderHeader emits these one per bit rather than as an integer
    for ($bit = 0; $bit -lt 8; $bit++) {
        $mask = 1 -shl $bit
        $suffix = '{0:X2}' -f $mask
        $d["NND_USER_PROFILE_$suffix"] = [int](($p.UserProfile -band $mask) -ne 0)
        $d["NND_USER_PROFILE_DRAWOBJ_$suffix"] = [int](($p.UserProfileDrawobj -band $mask) -ne 0)
    }
    $d['NND_USER_PROFILE'] = $p.UserProfile
    $d['NND_USER_PROFILE_DRAWOBJ'] = $p.UserProfileDrawobj

    return $d
}

# must stay in step with $(NnStdShaderProfile) in Shaders/Shaders.targets
$ProfileDefines = @{
    NND_PROF_FOG_MODEL = 1
    NND_PROF_FRAG_FOG = 0
    NND_PROF_DISTANCE_FOG = 0
    NND_PROF_MAX_PARALLEL_LIGHT = 1
    NND_PROF_MAX_POINT_LIGHT = 0
    NND_PROF_MAX_SPOT_LIGHT = 0
    NND_PROF_VTX_SPECULAR = 0
    NND_PROF_CALC_VTX_BINORMAL = 0
}

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
$fx = Join-Path $PSScriptRoot 'nnstdshader.fx'
$include = $PSScriptRoot

$work = foreach ($key in $Keys) {
    $out = Join-Path $OutDir "$key$Extension"
    if (-not $Force -and (Test-Path $out) -and (Get-Item $out).LastWriteTime -gt (Get-Item $fx).LastWriteTime) {
        continue
    }

    $defines = ConvertTo-Defines (ConvertFrom-ShaderKey $key)

    if ($Backend -eq 'FNA') {
        $args = @('/nologo', '/T', 'fx_2_0', '/I', $include)
        foreach ($kv in $ProfileDefines.GetEnumerator()) { $args += '/D'; $args += "$($kv.Key)=$($kv.Value)" }
        foreach ($kv in $defines.GetEnumerator()) { $args += '/D'; $args += "$($kv.Key)=$($kv.Value)" }
        $args += @('/Fo', $out, $fx)
    }
    else {
        # mgfxc takes one semicolon separated /Defines and resolves #include against the source
        # file's own directory, so there is no /I to pass
        $pairs = @()
        foreach ($kv in $ProfileDefines.GetEnumerator()) { $pairs += "$($kv.Key)=$($kv.Value)" }
        foreach ($kv in $defines.GetEnumerator()) { $pairs += "$($kv.Key)=$($kv.Value)" }
        $args = @($fx, $out, '/Profile:OpenGL', "/Defines:$($pairs -join ';')")
    }

    [pscustomobject]@{ key = $key; args = $args; out = $out }
}

if (-not $work) { Write-Host "all $($Keys.Count) permutation(s) up to date"; return }

Write-Host "compiling $($work.Count) of $($Keys.Count) permutation(s) with $Jobs job(s)"

$results = $work | ForEach-Object -ThrottleLimit $Jobs -Parallel {
    $item = $_
    $exe = if ($using:Backend -eq 'MonoGame') { $using:Mgfxc } else { $using:Fxc }
    # X1519 is the pixel half re-running the TEXCOORD cascade, X3206 is NN's own mul(float3,
    # float4x4) UV transform, X4717 is fxc grumbling that effects are deprecated. All expected.
    $text = & $exe @($item.args) 2>&1 | Where-Object { $_ -notmatch 'X1519|X3206|X4717|^\s*$|compilation object save succeeded' }
    [pscustomobject]@{ key = $item.key; ok = ($LASTEXITCODE -eq 0); log = ($text -join "`n") }
}

$KnownUnbuildable = @{ '0000000000000RDMRC00000564' = 'vertex only in retail too, no .PSH exists' }

$failed = $results | Where-Object { -not $_.ok -and -not $KnownUnbuildable.ContainsKey($_.key) }
$expected = $results | Where-Object { -not $_.ok -and $KnownUnbuildable.ContainsKey($_.key) }

foreach ($e in $expected) { Write-Host "skipped $($e.key): $($KnownUnbuildable[$e.key])" }
foreach ($f in $failed) { Write-Warning "$($f.key)`n$($f.log)" }

# fxc writes nothing on failure, so drop any stale blob left over from a previous build
foreach ($stale in @($failed) + @($expected)) {
    $out = Join-Path $OutDir "$($stale.key)$Extension"
    if (Test-Path $out) { Remove-Item $out; Write-Verbose "removed stale $out" }
}

$noisy = $results | Where-Object { $_.ok -and $_.log }
foreach ($n in $noisy) { Write-Verbose "$($n.key): $($n.log)" }

$ok = @($results | Where-Object { $_.ok }).Count
Write-Host "compiled $ok, skipped $(@($expected).Count), failed $(@($failed).Count)"
if ($failed) { exit 1 }
