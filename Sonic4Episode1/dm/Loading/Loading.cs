using static AppMain;

using static DmLoading.DME_LOADING_ACT;
using static DmLoading.DME_LOADING_DATA_TYPE;

internal static partial class DmLoading
{
    private const int DATA_TYPE_NUM = (int)DME_LOADING_DATA_TYPE_MAX;
    private const int ACT_COUNT = (int)ACT_NUM;

    private static readonly uint[] dm_act_id_tbl =
    [
        IDA_D_LOAD_ACT_BG_WHITE,
        IDA_D_LOAD_ACT_BG_BOTTOM,
        IDA_D_LOAD_ACT_OBI,
        IDA_D_LOAD_ACT_RUN,
        IDA_D_LOAD_ACT_PERIOD1,
        IDA_D_LOAD_ACT_PERIOD2,
        IDA_D_LOAD_ACT_PERIOD3,
        IDA_D_LOAD_ACT_TEXT_LOAD,
    ];

    private static DMS_LOADING_MGR dm_loading_mgr_p;

    private static A2S_AMA_HEADER[] dm_loading_ama = new A2S_AMA_HEADER[DATA_TYPE_NUM];
    private static AMS_AMB_HEADER[] dm_loading_amb = new AMS_AMB_HEADER[DATA_TYPE_NUM];
    private static AOS_TEXTURE[] dm_loading_tex = New<AOS_TEXTURE>(DATA_TYPE_NUM);

    private static bool dm_loading_check_load_comp;
    private static uint dm_loading_draw_state;

    public static void DmLoadingBuild(AMS_FS arc_amb)
    {
        dm_loading_mgr_p = new DMS_LOADING_MGR();

        for (int i = 0; i < DATA_TYPE_NUM; i++)
            dm_loading_tex[i] = new AOS_TEXTURE();

        for (int i = 0; i < DATA_TYPE_NUM; i++)
        {
            dm_loading_ama[i] = readAMAFile(amBindGet(arc_amb, 0, out _));
            dm_loading_amb[i] = readAMBFile(amBindGet(arc_amb, 1, out var dir));
            dm_loading_amb[i].dir = dir;
        }

        for (int i = 0; i < DATA_TYPE_NUM; i++)
        {
            AoTexBuild(dm_loading_tex[i], dm_loading_amb[i]);
            AoTexLoad(dm_loading_tex[i]);
        }
    }

    public static bool DmLoadingBuildCheck()
    {
        return dmLoadingIsTexLoad();
    }

    public static void DmLoadingFlush()
    {
        for (int i = 0; i < DATA_TYPE_NUM; i++)
            AoTexRelease(dm_loading_tex[i]);
    }

    public static bool DmLoadingFlushCheck()
    {
        if (!dmLoadingIsTexRelease())
            return false;

        dm_loading_mgr_p = null;
        return true;
    }

    public static void DmLoadingStart()
    {
        dmLoadingInit();
    }

    public static bool DmLoadingIsExit()
    {
        return dm_loading_mgr_p == null || dm_loading_mgr_p.tcb == null;
    }

    public static void DmLoadingExit()
    {
        if (dm_loading_mgr_p.tcb == null)
            return;

        mtTaskClearTcb(dm_loading_mgr_p.tcb);
        dm_loading_mgr_p.tcb = null;
    }

    public static void DmLoadingSetLoadComplete()
    {
        dm_loading_check_load_comp = true;
    }

    private static void dmLoadingInit()
    {
        dm_loading_mgr_p.tcb = MTM_TASK_MAKE_TCB(
            new GSF_TASK_PROCEDURE(dmLoadingProcMain),
            new GSF_TASK_PROCEDURE(dmLoadingDest),
            0U,
            DMD_LOADING_TASK_PAUSELEVEL,
            DMD_LOADING_TASK_PRIO_MAIN,
            DMD_LOADING_TASK_GROUP_MAIN,
            () => new DMS_LOADING_MAIN_WORK(),
            "LOADING_MAIN");

        var main_work = (DMS_LOADING_MAIN_WORK)dm_loading_mgr_p.tcb.work;

        main_work.draw_state = AoActSysGetDrawStateEnable() ? 1U : 0U;
        AoActSysSetDrawStateEnable(main_work.draw_state != 0U);

        if (main_work.draw_state != 0U)
            dm_loading_draw_state = AoActSysGetDrawState();

        dmLoadingSetInitData(main_work);

        main_work.proc_update = dmLoadingProcInit;
    }

    private static void dmLoadingSetInitData(DMS_LOADING_MAIN_WORK main_work)
    {
        dm_loading_check_load_comp = false;

        switch ((GSE_EVT_ID)SyGetEvtInfo().cur_evt_id)
        {
            case GSE_EVT_ID.GSD_EVT_ID_MAINGAME:
            case GSE_EVT_ID.GSD_EVT_ID_ENDING:
            case GSE_EVT_ID.GSD_EVT_ID_SPSTAGE_BRANCH:
                main_work.is_maingame_load = true;
                main_work.sonic_pos_x = DMD_LOADING_SONIC_MAIN_INIT_POS;
                break;

            default:
                main_work.is_maingame_load = false;
                main_work.sonic_pos_x = DMD_LOADING_SONIC_OTHER_INIT_POS;
                break;
        }

        main_work.lang_id = GsEnvGetLanguage();
    }

    private static void dmLoadingProcMain(MTS_TASK_TCB tcb)
    {
        var main_work = (DMS_LOADING_MAIN_WORK)tcb.work;

        if ((main_work.flag & DMD_LOADING_FLAG_EXIT) != 0)
            DmLoadingExit();

        main_work.proc_update?.Invoke(main_work);
        main_work.proc_draw?.Invoke(main_work);
    }

    private static void dmLoadingDest(MTS_TASK_TCB tcb)
    {
    }

    private static void dmLoadingProcInit(DMS_LOADING_MAIN_WORK main_work)
    {
        main_work.proc_update = dmLoadingProcCreateAct;
    }

    private static void dmLoadingProcCreateAct(DMS_LOADING_MAIN_WORK main_work)
    {
        for (int i = 0; i < ACT_COUNT; i++)
        {
            var ama = dm_loading_ama[(int)DME_LOADING_DATA_TYPE_CMN_DATA];
            var tex = dm_loading_tex[(int)DME_LOADING_DATA_TYPE_CMN_DATA];

            AoActSetTexture(AoTexGetTexList(tex));
            main_work.act[i] = AoActCreate(ama, dm_act_id_tbl[i]);
        }

        main_work.proc_update = dmLoadingProcFadeIn;
        main_work.proc_draw = dmLoadingProcActDraw;

        if (main_work.is_maingame_load)
        {
            IzFadeInitEasyColor(
                IZD_FADE_TASK_GROUP_DEF,
                DMD_LOADING_TASK_PAUSELEVEL,
                IZD_FADE_DT_PRIO_DEF,
                IZD_FADE_DRAW_STATE_DEF,
                IZE_FADE_SET_TYPE_TAKEOEVER,
                IZE_FADE_TYPE_BLACK_FADEIN,
                DMD_LOADING_FADEIN_TIME,
                true);
        }
        else
        {
            IzFadeInitEasy(
                IZE_FADE_SET_TYPE_NORMAL,
                IZE_FADE_TYPE_BLACK_FADEIN,
                DMD_LOADING_FADEIN_TIME);
        }
    }

    private static void dmLoadingProcFadeIn(DMS_LOADING_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;

        IzFadeExit();
        main_work.proc_update = dmLoadingProcNowLoading;
    }

    private static void dmLoadingProcNowLoading(DMS_LOADING_MAIN_WORK main_work)
    {
        if (dm_loading_check_load_comp && main_work.timer > 60.0f)
        {
            main_work.proc_update = dmLoadingProcAlreadyLoaded;
            main_work.timer = 0.0f;

            if (main_work.is_maingame_load)
            {
                IzFadeInitEasyColor(
                    IZD_FADE_TASK_GROUP_DEF,
                    DMD_LOADING_TASK_PAUSELEVEL,
                    IZD_FADE_DT_PRIO_DEF,
                    IZD_FADE_DRAW_STATE_DEF,
                    IZE_FADE_SET_TYPE_NORMAL,
                    IZE_FADE_TYPE_WHITE_FADEOUT,
                    DMD_LOADING_FADEOUT_TIME,
                    true);
            }
            else
            {
                IzFadeInitEasy(
                    IZE_FADE_SET_TYPE_NORMAL,
                    IZE_FADE_TYPE_WHITE_FADEOUT,
                    DMD_LOADING_FADEOUT_TIME);
            }
        }

        if (main_work.sonic_set_frame >= DMD_LOADING_LOAD_LOOP_TIME)
            main_work.sonic_set_frame = 0.0f;

        main_work.sonic_set_frame++;
        main_work.timer++;
    }

    private static void dmLoadingProcAlreadyLoaded(DMS_LOADING_MAIN_WORK main_work)
    {
        if (main_work.timer > DMD_LOADING_LOADED_WAIT_TIME)
        {
            main_work.proc_update = dmLoadingProcFadeOut;
            main_work.timer = 0.0f;
        }

        if (main_work.sonic_set_frame >= DMD_LOADING_LOAD_LOOP_TIME)
            main_work.sonic_set_frame = 0.0f;

        main_work.sonic_pos_x += DMD_LOADING_SONIC_MOVE_INIT_SPD;

        main_work.sonic_set_frame++;
        main_work.timer++;
    }

    private static void dmLoadingProcFadeOut(DMS_LOADING_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;

        main_work.proc_update = dmLoadingProcStopDraw;
        main_work.proc_draw = null;
    }

    private static void dmLoadingProcStopDraw(DMS_LOADING_MAIN_WORK main_work)
    {
        for (int i = 0; i < ACT_COUNT; i++)
        {
            if (main_work.act[i] == null)
                continue;

            AoActDelete(main_work.act[i]);
            main_work.act[i] = null;
        }

        main_work.proc_update = null;
        main_work.flag |= DMD_LOADING_FLAG_EXIT;
    }

    private static void dmLoadingProcActDraw(DMS_LOADING_MAIN_WORK main_work)
    {
        dmLoadingCommonDraw(main_work);

        if (main_work.draw_state != 0U)
            amDrawMakeTask(new TaskProc(dmLoadingTaskDraw), DMD_LOADING_DRAW_TASK_PRIO, null);
    }

    private static void dmLoadingTaskDraw(AMS_TCB tcb)
    {
        AoActDrawPre();
        amDrawExecCommand(dm_loading_draw_state);
        amDrawEndScene();
    }

    private static AOS_ACTION dmLoadingAct(DMS_LOADING_MAIN_WORK main_work, DME_LOADING_ACT act)
    {
        return main_work.act[(int)act];
    }

    private static void dmLoadingCommonDraw(DMS_LOADING_MAIN_WORK main_work)
    {
        AoActSysSetDrawTaskPrio(DMD_LOADING_DRAW_PRIO_BG);

        AoActSetTexture(AoTexGetTexList(dm_loading_tex[(int)DME_LOADING_DATA_TYPE_CMN_DATA]));

        for (int i = (int)ACT_BG_WHITE; i < ACT_COUNT; i++)
            AoActSortRegAction(main_work.act[i]);

        AoActSetFrame(dmLoadingAct(main_work, ACT_RUN), main_work.sonic_set_frame);
        AoActSetFrame(dmLoadingAct(main_work, ACT_TEXT_LOAD), main_work.lang_id);

        AoActSetTexture(AoTexGetTexList(dm_loading_tex[(int)DME_LOADING_DATA_TYPE_CMN_DATA]));

        for (int i = (int)ACT_BG_WHITE; i <= (int)ACT_OBI; i++)
            AoActUpdate(main_work.act[i], 0.0f);

        AoActAcmPush();
        AoActAcmInit();
        AoActAcmApplyTrans(main_work.sonic_pos_x, DMD_LOADING_SONIC_POS_Y, 0.0f);
        AoActUpdate(dmLoadingAct(main_work, ACT_RUN), 0.0f);
        AoActAcmPop();

        for (int i = (int)ACT_PERIOD1; i <= (int)ACT_PERIOD3; i++)
            AoActUpdate(main_work.act[i], 1.0f);

        AoActUpdate(dmLoadingAct(main_work, ACT_TEXT_LOAD), 0.0f);

        if (main_work.lang_id >= DMD_LOADING_LANG_TEX_FIRST)
        {
            dmLoadingAct(main_work, ACT_TEXT_LOAD).sprite.tex_id =
                DMD_LOADING_LANG_TEX_ID + main_work.lang_id - DMD_LOADING_LANG_TEX_FIRST;
        }

        AoActSortExecute();
        AoActSortDraw();
        AoActSortUnregAll();
    }

    private static bool dmLoadingIsTexLoad()
    {
        for (int i = 0; i < DATA_TYPE_NUM; i++)
        {
            if (!AoTexIsLoaded(dm_loading_tex[i]))
                return false;
        }

        return true;
    }

    private static bool dmLoadingIsTexRelease()
    {
        for (int i = 0; i < DATA_TYPE_NUM; i++)
        {
            if (!AoTexIsReleased(dm_loading_tex[i]))
                return false;
        }

        return true;
    }
}
