using System;
using System.Collections.Generic;
using er;
using er.web;
using gs.backup;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

using static AppMain;

internal static class DmTitle
{
    public const int DMD_TITLE_TASK_PAUSELEVEL = 0;
    public const int DMD_TITLE_TASK_PRIO_MAIN = 8192;
    public const int DMD_TITLE_TASK_GROUP_MAIN = 10;
    public const int DMD_TITLE_DRAW_PRIO_ACT = 33792;
    public const int DMD_TITLE_DRAW_PRIO_WIN = 34816;
    public const int DMD_TITLE_DRAW_PRIO_WIN_FIX = 35840;
    public const int DMD_TITLE_FILE_PATH_NUM_MAX = 60;
    public const int DMD_TITLE_IDLE_DISP_TIME = 40;
    public const int DMD_TITLE_IDLE_NONE_TIME = 30;
    public const int DMD_TITLE_DECIDE_EFCT_TIME = 60;
    public const int DMD_TITLE_DECIDE_BLINK_TIME = 4;
    public const int DMD_TITLE_AMA_DATA_ARC_ID = 0;
    public const int DMD_TITLE_AMB_DATA_ARC_ID = 1;

    public const float DMD_TITLE_CRSR_DFLT_POS_X = 304f;
    public const int DMD_TITLE_SLCT_MENU_NUM_FIRST = 3;
    public const int DMD_TITLE_SLCT_MENU_NUM_PLAYED = 4;
    public const int DMD_TITLE_SLCT_MENU_NUM_TRIAL = 3;

    public const float DMD_TITLE_CRSR_MOVE_TIME = 8f;

    public const int DMD_TITLE_DRAW_STATE_ID = 10;

    public const float DMD_TITLE_SIZE_WIDTH = 960.0f;
    public const float DMD_TITLE_SIZE_HEIGHT = 720.0f;
    public const float DMD_TITLE_SIZE_HALF_WIDTH = 480.0f;
    public const float DMD_TITLE_SIZE_HALF_HEIGHT = 360.0f;

    public const float DMD_TITLE_WIN_EFCT_TIME = 8.0f;
    public const float DMD_TITLE_WINDOW_SIZE_H = 180.0f;
    public const float DMD_TITLE_WIN_DEF_RATE = 1.0f;

    public const int DMD_TITLE_PUSH_START_DISP_TIME = 3600;
    public const int DMD_TITLE_END_GAME_WAIT_TIME = 60;

    private const float DMD_TITLE_WINDOW_SIZE_W_PC = 380.0f;
    private const float DMD_TITLE_WINDOW_SIZE_W_IOS = 420.0f;

    private const float DMD_TITLE_DISP_SCALE_TEXT_PC = 1.0f;
    private const float DMD_TITLE_DISP_SCALE_TEXT_IOS = 1.5f * 1.125f;

    private const float DMD_TITLE_MMENU_WIN_SIZE_W_PC = 486.0f;
    private const float DMD_TITLE_MMENU_WIN_SIZE_H_PC = 138.0f;

    private const float DMD_TITLE_MMENU_WIN_SIZE_W_IOS = 800.0f;
    private const float DMD_TITLE_MMENU_WIN_SIZE_H_IOS = 540.0f;
    private const float DMD_TITLE_MMENU_WIN_SIZE_W_2 = DMD_TITLE_MMENU_WIN_SIZE_W_IOS;
    private const float DMD_TITLE_MMENU_WIN_SIZE_H_2 = DMD_TITLE_MMENU_WIN_SIZE_H_IOS - 180.0f;

    private const float DMD_TITLE_MMENU_WIN_SIZE_W_OTHER = 486.0f;

    public const float DMD_TITLE_FADEIN_TIME = 32f;
    public const float DMD_TITLE_FADEOUT_TIME = 32f;

    public const int DMD_TITLE_BGM_FADEIN_TIME = 32;
    public const int DMD_TITLE_BGM_FADEOUT_TIME = 32;

    public const uint DMD_TITLE_FLAG_EXIT = 1;
    public const uint DMD_TITLE_FLAG_CANCEL = 2;
    public const uint DMD_TITLE_FLAG_DECIDE = 4;
    public const uint DMD_TITLE_FLAG_DISP_TEXT = 8;
    public const uint DMD_TITLE_FLAG_DECIDE_EFCT = 16;
    public const uint DMD_TITLE_FLAG_GAME_START = 32;
    public const uint DMD_TITLE_FLAG_MENU_UP_INPUT = 64;
    public const uint DMD_TITLE_FLAG_MENU_DOWN_INPUT = 128;
    public const uint DMD_TITLE_FLAG_MENU_CRSR_EFCT = 256;
    public const uint DMD_TITLE_FLAG_DISP_TITLE = 512;
    public const uint DMD_TITLE_FLAG_DISP_MAINMENU = 1024;
    public const uint DMD_TITLE_FLAG_BUY_COMP = 2048;
    public const uint DMD_TITLE_FLAG_EXIST_SAVE_DATA = 4096;
    public const uint DMD_TITLE_FLAG_BACK_DEL_SAVE = 8192;
    public const uint DMD_TITLE_FLAG_DEL_SAVE_DATA = 16384;
    public const uint DMD_TITLE_FLAG_NEW_GAME_START = 32768;
    public const uint DMD_TITLE_FLAG_WIN_EFCT_END = 65536;
    public const uint DMD_TITLE_FLAG_WIN_DRAW_START = 131072;
    public const uint DMD_TITLE_FLAG_COMP_NO_DISP_END = 262144;
    public const uint DMD_TITLE_FLAG_DEMO_SND_END = 524288;
    public const uint DMD_TITLE_FLAG_DISP_LOOP_END = 1048576;
    public const uint DMD_TITLE_FLAG_DISP_MMENU_WIN = 2097152;
    public const uint DMD_TITLE_FLAG_MMENU_WIN_EFCT_END = 4194304;
    public const uint DMD_TITLE_FLAG_DEL_DATA_BACK_TITLE = 8388608;
    public const uint DMD_TITLE_FLAG_DEL_DATA_BACK_MMENU = 16777216;
    public const uint DMD_TITLE_FLAG_DEL_DATA_INIT_START = 33554432;
    public const uint DMD_TITLE_FLAG_DEL_DATA_GAME_END = 67108864;
    public const uint DME_TITLE_FLAG_NEXT_EVT_TITLE = 1073741824;
    public const uint DMD_TITLE_FLAG_SIGN_OUT_EXIT = 2147483648;

    public const uint DMD_TITLE_DISP_FLAG_WIN_ACT = 1;
    public const uint DMD_TITLE_DISP_FLAG_ALL_ACTION = 2;


    public const int IDA_D_TITLE_ACT_BTN_L = 0;
    public const int IDA_D_TITLE_ACT_BTN_C = 1;
    public const int IDA_D_TITLE_ACT_BTN_R = 2;
    public const int IDA_D_TITLE_ACT_BTN02_L = 3;
    public const int IDA_D_TITLE_ACT_BTN02_C = 4;
    public const int IDA_D_TITLE_ACT_GAME_BTN_L = 5;
    public const int IDA_D_TITLE_ACT_GAME_BTN_C = 6;
    public const int IDA_D_TITLE_JP_ACT_TEX_HAZIME = 0;
    public const int IDA_D_TITLE_JP_ACT_TEX_TUZUKI = 1;
    public const int IDA_D_TITLE_JP_ACT_TEX_OPTION = 2;
    public const int IDA_D_TITLE_JP_ACT_TEX_KANZENBAN = 3;
    public const int IDA_D_TITLE_JP_ACT_TEX_WIN_MSG1 = 4;
    public const int IDA_D_TITLE_JP_ACT_TEX_WIN_MSG2 = 5;
    public const int IDA_D_TITLE_JP_ACT_TEX_START = 6;
    public const int IDA_D_TITLE_JP_ACT_TEX_MODORU = 7;
    public const int IDA_D_TITLE_JP_ACT_TEX_GAME = 8;
    public const int IDA_D_TITLE_JP_ACT_TEX_ACHIEVEMENTS = 9;
    public const int IDA_D_TITLE_JP_ACT_TEX_LEADERBOARDS = 10;
    public const int IDA_D_TITLE_OP_ACT_LOGO01 = 0;
    public const int IDA_D_TITLE_OP_ACT_LOGO02_YUBI = 1;
    public const int IDA_D_TITLE_OP_ACT_LOGO03 = 2;
    public const int IDA_D_TITLE_OP_ACT_LOGO04 = 3;
    public const int IDA_D_TITLE_OP_ACT_SEGALOGO = 4;

    const int IDA_D_TITLE_JP_ACT_TEX_HAZIME_PC = 0; // tex_hazime
    const int IDA_D_TITLE_JP_ACT_TEX_HAZIME2_PC = 1; // tex_hazime2
    const int IDA_D_TITLE_JP_ACT_TEX_TUZUKI_PC = 2; // tex_tuzuki
    const int IDA_D_TITLE_JP_ACT_TEX_TUZUKI2_PC = 3; // tex_tuzuki2
    const int IDA_D_TITLE_JP_ACT_TEX_OPTION_PC = 4; // tex_option
    const int IDA_D_TITLE_JP_ACT_TEX_OPTION2_PC = 5; // tex_option2
    const int IDA_D_TITLE_JP_ACT_TEX_RANKING_PC = 6; // tex_ranking
    const int IDA_D_TITLE_JP_ACT_TEX_RANKING2_PC = 7; // tex_ranking2
    const int IDA_D_TITLE_JP_ACT_TEX_ZISSEKI_PC = 8; // tex_zisseki
    const int IDA_D_TITLE_JP_ACT_TEX_ZISSEKI2_PC = 9; // tex_zisseki2
    const int IDA_D_TITLE_JP_ACT_TEX_LIBRARY_PC = 10; // tex_library
    const int IDA_D_TITLE_JP_ACT_TEX_LIBRARY2_PC = 11; // tex_library2
    const int IDA_D_TITLE_JP_ACT_TEX_KANZENBAN_PC = 12; // tex_kanzenban
    const int IDA_D_TITLE_JP_ACT_TEX_KANZENBAN2_PC = 13; // tex_kanzenban2
    const int IDA_D_TITLE_JP_ACT_TEX_WIN_MSG1_PC = 14; // tex_win_msg1
    const int IDA_D_TITLE_JP_ACT_TEX_WIN_MSG2_PC = 15; // tex_win_msg2
    const int IDA_D_TITLE_JP_ACT_TEX_START_PC = 16; // tex_start
    const int IDA_D_TITLE_JP_ACT_TEX_WIN_MSG3_PC = 17; // tex_win_msg3

    const int IDA_D_CMN_WIN_ACT_WIN_LINE = IDA_D_CMN_WIN_ACT_WIN_LINE_PC;

    const int IDA_D_TITLE_ACT_CURSOR_PC = 0;


    private static readonly string[] dm_title_file_lng_amb_name_tbl = new string[]
    {
        "DEMO/TITLE/D_TITLE_JP.AMB",
        "DEMO/TITLE/D_TITLE_US.AMB",
        "DEMO/TITLE/D_TITLE_FR.AMB",
        "DEMO/TITLE/D_TITLE_IT.AMB",
        "DEMO/TITLE/D_TITLE_GE.AMB",
        "DEMO/TITLE/D_TITLE_SP.AMB",
        "DEMO/TITLE/D_TITLE_FI.AMB",
        "DEMO/TITLE/D_TITLE_PT.AMB",
        "DEMO/TITLE/D_TITLE_RU.AMB",
        "DEMO/TITLE/D_TITLE_CN.AMB",
        "DEMO/TITLE/D_TITLE_HK.AMB"
    };
    public const string DMD_TITLE_DATA_FILENAME = "DEMO/TITLE/D_TITLE.AMB";

    private static readonly string[] dm_title_menu_cmn_amb_name_tbl = new string[]
    {
        g_dm_buy_screen_btn_amb_path,
        "DEMO/CMN/D_CMN_OBI.AMB",
        DMD_MANUAL_CMN_DATA_FILENAME
    };

    private static readonly string[] dm_title_menu_cmn_lng_amb_name_tbl = new string[]
    {
        "DEMO/CMN/D_CMN_MSG_JP.AMB",
        "DEMO/CMN/D_CMN_MSG_US.AMB",
        "DEMO/CMN/D_CMN_MSG_FR.AMB",
        "DEMO/CMN/D_CMN_MSG_IT.AMB",
        "DEMO/CMN/D_CMN_MSG_GE.AMB",
        "DEMO/CMN/D_CMN_MSG_SP.AMB",
        "DEMO/CMN/D_CMN_MSG_FI.AMB",
        "DEMO/CMN/D_CMN_MSG_PT.AMB",
        "DEMO/CMN/D_CMN_MSG_RU.AMB",
        "DEMO/CMN/D_CMN_MSG_CN.AMB",
        "DEMO/CMN/D_CMN_MSG_CN.AMB"
    };

    private static readonly float[] dm_title_crsr_pos_y_tbl = new float[]
    {
        372f,
        50f
    };

    private static readonly float[] dm_title_crsr_trial_pos_y_tbl = new float[]
    {
        445f,
        495f,
        545f
    };

    private static readonly float[,] dm_title_win_act_pos_tbl_iphone = new float[,]
    {
        {DMD_TITLE_SIZE_HALF_WIDTH + 42.0f, 280.0f},
        {DMD_TITLE_SIZE_HALF_WIDTH, 336.0f - 60.0f},	// メッセージ
	    {DMD_TITLE_SIZE_HALF_WIDTH, 336.0f - 60.0f},	// メッセージ
	    {DMD_TITLE_SIZE_HALF_WIDTH, 336.0f - 60.0f},	// メッセージ
	    {DMD_TITLE_SIZE_HALF_WIDTH - 80.0f, 274.0f},	// タイトルテキスト
	    {DMD_TITLE_SIZE_HALF_WIDTH - 176.0f, 420.0f},	// YES
	    {DMD_TITLE_SIZE_HALF_WIDTH + 176.0f, 420.0f},	// NO
    };

    private static readonly float[,] dm_title_win_act_pos_tbl_pc = new float[,]
    {
        {DMD_TITLE_SIZE_HALF_WIDTH + 42.0f, 280.0f},
        {DMD_TITLE_SIZE_HALF_WIDTH, 336.0f},			// メッセージ
	    {DMD_TITLE_SIZE_HALF_WIDTH, 336.0f},			// メッセージ
	    {DMD_TITLE_SIZE_HALF_WIDTH, 336.0f},			// メッセージ
	    {DMD_TITLE_SIZE_HALF_WIDTH - 80.0f, 274.0f},	// タイトルテキスト
	    {DMD_TITLE_SIZE_HALF_WIDTH - 88.0f, 420.0f},	// YES
	    {DMD_TITLE_SIZE_HALF_WIDTH + 88.0f, 420.0f},	// NO
    };

    // slot < 0 is main_work.ama[-slot - 1], the title archives; slot >= 0 is main_work.cmn_ama[slot]
    private const int AMA_TITLE_CMN = -1 - (int)DME_TITLE_DATA_TYPE.DME_TITLE_DATA_TYPE_CMN_DATA;
    private const int AMA_TITLE_LNG = -1 - (int)DME_TITLE_DATA_TYPE.DME_TITLE_DATA_TYPE_LANG_DATA;
    private const int AMA_CMN_BTN = 0;   // DEMO/CMN/D_CMN_BTN.AMB
    private const int AMA_CMN_WIN = 2;   // DEMO/CMN/D_CMN_WIN.AMB
    private const int AMA_CMN_MSG = 3;   // DEMO/CMN/D_CMN_MSG_XX.AMB

    private static readonly int[] dm_title_act_slot_tbl_iphone = BuildSlots(
        (AMA_TITLE_CMN, (int)DME_TITLE_ACT.ACT_LANG),
        (AMA_TITLE_LNG, (int)DME_TITLE_ACT.ACT_BTN_CANCEL),
        (AMA_CMN_BTN, (int)DME_TITLE_ACT.ACT_WIN_NO_BTN_L),
        (AMA_CMN_WIN, (int)DME_TITLE_ACT.ACT_TEX_YES),
        (AMA_CMN_MSG, (int)DME_TITLE_ACT.ACT_NUM));

    private static readonly int[] dm_title_act_slot_tbl_pc = BuildSlots(
        (AMA_TITLE_CMN, (int)DME_TITLE_ACT_REG.ACT_TEX_START),
        (AMA_TITLE_LNG, (int)DME_TITLE_ACT_REG.ACT_BTN_CANCEL),
        (AMA_CMN_BTN, (int)DME_TITLE_ACT_REG.ACT_WIN_LINE),
        (AMA_CMN_WIN, (int)DME_TITLE_ACT_REG.ACT_TEX_WINTITLE),
        (AMA_CMN_MSG, (int)DME_TITLE_ACT_REG.ACT_NUM));

    // the archive path is the same either way, amFsResolve decides which build's data lands there.
    // these tables describe that data, so they stay paired. picked once and not asked about again.
    internal sealed class DMS_TITLE_VARIANT
    {
        public uint[] act_id_tbl;
        public int[] act_slot_tbl;
        public float[,] win_act_pos_tbl;
        public DMS_TITLE_MAIN_WORK._proc_ proc_win_input;
    }

    private static DMS_TITLE_VARIANT dm_title_variant;

    // (slot, end) runs, each covering up to but not including `end`
    private static int[] BuildSlots(params (int Slot, int End)[] runs)
    {
        var slots = new int[runs[runs.Length - 1].End];
        var start = 0;
        foreach (var run in runs)
        {
            for (int i = start; i < run.End; i++)
                slots[i] = run.Slot;

            start = run.End;
        }

        return slots;
    }

    public static readonly uint[] dm_title_g_dm_act_id_tbl_iphone = new uint[]
    {
        0U,
        1U,
        2U,
        0U,
        1U,
        2U,
        0U,
        1U,
        2U,
        0U,
        1U,
        2U,
        0U,
        1U,
        2U,
        3U,
        4U,
        5U,
        6U,
        6U,
        0U,
        1U,
        2U,
        3U,
        9U,
        10U,
        7U,
        8U,
        4U,
        5U,
        1U,
        10U,
        0U,
        1U,
        2U,
        0U,
        1U,
        2U,
        1U,
        2U,
        0U
    };

    public static readonly uint[] dm_title_g_dm_act_id_tbl_pc = new uint[]
    {
        IDA_D_TITLE_ACT_CURSOR_PC,
        IDA_D_TITLE_JP_ACT_TEX_START_PC,
        IDA_D_TITLE_JP_ACT_TEX_HAZIME_PC,
        IDA_D_TITLE_JP_ACT_TEX_HAZIME2_PC,
        IDA_D_TITLE_JP_ACT_TEX_TUZUKI_PC,
        IDA_D_TITLE_JP_ACT_TEX_TUZUKI2_PC,
        IDA_D_TITLE_JP_ACT_TEX_OPTION_PC,
        IDA_D_TITLE_JP_ACT_TEX_OPTION2_PC,
        IDA_D_TITLE_JP_ACT_TEX_LIBRARY_PC,
        IDA_D_TITLE_JP_ACT_TEX_LIBRARY2_PC,
        IDA_D_TITLE_JP_ACT_TEX_KANZENBAN_PC,
        IDA_D_TITLE_JP_ACT_TEX_KANZENBAN2_PC,
        IDA_D_TITLE_JP_ACT_TEX_WIN_MSG1_PC,
        IDA_D_TITLE_JP_ACT_TEX_WIN_MSG2_PC,
        IDA_D_TITLE_JP_ACT_TEX_WIN_MSG3_PC,
        IDA_D_CMN_BTN_ACT_BACK_BTN,
        IDA_D_CMN_BTN_ACT_BTN_X_TITLE,
        IDA_D_CMN_WIN_ACT_WIN_LINE,
        IDA_D_CMN_MSG_JP_ACT_TEX_WINTITLE_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_YES_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_NO_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_BACK_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_OK_PC
    };

    // must stay below every table it references, static field initialisers run in textual order
    private static readonly DMS_TITLE_VARIANT dm_title_variant_iphone = new DMS_TITLE_VARIANT
    {
        act_id_tbl = dm_title_g_dm_act_id_tbl_iphone,
        act_slot_tbl = dm_title_act_slot_tbl_iphone,
        win_act_pos_tbl = dm_title_win_act_pos_tbl_iphone,
        proc_win_input = dmTitleInputProcWindowIphone,
    };

    private static readonly DMS_TITLE_VARIANT dm_title_variant_pc = new DMS_TITLE_VARIANT
    {
        act_id_tbl = dm_title_g_dm_act_id_tbl_pc,
        act_slot_tbl = dm_title_act_slot_tbl_pc,
        win_act_pos_tbl = dm_title_win_act_pos_tbl_pc,
        proc_win_input = dmTitleInputProcWindowPc,
    };

    internal static DMS_TITLE_VARIANT TitleVariantForTest(bool iphone)
    {
        return iphone ? dm_title_variant_iphone : dm_title_variant_pc;
    }

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

    // five of the eleven languages have no console archive, so this is not always true on desktop
    private static bool dmTitleIsConsoleDataComplete()
    {
        var lng = GsEnvGetLanguage();

        return amFsAllVariant(
            AMD_FS_VARIANT_CONSOLE,
            DMD_TITLE_DATA_FILENAME,
            dm_title_file_lng_amb_name_tbl[lng],
            dm_title_menu_cmn_lng_amb_name_tbl[lng],
            dm_title_menu_cmn_amb_name_tbl[0],
            dm_title_menu_cmn_amb_name_tbl[1],
            dm_title_menu_cmn_amb_name_tbl[2]);
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
            main_work.cur_crsr_pos_y = dm_title_crsr_trial_pos_y_tbl[main_work.cur_slct_menu];
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
            switch (main_work.cur_slct_menu)
            {
                case 0:
                    evt_case1 = 0;
                    dmTitleSetFirstPlayData(main_work);
                    nextDemoLevel = 0;
                    g_gs_main_sys_info.stage_id = 0;
                    g_gs_main_sys_info.char_id[0] = 0;
                    g_gs_main_sys_info.game_mode = 0;
                    if (GsTrialIsTrial())
                        g_gs_main_sys_info.rest_player_num = 3U;
                    g_gs_main_sys_info.game_flag &= 4294967167U;
                    GmMainGSInit();
                    break;
                case 1:
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
                case 2:
                    evt_case1 = 2;
                    dm_opt_show_xboxlive = false;
                    break;
                case 3:
                    evt_case1 = 2;
                    //LiveFeature.getInstance().ShowAchievements();
                    dm_opt_show_xboxlive = true;
                    break;
                case 4:
                    evt_case1 = 2;
                    //LiveFeature.getInstance().ShowLeaderboards();
                    dm_opt_show_xboxlive = true;
                    break;
            }

            SyDecideEvtCase(evt_case1);
            SyChangeNextEvt();
        }
    }

    private static void dmTitleLoadFontData(DMS_TITLE_MAIN_WORK main_work)
    {
        GsFontBuild();
        main_work.proc_update = (dmTitleIsLoadFontData);
    }

    private static void dmTitleIsLoadFontData(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!GsFontIsBuilded())
            return;
        main_work.proc_update = (dmTitleLoadRequest);
    }

    private static void dmTitleLoadRequest(DMS_TITLE_MAIN_WORK main_work)
    {
        main_work.arc_amb_fs[0] = amFsReadBackground(DMD_TITLE_DATA_FILENAME);
        main_work.arc_amb_fs[1] = amFsReadBackground(dm_title_file_lng_amb_name_tbl[GsEnvGetLanguage()]);
        for (int index = 0; index < 3; ++index)
            main_work.arc_cmn_amb_fs[index] = amFsReadBackground(dm_title_menu_cmn_amb_name_tbl[index]);
        main_work.arc_cmn_amb_fs[3] = amFsReadBackground(dm_title_menu_cmn_lng_amb_name_tbl[GsEnvGetLanguage()]);
        DmTitleOp.DmTitleOpLoad();
        main_work.proc_update = (dmTitleProcLoadWait);
    }

    private static void dmTitleProcLoadWait(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!dmTitleIsDataLoad(main_work))
            return;

        for (int index1 = 0; index1 < 2; ++index1)
        {
            main_work.arc_amb[index1] = readAMBFile(main_work.arc_amb_fs[index1]);
            main_work.ama[index1] = readAMAFile(amBindGet(main_work.arc_amb[index1], 0));

            string sPath;
            main_work.amb[index1] = readAMBFile(amBindGet(main_work.arc_amb[index1], 1, out sPath));
            main_work.amb[index1].dir = sPath;
            amFsClearRequest(main_work.arc_amb_fs[index1]);
            main_work.arc_amb_fs[index1] = null;

            AoTexBuild(main_work.tex[index1], main_work.amb[index1]);
            AoTexLoad(main_work.tex[index1]);
        }

        for (int index = 0; index < 4; ++index)
        {
            main_work.arc_cmn_amb[index] = readAMBFile(main_work.arc_cmn_amb_fs[index]);
            main_work.cmn_ama[index] = readAMAFile(amBindGet(main_work.arc_cmn_amb[index], 0));
            string sPath;
            main_work.cmn_amb[index] = readAMBFile(amBindGet(main_work.arc_cmn_amb[index], 1, out sPath));
            main_work.cmn_amb[index].dir = sPath;
            amFsClearRequest(main_work.arc_cmn_amb_fs[index]);
            main_work.arc_cmn_amb_fs[index] = null;
            AoTexBuild(main_work.cmn_tex[index], main_work.cmn_amb[index]);
            AoTexLoad(main_work.cmn_tex[index]);
        }

        DmTitleOp.DmTitleOpBuild();
        DmSndBgmPlayerInit();
        main_work.proc_update = (dmTitleProcTexBuildWait);
    }

    static void dmTitleProcTexBuildWait(DMS_TITLE_MAIN_WORK main_work)
    {
        var dm_title_g_dm_act_id_tbl = dm_title_variant.act_id_tbl;
        // テクスチャ構築完了判定
        if (dmTitleIsTexLoad(main_work)
            && DmSndBgmPlayerIsSndSysBuild())
        {
            var slots = dm_title_variant.act_slot_tbl;

            for (int i = 0; i < slots.Length; i++)
            {
                A2S_AMA_HEADER ama;
                AOS_TEXTURE tex;

                if (slots[i] < 0)
                {
                    var index = -slots[i] - 1;
                    ama = main_work.ama[index];
                    tex = main_work.tex[index];
                }
                else
                {
                    ama = main_work.cmn_ama[slots[i]];
                    tex = main_work.cmn_tex[slots[i]];
                }

                // 構築
                AoActSetTexture(AoTexGetTexList(tex));
                main_work.act[i] = AoActCreate(ama, dm_title_g_dm_act_id_tbl[i]);
            }

            if (main_work.iphone_menus)
            {
                {   //選択肢
                    int[] c_act_id_table = { (int)DME_TITLE_ACT.ACT_BTN_C, (int)DME_TITLE_ACT.ACT_BTN_C2, (int)DME_TITLE_ACT.ACT_BTN_C3 };
                    for (int i = 0, max = arrayof(c_act_id_table); i < max; ++i)
                    {
                        main_work.trg_slct[i] = new CTrgAoAction();
                        main_work.trg_slct[i].Create(main_work.act[c_act_id_table[i]]);
                    }
                }
                {   //ウインドウ・はい/いいえ
                    int[] c_act_id_table = { (int)DME_TITLE_ACT.ACT_WIN_NO_BTN_C, (int)DME_TITLE_ACT.ACT_WIN_YES_BTN_C };
                    for (int i = 0, max = arrayof(c_act_id_table); i < max; ++i)
                    {
                        main_work.trg_answer[i] = new CTrgAoAction();
                        main_work.trg_answer[i].Create(main_work.act[c_act_id_table[i]]);
                    }
                }
                {   //戻る
                    int c_act_id = (int)DME_TITLE_ACT.ACT_BACK_BTN_R;
                    main_work.trg_return.Create(main_work.act[c_act_id]);
                }
                {   //他のゲーム		
                    int c_act_id = (int)DME_TITLE_ACT.ACT_GAME_BTN_R;
                    main_work.trg_game.Create(main_work.act[c_act_id]);
                }
            }


            main_work.proc_update = dmTitleProcCheckLoadingEnd;

            DmTitleOp.DmTitleOpInit();

            // オープニング側の初期化内にObjInitがあり、そこでSTATE上書きされるので、再度設定
            AoActSysSetDrawState(DMD_TITLE_DRAW_STATE_ID);
            AoActSysSetDrawStateEnable(true);

        }
    }

    private static void dmTitleProcCheckLoadingEnd(DMS_TITLE_MAIN_WORK main_work)
    {
        if (dm_title_is_title_start)
        {
            main_work.disp_change_time = 40;
            main_work.flag |= 8U;
            main_work.proc_update = (dmTitleProcFadeIn);
            main_work.proc_draw = (dmTitleDrawSetProcDispData);
            main_work.flag |= 512U;
            DmTitleOp.DmTitleOpDispRightEnable(true);
            for (int index = 0; index < 2; ++index)
                main_work.mmenu_win_size_rate[index] = 0.0f;
            IzFadeInitEasy(1U, 0U, 32f);
            XBOXLive.allowShowUpdate = true;
        }
        else
        {
            main_work.proc_update = (dmTitleProcFadeIn);
            dmTitleSetMenuInfo(main_work);
            DmTitleOp.DmTitleOpSetRetOptionState();
            main_work.proc_draw = (dmTitleDrawSetProcDispData);
            main_work.flag |= 1024U;
            main_work.flag |= 2097152U;
            DmTitleOp.DmTitleOpDispRightEnable(false);
            for (int index = 0; index < 2; ++index)
                main_work.mmenu_win_size_rate[index] = 1f;
        }
    }

    private static void dmTitleProcFadeIn(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;
        IzFadeExit();
        AoPresenceSet(AO_PRESENCE_STAGE_ID_TITLE, false);
        if (dm_title_is_title_start)
        {
            main_work.proc_update = (dmTitleProcWaitInput);
            main_work.proc_input = (dmTitleInputProcTitle);
            DmSndBgmPlayerPlayBgm(1);
        }
        else
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.flag |= 2147483648U;
            DmSndBgmPlayerPlayBgm(0);
        }

        main_work.proc_win_update = (dmTitleProcWindowNodispIdle);
    }

    private static void dmTitleProcWaitInput(DMS_TITLE_MAIN_WORK main_work)
    {
        ++main_work.timer;
        if (main_work.timer > 3600.0)
        {
            main_work.proc_update = (dmTitleProcFadeOut);
            main_work.proc_input = null;
            main_work.timer = 0.0f;
            main_work.flag |= 1048576U;
            IzFadeInitEasy(0U, 1U, 32f);
            DmSndBgmPlayerExit();
            main_work.flag |= 524288U;
        }
        else
        {
            if (main_work.proc_input != null)
                main_work.proc_input(main_work);
            if (((int)main_work.flag & 32) == 0)
                return;
            main_work.proc_update = (dmTitleProcDecideEfct);
            main_work.proc_input = null;
            main_work.timer = 0.0f;
            main_work.flag &= 4294967263U;
            DmSoundPlaySE("Ok");
            DmSndBgmPlayerBgmStop();
        }
    }

    private static void dmTitleProcDecideEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.timer >= 60.0 && AoAccountSetCurrentIdIsFinished())
        {
            if (AoAccountGetCurrentId() >= 0)
            {
                main_work.flag &= 4294966783U;
                main_work.proc_update = (dmTitleProcCheckTrialIdle);
                GsRebootSetTitle();
            }
            else
            {
                main_work.proc_update = (dmTitleProcWaitInput);
                main_work.proc_input = (dmTitleInputProcTitle);
                DmSndBgmPlayerPlayBgm(1);
            }

            main_work.timer = 0.0f;
        }
        else
            ++main_work.timer;
    }

    private static void dmTitleProcCheckTrialIdle(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!GsTrialCheckIsFinished())
            return;
        if (GsTrialIsTrial())
        {
            main_work.proc_update = (dmTitleProcMainMenuOpenWin);
            main_work.proc_input = null;
            main_work.flag |= 2147483648U;
            main_work.flag |= 4096U;
            main_work.flag |= 2097152U;
            main_work.is_init_play = false;
            main_work.slct_menu_num = 3;
            main_work.cur_slct_menu = 0;
            main_work.cur_crsr_pos_y = dm_title_crsr_trial_pos_y_tbl[main_work.cur_slct_menu];
            DmSndBgmPlayerPlayBgm(0);
        }
        else
        {
            DmSaveStart(1U, false, false);
            main_work.proc_update = (dmTitleProcFileSlctWaitDataLoad);
            DmSndBgmPlayerPlayBgm(0);
        }
    }

    private static void dmTitleProcFileSlctWaitDataLoad(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!DmSaveIsExit())
            return;
        if (DmCmnBackupIsLoadSuccessed())
        {
            main_work.proc_update = (dmTitleProcMainMenuOpenWin);
            main_work.proc_input = null;
            main_work.flag |= 2147483648U;
            main_work.flag |= 4096U;
            main_work.flag |= 2097152U;
            if (GsMainSysIsStageClear(0))
            {
                main_work.is_init_play = false;
                main_work.slct_menu_num = 4;
                main_work.cur_slct_menu = 1;
            }
            else
            {
                main_work.is_init_play = true;
                main_work.slct_menu_num = 3;
                main_work.cur_slct_menu = 0;
            }

            main_work.flag |= 2097152U;
            main_work.cur_crsr_pos_y = !GsTrialIsTrial()
                ? main_work.cur_slct_menu * dm_title_crsr_pos_y_tbl[1] + dm_title_crsr_pos_y_tbl[0]
                : dm_title_crsr_trial_pos_y_tbl[main_work.cur_slct_menu];
            dmTitleSetLoadSysData(main_work);
        }
        else
        {
            int error = XmlStorage.GetLastError();
            main_work.is_init_play = true;
            main_work.slct_menu_num = 3;
            if (error == 1)
            {
                main_work.proc_update = (dmTitleProcFileSlctSaveStartWait);
            }
            else
            {
                main_work.proc_update = (dmTitleProcMainMenuOpenWin);
                main_work.proc_input = null;
                main_work.flag |= 2147483648U;
                main_work.flag |= 4096U;
                main_work.flag |= 2097152U;
                dmTitleSetInitSaveData(main_work);
                dmTitleSetInitSysData(main_work);
            }
        }
    }

    private static void dmTitleProcFileSlctSaveStartWait(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!DmSaveIsExit())
            return;
        DmSaveStart(4U, true, false);
        main_work.proc_update = (dmTitleProcFileSlctWaitDataSave);
    }

    private static void dmTitleProcFileSlctWaitDataSave(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!DmSaveIsExit())
            return;
        if (!DmCmnBackupIsSaveSuccessed())
        {
            main_work.proc_update = (dmTitleProcWaitInput);
            main_work.proc_input = (dmTitleInputProcTitle);
            main_work.flag |= 512U;
            main_work.flag &= 4294966271U;
            main_work.flag &= int.MaxValue;
            main_work.timer = 0.0f;
            DmSndBgmPlayerPlayBgm(1);
        }
        else
        {
            SSave.CreateInstance().Init();
            main_work.is_init_play = true;
            dmTitleSetInitSysData(main_work);
            main_work.proc_update = (dmTitleProcMainMenuOpenWin);
            main_work.proc_input = null;
            main_work.flag |= 2147483648U;
            main_work.flag |= 2097152U;
        }
    }

    private static void dmTitleProcMainMenuOpenWin(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 4194304) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.mmenu_win_timer = 0.0f;
            main_work.flag |= 1024U;
            main_work.flag &= 4294966783U;
            main_work.flag &= 4290772991U;
            DmTitleOp.DmTitleOpDispRightEnable(false);
            main_work.cur_slct_menu = GsTrialIsTrial() || main_work.is_init_play ? 0 : 1;
            main_work.cur_crsr_pos_y = !GsTrialIsTrial()
                ? main_work.cur_slct_menu * dm_title_crsr_pos_y_tbl[1] + dm_title_crsr_pos_y_tbl[0]
                : dm_title_crsr_trial_pos_y_tbl[main_work.cur_slct_menu];
            SaveState.showResumeWarning();
        }
        else
            dmTitleSetMMenuWinOpenEfct(main_work);
    }

    private static void dmTitleProcMainMenuCloseWin(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 4194304) != 0)
        {
            AoAccountClearCurrentId();
            SSave.CreateInstance().Init();
            main_work.flag &= int.MaxValue;
            main_work.proc_update = (dmTitleProcWaitInput);
            main_work.proc_input = (dmTitleInputProcTitle);
            main_work.flag |= 512U;
            main_work.flag &= 4294966271U;
            main_work.flag &= 4292870143U;
            DmTitleOp.DmTitleOpDispRightEnable(true);
            DmSndBgmPlayerPlayBgm(1);
            main_work.mmenu_win_timer = 0.0f;
            main_work.flag &= 4292870143U;
            main_work.flag &= 4290772991U;
        }
        else
            dmTitleSetMMenuWinCloseEfct(main_work);
    }

    private static void dmTitleProcMainMenuIdle(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.proc_input != null)
            main_work.proc_input(main_work);
        if (SaveState.shouldResume())
        {
            main_work.cur_slct_menu = 0;
            main_work.flag |= 4U;
        }

        if (((int)main_work.flag & 2) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuCloseWin);
            main_work.proc_input = null;
            main_work.mmenu_win_timer = 8f;
            main_work.flag &= 4294966271U;
            main_work.flag &= 4294967293U;
            main_work.flag &= 4294967291U;
            DmSoundPlaySE("Cancel");
        }
        else if (((int)main_work.flag & 4) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuDecideEfct);
            main_work.proc_input = null;
            main_work.timer = 0.0f;
            main_work.flag &= 4294967293U;
            main_work.flag &= 4294967291U;
            DmSoundPlaySE("Ok");
        }
        else if (((int)main_work.flag & 2048) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuCompBuyIdle);
            main_work.proc_input = null;
            main_work.timer = 0.0f;
            main_work.flag |= 262144U;
            //Upsell.launchUpsellScreen(main_work.buy_scr_work);
            main_work.flag &= 4294965247U;
        }
        else
        {
            if (((int)main_work.flag & 64) != 0 || ((int)main_work.flag & 128) != 0)
            {
                dmTitleSetChngFocusCrsrData(main_work);
                DmSoundPlaySE("Cursol");
                main_work.flag &= 4294967231U;
                main_work.flag &= 4294967167U;
            }

            if (((int)main_work.flag & 256) == 0)
                return;
            dmTitleSetCtrlFocusChangeEfct(main_work);
            if (!dmTitleIsCtrlFocusChangeEfctEnd(main_work))
                return;
            main_work.flag &= 4294967039U;
        }
    }

    private static void dmTitleProcMainMenuCompBuyFadeOut(DMS_TITLE_MAIN_WORK main_work)
    {
        if (IzFadeIsEnd())
        {
            main_work.proc_update = (dmTitleProcMainMenuCompBuyIdle);
            //Upsell.launchUpsellScreen(main_work.buy_scr_work);
            main_work.flag &= int.MaxValue;
            main_work.disp_flag &= 4294967293U;
        }

        if (((int)main_work.flag & 256) == 0)
            return;
        dmTitleSetCtrlFocusChangeEfct(main_work);
        if (!dmTitleIsCtrlFocusChangeEfctEnd(main_work))
            return;
        main_work.flag &= 4294967039U;
    }

    private static void dmTitleProcMainMenuCompBuyIdle(DMS_TITLE_MAIN_WORK main_work)
    {
        //if (Upsell.showUpsell)
        //    return;
        if (DmBuyScreenGetResult(main_work.buy_scr_work) == 0)
        {
            if (!GsTrialIsTrial())
            {
                main_work.is_init_play = true;
                main_work.is_no_save_data = true;
                event_after_buy = true;
            }

            if (((int)main_work.flag & 262144) != 0)
                main_work.flag &= 4294705151U;
            else
                IzFadeInitEasy(1U, 0U, 32f);
            DmSndBgmPlayerPlayBgm(0);
            main_work.proc_update = (dmTitleProcMainMenuCompBuyFadeIn);
            main_work.disp_flag |= 2U;
        }
        else
        {
            if (((int)main_work.flag & 262144) != 0)
                main_work.flag &= 4294705151U;
            else
                IzFadeInitEasy(1U, 0U, 32f);
            DmSndBgmPlayerPlayBgm(0);
            main_work.proc_update = (dmTitleProcMainMenuCompBuyFadeIn);
            main_work.disp_flag |= 2U;
        }
    }

    public static void dmTitleProcMainMenuComUpsellScreenFinished()
    {
        IzFadeInitEasy(1U, 0U, 32f);
        DmSndBgmPlayerPlayBgm(0);
    }

    private static void dmTitleProcMainMenuCompBuyFadeIn(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;
        if (DmBuyScreenGetResult(main_work.buy_scr_work) == 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.flag |= 2147483648U;
            if (!GsTrialIsTrial())
            {
                main_work.is_init_play = true;
                main_work.is_no_save_data = true;
                event_after_buy = true;
            }
        }
        else
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.flag |= 2147483648U;
        }

        for (int index = 0; index < 5; ++index)
            main_work.decide_menu_frm[index] = 0.0f;
    }

    private static void dmTitleProcMainMenuTrophyIdle(DMS_TITLE_MAIN_WORK main_work)
    {
    }

    private static void dmTitleProcMainMenuDecideEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 256) != 0)
        {
            dmTitleSetCtrlFocusChangeEfct(main_work);
            if (dmTitleIsCtrlFocusChangeEfctEnd(main_work))
                main_work.flag &= 4294967039U;
        }

        int curSlctMenu = 0;

        // 体験版・初期データ・データありで項目を切り替える
        if (main_work.cur_slct_menu > 0)
        {
            if (GsTrialIsTrial()
                || main_work.is_init_play)
            {
                curSlctMenu = main_work.cur_slct_menu + 1;
            }
            else
            {
                curSlctMenu = main_work.cur_slct_menu;
            }
        }
        if (main_work.timer > 15.0)
        {
            if (curSlctMenu == 4 && (GsTrialIsTrial() || XBOXLive.signinStatus == XBOXLive.SigninStatus.UpdateNeeded ||
                                     XBOXLive.signinStatus == XBOXLive.SigninStatus.Local))
            {
                for (int index = 0; index < 5; ++index)
                    main_work.decide_menu_frm[index] = 0.0f;
                main_work.flag &= 4294967291U;
                main_work.proc_update = (dmTitleProcMainMenuIdle);
                main_work.proc_input = (dmTitleInputProcMainMenu);
                if (XBOXLive.signinStatus == XBOXLive.SigninStatus.UpdateNeeded)
                {
                    XBOXLive.displayTitleUpdateMessage = true;
                }
                else
                {
                    List<string> stringList = new List<string>();
                    stringList.Add(Sonic4ep1.Strings.ID_OK);
                    string text;

                    if (!GsTrialIsTrial())
                    {
                        text = (XBOXLive.signinStatus != XBOXLive.SigninStatus.Local
                            ? Sonic4ep1.Strings.ID_LB_UPDATE
                            : Sonic4ep1.Strings.ID_LB_OFFLINE);
                    }
                    else
                    {
                        text = Sonic4ep1.Strings.ID_LB_BUY;
                    }

                    AppMain.g_ao_sys_global.is_show_ui = true;
                    Guide.BeginShowMessageBox(" ", text, (IEnumerable<string>)stringList, 0, MessageBoxIcon.Alert, new AsyncCallback(TitleMBResult), (object)null);
                }
            }
            else
            {
                if (curSlctMenu == 0 && SaveState.shouldResume())
                {
                    main_work.proc_update = (dmTitleProcFadeOut);
                }
                else
                {
                    if (curSlctMenu == 0 && !main_work.is_init_play && !GsTrialIsTrial())
                    {
                        if (dmTitleIsSaveRunning())
                        {
                            main_work.proc_update = (dmTitleProcMainMenuDelSaveWin);
                            main_work.announce_flag |= 1U << (int)DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA1;
                            if (main_work.iphone_menus)
                                main_work.flag &= 4292869119U;
                            return;
                        }

                        if (main_work.iphone_menus)
                        {
                            main_work.proc_update = (dmTitleProcFadeOut);
                        }
                        else
                        {
                            main_work.proc_update = (dmTitleProcNoSaveCheckInitStart);
                            main_work.announce_flag |= 1U << (int)DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA4;
                        }
                        return;
                    }

                    if (GsTrialIsTrial() && curSlctMenu == 1)
                    {
                        main_work.proc_update = (dmTitleProcMainMenuCompBuyFadeOut);
                        main_work.proc_input = null;
                        main_work.timer = 0.0f;
                        IzFadeInitEasy(0U, 1U, 32f);
                        DmSndBgmPlayerBgmStop();
                        return;
                    }

                    main_work.proc_update = (dmTitleProcFadeOut);
                }

                main_work.timer = 0.0f;
                for (int index = 0; index < 5; ++index)
                    main_work.decide_menu_frm[index] = 0.0f;
                IzFadeInitEasy(0U, 1U, 32f);
                if (curSlctMenu != 0)
                    return;
                DmSndBgmPlayerExit();
                main_work.flag |= 524288U;
            }
        }
        else
        {
            ++main_work.decide_menu_frm[curSlctMenu];
            ++main_work.timer;
        }
    }

    private static void TitleMBResult(IAsyncResult userResult)
    {
        g_ao_sys_global.is_show_ui = false;
    }

    private static void dmTitleProcMainMenuDelSaveWin(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 8192) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.timer = 0.0f;
            main_work.flag &= 4294959103U;
            for (int index = 0; index < 5; ++index)
                main_work.decide_menu_frm[index] = 0.0f;
            main_work.flag |= 2098176U;
        }
        else if (((int)main_work.flag & 16384) != 0)
        {
            SOption instance = SOption.CreateInstance();
            SOption.EControl.Type control = instance.GetControl();
            dmTitleSetInitSaveData(main_work);
            instance.SetControl(control);
            main_work.proc_update = (dmTitleProcSaveInitData);
            DmSaveMenuStart(false, false);
            main_work.flag &= 4294950911U;
        }

        if (((int)main_work.flag & 256) == 0)
            return;
        dmTitleSetCtrlFocusChangeEfct(main_work);
        if (!dmTitleIsCtrlFocusChangeEfctEnd(main_work))
            return;
        main_work.flag &= 4294967039U;
    }

    private static void dmTitleProcNoSaveCheckInitStart(DMS_TITLE_MAIN_WORK main_work)
    {
        if ((main_work.flag & DMD_TITLE_FLAG_DEL_DATA_BACK_MMENU) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.timer = 0.0f;
            main_work.flag &= ~DMD_TITLE_FLAG_DEL_DATA_BACK_MMENU;
            for (int index = 0; index < (int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_NUM; ++index)
                main_work.decide_menu_frm[index] = 0.0f;
        }
        else if ((main_work.flag & DMD_TITLE_FLAG_DEL_DATA_INIT_START) != 0)
        {
            dmTitleSetInitSaveData(main_work);
            main_work.proc_update = (dmTitleProcSaveInitData);
            main_work.flag &= ~DMD_TITLE_FLAG_DEL_SAVE_DATA;
        }

        if ((main_work.flag & DMD_TITLE_FLAG_MENU_CRSR_EFCT) == 0)
            return;
        dmTitleSetCtrlFocusChangeEfct(main_work);
        if (!dmTitleIsCtrlFocusChangeEfctEnd(main_work))
            return;
        main_work.flag &= ~DMD_TITLE_FLAG_MENU_CRSR_EFCT;
    }

    private static void dmTitleProcSaveInitData(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!DmSaveIsExit())
            return;
        if (!GsTrialIsTrial() && !DmCmnBackupIsSaveSuccessed())
        {
            main_work.proc_update = (dmTitleProcWaitInput);
            main_work.proc_input = (dmTitleInputProcTitle);
            main_work.flag |= 512U;
            main_work.flag &= 4294966271U;
            main_work.flag &= int.MaxValue;
            DmSndBgmPlayerPlayBgm(1);
        }
        else
        {
            main_work.proc_update = (dmTitleProcFadeOut);
            main_work.timer = 0.0f;
            IzFadeInitEasy(0U, 1U, 32f);
            DmSndBgmPlayerExit();
            main_work.flag |= 524288U;
        }
    }

    private static void dmTitleProcFadeOut(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;
        DmTitleOp.DmTitleOpExit();
        main_work.proc_update = (dmTitleProcDataRelease);
        main_work.proc_draw = null;
    }

    private static void dmTitleProcWindowNodispIdle(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.proc_win_input != null)
            main_work.proc_win_input(main_work);
        if (main_work.announce_flag == 0U)
            return;
        main_work.proc_win_update = (dmTitleProcWindowOpenEfct);
        main_work.proc_input = null;
        main_work.proc_win_input = null;
        main_work.win_timer = 0.0f;
        for (int index = 0; index < 5; ++index)
        {
            if ((main_work.announce_flag & (uint)(1 << index)) != 0)
            {
                main_work.win_mode = index;
                break;
            }
        }

        main_work.win_cur_slct = 0;
        DmSoundPlaySE("Window");
        main_work.flag |= 131072U;
    }

    private static void dmTitleProcWindowOpenEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 65536) != 0)
        {
            main_work.proc_win_update = (dmTitleProcWindowAnnounceIdle);
            main_work.proc_win_input = dm_title_variant.proc_win_input;
            main_work.disp_flag |= 1U;
            main_work.flag &= 4294901759U;
        }
        else
            dmTitleSetWinOpenEfct(main_work);
    }

    private static void dmTitleProcWindowAnnounceIdle(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.proc_win_input != null)
            main_work.proc_win_input(main_work);
        if (main_work.win_mode == 0)
        {
            if (((int)main_work.flag & 4) != 0 && main_work.win_cur_slct == 0)
            {
                main_work.proc_input = null;
                main_work.proc_win_input = null;
                main_work.win_timer = 8f;
                main_work.disp_flag &= 4294967294U;
                main_work.proc_win_update = (dmTitleProcWindowCloseEfct);
                DmSoundPlaySE("Ok");
                main_work.flag &= 4294967291U;
                main_work.flag &= 4294967293U;
            }
            else
            {
                if (((int)main_work.flag & 2) == 0 && (main_work.win_cur_slct != 1 || ((int)main_work.flag & 4) == 0))
                    return;
                main_work.proc_input = null;
                main_work.proc_win_input = null;
                main_work.win_timer = 8f;
                main_work.disp_flag &= 4294967294U;
                main_work.proc_win_update = (dmTitleProcWindowCloseEfct);
                DmSoundPlaySE("Cancel");
                main_work.flag |= 2U;
                main_work.flag &= 4294967291U;
            }
        }
        else
        {
            if (main_work.win_mode != 1)
                return;
            if (((int)main_work.flag & 4) != 0 && main_work.win_cur_slct == 0)
            {
                main_work.proc_input = null;
                main_work.proc_win_input = null;
                main_work.win_timer = 8f;
                main_work.disp_flag &= 4294967294U;
                main_work.proc_win_update = (dmTitleProcWindowCloseEfct);
                DmSoundPlaySE("Ok");
                main_work.flag &= 4294967291U;
                main_work.flag &= 4294967293U;
            }
            else
            {
                if (((int)main_work.flag & 2) == 0 && (main_work.win_cur_slct != 1 || ((int)main_work.flag & 4) == 0))
                    return;
                main_work.proc_input = null;
                main_work.proc_win_input = null;
                main_work.win_timer = 8f;
                main_work.disp_flag &= 4294967294U;
                main_work.proc_win_update = (dmTitleProcWindowCloseEfct);
                DmSoundPlaySE("Cancel");
                main_work.flag |= 2U;
                main_work.flag &= 4294967291U;
            }
        }
    }

    private static void dmTitleProcWindowCloseEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 65536) != 0)
        {
            main_work.proc_win_update = (dmTitleProcWindowNodispIdle);
            main_work.announce_flag &= (uint)~(1 << main_work.win_mode);
            if (main_work.win_mode == 0 || main_work.win_mode == 1)
            {
                if (((int)main_work.flag & 2) != 0)
                {
                    main_work.flag |= 8192U;
                    main_work.flag &= 4294967293U;
                }
                else if (main_work.win_mode == 0)
                    main_work.announce_flag |= 2U;
                else if (main_work.win_mode == 1)
                    main_work.flag |= 16384U;
            }
            else if (((int)main_work.flag & 2) != 0)
            {
                main_work.flag |= 16777216U;
                main_work.flag &= 4294967293U;
            }
            else if (main_work.win_mode == 2)
                main_work.flag |= 8388608U;
            else if (main_work.win_mode == 3)
                main_work.flag |= 33554432U;
            else
                main_work.flag |= 67108864U;

            main_work.flag &= 4294836223U;
            main_work.flag &= 4294901759U;
        }

        dmTitleSetWinCloseEfct(main_work);
    }

    private static void dmTitleProcDataRelease(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!DmTitleOp.DmTitleOpExitEndCheck())
            return;
        for (int index = 0; index < 2; ++index)
            AoTexRelease(main_work.tex[index]);
        for (int index = 0; index < 4; ++index)
            AoTexRelease(main_work.cmn_tex[index]);
        DmTitleOp.DmTitleOpFlush();
        main_work.proc_update = (dmTitleProcFinish);
    }

    private static void dmTitleProcFinish(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!dmTitleIsTexRelease(main_work))
            return;
        int length1 = main_work.trg_slct.Length;
        for (int index = 0; index < length1; ++index)
            main_work.trg_slct[index].Release();
        int length2 = main_work.trg_answer.Length;
        for (int index = 0; index < length2; ++index)
            main_work.trg_answer[index].Release();
        main_work.trg_return.Release();
        main_work.trg_game.Release();
        for (int index = 0; index < 41; ++index)
        {
            if (main_work.act[index] != null)
            {
                AoActDelete(main_work.act[index]);
                main_work.act[index] = null;
            }
        }

        for (int index = 0; index < 2; ++index)
        {
            if (main_work.arc_amb[index] != null)
                main_work.arc_amb[index] = null;
        }

        for (int index = 0; index < 4; ++index)
        {
            if (main_work.arc_cmn_amb[index] != null)
                main_work.arc_cmn_amb[index] = null;
        }

        DmTitleOp.DmTitleOpRelease();
        main_work.proc_update = (dmTitleProcWaitFinished);
    }

    private static void dmTitleProcWaitFinished(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!DmTitleOp.DmTitleOpReleaseCheck())
            return;
        if (((int)main_work.flag & 524288) != 0)
        {
            if (!DmSndBgmPlayerIsTaskExit())
                return;
            main_work.flag &= 4294443007U;
        }

        main_work.flag |= 1U;
        main_work.proc_update = null;
    }

    private static void dmTitleInputProcTitle(DMS_TITLE_MAIN_WORK main_work)
    {
        var stand = AoPad.AoPadSomeoneMStand(ControllerConsts.START | ControllerConsts.CONFIRM);
        if (amTpIsTouchPush(0) || stand != -1)
        {
            AoAccountSetCurrentIdStart(stand != -1 ? (uint)stand : 0);
            main_work.flag |= DMD_TITLE_FLAG_GAME_START;
        }

        if (!isBackKeyPressed())
            return;
        finish();
        setBackKeyRequest(false);
    }

    private static void dmTitleInputProcMainMenu(DMS_TITLE_MAIN_WORK main_work)
    {
        int length = main_work.trg_slct.Length;
        if (!LiveFeature.isInterrupted())
        {
            int now_slct_menu = 0;
            int cur_edge_menu_top = 0;
            int cur_edge_menu_bottom = 0;

            // 体験版・初期データ・データありで項目を切り替える
            if (main_work.cur_slct_menu > 0)
            {
                if (GsTrialIsTrial()
                    || main_work.is_init_play)
                {
                    now_slct_menu = main_work.cur_slct_menu + 1;
                }
                else
                {
                    now_slct_menu = main_work.cur_slct_menu;
                }
            }

            // キャンセル処理
            if (AoPad.AoPadMStand().HasFlag(ControllerConsts.CANCEL))
            {
                main_work.flag |= DMD_TITLE_FLAG_CANCEL;

                return;
            }

            // 決定処理
            if (AoPad.AoPadMStand().HasFlag(ControllerConsts.CONFIRM) || AoPad.AoPadMStand().HasFlag(ControllerConsts.START))
            {
                main_work.flag |= DMD_TITLE_FLAG_DECIDE;

                return;
            }


            // 体験版時の上下の端に当たるメニュー項目
            if (GsTrialIsTrial())
            {
                cur_edge_menu_top = main_work.slct_menu_num - 1;
                cur_edge_menu_bottom = main_work.slct_menu_num - 2;
            }
            // 製品版時の上下の端に当たるメニュー項目
            else
            {
                cur_edge_menu_top = 0;
                cur_edge_menu_bottom = main_work.slct_menu_num - 1;
            }

            // 十字キー操作
            if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.UP))
            {
                if (AoPad.AoPadMStand().HasFlag(ControllerConsts.UP)
                    || main_work.cur_slct_menu != cur_edge_menu_top)
                {
                    // 選択ZONE切り替え
                    main_work.flag |= DMD_TITLE_FLAG_MENU_UP_INPUT;
                }
            }
            else if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.DOWN))
            {
                if (AoPad.AoPadMStand().HasFlag(ControllerConsts.DOWN)
                    || main_work.cur_slct_menu != cur_edge_menu_bottom)
                {
                    // 選択ZONE切り替え
                    main_work.flag |= DMD_TITLE_FLAG_MENU_DOWN_INPUT;
                }
            }

            for (int index = 0; index < length; ++index)
            {
                CTrgAoAction ctrgAoAction = main_work.trg_slct[index];
                if (ctrgAoAction.GetState(0U)[10] && ctrgAoAction.GetState(0U)[1])
                {
                    main_work.cur_slct_menu = index;
                    main_work.flag |= 4U;
                    break;
                }
            }

            CTrgAoAction trgReturn = main_work.trg_return;
            if (trgReturn.GetState(0U)[10] && trgReturn.GetState(0U)[1] || isBackKeyPressed() ||
                AoPad.AoPadMStand().HasFlag(ControllerConsts.B))
            {
                setBackKeyRequest(false);
                //if (LiveFeature.isInterrupted())
                //    LiveFeature.endInterrupt();
                //else
                    main_work.flag |= 2U;
            }


            CTrgAoAction trgGame = main_work.trg_game;
            if (trgGame.GetState(0U)[10] && trgGame.GetState(0U)[1] || AoPad.AoPadMStand().HasFlag(ControllerConsts.Y))
            {
                string url = GsEnvGetLanguage() != 0 ? c_url : "http://sega.jp/kt/microsoft/smart/";
                DmSoundPlaySE("Ok");
                erWeb.StartWeb(url);
            }
        }
    }

    private static void dmTitleInputProcWindowPc(DMS_TITLE_MAIN_WORK main_work)
    {
        var pad = AoPad.AoPadMStand();

        if (pad.HasFlag(ControllerConsts.CANCEL))
            main_work.flag |= DMD_TITLE_FLAG_CANCEL;

        if (pad.HasFlag(ControllerConsts.CONFIRM))
            main_work.flag |= DMD_TITLE_FLAG_DECIDE;

        if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.LEFT))
        {
            if (main_work.win_cur_slct != 0)
            {
                main_work.win_cur_slct = 0;
                DmSoundPlaySE("Cursol");
            }
        }
        else if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.RIGHT))
        {
            if (main_work.win_cur_slct != 1)
            {
                main_work.win_cur_slct = 1;
                DmSoundPlaySE("Cursol");
            }
        }
    }

    private static void dmTitleInputProcWindowIphone(DMS_TITLE_MAIN_WORK main_work)
    {
        CTrgAoAction[] trgAnswer = main_work.trg_answer;

        for (int i = 0; i < 2; i++)
            trgAnswer[i].IsSelected = i == main_work.win_cur_slct;

        if (trgAnswer[0].GetState(0U)[10] && trgAnswer[0].GetState(0U)[1])
        {
            main_work.win_cur_slct = 1;
            main_work.flag |= DMD_TITLE_FLAG_DECIDE;
        }

        if (trgAnswer[1].GetState(0U)[10] && trgAnswer[1].GetState(0U)[1])
        {
            main_work.win_cur_slct = 0;
            main_work.flag |= DMD_TITLE_FLAG_DECIDE;
        }
    }

    // ==========================================================================
    // dmTitleDrawSetProcDispData
    /*!
        表示設定処理
     */
    // ==========================================================================
    private static void dmTitleDrawSetProcDispData(DMS_TITLE_MAIN_WORK main_work)
    {
        float tmp_win_size = 0;

        // 描画フラグONならば
        if ((main_work.disp_flag & DMD_TITLE_DISP_FLAG_ALL_ACTION) == DMD_TITLE_DISP_FLAG_ALL_ACTION)
        {
            // 共通描画設定部
            AoActSysSetDrawTaskPrio(0x8000);
            DmTitleOp.DmTitleOpDraw2D();

            AoActSortExecute();
            AoActSortDraw();
            AoActSortUnregAll();

            // 場面別描画設定部
            if ((main_work.flag & DMD_TITLE_FLAG_DISP_TITLE) == DMD_TITLE_FLAG_DISP_TITLE
                && DmTitleOp.DmTitleOpIsLogoActFinish())
            {
                dmTitleDrawProcTitle(main_work);
            }

            // メインメニュー用ウインドウ
            if ((main_work.flag & DMD_TITLE_FLAG_DISP_MMENU_WIN) == DMD_TITLE_FLAG_DISP_MMENU_WIN)
            {
                // ウインドウ用AO描画プライオリティ設定
                AoActSysSetDrawTaskPrio(DMD_TITLE_DRAW_PRIO_WIN_FIX);

                if (!GsTrialIsTrial() && main_work.is_init_play)
                {
                    tmp_win_size = -50;
                }
                else
                {
                    tmp_win_size = 0;
                }

                // サインアウト中に開閉演出中だった場合はウインドウを非表示
                if (main_work.mmenu_win_size_rate[0] < 0.9
                    && !AoAccountIsCurrentEnable())
                {
                    // 空処理
                }
                else
                {
                    // ウインドウ描画
                    if (!main_work.iphone_menus)
                    {
                        AoWinSysDrawState(AOD_WIN_TYPE_A
                                         , AoTexGetTexList(main_work.cmn_tex[2])          // ◆
                                         , 0
                                         , DMD_TITLE_SIZE_WIDTH / 2.0f          // ウインドウ中心X
                                         , DMD_TITLE_SIZE_HEIGHT / 2.0f + 130.0f         // ウインドウ中心Y
                                         , DMD_TITLE_MMENU_WIN_SIZE_W_PC * main_work.mmenu_win_size_rate[0]                   // ウインドウ横サイズ
                                         , DMD_TITLE_MMENU_WIN_SIZE_H_PC * main_work.mmenu_win_size_rate[1] + tmp_win_size    // ウインドウ縦サイズ
                                         , DMD_TITLE_DRAW_STATE_ID              // 描画STATE
                                         );
                    }
                    else
                    {
                        float[] win_size = { DMD_TITLE_MMENU_WIN_SIZE_W_IOS, DMD_TITLE_MMENU_WIN_SIZE_H_IOS };
                        bool is_trial = GsTrialIsTrial();
                        bool has_save_data = !main_work.is_init_play;
                        bool is_disp_2nd_elm = ((is_trial) ? true : has_save_data); //for SONIC4_TRIAL_EXIBITION set it false
                        if (!is_disp_2nd_elm)
                        {
                            win_size[AMD_X] = DMD_TITLE_MMENU_WIN_SIZE_W_2;
                            win_size[AMD_Y] = DMD_TITLE_MMENU_WIN_SIZE_H_2;
                        }
                        {//black bg rect
                            AoWinSysDrawState(AOD_WIN_TYPE_A
                                         , AoTexGetTexList(main_work.cmn_tex[2])          // ◆
                                         , 0
                                         , DMD_TITLE_SIZE_WIDTH / 2.0f          // ウインドウ中心X
                                         , 311.0f                                        // ウインドウ中心Y
                                         , win_size[AMD_X] * main_work.mmenu_win_size_rate[0]                  // ウインドウ横サイズ
                                         , win_size[AMD_Y] * main_work.mmenu_win_size_rate[1] + tmp_win_size   // ウインドウ縦サイズ
                                         , DMD_TITLE_DRAW_STATE_ID              // 描画STATE
                                         );
                        }


                    }
                }
                {
                    if ((main_work.flag & DMD_TITLE_FLAG_DISP_MAINMENU) == DMD_TITLE_FLAG_DISP_MAINMENU)
                    {
                        if (main_work.iphone_menus)
                        {
                            if ((main_work.flag_prev & DMD_TITLE_FLAG_DISP_MAINMENU) == 0)
                            {
                                //DMD_TITLE_FLAG_DISP_MAINMENUのONエッジ					
                                main_work.trg_return.ResetState();
                                for (int k = (int)DME_TITLE_ACT.ACT_BACK_BTN_L, max = (int)DME_TITLE_ACT.ACT_BACK_BTN_R + 1; k < max; ++k)
                                {
                                    AoActSetFrame(main_work.act[k], 0.0f);
                                }
                                //戻るボタンの演出を戻す	
                                main_work.trg_game.ResetState();
                                for (int k = (int)DME_TITLE_ACT.ACT_GAME_BTN_L, max = (int)DME_TITLE_ACT.ACT_GAME_BTN_R + 1; k < max; ++k)
                                {
                                    AoActSetFrame(main_work.act[k], 0.0f);
                                }
                            }
                        }
                        dmTitleDrawProcMainMenu(main_work);//draw softkeys and menu items
                    }
                }

                if ((main_work.flag & DMD_TITLE_FLAG_WIN_DRAW_START) == DMD_TITLE_FLAG_WIN_DRAW_START)
                {
                    // ウインドウ描画
                    dmTitleWinSelectDraw(main_work);
                }
            }
        }

        // 描画タスク生成
        amDrawMakeTask(dmTitleTaskDraw, 0x8000, 0);
    }

    private static void dmTitleDrawProcTitle(DMS_TITLE_MAIN_WORK main_work)
    {
        var ACT_TEX_START = main_work.iphone_menus ? (int)DME_TITLE_ACT.ACT_TEX_START : (int)DME_TITLE_ACT_REG.ACT_TEX_START;

        AoActSysSetDrawTaskPrio(33792U);
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        if ((main_work.flag & DMD_TITLE_FLAG_DISP_TEXT) == DMD_TITLE_FLAG_DISP_TEXT)
        {
            AoActSortRegAction(main_work.act[ACT_TEX_START]);
        }

        if (((int)main_work.flag & 16) != 0)
        {
            if (main_work.disp_timer >= 4.0)
            {
                main_work.disp_timer = 0.0f;
                main_work.flag ^= 8U;
            }
            else
                ++main_work.disp_timer;
        }
        else if (main_work.disp_timer >= (double)main_work.disp_change_time)
        {
            main_work.flag ^= 8U;
            main_work.disp_change_time = ((int)main_work.flag & 8) == 0 ? 30 : 40;
            main_work.disp_timer = 0.0f;
        }
        else
            ++main_work.disp_timer;

        if (GeEnvGetDecideKey() == GSE_DECIDE_KEY.GSD_DECIDE_KEY_O)
            AoActSetFrame(main_work.act[ACT_TEX_START], 0.0f);
        else
            AoActSetFrame(main_work.act[ACT_TEX_START], 1f);

        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        AoActUpdate(main_work.act[ACT_TEX_START], 0.0f);
        AoActSortExecute();
        AoActSortDraw();
        AoActSortUnregAll();
    }

    private static void dmTitleDrawProcMainMenu(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.iphone_menus)
        {
            bool is_trial = GsTrialIsTrial();
            bool has_save_data = !main_work.is_init_play;
            dmTitleDrawProcMainMenuIphone(main_work, has_save_data, is_trial);
        }
        else
        {
            float init_slct_dist = 0.0f;
            int init_dst_menu_data = 0;

            AoActSysSetDrawTaskPrio(DMD_TITLE_DRAW_PRIO_ACT);

            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

            AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_MENU_CRSR]);

            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

            if (GsTrialIsTrial())
            {
                dmTitleDrawProcMainMenuTrial(main_work);
            }

            else if (main_work.is_init_play)
            {
                dmTitleDrawProcMainMenuFirst(main_work);
            }

            else
            {
                dmTitleDrawProcMainMenuPlayed(main_work);
            }


            for (int i = (int)DME_TITLE_ACT_REG.ACT_TEX_GAME; i < (int)DME_TITLE_ACT_REG.ACT_BTN_CANCEL; i++)
            {
                if (main_work.act[i] != null)
                    AoActSetFrame(main_work.act[i], 1.0f);
            }

            // 決定演出用フレーム設定
            AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME_L]
                          , main_work.decide_menu_frm[(int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_START]);
            AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION_L]
                          , main_work.decide_menu_frm[(int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_OPTION]);


            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

            AoActAcmPush();
            AoActAcmInit();
            AoActAcmApplyTrans(DMD_TITLE_CRSR_DFLT_POS_X
                               , main_work.cur_crsr_pos_y
                               , 0.0f
                               );

            if (!GsTrialIsTrial() && main_work.is_init_play)
            {
                AoActAcmApplyTrans(0.0f, 85.0f, 0.0f);
            }
            else
            {
                AoActAcmApplyTrans(0.0f, 60.0f, 0.0f);
            }

            AoActUpdate(main_work.act[(int)DME_TITLE_ACT_REG.ACT_MENU_CRSR], 0.0f);
            AoActAcmPop();

            //#if _PC || _XBOX
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[0]));
            AoActUpdate(main_work.act[(int)DME_TITLE_ACT_REG.ACT_BTN_X], 0.0f);
            //#endif

            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

            // ここでPFごとの表示アクションを設定
#if _PC || _XBOX
    init_dst_menu_data = ACT_TEX_BUY_L;
#elif _PS3
    init_dst_menu_data = ACT_TEX_ZISSEKI_L;
#elif _WII
    init_dst_menu_data = ACT_TEX_RANK_L;
#endif
            init_dst_menu_data = (int)DME_TITLE_ACT_REG.ACT_TEX_BUY_L;

            if (GsTrialIsTrial())
            {
                AoActAcmPush();

                for (int i = (int)DME_TITLE_ACT_REG.ACT_TEX_GAME; i < (int)DME_TITLE_ACT_REG.ACT_BTN_CANCEL; i++)
                {
                    AoActAcmInit();

                    if (i == (int)DME_TITLE_ACT_REG.ACT_TEX_GAME_L
                        || i == (int)DME_TITLE_ACT_REG.ACT_TEX_GAME)
                    {
                        init_slct_dist = 50.0f;
                    }
                    else
                    {
                        init_slct_dist = 0.0f;
                    }

                    AoActAcmApplyTrans(0.0f, init_slct_dist, 0.0f);
                    AoActUpdate(main_work.act[i], 0.0f);
                }
                AoActAcmPop();
            }
            else if (GsTrialIsTrial() || main_work.is_init_play)
            {
                AoActAcmPush();

                for (int i = (int)DME_TITLE_ACT_REG.ACT_TEX_GAME; i < (int)DME_TITLE_ACT_REG.ACT_BTN_CANCEL; i++)
                {
                    AoActAcmInit();
                    if (i >= (int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY && i <= init_dst_menu_data)
                    {
                        init_slct_dist = -50.0f;
                    }
                    else if (i >= (int)DME_TITLE_ACT_REG.ACT_TEX_OPTION && i <= init_dst_menu_data)
                    {
                        init_slct_dist = 0.0f;
                    }
                    else
                    {
                        init_slct_dist = 50.0f;
                    }

                    AoActAcmApplyTrans(0.0f, init_slct_dist, 0.0f);

                    if (!GsTrialIsTrial() && main_work.is_init_play)
                    {
                        AoActAcmApplyTrans(0.0f, 35.0f, 0.0f);
                    }

                    AoActUpdate(main_work.act[i], 0.0f);
                }
                AoActAcmPop();
            }
            else
            {
                AoActAcmPush();
                for (int i = (int)DME_TITLE_ACT_REG.ACT_TEX_GAME; i < (int)DME_TITLE_ACT_REG.ACT_BTN_CANCEL; i++)
                {
                    AoActAcmInit();
                    if (i >= (int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY && i <= init_dst_menu_data)
                    {
                        init_slct_dist = 0.0f;
                    }
                    else
                    {
                        init_slct_dist = 50.0f;
                    }

                    AoActAcmApplyTrans(0.0f, init_slct_dist, 0.0f);
                    AoActAcmApplyTrans(0.0f, 10.0f, 0.0f);
                    AoActUpdate(main_work.act[i], 0.0f);
                }
                AoActAcmPop();
            }

            // ソート描画
            AoActSortExecute();
            AoActSortDraw();
            AoActSortUnregAll();
        }
    }



    // ==========================================================================
    // dmTitleDrawProcMainMenuTrial
    /*!
        メインメニュー時表示設定処理
     */
    // ==========================================================================
    static void dmTitleDrawProcMainMenuTrial(DMS_TITLE_MAIN_WORK main_work)
    {
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

        switch (main_work.cur_slct_menu)
        {
            case 0:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME_L]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                //AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME], 0.0f);
                break;
            case 1:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION_L]);
                //AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION_L], 0.0f);
                break;
            case 2:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                //AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY], 0.0f);
                break;
            case 3:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                //AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                break;
            default:
                break;
        }
    }



    // ==========================================================================
    // dmTitleDrawProcMainMenuFirst
    /*!
        メインメニュー時表示設定処理
     */
    // ==========================================================================
    static void dmTitleDrawProcMainMenuFirst(DMS_TITLE_MAIN_WORK main_work)
    {
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

        switch (main_work.cur_slct_menu)
        {
            case 0:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME_L]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME], 0.0f);
                break;
            case 1:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION_L]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION], 0.0f);
                break;
            case 2:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY_L]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY], 0.0f);
                break;
            default:
                break;
        }

    }

    // ==========================================================================
    // dmTitleDrawProcMainMenuPlayed
    /*!
        メインメニュー時表示設定処理
     */
    // ==========================================================================
    static void dmTitleDrawProcMainMenuPlayed(DMS_TITLE_MAIN_WORK main_work)
    {
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

        switch (main_work.cur_slct_menu)
        {
            case 0:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME_L]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_TUDUKI]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME_L], 0.0f);
                break;
            case 1:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_TUDUKI_L]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION], 0.0f);
                break;
            case 2:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_TUDUKI]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION_L]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION], 0.0f);
                break;
            case 3:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_TUDUKI]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY_L]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY], 0.0f);
                break;
            default:
                break;
        }

    }


    private static void CLocalBtnBase_Update(
        AOS_ACTION src,
        ArrayPointer<AOS_ACTION> btn,
        CTrgAoAction trg)
    {
        CLocalBtnBase_Update(src, btn, trg, 0.0f);
    }

    private static void CLocalBtnBase_Update(
        AOS_ACTION src,
        ArrayPointer<AOS_ACTION> btn,
        CTrgAoAction trg,
        float frame)
    {
        AoActAcmPush();
        AoActAcmInit();
        int[] numArray = new int[2]
        {
            (int) src.sprite.center_x,
            (int) src.sprite.center_y
        };
        AoActAcmApplyTrans(numArray[0], numArray[1], 1f);
        AoActUpdate(btn[0], frame);
        AoActUpdate(btn[1], frame);
        AoActUpdate(btn[2], frame);
        trg.Update();
        AoActAcmPop();
    }

    private static void CLocalBtnBase_Draw(ArrayPointer<AOS_ACTION> btn)
    {
        AoActSortRegAction(btn[0]);
        AoActSortRegAction(btn[1]);
        AoActSortRegAction(btn[2]);
    }

    private static void CLocalBtnBase_SetFrame(ArrayPointer<AOS_ACTION> btn, float frame)
    {
        AoActSetFrame(btn[0], frame);
        AoActSetFrame(btn[1], frame);
        AoActSetFrame(btn[2], frame);
    }

    private static void dmTitleDrawProcMainMenuIphone(
        DMS_TITLE_MAIN_WORK main_work,
        bool has_save_data,
        bool is_trial)
    {
        AoActSysSetDrawTaskPrio(33792U);
        int[] numArray1 = new int[3] { 20, 21, 22 };
        bool flag = has_save_data;
        int[] numArray2 = new int[3] { 0, 3, 6 };
        float[] numArray3 = new float[3];
        if (is_trial)
        {
            numArray1[1] = 23;
            flag = true;
        }

        if ((dmTitleProcMainMenuDecideEfct) == main_work.proc_update)
            numArray3[main_work.cur_slct_menu] = 1f;
        else if ((!IzFadeIsExe() || IzFadeIsEnd()) &&
                 (!((dmTitleProcSaveInitData) == main_work.proc_update) &&
                  (131072 & (int)main_work.flag) == 0))
        {
            for (int index = 0; index < 3; ++index)
            {
                float frame;
                if (!main_work.trg_slct[index].GetState(0U)[10] || !main_work.trg_slct[index].GetState(0U)[1])
                    frame = !main_work.trg_slct[index].GetState(0U)[0] ? 0.0f : 1f;
                else
                    frame = 2f;
                AoActSetFrame(main_work.act[numArray1[index]], frame);
                CLocalBtnBase_SetFrame(new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[index]), frame);
            }

            float frame1;
            if (!main_work.trg_return.GetState(0U)[10] || !main_work.trg_return.GetState(0U)[1])
                if (!main_work.trg_return.GetState(0U)[0])
                    frame1 = 2.0 > main_work.act[15].frame ? 0.0f : main_work.act[15].frame;
                else
                    frame1 = 1f;
            else
                frame1 = 2f;

            AoActSetFrame(main_work.act[26], frame1);
            int index1 = 15;
            for (int index2 = 17; index1 < index2; ++index1)
                AoActSetFrame(main_work.act[index1], frame1);
            float frame2;

            if (!main_work.trg_game.GetState(0U)[10] || !main_work.trg_game.GetState(0U)[1])
                if (!main_work.trg_game.GetState(0U)[0])
                    frame2 = 2.0 > main_work.act[17].frame ? 0.0f : main_work.act[17].frame;
                else
                    frame2 = 1f;
            else
                frame2 = 2f;

            AoActSetFrame(main_work.act[27], frame2);
            int index3 = 17;
            for (int index2 = 19; index3 < index2; ++index3)
                AoActSetFrame(main_work.act[index3], frame2);
        }

        if (!LiveFeature.isInterrupted())
        {
            var dm_title_g_dm_act_id_tbl = dm_title_variant.act_id_tbl;

            if (!flag)
            {
                main_work.ama[1].act_tbl[(int)dm_title_g_dm_act_id_tbl[20]].mtn.trs_tbl[0].trs_y = 250f;
                main_work.ama[1].act_tbl[(int)dm_title_g_dm_act_id_tbl[22]].mtn.trs_tbl[0].trs_y = 360f;
                //main_work.ama[1].act_tbl[(int)dm_title_g_dm_act_id_tbl[24]].mtn.trs_tbl[0].trs_y = 470f;
                //main_work.ama[1].act_tbl[(int)dm_title_g_dm_act_id_tbl[25]].mtn.trs_tbl[0].trs_y = 580f;
            }
            else
                main_work.ama[1].act_tbl[(int)dm_title_g_dm_act_id_tbl[21]].mtn.trs_tbl[0].trs_y = 305f;

            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            AoActUpdate(main_work.act[numArray1[0]], numArray3[0]);
            if (flag)
            {
                AoActUpdate(main_work.act[numArray1[1]], numArray3[1]);
                main_work.act[numArray1[1]].sprite.center_y = 305f;
            }

            AoActUpdate(main_work.act[numArray1[2]], numArray3[2]);
            //AoActUpdate(main_work.act[numArray1[3]], numArray3[3]);
            //AoActUpdate(main_work.act[numArray1[4]], numArray3[4]);
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
            CLocalBtnBase_Update(main_work.act[numArray1[0]],
                new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[0]), main_work.trg_slct[0], numArray3[0]);
            if (flag)
                CLocalBtnBase_Update(main_work.act[numArray1[1]],
                    new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[1]), main_work.trg_slct[1], numArray3[1]);
            CLocalBtnBase_Update(main_work.act[numArray1[2]],
                new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[2]), main_work.trg_slct[2], numArray3[2]);
            // CLocalBtnBase_Update(main_work.act[numArray1[3]], new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[3]), main_work.trg_slct[3], numArray3[3]);
            // CLocalBtnBase_Update(main_work.act[numArray1[4]], new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[4]), main_work.trg_slct[4], numArray3[4]);
        }

        float frame3 = 2.0 <= main_work.act[15].frame ? 1f : 0.0f;
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        AoActUpdate(main_work.act[26], frame3);
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        int index4 = 15;
        for (int index1 = 17; index4 < index1; ++index4)
            AoActUpdate(main_work.act[index4], frame3);
        main_work.trg_return.Update();
        float frame4 = 2.0 <= main_work.act[17].frame ? 1f : 0.0f;
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        AoActUpdate(main_work.act[27], frame4);
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        int index5 = 17;
        for (int index1 = 19; index5 < index1; ++index5)
            AoActUpdate(main_work.act[index5], frame4);
        main_work.trg_game.Update();
        if (!LiveFeature.isInterrupted())
        {
            CLocalBtnBase_Draw(new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[0]));
            AoActSortRegAction(main_work.act[numArray1[0]]);
            main_work.act[numArray1[0]].sprite.center_y -= 3f;
            if (flag)
            {
                CLocalBtnBase_Draw(new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[1]));
                AoActSortRegAction(main_work.act[numArray1[1]]);
                main_work.act[numArray1[1]].sprite.center_y -= 3f;
            }

            CLocalBtnBase_Draw(new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[2]));
            AoActSortRegAction(main_work.act[numArray1[2]]);
            main_work.act[numArray1[2]].sprite.center_y -= 3f;
            // CLocalBtnBase_Draw(new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[3]));
            // AoActSortRegAction(main_work.act[numArray1[3]]);
            // main_work.act[numArray1[3]].sprite.center_y -= 3f;
            // CLocalBtnBase_Draw(new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[4]));
            // AoActSortRegAction(main_work.act[numArray1[4]]);
            // main_work.act[numArray1[4]].sprite.center_y -= 3f;
        }

        if ((2097152 & (int)main_work.flag) != 0 && (131072 & (int)main_work.flag) == 0)
        {
            int index1 = 15;
            for (int index2 = 17; index1 < index2; ++index1)
                AoActSortRegAction(main_work.act[index1]);
            AoActSortRegAction(main_work.act[26]);
            int index3 = 17;
            for (int index2 = 19; index3 < index2; ++index3)
                AoActSortRegAction(main_work.act[index3]);
            AoActSortRegAction(main_work.act[27]);
        }

        AoActSortExecute();
        AoActSortDraw();
        AoActSortUnregAll();
    }

    private static void dmTitleWinSelectDraw(DMS_TITLE_MAIN_WORK main_work)
    {
        var dm_title_win_act_pos_tbl = dm_title_variant.win_act_pos_tbl;
        var ACT_WIN_NO_BTN_L = (int)DME_TITLE_ACT.ACT_WIN_NO_BTN_L;
        var ACT_WIN_YES_BTN_R = (int)DME_TITLE_ACT.ACT_WIN_YES_BTN_R;
        var ACT_TEX_YES = main_work.iphone_menus ? (int)DME_TITLE_ACT.ACT_TEX_YES : (int)DME_TITLE_ACT_REG.ACT_TEX_YES;
        var ACT_TEX_NO = main_work.iphone_menus ? (int)DME_TITLE_ACT.ACT_TEX_NO : (int)DME_TITLE_ACT_REG.ACT_TEX_NO;
        var ACT_WIN_TEX_MSG1 = main_work.iphone_menus ? (int)DME_TITLE_ACT.ACT_WIN_TEX_MSG1 : (int)DME_TITLE_ACT_REG.ACT_WIN_TEX_MSG1;
        var ACT_WIN_TEX_MSG2 = main_work.iphone_menus ? (int)DME_TITLE_ACT.ACT_WIN_TEX_MSG2 : (int)DME_TITLE_ACT_REG.ACT_WIN_TEX_MSG2;
        var ACT_WIN_TEX_MSG3 = main_work.iphone_menus ? 0 : (int)DME_TITLE_ACT_REG.ACT_WIN_TEX_MSG3;
        var ACT_WIN_LINE = main_work.iphone_menus ? 0 : (int)DME_TITLE_ACT_REG.ACT_WIN_LINE;
        var ACT_TEX_WINTITLE = main_work.iphone_menus ? 0 : (int)DME_TITLE_ACT_REG.ACT_TEX_WINTITLE;

        // ウインドウ用AO描画プライオリティ設定
        AoActSysSetDrawTaskPrio(DMD_TITLE_DRAW_PRIO_WIN_FIX);

        // ウインドウ描画
        AoWinSysDrawState(AOD_WIN_TYPE_A
                         , AoTexGetTexList(main_work.cmn_tex[2])          // ◆
                         , 0
                         , DMD_TITLE_SIZE_WIDTH / 2.0f          // ウインドウ中心X
                         , DMD_TITLE_SIZE_HEIGHT / 2.0f         // ウインドウ中心Y
                         , DMD_TITLE_WINDOW_SIZE_W_PC * DMD_TITLE_DISP_SCALE_TEXT_PC * main_work.win_size_rate[0]            // ウインドウ横サイズ
                         , DMD_TITLE_WINDOW_SIZE_H * DMD_TITLE_DISP_SCALE_TEXT_PC * main_work.win_size_rate[1]            // ウインドウ縦サイズ
                         , DMD_TITLE_DRAW_STATE_ID              // 描画STATE
                         );


        // ウインドウ内の項目描画
        if ((main_work.disp_flag & DMD_TITLE_DISP_FLAG_WIN_ACT) == DMD_TITLE_DISP_FLAG_WIN_ACT)
        {
            // 共通ウインドウACT登録
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[2]));
            //		AoActSortRegAction(main_work.act[ACT_WIN_LINE]);

            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
            //		AoActSortRegAction(main_work.act[ACT_TEX_WINTITLE]);

            switch ((DME_TITLE_WIN_TYPE)main_work.win_mode)
            {
                case DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA1:
                    // アクション登録
                    if (main_work.iphone_menus)
                    {
                        for (int i = ACT_WIN_NO_BTN_L, max = ACT_WIN_YES_BTN_R + 1; i < max; ++i)
                        {
                            AoActSortRegAction(main_work.act[i]);
                        }
                    }

                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG1]);
                    //			AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG2]);

                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                    AoActSortRegAction(main_work.act[ACT_TEX_YES]);
                    AoActSortRegAction(main_work.act[ACT_TEX_NO]);

                    if (!main_work.iphone_menus)
                    {
                        // 初期表示フレーム設定
                        if (main_work.win_cur_slct == 0)
                        {
                            AoActSetFrame(main_work.act[ACT_TEX_YES], 0.0f);
                            AoActSetFrame(main_work.act[ACT_TEX_NO], 1.0f);
                        }

                        else
                        {
                            AoActSetFrame(main_work.act[ACT_TEX_YES], 1.0f);
                            AoActSetFrame(main_work.act[ACT_TEX_NO], 0.0f);
                        }
                    }

                    break;

                case DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA2:
                    // アクション登録
                    if (main_work.iphone_menus)
                    {
                        for (int i = ACT_WIN_NO_BTN_L, max = ACT_WIN_YES_BTN_R + 1; i < max; ++i)
                        {
                            AoActSortRegAction(main_work.act[i]);
                        }
                    }
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG2]);

                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                    AoActSortRegAction(main_work.act[ACT_TEX_YES]);
                    AoActSortRegAction(main_work.act[ACT_TEX_NO]);
                    if (!main_work.iphone_menus)
                    {
                        // 初期表示フレーム設定
                        if (main_work.win_cur_slct == 0)
                        {
                            AoActSetFrame(main_work.act[ACT_TEX_YES], 0.0f);
                            AoActSetFrame(main_work.act[ACT_TEX_NO], 1.0f);
                        }

                        else
                        {
                            AoActSetFrame(main_work.act[ACT_TEX_YES], 1.0f);
                            AoActSetFrame(main_work.act[ACT_TEX_NO], 0.0f);
                        }
                    }

                    break;
                case DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA3:
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG3]);

                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                    AoActSortRegAction(main_work.act[ACT_TEX_YES]);
                    AoActSortRegAction(main_work.act[ACT_TEX_NO]);

                    // 初期表示フレーム設定
                    if (main_work.win_cur_slct == 0)
                    {
                        AoActSetFrame(main_work.act[ACT_TEX_YES], 0.0f);
                        AoActSetFrame(main_work.act[ACT_TEX_NO], 1.0f);
                    }

                    else
                    {
                        AoActSetFrame(main_work.act[ACT_TEX_YES], 1.0f);
                        AoActSetFrame(main_work.act[ACT_TEX_NO], 0.0f);
                    }

                    break;

                case DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA4:
                case DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA5:
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG3]);

                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                    AoActSortRegAction(main_work.act[ACT_TEX_YES]);
                    AoActSortRegAction(main_work.act[ACT_TEX_NO]);

                    // 初期表示フレーム設定
                    if (main_work.win_cur_slct == 0)
                    {
                        AoActSetFrame(main_work.act[ACT_TEX_YES], 0.0f);
                        AoActSetFrame(main_work.act[ACT_TEX_NO], 1.0f);
                    }

                    else
                    {
                        AoActSetFrame(main_work.act[ACT_TEX_YES], 1.0f);
                        AoActSetFrame(main_work.act[ACT_TEX_NO], 0.0f);
                    }

                    break;
                default:
                    // 例外
                    MTM_ASSERT(0);
                    break;
            }

            if (!main_work.iphone_menus)
            {
                // ACCUMURATEでトランスさせる
                AoActAcmPush();

                AoActAcmInit();
                AoActAcmApplyTrans(dm_title_win_act_pos_tbl[0, 0]
                                   , dm_title_win_act_pos_tbl[0, 1]
                                   , 0
                                   );

                AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[2]));

                // フレーム更新はSetFrameのみで行う
                AoActUpdate(main_work.act[ACT_WIN_LINE], 0.0f);

                AoActAcmPop();
            }


            AoActAcmPush();

            for (int i = 0; i < 3; i++)
            {
                AoActAcmInit();
                AoActAcmApplyTrans(dm_title_win_act_pos_tbl[i + 1, 0]
                                   , dm_title_win_act_pos_tbl[i + 1, 1]
                                   , 0
                                   );

                if (main_work.iphone_menus)
                {
                    if (i <= ACT_WIN_TEX_MSG2 - ACT_WIN_TEX_MSG1)
                    {
                        AoActAcmApplyScale(DMD_TITLE_DISP_SCALE_TEXT_IOS, DMD_TITLE_DISP_SCALE_TEXT_IOS);
                    }
                }

                // フレーム更新はSetFrameのみで行う
                AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                AoActUpdate(main_work.act[ACT_WIN_TEX_MSG1 + i], 0.0f);
            }

#if !_IPHONE
            for (int i = 0; i < 3; i++)
            {
#else //!_IPHONE
		    for (int i = 1; i < 3; i++) {
			    const int ACT_TEX_WINTITLE = ACT_TEX_YES - 1; //ループが1から始まるのでYES/NOが表示される様に相殺する
#endif //!_IPHONE
                AoActAcmInit();
                AoActAcmApplyTrans(dm_title_win_act_pos_tbl[i + 4, 0]
                                   , dm_title_win_act_pos_tbl[i + 4, 1]
                                   , 0
                                   );

                // フレーム更新はSetFrameのみで行う
                AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                AoActUpdate(main_work.act[ACT_TEX_WINTITLE + i], 0.0f);

                if (main_work.iphone_menus)
                {
                    //        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[2]));
                    //        const int c_btn_base_table[3][2] = {
                    //            { 0, -1}
                    //, { ACT_WIN_YES_BTN_L, ACT_WIN_YES_BTN_R}
                    //, { ACT_WIN_NO_BTN_L, ACT_WIN_NO_BTN_R}
                    //        }
                    //        ;
                    //        AoActUpdate(main_work.act[c_btn_base_table[i][0] + 1], 0.0f); //当たり判定更新
                    //        const int c_btn_trg[3] = { -1, 1, 0 };
                    //        er::CTrgAoAction & answer = main_work.trg_answer[c_btn_trg[i]];
                    //        if (0 <= c_btn_trg[i])
                    //        {
                    //            answer.Update();
                    //        }
                    //        float set_frame = ((answer.GetState(0)[er::CTrgState::EState::On]) ? 1.0f : 0.0f);
                    //        for (int k = c_btn_base_table[i][0], max = c_btn_base_table[i][1] + 1; k < max; ++k)
                    //        {
                    //            AoActSetFrame(main_work.act[k], set_frame);
                    //            AoActUpdate(main_work.act[k], 0.0f);
                    //        }
                }
            }

            AoActAcmPop();

            // ソート実行
            AoActSortExecute();

            // ソートバッファ描画
            AoActSortDraw();

            // ソートバッファ全解除
            AoActSortUnregAll();
        }
    }

    private static void dmTitleTaskDraw(AMS_TCB tcb)
    {
        AoActDrawPre();
        amDrawExecCommand(10U);
        amDrawEndScene();
    }

    private static bool dmTitleIsDataLoad(DMS_TITLE_MAIN_WORK main_work)
    {
        for (int index = 0; index < 2; ++index)
        {
            if (!amFsIsComplete(main_work.arc_amb_fs[index]))
                return false;
        }

        for (int index = 0; index < 4; ++index)
        {
            if (!amFsIsComplete(main_work.arc_cmn_amb_fs[index]))
                return false;
        }

        return DmTitleOp.DmTitleOpLoadCheck();
    }

    private static bool dmTitleIsTexLoad(DMS_TITLE_MAIN_WORK main_work)
    {
        for (int index = 0; index < 2; ++index)
        {
            if (!AoTexIsLoaded(main_work.tex[index]))
                return false;
        }

        for (int index = 0; index < 4; ++index)
        {
            if (!AoTexIsLoaded(main_work.cmn_tex[index]))
                return false;
        }

        return GsFontIsBuilded() && DmTitleOp.DmTitleOpBuildCheck();
    }

    private static bool dmTitleIsTexRelease(DMS_TITLE_MAIN_WORK main_work)
    {
        for (int index = 0; index < 2; ++index)
        {
            if (!AoTexIsReleased(main_work.tex[index]))
                return false;
        }

        for (int index = 0; index < 4; ++index)
        {
            if (!AoTexIsReleased(main_work.cmn_tex[index]))
                return false;
        }

        return DmTitleOp.DmTitleOpFlushCheck();
    }

    private static void dmTitleSetChngFocusCrsrData(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 64) != 0)
        {
            main_work.prev_slct_menu = main_work.cur_slct_menu;
            main_work.cur_slct_menu =
                dmTitleGetRevisedMenuFocus(main_work.cur_slct_menu, -1, main_work.slct_menu_num);
            main_work.src_crsr_pos_y = main_work.cur_crsr_pos_y;
            main_work.dst_crsr_pos_y = !GsTrialIsTrial()
                ? dm_title_crsr_pos_y_tbl[0] + main_work.cur_slct_menu * dm_title_crsr_pos_y_tbl[1]
                : dm_title_crsr_trial_pos_y_tbl[main_work.cur_slct_menu];
            main_work.flag |= 256U;
        }

        if (((int)main_work.flag & 128) == 0)
            return;
        main_work.prev_slct_menu = main_work.cur_slct_menu;
        main_work.cur_slct_menu = dmTitleGetRevisedMenuFocus(main_work.cur_slct_menu, 1, main_work.slct_menu_num);
        main_work.src_crsr_pos_y = main_work.cur_crsr_pos_y;
        main_work.dst_crsr_pos_y = !GsTrialIsTrial()
            ? dm_title_crsr_pos_y_tbl[0] + main_work.cur_slct_menu * dm_title_crsr_pos_y_tbl[1]
            : dm_title_crsr_trial_pos_y_tbl[main_work.cur_slct_menu];
        main_work.flag |= 256U;
    }

    private static int dmTitleGetRevisedMenuFocus(int idx, int diff, int menu_num)
    {
        int num = idx + diff;
        if (num < 0)
            num = menu_num - 1;
        if (num >= menu_num)
            num = 0;
        return num;
    }

    private static void dmTitleSetCtrlFocusChangeEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        float[] numArray1 = new float[2];
        float[] numArray2 = new float[2]
        {
            0.0f,
            main_work.dst_crsr_pos_y - main_work.src_crsr_pos_y
        };
        numArray1[1] = numArray2[1] / 8f;
        main_work.cur_crsr_pos_y += numArray1[1];
    }

    private static void dmTitleSetWinOpenEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.win_timer > 8.0)
        {
            main_work.flag |= 65536U;
            main_work.win_timer = 0.0f;
            for (uint index = 0; index < 2U; ++index)
                main_work.win_size_rate[(int)index] = 1f;
        }
        else
            ++main_work.win_timer;

        for (uint index = 0; index < 2U; ++index)
        {
            main_work.win_size_rate[(int)index] = main_work.win_timer == 0.0 ? 1f : main_work.win_timer / 8f;
            if (main_work.win_size_rate[(int)index] > 1.0)
                main_work.win_size_rate[(int)index] = 1f;
        }
    }

    private static void dmTitleSetWinCloseEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        for (uint index = 0; index < 2U; ++index)
            main_work.win_size_rate[(int)index] = main_work.win_timer == 0.0 ? 0.0f : main_work.win_timer / 8f;
        if (main_work.win_timer < 0.0)
        {
            main_work.flag |= 65536U;
            main_work.win_timer = 0.0f;
            for (uint index = 0; index < 2U; ++index)
                main_work.win_size_rate[(int)index] = 0.0f;
        }
        else
            --main_work.win_timer;
    }

    private static void dmTitleSetMMenuWinOpenEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        if (0.0 == main_work.mmenu_win_timer)
            DmSoundPlaySE("Window");
        if (main_work.mmenu_win_timer > 8.0)
        {
            main_work.flag |= 4194304U;
            main_work.mmenu_win_timer = 0.0f;
            for (uint index = 0; index < 2U; ++index)
                main_work.mmenu_win_size_rate[(int)index] = 1f;
        }
        else
            ++main_work.mmenu_win_timer;

        for (uint index = 0; index < 2U; ++index)
        {
            main_work.mmenu_win_size_rate[(int)index] =
                main_work.mmenu_win_timer == 0.0 ? 1f : main_work.mmenu_win_timer / 8f;
            if (main_work.mmenu_win_size_rate[(int)index] > 1.0)
                main_work.mmenu_win_size_rate[(int)index] = 1f;
        }
    }

    private static void dmTitleSetMMenuWinCloseEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        for (uint index = 0; index < 2U; ++index)
            main_work.mmenu_win_size_rate[(int)index] =
                main_work.mmenu_win_timer == 0.0 ? 0.0f : main_work.mmenu_win_timer / 8f;
        if (main_work.mmenu_win_timer < 0.0)
        {
            main_work.flag |= 4194304U;
            main_work.mmenu_win_timer = 0.0f;
            for (uint index = 0; index < 2U; ++index)
                main_work.mmenu_win_size_rate[(int)index] = 0.0f;
        }
        else
            --main_work.mmenu_win_timer;
    }

    private static bool dmTitleIsCtrlFocusChangeEfctEnd(DMS_TITLE_MAIN_WORK main_work)
    {
        float num = main_work.dst_crsr_pos_y - main_work.src_crsr_pos_y;
        if (main_work.cur_crsr_pos_y >= (double)main_work.dst_crsr_pos_y && num >= 0.0)
        {
            main_work.cur_crsr_pos_y = main_work.dst_crsr_pos_y;
            main_work.timer = 0.0f;
            return true;
        }

        if (main_work.cur_crsr_pos_y <= (double)main_work.dst_crsr_pos_y && num <= 0.0)
        {
            main_work.cur_crsr_pos_y = main_work.dst_crsr_pos_y;
            main_work.timer = 0.0f;
            return true;
        }

        ++main_work.timer;
        return false;
    }

    private static void dmTitleSetLoadSysData(DMS_TITLE_MAIN_WORK main_work)
    {
        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        uint num = 0;

        SOption instance1 = SOption.CreateInstance();
        int volumeBgm = (int)instance1.GetVolumeBgm();
        int volumeSe = (int)instance1.GetVolumeSe();
        mainSysInfo.bgm_volume = volumeBgm / 10;
        mainSysInfo.se_volume = volumeSe / 10;
        DmSoundSetVolumeBGM(mainSysInfo.bgm_volume);
        DmSoundSetVolumeSE(mainSysInfo.se_volume);

        mainSysInfo.game_flag |= GSD_MAINSYS_GAME_FLAG_PAD_VIB_ENABLE;
        SSpecial instance2 = SSpecial.CreateInstance();
        for (int index = 0; index < 7; ++index)
        {
            if (instance2[(ushort)index].IsGetEmerald())
                num |= (uint)(1 << index);
        }

        if (num == (uint)sbyte.MaxValue)
            mainSysInfo.game_flag |= GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
        else
            mainSysInfo.game_flag &= ~GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;

        SSystem instance3 = SSystem.CreateInstance();
        mainSysInfo.rest_player_num = instance3.PlayerStock;
        mainSysInfo.ene_kill_count = instance3.Killed;
        mainSysInfo.final_clear_count = instance3.ClearCount;
        switch (SOption.CreateInstance().GetControl())
        {
            case SOption.EControl.Type.VirtualPadDown:
                mainSysInfo.game_flag &= ~GSD_MAINSYS_GAME_FLAG_INPUT_IS_FLICK;
                mainSysInfo.game_flag |= GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC;
                break;
            case SOption.EControl.Type.VirtualPadUp:
                mainSysInfo.game_flag |= GSD_MAINSYS_GAME_FLAG_INPUT_IS_FLICK;
                mainSysInfo.game_flag |= GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC;
                break;
            default:
                mainSysInfo.game_flag &= ~GSD_MAINSYS_GAME_FLAG_INPUT_IS_FLICK;
                mainSysInfo.game_flag &= ~GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC;
                break;
        }

        mainSysInfo.is_spe_clear = false;
        mainSysInfo.is_first_play = false;
    }

    private static void dmTitleSetInitSysData(DMS_TITLE_MAIN_WORK main_work)
    {
        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        SOption instance = SOption.CreateInstance();
        int volumeBgm = (int)instance.GetVolumeBgm();
        int volumeSe = (int)instance.GetVolumeSe();
        mainSysInfo.bgm_volume = volumeBgm / 10f;
        mainSysInfo.se_volume = volumeSe / 10f;
        DmSoundSetVolumeBGM(mainSysInfo.bgm_volume);
        DmSoundSetVolumeSE(mainSysInfo.se_volume);
        instance.IsVibration();
        mainSysInfo.game_flag |= GSD_MAINSYS_GAME_FLAG_PAD_VIB_ENABLE;
        mainSysInfo.game_flag &= GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
        mainSysInfo.rest_player_num = GSD_MAINSYS_PLAYER_REST_DEF;
        mainSysInfo.ene_kill_count = 0;
        mainSysInfo.final_clear_count = 0;
        mainSysInfo.is_spe_clear = false;
        mainSysInfo.is_first_play = false;
    }

    private static void dmTitleSetMenuInfo(DMS_TITLE_MAIN_WORK main_work)
    {
        if (GsTrialIsTrial())
        {
            main_work.slct_menu_num = DMD_TITLE_SLCT_MENU_NUM_TRIAL;
            main_work.is_init_play = true;
        }
        else if (!GsMainSysIsStageClear(GSD_MAIN_STAGE_ID_1_1))
        {
            main_work.slct_menu_num = DMD_TITLE_SLCT_MENU_NUM_FIRST;
            main_work.is_init_play = true;
        }
        else
        {
            main_work.slct_menu_num = DMD_TITLE_SLCT_MENU_NUM_PLAYED;
            main_work.is_init_play = false;
        }
    }

    private static void dmTitleSetFirstPlayData(DMS_TITLE_MAIN_WORK main_work)
    {
        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        SSystem instance = SSystem.CreateInstance();
        mainSysInfo.rest_player_num = GSD_MAINSYS_PLAYER_REST_DEF;
        instance.PlayerStock = mainSysInfo.rest_player_num;
        dmTitleSetLoadSysData(main_work);
    }

    private static void dmTitleSetInitSaveData(DMS_TITLE_MAIN_WORK main_work)
    {
        GsGetMainSysInfo().Save.Init();
    }

    private static bool dmTitleIsSaveRunning()
    {
        return GsGetMainSysInfo().is_save_run != 0U;
    }

    private static bool dmTitleIsChangeOptVol()
    {
        SOption instance = SOption.CreateInstance();
        return instance.GetVolumeBgm() != 100U || instance.GetVolumeSe() != 100U;
    }
}