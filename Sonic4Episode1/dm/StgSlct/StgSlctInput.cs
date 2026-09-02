using static AoPad;

using static AppMain;

using static StgSlct.DME_STGSLCT_ACT_PAGE;
using static StgSlct.DME_STGSLCT_PLAY_MODE;
using static StgSlct.DME_STGSLCT_WIN;

internal static partial class StgSlct
{
    // ---- shared between the console arms and, where the C++ leaves them unguarded, the mobile ones

    // Picking the new zone is the pad's job only. The touch handlers have already assigned
    // cur_zone and chng_zone by the time they reach the shared body, which is why the C++ keeps
    // these two lines inside #if !_IPHONE and the rest outside it.
    private static void dmStgSlctChangeZonePad(
        DMS_STGSLCT_MAIN_WORK main_work, DMS_STGSLCT_BOUNDS bounds, int diff)
    {
        main_work.chng_zone = main_work.cur_zone;
        main_work.cur_zone = (uint)dmStgSlctGetRevisedZoneNo(
            (int)main_work.cur_zone, diff, bounds.is_final_open, bounds.is_spe_open);

        dmStgSlctApplyZoneChange(main_work, diff);
    }

    // the L/R body, shared by both input procs and by the touch path. The C++ runs the shoulder
    // button check on touch too, where it simply never fires.
    private static void dmStgSlctApplyZoneChange(DMS_STGSLCT_MAIN_WORK main_work, int diff)
    {
        var src = (int)dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];

        main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_ZONE;

        main_work.act_move_dest[0] = diff < 0
            ? DMD_STGSLCT_STAGE_TAB_NODISP_POS_X
            : DMD_STGSLCT_STAGE_TAB_NODISP_POS_X * -1;

        main_work.act_move_dest[1] = dm_stgslct_variant.act_table_top_pos_x;

        if (main_work.cur_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
        {
            main_work.crsr_pos_y = 320.0f;
            main_work.crsr_idx = 0;
        }

        if (main_work.chng_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
        {
            main_work.crsr_pos_y = 160.0f;
            main_work.crsr_idx = 0;
        }

        main_work.prev_disp_no = main_work.focus_disp_no;
        main_work.focus_disp_no = 0;
        main_work.cur_stage = main_work.crsr_idx + main_work.focus_disp_no + src;

        dmStgSlctSetActChngZonePosInit(main_work, diff);

        DmSoundPlaySE("Cursol");

        var shoulder = diff < 0 ? BUTTON.L : BUTTON.R;

        if (AoPadMRepeat().HasFlag(shoulder) || AoPadMStand().HasFlag(shoulder))
        {
            main_work.flag |= (uint)(diff < 0
                ? DMD_STGSLCT_FLAG_ACT_PUSH_L_BTN
                : DMD_STGSLCT_FLAG_ACT_PUSH_R_BTN);

            if (diff < 0)
                main_work.btn_l_disp_frm = 0;
            else
                main_work.btn_r_disp_frm = 0;
        }

        main_work.timer = 0;
    }

    // the UP/DOWN body, identical in dmStgSlctInputProcStageSelect and ...StageSelectMove
    private static void dmStgSlctMoveCursorVertical(
        DMS_STGSLCT_MAIN_WORK main_work, DMS_STGSLCT_BOUNDS bounds, bool up)
    {
        if (main_work.cur_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
            return;

        var at_edge = up ? main_work.crsr_idx == 0 : main_work.crsr_idx == 3;

        if (at_edge && bounds.act_comp > DMD_STGSLCT_ACT_VRTCL_CHNG_NUM)
            main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
        else
            main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;

        main_work.flag |= (uint)(up
            ? DMD_STGSLCT_FLAG_UP_CHNG_CRSR
            : DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR);
    }

    private static bool dmStgSlctInputCancelDecide(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (AoPadStand().HasFlag(BUTTON.CANCEL))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;
            return true;
        }

        if (AoPadStand().HasFlag(BUTTON.DECIDE))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;
            return true;
        }

        return false;
    }

    private static bool dmStgSlctInputToggleGameMode(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (!AoPadStand().HasFlag(BUTTON.X))
            return false;

        main_work.cur_game_mode ^= (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK;
        main_work.mode_tex_frm = 1;

        DmSoundPlaySE("Cursol");

        return true;
    }

    // TODO(port): dmStgSlct.cpp:5153. Raising the flag reaches DME_STGSLCT_NEXT_EVT_RANKING, and
    // GSD_EVT_ID_RANKING is wired to AppMain.DbgDummyInitResult, an empty method. The event fires,
    // no task is ever made, and nothing transitions again, so the game hangs. Restore this the
    // moment dmRanking.cpp is ported and sysEvtDataArray1[7] points at something real.
    private static void dmStgSlctInputChangeEvtRanking(DMS_STGSLCT_MAIN_WORK main_work)
    {
        //if (AoPadStand().HasFlag(ControllerConsts.Y))
        //    main_work.flag |= DMD_STGSLCT_FLAG_CHANGE_EVT_RANKING;
    }

    // ---- zone select

    private static void dmStgSlctInputProcZoneSelectPc(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var tmp_cur_zone = main_work.cur_zone;

        if (dmStgSlctInputCancelDecide(main_work))
            return;

        var dirs = dm_stgslct_zone_input_dir_tbl[main_work.is_final_open];

        for (int dir = 0; dir < 4; dir++)
        {
            var key = dir switch
            {
                0 => BUTTON.UP,
                1 => BUTTON.DOWN,
                2 => BUTTON.LEFT,
                _ => BUTTON.RIGHT,
            };

            if (!AoPadMStand().HasFlag(key))
                continue;

            main_work.cur_zone = (uint)dirs[tmp_cur_zone][dir];

            if (main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE.DME_STGSLCT_ZONE_TYPE_NONE)
            {
                DmSoundPlaySE("Cursol");
                main_work.zone_scr_id = 0;
            }

            break;
        }

        if (main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE.DME_STGSLCT_ZONE_TYPE_NONE)
            main_work.cur_zone = tmp_cur_zone;
    }

    private static void dmStgSlctInputProcZoneSelectIphone(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var tmp_cur_zone = main_work.cur_zone;
        var touch = main_work.touch;

        var cancel = touch.trg_cancel.GetState(0U);

        if (cancel[er.CTrgState.EState.Up] && cancel[er.CTrgState.EState.Prev])
        {
            main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;
            return;
        }

        for (int i = 0; i < touch.trg_zone.Length; i++)
        {
            var zone = touch.trg_zone[i].GetState(0U);

            if (zone[er.CTrgState.EState.Up] && zone[er.CTrgState.EState.Prev])
            {
                main_work.cur_zone = (uint)i;
                main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;
                break;
            }

            if (zone[er.CTrgState.EState.Stand])
            {
                main_work.cur_zone = (uint)i;
                touch.is_disp_cover = true;
                break;
            }

            if (zone[er.CTrgState.EState.Out])
            {
                // SEGA's sentinel for "nothing highlighted". It is not ZONE_TYPE_NONE, so the
                // restore below does not catch it, see docs/stgslct-port-quirks.md
                main_work.cur_zone = uint.MaxValue;
                touch.is_disp_cover = false;
            }
        }

        if (main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE.DME_STGSLCT_ZONE_TYPE_NONE)
            main_work.cur_zone = tmp_cur_zone;
    }

    // ---- act select

    private static void dmStgSlctInputProcStageSelectPc(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var bounds = dmStgSlctGetBounds(main_work);

        if (dmStgSlctInputCancelDecide(main_work))
            return;

        if (dmStgSlctInputToggleGameMode(main_work))
            return;

        dmStgSlctInputChangeEvtRanking(main_work);

        if (AoPadMRepeat().HasFlag(BUTTON.UP))
        {
            if (AoPadMStand().HasFlag(BUTTON.UP) || main_work.cur_stage != bounds.src)
                dmStgSlctMoveCursorVertical(main_work, bounds, up: true);

            return;
        }

        if (AoPadMRepeat().HasFlag(BUTTON.DOWN))
        {
            if (AoPadMStand().HasFlag(BUTTON.DOWN) || main_work.cur_stage != bounds.dst - 1)
                dmStgSlctMoveCursorVertical(main_work, bounds, up: false);

            return;
        }

        if (dmStgSlctInputZoneRepeat(main_work, bounds, left: true))
            return;

        dmStgSlctInputZoneRepeat(main_work, bounds, left: false);
    }

    private static bool dmStgSlctInputZoneRepeat(
        DMS_STGSLCT_MAIN_WORK main_work, DMS_STGSLCT_BOUNDS bounds, bool left)
    {
        var dpad = left ? BUTTON.LEFT : BUTTON.RIGHT;
        var shoulder = left ? BUTTON.L : BUTTON.R;

        if (!AoPadMRepeat().HasFlag(dpad) && !AoPadMRepeat().HasFlag(shoulder))
            return false;

        var tapped = AoPadMStand().HasFlag(dpad) || AoPadMStand().HasFlag(shoulder);

        var past_edge = left
            ? main_work.cur_zone != (uint)DME_STGSLCT_ACT_PAGE_1
            : main_work.cur_zone != (uint)bounds.edge;

        if (tapped || past_edge)
            dmStgSlctChangeZonePad(main_work, bounds, left ? -1 : 1);

        return true;
    }

    private static void dmStgSlctInputProcStageSelectIphone(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var bounds = dmStgSlctGetBounds(main_work);
        var touch = main_work.touch;

        var cancel = touch.trg_cancel.GetState(0U);
        var dragging = touch.trg_act_move.GetState(0U)[er.CTrgState.EState.DragAndDrop];

        if (!dragging
            && cancel[er.CTrgState.EState.Lock]
            && cancel[er.CTrgState.EState.Up]
            && cancel[er.CTrgState.EState.Prev])
        {
            main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;
            return;
        }

        touch.UpdateModeToggle(main_work);

        if (touch.PickAct(main_work, bounds.src))
            return;

        touch.move_left = false;
        touch.move_right = false;

        // every one of these can set move_left or move_right, so none may be short-circuited
        var tabbed = touch.PickZoneTab(main_work);
        var arrowed = touch.PickZoneArrow(main_work);
        var dragged = touch.ApplyDrag(main_work, bounds.src, bounds.dst);

        if (tabbed || arrowed || dragged || (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL) != 0)
        {
            foreach (var trg in touch.trg_act)
                trg.DelLock();

            touch.is_disp_cover = false;
        }

        if (touch.move_left)
            dmStgSlctApplyZoneChange(main_work, -1);
        else if (touch.move_right)
            dmStgSlctApplyZoneChange(main_work, 1);
    }

    // ---- the two the C++ never guards, so they run on both paths and simply find no pad on mobile

    private static void dmStgSlctInputProcStageSelectChngZone(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var bounds = dmStgSlctGetBounds(main_work);

        if (dmStgSlctInputCancelDecide(main_work))
            return;

        if (dmStgSlctInputToggleGameMode(main_work))
            return;

        dmStgSlctInputChangeEvtRanking(main_work);

        if (dmStgSlctInputZoneRepeat(main_work, bounds, left: true))
            return;

        dmStgSlctInputZoneRepeat(main_work, bounds, left: false);
    }

    private static void dmStgSlctInputProcStageSelectMove(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var bounds = dmStgSlctGetBounds(main_work);

        if (dmStgSlctInputCancelDecide(main_work))
            return;

        if (dmStgSlctInputToggleGameMode(main_work))
            return;

        dmStgSlctInputChangeEvtRanking(main_work);

        if (AoPadMRepeat().HasFlag(BUTTON.UP))
        {
            if (AoPadMStand().HasFlag(BUTTON.UP) || main_work.cur_stage != bounds.src)
                dmStgSlctMoveCursorVertical(main_work, bounds, up: true);

            return;
        }

        if (AoPadMRepeat().HasFlag(BUTTON.DOWN))
        {
            if (AoPadMStand().HasFlag(BUTTON.DOWN) || main_work.cur_stage != bounds.dst - 1)
                dmStgSlctMoveCursorVertical(main_work, bounds, up: false);
        }
    }

    // ---- the window

    private static void dmStgSlctInputProcWinDispIdlePc(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (AoPadStand().HasFlag(BUTTON.CANCEL))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;
            return;
        }

        if (AoPadStand().HasFlag(BUTTON.DECIDE))
        {
            main_work.flag |= (uint)(main_work.win_cur_slct == 0
                || main_work.win_mode == (int)DME_STGSLCT_WIN_MENU
                    ? DMD_STGSLCT_FLAG_DECIDE
                    : DMD_STGSLCT_FLAG_CANCEL);

            return;
        }

        if (main_work.win_mode == (int)DME_STGSLCT_WIN_MENU)
        {
            dmStgSlctWindowPick(main_work, BUTTON.UP, BUTTON.DOWN);
        }
        else if (main_work.win_mode == (int)DME_STGSLCT_WIN_STG_SLCT)
        {
            dmStgSlctWindowPick(main_work, BUTTON.LEFT, BUTTON.RIGHT);
        }
    }

    private static void dmStgSlctWindowPick(
        DMS_STGSLCT_MAIN_WORK main_work, BUTTON to_first, BUTTON to_second)
    {
        if (AoPadMStand().HasFlag(to_first))
        {
            if (main_work.win_cur_slct != 0)
                DmSoundPlaySE("Cursol");

            main_work.win_cur_slct = 0;
        }
        else if (AoPadMStand().HasFlag(to_second))
        {
            if (main_work.win_cur_slct != 1)
                DmSoundPlaySE("Cursol");

            main_work.win_cur_slct = 1;
        }
    }

    private static void dmStgSlctInputProcWinDispIdleIphone(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.win_mode == (int)DME_STGSLCT_WIN_STG_SLCT)
        {
            main_work.touch.UpdateAnswerButtons(main_work);
            return;
        }

        // no choices to make, so a touch anywhere dismisses it
        if (amTpIsTouchPush(0))
            main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;
    }
}
