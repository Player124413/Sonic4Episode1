using static AppMain;

// the results screen; G_RSLT is members 30-43 of CPIT_MAIN, and its action ids are a strict prefix
// of the mobile ones: 0-99 identical, iPhone appends 100-105 for touch buttons.
internal static partial class GmClearDemo
{
    // the G_RSLT_<lang> member is the only one that can sit past the end of the archive; the shared
    // G_RSLT pair is hardcoded 30/31 at every call site
    private static int gmClearDemoLanguage()
    {
        return GmGameDatCockpitLanguage(g_gm_clear_demo_data_amb_id);
    }

    public static bool GmClearDemoBuildCheck()
    {
        return gmClearDemoIsTexLoad();
    }

    public static void GmClearDemoBuild()
    {
        int language = GsEnvGetLanguage();
        int num = language < 1 ? 0 : 2 * language;
        gm_clrdm_mgr.Clear();
        gm_clrdm_mgr_p = gm_clrdm_mgr;
        for (int index = 0; index < 2; ++index)
            gm_clrdm_tex[index].Clear();
        gm_clrdm_amb[0] = readAMBFile(ObjDataLoadAmbIndex(null, 31, g_gm_gamedat_cockpit_main_arc));
        gm_clrdm_amb[1] = readAMBFile(ObjDataLoadAmbIndex(null, g_gm_clear_demo_data_amb_id[gmClearDemoLanguage()], g_gm_gamedat_cockpit_main_arc));
        for (int index = 0; index < 2; ++index)
        {
            AoTexBuild(gm_clrdm_tex[index], gm_clrdm_amb[index]);
            AoTexLoad(gm_clrdm_tex[index]);
        }
    }

    public static void GmClearDemoFlush()
    {
        for (int index = 0; index < 2; ++index)
            AoTexRelease(gm_clrdm_tex[index]);
    }

    public static bool GmClearDemoFlushCheck()
    {
        return gmClearDemoIsTexRelease();
    }

    public static void GmClearDemoStart()
    {
        gmClearDemoInit();
    }

    public static void GmClearDemoExit()
    {
        if (gm_clrdm_mgr_p.tcb == null)
            return;
        mtTaskClearTcb(gm_clrdm_mgr_p.tcb);
        gm_clrdm_mgr_p.tcb = null;
    }

    public static bool GmClearDemoIsExit()
    {
        return gm_clrdm_mgr_p.tcb == null;
    }

    public static void GmClearDemoRetryStart()
    {
        gmClearDemoRetryInit();
    }

    private static void gmClearDemoInit()
    {
        gm_clrdm_variant = gmClearDemoIsConsoleDataComplete()
            ? gm_clrdm_variant_console
            : gm_clrdm_variant_mobile;
        gm_clrdm_has_next_prompt = gmClearDemoIsNextPromptDataComplete();

        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        MTS_TASK_TCB mtsTaskTcb = MTM_TASK_MAKE_TCB(new GSF_TASK_PROCEDURE(gmClearDemoProcMain), new GSF_TASK_PROCEDURE(gmClearDemoDest), 0U, 0, 18448U, 5, () => new GMS_CLRDM_MAIN_WORK(), "CLRDM_MAIN");
        gm_clrdm_mgr_p.tcb = mtsTaskTcb;
        GMS_CLRDM_MAIN_WORK work = (GMS_CLRDM_MAIN_WORK) mtsTaskTcb.work;
        for (int index = 0; index < 2; ++index)
            work.tex[index] = gm_clrdm_tex[index];
        work.trg_retry.Constructor();
        work.trg_back.Constructor();
        work.count = 1;
        work.stage_id = mainSysInfo.stage_id;
        work.game_mode = mainSysInfo.game_mode;
        if (work.stage_id <= 20)
        {
            work.is_clear_spe_stg = false;
            gmClearDemoSetPlayGameScore(work);
        }
        else
        {
            work.is_clear_spe_stg = true;
            gmClearDemoSetSpecialStageScore(work);
            gmClearDemoSetSpecialStageClearInfo(work);
        }

        gmClearDemoSetSaveScoreData(work);
        if (work.stage_id >= 21 && work.stage_id < 29)
        {
            if (work.game_mode == 0)
            {
                gmClearDemoCreateObjSpeScore(work);
                gmClearDemoCreateObjSpecialScoreAtk(work);
                gmClearDemoSetInitDispAct(work);
            }
            else
            {
                gmClearDemoCreateObjSpeTime(work);
                gmClearDemoCreateObjSpecialTimeAtk(work);
            }
        }
        else if (work.game_mode == 0)
        {
            gmClearDemoCreateObjActScore(work);
            gmClearDemoCreateObjAct(work);
            gmClearDemoSetInitDispAct(work);
        }
        else
        {
            gmClearDemoCreateObjActTime(work);
            gmClearDemoCreateObjNormalTimeAtk(work);
        }

        if (mainSysInfo.stage_id >= 16 && mainSysInfo.stage_id <= 20)
            GmSoundPlayClearFinal();
        else
            GmSoundPlayClear();
        gmClearDemoSetMainUpdateProc(work);
    }

    private static void gmClearDemoRetryInit()
    {
        gm_clrdm_variant = gmClearDemoIsConsoleDataComplete()
            ? gm_clrdm_variant_console
            : gm_clrdm_variant_mobile;
        gm_clrdm_has_next_prompt = false;

        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        MTS_TASK_TCB mtsTaskTcb = MTM_TASK_MAKE_TCB(new GSF_TASK_PROCEDURE(gmClearDemoProcMain), new GSF_TASK_PROCEDURE(gmClearDemoDest), 0U, 0, 18448U, 5, () => new GMS_CLRDM_MAIN_WORK(), "CLRDM_MAIN");
        gm_clrdm_mgr_p.tcb = mtsTaskTcb;
        GMS_CLRDM_MAIN_WORK work = (GMS_CLRDM_MAIN_WORK) mtsTaskTcb.work;
        for (int index = 0; index < 2; ++index)
            work.tex[index] = gm_clrdm_tex[index];
        work.trg_retry.Constructor();
        work.trg_back.Constructor();
        work.count = 1;
        work.stage_id = mainSysInfo.stage_id;
        work.is_clear_spe_stg = work.stage_id >= 21;
        work.game_mode = mainSysInfo.game_mode;
        gmClearDemoCreateObjActTime(work);
        gmClearDemoCreateObjNormalTimeAtk(work);
        gmClearDemoSetRetryInitAct(work);
        work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcRetryStart);
    }

    private static void gmClearDemoSetMainUpdateProc(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (main_work.stage_id >= 21)
        {
            if (main_work.game_mode == 0)
            {
                main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeScoreMoveEfct);
            }
            else
            {
                main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeTimeMoveEfct);
                gmClearDemoSetClearTimeRecord(main_work);
                gmClearDemoSetSortBufTimeAct(main_work);
                main_work.tex_new_record_act.obj_2d.speed = 0.0f;
            }
        }
        else if (main_work.game_mode == 0)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcMoveEfct);
        }
        else
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcTimeMoveEfct);
            gmClearDemoSetClearTimeRecord(main_work);
            gmClearDemoSetSortBufTimeAct(main_work);
            main_work.tex_new_record_act.obj_2d.speed = 0.0f;
        }
    }

    private static void gmClearDemoProcMain(MTS_TASK_TCB tcb)
    {
        GMS_CLRDM_MAIN_WORK work = (GMS_CLRDM_MAIN_WORK) tcb.work;
        if (((int) work.flag & 1) != 0)
        {
            if (gm_clrdm_mgr_p.tcb != null)
            {
                mtTaskClearTcb(gm_clrdm_mgr_p.tcb);
                gm_clrdm_mgr_p.tcb = null;
            }

            if (work.stage_id < 21)
            {
                if (((int) g_gm_main_system.game_flag & 16384) != 0)
                {
                    g_gs_main_sys_info.prev_stage_id = g_gs_main_sys_info.stage_id;
                    DmCmnBackupSave(false, false, false);
                }
                else
                    g_gs_main_sys_info.prev_stage_id = ushort.MaxValue;
            }

            // the NEXT prompt skips the map, where a clear would normally reach the disk; this is
            // the actual save path when skipping ahead, since DmSaveMenuStart no-ops during the
            // main game
            if ((g_gm_main_system.game_flag & GMD_GAME_FLAG_NEXT_STAGE) != 0)
                DmCmnBackupSave(false, false, false);

            if (((int) work.flag & 16384) != 0)
                work.flag &= 4294950911U;
            g_gm_main_system.game_flag |= 16U;
            if (((int) work.flag & 8192) == 0)
                return;
            g_gm_main_system.game_flag |= 2U;
            work.flag &= 4294959103U;
        }
        else
        {
            if (work.proc_update == null)
                return;
            work.proc_update(work);
        }
    }

    private static void gmClearDemoDest(MTS_TASK_TCB tcb)
    {
    }

    private static bool gmClearDemoIsTexLoad()
    {
        int num = 0;
        for (int index = 0; index < 2; ++index)
        {
            if (AoTexIsLoaded(gm_clrdm_tex[index]))
                num |= 1 << index;
        }

        return num == 3;
    }

    private static bool gmClearDemoIsTexRelease()
    {
        int num = 0;
        for (int index = 0; index < 2; ++index)
        {
            if (AoTexIsReleased(gm_clrdm_tex[index]))
                num |= 1 << index;
        }

        return num == 3;
    }

    private static short gmClearDemoGetRingNum()
    {
        return g_gm_main_system.ply_work[0] != null ? g_gm_main_system.ply_work[0].ring_num : (short) 0;
    }

    private static uint gmClearDemoGetScore()
    {
        return g_gm_main_system.ply_work[0] != null ? g_gm_main_system.ply_work[0].score : 0U;
    }

    private static uint gmClearDemoGetGameTime()
    {
        if (g_gs_main_sys_info.stage_id >= 21 && ((int) g_gm_main_system.game_flag & 65536) == 0)
            return 0;
        return g_gm_main_system.game_time >= 35999U ? 35999U : g_gm_main_system.game_time;
    }

    private static uint gmClearDemoGetChallengeNum()
    {
        return g_gm_main_system.player_rest_num[0];
    }

    private static void gmClearDemoSetBgColorBlack()
    {
        gmClearDemoSetBgColorBlack(null);
    }

    private static void gmClearDemoSetBgColorBlack(AMS_TCB tcb)
    {
        if (tcb != null)
            amDrawSetBGColor(new NNS_RGBA_U8(0, 0, 0, byte.MaxValue));
        else
            amDrawMakeTask(new TaskProc(gmClearDemoSetBgColorBlack), 65280);
    }
}
