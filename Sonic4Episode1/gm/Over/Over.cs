using static AppMain;

using static GmOver.GME_OVER_FADEOUT_SUBPART;
using static GmOver.GME_OVER_STRING_SUBPART;
using static GmOver.GME_OVER_TEX_IDX;
using static GmOver.GME_OVER_TYPE;

internal static partial class GmOver
{
    private const int TEX_NUM = (int)GME_OVER_TEX_IDX_MAX;
    private const int STRING_NUM = (int)GME_OVER_STRING_SUBPART_MAX;
    private const int FADEOUT_NUM = (int)GME_OVER_FADEOUT_SUBPART_MAX;

    private static MTS_TASK_TCB gm_over_tcb;
    private static AMS_AMB_HEADER[] gm_over_texamb_list = new AMS_AMB_HEADER[TEX_NUM];
    private static readonly AOS_TEXTURE[] gm_over_textures = New<AOS_TEXTURE>(TEX_NUM);

    // slot 1 is the G_OVER_<lang> member, the only one that can sit past the end of the archive
    private static int gmOverLanguage()
    {
        return GmGameDatCockpitLanguage(gm_over_tex_amb_idx_tbl, 1);
    }

    public static void GmOverBuildDataInit()
    {
        var lang = gmOverLanguage();

        for (int i = 0; i < TEX_NUM; i++)
        {
            gm_over_textures[i].Clear();

            gm_over_texamb_list[i] = (AMS_AMB_HEADER)ObjDataLoadAmbIndex(
                null, gm_over_tex_amb_idx_tbl[lang][i], GmGameDatGetCockpitData());

            AoTexBuild(gm_over_textures[i], gm_over_texamb_list[i]);
            AoTexLoad(gm_over_textures[i]);
        }
    }

    public static bool GmOverBuildDataLoop()
    {
        for (int i = 0; i < TEX_NUM; i++)
        {
            if (!AoTexIsLoaded(gm_over_textures[i]))
                return false;
        }

        return true;
    }

    public static void GmOverFlushDataInit()
    {
        for (int i = 0; i < TEX_NUM; i++)
            AoTexRelease(gm_over_textures[i]);
    }

    public static bool GmOverFlushDataLoop()
    {
        var done = true;

        for (int i = 0; i < TEX_NUM; i++)
        {
            if (gm_over_texamb_list[i] == null)
                continue;

            if (!AoTexIsReleased(gm_over_textures[i]))
            {
                done = false;
                continue;
            }

            gm_over_texamb_list[i] = null;
            gm_over_textures[i].Clear();
        }

        return done;
    }

    public static void GmOverStart(int type)
    {
        SaveState.deleteSave();

        gm_over_tcb = MTM_TASK_MAKE_TCB(
            new GSF_TASK_PROCEDURE(gmOverMain),
            new GSF_TASK_PROCEDURE(gmOverDest),
            0U,
            GMD_OVER_TASK_PAUSELEVEL,
            GMD_TASK_PRIO_GAMEOVER,
            GMD_TASK_GROUP_GAMEOVER,
            () => new GMS_OVER_MGR_WORK(),
            "GM_OVER_MGR");

        var mgr_work = (GMS_OVER_MGR_WORK)gm_over_tcb.work;
        mgr_work.Clear();

        var lang = gmOverLanguage();

        for (int i = 0; i < STRING_NUM; i++)
        {
            mgr_work.string_sub_parts[i] = gmOverCreatePart(
                gm_over_ama_amb_idx_tbl[lang][(int)GME_OVER_TEX_IDX_STRING],
                gm_over_textures[(int)GME_OVER_TEX_IDX_STRING],
                gm_over_string_act_id_tbl[i]);
        }

        for (int i = 0; i < FADEOUT_NUM; i++)
        {
            var part = gmOverCreatePart(
                gm_over_ama_amb_idx_tbl[lang][(int)GME_OVER_TEX_IDX_COMMON],
                gm_over_textures[(int)GME_OVER_TEX_IDX_COMMON],
                gm_over_fadeout_act_id_tbl[i]);

            var obj_work = (OBS_OBJECT_WORK)part;
            obj_work.pos.z = GMD_OVER_PART_FADEOUT_POS_Z;
            obj_work.disp_flag &= ~OBD_DISP_REPEAT;

            mgr_work.fadeout_sub_parts[i] = part;
        }

        switch ((GME_OVER_TYPE)type)
        {
            case GME_OVER_TYPE_GAMEOVER:
                gmOverProcUpdateGOInit(mgr_work);
                break;

            case GME_OVER_TYPE_TIMEOVER:
                gmOverProcUpdateTOInit(mgr_work);
                break;
        }

        mgr_work.proc_disp = gmOverProcDispLoop;
    }

    private static GMS_COCKPIT_2D_WORK gmOverCreatePart(int ama_id, AOS_TEXTURE tex, uint act_id)
    {
        var obj_work = GmCockpit.GMM_COCKPIT_CREATE_WORK(
            () => new GMS_COCKPIT_2D_WORK(), null, 0, "GAME_OVER");

        var part = (GMS_COCKPIT_2D_WORK)obj_work;

        ObjObjectAction2dAMALoadSetTexlist(
            obj_work, part.obj_2d, null, null,
            ama_id, GmGameDatGetCockpitData(), AoTexGetTexList(tex), act_id, 0);

        gmOverSetActionHide(part);
        return part;
    }

    public static void GmOverExit()
    {
        if (gm_over_tcb == null)
            return;

        mtTaskClearTcb(gm_over_tcb);
        gm_over_tcb = null;
    }

    // the C++ is AoPadDirect() & (KEY_R_UP|DOWN|LEFT|RIGHT) and has no touch arm at all; the
    // hardware back button is a WP7 addition
    private static bool gmOverIsSkipKeyOn()
    {
        if ((AoPad.AoPadDirect() & ControllerConsts.CONFIRM) == 0 && !isBackKeyPressed())
            return false;

        setBackKeyRequest(false);
        return true;
    }

    private static void gmOverSetActionHide(GMS_COCKPIT_2D_WORK cpit_2d)
    {
        ((OBS_OBJECT_WORK)cpit_2d).disp_flag |= OBD_DISP_NODISP | OBD_DISP_NOUPDATE;
    }

    private static void gmOverSetActionPlay(GMS_COCKPIT_2D_WORK cpit_2d)
    {
        ((OBS_OBJECT_WORK)cpit_2d).disp_flag &= ~(OBD_DISP_NODISP | OBD_DISP_NOUPDATE);
    }

    private static bool gmOverIsActionEnd(GMS_COCKPIT_2D_WORK cpit_2d)
    {
        return (((OBS_OBJECT_WORK)cpit_2d).disp_flag & OBD_DISP_END) != 0;
    }

    private static void gmOverDest(MTS_TASK_TCB tcb)
    {
    }

    private static void gmOverMain(MTS_TASK_TCB tcb)
    {
        var mgr_work = (GMS_OVER_MGR_WORK)tcb.work;

        mgr_work.proc_update?.Invoke(mgr_work);
        mgr_work.proc_disp?.Invoke(mgr_work);
    }

    private static void gmOverProcUpdateGOInit(GMS_OVER_MGR_WORK mgr_work)
    {
        mgr_work.wait_timer = GMD_OVER_MGR_START_WAIT_TIME;
        mgr_work.proc_update = gmOverProcUpdateGOWaitStart;
    }

    private static void gmOverProcUpdateGOWaitStart(GMS_OVER_MGR_WORK mgr_work)
    {
        if (mgr_work.wait_timer != 0U)
        {
            mgr_work.wait_timer--;
            return;
        }

        gmOverSetActionPlay(mgr_work.string_sub_parts[(int)GME_OVER_STRING_SUBPART_GOV_GAME]);
        gmOverSetActionPlay(mgr_work.string_sub_parts[(int)GME_OVER_STRING_SUBPART_GOV_OVER]);

        mgr_work.wait_timer = GMD_OVER_MGR_GAMEOVER_LOOP_DURATION;
        mgr_work.proc_update = gmOverProcUpdateGOLoop;
    }

    // only the GAME OVER chain has a skip; TIME OVER's duration lives in the fade action itself
    private static void gmOverProcUpdateGOLoop(GMS_OVER_MGR_WORK mgr_work)
    {
        if (gmOverIsSkipKeyOn())
            mgr_work.wait_timer = 0U;

        if (mgr_work.wait_timer != 0U)
        {
            mgr_work.wait_timer--;
            return;
        }

        gmOverSetActionPlay(mgr_work.fadeout_sub_parts[(int)GME_OVER_FADEOUT_SUBPART_GOV_FOUT]);
        mgr_work.proc_update = gmOverProcUpdateGOWaitFadeEnd;
    }

    private static void gmOverProcUpdateGOWaitFadeEnd(GMS_OVER_MGR_WORK mgr_work)
    {
        if (!gmOverIsActionEnd(mgr_work.fadeout_sub_parts[(int)GME_OVER_FADEOUT_SUBPART_GOV_FOUT]))
            return;

        IzFadeInitEasy(
            IZE_FADE_SET_TYPE_NORMAL, IZE_FADE_TYPE_BLACK_FADEOUT, GMD_OVER_FADEOUT_TIME);

        mgr_work.proc_update = gmOverProcUpdateGOWaitFinalizeFade;
    }

    private static void gmOverProcUpdateGOWaitFinalizeFade(GMS_OVER_MGR_WORK mgr_work)
    {
        if (!IzFadeIsEnd())
            return;

        g_gm_main_system.game_flag |= GMD_GAME_FLAG_GAMEOVER_END;
        mgr_work.proc_update = null;
    }

    private static void gmOverProcUpdateTOInit(GMS_OVER_MGR_WORK mgr_work)
    {
        mgr_work.wait_timer = GMD_OVER_MGR_START_WAIT_TIME;
        mgr_work.proc_update = gmOverProcUpdateTOWaitStart;
    }

    private static void gmOverProcUpdateTOWaitStart(GMS_OVER_MGR_WORK mgr_work)
    {
        if (mgr_work.wait_timer != 0U)
        {
            mgr_work.wait_timer--;
            return;
        }

        gmOverSetActionPlay(mgr_work.string_sub_parts[(int)GME_OVER_STRING_SUBPART_TOV_TIME]);
        gmOverSetActionPlay(mgr_work.string_sub_parts[(int)GME_OVER_STRING_SUBPART_TOV_OVER]);
        gmOverSetActionPlay(mgr_work.fadeout_sub_parts[(int)GME_OVER_FADEOUT_SUBPART_TOV_FOUT]);

        mgr_work.proc_update = gmOverProcUpdateTOWaitFadeEnd;
    }

    private static void gmOverProcUpdateTOWaitFadeEnd(GMS_OVER_MGR_WORK mgr_work)
    {
        if (!gmOverIsActionEnd(mgr_work.fadeout_sub_parts[(int)GME_OVER_FADEOUT_SUBPART_TOV_FOUT]))
            return;

        IzFadeInitEasy(
            IZE_FADE_SET_TYPE_NORMAL, IZE_FADE_TYPE_BLACK_FADEOUT, GMD_OVER_FADEOUT_TIME);

        mgr_work.proc_update = gmOverProcUpdateTOWaitFinalizeFade;
    }

    private static void gmOverProcUpdateTOWaitFinalizeFade(GMS_OVER_MGR_WORK mgr_work)
    {
        if (!IzFadeIsEnd())
            return;

        g_gm_main_system.game_flag |= GMD_GAME_FLAG_GAMEOVER_END;
        mgr_work.proc_update = null;
    }

    private static void gmOverProcDispLoop(GMS_OVER_MGR_WORK mgr_work)
    {
    }
}
