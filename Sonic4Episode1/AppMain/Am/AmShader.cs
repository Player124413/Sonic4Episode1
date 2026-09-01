using System;
using Microsoft.Xna.Framework.Graphics;

internal partial class AppMain
{
    // The C++ takes separate VS/PS bytecode blobs (amShader.cpp:491) because D3D9 creates them
    // separately. XNA bundles both in one Effect, so a shader is identified by asset name.
    private static int amShaderBuild(string name, AMS_SHADER shader)
    {
        if (shader == null)
            return -1;

        shader.name = name;
        return amDrawRegistCommand(AMD_REGIST_CREATE_SHADER, new AMS_PARAM_CREATE_SHADER
        {
            name = name,
            shader = shader
        });
    }

    private static int amShaderRelease(AMS_SHADER shader)
    {
        if (shader == null || shader.effect == null)
            return -1;

        return amDrawRegistCommand(AMD_REGIST_RELEASE_SHADER, new AMS_PARAM_RELEASE_SHADER
        {
            shader = shader
        });
    }

    private static bool amShaderIsValid(AMS_SHADER shader)
    {
        return shader != null && shader.effect != null;
    }

    private static Effect amShaderLoadEffect(string name)
    {
#if WINDOWSPHONE7_5
        return null;
#else
        try
        {
#if MONOGAME
            // MonoGame's ContentManager only resolves .xnb, and Shaders.targets ships raw .mgfxo,
            // so this reads it the same way mppLoadStdShader does
            using var stream = TitleContainer.OpenStream(
                "Content\\Shaders\\" + name + MPP_SHADER_EXTENSION);
            var code = new byte[stream.Length];
            stream.ReadExactly(code, 0, code.Length);
            return new Effect(Sonic4Ep1.pInstance.GraphicsDevice, code);
#else
            return Sonic4Ep1.pInstance.Content.Load<Effect>("Shaders\\" + name);
#endif
        }
        catch (Exception ex)
        {
            amSystemLog($"[ERR] shader '{name}' load failed: {ex.Message}\n");
            return null;
        }
#endif
    }
}
