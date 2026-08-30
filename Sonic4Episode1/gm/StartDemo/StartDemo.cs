using System;

using static AppMain;

using static GmStartDemo.GME_START_DEMO_ACTION_INDEX_ACT;
using static GmStartDemo.GME_START_DEMO_ACTION_INDEX_CMN;
using static GmStartDemo.GME_START_DEMO_ACTION_INDEX_ZONE;
using static GmStartDemo.GME_START_DEMO_DATA_TYPE;

internal static partial class GmStartDemo
{
    private const int DATA_TYPE_NUM = (int)GMD_START_DEMO_DATA_TYPE_NUM;
    private const int CMN_NUM = (int)GMD_START_DEMO_ACTION_INDEX_CMN_NUM;
    private const int ZONE_NUM = (int)GMD_START_DEMO_ACTION_INDEX_ZONE_NUM;
    private const int ACT_NUM = (int)GMD_START_DEMO_ACTION_INDEX_ACT_NUM;

    private static readonly GMS_START_DEMO_DATA g_start_demo_data_real = new GMS_START_DEMO_DATA();
    private static GMS_START_DEMO_DATA g_start_demo_data;

    private static readonly GMS_START_DEMO_MGR g_start_demo_mgr_real = new GMS_START_DEMO_MGR();
    private static GMS_START_DEMO_MGR g_start_demo_mgr;

    private static bool gm_start_demo_is_console;

    public static void GmStartDemoBuild()
    {
        gmStartDemoDataInit();
        var start_demo_data = gmStartDemoDataGetInfo();

        gm_start_demo_is_console = amFsAllMemberVariant(
            AMD_FS_VARIANT_CONSOLE, GMD_START_DEMO_DATA_FILENAME, gm_start_demo_console_members);

        var lang = gmStartDemoLanguage();

        for (int i = 0; i < DATA_TYPE_NUM; i++)
        {
            start_demo_data.demo_amb[i] = (AMS_AMB_HEADER)ObjDataLoadAmbIndex(
                null, gm_start_demo_data_amb_id[lang][i], GmGameDatGetCockpitData());
        }

        start_demo_data.flag_regist = false;
    }

    public static bool GmStartDemoBuildCheck()
    {
        var start_demo_data = gmStartDemoDataGetInfo();

        if (!start_demo_data.flag_regist)
        {
            if (GsMainSysGetDisplayListRegistNum()
                >= AMD_REGISTLIST_NUM - GMD_START_DEMO_BUILD_REG_ALLOWANCE_NUM)
                return false;

            for (int i = 0; i < DATA_TYPE_NUM; i++)
            {
                AoTexBuild(start_demo_data.aos_texture[i], start_demo_data.demo_amb[i]);
                AoTexLoad(start_demo_data.aos_texture[i]);
            }

            start_demo_data.flag_regist = true;
        }

        return gmStartDemoDataCheckLoading();
    }

    public static void GmStartDemoFlush()
    {
        var start_demo_data = gmStartDemoDataGetInfo();

        for (int i = 0; i < DATA_TYPE_NUM; i++)
            AoTexRelease(start_demo_data.aos_texture[i]);
    }

    public static bool GmStartDemoFlushCheck()
    {
        if (!gmStartDemoDataCheckRelease())
            return false;

        gmStartDemoDataFlush();
        return true;
    }

    public static void GmStartDemoStart()
    {
        gmStartDemoInit();
        gmStartDemoSetGameFlag(GMD_GAME_FLAG_START_DEMO);
    }

    public static void GmStartDemoExit()
    {
        gmStartDemoExit();
    }

    private static int gmStartDemoLanguage()
    {
        return GmGameDatCockpitLanguage(gm_start_demo_data_amb_id, 1);
    }

    private static void gmStartDemoDataInit()
    {
        g_start_demo_data_real.Clear();
        g_start_demo_data = g_start_demo_data_real;
    }

    private static void gmStartDemoDataFlush()
    {
        g_start_demo_data = null;
    }

    private static GMS_START_DEMO_DATA gmStartDemoDataGetInfo()
    {
        return g_start_demo_data;
    }

    private static bool gmStartDemoDataCheckLoading()
    {
        var start_demo_data = gmStartDemoDataGetInfo();

        for (int i = 0; i < DATA_TYPE_NUM; i++)
        {
            if (!AoTexIsLoaded(start_demo_data.aos_texture[i]))
                return false;
        }

        return true;
    }

    private static bool gmStartDemoDataCheckRelease()
    {
        var start_demo_data = gmStartDemoDataGetInfo();
        if (start_demo_data == null)
            return true;

        for (int i = 0; i < DATA_TYPE_NUM; i++)
        {
            if (!AoTexIsReleased(start_demo_data.aos_texture[i]))
                return false;
        }

        return true;
    }

    private static GMS_START_DEMO_MGR gmStartDemoMgrGetInfo()
    {
        return g_start_demo_mgr;
    }

    private static void gmStartDemoInit()
    {
        g_start_demo_mgr_real.Clear();
        g_start_demo_mgr = g_start_demo_mgr_real;

        g_start_demo_mgr.main_tcb = MTM_TASK_MAKE_TCB(
            new GSF_TASK_PROCEDURE(gmStartDemoProcMain),
            null,
            0U,
            GMD_TASK_PAUSELEVEL_DEF,
            GMD_TASK_PRIO_STARTDEMO,
            GMD_TASK_GROUP_STARTDEMO,
            () => new GMS_START_DEMO_WORK(),
            "START_DEMO_MAIN");

        var work = (GMS_START_DEMO_WORK)g_start_demo_mgr.main_tcb.work;
        work.counter = 0U;
        work.update = gmStartDemoProcFade;

        gmStartDemo2DActionCreate(work);

        GmPlySeqInitDemoFw(g_gm_main_system.ply_work[GSD_MAIN_PLAYER_1P]);

        IzFadeInitEasy(
            IZE_FADE_SET_TYPE_TAKEOEVER,
            IZE_FADE_TYPE_WHITE_FADEIN,
            GMD_START_DEMO_FADEIN_TIME);
    }

    private static void gmStartDemoRequestExit()
    {
        var start_demo_mgr = gmStartDemoMgrGetInfo();
        if (start_demo_mgr == null)
            return;

        if (start_demo_mgr.main_tcb != null)
        {
            mtTaskClearTcb(start_demo_mgr.main_tcb);
            start_demo_mgr.main_tcb = null;
        }

        gmStartDemoClearGameFlag(GMD_GAME_FLAG_START_DEMO);
    }

    private static void gmStartDemoExit()
    {
        if (g_start_demo_mgr == null)
            return;

        gmStartDemoRequestExit();
        g_start_demo_mgr = null;
    }

    private static void gmStartDemoProcMain(MTS_TASK_TCB tcb)
    {
        var work = (GMS_START_DEMO_WORK)tcb.work;

        if ((work.flag & GMD_START_DEMO_FLAG_EXIT) != 0)
        {
            gmStartDemoRequestExit();
            return;
        }

        work.update?.Invoke(work);

        work.counter++;
    }

    private static void gmStartDemoSetGameFlag(uint flag)
    {
        g_gm_main_system.game_flag |= flag;
    }

    private static void gmStartDemoClearGameFlag(uint flag)
    {
        g_gm_main_system.game_flag &= ~flag;
    }

    // the four groups are created, animated and killed together, and the C++ walks all four by hand
    // in five different places
    private static void gmStartDemoEachAction(GMS_START_DEMO_WORK work, Action<GMS_COCKPIT_2D_WORK> body)
    {
        foreach (var group in new[] { work.action_obj_work_cmn, work.action_obj_work_zone, work.action_obj_work_act })
        {
            foreach (var action in group)
            {
                if (action != null)
                    body(action);
            }
        }

        if (work.action_obj_work_message != null)
            body(work.action_obj_work_message);
    }

    private static void gmStartDemo2DActionCreate(GMS_START_DEMO_WORK work)
    {
        var start_demo_data = gmStartDemoDataGetInfo();
        var main_sys_info = GsGetMainSysInfo();
        var lang = gmStartDemoLanguage();

        for (int i = 0; i < CMN_NUM; i++)
        {
            var data_type = (int)gm_start_demo_data_type_cmn[i];

            work.action_obj_work_cmn[i] = gmStartDemo2DActionCreate(
                gm_start_demo_action_name_cmn[i],
                start_demo_data.aos_texture[data_type],
                gm_start_demo_data_ama_id[lang][data_type],
                gm_start_demo_action_id_cmn[i],
                gm_start_demo_action_node_flag_cmn[i]);
        }

        var zone_type = g_gm_gamedat_zone_type_tbl[main_sys_info.stage_id];

        for (int i = 0; i < ZONE_NUM; i++)
        {
            var data_type = (int)gm_start_demo_data_type_zone[i];

            work.action_obj_work_zone[i] = gmStartDemo2DActionCreate(
                gm_start_demo_action_name_zone[i],
                start_demo_data.aos_texture[data_type],
                gm_start_demo_data_ama_id[lang][data_type],
                gm_start_demo_action_id_zone[zone_type][i],
                gm_start_demo_action_node_flag_zone[i]);
        }

        var act_no = gm_start_demo_act_no[main_sys_info.stage_id];

        // the final zone is one long boss act, so it has no act digit and no "ACT" word
        if (zone_type != GSD_MAIN_ZONE_TYPE_FINAL)
        {
            for (int i = 0; i < ACT_NUM; i++)
            {
                var data_type = (int)gm_start_demo_data_type_act[i];

                work.action_obj_work_act[i] = gmStartDemo2DActionCreate(
                    gm_start_demo_action_name_act[i],
                    start_demo_data.aos_texture[data_type],
                    gm_start_demo_data_ama_id[lang][data_type],
                    gm_start_demo_action_id_act[act_no][i],
                    gm_start_demo_action_node_flag_act[i]);
            }
        }

        {
            var data_type = (int)GMD_START_DEMO_DATA_TYPE_LANG;

            work.action_obj_work_message = gmStartDemo2DActionCreate(
                gm_start_demo_action_name_message,
                start_demo_data.aos_texture[data_type],
                gm_start_demo_data_ama_id[lang][data_type],
                gm_start_demo_action_id_message[zone_type][act_no],
                true);
        }

        gmStartDemoEachAction(work, action => action.obj_2d.speed = 0.0f);
    }

    private static GMS_COCKPIT_2D_WORK gmStartDemo2DActionCreate(
        string tcb_name,
        AOS_TEXTURE aos_texture,
        int ama_id,
        int action_id,
        bool node_flag)
    {
        if (action_id == GMD_START_DEMO_INVLID_ID)
            return null;

        var node = node_flag ? 1 : 0;

        var obj_work = (GMS_COCKPIT_2D_WORK)GmCockpit.GMM_COCKPIT_CREATE_WORK(
            () => new GMS_COCKPIT_2D_WORK(), null, 0, tcb_name);

        obj_work.cpit_com.obj_work.disp_flag |= (uint)node;

        ObjObjectAction2dAMALoadSetTexlist(
            obj_work.cpit_com.obj_work,
            obj_work.obj_2d,
            null,
            null,
            ama_id,
            GmGameDatGetCockpitData(),
            AoTexGetTexList(aos_texture),
            (uint)action_id,
            node);

        if (!gm_start_demo_is_console)
            ((OBS_OBJECT_WORK)obj_work).pos.z -= FX_F32_TO_FX32(GMD_START_DEMO_MOBILE_POS_Z);

        return obj_work;
    }

    private static void gmStartDemoProcFade(GMS_START_DEMO_WORK work)
    {
        if (!IzFadeIsEnd())
            return;

        IzFadeExit();

        work.update = gmStartDemoProcIn;
        work.counter = 0U;

        gmStartDemoEachAction(work, action =>
        {
            action.obj_2d.speed = 1.0f;
            action.obj_2d.frame = 0.0f;
        });
    }

    private static void gmStartDemoProcIn(GMS_START_DEMO_WORK work)
    {
        if (work.counter < GMD_START_DEMO_COUNTER_IN_END)
            return;

        work.update = gmStartDemoProcWait;

        gmStartDemoEachAction(work, action =>
        {
            action.obj_2d.speed = 0.0f;
            action.obj_2d.frame = GMD_START_DEMO_FRAME_HELD;
        });
    }

    private static void gmStartDemoProcWait(GMS_START_DEMO_WORK work)
    {
        if (work.counter < GMD_START_DEMO_COUNTER_WAIT_END)
            return;

        work.update = gmStartDemoProcOut;

        if (GmStartMsgIsExe())
        {
            GmStartMsgInit();
        }
        else
        {
            GmPlySeqChangeSequence(g_gm_main_system.ply_work[GSD_MAIN_PLAYER_1P], GME_PLY_SEQ_STATE_FW);
            gmStartDemoSetGameFlag(GMD_GAME_FLAG_COUNT_GAME_TIME);
        }

        gmStartDemoEachAction(work, action => action.obj_2d.speed = 1.0f);
    }

    private static void gmStartDemoProcOut(GMS_START_DEMO_WORK work)
    {
        if (work.counter <= GMD_START_DEMO_COUNTER_OUT_END)
            return;

        work.update = gmStartDemoProcEnd;
    }

    private static void gmStartDemoProcEnd(GMS_START_DEMO_WORK work)
    {
        work.update = null;
        gmStartDemoRequestExit();

        gmStartDemoEachAction(
            work, action => action.cpit_com.obj_work.flag |= OBD_OBJECT_TASKCLEAR_REQUEST);
    }
}
