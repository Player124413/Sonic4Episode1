// The SDK ships the user-profile hooks and none of the bodies; Sonic 4's live only in its built
// NNSTDSHADER/SHADER.AMB. The two recovered here are transcribed from that archive, see
// docs/nn-stdshader-ubershader.md. They go in the wrapper so the vendored SDK sources stay
// byte-identical.
#ifndef NND_USER_PROFILE
#define NND_USER_PROFILE            (0)
#endif
#ifndef NND_OPT_TEX_SAMPLER2D1
#define NND_OPT_TEX_SAMPLER2D1      (-1)
#endif

#if ( NND_USER_PROFILE != 0 ) && ( NND_OPT_TEX_SAMPLER2D1 != -1 )
#define NND_USER_PROFILE_REFRACTS   1
#else
#define NND_USER_PROFILE_REFRACTS   0
#endif

#ifdef NND_FLAT_UNIFORM_STRUCTS
#if ( NND_PROF_MAX_PARALLEL_LIGHT + NND_PROF_MAX_POINT_LIGHT + NND_PROF_MAX_SPOT_LIGHT ) > 1
#error NND_FLAT_LIGHT_SOURCE only covers one light
#endif

float4 u_FrontMaterialReg0, u_FrontMaterialReg1, u_FrontMaterialReg2, u_FrontMaterialReg3, u_FrontMaterialReg4;
float4 u_FogReg0, u_FogReg1, u_FogReg2, u_FogReg3, u_FogReg4;
float4 u_FrontLightModelProductReg0;
float4 u_LightSourceReg0,  u_LightSourceReg1,  u_LightSourceReg2,  u_LightSourceReg3;
float4 u_LightSourceReg4,  u_LightSourceReg5,  u_LightSourceReg6,  u_LightSourceReg7;
float4 u_LightSourceReg8,  u_LightSourceReg9,  u_LightSourceReg10, u_LightSourceReg11;

#include "nnstd-flat-uniforms.fxh"
#endif

#define main nnStdShaderVS
#include "nnstdshader/nnstdshader.vsh"
#undef main

#if NND_USER_PROFILE_REFRACTS
// Profile 1 reads the screen coordinate straight off the clip position; profile 2 carries the view
// position and projects it in the pixel half instead, through u_UserUniform. Written before the
// rename prologue below so g_ViewPos is still the vertex half's.
struct VS_OUTPUT_USERPROFILE
{
    VS_OUTPUT   base;
#if NND_USER_PROFILE == 1
    float2      UserCoord : TEXCOORD7;
#else
    float4      UserCoord : TEXCOORD7;
#endif
};

VS_OUTPUT_USERPROFILE nnStdShaderVSUserProfile(VS_INPUT Input)
{
    VS_OUTPUT_USERPROFILE output;
    output.base = nnStdShaderVS(Input);

#if NND_USER_PROFILE == 1
    float2 ndc = output.base.Pos.xy / output.base.Pos.w;
    output.UserCoord = ndc * float2(0.5f, -0.5f) + 0.5f;
#else
    output.UserCoord = g_ViewPos;
#endif

    return output;
}
#endif

#ifdef NND_FLAT_UNIFORM_STRUCTS
#undef NND_FLAT_LIGHT_SOURCE
#define NND_FLAT_LIGHT_SOURCE(name) NND_FLAT_LIGHT_SOURCE_PS(name)
#endif

#define MATERIAL_PARAMETERS         ps_MATERIAL_PARAMETERS
#define LIGHT_SOURCE_PARAMETERS     ps_LIGHT_SOURCE_PARAMETERS
#define LIGHT_MODEL_PRODUCTS        ps_LIGHT_MODEL_PRODUCTS
#define FOG_PARAMETERS              ps_FOG_PARAMETERS

#define u_TextureMatrix             ps_u_TextureMatrix
#define u_FrontMaterial             ps_u_FrontMaterial
#define u_Fog                       ps_u_Fog
#define u_LightSource               ps_u_LightSource
#define u_FrontLightModelProduct    ps_u_FrontLightModelProduct
#define u_PointLightFallOffEnd      ps_u_PointLightFallOffEnd
#define u_PointLightFallOffScale    ps_u_PointLightFallOffScale
#define u_TexUser1Param             ps_u_TexUser1Param
#define u_TexUser2Param             ps_u_TexUser2Param
#define u_TexUser3Param             ps_u_TexUser3Param
#define u_TexUser4Param             ps_u_TexUser4Param
#define u_TexUser5Param             ps_u_TexUser5Param
#define u_TexUser6Param             ps_u_TexUser6Param
#define u_TexUser7Param             ps_u_TexUser7Param
#define u_TexUser8Param             ps_u_TexUser8Param
#define u_UserUniform               ps_u_UserUniform

#define g_Input                     ps_g_Input
#define g_Output                    ps_g_Output
#define g_Normal                    ps_g_Normal
#define g_EyeDir                    ps_g_EyeDir
#define g_Tangent                   ps_g_Tangent
#define g_Binormal                  ps_g_Binormal

#define CalcSphereMapTexCoord       ps_CalcSphereMapTexCoord
#define CalcLight                   ps_CalcLight
#define CalcFog                     ps_CalcFog

#define main nnStdShaderPS
#include "nnstdshader/nnstdshader.psh"
#undef main

// D3D9 did alpha test in fixed function, XNA's is not
float4 u_AlphaTest = float4(0.0f, 0.0f, -1.0f, 0.0f);

PS_OUTPUT nnStdShaderPSAlphaTest(PS_INPUT Input)
{
    PS_OUTPUT output = nnStdShaderPS(Input);
    clip((output.Col.a - u_AlphaTest.x) * u_AlphaTest.y - u_AlphaTest.z);
    return output;
}

#if NND_USER_PROFILE_REFRACTS
struct PS_INPUT_USERPROFILE
{
    PS_INPUT    base;
#if NND_USER_PROFILE == 1
    float2      UserCoord : TEXCOORD7;
#else
    float4      UserCoord : TEXCOORD7;
#endif
};

// Both bodies replace the whole pipeline: no lighting, no CalcAlpha, and the base map is an
// offset field rather than a colour. The distortion is the same shape in each, the tuning is not.
PS_OUTPUT nnStdShaderPSUserProfile(PS_INPUT_USERPROFILE Input)
{
    g_Input = Input.base;
    SampleTexMap();

#if ( NND_OPT_TEX_DECAL != -1 )
    float  decalAlpha = g_smpDecal.a * u_TexDecalAlpha;
    float3 decalColor = g_smpDecal.rgb;
#else
    float  decalAlpha = 0.0f;
    float3 decalColor = float3(0.0f, 0.0f, 0.0f);
#endif

    float2 offset;
    offset.x = g_smpBase.b;
    offset.y = g_smpBase.a * u_TexBaseAlpha;

#if NND_USER_PROFILE == 1
    offset = offset * 0.01f - 0.005f;
    offset.y *= 15.0f;
    float2 screen = Input.UserCoord;
#else
    // the bias is 0.01 * 128/255, so an untouched 8 bit offset map reads as no displacement
    offset = offset * 0.01f - 0.00501960795f;
    float3 clipXYW;
    clipXYW  = u_UserUniform[0].xyw * Input.UserCoord.x;
    clipXYW += u_UserUniform[1].xyw * Input.UserCoord.y;
    clipXYW += u_UserUniform[2].xyw * Input.UserCoord.z;
    clipXYW += u_UserUniform[3].xyw * Input.UserCoord.w;
    float2 screen = (clipXYW.xy / clipXYW.z) * float2(0.5f, -0.5f) + 0.5f;
#endif

    float3 scene = tex2D(s_texUserSampler2D1, screen + offset).rgb;

#if NND_USER_PROFILE == 1
    float3 color = saturate(decalColor * decalAlpha + scene);
#else
    float3 color = lerp(scene, decalColor, decalAlpha);
#endif

    PS_OUTPUT output;
    output.Col.rgb = lerp(u_Fog.color.rgb, saturate(color * g_Input.FrontCol[0].rgb),
                          g_Input.ViewPosFog.NND_SUFFIX_FOG);
    output.Col.a = ceil(decalAlpha) * g_Input.FrontCol[0].a;
    return output;
}
#endif

technique NNStdShader
{
    pass P0
    {
#if NND_USER_PROFILE_REFRACTS
        VertexShader = compile vs_3_0 nnStdShaderVSUserProfile();
        PixelShader  = compile ps_3_0 nnStdShaderPSUserProfile();
#else
        VertexShader = compile vs_3_0 nnStdShaderVS();
        PixelShader  = compile ps_3_0 nnStdShaderPSAlphaTest();
#endif
    }
}
