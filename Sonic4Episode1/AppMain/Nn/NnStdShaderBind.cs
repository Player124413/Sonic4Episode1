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

    // NN folds the material control block into the material colours before upload, the fixed
    // function path does it in nnPutMaterialColorGL. without this nothing fades by draw_state
    private static readonly NNS_MATERIAL_STDSHADER_COLOR nngStdShaderMatCtrlColor =
        new NNS_MATERIAL_STDSHADER_COLOR();

    private const uint NND_DRAWOBJ_MATCTRL_COLOR_MASK =
        NND_DRAWOBJ_MATCTRL_DIFFUSE | NND_DRAWOBJ_MATCTRL_AMBIENT
        | NND_DRAWOBJ_MATCTRL_SPECULAR | NND_DRAWOBJ_MATCTRL_ALPHA;

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
                Tangent = arrays.Tangents != null ? arrays.Tangents[i] : Vector3.Zero,
                Binormal = arrays.Binormals != null ? arrays.Binormals[i] : Vector3.Zero,
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

    private static NNS_MATERIAL_STDSHADER_COLOR nnApplyMaterialControlColor(
        NNS_MATERIAL_STDSHADER_COLOR pColor, uint drawFlag)
    {
        if (pColor == null || (drawFlag & NND_DRAWOBJ_MATCTRL_COLOR_MASK) == 0)
            return pColor;

        NNS_MATERIAL_STDSHADER_COLOR color = nngStdShaderMatCtrlColor.Assign(pColor);

        if ((drawFlag & NND_DRAWOBJ_MATCTRL_AMBIENT) != 0)
        {
            switch (nngMatCtrlAmbient.mode)
            {
                case NNE_MATCTRLMODE_REPLACE:
                    color.Ambient.r = nngMatCtrlAmbient.col.r;
                    color.Ambient.g = nngMatCtrlAmbient.col.g;
                    color.Ambient.b = nngMatCtrlAmbient.col.b;
                    color.Ambient.a = 1f;
                    break;
                case NNE_MATCTRLMODE_ADD:
                    color.Ambient.r += nngMatCtrlAmbient.col.r;
                    color.Ambient.g += nngMatCtrlAmbient.col.g;
                    color.Ambient.b += nngMatCtrlAmbient.col.b;
                    break;
                case NNE_MATCTRLMODE_MODULATE:
                    color.Ambient.r *= nngMatCtrlAmbient.col.r;
                    color.Ambient.g *= nngMatCtrlAmbient.col.g;
                    color.Ambient.b *= nngMatCtrlAmbient.col.b;
                    break;
            }
        }

        if ((drawFlag & NND_DRAWOBJ_MATCTRL_DIFFUSE) != 0)
        {
            switch (nngMatCtrlDiffuse.mode)
            {
                case NNE_MATCTRLMODE_REPLACE:
                    color.Diffuse.r = nngMatCtrlDiffuse.col.r;
                    color.Diffuse.g = nngMatCtrlDiffuse.col.g;
                    color.Diffuse.b = nngMatCtrlDiffuse.col.b;
                    break;
                case NNE_MATCTRLMODE_ADD:
                    color.Diffuse.r += nngMatCtrlDiffuse.col.r;
                    color.Diffuse.g += nngMatCtrlDiffuse.col.g;
                    color.Diffuse.b += nngMatCtrlDiffuse.col.b;
                    break;
                case NNE_MATCTRLMODE_MODULATE:
                    color.Diffuse.r *= nngMatCtrlDiffuse.col.r;
                    color.Diffuse.g *= nngMatCtrlDiffuse.col.g;
                    color.Diffuse.b *= nngMatCtrlDiffuse.col.b;
                    break;
            }
        }

        if ((drawFlag & NND_DRAWOBJ_MATCTRL_ALPHA) != 0)
        {
            switch (nngMatCtrlAlpha.mode)
            {
                case NNE_MATCTRLMODE_REPLACE:
                    color.Diffuse.a = nngMatCtrlAlpha.alpha;
                    break;
                case NNE_MATCTRLMODE_ADD:
                    color.Diffuse.a += nngMatCtrlAlpha.alpha;
                    break;
                case NNE_MATCTRLMODE_MODULATE:
                    color.Diffuse.a *= nngMatCtrlAlpha.alpha;
                    break;
            }
        }

        if ((drawFlag & NND_DRAWOBJ_MATCTRL_SPECULAR) != 0)
        {
            switch (nngMatCtrlSpecular.mode)
            {
                case NNE_MATCTRLMODE_REPLACE:
                    color.Specular.r = nngMatCtrlSpecular.col.r;
                    color.Specular.g = nngMatCtrlSpecular.col.g;
                    color.Specular.b = nngMatCtrlSpecular.col.b;
                    color.Specular.a = 1f;
                    break;
                case NNE_MATCTRLMODE_ADD:
                    color.Specular.r += nngMatCtrlSpecular.col.r;
                    color.Specular.g += nngMatCtrlSpecular.col.g;
                    color.Specular.b += nngMatCtrlSpecular.col.b;
                    break;
                case NNE_MATCTRLMODE_MODULATE:
                    color.Specular.r *= nngMatCtrlSpecular.col.r;
                    color.Specular.g *= nngMatCtrlSpecular.col.g;
                    color.Specular.b *= nngMatCtrlSpecular.col.b;
                    break;
            }
        }

        return color;
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
        effect.SetMaterial(nnApplyMaterialControlColor(material.pColor, val.DrawFlag), OpenGL.mppglGetAmbient());
        effect.SetTextureMatrices(material);
        effect.SetMapBlend(material);

        mppSetStdShaderAlphaTest(effect);

        // the permutation always computes fog and has no on/off switch, so "off" means moving it
        // past the far plane
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

    // XNA has no alpha test render state, so it goes to the shader instead; a caller that skips
    // this gets no cutouts at all rather than an error
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

    // fxc emits the D3D9 effect that MojoShader reads
    private const string MPP_SHADER_EXTENSION = ".fxb";

    // where the retail PC build keeps its permutation set. Ours holds one combined fx_2_0 effect
    // per key where retail splits .VSH and .PSH, so only the path and the 26 digit key names line up
    private const string MPP_STDSHADER_ARCHIVE = "NNSTDSHADER/SHADER.AMB";

    private static AMS_AMB_HEADER nngStdShaderArchive;

    private static AMS_AMB_HEADER mppStdShaderArchive()
    {
        if (nngStdShaderArchive != null)
            return nngStdShaderArchive;

        var amb = new AMS_AMB_HEADER();
        amb.data = ContentSource.Current.Read(MPP_STDSHADER_ARCHIVE);

        using (var stream = new MemoryStream(amb.data))
            readAmbHeader(stream, ref amb);

        amb.buf = new object[amb.file_num];
        nngStdShaderArchive = amb;
        return amb;
    }

    private static NnStdShaderEffect mppLoadStdShader(NNS_STDSHADER_NAME name)
    {
        if (nngStdShaderCache.TryGetValue(name, out NnStdShaderEffect cached))
            return cached;

        string permutation = name.ToString();
        NnStdShaderEffect effect = null;
        try
        {
            var member = permutation + MPP_SHADER_EXTENSION;
            var chunk = amBindSearch(mppStdShaderArchive(), member);
            if (chunk == null)
                throw new FileNotFoundException($"no '{member}' in {MPP_STDSHADER_ARCHIVE}");

            // Effect wants the blob on its own, and the chunk is a window onto the whole archive
            var code = new byte[chunk.length];
            Buffer.BlockCopy(chunk.array, chunk.offset, code, 0, code.Length);
            effect = new NnStdShaderEffect(OpenGL.mppglGetDevice(), code);
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
