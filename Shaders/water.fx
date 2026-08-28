// Lost Labyrinth screen-space water refraction.
// Ported from the shipping HLSL in gmWaterSurface.cpp:289-346, which the mobile builds
// compiled out. The C++ vertex shader consumed pre-transformed D3DFVF_XYZRHW positions and
// only subtracted the D3D9 half-texel; XNA has no such vertex format, so positions come in
// as screen pixels and MatrixTransform does the projection.

#include "Macros.fxh"

float4x4 MatrixTransform;

DECLARE_TEXTURE(TexColor, 0);   // the scene, copied from the render target
DECLARE_TEXTURE(TexOffset, 1);  // wave map, drives the refraction offset
DECLARE_TEXTURE(TexGray, 2);    // vertical ramp

// The PC build fed three texcoord sets through the vertex format. The PS3 one instead derives
// the screen UV from clip space and the ramp coordinate from vertex alpha (gmWaterSurface.cpp
// :372-380), which is the same result from a stock VertexPositionColorTexture, so follow that.
struct VS_INPUT
{
    float4 Pos   : POSITION0;
    float4 Color : COLOR0;
    float2 UV    : TEXCOORD0;
};

struct VS_OUTPUT
{
    float4 Pos   : POSITION0;
    float4 Color : COLOR0;
    float2 UV0   : TEXCOORD0;
    float2 UV1   : TEXCOORD1;
    float2 UV2   : TEXCOORD2;
};

VS_OUTPUT WaterVS(VS_INPUT input)
{
    VS_OUTPUT output;
    output.Pos = mul(input.Pos, MatrixTransform);
    output.UV0 = output.Pos.xy * float2(0.5f, -0.5f) + 0.5f;
    output.UV1 = input.UV;
    output.UV2 = float2(0.5f, input.Color.a);
    output.Color = input.Color;
    return output;
}

float4 WaterPS(VS_OUTPUT input) : COLOR0
{
    float4 uv = SAMPLE_TEXTURE(TexOffset, input.UV1);

    // the original 0.03/0.015 is commented out just above this line in the C++;
    // someone cut the distortion 3x late and both the PC and PS3 copies carry the edit
    uv.zw = uv.zw * 0.01f - 0.005f;

    // channels are crossed on purpose: x takes w, y takes z
    uv.x = input.UV0.x + uv.w;
    uv.y = input.UV0.y + uv.z;

    uv = input.Color * SAMPLE_TEXTURE(TexColor, uv.xy);
    uv *= SAMPLE_TEXTURE(TexGray, input.UV2);
    uv.w = 1.0f;
    return uv;
}

technique Water
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL WaterVS();
        PixelShader  = compile PS_SHADERMODEL WaterPS();
    }
}
