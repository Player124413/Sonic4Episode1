using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using mpp;

internal partial class AppMain
{
    // NNE_SAMPLERSTATETYPE_DXG20, nnlmaterialdxg20.h:41. D3DSAMPLERSTATETYPE rebased to zero, so
    // NNE_x is D3DSAMP_x - 1
    public const int NNE_SAMPLERSTATETYPE_ADDRESSU = 0;
    public const int NNE_SAMPLERSTATETYPE_ADDRESSV = 1;
    public const int NNE_SAMPLERSTATETYPE_ADDRESSW = 2;
    public const int NNE_SAMPLERSTATETYPE_BORDERCOLOR = 3;
    public const int NNE_SAMPLERSTATETYPE_MAGFILTER = 4;
    public const int NNE_SAMPLERSTATETYPE_MINFILTER = 5;
    public const int NNE_SAMPLERSTATETYPE_MIPFILTER = 6;
    public const int NNE_SAMPLERSTATETYPE_MIPMAPLODBIAS = 7;
    public const int NNE_SAMPLERSTATETYPE_MAXMIPLEVEL = 8;
    public const int NNE_SAMPLERSTATETYPE_MAXANISOTROPY = 9;
    public const int NNE_SAMPLERSTATETYPE_SRGBTEXTURE = 10;
    public const int NNE_SAMPLERSTATETYPE_ELEMENTINDEX = 11;
    public const int NNE_SAMPLERSTATETYPE_DMAPOFFSET = 12;
    public const int NNE_SAMPLERSTATETYPE_MAX = 13;

    public const uint D3DTADDRESS_WRAP = 1;
    public const uint D3DTADDRESS_MIRROR = 2;
    public const uint D3DTADDRESS_CLAMP = 3;

    public const uint D3DTEXF_NONE = 0;
    public const uint D3DTEXF_POINT = 1;
    public const uint D3DTEXF_LINEAR = 2;

    private static readonly NNS_MATCTRL_USERSAMPLER[] nngMatCtrlUserSampler =
        NewUserSamplers((int)NNE_USER_SAMPLER.NNE_USER_SAMPLER_MAX);

    // NNS_STDSHADER_CONFIG.nUserUniform, set to 4 by main.cpp:759; that is how many the shader
    // declares, where the bank NN keeps is the full 256
    public const int NND_PROF_MAX_USER_UNIFORM = 4;

    private static readonly Vector4[] nngUserUniform = new Vector4[NND_MAX_NUM_USER_UNIFORM];
    private static readonly Vector4[] nngUserUniformShader = new Vector4[NND_PROF_MAX_USER_UNIFORM];

    public static Vector4[] mppStdShaderUserUniforms()
    {
        System.Array.Copy(nngUserUniform, nngUserUniformShader, NND_PROF_MAX_USER_UNIFORM);
        return nngUserUniformShader;
    }

    private static NNS_MATCTRL_USERSAMPLER[] NewUserSamplers(int count)
    {
        var samplers = new NNS_MATCTRL_USERSAMPLER[count];
        for (int i = 0; i < count; ++i)
            samplers[i] = new NNS_MATCTRL_USERSAMPLER();
        return samplers;
    }

    public static void nnGetMaterialControlUserSamplerDefaultStateDXG20(uint[] state)
    {
        state[NNE_SAMPLERSTATETYPE_ADDRESSU] = D3DTADDRESS_WRAP;
        state[NNE_SAMPLERSTATETYPE_ADDRESSV] = D3DTADDRESS_WRAP;
        state[NNE_SAMPLERSTATETYPE_ADDRESSW] = D3DTADDRESS_WRAP;
        state[NNE_SAMPLERSTATETYPE_BORDERCOLOR] = 0;
        state[NNE_SAMPLERSTATETYPE_MAGFILTER] = D3DTEXF_POINT;
        state[NNE_SAMPLERSTATETYPE_MINFILTER] = D3DTEXF_POINT;
        state[NNE_SAMPLERSTATETYPE_MIPFILTER] = D3DTEXF_NONE;
        state[NNE_SAMPLERSTATETYPE_MIPMAPLODBIAS] = 0;
        state[NNE_SAMPLERSTATETYPE_MAXMIPLEVEL] = 0;
        state[NNE_SAMPLERSTATETYPE_MAXANISOTROPY] = 1;
        state[NNE_SAMPLERSTATETYPE_SRGBTEXTURE] = 0;
        state[NNE_SAMPLERSTATETYPE_ELEMENTINDEX] = 0;
        state[NNE_SAMPLERSTATETYPE_DMAPOFFSET] = 0;
    }

    private static readonly uint[] nngUserSamplerDefaultState = new uint[NNE_SAMPLERSTATETYPE_MAX];

    public static void nnInitMaterialControlUserSamplerDXG20()
    {
        nnGetMaterialControlUserSamplerDefaultStateDXG20(nngUserSamplerDefaultState);

        foreach (NNS_MATCTRL_USERSAMPLER sampler in nngMatCtrlUserSampler)
        {
            sampler.texmap = null;
            mppStoreUserSamplerState(sampler, nngUserSamplerDefaultState);
        }
    }

    public static void nnSetMaterialControlUserSamplerDXG20(
        NNE_USER_SAMPLER idx, Texture texmap, uint[] state)
    {
        NNS_MATCTRL_USERSAMPLER sampler = nngMatCtrlUserSampler[(int)idx];
        sampler.texmap = texmap;
        mppStoreUserSamplerState(sampler, state);
    }

    private static void mppStoreUserSamplerState(NNS_MATCTRL_USERSAMPLER sampler, uint[] state)
    {
        bool changed = sampler.sampler == null;
        for (int i = 0; i < NNE_SAMPLERSTATETYPE_MAX; ++i)
        {
            if (sampler.state[i] == state[i])
                continue;

            sampler.state[i] = state[i];
            changed = true;
        }

        if (changed)
            sampler.sampler = mppSamplerStateFrom(sampler.state);
    }

    public static void nnSetUserUniformDXG20(int idx, float x, float y, float z, float w)
    {
        if ((uint)idx >= NND_MAX_NUM_USER_UNIFORM)
            return;

        nngUserUniform[idx] = new Vector4(x, y, z, w);
    }

    private static readonly Vector4[] nngProjectionUserUniforms = new Vector4[4];

    // the shader projects a view position with this to find where on screen it is, so it has to be
    // the matrix actually bound at draw time, not the NN copy
    public static void mppSetProjectionUserUniforms()
    {
        NnStdShaderEffect.ProjectionUserUniforms(OpenGL.mppglGetProjection(), nngProjectionUserUniforms);
        for (int k = 0; k < nngProjectionUserUniforms.Length; ++k)
        {
            Vector4 row = nngProjectionUserUniforms[k];
            nnSetUserUniformDXG20(k, row.X, row.Y, row.Z, row.W);
        }
    }

    // NN also sets the tex matrix here, but the stock shader never reads it
    private static void mppPutMaterialTextureUserSampler(int slot, NNE_USER_SAMPLER idx)
    {
        NNS_MATCTRL_USERSAMPLER sampler = nngMatCtrlUserSampler[(int)idx];
        if (sampler.texmap == null)
            return;

        GraphicsDevice device = OpenGL.mppglGetDevice();
        device.Textures[slot] = sampler.texmap;
        device.SamplerStates[slot] = sampler.sampler;
    }

    private static SamplerState mppSamplerStateFrom(uint[] state)
    {
        return new SamplerState
        {
            AddressU = mppTextureAddressFrom(state[NNE_SAMPLERSTATETYPE_ADDRESSU]),
            AddressV = mppTextureAddressFrom(state[NNE_SAMPLERSTATETYPE_ADDRESSV]),
            AddressW = mppTextureAddressFrom(state[NNE_SAMPLERSTATETYPE_ADDRESSW]),
            Filter = mppTextureFilterFrom(state),
            MaxAnisotropy = (int)state[NNE_SAMPLERSTATETYPE_MAXANISOTROPY],
            MaxMipLevel = (int)state[NNE_SAMPLERSTATETYPE_MAXMIPLEVEL],
            MipMapLevelOfDetailBias = state[NNE_SAMPLERSTATETYPE_MIPMAPLODBIAS],
        };
    }

    private static TextureAddressMode mppTextureAddressFrom(uint address)
    {
        switch (address)
        {
            case D3DTADDRESS_MIRROR: return TextureAddressMode.Mirror;
            case D3DTADDRESS_CLAMP: return TextureAddressMode.Clamp;
            default: return TextureAddressMode.Wrap;
        }
    }

    // XNA collapses D3D9's three independent filters into one enum, so only the combinations it
    // names are reachable. Every user-sampler site asks for linear with no mip filter.
    private static TextureFilter mppTextureFilterFrom(uint[] state)
    {
        bool mag = state[NNE_SAMPLERSTATETYPE_MAGFILTER] != D3DTEXF_POINT;
        bool min = state[NNE_SAMPLERSTATETYPE_MINFILTER] != D3DTEXF_POINT;
        bool mip = state[NNE_SAMPLERSTATETYPE_MIPFILTER] == D3DTEXF_LINEAR;

        if (mag && min)
            return mip ? TextureFilter.Linear : TextureFilter.LinearMipPoint;
        if (!mag && !min)
            return mip ? TextureFilter.PointMipLinear : TextureFilter.Point;

        return mag ? TextureFilter.MinPointMagLinearMipPoint : TextureFilter.MinLinearMagPointMipPoint;
    }
}
