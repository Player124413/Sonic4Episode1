using static AppMain;

// PC's "press Y for the next act" prompt isn't in the leak or the mobile archive, so it's gated
// on the loaded G_RSLT by action name instead of the variant
internal static partial class GmClearDemo
{
    private static bool gm_clrdm_has_next_prompt;

    // the mobile G_RSLT has its touch buttons at 100-105, so ids alone would pass on the wrong
    // archive. Names settle it, and the language archive has to carry tex_next as well
    private static bool gmClearDemoIsNextPromptDataComplete()
    {
        if (!gm_clrdm_variant.is_console)
            return false;

        var cockpit = GmGameDatGetCockpitData();
        var rslt = readAMAFile(ObjDataLoadAmbIndex(null, IDB_CPIT_MAIN_G_RSLT_AMA, cockpit));
        var lang = readAMAFile(ObjDataLoadAmbIndex(null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], cockpit));
        if (rslt == null || lang == null)
            return false;

        return rslt.FindAct("tbl01_l") == GMD_CLRDM_NEXT_TBL_ACT_ID
            && rslt.FindAct("tbl01_c") == GMD_CLRDM_NEXT_TBL_ACT_ID + 1
            && rslt.FindAct("tbl01_r") == GMD_CLRDM_NEXT_TBL_ACT_ID + 2
            && rslt.FindAct("btn_next_key") == GMD_CLRDM_NEXT_BTN_KEY_ACT_ID
            && rslt.FindAct("btn_next") == GMD_CLRDM_NEXT_BTN_PAD_ACT_ID
            && lang.FindAct("tex_next") == GMD_CLRDM_NEXT_TEX_ACT_ID;
    }

    private static void gmClearDemoCreateObjNextPromptWork(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (!gm_clrdm_has_next_prompt)
            return;

        TaskWorkFactoryDelegate work_size = () => new GMS_COCKPIT_2D_WORK();
        for (int index = 0; index < 3; ++index)
            main_work.next_tbl_act[index] = (GMS_COCKPIT_2D_WORK) GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_NEXT_TBL");
        main_work.next_btn_act = (GMS_COCKPIT_2D_WORK) GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_NEXT_BTN");
        main_work.next_tex_act = (GMS_COCKPIT_2D_WORK) GmCockpit.GMM_COCKPIT_CREATE_WORK(work_size, null, 0, "CLRDM_NEXT_TEXT");
    }

    private static void gmClearDemoCreateObjNextPromptAct(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (!gm_clrdm_has_next_prompt)
            return;

        var cockpit = GmGameDatGetCockpitData();
        var cmn_texlist = AoTexGetTexList(main_work.tex[0]);
        for (int index = 0; index < 3; ++index)
            ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.next_tbl_act[index], main_work.next_tbl_act[index].obj_2d, null, null, IDB_CPIT_MAIN_G_RSLT_AMA, cockpit, cmn_texlist, (uint) (GMD_CLRDM_NEXT_TBL_ACT_ID + index), 0);

        var btn_id = AoPad.AoPadIsGamePadConnected() ? GMD_CLRDM_NEXT_BTN_PAD_ACT_ID : GMD_CLRDM_NEXT_BTN_KEY_ACT_ID;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.next_btn_act, main_work.next_btn_act.obj_2d, null, null, IDB_CPIT_MAIN_G_RSLT_AMA, cockpit, cmn_texlist, (uint) btn_id, 0);
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.next_tex_act, main_work.next_tex_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], cockpit, AoTexGetTexList(main_work.tex[1]), GMD_CLRDM_NEXT_TEX_ACT_ID, 0);
    }

    private static void gmClearDemoFreezeNextPrompt(GMS_CLRDM_MAIN_WORK main_work)
    {
        for (int index = 0; index < 3; ++index)
            ((OBS_OBJECT_WORK) main_work.next_tbl_act[index]).disp_flag |= OBD_DISP_NOUPDATE;
        ((OBS_OBJECT_WORK) main_work.next_btn_act).disp_flag |= OBD_DISP_NOUPDATE;
        ((OBS_OBJECT_WORK) main_work.next_tex_act).disp_flag |= OBD_DISP_NOUPDATE;
    }

    private static void gmClearDemoUnfreezeNextPrompt(GMS_CLRDM_MAIN_WORK main_work)
    {
        for (int index = 0; index < 3; ++index)
            ((OBS_OBJECT_WORK) main_work.next_tbl_act[index]).disp_flag &= ~OBD_DISP_NOUPDATE;
        ((OBS_OBJECT_WORK) main_work.next_btn_act).disp_flag &= ~OBD_DISP_NOUPDATE;
        ((OBS_OBJECT_WORK) main_work.next_tex_act).disp_flag &= ~OBD_DISP_NOUPDATE;
    }

    private static void gmClearDemoSetNextPromptFrame(GMS_CLRDM_MAIN_WORK main_work, float frame)
    {
        for (int index = 0; index < 3; ++index)
            main_work.next_tbl_act[index].obj_2d.frame = frame;
        main_work.next_btn_act.obj_2d.frame = frame;
        main_work.next_tex_act.obj_2d.frame = frame;
    }

    // frozen at frame 0 it never shows: the final zone, a clear leading into a special stage, and
    // a special stage not entered from a clear
    private static bool gmClearDemoIsNextPromptSuppressed(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (main_work.stage_id >= GSD_MAIN_STAGE_ID_FINAL_1 && main_work.stage_id <= GSD_MAIN_STAGE_ID_FINAL_5)
            return true;
        if ((g_gm_main_system.game_flag & GMD_GAME_FLAG_SPECIAL_STAGE) != 0)
            return true;
        return main_work.is_clear_spe_stg && g_gs_main_sys_info.prev_stage_id == ushort.MaxValue;
    }

    // this is a frame based state machine: slides in, holds at 290 until Y is pressed, plays
    // through to 307. clamps to 290 instead of checking equality so a frame skip can't step over
    // the hold.
    private static void gmClearDemoSetNextPromptInput(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (!gm_clrdm_has_next_prompt)
            return;

        var pressed = (AoPad.AoPadStand() & BUTTON.Y) != 0;
        if (gmClearDemoIsNextPromptSuppressed(main_work))
        {
            gmClearDemoFreezeNextPrompt(main_work);
            if (pressed)
                amFlagOn(ref main_work.flag, GMD_CLRDM_FLAG_SKIP);
            return;
        }

        var frame = main_work.next_tex_act.obj_2d.frame;
        if (frame >= GMD_CLRDM_NEXT_SETTLED_FRAME)
        {
            if (pressed)
                amFlagOn(ref main_work.flag, GMD_CLRDM_FLAG_SKIP);
            return;
        }

        if (frame < GMD_CLRDM_NEXT_HOLD_FRAME || (main_work.flag & GMD_CLRDM_FLAG_NEXT_PRESSED) != 0)
            return;

        gmClearDemoFreezeNextPrompt(main_work);
        gmClearDemoSetNextPromptFrame(main_work, GMD_CLRDM_NEXT_HOLD_FRAME);
        if (!pressed)
            return;

        gmClearDemoUnfreezeNextPrompt(main_work);
        amFlagOn(ref main_work.flag, GMD_CLRDM_FLAG_NEXT_PRESSED);
        g_gm_main_system.game_flag |= GMD_GAME_FLAG_NEXT_STAGE;
    }

    private static void gmClearDemoSettleNextPrompt(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (!gm_clrdm_has_next_prompt)
            return;

        if (main_work.next_btn_act.obj_2d.frame < GMD_CLRDM_NEXT_SETTLED_FRAME)
            gmClearDemoSetNextPromptFrame(main_work, GMD_CLRDM_NEXT_SETTLED_FRAME);
        gmClearDemoUnfreezeNextPrompt(main_work);
    }
}
