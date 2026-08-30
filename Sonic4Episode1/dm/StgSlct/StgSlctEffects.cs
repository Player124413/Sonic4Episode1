using static AppMain;

using static StgSlct.DME_STGSLCT_ZONE_TYPE;

internal static partial class StgSlct
{
    // which way each zone is still travelling, per is_final_open, per zone. +1 means the "not
    // finished yet" guard is pos > target, -1 means pos < target. The in and out sets are not
    // inverses of each other, and SEGA marked the all-open zone 5 rows suspect in both. Treated
    // as data and reproduced exactly, see docs/stgslct-port-quirks.md
    private static readonly sbyte[][][] dm_stgslct_zone_out_guard_tbl = [
        [[+1, +1], [+1, -1], [-1, +1], [-1, -1]],
        [[+1, +1], [+1, +1], [-1, +1], [-1, -1], [+1, -1]],
        [[+1, +1], [+1, +1], [-1, +1], [-1, -1], [+1, -1], [+1, -1]],
        [[+1, +1], [+1, +1], [-1, +1], [-1, -1], [+1, -1], [+1, +1]],
    ];

    private static readonly sbyte[][][] dm_stgslct_zone_in_guard_tbl = [
        [[-1, -1], [-1, +1], [+1, -1], [+1, +1]],
        [[-1, -1], [-1, -1], [+1, -1], [+1, +1], [-1, +1]],
        [[-1, -1], [-1, -1], [+1, -1], [+1, +1], [-1, +1], [-1, +1]],
        [[-1, -1], [-1, -1], [+1, -1], [+1, +1], [-1, -1], [-1, +1]],
    ];

    private static bool dmStgSlctStillMoving(float pos, float target, sbyte guard)
    {
        return guard > 0 ? pos > target : pos < target;
    }

    private static void dmStgSlctSlide(ref float pos, float from, float to)
    {
        pos += (to - from) / DMD_STGSLCT_ZONE_EFCT_TIME;
    }

    private static bool dmStgSlctSlideDone(ref float pos, float from, float to)
    {
        var arrived = to < from ? pos <= to : pos >= to;

        if (!arrived)
            return false;

        pos = to;
        return true;
    }

    private static void dmStgSlctSetDecideZoneEfctPos(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.decide_zone_efct_dist_x = 0;
        main_work.decide_zone_efct_dist_y = main_work.timer < DMD_STGSLCT_ZONE_DECIDE_EFCT_ON
            ? DMD_STGSLCT_ZONE_DECIDE_EFCT_ON_POS_Y
            : 0;
    }

    private static bool dmStgSlctIsDecideZoneEfctPos(DMS_STGSLCT_MAIN_WORK main_work)
    {
        return main_work.timer > DMD_STGSLCT_ZONE_DECIDE_EFCT_TIME;
    }

    private static void dmStgSlctSetZonePosOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var disp = dm_stgslct_variant.zone_disp_pos_tbl[main_work.is_final_open];
        var nodisp = dm_stgslct_variant.zone_nodisp_pos_tbl[main_work.is_final_open];

        for (int i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
        {
            if ((main_work.efct_out_flag & (1u << i)) == 0 || i == main_work.cur_zone)
                continue;

            dmStgSlctSlide(ref main_work.zone_pos[i][0], disp[i][0], nodisp[i][0]);
            dmStgSlctSlide(ref main_work.zone_pos[i][1], disp[i][1], nodisp[i][1]);
        }

        dmStgSlctSetEmeTableOutEfct(main_work);
    }

    private static bool dmStgSlctIsZonePosOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var nodisp = dm_stgslct_variant.zone_nodisp_pos_tbl[main_work.is_final_open];
        var guards = dm_stgslct_zone_out_guard_tbl[main_work.is_final_open];

        for (int i = 0; i < guards.Length; i++)
        {
            if (i == main_work.cur_zone)
                continue;

            if (dmStgSlctStillMoving(main_work.zone_pos[i][0], nodisp[i][0], guards[i][0])
                || dmStgSlctStillMoving(main_work.zone_pos[i][1], nodisp[i][1], guards[i][1]))
            {
                return false;
            }
        }

        main_work.efct_out_flag = 0;

        return dmStgSlctIsEmeTableOutEfctEnd(main_work);
    }

    private static void dmStgSlctSetZonePosInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var disp = dm_stgslct_variant.zone_disp_pos_tbl[main_work.is_final_open];
        var nodisp = dm_stgslct_variant.zone_nodisp_pos_tbl[main_work.is_final_open];

        for (int i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
        {
            dmStgSlctSlide(ref main_work.zone_pos[i][0], nodisp[i][0], disp[i][0]);
            dmStgSlctSlide(ref main_work.zone_pos[i][1], nodisp[i][1], disp[i][1]);
        }

        dmStgSlctSetEmeTableInEfct(main_work);

        main_work.timer++;
    }

    // unlike the out version this does not skip cur_zone
    private static bool dmStgSlctIsZonePosInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var disp = dm_stgslct_variant.zone_disp_pos_tbl[main_work.is_final_open];
        var guards = dm_stgslct_zone_in_guard_tbl[main_work.is_final_open];

        for (int i = 0; i < guards.Length; i++)
        {
            if (dmStgSlctStillMoving(main_work.zone_pos[i][0], disp[i][0], guards[i][0])
                || dmStgSlctStillMoving(main_work.zone_pos[i][1], disp[i][1], guards[i][1]))
            {
                return false;
            }
        }

        return dmStgSlctIsEmeTableInEfctEnd(main_work);
    }

    private static void dmStgSlctSetDecideZonePosOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var disp = dm_stgslct_variant.zone_disp_pos_tbl[main_work.is_final_open];
        var nodisp = dm_stgslct_variant.zone_nodisp_pos_tbl[main_work.is_final_open];

        dmStgSlctSlide(
            ref main_work.zone_pos[main_work.cur_zone][0],
            disp[main_work.cur_zone][0],
            nodisp[0][0]);
    }

    private static void dmStgSlctZoneActRange(
        DMS_STGSLCT_MAIN_WORK main_work, uint zone, out int start, out int end)
    {
        start = (int)dm_stgslct_zone_act_num_tbl[zone][0];
        end = start + (int)dm_stgslct_zone_act_num_tbl[zone][1];
    }

    private static void dmStgSlctSetStagePosInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var start, out var end);

        if (main_work.act_top_pos_x[start] > dm_stgslct_variant.act_table_top_pos_x)
        {
            for (int i = start; i < end; i++)
            {
                dmStgSlctSlide(
                    ref main_work.act_top_pos_x[i],
                    DMD_STGSLCT_STAGE_TAB_NODISP_POS_X,
                    dm_stgslct_variant.act_table_top_pos_x);
            }
        }

        dmStgSlctSetDecideZonePosOutEfct(main_work);
        dmStgSlctSetModeTexInEfct(main_work);
    }

    // the all-open table here is not a slip: SEGA indexes it regardless of is_final_open
    private static bool dmStgSlctIsStagePosInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var start, out var end);

        if (!dmStgSlctIsModeTexInEfctEnd(main_work))
            return false;

        var parked = dm_stgslct_variant.zone_nodisp_pos_tbl[DMD_STGSLCT_ZONE_ALL_OPEN][0][0];

        if (main_work.act_top_pos_x[start] > dm_stgslct_variant.act_table_top_pos_x
            || main_work.zone_pos[main_work.cur_zone][0] >= parked)
        {
            return false;
        }

        for (int i = start; i < end; i++)
            main_work.act_top_pos_x[i] = dm_stgslct_variant.act_table_top_pos_x;

        main_work.zone_pos[main_work.cur_zone][0] = parked;

        return true;
    }

    private static void dmStgSlctSetStagePosOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var start, out var end);

        for (int i = start; i < end; i++)
        {
            dmStgSlctSlide(
                ref main_work.act_top_pos_x[i],
                dm_stgslct_variant.act_table_top_pos_x,
                DMD_STGSLCT_STAGE_TAB_NODISP_POS_X);
        }

        dmStgSlctSetModeTexOutEfct(main_work);
    }

    private static bool dmStgSlctIsStagePosOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var start, out var end);

        if (!dmStgSlctIsModeTexOutEfctEnd(main_work))
            return false;

        if (main_work.act_top_pos_x[start] < DMD_STGSLCT_STAGE_TAB_NODISP_POS_X)
            return false;

        for (int i = start; i < end; i++)
            main_work.act_top_pos_x[i] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;

        return true;
    }

    // both loops write act_top_pos_x, the second using act_move_dest[1]. That is SEGA's, and it is
    // why the completion check below reads act_top_pos_x for the cur_zone run too.
    private static void dmStgSlctSetStageZoneChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        dmStgSlctZoneActRange(main_work, main_work.chng_zone, out var chng_start, out var chng_end);

        for (int i = chng_start; i < chng_end; i++)
        {
            dmStgSlctSlide(
                ref main_work.act_top_pos_x[i], main_work.act_move_src[0], main_work.act_move_dest[0]);
        }

        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var cur_start, out var cur_end);

        for (int i = cur_start; i < cur_end; i++)
        {
            dmStgSlctSlide(
                ref main_work.act_top_pos_x[i], main_work.act_move_src[1], main_work.act_move_dest[1]);
        }
    }

    private static bool dmStgSlctIsStageZoneChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.act_move_dest[0] == 0)
            return false;

        var moving_right = main_work.act_move_dest[0] > 0;

        dmStgSlctZoneActRange(main_work, main_work.chng_zone, out var chng_start, out var chng_end);
        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var cur_start, out var cur_end);

        var chng_done = dmStgSlctParkActRun(
            main_work, chng_start, chng_end, main_work.act_move_dest[0], moving_right);

        var cur_done = dmStgSlctParkActRun(
            main_work, cur_start, cur_end, main_work.act_move_dest[1], moving_right);

        return chng_done && cur_done;
    }

    private static bool dmStgSlctParkActRun(
        DMS_STGSLCT_MAIN_WORK main_work, int start, int end, float dest, bool moving_right)
    {
        var arrived = moving_right
            ? main_work.act_top_pos_x[start] >= dest
            : main_work.act_top_pos_x[start] <= dest;

        if (!arrived)
            return false;

        for (int i = start; i < end; i++)
            main_work.act_top_pos_x[i] = dest;

        return true;
    }

    private static void dmStgSlctSetStageVrtclChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var start, out var end);

        for (int i = start; i < end; i++)
        {
            main_work.act_top_pos_y[i] +=
                (main_work.act_move_pos_dst[i] - main_work.act_move_pos_src[i])
                / DMD_STGSLCT_ACT_VRTCL_MOVE_TIME;
        }
    }

    private static bool dmStgSlctIsStageVrtclChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.timer < DMD_STGSLCT_ACT_VRTCL_MOVE_TIME)
            return false;

        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var start, out var end);

        for (int i = start; i < end; i++)
            main_work.act_top_pos_y[i] = main_work.act_move_pos_dst[i];

        return true;
    }

    private static void dmStgSlctSetStageCrsrChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.crsr_pos_y +=
            (main_work.crsr_move_dst - main_work.crsr_move_src) / DMD_STGSLCT_CRSR_MOVE_TIME;
    }

    private static bool dmStgSlctIsStageCrsrChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.timer < DMD_STGSLCT_CRSR_MOVE_TIME)
            return false;

        main_work.crsr_pos_y = main_work.crsr_move_dst;

        return true;
    }

    private static void dmStgSlctSetWinOpenEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (!dm_stgslct_variant.is_console
            && main_work.win_timer >= 0.0f && main_work.win_timer < 1.0f)
        {
            DmSoundPlaySE("Window");
        }

        if (main_work.win_timer > DMD_STGSLCT_WIN_EFCT_TIME)
        {
            main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT_END;
            main_work.win_timer = 0.0f;

            for (int i = 0; i < 2; i++)
                main_work.win_size_rate[i] = DMD_STGSLCT_WIN_DEF_RATE;
        }
        else
        {
            main_work.win_timer++;
        }

        for (int i = 0; i < 2; i++)
        {
            main_work.win_size_rate[i] = main_work.win_timer != 0
                ? main_work.win_timer / DMD_STGSLCT_WIN_EFCT_TIME
                : 1.0f;

            if (main_work.win_size_rate[i] > 1.0f)
                main_work.win_size_rate[i] = 1.0f;
        }
    }

    private static void dmStgSlctSetWinCloseEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        for (int i = 0; i < 2; i++)
        {
            main_work.win_size_rate[i] = main_work.win_timer != 0
                ? main_work.win_timer / DMD_STGSLCT_WIN_EFCT_TIME
                : 0.0f;
        }

        if (main_work.win_timer < 0.0f)
        {
            main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT_END;
            main_work.win_timer = 0.0f;

            for (int i = 0; i < 2; i++)
                main_work.win_size_rate[i] = 0.0f;
        }
        else
        {
            main_work.win_timer--;
        }
    }

    private static void dmStgSlctSetEmeTableInEfct(DMS_STGSLCT_MAIN_WORK main_work) =>
        dmStgSlctSlide(ref main_work.chaos_eme_pos_y,
            DMD_STGSLCT_DOWN_ACT_NODISP_POS, DMD_STGSLCT_DOWN_ACT_DISP_POS);

    private static bool dmStgSlctIsEmeTableInEfctEnd(DMS_STGSLCT_MAIN_WORK main_work) =>
        dmStgSlctSlideDone(ref main_work.chaos_eme_pos_y,
            DMD_STGSLCT_DOWN_ACT_NODISP_POS, DMD_STGSLCT_DOWN_ACT_DISP_POS);

    private static void dmStgSlctSetEmeTableOutEfct(DMS_STGSLCT_MAIN_WORK main_work) =>
        dmStgSlctSlide(ref main_work.chaos_eme_pos_y,
            DMD_STGSLCT_DOWN_ACT_DISP_POS, DMD_STGSLCT_DOWN_ACT_NODISP_POS);

    private static bool dmStgSlctIsEmeTableOutEfctEnd(DMS_STGSLCT_MAIN_WORK main_work) =>
        dmStgSlctSlideDone(ref main_work.chaos_eme_pos_y,
            DMD_STGSLCT_DOWN_ACT_DISP_POS, DMD_STGSLCT_DOWN_ACT_NODISP_POS);

    private static void dmStgSlctSetModeTexInEfct(DMS_STGSLCT_MAIN_WORK main_work) =>
        dmStgSlctSlide(ref main_work.mode_tex_pos_y,
            DMD_STGSLCT_DOWN_ACT_NODISP_POS, DMD_STGSLCT_DOWN_ACT_DISP_POS);

    private static bool dmStgSlctIsModeTexInEfctEnd(DMS_STGSLCT_MAIN_WORK main_work) =>
        dmStgSlctSlideDone(ref main_work.mode_tex_pos_y,
            DMD_STGSLCT_DOWN_ACT_NODISP_POS, DMD_STGSLCT_DOWN_ACT_DISP_POS);

    private static void dmStgSlctSetModeTexOutEfct(DMS_STGSLCT_MAIN_WORK main_work) =>
        dmStgSlctSlide(ref main_work.mode_tex_pos_y,
            DMD_STGSLCT_DOWN_ACT_DISP_POS, DMD_STGSLCT_DOWN_ACT_NODISP_POS);

    private static bool dmStgSlctIsModeTexOutEfctEnd(DMS_STGSLCT_MAIN_WORK main_work) =>
        dmStgSlctSlideDone(ref main_work.mode_tex_pos_y,
            DMD_STGSLCT_DOWN_ACT_DISP_POS, DMD_STGSLCT_DOWN_ACT_NODISP_POS);

    private static void dmStgSlctSetBgFadeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var alpha = (int)main_work.bg_fade.a;

        if (main_work.cur_zone != main_work.cur_bg_id)
        {
            main_work.flag |= DMD_STGSLCT_FLAG_BG_FADE_EFCT;
            main_work.next_bg_id = main_work.cur_zone;
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_BG_FADE_EFCT) != 0)
        {
            if (alpha < 255)
            {
                alpha += DMD_STGSLCT_BG_FADE_SPEED;
            }
            else
            {
                alpha = 255;
                main_work.cur_bg_id = main_work.next_bg_id;
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_BG_FADE_EFCT);
            }
        }
        else if (alpha > 0)
        {
            alpha -= DMD_STGSLCT_BG_FADE_SPEED;
        }

        main_work.bg_fade.a = (byte)MTM_MATH_CLIP(alpha, 0, 255);
    }

    private static void dmStgSlctSetZoneScrChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.zone_scr_id > DMD_STGSLCT_ZONE_SCR_CHANGE_ALL_FRM)
            main_work.zone_scr_id = 0;
        else
            main_work.zone_scr_id++;
    }
}
