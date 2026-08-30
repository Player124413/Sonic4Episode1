using er;

using static AppMain;

using static StgSlct.DME_STGSLCT_ACT_REG;
using static StgSlct.DME_STGSLCT_MODE_STATE;
using static StgSlct.DME_STGSLCT_PLAY_MODE;
using static StgSlct.DME_STGSLCT_WIN;
using static StgSlct.DME_STGSLCT_ZONE_TYPE;

internal static partial class StgSlct
{
    // The C++ wraps every sort flush on the mobile path in _am_sample_draw_enable and leaves the
    // console path unguarded. One helper rather than that pair of #ifs at a dozen sites.
    private static void dmStgSlctFlushSort()
    {
        if (dm_stgslct_variant.is_console || _am_sample_draw_enable)
        {
            AoActSortExecute();
            AoActSortDraw();
        }

        AoActSortUnregAll();
    }

    // dmStgSlctCommonFixDraw is the one place mobile flushes with the fix-sort instead
    private static void dmStgSlctFlushSortFix()
    {
        if (dm_stgslct_variant.is_console)
        {
            AoActSortExecute();
            AoActSortDraw();
        }
        else if (_am_sample_draw_enable)
        {
            AoActSortExecuteFix();
            AoActSortDraw();
        }

        AoActSortUnregAll();
    }

    private static void dmStgSlctSetFrame(
        DMS_STGSLCT_MAIN_WORK main_work, DME_STGSLCT_ACT_REG key, float frame)
    {
        AoActSetFrame(dmStgSlctAct(main_work, key), frame);
    }

    private static void dmStgSlctRegAction(DMS_STGSLCT_MAIN_WORK main_work, DME_STGSLCT_ACT_REG key)
    {
        AoActSortRegAction(dmStgSlctAct(main_work, key));
    }

    private static void dmStgSlctUpdateAct(
        DMS_STGSLCT_MAIN_WORK main_work, DME_STGSLCT_ACT_REG key, float frame)
    {
        AoActUpdate(dmStgSlctAct(main_work, key), frame);
    }

    private static void dmStgSlctProcActDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // the banner never stops moving, so it ticks here rather than in a state
        dmStgSlctSetObiEfctPos(main_work);

        dmStgSlctCommonDraw(main_work);
        dmStgSlctZoneSelectDraw(main_work);

        if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ACT_SLCT)
            dmStgSlctStageSelectDraw(main_work);

        dmStgSlctCommonFixDraw(main_work);

        // drawn without an action, because it needs an alpha gradient
        dmStgSlctMakeVertexAct(main_work, main_work.up_bg_vrtx);
    }

    private static void dmStgSlctCommonDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_BG);
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        // mobile parks cur_bg_id at 0xFFFFFFFF while no zone is highlighted and draws the plain
        // wave background instead of the four zone quads
        if (!dm_stgslct_variant.is_console && main_work.cur_bg_id == uint.MaxValue)
        {
            dmStgSlctRegAction(main_work, ACT_WAVE_BG);
            AoActAcmPush();
            AoActAcmInit();
            AoActAcmApplyFade(main_work.bg_fade);
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[0]));
            dmStgSlctUpdateAct(main_work, ACT_WAVE_BG, 1.0f);
            AoActAcmPop();
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                var act = main_work.act[dmStgSlctActIdx(ACT_ZONE_BG_LT) + i];

                AoActSortRegAction(act);
                AoActSetFrame(act, main_work.cur_bg_id);
                AoActAcmPush();
                AoActAcmInit();
                AoActAcmApplyFade(main_work.bg_fade);
                AoActUpdate(act, 0.0f);
                AoActAcmPop();
            }
        }

        dmStgSlctFlushSort();

        AoActSysSetDrawTaskPrio(0x2c00);

        for (int i = dmStgSlctActIdx(ACT_DOWN_BG); i <= dmStgSlctActIdx(ACT_BLUE_BG); i++)
            AoActSortRegAction(main_work.act[i]);

        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[0]));

        for (int i = dmStgSlctActIdx(ACT_DOWN_BG); i <= dmStgSlctActIdx(ACT_BLUE_BG); i++)
            AoActUpdate(main_work.act[i], 0.0f);

        dmStgSlctFlushSort();
    }

    private static void dmStgSlctSetSonicStockDispFrame(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // stage select shows one more life than the game does, deliberately
        var remaining = (int)main_work.player_stock - 1;

        if (remaining < 0)
            remaining = 0;

        if (remaining > (int)GSD_MAINSYS_PLAYER_REST_MAX - 1)
            remaining = (int)GSD_MAINSYS_PLAYER_REST_MAX - 1;

        var digits = new int[3];

        for (int j = 0; j < 3; j++)
        {
            var place = 1;

            for (int k = 0; k < 3 - j - 1; k++)
                place *= 10;

            if (remaining >= place)
            {
                digits[j] = remaining / place;
                remaining -= digits[j] * place;
            }
        }

        for (int j = 0; j < 3; j++)
            AoActSetFrame(main_work.act[dmStgSlctActIdx(ACT_REST_NUM_100) + j], digits[j]);
    }

    private static void dmStgSlctZoneSelectDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_ZONE);

        for (int i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
            dmStgSlctOneZoneTableDraw(main_work, i);

        // mobile draws no cursor at all
        if (!dm_stgslct_variant.is_console)
            return;

        if (main_work.state != (int)DME_STGSLCT_MODE_STATE_ZONE_SLCT)
            return;

        var decide = (main_work.flag & DMD_STGSLCT_FLAG_DECIDE_ZONE_TABLE_EFCT) != 0;
        var dist_x = decide ? main_work.decide_zone_efct_dist_x : 0;
        var dist_y = decide ? main_work.decide_zone_efct_dist_y : 0;

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        var cursor = dmStgSlctActIdx(ACT_TAB_ZONE_CURSOR2);

        for (int i = 0; i < 3; i++)
            AoActSortRegAction(main_work.act[cursor + i]);

        AoActAcmPush();
        AoActAcmInit();
        AoActAcmApplyTrans(
            main_work.zone_pos[main_work.cur_zone][0] + dist_x,
            main_work.zone_pos[main_work.cur_zone][1] + dist_y,
            0.0f);

        for (int i = 0; i < 3; i++)
            AoActUpdate(main_work.act[cursor + i]);

        AoActAcmPop();

        dmStgSlctFlushSort();
    }

    // the white-flash unlock effect hides the zone it is about to reveal
    private static bool dmStgSlctIsZoneHiddenByFlash(DMS_STGSLCT_MAIN_WORK main_work, int zone)
    {
        if ((main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT) == 0)
            return false;

        if ((main_work.announce_flag & 1u << (int)DME_STGSLCT_WIN_1_1_CLEAR) != 0)
            return zone != (int)DME_STGSLCT_ZONE_TYPE_1;

        if ((main_work.announce_flag & 1u << (int)DME_STGSLCT_WIN_FINAL_CAN_SLCT) != 0)
            return zone == (int)DME_STGSLCT_ZONE_TYPE_FINAL;

        if ((main_work.announce_flag & 1u << (int)DME_STGSLCT_WIN_SPESTE_CAN_SLCT) != 0)
            return zone == (int)DME_STGSLCT_ZONE_TYPE_SPE;

        return false;
    }

    private static void dmStgSlctOneZoneTableDraw(DMS_STGSLCT_MAIN_WORK main_work, int zone)
    {
        if (dmStgSlctIsZoneHiddenByFlash(main_work, zone))
            return;

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        dmStgSlctRegAction(main_work, ACT_TAB_ZONE_TAB);
        AoActSortRegAction(main_work.act[dmStgSlctActIdx(ACT_TAB_ZONE_SCR1) + zone]);

        if (zone != (int)DME_STGSLCT_ZONE_TYPE_SPE)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
            dmStgSlctRegAction(main_work, ACT_TAB_ZONE_TEXT);
        }
        else
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            dmStgSlctRegAction(main_work, ACT_TAB_ZONE_TEXT_S);
        }

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        // console dims every zone but the selected one. mobile only dims while a finger is down,
        // and animates all six rather than just the selected one.
        var unselected = main_work.cur_zone != zone;
        var covered = dm_stgslct_variant.is_console
            ? unselected
            : unselected && main_work.touch.is_disp_cover;

        if (covered)
        {
            dmStgSlctRegAction(main_work, ACT_TAB_ZONE_COVER2);
            dmStgSlctRegAction(main_work, ACT_TAB_ZONE_COVER1);
            dmStgSlctRegAction(main_work, ACT_TAB_ZONE_COVER3);
        }

        var scr = dmStgSlctActIdx(ACT_TAB_ZONE_SCR1) + zone;
        var scr_a = dmStgSlctActIdx(ACT_TAB_ZONE_SCR1_1a) + zone * 3;

        if (dm_stgslct_variant.is_console && unselected)
        {
            AoActSetFrame(main_work.act[scr], 0.0f);

            for (int i = 0; i < 3; i++)
                AoActSetFrame(main_work.act[scr_a + i], 0.0f);
        }
        else
        {
            AoActSetFrame(main_work.act[scr], main_work.zone_scr_id);

            if (zone < 4)
            {
                for (int i = 0; i < 3; i++)
                {
                    AoActSortRegAction(main_work.act[scr_a + i]);
                    AoActSetFrame(main_work.act[scr_a + i], main_work.zone_scr_id);
                }
            }
        }

        dmStgSlctSetFrame(main_work, ACT_TAB_ZONE_TEXT, zone);

        var decide = (main_work.flag & DMD_STGSLCT_FLAG_DECIDE_ZONE_TABLE_EFCT) != 0
            && main_work.cur_zone == zone;

        AoActAcmPush();
        AoActAcmInit();
        AoActAcmApplyTrans(
            main_work.zone_pos[zone][0] + (decide ? main_work.decide_zone_efct_dist_x : 0),
            main_work.zone_pos[zone][1] + (decide ? main_work.decide_zone_efct_dist_y : 0),
            0.0f);

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        dmStgSlctUpdateAct(main_work, ACT_TAB_ZONE_TAB, 0.0f);
        AoActUpdate(main_work.act[scr], 0.0f);

        main_work.touch?.trg_zone[zone].Update();

        for (int i = dmStgSlctActIdx(ACT_TAB_ZONE_SCR1_1a); i <= dmStgSlctActIdx(ACT_TAB_ZONE_SCR4_3a); i++)
            AoActUpdate(main_work.act[i], 0.0f);

        dmStgSlctUpdateAct(main_work, ACT_TAB_ZONE_COVER2, 0.0f);
        dmStgSlctUpdateAct(main_work, ACT_TAB_ZONE_COVER1, 0.0f);
        dmStgSlctUpdateAct(main_work, ACT_TAB_ZONE_COVER3, 0.0f);

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        dmStgSlctUpdateAct(main_work, ACT_TAB_ZONE_TEXT, 0.0f);
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        dmStgSlctUpdateAct(main_work, ACT_TAB_ZONE_TEXT_S, 0.0f);

        AoActAcmPop();

        dmStgSlctFlushSort();
    }

    private static void dmStgSlctCommonFixDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (dm_stgslct_variant.is_console)
            dmStgSlctCommonFixDrawPc(main_work);
        else
            dmStgSlctCommonFixDrawIphone(main_work);
    }

    // how far the down-icon row is nudged, and how many of them there are
    private static void dmStgSlctOpenZoneLayout(
        DMS_STGSLCT_MAIN_WORK main_work, out int open_zone_num, out float disp_dist)
    {
        if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
        {
            open_zone_num = 6;
            disp_dist = 0.0f;
        }
        else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0
            || (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
        {
            open_zone_num = 5;
            disp_dist = 24.0f;
        }
        else
        {
            open_zone_num = 4;
            disp_dist = 64.0f;
        }
    }

    private static void dmStgSlctDrawDownIcons(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.state != (int)DME_STGSLCT_MODE_STATE_ACT_SLCT)
            return;

        dmStgSlctOpenZoneLayout(main_work, out var open_zone_num, out var disp_dist);

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        var down = dmStgSlctActIdx(ACT_ICON_DOWN_1);

        for (int i = 0; i < open_zone_num; i++)
        {
            AoActSortRegAction(main_work.act[down + i]);

            var lit = main_work.cur_zone == i;

            // with special open but final closed, the special zone lights the final slot's icon
            if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0
                && (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) == 0
                && main_work.cur_zone == 5 && i == 4)
            {
                lit = true;
            }

            AoActSetFrame(main_work.act[down + i], lit ? 0.0f : 1.0f);

            AoActAcmPush();
            AoActAcmInit();
            AoActAcmApplyTrans(disp_dist, 0.0f, 0.0f);
            AoActUpdate(main_work.act[down + i], 0.0f);

            main_work.touch?.trg_act_tab[i].Update();

            AoActAcmPop();
        }
    }

    // the header block both arms draw: mode tabs, life counter, emeralds, zone name, back text
    private static void dmStgSlctRegisterFixCommon(DMS_STGSLCT_MAIN_WORK main_work)
    {
        AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_FIX);
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        for (int i = dmStgSlctActIdx(ACT_TAB_MODE_L); i <= dmStgSlctActIdx(ACT_TAB_EMER); i++)
            AoActSortRegAction(main_work.act[i]);

        var emer = dmStgSlctActIdx(ACT_ICON_EMER_1);

        for (int i = 0; i < 7; i++)
        {
            if ((main_work.get_emerald & 1u << i) != 0)
                AoActSortRegAction(main_work.act[emer + i]);
        }
    }

    private static void dmStgSlctRegisterModeTabs(DMS_STGSLCT_MAIN_WORK main_work, bool set_frames)
    {
        if (main_work.state != (int)DME_STGSLCT_MODE_STATE_ACT_SLCT)
            return;

        var score_mode = main_work.cur_game_mode != (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK;

        var first = score_mode ? ACT_TAB_STATE_L : ACT_TAB_STATE_L2;
        var last = score_mode ? ACT_TAB_STATE_R : ACT_TAB_STATE_R2;

        for (int i = dmStgSlctActIdx(first); i <= dmStgSlctActIdx(last); i++)
        {
            AoActSortRegAction(main_work.act[i]);

            if (set_frames)
                AoActSetFrame(main_work.act[i], main_work.mode_tex_frm);
        }
    }

    private static void dmStgSlctUpdateHeaderActs(DMS_STGSLCT_MAIN_WORK main_work)
    {
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        for (int i = dmStgSlctActIdx(ACT_TAB_MODE_L); i <= dmStgSlctActIdx(ACT_REST_NUM_1); i++)
            AoActUpdate(main_work.act[i], 0.0f);

        AoActAcmPush();

        for (int i = dmStgSlctActIdx(ACT_TAB_EMER); i <= dmStgSlctActIdx(ACT_ICON_EMER_7); i++)
        {
            AoActAcmInit();
            AoActAcmApplyTrans(0.0f, main_work.chaos_eme_pos_y, 0.0f);
            AoActUpdate(main_work.act[i], 0.0f);
        }

        AoActAcmPop();
    }

    private static void dmStgSlctDrawZoneName(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE_SPE)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
            dmStgSlctRegAction(main_work, ACT_TEX_ZONE_UP);
        }
        else
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            dmStgSlctRegAction(main_work, ACT_TEX_ZONE_UP_S);
        }
    }

    private static bool dmStgSlctIsWindowOpen(DMS_STGSLCT_MAIN_WORK main_work)
    {
        return main_work.win_size_rate[0] != 0.0f || main_work.win_size_rate[1] != 0.0f;
    }

    private static void dmStgSlctCommonFixDrawPc(DMS_STGSLCT_MAIN_WORK main_work)
    {
        dmStgSlctRegisterFixCommon(main_work);

        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[1]));

        if (!dmStgSlctIsWindowOpen(main_work))
            dmStgSlctRegAction(main_work, ACT_BTN_CANCEL1);

        if (main_work.mode_tex_move_frm == 5)
        {
            dmStgSlctRegAction(main_work, ACT_BTN_LB);
            dmStgSlctRegAction(main_work, ACT_BTN_MENU);
        }

        if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ACT_SLCT)
        {
            dmStgSlctRegAction(main_work, ACT_BTN_LB_ARROW);
            dmStgSlctRegAction(main_work, ACT_BTN_RB_ARROW);
            dmStgSlctSetFrame(main_work, ACT_BTN_LB_ARROW, main_work.btn_l_disp_frm);
            dmStgSlctSetFrame(main_work, ACT_BTN_RB_ARROW, main_work.btn_r_disp_frm);
        }

        if (main_work.btn_l_disp_frm < DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM)
            main_work.btn_l_disp_frm++;
        else
            main_work.btn_l_disp_frm = DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM;

        if (main_work.btn_r_disp_frm < DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM)
            main_work.btn_r_disp_frm++;
        else
            main_work.btn_r_disp_frm = DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM;

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        dmStgSlctRegisterModeTabs(main_work, set_frames: true);

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        dmStgSlctRegAction(main_work, ACT_TEX_SONIC);

        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

        if (main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK)
        {
            dmStgSlctRegAction(main_work, ACT_TEX_MODE_SCORE);
            dmStgSlctSetFrame(main_work, ACT_TEX_MODE_SCORE, main_work.mode_tex_move_frm);
            dmStgSlctRegAction(main_work, ACT_TEX_BIG_TIME);
            dmStgSlctSetFrame(main_work, ACT_TEX_BIG_TIME, main_work.mode_tex_frm);
            dmStgSlctRegAction(main_work, ACT_TEX_TIME_EFCT);
            dmStgSlctSetFrame(main_work, ACT_TEX_TIME_EFCT, main_work.mode_tex_frm);
        }
        else
        {
            dmStgSlctRegAction(main_work, ACT_TEX_MODE_TIME);
            dmStgSlctSetFrame(main_work, ACT_TEX_MODE_TIME, main_work.mode_tex_move_frm);
            dmStgSlctRegAction(main_work, ACT_TEX_BIG_SCORE);
            dmStgSlctSetFrame(main_work, ACT_TEX_BIG_SCORE, main_work.mode_tex_frm);
            dmStgSlctRegAction(main_work, ACT_TEX_SCORE_EFCT);
            dmStgSlctSetFrame(main_work, ACT_TEX_SCORE_EFCT, main_work.mode_tex_frm);
        }

        if (main_work.mode_tex_frm < DMD_STGSLCT_MODE_TEX_FRAME_MAX)
            main_work.mode_tex_frm++;

        dmStgSlctRegAction(main_work, ACT_TEX_MENU);

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        dmStgSlctRegAction(main_work, ACT_TEX_EMER);

        dmStgSlctDrawZoneName(main_work);

        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));

        if (!dmStgSlctIsWindowOpen(main_work))
            dmStgSlctRegAction(main_work, ACT_TEX_FIX_BACK);

        dmStgSlctSetSonicStockDispFrame(main_work);

        dmStgSlctSetFrame(main_work, ACT_BTN_CANCEL1, main_work.is_jp_region ? 0.0f : 1.0f);
        dmStgSlctSetFrame(main_work, ACT_TEX_MENU, main_work.mode_tex_move_frm);
        dmStgSlctSetFrame(main_work, ACT_TEX_ZONE_UP, main_work.cur_zone);

        dmStgSlctUpdateHeaderActs(main_work);

        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[1]));

        for (int i = dmStgSlctActIdx(ACT_BTN_CANCEL1); i <= dmStgSlctActIdx(ACT_BTN_MENU); i++)
            AoActUpdate(main_work.act[i], 0.0f);

        dmStgSlctUpdateAct(main_work, ACT_BTN_LB_ARROW, 0.0f);
        dmStgSlctUpdateAct(main_work, ACT_BTN_RB_ARROW, 0.0f);

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        AoActAcmPush();

        for (int i = dmStgSlctActIdx(ACT_TAB_STATE_L); i <= dmStgSlctActIdx(ACT_TAB_STATE_R2); i++)
        {
            AoActAcmInit();
            AoActAcmApplyTrans(0.0f, main_work.mode_tex_pos_y, 0.0f);
            AoActUpdate(main_work.act[i], 0.0f);
        }

        AoActAcmPop();

        for (int i = dmStgSlctActIdx(ACT_TEX_MODE_TIME); i <= dmStgSlctActIdx(ACT_TEX_EMER); i++)
        {
            if (i != dmStgSlctActIdx(ACT_TEX_EMER))
            {
                AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                AoActUpdate(main_work.act[i], 0.0f);
                continue;
            }

            AoActAcmPush();
            AoActAcmInit();
            AoActAcmApplyTrans(0.0f, main_work.chaos_eme_pos_y, 0.0f);
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
            AoActUpdate(main_work.act[i], 0.0f);
            AoActAcmPop();
        }

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        dmStgSlctUpdateAct(main_work, ACT_TEX_ZONE_UP, 0.0f);
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        dmStgSlctUpdateAct(main_work, ACT_TEX_ZONE_UP_S, 0.0f);

        AoActAcmPush();

        for (int i = dmStgSlctActIdx(ACT_TEX_BIG_TIME); i <= dmStgSlctActIdx(ACT_TEX_SCORE_EFCT); i++)
        {
            AoActAcmInit();
            AoActAcmApplyTrans(0.0f, main_work.mode_tex_pos_y, 0.0f);
            AoActUpdate(main_work.act[i], 0.0f);
        }

        AoActAcmPop();

        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
        dmStgSlctUpdateAct(main_work, ACT_TEX_FIX_BACK, 0.0f);

        dmStgSlctDrawDownIcons(main_work);

        dmStgSlctFlushSortFix();

        dmStgSlctDrawObi(main_work);

        dmStgSlctFlushSort();
    }

    private static void dmStgSlctSetDrawStageSelectTable(
        DMS_STGSLCT_MAIN_WORK main_work, uint zone, bool is_trg_update)
    {
        if (dm_stgslct_variant.is_console)
            dmStgSlctSetDrawStageSelectTablePc(main_work, zone);
        else
            dmStgSlctSetDrawStageSelectTableIphone(main_work, zone, is_trg_update);
    }

    // where row i of the given zone sits
    private static float dmStgSlctRowY(DMS_STGSLCT_MAIN_WORK main_work, int i, int start)
    {
        return main_work.act_top_pos_y[i]
            + (i - start) * (dm_stgslct_variant.act_table_dist_y - 8.0f);
    }

    private static void dmStgSlctRegisterScoreOrTime(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE_NORMAL)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            dmStgSlctRegAction(main_work, ACT_TAB_TEX_SCORE);
            return;
        }

        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        dmStgSlctRegAction(main_work, ACT_TAB_TEX_TIME);

        var digit = dmStgSlctActIdx(ACT_TAB_S_NUM1);

        for (int k = 0; k < 7; k++)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
            AoActSortRegAction(main_work.act[digit + k]);
        }
    }

    private static void dmStgSlctSetDrawStageSelectTablePc(DMS_STGSLCT_MAIN_WORK main_work, uint zone)
    {
        dmStgSlctZoneActRange(main_work, zone, out var start, out var end);

        var special = zone == (uint)DME_STGSLCT_ACT_PAGE.DME_STGSLCT_ACT_PAGE_SPE;

        for (int i = start; i < end; i++)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

            for (int j = dmStgSlctActIdx(ACT_TAB_TABLE2); j <= dmStgSlctActIdx(ACT_TAB_TABLE3); j++)
                AoActSortRegAction(main_work.act[j]);

            if (!special)
                dmStgSlctRegisterNormalRow(main_work, i);
            else
                dmStgSlctRegisterSpecialRow(main_work, i, start);

            dmStgSlctRegisterScoreOrTime(main_work);

            dmStgSlctSetTableActiveInfo(main_work, i);
            dmStgSlctSetFrame(main_work, ACT_TAB_ZONE_TEXT, dm_stgslct_act_table_disp_id_tbl[i]);
            dmStgSlctSetScoreDispFrame(main_work, zone, (uint)i);

            if (main_work.cur_stage != i)
            {
                AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
                dmStgSlctRegAction(main_work, ACT_TAB_COVER2);
                dmStgSlctRegAction(main_work, ACT_TAB_COVER1);
                dmStgSlctRegAction(main_work, ACT_TAB_COVER3);
            }

            var row_y = dmStgSlctRowY(main_work, i, start);

            AoActAcmPush();

            for (int j = dmStgSlctActIdx(ACT_TAB_START); j <= dmStgSlctActIdx(ACT_TAB_END); j++)
            {
                AoActAcmInit();
                AoActAcmApplyTrans(main_work.act_top_pos_x[i], row_y, 0.0f);

                var localised = j == dmStgSlctActIdx(ACT_TAB_TEXT)
                    || j == dmStgSlctActIdx(ACT_TAB_MESS)
                    || j >= dmStgSlctActIdx(ACT_TAB_TEX_SCORE)
                        && j <= dmStgSlctActIdx(ACT_TAB_TEX_SPE_STAGE);

                AoActSetTexture(AoTexGetTexList(localised ? main_work.tex[1] : main_work.tex[0]));
                AoActUpdate(main_work.act[j], 0.0f);
            }

            for (int j = dmStgSlctActIdx(ACT_TAB_COVER2); j <= dmStgSlctActIdx(ACT_TAB_COVER3); j++)
            {
                AoActAcmInit();
                AoActAcmApplyTrans(main_work.act_top_pos_x[i], row_y, 0.0f);
                AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
                AoActUpdate(main_work.act[j], 0.0f);
            }

            AoActAcmPop();

            AoActSortExecute();
            AoActSortDraw();
            AoActSortUnregAll();
        }
    }

    private static void dmStgSlctRegisterNormalRow(DMS_STGSLCT_MAIN_WORK main_work, int i)
    {
        dmStgSlctRegAction(main_work, ACT_TAB_SCR_BG);
        dmStgSlctRegAction(main_work, ACT_TAB_SCR);

        // stage 16 is the final zone's single act, and every fourth act is a boss
        var boss = i == 16 || (i + 1) % 4 == 0 && i != 0;

        if (boss)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            dmStgSlctRegAction(main_work, ACT_TAB_TEX_BOSS);
        }
        else
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            dmStgSlctRegAction(main_work, ACT_TAB_TEXT);
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
            dmStgSlctSetFrame(main_work, ACT_TAB_A_NUM, dm_stgslct_act_num_disp_id_tbl[i]);
            dmStgSlctRegAction(main_work, ACT_TAB_A_NUM);
        }

        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        dmStgSlctRegAction(main_work, ACT_TAB_MESS);
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        dmStgSlctRegAction(main_work, ACT_TAB_LINE);

        dmStgSlctRegisterEmeraldIcon(main_work, i);
    }

    private static void dmStgSlctRegisterEmeraldIcon(DMS_STGSLCT_MAIN_WORK main_work, int i)
    {
        for (int k = 0; k < 7; k++)
        {
            if (main_work.eme_stage_no[k] != i || (main_work.get_emerald & 1u << k) == 0)
                continue;

            dmStgSlctRegAction(main_work, ACT_TAB_ICON_EMER);
            dmStgSlctSetFrame(main_work, ACT_TAB_ICON_EMER, k);
            break;
        }
    }

    private static void dmStgSlctRegisterSpecialRow(
        DMS_STGSLCT_MAIN_WORK main_work, int i, int start)
    {
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        if (GsMainSysIsStageClear(GSD_MAIN_STAGE_ID_SS1 + (i - start)))
            dmStgSlctRegAction(main_work, ACT_TAB_ICON_SPE_EMER);

        dmStgSlctRegAction(main_work, ACT_TAB_NUM_SPE_STAGE);
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        dmStgSlctRegAction(main_work, ACT_TAB_TEX_SPE_STAGE);

        dmStgSlctSetFrame(main_work, ACT_TAB_ICON_SPE_EMER, i - start);
        dmStgSlctSetFrame(main_work, ACT_TAB_NUM_SPE_STAGE, dm_stgslct_act_num_disp_id_tbl[i]);
    }

    // mobile redraws each row through transient actions so it can batch by texture rather than by
    // row, which is why it walks the whole zone once per sprite instead of once per stage
    private static readonly CActionDraw dm_stgslct_act_draw_sys = new CActionDraw();

    private const int DMD_STGSLCT_SLOT_CMN = 0;
    private const int DMD_STGSLCT_SLOT_LNG = 1;

    private static void dmStgSlctEntryRow(
        DMS_STGSLCT_MAIN_WORK main_work, int slot, DME_STGSLCT_ACT_REG key, int i, int start)
    {
        var idx = dmStgSlctActIdx(key);

        dm_stgslct_act_draw_sys.Entry(
            main_work.ama[slot],
            dm_stgslct_variant.act_id_tbl[idx],
            main_work.act[idx].frame,
            main_work.act_top_pos_x[i],
            dmStgSlctRowY(main_work, i, start));
    }

    private static void dmStgSlctUpdateAndEntry(
        DMS_STGSLCT_MAIN_WORK main_work, int slot, DME_STGSLCT_ACT_REG key, int i, int start)
    {
        AoActSetTexture(AoTexGetTexList(main_work.tex[slot]));
        dmStgSlctUpdateAct(main_work, key, 0.0f);
        dmStgSlctEntryRow(main_work, slot, key, i, start);
    }

    private static void dmStgSlctFlushBatch()
    {
        dm_stgslct_act_draw_sys.Draw();
        dm_stgslct_act_draw_sys.Clear();
    }

    private static void dmStgSlctSetDrawStageSelectTableIphone(
        DMS_STGSLCT_MAIN_WORK main_work, uint zone, bool is_trg_update)
    {
        dmStgSlctZoneActRange(main_work, zone, out var start, out var end);

        var touch = main_work.touch;

        AoActSetTexture(AoTexGetTexList(main_work.tex[DMD_STGSLCT_SLOT_CMN]));

        for (int i = start; i < end; i++)
        {
            for (int j = dmStgSlctActIdx(ACT_TAB_TABLE2); j <= dmStgSlctActIdx(ACT_TAB_TABLE3); j++)
            {
                dmStgSlctSetTableActiveInfo(main_work, i);

                AoActAcmPush();
                AoActAcmInit();
                AoActAcmApplyTrans(
                    main_work.act_top_pos_x[i], dmStgSlctRowY(main_work, i, start), 0.0f);
                AoActUpdate(main_work.act[j], 0.0f);
                AoActAcmPop();

                dm_stgslct_act_draw_sys.Entry(
                    main_work.ama[DMD_STGSLCT_SLOT_CMN],
                    dm_stgslct_variant.act_id_tbl[j],
                    main_work.act[j].frame,
                    main_work.act_top_pos_x[i],
                    dmStgSlctRowY(main_work, i, start));
            }

            // drawing can reach here more than once a frame, so the trigger is pumped explicitly
            // by the one caller that asks for it
            if (is_trg_update && i - start < touch.trg_act.Length)
                touch.trg_act[i - start].Update();
        }

        dmStgSlctFlushBatch();

        if (zone != (uint)DME_STGSLCT_ACT_PAGE.DME_STGSLCT_ACT_PAGE_SPE)
            dmStgSlctDrawNormalRowsIphone(main_work, start, end);
        else
            dmStgSlctDrawSpecialRowsIphone(main_work, start, end);

        dmStgSlctDrawScoreOrTimeIphone(main_work, zone, start, end);

        dmStgSlctFlushBatch();

        for (int i = start; i < end; i++)
        {
            dmStgSlctSetScoreDispFrame(main_work, zone, (uint)i);
            dmStgSlctFlushSort();
        }

        for (int i = start; i < end; i++)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[DMD_STGSLCT_SLOT_CMN]));

            if (main_work.cur_stage == i || !touch.is_disp_cover)
                continue;

            dmStgSlctUpdateAct(main_work, ACT_TAB_COVER2, 0.0f);
            dmStgSlctUpdateAct(main_work, ACT_TAB_COVER1, 0.0f);
            dmStgSlctUpdateAct(main_work, ACT_TAB_COVER3, 0.0f);

            dmStgSlctEntryRow(main_work, DMD_STGSLCT_SLOT_CMN, ACT_TAB_COVER3, i, start);
            dmStgSlctEntryRow(main_work, DMD_STGSLCT_SLOT_CMN, ACT_TAB_COVER1, i, start);
            dmStgSlctEntryRow(main_work, DMD_STGSLCT_SLOT_CMN, ACT_TAB_COVER2, i, start);
        }

        dmStgSlctFlushBatch();
    }

    private static void dmStgSlctDrawNormalRowsIphone(
        DMS_STGSLCT_MAIN_WORK main_work, int start, int end)
    {
        for (int i = start; i < end; i++)
            dmStgSlctUpdateAndEntry(main_work, DMD_STGSLCT_SLOT_CMN, ACT_TAB_SCR_BG, i, start);

        for (int i = start; i < end; i++)
        {
            dmStgSlctSetTableActiveInfo(main_work, i);
            dmStgSlctUpdateAndEntry(main_work, DMD_STGSLCT_SLOT_CMN, ACT_TAB_SCR, i, start);
        }

        dmStgSlctFlushBatch();

        for (int i = start; i < end; i++)
        {
            var boss = i == 16 || (i + 1) % 4 == 0 && i != 0;

            dmStgSlctUpdateAndEntry(
                main_work, DMD_STGSLCT_SLOT_LNG, boss ? ACT_TAB_TEX_BOSS : ACT_TAB_TEXT, i, start);
        }

        for (int i = start; i < end; i++)
        {
            if (i == 16 || (i + 1) % 4 == 0 && i != 0)
                continue;

            AoActSetTexture(AoTexGetTexList(main_work.tex[DMD_STGSLCT_SLOT_CMN]));
            dmStgSlctSetFrame(main_work, ACT_TAB_A_NUM, dm_stgslct_act_num_disp_id_tbl[i]);
            dmStgSlctUpdateAct(main_work, ACT_TAB_A_NUM, 0.0f);
            dmStgSlctEntryRow(main_work, DMD_STGSLCT_SLOT_CMN, ACT_TAB_A_NUM, i, start);
        }

        dmStgSlctFlushBatch();

        for (int i = start; i < end; i++)
        {
            dmStgSlctSetTableActiveInfo(main_work, i);
            dmStgSlctUpdateAndEntry(main_work, DMD_STGSLCT_SLOT_LNG, ACT_TAB_MESS, i, start);
        }

        for (int i = start; i < end; i++)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[DMD_STGSLCT_SLOT_CMN]));
            dmStgSlctUpdateAct(main_work, ACT_TAB_LINE, 0.0f);
            dmStgSlctRegAction(main_work, ACT_TAB_LINE);
            dmStgSlctEntryRow(main_work, DMD_STGSLCT_SLOT_CMN, ACT_TAB_LINE, i, start);
        }

        dmStgSlctFlushBatch();

        for (int i = start; i < end; i++)
        {
            for (int k = 0; k < 7; k++)
            {
                if (main_work.eme_stage_no[k] != i || (main_work.get_emerald & 1u << k) == 0)
                    continue;

                dmStgSlctUpdateAct(main_work, ACT_TAB_ICON_EMER, 0.0f);
                dmStgSlctSetFrame(main_work, ACT_TAB_ICON_EMER, k);
                dmStgSlctEntryRow(main_work, DMD_STGSLCT_SLOT_CMN, ACT_TAB_ICON_EMER, i, start);
                break;
            }
        }

        dmStgSlctFlushBatch();
    }

    private static void dmStgSlctDrawSpecialRowsIphone(
        DMS_STGSLCT_MAIN_WORK main_work, int start, int end)
    {
        AoActSetTexture(AoTexGetTexList(main_work.tex[DMD_STGSLCT_SLOT_LNG]));

        for (int i = start; i < end; i++)
            dmStgSlctUpdateAndEntry(main_work, DMD_STGSLCT_SLOT_LNG, ACT_TAB_TEX_SPE_STAGE, i, start);

        dmStgSlctFlushBatch();

        AoActSetTexture(AoTexGetTexList(main_work.tex[DMD_STGSLCT_SLOT_CMN]));

        for (int i = start; i < end; i++)
        {
            if (!GsMainSysIsStageClear(GSD_MAIN_STAGE_ID_SS1 + (i - start)))
                continue;

            dmStgSlctSetFrame(main_work, ACT_TAB_ICON_SPE_EMER, i - start);
            dmStgSlctUpdateAct(main_work, ACT_TAB_ICON_SPE_EMER, 0.0f);
            dmStgSlctEntryRow(main_work, DMD_STGSLCT_SLOT_CMN, ACT_TAB_ICON_SPE_EMER, i, start);
        }

        for (int i = start; i < end; i++)
        {
            dmStgSlctSetFrame(main_work, ACT_TAB_NUM_SPE_STAGE, dm_stgslct_act_num_disp_id_tbl[i]);
            dmStgSlctUpdateAct(main_work, ACT_TAB_NUM_SPE_STAGE, 0.0f);
            dmStgSlctEntryRow(main_work, DMD_STGSLCT_SLOT_CMN, ACT_TAB_NUM_SPE_STAGE, i, start);
        }

        dmStgSlctFlushBatch();
    }

    private static void dmStgSlctDrawScoreOrTimeIphone(
        DMS_STGSLCT_MAIN_WORK main_work, uint zone, int start, int end)
    {
        var score = main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE_NORMAL;
        var label = score ? ACT_TAB_TEX_SCORE : ACT_TAB_TEX_TIME;

        AoActSetTexture(AoTexGetTexList(main_work.tex[DMD_STGSLCT_SLOT_LNG]));

        for (int i = start; i < end; i++)
            dmStgSlctUpdateAndEntry(main_work, DMD_STGSLCT_SLOT_LNG, label, i, start);

        var digit = dmStgSlctActIdx(ACT_TAB_S_NUM1);

        for (int i = start; i < end; i++)
        {
            dmStgSlctSetScoreDispFrame(main_work, zone, (uint)i);

            var first = 0;

            // score skips the leading zeroes; time always draws its seven glyphs
            if (score)
            {
                while (first < DMD_STGSLCT_DISP_SCORE_DIGIT_NUM
                    && main_work.act[digit + first].frame == 0.0f)
                {
                    first++;
                }
            }

            var last = score ? DMD_STGSLCT_DISP_SCORE_DIGIT_NUM : 7;

            for (int k = first; k < last; k++)
            {
                AoActSetTexture(AoTexGetTexList(main_work.tex[DMD_STGSLCT_SLOT_CMN]));
                AoActUpdate(main_work.act[digit + k], 0.0f);

                dm_stgslct_act_draw_sys.Entry(
                    main_work.ama[DMD_STGSLCT_SLOT_CMN],
                    dm_stgslct_variant.act_id_tbl[digit + k],
                    main_work.act[digit + k].frame,
                    main_work.act_top_pos_x[i],
                    dmStgSlctRowY(main_work, i, start));
            }
        }
    }

    // The gradient strips behind the top and bottom of the screen, two columns by two rows. SEGA
    // writes all four out longhand, each with its own copy of the blend state; only these six
    // numbers and the texture step actually differ.
    // x0, x1, y, first y offset, second y offset, then the three v coordinates.
    private static readonly float[][] dm_stgslct_bg_quad_tbl = [
        [-160.0f,  864.0f,   0.0f,   64.0f,  128.0f, 0.0f,    0.125f,  0.25f],
        [-160.0f,  864.0f, 720.0f, -128.0f, -192.0f, 0.8125f, 0.3125f, 0.0625f],
        [ 864.0f, 1120.0f,   0.0f,   64.0f,  128.0f, 0.0f,    0.125f,  0.25f],
        [ 864.0f, 1120.0f, 720.0f, -128.0f, -192.0f, 0.8125f, 0.3125f, 0.0625f],
    ];

    private const int DMD_STGSLCT_BG_TEX_BASE_CONSOLE = 42;
    private const int DMD_STGSLCT_BG_TEX_BASE_MOBILE = 35;

    private static void dmStgSlctMakeVertexAct(
        DMS_STGSLCT_MAIN_WORK main_work, AMS_PARAM_DRAW_PRIMITIVE param)
    {
        if (main_work.bg_fade.a > 0)
        {
            amDrawSetFogColor(120, 1.0f, 1.0f, 1.0f);

            var n = 2.0f - main_work.bg_fade.a / 255.0f;

            amDrawSetFogRange(120, n, n + 1.0f);
            amDrawSetFog(120, 1);
        }
        else
        {
            amDrawSetFog(120, 0);
        }

        for (int quad = 0; quad < dm_stgslct_bg_quad_tbl.Length; quad++)
            dmStgSlctDrawBgQuad(main_work, param, quad);
    }

    private static void dmStgSlctDrawBgQuad(
        DMS_STGSLCT_MAIN_WORK main_work, AMS_PARAM_DRAW_PRIMITIVE param, int quad)
    {
        var q = dm_stgslct_bg_quad_tbl[quad];

        param.mtx = null;
        param.vtxPCT3D = amDrawAlloc_NNS_PRIM3D_PCT(6);

        var v = param.vtxPCT3D.buffer;

        v[0].Pos.x = v[2].Pos.x = v[4].Pos.x = q[0];
        v[1].Pos.x = v[3].Pos.x = v[5].Pos.x = q[1];
        v[0].Pos.y = v[1].Pos.y = q[2];
        v[2].Pos.y = v[3].Pos.y = q[2] + q[3];
        v[4].Pos.y = v[5].Pos.y = q[2] + q[4];
        v[0].Pos.z = v[1].Pos.z = v[2].Pos.z = v[3].Pos.z = v[4].Pos.z = v[5].Pos.z = -2.0f;

        v[0].Col = v[2].Col = AMD_RGBA8888(255, 255, 255, 255);
        v[1].Col = v[3].Col = AMD_RGBA8888(255, 255, 255, 255);
        v[4].Col = v[5].Col = AMD_RGBA8888(255, 255, 255, 0);

        v[0].Tex.u = v[2].Tex.u = v[4].Tex.u = 0.0f;
        v[1].Tex.u = v[3].Tex.u = v[5].Tex.u = 1.0f;
        v[0].Tex.v = v[1].Tex.v = q[5];
        v[2].Tex.v = v[3].Tex.v = q[6];
        v[4].Tex.v = v[5].Tex.v = q[7];

        param.format3D = NNE_PRIM3D_FMT_PCT;
        param.type = NNE_PRIM_TRIANGLE_STRIP;
        param.count = 6;
        param.texlist = AoTexGetTexList(main_work.tex[0]);

        // mobile draws the strip fully transparent and untextured while no zone is highlighted
        if (!dm_stgslct_variant.is_console && main_work.cur_bg_id == uint.MaxValue)
        {
            for (int i = 0; i < 6; i++)
                v[i].Col &= 0xFFFFFF00;

            param.texlist = null;
            param.texId = -1;
        }
        else
        {
            var tex_base = dm_stgslct_variant.is_console
                ? DMD_STGSLCT_BG_TEX_BASE_CONSOLE
                : DMD_STGSLCT_BG_TEX_BASE_MOBILE;

            param.texId = (int)(tex_base + quad * 6 + main_work.cur_bg_id);
        }

        param.ablend = NNE_PRIM_ALPHABLEND_ON;
        param.zOffset = -1.0f;
        param.uwrap = NNE_PRIM_TEXWRAP_CLAMP;
        param.vwrap = NNE_PRIM_TEXWRAP_CLAMP;
        param.sortZ = 0.0f;

        // TODO(port): the _PC/_XBOX arm of dmStgSlct.cpp:9080. NNE_BLENDMODE_* and NNE_BLENDOP_*
        // are not in this repo yet, so the primitive keeps whatever blend state the allocator
        // hands it. The PS3 and Wii arms are dropped, we do not target either.
        //param.bldSrc = NNE_BLENDMODE_SRCALPHA;
        //param.bldDst = NNE_BLENDMODE_INVSRCALPHA;
        //param.bldMode = NNE_BLENDOP_ADD;

        param.aTest = 0;
        param.zMask = 1;
        param.zTest = 0;
        param.noSort = 1;

        AoActDrawCorWide(param.vtxPCT3D, 6, AOE_ACT_CORW.AOD_ACT_CORW_CENTER);
        amDrawPrimitive3D(120, param);
    }

    private static void dmStgSlctWinSelectDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_WIN_FIX);

        var win_w = dm_stgslct_variant.window_size_w;
        var win_h = DMD_STGSLCT_WINDOW_SIZE_H;

        if (!dm_stgslct_variant.is_console)
        {
            win_w *= DMD_STGSLCT_DISP_SCALE_TEXT_MOBILE;
            win_h *= DMD_STGSLCT_DISP_SCALE_TEXT_MOBILE;

            // the act-confirm window is drawn a shade larger than the rest
            if (main_work.win_mode == (int)DME_STGSLCT_WIN_STG_SLCT)
            {
                win_w *= 1.01f;
                win_h *= 1.27f;
            }
        }

        // mobile skips the frame entirely until the open effect has some size to it
        if (dm_stgslct_variant.is_console
            || main_work.win_size_rate[0] > 0.0f && main_work.win_size_rate[1] > 0.0f)
        {
            AoWinSysDrawTask(
                AOD_WIN_TYPE_A,
                AoTexGetTexList(main_work.cmn_tex[3]),
                0,
                DMD_STGSLCT_SIZE_WIDTH / 2.0f,
                DMD_STGSLCT_SIZE_HEIGHT / 2.0f,
                win_w * main_work.win_size_rate[0],
                win_h * main_work.win_size_rate[1],
                (ushort)dm_stgslct_variant.draw_prio_win);
        }

        if ((main_work.disp_flag & DMD_STGSLCT_DISP_FLAG_WIN_ACT) == 0)
            return;

        dmStgSlctRegisterWindowContents(main_work);

        dmStgSlctSetFrame(main_work, ACT_BTN_CANCEL2, main_work.is_jp_region ? 0.0f : 1.0f);

        dmStgSlctUpdateWindowActs(main_work);

        dmStgSlctFlushSort();
    }

    private static void dmStgSlctRegisterWindowContents(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var console = dm_stgslct_variant.is_console;
        var frm = dm_stgslct_win_act_frm_tbl[main_work.win_mode];

        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));

        if (main_work.win_mode == (int)DME_STGSLCT_WIN_MENU)
        {
            if (!console)
                return;

            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[1]));
            dmStgSlctRegAction(main_work, ACT_BTN_CANCEL2);
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
            dmStgSlctRegAction(main_work, ACT_TEX_BACK1);

            dmStgSlctSetFrame(main_work, ACT_TEX_WINTITLE, frm[0]);
            dmStgSlctSetFrame(main_work, ACT_BTN_CANCEL2, 0.0f);
            dmStgSlctSetFrame(main_work, ACT_TEX_BACK1, 0.0f);
            return;
        }

        if (main_work.win_mode == (int)DME_STGSLCT_WIN_STG_SLCT)
        {
            if (console)
            {
                AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[1]));
                dmStgSlctRegAction(main_work, ACT_BTN_CANCEL2);
            }
            else
            {
                AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));

                for (int i = (int)DME_STGSLCT_ACT.ACT_YES_BTN_L;
                     i <= (int)DME_STGSLCT_ACT.ACT_NO_BTN_R; i++)
                {
                    AoActSortRegAction(main_work.act[i]);
                }
            }

            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            dmStgSlctRegAction(main_work, ACT_WIN_TEX_MSG2);
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
            dmStgSlctRegAction(main_work, ACT_TEX_YES);
            dmStgSlctRegAction(main_work, ACT_TEX_NO);

            if (console)
            {
                dmStgSlctRegAction(main_work, ACT_TEX_BACK1);
                dmStgSlctSetFrame(main_work, ACT_TEX_WINTITLE, frm[0]);
            }

            dmStgSlctSetFrame(
                main_work,
                ACT_WIN_TEX_MSG2,
                main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE_SPE ? 0.0f : 1.0f);

            if (console)
            {
                var slct = dm_stgslct_win_disp_slct_frm_tbl[main_work.win_cur_slct];

                dmStgSlctSetFrame(main_work, ACT_TEX_YES, slct[0]);
                dmStgSlctSetFrame(main_work, ACT_TEX_NO, slct[1]);
                dmStgSlctSetFrame(main_work, ACT_BTN_CANCEL2, 0.0f);
                dmStgSlctSetFrame(main_work, ACT_TEX_BACK1, 0.0f);
            }

            return;
        }

        // the seven announce windows differ only in which row of dm_stgslct_win_act_frm_tbl they
        // read, and that row is win_mode, so SEGA's seven identical cases collapse to this one
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        dmStgSlctRegAction(main_work, ACT_WIN_TEX_MSG);

        if (console)
        {
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
            dmStgSlctRegAction(main_work, ACT_TEX_OK);
            dmStgSlctSetFrame(main_work, ACT_TEX_WINTITLE, frm[0]);
        }

        dmStgSlctSetFrame(main_work, ACT_WIN_TEX_MSG, frm[1]);

        if (console)
            dmStgSlctSetFrame(main_work, ACT_TEX_OK, 0.0f);
    }

    private static void dmStgSlctUpdateWindowActs(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var pos = dm_stgslct_variant.win_act_pos_tbl;

        if (dm_stgslct_variant.is_console)
        {
            AoActAcmPush();
            AoActAcmInit();
            AoActAcmApplyTrans(pos[0][0], pos[0][1], 0.0f);
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
            dmStgSlctUpdateAct(main_work, ACT_WIN_LINE, 0.0f);
            AoActAcmPop();
        }

        AoActAcmPush();

        var cancel = dmStgSlctActIdx(ACT_BTN_CANCEL2);

        for (int k = 0; k < 3; k++)
        {
            AoActAcmInit();
            AoActAcmApplyTrans(pos[k + 1][0], pos[k + 1][1], 0.0f);

            // the back text shifts by however wide the word is in this language
            if (k == 0)
                AoActAcmApplyTrans(dm_stgslct_back_text_length_tbl[GsEnvGetLanguage()], 0.0f, 0.0f);

            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[1]));
            AoActUpdate(main_work.act[cancel + k], 0.0f);
        }

        AoActAcmPop();
        AoActAcmPush();

        var msg = dmStgSlctActIdx(ACT_WIN_TEX_MSG);

        for (int k = 0; k < 5; k++)
        {
            AoActAcmInit();
            AoActAcmApplyTrans(pos[4 + k][0], pos[4 + k][1], 0.0f);

            if (!dm_stgslct_variant.is_console)
            {
                AoActAcmApplyScale(
                    DMD_STGSLCT_DISP_SCALE_TEXT_MOBILE, DMD_STGSLCT_DISP_SCALE_TEXT_MOBILE);
            }

            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            AoActUpdate(main_work.act[msg + k], 0.0f);
        }

        AoActAcmPop();

        dmStgSlctUpdateWindowText(main_work);
    }

    // console walks WINTITLE..NO against positions 9..13; mobile has no window title or OK, so it
    // walks BACK1..NO against 11..13
    private static void dmStgSlctUpdateWindowText(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var console = dm_stgslct_variant.is_console;
        var pos = dm_stgslct_variant.win_act_pos_tbl;
        var touch = main_work.touch;

        var first = console ? dmStgSlctActIdx(ACT_TEX_WINTITLE) : dmStgSlctActIdx(ACT_TEX_BACK1);
        var count = console ? 5 : 3;
        var pos_first = console ? 9 : 11;

        AoActAcmPush();

        for (int k = 0; k < count; k++)
        {
            AoActAcmInit();
            AoActAcmApplyTrans(pos[pos_first + k][0], pos[pos_first + k][1], 0.0f);

            if (main_work.win_mode == (int)DME_STGSLCT_WIN_SSONIC_SLCT && k == 2)
                AoActAcmApplyTrans(0.0f, 16.0f, 0.0f);

            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
            AoActUpdate(main_work.act[first + k], 0.0f);

            if (touch == null)
                continue;

            if (first + k == (int)DME_STGSLCT_ACT.ACT_TEX_YES)
                dmStgSlctUpdateAnswerSprites(main_work, touch.trg_answer[1], true);
            else if (first + k == (int)DME_STGSLCT_ACT.ACT_TEX_NO)
                dmStgSlctUpdateAnswerSprites(main_work, touch.trg_answer[0], false);
        }

        AoActAcmPop();
    }

    private static void dmStgSlctUpdateAnswerSprites(
        DMS_STGSLCT_MAIN_WORK main_work, CTrgAoAction answer, bool yes)
    {
        var first = yes ? (int)DME_STGSLCT_ACT.ACT_YES_BTN_L : (int)DME_STGSLCT_ACT.ACT_NO_BTN_L;
        var last = yes ? (int)DME_STGSLCT_ACT.ACT_YES_BTN_R : (int)DME_STGSLCT_ACT.ACT_NO_BTN_R;

        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));

        for (int i = first; i <= last; i++)
            AoActUpdate(main_work.act[i], main_work.act[i].frame >= 2.0f ? 1.0f : 0.0f);

        answer.Update();
    }

    private static void dmStgSlctSetScoreDispFrame(
        DMS_STGSLCT_MAIN_WORK main_work, uint zone, uint act_no)
    {
        if (main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE_NORMAL)
            dmStgSlctSetScoreDigits(main_work, (int)act_no);
        else
            dmStgSlctSetTimeDigits(main_work, (int)act_no);
    }

    private static void dmStgSlctSetScoreDigits(DMS_STGSLCT_MAIN_WORK main_work, int act_no)
    {
        var digit = dmStgSlctActIdx(ACT_TAB_S_NUM1);

        if (main_work.hi_score[act_no] == DMD_STGSLCT_INIT_SCORE_NUM)
        {
            for (int k = 0; k < 9; k++)
            {
                AoActSortRegAction(main_work.act[digit + k]);
                AoActSetFrame(main_work.act[digit + k], DMD_STGSLCT_NO_ACTIVE_NUM_ID);
            }

            return;
        }

        var value = main_work.hi_score[act_no];
        var digits = new int[DMD_STGSLCT_DISP_SCORE_DIGIT_NUM];
        var leading = false;

        for (int j = 0; j < DMD_STGSLCT_DISP_SCORE_DIGIT_NUM; j++)
        {
            var place = 1;

            if (DMD_STGSLCT_DISP_SCORE_DIGIT_NUM - j - 1 <= 0)
            {
                value = 1;
            }
            else
            {
                for (int k = 0; k < DMD_STGSLCT_DISP_SCORE_DIGIT_NUM - j - 1; k++)
                    place *= 10;
            }

            if (value >= place)
            {
                AoActSortRegAction(main_work.act[digit + j]);

                if (j >= DMD_STGSLCT_DISP_SCORE_DIGIT_NUM - 1)
                {
                    digits[j] = 0;
                }
                else
                {
                    digits[j] = value / place;

                    // the -1 is SEGA's, not a transcription slip
                    value -= digits[j] * place - 1;
                }

                leading = true;
            }
            else
            {
                if (leading)
                    AoActSortRegAction(main_work.act[digit + j]);

                digits[j] = 0;
            }
        }

        for (int k = 0; k < 9; k++)
            AoActSetFrame(main_work.act[digit + k], digits[k]);
    }

    private static void dmStgSlctSetTimeDigits(DMS_STGSLCT_MAIN_WORK main_work, int act_no)
    {
        var digit = dmStgSlctActIdx(ACT_TAB_S_NUM1);

        if (main_work.record_time[act_no] == DMD_STGSLCT_INIT_RECORD_TIME_NUM)
        {
            for (int k = 0; k < 7; k++)
            {
                AoActSetFrame(
                    main_work.act[digit + k],
                    k == 1 || k == 4 ? DMD_STGSLCT_TIME_COLON_ID : DMD_STGSLCT_NO_ACTIVE_NUM_ID);
            }

            return;
        }

        ushort min = 0;
        ushort sec = 0;
        ushort msec = 0;

        AkUtilFrame60ToTime((uint)main_work.record_time[act_no], ref min, ref sec, ref msec);

        AoActSetFrame(main_work.act[digit + 0], min);
        AoActSetFrame(main_work.act[digit + 1], DMD_STGSLCT_TIME_COLON_ID);
        AoActSetFrame(main_work.act[digit + 2], sec / 10);
        AoActSetFrame(main_work.act[digit + 3], sec % 10);
        AoActSetFrame(main_work.act[digit + 4], DMD_STGSLCT_TIME_COLON_ID);
        AoActSetFrame(main_work.act[digit + 5], msec / 10);
        AoActSetFrame(main_work.act[digit + 6], msec % 10);
    }

    private static void dmStgSlctStageSelectDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_STAGE);
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        dmStgSlctSetDrawStageSelectTable(main_work, main_work.cur_zone, is_trg_update: true);

        // console keeps drawing the outgoing zone whenever one is set; mobile only while the
        // change is actually animating
        var draw_outgoing = main_work.chng_zone != (uint)DME_STGSLCT_ACT_PAGE.DME_STGSLCT_ACT_PAGE_NONE
            && (dm_stgslct_variant.is_console
                || (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_ZONE) != 0);

        if (draw_outgoing)
            dmStgSlctSetDrawStageSelectTable(main_work, main_work.chng_zone, is_trg_update: false);

        main_work.touch?.trg_act_move.Update();

        if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ACT_SLCT
            && main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE_SPE)
        {
            main_work.disp_flag |= DMD_STGSLCT_DISP_FLAG_ACT_CRSR;
        }
        else
        {
            main_work.disp_flag &= unchecked((uint)~DMD_STGSLCT_DISP_FLAG_ACT_CRSR);
        }

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        dmStgSlctRegisterScrollArrows(main_work);

        AoActAcmPush();

        // console nudges its three-piece cursor across and mobile has only the two arrows
        var last = dm_stgslct_variant.is_console ? ACT_TAB_CURSOR_3 : ACT_TAB_CURSOR_DOWN;

        for (int i = dmStgSlctActIdx(ACT_TAB_CURSOR_UP); i <= dmStgSlctActIdx(last); i++)
        {
            AoActAcmInit();

            if (dm_stgslct_variant.is_console)
                AoActAcmApplyTrans(312.0f, 0.0f, 0.0f);

            AoActUpdate(main_work.act[i], 1.0f);
        }

        AoActAcmPop();

        dmStgSlctFlushSort();

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        if ((main_work.disp_flag & DMD_STGSLCT_DISP_FLAG_LR_ARROW) == 0)
            return;

        var arrow = dmStgSlctActIdx(ACT_ICON_L_ARROW);

        for (int i = 0; i < 2; i++)
        {
            AoActSortRegAction(main_work.act[arrow + i]);
            AoActUpdate(main_work.act[arrow + i], 1.0f);
        }

        if (main_work.touch != null)
        {
            foreach (var lr in main_work.touch.trg_act_lr)
                lr.Update();
        }

        dmStgSlctFlushSort();
    }

    // console shows the up/down pair only on the special stage list; mobile shows whichever end
    // of the scroll it can still travel toward
    private static void dmStgSlctRegisterScrollArrows(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (dm_stgslct_variant.is_console)
        {
            if ((main_work.disp_flag & DMD_STGSLCT_DISP_FLAG_ACT_CRSR) == 0)
                return;

            dmStgSlctRegAction(main_work, ACT_TAB_CURSOR_UP);
            dmStgSlctRegAction(main_work, ACT_TAB_CURSOR_DOWN);
            return;
        }

        if ((main_work.disp_flag & DMD_STGSLCT_DISP_FLAG_LR_ARROW) == 0
            || main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE_FINAL)
        {
            return;
        }

        var changing = (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL) != 0;

        if (main_work.focus_disp_no > 0 || changing)
            dmStgSlctRegAction(main_work, ACT_TAB_CURSOR_UP);

        var move_limit = main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE_SPE ? 4 : 1;

        if (main_work.focus_disp_no < move_limit || changing)
            dmStgSlctRegAction(main_work, ACT_TAB_CURSOR_DOWN);
    }

    // console only: the scrolling banner across the bottom
    private static void dmStgSlctDrawObi(DMS_STGSLCT_MAIN_WORK main_work)
    {
        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[2]));

        for (int i = dmStgSlctActIdx(ACT_OBI_C); i <= dmStgSlctActIdx(ACT_OBI_R); i++)
            AoActSortRegAction(main_work.act[i]);

        for (int i = dmStgSlctActIdx(ACT_OBI_C); i <= dmStgSlctActIdx(ACT_OBI_R); i++)
            AoActUpdate(main_work.act[i], 0.0f);

        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

        var obi = dmStgSlctActIdx(ACT_TEX_OBI1);
        var spe_act_select = main_work.state == (int)DME_STGSLCT_MODE_STATE_ACT_SLCT
            && main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE_SPE;

        // two copies so the scroll can loop
        for (int i = 0; i < 2; i++)
        {
            AoActSortRegAction(main_work.act[obi + i]);
            AoActSetFrame(main_work.act[obi + i], spe_act_select ? 2.0f : main_work.state);

            AoActAcmPush();
            AoActAcmInit();
            AoActAcmApplyTrans(main_work.obi_pos[i], 0.0f, 0.0f);
            AoActAcmApplyScale(
                DMD_STGSLCT_OBI_MSG_SCALE_SIZE_CONSOLE, DMD_STGSLCT_OBI_MSG_SCALE_SIZE_CONSOLE);
            AoActUpdate(main_work.act[obi + i], 0.0f);
            AoActAcmPop();
        }
    }

    // mobile forces the localised zone name while nothing is highlighted and during the zone
    // select entry effect, otherwise the wrong zone's name shows on the way back in
    private static bool dmStgSlctUseLocalisedZoneName(DMS_STGSLCT_MAIN_WORK main_work)
    {
        return main_work.cur_bg_id == uint.MaxValue
            || main_work.state == (int)DME_STGSLCT_MODE_STATE_ZONE_SLCT;
    }

    private static void dmStgSlctCommonFixDrawIphone(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var touch = main_work.touch;

        dmStgSlctRegisterFixCommon(main_work);

        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[1]));

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        dmStgSlctRegisterModeTabs(main_work, set_frames: false);

        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

        dmStgSlctRegAction(
            main_work,
            main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK
                ? ACT_TEX_BIG_TIME
                : ACT_TEX_BIG_SCORE);

        main_work.mode_tex_frm = 0.0f;

        if (dmStgSlctUseLocalisedZoneName(main_work))
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            dmStgSlctRegAction(main_work, ACT_TEX_ZONE_UP_S);
        }
        else
        {
            dmStgSlctDrawZoneName(main_work);
        }

        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));

        if (!dmStgSlctIsWindowOpen(main_work))
        {
            dmStgSlctRegAction(main_work, ACT_TEX_FIX_BACK);

            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));

            for (int i = (int)DME_STGSLCT_ACT.ACT_BACK_BTN01_L;
                 i <= (int)DME_STGSLCT_ACT.ACT_BACK_BTN01_R; i++)
            {
                AoActSortRegAction(main_work.act[i]);
            }
        }

        dmStgSlctSetSonicStockDispFrame(main_work);

        dmStgSlctSetFrame(main_work, ACT_BTN_CANCEL1, main_work.is_jp_region ? 0.0f : 1.0f);
        dmStgSlctSetFrame(main_work, ACT_TEX_ZONE_UP, main_work.cur_zone);
        dmStgSlctSetFrame(
            main_work, ACT_TEX_ZONE_UP_S, dmStgSlctUseLocalisedZoneName(main_work) ? 1.0f : 0.0f);

        dmStgSlctUpdateHeaderActs(main_work);

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        AoActAcmPush();

        // the mode tab strip rides along with whatever the drag has done to it, but only updates
        // its own animation while nothing is holding it
        var window_open = main_work.proc_win_update != null
            && main_work.proc_win_update != dmStgSlctProcWindowNodispIdle;

        var mode_held = touch.trg_mode[0].GetState(0U)[CTrgState.EState.Lock]
            || touch.trg_mode[1].GetState(0U)[CTrgState.EState.Lock];

        if (touch.trg_act_move.GetState(0U)[CTrgState.EState.DragAndDrop] || window_open || !mode_held)
            AoActUpdate(dmStgSlctAct(main_work, ACT_TAB_STATE_MOVE));

        var move_act = dmStgSlctAct(main_work, ACT_TAB_STATE_MOVE);
        var move_x = move_act.sprite.center_x - touch.act_tab_state_move_base_pos[AMD_X];
        var move_y = move_act.sprite.center_y - touch.act_tab_state_move_base_pos[AMD_Y];

        for (int i = dmStgSlctActIdx(ACT_TAB_STATE_L); i <= dmStgSlctActIdx(ACT_TAB_STATE_R2); i++)
        {
            AoActAcmInit();
            AoActAcmApplyTrans(0.0f, main_work.mode_tex_pos_y, 0.0f);
            AoActAcmApplyTrans(move_x, move_y, 0.0f);
            AoActUpdate(main_work.act[i], main_work.act[i].frame >= 2.0f ? 1.0f : 0.0f);
        }

        AoActAcmPop();

        foreach (var mode in touch.trg_mode)
            mode.Update();

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        dmStgSlctUpdateAct(main_work, ACT_TEX_ZONE_UP, 0.0f);
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        dmStgSlctUpdateAct(main_work, ACT_TEX_ZONE_UP_S, 0.0f);

        AoActAcmPush();

        for (int i = dmStgSlctActIdx(ACT_TEX_BIG_TIME); i <= dmStgSlctActIdx(ACT_TEX_BIG_SCORE); i++)
        {
            AoActAcmInit();
            AoActAcmApplyTrans(0.0f, main_work.mode_tex_pos_y, 0.0f);
            AoActAcmApplyTrans(move_x, move_y, 0.0f);
            AoActUpdate(main_work.act[i], main_work.act[i].frame >= 2.0f ? 1.0f : 0.0f);
        }

        AoActAcmPop();

        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
        dmStgSlctUpdateAct(main_work, ACT_TEX_FIX_BACK, 0.0f);

        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));

        for (int i = (int)DME_STGSLCT_ACT.ACT_BACK_BTN01_L;
             i <= (int)DME_STGSLCT_ACT.ACT_BACK_BTN01_R; i++)
        {
            AoActUpdate(main_work.act[i], main_work.act[i].frame >= 2.0f ? 1.0f : 0.0f);
        }

        touch.trg_cancel.Update();

        dmStgSlctDrawDownIcons(main_work);

        dmStgSlctFlushSortFix();

        // no obi on mobile, but the flush still happens
        dmStgSlctFlushSort();
    }
}
