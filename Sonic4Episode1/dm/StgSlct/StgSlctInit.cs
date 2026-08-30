using gs.backup;

using static AppMain;
using static AppMain.GSE_EVT_ID;

using static StgSlct.DME_STGSLCT_ACT_PAGE;
using static StgSlct.DME_STGSLCT_STAGE;
using static StgSlct.DME_STGSLCT_WIN;
using static StgSlct.DME_STGSLCT_ZONE_TYPE;

internal static partial class StgSlct
{
    private static void dmStgSlctSetInitData(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var tmp_prev_evt = SyGetEvtInfo().old_evt_id;

        if (tmp_prev_evt == (short)GSD_EVT_ID_MAINGAME
            || tmp_prev_evt == (short)GSD_EVT_ID_SPSTAGE_BRANCH)
        {
            dm_stgslct_is_stage_start = true;
            main_work.cur_game_mode = (uint)g_gs_main_sys_info.game_mode;
        }
        else if (tmp_prev_evt == (short)GSD_EVT_ID_RANKING)
        {
            dm_stgslct_is_stage_start = true;
            main_work.cur_game_mode = (uint)g_gs_main_sys_info.game_mode;
            main_work.flag |= DMD_STGSLCT_FLAG_PREV_EVT_RANKING;
        }
        else
        {
            dm_stgslct_is_stage_start = false;
            main_work.cur_game_mode = 0;
        }

        main_work.bg_fade.r = 255;
        main_work.bg_fade.g = 255;
        main_work.bg_fade.b = 255;
        main_work.bg_fade.a = 0;

        if (dm_stgslct_is_stage_start)
            dmStgSlctSetInitFocus(main_work);

        main_work.btn_l_disp_frm = DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM;
        main_work.btn_r_disp_frm = DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM;

        main_work.cur_bg_id = main_work.cur_zone;

        main_work.obi_pos[0] = 0.0f;
        main_work.obi_pos[1] = DMD_STGSLCT_OBI_MOVE_START_POS;
    }

    private static void dmStgSlctSetInitFocus(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.chng_zone = (uint)DME_STGSLCT_ACT_PAGE_NONE;
        main_work.focus_disp_no = 0;

        if (g_gs_main_sys_info.stage_id < GSD_MAIN_STAGE_ID_FINAL_1)
        {
            main_work.cur_stage = dmStgSlctSetNextFocusAct(main_work, g_gs_main_sys_info.stage_id);
            main_work.cur_zone = dm_stgslct_act_zone_no_tbl[main_work.cur_stage];
            main_work.crsr_idx = main_work.cur_stage % 4;
        }
        else if (g_gs_main_sys_info.stage_id >= GSD_MAIN_STAGE_ID_ENDING)
        {
            MTM_ASSERT(0);

            main_work.cur_stage = 0;
            main_work.cur_zone = 0;
            main_work.crsr_idx = 0;
        }
        else if (g_gs_main_sys_info.stage_id >= GSD_MAIN_STAGE_ID_SS1)
        {
            if (g_gs_main_sys_info.prev_stage_id != 0xffff)
            {
                main_work.cur_stage = dmStgSlctSetNextFocusAct(main_work, g_gs_main_sys_info.prev_stage_id);
                main_work.cur_zone = dm_stgslct_act_zone_no_tbl[main_work.cur_stage];
                main_work.crsr_idx = main_work.cur_stage % 4;
            }
            else
            {
                main_work.cur_stage = (int)(dm_stgslct_zone_act_num_tbl[(int)DME_STGSLCT_ACT_PAGE_SPE][0]
                    + (g_gs_main_sys_info.stage_id - GSD_MAIN_STAGE_ID_SS1));
                main_work.cur_zone = (uint)DME_STGSLCT_ZONE_TYPE_SPE;

                if (main_work.cur_stage <= (int)DME_STGSLCT_STAGE_S_4)
                {
                    main_work.focus_disp_no = main_work.cur_stage - (int)DME_STGSLCT_STAGE_S_1;
                    main_work.crsr_idx = 0;
                }
                else
                {
                    main_work.focus_disp_no = 3;
                    main_work.crsr_idx = main_work.cur_stage - (int)DME_STGSLCT_STAGE_S_4;
                }
            }
        }
        else
        {
            main_work.cur_stage = (int)DME_STGSLCT_STAGE_F_1;
            main_work.cur_zone = (uint)DME_STGSLCT_ZONE_TYPE_FINAL;
            main_work.crsr_idx = 0;
        }

        main_work.crsr_pos_y = main_work.cur_zone != (uint)DME_STGSLCT_ACT_PAGE_FINAL
            ? dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_idx]
            : 320.0f;
    }

    private static void dmStgSlctSetHiScore(DMS_STGSLCT_MAIN_WORK main_work)
    {
    }

    private static void dmStgSlctSetClearInfo(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var data = SStage.CreateInstance();

        for (int i = 0; i <= GSD_MAIN_STAGE_ID_FINAL_1; i++)
        {
            main_work.n_sonic_hi_score[i] = (int)data[i].GetHighScore(false);
            main_work.s_sonic_hi_score[i] = (int)data[i].GetHighScore(true);

            main_work.hi_score[i] = dmStgSlctPickBest(
                main_work.n_sonic_hi_score[i], main_work.s_sonic_hi_score[i],
                DMD_STGSLCT_INIT_SCORE_NUM, higher_is_better: true);

            if (main_work.hi_score[i] > DMD_STGSLCT_INIT_SCORE_NUM)
                main_work.hi_score[i] = DMD_STGSLCT_INIT_SCORE_NUM - 1;

            main_work.n_sonic_record_time[i] = (int)data[i].GetFastTime(false);
            main_work.s_sonic_record_time[i] = (int)data[i].GetFastTime(true);

            main_work.record_time[i] = dmStgSlctPickBest(
                main_work.n_sonic_record_time[i], main_work.s_sonic_record_time[i],
                DMD_STGSLCT_INIT_RECORD_TIME_NUM, higher_is_better: false);

            if (main_work.record_time[i] > 36000)
                main_work.record_time[i] = 35999;
        }

        var spe_data = SSpecial.CreateInstance();

        for (int i = 0; i < 7; i++)
        {
            var stage = i + (int)DME_STGSLCT_STAGE_S_1;

            main_work.hi_score[stage] = (int)spe_data[i].GetHighScore();

            if (main_work.hi_score[stage] > DMD_STGSLCT_INIT_SCORE_NUM)
                main_work.hi_score[stage] = DMD_STGSLCT_INIT_SCORE_NUM - 1;

            main_work.record_time[stage] = (int)spe_data[i].GetFastTime();

            if (main_work.record_time[i] > DMD_STGSLCT_INIT_RECORD_TIME_NUM)
                main_work.record_time[i] = DMD_STGSLCT_INIT_RECORD_TIME_NUM - 1;
        }

        for (int i = 0; i < (int)DME_STGSLCT_STAGE_NUM; i++)
        {
            main_work.is_clear_stage[i] =
                main_work.hi_score[i] == DMD_STGSLCT_INIT_SCORE_NUM
                && main_work.record_time[i] == DMD_STGSLCT_INIT_RECORD_TIME_NUM
                    ? 0
                    : 1;
        }

        dmStgSlctSetStageAvailability(main_work);

        if (main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_1_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_2_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_3_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_4_B] == 1)
        {
            main_work.is_final_open |= DMD_STGSLCT_ZONE_FINAL_OPEN;
        }

        main_work.player_stock = g_gs_main_sys_info.rest_player_num;

        for (int i = 0; i < 7; i++)
        {
            if (spe_data[i].IsGetEmerald())
            {
                main_work.is_final_open |= DMD_STGSLCT_ZONE_SPECIAL_OPEN;
                main_work.get_emerald |= 1u << i;
                main_work.eme_stage_no[i] = dm_stgslct_eme_get_act_no_tbl[(int)spe_data[i].GetEmeraldStage()];
            }
            else
            {
                main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_S_1 + i] = -1;
            }
        }
    }

    private static int dmStgSlctPickBest(int normal, int super, int unset, bool higher_is_better)
    {
        if (normal == unset)
            return super;

        if (super == unset)
            return normal;

        var normal_wins = higher_is_better ? normal >= super : normal <= super;

        return normal_wins ? normal : super;
    }

    private static void dmStgSlctSetStageAvailability(DMS_STGSLCT_MAIN_WORK main_work)
    {
        uint no_active = 0;

        for (int i = 0; i < (int)DME_STGSLCT_STAGE_NUM; i++)
        {
            if (i > (int)DME_STGSLCT_STAGE_F_1)
            {
                if (main_work.is_clear_stage[i] == 0)
                    main_work.is_clear_stage[i] = -1;
            }

            if (i == 17)
            {
                main_work.is_clear_stage[i] = 1;
            }
            else if (i == (int)DME_STGSLCT_STAGE_F_1)
            {
                for (int j = (int)DME_STGSLCT_STAGE_1_B; j < (int)DME_STGSLCT_STAGE_F_1; j += 4)
                {
                    if (main_work.is_clear_stage[j] != 1)
                        no_active = 1;
                }

                if (no_active != 0)
                    main_work.is_clear_stage[i] = -1;
            }
            else if (i < (int)DME_STGSLCT_STAGE_F_1 && (i + 1) % 4 == 0)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (main_work.is_clear_stage[i - 3 + j] == 0)
                        no_active = 1;
                }

                if (no_active != 0)
                    main_work.is_clear_stage[i] = -1;
            }

            no_active = 0;
        }
    }

    private static void dmStgSlctSetAnnounceMsg(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var sys_data = SSystem.CreateInstance();

        dmStgSlctAnnounce(main_work, sys_data, SSystem.EAnnounce.OpenZoneSelect,
            DME_STGSLCT_WIN_1_1_CLEAR,
            main_work.is_clear_stage[0] == 1);

        dmStgSlctAnnounce(main_work, sys_data, SSystem.EAnnounce.OpenZone1Boss,
            DME_STGSLCT_WIN_BOSS1_CAN_PLAY,
            main_work.is_clear_stage[0] == 1
            && main_work.is_clear_stage[1] == 1
            && main_work.is_clear_stage[2] == 1);

        dmStgSlctAnnounce(main_work, sys_data, SSystem.EAnnounce.OpenZone2Boss,
            DME_STGSLCT_WIN_BOSS2_CAN_PLAY,
            main_work.is_clear_stage[4] == 1
            && main_work.is_clear_stage[5] == 1
            && main_work.is_clear_stage[6] == 1);

        dmStgSlctAnnounce(main_work, sys_data, SSystem.EAnnounce.OpenZone3Boss,
            DME_STGSLCT_WIN_BOSS3_CAN_PLAY,
            main_work.is_clear_stage[8] == 1
            && main_work.is_clear_stage[9] == 1
            && main_work.is_clear_stage[10] == 1);

        dmStgSlctAnnounce(main_work, sys_data, SSystem.EAnnounce.OpenZone4Boss,
            DME_STGSLCT_WIN_BOSS4_CAN_PLAY,
            main_work.is_clear_stage[12] == 1
            && main_work.is_clear_stage[13] == 1
            && main_work.is_clear_stage[14] == 1);

        dmStgSlctAnnounce(main_work, sys_data, SSystem.EAnnounce.OpenFinalZone,
            DME_STGSLCT_WIN_FINAL_CAN_SLCT,
            main_work.is_clear_stage[3] == 1
            && main_work.is_clear_stage[7] == 1
            && main_work.is_clear_stage[11] == 1
            && main_work.is_clear_stage[15] == 1);

        dmStgSlctAnnounce(main_work, sys_data, SSystem.EAnnounce.OpenSuperSonic,
            DME_STGSLCT_WIN_SSONIC_SLCT,
            main_work.is_clear_stage[23] == 1);

        dmStgSlctAnnounce(main_work, sys_data, SSystem.EAnnounce.OpenSpecialStage,
            DME_STGSLCT_WIN_SPESTE_CAN_SLCT,
            (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0);
    }

    private static void dmStgSlctAnnounce(
        DMS_STGSLCT_MAIN_WORK main_work,
        SSystem sys_data,
        SSystem.EAnnounce announce,
        DME_STGSLCT_WIN win,
        bool unlocked)
    {
        if (sys_data.IsAnnounce(announce) || !unlocked)
            return;

        main_work.announce_flag |= 1u << (int)win;
        sys_data.SetAnnounce(announce, true);
    }
}
