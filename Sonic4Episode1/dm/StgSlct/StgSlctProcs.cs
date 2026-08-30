using static AppMain;

using static StgSlct.DME_STGSLCT_ACT_PAGE;
using static StgSlct.DME_STGSLCT_MODE_STATE;
using static StgSlct.DME_STGSLCT_NEXT_EVT;
using static StgSlct.DME_STGSLCT_STAGE;
using static StgSlct.DME_STGSLCT_WIN;
using static StgSlct.DME_STGSLCT_ZONE_TYPE;

internal static partial class StgSlct
{
    // The C++ tests proc_win_update against dmStgSlctProcWindowNodispIdle in four places as its
    // "no dialog open" check. Delegate identity, kept as-is, but named so the call sites read.
    private static bool dmStgSlctIsWindowIdle(DMS_STGSLCT_MAIN_WORK main_work)
    {
        return main_work.proc_win_update == dmStgSlctProcWindowNodispIdle;
    }

    private static void dmStgSlctSeedZonePos(DMS_STGSLCT_MAIN_WORK main_work, bool visible)
    {
        var pos = visible
            ? dm_stgslct_variant.zone_disp_pos_tbl[main_work.is_final_open]
            : dm_stgslct_variant.zone_nodisp_pos_tbl[main_work.is_final_open];

        for (int i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
        {
            main_work.zone_pos[i][0] = pos[i][0];
            main_work.zone_pos[i][1] = pos[i][1];
        }
    }

    private static void dmStgSlctProcCreateAct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var slots = dm_stgslct_variant.act_slot_tbl;
        var ids = dm_stgslct_variant.act_id_tbl;

        for (int i = 0; i < dm_stgslct_variant.act_num; i++)
        {
            A2S_AMA_HEADER ama;
            AOS_TEXTURE tex;

            switch (slots[i])
            {
                case DMD_STGSLCT_SLOT_CMN_DATA:
                    ama = main_work.ama[(int)DME_STGSLCT_DATA_TYPE.DME_STGSLCT_DATA_TYPE_CMN_DATA];
                    tex = main_work.tex[(int)DME_STGSLCT_DATA_TYPE.DME_STGSLCT_DATA_TYPE_CMN_DATA];
                    break;

                case DMD_STGSLCT_SLOT_LANG_DATA:
                    ama = main_work.ama[(int)DME_STGSLCT_DATA_TYPE.DME_STGSLCT_DATA_TYPE_LANG_DATA];
                    tex = main_work.tex[(int)DME_STGSLCT_DATA_TYPE.DME_STGSLCT_DATA_TYPE_LANG_DATA];
                    break;

                default:
                    ama = main_work.cmn_ama[slots[i] - DMD_STGSLCT_SLOT_CMN_BG];
                    tex = main_work.cmn_tex[slots[i] - DMD_STGSLCT_SLOT_CMN_BG];
                    break;
            }

            AoActSetTexture(AoTexGetTexList(tex));
            main_work.act[i] = AoActCreate(ama, ids[i]);
        }

        main_work.touch?.Build(main_work);

        main_work.proc_menu_update = dmStgSlctProcSetDispEfctData;
    }

    private static void dmStgSlctProcSetDispEfctData(DMS_STGSLCT_MAIN_WORK main_work)
    {
        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var act_start, out var act_end);

        if ((main_work.announce_flag & 1u << (int)DME_STGSLCT_WIN_1_1_CLEAR) != 0
            || (main_work.announce_flag & 1u << (int)DME_STGSLCT_WIN_SPESTE_CAN_SLCT) != 0
            || (main_work.announce_flag & 1u << (int)DME_STGSLCT_WIN_FINAL_CAN_SLCT) != 0)
        {
            main_work.flag |= DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT;
        }

        if (dm_stgslct_is_stage_start && (main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT) == 0)
        {
            main_work.proc_draw = dmStgSlctStageSelectDraw;
            main_work.state = (int)DME_STGSLCT_MODE_STATE_ACT_SLCT;
        }
        else
        {
            main_work.proc_draw = dmStgSlctZoneSelectDraw;
            main_work.state = (int)DME_STGSLCT_MODE_STATE_ZONE_SLCT;
        }

        if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ZONE_SLCT)
        {
            dmStgSlctSeedZonePos(main_work, visible: true);

            for (int j = 0; j < (int)DME_STGSLCT_STAGE_NUM; j++)
            {
                main_work.act_top_pos_x[j] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
                main_work.act_top_pos_y[j] = dm_stgslct_variant.act_disp_y_pos_tbl[j];
            }

            main_work.mode_tex_pos_y = DMD_STGSLCT_DOWN_ACT_NODISP_POS;
            main_work.chaos_eme_pos_y = DMD_STGSLCT_DOWN_ACT_DISP_POS;
            main_work.mode_tex_move_frm = 0;
        }
        else
        {
            dmStgSlctSeedZonePos(main_work, visible: false);

            for (int j = act_start; j < act_end; j++)
            {
                main_work.act_top_pos_x[j] = dm_stgslct_variant.act_table_top_pos_x;
                main_work.act_top_pos_y[j] = main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE_SPE
                    ? dm_stgslct_variant.act_disp_y_pos_tbl[j]
                    : dm_stgslct_variant.act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
            }

            if (main_work.touch != null)
                dmStgSlctStageSelectChngZoneSetInZoneScroll(main_work, main_work.cur_stage);

            main_work.mode_tex_pos_y = DMD_STGSLCT_DOWN_ACT_DISP_POS;
            main_work.chaos_eme_pos_y = DMD_STGSLCT_DOWN_ACT_NODISP_POS;
            main_work.mode_tex_move_frm = 5;
        }

        main_work.crsr_pos_y = dm_stgslct_variant.act_disp_y_pos_tbl[0];
        main_work.proc_draw = dmStgSlctProcActDraw;

        IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL, IZE_FADE_TYPE_BLACK_FADEIN, DMD_STGSLCT_FADEIN_TIME);

        main_work.proc_menu_update = dmStgSlctProcFadeIn;
    }

    private static void dmStgSlctProcFadeIn(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;

        IzFadeExit();

        if ((main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT) != 0)
        {
            IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL, IZE_FADE_TYPE_WHITE_FADEOUT, DMD_STGSLCT_FADEIN_TIME);
            main_work.proc_menu_update = dmStgSlctProcSetWhiteFlashEfct;
        }
        else
        {
            main_work.proc_menu_update = dmStgSlctProcSetSlctStartData;
        }
    }

    private static void dmStgSlctProcSetWhiteFlashEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;

        IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL, IZE_FADE_TYPE_WHITE_FADEIN, DMD_STGSLCT_FADEIN_TIME);

        main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT);
        main_work.proc_menu_update = dmStgSlctProcIsWhiteFlashEfctEnd;
    }

    private static void dmStgSlctProcIsWhiteFlashEfctEnd(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;

        IzFadeExit();
        main_work.proc_menu_update = dmStgSlctProcSetSlctStartData;
    }

    private static void dmStgSlctProcSetSlctStartData(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.proc_win_update = dmStgSlctProcWindowNodispIdle;

        if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ZONE_SLCT)
        {
            main_work.proc_input = dm_stgslct_variant.input_zone_select;
            main_work.proc_menu_update = dmStgSlctProcZoneSelectIdle;
        }
        else
        {
            main_work.proc_input = dm_stgslct_variant.input_stage_select;
            main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;
            main_work.disp_flag |= DMD_STGSLCT_DISP_FLAG_LR_ARROW;
        }

        main_work.zone_scr_id = 0;
    }

    private static void dmStgSlctProcZoneSelectIdle(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.proc_input?.Invoke(main_work);

        main_work.touch?.UpdateBackButtonZoneSelect(main_work);

        if ((main_work.flag & DMD_STGSLCT_FLAG_CANCEL) != 0)
        {
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);
            main_work.proc_win_input = null;
            main_work.proc_input = null;
            main_work.proc_win_update = null;
            main_work.proc_menu_update = dmStgSlctProcFadeOut;
            main_work.next_evt = (int)DME_STGSLCT_NEXT_EVT_MAINMENU;

            IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL, IZE_FADE_TYPE_BLACK_FADEOUT, DMD_STGSLCT_FADEOUT_TIME);

            DmSoundPlaySE("Cancel");
            return;
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE) != 0)
        {
            DmSoundPlaySE("Ok");
            main_work.proc_menu_update = dmStgSlctProcZoneSelectDecideEfct;
            main_work.flag |= DMD_STGSLCT_FLAG_DECIDE_ZONE_TABLE_EFCT;
            main_work.timer = 0;
            return;
        }

        dmStgSlctSetZoneScrChangeEfct(main_work);
    }

    private static void dmStgSlctProcZoneSelectDecideEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.timer++;

        dmStgSlctSetDecideZoneEfctPos(main_work);

        if (!dmStgSlctIsDecideZoneEfctPos(main_work))
        {
            dmStgSlctSetZoneScrChangeEfct(main_work);
            return;
        }

        main_work.proc_menu_update = dmStgSlctProcZoneSelectOutEfct;
        main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);

        for (int i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
        {
            if (main_work.cur_zone != i)
                main_work.efct_out_flag |= 1u << i;
        }

        main_work.decide_zone_efct_dist_x = 0;
        main_work.decide_zone_efct_dist_y = 0;
        main_work.cur_stage = (int)dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        main_work.prev_stage = main_work.cur_stage;
        main_work.proc_input = null;
        main_work.timer = 0;

        main_work.touch?.trg_act_move.ResetState();

        main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
        main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);
    }

    private static void dmStgSlctProcZoneSelectOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        dmStgSlctZoneActRange(main_work, main_work.cur_zone, out var act_start, out var act_end);

        if (dmStgSlctIsZonePosOutEfct(main_work))
        {
            main_work.proc_menu_update = dmStgSlctProcStageSelectInEfct;
            main_work.efct_time = 0;

            // parked at the all-open table regardless of is_final_open, as SEGA has it
            var parked = dm_stgslct_variant.zone_nodisp_pos_tbl[DMD_STGSLCT_ZONE_ALL_OPEN];

            for (int i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                if (main_work.cur_zone == i)
                    continue;

                main_work.zone_pos[i][0] = parked[i][0];
                main_work.zone_pos[i][1] = parked[i][1];
            }

            for (int i = act_start; i < act_end; i++)
            {
                main_work.act_top_pos_x[i] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
                main_work.act_top_pos_y[i] = main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE_SPE
                    ? dm_stgslct_variant.act_disp_y_pos_tbl[i]
                    : dm_stgslct_variant.act_tab_disp_y_pos_tbl[0];
            }

            main_work.state = (int)DME_STGSLCT_MODE_STATE_ACT_SLCT;
            main_work.mode_tex_move_frm = 0;

            if (main_work.touch != null)
            {
                main_work.touch.is_disp_cover = false;
                dmStgSlctStageSelectChngZoneSetInZoneScroll(main_work, 0);
            }

            return;
        }

        dmStgSlctSetZonePosOutEfct(main_work);

        if (main_work.efct_time == 20)
            main_work.efct_out_flag |= 1u << (int)main_work.cur_zone;

        main_work.efct_time++;

        dmStgSlctSetZoneScrChangeEfct(main_work);
    }

    private static void dmStgSlctProcStageSelectInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        dmStgSlctSetStagePosInEfct(main_work);

        if (dmStgSlctIsStagePosInEfct(main_work))
        {
            main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;

            if (dmStgSlctIsWindowIdle(main_work))
                main_work.proc_input = dm_stgslct_variant.input_stage_select;

            main_work.disp_flag |= DMD_STGSLCT_DISP_FLAG_LR_ARROW;
            main_work.crsr_idx = 0;

            main_work.crsr_pos_y = main_work.cur_zone != (uint)DME_STGSLCT_ACT_PAGE_FINAL
                ? dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_idx]
                : 320.0f;

            if (dm_stgslct_variant.is_console)
                main_work.focus_disp_no = 0;

            return;
        }

        main_work.mode_tex_move_frm++;

        if (main_work.mode_tex_move_frm > 5)
            main_work.mode_tex_move_frm = 5;
    }

    private static void dmStgSlctProcStageSelectIdle(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.proc_input?.Invoke(main_work);

        main_work.touch?.UpdateBackButtonStageSelect(main_work);

        if ((main_work.flag & DMD_STGSLCT_FLAG_CANCEL) != 0)
        {
            main_work.proc_menu_update = dmStgSlctProcStageSelectOutEfct;
            DmSoundPlaySE("Cancel");
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);
            main_work.proc_input = null;
            main_work.disp_flag &= unchecked((uint)~DMD_STGSLCT_DISP_FLAG_LR_ARROW);
            return;
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE) != 0)
        {
            if (!dmStgSlctIsCanSelectAct(main_work))
            {
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);

                if (main_work.touch != null)
                    main_work.touch.is_disp_cover = false;

                return;
            }

            DmSoundPlaySE("Ok");
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
            main_work.proc_win_update = dmStgSlctProcWindowOpenEfct;
            main_work.proc_input = null;
            main_work.proc_win_input = null;
            main_work.win_timer = 0;
            main_work.win_cur_slct = 0;
            main_work.win_mode = (int)DME_STGSLCT_WIN_STG_SLCT;

            if (main_work.touch != null)
            {
                main_work.touch.win_is_disp_cover = false;

                // hold the window shut for ten frames, and off zero so the open effect's
                // "timer == 0" test does not fire on the way past
                main_work.win_timer = -10.5f;
            }

            return;
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_CHANGE_EVT_RANKING) != 0)
        {
            main_work.proc_menu_update = dmStgSlctProcFadeOut;

            IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL, IZE_FADE_TYPE_BLACK_FADEOUT, DMD_STGSLCT_FADEOUT_TIME);

            main_work.next_evt = (int)DME_STGSLCT_NEXT_EVT_RANKING;
            DmSoundPlaySE("Ok");
            main_work.proc_input = null;
            main_work.proc_win_input = null;
            main_work.win_timer = 0;
            main_work.win_cur_slct = 0;
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CHANGE_EVT_RANKING);
            return;
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_ZONE) != 0)
        {
            main_work.proc_menu_update = dmStgSlctProcStageSelectChngZone;
            main_work.proc_input = dmStgSlctInputProcStageSelectChngZone;
            DmSoundPlaySE("Cursol");
            main_work.timer = 0;

            if (main_work.touch != null)
                dmStgSlctStageSelectChngZoneSetInZoneScroll(main_work, 0);

            return;
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR) != 0
            || (main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR) != 0)
        {
            main_work.proc_menu_update = dmStgSlctProcStageSelectChngVrtclAct;
            main_work.proc_input = dmStgSlctInputProcStageSelectMove;
            DmSoundPlaySE("Cursol");
            dmStgSlctSetFocusChangeEfctData(main_work);
            main_work.timer = 0;
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_UP_CHNG_CRSR);
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR);
            return;
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL) != 0)
        {
            main_work.proc_menu_update = dmStgSlctProcStageSelectChngVrtclAct;
            main_work.proc_input = dmStgSlctInputProcStageSelectMove;
            dmStgSlctSetFocusChangeEfctData(main_work);
            main_work.timer = 0;
        }
    }

    private static void dmStgSlctProcStageSelectChngZone(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.proc_input?.Invoke(main_work);

        if (!dmStgSlctIsStageZoneChangeEfct(main_work))
        {
            dmStgSlctSetStageZoneChangeEfct(main_work);
            return;
        }

        main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;

        if (dmStgSlctIsWindowIdle(main_work))
            main_work.proc_input = dm_stgslct_variant.input_stage_select;

        main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_ACT_CHNG_ZONE);
    }

    private static void dmStgSlctProcStageSelectChngVrtclAct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.proc_input?.Invoke(main_work);

        if ((main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR) != 0
            || (main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR) != 0)
        {
            dmStgSlctSetFocusChangeEfctData(main_work);
            DmSoundPlaySE("Cursol");
            main_work.timer = 0;
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_UP_CHNG_CRSR);
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR);
            return;
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL) == 0
            && (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_CRSR) == 0)
        {
            main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;

            if (dmStgSlctIsWindowIdle(main_work))
                main_work.proc_input = dm_stgslct_variant.input_stage_select;

            main_work.timer = 0;
            return;
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL) != 0)
        {
            dmStgSlctSetStageVrtclChangeEfct(main_work);

            if (dmStgSlctIsStageVrtclChangeEfct(main_work))
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL);
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_CRSR) != 0)
        {
            dmStgSlctSetStageCrsrChangeEfct(main_work);

            if (dmStgSlctIsStageCrsrChangeEfct(main_work))
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_ACT_CHNG_CRSR);
        }

        main_work.timer++;
    }

    private static void dmStgSlctProcStageSelectOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (dmStgSlctIsStagePosOutEfct(main_work))
        {
            main_work.proc_menu_update = dmStgSlctProcZoneSelectInEfct;
            main_work.state = (int)DME_STGSLCT_MODE_STATE_ZONE_SLCT;

            dmStgSlctSeedZonePos(main_work, visible: false);

            main_work.mode_tex_move_frm = 0;
            main_work.zone_scr_id = 0;

            if (main_work.touch != null)
                main_work.touch.is_disp_cover = false;

            return;
        }

        dmStgSlctSetStagePosOutEfct(main_work);

        main_work.mode_tex_move_frm++;

        if (main_work.mode_tex_move_frm > 10)
            main_work.mode_tex_move_frm = 10;
    }

    private static void dmStgSlctProcZoneSelectInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (!dmStgSlctIsZonePosInEfct(main_work))
        {
            dmStgSlctSetZonePosInEfct(main_work);
            return;
        }

        main_work.proc_menu_update = dmStgSlctProcZoneSelectIdle;

        dmStgSlctSeedZonePos(main_work, visible: true);

        if (dmStgSlctIsWindowIdle(main_work))
            main_work.proc_input = dm_stgslct_variant.input_zone_select;
    }

    private static void dmStgSlctProcWindowNodispIdle(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.proc_win_input?.Invoke(main_work);

        if ((main_work.flag & DMD_STGSLCT_FLAG_DISP_MENU) == 0 && main_work.announce_flag == 0)
            return;

        main_work.proc_win_update = dmStgSlctProcWindowOpenEfct;
        main_work.proc_input = null;
        main_work.proc_win_input = null;
        main_work.win_timer = 0;

        for (int i = (int)DME_STGSLCT_WIN_1_1_CLEAR; i < (int)DME_STGSLCT_WIN_NUM; i++)
        {
            if ((main_work.announce_flag & 1u << i) != 0)
            {
                main_work.win_mode = i;
                break;
            }
        }

        main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DISP_MENU);

        DmSoundPlaySE("Window");
    }

    private static void dmStgSlctProcWindowOpenEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if ((main_work.flag & DMD_STGSLCT_FLAG_WIN_EFCT_END) != 0)
        {
            main_work.proc_win_update = dmStgSlctProcWindowAnnounceIdle;
            main_work.proc_win_input = dm_stgslct_variant.input_win_disp_idle;

            main_work.touch?.ResetAnswers(main_work);

            main_work.disp_flag |= DMD_STGSLCT_DISP_FLAG_WIN_ACT;
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_WIN_EFCT_END);
        }
        else
        {
            dmStgSlctSetWinOpenEfct(main_work);
        }

        dmStgSlctWinSelectDraw(main_work);
    }

    private static void dmStgSlctProcWindowAnnounceIdle(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.proc_win_input?.Invoke(main_work);

        if (main_work.win_mode == (int)DME_STGSLCT_WIN_MENU)
            dmStgSlctWindowMenuChoice(main_work);
        else if (main_work.win_mode != (int)DME_STGSLCT_WIN_STG_SLCT)
            dmStgSlctWindowAnnounceDismiss(main_work);
        else
            dmStgSlctWindowStageChoice(main_work);

        dmStgSlctWinSelectDraw(main_work);
    }

    private static void dmStgSlctWindowMenuChoice(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE) != 0)
        {
            dmStgSlctLeaveWindow(main_work);
            main_work.proc_menu_update = dmStgSlctProcFadeOut;

            IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL, IZE_FADE_TYPE_BLACK_FADEOUT, DMD_STGSLCT_FADEOUT_TIME);

            main_work.next_evt = main_work.win_cur_slct == 0
                ? (int)DME_STGSLCT_NEXT_EVT_RANKING
                : (int)DME_STGSLCT_NEXT_EVT_MAINMENU;

            DmSoundPlaySE("Ok");
            dmStgSlctClearDecideCancel(main_work);
        }
        else if ((main_work.flag & DMD_STGSLCT_FLAG_CANCEL) != 0)
        {
            dmStgSlctCloseWindow(main_work);
            DmSoundPlaySE("Cancel");
            dmStgSlctClearDecideCancel(main_work);
        }
    }

    private static void dmStgSlctWindowAnnounceDismiss(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE) == 0
            && (main_work.flag & DMD_STGSLCT_FLAG_CANCEL) == 0)
        {
            return;
        }

        dmStgSlctCloseWindow(main_work);
        DmSoundPlaySE("Ok");
        dmStgSlctClearDecideCancel(main_work);
    }

    private static void dmStgSlctWindowStageChoice(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE) != 0 && main_work.win_cur_slct == 0)
        {
            dmStgSlctLeaveWindow(main_work);
            main_work.proc_menu_update = dmStgSlctProcFadeOut;

            IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL, IZE_FADE_TYPE_BLACK_FADEOUT, DMD_STGSLCT_FADEOUT_TIME);

            main_work.next_evt = main_work.cur_zone == (uint)DME_STGSLCT_ACT_PAGE_SPE
                ? (int)DME_STGSLCT_NEXT_EVT_SPESTE
                : (int)DME_STGSLCT_NEXT_EVT_MAINGAME;

            DmSndBgmPlayerExit();
            main_work.flag |= DMD_STGSLCT_FLAG_DEMO_SND_END;

            DmSoundPlaySE("Ok");
            dmStgSlctClearDecideCancel(main_work);
        }
        else if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE) != 0
            || (main_work.flag & DMD_STGSLCT_FLAG_CANCEL) != 0)
        {
            var cancelled = (main_work.flag & DMD_STGSLCT_FLAG_CANCEL) != 0;

            dmStgSlctCloseWindow(main_work);
            DmSoundPlaySE(cancelled ? "Cancel" : "Ok");
            dmStgSlctClearDecideCancel(main_work);
        }
    }

    private static void dmStgSlctLeaveWindow(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.proc_input = null;
        main_work.proc_win_input = null;
        main_work.win_timer = DMD_STGSLCT_WIN_EFCT_TIME;
    }

    private static void dmStgSlctCloseWindow(DMS_STGSLCT_MAIN_WORK main_work)
    {
        dmStgSlctLeaveWindow(main_work);

        main_work.disp_flag &= unchecked((uint)~DMD_STGSLCT_DISP_FLAG_WIN_ACT);
        main_work.proc_win_update = dmStgSlctProcWindowCloseEfct;
    }

    private static void dmStgSlctClearDecideCancel(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
        main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);
    }

    private static void dmStgSlctProcWindowCloseEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if ((main_work.flag & DMD_STGSLCT_FLAG_WIN_EFCT_END) != 0)
        {
            main_work.proc_win_update = dmStgSlctProcWindowNodispIdle;
            main_work.announce_flag &= unchecked((uint)~(1 << main_work.win_mode));

            if (main_work.announce_flag == 0)
            {
                if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ZONE_SLCT)
                {
                    main_work.proc_input = dm_stgslct_variant.input_zone_select;
                }
                else if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ACT_SLCT)
                {
                    main_work.proc_input = dm_stgslct_variant.input_stage_select;

                    if (main_work.touch != null)
                        main_work.touch.is_disp_cover = false;
                }
            }

            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_WIN_EFCT_END);
        }

        dmStgSlctSetWinCloseEfct(main_work);
        dmStgSlctWinSelectDraw(main_work);
    }

    private static void dmStgSlctProcFadeOut(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;

        main_work.proc_win_update = null;
        main_work.proc_menu_update = dmStgSlctProcStopDraw;
        main_work.proc_draw = null;
        main_work.timer = 0;
    }

    private static void dmStgSlctProcStopDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.proc_menu_update = dmStgSlctProcDataRelease;
    }

    private static void dmStgSlctProcDataRelease(DMS_STGSLCT_MAIN_WORK main_work)
    {
        for (int i = 0; i < (int)DME_STGSLCT_DATA_TYPE.DME_STGSLCT_DATA_TYPE_MAX; i++)
            AoTexRelease(main_work.tex[i]);

        for (int i = 0; i < 5; i++)
            AoTexRelease(main_work.cmn_tex[i]);

        main_work.proc_menu_update = dmStgSlctProcFinish;
    }

    private static void dmStgSlctProcFinish(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (!dmStgSlctIsTexRelease(main_work))
            return;

        main_work.touch?.Release();

        for (int i = 0; i < main_work.act.Length; i++)
        {
            if (main_work.act[i] == null)
                continue;

            AoActDelete(main_work.act[i]);
            main_work.act[i] = null;
        }

        for (int i = 0; i < 2; i++)
        {
            if (main_work.arc_amb[i] == null)
                continue;

            amMemFree(main_work.arc_amb[i]);
            main_work.arc_amb[i] = null;
        }

        for (int i = 0; i < 5; i++)
        {
            if (main_work.arc_cmn_amb[i] == null)
                continue;

            amMemFree(main_work.arc_cmn_amb[i]);
            main_work.arc_cmn_amb[i] = null;
        }

        main_work.proc_win_update = null;
        main_work.proc_menu_update = dmStgSlctProcWaitFinished;
    }

    private static void dmStgSlctProcWaitFinished(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if ((main_work.flag & DMD_STGSLCT_FLAG_DEMO_SND_END) != 0)
        {
            if (!DmSndBgmPlayerIsTaskExit())
                return;

            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DEMO_SND_END);
        }

        main_work.flag |= DMD_STGSLCT_FLAG_EXIT;
        main_work.proc_win_update = null;
        main_work.proc_menu_update = null;
    }
}
