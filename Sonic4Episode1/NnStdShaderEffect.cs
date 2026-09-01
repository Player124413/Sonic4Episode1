using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


// binds a compiled nnstdshader permutation the way NN's runtime does
internal sealed class NnStdShaderEffect : Effect
{
    public NnStdShaderEffect(GraphicsDevice device, byte[] effectCode)
        : base(device, effectCode)
    {
    }

    // rigid permutations read u_ModelViewMatrix; envelope ones read u_ProjectionMatrix alone and
    // carry the model view in the bone matrices instead
    public void SetTransforms(Matrix modelView, Matrix projection)
    {
        SetMatrix("u_ModelViewMatrix", modelView);
        SetMatrix("u_ModelViewProjectionMatrix", modelView * projection);
        SetMatrix("u_ProjectionMatrix", projection);
        SetMatrix("u_NormalMatrix", NormalMatrixOf(modelView));
    }

    // u_ModelViewBoneMatrix is float4x4[4] for VtxTfmMode 1, float4x4[16] for mode 2; the normal
    // matrix array is absent when the permutation has no normal to transform
    public void SetBoneMatrices(Matrix[] bones)
    {
        SetShared("u_ModelViewBoneMatrix", parameter => SetMatrixArray(parameter, bones));

        EffectParameter normals = Parameters["u_ModelViewBoneNormalMatrix"];
        if (normals == null && Parameters["ps_u_ModelViewBoneNormalMatrix"] == null)
            return;

        if (_boneNormals.Length != bones.Length)
            _boneNormals = new Matrix[bones.Length];
        for (int i = 0; i < bones.Length; ++i)
            _boneNormals[i] = NormalMatrixOf(bones[i]);

        SetShared("u_ModelViewBoneNormalMatrix", parameter => SetMatrixArray(parameter, _boneNormals));
    }

    private Matrix[] _boneNormals = Array.Empty<Matrix>();

    // NN takes normals through the inverse transpose so non-uniform scale does not shear them
    private static Matrix NormalMatrixOf(Matrix transform)
    {
        Matrix normalMatrix = Matrix.Transpose(Matrix.Invert(transform));
        normalMatrix.M14 = normalMatrix.M24 = normalMatrix.M34 = 0f;
        normalMatrix.M41 = normalMatrix.M42 = normalMatrix.M43 = 0f;
        normalMatrix.M44 = 1f;
        return normalMatrix;
    }

    // FNA's SetValue(Matrix[]) doesn't check length against the parameter, so clamp here
    private static void SetMatrixArray(EffectParameter parameter, Matrix[] values)
    {
        int capacity = parameter.Elements == null || parameter.Elements.Count == 0
            ? 1
            : parameter.Elements.Count;

        if (values.Length <= capacity)
        {
            parameter.SetValue(values);
            return;
        }

        var clamped = new Matrix[capacity];
        Array.Copy(values, clamped, capacity);
        parameter.SetValue(clamped);
    }

    // GL convention, which nnstdshader keeps: a parallel light's position holds the direction
    // toward the light, in view space, and CalcLight reads position.xyz as that direction.
    public void SetParallelLight(int index, Vector3 directionToLightView, Vector3 diffuse, Vector3 specular)
    {
        Vector3 direction = Vector3.Normalize(directionToLightView);
        Vector4[] registers =
        {
            Vector4.Zero,                       // ambient, unused while NND_PROF_LIGHT_AMBIENT is 0
            new Vector4(diffuse, 1f),
            new Vector4(specular, 1f),
            new Vector4(direction, 0f),
        };

        SetShared("u_LightSource", lights =>
        {
            EffectParameter light = LightElement(lights, index);
            if (light != null)
                light.SetValue(registers);
        });
    }

    // a permutation reading only element 0 can come back from MojoShader as a plain struct, no Elements
    private static EffectParameter LightElement(EffectParameter lights, int index)
    {
        if (lights.Elements != null && lights.Elements.Count > index)
            return lights.Elements[index];

        return index == 0 ? lights : null;
    }

    public void SetMaterial(AppMain.NNS_MATERIAL_STDSHADER_COLOR color, Vector3 sceneAmbient)
    {
        if (color == null)
            return;

        SetStruct("u_FrontMaterial",
            ToVector4(color.Ambient),
            ToVector4(color.Diffuse),
            ToVector4(color.Specular),
            ToVector4(color.Emission),
            new Vector4(color.Shininess, 0f, 0f, 0f));

        // GL's derived scene colour, added to the diffuse term by nnstdshader as the unlit floor
        Vector4 sceneColor = new Vector4(
            color.Emission.r + color.Ambient.r * sceneAmbient.X,
            color.Emission.g + color.Ambient.g * sceneAmbient.Y,
            color.Emission.b + color.Ambient.b * sceneAmbient.Z,
            1f);

        SetStruct("u_FrontLightModelProduct", sceneColor);
    }

    // leaving this unset does not disable fog, it lerps the whole surface to an uninitialised colour
    public void SetFog(Vector3 color, float start, float end)
    {
        SetStruct("u_Fog",
            new Vector4(color, 1f),
            Vector4.Zero,                                           // density, EXP models only
            new Vector4(start, 0f, 0f, 0f),
            new Vector4(end, 0f, 0f, 0f),
            new Vector4(1f / Math.Max(end - start, 1e-6f), 0f, 0f, 0f));
    }

    // D3DCMPFUNC; the shader's clip takes a reference/sense/bias, which covers every comparison
    // except EQUAL and NOTEQUAL, unused by retail data
    public bool SetAlphaTest(int compare, float reference)
    {
        switch (compare)
        {
            case 1: return SetAlphaTest(0f, 0f, 1f);                    // never
            case 2: return SetAlphaTest(reference, -1f, AlphaTestBias); // less
            case 4: return SetAlphaTest(reference, -1f, 0f);            // less or equal
            case 5: return SetAlphaTest(reference, 1f, AlphaTestBias);  // greater
            case 7: return SetAlphaTest(reference, 1f, 0f);             // greater or equal
            case 8: return SetAlphaTestAlways();
            default: return false;
        }
    }

    // what the clip in the pixel shader is currently being told to do
    public string AlphaTestValue()
    {
        var parameter = Parameters["u_AlphaTest"];
        return parameter == null ? "no parameter" : parameter.GetValueVector4().ToString();
    }

    public bool SetAlphaTestAlways()
    {
        return SetAlphaTest(0f, 0f, -1f);
    }

    // alpha arrives as 8 bit, so anything well inside one step separates > from >=
    private const float AlphaTestBias = 1f / 1024f;

    private bool SetAlphaTest(float reference, float sense, float bias)
    {
        EffectParameter parameter = Parameters["u_AlphaTest"];
        if (parameter == null)
            return false;

        parameter.SetValue(new Vector4(reference, sense, bias, 0f));
        return true;
    }

    public void SetTextureMatrices(AppMain.NNS_MATERIAL_STDSHADER_DESC material)
    {
        if (material.pTexDesc == null)
            return;

        Matrix[] matrices = new Matrix[material.pTexDesc.Length];
        for (int i = 0; i < matrices.Length; ++i)
            matrices[i] = TextureMatrixOf(material.pTexDesc[i]);

        SetShared("u_TextureMatrix", parameter => parameter.SetValue(matrices));
    }

    // shader does mul(float3(u, v, 1), matrix), so the offset sits in row 3, not XNA's usual row 4
    private static Matrix TextureMatrixOf(AppMain.NNS_MATERIAL_STDSHADER_TEXMAP_DESC desc)
    {
        if ((desc.fType & AppMain.NND_TEXTURETYPE_STDSHADER_NO_UV_TRANSFORM) != 0)
            return Matrix.Identity;

        Matrix matrix = Matrix.Identity;
        matrix.M11 = desc.Scale.u;
        matrix.M22 = desc.Scale.v;
        matrix.M31 = desc.Offset.u;
        matrix.M32 = desc.Offset.v;
        return matrix;
    }

    // u_UserUniform[4]. gmMap.cpp:3249-3259 pushes all four, and NNM_MTX(m, row, col) is
    // m[col][row] (nns.h:788), so uniform k is m[k][0..3]: four consecutive floats, which is row k
    // of the storage either way round. NN is column-vector and XNA row-vector, and that transpose
    // is exactly what makes the same four floats mean the same thing in both.
    // the refraction body lives in the pixel half, so ps_u_UserUniform is the copy that matters
    public void SetUserUniforms(Vector4[] uniforms)
    {
        SetShared("u_UserUniform", parameter => parameter.SetValue(uniforms));
    }

    internal static Vector4[] ProjectionUserUniforms(Matrix projection)
    {
        return new[]
        {
            new Vector4(projection.M11, projection.M12, projection.M13, projection.M14),
            new Vector4(projection.M21, projection.M22, projection.M23, projection.M24),
            new Vector4(projection.M31, projection.M32, projection.M33, projection.M34),
            new Vector4(projection.M41, projection.M42, projection.M43, projection.M44),
        };
    }

    public void SetMapBlend(AppMain.NNS_MATERIAL_STDSHADER_DESC material)
    {
        SetFloat("u_TexBaseAlpha", BlendOf(material, AppMain.NND_TEXTURETYPE_STDSHADER_BASE_MAP));
        SetFloat("u_TexDecalAlpha", BlendOf(material, AppMain.NND_TEXTURETYPE_STDSHADER_DECAL_MAP));
        SetFloat("u_TexDecal2Alpha", BlendOf(material, AppMain.NND_TEXTURETYPE_STDSHADER_DECAL2_MAP));
        SetFloat("u_TexDecal3Alpha", BlendOf(material, AppMain.NND_TEXTURETYPE_STDSHADER_DECAL3_MAP));
    }

    private static float BlendOf(AppMain.NNS_MATERIAL_STDSHADER_DESC material, uint mapBit)
    {
        AppMain.NNS_MATERIAL_STDSHADER_TEXMAP_DESC desc = AppMain.mppStdShaderTexMap(material, mapBit);
        return desc == null ? 1f : desc.Blend;
    }

    // D3DTEXTUREADDRESS, which is what NNS_MATERIAL_STDSHADER_TEXMAP_DESC.WrapS/WrapT carry
    public static TextureAddressMode AddressModeOf(int wrap)
    {
        switch (wrap)
        {
            case 1: return TextureAddressMode.Wrap;
            case 2: return TextureAddressMode.Mirror;
            case 3: return TextureAddressMode.Clamp;
            case 4: return TextureAddressMode.Clamp; // no border mode in XNA; clamp is the closest
            default: return TextureAddressMode.Wrap;
        }
    }

    // vertex and pixel halves each declare their own copy of a shared uniform (u_X / ps_u_X); set
    // both, since which half actually reads it depends on the permutation
    private void SetShared(string name, Action<EffectParameter> set)
    {
        EffectParameter vertexSide = Parameters[name];
        if (vertexSide != null)
            set(vertexSide);

        EffectParameter pixelSide = Parameters["ps_" + name];
        if (pixelSide != null)
            set(pixelSide);
    }

    // FNA packs struct members tightly by size where D3D9 puts each on its own float4 register, so
    // set one Vector4 per register directly rather than by member name
    private void SetStruct(string name, params Vector4[] registers)
    {
        SetShared(name, parameter => parameter.SetValue(registers));
    }

    private void SetMatrix(string name, Matrix value)
    {
        SetShared(name, parameter => parameter.SetValue(value));
    }

    private void SetFloat(string name, float value)
    {
        SetShared(name, parameter => parameter.SetValue(value));
    }

    private static Vector4 ToVector4(NNS_RGBA rgba)
    {
        return new Vector4(rgba.r, rgba.g, rgba.b, rgba.a);
    }
}
