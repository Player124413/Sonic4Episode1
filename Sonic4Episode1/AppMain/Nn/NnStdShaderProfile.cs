internal partial class AppMain
{
    public const uint NND_DRAWOBJ_MATCTRL_SHADOWMAP1 = 1U << 29;
    public const uint NND_DRAWOBJ_MATCTRL_SHADOWMAP2 = 2U << 29;
    public const uint NND_DRAWOBJ_MATCTRL_SHADOWMAP_MASK = 3U << 29;
    public const uint NND_DRAWOBJ_MATCTRL_USERSAMPLER2D1 = 1U << 26;
    public const uint NND_DRAWOBJ_MATCTRL_USERSAMPLER2D2 = 1U << 27;
    public const uint NND_DRAWOBJ_MATCTRL_USERSAMPLER3D1 = 1U << 28;
    public const uint NND_DRAWOBJ_MATCTRL_USERSAMPLER3D2 = 1U << 29;

    public static uint nnDrawObjShaderUserProfile(int n) => ((uint)n & 0xFF) << 2;

    private const int mppMaxSimultaneousTextures = 8;

    public static int nnSetupShaderProfile(
        NNS_STDSHADER_PROFILE profile,
        NNS_MATERIALPTR matPtr,
        uint drawFlag,
        uint drawFlagEx,
        uint subobjFType,
        uint vtxFormat)
    {
        nnInitStdShaderProfile(profile);

        if ((matPtr.fType & NND_MATTYPE_STDSHADERDESC) == 0)
            return -1;

        if (!(matPtr.pMaterial is NNS_MATERIAL_STDSHADER_DESC material))
            return -1;

        profile.bVtxCol = (vtxFormat & (NND_VTXMEMBER_DX_COL_U32 | NND_VTXMEMBER_DX_COL2_U32)) != 0 ? 1 : 0;
        profile.bVtxColSpe = (vtxFormat & NND_VTXMEMBER_DX_COL2_U32) != 0 ? 1 : 0;
        profile.bVtxNrm = (vtxFormat & (NND_VTXMEMBER_DX_NRM_F32 | NND_VTXMEMBER_DX_NRM_U32)) != 0 ? 1 : 0;
        profile.bVtxObjSpaceNrm = (vtxFormat & NND_VTXMEMBER_DX_OBJECT_SPACE_NORMAL) != 0 ? 1 : 0;
        profile.bVtxReverseTan = (vtxFormat & NND_VTXMEMBER_DX_REVERSE_TANGENT) != 0 ? 1 : 0;

        bool rigid = (subobjFType & NND_SUBOBJTYPE_RIGID) != 0
            || (vtxFormat & NND_VTXMEMBER_DX_BLEND_MASK) != NND_VTXMEMBER_DX_BLEND4FIXED;
        profile.VtxTfmMode = rigid
            ? 0
            : ((vtxFormat & NND_VTXMEMBER_DX_BLENDFMT_I8) != 0 ? 2 : 1);

        if ((material.fFlag & 2) == 0)
        {
            profile.bLighting = (drawFlag & NND_DRAWOBJ_DISABLE_LIGHTING) == 0 ? 1 : 0;
            profile.bSpecular = mppStdShaderSpecular(material, drawFlag);
            profile.bTwoSidedLighting = (int)((material.fFlag >> 3) & 1);
            profile.nFragParallelLight = (int)((drawFlag & NND_DRAWOBJ_FRAGPARALIGHT_MASK) >> 16);
            profile.nFragPointLight = (int)((drawFlag & NND_DRAWOBJ_FRAGPOINTLIGHT_MASK) >> 18);
        }

        int texCount = 0;
        if ((drawFlag & NND_DRAWOBJ_IGNORETEXTURE) == 0)
            texCount = mppStdShaderSetupTextures(profile, material);

        if ((matPtr.fType & NND_MATTYPE_USERPROFILE) != 0)
            mppAssertNotImpl();

        mppStdShaderSetupSamplers(profile, drawFlag, drawFlagEx, texCount);
        profile.UserProfileDrawobj = (int)((drawFlagEx >> 2) & 0xFF);
        return 0;
    }

    private static int mppStdShaderSpecular(NNS_MATERIAL_STDSHADER_DESC material, uint drawFlag)
    {
        if ((drawFlag & NND_DRAWOBJ_IGNOREMATSPEC) != 0)
            return 0;

        if ((drawFlag & NND_DRAWOBJ_MATCTRL_SPECULAR) != 0)
            return 1;

        NNS_MATERIAL_STDSHADER_COLOR color = material.pColor;
        if (color == null || color.SpecularIntensity == 0f)
            return 0;

        return color.Specular.r != 0f || color.Specular.g != 0f || color.Specular.b != 0f ? 1 : 0;
    }

    private static int mppStdShaderSetupTextures(
        NNS_STDSHADER_PROFILE profile, NNS_MATERIAL_STDSHADER_DESC material)
    {
        if (material.pTexDesc == null)
            return 0;

        int texCount = 0;
        int coord = 0;
        int limit = material.nTex < mppMaxSimultaneousTextures ? material.nTex : mppMaxSimultaneousTextures;

        for (int i = 0; i < limit && i < material.pTexDesc.Length; ++i)
        {
            if (coord >= 8)
                break;

            NNS_MATERIAL_STDSHADER_TEXMAP_DESC desc = material.pTexDesc[i];
            uint map = desc.fType & NND_TEXTURETYPE_STDSHADER_MAP_MASK;
            bool takesCoord = true;

            switch (map)
            {
                case NND_TEXTURETYPE_STDSHADER_NORMAL_MAP:
                    profile.NormalMapType = (desc.fType & NND_TEXTURETYPE_STDSHADER_NORMAL_MAP_AXLY) != 0 ? 2 : 1;
                    break;
                case NND_TEXTURETYPE_STDSHADER_BASE_MAP: profile.bBaseMap = 1; break;
                case NND_TEXTURETYPE_STDSHADER_DECAL_MAP: profile.nDecalMap = 1; break;
                case NND_TEXTURETYPE_STDSHADER_DECAL2_MAP: profile.nDecalMap = 2; break;
                case NND_TEXTURETYPE_STDSHADER_DECAL3_MAP: profile.nDecalMap = 3; break;
                case NND_TEXTURETYPE_STDSHADER_SPECULAR_MAP: profile.bSpecularMap = 1; break;
                case NND_TEXTURETYPE_STDSHADER_SHININESS_MAP: profile.bShininessMap = 1; break;
                case NND_TEXTURETYPE_STDSHADER_ENVMASK_MAP: profile.bEnvMaskMap = 1; break;
                case NND_TEXTURETYPE_STDSHADER_MODULATE_MAP: profile.bModulateMap = 1; break;
                case NND_TEXTURETYPE_STDSHADER_ADD_MAP: profile.bAddMap = 1; break;
                case NND_TEXTURETYPE_STDSHADER_OPACITY_MAP: profile.bOpacityMap = 1; break;
                case NND_TEXTURETYPE_STDSHADER_USER1_MAP: profile.bUser1Map = 1; break;
                case NND_TEXTURETYPE_STDSHADER_USER2_MAP: profile.bUser2Map = 1; break;
                case NND_TEXTURETYPE_STDSHADER_USER3_MAP: profile.bUser3Map = 1; break;
                case NND_TEXTURETYPE_STDSHADER_USER4_MAP: profile.bUser4Map = 1; break;
                case NND_TEXTURETYPE_STDSHADER_USER5_MAP: profile.bUser5Map = 1; break;
                case NND_TEXTURETYPE_STDSHADER_USER6_MAP: profile.bUser6Map = 1; break;
                case NND_TEXTURETYPE_STDSHADER_USER7_MAP: profile.bUser7Map = 1; break;
                case NND_TEXTURETYPE_STDSHADER_USER8_MAP: profile.bUser8Map = 1; break;

                case NND_TEXTURETYPE_STDSHADER_DUALPARABOLOID_MAP:
                    profile.bDualParaboloidMap = 1;
                    takesCoord = false;
                    break;

                default:
                    takesCoord = false;
                    break;
            }

            if (takesCoord)
                profile.SetTexCoord(coord++, desc.TexCoord);

            ++texCount;
        }

        return texCount;
    }

    private static void mppStdShaderSetupSamplers(
        NNS_STDSHADER_PROFILE profile, uint drawFlag, uint drawFlagEx, int used)
    {
        uint shadow = drawFlag & NND_DRAWOBJ_MATCTRL_SHADOWMAP_MASK;
        if (shadow == NND_DRAWOBJ_MATCTRL_SHADOWMAP2 && used + 1 < mppMaxSimultaneousTextures)
        {
            profile.nShadowMap = 2;
            used += 2;
        }
        else if (shadow != 0 && used < mppMaxSimultaneousTextures)
        {
            profile.nShadowMap = 1;
            ++used;
        }

        mppStdShaderSampler(profile, 30, drawFlagEx, NND_DRAWOBJ_MATCTRL_USERSAMPLER2D1, ref used);
        mppStdShaderSampler(profile, 31, drawFlagEx, NND_DRAWOBJ_MATCTRL_USERSAMPLER2D2, ref used);
        mppStdShaderSampler(profile, 32, drawFlagEx, NND_DRAWOBJ_MATCTRL_USERSAMPLER3D1, ref used);
        mppStdShaderSampler(profile, 33, drawFlagEx, NND_DRAWOBJ_MATCTRL_USERSAMPLER3D2, ref used);
        mppStdShaderSampler(profile, 34, drawFlagEx, NND_DRAWOBJ_MATCTRL_USERSAMPLERCUBE1, ref used);
        mppStdShaderSampler(profile, 35, drawFlagEx, NND_DRAWOBJ_MATCTRL_USERSAMPLERCUBE2, ref used);
    }

    private static void mppStdShaderSampler(
        NNS_STDSHADER_PROFILE profile, int field, uint drawFlagEx, uint bit, ref int used)
    {
        if ((drawFlagEx & bit) == 0 || used >= mppMaxSimultaneousTextures)
            return;

        profile[field] = 1;
        ++used;
    }

    public static void nnInitStdShaderProfile(NNS_STDSHADER_PROFILE profile)
    {
        for (int i = 0; i < NNS_STDSHADER_PROFILE.FieldCount; ++i)
            profile[i] = 0;
    }
}
