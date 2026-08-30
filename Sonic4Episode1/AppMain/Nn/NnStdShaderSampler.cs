internal partial class AppMain
{
    // order NN's preprocessor allocates sampler registers in (nnstdshader/sampler_def.h); a map's
    // register depends on which other maps are present, not on its own bit position
    private static readonly uint[] mppStdShaderMapOrder =
    {
        NND_TEXTURETYPE_STDSHADER_NORMAL_MAP,
        NND_TEXTURETYPE_STDSHADER_BASE_MAP,
        NND_TEXTURETYPE_STDSHADER_DECAL_MAP,
        NND_TEXTURETYPE_STDSHADER_DECAL2_MAP,
        NND_TEXTURETYPE_STDSHADER_DECAL3_MAP,
        NND_TEXTURETYPE_STDSHADER_SPECULAR_MAP,
        NND_TEXTURETYPE_STDSHADER_SHININESS_MAP,
        NND_TEXTURETYPE_STDSHADER_DUALPARABOLOID_MAP,
        NND_TEXTURETYPE_STDSHADER_ENVMASK_MAP,
        NND_TEXTURETYPE_STDSHADER_MODULATE_MAP,
        NND_TEXTURETYPE_STDSHADER_ADD_MAP,
        NND_TEXTURETYPE_STDSHADER_OPACITY_MAP,
        NND_TEXTURETYPE_STDSHADER_USER1_MAP,
        NND_TEXTURETYPE_STDSHADER_USER2_MAP,
        NND_TEXTURETYPE_STDSHADER_USER3_MAP,
        NND_TEXTURETYPE_STDSHADER_USER4_MAP,
        NND_TEXTURETYPE_STDSHADER_USER5_MAP,
        NND_TEXTURETYPE_STDSHADER_USER6_MAP,
        NND_TEXTURETYPE_STDSHADER_USER7_MAP,
        NND_TEXTURETYPE_STDSHADER_USER8_MAP,
    };

    public static int mppStdShaderSamplerSlot(uint fTexType, uint mapBit)
    {
        int slot = 0;
        foreach (uint candidate in mppStdShaderMapOrder)
        {
            if (candidate == mapBit)
                return (fTexType & mapBit) != 0 ? slot : -1;
            if ((fTexType & candidate) != 0)
                ++slot;
        }
        return -1;
    }

    public static int mppStdShaderSamplerCount(uint fTexType)
    {
        int count = 0;
        foreach (uint candidate in mppStdShaderMapOrder)
        {
            if ((fTexType & candidate) != 0)
                ++count;
        }
        return count;
    }

    public static NNS_MATERIAL_STDSHADER_TEXMAP_DESC mppStdShaderTexMap(
        NNS_MATERIAL_STDSHADER_DESC material, uint mapBit)
    {
        if (material.pTexDesc == null)
            return null;

        foreach (NNS_MATERIAL_STDSHADER_TEXMAP_DESC desc in material.pTexDesc)
        {
            if ((desc.fType & mapBit) != 0)
                return desc;
        }
        return null;
    }
}
