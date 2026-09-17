internal partial class AppMain
{
    private static OBS_ACTION3D_NN_WORK[] gm_gmk_candle_obj_3d_list;

    private const uint GMD_GMK_CANDLE_LIT = 1U;
    private const int GMD_GMK_CANDLE_FLAME_OFST_Y = 86016;
    private const int GMD_GMK_CANDLE_BURN_UNIT = 122880;
    private const int GMD_GMK_CANDLE_FADE_FRAMES = 16;

    private static void GmGmkCandleBuild()
    {
        gm_gmk_candle_obj_3d_list = GmGameDBuildRegBuildModel(
            GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_CANDLE_MDL),
            GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_CANDLE_TEX), 0U);
    }

    private static void GmGmkCandleFlush()
    {
        AMS_AMB_HEADER gimmickData = GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_CANDLE_MDL);
        GmGameDBuildRegFlushModel(gm_gmk_candle_obj_3d_list, gimmickData.file_num);
    }

    // ev 297. Touching it lights it, with no check on whether Sonic is carrying the torch: the
    // torch is thematic, the candle is the mechanism. Lighting adds rect_y to the region's light
    // and rect_w to the counter it shares with the switch of the same number
    private static OBS_OBJECT_WORK GmGmkCandleInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        OBS_OBJECT_WORK work = GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y, () => new GMS_GMK_CANDLE_WORK(), "GMK_CANDLE");
        GMS_ENEMY_3D_WORK enemy = (GMS_ENEMY_3D_WORK)work;
        GMS_GMK_CANDLE_WORK candle = (GMS_GMK_CANDLE_WORK)work;

        ObjObjectCopyAction3dNNModel(work, gm_gmk_candle_obj_3d_list[0], enemy.obj_3d);
        work.pos.z = -393216;

        OBS_RECT_WORK pRec = enemy.ene_com.rect_work[2];
        ObjRectGroupSet(pRec, 1, 1);
        ObjRectAtkSet(pRec, 0, 0);
        ObjRectDefSet(pRec, 65534, 0);
        ObjRectWorkSet(pRec, -8, -8, 8, 8);
        pRec.ppDef = new OBS_RECT_WORK_Delegate1(gmGmkLl2TouchDefFunc);
        pRec.parent_obj = work;
        work.move_flag |= OBD_MOVE_NOMOVE | OBD_MOVE_NOCOL;
        work.disp_flag |= OBD_DISP_NODIRFLIP;
        work.user_flag = 0U;

        candle.region = MTM_MATH_CLIP((sbyte)eve_rec.left, 0, 64);
        work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkCandleStateUnlit);
        return work;
    }

    private static void gmGmkCandleStateUnlit(OBS_OBJECT_WORK obj_work)
    {
        if ((obj_work.user_flag & GMD_GMK_LL2_TOUCHED) == 0U)
            return;
        obj_work.user_flag &= ~GMD_GMK_LL2_TOUCHED;

        GMS_GMK_CANDLE_WORK candle = (GMS_GMK_CANDLE_WORK)obj_work;
        GMS_EVE_RECORD_EVENT eve_rec = ((GMS_ENEMY_COM_WORK)obj_work).eve_rec;

        if ((obj_work.user_flag & GMD_GMK_CANDLE_LIT) == 0U)
        {
            obj_work.user_flag |= GMD_GMK_CANDLE_LIT;

            GMS_GMK_SW_STATE_WORK region = gm_gmk_switch_state[candle.region];
            region.counter += eve_rec.width;
            region.time += (sbyte)eve_rec.top;
            if (region.time >= GMD_GMK_LL2_LIGHT_THRESHOLD)
                region.sw = true;

            candle.flame = GmEfctZoneEsCreate(obj_work, GSD_MAIN_ZONE_TYPE_3, GME_EFCT_Z03_IDX_PC_FIRE_05);
            if (candle.flame != null)
            {
                OBS_OBJECT_WORK flame_work = candle.flame.efct_com.obj_work;
                flame_work.pos.y -= GMD_GMK_CANDLE_FLAME_OFST_Y;
                flame_work.ppOut = new MPP_VOID_OBS_OBJECT_WORK(gmGmkCandleFlameDraw);
                flame_work.user_timer = GMD_GMK_CANDLE_FADE_FRAMES + 1;
            }

            GmSoundPlaySE("CandleStick");
        }

        obj_work.flag |= 16U;
        candle.burn_timer = eve_rec.height * GMD_GMK_CANDLE_BURN_UNIT;
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkCandleStateLit);
    }

    private static void gmGmkCandleStateLit(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_CANDLE_WORK candle = (GMS_GMK_CANDLE_WORK)obj_work;
        GMS_GMK_SW_STATE_WORK region = gm_gmk_switch_state[candle.region];

        // re-touching reloads the burn timer rather than stacking another light
        if ((obj_work.user_flag & GMD_GMK_LL2_TOUCHED) != 0U)
        {
            obj_work.user_flag &= ~GMD_GMK_LL2_TOUCHED;
            candle.burn_timer = ((GMS_ENEMY_COM_WORK)obj_work).eve_rec.height * GMD_GMK_CANDLE_BURN_UNIT;
        }

        if (candle.burn_timer != 0)
        {
            candle.burn_timer -= (int)(amSystemGetFrameRateMain() * 4096f);
            if (candle.burn_timer < 0)
                candle.burn_timer = 0;
            if (candle.burn_timer != 0 && !region.reset)
                return;
        }
        else if (!region.reset)
            return;

        GMS_EVE_RECORD_EVENT eve_rec = ((GMS_ENEMY_COM_WORK)obj_work).eve_rec;
        obj_work.user_flag &= ~GMD_GMK_CANDLE_LIT;

        region.time -= (sbyte)eve_rec.top;
        if (region.time < 0)
            region.time = 0;
        if (region.time < GMD_GMK_LL2_LIGHT_THRESHOLD)
            region.sw = false;

        region.counter -= eve_rec.width;
        if (region.counter < 0)
            region.counter = 0;

        obj_work.flag &= ~16U;
        if (candle.flame != null)
            candle.flame.efct_com.obj_work.user_timer = GMD_GMK_CANDLE_FADE_FRAMES;
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkCandleStateUnlit);
    }

    // the flame effect's own draw slot, so the blob follows it rather than the candle. Scale runs
    // 1 while lit, then 16/16 down to 0 over the sixteen frames after it goes out
    // TODO(port): PC culls this against the screen with a 20 and a 128 unit margin, and uses the
    // near one to set OBD_DISP_NOUPDATE|NODISP on the sprite. Both are pure culling and neither is
    // ported, so an off-screen flame keeps updating and keeps queueing its blob
    private static void gmGmkCandleFlameDraw(OBS_OBJECT_WORK obj_work)
    {
        float scale = 1f;
        if (obj_work.user_timer <= GMD_GMK_CANDLE_FADE_FRAMES)
        {
            if (obj_work.user_timer > 0 && ObjObjectPauseCheck(0U) == 0U)
                --obj_work.user_timer;
            scale = obj_work.user_timer / (float)GMD_GMK_CANDLE_FADE_FRAMES;
        }

        ObjDrawActionSummary(obj_work);
        GmDarkLightSpriteDraw(
            obj_work.pos.x >> 12, -(obj_work.pos.y >> 12), -96f,
            OBD_DRAW_CMD_STATE_LIGHTMAP, scale, scale, byte.MaxValue);
    }
}
