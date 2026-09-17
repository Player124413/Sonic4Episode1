internal partial class AppMain
{
    internal const uint GMD_GMK_LL2_TOUCHED = 2U;

    // ev 313. PC shares this maker with 300 and 301, which pick different states off the id, but
    // neither is placed anywhere in ZONE32 and neither has a remap row, so only this arm exists.
    // The 313s come in pairs either side of a doorway, one with flag bit 0 set and one clear, so
    // whichever you touched last decides which side of the threshold you are on
    private static OBS_OBJECT_WORK GmGmkLl2VolumeInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        OBS_OBJECT_WORK work = GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y, () => new GMS_ENEMY_COM_WORK(), "GMK_LL2_VOLUME");
        OBS_RECT_WORK pRec = ((GMS_ENEMY_COM_WORK)work).rect_work[2];
        ObjRectGroupSet(pRec, 1, 1);
        ObjRectAtkSet(pRec, 0, 0);
        ObjRectDefSet(pRec, 65534, 0);
        ObjRectSet(pRec.rect, 0, 0, eve_rec.width, eve_rec.height);
        pRec.ppDef = new OBS_RECT_WORK_Delegate1(gmGmkLl2TouchDefFunc);
        pRec.parent_obj = work;
        pRec.flag |= 192U;
        work.move_flag |= 8480U;
        work.disp_flag |= OBD_DISP_NODISP;
        work.user_work = (uint)MTM_MATH_CLIP(eve_rec.left, 0, 64);
        work.user_flag = 0U;
        work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkLl2DarkTriggerState);
        return work;
    }

    // shared with the candle: latch the touch on the owner and let its state read it
    internal static void gmGmkLl2TouchDefFunc(OBS_RECT_WORK mine_rect, OBS_RECT_WORK match_rect)
    {
        if (mine_rect.parent_obj == null || match_rect.parent_obj == null
            || match_rect.parent_obj.obj_type != 1)
            return;
        mine_rect.parent_obj.user_flag |= GMD_GMK_LL2_TOUCHED;
    }

    // the only thing in the stage that moves the darkness. Standing in one holds the target at its
    // end, because the touch re-latches every frame and the step is clamped
    private static void gmGmkLl2DarkTriggerState(OBS_OBJECT_WORK obj_work)
    {
        if ((obj_work.user_flag & GMD_GMK_LL2_TOUCHED) == 0U)
            return;
        obj_work.user_flag &= ~GMD_GMK_LL2_TOUCHED;

        GMS_ENEMY_COM_WORK ene_com = (GMS_ENEMY_COM_WORK)obj_work;
        if ((ene_com.eve_rec.flag & 1) != 0)
            gm_dark_target += GMD_DARK_LEVEL_MAX;
        else
            gm_dark_target -= GMD_DARK_LEVEL_MAX;

        gm_dark_target = MTM_MATH_CLIP(gm_dark_target, 0, GMD_DARK_LEVEL_MAX);
    }
}
