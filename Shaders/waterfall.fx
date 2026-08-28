// Lost Labyrinth waterfall distortion.
//
// Transcribed from NNSTDSHADER/SHADER.AMB entry 247 (0000000000004RDMRC20006000.PSH), the
// nnstdshader permutation the waterfall's material selects on PC/360. See docs/nnstdshader.md
// for how that archive was recovered and read.
//
// The original is fed by NN's shader system, which supplies the projection as four user
// uniforms and picks the permutation from the material's profile. We have no per-material
// shader selection, so the material callback binds this effect and sets its parameters, and
// the projection comes in as an ordinary matrix.

#include "Macros.fxh"

// setupEffect never touches World on the stock effects, so it stays identity; only these two
float4x4 View;
float4x4 Projection;

// u_TexBaseAlpha / u_TexDecalAlpha in the original
float BaseAlpha;
float DecalAlpha;

float4 FogColor;

DECLARE_TEXTURE(TexBase, 0);            // offset map, drives the refraction
DECLARE_TEXTURE(TexDecal, 1);           // the waterfall's own artwork
DECLARE_TEXTURE(TexUserSampler2D1, 2);  // the scene, copied from the render target

struct VS_INPUT
{
    float4 Pos      : POSITION0;
    float2 UVBase   : TEXCOORD0;
    float4 Color    : COLOR0;
    float2 UVDecal  : TEXCOORD1;
};

struct VS_OUTPUT
{
    float4 Pos      : POSITION0;
    float4 Color    : COLOR0;
    float2 UVBase   : TEXCOORD0;
    float2 UVDecal  : TEXCOORD1;
    float4 Clip     : TEXCOORD2;
};

VS_OUTPUT WaterfallVS(VS_INPUT input)
{
    VS_OUTPUT output;
    float4 view = mul(input.Pos, View);
    output.Pos = mul(view, Projection);

    // the original reconstructs this from u_UserUniform[0..3]; same result, one matrix
    output.Clip = output.Pos;
    output.Color = input.Color;
    output.UVBase = input.UVBase;
    output.UVDecal = input.UVDecal;
    return output;
}

float4 WaterfallPS(VS_OUTPUT input) : COLOR0
{
    float4 decal = SAMPLE_TEXTURE(TexDecal, input.UVDecal);
    float da = decal.w * DecalAlpha;

    // The original leaves this to the material's alpha test, which an override effect bypasses.
    // Without it the transparent parts of the fall's quad still write colour and depth and
    // punch a hole in the terrain behind.
    clip(da - (1.0f / 255.0f));

    float4 base = SAMPLE_TEXTURE(TexBase, input.UVBase);

    // x from blue, y from alpha, exactly as the disassembly reads them
    float2 ofs;
    ofs.x = base.z * 0.01f - 0.00501960795f;
    ofs.y = (base.w * BaseAlpha) * 0.01f - 0.00501960795f;

    float2 ndc = input.Clip.xy / input.Clip.w;
    float2 uv = ndc * float2(0.5f, -0.5f) + ofs + 0.5f;

    float3 scene = SAMPLE_TEXTURE(TexUserSampler2D1, uv).rgb;

    // the decal sits over the refracted scene, weighted by its own alpha
    float3 col = saturate(lerp(scene, decal.rgb, da) * input.Color.rgb);

    // ceil(da) in the original: the silhouette is a hard mask, not a soft edge
    return float4(col, ceil(da) * input.Color.a);
}

technique Waterfall
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL WaterfallVS();
        PixelShader  = compile PS_SHADERMODEL WaterfallPS();
    }
}
