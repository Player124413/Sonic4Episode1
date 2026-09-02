using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// binds a compiled nnstdshader permutation the way NN's runtime does
internal sealed class NnStdShaderEffect : Effect
{
    // nnSetShaderBoneMatrix asserts the palette at 16, and a material carries at most
    // mppMaxSimultaneousTextures maps
    private const int MaxBoneMatrices = 16;
    private const int MaxTextureMatrices = 8;

    private const int MaterialRegisters = 5;
    private const int FogRegisters = 5;
    private const int LightModelRegisters = 1;
    private const int LightRegistersSet = 4;

    public NnStdShaderEffect(GraphicsDevice device, byte[] effectCode)
        : base(device, effectCode)
    {
        modelView = new Uniform(this, "u_ModelViewMatrix");
        modelViewProjection = new Uniform(this, "u_ModelViewProjectionMatrix");
        projection = new Uniform(this, "u_ProjectionMatrix");
        normal = new Uniform(this, "u_NormalMatrix");
        boneMatrix = new Uniform(this, "u_ModelViewBoneMatrix");
        boneNormalMatrix = new Uniform(this, "u_ModelViewBoneNormalMatrix");
        textureMatrix = new Uniform(this, "u_TextureMatrix");
        userUniform = new Uniform(this, "u_UserUniform");
        texBaseAlpha = new Uniform(this, "u_TexBaseAlpha");
        texDecalAlpha = new Uniform(this, "u_TexDecalAlpha");
        texDecal2Alpha = new Uniform(this, "u_TexDecal2Alpha");
        texDecal3Alpha = new Uniform(this, "u_TexDecal3Alpha");
        alphaTest = Parameters["u_AlphaTest"];

        frontMaterial = new StructUniform(this, "u_FrontMaterial", MaterialRegisters);
        frontLightModelProduct = new StructUniform(this, "u_FrontLightModelProduct", LightModelRegisters);
        fog = new StructUniform(this, "u_Fog", FogRegisters);
        lightSource = new LightUniform(this, "u_LightSource");
    }

    private readonly Uniform modelView;
    private readonly Uniform modelViewProjection;
    private readonly Uniform projection;
    private readonly Uniform normal;
    private readonly Uniform boneMatrix;
    private readonly Uniform boneNormalMatrix;
    private readonly Uniform textureMatrix;
    private readonly Uniform userUniform;
    private readonly Uniform texBaseAlpha;
    private readonly Uniform texDecalAlpha;
    private readonly Uniform texDecal2Alpha;
    private readonly Uniform texDecal3Alpha;
    private readonly EffectParameter alphaTest;

    private readonly StructUniform frontMaterial;
    private readonly StructUniform frontLightModelProduct;
    private readonly StructUniform fog;
    private readonly LightUniform lightSource;

    private readonly Matrix[] boneNormals = new Matrix[MaxBoneMatrices];
    private readonly Matrix[] textureMatrices = new Matrix[MaxTextureMatrices];
    private Matrix[][] matrixBuffers = new Matrix[MaxBoneMatrices + 1][];

    // rigid permutations read u_ModelViewMatrix; envelope ones read u_ProjectionMatrix alone and
    // carry the model view in the bone matrices instead
    public void SetTransforms(Matrix modelViewMatrix, Matrix projectionMatrix)
    {
        modelView.Set(modelViewMatrix);
        modelViewProjection.Set(modelViewMatrix * projectionMatrix);
        projection.Set(projectionMatrix);
        normal.Set(NormalMatrixOf(modelViewMatrix));
    }

    public void SetBoneMatrices(Matrix[] bones)
    {
        SetBoneMatrices(bones, bones.Length);
    }

    // u_ModelViewBoneMatrix is float4x4[4] for VtxTfmMode 1, float4x4[16] for mode 2; the normal
    // matrix array is absent when the permutation has no normal to transform
    public void SetBoneMatrices(Matrix[] bones, int count)
    {
        if (count > MaxBoneMatrices)
            count = MaxBoneMatrices;

        SetMatrixArray(boneMatrix.Vertex, bones, count);
        SetMatrixArray(boneMatrix.Pixel, bones, count);

        if (boneNormalMatrix.Vertex == null && boneNormalMatrix.Pixel == null)
            return;

        for (int i = 0; i < count; ++i)
            boneNormals[i] = NormalMatrixOf(bones[i]);

        SetMatrixArray(boneNormalMatrix.Vertex, boneNormals, count);
        SetMatrixArray(boneNormalMatrix.Pixel, boneNormals, count);
    }

    // NN takes normals through the inverse transpose so non-uniform scale does not shear them
    private static Matrix NormalMatrixOf(Matrix transform)
    {
        Matrix normalMatrix = Matrix.Transpose(Matrix.Invert(transform));
        normalMatrix.M14 = normalMatrix.M24 = normalMatrix.M34 = 0f;
        normalMatrix.M41 = normalMatrix.M42 = normalMatrix.M43 = 0f;
        normalMatrix.M44 = 1f;
        return normalMatrix;
    }

    // SetValue(Matrix[]) writes value.Length entries without checking the parameter's capacity, so
    // the array handed over has to be exactly what fits
    private void SetMatrixArray(EffectParameter parameter, Matrix[] values, int count)
    {
        if (parameter == null || count <= 0)
            return;

        bool isArray = parameter.Elements != null && parameter.Elements.Count > 0;

#if MONOGAME
        // MonoGame's SetValue(Matrix[]) walks Elements, and a one entry array does not have any,
        // it comes back as a plain matrix
        if (!isArray)
        {
            parameter.SetValue(values[0]);
            return;
        }
#endif

        int capacity = isArray ? parameter.Elements.Count : 1;
        if (count > capacity)
            count = capacity;

        parameter.SetValue(count == values.Length ? values : ExactLength(values, count));
    }

    private Matrix[] ExactLength(Matrix[] values, int count)
    {
        if (count >= matrixBuffers.Length)
            Array.Resize(ref matrixBuffers, count + 1);

        Matrix[] buffer = matrixBuffers[count];
        if (buffer == null)
        {
            buffer = new Matrix[count];
            matrixBuffers[count] = buffer;
        }

        Array.Copy(values, buffer, count);
        return buffer;
    }

    // GL convention, which nnstdshader keeps: a parallel light's position holds the direction
    // toward the light, in view space, and CalcLight reads position.xyz as that direction.
    public void SetParallelLight(int index, Vector3 directionToLightView, Vector3 diffuse, Vector3 specular)
    {
        Vector3 direction = Vector3.Normalize(directionToLightView);
        lightSource.Set(
            index,
            Vector4.Zero,                       // ambient, unused while NND_PROF_LIGHT_AMBIENT is 0
            new Vector4(diffuse, 1f),
            new Vector4(specular, 1f),
            new Vector4(direction, 0f));
    }

    public void SetMaterial(AppMain.NNS_MATERIAL_STDSHADER_COLOR color, Vector3 sceneAmbient)
    {
        if (color == null)
            return;

        frontMaterial.Set(
            ToVector4(color.Ambient),
            ToVector4(color.Diffuse),
            ToVector4(color.Specular),
            ToVector4(color.Emission),
            new Vector4(color.Shininess, 0f, 0f, 0f));

        // GL's derived scene colour, added to the diffuse term by nnstdshader as the unlit floor
        frontLightModelProduct.Set(new Vector4(
            color.Emission.r + color.Ambient.r * sceneAmbient.X,
            color.Emission.g + color.Ambient.g * sceneAmbient.Y,
            color.Emission.b + color.Ambient.b * sceneAmbient.Z,
            1f));
    }

    // leaving this unset does not disable fog, it lerps the whole surface to an uninitialised colour
    public void SetFog(Vector3 color, float start, float end)
    {
        fog.Set(
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
        return alphaTest == null ? "no parameter" : alphaTest.GetValueVector4().ToString();
    }

    public bool SetAlphaTestAlways()
    {
        return SetAlphaTest(0f, 0f, -1f);
    }

    // alpha arrives as 8 bit, so anything well inside one step separates > from >=
    private const float AlphaTestBias = 1f / 1024f;

    private bool SetAlphaTest(float reference, float sense, float bias)
    {
        if (alphaTest == null)
            return false;

        alphaTest.SetValue(new Vector4(reference, sense, bias, 0f));
        return true;
    }

    public void SetTextureMatrices(AppMain.NNS_MATERIAL_STDSHADER_DESC material)
    {
        if (material.pTexDesc == null)
            return;

        int count = Math.Min(material.pTexDesc.Length, MaxTextureMatrices);
        for (int i = 0; i < count; ++i)
            textureMatrices[i] = TextureMatrixOf(material.pTexDesc[i]);

        SetMatrixArray(textureMatrix.Vertex, textureMatrices, count);
        SetMatrixArray(textureMatrix.Pixel, textureMatrices, count);
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
        userUniform.Set(uniforms);
    }

    internal static void ProjectionUserUniforms(Matrix projection, Vector4[] rows)
    {
        rows[0] = new Vector4(projection.M11, projection.M12, projection.M13, projection.M14);
        rows[1] = new Vector4(projection.M21, projection.M22, projection.M23, projection.M24);
        rows[2] = new Vector4(projection.M31, projection.M32, projection.M33, projection.M34);
        rows[3] = new Vector4(projection.M41, projection.M42, projection.M43, projection.M44);
    }

    internal static Vector4[] ProjectionUserUniforms(Matrix projection)
    {
        var rows = new Vector4[4];
        ProjectionUserUniforms(projection, rows);
        return rows;
    }

    public void SetMapBlend(AppMain.NNS_MATERIAL_STDSHADER_DESC material)
    {
        texBaseAlpha.Set(BlendOf(material, AppMain.NND_TEXTURETYPE_STDSHADER_BASE_MAP));
        texDecalAlpha.Set(BlendOf(material, AppMain.NND_TEXTURETYPE_STDSHADER_DECAL_MAP));
        texDecal2Alpha.Set(BlendOf(material, AppMain.NND_TEXTURETYPE_STDSHADER_DECAL2_MAP));
        texDecal3Alpha.Set(BlendOf(material, AppMain.NND_TEXTURETYPE_STDSHADER_DECAL3_MAP));
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

    private static Vector4 ToVector4(NNS_RGBA rgba)
    {
        return new Vector4(rgba.r, rgba.g, rgba.b, rgba.a);
    }

    // vertex and pixel halves each declare their own copy of a shared uniform (u_X / ps_u_X), and
    // which half reads it depends on the permutation, so both get resolved once and set together
    private readonly struct Uniform
    {
        public readonly EffectParameter Vertex;
        public readonly EffectParameter Pixel;

        public Uniform(Effect effect, string name)
        {
            Vertex = effect.Parameters[name];
            Pixel = effect.Parameters["ps_" + name];
        }

        public void Set(float value)
        {
            if (Vertex != null) Vertex.SetValue(value);
            if (Pixel != null) Pixel.SetValue(value);
        }

        public void Set(Matrix value)
        {
            if (Vertex != null) Vertex.SetValue(value);
            if (Pixel != null) Pixel.SetValue(value);
        }

        public void Set(Vector4[] values)
        {
            if (Vertex != null) Vertex.SetValue(values);
            if (Pixel != null) Pixel.SetValue(values);
        }
    }

    // D3D9 puts each struct member on its own float4 register where the runtime packs them tightly
    // by size, so one Vector4 per register goes in rather than a value per member name
    private sealed class StructUniform
    {
        private readonly Vector4[] registers;

#if MONOGAME
        // MonoGame cannot compile a struct uniform at all, so its permutations rebuild the NN
        // structs from loose float4 registers named u_XReg0, u_XReg1 and so on, see
        // docs/monogame-mojoshader-ctab.md. One bank serves both halves, and a register the
        // permutation never reads is absent from the constant table.
        private readonly EffectParameter[] parameters;
#else
        private readonly Uniform uniform;
#endif

        public StructUniform(Effect effect, string name, int registerCount)
        {
            registers = new Vector4[registerCount];
#if MONOGAME
            parameters = new EffectParameter[registerCount];
            for (int i = 0; i < registerCount; ++i)
                parameters[i] = effect.Parameters[name + "Reg" + i];
#else
            uniform = new Uniform(effect, name);
#endif
        }

        public void Set(Vector4 register0)
        {
            registers[0] = register0;
            Apply();
        }

        public void Set(Vector4 register0, Vector4 register1, Vector4 register2, Vector4 register3, Vector4 register4)
        {
            registers[0] = register0;
            registers[1] = register1;
            registers[2] = register2;
            registers[3] = register3;
            registers[4] = register4;
            Apply();
        }

        private void Apply()
        {
#if MONOGAME
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (parameters[i] != null)
                    parameters[i].SetValue(registers[i]);
            }
#else
            uniform.Set(registers);
#endif
        }
    }

    private sealed class LightUniform
    {
        private readonly Vector4[] registers = new Vector4[LightRegistersSet];

#if MONOGAME
        // the flat registers run across the whole array, so a light starts every
        // NnStdShaderLightRegisters of them
        private const int NnStdShaderLightRegisters = 12;

        private readonly EffectParameter[] parameters;
#else
        private readonly Uniform uniform;
#endif

        public LightUniform(Effect effect, string name)
        {
#if MONOGAME
            parameters = new EffectParameter[NnStdShaderLightRegisters];
            for (int i = 0; i < parameters.Length; ++i)
                parameters[i] = effect.Parameters[name + "Reg" + i];
#else
            uniform = new Uniform(effect, name);
#endif
        }

        public void Set(int index, Vector4 ambient, Vector4 diffuse, Vector4 specular, Vector4 position)
        {
            registers[0] = ambient;
            registers[1] = diffuse;
            registers[2] = specular;
            registers[3] = position;

#if MONOGAME
            int first = index * NnStdShaderLightRegisters;
            if (first + registers.Length > parameters.Length)
                return;

            for (int i = 0; i < registers.Length; ++i)
            {
                if (parameters[first + i] != null)
                    parameters[first + i].SetValue(registers[i]);
            }
#else
            Element(uniform.Vertex, index)?.SetValue(registers);
            Element(uniform.Pixel, index)?.SetValue(registers);
#endif
        }

#if !MONOGAME
        // a permutation reading only element 0 can come back from MojoShader as a plain struct, no Elements
        private static EffectParameter Element(EffectParameter lights, int index)
        {
            if (lights == null)
                return null;

            if (lights.Elements != null && lights.Elements.Count > index)
                return lights.Elements[index];

            return index == 0 ? lights : null;
        }
#endif
    }
}
