using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using mpp;

internal partial class AppMain
{
    private static readonly Dictionary<string, NnStdShaderEffect> nngStdShaderCache =
        new Dictionary<string, NnStdShaderEffect>();

    private static NNS_MATRIX[] nngWorldMatrixPalette;
    private static Matrix nngStdShaderView = Matrix.Identity;
    private static Matrix nngStdShaderModelView = Matrix.Identity;
    private static Matrix[] nngStdShaderBones = Array.Empty<Matrix>();
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

        NNM_ASSERT(desc.nBoneMtx <= 16, "Matrix palette Overflow\n");

        // shader always reads four matrices in this mode, pad the tail with identity
        int count = desc.nBoneMtx;
        if ((desc.Format & NND_VTXMEMBER_DX_BLEND_MASK) == NND_VTXMEMBER_DX_BLEND4FIXED
            && (desc.Format & NND_VTXMEMBER_DX_BLENDFMT_I8) == 0)
            count = Math.Max(count, 4);

        if (nngStdShaderBones.Length < count)
            nngStdShaderBones = new Matrix[count];

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

        var profile = new NNS_STDSHADER_PROFILE();
        if (nnSetupShaderProfile(profile, val.pMaterial, drawFlag, 0U, subobj.fType, desc.Format) < 0)
            return;

        NnStdShaderEffect effect = mppLoadStdShader(nnGetStdShaderName(profile).ToString());
        if (effect == null)
            return;

        Matrix projection = OpenGL.mppglGetProjection();
        effect.SetTransforms(nngStdShaderModelView, projection);
        if (nngStdShaderBoneCount > 0)
        {
            var bones = new Matrix[nngStdShaderBoneCount];
            Array.Copy(nngStdShaderBones, bones, nngStdShaderBoneCount);
            effect.SetBoneMatrices(bones);
        }

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

        // XNA has no alpha test render state, so it goes to the shader instead
        if (nngShaderRenderState[D3DRS_ALPHATESTENABLE] != 0)
        {
            float reference = (nngShaderRenderState[D3DRS_ALPHAREF] & 0xFF) / 255f;
            if (!effect.SetAlphaTest((int)nngShaderRenderState[D3DRS_ALPHAFUNC], reference))
                NNM_ASSERT(false, "unsupported alpha compare function\n");
        }
        else
        {
            effect.SetAlphaTestAlways();
        }

        // permutation always computes fog, so "off" means moving it past the far plane
        OpenGL.mppglGetFog(out bool fogEnabled, out Vector3 fogColor, out float fogStart, out float fogEnd);
        if (fogEnabled && (material.fFlag & NND_MATFLAG_DISABLE_FOGGING) == 0)
            effect.SetFog(fogColor, fogStart, fogEnd);
        else
            effect.SetFog(Vector3.Zero, 1e9f, 2e9f);

        mppBindStdShaderTextures(material);
        effect.CurrentTechnique.Passes[0].Apply();
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
            device.SamplerStates[slot] = new SamplerState
            {
                Filter = TextureFilter.Linear,
                AddressU = mppStdShaderAddressMode(texDesc.WrapS),
                AddressV = mppStdShaderAddressMode(texDesc.WrapT),
            };
        }
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

    private static NnStdShaderEffect mppLoadStdShader(string permutation)
    {
        if (nngStdShaderCache.TryGetValue(permutation, out NnStdShaderEffect cached))
            return cached;

        NnStdShaderEffect effect = null;
        try
        {
            using (Stream stream = TitleContainer.OpenStream("Content\\Shaders\\nnstd\\" + permutation + ".fxb"))
            {
                var code = new byte[stream.Length];
                stream.ReadExactly(code, 0, code.Length);
                effect = new NnStdShaderEffect(OpenGL.mppglGetDevice(), code);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"nnstdshader {permutation}: {ex.Message}");
        }

        nngStdShaderCache[permutation] = effect;
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
