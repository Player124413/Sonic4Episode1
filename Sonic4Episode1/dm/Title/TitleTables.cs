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

    // dmTitle.cpp:711-733. this is the trailing #else, which iPhone lands in
    private static readonly float[] dm_title_crsr_trial_pos_y_tbl_iphone = new float[]
    {
        445f,
        495f,
        545f
    };

    // the _PC || _XBOX arm is {422, 472, 522, 572, 622, 372} for START, OPTION, RANK, ZISSEKI,
    // LIBRARY, BUY (TUDUKI's 422 is commented out in SEGA's source). RANK and ZISSEKI are dropped,
    // so 522 and 572 go with them and LIBRARY moves up into 522
    private static readonly float[] dm_title_crsr_trial_pos_y_tbl_pc = new float[]
    {
        422f,
        472f,
        522f,
        372f
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

        act_num = (int)DME_TITLE_ACT.ACT_NUM,
        act_index = dmTitleBuildActIndex(typeof(DME_TITLE_ACT)),

        window_size_w = DMD_TITLE_WINDOW_SIZE_W_IOS,
        disp_scale_text = DMD_TITLE_DISP_SCALE_TEXT_IOS,
        crsr_dflt_pos_x = DMD_TITLE_CRSR_DFLT_POS_X_IOS,
        mmenu_win_size_w = DMD_TITLE_MMENU_WIN_SIZE_W_IOS,
        mmenu_win_size_h = DMD_TITLE_MMENU_WIN_SIZE_H_IOS,
        mmenu_win_size_w2 = DMD_TITLE_MMENU_WIN_SIZE_W_2,
        mmenu_win_size_h2 = DMD_TITLE_MMENU_WIN_SIZE_H_2,

        crsr_trial_pos_y_tbl = dm_title_crsr_trial_pos_y_tbl_iphone,
        slct_menu_num_first = DMD_TITLE_SLCT_MENU_NUM_FIRST_IOS,
        slct_menu_num_played = DMD_TITLE_SLCT_MENU_NUM_PLAYED_IOS,
        slct_menu_num_trial = DMD_TITLE_SLCT_MENU_NUM_TRIAL_IOS,

        menu_actions =
        [
            DME_TITLE_MENU_ACTION.NEW_GAME,
            DME_TITLE_MENU_ACTION.CONTINUE,
            DME_TITLE_MENU_ACTION.OPTION,
            DME_TITLE_MENU_ACTION.ACHIEVEMENTS,
            DME_TITLE_MENU_ACTION.LEADERBOARDS,
        ],

        is_console = false,
        fs_variant = AMD_FS_VARIANT_BASE,
    };

    private static readonly DMS_TITLE_VARIANT dm_title_variant_pc = new DMS_TITLE_VARIANT
    {
        act_id_tbl = dm_title_g_dm_act_id_tbl_pc,
        act_slot_tbl = dm_title_act_slot_tbl_pc,
        win_act_pos_tbl = dm_title_win_act_pos_tbl_pc,
        proc_win_input = dmTitleInputProcWindowPc,

        act_num = (int)DME_TITLE_ACT_REG.ACT_NUM,
        act_index = dmTitleBuildActIndex(typeof(DME_TITLE_ACT_REG)),

        window_size_w = DMD_TITLE_WINDOW_SIZE_W_PC,
        disp_scale_text = DMD_TITLE_DISP_SCALE_TEXT_PC,
        crsr_dflt_pos_x = DMD_TITLE_CRSR_DFLT_POS_X_PC,
        mmenu_win_size_w = DMD_TITLE_MMENU_WIN_SIZE_W_PC,
        mmenu_win_size_h = DMD_TITLE_MMENU_WIN_SIZE_H_PC,
        mmenu_win_size_w2 = null,
        mmenu_win_size_h2 = null,

        crsr_trial_pos_y_tbl = dm_title_crsr_trial_pos_y_tbl_pc,
        slct_menu_num_first = DMD_TITLE_SLCT_MENU_NUM_FIRST_PC,
        slct_menu_num_played = DMD_TITLE_SLCT_MENU_NUM_PLAYED_PC,
        slct_menu_num_trial = DMD_TITLE_SLCT_MENU_NUM_TRIAL_PC,

        menu_actions =
        [
            DME_TITLE_MENU_ACTION.NEW_GAME,
            DME_TITLE_MENU_ACTION.CONTINUE,
            DME_TITLE_MENU_ACTION.OPTION,
            DME_TITLE_MENU_ACTION.LIBRARY,
            DME_TITLE_MENU_ACTION.BUY,
        ],

        is_console = true,
        fs_variant = AMD_FS_VARIANT_CONSOLE,
    };
}
