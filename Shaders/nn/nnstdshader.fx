#define main nnStdShaderVS
#include "nnstdshader/nnstdshader.vsh"
#undef main

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

technique NNStdShader
{
    pass P0
    {
        VertexShader = compile vs_3_0 nnStdShaderVS();
        PixelShader  = compile ps_3_0 nnStdShaderPSAlphaTest();
    }
}
