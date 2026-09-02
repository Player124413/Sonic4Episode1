using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using mpp;

internal partial class AppMain
{
    private const int D3DRS_ZENABLE = 7;
    private const int D3DRS_FILLMODE = 8;
    private const int D3DRS_ZWRITEENABLE = 14;
    private const int D3DRS_ALPHATESTENABLE = 15;
    private const int D3DRS_SRCBLEND = 19;
    private const int D3DRS_DESTBLEND = 20;
    private const int D3DRS_CULLMODE = 22;
    private const int D3DRS_ZFUNC = 23;
    private const int D3DRS_ALPHAREF = 24;
    private const int D3DRS_ALPHAFUNC = 25;
    private const int D3DRS_ALPHABLENDENABLE = 27;
    private const int D3DRS_COLORWRITEENABLE = 168;
    private const int D3DRS_BLENDOP = 171;
    private const int D3DRS_BLENDFACTOR = 193;
    private const int D3DRS_SEPARATEALPHABLENDENABLE = 206;
    private const int D3DRS_SRCBLENDALPHA = 207;
    private const int D3DRS_DESTBLENDALPHA = 208;
    private const int D3DRS_BLENDOPALPHA = 209;

    private static readonly uint[] nngShaderRenderState = new uint[210];
    private static bool nngShaderRenderStateDirty = true;

    private static BlendState nngShaderBlendState;
    private static DepthStencilState nngShaderDepthState;
    private static RasterizerState nngShaderRasterState;
    public static void nnSetRenderState(int state, uint value)
    {
        if (state < 0 || state >= nngShaderRenderState.Length || nngShaderRenderState[state] == value)
            return;

        nngShaderRenderState[state] = value;
        nngShaderRenderStateDirty = true;
    }

    public static void mppApplyStdShaderRenderState()
    {
        mppApplyStdShaderRenderState(OpenGL.mppglGetDevice());
    }

    // the viewer runs its own device rather than the one OpenGL holds, and applies the same state
    public static void mppApplyStdShaderRenderState(GraphicsDevice device)
    {
        if (nngShaderRenderStateDirty)
        {
            nngShaderBlendState = mppBuildBlendState();
            nngShaderDepthState = mppBuildDepthState();
            nngShaderRasterState = mppBuildRasterState();
            nngShaderRenderStateDirty = false;
        }

        device.BlendState = nngShaderBlendState;
        device.DepthStencilState = nngShaderDepthState;
        device.RasterizerState = nngShaderRasterState;
    }

    // a material change dirties the state, so these are built once per distinct combination rather
    // than once per draw
    private static readonly Dictionary<ulong, BlendState> nngShaderBlendStates = new Dictionary<ulong, BlendState>();
    private static readonly Dictionary<int, DepthStencilState> nngShaderDepthStates = new Dictionary<int, DepthStencilState>();
    private static readonly Dictionary<int, RasterizerState> nngShaderRasterStates = new Dictionary<int, RasterizerState>();

    private static BlendState mppBuildBlendState()
    {
        bool separate = nngShaderRenderState[D3DRS_SEPARATEALPHABLENDENABLE] != 0;
        Blend colorSource = mppD3DBlend(nngShaderRenderState[D3DRS_SRCBLEND]);
        Blend colorDestination = mppD3DBlend(nngShaderRenderState[D3DRS_DESTBLEND]);
        BlendFunction colorFunction = mppD3DBlendOp(nngShaderRenderState[D3DRS_BLENDOP]);
        Blend alphaSource = mppD3DBlend(nngShaderRenderState[separate ? D3DRS_SRCBLENDALPHA : D3DRS_SRCBLEND]);
        Blend alphaDestination = mppD3DBlend(nngShaderRenderState[separate ? D3DRS_DESTBLENDALPHA : D3DRS_DESTBLEND]);
        BlendFunction alphaFunction = mppD3DBlendOp(nngShaderRenderState[separate ? D3DRS_BLENDOPALPHA : D3DRS_BLENDOP]);

        // D3DRS_ALPHABLENDENABLE off is the same surface as blending with ONE / ZERO
        if (nngShaderRenderState[D3DRS_ALPHABLENDENABLE] == 0)
        {
            colorSource = alphaSource = Blend.One;
            colorDestination = alphaDestination = Blend.Zero;
            colorFunction = alphaFunction = BlendFunction.Add;
        }

        ColorWriteChannels channels = mppD3DColorWrite(nngShaderRenderState[D3DRS_COLORWRITEENABLE]);
        uint factor = nngShaderRenderState[D3DRS_BLENDFACTOR];

        ulong key = factor
            | ((ulong)colorSource << 32)
            | ((ulong)colorDestination << 36)
            | ((ulong)colorFunction << 40)
            | ((ulong)alphaSource << 44)
            | ((ulong)alphaDestination << 48)
            | ((ulong)alphaFunction << 52)
            | ((ulong)channels << 56);

        if (nngShaderBlendStates.TryGetValue(key, out BlendState cached))
            return cached;

        var state = new BlendState
        {
            ColorSourceBlend = colorSource,
            ColorDestinationBlend = colorDestination,
            ColorBlendFunction = colorFunction,
            AlphaSourceBlend = alphaSource,
            AlphaDestinationBlend = alphaDestination,
            AlphaBlendFunction = alphaFunction,
            ColorWriteChannels = channels,
            BlendFactor = mppD3DColor(factor),
        };

        nngShaderBlendStates[key] = state;
        return state;
    }

    private static DepthStencilState mppBuildDepthState()
    {
        bool enable = nngShaderRenderState[D3DRS_ZENABLE] != 0;
        bool write = nngShaderRenderState[D3DRS_ZWRITEENABLE] != 0;
        CompareFunction function = mppD3DCmpFunc(nngShaderRenderState[D3DRS_ZFUNC]);

        int key = (enable ? 1 : 0) | (write ? 2 : 0) | ((int)function << 2);
        if (nngShaderDepthStates.TryGetValue(key, out DepthStencilState cached))
            return cached;

        var state = new DepthStencilState
        {
            DepthBufferEnable = enable,
            DepthBufferWriteEnable = write,
            DepthBufferFunction = function,
        };

        nngShaderDepthStates[key] = state;
        return state;
    }

    private static RasterizerState mppBuildRasterState()
    {
        CullMode cull = mppD3DCull(nngShaderRenderState[D3DRS_CULLMODE]);
        FillMode fill = nngShaderRenderState[D3DRS_FILLMODE] == 2 ? FillMode.WireFrame : FillMode.Solid;

        int key = (int)cull | ((int)fill << 2);
        if (nngShaderRasterStates.TryGetValue(key, out RasterizerState cached))
            return cached;

        var state = new RasterizerState
        {
            CullMode = cull,
            FillMode = fill,
            ScissorTestEnable = true,
        };

        nngShaderRasterStates[key] = state;
        return state;
    }

    // D3DCOLOR is 0xAARRGGBB
    private static Microsoft.Xna.Framework.Color mppD3DColor(uint value)
    {
        return new Microsoft.Xna.Framework.Color(
            (int)((value >> 16) & 0xFF),
            (int)((value >> 8) & 0xFF),
            (int)(value & 0xFF),
            (int)((value >> 24) & 0xFF));
    }

    private static Blend mppD3DBlend(uint value)
    {
        switch (value)
        {
            case 1: return Blend.Zero;
            case 2: return Blend.One;
            case 3: return Blend.SourceColor;
            case 4: return Blend.InverseSourceColor;
            case 5: return Blend.SourceAlpha;
            case 6: return Blend.InverseSourceAlpha;
            case 7: return Blend.DestinationAlpha;
            case 8: return Blend.InverseDestinationAlpha;
            case 9: return Blend.DestinationColor;
            case 10: return Blend.InverseDestinationColor;
            case 11: return Blend.SourceAlphaSaturation;
            case 14: return Blend.BlendFactor;
            case 15: return Blend.InverseBlendFactor;
            default: return Blend.One;
        }
    }

    private static BlendFunction mppD3DBlendOp(uint value)
    {
        switch (value)
        {
            case 2: return BlendFunction.Subtract;
            case 3: return BlendFunction.ReverseSubtract;
            case 4: return BlendFunction.Min;
            case 5: return BlendFunction.Max;
            default: return BlendFunction.Add;
        }
    }

    private static CompareFunction mppD3DCmpFunc(uint value)
    {
        switch (value)
        {
            case 1: return CompareFunction.Never;
            case 2: return CompareFunction.Less;
            case 3: return CompareFunction.Equal;
            case 4: return CompareFunction.LessEqual;
            case 5: return CompareFunction.Greater;
            case 6: return CompareFunction.NotEqual;
            case 7: return CompareFunction.GreaterEqual;
            default: return CompareFunction.Always;
        }
    }

    private static CullMode mppD3DCull(uint value)
    {
        switch (value)
        {
            case 2: return CullMode.CullClockwiseFace;
            case 3: return CullMode.CullCounterClockwiseFace;
            default: return CullMode.None;
        }
    }

    private static ColorWriteChannels mppD3DColorWrite(uint value)
    {
        var channels = ColorWriteChannels.None;
        if ((value & 1) != 0) channels |= ColorWriteChannels.Red;
        if ((value & 2) != 0) channels |= ColorWriteChannels.Green;
        if ((value & 4) != 0) channels |= ColorWriteChannels.Blue;
        if ((value & 8) != 0) channels |= ColorWriteChannels.Alpha;
        return channels;
    }

    public static void nnInitPreviousMaterialValueStdShader()
    {
        nnmaterialcore.nngPreMatFlag = uint.MaxValue;
        nnmaterialcore.nngpPreMatColor = null;
        nnmaterialcore.nngpPreMatLogic = null;
    }

    public static void nnPutMaterialFlagStdShader(NNS_DRAWCALLBACK_VAL val, uint fMatFlag)
    {
        switch (val.DrawFlag & 0x60U)
        {
            case 32:
                nnSetRenderState(D3DRS_CULLMODE, 1);
                break;
            case 64:
                nnSetRenderState(D3DRS_CULLMODE, 3);
                break;
            case 96:
                nnSetRenderState(D3DRS_CULLMODE, 2);
                break;
            default:
                nnSetRenderState(D3DRS_CULLMODE, (fMatFlag & NND_MATFLAG_DOUBLESIDE) == 0 ? 2U : 1U);
                break;
        }

        if ((val.DrawFlag & 4U) != 0)
            nnSetRenderState(D3DRS_ZWRITEENABLE, 0);
        else
            nnSetRenderState(D3DRS_ZWRITEENABLE, (fMatFlag & NND_MATFLAG_DISABLE_DEPTH_WRITING) == 0 ? 1U : 0U);

        if ((val.DrawFlag & 0x80000000U) != 0)
        {
            nnSetRenderState(D3DRS_COLORWRITEENABLE, 0);
        }
        else
        {
            uint channels = ((fMatFlag & NND_MATFLAG_DISABLE_COLOR_A_WRITING) != 0 ? 0U : 8U)
                | ((fMatFlag & NND_MATFLAG_DISABLE_COLOR_B_WRITING) != 0 ? 0U : 4U)
                | ((fMatFlag & NND_MATFLAG_DISABLE_COLOR_G_WRITING) != 0 ? 0U : 2U)
                | ((fMatFlag & NND_MATFLAG_DISABLE_COLOR_R_WRITING) == 0 ? 1U : 0U);
            nnSetRenderState(D3DRS_COLORWRITEENABLE, channels);
        }

        nnmaterialcore.nngPreMatFlag = fMatFlag;
    }

    public static void nnPutMaterialLogicStdShader(NNS_DRAWCALLBACK_VAL val, NNS_MATERIAL_STDSHADER_LOGIC pLogic)
    {
        uint logicflag = pLogic.fFlag;
        if ((logicflag & 1) != 0)
        {
            nnSetRenderState(D3DRS_ALPHABLENDENABLE, 1);
            nnSetRenderState(D3DRS_SRCBLEND, pLogic.SrcFactorRGB);
            nnSetRenderState(D3DRS_DESTBLEND, pLogic.DstFactorRGB);
            nnSetRenderState(D3DRS_BLENDOP, pLogic.BlendOp);
            if ((logicflag & 2) != 0)
            {
                nnSetRenderState(D3DRS_SRCBLENDALPHA, pLogic.SrcFactorA);
                nnSetRenderState(D3DRS_DESTBLENDALPHA, pLogic.DstFactorA);
                nnSetRenderState(D3DRS_BLENDOPALPHA, pLogic.BlendOp);
                nnSetRenderState(D3DRS_SEPARATEALPHABLENDENABLE, 1);
            }
            else
            {
                nnSetRenderState(D3DRS_SRCBLENDALPHA, 2);
                nnSetRenderState(D3DRS_DESTBLENDALPHA, 1);
                nnSetRenderState(D3DRS_BLENDOPALPHA, 1);
                nnSetRenderState(D3DRS_SEPARATEALPHABLENDENABLE, 0);
            }
        }
        else
        {
            nnSetRenderState(D3DRS_ALPHABLENDENABLE, 0);
            nnSetRenderState(D3DRS_SRCBLEND, 2);
            nnSetRenderState(D3DRS_DESTBLEND, 1);
            nnSetRenderState(D3DRS_BLENDOP, 1);
            nnSetRenderState(D3DRS_SRCBLENDALPHA, 2);
            nnSetRenderState(D3DRS_DESTBLENDALPHA, 1);
            nnSetRenderState(D3DRS_BLENDOPALPHA, 1);
            nnSetRenderState(D3DRS_SEPARATEALPHABLENDENABLE, 0);
        }

        nnSetRenderState(D3DRS_BLENDFACTOR, pLogic.BlendColor);

        NNM_ASSERT((logicflag & 4) == 0, "Direct3D9 not support LogicOp \n");

        if ((logicflag & 8) == 0)
        {
            nnSetRenderState(D3DRS_ALPHATESTENABLE, 0);
        }
        else
        {
            nnSetRenderState(D3DRS_ALPHATESTENABLE, 1);
            nnSetRenderState(D3DRS_ALPHAREF, pLogic.AlphaRef & 0xFF);
            nnSetRenderState(D3DRS_ALPHAFUNC, pLogic.AlphaFunc);
        }

        if ((val.DrawFlag & 8U) != 0 || (logicflag & 0x10) == 0)
        {
            nnSetRenderState(D3DRS_ZENABLE, 0);
        }
        else
        {
            nnSetRenderState(D3DRS_ZENABLE, 1);
            nnSetRenderState(D3DRS_ZFUNC, pLogic.DepthFunc);
        }

        nnmaterialcore.nngpPreMatLogic = pLogic;
    }

    public static int nnPutMaterialCoreStdShader(NNS_DRAWCALLBACK_VAL val)
    {
        NNS_MATERIALPTR pMatPtr = val.pMaterial;
        nnSetRenderState(D3DRS_FILLMODE, 3);

        if (!(pMatPtr.pMaterial is NNS_MATERIAL_STDSHADER_DESC pMatDesc))
        {
            NNM_ASSERT(false, "material type err \n");
            return 1;
        }

        if (val.iPrevMaterial == -1 || val.bModified != 0)
            nnInitPreviousMaterialValueStdShader();
        else if (val.iMaterial == val.iPrevMaterial)
            return 1;

        if (pMatDesc.fFlag != nnmaterialcore.nngPreMatFlag)
            nnPutMaterialFlagStdShader(val, pMatDesc.fFlag);
        if (pMatDesc.pColor != nnmaterialcore.nngpPreMatColor)
            nnmaterialcore.nngpPreMatColor = pMatDesc.pColor;
        if (pMatDesc.pLogic != nnmaterialcore.nngpPreMatLogic && pMatDesc.pLogic != null)
            nnPutMaterialLogicStdShader(val, pMatDesc.pLogic);

        mppLogMaterialState(pMatDesc);
        return 1;
    }

    // TODO(wam): temporary. Both the game and the editor come through here, so their output is
    // directly comparable; the editor renders the haloed geometry correctly and the game does not.
    private static readonly System.Collections.Generic.HashSet<string> mppMaterialStateSeen = new();

    private static void mppLogMaterialState(NNS_MATERIAL_STDSHADER_DESC desc)
    {
        if (mppMaterialStateSeen.Count >= 40)
            return;

        string line =
            $"fFlag {desc.fFlag:X4} logic {(desc.pLogic == null ? "ABSENT" : "present")}"
            + $" texType {desc.fTexType:X6}"
            + $" | blend {nngShaderRenderState[D3DRS_ALPHABLENDENABLE]}"
            + $" {nngShaderRenderState[D3DRS_SRCBLEND]}/{nngShaderRenderState[D3DRS_DESTBLEND]}"
            + $" op {nngShaderRenderState[D3DRS_BLENDOP]}"
            + $" write {nngShaderRenderState[D3DRS_COLORWRITEENABLE]}"
            + $" | atest {nngShaderRenderState[D3DRS_ALPHATESTENABLE]}"
            + $" func {nngShaderRenderState[D3DRS_ALPHAFUNC]} ref {nngShaderRenderState[D3DRS_ALPHAREF] & 0xFF}"
            + $" | z {nngShaderRenderState[D3DRS_ZENABLE]}/{nngShaderRenderState[D3DRS_ZFUNC]}"
            + $" zw {nngShaderRenderState[D3DRS_ZWRITEENABLE]} cull {nngShaderRenderState[D3DRS_CULLMODE]}";

        if (mppMaterialStateSeen.Add(line))
            System.Diagnostics.Debug.WriteLine("[matstate] " + line);
    }
}
