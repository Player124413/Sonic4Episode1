using System;
using System.Collections.Generic;
using er;
using er.web;
using gs.backup;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

using static AppMain;

internal static partial class DmTitle
{
    private static bool dm_title_is_title_start = false;

    public static void DmTitleStart(object arg)
    {
        dm_title_is_title_start = true;
        int oldEvtId = SyGetEvtInfo().old_evt_id;
        if (dm_title_is_title_start)
            SSave.CreateInstance().Init();
        dmTitleInit();
    }

    public static void DmMainMenuStart(object arg)
    {
        dm_title_is_title_start = false;
        dmTitleInit();
    }

    private static void dmTitleInit()
    {
        // GC.Collect();
        AoActSysSetDrawStateEnable(true);
        AoActSysSetDrawState(10U);
        var work = (DMS_TITLE_MAIN_WORK)MTM_TASK_MAKE_TCB(dmTitleProcMain, dmTitleDest, 0U, 0, 8192U, 10, () => new DMS_TITLE_MAIN_WORK(), "TITLE_MAIN").work;
        dmTitleSetInitDispData(work);
        if (dm_title_is_title_start)
            AoAccountClearCurrentId();
        work.is_jp_region = GeEnvGetDecideKey() == GSE_DECIDE_KEY.GSD_DECIDE_KEY_O;
        work.proc_update = dmTitleLoadFontData;
        work.iphone_menus = !dmTitleIsConsoleDataComplete();
        dm_title_variant = work.iphone_menus ? dm_title_variant_iphone : dm_title_variant_pc;
    }

    private static void dmTitleSetInitDispData(DMS_TITLE_MAIN_WORK main_work)
    {
        var tmp_prev_evt = (GSE_EVT_ID)SyGetEvtInfo().old_evt_id;

        switch (tmp_prev_evt)
        {
            case GSE_EVT_ID.GSD_EVT_ID_MAINGAME:
                main_work.cur_slct_menu = (int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_START;
                break;
            case GSE_EVT_ID.GSD_EVT_ID_MAP:
                main_work.cur_slct_menu = (int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_TUDUKI;
                break;
            case GSE_EVT_ID.GSD_EVT_ID_OPTION:
                if (!GsMainSysIsStageClear(GSD_MAIN_STAGE_ID_1_1)
                    || GsTrialIsTrial())
                {
                    main_work.cur_slct_menu = (int)(DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_OPTION - 1);
                }
                else
                {
                    main_work.cur_slct_menu = (int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_OPTION;
                }

                break;
            case GSE_EVT_ID.GSD_EVT_ID_STAFFROLL:
                main_work.cur_slct_menu = (int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_TUDUKI;
                break;
            default:
                main_work.cur_slct_menu = (int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_START;
                break;
        }

        // 体験版でゲームプレイ後は必ずカーソルを完全版購入へ
        //if (tmp_prev_evt == GSE_EVT_ID.GSD_EVT_ID_BUYSCREEN
        // && GsTrialIsTrial()) {
        // main_work.cur_slct_menu = DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_BUY - 1;
        //}


        // タイトル全体表示フラグON(これがOFFになるときは完全版購入画面の表示時のみ)
        main_work.disp_flag |= DMD_TITLE_DISP_FLAG_ALL_ACTION;

        // カーソルを初期表示位置に設定
        if (GsTrialIsTrial())
        {
            main_work.cur_crsr_pos_y = dm_title_variant.crsr_trial_pos_y_tbl[main_work.cur_slct_menu];
        }
        else
        {
            main_work.cur_crsr_pos_y = main_work.cur_slct_menu * dm_title_crsr_pos_y_tbl[1]
                                               + dm_title_crsr_pos_y_tbl[0];
        }
    }

    private static void dmTitleProcMain(MTS_TASK_TCB tcb)
    {
        DMS_TITLE_MAIN_WORK work = (DMS_TITLE_MAIN_WORK)tcb.work;
        work.flag_prev = work.flag;
        if (((int)work.flag & 1) != 0)
        {
            mtTaskClearTcb(tcb);
            dmTitleSetNextEvent(work);
        }

        if (((int)work.flag & int.MinValue) != 0 && event_after_buy)
        {
            work.is_init_play = true;
            work.is_no_save_data = true;
            event_after_buy = false;
            work.proc_update = (dmTitleProcFadeOut);
            work.flag &= int.MaxValue;
            work.flag |= 1073741824U;
            IzFadeInitEasy(1U, 1U, 32f);
            DmSndBgmPlayerExit();
            work.flag |= 524288U;
            work.flag &= 4294967291U;
            work.flag &= 4294967293U;
            work.proc_input = null;
            work.proc_win_input = null;
            work.win_timer = 0.0f;
            work.win_cur_slct = 0;
            work.win_mode = 0;
        }

        if (work.proc_win_update != null)
            work.proc_win_update(work);
        if (work.proc_update != null)
            work.proc_update(work);
        if (work.proc_draw == null)
            return;
        work.proc_draw(work);
    }

    private static void dmTitleDest(MTS_TASK_TCB tcb)
    {
        UNREFERENCED_PARAMETER(tcb);
    }

    private static void dmTitleSetNextEvent(DMS_TITLE_MAIN_WORK main_work)
    {
        short evt_case1 = 0;
        if (((int)main_work.flag & 1073741824) != 0)
        {
            short evt_case2 = 5;
            main_work.flag &= 3221225471U;
            SyDecideEvtCase(evt_case2);
            SyChangeNextEvt();
        }
        else if (((int)main_work.flag & 1048576) != 0)
        {
            short evt_case2 = 4;
            main_work.flag &= 4293918719U;
            SyDecideEvtCase(evt_case2);
            SyChangeNextEvt();
        }
        else
        {
            switch (dmTitleMenuAction(main_work))
            {
                case DME_TITLE_MENU_ACTION.NEW_GAME:
                    evt_case1 = 0;
                    dmTitleSetFirstPlayData(main_work);
                    nextDemoLevel = 0;
                    g_gs_main_sys_info.stage_id = 5; // TEMP(z2a2-test): boot straight to Zone 2 Act 2, revert before merge
                    g_gs_main_sys_info.char_id[0] = 0;
                    g_gs_main_sys_info.game_mode = 0;
                    if (GsTrialIsTrial())
                        g_gs_main_sys_info.rest_player_num = 3U;
                    g_gs_main_sys_info.game_flag &= 4294967167U;
                    GmMainGSInit();
                    break;
                case DME_TITLE_MENU_ACTION.CONTINUE:
                    if (!GsMainSysIsStageClear(0) || main_work.is_init_play)
                    {
                        evt_case1 = 0;
                        g_gs_main_sys_info.stage_id = 0;
                        g_gs_main_sys_info.char_id[0] = 0;
                        g_gs_main_sys_info.game_mode = 0;
                        g_gs_main_sys_info.game_flag &= 4294967167U;
                        GmMainGSInit();
                        break;
                    }

                    evt_case1 = 1;
                    break;
                case DME_TITLE_MENU_ACTION.OPTION:
                    evt_case1 = 2;
                    dm_opt_show_xboxlive = false;
                    break;
                case DME_TITLE_MENU_ACTION.ACHIEVEMENTS:
                    evt_case1 = 2;
                    //LiveFeature.getInstance().ShowAchievements();
                    dm_opt_show_xboxlive = true;
                    break;
                case DME_TITLE_MENU_ACTION.LEADERBOARDS:
                    evt_case1 = 2;
                    //LiveFeature.getInstance().ShowLeaderboards();
                    dm_opt_show_xboxlive = true;
                    break;

                // both are resolved in dmTitleProcMainMenuDecideEfct and never reach an event,
                // which is why dmTitle.cpp:1429 leaves them on default too
                case DME_TITLE_MENU_ACTION.LIBRARY:
                case DME_TITLE_MENU_ACTION.BUY:
                    return;
            }

            SyDecideEvtCase(evt_case1);
            SyChangeNextEvt();
        }
    }
}
