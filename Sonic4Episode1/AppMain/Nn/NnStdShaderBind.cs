using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using mpp;

internal partial class AppMain
{
    // the name packs the whole profile, so it keys the cache directly and the 26 digit string only
    // gets built on a miss, to name the file
    private static readonly Dictionary<NNS_STDSHADER_NAME, NnStdShaderEffect> nngStdShaderCache =
        new Dictionary<NNS_STDSHADER_NAME, NnStdShaderEffect>();

    // nnBindStdShader runs once per meshset, so everything it needs is kept rather than made
    private static readonly NNS_STDSHADER_PROFILE nngStdShaderProfile = new NNS_STDSHADER_PROFILE();

    private const int mppMaxBoneMatrices = 16;

    private static NNS_MATRIX[] nngWorldMatrixPalette;
    private static Matrix nngStdShaderView = Matrix.Identity;
    private static Matrix nngStdShaderModelView = Matrix.Identity;
    private static readonly Matrix[] nngStdShaderBones = new Matrix[mppMaxBoneMatrices];
    private static int nngStdShaderBoneCount;

    public static void nnSetWorldMatrixPalette(NNS_MATRIX[] mtxpal)
    {
        nngWorldMatrixPalette = mtxpal;
    }

    // nnDrawObject always passes null; the palette already includes view space by this point
    public static void nnSetViewMatrix(NNS_MATRIX view)
    {
        nngStdShaderView = view == null ? Matrix.Identity : (Matrix)view;
    }

    // keys on iMatrix, not on rigid versus pliable
    public static void nnSetShaderMatrix(NNS_DRAWCALLBACK_VAL val, NNS_MESHSET meshset)
    {
        if (meshset.iMatrix < 0 || nngWorldMatrixPalette == null)
            nngStdShaderModelView = nngStdShaderView;
        else
            nngStdShaderModelView = (Matrix)nngWorldMatrixPalette[meshset.iMatrix] * nngStdShaderView;
    }

    public static void nnSetShaderBoneMatrix(NNS_DRAWCALLBACK_VAL val, NNS_MESHSET meshset)
    {
        nngStdShaderBoneCount = 0;
        if (!(val.pVtxListPtr.pVtxList is NNS_VTXLIST_DX_DESC desc)
            || desc.pBoneMtxList == null
            || desc.nBoneMtx <= 0
            || nngWorldMatrixPalette == null)
            return;

        NNM_ASSERT(desc.nBoneMtx <= mppMaxBoneMatrices, "Matrix palette Overflow\n");

        // shader always reads four matrices in this mode, pad the tail with identity
        int count = desc.nBoneMtx;
        if ((desc.Format & NND_VTXMEMBER_DX_BLEND_MASK) == NND_VTXMEMBER_DX_BLEND4FIXED
            && (desc.Format & NND_VTXMEMBER_DX_BLENDFMT_I8) == 0)
            count = Math.Max(count, 4);

        if (count > mppMaxBoneMatrices)
            count = mppMaxBoneMatrices;

        for (int i = 0; i < count; ++i)
        {
            nngStdShaderBones[i] = i < desc.nBoneMtx
                ? (Matrix)nngWorldMatrixPalette[(int)desc.pBoneMtxList[i]]
                : Matrix.Identity;
        }

        nngStdShaderBoneCount = count;
    }

    public static void nnSetVertexFormat(NNS_VTXLISTPTR vlistptr)
    {
        if (!(vlistptr.pVtxList is NNS_VTXLIST_DX_DESC desc))
            return;

        GraphicsDevice device = OpenGL.mppglGetDevice();
        if (desc.pD3DVertexBuffer == null)
            mppBuildVertexBufferDXG20(device, desc);

        device.SetVertexBuffer(desc.pD3DVertexBuffer);
    }

    private static void mppBuildVertexBufferDXG20(GraphicsDevice device, NNS_VTXLIST_DX_DESC desc)
    {
        DXVertexArrays arrays = DXVertexArrays.Decode(desc, desc.mppBigEndian);
        var vertices = new DXVertex[desc.nVertex];
        for (int i = 0; i < vertices.Length; ++i)
        {
            Vector4 weights = arrays.BlendWeights != null ? arrays.BlendWeights[i] : Vector4.Zero;
            byte[] bones = arrays.BlendIndices != null ? arrays.BlendIndices[i] : null;

            vertices[i] = new DXVertex
            {
                Position = arrays.Positions[i],
                BlendWeights = new Vector3(weights.X, weights.Y, weights.Z),
                BlendIndices = bones == null
                    ? new Byte4(0f, 0f, 0f, 0f)
                    : new Byte4(bones[0], bones[1], bones[2], bones[3]),
                Normal = arrays.Normals != null ? arrays.Normals[i] : Vector3.Up,
                TexCoord = arrays.TexCoords != null ? arrays.TexCoords[i] : Vector2.Zero,
                TexCoord2 = arrays.TexCoords2 != null ? arrays.TexCoords2[i] : Vector2.Zero,
                TexCoord3 = arrays.TexCoords3 != null ? arrays.TexCoords3[i] : Vector2.Zero,
                Color = arrays.Colors != null ? arrays.Colors[i] : Color.White,
            };
        }

        desc.pD3DVertexDecl = DXVertex.Declaration;
        desc.pD3DVertexBuffer = new VertexBuffer(device, DXVertex.Declaration, vertices.Length, BufferUsage.WriteOnly);
        desc.pD3DVertexBuffer.SetData(vertices);
    }

    public static void nnBindStdShader(NNS_DRAWCALLBACK_VAL val)
    {
        NNS_SUBOBJ subobj = val.pObject.pSubobjList[val.iSubobject];
        if (!(val.pVtxListPtr.pVtxList is NNS_VTXLIST_DX_DESC desc))
            return;
        if (!(val.pMaterial.pMaterial is NNS_MATERIAL_STDSHADER_DESC material))
            return;

        // the draw flag never carries a fragment light count (GLES11 lights fixed-function), but
        // zero asks for a permutation retail never built, so force the one every lit key carries
        uint drawFlag = val.DrawFlag | NND_DRAWOBJ_FRAGPARALIGHT1;

        NNS_STDSHADER_PROFILE profile = nngStdShaderProfile;
        if (nnSetupShaderProfile(profile, val.pMaterial, drawFlag, val.DrawFlagEx, subobj.fType, desc.Format) < 0)
            return;

        NnStdShaderEffect effect = mppLoadStdShader(nnGetStdShaderName(profile));
        if (effect == null)
            return;

        Matrix projection = OpenGL.mppglGetProjection();
        effect.SetTransforms(nngStdShaderModelView, projection);
        if (nngStdShaderBoneCount > 0)
            effect.SetBoneMatrices(nngStdShaderBones, nngStdShaderBoneCount);

        // nnstdshader wants the direction toward the light, in view space; GL stores the direction
        // the light travels, and _am_draw_world_view_matrix already folds in the view
        Vector3 lightDirection = Vector3.Up;
        if (OpenGL.mppglGetLight(0, out Vector3 diffuse, out Vector3 specular, out Vector3 travel))
            lightDirection = Vector3.TransformNormal(-travel, (Matrix)_am_draw_world_view_matrix);
        else
            diffuse = specular = Vector3.Zero;

        effect.SetParallelLight(0, lightDirection, diffuse, specular);
        effect.SetMaterial(material.pColor, OpenGL.mppglGetAmbient());
        effect.SetTextureMatrices(material);
        effect.SetMapBlend(material);

        mppSetStdShaderAlphaTest(effect);

        // The switch and the range live in NN's own globals and only reach the wrapper through
        // nnPutFogSwitchGL, which the fixed function material path calls (NnDraw.cs:713) and this
        // one never does. Only the colour gets there, via glFogfv. Same test as that call site.
        //
        // The permutation always computes fog, so "off" means moving it past the far plane.
        OpenGL.mppglGetFog(out _, out Vector3 fogColor, out _, out _);
        if (nngFogSwitch && (material.fFlag & NND_MATFLAG_DISABLE_FOGGING) == 0)
            effect.SetFog(fogColor, nngFogStart, nngFogEnd);
        else
            effect.SetFog(Vector3.Zero, 1e9f, 2e9f);

        mppBindStdShaderTextures(material);
        mppBindStdShaderUserSamplers(effect, material, profile);
        effect.CurrentTechnique.Passes[0].Apply();
    }

    // the user samplers sit behind the material's own maps and the shadow maps, and the uniforms
    // are NN's global bank, so both come from state the draw call never carries
    private static void mppBindStdShaderUserSamplers(
        NnStdShaderEffect effect, NNS_MATERIAL_STDSHADER_DESC material, NNS_STDSHADER_PROFILE profile)
    {
        for (var idx = NNE_USER_SAMPLER.NNE_USER_SAMPLER_2D_1;
             idx < NNE_USER_SAMPLER.NNE_USER_SAMPLER_MAX;
             ++idx)
        {
            if (profile.GetUserSampler(idx) == 0)
                continue;

            mppPutMaterialTextureUserSampler(
                mppStdShaderUserSamplerSlot(material.fTexType, profile.nShadowMap, idx), idx);
        }

        effect.SetUserUniforms(mppStdShaderUserUniforms());
    }

    // XNA has no alpha test render state, so it goes to the shader instead. Every permutation
    // compiles the clip, and u_AlphaTest defaults to never clipping, so a caller that skips this
    // gets no cutouts at all rather than an error.
    // for diagnostics: what the last material asked the clip to do
    public static string mppStdShaderAlphaTestState()
    {
        return nngShaderRenderState[D3DRS_ALPHATESTENABLE] == 0
            ? "off"
            : $"func {nngShaderRenderState[D3DRS_ALPHAFUNC]} ref {nngShaderRenderState[D3DRS_ALPHAREF] & 0xFF}";
    }

    public static void mppSetStdShaderAlphaTest(NnStdShaderEffect effect)
    {
        if (nngShaderRenderState[D3DRS_ALPHATESTENABLE] == 0)
        {
            effect.SetAlphaTestAlways();
            return;
        }

        float reference = (nngShaderRenderState[D3DRS_ALPHAREF] & 0xFF) / 255f;
        if (!effect.SetAlphaTest((int)nngShaderRenderState[D3DRS_ALPHAFUNC], reference))
            NNM_ASSERT(false, "unsupported alpha compare function\n");
    }

    private static void mppBindStdShaderTextures(NNS_MATERIAL_STDSHADER_DESC material)
    {
        if (material.pTexDesc == null || nngCurrentTextureList == null)
            return;

        GraphicsDevice device = OpenGL.mppglGetDevice();
        foreach (NNS_MATERIAL_STDSHADER_TEXMAP_DESC texDesc in material.pTexDesc)
        {
            uint mapBit = texDesc.fType & NND_TEXTURETYPE_STDSHADER_MAP_MASK;
            int slot = mppStdShaderSamplerSlot(material.fTexType, mapBit);
            if (slot < 0 || texDesc.iTexIdx < 0 || texDesc.iTexIdx >= nngCurrentTextureList.nTex)
                continue;

            device.Textures[slot] = OpenGL.mppglGetTexture(
                nngCurrentTextureList.pTexInfoList[texDesc.iTexIdx].TexName);
            device.SamplerStates[slot] = mppStdShaderLinearSampler(
                mppStdShaderAddressMode(texDesc.WrapS), mppStdShaderAddressMode(texDesc.WrapT));
        }
    }

    // material maps are always linear, so the only thing that varies is the pair of address modes
    private static readonly SamplerState[] nngStdShaderLinearSamplers = new SamplerState[16];

    private static SamplerState mppStdShaderLinearSampler(TextureAddressMode u, TextureAddressMode v)
    {
        int index = (int)u * 4 + (int)v;
        SamplerState sampler = nngStdShaderLinearSamplers[index];
        if (sampler == null)
        {
            sampler = new SamplerState
            {
                Filter = TextureFilter.Linear,
                AddressU = u,
                AddressV = v,
            };
            nngStdShaderLinearSamplers[index] = sampler;
        }

        return sampler;
    }

    // D3DTEXTUREADDRESS, which is what WrapS/WrapT carry. XNA has no border mode.
    private static TextureAddressMode mppStdShaderAddressMode(int wrap)
    {
        switch (wrap)
        {
            case 2: return TextureAddressMode.Mirror;
            case 3:
            case 4: return TextureAddressMode.Clamp;
            default: return TextureAddressMode.Wrap;
        }
    }

    // fxc emits a D3D9 effect that MojoShader reads; mgfxc emits MGFX. Shaders.targets builds
    // whichever the backend needs, so the extension follows the backend
#if MONOGAME
    private const string MPP_SHADER_EXTENSION = ".mgfxo";
#else
    private const string MPP_SHADER_EXTENSION = ".fxb";
#endif

    private static NnStdShaderEffect mppLoadStdShader(NNS_STDSHADER_NAME name)
    {
        if (nngStdShaderCache.TryGetValue(name, out NnStdShaderEffect cached))
            return cached;

        string permutation = name.ToString();
        NnStdShaderEffect effect = null;
        try
        {
            using (Stream stream = TitleContainer.OpenStream(
                "Content\\Shaders\\nnstd\\" + permutation + MPP_SHADER_EXTENSION))
            {
                var code = new byte[stream.Length];
                stream.Read(code, 0, code.Length);
                effect = new NnStdShaderEffect(OpenGL.mppglGetDevice(), code);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"nnstdshader {permutation}: {ex.Message}");
        }

        nngStdShaderCache[name] = effect;
        return effect;
    }

    public static void nnDrawMeshIndexPrimitiveList(NNS_VTXLISTPTR vlistptr, NNS_PRIMLISTPTR plistptr)
    {
        if (!(vlistptr.pVtxList is NNS_VTXLIST_DX_DESC desc)
            || !(plistptr.pPrimList is NNS_PRIMTYPE_DX_STRIPLIST strip))
            return;

        // XNA needs render states turned into state objects before the draw call
        mppApplyStdShaderRenderState();

        GraphicsDevice device = OpenGL.mppglGetDevice();
        if (strip.pD3DIndexBuffer == null)
        {
            strip.pD3DIndexBuffer = new IndexBuffer(
                device, IndexElementSize.SixteenBits, strip.pList.Length, BufferUsage.WriteOnly);
            strip.pD3DIndexBuffer.SetData(strip.pList);
        }

        device.Indices = strip.pD3DIndexBuffer;

        int startIndex = 0;
        for (int i = 0; i < strip.nPrimitive; ++i)
        {
            int length = strip.pLengthList[i];
            if (length >= 3)
            {
                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0,
                    (int)desc.nVertex, startIndex, length - 2);
            }
            startIndex += length;
        }

        device.Indices = null;
        device.SetVertexBuffer(null);
    }

    // buffers hang off the descriptors and are built on first draw, so this can just share them
    public static uint nnVertexBufferObjectDXG20(NNS_OBJECT dstobj, NNS_OBJECT srcobj, uint flag)
    {
        if (dstobj != null)
            dstobj.Assign(srcobj);

        return 0;
    }

    public static void nnDeleteVertexObjectDXG20(NNS_OBJECT obj)
    {
        for (int i = 0; i < obj.nVtxList; ++i)
        {
            if (obj.pVtxListPtrList[i].pVtxList is NNS_VTXLIST_DX_DESC desc && desc.pD3DVertexBuffer != null)
            {
                desc.pD3DVertexBuffer.Dispose();
                desc.pD3DVertexBuffer = null;
            }
        }

        for (int i = 0; i < obj.nPrimList; ++i)
        {
            if (obj.pPrimListPtrList[i].pPrimList is NNS_PRIMTYPE_DX_STRIPLIST strip && strip.pD3DIndexBuffer != null)
            {
                strip.pD3DIndexBuffer.Dispose();
                strip.pD3DIndexBuffer = null;
            }
        }
    }
}
