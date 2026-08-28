// Splash Hill sea reflection.
//
// Transcribed from NNSTDSHADER/SHADER.AMB entry 232 (0000000000002RDMRC20006000.PSH). The sea
// model declares two texture maps (sea_refract.dds then sea_color.dds), so it takes one of the
// three-texture permutations, not the two-texture entry 441. See docs/nnstdshader.md.
//
// s_texBase is the refraction offset map and s_texDecal is the water's own surface colour,
// added over the refracted scene. The mobile material points at the single scrolling SEA_U_D
// instead, so the callback injects both by name.

#include "Macros.fxh"

float4x4 View;
float4x4 Projection;

float BaseAlpha;    // u_TexBaseAlpha, c1
float DecalAlpha;   // u_TexDecalAlpha, c2

float3 FogColor;    // u_Fog.color, c0
float FogStart;
float FogEnd;
float FogEnabled;

// nnPutMaterialCore pushes the material diffuse into the GL current colour, which is what
// stands in for v1 when the mesh carries no colour array. The stock effects get it as
// DiffuseColor; without it the sea loses its tint and comes out the fog colour.
float3 DiffuseColor;
float DiffuseAlpha;

DECLARE_TEXTURE(TexBase, 0);            // s_texBase, SEA_REFRACT
DECLARE_TEXTURE(TexDecal, 1);           // s_texDecal, SEA_COLOR
DECLARE_TEXTURE(TexUserSampler2D1, 2);  // s_texUserSampler2D1, the scene render target

struct VS_INPUT
{
    float4 Pos   : POSITION0;
    float2 UV    : TEXCOORD0;
    float4 Color : COLOR0;
};

struct VS_OUTPUT
{
    float4 Pos   : POSITION0;
    float4 Color : COLOR0;   // v1
    float2 UV    : TEXCOORD0;
    float2 Screen : TEXCOORD1;  // v3.xy, the screen UV the original builds in its vertex shader
    float Fog    : TEXCOORD2;   // v0.w
};

VS_OUTPUT SeaVS(VS_INPUT input)
{
    VS_OUTPUT output;
    float4 view = mul(input.Pos, View);
    float4 clip = mul(view, Projection);
    output.Pos = clip;
    output.Color = input.Color;
    output.UV = input.UV;
    output.Screen = (clip.xy / clip.w) * float2(0.5f, -0.5f) + 0.5f;

    float dist = abs(view.z);
    float fog = saturate((FogEnd - dist) / max(FogEnd - FogStart, 0.0001f));
    output.Fog = lerp(1.0f, fog, FogEnabled);
    return output;
}

float4 SeaPS(VS_OUTPUT input) : COLOR0
{
    // the original takes the decal from a second UV set (v2.zw); our model carries one
    float4 decal = SAMPLE_TEXTURE(TexDecal, input.UV);
    float da = decal.w * DecalAlpha;

    float4 base = SAMPLE_TEXTURE(TexBase, input.UV);

    float2 uv;
    uv.x = input.Screen.x + (base.z * 0.01f - 0.005f);

    // the vertical warp is 15x the horizontal; a grazing surface covers far more world
    // distance per screen pixel vertically, so the asymmetry is the point
    uv.y = input.Screen.y + ((base.w * BaseAlpha) * 0.01f - 0.005f) * 15.0f;

    float3 scene = SAMPLE_TEXTURE(TexUserSampler2D1, uv).rgb;

    float4 vcol = input.Color * float4(DiffuseColor, DiffuseAlpha);

    // the surface colour is added over the refracted scene, weighted by its own alpha
    float3 col = saturate(saturate(decal.rgb * da + scene) * vcol.rgb);

    return float4(lerp(FogColor, col, input.Fog), ceil(da) * vcol.a);
}

technique Sea
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL SeaVS();
        PixelShader  = compile PS_SHADERMODEL SeaPS();
    }
}
