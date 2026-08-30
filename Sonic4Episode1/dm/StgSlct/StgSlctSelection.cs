using static AppMain;

using static StgSlct.DME_STGSLCT_ACT_PAGE;
using static StgSlct.DME_STGSLCT_PLAY_MODE;
using static StgSlct.DME_STGSLCT_STAGE;
using static StgSlct.DME_STGSLCT_ZONE_TYPE;

internal static partial class StgSlct
{
    // the prologue three input procs each compute for themselves
    private struct DMS_STGSLCT_BOUNDS
    {
        public int src;
        public int dst;
        public int act_comp;
        public int is_final_open;
        public int is_spe_open;
        public int edge;
    }

    private static DMS_STGSLCT_BOUNDS dmStgSlctGetBounds(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var bounds = default(DMS_STGSLCT_BOUNDS);

        bounds.src = (int)dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        bounds.dst = bounds.src + (int)dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];
        bounds.act_comp = (int)dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] - 1;
        bounds.edge = (int)DME_STGSLCT_ACT_PAGE_4;

        if (main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_1_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_2_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_3_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_4_B] == 1)
        {
            bounds.is_final_open = 1;
            bounds.edge = (int)DME_STGSLCT_ACT_PAGE_FINAL;
        }

        // the else arm makes is_spe_open depend on the last iteration when nothing matches, which
        // is only equivalent to "any" because the loop breaks on the first hit. SEGA's shape, kept.
        for (int i = (int)DME_STGSLCT_STAGE_S_1; i < (int)DME_STGSLCT_STAGE_NUM; i++)
        {
            if (main_work.is_clear_stage[i] != -1)
            {
                bounds.is_spe_open = 1;
                bounds.edge = (int)DME_STGSLCT_ACT_PAGE_SPE;
                break;
            }

            bounds.is_spe_open = 0;
        }

        return bounds;
    }

    private static int dmStgSlctGetRevisedStageVrtclNo(int idx, int diff, int crsr_idx)
    {
        var result = idx + diff;

        if (result < 0)
            result = crsr_idx == 0 ? 3 : 0;

        if (result >= 4)
            result = crsr_idx == 3 ? 0 : 3;

        return result;
    }

    private static int dmStgSlctGetRevisedStageCrsrNo(int idx, int diff, int zone_no, int disp_act)
    {
        var result = idx + diff;

        var tmp_no = (int)dm_stgslct_zone_act_num_tbl[zone_no][1] - 4;

        if (tmp_no < 0)
            tmp_no = 0;

        if (result < 0)
            result = disp_act == 0 ? 3 : 0;

        if (result > 3)
            result = disp_act == tmp_no ? 0 : 3;

        return result;
    }

    private static int dmStgSlctGetRevisedZoneNo(int idx, int diff, int is_final_open, int is_spe_open)
    {
        var result = idx + diff;

        if (is_final_open != 0)
        {
            if (is_spe_open != 0)
            {
                if (result < (int)DME_STGSLCT_ACT_PAGE_1)
                    result = (int)DME_STGSLCT_ACT_PAGE_NUM - 1;

                if (result >= (int)DME_STGSLCT_ACT_PAGE_NUM)
                    result = (int)DME_STGSLCT_ACT_PAGE_1;
            }
            else
            {
                if (result < (int)DME_STGSLCT_ACT_PAGE_1)
                    result = (int)DME_STGSLCT_ACT_PAGE_FINAL;

                if (result > (int)DME_STGSLCT_ACT_PAGE_FINAL)
                    result = (int)DME_STGSLCT_ACT_PAGE_1;
            }

            return result;
        }

        if (is_spe_open != 0)
        {
            // final is closed, so step over its slot rather than landing on it
            if (result == (int)DME_STGSLCT_ACT_PAGE_FINAL)
            {
                result = diff > 0
                    ? (int)DME_STGSLCT_ACT_PAGE_SPE
                    : (int)DME_STGSLCT_ACT_PAGE_4;
            }

            if (result < (int)DME_STGSLCT_ACT_PAGE_1)
                result = (int)DME_STGSLCT_ACT_PAGE_NUM - 1;

            if (result >= (int)DME_STGSLCT_ACT_PAGE_NUM)
                result = (int)DME_STGSLCT_ACT_PAGE_1;

            return result;
        }

        if (result < (int)DME_STGSLCT_ACT_PAGE_1)
            result = (int)DME_STGSLCT_ACT_PAGE_4;

        if (result > (int)DME_STGSLCT_ACT_PAGE_4)
            result = (int)DME_STGSLCT_ACT_PAGE_1;

        return result;
    }

    private static void dmStgSlctSetActChngZonePosInit(DMS_STGSLCT_MAIN_WORK main_work, int diff)
    {
        dmStgSlctZoneActRange(main_work, main_work.chng_zone, out var out_start, out var out_end);

        for (int i = out_start; i < out_end; i++)
            main_work.act_top_pos_x[i] = dm_stgslct_variant.act_table_top_pos_x;

        main_work.act_move_src[0] = main_work.act_top_pos_x[out_start];

        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var in_start, out var in_end);

        for (int i = in_start; i < in_end; i++)
        {
            main_work.act_top_pos_x[i] = diff > 0
                ? DMD_STGSLCT_STAGE_TAB_NODISP_POS_X
                : DMD_STGSLCT_STAGE_TAB_NODISP_POS_X * -1;

            main_work.act_top_pos_y[i] = dm_stgslct_variant.act_disp_y_pos_tbl[i];
        }

        main_work.act_move_src[1] = main_work.act_top_pos_x[in_start];
    }

    private static void dmStgSlctSetFocusChangeEfctData(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var bounds = dmStgSlctGetBounds(main_work);

        var chng_sign = 0;

        if ((main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR) != 0)
            chng_sign = -1;
        else if ((main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR) != 0)
            chng_sign = 1;
        else if (dm_stgslct_variant.is_console)
            MTM_ASSERT(0);

        main_work.prev_stage = main_work.cur_stage;
        main_work.prev_disp_no = main_work.focus_disp_no;

        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL) != 0)
        {
            dmStgSlctSetVerticalFocus(main_work, bounds, chng_sign);
            dmStgSlctSettleCursorWrap(main_work, chng_sign);
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_CRSR) != 0)
            dmStgSlctMoveCursor(main_work, chng_sign);

        main_work.cur_stage = main_work.crsr_idx + main_work.focus_disp_no + bounds.src;
    }

    private static void dmStgSlctSetVerticalFocus(
        DMS_STGSLCT_MAIN_WORK main_work, DMS_STGSLCT_BOUNDS bounds, int chng_sign)
    {
        if (dm_stgslct_variant.is_console)
        {
            main_work.prev_disp_no = main_work.focus_disp_no;
            main_work.focus_disp_no = dmStgSlctGetRevisedStageVrtclNo(
                main_work.focus_disp_no, chng_sign, main_work.crsr_idx);

            for (int i = bounds.src; i < bounds.dst; i++)
            {
                main_work.act_move_pos_src[i] = main_work.act_top_pos_y[i];
                main_work.act_move_pos_dst[i] =
                    dm_stgslct_variant.act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
            }

            return;
        }

        // mobile drives focus_disp_no from the drag instead, so it only copies the target across
        for (int i = bounds.src; i < bounds.dst; i++)
        {
            main_work.act_move_pos_src[i] = main_work.act_top_pos_y[i];
            main_work.act_move_pos_dst[i] = main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE_FINAL
                ? dm_stgslct_variant.act_disp_y_pos_tbl[i]
                : dm_stgslct_variant.act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
        }
    }

    private static void dmStgSlctSettleCursorWrap(DMS_STGSLCT_MAIN_WORK main_work, int chng_sign)
    {
        if ((main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR) != 0
            && main_work.prev_disp_no == 0 && main_work.focus_disp_no == 3)
        {
            main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR) != 0
            && main_work.prev_disp_no == 3 && main_work.focus_disp_no == 0)
        {
            main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
        }

        if (main_work.focus_disp_no == 3 && main_work.prev_disp_no == 0
            || main_work.focus_disp_no == 0 && main_work.prev_disp_no == 3)
        {
            dmStgSlctMoveCursor(main_work, chng_sign);
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_ACT_CHNG_CRSR);
            return;
        }

        var parked_at_top = (main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR) != 0
            && main_work.focus_disp_no == 0 && main_work.prev_disp_no == 0;

        var parked_at_bottom = (main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR) != 0
            && main_work.focus_disp_no == 3 && main_work.prev_disp_no == 3;

        if (!parked_at_top && !parked_at_bottom)
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_ACT_CHNG_CRSR);
    }

    private static void dmStgSlctMoveCursor(DMS_STGSLCT_MAIN_WORK main_work, int chng_sign)
    {
        main_work.crsr_prev_idx = main_work.crsr_idx;

        main_work.crsr_idx = dmStgSlctGetRevisedStageCrsrNo(
            main_work.crsr_idx, chng_sign, (int)main_work.cur_zone, main_work.prev_disp_no);

        main_work.crsr_move_src = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_prev_idx];
        main_work.crsr_move_dst = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_idx];
    }

    private static void dmStgSlctSetTableActiveInfo(DMS_STGSLCT_MAIN_WORK main_work, int cnt)
    {
        var table2 = dm_stgslct_variant.act_tab_table2;
        var table3 = dm_stgslct_variant.act_tab_table3;
        var scr = dm_stgslct_variant.act_tab_scr;
        var mess = dm_stgslct_variant.act_tab_mess;

        if (main_work.is_clear_stage[cnt] < 0)
        {
            dmStgSlctSetTableFrame(main_work, table2, table3, 1.0f);
            AoActSetFrame(main_work.act[scr], 17.0f);
            AoActSetFrame(main_work.act[mess], 17.0f);
            return;
        }

        dmStgSlctSetTableFrame(main_work, table2, table3, 0.0f);
        AoActSetFrame(main_work.act[scr], dm_stgslct_disp_msg_id_table_tbl[cnt]);
        AoActSetFrame(main_work.act[mess], dm_stgslct_disp_msg_id_table_tbl[cnt]);

        var time_attack_locked = main_work.hi_score[cnt] == DMD_STGSLCT_INIT_SCORE_NUM
            && main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK;

        dmStgSlctSetTableFrame(main_work, table2, table3, time_attack_locked ? 1.0f : 0.0f);
    }

    private static void dmStgSlctSetTableFrame(
        DMS_STGSLCT_MAIN_WORK main_work, int start, int end, float frame)
    {
        for (int j = start; j <= end; j++)
            AoActSetFrame(main_work.act[j], frame);
    }

    private static bool dmStgSlctIsCanSelectAct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.is_clear_stage[main_work.cur_stage] == -1)
        {
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
            return false;
        }

        // time attack stays locked until the stage has been cleared for score
        return main_work.hi_score[main_work.cur_stage] != DMD_STGSLCT_INIT_SCORE_NUM
            || main_work.cur_game_mode != (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK;
    }

    // mobile only. The forward declaration in the C++ carries a second `zone` parameter that the
    // definition and its one call site do not have; the one-parameter form is the real one.
    private static bool dmStgSlctIsBossFocus(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE_FINAL
            || main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE_SPE)
        {
            return false;
        }

        if (main_work.cur_game_mode != (uint)GSD_GAME_MODE_STORY)
            return false;

        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var start, out var end);

        var cleared = 0;

        for (int i = start; i < end; i++)
        {
            if (main_work.is_clear_stage[i] == 1)
                cleared++;
        }

        // three of four done means the boss is what you came back for
        return cleared == 3;
    }

    // mobile only: where the incoming zone's act list should be scrolled to
    private static void dmStgSlctStageSelectChngZoneSetInZoneScroll(
        DMS_STGSLCT_MAIN_WORK main_work, int stage)
    {
        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var start, out var end);

        if (main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE_FINAL)
        {
            main_work.focus_disp_no = 0;

            for (int i = start; i < end; i++)
                main_work.act_top_pos_y[i] = dm_stgslct_variant.act_disp_y_pos_tbl[i];

            return;
        }

        if (main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE_SPE)
        {
            main_work.focus_disp_no =
                MTM_MATH_CLIP(stage - (int)DME_STGSLCT_STAGE_S_2, 0, 4);
        }
        else if (dmStgSlctIsBossFocus(main_work))
        {
            main_work.focus_disp_no = 1;
        }
        else
        {
            var on_boss = stage == (int)DME_STGSLCT_STAGE_1_B
                || stage == (int)DME_STGSLCT_STAGE_2_B
                || stage == (int)DME_STGSLCT_STAGE_3_B
                || stage == (int)DME_STGSLCT_STAGE_4_B;

            main_work.focus_disp_no = on_boss ? 1 : 0;
        }

        for (int i = start; i < end; i++)
            main_work.act_top_pos_y[i] = dm_stgslct_variant.act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
    }

    private static void dmStgSlctSetObiEfctPos(DMS_STGSLCT_MAIN_WORK main_work)
    {
        for (int i = 0; i < 2; i++)
        {
            if (main_work.obi_pos[i] < DMD_STGSLCT_OBI_MOVE_END_POS)
                main_work.obi_pos[i] = DMD_STGSLCT_OBI_MOVE_START_POS;

            main_work.obi_pos[i] += DMD_STGSLCT_OBI_MOVE_SPEED;
        }
    }

    private static int dmStgSlctSetNextFocusAct(DMS_STGSLCT_MAIN_WORK main_work, int set_stage_id)
    {
        if (set_stage_id >= GSD_MAIN_STAGE_ID_SS1)
        {
            return (int)dm_stgslct_zone_act_num_tbl[(int)DME_STGSLCT_ACT_PAGE_SPE][0]
                + (set_stage_id - GSD_MAIN_STAGE_ID_SS1);
        }

        if (set_stage_id >= GSD_MAIN_STAGE_ID_FINAL_1)
            return GSD_MAIN_STAGE_ID_FINAL_1;

        if (!g_gs_main_sys_info.is_first_play)
            return set_stage_id;

        if (g_gs_main_sys_info.game_mode == (int)GSD_GAME_MODE_TIME_ATTACK)
            return set_stage_id;

        if ((main_work.flag & DMD_STGSLCT_FLAG_PREV_EVT_RANKING) != 0)
            return set_stage_id;

        // first uncleared act in the zone the player was last in
        var zone_act_id = (int)dm_stgslct_zone_array_act_tbl[set_stage_id];

        for (int i = set_stage_id - zone_act_id; i < set_stage_id - zone_act_id + 4; i++)
        {
            if (!GsMainSysIsStageClear(i))
                return i;
        }

        // then the first uncleared act anywhere
        for (int zone = 0; zone < 4; zone++)
        {
            var zone_start = (int)dm_stgslct_zone_act_num_tbl[zone][0];

            for (int i = 0; i < 4; i++)
            {
                if (!GsMainSysIsStageClear(zone_start + i))
                    return zone_start + i;
            }
        }

        return GSD_MAIN_STAGE_ID_FINAL_1;
    }
}
