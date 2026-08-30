using gs.backup;

using static AppMain;

internal static partial class GmClearDemo
{
    private static void gmClearDemoSetPlayGameScore(GMS_CLRDM_MAIN_WORK main_work)
    {
        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        mainSysInfo.clear_score = gmClearDemoGetScore();
        mainSysInfo.clear_time = (int) gmClearDemoGetGameTime();
        mainSysInfo.clear_ring = (uint) gmClearDemoGetRingNum();
        main_work.time_score[0] = 0U;
        main_work.time_score[1] = gmClearDemoGetTimeScore(mainSysInfo.clear_time);
        main_work.ring_score[0] = 0U;
        main_work.ring_score[1] = mainSysInfo.clear_ring * 100U;
        if (main_work.ring_score[1] > 99900U)
            main_work.ring_score[1] = 99900U;
        main_work.total_score[1] = mainSysInfo.clear_score;
        main_work.total_score[0] = main_work.total_score[1] + main_work.ring_score[1] + main_work.time_score[1];
        if (main_work.total_score[0] <= 999999990U)
            return;
        main_work.total_score[0] = 999999990U;
    }

    private static void gmClearDemoSetSaveScoreData(GMS_CLRDM_MAIN_WORK main_work)
    {
        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        uint high_score = main_work.total_score[0] < 10U ? 0U : main_work.total_score[0];
        SSystem instance1 = SSystem.CreateInstance();
        SStage stage = SStage.CreateInstance();
        SSpecial special = SSpecial.CreateInstance();
        if (main_work.stage_id >= 21)
        {
            uint num = main_work.stage_id - 21U;
            GMM_MAIN_GOAL_AS_SUPER_SONIC();
            if (mainSysInfo.game_mode == 0)
            {
                if (((int) g_gm_main_system.game_flag & 65536) != 0 && !main_work.is_first_spe_clear)
                    GmPlayerStockGet(g_gm_main_system.ply_work[0], 1);
                bool is_uploaded = /*LiveFeature.getInstance().saveLeaderBoardScore(main_work.stage_id - 4, (int) high_score)*/ false;
                special[(int) num].SetHighScoreUploaded(is_uploaded);
                if (is_uploaded && !special[(int) num].IsScoreUploadedOnce())
                    special[(int) num].SetScoreUploadedOnce(true);
            }
            else if (mainSysInfo.game_mode == 1)
            {
                if (((int) g_gm_main_system.game_flag & 65536) != 0 && !main_work.is_first_spe_clear)
                    GmPlayerStockGet(g_gm_main_system.ply_work[0], 1);
                bool is_uploaded = false;
                if (((int) g_gm_main_system.game_flag & 65536) != 0)
                {
                    is_uploaded = /*LiveFeature.getInstance().saveLeaderBoardScore(main_work.stage_id - 4 + 24, mainSysInfo.clear_time)*/ false;
                    special[(int) num].SetFastTimeUploaded(is_uploaded);
                }

                if (is_uploaded && !special[(int) num].IsTimeUploadedOnce())
                    special[(int) num].SetTimeUploadedOnce(true);
            }

            if ((special[(int) num].GetHighScore() < high_score || special[(int) num].GetHighScore() == 0) && ((int) g_gm_main_system.game_flag & 65536) != 0)
            {
                special[(int) num].SetHighScore(high_score);
                main_work.flag |= 32768U;
            }

            if ((special[(int) num].GetFastTime() < (uint) mainSysInfo.clear_time || special[(int) num].GetFastTime() == 0) && ((int) g_gm_main_system.game_flag & 65536) != 0)
            {
                special[(int) num].SetFastTime((uint) mainSysInfo.clear_time);
                main_work.flag |= 4096U;
            }
        }
        else
        {
            uint num = main_work.stage_id;
            if (num > 16U && num <= 20U)
                num = 16U;
            bool flag = GMM_MAIN_USE_SUPER_SONIC();

            if (GMM_MAIN_GOAL_AS_SUPER_SONIC())
                stage[(int) num].SetUseSuperSonicOnce(true);
            if (mainSysInfo.game_mode == 0)
            {
                high_score = main_work.total_score[0] < 10U ? 0U : main_work.total_score[0];
                g_gs_main_sys_info.is_first_play = stage[(int) num].GetHighScore(false) == 0;
                bool is_uploaded = /*LiveFeature.getInstance().saveLeaderBoardScore((int) num, (int) high_score)*/ false;
                stage[(int) num].SetHighScoreUploaded(flag, is_uploaded);
                if (is_uploaded && !stage[(int) num].IsScoreUploadedOnce())
                    stage[(int) num].SetScoreUploadedOnce(true);
                g_gs_main_sys_info.prev_stage_id = ((int) g_gm_main_system.game_flag & 16384) == 0 ? ushort.MaxValue : g_gs_main_sys_info.stage_id;
            }
            else if (mainSysInfo.game_mode == 1 && (stage[(int) num].GetFastTime(flag) > (uint) mainSysInfo.clear_time || stage[(int) num].GetFastTime(flag) == 0))
            {
                bool is_uploaded = /*LiveFeature.getInstance().saveLeaderBoardScore((int) num + 24, mainSysInfo.clear_time)*/ false;
                stage[(int) num].SetFastTimeUploaded(flag, is_uploaded);
                if (is_uploaded && !stage[(int) num].IsTimeUploadedOnce())
                    stage[(int) num].SetTimeUploadedOnce(true);
            }

            if (flag)
            {
                if (stage[(int) num].GetFastTime(false) > (uint) mainSysInfo.clear_time || stage[(int) num].GetFastTime(false) == 0)
                    main_work.flag |= 4096U;
            }
            else if (stage[(int) num].GetFastTime(true) > (uint) mainSysInfo.clear_time || stage[(int) num].GetFastTime(true) == 0)
                main_work.flag |= 4096U;

            stage[(int) num].SetFastTime((uint) mainSysInfo.clear_time, flag);

            if (stage[(int) num].GetHighScore(flag) < high_score || stage[(int) num].GetHighScore(flag) == 0)
                stage[(int) num].SetHighScore(high_score, flag);
        }


        instance1.Killed = mainSysInfo.ene_kill_count;
        instance1.PlayerStock = mainSysInfo.rest_player_num;
        if (mainSysInfo.stage_id >= 16 && mainSysInfo.stage_id <= 20)
        {
            HgTrophyIncFinalClearCount();
            instance1.ClearCount = mainSysInfo.final_clear_count;
        }

        HgTrophyTryAcquisition(4);
    }

    private static void gmClearDemoSetClearTimeRecord(GMS_CLRDM_MAIN_WORK main_work)
    {
        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        main_work.clear_time = (uint) mainSysInfo.clear_time;
        AkUtilFrame60ToTime(main_work.clear_time, ref main_work.time_min, ref main_work.time_sec, ref main_work.time_msec);
        main_work.record_time_num_act[0].obj_2d.speed = 0.0f;
        main_work.record_time_num_act[2].obj_2d.speed = 0.0f;
        main_work.record_time_num_act[3].obj_2d.speed = 0.0f;
        main_work.record_time_num_act[5].obj_2d.speed = 0.0f;
        main_work.record_time_num_act[6].obj_2d.speed = 0.0f;
        main_work.record_time_num_act[1].obj_2d.speed = 0.0f;
        main_work.record_time_num_act[4].obj_2d.speed = 0.0f;
        for (int index = 0; index < 7; ++index)
            ((OBS_OBJECT_WORK) main_work.record_time_num_act[index]).disp_flag &= 4294967263U;
    }

    private static void gmClearDemoSetSpecialStageScore(GMS_CLRDM_MAIN_WORK main_work)
    {
        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        mainSysInfo.clear_score = gmClearDemoGetScore();
        mainSysInfo.clear_time = (int) gmClearDemoGetGameTime();
        mainSysInfo.clear_ring = (uint) gmClearDemoGetRingNum();
        main_work.time_score[0] = 0U;
        main_work.time_score[1] = gmClearDemoGetTimeSpeScore(mainSysInfo.clear_time);
        main_work.ring_score[0] = 0U;
        main_work.ring_score[1] = mainSysInfo.clear_ring * 100U;
        if (main_work.ring_score[1] > 99900U)
            main_work.ring_score[1] = 99900U;
        main_work.total_score[1] = mainSysInfo.clear_score;
        main_work.total_score[0] = main_work.total_score[1] + main_work.ring_score[1] + main_work.time_score[1];
        if (main_work.total_score[0] <= 999999990U)
            return;
        main_work.total_score[0] = 999999990U;
    }

    private static void gmClearDemoSetSpecialStageClearInfo(GMS_CLRDM_MAIN_WORK main_work)
    {
        ushort num1 = g_gs_main_sys_info.prev_stage_id == ushort.MaxValue ? ushort.MaxValue : g_gs_main_sys_info.prev_stage_id;
        SSpecial instance = SSpecial.CreateInstance();
        uint num2 = main_work.stage_id - 21U;
        if (((int) g_gs_main_sys_info.game_flag & 32) != 0)
            main_work.is_full_eme = true;
        if (((int) g_gm_main_system.game_flag & 65536) != 0)
        {
            main_work.is_get_eme = true;
            if (!instance[(int) num2].IsGetEmerald() && num1 != ushort.MaxValue)
            {
                instance[(int) num2].SetEmeraldStage((EEmeraldStage.Type) dm_clrdm_spe_stg_get_act_no[num1]);
                main_work.is_first_spe_clear = true;
            }

            main_work.get_eme_no = (int) num2;
            if (g_gs_main_sys_info.stage_id == 27)
                g_gs_main_sys_info.game_flag |= 32U;
            main_work.has_eme_num = (int) num2 + 1;
            for (int index = 7; index > 0; --index)
            {
                if (instance[index - 1].IsGetEmerald())
                {
                    main_work.has_eme_num = index;
                    break;
                }
            }
        }
        else
        {
            main_work.is_get_eme = false;
            main_work.get_eme_no = -1;
            for (int index = 7; index > 0; --index)
            {
                if (instance[index - 1].IsGetEmerald())
                {
                    main_work.has_eme_num = index;
                    break;
                }
            }
        }
    }

    private static uint gmClearDemoGetTimeScore(int clear_time)
    {
        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        uint num1 = 0;
        ushort min = 0;
        ushort sec = 0;
        ushort msec = 0;
        AkUtilFrame60ToTime((uint) clear_time, ref min, ref sec, ref msec);
        int num2 = min * 60 + sec;
        for (int index = 0; index < 9; ++index)
        {
            if (num2 < dm_clrdm_clear_time_sec_tbl[mainSysInfo.stage_id][index])
            {
                num1 = dm_clrdm_clear_time_score_tbl[index];
                break;
            }
        }

        if (((int) g_gs_main_sys_info.game_flag & 8) != 0)
            num1 = 0U;
        return num1;
    }

    private static uint gmClearDemoGetTimeSpeScore(int clear_time)
    {
        ushort min = 0;
        ushort sec = 0;
        ushort msec = 0;
        AkUtilFrame60ToTime((uint) clear_time, ref min, ref sec, ref msec);
        uint num = (uint) ((min * 60 + sec) * 100);
        if (((int) g_gm_main_system.game_flag & 131072) != 0)
            num = 0U;
        if (((int) g_gm_main_system.game_flag & 262144) != 0)
            num = 0U;
        return num;
    }

    private static void gmClearDemoSetCalcScore(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (main_work.ring_score[1] <= 0U && main_work.time_score[1] <= 0U)
        {
            main_work.proc_calc_score = null;
            amFlagOn(ref main_work.flag, 8U);
        }
        else
        {
            if (main_work.time_score[1] > 0U)
            {
                main_work.time_score[1] -= 100U;
                main_work.total_score[1] += 100U;
            }

            if (main_work.ring_score[1] <= 0U)
                return;
            main_work.ring_score[1] -= 100U;
            main_work.total_score[1] += 100U;
        }
    }

    private static void gmClearDemoSetScoreData(GMS_CLRDM_MAIN_WORK main_work)
    {
        gmClearDemoSetDispScore(main_work.time_num_act, main_work.time_score[1], 5U);
        gmClearDemoSetDispScore(main_work.ring_num_act, main_work.ring_score[1], 5U);
        gmClearDemoSetDispScore(main_work.total_num_act, main_work.total_score[1], 9U);
    }

    private static void gmClearDemoSetDispScore(
        GMS_COCKPIT_2D_WORK[] score_act,
        uint score,
        uint digits)
    {
        int num1 = 1;
        for (int index = 0; index < 1; ++index)
        {
            score_act[digits - 1U - index].obj_2d.frame = 0.0f;
            score_act[digits - 1U - index].obj_2d.speed = 0.0f;
        }

        for (int index1 = 1; index1 < (int) digits; ++index1)
        {
            for (int index2 = 0; index2 < index1; ++index2)
                num1 *= 10;
            if (score >= (uint) num1)
            {
                float num2 = (int) (float) (score / num1) % 10;
                score_act[digits - 1U - index1].obj_2d.frame = num2;
                score_act[digits - 1U - index1].obj_2d.speed = 0.0f;
            }
            else
            {
                score_act[digits - 1U - index1].obj_2d.frame = 0.0f;
                score_act[digits - 1U - index1].obj_2d.speed = 0.0f;
            }

            num1 = 1;
        }
    }
}
