internal partial class AppMain
{
    // takes the decoration texture archive, GMD_DECO_DATA_INDEX_AMB_TEX, already parsed. Its first
    // member is a TXB naming one texture, GLARE.DDS on console and the whole atlas on mobile, which
    // is why glare only exists on the console arm.
    public static void GmDecoGlareSetData(AMS_AMB_HEADER tex_amb)
    {
        pIF.Clear();
        pIF.amb_header = tex_amb;
        TXB_HEADER txb = readTXBfile(amBindGet(pIF.amb_header, 0));
        pIF.tex_buf = amTxbGetTexFileList(txb);
        nnSetUpTexlist(out pIF.texlist, pIF.tex_buf.nTex, ref pIF.texlistbuf);
        pIF.regId = amTextureLoad(pIF.texlist, pIF.tex_buf, null, pIF.amb_header);
        pIF.drawFlag = 1;
        pIF.texId = 0;
    }

    public static void GmDecoGlareDraw(OBS_OBJECT_WORK obj_work)
    {
        int userWork = (int)obj_work.user_work;
        if (userWork < 14)
            return;
        GMS_DECOGLARE_PARAM gmsDecoglareParam;
        switch (userWork)
        {
            case 14:
            case 109:
                gmsDecoglareParam = _gm_decoGlare_param[0];
                break;
            case 15:
                gmsDecoglareParam = _gm_decoGlare_param[1];
                break;
            case 16:
            case 110:
                gmsDecoglareParam = _gm_decoGlare_param[2];
                break;
            case 54:
                gmsDecoglareParam = _gm_decoGlare_param[3];
                break;
            default:
                return;
        }
        if (((int)obj_work.disp_flag & 32) != 0)
            return;
        VecFx32 pos = new VecFx32(obj_work.pos);
        gmDecoGlareDrawSprite(gmsDecoglareParam, gmsDecoglareParam.color, pos.x >> 12, -(pos.y >> 12),
            256f, OBD_DRAW_CMD_STATE_WATER, 1f, 1f, 1, 10f);
    }

    // the darkness draws the same quad from a synthetic preset: white, four times the size, the
    // alpha the caller asks for, no z test because it lands in the light map rather than the scene,
    // and no corner offset. Skipped once the stage is fully lit, where the composite is off anyway
    public static void GmDarkLightSpriteDraw(
      float x,
      float y,
      float z,
      uint command_state,
      float scale_x,
      float scale_y,
      byte alpha)
    {
        if (gm_dark_level >= 255)
            return;

        gmDecoGlareDrawSprite(gm_dark_light_sprite_param, 0xFFFFFF00U | alpha, x, y, z,
            command_state, scale_x, scale_y, 0, 0.0f);
    }

    private static void gmDecoGlareDrawSprite(
      GMS_DECOGLARE_PARAM param,
      uint color,
      float x,
      float y,
      float z,
      uint command_state,
      float scale_x,
      float scale_y,
      short ztest,
      float corner_ofst)
    {
        if (!amDrawIsRegistComplete(pIF.regId) || pIF.drawFlag != 1)
            return;
        AMS_PARAM_DRAW_PRIMITIVE setParam = GlobalPool<AMS_PARAM_DRAW_PRIMITIVE>.Alloc();
        setParam.aTest = 0;
        setParam.zMask = 0;
        setParam.zTest = ztest;
        setParam.ablend = 1;
        amDrawGetPrimBlendParam(param.ablend, setParam);
        NNS_PRIM3D_PCT_ARRAY nnsPriM3DPctArray = amDrawAlloc_NNS_PRIM3D_PCT(6);
        NNS_PRIM3D_PCT[] buffer = nnsPriM3DPctArray.buffer;
        int offset = nnsPriM3DPctArray.offset;
        float half_w = param.size * 0.5f * scale_x;
        float half_h = param.size * 0.5f * scale_y;
        float cx = x + corner_ofst;
        float cy = y - corner_ofst;
        buffer[offset].Pos.Assign(cx - half_w, cy + half_h, z);
        buffer[offset + 1].Pos.Assign(cx + half_w, cy + half_h, z);
        buffer[offset + 2].Pos.Assign(cx - half_w, cy - half_h, z);
        buffer[offset + 5].Pos.Assign(cx + half_w, cy - half_h, z);
        buffer[offset].Col = color;
        buffer[offset + 1].Col = color;
        buffer[offset + 2].Col = color;
        buffer[offset + 5].Col = color;
        buffer[offset].Tex.u = 0.0f;
        buffer[offset].Tex.v = 0.0f;
        buffer[offset + 1].Tex.u = 1f;
        buffer[offset + 1].Tex.v = 0.0f;
        buffer[offset + 2].Tex.u = 0.0f;
        buffer[offset + 2].Tex.v = 1f;
        buffer[offset + 5].Tex.u = 1f;
        buffer[offset + 5].Tex.v = 1f;
        buffer[offset + 3] = buffer[offset + 1];
        buffer[offset + 4] = buffer[offset + 2];
        setParam.format3D = 4;
        setParam.type = 0;
        setParam.vtxPCT3D = nnsPriM3DPctArray;
        setParam.texlist = pIF.texlist;
        setParam.texId = pIF.texId;
        setParam.count = 6;
        setParam.sortZ = param.sort_z;
        amDrawPrimitive3D(command_state, setParam);
        GlobalPool<AMS_PARAM_DRAW_PRIMITIVE>.Release(setParam);
    }

    public static void GmDecoGlareDataRelease()
    {
        pIF.drawFlag = 2;
        if (pIF.texlist != null)
        {
            pIF.regId = amTextureRelease(pIF.texlist);
            pIF.texlist = null;
        }
        if (pIF.texlistbuf == null)
            return;
        pIF.texlistbuf = null;
    }

    public static GMDECO_GLARE_INTERFACE GmDecoGlareGetGlobal()
    {
        return pIF;
    }
}