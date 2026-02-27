using System;
using System.Runtime.CompilerServices;

using s16 = short;
using u16 = ushort;
using s32 = int;
using u32 = uint;

partial class AppMain
{

    public const int DMD_STGSLCT_TASK_PAUSELEVEL = 0;
    public const int DMD_STGSLCT_TASK_PRIO_MAIN = 0x2000;
    public const int DMD_STGSLCT_TASK_GROUP_MAIN = 0;

    public const int DMD_STGSLCT_FILE_PATH_NUM_MAX = 60;

    public const string DMD_STGSLCT_CMN_DATA_FILENAME = "DEMO/CMN/D_CMN_WIN.AMB";
    public const string DMD_STGSLCT_DATA_FILENAME = "DEMO/STGSLCT/D_STGSLCT.AMB";

    public const float DMD_STGSLCT_SIZE_WIDTH = 960.0f;
    public const float DMD_STGSLCT_SIZE_HEIGHT = 720.0f;
    public const float DMD_STGSLCT_SIZE_HALF_WIDTH = 480.0f;
    public const float DMD_STGSLCT_SIZE_HALF_HEIGHT = 360.0f;

    public const int DMD_STGSLCT_DISP_SCORE_DIGIT_NUM = 9;

    // Priority settings
    public const int DMD_STGSLCT_DRAW_PRIO_ZONE = 0x2000;
    public const int DMD_STGSLCT_DRAW_PRIO_STAGE = 0x2000;
    public const int DMD_STGSLCT_DRAW_PRIO_BG = 0x1000;
    public const int DMD_STGSLCT_DRAW_PRIO_FIX = 0x3000;

#if !_IPHONE
    public const int DMD_STGSLCT_DRAW_PRIO_WIN = 0x3000;
#else
    public const int DMD_STGSLCT_DRAW_PRIO_WIN = 0x3500;
#endif
    public const int DMD_STGSLCT_DRAW_PRIO_WIN_FIX = 0x4000;

    // Display-related
#if _WII
    public const float DMD_STGSLCT_ACT_TABLE_TOP_POS_X = 151.0f;
#elif _IPHONE
    public const float DMD_STGSLCT_ACT_TABLE_TOP_POS_X = 100.0f;
#else
    public const float DMD_STGSLCT_ACT_TABLE_TOP_POS_X = 170.0f;
#endif

#if !_IPHONE
    public const float DMD_STGSLCT_ACT_TABLE_DIST_Y = 96.0f;
#else
    public const float DMD_STGSLCT_ACT_TABLE_DIST_Y = 96.0f + 22.0f;
#endif

    public const int DMD_STGSLCT_INIT_SCORE_NUM = 1000000000;
    public const int DMD_STGSLCT_INIT_RECORD_TIME_NUM = 36000;

    public const int DMD_STGSLCT_CONV_SCORE_RATE = 10;

    public const int DMD_STGSLCT_DOWN_ACT_DISP_POS = 0;
    public const int DMD_STGSLCT_DOWN_ACT_NODISP_POS = 180;
    public const int DMD_STGSLCT_MODE_TEX_FRAME_MAX = 60;

    public const float DMD_STGSLCT_STAGE_TAB_NODISP_POS_X = 1120f;

    public const float DMD_STGSLCT_NO_ACTIVE_NUM_ID = 10f;
    public const float DMD_STGSLCT_TIME_COLON_ID = 11f;

    // Window-related
#if !_IPHONE
    public const float DMD_STGSLCT_WINDOW_SIZE_W = 380f;
    public const float DMD_STGSLCT_WINDOW_SIZE_H = 180f;
#else
    public const float DMD_STGSLCT_WINDOW_SIZE_W = 420f;
    public const float DMD_STGSLCT_WINDOW_SIZE_H = 180f;
#endif
    public const float DMD_STGSLCT_WIN_DEF_RATE = 1.0f;

    // Fade settings
    public const float DMD_STGSLCT_FADEIN_TIME = 32.0f;
    public const float DMD_STGSLCT_FADEOUT_TIME = 32.0f;

    public const int DMD_STGSLCT_BGM_FADEIN_TIME = 32;
    public const int DMD_STGSLCT_BGM_FADEOUT_TIME = 32;

    public const int DMD_STGSLCT_BG_FADE_SPEED = 20;

    // Effects
    public const float DMD_STGSLCT_ZONE_EFCT_TIME = 16.0f;
    public const float DMD_STGSLCT_ACT_EFCT_TIME = 16.0f;
    public const float DMD_STGSLCT_WIN_EFCT_TIME = 8.0f;

    public const float DMD_STGSLCT_OBI_MOVE_START_POS = 1120f;
    public const float DMD_STGSLCT_OBI_MOVE_END_POS = -1120f;
    public const float DMD_STGSLCT_OBI_MOVE_SPEED = -3f;

    public const float DMD_STGSLCT_ACT_VRTCL_CHNG_DIST = 128f;
    public const int DMD_STGSLCT_ACT_VRTCL_CHNG_NUM = 3;
    public const float DMD_STGSLCT_ACT_VRTCL_MOVE_TIME = 12.0f;
    public const float DMD_STGSLCT_CRSR_MOVE_TIME = 8.0f;

    public const int DMD_STGSLCT_ZONE_FINAL_OPEN = 1 << 0;
    public const int DMD_STGSLCT_ZONE_SPECIAL_OPEN = 1 << 1;
    public const int DMD_STGSLCT_ZONE_ALL_OPEN = (1 << 0) | (1 << 1);

    public const int DMD_STGSLCT_ZONE_DECIDE_EFCT_ON = 6;
    public const int DMD_STGSLCT_ZONE_DECIDE_EFCT_TIME = 10;
    public const int DMD_STGSLCT_ZONE_DECIDE_EFCT_ON_POS_X = 8;
    public const int DMD_STGSLCT_ZONE_DECIDE_EFCT_ON_POS_Y = 8;

    public const int DMD_STGSLCT_ZONE_SCR_ID_NUM = 3;
    public const int DMD_STGSLCT_ZONE_SCR_CHANGE_ALL_FRM = 360;

    public const int DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM = 12;

    // Flag-related
    public const int DMD_STGSLCT_FLAG_EXIT = 1 << 0;
    public const int DMD_STGSLCT_FLAG_CANCEL = 1 << 1;
    public const int DMD_STGSLCT_FLAG_DECIDE = 1 << 2;
    public const int DMD_STGSLCT_FLAG_DISP_MENU = 1 << 3;
    public const int DMD_STGSLCT_FLAG_WIN_EFCT_END = 1 << 4;
    public const int DMD_STGSLCT_FLAG_ACT_CHNG_ZONE = 1 << 5;
    public const int DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL = 1 << 6;
    public const int DMD_STGSLCT_FLAG_ACT_CHNG_CRSR = 1 << 7;
    public const int DMD_STGSLCT_FLAG_ACT_RE_CHNG_ZONE = 1 << 8;
    public const int DMD_STGSLCT_FLAG_ACT_RE_CHNG_VRTCL = 1 << 9;
    public const int DMD_STGSLCT_FLAG_ACT_RE_CHNG_CRSR = 1 << 10;
    public const int DMD_STGSLCT_FLAG_UP_CHNG_CRSR = 1 << 11;
    public const int DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR = 1 << 12;
    public const int DMD_STGSLCT_FLAG_EME_TBL_MOVE_NODISP = 1 << 13;
    public const int DMD_STGSLCT_FLAG_EME_TBL_MOVE_DISP = 1 << 14;
    public const int DMD_STGSLCT_FLAG_MODE_TEX_MOVE_NODISP = 1 << 15;
    public const int DMD_STGSLCT_FLAG_MODE_TEX_MOVE_DISP = 1 << 16;
    public const int DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT = 1 << 17;
    public const int DMD_STGSLCT_FLAG_DECIDE_ZONE_TABLE_EFCT = 1 << 18;
    public const int DMD_STGSLCT_FLAG_CHANGE_EVT_RANKING = 1 << 19;
    public const int DMD_STGSLCT_FLAG_CHANGE_DISP_BG = 1 << 20;
    public const int DMD_STGSLCT_FLAG_BG_FADE_EFCT = 1 << 21;
    public const int DMD_STGSLCT_FLAG_ACT_PUSH_L_BTN = 1 << 22;
    public const int DMD_STGSLCT_FLAG_ACT_PUSH_R_BTN = 1 << 23;
    public const int DMD_STGSLCT_FLAG_PREV_EVT_RANKING = 1 << 24;
    public const int DMD_STGSLCT_FLAG_DEMO_SND_END = 1 << 25;
    public const int DMD_STGSLCT_FLAG_CHECK_SIGN_OUT_OK = 1 << 30;
    public const int DMD_STGSLCT_FLAG_SIGN_OUT_EXIT = 1 << 31;

    // Action display flags
    public const int DMD_STGSLCT_DISP_FLAG_WIN_ACT = 1 << 0;
    public const int DMD_STGSLCT_DISP_FLAG_LR_ARROW = 1 << 1;
    public const int DMD_STGSLCT_DISP_FLAG_ACT_CRSR = 1 << 2;

#if _WII
    public const float DMD_STGSLCT_DISP_SCALE_TEXT = 1.5f;
#elif _IPHONE
    public const float DMD_STGSLCT_DISP_SCALE_TEXT = 1.5f * 1.125f;
#endif

#if _PS3 || _XBOX || _PC
    public const float DMD_STGSLCT_OBI_MSG_SCALE_SIZE = 1.2f;
#endif


    // ----- Macro Functions -----------------------------------（処理マクロ定義）


    public static class DME_STGSLCT_NEXT_EVT
    {
        public static int MAINGAME = 0,
            SPESTE = 1,
            RANKING = 2,
            MAINMENU = 3,
            TITLE = 4,
            MAX = 5;
    }

    public static class DME_STGSLCT_DATA_TYPE
    {
        public static int CMN_DATA = 0,
            LANG_DATA = 1,
            MAX = 2,
            NONE = 3;
    }

    public static class DME_STGSLCT_PLATFORM_TYPE
    {
        public static int PC = 0,
            XBOX = 1,
            PS3 = 2,
            WII = 3,
            IPHONE = 4,
            MAX = 5,
            NONE = 6;
    }

    public static class DME_STGSLCT_ZONE_TYPE
    {
        public static int TYPE_1 = 0,
            TYPE_2 = 1,
            TYPE_3 = 2,
            TYPE_4 = 3,
            FINAL = 4,
            SPE = 5,
            NUM = 6,
            NONE = 7;
    }

    public static class DME_STGSLCT_MODE_STATE
    {
        public static int ZONE_SLCT = 0,
            ACT_SLCT = 1,
            NUM = 2,
            NONE = 3;
    }

    public static class DME_STGSLCT_PLAY_MODE
    {
        public static int NORMAL = 0,
            TIME_ATK = 1,
            NUM = 2,
            NONE = 3;
    }

    public static class DME_STGSLCT_WIN
    {
        public static int MENU = 0,
            STG_SLCT = 1,
            _1_1_CLEAR = 2,
            BOSS1_CAN_PLAY = 3,
            BOSS2_CAN_PLAY = 4,
            BOSS3_CAN_PLAY = 5,
            BOSS4_CAN_PLAY = 6,
            FINAL_CAN_SLCT = 7,
            SSONIC_SLCT = 8,
            SPESTE_CAN_SLCT = 9,
            NUM = 10,
            NONE = 11;
    }

    public static class DME_STGSLCT_ACT_PAGE
    {
        public static int PAGE_1 = 0,
            PAGE_2 = 1,
            PAGE_3 = 2,
            PAGE_4 = 3,
            FINAL = 4,
            SPE = 5,
            NUM = 6,
            NONE = 7;
    }

    public static class DME_STGSLCT_STAGE
    {
        public static int _1_1 = 0,
            _1_2 = 1,
            _1_3 = 2,
            _1_B = 3,
            _2_1 = 4,
            _2_2 = 5,
            _2_3 = 6,
            _2_B = 7,
            _3_1 = 8,
            _3_2 = 9,
            _3_3 = 10,
            _3_B = 11,
            _4_1 = 12,
            _4_2 = 13,
            _4_3 = 14,
            _4_B = 15,
            F_1 = 16,
            S_1 = 17,
            S_2 = 18,
            S_3 = 19,
            S_4 = 20,
            S_5 = 21,
            S_6 = 22,
            S_7 = 23,
            NUM = 24,
            NONE = 25;
    }






    // 各国別AMBファイルパステーブル
    private static readonly string[] dm_stgslct_main_lng_amb_name_tbl = new string[] {
    "DEMO/STGSLCT/D_STGSLCT_JP.AMB",
    "DEMO/STGSLCT/D_STGSLCT_US.AMB",
    "DEMO/STGSLCT/D_STGSLCT_FR.AMB",
    "DEMO/STGSLCT/D_STGSLCT_IT.AMB",
    "DEMO/STGSLCT/D_STGSLCT_GE.AMB",
    "DEMO/STGSLCT/D_STGSLCT_SP.AMB",
};


    // メニュー共通AMBファイルパステーブル
    private static readonly string[] dm_stgslct_menu_cmn_amb_name_tbl = new string[] {
    "DEMO/CMN/D_CMN_BG.AMB",
    "DEMO/CMN/D_CMN_BTN.AMB",
    "DEMO/CMN/D_CMN_OBI.AMB",
    "DEMO/CMN/D_CMN_WIN.AMB",
};

    // 各国別メニュー共通AMBファイルパステーブル
    private static readonly string[] dm_stgslct_menu_cmn_lng_amb_name_tbl = new string[] {
    "DEMO/CMN/D_CMN_MSG_JP.AMB",
    "DEMO/CMN/D_CMN_MSG_US.AMB",
    "DEMO/CMN/D_CMN_MSG_FR.AMB",
    "DEMO/CMN/D_CMN_MSG_IT.AMB",
    "DEMO/CMN/D_CMN_MSG_GE.AMB",
    "DEMO/CMN/D_CMN_MSG_SP.AMB",
};


    // ZONEごとのACT数テーブル
    private static readonly u32[,] dm_stgslct_zone_act_num_tbl = new u32[,] {
        {0,  4},
        {4,  4},
        {8,  4},
        {12, 4},
        {16, 1},
        {17, 7},
    };


    // ZONEごとのACT縦切り替えテーブル
    private static readonly u32[] dm_stgslct_act_chng_vrtcl_num_tbl = new u32[] {
    0,
    0,
    0,
    0,
    0,
    4,
};


    // STAGEごとのZONE番号テーブル
    private static readonly u32[] dm_stgslct_act_zone_no_tbl[DME_STGSLCT_STAGE_NUM] = {
    0, 0, 0, 0,
    1, 1, 1, 1,
    2, 2, 2, 2,
    3, 3, 3, 3,
    4,
    5, 5, 5, 5, 5, 5, 5,
};


    // エメラルドGETステージテーブル
    private static readonly u32[] dm_stgslct_eme_get_act_no_tbl = new u32[13] {
     0,
     0,  1,  2,
     4,  5,  6,
     8,  9, 10,
    12, 13, 14,
};


    // GSにあるステージID配列への変換テーブル
    private static readonly u16[,] dm_stgslct_conv_stage_no_tbl = new u16[6, 7]{
    { 0,  1,  2,  3, 0xffff, 0xffff, 0xffff},
    { 4,  5,  6,  7, 0xffff, 0xffff, 0xffff},
    { 8,  9, 10, 11, 0xffff, 0xffff, 0xffff},
    {12, 13, 14, 15, 0xffff, 0xffff, 0xffff},
    {16, 16, 16, 16,     16, 0xffff, 0xffff},
    {21, 22, 23, 24,     25,     26,     27},
};


    // 各ゾーンのACT配列テーブル
    private static readonly u32[] dm_stgslct_zone_array_act_tbl = new u32[DME_STGSLCT_STAGE_NUM]{
    0, 1, 2, 3,						// ZONE1
	0, 1, 2, 3,						// ZONE2
	0, 1, 2, 3,						// ZONE3
	0, 1, 2, 3,						// ZONE4
	0,								// FINAL STAGE
	0, 1, 2, 3, 4, 5, 6,			// SPECIAL STAGE
};


    // ZONE選択での十字キー入力遷移先テーブル(4つVer)
    private static readonly s32[,] dm_stgslct_n_zone_input_dir_tbl = new s32[DME_STGSLCT_ZONE_TYPE_NUM, 4] {
	// 			↑							↓
	// 				←							→
	{DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_2
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_3
},		// ZONE1
	{
    DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_4},		// ZONE2
	{
    DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_4
        , DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_NONE},		// ZONE3
	{
    DME_STGSLCT_ZONE_TYPE_3, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_2, DME_STGSLCT_ZONE_TYPE_NONE},		// ZONE4
	{
    DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE},	// FINAL
	{
    DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE},	// SPE
};


    // ZONE選択での十字キー入力遷移先テーブル(5つVer)
    private static readonly s32[,] dm_stgslct_s_zone_input_dir_tbl = new s32[DME_STGSLCT_ZONE_TYPE_NUM, 4] {
    // 			↑							↓
    // 				←							→
    {
        DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_2
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_SPE
},	// ZONE1
	{
    DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_SPE},	// ZONE2
	{
    DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_4
        , DME_STGSLCT_ZONE_TYPE_SPE, DME_STGSLCT_ZONE_TYPE_NONE},	// ZONE3
	{
    DME_STGSLCT_ZONE_TYPE_3, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_SPE, DME_STGSLCT_ZONE_TYPE_NONE},	// ZONE4
	{
    DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE},	// FINAL
	{
    DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_2
        , DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_3},		// SPE
}
;


    // ZONE選択での十字キー入力遷移先テーブル(5つVer)
    private static readonly s32[,] dm_stgslct_f_zone_input_dir_tbl = new s32[DME_STGSLCT_ZONE_TYPE_NUM, 4] {
    // 			↑							↓
    // 				←							→
    {
        DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_2
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_FINAL
},	// ZONE1
	{
    DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_FINAL},	// ZONE2
	{
    DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_4
        , DME_STGSLCT_ZONE_TYPE_FINAL, DME_STGSLCT_ZONE_TYPE_NONE},	// ZONE3
	{
    DME_STGSLCT_ZONE_TYPE_3, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_FINAL, DME_STGSLCT_ZONE_TYPE_NONE},	// ZONE4
	{
    DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_2
        , DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_3},		// FINAL
	{
    DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE},	// SPE
}
;


    // ZONE選択での十字キー入力遷移先テーブル(6つVer)
    private static readonly s32[,] dm_stgslct_a_zone_input_dir_tbl = new s32[DME_STGSLCT_ZONE_TYPE_NUM, 4]{
    // 			↑							↓
    // 				←							→
    {
        DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_2
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_FINAL},	// ZONE1
	{
        DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_SPE},	// ZONE2
	{
        DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_4
        , DME_STGSLCT_ZONE_TYPE_FINAL, DME_STGSLCT_ZONE_TYPE_NONE},	// ZONE3
	{
        DME_STGSLCT_ZONE_TYPE_3, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_SPE, DME_STGSLCT_ZONE_TYPE_NONE},	// ZONE4
	{
        DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_SPE
        , DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_3},		// FINAL
	{
        DME_STGSLCT_ZONE_TYPE_FINAL, DME_STGSLCT_ZONE_TYPE_NONE
        , DME_STGSLCT_ZONE_TYPE_2, DME_STGSLCT_ZONE_TYPE_4},		// SPE
}
    ;



    // ZONE表示位置テーブル(FINAL無)
    private static readonly float[,] dm_stgslct_n_zone_disp_pos_tbl = new float[DME_STGSLCT_ACT_PAGE_NUM, 2] {
#if !_IPHONE
    { 160.0f, 150.0f},
    { 160.0f, 350.0f},
    { 560.0f, 150.0f},
    { 560.0f, 350.0f},
    { 360.0f,-280.0f},
    { 360.0f, 780.0f},
#else //!_IPHONE
	{160.0f-50.0f, 150.0f-30.0f},
	{160.0f-50.0f, 350.0f-20.0f},
	{560.0f-10.0f, 150.0f-30.0f},
	{560.0f-10.0f, 350.0f-20.0f},
	{360.0f-30.0f,-280.0f-30.0f},
	{360.0f-30.0f, 780.0f-20.0f},
#endif //!_IPHONE
}
    ;


    // ZONE非表示位置テーブル(FINAL無)
    private static readonly float[,] dm_stgslct_n_zone_nodisp_pos_tbl[DME_STGSLCT_ACT_PAGE_NUM][2] = {
#if !_IPHONE
    { -416.0f, 100.0f},
	{ -416.0f, 400.0f},
	{ 1120.0f, 100.0f},
	{ 1120.0f, 400.0f},
	{ 360.0f, -280.0f},
	{ 360.0f,  780.0f},
#else //!_IPHONE
	{-416.0f-50.0f, 100.0f -30.0f},
	{-416.0f-50.0f, 400.0f -20.0f},
	{1120.0f-10.0f, 100.0f -30.0f},
	{1120.0f-10.0f, 400.0f -20.0f},
	{360.0f -30.0f, -280.0f-30.0f},
	{360.0f -30.0f,  780.0f-20.0f},
#endif //!_IPHONE
}
;


// ZONE表示位置テーブル(FINAL解放)
private static readonly float[,] dm_stgslct_f_zone_disp_pos_tbl[DME_STGSLCT_ACT_PAGE_NUM][2] = {
#if !_IPHONE
    { 80.0f, 150.0f},
	{ 80.0f, 350.0f},
	{ 640.0f, 150.0f},
	{ 640.0f, 350.0f},
	{ 360.0f, 260.0f},
	{ 360.0f, 780.0f},
#else //!_IPHONE
	{80.0f -50.0f, 150.0f-30.0f},
	{80.0f -50.0f, 350.0f-20.0f},
	{640.0f-10.0f, 150.0f-30.0f},
	{640.0f-10.0f, 350.0f-20.0f},
	{360.0f-30.0f, 260.0f-30.0f},
	{360.0f-30.0f, 780.0f-20.0f},
#endif //!_IPHONE
}
;


// ZONE非表示位置テーブル(FINAL解放)
private static readonly float[,] dm_stgslct_f_zone_nodisp_pos_tbl[DME_STGSLCT_ACT_PAGE_NUM][2] = {
#if !_IPHONE
    { -416.0f, 100.0f},
	{ -416.0f, 400.0f},
	{ 1120.0f, 100.0f},
	{ 1120.0f, 400.0f},
	{ 360.0f, -280.0f},
	{ 360.0f,  780.0f},
#else //!_IPHONE
	{-416.0f-50.0f, 100.0f -30.0f},
	{-416.0f-50.0f, 400.0f -20.0f},
	{1120.0f-10.0f, 100.0f -30.0f},
	{1120.0f-10.0f, 400.0f -20.0f},
	{360.0f -30.0f, -280.0f-30.0f},
	{360.0f -30.0f,  780.0f-20.0f},
#endif //!_IPHONE
}
;


// ZONE表示位置テーブル(スペ解放)
private static readonly float[,] dm_stgslct_s_zone_disp_pos_tbl[DME_STGSLCT_ACT_PAGE_NUM][2] = {
#if !_IPHONE
    { 80.0f, 150.0f},
	{ 80.0f, 350.0f},
	{ 640.0f, 150.0f},
	{ 640.0f, 350.0f},
	{ 360.0f,-280.0f},
	{ 360.0f, 260.0f},
#else //!_IPHONE
	{80.0f -50.0f, 150.0f-30.0f},
	{80.0f -50.0f, 350.0f-20.0f},
	{640.0f-10.0f, 150.0f-30.0f},
	{640.0f-10.0f, 350.0f-20.0f},
	{360.0f-30.0f,-280.0f-30.0f},
	{360.0f-30.0f, 260.0f-20.0f},
#endif //!_IPHONE
}
;


// ZONE非表示位置テーブル(スペ解放)
private static readonly float[,] dm_stgslct_s_zone_nodisp_pos_tbl[DME_STGSLCT_ACT_PAGE_NUM][2] = {
#if !_IPHONE
    { -416.0f, 100.0f},
	{ -416.0f, 400.0f},
	{ 1120.0f, 100.0f},
	{ 1120.0f, 400.0f},
	{ 360.0f, -280.0f},
	{ 360.0f,  780.0f},
#else //!_IPHONE
	{-416.0f-50.0f, 100.0f -30.0f},
	{-416.0f-50.0f, 400.0f -20.0f},
	{1120.0f-10.0f, 100.0f -30.0f},
	{1120.0f-10.0f, 400.0f -20.0f},
	{360.0f -30.0f, -280.0f-30.0f},
	{360.0f -30.0f,  780.0f-20.0f},
#endif //!_IPHONE
}
;


// ZONE表示位置テーブル(6つVer)
private static readonly float[,] dm_stgslct_a_zone_disp_pos_tbl[DME_STGSLCT_ACT_PAGE_NUM][2] = {
#if !_IPHONE
    { 80.0f, 150.0f},
	{ 80.0f, 350.0f},
	{ 640.0f, 150.0f},
	{ 640.0f, 350.0f},
	{ 360.0f, 150.0f},
	{ 360.0f, 350.0f},
#else //!_IPHONE
	{80.0f -50.0f, 150.0f-30.0f},
	{80.0f -50.0f, 350.0f-20.0f},
	{640.0f-10.0f, 150.0f-30.0f},
	{640.0f-10.0f, 350.0f-20.0f},
	{360.0f-30.0f, 150.0f-30.0f},
	{360.0f-30.0f, 350.0f-20.0f},
#endif //!_IPHONE
}
;


// ZONE非表示位置テーブル(6つVersion)
private static readonly float[,] dm_stgslct_a_zone_nodisp_pos_tbl[DME_STGSLCT_ACT_PAGE_NUM][2] = {
#if !_IPHONE
    { -416.0f, 100.0f},
	{ -416.0f, 380.0f},
	{ 1120.0f, 120.0f},
	{ 1120.0f, 380.0f},
	{ 360.0f, -280.0f},
	{ 360.0f,  780.0f},
#else //!_IPHONE
	{-416.0f-50.0f, 100.0f -30.0f},
	{-416.0f-50.0f, 380.0f -20.0f},
	{1120.0f-10.0f, 120.0f -30.0f},
	{1120.0f-10.0f, 380.0f -20.0f},
	{360.0f -30.0f, -280.0f-30.0f},
	{360.0f -30.0f,  780.0f-20.0f},
#endif //!_IPHONE
}
;



// ZONEごとのACTトップ表示位置Yテーブル
private static readonly float[] dm_stgslct_act_disp_y_pos_tbl[DME_STGSLCT_STAGE_NUM] = {
#if !_IPHONE
	160.0f, 160.0f, 160.0f, 160.0f,
    160.0f, 160.0f, 160.0f, 160.0f,
    160.0f, 160.0f, 160.0f, 160.0f,
    160.0f, 160.0f, 160.0f, 160.0f,
    320.0f,
    160.0f, 160.0f, 160.0f, 160.0f, 160.0f, 160.0f, 160.0f
#else //!_IPHONE
	190.0f, 190.0f, 190.0f, 190.0f,
	190.0f, 190.0f, 190.0f, 190.0f,
	190.0f, 190.0f, 190.0f, 190.0f,
	190.0f, 190.0f, 190.0f, 190.0f,
	320.0f,
	190.0f, 190.0f, 190.0f, 190.0f, 190.0f, 190.0f, 190.0f
#endif //!_IPHONE
};


    // ZONEごとのACTテーブル表示位置Yテーブル
#if !_IPHONE
    const static float dm_stgslct_act_tab_disp_y_pos_tbl[4] = {
    160.0f,
    160.0f + 88.f * -1.f,
    160.0f + 88.f * -2.f,
    160.0f + 88.f * -3.f,
};
#else //!_IPHONE
const static float dm_stgslct_act_tab_disp_y_pos_tbl[5] = {
	dm_stgslct_act_disp_y_pos_tbl[DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) * 0,
	dm_stgslct_act_disp_y_pos_tbl[DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) * 1,
	dm_stgslct_act_disp_y_pos_tbl[DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) * 2,
	dm_stgslct_act_disp_y_pos_tbl[DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) * 3,
	dm_stgslct_act_disp_y_pos_tbl[DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) * 4,
};
#endif //!_IPHONE



    // ZONEごとのカーソル表示位置Yテーブル
    const static float dm_stgslct_act_crsr_disp_y_pos_tbl[4] = {
    160.0f,
    160.0f + 88.f * 1.f,
    160.0f + 88.f * 2.f,
    160.0f + 88.f * 3.f,
};


    // 表示ウインドウアクションIDテーブル
    private static readonly float[,] dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_NUM][3] = {
    // タイトル			メッセージ			OK
    { 2.f, 0.f, 1.f
},		// メニュー
	{ 3.f, 0.f, 0.f},		// アクト決定
	{ 1.f, 1.f, 0.f},		// ACT選択可能メッセージ
	{ 1.f, 2.f, 0.f},		// BOSSACT選択可能メッセージ
	{ 1.f, 3.f, 0.f},		// BOSSACT選択可能メッセージ
	{ 1.f, 4.f, 0.f},		// BOSSACT選択可能メッセージ
	{ 1.f, 5.f, 0.f},		// BOSSACT選択可能メッセージ
	{ 1.f, 6.f, 0.f},		// FINALZONE選択可能メッセージ
	{ 1.f, 7.f, 0.f},		// スーパーソニック変身可能メッセージ
	{ 1.f, 7.f, 0.f},		// SPECIALZONE選択可能メッセージ
//	フレーム、フレーム、パターン番号
}
;


// ウインドウ選択肢用表示フレームテーブル(現状２つ用)
private static readonly float[,] dm_stgslct_win_disp_slct_frm_tbl[2][2] = {
    { 0.f, 1.f},		// 左(上)がアクティブ
	{ 1.f, 0.f},		// 右(下)がアクティブ
}
;



// ACT縦並びの表示位置テーブル
const static float dm_stgslct_vrtcl_disp_pos_y_tbl[7 - DMD_STGSLCT_ACT_VRTCL_CHNG_NUM] = {
    DMD_STGSLCT_ACT_VRTCL_CHNG_DIST * 0.f,
    DMD_STGSLCT_ACT_VRTCL_CHNG_DIST * 1.f,
    DMD_STGSLCT_ACT_VRTCL_CHNG_DIST * 2.f,
    DMD_STGSLCT_ACT_VRTCL_CHNG_DIST * 3.f,
};


const static float dm_stgslct_back_text_length_tbl[6] = {
    -39.f,
    -53.f,
    -69.f,
    -77.f,
    -70.f,
    -55.f,
};

#if !(_WII || _IPHONE)
const static float dm_stgslct_win_act_pos_tbl[14][2] = {
    { DMD_STGSLCT_SIZE_HALF_WIDTH + 42.f, 280.0f},	// ウインドウ内のライン
	{ DMD_STGSLCT_SIZE_HALF_WIDTH + 182.f, 264.0f},	// キャンセルボタン
	{ DMD_STGSLCT_SIZE_HALF_WIDTH - 136.f, 400.0f},	// Xボタン
	{ DMD_STGSLCT_SIZE_HALF_WIDTH - 90.f, 400.0f},	// Yボタン
	{ DMD_STGSLCT_SIZE_HALF_WIDTH, 360.0f},			// メッセージ
	{ DMD_STGSLCT_SIZE_HALF_WIDTH, 344.0f},			// メッセージ2
	{ DMD_STGSLCT_SIZE_HALF_WIDTH, 360.0f},			// スーパーソニックメッセージ
	{ DMD_STGSLCT_SIZE_HALF_WIDTH, 344.0f},			// オプション
	{ DMD_STGSLCT_SIZE_HALF_WIDTH, 404.0f},			// メインメニュー
	{ DMD_STGSLCT_SIZE_HALF_WIDTH - 80.f, 274.0f},	// タイトルテキスト
	{ DMD_STGSLCT_SIZE_HALF_WIDTH + 202.f, 264.0f},	// 戻る
	{ DMD_STGSLCT_SIZE_HALF_WIDTH, 420.0f},			// OK
	{ DMD_STGSLCT_SIZE_HALF_WIDTH - 88.f, 420.0f},	// YES
	{ DMD_STGSLCT_SIZE_HALF_WIDTH + 88.f, 420.0f},	// NO
}
;
#elif _WII
const static float dm_stgslct_win_act_pos_tbl[14][2] = {
	{DMD_STGSLCT_SIZE_HALF_WIDTH + 42.f, 280.0f},	// ウインドウ内のライン
	{DMD_STGSLCT_SIZE_HALF_WIDTH + 278.f, 224.0f},	// キャンセルボタン
	{DMD_STGSLCT_SIZE_HALF_WIDTH - 136.f, 400.0f},	// Xボタン
	{DMD_STGSLCT_SIZE_HALF_WIDTH - 90.f, 400.0f},	// Yボタン
	{DMD_STGSLCT_SIZE_HALF_WIDTH, 360.0f},			// メッセージ
	{DMD_STGSLCT_SIZE_HALF_WIDTH, 328.0f},			// メッセージ2
	{DMD_STGSLCT_SIZE_HALF_WIDTH, 360.0f},			// スーパーソニックメッセージ
	{DMD_STGSLCT_SIZE_HALF_WIDTH, 344.0f},			// オプション
	{DMD_STGSLCT_SIZE_HALF_WIDTH, 404.0f},			// メインメニュー
	{DMD_STGSLCT_SIZE_HALF_WIDTH - 80.f, 274.0f},	// タイトルテキスト
	{DMD_STGSLCT_SIZE_HALF_WIDTH + 298.f, 224.0f},	// 戻る
	{DMD_STGSLCT_SIZE_HALF_WIDTH, 420.0f},			// OK
	{DMD_STGSLCT_SIZE_HALF_WIDTH - 88.f, 420.0f},	// YES
	{DMD_STGSLCT_SIZE_HALF_WIDTH + 88.f, 420.0f},	// NO
};
#else // #if _IPHONE
const static float dm_stgslct_win_act_pos_tbl[14][2] = {
	{DMD_STGSLCT_SIZE_HALF_WIDTH + 42.f, 280.0f},	// ウインドウ内のライン
	{DMD_STGSLCT_SIZE_HALF_WIDTH + 278.f, 224.0f},	// キャンセルボタン
	{DMD_STGSLCT_SIZE_HALF_WIDTH - 136.f, 400.0f},	// Xボタン
	{DMD_STGSLCT_SIZE_HALF_WIDTH - 90.f, 400.0f},	// Yボタン
	{DMD_STGSLCT_SIZE_HALF_WIDTH, 360.0f},			// メッセージ
	{DMD_STGSLCT_SIZE_HALF_WIDTH, 248.0f},			// メッセージ2
	{DMD_STGSLCT_SIZE_HALF_WIDTH, 360.0f},			// スーパーソニックメッセージ
	{DMD_STGSLCT_SIZE_HALF_WIDTH, 344.0f},			// オプション
	{DMD_STGSLCT_SIZE_HALF_WIDTH, 404.0f},			// メインメニュー
	{DMD_STGSLCT_SIZE_HALF_WIDTH - 80.f, 274.0f},	// タイトルテキスト
	{DMD_STGSLCT_SIZE_HALF_WIDTH + 298.f, 224.0f},	// 戻る
	{DMD_STGSLCT_SIZE_HALF_WIDTH, 420.0f},			// OK
	{DMD_STGSLCT_SIZE_HALF_WIDTH - 176.f, 420.0f},	// YES
	{DMD_STGSLCT_SIZE_HALF_WIDTH + 176.f, 420.0f},	// NO
};
#endif // #if !(_WII || _IPHONE)

// 各ステージの設定アクションID共通テーブル
const static float dm_stgslct_act_table_disp_id_tbl[DME_STGSLCT_STAGE_NUM] = {
    0.0f, 1.0f, 2.0f, 3.0f,						// ZONE1
	0.0f, 1.0f, 2.0f, 3.0f,						// ZONE2
	0.0f, 1.0f, 2.0f, 3.0f,						// ZONE3
	0.0f, 1.0f, 2.0f, 3.0f,						// ZONE4
	4.0f,										// FINAL STAGE
	5.0f, 5.0f, 5.0f, 5.0f, 5.0f, 5.0f, 5.0f,	// SPECIAL STAGE
};


// 各ステージのメッセージアクションID共通テーブル
const static float dm_stgslct_disp_msg_id_table_tbl[DME_STGSLCT_STAGE_NUM] = {
    0.0f, 1.0f, 2.0f, 3.0f,						// ZONE1
	4.0f, 5.0f, 6.0f, 7.0f,						// ZONE2
	8.0f, 9.0f, 10.0f, 11.0f,					// ZONE3
	12.0f, 13.0f, 14.0f, 15.0f,					// ZONE4
	16.0f,										// FINAL STAGE
	5.0f, 5.0f, 5.0f, 5.0f, 5.0f, 5.0f, 5.0f,	// SPECIAL STAGE
};


// 各ステージのY座標テーブル
const static float dm_stgslct_act_disp_pos_y_tbl[DME_STGSLCT_STAGE_NUM] = {
    0.0f, 1.0f, 2.0f, 3.0f,						// ZONE1
	0.0f, 1.0f, 2.0f, 3.0f,						// ZONE2
	0.0f, 1.0f, 2.0f, 3.0f,						// ZONE3
	0.0f, 1.0f, 2.0f, 3.0f,						// ZONE4
	0.0f,										// FINAL STAGE
	0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f,	// SPECIAL STAGE
};


// 各ステージのACTアクションIDテーブル
const static float dm_stgslct_act_num_disp_id_tbl[DME_STGSLCT_STAGE_NUM] = {
    0.0f, 1.0f, 2.0f, 0.0f,						// ZONE1
	0.0f, 1.0f, 2.0f, 0.0f,						// ZONE2
	0.0f, 1.0f, 2.0f, 0.0f,						// ZONE3
	0.0f, 1.0f, 2.0f, 0.0f,						// ZONE4
	0.0f,										// FINAL STAGE
	0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f,	// SPECIAL STAGE
};




// アクションIDテーブル(初期状態)
const static u32 g_dm_act_id_tbl[ACT_NUM] = {
    IDA_D_STGSLCT_ACT_Z_BG_LT,
    IDA_D_STGSLCT_ACT_Z_BG_LB,
    IDA_D_STGSLCT_ACT_Z_BG_RT,
    IDA_D_STGSLCT_ACT_Z_BG_RB,
    IDA_D_STGSLCT_ACT_TAB_NORMAL_L,
#if !_IPHONE
	IDA_D_STGSLCT_ACT_TAB_NORMAL_R,
#endif //!_IPHONE
	IDA_D_STGSLCT_ACT_ICON_SONIC,
#if !_IPHONE
	IDA_D_STGSLCT_ACT_TEX_SONIC,
#endif //!_IPHONE
	IDA_D_STGSLCT_ACT_NUM_A_100,
    IDA_D_STGSLCT_ACT_NUM_A_10,
    IDA_D_STGSLCT_ACT_NUM_A_1,
    IDA_D_STGSLCT_ACT_EMER_TAB1,
    IDA_D_STGSLCT_ACT_ICON_EMER1,
    IDA_D_STGSLCT_ACT_ICON_EMER2,
    IDA_D_STGSLCT_ACT_ICON_EMER3,
    IDA_D_STGSLCT_ACT_ICON_EMER4,
    IDA_D_STGSLCT_ACT_ICON_EMER5,
    IDA_D_STGSLCT_ACT_ICON_EMER6,
    IDA_D_STGSLCT_ACT_ICON_EMER7,
    IDA_D_STGSLCT_ACT_TEX_ZNAME,

	// モード共通・言語別
#if !_IPHONE
	IDA_D_STGSLCT_JP_ACT_TEX_TIME,
    IDA_D_STGSLCT_JP_ACT_TEX_SCORE,
    IDA_D_STGSLCT_JP_ACT_TEX_MENU,
    IDA_D_STGSLCT_ACT_TEX_EME,
#endif //!_IPHONE
	IDA_D_STGSLCT_JP_ACT_TEX_ZNAME,		// ZONE名
#if !_IPHONE
	IDA_D_STGSLCT_JP_ACT_TEX_MESS,
    IDA_D_STGSLCT_JP_ACT_TEX_MESS,
#endif //!_IPHONE

	// ZONE選択・言語共通
	IDA_D_STGSLCT_ACT_ZONE_SCR1,
    IDA_D_STGSLCT_ACT_ZONE_SCR2,
    IDA_D_STGSLCT_ACT_ZONE_SCR3,
    IDA_D_STGSLCT_ACT_ZONE_SCR4,
    IDA_D_STGSLCT_ACT_ZONE_SCR5,
    IDA_D_STGSLCT_ACT_ZONE_SCR6,
    IDA_D_STGSLCT_ACT_ZONE_SCR1_A,
    IDA_D_STGSLCT_ACT_ZONE_SCR1_B,
    IDA_D_STGSLCT_ACT_ZONE_SCR1_C,
    IDA_D_STGSLCT_ACT_ZONE_SCR2_A,
    IDA_D_STGSLCT_ACT_ZONE_SCR2_B,
    IDA_D_STGSLCT_ACT_ZONE_SCR2_C,
    IDA_D_STGSLCT_ACT_ZONE_SCR3_A,
    IDA_D_STGSLCT_ACT_ZONE_SCR3_B,
    IDA_D_STGSLCT_ACT_ZONE_SCR3_C,
    IDA_D_STGSLCT_ACT_ZONE_SCR4_A,
    IDA_D_STGSLCT_ACT_ZONE_SCR4_B,
    IDA_D_STGSLCT_ACT_ZONE_SCR4_C,
    IDA_D_STGSLCT_ACT_ZONE_TAB,
    IDA_D_STGSLCT_ACT_ZONE_TEXT,
    IDA_D_STGSLCT_JP_ACT_ZONE_TEXT,
    IDA_D_STGSLCT_ACT_ZONE_COVER1,
    IDA_D_STGSLCT_ACT_ZONE_COVER2,
    IDA_D_STGSLCT_ACT_ZONE_COVER3,
#if !_IPHONE
	IDA_D_STGSLCT_ACT_ZONE_CURSOR2,
    IDA_D_STGSLCT_ACT_ZONE_CURSOR1,
    IDA_D_STGSLCT_ACT_ZONE_CURSOR3,
#endif //!_IPHONE

	// ACT選択
	IDA_D_STGSLCT_ACT_ICON1,
    IDA_D_STGSLCT_ACT_ICON2,
    IDA_D_STGSLCT_ACT_ICON3,
    IDA_D_STGSLCT_ACT_ICON4,
    IDA_D_STGSLCT_ACT_ICON5,
    IDA_D_STGSLCT_ACT_ICON6,
    IDA_D_STGSLCT_ACT_ARROW1,
    IDA_D_STGSLCT_ACT_ARROW2,
    IDA_D_STGSLCT_ACT_TAB_TIME_L,
    IDA_D_STGSLCT_ACT_TAB_TIME_C,
    IDA_D_STGSLCT_ACT_TAB_TIME_R,
    IDA_D_STGSLCT_ACT_TAB_TIME_L2,
    IDA_D_STGSLCT_ACT_TAB_TIME_C2,
    IDA_D_STGSLCT_ACT_TAB_TIME_R2,
    IDA_D_STGSLCT_ACT_TAB_TIME_MOVE,
    IDA_D_STGSLCT_ACT_ACT1_TAB_B,
    IDA_D_STGSLCT_ACT_ACT1_TAB_A,
    IDA_D_STGSLCT_ACT_ACT1_TAB_C,
    IDA_D_STGSLCT_ACT_ACT1_SCR,
    IDA_D_STGSLCT_ACT_ACT1_SCR_BG,
    IDA_D_STGSLCT_JP_ACT_ACT1_TEX_ACT,
    IDA_D_STGSLCT_ACT_ACT1_NUM_B,
    IDA_D_STGSLCT_JP_ACT_ACT1_MESS,
    IDA_D_STGSLCT_ACT_ACT1_LINE,
    IDA_D_STGSLCT_ACT_ACT1_ICON_EMER,
    IDA_D_STGSLCT_ACT_SPE_NUM_STAGE,
    IDA_D_STGSLCT_ACT_SPE_EMER,
    IDA_D_STGSLCT_ACT_ACT_CURSOR_UP,
    IDA_D_STGSLCT_ACT_ACT_CURSOR_DOWN,
#if !_IPHONE
	IDA_D_STGSLCT_ACT_ACT_CURSOR2,
    IDA_D_STGSLCT_ACT_ACT_CURSOR1,
    IDA_D_STGSLCT_ACT_ACT_CURSOR3,
#endif //!_IPHONE
	IDA_D_STGSLCT_JP_ACT_ACT1_TEX_SCORE,
    IDA_D_STGSLCT_JP_ACT_ACT1_TEX_TIME,
    IDA_D_STGSLCT_JP_ACT_BOSS_TEX_BOSS,
    IDA_D_STGSLCT_JP_ACT_SPE_TEX_STAGE,
    IDA_D_STGSLCT_ACT_ACT1_NUM_A1,
    IDA_D_STGSLCT_ACT_ACT1_NUM_A2,
    IDA_D_STGSLCT_ACT_ACT1_NUM_A3,
    IDA_D_STGSLCT_ACT_ACT1_NUM_A4,
    IDA_D_STGSLCT_ACT_ACT1_NUM_A5,
    IDA_D_STGSLCT_ACT_ACT1_NUM_A6,
    IDA_D_STGSLCT_ACT_ACT1_NUM_A7,
    IDA_D_STGSLCT_ACT_ACT1_NUM_A8,
    IDA_D_STGSLCT_ACT_ACT1_NUM_A9,
    IDA_D_STGSLCT_ACT_ACT1_COVER2,
    IDA_D_STGSLCT_ACT_ACT1_COVER1,
    IDA_D_STGSLCT_ACT_ACT1_COVER3,

    IDA_D_STGSLCT_JP_ACT_TEX_SCTM01A,
    IDA_D_STGSLCT_JP_ACT_TEX_SCTM02A,
#if !_IPHONE
	IDA_D_STGSLCT_JP_ACT_TEX_SCTM01B,
    IDA_D_STGSLCT_JP_ACT_TEX_SCTM02B,
#endif //!_IPHONE

	IDA_D_STGSLCT_JP_ACT_TEX_WIN_MSG,
    IDA_D_STGSLCT_JP_ACT_TEX_WIN_MSG2,
    IDA_D_STGSLCT_JP_ACT_TEX_WIN_SSONIC,

	// メニュー共通データ
	IDA_D_CMN_BG_ACT_BG_WAVE,				//!< 
	IDA_D_CMN_BG_ACT_BG_DOWN_WHITE,			//!< 
	IDA_D_CMN_BG_ACT_BG_BLUE,				//!<
	
	IDA_D_CMN_BTN_ACT_BTN_BACK_STGSLCT,		//!< 
	IDA_D_CMN_BTN_ACT_BTN_LB_STGSLCT,		//!< 
	IDA_D_CMN_BTN_ACT_BTN_Y_STGSLCT,		//!< 
	IDA_D_CMN_BTN_ACT_BTN_LEFT_LB_STGSLCT,	//!< 
	IDA_D_CMN_BTN_ACT_BTN_LEFT_RB_STGSLCT,	//!< 
	IDA_D_CMN_BTN_ACT_BACK_BTN,				//!<
#if !_WII
	IDA_D_CMN_BTN_ACT_BTN_X,				//!< 
	IDA_D_CMN_BTN_ACT_BTN_Y,				//!<
#else
	IDA_D_CMN_BTN_ACT_BACK_BTN,				//!< 
	IDA_D_CMN_BTN_ACT_BACK_BTN,				//!<
#endif
	
#if !_IPHONE
	IDA_D_CMN_OBI_ACT_OBI_CENTER,			//!< 
	IDA_D_CMN_OBI_ACT_OBI_LEFT,				//!< 
	IDA_D_CMN_OBI_ACT_OBI_RIGHT,			//!< 
	
	IDA_D_CMN_WIN_ACT_WIN_LINE,				//!< 
#else //!_IPHONE
	IDA_D_CMN_WIN_ACT_STGSLCT_BACK_L,
	IDA_D_CMN_WIN_ACT_STGSLCT_BACK_C,

	IDA_D_CMN_WIN_ACT_BTN01_L,
	IDA_D_CMN_WIN_ACT_BTN01_C,
	IDA_D_CMN_WIN_ACT_BTN01_R,
	IDA_D_CMN_WIN_ACT_BTN01_L,
	IDA_D_CMN_WIN_ACT_BTN01_C,
	IDA_D_CMN_WIN_ACT_BTN01_R,
#endif //!_IPHONE
	
	IDA_D_CMN_MSG_JP_ACT_TEX_BACK_STGSLCT,	//!< 
#if !_IPHONE
	IDA_D_CMN_MSG_JP_ACT_TEX_WINTITLE,		//!< 
#endif //!_IPHONE
	IDA_D_CMN_MSG_JP_ACT_TEX_BACK,			//!< 
#if !_IPHONE
	IDA_D_CMN_MSG_JP_ACT_TEX_OK,			//!< 
#endif //!_IPHONE
	IDA_D_CMN_MSG_JP_ACT_TEX_YES,			//!< 
	IDA_D_CMN_MSG_JP_ACT_TEX_NO,			//!< 
	
};


static BOOL dm_stgslct_is_stage_start = FALSE;


// ----- Global Functions ----------------------（グローバル関数の定義：外部）
// ==========================================================================
// DmStgSlctStart
/*!
	ステージ選択画面開始処理
 */
// ==========================================================================
void DmStgSlctStart(void* arg)
{
    UNREFERENCED_PARAMETER(arg);

    dmStgSlctInit();
}


// ----- Static Functions --------------------（スタティック関数の定義：局所）
// ==========================================================================
// dmStgSlctInit
/*!
	ステージ選択画面初期化処理
 */
// ==========================================================================
void dmStgSlctInit(void)
{
    MTS_TASK_TCB tcb;
    DMS_STGSLCT_MAIN_WORK main_work;

    // アクションシステム初期化
    AoActSysSetDrawStateEnable(FALSE);      // どのデモを開始する際も必ず設定

    // メインタスク作成

    //mppAchievementSupport::get().event_LoseRingCounterClear();//sss - bad

    tcb = MTM_TASK_MAKE_TCB(dmStgSlctProcMain
                            , dmStgSlctDest
                            , 0
                            , DMD_STGSLCT_TASK_PAUSELEVEL
                            , DMD_STGSLCT_TASK_PRIO_MAIN
                            , DMD_STGSLCT_TASK_GROUP_MAIN
                            , sizeof(DMS_STGSLCT_MAIN_WORK)
                            , "STGSLCT_MAIN"
                            );

    // ワーク初期化
    main_work = (DMS_STGSLCT_MAIN_WORK*)mtTaskGetTcbWork(tcb);

    // 初期化処理があればここに記述
    // リージョンデータ取得(ボタン表示切り替え用)
    if (GeEnvGetDecideKey() == GSD_DECIDE_KEY_O)
    {
        main_work.is_jp_region = TRUE;
    }
    else
    {
        main_work.is_jp_region = FALSE;
    }

#if defined (MTD_DEBUG)
	// ※※※ここでステージ全解放入力があるかのチェックを行う		※デバッグ時のみ有効
#if !_IPHONE
	if (AoPadDirect() & KEY_R_UP) {
#else //!_IPHONE
	if (amTpIsTouchOn(2)) {
#endif //!_IPHONE
		dmStgSlctSetAllOpenStage(main_work);
	}
#endif



    // 初期化設定
    dmStgSlctSetInitData(main_work);

    // ハイスコア設定(レコードタイムも)
    dmStgSlctSetHiScore(main_work);

    // 各ステージのクリア情報設定(ここからどのステージが選択できるかを判別)
    dmStgSlctSetClearInfo(main_work);

# ifdef MPPDEBUG_OPEN_ALL			
    if (!false)
    {
        {//qqq//test -- open all
            for (int iii = 0; iii <= 23; iii++)
            {
                main_work.is_clear_stage[iii] = TRUE;
                /*
				if(true) {//test2 time att
					main_work.record_time[iii] = 6000+iii;
					main_work.hi_score[iii] = 5000+iii;
				}*/
            }
            main_work.is_final_open = DMD_STGSLCT_ZONE_ALL_OPEN;
        }
    }
#endif


    // 初期アナウンス表示設定(前イベント、グローバル変数から取得)
    dmStgSlctSetAnnounceMsg(main_work);

    main_work.tex_u[0] = main_work.tex_u[1] =
    main_work.tex_v[0] = main_work.tex_v[1] = 0.1f;

    // プロシージャ設定
    main_work.proc_menu_update = dmStgSlctLoadFontData;


    //	DmLoadingStart();

    // タスク開始
    //	amTaskStart(tcb);
}



// ==========================================================================
// dmStgSlctSetInitData
/*!
	必要初期化設定処理
 */
// ==========================================================================
void dmStgSlctSetInitData(DMS_STGSLCT_MAIN_WORK* main_work)
{
    s16 tmp_prev_evt = 0;

    UNREFERENCED_PARAMETER(main_work);

    tmp_prev_evt = SyGetEvtInfo().old_evt_id;

    // ZONE選択かACT選択かのどちらから始めるかを設定
    if (tmp_prev_evt == GSD_EVT_ID_MAINGAME
        || tmp_prev_evt == GSD_EVT_ID_SPSTAGE_BRANCH)
    {
        dm_stgslct_is_stage_start = TRUE;

        // ゲームモード設定
        main_work.cur_game_mode = (u32)g_gs_main_sys_info.game_mode;
    }
    else if (tmp_prev_evt == GSD_EVT_ID_TITLE
             || tmp_prev_evt == GSD_EVT_ID_MAINMENU)
    {
        dm_stgslct_is_stage_start = FALSE;

        // ゲームモード設定(デフォルト設定)
        main_work.cur_game_mode = (u32)0;
    }
    else if (tmp_prev_evt == GSD_EVT_ID_RANKING)
    {
        // ACT選択からしか遷移できないため、ACT選択からスタート
        dm_stgslct_is_stage_start = TRUE;

        // ゲームモード設定
        main_work.cur_game_mode = (u32)g_gs_main_sys_info.game_mode;

        // ランキングから戻ったことを示すフラグON
        main_work.flag |= DMD_STGSLCT_FLAG_PREV_EVT_RANKING;
    }
    else
    {
        // 上記のパターン以外はありえないので現状アサート	◆
        //		MTM_ASSERT(0);
        dm_stgslct_is_stage_start = FALSE;

        // ゲームモード設定(デフォルト設定)
        main_work.cur_game_mode = (u32)0;
    }

    // BGフェード初期値設定
    main_work.bg_fade.r = 255;
    main_work.bg_fade.g = 255;
    main_work.bg_fade.b = 255;
    main_work.bg_fade.a = 0;

    if (dm_stgslct_is_stage_start)
    {
        if (g_gs_main_sys_info.stage_id < GSD_MAIN_STAGE_ID_FINAL_1)
        {
            main_work.cur_stage = dmStgSlctSetNextFocusAct(main_work, g_gs_main_sys_info.stage_id);

            //			main_work.cur_stage = g_gs_main_sys_info.stage_id;

            main_work.cur_zone = dm_stgslct_act_zone_no_tbl[main_work.cur_stage];
            main_work.chng_zone = DME_STGSLCT_ACT_PAGE_NONE;

            main_work.focus_disp_no = 0;
            main_work.crsr_idx = (s32)(main_work.cur_stage % 4);
        }
        else if (g_gs_main_sys_info.stage_id >= GSD_MAIN_STAGE_ID_ENDING)
        {
            // エンディングステージからステセレには来ないため、アサート
            MTM_ASSERT(0);

            // 仮設定
            main_work.cur_stage = 0;
            main_work.cur_zone = 0;
            main_work.chng_zone = DME_STGSLCT_ACT_PAGE_NONE;

            main_work.focus_disp_no = 0;
            main_work.crsr_idx = 0;
        }
        else if (g_gs_main_sys_info.stage_id >= GSD_MAIN_STAGE_ID_SS1)
        {
            // 通常ACTからスペステリングに入った場合
            if (g_gs_main_sys_info.prev_stage_id != 0xffff)
            {
                main_work.cur_stage = dmStgSlctSetNextFocusAct(main_work, g_gs_main_sys_info.prev_stage_id);
                main_work.cur_zone = dm_stgslct_act_zone_no_tbl[main_work.cur_stage];
                main_work.chng_zone = DME_STGSLCT_ACT_PAGE_NONE;

                main_work.focus_disp_no = 0;
                main_work.crsr_idx = (s32)(main_work.cur_stage % 4);
            }
            // ステージ選択からスペステを選んだ場合
            else
            {
                main_work.cur_stage = dm_stgslct_zone_act_num_tbl[DME_STGSLCT_ACT_PAGE_SPE][0]
                                        + (g_gs_main_sys_info.stage_id - GSD_MAIN_STAGE_ID_SS1);
                main_work.cur_zone = DME_STGSLCT_ZONE_TYPE_SPE;
                main_work.chng_zone = DME_STGSLCT_ACT_PAGE_NONE;

                if (main_work.cur_stage <= DME_STGSLCT_STAGE_S_4)
                {
                    main_work.focus_disp_no = main_work.cur_stage - DME_STGSLCT_STAGE_S_1;
                    main_work.crsr_idx = 0;
                }
                else
                {
                    main_work.focus_disp_no = 3;
                    main_work.crsr_idx = main_work.cur_stage - DME_STGSLCT_STAGE_S_4;
                }
            }
        }
        else
        {
            main_work.cur_stage = DME_STGSLCT_STAGE_F_1;
            main_work.cur_zone = DME_STGSLCT_ZONE_TYPE_FINAL;
            main_work.chng_zone = DME_STGSLCT_ACT_PAGE_NONE;

            main_work.focus_disp_no = 0;
            main_work.crsr_idx = 0;
        }

        if (main_work.cur_zone != DME_STGSLCT_ACT_PAGE_FINAL)
        {
            main_work.crsr_pos_y = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_idx];
        }
        else
        {
            main_work.crsr_pos_y = 320.f;
        }
    }

    // アニメーション初期化
    main_work.btn_l_disp_frm = DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM;
    main_work.btn_r_disp_frm = DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM;

    // 背景設定
    main_work.cur_bg_id = main_work.cur_zone;

    // 帯テキストの初期表示位置設定
    main_work.obi_pos[0] = 0.f;
    main_work.obi_pos[1] = DMD_STGSLCT_OBI_MOVE_START_POS;
}



// ==========================================================================
// dmStgSlctSetHiScore
/*!
	ハイスコア設定処理
 */
// ==========================================================================
void dmStgSlctSetHiScore(DMS_STGSLCT_MAIN_WORK* main_work)
{
    UNREFERENCED_PARAMETER(main_work);

    // セーブデータ内のハイスコアを設定


}



// ==========================================================================
// dmStgSlctSetClearInfo
/*!
	クリア情報設定処理
 */
// ==========================================================================
void dmStgSlctSetClearInfo(DMS_STGSLCT_MAIN_WORK* main_work)
{
    u32 no_active = 0;
    u32 i = 0;
    u32 j = 0;

    // 通常ステージデータインスタンス作成
    gs::backup::SStage & data
        = gs::backup::SStage::CreateInstance();

    for (u32 i = 0; i <= GSD_MAIN_STAGE_ID_FINAL_1; i++)
    {
        // 通常ソニックのハイスコア取得
        main_work.n_sonic_hi_score[i] = (s32)data[i].GetHighScore(false);

        // スーパーソニックのハイスコア取得
        main_work.s_sonic_hi_score[i] = (s32)data[i].GetHighScore(true);

        // ２種のソニックのハイスコアがどちらも初期値でない場合
        if (main_work.n_sonic_hi_score[i] != DMD_STGSLCT_INIT_SCORE_NUM
            && main_work.s_sonic_hi_score[i] != DMD_STGSLCT_INIT_SCORE_NUM)
        {
            // ハイスコアが高い方を設定
            if (main_work.n_sonic_hi_score[i] >= main_work.s_sonic_hi_score[i])
            {
                // 通常ソニックの方が高い場合(同値含む)
                main_work.hi_score[i] = main_work.n_sonic_hi_score[i];
            }
            else
            {
                // スーパーソニックの方が高い場合
                main_work.hi_score[i] = main_work.s_sonic_hi_score[i];
            }
        }

        else
        {
            if (main_work.n_sonic_hi_score[i] == DMD_STGSLCT_INIT_SCORE_NUM
                && main_work.s_sonic_hi_score[i] == DMD_STGSLCT_INIT_SCORE_NUM)
            {
                // 初期値設定(どちらも初期値のため、通常ソニックの値を設定)
                main_work.hi_score[i] = main_work.n_sonic_hi_score[i];
            }

            else if (main_work.n_sonic_hi_score[i] == DMD_STGSLCT_INIT_SCORE_NUM)
            {
                // 通常ソニックが初期値のため、スパソニのスコアを設定
                main_work.hi_score[i] = main_work.s_sonic_hi_score[i];
            }
            else if (main_work.s_sonic_hi_score[i] == DMD_STGSLCT_INIT_SCORE_NUM)
            {
                // スパソニが初期値のため、通常ソニックのスコアを設定
                main_work.hi_score[i] = main_work.n_sonic_hi_score[i];
            }
            else
            {
                MTM_ASSERT(0);
                // ここに来る場合はありえないためASSERTをかけ、保険で通常ソニックの値を設定
                main_work.hi_score[i] = main_work.n_sonic_hi_score[i];
            }
        }

        if (main_work.hi_score[i] > DMD_STGSLCT_INIT_SCORE_NUM)
        {
            main_work.hi_score[i] = DMD_STGSLCT_INIT_SCORE_NUM - 1;
        }


        // 通常ソニックのレコード取得
        main_work.n_sonic_record_time[i] = (s32)data[i].GetFastTime(false);

        // スーパーソニックのレコード取得
        main_work.s_sonic_record_time[i] = (s32)data[i].GetFastTime(true);

        // ２種のソニックのレコードタイムがどちらも初期値でない場合
        if (main_work.n_sonic_record_time[i] != DMD_STGSLCT_INIT_RECORD_TIME_NUM
            && main_work.s_sonic_record_time[i] != DMD_STGSLCT_INIT_RECORD_TIME_NUM)
        {
            // ハイスコアが高い方を設定
            if (main_work.n_sonic_record_time[i] <= main_work.s_sonic_record_time[i])
            {
                // 通常ソニックの方が高い場合(同値含む)
                main_work.record_time[i] = main_work.n_sonic_record_time[i];
            }
            else
            {
                // スーパーソニックの方が高い場合
                main_work.record_time[i] = main_work.s_sonic_record_time[i];
            }
        }

        else
        {
            if (main_work.n_sonic_record_time[i] == DMD_STGSLCT_INIT_RECORD_TIME_NUM
                && main_work.s_sonic_record_time[i] == DMD_STGSLCT_INIT_RECORD_TIME_NUM)
            {
                // 初期値設定(どちらも初期値のため、通常ソニックの値を設定)
                main_work.record_time[i] = main_work.n_sonic_record_time[i];
            }

            else if (main_work.n_sonic_record_time[i] == DMD_STGSLCT_INIT_RECORD_TIME_NUM)
            {
                // 通常ソニックが初期値のため、スパソニのスコアを設定
                main_work.record_time[i] = main_work.s_sonic_record_time[i];
            }
            else if (main_work.s_sonic_record_time[i] == DMD_STGSLCT_INIT_RECORD_TIME_NUM)
            {
                // スパソニが初期値のため、通常ソニックのスコアを設定
                main_work.record_time[i] = main_work.n_sonic_record_time[i];
            }
            else
            {
                MTM_ASSERT(0);
                // ここに来る場合はありえないためASSERTをかけ、保険で通常ソニックの値を設定
                main_work.record_time[i] = main_work.n_sonic_record_time[i];
            }
        }

        // 通常ステージのレコードタイム取得(通常ソニック)
        //		main_work.record_time[i] = (s32)data[i].GetFastTime(false);

        if (main_work.record_time[i] > 36000)
        {
            main_work.record_time[i] = 35999;
        }
    }


    // スペシャルステージデータインスタンス作成
    gs::backup::SSpecial & spe_data
        = gs::backup::SSpecial::CreateInstance();

    for (u32 i = 0; i < 7; i++)
    {
        // スペシャルステージのハイスコア取得
        main_work.hi_score[i + DME_STGSLCT_STAGE_S_1] = (s32)spe_data[i].GetHighScore();

        if (main_work.hi_score[i + DME_STGSLCT_STAGE_S_1] > DMD_STGSLCT_INIT_SCORE_NUM)
        {
            main_work.hi_score[i + DME_STGSLCT_STAGE_S_1] = DMD_STGSLCT_INIT_SCORE_NUM - 1;
        }

        // スペシャルステージのレコードタイム取得
        main_work.record_time[i + DME_STGSLCT_STAGE_S_1]
            = (s32)spe_data[i].GetFastTime();

        if (main_work.record_time[i] > DMD_STGSLCT_INIT_RECORD_TIME_NUM)
        {
            main_work.record_time[i] = DMD_STGSLCT_INIT_RECORD_TIME_NUM - 1;
        }
    }


    // 各ステージのクリア設定処理(セーブデータから)
    for (s32 i = 0; i < DME_STGSLCT_STAGE_NUM; i++)
    {
        // 仮の設定処理
        if (main_work.hi_score[i] == DMD_STGSLCT_INIT_SCORE_NUM
            && main_work.record_time[i] == DMD_STGSLCT_INIT_RECORD_TIME_NUM)
        {
            main_work.is_clear_stage[i] = 0;
        }
        else
        {
            main_work.is_clear_stage[i] = 1;
        }
    }


    for (i = 0; i < DME_STGSLCT_STAGE_NUM; i++)
    {

        // スペシャルステージ
        if (i > DME_STGSLCT_STAGE_F_1)
        {
            if (!main_work.is_clear_stage[i])
            {
                // スペシャルステージを選択不可にする
                main_work.is_clear_stage[i] = -1;
            }
        }
        if (i == 17)
        {
            main_work.is_clear_stage[i] = 1;
        }

        // FINALステージ
        else if (i == DME_STGSLCT_STAGE_F_1)
        {
            for (j = DME_STGSLCT_STAGE_1_B; j < DME_STGSLCT_STAGE_F_1; j += 4)
            {
                if (main_work.is_clear_stage[j] != 1)
                {
                    no_active = 1;
                }
            }

            if (no_active)
            {
                // FINALステージを選択不可にする(表示させない)
                main_work.is_clear_stage[i] = -1;
            }
        }
        // BOSSステージ
        else if (i < DME_STGSLCT_STAGE_F_1 && ((i + 1) % 4 == 0))
        {
            for (j = 0; j < 3; j++)
            {
                if (!main_work.is_clear_stage[i - 3 + j])
                {
                    no_active = 1;
                }
            }

            if (no_active)
            {
                main_work.is_clear_stage[i] = -1;
            }
        }

        no_active = 0;
    }


    if (main_work.is_clear_stage[DME_STGSLCT_STAGE_1_B] == 1
        && main_work.is_clear_stage[DME_STGSLCT_STAGE_2_B] == 1
        && main_work.is_clear_stage[DME_STGSLCT_STAGE_3_B] == 1
        && main_work.is_clear_stage[DME_STGSLCT_STAGE_4_B] == 1)
    {
        main_work.is_final_open |= DMD_STGSLCT_ZONE_FINAL_OPEN;
    }

    // 残機数設定用にシステムデータ取得
    //	gs::backup::SSystem &sys_data = gs::backup::SSystem::CreateInstance();

    // 残機数設定
    main_work.player_stock = g_gs_main_sys_info.rest_player_num;//sys_data.GetPlayerStock();

    // カオスエメラルド取得設定
    for (u32 i = 0; i < 7; i++)
    {
        if (spe_data[i].IsGetEmerald())
        {
            // カオスエメラルドを一つでも所持していたらスペシャルゾーン解放
            main_work.is_final_open |= DMD_STGSLCT_ZONE_SPECIAL_OPEN;

            main_work.get_emerald |= 1 << i;

            main_work.eme_stage_no[i] = dm_stgslct_eme_get_act_no_tbl[spe_data[i].GetEmeraldStage()];
        }
        else
        {
            main_work.is_clear_stage[DME_STGSLCT_STAGE_S_1 + i] = -1;
        }
    }
}



// ==========================================================================
// dmStgSlctSetAnnounceMsg
/*!
	画面遷移時に表示するアナウンス設定処理
 */
// ==========================================================================
void dmStgSlctSetAnnounceMsg(DMS_STGSLCT_MAIN_WORK* main_work)
{
    UNREFERENCED_PARAMETER(main_work);

    // アナウンス表示判定用にシステムデータ取得
    gs::backup::SSystem & sys_data = gs::backup::SSystem::CreateInstance();

    // ステセレオープンアナウンス
    if (sys_data.IsAnnounce(gs::backup::SSystem::EAnnounce::OpenZoneSelect) == false
        && main_work.is_clear_stage[0] == TRUE)
    {
        main_work.announce_flag |= 1 << DME_STGSLCT_WIN_1_1_CLEAR;

        sys_data.SetAnnounce(gs::backup::SSystem::EAnnounce::OpenZoneSelect, true);
    }

    // BOSS1オープンアナウンス
    if (sys_data.IsAnnounce(gs::backup::SSystem::EAnnounce::OpenZone1Boss) == false
        && main_work.is_clear_stage[0] == TRUE
        && main_work.is_clear_stage[1] == TRUE
        && main_work.is_clear_stage[2] == TRUE)
    {
        main_work.announce_flag |= 1 << DME_STGSLCT_WIN_BOSS1_CAN_PLAY;

        sys_data.SetAnnounce(gs::backup::SSystem::EAnnounce::OpenZone1Boss, true);
    }

    // BOSS2オープンアナウンス
    if (sys_data.IsAnnounce(gs::backup::SSystem::EAnnounce::OpenZone2Boss) == false
        && main_work.is_clear_stage[4] == TRUE
        && main_work.is_clear_stage[5] == TRUE
        && main_work.is_clear_stage[6] == TRUE)
    {
        main_work.announce_flag |= 1 << DME_STGSLCT_WIN_BOSS2_CAN_PLAY;

        sys_data.SetAnnounce(gs::backup::SSystem::EAnnounce::OpenZone2Boss, true);
    }

    // BOSS3オープンアナウンス
    if (sys_data.IsAnnounce(gs::backup::SSystem::EAnnounce::OpenZone3Boss) == false
        && main_work.is_clear_stage[8] == TRUE
        && main_work.is_clear_stage[9] == TRUE
        && main_work.is_clear_stage[10] == TRUE)
    {
        main_work.announce_flag |= 1 << DME_STGSLCT_WIN_BOSS3_CAN_PLAY;

        sys_data.SetAnnounce(gs::backup::SSystem::EAnnounce::OpenZone3Boss, true);
    }

    // BOSS4オープンアナウンス
    if (sys_data.IsAnnounce(gs::backup::SSystem::EAnnounce::OpenZone4Boss) == false
        && main_work.is_clear_stage[12] == TRUE
        && main_work.is_clear_stage[13] == TRUE
        && main_work.is_clear_stage[14] == TRUE)
    {
        main_work.announce_flag |= 1 << DME_STGSLCT_WIN_BOSS4_CAN_PLAY;

        sys_data.SetAnnounce(gs::backup::SSystem::EAnnounce::OpenZone4Boss, true);
    }

    // FINALステージオープンアナウンス
    if (sys_data.IsAnnounce(gs::backup::SSystem::EAnnounce::OpenFinalZone) == false
        && main_work.is_clear_stage[3] == TRUE     // 全てのボスステージクリアならば
        && main_work.is_clear_stage[7] == TRUE
        && main_work.is_clear_stage[11] == TRUE
        && main_work.is_clear_stage[15] == TRUE)
    {
        main_work.announce_flag |= 1 << DME_STGSLCT_WIN_FINAL_CAN_SLCT;

        sys_data.SetAnnounce(gs::backup::SSystem::EAnnounce::OpenFinalZone, true);
    }

    // スーパーソニックオープンアナウンス
    if (sys_data.IsAnnounce(gs::backup::SSystem::EAnnounce::OpenSuperSonic) == false
        && main_work.is_clear_stage[23] == TRUE)
    {
        main_work.announce_flag |= 1 << DME_STGSLCT_WIN_SSONIC_SLCT;

        sys_data.SetAnnounce(gs::backup::SSystem::EAnnounce::OpenSuperSonic, true);
    }

    // スペステオープンアナウンス
    if (sys_data.IsAnnounce(gs::backup::SSystem::EAnnounce::OpenSpecialStage) == false
        && main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
    {
        main_work.announce_flag |= 1 << DME_STGSLCT_WIN_SPESTE_CAN_SLCT;

        sys_data.SetAnnounce(gs::backup::SSystem::EAnnounce::OpenSpecialStage, true);
    }

}



// ==========================================================================
// dmStgSlctProcMain
/*!
	ステージ選択画面メインプロシージャ処理
 */
// ==========================================================================
void dmStgSlctProcMain(MTS_TASK_TCB* tcb)
{
    DMS_STGSLCT_MAIN_WORK* main_work;

    // ワーク取得
    main_work = (DMS_STGSLCT_MAIN_WORK*)mtTaskGetTcbWork(tcb);

    // 終了処理
    if (main_work.flag & DMD_STGSLCT_FLAG_EXIT)
    {
        // タスククリア
        mtTaskClearTcb(tcb);

        // イベント遷移用設定
        dmStgSlctSetNextEvt(main_work);
    }

    // システム関連処理(サインアウト時はタイトルへ戻す)
    if (main_work.flag & DMD_STGSLCT_FLAG_SIGN_OUT_EXIT
        && !AoAccountIsCurrentEnable())
    {

        main_work.proc_menu_update = dmStgSlctProcFadeOut;

        // サインアウト終了フラグOFF
        main_work.flag &= ~DMD_STGSLCT_FLAG_SIGN_OUT_EXIT;
        main_work.next_evt = DME_STGSLCT_NEXT_EVT_TITLE;

        // フェード終了		※問題あれば有効にする
        //		IzFadeExit();

        // フェードアウト開始
        IzFadeInitEasy(IZE_FADE_SET_TYPE_TAKEOEVER
                       , IZE_FADE_TYPE_BLACK_FADEOUT
                       , DMD_STGSLCT_FADEOUT_TIME
                       );

        // BGMフェードアウト開始
        DmSndBgmPlayerExit();
        main_work.flag |= DMD_STGSLCT_FLAG_DEMO_SND_END;

        // ウインドウ遷移関連設定
        main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;
        main_work.flag &= ~DMD_STGSLCT_FLAG_CANCEL;
        main_work.proc_input = NULL;
        main_work.proc_win_input = NULL;
        main_work.win_timer = 0;
        main_work.win_cur_slct = 0;
        main_work.win_mode = DME_STGSLCT_WIN_STG_SLCT;
#if _IPHONE
		main_work.win_is_disp_cover = false;
#endif //_IPHONE
    }

    // BGフェード処理
    dmStgSlctSetBgFadeEfct(main_work);

    // ウインドウ処理用プロシージャ
    if (main_work.proc_win_update)
    {
        main_work.proc_win_update(main_work);
    }

    // メニュー処理用プロシージャ
    if (main_work.proc_menu_update)
    {
        main_work.proc_menu_update(main_work);
    }

    // 描画設定プロシージャ
    if (main_work.proc_draw)
    {
        main_work.proc_draw(main_work);
    }

}


// ==========================================================================
// dmStgSlctDest
/*!
	ステージ選択画面終了処理
 */
// ==========================================================================
void dmStgSlctDest(MTS_TASK_TCB* tcb)
{
    UNREFERENCED_PARAMETER(tcb);

}



// ==========================================================================
// dmStgSlctSetNextEvt
/*!
	次のイベント遷移設定処理
 */
// ==========================================================================
void dmStgSlctSetNextEvt(DMS_STGSLCT_MAIN_WORK* main_work)
{
    s16 set_next_evt = 0;
    u16 set_play_act = 0;

    // ステージID変換
    set_play_act = (u16)main_work.cur_stage;

    // 遷移先別設定処理
    switch (main_work.next_evt)
    {
        case DME_STGSLCT_NEXT_EVT_MAINGAME:
            // ステージID設定
            g_gs_main_sys_info.stage_id = set_play_act;
            g_gs_main_sys_info.prev_stage_id = 0xffff;

            // 初期状態のキャラ設定(ノーマルソニック)
            g_gs_main_sys_info.char_id[0] = GSD_CHAR_ID_SONIC;

            // ゲームモード設定(スコアかタイムか)
            g_gs_main_sys_info.game_mode = (GSE_GAME_MODE)main_work.cur_game_mode;

            // 最後のスペステがクリアしている場合、スパソニフラグON
            if (main_work.is_clear_stage[23] == TRUE)
            {
                g_gs_main_sys_info.game_flag |= GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
            }
            else
            {
                g_gs_main_sys_info.game_flag &= ~GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
            }

            // コントロール設定
#if !_IPHONE
            g_gs_main_sys_info.game_flag |= GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC;
#endif //!_IPHONE

            // スペステフラグOFF(ゲームへ遷移する際は必ずフラグOFF)
            g_gs_main_sys_info.game_flag &= ~GSD_MAINSYS_GAME_FLAG_SPECIAL_STAGE;

            // ゲーム開始前初期化
            GmMainGSInit();

            break;

        case DME_STGSLCT_NEXT_EVT_SPESTE:
            // ステージID設定
            g_gs_main_sys_info.stage_id = (u16)(set_play_act + 4);
            g_gs_main_sys_info.prev_stage_id = 0xffff;

            // 最後のスペステがクリアしている場合、スパソニフラグON
            if (main_work.is_clear_stage[23] == TRUE)
            {
                g_gs_main_sys_info.game_flag |= GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
            }
            else
            {
                g_gs_main_sys_info.game_flag &= ~GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
            }

            // ゲームモード設定(スコアかタイムか)
            g_gs_main_sys_info.game_mode = (GSE_GAME_MODE)main_work.cur_game_mode;

            // コントロール設定
#if !_IPHONE
            g_gs_main_sys_info.game_flag |= GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC;
#endif //!_IPHONE

            // スペステフラグOFF(ゲームへ遷移する際は必ずフラグOFF)
            g_gs_main_sys_info.game_flag &= ~GSD_MAINSYS_GAME_FLAG_SPECIAL_STAGE;

            break;

        case DME_STGSLCT_NEXT_EVT_RANKING:
            dm_stgslct_is_stage_start = TRUE;

            // ここでステージIDを一旦設定(指定したステージのランキングを開くため)
            if (main_work.cur_zone == DME_STGSLCT_ZONE_TYPE_SPE)
            {
                g_gs_main_sys_info.stage_id = (u16)(set_play_act + 4);
                g_gs_main_sys_info.prev_stage_id = 0xffff;
            }
            else
            {
                g_gs_main_sys_info.stage_id = set_play_act;
                g_gs_main_sys_info.prev_stage_id = 0xffff;
            }

            // ゲームモード設定
            if (main_work.cur_game_mode == DME_STGSLCT_PLAY_MODE_NORMAL)
            {
                g_gs_main_sys_info.game_mode = GSD_GAME_MODE_STORY;
            }
            else
            {
                g_gs_main_sys_info.game_mode = GSD_GAME_MODE_TIME_ATTACK;
            }

            break;

        case DME_STGSLCT_NEXT_EVT_MAINMENU:
            break;

        case DME_STGSLCT_NEXT_EVT_TITLE:
            break;

        default:
            break;
    }

    // イベント遷移設定
    set_next_evt = (s16)main_work.next_evt;

    SyDecideEvtCase(set_next_evt);
    SyChangeNextEvt();
}



// ==========================================================================
// dmStgSlctLoadFontData
/*!
	フォントデータ読み込みリクエスト処理
 */
// ==========================================================================
void dmStgSlctLoadFontData(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // gsFont構築
    GsFontBuild();

    main_work.proc_menu_update = dmStgSlctIsLoadFontData;
}



// ==========================================================================
// dmStgSlctIsLoadFontData
/*!
	フォントデータ読み込み終了チェック処理
 */
// ==========================================================================
void dmStgSlctIsLoadFontData(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // gsFont構築
    if (GsFontIsBuilded())
    {
        main_work.proc_menu_update = dmStgSlctLoadRequest;

        return;
    }
}



// ==========================================================================
// dmStgSlctLoadRequest
/*!
	ファイル読み込みリクエスト処理
 */
// ==========================================================================
void dmStgSlctLoadRequest(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // ファイル読み込み開始
    main_work.arc_amb_fs[0] = amFsReadBackground("DEMO/STGSLCT/D_STGSLCT.AMB");
    main_work.arc_amb_fs[1] = amFsReadBackground((char*)dm_stgslct_main_lng_amb_name_tbl[GsEnvGetLanguage()]);

    // メニュー共通データ読み込み
    for (int i = 0; i < 4; i++)
    {
        main_work.arc_cmn_amb_fs[i] = amFsReadBackground((char*)dm_stgslct_menu_cmn_amb_name_tbl[i]);
    }

    main_work.arc_cmn_amb_fs[4] = amFsReadBackground((char*)dm_stgslct_menu_cmn_lng_amb_name_tbl[GsEnvGetLanguage()]);

    // 次へ遷移
    main_work.proc_menu_update = dmStgSlctProcLoadWait;
}


// ==========================================================================
// dmStgSlctProcLoadWait
/*!
	ファイル読み込み待ち処理
 */
// ==========================================================================
void dmStgSlctProcLoadWait(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // ファイル読み込み完了待ち
    if (dmStgSlctIsDataLoad(main_work))
    {       // ファイル読込み完了チェック関数にする

        // ファイル取得
        for (int i = 0; i < 2; i++)
        {
            main_work.arc_amb[i] = (void*)main_work.arc_amb_fs[i].buf;
            main_work.arc_amb_fs[i].buf = NULL;

            amBindConv((u8*)main_work.arc_amb[i]);

            // AMBファイルロード
            main_work.ama[i] = amBindGet((AMS_AMB_HEADER*)main_work.arc_amb[i]
                                          , 0
                                          );

            main_work.amb[i] = amBindGet((AMS_AMB_HEADER*)main_work.arc_amb[i]
                                          , 1
                                          );

            // リクエストクリア
            amFsClearRequest(main_work.arc_amb_fs[i]);
            main_work.arc_amb_fs[i] = NULL;

            // アドレス変換
            amConvertAddress(main_work.ama[i]);
            amConvertAddress(main_work.amb[i]);

            // テクスチャ構築開始
            AoTexBuild(&main_work.tex[i], main_work.amb[i]);
            AoTexLoad(&main_work.tex[i]);
        }

        //
        GsFontBuild();

        // サウンド構築
        DmSndBgmPlayerInit();
        //		DmSoundBuild();

        // 次へ遷移
        main_work.proc_menu_update = dmStgSlctProcLoadWait2;
    }
}



// ==========================================================================
// dmStgSlctProcLoadWait2
/*!
	ファイル読み込み待ち処理		◆暫定対応
 */
// ==========================================================================
void dmStgSlctProcLoadWait2(DMS_STGSLCT_MAIN_WORK* main_work)
{
    if (dmStgSlctIsTexLoad(main_work) == 1
        && DmSndBgmPlayerIsSndSysBuild())
    {
        // メニュー共通データ
        for (int i = 0; i < 5; i++)
        {
            main_work.arc_cmn_amb[i] = (void*)main_work.arc_cmn_amb_fs[i].buf;
            main_work.arc_cmn_amb_fs[i].buf = NULL;

            amBindConv((u8*)main_work.arc_cmn_amb[i]);

            // AMBファイルロード
            main_work.cmn_ama[i] = amBindGet((AMS_AMB_HEADER*)main_work.arc_cmn_amb[i]
                                              , 0
                                              );

            main_work.cmn_amb[i] = amBindGet((AMS_AMB_HEADER*)main_work.arc_cmn_amb[i]
                                              , 1
                                              );

            // リクエストクリア
            amFsClearRequest(main_work.arc_cmn_amb_fs[i]);
            main_work.arc_cmn_amb_fs[i] = NULL;

            // アドレス変換
            amConvertAddress(main_work.cmn_ama[i]);
            amConvertAddress(main_work.cmn_amb[i]);

            // テクスチャ構築開始
            AoTexBuild(&main_work.cmn_tex[i], main_work.cmn_amb[i]);
            AoTexLoad(&main_work.cmn_tex[i]);
        }

        // 次へ遷移
        main_work.proc_menu_update = dmStgSlctProcTexBuildWait;
    }
}



// ==========================================================================
// dmStgSlctProcTexBuildWait
/*!
	テクスチャ構築待ち処理
 */
// ==========================================================================
void dmStgSlctProcTexBuildWait(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // テクスチャ構築完了判定
    if (dmStgSlctIsTexLoad2(main_work) == 1
        && DmSndBgmPlayerIsSndSysBuild())
    {

        // 次へ遷移
        main_work.proc_menu_update = dmStgSlctProcCheckLoadingEnd;

        // ローディング終了設定
        //		DmLoadingSetLoadComplete();

        // ここに仮のセーブ待ち処理追加
        DmSaveMenuStart(TRUE);

        //		DmSoundInit();
    }
}



// ==========================================================================
// dmStgSlctProcCheckLoadingEnd
/*!
	ローディング終了待ち処理
 */
// ==========================================================================
void dmStgSlctProcCheckLoadingEnd(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // テクスチャ構築完了判定
    if (DmSaveIsExit())
    {
        // 次へ遷移
        main_work.proc_menu_update = dmStgSlctProcCreateAct;

        DmSndBgmPlayerPlayBgm(DME_SND_BGM_PLAYER_IDX_MENU);
        //		DmSoundPlayMenuBGM(DME_SOUND_BGM_IDX_MENU
        //						   , DMD_STGSLCT_BGM_FADEIN_TIME);

        // サインアウト終了フラグON
        main_work.flag |= DMD_STGSLCT_FLAG_SIGN_OUT_EXIT;
    }
}



// ==========================================================================
// dmStgSlctProcCreateAct
/*!
	アクション生成処理
 */
// ==========================================================================
void dmStgSlctProcCreateAct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // ファイル選別
    const void* ama = NULL;
    AOS_TEXTURE* tex = NULL;

    // アクション構築
    for (u32 i = 0; i < ACT_NUM; ++i)
    {
        if (i >= ACT_TEX_FIX_BACK)
        {
            ama = main_work.cmn_ama[4];
            tex = &main_work.cmn_tex[4];
        }
#if !_IPHONE
        else if (i >= ACT_WIN_LINE)
        {
            ama = main_work.cmn_ama[3];
            tex = &main_work.cmn_tex[3];
        }
        else if (i >= ACT_OBI_C)
        {
            ama = main_work.cmn_ama[2];
            tex = &main_work.cmn_tex[2];
        }
#else //!_IPHONE
		else if (ACT_BACK_BTN01_L <= i && i <= ACT_BACK_BTN01_R) {
			ama = main_work.cmn_ama[3];
			tex = &main_work.cmn_tex[3];
		}
		else if (ACT_YES_BTN_L <= i && i <= ACT_NO_BTN_R) {
			ama = main_work.cmn_ama[3];
			tex = &main_work.cmn_tex[3];
		}
#endif //!_IPHONE
        else if (i >= ACT_BTN_CANCEL1)
        {
            ama = main_work.cmn_ama[1];
            tex = &main_work.cmn_tex[1];
        }
        else if (i >= ACT_WAVE_BG)
        {
            ama = main_work.cmn_ama[0];
            tex = &main_work.cmn_tex[0];
        }

        else if (i >= ACT_TEX_BIG_TIME)
        {
            ama = main_work.ama[DME_STGSLCT_DATA_TYPE_LANG_DATA];
            tex = &main_work.tex[DME_STGSLCT_DATA_TYPE_LANG_DATA];
        }
#if !_IPHONE
        else if ((i >= ACT_TEX_MODE_TIME && i <= ACT_TEX_MENU)
                 || (i >= ACT_TEX_ZONE_UP_S && i <= ACT_TEX_OBI2)
#else //!_IPHONE
		else if ((i == ACT_TEX_ZONE_UP_S)
#endif //!_IPHONE
                 || (i >= ACT_TAB_TEX_SCORE && i <= ACT_TAB_TEX_SPE_STAGE)
                 || i == ACT_TAB_ZONE_TEXT_S
                 || i == ACT_TAB_TEXT
                 || i == ACT_TAB_MESS)
        {
            ama = main_work.ama[DME_STGSLCT_DATA_TYPE_LANG_DATA];
            tex = &main_work.tex[DME_STGSLCT_DATA_TYPE_LANG_DATA];
        }
        else
        {
            ama = main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA];
            tex = &main_work.tex[DME_STGSLCT_DATA_TYPE_CMN_DATA];
        }

        // 構築
        AoActSetTexture(AoTexGetTexList(tex));
        main_work.act[i] = AoActCreate(ama, g_dm_act_id_tbl[i]);
    }

#if _IPHONE
	{
		AoActUpdate(main_work.act[ACT_TAB_STATE_MOVE], 0.0f);
		main_work.act_tab_state_move_base_pos[AMD_X] = main_work.act[ACT_TAB_STATE_MOVE].sprite.center_x;
		main_work.act_tab_state_move_base_pos[AMD_Y] = main_work.act[ACT_TAB_STATE_MOVE].sprite.center_y;
	}
#endif //_IPHONE
#if _IPHONE    //当たり判定構築          
	{
		//ゾーン選択用当たり判定
		for (er::CTrgAoAction *zone = main_work.trg_zone, *zone_end = main_work.trg_zone + arrayof(main_work.trg_zone); zone != zone_end; ++zone) {
			new(zone) er::CTrgAoAction();
			zone.Create(main_work.act[ACT_TAB_ZONE_TAB]);
		}
		//アクト選択用当たり判定
		for (er::CTrgAoAction *act = main_work.trg_act, *act_end = main_work.trg_act + arrayof(main_work.trg_act); act != act_end; ++act) {
			new(act) er::CTrgAoAction();
			act.Create(main_work.act[ACT_TAB_TABLE2]);
		}
		//アクト選択時のゾーンタブ用当たり判定
		for (int i = 0, max = arrayof(main_work.trg_act_tab); i < max; ++i) {
			er::CTrgAoAction &tab = main_work.trg_act_tab[i];
			new(&tab) er::CTrgAoAction();
			tab.Create(main_work.act[ACT_ICON_DOWN_1 + i]);
		}
		//アクト選択時のLR用当たり判定
		for (int i = 0, max = arrayof(main_work.trg_act_lr); i < max; ++i) {
			er::CTrgAoAction &lr = main_work.trg_act_lr[i];
			int act_id = ((0 == i)? ACT_ICON_L_ARROW: ACT_ICON_R_ARROW);
			new(&lr) er::CTrgAoAction();
			lr.Create(main_work.act[act_id]);
		}
		{	//アクト選択時の上下ドラッグ用、左右フリック用当たり判定
			er::CTrgFlick &move = main_work.trg_act_move;
			new(&move) er::CTrgFlick();
			move.Create(0,0,static_cast<float>(AMD_SCREEN_2D_WIDTH),static_cast<float>(AMD_SCREEN_2D_HEIGHT));
			move.SetMoveThreshold(1);
		}
		//アクト選択時のモード切り替え用当たり判定
		for (int i = 0, max = arrayof(main_work.trg_mode); i < max; ++i) {
			er::CTrgAoAction &mode = main_work.trg_mode[i];
			int act_id = ((0 == i)? ACT_TAB_STATE_C: ACT_TAB_STATE_C2);
			new(&mode) er::CTrgAoAction();
			mode.Create(main_work.act[act_id]);
		}
		{	//キャンセル用当たり判定
			er::CTrgAoAction &cancel = main_work.trg_cancel;
			new(&cancel) er::CTrgAoAction();
			cancel.Create(main_work.act[ACT_BACK_BTN01_R]);
		}
		//ウインドウ・はい/いいえ用当たり判定
		for (int i = 0, max = arrayof(main_work.trg_answer); i < max; ++i) {
			er::CTrgAoAction &answer = main_work.trg_answer[i];
			int act_id = ((0 == i)? ACT_NO_BTN_C: ACT_YES_BTN_C);
			new(&answer) er::CTrgAoAction();
			answer.Create(main_work.act[act_id]);
		}
	}
#endif //_IPHONE	//当たり判定構築

    // イベント遷移
    main_work.proc_menu_update = dmStgSlctProcSetDispEfctData;
}



// ==========================================================================
// dmStgSlctProcSetDispEfctData
/*!
	表示演出周り設定処理
 */
// ==========================================================================
void dmStgSlctProcSetDispEfctData(DMS_STGSLCT_MAIN_WORK* main_work)
{
    u32 act_num_start = 0;
    u32 act_num_end = 0;

    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

    // もし何かの解放メッセージ表示がある場合、強制的にZONE選択にする

    // もし何かの解放メッセージ表示がある場合、白フラッシュ演出が出るようにする
    // ※その際、ウインドウ表示開始は白フラッシュが終ってからにする
    if ((main_work.announce_flag & 1 << DME_STGSLCT_WIN_1_1_CLEAR)
        || (main_work.announce_flag & 1 << DME_STGSLCT_WIN_SPESTE_CAN_SLCT)
        || (main_work.announce_flag & 1 << DME_STGSLCT_WIN_FINAL_CAN_SLCT))
    {
        // 白フラッシュフラグON
        main_work.flag |= DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT;
    }

    // 


    // 最初に表示する状態を設定
    if (dm_stgslct_is_stage_start
        && !(main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT))
    {
        // ACTセレクトの描画プロシージャ設定
        main_work.proc_draw = dmStgSlctStageSelectDraw;
        main_work.state = DME_STGSLCT_MODE_STATE_ACT_SLCT;
    }
    else
    {
        // ZONEセレクトの描画プロシージャ設定
        main_work.proc_draw = dmStgSlctZoneSelectDraw;
        main_work.state = DME_STGSLCT_MODE_STATE_ZONE_SLCT;
#if _IPHONE
		main_work.cur_zone = main_work.cur_zone = u32(-1);
#endif //_IPHONE
    }

    // ※この初期表示位置については、初期状態がZONE選択かACT選択かで分かれる
    if (main_work.state == DME_STGSLCT_MODE_STATE_ZONE_SLCT)
    {
        if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
        {
            // ZONEテーブルの初期表示位置設定
            for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                main_work.zone_pos[i][0] = dm_stgslct_a_zone_disp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_a_zone_disp_pos_tbl[i][1];
            }
        }

        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
        {
            // ZONEテーブルの初期表示位置設定
            for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                main_work.zone_pos[i][0] = dm_stgslct_s_zone_disp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_s_zone_disp_pos_tbl[i][1];
            }
        }

        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
        {
            // ZONEテーブルの初期表示位置設定
            for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                main_work.zone_pos[i][0] = dm_stgslct_f_zone_disp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_f_zone_disp_pos_tbl[i][1];
            }
        }
        else
        {
            // ZONEテーブルの初期表示位置設定
            for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                main_work.zone_pos[i][0] = dm_stgslct_n_zone_disp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_n_zone_disp_pos_tbl[i][1];
            }
        }

        // ACTテーブルの外側表示位置設定
        for (u32 j = 0; j < DME_STGSLCT_STAGE_NUM; j++)
        {
            main_work.act_top_pos_x[j] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
            main_work.act_top_pos_y[j] = dm_stgslct_act_disp_y_pos_tbl[j];
            //											+ (float)(dm_stgslct_act_disp_pos_y_tbl[j] * DMD_STGSLCT_ACT_TABLE_DIST_Y);
        }

        // モードテキスト、エメラルドテーブルの初期表示位置設定
        main_work.mode_tex_pos_y = DMD_STGSLCT_DOWN_ACT_NODISP_POS;
        main_work.chaos_eme_pos_y = DMD_STGSLCT_DOWN_ACT_DISP_POS;

        // ZONE選択から始まる場合は初めから左上・右上のテキストを非表示
        main_work.mode_tex_move_frm = 0;
    }
    // ACT選択から始まるとき
    else
    {
        // ZONEテーブルの初期表示位置設定
        if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
        {
            // ZONEテーブルの初期表示位置設定
            for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                main_work.zone_pos[i][0] = dm_stgslct_a_zone_nodisp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_a_zone_nodisp_pos_tbl[i][1];
            }
        }

        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
        {
            // ZONEテーブルの初期表示位置設定
            for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                main_work.zone_pos[i][0] = dm_stgslct_s_zone_nodisp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_s_zone_nodisp_pos_tbl[i][1];
            }
        }

        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
        {
            // ZONEテーブルの初期表示位置設定
            for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                main_work.zone_pos[i][0] = dm_stgslct_f_zone_nodisp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_f_zone_nodisp_pos_tbl[i][1];
            }
        }
        else
        {
            // ZONEテーブルの初期表示位置設定
            for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                main_work.zone_pos[i][0] = dm_stgslct_n_zone_nodisp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_n_zone_nodisp_pos_tbl[i][1];
            }
        }



        // ACTテーブルの内側表示位置設定
        for (u32 j = act_num_start; j < act_num_end; j++)
        {
            main_work.act_top_pos_x[j] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;
            if (main_work.cur_zone != DME_STGSLCT_ZONE_TYPE_SPE)
            {
                main_work.act_top_pos_y[j] = dm_stgslct_act_disp_y_pos_tbl[j];
            }
            else
            {
                main_work.act_top_pos_y[j] = dm_stgslct_act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
            }
            //											+ (float)(dm_stgslct_act_disp_pos_y_tbl[j] * DMD_STGSLCT_ACT_TABLE_DIST_Y);

            //			main_work.act_top_pos_y[j] = dm_stgslct_act_disp_y_pos_tbl[j]
            //											+ (float)(dm_stgslct_act_disp_pos_y_tbl[j] * DMD_STGSLCT_ACT_TABLE_DIST_Y);
        }

#if _IPHONE
		dmStgSlctStageSelectChngZoneSetInZoneScroll(main_work, main_work.cur_stage);
#endif //_IPHONE		

        // モードテキスト、エメラルドテーブルの初期表示位置設定
        main_work.mode_tex_pos_y = DMD_STGSLCT_DOWN_ACT_DISP_POS;
        main_work.chaos_eme_pos_y = DMD_STGSLCT_DOWN_ACT_NODISP_POS;

        // ACT選択から始まる場合は初めから左上・右上のテキストを表示
        main_work.mode_tex_move_frm = 5;
    }

    main_work.crsr_pos_y = dm_stgslct_act_disp_y_pos_tbl[0];

    // テクスチャセットまで出来たので描画プロシージャを設定
    main_work.proc_draw = dmStgSlctProcActDraw;

    // フェード処理開始
    IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                   , IZE_FADE_TYPE_BLACK_FADEIN
                   , DMD_STGSLCT_FADEIN_TIME
                   );

    // イベント遷移
    main_work.proc_menu_update = dmStgSlctProcFadeIn;
}



// ==========================================================================
// dmStgSlctProcFadeIn
/*!
	フェードイン中処理
 */
// ==========================================================================
void dmStgSlctProcFadeIn(DMS_STGSLCT_MAIN_WORK* main_work)
{
    if (IzFadeIsEnd())
    {
        // フェード終了
        IzFadeExit();

        // ここで白演出があれば演出へ遷移し、なければ通常処理
        if (main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT)
        {
            // 白フェードアウト設定
            IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                           , IZE_FADE_TYPE_WHITE_FADEOUT
                           , DMD_STGSLCT_FADEIN_TIME
                           );

            // 白フラッシュ演出シーケンスへ
            main_work.proc_menu_update = dmStgSlctProcSetWhiteFlashEfct;
        }

        else
        {
            // 通常設定処理へ
            main_work.proc_menu_update = dmStgSlctProcSetSlctStartData;
        }
    }
}



// ==========================================================================
// dmStgSlctProcSetWhiteFlashEfct
/*!
	白フラッシュ演出中処理
 */
// ==========================================================================
void dmStgSlctProcSetWhiteFlashEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    if (IzFadeIsEnd())
    {
        // フェード処理開始
        IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                       , IZE_FADE_TYPE_WHITE_FADEIN
                       , DMD_STGSLCT_FADEIN_TIME
                       );

        // ここで隠れていた分のゾーンを表示にする


        // 白フラッシュ演出フラグOFF
        main_work.flag &= ~DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT;

        // 白フェードアウト終了待ち処理へ
        main_work.proc_menu_update = dmStgSlctProcIsWhiteFlashEfctEnd;
    }
}



// ==========================================================================
// dmStgSlctProcIsWhiteFlashEfctEnd
/*!
	白フラッシュ演出中処理
 */
// ==========================================================================
void dmStgSlctProcIsWhiteFlashEfctEnd(DMS_STGSLCT_MAIN_WORK* main_work)
{
    if (IzFadeIsEnd())
    {
        // フェード終了
        IzFadeExit();

        // ZONE選択へ
        main_work.proc_menu_update = dmStgSlctProcSetSlctStartData;
    }
}



// ==========================================================================
// dmStgSlctProcSetSlctStartData
/*!
	各選択画面へ遷移するための設定処理
 */
// ==========================================================================
void dmStgSlctProcSetSlctStartData(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // ウインドウアナウンスがあればウインドウプロシージャ設定
    main_work.proc_win_update = dmStgSlctProcWindowNodispIdle;

    // ※この初期表示位置については、初期状態がZONE選択かACT選択かで分かれる
    if (main_work.state == DME_STGSLCT_MODE_STATE_ZONE_SLCT)
    {
        // 入力処理設定		仮
        main_work.proc_input = dmStgSlctInputProcZoneSelect;

        // ZONE選択へ
        main_work.proc_menu_update = dmStgSlctProcZoneSelectIdle;
    }
    else
    {
        // 入力処理設定		仮
        main_work.proc_input = dmStgSlctInputProcStageSelect;

        // ZONE選択へ
        main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;

        main_work.disp_flag |= DMD_STGSLCT_DISP_FLAG_LR_ARROW;
    }

    // 初期ZONEスクリーンID設定
    main_work.zone_scr_id = 0;
}



// ==========================================================================
// dmStgSlctProcZoneSelectIdle
/*!
	ゾーン選択時の入力待ち中処理
 */
// ==========================================================================
void dmStgSlctProcZoneSelectIdle(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 入力処理
    if (main_work.proc_input)
    {
        main_work.proc_input(main_work);
    }

#if _IPHONE
	{	//戻る演出
		er::CTrgAoAction &cancel = main_work.trg_cancel;
		float frame = main_work.act[ACT_BACK_BTN01_L].frame;
		if (cancel.GetState(0)[er::CTrgState::EState::Up] && cancel.GetState(0)[er::CTrgState::EState::Prev]) {
			frame = 2.0f;
		} else if (cancel.GetState(0)[er::CTrgState::EState::On]) {
			frame = 1.0f;
		} else if (2.0f <= frame) {
			//決定演出中なら続ける
		} else {
			frame = 0.0f;
		}

		AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[3]));
		for (int i = ACT_BACK_BTN01_L; i <= ACT_BACK_BTN01_R; i++) {
			AoActSetFrame(main_work.act[i], frame);
			AoActUpdate(main_work.act[i], 0.0f);
		}
	}
#endif //_IPHONE

    // キャンセルフラグONならば
    if (main_work.flag & DMD_STGSLCT_FLAG_CANCEL)
    {
        // フラグOFF
        main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;
        main_work.flag &= ~DMD_STGSLCT_FLAG_CANCEL;

        main_work.proc_win_input = NULL;
        main_work.proc_input = NULL;

        main_work.proc_win_update = NULL;
        main_work.proc_menu_update = dmStgSlctProcFadeOut;

        // メインメニューへイベント遷移先設定
        main_work.next_evt = DME_STGSLCT_NEXT_EVT_MAINMENU;

        // フェードアウト開始
        IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                       , IZE_FADE_TYPE_BLACK_FADEOUT
                       , DMD_STGSLCT_FADEOUT_TIME
                       );

        DmSoundPlaySE("Cancel");

        // BGMフェードアウト開始
#if defined (GSD_DEBUG_DEMO_SELECT) || defined (GSD_DEBUG_DEMO_SELECT_TOP)
		DmSndBgmPlayerExit();
		main_work.flag |= DMD_STGSLCT_FLAG_DEMO_SND_END;
#endif
        //		DmSoundStopBGM(DMD_STGSLCT_BGM_FADEOUT_TIME);

        return;
    }

    // 決定フラグONならば
    if (main_work.flag & DMD_STGSLCT_FLAG_DECIDE)
    {

        DmSoundPlaySE("Ok");

        main_work.proc_menu_update = dmStgSlctProcZoneSelectDecideEfct;

        // 決定ZONEテーブルの移動演出フラグON
        main_work.flag |= DMD_STGSLCT_FLAG_DECIDE_ZONE_TABLE_EFCT;

        main_work.timer = 0;

        return;
    }

    // ZONEスクリーン切り替え演出
    dmStgSlctSetZoneScrChangeEfct(main_work);
}



// ==========================================================================
// dmStgSlctProcZoneSelectDecideEfct
/*!
	ゾーン選択時のゾーンテーブル掃け演出中処理
 */
// ==========================================================================
void dmStgSlctProcZoneSelectDecideEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    main_work.timer++;

    // 演出設定処理
    dmStgSlctSetDecideZoneEfctPos(main_work);

    // 演出終了チェック
    if (dmStgSlctIsDecideZoneEfctPos(main_work))
    {
        main_work.proc_menu_update = dmStgSlctProcZoneSelectOutEfct;

        main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;

        // 掃け演出開始フラグ(アクティブは開始をずらす)
        for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
        {
            if (main_work.cur_zone != i)
            {
                main_work.efct_out_flag |= 1 << i;
            }
        }

        main_work.decide_zone_efct_dist_x = 0;
        main_work.decide_zone_efct_dist_y = 0;

        main_work.cur_stage = (s32)dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        main_work.prev_stage = main_work.cur_stage;

        main_work.proc_input = NULL;

        main_work.timer = 0;

#if _IPHONE
		main_work.trg_act_move.ResetState();
#endif _IPHONE

        // フラグOFF
        main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;
        main_work.flag &= ~DMD_STGSLCT_FLAG_CANCEL;

        return;
    }

    // ZONEスクリーン切り替え演出
    dmStgSlctSetZoneScrChangeEfct(main_work);
}



// ==========================================================================
// dmStgSlctProcZoneSelectOutEfct
/*!
	ゾーン選択時のゾーンテーブル掃け演出中処理
 */
// ==========================================================================
void dmStgSlctProcZoneSelectOutEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    u32 act_num_start = 0;
    u32 act_num_end = 0;

    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

    // 掃け演出が終了したら
    if (dmStgSlctIsZonePosOutEfct(main_work))
    {
        main_work.proc_menu_update = dmStgSlctProcStageSelectInEfct;

        main_work.efct_time = 0;

        // 指定座標に設定
        for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
        {
            if (main_work.cur_zone != i)
            {
                main_work.zone_pos[i][0] = dm_stgslct_a_zone_nodisp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_a_zone_nodisp_pos_tbl[i][1];
            }
        }

        // ACT側も演出用の座標に設定
        for (u32 i = act_num_start; i < act_num_end; i++)
        {
            main_work.act_top_pos_x[i] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;

            if (main_work.cur_zone != DME_STGSLCT_ZONE_TYPE_SPE)
            {
                main_work.act_top_pos_y[i] = dm_stgslct_act_disp_y_pos_tbl[i];
            }
            else
            {
                main_work.act_top_pos_y[i] = dm_stgslct_act_tab_disp_y_pos_tbl[0];
            }
        }

        main_work.state = DME_STGSLCT_MODE_STATE_ACT_SLCT;

        // ZONE選択へ戻ると左上・右上のテキストのACTIONフレームを0に戻す(リセット)
        main_work.mode_tex_move_frm = 0;

#if _IPHONE
		main_work.is_disp_cover = false; //アクトセレクトのカバーを外す
		dmStgSlctStageSelectChngZoneSetInZoneScroll(main_work);
#endif //_IPHONE

        return;
    }

    // 掃け演出処理
    dmStgSlctSetZonePosOutEfct(main_work);

    // アクティブ分の演出開始フラグON
    if (main_work.efct_time == 20)
    {
        main_work.efct_out_flag |= 1 << main_work.cur_zone;
    }

    // タイマー更新
    main_work.efct_time++;

    // ZONEスクリーン切り替え演出
    dmStgSlctSetZoneScrChangeEfct(main_work);
}



// ==========================================================================
// dmStgSlctProcStageSelectInEfct
/*!
	ステージ選択時のステージテーブル入り演出中処理
 */
// ==========================================================================
void dmStgSlctProcStageSelectInEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 入り演出処理
    dmStgSlctSetStagePosInEfct(main_work);

    // 入り演出が終了したら
    if (dmStgSlctIsStagePosInEfct(main_work))
    {
        main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;

        if (main_work.proc_win_update == dmStgSlctProcWindowNodispIdle)
        {
            main_work.proc_input = dmStgSlctInputProcStageSelect;
        }

        main_work.disp_flag |= DMD_STGSLCT_DISP_FLAG_LR_ARROW;

        // カーソル初期設定
        main_work.crsr_idx = 0;

        if (main_work.cur_zone != DME_STGSLCT_ACT_PAGE_FINAL)
        {
            main_work.crsr_pos_y = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_idx];
        }
        else
        {
            main_work.crsr_pos_y = 320.f;
        }

#if !_IPHONE
        main_work.focus_disp_no = 0;
#endif //!_IPHONE

        return;
    }

    // ACT選択に切り替え時は左上・右上のテキストを移動開始
    main_work.mode_tex_move_frm++;

    if (main_work.mode_tex_move_frm > 5)
    {
        main_work.mode_tex_move_frm = 5;
    }


    // 座標再設定

}



// ==========================================================================
// dmStgSlctProcStageSelectIdle
/*!
	ステージ選択時の入力待ち中処理
 */
// ==========================================================================
void dmStgSlctProcStageSelectIdle(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 入力処理
    if (main_work.proc_input)
    {
        main_work.proc_input(main_work);
    }

#if _IPHONE
	{	//戻る演出
		er::CTrgAoAction &cancel = main_work.trg_cancel;
		float frame = main_work.act[ACT_BACK_BTN01_L].frame;
		if (!main_work.trg_act_move.GetState(0)[er::CTrgState::EState::DragAndDrop] && cancel.GetState(0)[er::CTrgState::EState::Lock]) {
			if (cancel.GetState(0)[er::CTrgState::EState::Up] && cancel.GetState(0)[er::CTrgState::EState::Prev]) {
				frame = 2.0f;
			} else if (cancel.GetState(0)[er::CTrgState::EState::On]) {
				frame = 1.0f;
			} else {
				frame = 0.0f;
			}
		} else if (2.0f <= frame) {
			//決定演出中なら続ける
		} else {
			frame = 0.0f;
		}

		AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[3]));
		for (int i = ACT_BACK_BTN01_L; i <= ACT_BACK_BTN01_R; i++) {
			AoActSetFrame(main_work.act[i], frame);
			AoActUpdate(main_work.act[i], 0.0f);
		}
	}
#endif //_IPHONE

    // キャンセルフラグONならば
    if (main_work.flag & DMD_STGSLCT_FLAG_CANCEL)
    {
        main_work.proc_menu_update = dmStgSlctProcStageSelectOutEfct;

        DmSoundPlaySE("Cancel");

        main_work.flag &= ~DMD_STGSLCT_FLAG_CANCEL;

        main_work.proc_input = NULL;

        main_work.disp_flag &= ~DMD_STGSLCT_DISP_FLAG_LR_ARROW;

        return;
    }

    // 決定フラグONならば
    if (main_work.flag & DMD_STGSLCT_FLAG_DECIDE)
    {
        //		main_work.proc_menu_update = dmStgSlctProcStageSelectOutEfct;

#if 1
		if (!dmStgSlctIsCanSelectAct(main_work)) {
			main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;
			
#if _IPHONE //アクトセレクトのカバーを外す                 
			main_work.is_disp_cover = false;
#endif //_IPHONE	//アクトセレクトのカバーを外す
			return;
		}
#else
        if (main_work.is_clear_stage[main_work.cur_stage] == -1)
        {
            main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;

#if _IPHONE //アクトセレクトのカバーを外す                 
			main_work.is_disp_cover = false;
#endif //_IPHONE	//アクトセレクトのカバーを外す
            return;
        }
#endif

        DmSoundPlaySE("Ok");

        main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;

        // ACT決定確認ウインドウ状態へ
        main_work.proc_win_update = dmStgSlctProcWindowOpenEfct;

        // ウインドウ遷移関連設定
        main_work.proc_input = NULL;
        main_work.proc_win_input = NULL;
        main_work.win_timer = 0;
        main_work.win_cur_slct = 0;
        main_work.win_mode = DME_STGSLCT_WIN_STG_SLCT;
#if _IPHONE
		main_work.win_is_disp_cover = false;
		main_work.win_timer = -10.5f; //ウインドウの開きを10フレーム遅らす(かつインクリメントで0.0fと通らない様に)
#endif //_IPHONE

        return;
    }

    // ランキング遷移ボタン押下時
    if (main_work.flag & DMD_STGSLCT_FLAG_CHANGE_EVT_RANKING)
    {

        main_work.proc_menu_update = dmStgSlctProcFadeOut;

        // フェードアウト開始
        IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                       , IZE_FADE_TYPE_BLACK_FADEOUT
                       , DMD_STGSLCT_FADEOUT_TIME
                       );

        // ランキングへ遷移イベント設定
        main_work.next_evt = DME_STGSLCT_NEXT_EVT_RANKING;

        // ボタン押下時
        DmSoundPlaySE("Ok");

        // BGMフェードアウト開始
#if defined (GSD_DEBUG_DEMO_SELECT) || defined (GSD_DEBUG_DEMO_SELECT_TOP)
		DmSndBgmPlayerExit();
		main_work.flag |= DMD_STGSLCT_FLAG_DEMO_SND_END;
#endif
        //		DmSoundStopBGM(DMD_STGSLCT_BGM_FADEOUT_TIME);

        // フラグOFF
        main_work.proc_input = NULL;
        main_work.proc_win_input = NULL;
        main_work.win_timer = 0;
        main_work.win_cur_slct = 0;
        main_work.flag &= ~DMD_STGSLCT_FLAG_CHANGE_EVT_RANKING;

        return;
    }

    // ZONE切り替え
    if (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_ZONE)
    {
        main_work.proc_menu_update = dmStgSlctProcStageSelectChngZone;

        // ウインドウ遷移関連設定
        main_work.proc_input = dmStgSlctInputProcStageSelectChngZone;//NULL;

        DmSoundPlaySE("Cursol");

        main_work.timer = 0;
#if _IPHONE
		dmStgSlctStageSelectChngZoneSetInZoneScroll(main_work);
#endif //_IPHONE

        return;
    }

#if !_IPHONE
    if (main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR
        || main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR)
    {
        //	if (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL
        //		|| main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_CRSR) {
        main_work.proc_menu_update = dmStgSlctProcStageSelectChngVrtclAct;

        // ウインドウ遷移関連設定
        main_work.proc_input = dmStgSlctInputProcStageSelectMove;//NULL;

        DmSoundPlaySE("Cursol");

        dmStgSlctSetFocusChangeEfctData(main_work);

        main_work.timer = 0;

        // フラグOFF
        main_work.flag &= ~DMD_STGSLCT_FLAG_UP_CHNG_CRSR;
        main_work.flag &= ~DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR;

        return;
    }
#else //!_IPHONE
	if (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL) {
		main_work.proc_menu_update = dmStgSlctProcStageSelectChngVrtclAct;
		
		// ウインドウ遷移関連設定
		main_work.proc_input = dmStgSlctInputProcStageSelectMove;//NULL;
		
		dmStgSlctSetFocusChangeEfctData(main_work);
		
		main_work.timer = 0;
		
		return;
	}
#endif //!_IPHONE
}



// ==========================================================================
// dmStgSlctProcStageSelectChngZone
/*!
	ステージ選択時のゾーン切り替え演出中処理
 */
// ==========================================================================
void dmStgSlctProcStageSelectChngZone(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 入力処理
    if (main_work.proc_input)
    {
        main_work.proc_input(main_work);
    }

    // 掃け演出が終了したら
    if (dmStgSlctIsStageZoneChangeEfct(main_work))
    {
        // ここの遷移はキャンセルか決定かで分岐する
        main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;

        if (main_work.proc_win_update == dmStgSlctProcWindowNodispIdle)
        {
            main_work.proc_input = dmStgSlctInputProcStageSelect;
        }

        main_work.flag &= ~DMD_STGSLCT_FLAG_ACT_CHNG_ZONE;     // ◆

        return;
    }

    // 掃け演出処理
    dmStgSlctSetStageZoneChangeEfct(main_work);

    // 座標再設定

}



// ==========================================================================
// dmStgSlctProcStageSelectChngVrtclAct
/*!
	ステージ選択時のゾーン切り替え演出中処理
 */
// ==========================================================================
void dmStgSlctProcStageSelectChngVrtclAct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 入力処理
    if (main_work.proc_input)
    {
        main_work.proc_input(main_work);
    }

    if (main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR
        || main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR)
    {

        dmStgSlctSetFocusChangeEfctData(main_work);

        DmSoundPlaySE("Cursol");

        main_work.timer = 0;

        // フラグOFF
        main_work.flag &= ~DMD_STGSLCT_FLAG_UP_CHNG_CRSR;
        main_work.flag &= ~DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR;

        return;
    }

    // 掃け演出が終了したら
    if (!(main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL)
        && !(main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_CRSR))
    {
        // ここの遷移はキャンセルか決定かで分岐する
        main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;

        if (main_work.proc_win_update == dmStgSlctProcWindowNodispIdle)
        {
            main_work.proc_input = dmStgSlctInputProcStageSelect;
        }

        // 座標再設定処理

        main_work.timer = 0;

        return;
    }

    // 掃け演出処理
    if (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL)
    {
        dmStgSlctSetStageVrtclChangeEfct(main_work);

        if (dmStgSlctIsStageVrtclChangeEfct(main_work))
        {
            main_work.flag &= ~DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
        }
    }

    // カーソル移動
    if (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_CRSR)
    {
        dmStgSlctSetStageCrsrChangeEfct(main_work);

        if (dmStgSlctIsStageCrsrChangeEfct(main_work))
        {
            main_work.flag &= ~DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
        }
    }

    // 座標再設定
    main_work.timer++;
}



// ==========================================================================
// dmStgSlctProcStageSelectOutEfct
/*!
	ステージ選択時のステージテーブル掃け演出中処理
 */
// ==========================================================================
void dmStgSlctProcStageSelectOutEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 掃け演出が終了したら
    if (dmStgSlctIsStagePosOutEfct(main_work))
    {
        // ここの遷移はキャンセルか決定かで分岐する
        main_work.proc_menu_update = dmStgSlctProcZoneSelectInEfct;

        // STATEをZONE選択に設定
        main_work.state = DME_STGSLCT_MODE_STATE_ZONE_SLCT;

        // ZONEテーブルの位置を設定
        for (int i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
        {
            if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
            {
                // ZONEテーブルの初期表示位置設定
                main_work.zone_pos[i][0] = dm_stgslct_a_zone_nodisp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_a_zone_nodisp_pos_tbl[i][1];
            }

            else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
            {
                // ZONEテーブルの初期表示位置設定
                main_work.zone_pos[i][0] = dm_stgslct_s_zone_nodisp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_s_zone_nodisp_pos_tbl[i][1];
            }

            else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
            {
                // ZONEテーブルの初期表示位置設定
                main_work.zone_pos[i][0] = dm_stgslct_f_zone_nodisp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_f_zone_nodisp_pos_tbl[i][1];
            }
            else
            {
                // ZONEテーブルの初期表示位置設定
                main_work.zone_pos[i][0] = dm_stgslct_n_zone_nodisp_pos_tbl[i][0];
                main_work.zone_pos[i][1] = dm_stgslct_n_zone_nodisp_pos_tbl[i][1];
            }
        }

        // ZONE選択ではスペステがないため、その分をCLIPする
        //		if (main_work.cur_zone > DME_STGSLCT_ACT_PAGE_FINAL
        //			|| main_work.cur_zone < DME_STGSLCT_ACT_PAGE_1) {
        //			// 範囲外の値の場合は全てZONE１に設定
        //			main_work.cur_zone = DME_STGSLCT_ACT_PAGE_1;
        //		}

        // ZONE選択へ戻ると左上・右上のテキストのACTIONフレームを0に戻す(リセット)
        main_work.mode_tex_move_frm = 0;

        // 初期ZONEスクリーンID設定
        main_work.zone_scr_id = 0;

#if _IPHONE  //ゾーンセレクトのカバーを外す                 
		main_work.is_disp_cover = false;
#endif //_IPHONE	//ゾーンセレクトのカバーを外す

        return;
    }

    // 掃け演出処理
    dmStgSlctSetStagePosOutEfct(main_work);


    // ACT選択に切り替え時は左上・右上のテキストを移動開始
    main_work.mode_tex_move_frm++;

    if (main_work.mode_tex_move_frm > 10)
    {
        main_work.mode_tex_move_frm = 10;
    }


    // 座標再設定

}



// ==========================================================================
// dmStgSlctProcZoneSelectInEfct
/*!
	ゾーン選択時のゾーンテーブル入り演出中処理
 */
// ==========================================================================
void dmStgSlctProcZoneSelectInEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 入り演出が終了したら
    if (dmStgSlctIsZonePosInEfct(main_work))
    {
        main_work.proc_menu_update = dmStgSlctProcZoneSelectIdle;

        // 座標設定(保険)
        for (int i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
        {
            if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
            {
                // ZONEテーブルの初期表示位置設定
                for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_a_zone_disp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_a_zone_disp_pos_tbl[i][1];
                }
            }

            else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
            {
                // ZONEテーブルの初期表示位置設定
                for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_s_zone_disp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_s_zone_disp_pos_tbl[i][1];
                }
            }

            else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
            {
                // ZONEテーブルの初期表示位置設定
                for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_f_zone_disp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_f_zone_disp_pos_tbl[i][1];
                }
            }
            else
            {
                // ZONEテーブルの初期表示位置設定
                for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_n_zone_disp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_n_zone_disp_pos_tbl[i][1];
                }
            }
        }

        if (main_work.proc_win_update == dmStgSlctProcWindowNodispIdle)
        {
            main_work.proc_input = dmStgSlctInputProcZoneSelect;
        }

        return;
    }

    // 入り演出処理
    dmStgSlctSetZonePosInEfct(main_work);

    // 座標再設定

#if _IPHONE
	main_work.cur_zone = u32(-1);
#endif //_IPHONE
}



// ==========================================================================
// dmStgSlctProcWindowNodispIdle
/*!
	ウインドウ非表示待ち中処理
 */
// ==========================================================================
void dmStgSlctProcWindowNodispIdle(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // ウインドウ入力処理
    if (main_work.proc_win_input)
    {
        main_work.proc_win_input(main_work);
    }

    // メニュー遷移フラグONならば
    if (main_work.flag & DMD_STGSLCT_FLAG_DISP_MENU
        || main_work.announce_flag)
    {
        main_work.proc_win_update = dmStgSlctProcWindowOpenEfct;

        // 通常処理の入力処理をなくす(二重入力を防ぐため)
        main_work.proc_input = NULL;

        // ウインドウ開閉演出時は入力処理なし
        main_work.proc_win_input = NULL;

        // ウインドウ演出用タイマー初期化
        main_work.win_timer = 0;

        // ウインドウ選択変数設定
        for (u32 i = DME_STGSLCT_WIN_1_1_CLEAR; i < DME_STGSLCT_WIN_NUM; i++)
        {
            if (main_work.announce_flag & 1 << i)
            {
                main_work.win_mode = (s32)i;
                break;
            }
        }
        //		main_work.win_cur_slct = 0;

        // ウインドウ演出中フラグON
        main_work.flag &= ~DMD_STGSLCT_FLAG_DISP_MENU;
        //		main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT;

        DmSoundPlaySE("Window");
    }


}




// ==========================================================================
// dmStgSlctProcWindowOpenEfct
/*!
	ウインドウオープン中処理
 */
// ==========================================================================
void dmStgSlctProcWindowOpenEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 演出終了チェック
    if (main_work.flag & DMD_STGSLCT_FLAG_WIN_EFCT_END)
    {
        // ウインドウのプロシージャ設定
        main_work.proc_win_update = dmStgSlctProcWindowAnnounceIdle;

        // 入力処理設定
        main_work.proc_win_input = dmStgSlctInputProcWinDispIdle;

#if _IPHONE
		for (AOS_ACTION **act = &main_work.act[ACT_YES_BTN_L], **act_end = &main_work.act[ACT_NO_BTN_R + 1]; act != act_end; ++act) {
			AoActSetFrame(*act, 0.0f);					
		}
		for (er::CTrgAoAction *answer = main_work.trg_answer, *answer_end = main_work.trg_answer + arrayof(main_work.trg_answer); answer != answer_end; ++answer) {
			answer.ResetState();
		}
#endif //_IPHONE

        // ウインドウ内アクション表示フラグON
        main_work.disp_flag |= DMD_STGSLCT_DISP_FLAG_WIN_ACT;

        // ウインドウ演出中フラグOFF
        main_work.flag &= ~DMD_STGSLCT_FLAG_WIN_EFCT_END;
    }
    else
    {
        // ウインドウオープン演出処理
        dmStgSlctSetWinOpenEfct(main_work);
        //		main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT_END;
    }

    // ウインドウ描画
    dmStgSlctWinSelectDraw(main_work);
}



// ==========================================================================
// dmStgSlctProcWindowAnnounceIdle
/*!
	ウインドウ入力待ち処理
 */
// ==========================================================================
void dmStgSlctProcWindowAnnounceIdle(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // ウインドウ入力処理
    if (main_work.proc_win_input)
    {
        main_work.proc_win_input(main_work);
    }

    // ACT決定ウインドウ以外の場合
    if (main_work.win_mode == DME_STGSLCT_WIN_MENU)
    {
        if (main_work.flag & DMD_STGSLCT_FLAG_DECIDE)
        {
            // 通常処理の入力処理をなくす(二重入力を防ぐため)
            main_work.proc_input = NULL;

            // ウインドウ開閉演出時は入力処理なし
            main_work.proc_win_input = NULL;

            // ウインドウ演出時間設定
            main_work.win_timer = DMD_STGSLCT_WIN_EFCT_TIME;

            main_work.proc_menu_update = dmStgSlctProcFadeOut;

            // フェードアウト開始
            IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                           , IZE_FADE_TYPE_BLACK_FADEOUT
                           , DMD_STGSLCT_FADEOUT_TIME
                           );

            // オプション決定時
            if (main_work.win_cur_slct == 0)
            {
                // オプションへ遷移イベント設定
                main_work.next_evt = DME_STGSLCT_NEXT_EVT_RANKING;

                DmSoundPlaySE("Ok");
            }
            // メインメニューへ戻る決定時
            else
            {
                // メインメニューへイベント遷移先設定
                main_work.next_evt = DME_STGSLCT_NEXT_EVT_MAINMENU;

                DmSoundPlaySE("Ok");
            }

            // BGMフェードアウト開始
#if defined (GSD_DEBUG_DEMO_SELECT) || defined (GSD_DEBUG_DEMO_SELECT_TOP)
			DmSndBgmPlayerExit();
			main_work.flag |= DMD_STGSLCT_FLAG_DEMO_SND_END;
#endif
            //			DmSoundStopBGM(DMD_STGSLCT_BGM_FADEOUT_TIME);

            // フラグOFF
            main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;
            main_work.flag &= ~DMD_STGSLCT_FLAG_CANCEL;
        }
        else if (main_work.flag & DMD_STGSLCT_FLAG_CANCEL)
        {
            // 通常処理の入力処理をなくす(二重入力を防ぐため)
            main_work.proc_input = NULL;

            // ウインドウ開閉演出時は入力処理なし
            main_work.proc_win_input = NULL;

            // ウインドウ演出時間設定
            main_work.win_timer = DMD_STGSLCT_WIN_EFCT_TIME;
            //			main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT;

            // ウインドウ内アクション表示フラグOFF
            main_work.disp_flag &= ~DMD_STGSLCT_DISP_FLAG_WIN_ACT;

            main_work.proc_win_update = dmStgSlctProcWindowCloseEfct;

            DmSoundPlaySE("Cancel");

            // フラグOFF
            main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;
            main_work.flag &= ~DMD_STGSLCT_FLAG_CANCEL;
        }
    }
    else if (main_work.win_mode != DME_STGSLCT_WIN_STG_SLCT)
    {
        // メニュー遷移フラグONならば
        if (main_work.flag & DMD_STGSLCT_FLAG_DECIDE
            || main_work.flag & DMD_STGSLCT_FLAG_CANCEL)
        {
            // 通常処理の入力処理をなくす(二重入力を防ぐため)
            main_work.proc_input = NULL;

            // ウインドウ開閉演出時は入力処理なし
            main_work.proc_win_input = NULL;

            // ウインドウ演出時間設定
            main_work.win_timer = DMD_STGSLCT_WIN_EFCT_TIME;
            //			main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT;

            // ウインドウ内アクション表示フラグOFF
            main_work.disp_flag &= ~DMD_STGSLCT_DISP_FLAG_WIN_ACT;

            main_work.proc_win_update = dmStgSlctProcWindowCloseEfct;

            DmSoundPlaySE("Ok");

            // フラグOFF
            main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;
            main_work.flag &= ~DMD_STGSLCT_FLAG_CANCEL;
        }
    }

    // ACT決定ウインドウの場合
    else
    {
        if (main_work.flag & DMD_STGSLCT_FLAG_DECIDE
            && !main_work.win_cur_slct)
        {
            // 通常処理の入力処理をなくす(二重入力を防ぐため)
            main_work.proc_input = NULL;

            // ウインドウ開閉演出時は入力処理なし
            main_work.proc_win_input = NULL;

            // ウインドウ演出時間設定
            main_work.win_timer = DMD_STGSLCT_WIN_EFCT_TIME;
            //			main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT;

            //			main_work.proc_win_update = NULL;
            main_work.proc_menu_update = dmStgSlctProcFadeOut;

            // フェードアウト開始
            IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                           , IZE_FADE_TYPE_BLACK_FADEOUT
                           , DMD_STGSLCT_FADEOUT_TIME
                           );

            // ステージ番号でスペステが通常かをチェック
            if (main_work.cur_zone == DME_STGSLCT_ACT_PAGE_SPE)
            {
                // スペステイベント遷移先設定
                main_work.next_evt = DME_STGSLCT_NEXT_EVT_SPESTE;
            }
            else
            {
                // 通常ACTイベント遷移先設定
                main_work.next_evt = DME_STGSLCT_NEXT_EVT_MAINGAME;
            }

            // BGMフェードアウト開始
            DmSndBgmPlayerExit();
            main_work.flag |= DMD_STGSLCT_FLAG_DEMO_SND_END;

            DmSoundPlaySE("Ok");

            // フラグOFF
            main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;
            main_work.flag &= ~DMD_STGSLCT_FLAG_CANCEL;
        }

        else if (main_work.flag & DMD_STGSLCT_FLAG_DECIDE
            || main_work.flag & DMD_STGSLCT_FLAG_CANCEL)
        {
            // 通常処理の入力処理をなくす(二重入力を防ぐため)
            main_work.proc_input = NULL;

            // ウインドウ開閉演出時は入力処理なし
            main_work.proc_win_input = NULL;

            // ウインドウ演出時間設定
            main_work.win_timer = DMD_STGSLCT_WIN_EFCT_TIME;

            // ウインドウ内アクション表示フラグOFF
            main_work.disp_flag &= ~DMD_STGSLCT_DISP_FLAG_WIN_ACT;

            main_work.proc_win_update = dmStgSlctProcWindowCloseEfct;

            if (main_work.flag & DMD_STGSLCT_FLAG_CANCEL)
            {
                DmSoundPlaySE("Cancel");
            }
            else
            {
                DmSoundPlaySE("Ok");
            }

            // フラグOFF
            main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;
            main_work.flag &= ~DMD_STGSLCT_FLAG_CANCEL;
        }
    }



    // ウインドウ描画
    dmStgSlctWinSelectDraw(main_work);
}



// ==========================================================================
// dmStgSlctProcWindowCloseEfct
/*!
	ウインドウクローズ中処理
 */
// ==========================================================================
void dmStgSlctProcWindowCloseEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 演出終了チェック
    if (main_work.flag & DMD_STGSLCT_FLAG_WIN_EFCT_END)
    {
        // ウインドウのプロシージャ設定
        main_work.proc_win_update = dmStgSlctProcWindowNodispIdle;

        // アナウンス分のフラグOFF
        main_work.announce_flag &= ~(1 << main_work.win_mode);

        if (!main_work.announce_flag)
        {
            if (main_work.state == DME_STGSLCT_MODE_STATE_ZONE_SLCT)
            {
                main_work.proc_input = dmStgSlctInputProcZoneSelect;
            }
            else if (main_work.state == DME_STGSLCT_MODE_STATE_ACT_SLCT)
            {
                main_work.proc_input = dmStgSlctInputProcStageSelect;
#if _IPHONE   //アクトセレクトのカバーを外す                 
				main_work.is_disp_cover = false;
#endif //_IPHONE	//アクトセレクトのカバーを外す
            }
        }

        // ウインドウ演出中フラグOFF
        main_work.flag &= ~DMD_STGSLCT_FLAG_WIN_EFCT_END;
    }

    // ウインドウオープン演出処理
    dmStgSlctSetWinCloseEfct(main_work);
    //	main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT_END;

    // ウインドウ描画
    dmStgSlctWinSelectDraw(main_work);
}



// ==========================================================================
// dmStgSlctProcFadeOut
/*!
	フェードアウト中処理
 */
// ==========================================================================
void dmStgSlctProcFadeOut(DMS_STGSLCT_MAIN_WORK* main_work)
{
    if (IzFadeIsEnd())
    {
        // 遷移先なし
        main_work.proc_win_update = NULL;
        main_work.proc_menu_update = dmStgSlctProcStopDraw;
        main_work.proc_draw = NULL;

        main_work.timer = 0;

        //		DmSoundExit();

        return;
    }
}

// ==========================================================================
// dmStgSlctProcStopDraw
/*!
	描画停止処理
 */
// ==========================================================================
void dmStgSlctProcStopDraw(DMS_STGSLCT_MAIN_WORK* main_work)
{
    main_work.proc_menu_update = dmStgSlctProcDataRelease;
}

// ==========================================================================
// dmStgSlctProcDataRelease
/*!
	ファイル解放リクエスト処理
 */
// ==========================================================================
void dmStgSlctProcDataRelease(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // テクスチャ解放
    for (int i = 0; i < DME_STGSLCT_DATA_TYPE_MAX; i++)
    {
        AoTexRelease(&main_work.tex[i]);
    }

    // メニュー共通テクスチャ解放
    for (int i = 0; i < 5; i++)
    {
        AoTexRelease(&main_work.cmn_tex[i]);
    }

    //	DmSoundFlush();

    // 次へ遷移
    main_work.proc_menu_update = dmStgSlctProcFinish;
}


// ==========================================================================
// dmStgSlctProcFinish
/*!
	終了処理
 */
// ==========================================================================
void dmStgSlctProcFinish(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // テクスチャ解放完了判定
    if (dmStgSlctIsTexRelease(main_work) == 1)
    {
#if _IPHONE //当たり判定解放          
		{
			//ゾーン選択用当たり判定
			for (er::CTrgAoAction *zone = main_work.trg_zone, *zone_end = main_work.trg_zone + arrayof(main_work.trg_zone); zone != zone_end; ++zone) {
				zone.Release();
				zone.~CTrgAoAction();
			}
			//アクト選択用当たり判定
			for (er::CTrgAoAction *act = main_work.trg_act, *act_end = main_work.trg_act + arrayof(main_work.trg_act); act != act_end; ++act) {
				act.Release();
				act.~CTrgAoAction();
			}
			//アクト選択時のゾーンタブ用当たり判定
			for (er::CTrgAoAction *tab = main_work.trg_act_tab, *tab_end = main_work.trg_act_tab + arrayof(main_work.trg_act_tab); tab != tab_end; ++tab) {
				tab.Release();
				tab.~CTrgAoAction();
			}
			//アクト選択時のLR用当たり判定
			for (er::CTrgAoAction *lr = main_work.trg_act_lr, *lr_end = main_work.trg_act_lr + arrayof(main_work.trg_act_lr); lr != lr_end; ++lr) {
				lr.Release();
				lr.~CTrgAoAction();
			}
			{	//アクト選択時の上下ドラッグ用、左右フリック用当たり判定
				er::CTrgFlick &move = main_work.trg_act_move;
				move.Release();
				move.~CTrgFlick();
			}
			//アクト選択時のモード切り替え用当たり判定
			for (er::CTrgAoAction *mode = main_work.trg_mode, *mode_end = main_work.trg_mode + arrayof(main_work.trg_mode); mode != mode_end; ++mode) {
				mode.Release();
				mode.~CTrgAoAction();
			}
			{	//キャンセル用当たり判定
				er::CTrgAoAction &cancel = main_work.trg_cancel;
				cancel.Release();
				cancel.~CTrgAoAction();
			}
			//アクト選択時のモード切り替え用当たり判定
			for (er::CTrgAoAction *answer = main_work.trg_answer, *answer_end = main_work.trg_answer + arrayof(main_work.trg_answer); answer != answer_end; ++answer) {
				answer.Release();
				answer.~CTrgAoAction();
			}
		}
#endif //_IPHONE	//当たり判定解放

        for (int i = 0; i < ACT_NUM; i++)
        {
            if (main_work.act[i])
            {
                AoActDelete(main_work.act[i]);
                main_work.act[i] = NULL;
            }
        }

        // AMB解放
        for (int i = 0; i < 2; i++)
        {
            // ファイル解放
            if (main_work.arc_amb[i])
            {
                amMemFree(main_work.arc_amb[i]);
                main_work.arc_amb[i] = NULL;
            }
        }

        // アクション解放
        for (int i = 0; i < 5; i++)
        {
            // ファイル解放
            if (main_work.arc_cmn_amb[i])
            {
                amMemFree(main_work.arc_cmn_amb[i]);
                main_work.arc_cmn_amb[i] = NULL;
            }
        }

        // 終了処理へ
        //		main_work.flag |= DMD_STGSLCT_FLAG_EXIT;
        main_work.proc_win_update = NULL;
        main_work.proc_menu_update = dmStgSlctProcWaitFinished;
    }
}



// ==========================================================================
// dmStgSlctProcWaitFinished
/*!
	終了処理
 */
// ==========================================================================
void dmStgSlctProcWaitFinished(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // サウンド終了フラグONならば
    if (main_work.flag & DMD_STGSLCT_FLAG_DEMO_SND_END)
    {
        if (DmSndBgmPlayerIsTaskExit())
        {
            // 終了処理へ
            main_work.flag |= DMD_STGSLCT_FLAG_EXIT;
            main_work.proc_win_update = NULL;
            main_work.proc_menu_update = NULL;

            main_work.flag &= ~DMD_STGSLCT_FLAG_DEMO_SND_END;
        }
    }
    else
    {
        // 終了処理へ
        main_work.flag |= DMD_STGSLCT_FLAG_EXIT;
        main_work.proc_win_update = NULL;
        main_work.proc_menu_update = NULL;
    }
}



// ==========================================================================
// dmStgSlctInputProcZoneSelect
/*!
	ZONE選択用入力プロシージャ処理(据え置き版)
 */
// ==========================================================================
void dmStgSlctInputProcZoneSelect(DMS_STGSLCT_MAIN_WORK* main_work)
{
    u32 tmp_cur_zone = main_work.cur_zone;

    // キャンセル処理
#if !_IPHONE
    if (AoPadStand() & GSD_KEY_CANCEL)
    {
#else //!_IPHONE
	if (main_work.trg_cancel.GetState(0)[er::CTrgState::EState::Up] && main_work.trg_cancel.GetState(0)[er::CTrgState::EState::Prev]) {
#endif //!_IPHONE
        main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;

        return;
    }

#if !_IPHONE
    // ステージ選択への遷移処理
    if (AoPadStand() & GSD_KEY_DECIDE)
    {
        main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;

        return;
    }
#endif //!_IPHONE

    // ゲームモード切替
#if 0
	if (AoPadStand() & KEY_R_LEFT) {
		main_work.cur_game_mode ^= DME_STGSLCT_PLAY_MODE_TIME_ATK;
		
		DmSoundPlaySE("Cursol");
		
		return;
	}
#endif


#if !_IPHONE
    // 十字キー操作
    if (AoPadMStand() & GSD_KEY_UP)
    {
        // 選択ZONE切り替え
        if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
        {
            main_work.cur_zone = (u32)dm_stgslct_a_zone_input_dir_tbl[tmp_cur_zone][0];
        }
        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
        {
            main_work.cur_zone = (u32)dm_stgslct_s_zone_input_dir_tbl[tmp_cur_zone][0];
        }
        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
        {
            main_work.cur_zone = (u32)dm_stgslct_f_zone_input_dir_tbl[tmp_cur_zone][0];
        }
        else
        {
            main_work.cur_zone = (u32)dm_stgslct_n_zone_input_dir_tbl[tmp_cur_zone][0];
        }

        if (main_work.cur_zone != DME_STGSLCT_ZONE_TYPE_NONE)
        {
            DmSoundPlaySE("Cursol");
            main_work.zone_scr_id = 0;
        }
        // 切り替えフラグ？
    }
    else if (AoPadMStand() & GSD_KEY_DOWN)
    {
        // 選択ZONE切り替え
        if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
        {
            main_work.cur_zone = (u32)dm_stgslct_a_zone_input_dir_tbl[tmp_cur_zone][1];
        }
        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
        {
            main_work.cur_zone = (u32)dm_stgslct_s_zone_input_dir_tbl[tmp_cur_zone][1];
        }
        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
        {
            main_work.cur_zone = (u32)dm_stgslct_f_zone_input_dir_tbl[tmp_cur_zone][1];
        }
        else
        {
            main_work.cur_zone = (u32)dm_stgslct_n_zone_input_dir_tbl[tmp_cur_zone][1];
        }

        if (main_work.cur_zone != DME_STGSLCT_ZONE_TYPE_NONE)
        {
            DmSoundPlaySE("Cursol");
            main_work.zone_scr_id = 0;
        }
        //		main_work.cur_zone = dm_stgslct_zone_input_dir_tbl[tmp_cur_zone][1];
    }
    else if (AoPadMStand() & GSD_KEY_LEFT)
    {
        // 選択ZONE切り替え
        if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
        {
            main_work.cur_zone = (u32)dm_stgslct_a_zone_input_dir_tbl[tmp_cur_zone][2];
        }
        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
        {
            main_work.cur_zone = (u32)dm_stgslct_s_zone_input_dir_tbl[tmp_cur_zone][2];
        }
        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
        {
            main_work.cur_zone = (u32)dm_stgslct_f_zone_input_dir_tbl[tmp_cur_zone][2];
        }
        else
        {
            main_work.cur_zone = (u32)dm_stgslct_n_zone_input_dir_tbl[tmp_cur_zone][2];
        }

        if (main_work.cur_zone != DME_STGSLCT_ZONE_TYPE_NONE)
        {
            DmSoundPlaySE("Cursol");
            main_work.zone_scr_id = 0;
        }
        //		main_work.cur_zone = dm_stgslct_zone_input_dir_tbl[tmp_cur_zone][2];
    }
    else if (AoPadMStand() & GSD_KEY_RIGHT)
    {
        // 選択ZONE切り替え
        if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
        {
            main_work.cur_zone = (u32)dm_stgslct_a_zone_input_dir_tbl[tmp_cur_zone][3];
        }
        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
        {
            main_work.cur_zone = (u32)dm_stgslct_s_zone_input_dir_tbl[tmp_cur_zone][3];
        }
        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
        {
            main_work.cur_zone = (u32)dm_stgslct_f_zone_input_dir_tbl[tmp_cur_zone][3];
        }
        else
        {
            main_work.cur_zone = (u32)dm_stgslct_n_zone_input_dir_tbl[tmp_cur_zone][3];
        }

        if (main_work.cur_zone != DME_STGSLCT_ZONE_TYPE_NONE)
        {
            DmSoundPlaySE("Cursol");
            main_work.zone_scr_id = 0;
        }
        //		main_work.cur_zone = dm_stgslct_zone_input_dir_tbl[tmp_cur_zone][3];
    }
#else //!_IPHONE
	for (int i = 0; i < arrayof(main_work.trg_zone); ++i) {
		const DME_STGSLCT_ZONE_TYPE c_zone_table[] = {	DME_STGSLCT_ZONE_TYPE_1
													,	DME_STGSLCT_ZONE_TYPE_2
													,	DME_STGSLCT_ZONE_TYPE_3
													,	DME_STGSLCT_ZONE_TYPE_4
													,	DME_STGSLCT_ZONE_TYPE_FINAL
													,	DME_STGSLCT_ZONE_TYPE_SPE
													};
		er::CTrgAoAction &zone = main_work.trg_zone[i];
		if (zone.GetState(0)[er::CTrgState::EState::Up] && zone.GetState(0)[er::CTrgState::EState::Prev]) {
			//選択
			main_work.cur_zone = c_zone_table[i];
			main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;
			break;
		} else if (zone.GetState(0)[er::CTrgState::EState::Stand]) {
			//タップ中
			main_work.cur_zone = c_zone_table[i];
			main_work.is_disp_cover = true;
			break;
		} else if (zone.GetState(0)[er::CTrgState::EState::Out]) {
			//タップキャンセル
			main_work.cur_zone = u32(-1);
			main_work.is_disp_cover = false;
		}
	}
#endif //!_IPHONE

    if (main_work.cur_zone == DME_STGSLCT_ZONE_TYPE_NONE)
    {
        main_work.cur_zone = tmp_cur_zone;
    }
}



// ==========================================================================
// dmStgSlctInputProcStageSelect
/*!
	STAGE選択用入力プロシージャ処理(据え置き版)
 */
// ==========================================================================
void dmStgSlctInputProcStageSelect(DMS_STGSLCT_MAIN_WORK* main_work)
{
    u32 src = 0;
    u32 dst = 0;
    u32 act_comp = 0;
    s32 is_final_open = 0;
    s32 is_spe_open = 0;
    s32 edge = 0;

    src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];
    act_comp = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] - 1;

    edge = DME_STGSLCT_ACT_PAGE_4;

    // FINALオープン状態かどうか
    if (main_work.is_clear_stage[DME_STGSLCT_STAGE_1_B] == 1
        && main_work.is_clear_stage[DME_STGSLCT_STAGE_2_B] == 1
        && main_work.is_clear_stage[DME_STGSLCT_STAGE_3_B] == 1
        && main_work.is_clear_stage[DME_STGSLCT_STAGE_4_B] == 1)
    {
        is_final_open = 1;
        edge = DME_STGSLCT_ACT_PAGE_FINAL;
    }

    // スペステオープン状態かどうか
    for (u32 i = DME_STGSLCT_STAGE_S_1; i < DME_STGSLCT_STAGE_NUM; i++)
    {
        if (main_work.is_clear_stage[i] != -1)
        {
            is_spe_open = 1;
            edge = DME_STGSLCT_ACT_PAGE_SPE;
            break;
        }
        else
        {
            is_spe_open = 0;
        }
    }

    // キャンセル処理
#if !_IPHONE
    if (AoPadStand() & GSD_KEY_CANCEL)
    {
#else //!_IPHONE
	if (!main_work.trg_act_move.GetState(0)[er::CTrgState::EState::DragAndDrop]
				&& main_work.trg_cancel.GetState(0)[er::CTrgState::EState::Lock]
				&& main_work.trg_cancel.GetState(0)[er::CTrgState::EState::Up]
				&& main_work.trg_cancel.GetState(0)[er::CTrgState::EState::Prev]) {
#endif //!_IPHONE
        main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;

        return;
    }

    // ステージ決定処理
#if !_IPHONE
    if (AoPadStand() & GSD_KEY_DECIDE)
    {
        main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;

        return;
    }
#endif //!_IPHONE

    // ゲームモード切替
#if !_IPHONE
    if (AoPadStand() & KEY_R_LEFT)
    {
        main_work.cur_game_mode ^= DME_STGSLCT_PLAY_MODE_TIME_ATK;

        // ゲームモードテキストアクションのフレーム初期化(アニメーション開始)
        main_work.mode_tex_frm = 1;

        DmSoundPlaySE("Cursol");

        return;
    }
#else //!_IPHONE
	{
		f32 frame;
		if (!main_work.trg_act_move.GetState(0)[er::CTrgState::EState::DragAndDrop] && (main_work.trg_mode[0].GetState(0)[er::CTrgState::EState::Lock] || main_work.trg_mode[1].GetState(0)[er::CTrgState::EState::Lock])) {
			//ロック中なら
			if (main_work.trg_mode[0].GetState(0)[er::CTrgState::EState::Up] && main_work.trg_mode[0].GetState(0)[er::CTrgState::EState::Prev]) {
				//決定
				frame = 2.0f;
			} else if (main_work.trg_mode[1].GetState(0)[er::CTrgState::EState::Up] && main_work.trg_mode[1].GetState(0)[er::CTrgState::EState::Prev]) {
				//決定
				frame = 2.0f;
			} else if (main_work.trg_mode[0].GetState(0)[er::CTrgState::EState::On] || main_work.trg_mode[1].GetState(0)[er::CTrgState::EState::On]) {
				//ON
				frame = 1.0f;
			} else {
				//OFF
				frame = 0.0f;
			}
		} else if (2.0f < main_work.act[ACT_TAB_STATE_L].frame) {
			//決定演出中
			frame = -1.0f;
		} else {
			//OFF
			frame = 0.0f;
		}
		if (0.0f <= frame) {
			if (2.0f == frame) {
				//決定なら
				main_work.cur_game_mode ^= DME_STGSLCT_PLAY_MODE_TIME_ATK;
				DmSoundPlaySE("Cursol");
			}
			for (AOS_ACTION **act = &main_work.act[ACT_TAB_STATE_L], **act_end = &main_work.act[ACT_TAB_STATE_R2 + 1]; act != act_end; ++act) {
				AoActSetFrame(*act, frame);
			}
			for (AOS_ACTION **act = &main_work.act[ACT_TEX_BIG_TIME], **act_end = &main_work.act[ACT_TEX_BIG_SCORE + 1]; act != act_end; ++act) {
				AoActSetFrame(*act, frame);
			}
		}
	}
#endif //!_IPHONE


#if !_IPHONE
    // ランキング切り替え
    dmStgSlctInputChangeEvtRanking(main_work);
#endif //!_IPHONE

#if _IPHONE
	//アクト枠クリック
	u32 act_num = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];
	if (3 < act_num) {
		//アクト数が3を超える場合は3でリミットさせる(タッチ領域がモード切り換えのタッチと被る為)
		act_num = 3;
	}
	for (u32 i = main_work.focus_disp_no, max = i + act_num; i < max; ++i) {
		er::CTrgAoAction &act = main_work.trg_act[i];
		if (act.GetState(0)[er::CTrgState::EState::Down]) {
			main_work.cur_stage = (s32)(src + i);
			main_work.is_disp_cover = true;
		}			
		if (act.GetState(0)[er::CTrgState::EState::Out]) {
			main_work.is_disp_cover = false;
		}			
		if (act.GetState(0)[er::CTrgState::EState::Click]) {
			main_work.cur_stage = (s32)(src + i);
			main_work.is_disp_cover = true;
			main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;
			return;
		}			
	}
	//ゾーン移動用変数
	bool is_move_left = false;
	bool is_move_right = false;
	bool is_move_drag = false;
	s32 &is_open = main_work.is_final_open;
	//ゾーンタブスタンド
	for (int i = 0, max = arrayof(main_work.trg_act_tab); i < max; ++i) {
		int zone_index = i;
		if ((DMD_STGSLCT_ZONE_SPECIAL_OPEN & is_open) && !(DMD_STGSLCT_ZONE_FINAL_OPEN & is_open)) {
			// FINALが開いてない状態でスペステが開いた状態の場合の補正処理
			switch (i) {
			case DME_STGSLCT_ZONE_TYPE_FINAL: //ファイナル
				//ファイナルの当たりはスペステとして判定する
				zone_index = DME_STGSLCT_ZONE_TYPE_SPE;
				break;
			case DME_STGSLCT_ZONE_TYPE_SPE: //スペステ
				//スペステの当たり判定を取らない
				continue;
				break;
			default:
				break;
			}
		}
		er::CTrgAoAction &act = main_work.trg_act_tab[i];
		if (act.GetState(0)[er::CTrgState::EState::Down]) {
			if (main_work.cur_zone != zone_index) {
				main_work.chng_zone = main_work.cur_zone;
				main_work.cur_zone = zone_index;
				((main_work.chng_zone < main_work.cur_zone)? is_move_right: is_move_left) = true;
			}
		}			
	}
	//ゾーンLRボタンリピート
	for (int i = 0, max = arrayof(main_work.trg_act_lr); i < max; ++i) {
		er::CTrgAoAction &lr = main_work.trg_act_lr[i];

		if (!main_work.trg_act_move.GetState(0)[er::CTrgState::EState::DragAndDrop]
									&& lr.GetState(0)[er::CTrgState::EState::Repeat]
									&& lr.GetState(0)[er::CTrgState::EState::Lock]) {
			main_work.chng_zone = main_work.cur_zone;
			main_work.cur_zone = (u32)dmStgSlctGetRevisedZoneNo((s32)main_work.cur_zone
																, ((0 == i)? -1: 1)
																, DMD_STGSLCT_ZONE_FINAL_OPEN & is_open
																, DMD_STGSLCT_ZONE_SPECIAL_OPEN & is_open
																);
			((0 == i)? is_move_left: is_move_right) = true;
		}
	}
	//ムーブエリア
	{
		const er::CTrgStateEx &state = main_work.trg_act_move.GetState(0);
		er::CTrgStateEx::TDragSpeed spd = state.GetDragSpeed();
		if ((3.0f <= std::abs(spd.x())) && (std::abs(spd.y()) < std::abs(spd.x()))) {
			//左右フリック
			((0 <= spd.x())? is_move_left: is_move_right) = true;
			//右にフリックするとカーソルは左に動く
			main_work.chng_zone = main_work.cur_zone;
			main_work.cur_zone = (u32)dmStgSlctGetRevisedZoneNo((s32)main_work.cur_zone
																, ((is_move_left)? 1: -1)
																, DMD_STGSLCT_ZONE_FINAL_OPEN & is_open
																, DMD_STGSLCT_ZONE_SPECIAL_OPEN & is_open
																);
		} else if (state[er::CTrgState::EState::Up]) {
			//上下フリック
			if (state[er::CTrgState::EState::DragAndDrop]) {
				//ドラッグ中なら

				//上にフリックするとカーソルは下に動く
				s32 move_min = 0;
				s32 move_max;
				switch (main_work.cur_zone) {
				case DME_STGSLCT_ZONE_TYPE_FINAL: //ファイナルステージ
					move_max = 0;
					break;
				case DME_STGSLCT_ZONE_TYPE_SPE: //スペシャルステージ
					move_max = 4;
					break;
				default:
					move_max = 1;
					break;
				}
				s32 focus_disp_no = (static_cast<s32>(dm_stgslct_act_disp_y_pos_tbl[0]) - main_work.act_top_pos_y[src] + (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) / 2);
				focus_disp_no -= static_cast<s32>(spd.y() * static_cast<float>(DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) * 0.25f);
				focus_disp_no /= (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8);
				focus_disp_no = amClamp(focus_disp_no, move_min, move_max);
				main_work.prev_disp_no = main_work.focus_disp_no;
				main_work.focus_disp_no = focus_disp_no;
				main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
			}
		} else if (state[er::CTrgState::EState::Move]) {
			//上下ドラッグ
			er::CTrgState::TMove move = state.GetLastMove();
			if ((move.y() < 0) || (0 < move.y())) {
				for (int i = src; i < dst; ++i) {
					main_work.act_top_pos_y[i] += move.y() * 2;
				}
				is_move_drag = true;
			}
		}
	}
	//移動が有ると、アクト決定が発動しない様にトリガのロックを切る
	if (is_move_left || is_move_right || is_move_drag || (DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL & main_work.flag)) {
		for (er::CTrgAoAction *act = main_work.trg_act, *act_end = main_work.trg_act + arrayof(main_work.trg_act); act != act_end; ++act) {
			act.DelLock();
		}
		main_work.is_disp_cover = false;
	}
#endif //_IPHONE

    // 十字キー操作
#if !_IPHONE
    if (AoPadMRepeat() & GSD_KEY_UP)
    {
        if (AoPadMStand() & GSD_KEY_UP
            || main_work.cur_stage != (s32)src)
        {
#else //!_IPHONE
	if (false) {{
#endif //!_IPHONE

            if (main_work.cur_zone == DME_STGSLCT_ACT_PAGE_FINAL)
            {
                return;
            }

            // 選択STAGE切り替え
            if (main_work.crsr_idx == 0)
            {
                if (act_comp > DMD_STGSLCT_ACT_VRTCL_CHNG_NUM)
                {
                    // テーブル移動演出フラグON
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
                }

                else
                {
                    // カーソル移動フラグON
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
                }
            }

            else
            {
                // カーソル移動フラグON
                main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
            }


            main_work.flag |= DMD_STGSLCT_FLAG_UP_CHNG_CRSR;

        }

        return;
    }

#if !_IPHONE
    else if (AoPadMRepeat() & GSD_KEY_DOWN)
    {
        if (AoPadMStand() & GSD_KEY_DOWN
            || main_work.cur_stage != (s32)(dst - 1))
        {
#else //!_IPHONE
	else if (false) {{
#endif //!_IPHONE

            if (main_work.cur_zone == DME_STGSLCT_ACT_PAGE_FINAL)
            {
                return;
            }

            // 選択STAGE切り替え
            if (main_work.crsr_idx == 3)
            {
                if (act_comp > DMD_STGSLCT_ACT_VRTCL_CHNG_NUM)
                {
                    // テーブル移動演出フラグON
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
                }
                else
                {
                    // カーソル移動フラグON
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
                }
            }

            else
            {
                // カーソル移動フラグON
                main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
            }

            main_work.flag |= DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR;
        }

        return;
    }

#if !_IPHONE
#if !_WII
    else if ((AoPadMRepeat() & GSD_KEY_LEFT) || (AoPadMRepeat() & KEY_L1))
    {
        if (((AoPadMStand() & GSD_KEY_LEFT) || (AoPadMStand() & KEY_L1))
#else
	else if (AoPadMRepeat() & GSD_KEY_LEFT) {
		if (AoPadMStand() & GSD_KEY_LEFT
#endif
            || main_work.cur_zone != DME_STGSLCT_ACT_PAGE_1)
        {
            // 選択ZONE切り替え
            main_work.chng_zone = main_work.cur_zone;
            main_work.cur_zone = (u32)dmStgSlctGetRevisedZoneNo((s32)main_work.cur_zone
                                                                , -1
                                                                , is_final_open
                                                                , is_spe_open
                                                                );
#else //!_IPHONE
	else if (is_move_left) {{
#endif //!_IPHONE

            src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
            dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];

            main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_ZONE;
            main_work.act_move_dest[0] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
            main_work.act_move_dest[1] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;

            // 選択中のステージ切り替え
            if (main_work.cur_zone == DME_STGSLCT_ACT_PAGE_FINAL)
            {
                main_work.crsr_pos_y = 320.f;
                main_work.crsr_idx = 0;
            }

            if (main_work.chng_zone == DME_STGSLCT_ACT_PAGE_FINAL)
            {
                main_work.crsr_pos_y = 160.f;
                main_work.crsr_idx = 0;
            }

            main_work.prev_disp_no = main_work.focus_disp_no;
            main_work.focus_disp_no = 0;

            main_work.cur_stage = (s32)(main_work.crsr_idx + main_work.focus_disp_no + src);

            // 移動時の座標設定
            dmStgSlctSetActChngZonePosInit(main_work, -1);

            DmSoundPlaySE("Cursol");

            if (AoPadMRepeat() & KEY_L1
                || AoPadMStand() & KEY_L1)
            {
                main_work.flag |= DMD_STGSLCT_FLAG_ACT_PUSH_L_BTN;
                main_work.btn_l_disp_frm = 0;
            }


            main_work.timer = 0;
        }

        return;
    }


#if !_IPHONE
#if !_WII
    else if ((AoPadMRepeat() & GSD_KEY_RIGHT) || (AoPadMRepeat() & KEY_R1))
    {
        if (((AoPadMStand() & GSD_KEY_RIGHT) || (AoPadMStand() & KEY_R1))
#else
	else if (AoPadMRepeat() & GSD_KEY_RIGHT) {
		if (AoPadMStand() & GSD_KEY_RIGHT
#endif
            || main_work.cur_zone != (u32)edge)
        {
            // 選択ZONE切り替え
            main_work.chng_zone = main_work.cur_zone;
            main_work.cur_zone = (u32)dmStgSlctGetRevisedZoneNo((s32)main_work.cur_zone
                                                                , 1
                                                                , is_final_open
                                                                , is_spe_open
                                                                );
#else //!_IPHONE
	else if (is_move_right) {{
		// 選択ZONE切り替え
#endif //!_IPHONE

            src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
            dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];

            main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_ZONE;
            main_work.act_move_dest[0] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X * (-1);
            main_work.act_move_dest[1] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;

            // 選択中のステージ切り替え
            if (main_work.cur_zone == DME_STGSLCT_ACT_PAGE_FINAL)
            {
                main_work.crsr_pos_y = 320.f;
                main_work.crsr_idx = 0;
            }

            if (main_work.chng_zone == DME_STGSLCT_ACT_PAGE_FINAL)
            {
                main_work.crsr_pos_y = 160.f;
                main_work.crsr_idx = 0;
            }

            main_work.prev_disp_no = main_work.focus_disp_no;
            main_work.focus_disp_no = 0;

            main_work.cur_stage = (s32)(main_work.crsr_idx + main_work.focus_disp_no + src);

            // 移動時の座標設定
            dmStgSlctSetActChngZonePosInit(main_work, 1);

            DmSoundPlaySE("Cursol");

            if (AoPadMRepeat() & KEY_R1
                || AoPadMStand() & KEY_R1)
            {
                main_work.flag |= DMD_STGSLCT_FLAG_ACT_PUSH_R_BTN;
                main_work.btn_r_disp_frm = 0;
            }


            main_work.timer = 0;
        }

        return;
    }
}



// ==========================================================================
// dmStgSlctInputProcStageSelectChngZone
/*!
	STAGE選択用入力プロシージャ処理(据え置き版)
 */
// ==========================================================================
void dmStgSlctInputProcStageSelectChngZone(DMS_STGSLCT_MAIN_WORK* main_work)
{
    u32 src = 0;
    u32 dst = 0;
    u32 act_comp = 0;
    s32 is_final_open = 0;
    s32 is_spe_open = 0;
    s32 edge = 0;

    src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];
    act_comp = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] - 1;

    edge = DME_STGSLCT_ACT_PAGE_4;

    // FINALオープン状態かどうか
    if (main_work.is_clear_stage[DME_STGSLCT_STAGE_1_B] == 1
        && main_work.is_clear_stage[DME_STGSLCT_STAGE_2_B] == 1
        && main_work.is_clear_stage[DME_STGSLCT_STAGE_3_B] == 1
        && main_work.is_clear_stage[DME_STGSLCT_STAGE_4_B] == 1)
    {
        is_final_open = 1;
        edge = DME_STGSLCT_ACT_PAGE_FINAL;
    }

    // スペステオープン状態かどうか
    for (u32 i = DME_STGSLCT_STAGE_S_1; i < DME_STGSLCT_STAGE_NUM; i++)
    {
        if (main_work.is_clear_stage[i] != -1)
        {
            is_spe_open = 1;
            edge = DME_STGSLCT_ACT_PAGE_SPE;
            break;
        }
        else
        {
            is_spe_open = 0;
        }
    }

    // キャンセル処理
    if (AoPadStand() & GSD_KEY_CANCEL)
    {
        main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;

        return;
    }

    // ステージ決定処理
    if (AoPadStand() & GSD_KEY_DECIDE)
    {
        main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;

        return;
    }

    // ゲームモード切替
#if 1
	if (AoPadStand() & KEY_R_LEFT) {
		main_work.cur_game_mode ^= DME_STGSLCT_PLAY_MODE_TIME_ATK;
		
		// ゲームモードテキストアクションのフレーム初期化(アニメーション開始)
		main_work.mode_tex_frm = 1;
		
		DmSoundPlaySE("Cursol");
		
		return;
	}
#endif

    // ランキング切り替え
    dmStgSlctInputChangeEvtRanking(main_work);


#if !_WII
    if ((AoPadMRepeat() & GSD_KEY_LEFT) || (AoPadMRepeat() & KEY_L1))
    {
        if (((AoPadMStand() & GSD_KEY_LEFT) || (AoPadMStand() & KEY_L1))
#else
	if (AoPadMRepeat() & GSD_KEY_LEFT) {
		if (AoPadMStand() & GSD_KEY_LEFT
#endif

            || main_work.cur_zone != DME_STGSLCT_ACT_PAGE_1)
        {
            // 選択ZONE切り替え
            main_work.chng_zone = main_work.cur_zone;
            main_work.cur_zone = (u32)dmStgSlctGetRevisedZoneNo((s32)main_work.cur_zone
                                                                , -1
                                                                , is_final_open
                                                                , is_spe_open
                                                                );

            src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
            dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];

            main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_ZONE;
            main_work.act_move_dest[0] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
            main_work.act_move_dest[1] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;

            // 選択中のステージ切り替え
            if (main_work.cur_zone == DME_STGSLCT_ACT_PAGE_FINAL)
            {
                main_work.crsr_pos_y = 320.f;
                main_work.crsr_idx = 0;
            }

            if (main_work.chng_zone == DME_STGSLCT_ACT_PAGE_FINAL)
            {
                main_work.crsr_pos_y = 160.f;
                main_work.crsr_idx = 0;
            }

            main_work.prev_disp_no = main_work.focus_disp_no;
            main_work.focus_disp_no = 0;

            main_work.cur_stage = (s32)(main_work.crsr_idx + main_work.focus_disp_no + src);

            // 移動時の座標設定
            dmStgSlctSetActChngZonePosInit(main_work, -1);

            DmSoundPlaySE("Cursol");

            if (AoPadMRepeat() & KEY_L1
                || AoPadMStand() & KEY_L1)
            {
                main_work.flag |= DMD_STGSLCT_FLAG_ACT_PUSH_L_BTN;
                main_work.btn_l_disp_frm = 0;
            }

            main_work.timer = 0;
        }

        return;
    }


#if !_WII
    else if ((AoPadMRepeat() & GSD_KEY_RIGHT) || (AoPadMRepeat() & KEY_R1))
    {
        if (((AoPadMStand() & GSD_KEY_RIGHT) || (AoPadMStand() & KEY_R1))
#else
	else if (AoPadMRepeat() & GSD_KEY_RIGHT) {
		if (AoPadMStand() & GSD_KEY_RIGHT
#endif
            || main_work.cur_zone != (u32)edge)
        {
            // 選択ZONE切り替え
            main_work.chng_zone = main_work.cur_zone;
            main_work.cur_zone = (u32)dmStgSlctGetRevisedZoneNo((s32)main_work.cur_zone
                                                                , 1
                                                                , is_final_open
                                                                , is_spe_open
                                                                );

            src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
            dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];

            main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_ZONE;
            main_work.act_move_dest[0] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X * (-1);
            main_work.act_move_dest[1] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;

            // 選択中のステージ切り替え
            if (main_work.cur_zone == DME_STGSLCT_ACT_PAGE_FINAL)
            {
                main_work.crsr_pos_y = 320.f;
                main_work.crsr_idx = 0;
            }

            if (main_work.chng_zone == DME_STGSLCT_ACT_PAGE_FINAL)
            {
                main_work.crsr_pos_y = 160.f;
                main_work.crsr_idx = 0;
            }

            main_work.prev_disp_no = main_work.focus_disp_no;
            main_work.focus_disp_no = 0;

            main_work.cur_stage = (s32)(main_work.crsr_idx + main_work.focus_disp_no + src);

            // 移動時の座標設定
            dmStgSlctSetActChngZonePosInit(main_work, 1);

            DmSoundPlaySE("Cursol");

            if (AoPadMRepeat() & KEY_R1
                || AoPadMStand() & KEY_R1)
            {
                main_work.flag |= DMD_STGSLCT_FLAG_ACT_PUSH_R_BTN;
                main_work.btn_r_disp_frm = 0;
            }

            main_work.timer = 0;
        }

        return;
    }
}




// ==========================================================================
// dmStgSlctInputProcStageSelectMove
/*!
	STAGE選択用入力プロシージャ処理(据え置き版)
 */
// ==========================================================================
void dmStgSlctInputProcStageSelectMove(DMS_STGSLCT_MAIN_WORK* main_work)
{
    u32 src = 0;
    u32 dst = 0;
    u32 act_comp = 0;

    src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];
    act_comp = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] - 1;

    // キャンセル処理
    if (AoPadStand() & GSD_KEY_CANCEL)
    {
        main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;

        return;
    }

    // ステージ決定処理
    if (AoPadStand() & GSD_KEY_DECIDE)
    {
        main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;

        return;
    }

    // ゲームモード切替
#if 1
	if (AoPadStand() & KEY_R_LEFT) {
		main_work.cur_game_mode ^= DME_STGSLCT_PLAY_MODE_TIME_ATK;
		
		// ゲームモードテキストアクションのフレーム初期化(アニメーション開始)
		main_work.mode_tex_frm = 1;
		
		DmSoundPlaySE("Cursol");
		
		return;
	}
#endif

    // ランキング切り替え
    dmStgSlctInputChangeEvtRanking(main_work);


    // 十字キー操作
    if (AoPadMRepeat() & GSD_KEY_UP)
    {
        if (AoPadMStand() & GSD_KEY_UP
            || main_work.cur_stage != (s32)src)
        {

            if (main_work.cur_zone == DME_STGSLCT_ACT_PAGE_FINAL)
            {
                return;
            }

            // 選択STAGE切り替え
            if (main_work.crsr_idx == 0)
            {
                if (act_comp > DMD_STGSLCT_ACT_VRTCL_CHNG_NUM)
                {
                    // テーブル移動演出フラグON
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
                }

                else
                {
                    // カーソル移動フラグON
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
                }
            }

            else
            {
                // カーソル移動フラグON
                main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
            }


            main_work.flag |= DMD_STGSLCT_FLAG_UP_CHNG_CRSR;

        }

        return;
    }

    else if (AoPadMRepeat() & GSD_KEY_DOWN)
    {
        if (AoPadMStand() & GSD_KEY_DOWN
            || main_work.cur_stage != (s32)(dst - 1))
        {

            if (main_work.cur_zone == DME_STGSLCT_ACT_PAGE_FINAL)
            {
                return;
            }

            // 選択STAGE切り替え
            if (main_work.crsr_idx == 3)
            {
                if (act_comp > DMD_STGSLCT_ACT_VRTCL_CHNG_NUM)
                {
                    // テーブル移動演出フラグON
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
                }
                else
                {
                    // カーソル移動フラグON
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
                }
            }

            else
            {
                // カーソル移動フラグON
                main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
            }

            main_work.flag |= DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR;

        }

        return;
    }
}




#if 1
// ==========================================================================
// dmStgSlctInputChangeEvtRanking
/*!
	ウインドウ非表示用入力プロシージャ処理(据え置き版)
 */
// ==========================================================================
void dmStgSlctInputChangeEvtRanking(DMS_STGSLCT_MAIN_WORK *main_work)
{
	// メニュー遷移フラグ処理
#if !_WII
	if (AoPadStand() & KEY_R_UP) {
		main_work.flag |= DMD_STGSLCT_FLAG_CHANGE_EVT_RANKING;
		
		return;
	}
#else
	// Wii版は処理なし
	UNREFERENCED_PARAMETER(main_work);
#endif
	
	
}
#endif


// ==========================================================================
// dmStgSlctInputProcWinDispIdle
/*!
	ウインドウ表示用入力プロシージャ処理(据え置き版)
 */
// ==========================================================================
void dmStgSlctInputProcWinDispIdle(DMS_STGSLCT_MAIN_WORK* main_work)
{
#if !_IPHONE
    // キャンセル処理
    if (AoPadStand() & GSD_KEY_CANCEL)
    {
        main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;

        return;
    }

    // 決定処理
    if (AoPadStand() & GSD_KEY_DECIDE)
    {
        if (!main_work.win_cur_slct
            || main_work.win_mode == DME_STGSLCT_WIN_MENU)
        {
            main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;
        }
        else
        {
            main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;
        }

        return;
    }


    if (main_work.win_mode == DME_STGSLCT_WIN_MENU)
    {
        // 十字キー操作
        if (AoPadMStand() & GSD_KEY_UP)
        {
            if (main_work.win_cur_slct != 0)
            {
                DmSoundPlaySE("Cursol");
            }

            // 選択項目切り替え
            main_work.win_cur_slct = 0;

            // 切り替えフラグ？
        }
        else if (AoPadMStand() & GSD_KEY_DOWN)
        {
            if (main_work.win_cur_slct != 1)
            {
                DmSoundPlaySE("Cursol");
            }

            // 選択項目切り替え
            main_work.win_cur_slct = 1;
        }
    }


    if (main_work.win_mode == DME_STGSLCT_WIN_STG_SLCT)
    {
        if (AoPadMStand() & GSD_KEY_LEFT)
        {
            if (main_work.win_cur_slct != 0)
            {
                DmSoundPlaySE("Cursol");
            }

            // 選択項目切り替え
            main_work.win_cur_slct = 0;
        }
        else if (AoPadMStand() & GSD_KEY_RIGHT)
        {
            if (main_work.win_cur_slct != 1)
            {
                DmSoundPlaySE("Cursol");
            }

            // 選択項目切り替え
            main_work.win_cur_slct = 1;
        }
    }
#else //!_IPHONE
	if (main_work.win_mode == DME_STGSLCT_WIN_STG_SLCT) {
		//選択肢有り
		for (int i = 0, max = (ACT_TEX_NO - ACT_TEX_YES + 1); i < max; ++i) {
			const er::CTrgAoAction *answer;
			AOS_ACTION **act, **act_end;
			s32 win_cur_slct;
			u32 flag;
			switch (ACT_TEX_YES + i) {
			case ACT_TEX_NO:
				answer = &main_work.trg_answer[0];
				act = &main_work.act[ACT_NO_BTN_L];
				act_end = &main_work.act[ACT_NO_BTN_R + 1];
				win_cur_slct = 1;
				flag = DMD_STGSLCT_FLAG_CANCEL;
				break;
			case ACT_TEX_YES:
				answer = &main_work.trg_answer[1];
				act = &main_work.act[ACT_YES_BTN_L];
				act_end = &main_work.act[ACT_YES_BTN_R + 1];
				win_cur_slct = 0;
				flag = DMD_STGSLCT_FLAG_DECIDE;
				break;
			default:
				answer = NULL;
				break;
			}
			float frame;
			if (answer.GetState(0)[er::CTrgState::EState::Up] && answer.GetState(0)[er::CTrgState::EState::Prev]) {
				frame = 2.0f;
				main_work.win_cur_slct = win_cur_slct;
				main_work.flag |= flag;
			} else if (answer.GetState(0)[er::CTrgState::EState::On]) {
				frame = 1.0f;
			} else if (2.0f <= (*act).frame) {
				frame = (*act).frame;
			} else {
				frame = 0.0f;
			}
			while (act != act_end) {
				AoActSetFrame(*act, frame);
				AoActUpdate(*act, 0.0f);
				++act;
			}
		}
	} else {
		//選択肢無し
		if (amTpIsTouchPush(0)) {
			main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;
		}
	}
#endif //!_IPHONE
}



// ==========================================================================
// dmStgSlctProcActDraw
/*!
	描画設定プロシージャ処理
 */
// ==========================================================================
void dmStgSlctProcActDraw(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 帯移動演出用		※常に移動しつづけるのでここに配置
    dmStgSlctSetObiEfctPos(main_work);

    // 共通描画処理は描画時は常に設定
    dmStgSlctCommonDraw(main_work);

    // ZONE選択部分描画設定
    dmStgSlctZoneSelectDraw(main_work);

    if (main_work.state == DME_STGSLCT_MODE_STATE_ACT_SLCT)
    {
        dmStgSlctStageSelectDraw(main_work);
    }

    // FIX部分描画設定
    dmStgSlctCommonFixDraw(main_work);

    // アクションを使用せず、テクスチャを描画(アルファのグラデーション使用のため)
    dmStgSlctMakeVertexAct(main_work, &main_work.up_bg_vrtx);
}


// ==========================================================================
// dmStgSlctCommonDraw
/*!
	共通描画設定処理
 */
// ==========================================================================
void dmStgSlctCommonDraw(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // ゾーンテーブル用AO描画プライオリティ設定
    AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_BG);

    // アクション更新
    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

#if _IPHONE
	if (u32(-1) == main_work.cur_bg_id) {
		//ゾーンセレクト用特別背景なら透明にする
		AoActSortRegAction(main_work.act[ACT_WAVE_BG]);
		AoActAcmPush();
		AoActAcmInit();
		AoActAcmApplyFade(main_work.bg_fade);
		AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[0]));
		AoActUpdate(main_work.act[ACT_WAVE_BG], 1.f);
		AoActAcmPop();
	} else //↓に繋げる
#endif //_IPHONE
    for (int i = 0; i < 4; i++)
    {
        AoActSortRegAction(main_work.act[ACT_ZONE_BG_LT + i]);

        AoActSetFrame(main_work.act[ACT_ZONE_BG_LT + i], (f32)main_work.cur_bg_id);
        //		AoActSetFrame(main_work.act[ACT_ZONE_BG_LT + i], (f32)main_work.cur_zone);

        AoActAcmPush();

        AoActAcmInit();

        AoActAcmApplyFade(main_work.bg_fade);

        AoActUpdate(main_work.act[ACT_ZONE_BG_LT + i], 0.f);

        AoActAcmPop();
    }

#if _IPHONE
	if (_am_sample_draw_enable) {
#endif //_IPHONE
    // ソート実行
    AoActSortExecute();

    // ソートバッファ描画
    AoActSortDraw();
#if _IPHONE
	}
#endif //_IPHONE

    // ソートバッファ全解除
    AoActSortUnregAll();


    // ゾーンテーブル用AO描画プライオリティ設定
    AoActSysSetDrawTaskPrio(0x2c00);

    // アクション登録(登録は全てここで行うようにし、実際の登録するかはフラグにて設定するようにする)
    for (int i = ACT_DOWN_BG; i <= ACT_BLUE_BG; i++)
    {
        AoActSortRegAction(main_work.act[i]);
    }

    // アクション更新
    AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[0]));

    for (int i = ACT_DOWN_BG; i <= ACT_BLUE_BG; i++)
    {
        AoActUpdate(main_work.act[i], 0.0f);
    }

#if _IPHONE
	if (_am_sample_draw_enable) {
#endif //_IPHONE
    // ソート実行
    AoActSortExecute();

    // ソートバッファ描画
    AoActSortDraw();
#if _IPHONE
	}
#endif //_IPHONE

    // ソートバッファ全解除
    AoActSortUnregAll();

}



// ==========================================================================
// dmStgSlctCommonFixDraw
/*!
	共通描画設定処理
 */
// ==========================================================================
void dmStgSlctCommonFixDraw(DMS_STGSLCT_MAIN_WORK* main_work)
{
    u32 open_zone_num = 0;
    float tmp_disp_dist = 0.f;

    // ゾーンテーブル用AO描画プライオリティ設定
    AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_FIX);

    // アクション更新
    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

    // アクション登録(登録は全てここで行うようにし、実際の登録するかはフラグにて設定するようにする)
    for (int i = ACT_TAB_MODE_L; i <= ACT_TAB_EMER; i++)
    {
        AoActSortRegAction(main_work.act[i]);
    }

    for (int i = ACT_ICON_EMER_1; i <= ACT_ICON_EMER_7; i++)
    {
        if (main_work.get_emerald & 1 << (i - ACT_ICON_EMER_1))
        {
            AoActSortRegAction(main_work.act[i]);
        }
    }

    // アクション更新
    AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[1]));

#if !_IPHONE //iPhoneではボタン類を描画しない
    // ウインドウ表示中は非表示
    if (!main_work.win_size_rate[0] && !main_work.win_size_rate[1])
    {
        AoActSortRegAction(main_work.act[ACT_BTN_CANCEL1]);
    }

    if (main_work.mode_tex_move_frm == 5)
    {
        AoActSortRegAction(main_work.act[ACT_BTN_LB]);

#if !_WII
        AoActSortRegAction(main_work.act[ACT_BTN_MENU]);
#endif
    }

#if !_WII
    if (main_work.state == DME_STGSLCT_MODE_STATE_ACT_SLCT)
    {
        AoActSortRegAction(main_work.act[ACT_BTN_LB_ARROW]);
        AoActSortRegAction(main_work.act[ACT_BTN_RB_ARROW]);

        AoActSetFrame(main_work.act[ACT_BTN_LB_ARROW], (f32)main_work.btn_l_disp_frm);
        AoActSetFrame(main_work.act[ACT_BTN_RB_ARROW], (f32)main_work.btn_r_disp_frm);
    }

    // 左上・右上のボタン周り表示の演出フレーム処理
    if (main_work.btn_l_disp_frm < DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM)
    {
        main_work.btn_l_disp_frm++;
    }
    else
    {
        main_work.btn_l_disp_frm = DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM;
    }


    if (main_work.btn_r_disp_frm < DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM)
    {
        main_work.btn_r_disp_frm++;
    }
    else
    {
        main_work.btn_r_disp_frm = DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM;
    }
#endif
#endif //!_IPHONE //iPhoneではボタン類を描画しない

    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

    if (main_work.state == DME_STGSLCT_MODE_STATE_ACT_SLCT)
    {
        if (main_work.cur_game_mode != DME_STGSLCT_PLAY_MODE_TIME_ATK)
        {
            AoActSortRegAction(main_work.act[ACT_TAB_STATE_L]);
            AoActSortRegAction(main_work.act[ACT_TAB_STATE_C]);
            AoActSortRegAction(main_work.act[ACT_TAB_STATE_R]);

#if !_IPHONE
            AoActSetFrame(main_work.act[ACT_TAB_STATE_L], (f32)main_work.mode_tex_frm);
            AoActSetFrame(main_work.act[ACT_TAB_STATE_C], (f32)main_work.mode_tex_frm);
            AoActSetFrame(main_work.act[ACT_TAB_STATE_R], (f32)main_work.mode_tex_frm);
#endif //!_IPHONE
        }
        else
        {
            AoActSortRegAction(main_work.act[ACT_TAB_STATE_L2]);
            AoActSortRegAction(main_work.act[ACT_TAB_STATE_C2]);
            AoActSortRegAction(main_work.act[ACT_TAB_STATE_R2]);

#if !_IPHONE
            AoActSetFrame(main_work.act[ACT_TAB_STATE_L2], (f32)main_work.mode_tex_frm);
            AoActSetFrame(main_work.act[ACT_TAB_STATE_C2], (f32)main_work.mode_tex_frm);
            AoActSetFrame(main_work.act[ACT_TAB_STATE_R2], (f32)main_work.mode_tex_frm);
#endif //!_IPHONE
        }
    }

#if !_IPHONE   //すでに描画している               
    // アクション更新
    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

    // アクション登録(登録は全てここで行うようにし、実際の登録するかはフラグにて設定するようにする)
    AoActSortRegAction(main_work.act[ACT_TEX_SONIC]);
#endif //!_IPHONE	//すでに描画している

    // アクション更新
    AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));

    // タイムアタックの場合
    if (main_work.cur_game_mode == DME_STGSLCT_PLAY_MODE_TIME_ATK)
    {
#if !_IPHONE
        AoActSortRegAction(main_work.act[ACT_TEX_MODE_SCORE]);
        AoActSetFrame(main_work.act[ACT_TEX_MODE_SCORE], (f32)main_work.mode_tex_move_frm);
#endif //!_IPHONE

        AoActSortRegAction(main_work.act[ACT_TEX_BIG_TIME]);
#if !_IPHONE
        AoActSetFrame(main_work.act[ACT_TEX_BIG_TIME], (f32)main_work.mode_tex_frm);
        AoActSortRegAction(main_work.act[ACT_TEX_TIME_EFCT]);
        AoActSetFrame(main_work.act[ACT_TEX_TIME_EFCT], (f32)main_work.mode_tex_frm);
#endif //!_IPHONE
    }
    // スコアアタックの場合
    else
    {
#if !_IPHONE
        AoActSortRegAction(main_work.act[ACT_TEX_MODE_TIME]);
        AoActSetFrame(main_work.act[ACT_TEX_MODE_TIME], (f32)main_work.mode_tex_move_frm);
#endif //!_IPHONE

        AoActSortRegAction(main_work.act[ACT_TEX_BIG_SCORE]);
#if !_IPHONE
        AoActSetFrame(main_work.act[ACT_TEX_BIG_SCORE], (f32)main_work.mode_tex_frm);
        AoActSortRegAction(main_work.act[ACT_TEX_SCORE_EFCT]);
        AoActSetFrame(main_work.act[ACT_TEX_SCORE_EFCT], (f32)main_work.mode_tex_frm);
#endif //!_IPHONE
    }

    // モードテキストのフレーム更新
#if !_IPHONE
    if (main_work.mode_tex_frm < DMD_STGSLCT_MODE_TEX_FRAME_MAX)
    {
        main_work.mode_tex_frm++;
    }
#else //!_IPHONE
	main_work.mode_tex_frm = 0.0f;
#endif //!_IPHONE

#if !(_WII || _IPHONE)
    AoActSortRegAction(main_work.act[ACT_TEX_MENU]);
#endif //!(_WII || _IPHONE)

#if !_IPHONE
    for (int i = ACT_TEX_EMER; i < ACT_TEX_ZONE_UP_S; i++)
    {
        if (i == ACT_TEX_EMER)
        {
            AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
        }
        else
        {
            AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
        }

        AoActSortRegAction(main_work.act[i]);
    }
#endif //!_IPHONE

#if _IPHONE
	if ((u32(-1) == main_work.cur_bg_id) || (DME_STGSLCT_MODE_STATE_ZONE_SLCT == main_work.state)) {
		// 言語別データのゾーン名を使用

		//アクトセレクトからゾーンセレクトに戻ってくる時にエッグマンステージが表示される事が有る
		//それの対応する為にゾーンセレクトの入り演出中は強制的にこのゾーン名を使用する
		AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
		AoActSortRegAction(main_work.act[ACT_TEX_ZONE_UP_S]);
	} else //↓に繋げる
#endif //_IPHONE
    // スペステ以外なら
    if (main_work.cur_zone != DME_STGSLCT_ZONE_TYPE_SPE)
    {
        // 共通データのゾーン名を使用
        AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
        AoActSortRegAction(main_work.act[ACT_TEX_ZONE_UP]);
    }
    // スペステなら
    else
    {
        // 言語別データのゾーン名を使用
        AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
        AoActSortRegAction(main_work.act[ACT_TEX_ZONE_UP_S]);
    }

    AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));

    // ウインドウ表示中は非表示
    if (!main_work.win_size_rate[0] && !main_work.win_size_rate[1])
    {
        AoActSortRegAction(main_work.act[ACT_TEX_FIX_BACK]);
#if _IPHONE
		AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[3]));
		for (int i = ACT_BACK_BTN01_L; i <= ACT_BACK_BTN01_R; i++) {
			AoActSortRegAction(main_work.act[i]);
		}
#endif //_IPHONE
    }

    // 残機設定部分
    dmStgSlctSetSonicStockDispFrame(main_work);


    if (main_work.is_jp_region)
    {
        AoActSetFrame(main_work.act[ACT_BTN_CANCEL1], (f32)0.f);
    }
    else
    {
        AoActSetFrame(main_work.act[ACT_BTN_CANCEL1], (f32)1.f);
    }

#if !_IPHONE
    AoActSetFrame(main_work.act[ACT_TEX_MENU], (f32)main_work.mode_tex_move_frm);
#endif //!_IPHONE

    AoActSetFrame(main_work.act[ACT_TEX_ZONE_UP], (f32)main_work.cur_zone);
#if _IPHONE
	{
		f32 frame = 0.0f;
		if ((u32(-1) == main_work.cur_bg_id) || (DME_STGSLCT_MODE_STATE_ZONE_SLCT == main_work.state)) {
			// 言語別データのゾーン名を使用
	
			//アクトセレクトからゾーンセレクトに戻ってくる時にエッグマンステージが表示される事が有る
			//それの対応する為にゾーンセレクトの入り演出中は強制的にこのゾーン名を使用する
			frame = 1.0f;
		}
		AoActSetFrame(main_work.act[ACT_TEX_ZONE_UP_S], frame);
	}
#endif //_IPHONE


    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

    // アクション登録(登録は全てここで行うようにし、実際の登録するかはフラグにて設定するようにする)
    for (int i = ACT_TAB_MODE_L; i <= ACT_REST_NUM_1; i++)
    {
        AoActUpdate(main_work.act[i], 0.0f);
    }


    AoActAcmPush();

    for (int i = ACT_TAB_EMER; i <= ACT_ICON_EMER_7; i++)
    {

        AoActAcmInit();
        AoActAcmApplyTrans(0.f
                           , main_work.chaos_eme_pos_y
                           , 0.f
                           );

        AoActUpdate(main_work.act[i], 0.0f);
    }

    AoActAcmPop();


    // ボタン
#if !_IPHONE //iPhoneではボタン類を描画しない
    AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[1]));

    for (int i = ACT_BTN_CANCEL1; i <= ACT_BTN_MENU; i++)
    {
        AoActUpdate(main_work.act[i], 0.0f);
    }

#if !_WII
    AoActUpdate(main_work.act[ACT_BTN_LB_ARROW], 0.0f);
    AoActUpdate(main_work.act[ACT_BTN_RB_ARROW], 0.0f);
#endif
#endif //!_IPHONE //iPhoneではボタン類を描画しない

    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
    AoActAcmPush();

#if _IPHONE
	bool is_disp_act_enter_window = false; //アクト決定ウインドウの表示中か
	if ((NULL != main_work.proc_win_update) && (dmStgSlctProcWindowNodispIdle != main_work.proc_win_update)) {
		//ウインドウ表示中
		is_disp_act_enter_window = true;
	}
	if (main_work.trg_act_move.GetState(0)[er::CTrgState::EState::DragAndDrop] || is_disp_act_enter_window
			|| (!main_work.trg_mode[0].GetState(0)[er::CTrgState::EState::Lock] && !main_work.trg_mode[1].GetState(0)[er::CTrgState::EState::Lock])) {
		//アクト決定ウインドウが表示されていたら、もしくはモード切り換えがロックされていなかったら
		//更新
		AoActUpdate(main_work.act[ACT_TAB_STATE_MOVE]);
	}
	f32 tab_state_move_pos[AMD_XY] = {main_work.act[ACT_TAB_STATE_MOVE].sprite.center_x - main_work.act_tab_state_move_base_pos[AMD_X], main_work.act[ACT_TAB_STATE_MOVE].sprite.center_y - main_work.act_tab_state_move_base_pos[AMD_Y]};
#endif //_IPHONE
    for (int i = ACT_TAB_STATE_L; i <= ACT_TAB_STATE_R2; i++)
    {

        AoActAcmInit();
        AoActAcmApplyTrans(0.f
                           , main_work.mode_tex_pos_y
                           , 0.f
                           );
#if !_IPHONE
        AoActUpdate(main_work.act[i], 0.0f);
#else //!_IPHONE
		AoActAcmApplyTrans(tab_state_move_pos[AMD_X], tab_state_move_pos[AMD_Y], 0.0f);
		f32 frame = ((2.0f <= main_work.act[i].frame)? 1.0f: 0.0f);
		AoActUpdate(main_work.act[i], frame);
#endif //!_IPHONE

    }

    AoActAcmPop();

#if !_IPHONE
    AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));

    for (int i = ACT_TEX_MODE_TIME; i <= ACT_TEX_EMER; i++)
    {
        if (i != ACT_TEX_EMER)
        {
            AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
            AoActUpdate(main_work.act[i], 0.0f);
        }
        else
        {
            AoActAcmPush();
            AoActAcmInit();
            AoActAcmApplyTrans(0.f
                               , main_work.chaos_eme_pos_y
                               , 0.f
                               );

            AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
            AoActUpdate(main_work.act[i], 0.0f);

            AoActAcmPop();
        }
    }
#endif //!_IPHONE
#if _IPHONE    //当たり判定処理          
	for (er::CTrgAoAction *mode = main_work.trg_mode, *mode_end = main_work.trg_mode + arrayof(main_work.trg_mode); mode != mode_end; ++mode) {
		mode.Update();
	}
#endif //_IPHONE	//当たり判定処理

    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
    AoActUpdate(main_work.act[ACT_TEX_ZONE_UP], 0.0f);

    AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
    AoActUpdate(main_work.act[ACT_TEX_ZONE_UP_S], 0.0f);


    AoActAcmPush();

#if !_IPHONE
    for (int i = ACT_TEX_BIG_TIME; i <= ACT_TEX_SCORE_EFCT; i++)
    {
#else //!_IPHONE
	for (int i = ACT_TEX_BIG_TIME; i <= ACT_TEX_BIG_SCORE; i++) {
#endif //!_IPHONE

        AoActAcmInit();
        AoActAcmApplyTrans(0.f
                           , main_work.mode_tex_pos_y
                           , 0.f
                           );
#if !_IPHONE
        AoActUpdate(main_work.act[i], 0.0f);
#else //!_IPHONE
		AoActAcmApplyTrans(tab_state_move_pos[AMD_X], tab_state_move_pos[AMD_Y], 0.0f);
		f32 frame = ((2.0f <= main_work.act[i].frame)? 1.0f: 0.0f);
		AoActUpdate(main_work.act[i], frame);
#endif //!_IPHONE

    }

    AoActAcmPop();

    // 戻るテキスト
    AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
    AoActUpdate(main_work.act[ACT_TEX_FIX_BACK], 0.0f);
#if _IPHONE
	AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[3]));
	for (int i = ACT_BACK_BTN01_L; i <= ACT_BACK_BTN01_R; i++) {
		float update_frame = ((2.0f <= main_work.act[i].frame)? 1.0f: 0.0f);
		AoActUpdate(main_work.act[i], update_frame);
	}
#endif //_IPHONE
#if _IPHONE //当たり判定処理          
	{
		er::CTrgAoAction &cancel = main_work.trg_cancel;
		cancel.Update();
	}
#endif //_IPHONE	//当たり判定処理



    if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
    {
        open_zone_num = 6;
        tmp_disp_dist = 0.f;
    }
    else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN
             || main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
    {
        open_zone_num = 5;
        tmp_disp_dist = 24.f;
    }
    else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN
             || main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
    {
        open_zone_num = 5;
        tmp_disp_dist = 24.f;
    }
    else
    {
        open_zone_num = 4;
        tmp_disp_dist = 64.f;
    }

    // ダウンアイコン表示
    if (main_work.state == DME_STGSLCT_MODE_STATE_ACT_SLCT)
    {
        AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

        for (u32 i = 0; i < open_zone_num; i++)
        {
            AoActSortRegAction(main_work.act[ACT_ICON_DOWN_1 + i]);

            if (main_work.cur_zone == i)
            {
                AoActSetFrame(main_work.act[ACT_ICON_DOWN_1 + i], 0.0f);
            }
            else
            {
                AoActSetFrame(main_work.act[ACT_ICON_DOWN_1 + i], 1.0f);
            }

            // FINALが開いてない状態でスペステが開いた状態の場合の補正処理
            if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN
                && main_work.cur_zone == 5 && i == 4)
            {
                if (!(main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN))
                {
                    AoActSetFrame(main_work.act[ACT_ICON_DOWN_1 + i], 0.0f);
                }
            }

            AoActAcmPush();

            AoActAcmInit();
            AoActAcmApplyTrans(tmp_disp_dist, 0.f, 0.f);

            AoActUpdate(main_work.act[ACT_ICON_DOWN_1 + i], 0.0f);

#if _IPHONE  //当たり判定処理          
			{
				er::CTrgAoAction &tab = main_work.trg_act_tab[i];
				tab.Update();
			}
#endif //_IPHONE	//当たり判定処理

            AoActAcmPop();
        }
    }

#if _IPHONE
	if (_am_sample_draw_enable) {
#endif //_IPHONE
    // ソート実行
#if !_IPHONE
    AoActSortExecute();
#else //!_IPHONE
	AoActSortExecuteFix();
#endif //!_IPHONE

    // ソートバッファ描画
    AoActSortDraw();
#if _IPHONE
	}
#endif //_IPHONE

    // ソートバッファ全解除
    AoActSortUnregAll();


#if !_IPHONE
    // 帯描画設定
    AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[2]));

    for (int i = ACT_OBI_C; i <= ACT_OBI_R; i++)
    {
        AoActSortRegAction(main_work.act[i]);
    }

    // 帯
    AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[2]));

    for (int i = ACT_OBI_C; i <= ACT_OBI_R; i++)
    {
        AoActUpdate(main_work.act[i], 0.0f);
    }
#endif //!_IPHONE


    // アクション更新
    AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));

    // 帯はループ用に２つ用意
#if !_IPHONE
    for (int i = 0; i < 2; i++)
    {
        AoActSortRegAction(main_work.act[ACT_TEX_OBI1 + i]);

        if (main_work.state == DME_STGSLCT_MODE_STATE_ACT_SLCT
            && main_work.cur_zone == DME_STGSLCT_ZONE_TYPE_SPE)
        {
            AoActSetFrame(main_work.act[ACT_TEX_OBI1 + i], 2.f);
        }
        else
        {
            AoActSetFrame(main_work.act[ACT_TEX_OBI1 + i], (float)main_work.state);
        }

        // 帯用
        AoActAcmPush();

        AoActAcmInit();
        AoActAcmApplyTrans(main_work.obi_pos[i]
                           , 0
                           , 0
                           );

#if _PS3 || _XBOX || _PC
		AoActAcmApplyScale(DMD_STGSLCT_OBI_MSG_SCALE_SIZE
						   , DMD_STGSLCT_OBI_MSG_SCALE_SIZE);
#endif

        AoActUpdate(main_work.act[ACT_TEX_OBI1 + i], 0.f);

        AoActAcmPop();
    }
#endif //!_IPHONE

#if _IPHONE
	if (_am_sample_draw_enable) {
#endif //_IPHONE
    // ソート実行
    AoActSortExecute();

    // ソートバッファ描画
    AoActSortDraw();
#if _IPHONE
	}
#endif //_IPHONE

    // ソートバッファ全解除
    AoActSortUnregAll();
}



// ==========================================================================
// dmStgSlctSetSonicStockDispFrame
/*!
	ゾーン選択用描画設定処理
 */
// ==========================================================================
void dmStgSlctSetSonicStockDispFrame(DMS_STGSLCT_MAIN_WORK* main_work)
{
    int tmp_digit_data = 0;
    int tmp_digit[3] = { 0, 0, 0 };
    int tmp_calc = 1;

    //	tmp_digit_data = (int)(main_work.player_stock);
    tmp_digit_data = (int)(main_work.player_stock - 1);    // ※※※一旦、ステセレの残機表示はゲーム中より+1の状態にする

    if (tmp_digit_data < 0)
    {
        tmp_digit_data = 0;
    }
    if (tmp_digit_data > GSD_MAINSYS_PLAYER_REST_MAX - 1)
    {
        tmp_digit_data = GSD_MAINSYS_PLAYER_REST_MAX - 1;
    }



    // 残機数描画
    for (u32 j = 0; j < 3; j++)
    {
        for (u32 k = 0; k < 3 - j - 1; k++)
        {
            tmp_calc = tmp_calc * 10;
        }


        if (tmp_digit_data >= tmp_calc)
        {
            tmp_digit[j] = (s32)(tmp_digit_data / tmp_calc);
            tmp_digit_data -= (int)((tmp_digit[j]) * tmp_calc);

            //			disp_zero_num = TRUE;
        }
        else
        {
            //			if (disp_zero_num) {
            //				AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + j]);
            //			}

            tmp_digit[j] = 0;
        }

        tmp_calc = 1;
    }

    for (u32 j = 0; j < 3; j++)
    {
        AoActSetFrame(main_work.act[ACT_REST_NUM_100 + j], (f32)tmp_digit[j]);
    }

}



// ==========================================================================
// dmStgSlctZoneSelectDraw
/*!
	ゾーン選択用描画設定処理
 */
// ==========================================================================
void dmStgSlctZoneSelectDraw(DMS_STGSLCT_MAIN_WORK* main_work)
{
    u32 i = 0;
    s32 tmp_decide_dst_x = 0;
    s32 tmp_decide_dst_y = 0;

    // ゾーンテーブル用AO描画プライオリティ設定
    AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_ZONE);

    for (i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
    {
        // ZONE一つ分の描画設定
        dmStgSlctOneZoneTableDraw(main_work, i);
    }

    // 決定ZONE演出フラグONならば
    if (main_work.flag & DMD_STGSLCT_FLAG_DECIDE_ZONE_TABLE_EFCT)
    {
        tmp_decide_dst_x = main_work.decide_zone_efct_dist_x;
        tmp_decide_dst_y = main_work.decide_zone_efct_dist_y;
    }
    else
    {
        tmp_decide_dst_x = 0;
        tmp_decide_dst_y = 0;
    }

#if !_IPHONE   //iPhoneではカーソルを描画しない                        
    // カーソル描画
    if (main_work.state == DME_STGSLCT_MODE_STATE_ZONE_SLCT)
    {
        AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

        for (i = 0; i < 3; i++)
        {
            AoActSortRegAction(main_work.act[ACT_TAB_ZONE_CURSOR2 + i]);
        }

        AoActAcmPush();

        AoActAcmInit();
        AoActAcmApplyTrans(main_work.zone_pos[main_work.cur_zone][0] + tmp_decide_dst_x
                           , main_work.zone_pos[main_work.cur_zone][1] + tmp_decide_dst_y
                           , 0
                           );

        for (i = 0; i < 3; i++)
        {
            AoActUpdate(main_work.act[ACT_TAB_ZONE_CURSOR2 + i]);
        }

        AoActAcmPop();

#if _IPHONE
	if (_am_sample_draw_enable) {
#endif //_IPHONE
        // ソート実行
        AoActSortExecute();

        // ソートバッファ描画
        AoActSortDraw();
#if _IPHONE
	}
#endif //_IPHONE

        // ソートバッファ全解除
        AoActSortUnregAll();
    }
#endif //!_IPHONE	//iPhoneではカーソルを描画しない
}



// ==========================================================================
// dmStgSlctOneZoneTableDraw
/*!
	ゾーン選択時のゾーンテーブル一つ分描画設定処理
 */
// ==========================================================================
void dmStgSlctOneZoneTableDraw(DMS_STGSLCT_MAIN_WORK* main_work, u32 i)
{
    s32 tmp_decide_dst_x = 0;
    s32 tmp_decide_dst_y = 0;

    // ここで白フラッシュ解放演出の場合、指定のゾーンテーブルを非表示にする
    if (main_work.announce_flag & 1 << DME_STGSLCT_WIN_1_1_CLEAR)
    {
        if (main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT
            && (i != DME_STGSLCT_ZONE_TYPE_1))
        {
            return;
        }
    }
    else if (main_work.announce_flag & 1 << DME_STGSLCT_WIN_FINAL_CAN_SLCT)
    {
        if (main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT
            && (i == DME_STGSLCT_ZONE_TYPE_FINAL))
        {
            return;
        }
    }
    else if (main_work.announce_flag & 1 << DME_STGSLCT_WIN_SPESTE_CAN_SLCT)
    {
        if (main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT
            && (i == DME_STGSLCT_ZONE_TYPE_SPE))
        {
            return;
        }
    }


    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

    AoActSortRegAction(main_work.act[ACT_TAB_ZONE_TAB]);
    AoActSortRegAction(main_work.act[ACT_TAB_ZONE_SCR1 + i]);

    if (i != DME_STGSLCT_ZONE_TYPE_SPE)
    {
        AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
        AoActSortRegAction(main_work.act[ACT_TAB_ZONE_TEXT]);
    }
    else
    {
        AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
        AoActSortRegAction(main_work.act[ACT_TAB_ZONE_TEXT_S]);
    }

    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

    // ここでiを見て、アクティブ以外は登録
#if !_IPHONE   //iPhoneではカーソルがあっていないゾーンも明るく表示する                                    
    if (main_work.cur_zone != i)
    {
#else //!_IPHONE	//iPhoneではカーソルがあっていないゾーンも明るく表示する
	if ((main_work.cur_zone != i) && main_work.is_disp_cover) {
#endif //!_IPHONE	//iPhoneではカーソルがあっていないゾーンも明るく表示する
        AoActSortRegAction(main_work.act[ACT_TAB_ZONE_COVER2]);
        AoActSortRegAction(main_work.act[ACT_TAB_ZONE_COVER1]);
        AoActSortRegAction(main_work.act[ACT_TAB_ZONE_COVER3]);
    }

    // iを見て、テクスチャを切り替える(フレームで)
#if !_IPHONE   //カーソルがあっていないゾーンもアニメーションさせる                               
    if (main_work.cur_zone != i)
    {
#else //!_IPHONE	//カーソルがあっていないゾーンもアニメーションさせる
	if (false) {
#endif //!_IPHONE	//カーソルがあっていないゾーンもアニメーションさせる
        AoActSetFrame(main_work.act[ACT_TAB_ZONE_SCR1 + i], 0.f);

        AoActSetFrame(main_work.act[ACT_TAB_ZONE_SCR1_1a + i * 3], 0.f);
        AoActSetFrame(main_work.act[ACT_TAB_ZONE_SCR1_2a + i * 3], 0.f);
        AoActSetFrame(main_work.act[ACT_TAB_ZONE_SCR1_3a + i * 3], 0.f);
    }
    else
    {
        AoActSetFrame(main_work.act[ACT_TAB_ZONE_SCR1 + i], (f32)main_work.zone_scr_id);

        if (i < 4)
        {
            AoActSortRegAction(main_work.act[ACT_TAB_ZONE_SCR1_1a + i * 3]);
            AoActSortRegAction(main_work.act[ACT_TAB_ZONE_SCR1_2a + i * 3]);
            AoActSortRegAction(main_work.act[ACT_TAB_ZONE_SCR1_3a + i * 3]);

            AoActSetFrame(main_work.act[ACT_TAB_ZONE_SCR1_1a + i * 3], (f32)main_work.zone_scr_id);
            AoActSetFrame(main_work.act[ACT_TAB_ZONE_SCR1_2a + i * 3], (f32)main_work.zone_scr_id);
            AoActSetFrame(main_work.act[ACT_TAB_ZONE_SCR1_3a + i * 3], (f32)main_work.zone_scr_id);
        }
    }

    //	AoActSetFrame(main_work.act[ACT_TAB_ZONE_SCR1 + i], (f32)i);
    AoActSetFrame(main_work.act[ACT_TAB_ZONE_TEXT], (f32)i);

    // 決定ZONE演出フラグONならば
    if (main_work.flag & DMD_STGSLCT_FLAG_DECIDE_ZONE_TABLE_EFCT
        && main_work.cur_zone == i)
    {
        tmp_decide_dst_x = main_work.decide_zone_efct_dist_x;
        tmp_decide_dst_y = main_work.decide_zone_efct_dist_y;
    }
    else
    {
        tmp_decide_dst_x = 0;
        tmp_decide_dst_y = 0;
    }

    // ACCUMURATEでトランスさせる
    AoActAcmPush();

    AoActAcmInit();
    AoActAcmApplyTrans((f32)(main_work.zone_pos[i][0] + tmp_decide_dst_x)
                       , (f32)(main_work.zone_pos[i][1] + tmp_decide_dst_y)
                       , 0
                       );

    // テーブル一つ分はトランス値は一定のため、まとめてUpdateする
    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
    AoActUpdate(main_work.act[ACT_TAB_ZONE_TAB], 0.0f);
    AoActUpdate(main_work.act[ACT_TAB_ZONE_SCR1 + i], 0.0f);

#if _IPHONE    //当たり判定処理          
	{
		er::CTrgAoAction &zone = main_work.trg_zone[i];
		zone.Update();
	}
#endif //_IPHONE	//当たり判定処理

    for (i = ACT_TAB_ZONE_SCR1_1a; i <= ACT_TAB_ZONE_SCR4_3a; i++)
    {
        AoActUpdate(main_work.act[i], 0.0f);
    }

    AoActUpdate(main_work.act[ACT_TAB_ZONE_COVER2], 0.0f);
    AoActUpdate(main_work.act[ACT_TAB_ZONE_COVER1], 0.0f);
    AoActUpdate(main_work.act[ACT_TAB_ZONE_COVER3], 0.0f);

    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
    AoActUpdate(main_work.act[ACT_TAB_ZONE_TEXT], 0.0f);

    AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
    AoActUpdate(main_work.act[ACT_TAB_ZONE_TEXT_S], 0.0f);

    AoActAcmPop();

#if _IPHONE
	if (_am_sample_draw_enable) {
#endif //_IPHONE
    // ソート実行
    AoActSortExecute();

    // ソートバッファ描画
    AoActSortDraw();
#if _IPHONE
	}
#endif //_IPHONE

    // ソートバッファ全解除
    AoActSortUnregAll();
}



// ==========================================================================
// dmStgSlctStageSelectDraw
/*!
	ステージ選択用描画設定処理
 */
// ==========================================================================
void dmStgSlctStageSelectDraw(DMS_STGSLCT_MAIN_WORK* main_work)
{
    u32 i = 0;

    // ステージテーブル用AO描画プライオリティ設定
    AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_STAGE);

    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

    // 現在選択中のZONE分のACTテーブル表示
    dmStgSlctSetDrawStageSelectTable(main_work, main_work.cur_zone, true);


    // 切り替え分のZONE分のACTテーブル表示
#if !_IPHONE
    if (main_work.chng_zone != DME_STGSLCT_ACT_PAGE_NONE)
    {
#else //!_IPHONE
	if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_ZONE) && (main_work.chng_zone != DME_STGSLCT_ACT_PAGE_NONE)) {
#endif //!_IPHONE
        dmStgSlctSetDrawStageSelectTable(main_work, main_work.chng_zone);
    }

#if _IPHONE    //当たり判定処理          
	{
		er::CTrgFlick &move = main_work.trg_act_move;
		move.Update();
	}
#endif //_IPHONE	//当たり判定処理

    // ここでカーソル表示フラグの設定
    if (main_work.state == DME_STGSLCT_MODE_STATE_ACT_SLCT
        && main_work.cur_zone == DME_STGSLCT_ZONE_TYPE_SPE)
    {
        main_work.disp_flag |= DMD_STGSLCT_DISP_FLAG_ACT_CRSR;
    }
    else
    {
        main_work.disp_flag &= ~DMD_STGSLCT_DISP_FLAG_ACT_CRSR;
    }


    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

#if 1
	// カーソル表示
#if !_IPHONE
	if (main_work.disp_flag & DMD_STGSLCT_DISP_FLAG_ACT_CRSR) {
		AoActSortRegAction(main_work.act[ACT_TAB_CURSOR_UP]);
		AoActSortRegAction(main_work.act[ACT_TAB_CURSOR_DOWN]);
	}
#else //!_IPHONE
	if ((main_work.disp_flag & DMD_STGSLCT_DISP_FLAG_LR_ARROW) && (DME_STGSLCT_ZONE_TYPE_FINAL != main_work.cur_zone)) {
		if ((0 < main_work.focus_disp_no) || (DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL & main_work.flag)) {
			AoActSortRegAction(main_work.act[ACT_TAB_CURSOR_UP]);
		}
		const s32 c_move_limit = ((DME_STGSLCT_ZONE_TYPE_SPE == main_work.cur_zone)? 4: 1);
		if ((main_work.focus_disp_no < c_move_limit) || (DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL & main_work.flag)) {
			AoActSortRegAction(main_work.act[ACT_TAB_CURSOR_DOWN]);
		}
	}
#endif //!_IPHONE
	
	// ACCUMURATEでトランスさせる
	AoActAcmPush();
	
#if !_IPHONE
	for (i = ACT_TAB_CURSOR_UP; i <= ACT_TAB_CURSOR_3; i++) {
		AoActAcmInit();
		AoActAcmApplyTrans(312.f
						   , 0.f
						   , 0.f
						   );
#else //!_IPHONE
	for (i = ACT_TAB_CURSOR_UP; i <= ACT_TAB_CURSOR_DOWN; i++) {
		AoActAcmInit();
#endif //!_IPHONE

	// テーブル一つ分はトランス値は一定のため、まとめてUpdateする
	
		// フレーム更新はSetFrameのみで行う
		AoActUpdate(main_work.act[i], 1.0f);
	}
	
	AoActAcmPop();
#endif

#if _IPHONE
	if (_am_sample_draw_enable) {
#endif //_IPHONE
    // ソート実行
    AoActSortExecute();

    // ソートバッファ描画
    AoActSortDraw();
#if _IPHONE
	}
#endif //_IPHONE

    // ソートバッファ全解除
    AoActSortUnregAll();


    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

    // 切り替え矢印表示
    if (main_work.disp_flag & DMD_STGSLCT_DISP_FLAG_LR_ARROW)
    {
        for (i = 0; i < 2; i++)
        {
            AoActSortRegAction(main_work.act[ACT_ICON_L_ARROW + i]);
            AoActUpdate(main_work.act[ACT_ICON_L_ARROW + i], 1.0f);
        }

#if _IPHONE    //当たり判定処理          
		for (er::CTrgAoAction *lr = main_work.trg_act_lr, *lr_end = main_work.trg_act_lr + arrayof(main_work.trg_act_lr); lr != lr_end; ++lr) {
			lr.Update();
		}
#endif //_IPHONE	//当たり判定処理

#if _IPHONE
	if (_am_sample_draw_enable) {
#endif //_IPHONE
        // ソート実行
        AoActSortExecute();

        // ソートバッファ描画
        AoActSortDraw();
#if _IPHONE
	}
#endif //_IPHONE

        // ソートバッファ全解除
        AoActSortUnregAll();
    }

}



// ==========================================================================
// dmStgSlctSetDrawStageSelectTable
/*!
	ステージ選択用描画設定処理
 */
// ==========================================================================
void dmStgSlctSetDrawStageSelectTable(DMS_STGSLCT_MAIN_WORK* main_work, u32 zone, bool is_trg_update)
{
#if !_IPHONE
    u32 act_num_start = 0;
    u32 act_num_end = 0;
    u32 i = 0;
    u32 j = 0;
    u32 k = 0;

    act_num_start = dm_stgslct_zone_act_num_tbl[zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[zone][1] + act_num_start;

    for (i = act_num_start; i < act_num_end; i++)
    {
        AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

        // 台紙描画登録
        for (j = ACT_TAB_TABLE2; j <= ACT_TAB_TABLE3; j++)
        {
            AoActSortRegAction(main_work.act[j]);
        }

        // ステージ別テーブル上のアクション描画登録
        if (zone != DME_STGSLCT_ACT_PAGE_SPE)
        {
            AoActSortRegAction(main_work.act[ACT_TAB_SCR_BG]);
            AoActSortRegAction(main_work.act[ACT_TAB_SCR]);

            if (i != 16)
            {
                if ((i + 1) % 4 != 0
                    || i == 0)
                {
                    // 通常ACT
                    AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_TAB_TEXT]);

                    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
                    AoActSetFrame(main_work.act[ACT_TAB_A_NUM], dm_stgslct_act_num_disp_id_tbl[i]);
                    AoActSortRegAction(main_work.act[ACT_TAB_A_NUM]);
                }
                else
                {
                    // BOSS
                    AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_TAB_TEX_BOSS]);
                }
            }
            else
            {
                // FINAL
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_TAB_TEX_BOSS]);
            }

            AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
            AoActSortRegAction(main_work.act[ACT_TAB_MESS]);

            AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
            AoActSortRegAction(main_work.act[ACT_TAB_LINE]);
            //			AoActSortRegAction(main_work.act[ACT_TAB_ICON_EMER]);		// エメラルドは既定ステージのみ登録 ◆

            for (k = 0; k < 7; k++)
            {
                if (main_work.eme_stage_no[k] == i
                    && main_work.get_emerald & 1 << k)
                {
                    AoActSortRegAction(main_work.act[ACT_TAB_ICON_EMER]);
                    AoActSetFrame(main_work.act[ACT_TAB_ICON_EMER], (float)k);

                    break;
                }
            }

            // モードを見てSCOREかTIMEの表示を分ける
            if (main_work.cur_game_mode == DME_STGSLCT_PLAY_MODE_NORMAL)
            {
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_TAB_TEX_SCORE]);

                AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
                for (k = 0; k < 9; k++)
                {
                    //					AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + k]);		// ※ここはスコアの桁数分のみ登録する
                }
            }
            else
            {
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_TAB_TEX_TIME]);
                for (k = 0; k < 7; k++)
                {
                    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
                    AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + k]);
                }
            }
        }

        else
        {
            AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));

            if (GsMainSysIsStageClear(GSD_MAIN_STAGE_ID_SS1 + (i - act_num_start)))
            {
                AoActSortRegAction(main_work.act[ACT_TAB_ICON_SPE_EMER]);
            }

            AoActSortRegAction(main_work.act[ACT_TAB_NUM_SPE_STAGE]);

            AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
            AoActSortRegAction(main_work.act[ACT_TAB_TEX_SPE_STAGE]);
            //			AoActSortRegAction(main_work.act[ACT_TAB_TEX_TIME]);

            // モードを見てSCOREかTIMEの表示を分ける
            if (main_work.cur_game_mode == DME_STGSLCT_PLAY_MODE_NORMAL)
            {
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_TAB_TEX_SCORE]);

                AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
                for (k = 0; k < 9; k++)
                {
                    //					AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + k]);		// ※ここはスコアの桁数分のみ登録する
                }
            }
            else
            {
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_TAB_TEX_TIME]);
                for (k = 0; k < 7; k++)
                {
                    AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
                    AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + k]);
                }
            }

            AoActSetFrame(main_work.act[ACT_TAB_ICON_SPE_EMER], (float)(i - act_num_start));
            AoActSetFrame(main_work.act[ACT_TAB_NUM_SPE_STAGE], dm_stgslct_act_num_disp_id_tbl[i]);
        }

        // ステージ台紙のアクティブ・非アクティブ設定
        dmStgSlctSetTableActiveInfo(main_work, i);

        // ステージ番号からテキスト、ACT番号、を設定
        AoActSetFrame(main_work.act[ACT_TAB_ZONE_TEXT], dm_stgslct_act_table_disp_id_tbl[i]);

        // スコアを見てフレームを設定


        // エメラルド取得情報からフレームを設定


        // モードを見てSCOREかTIMEの表示を分ける
        dmStgSlctSetScoreDispFrame(main_work, zone, i);

        // ここでiを見て、アクティブ以外は登録
        if (main_work.cur_stage != (s32)i)
        {
            AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
            AoActSortRegAction(main_work.act[ACT_TAB_COVER2]);
            AoActSortRegAction(main_work.act[ACT_TAB_COVER1]);
            AoActSortRegAction(main_work.act[ACT_TAB_COVER3]);
        }

        // ACCUMURATEでトランスさせる
        AoActAcmPush();


        for (j = ACT_TAB_START; j <= ACT_TAB_END; j++)
        {
            AoActAcmInit();
            AoActAcmApplyTrans(main_work.act_top_pos_x[i]
                               , main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8)
                               , 0
                               );

            if (j == ACT_TAB_TEXT || j == ACT_TAB_MESS
                || (j >= ACT_TAB_TEX_SCORE && j <= ACT_TAB_TEX_SPE_STAGE))
            {
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
            }
            else
            {
                AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
            }

            // フレーム更新はSetFrameのみで行う
            AoActUpdate(main_work.act[j], 0.0f);
        }

        for (j = ACT_TAB_COVER2; j <= ACT_TAB_COVER3; j++)
        {
            AoActAcmInit();

            AoActAcmApplyTrans(main_work.act_top_pos_x[i]
                               , main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8)
                               , 0
                               );

            AoActSetTexture(AoTexGetTexList(&main_work.tex[0]));
            AoActUpdate(main_work.act[j], 0.0f);
        }

        AoActAcmPop();

        // ソート実行
        AoActSortExecute();

        // ソートバッファ描画
        AoActSortDraw();

        // ソートバッファ全解除
        AoActSortUnregAll();
    }
#else //!_IPHONE//■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
	//アクト単位で書くのではなく、同じテクスチャを使用しているアクションを横断的に描画していく
	u32 act_num_start = 0;
	u32 act_num_end = 0;
	u32 i = 0;
	u32 j = 0;
	u32 k = 0;

	act_num_start = dm_stgslct_zone_act_num_tbl[zone][0];
	act_num_end = dm_stgslct_zone_act_num_tbl[zone][1] + act_num_start;
	
	class CActionDraw {
	public:
	void Entry(const void *ama, u32 id, f32 frame, f32 x, f32 y) {
		if (_am_sample_draw_enable) {
			AOS_ACTION *buf = AoActCreate(ama, id, frame);
			AoActAcmPush();
			AoActAcmInit();
			AoActAcmApplyTrans(x, y, 0);
			AoActUpdate(buf, 0.0f);
			AoActSortRegAction(buf);
			AoActAcmPop();
			m_action_array.push_back(buf);
		}
	}
	void Clear() {
		for (TActionArray::iterator action = m_action_array.begin(), action_end = m_action_array.end(); action != action_end; ++action) {
			AoActDelete(*action);
		}
		m_action_array.clear();
	}
	void Draw() {
		if (_am_sample_draw_enable) {
			AoActSortExecute();
			AoActSortDraw();
		}
		AoActSortUnregAll();
	}
	CActionDraw() : m_action_array() {}
	~CActionDraw() {Clear();}
	private:
		typedef accel::CCircularBuffer<AOS_ACTION *, 100> TActionArray;
		TActionArray m_action_array;
	};
	CActionDraw act_draw_sys;

	//台紙
	AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_CMN_DATA]));
	for (i = act_num_start; i < act_num_end; i++) {
		for (j = ACT_TAB_TABLE2; j <= ACT_TAB_TABLE3; j++) {
			dmStgSlctSetTableActiveInfo(main_work, i);
			AoActAcmPush();
			AoActAcmInit();
			AoActAcmApplyTrans(main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8), 0);
			AoActUpdate(main_work.act[j], 0.0f);
			AoActAcmPop();
			act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[j], main_work.act[j].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
		}
#if _IPHONE    //当たり判定処理          
		if (is_trg_update) {
			//描画の関係上1fに複数回呼ばれる可能性があるので、明示的に1回に押さえる
			if ((act_num_start <= i) && (i < (act_num_start + arrayof(main_work.trg_act)))) {
				u32 index = i - act_num_start;
				er::CTrgAoAction &act = main_work.trg_act[index];
				act.Update();
			}
		}
#endif //_IPHONE	//当たり判定処理
	}
	act_draw_sys.Draw();
	act_draw_sys.Clear();

	// ステージ別テーブル上のアクション描画登録
	if (zone != DME_STGSLCT_ACT_PAGE_SPE) {
		//スペステ以外
		AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_CMN_DATA]));
		for (i = act_num_start; i < act_num_end; i++) {
			AoActUpdate(main_work.act[ACT_TAB_SCR_BG], 0.0f);
			act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[ACT_TAB_SCR_BG], main_work.act[ACT_TAB_SCR_BG].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
		}
		for (i = act_num_start; i < act_num_end; i++) {
			dmStgSlctSetTableActiveInfo(main_work, i);
			AoActUpdate(main_work.act[ACT_TAB_SCR], 0.0f);
			act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[ACT_TAB_SCR], main_work.act[ACT_TAB_SCR].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
		}
		act_draw_sys.Draw();
		act_draw_sys.Clear();

		for (i = act_num_start; i < act_num_end; i++) {
			if (i == 16) {
				// FINAL
				AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_LANG_DATA]));
				AoActUpdate(main_work.act[ACT_TAB_TEX_BOSS], 0.0f);
				act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[ACT_TAB_TEX_BOSS], main_work.act[ACT_TAB_TEX_BOSS].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
			} else if (((i + 1) % 4 != 0) || (i == 0)) {
				// 通常ACT
				AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_LANG_DATA]));
				AoActUpdate(main_work.act[ACT_TAB_TEXT], 0.0f);
				act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[ACT_TAB_TEXT], main_work.act[ACT_TAB_TEXT].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
			} else {
				// BOSS
				AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_LANG_DATA]));
				AoActUpdate(main_work.act[ACT_TAB_TEX_BOSS], 0.0f);
				act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[ACT_TAB_TEX_BOSS], main_work.act[ACT_TAB_TEX_BOSS].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
			}
		}
		for (i = act_num_start; i < act_num_end; i++) {
			if (i == 16) {
				// FINAL
			} else if (((i + 1) % 4 != 0) || (i == 0)) {
				// 通常ACT
				AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_CMN_DATA]));
				AoActSetFrame(main_work.act[ACT_TAB_A_NUM], dm_stgslct_act_num_disp_id_tbl[i]);
				AoActUpdate(main_work.act[ACT_TAB_A_NUM], 0.0f);
				act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[ACT_TAB_A_NUM], main_work.act[ACT_TAB_A_NUM].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
			} else {
				// BOSS
			}
		}
		act_draw_sys.Draw();
		act_draw_sys.Clear();

		for (i = act_num_start; i < act_num_end; i++) {
			// ステージ台紙のアクティブ・非アクティブ設定
			dmStgSlctSetTableActiveInfo(main_work, i);
			AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_LANG_DATA]));
			AoActUpdate(main_work.act[ACT_TAB_MESS], 0.0f);
			act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[ACT_TAB_MESS], main_work.act[ACT_TAB_MESS].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
		}

		for (i = act_num_start; i < act_num_end; i++) {
			AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_CMN_DATA]));
			AoActUpdate(main_work.act[ACT_TAB_LINE], 0.0f);
			AoActSortRegAction(main_work.act[ACT_TAB_LINE]);
			act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[ACT_TAB_LINE], main_work.act[ACT_TAB_LINE].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
		}
		act_draw_sys.Draw();
		act_draw_sys.Clear();

		for (i = act_num_start; i < act_num_end; i++) {
			for (k = 0; k < 7; k++) {
				if ((main_work.eme_stage_no[k] == i) && (main_work.get_emerald & (1 << k))) {
					AoActUpdate(main_work.act[ACT_TAB_ICON_EMER], 0.0f);
					AoActSetFrame(main_work.act[ACT_TAB_ICON_EMER], (float)k);
					act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[ACT_TAB_ICON_EMER], main_work.act[ACT_TAB_ICON_EMER].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
					break;
				}
			}
		}
		act_draw_sys.Draw();
		act_draw_sys.Clear();

	} else {
		//スペステ
		AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_LANG_DATA]));
		for (i = act_num_start; i < act_num_end; i++) {
			AoActUpdate(main_work.act[ACT_TAB_TEX_SPE_STAGE], 0.0f);
			act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[ACT_TAB_TEX_SPE_STAGE], main_work.act[ACT_TAB_TEX_SPE_STAGE].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
		}
		act_draw_sys.Draw();
		act_draw_sys.Clear();

		AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_CMN_DATA]));
		for (i = act_num_start; i < act_num_end; i++) {
			if (GsMainSysIsStageClear(GSD_MAIN_STAGE_ID_SS1 + (i - act_num_start))) {
				AoActSetFrame(main_work.act[ACT_TAB_ICON_SPE_EMER], (float)(i - act_num_start));
				AoActUpdate(main_work.act[ACT_TAB_ICON_SPE_EMER], 0.0f);
				act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[ACT_TAB_ICON_SPE_EMER], main_work.act[ACT_TAB_ICON_SPE_EMER].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
			}
		}
		for (i = act_num_start; i < act_num_end; i++) {
			AoActSetFrame(main_work.act[ACT_TAB_NUM_SPE_STAGE], dm_stgslct_act_num_disp_id_tbl[i]);
			AoActUpdate(main_work.act[ACT_TAB_NUM_SPE_STAGE], 0.0f);
			act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[ACT_TAB_NUM_SPE_STAGE], main_work.act[ACT_TAB_NUM_SPE_STAGE].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
		}
		act_draw_sys.Draw();
		act_draw_sys.Clear();
	}
			
	// モードを見てSCOREかTIMEの表示を分ける
	if (main_work.cur_game_mode == DME_STGSLCT_PLAY_MODE_NORMAL) {
		//SCORE
		AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_LANG_DATA]));
		for (i = act_num_start; i < act_num_end; i++) {
			AoActUpdate(main_work.act[ACT_TAB_TEX_SCORE], 0.0f);
			act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[ACT_TAB_TEX_SCORE], main_work.act[ACT_TAB_TEX_SCORE].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
		}
		for (i = act_num_start; i < act_num_end; i++) {
			dmStgSlctSetScoreDispFrame(main_work, zone, i);
			u32 k = 0;
			for (; k < DMD_STGSLCT_DISP_SCORE_DIGIT_NUM; k++) {
				if (0 != main_work.act[ACT_TAB_S_NUM1 + k].frame) {
					break;
				}
			}
			for (; k < DMD_STGSLCT_DISP_SCORE_DIGIT_NUM; k++) {
				AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_CMN_DATA]));
				AoActUpdate(main_work.act[ACT_TAB_S_NUM1 + k], 0.0f);
				act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[ACT_TAB_S_NUM1 + k], main_work.act[ACT_TAB_S_NUM1 + k].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
			}
		}
	} else {
		//TIME
		AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_LANG_DATA]));
		for (i = act_num_start; i < act_num_end; i++) {
			AoActUpdate(main_work.act[ACT_TAB_TEX_TIME], 0.0f);
			act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[ACT_TAB_TEX_TIME], main_work.act[ACT_TAB_TEX_TIME].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
		}
		for (i = act_num_start; i < act_num_end; i++) {
			dmStgSlctSetScoreDispFrame(main_work, zone, i);
			for (k = 0; k < 7; k++) {
				AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_CMN_DATA]));
				AoActUpdate(main_work.act[ACT_TAB_S_NUM1 + k], 0.0f);
				act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[ACT_TAB_S_NUM1 + k], main_work.act[ACT_TAB_S_NUM1 + k].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
			}
		}
	}
	act_draw_sys.Draw();
	act_draw_sys.Clear();
			
	//スコア描画
	for (i = act_num_start; i < act_num_end; i++) {
		dmStgSlctSetScoreDispFrame(main_work, zone, i);
		if (_am_sample_draw_enable) {
			AoActSortExecute();
			AoActSortDraw();
		}
		AoActSortUnregAll();
	}

	// ここでiを見て、アクティブ以外は登録
	for (i = act_num_start; i < act_num_end; i++) {
		AoActSetTexture(AoTexGetTexList(&main_work.tex[DME_STGSLCT_DATA_TYPE_CMN_DATA]));
		if ((main_work.cur_stage != (s32)i) && main_work.is_disp_cover) {
			AoActUpdate(main_work.act[ACT_TAB_COVER2], 0.0f);
			AoActUpdate(main_work.act[ACT_TAB_COVER1], 0.0f);
			AoActUpdate(main_work.act[ACT_TAB_COVER3], 0.0f);
			act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[ACT_TAB_COVER3], main_work.act[ACT_TAB_COVER3].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
			act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[ACT_TAB_COVER1], main_work.act[ACT_TAB_COVER1].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
			act_draw_sys.Entry(main_work.ama[DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[ACT_TAB_COVER2], main_work.act[ACT_TAB_COVER2].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
		}
	}
	act_draw_sys.Draw();
	act_draw_sys.Clear();
#endif //!_IPHONE
}



// ==========================================================================
// dmStgSlctSetScoreDispFrame
/*!
	各ステージの表示描画フレーム設定処理
 */
// ==========================================================================
void dmStgSlctSetScoreDispFrame(DMS_STGSLCT_MAIN_WORK* main_work, u32 zone, u32 act_no)
{
    UNREFERENCED_PARAMETER(zone);

    int tmp_digit_data = 0;
    s32 tmp_digit[DMD_STGSLCT_DISP_SCORE_DIGIT_NUM] = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    int tmp_calc = 1;

    u16 time_min = 0;
    u16 time_sec = 0;
    u16 time_msec = 0;
    int time_digit[2] = { 0, 0 };
    float time_digit_data = 0.f;
    BOOL disp_zero_num = FALSE;


    if (main_work.cur_game_mode == DME_STGSLCT_PLAY_MODE_NORMAL)
    {
        if (main_work.hi_score[act_no] != DMD_STGSLCT_INIT_SCORE_NUM)
        {

            tmp_digit_data = (int)main_work.hi_score[act_no];

            // スコア描画
            for (u32 j = 0; j < DMD_STGSLCT_DISP_SCORE_DIGIT_NUM; j++)
            {

                if (DMD_STGSLCT_DISP_SCORE_DIGIT_NUM - j - 1 <= 0)
                {
                    tmp_digit_data = 1;
                }
                else
                {
                    for (u32 k = 0; k < DMD_STGSLCT_DISP_SCORE_DIGIT_NUM - j - 1; k++)
                    {
                        tmp_calc = tmp_calc * 10;
                    }
                }

                if (tmp_digit_data >= tmp_calc)
                {
                    AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + j]);
                    if (j >= DMD_STGSLCT_DISP_SCORE_DIGIT_NUM - 1)
                    {
                        tmp_digit[j] = 0;
                    }
                    else
                    {
                        tmp_digit[j] = (s32)(tmp_digit_data / tmp_calc);
                        tmp_digit_data -= (int)((tmp_digit[j]) * tmp_calc - 1);
                    }
                    disp_zero_num = TRUE;
                }
                else
                {
                    if (disp_zero_num)
                    {
                        AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + j]);
                    }

                    tmp_digit[j] = 0;
                }

                tmp_calc = 1;
            }

            // スコアの各桁の数字フレーム設定
            for (u32 k = 0; k < 9; k++)
            {
                AoActSetFrame(main_work.act[ACT_TAB_S_NUM1 + k], (float)tmp_digit[k]);
            }
        }
        else
        {
            for (u32 k = 0; k < 9; k++)
            {
                AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + k]);
                AoActSetFrame(main_work.act[ACT_TAB_S_NUM1 + k], DMD_STGSLCT_NO_ACTIVE_NUM_ID);
            }
        }
    }

    else
    {

        if (main_work.record_time[act_no] != DMD_STGSLCT_INIT_RECORD_TIME_NUM)
        {
            // 取得したクリアタイムを分・秒・ミリ秒に変換
            AkUtilFrame60ToTime((u32)main_work.record_time[act_no]
                                , &time_min
                                , &time_sec
                                , &time_msec
                                );

            time_digit_data = (float)time_sec;

            if (time_digit_data >= 10)
            {
                time_digit[0] = (int)(time_digit_data / 10.f);
                time_digit_data -= (float)(time_digit[0] * 10.f);
            }
            else
            {
                time_digit[0] = 0;
            }

            time_digit[1] = (int)time_digit_data;

            // 分
            AoActSetFrame(main_work.act[ACT_TAB_S_NUM1], (f32)time_min);

            AoActSetFrame(main_work.act[ACT_TAB_S_NUM2], DMD_STGSLCT_TIME_COLON_ID);

            // 秒
            AoActSetFrame(main_work.act[ACT_TAB_S_NUM3], (float)time_digit[0]);
            AoActSetFrame(main_work.act[ACT_TAB_S_NUM4], (float)time_digit[1]);

            AoActSetFrame(main_work.act[ACT_TAB_S_NUM5], DMD_STGSLCT_TIME_COLON_ID);

            time_digit_data = (float)time_msec;

            if (time_digit_data >= 10)
            {
                time_digit[0] = (int)(time_digit_data / 10.f);
                time_digit_data -= (float)(time_digit[0] * 10.f);
            }
            else
            {
                time_digit[0] = 0;
            }

            time_digit[1] = (int)time_digit_data;

            // ミリ秒
            AoActSetFrame(main_work.act[ACT_TAB_S_NUM6], (float)time_digit[0]);
            AoActSetFrame(main_work.act[ACT_TAB_S_NUM7], (float)time_digit[1]);
            //			AoActSetFrame(main_work.act[ACT_TAB_S_NUM8], 10.f);
            //			AoActSetFrame(main_work.act[ACT_TAB_S_NUM9], 10.f);

        }
        // こちらは未プレイ時の表示設定(-で全て表示)
        else
        {
            for (u32 k = 0; k < 7; k++)
            {
                if (k == 1 || k == 4)
                {
                    AoActSetFrame(main_work.act[ACT_TAB_S_NUM1 + k]
                                  , DMD_STGSLCT_TIME_COLON_ID
                                  );
                }
                else
                {
                    AoActSetFrame(main_work.act[ACT_TAB_S_NUM1 + k]
                                  , DMD_STGSLCT_NO_ACTIVE_NUM_ID
                                  );
                }
            }
        }
    }
}



// ==========================================================================
// dmStgSlctWinSelectDraw
/*!
	ウインドウ用描画設定処理
 */
// ==========================================================================
void dmStgSlctWinSelectDraw(DMS_STGSLCT_MAIN_WORK* main_work)
{
    f32 tmp_win_size[2] = { 0.f, 0.f };

    // ウインドウ用AO描画プライオリティ設定
    AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_WIN_FIX);

#if _WII || _IPHONE
	tmp_win_size[0] = DMD_STGSLCT_WINDOW_SIZE_W * DMD_STGSLCT_DISP_SCALE_TEXT;
	tmp_win_size[1] = DMD_STGSLCT_WINDOW_SIZE_H * DMD_STGSLCT_DISP_SCALE_TEXT;
#else
    tmp_win_size[0] = DMD_STGSLCT_WINDOW_SIZE_W;
    tmp_win_size[1] = DMD_STGSLCT_WINDOW_SIZE_H;
#endif

    // ウインドウ描画
#if _IPHONE
	switch (main_work.win_mode) {
	case DME_STGSLCT_WIN_STG_SLCT: //アクト決定確認
		//少し大きくする
#if defined(AMD_DEBUG)
		static float c_scale_x[AMD_XY] = {1.01f, 1.27f};
		if (AoPadRepeat() & KEY_R_UP) {
			c_scale_x[AMD_Y] += 0.01f;
		} else if (AoPadRepeat() & KEY_R_DOWN) {
			c_scale_x[AMD_Y] -= 0.01f;
		}
		if (AoPadRepeat() & KEY_R_LEFT) {
			c_scale_x[AMD_X] += 0.01f;
		} else if (AoPadRepeat() & KEY_R_RIGHT) {
			c_scale_x[AMD_X] -= 0.01f;
		}
		amPrintf(60, 44, "%6.2f  %6.2f", c_scale_x[AMD_X], c_scale_x[AMD_Y]);
		amPrintf(59, 41, "%6.2f  %6.2f", c_scale_x[AMD_X], c_scale_x[AMD_Y]);
#else //defined(AMD_DEBUG)
		const float c_scale_x[AMD_XY] = {1.01f, 1.27f};
#endif //defined(AMD_DEBUG)
		for (int i = 0; i < AMD_XY; ++i) {
			tmp_win_size[i] *= c_scale_x[i];
		}
		break;
	}
	if ((0.0f < main_work.win_size_rate[0]) && (0.0f < main_work.win_size_rate[1])) //↓下に繋げる
#endif //_IPHONE
    AoWinSysDrawTask(AOD_WIN_TYPE_A
                     , AoTexGetTexList(&main_work.cmn_tex[3])
                     , 0
                     , DMD_STGSLCT_SIZE_WIDTH / 2.0f        // ウインドウ中心X
                     , DMD_STGSLCT_SIZE_HEIGHT / 2.0f       // ウインドウ中心Y
                     , tmp_win_size[0] * main_work.win_size_rate[0]            // ウインドウ横サイズ
                     , tmp_win_size[1] * main_work.win_size_rate[1]            // ウインドウ縦サイズ
                     , DMD_STGSLCT_DRAW_PRIO_WIN            // 描画タスク優先度
                     );

    // ウインドウ内の項目描画
    if (main_work.disp_flag & DMD_STGSLCT_DISP_FLAG_WIN_ACT)
    {       // ウインドウが表示しきっているならば
            // 共通ウインドウACT登録
        AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[3]));
        //		AoActSortRegAction(main_work.act[ACT_WIN_LINE]);

        AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
        //		AoActSortRegAction(main_work.act[ACT_TEX_WINTITLE]);


        switch (main_work.win_mode)
        {
            case DME_STGSLCT_WIN_MENU:
#if !_IPHONE
                // アクション登録
                AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[1]));
                AoActSortRegAction(main_work.act[ACT_BTN_CANCEL2]);

                AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
                AoActSortRegAction(main_work.act[ACT_TEX_BACK1]);

                // 初期表示フレーム設定
                AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_MENU][0]);
                AoActSetFrame(main_work.act[ACT_BTN_CANCEL2], 0.f);
                AoActSetFrame(main_work.act[ACT_TEX_BACK1], 0.f);
#endif //!_IPHONE

                break;

            case DME_STGSLCT_WIN_STG_SLCT:
#if !_IPHONE
                AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[1]));
                AoActSortRegAction(main_work.act[ACT_BTN_CANCEL2]);
#else //!_IPHONE
			AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[3]));
			for (int i = ACT_YES_BTN_L, max = ACT_NO_BTN_R; i <= max; ++i) {
				AoActSortRegAction(main_work.act[i]);
			}
#endif //!_IPHONE
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG2]);

                AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
                AoActSortRegAction(main_work.act[ACT_TEX_YES]);
                AoActSortRegAction(main_work.act[ACT_TEX_NO]);
#if !_IPHONE
                AoActSortRegAction(main_work.act[ACT_TEX_BACK1]);

                // 初期表示フレーム設定
                AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_STG_SLCT][0]);
#endif //!_IPHONE

                if (main_work.cur_zone != DME_STGSLCT_ZONE_TYPE_SPE)
                {
                    AoActSetFrame(main_work.act[ACT_WIN_TEX_MSG2], 0.f);
                }
                else
                {
                    AoActSetFrame(main_work.act[ACT_WIN_TEX_MSG2], 1.f);
                }

#if !_IPHONE
                AoActSetFrame(main_work.act[ACT_TEX_YES]
                              , dm_stgslct_win_disp_slct_frm_tbl[main_work.win_cur_slct][0]);
                AoActSetFrame(main_work.act[ACT_TEX_NO]
                              , dm_stgslct_win_disp_slct_frm_tbl[main_work.win_cur_slct][1]);

                AoActSetFrame(main_work.act[ACT_BTN_CANCEL2], 0.f);

                AoActSetFrame(main_work.act[ACT_TEX_BACK1], 0.f);
#endif //!_IPHONE

                break;

            case DME_STGSLCT_WIN_1_1_CLEAR:
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG]);

#if !_IPHONE
                AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
                AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                // 初期表示フレーム設定
                AoActSetFrame(main_work.act[ACT_TEX_WINTITLE],
                              dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_1_1_CLEAR][0]);
#endif //!_IPHONE
                AoActSetFrame(main_work.act[ACT_WIN_TEX_MSG]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_1_1_CLEAR][1]);
#if !_IPHONE
                AoActSetFrame(main_work.act[ACT_TEX_OK], 0.f);
#endif //!_IPHONE

                break;

            case DME_STGSLCT_WIN_BOSS1_CAN_PLAY:
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG]);

#if !_IPHONE
                AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
                AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                // 初期表示フレーム設定
                AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_BOSS1_CAN_PLAY][0]);
#endif //!_IPHONE
                AoActSetFrame(main_work.act[ACT_WIN_TEX_MSG]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_BOSS1_CAN_PLAY][1]);
#if !_IPHONE
                AoActSetFrame(main_work.act[ACT_TEX_OK], 0.f);
#endif //!_IPHONE

                break;

            case DME_STGSLCT_WIN_BOSS2_CAN_PLAY:
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG]);

#if !_IPHONE
                AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
                AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                // 初期表示フレーム設定
                AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_BOSS2_CAN_PLAY][0]);
#endif //!_IPHONE
                AoActSetFrame(main_work.act[ACT_WIN_TEX_MSG]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_BOSS2_CAN_PLAY][1]);
#if !_IPHONE
                AoActSetFrame(main_work.act[ACT_TEX_OK], 0.f);
#endif //!_IPHONE

                break;

            case DME_STGSLCT_WIN_BOSS3_CAN_PLAY:
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG]);

#if !_IPHONE
                AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
                AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                // 初期表示フレーム設定
                AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_BOSS3_CAN_PLAY][0]);
#endif //!_IPHONE
                AoActSetFrame(main_work.act[ACT_WIN_TEX_MSG]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_BOSS3_CAN_PLAY][1]);
#if !_IPHONE
                AoActSetFrame(main_work.act[ACT_TEX_OK], 0.f);
#endif //!_IPHONE

                break;

            case DME_STGSLCT_WIN_BOSS4_CAN_PLAY:
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG]);

#if !_IPHONE
                AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
                AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                // 初期表示フレーム設定
                AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_BOSS4_CAN_PLAY][0]);
#endif //!_IPHONE
                AoActSetFrame(main_work.act[ACT_WIN_TEX_MSG]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_BOSS4_CAN_PLAY][1]);
#if !_IPHONE
                AoActSetFrame(main_work.act[ACT_TEX_OK], 0.f);
#endif //!_IPHONE

                break;

            case DME_STGSLCT_WIN_FINAL_CAN_SLCT:
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG]);

#if !_IPHONE
                AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
                AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                // 初期表示フレーム設定
                AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_FINAL_CAN_SLCT][0]);
#endif //!_IPHONE
                AoActSetFrame(main_work.act[ACT_WIN_TEX_MSG]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_FINAL_CAN_SLCT][1]);
#if !_IPHONE
                AoActSetFrame(main_work.act[ACT_TEX_OK], 0.f);
#endif //!_IPHONE

                break;

            case DME_STGSLCT_WIN_SSONIC_SLCT:
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG_SSONIC]);
                //			AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG]);

#if !_IPHONE
                AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
                AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                // 初期表示フレーム設定
                AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_SSONIC_SLCT][0]);
                //			AoActSetFrame(main_work.act[ACT_WIN_TEX_MSG]
                //						  , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_SSONIC_SLCT][1]);
                AoActSetFrame(main_work.act[ACT_TEX_OK], 0.f);
#endif //!_IPHONE

                break;

            case DME_STGSLCT_WIN_SPESTE_CAN_SLCT:
                AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG]);

#if !_IPHONE
                AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
                AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                // 初期表示フレーム設定
                AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_SPESTE_CAN_SLCT][0]);
#endif //!_IPHONE
                AoActSetFrame(main_work.act[ACT_WIN_TEX_MSG]
                              , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_SPESTE_CAN_SLCT][1]);
#if !_IPHONE
                AoActSetFrame(main_work.act[ACT_TEX_OK], 0.f);
#endif //!_IPHONE

                break;

            default:
                // 例外
                amAssert(0);
                break;
        }


        if (main_work.is_jp_region)
        {
            AoActSetFrame(main_work.act[ACT_BTN_CANCEL2], (f32)0.f);
        }
        else
        {
            AoActSetFrame(main_work.act[ACT_BTN_CANCEL2], (f32)1.f);
        }


#if !_IPHONE
        // ACCUMURATEでトランスさせる
        AoActAcmPush();

        AoActAcmInit();
        AoActAcmApplyTrans(dm_stgslct_win_act_pos_tbl[0][0]
                           , dm_stgslct_win_act_pos_tbl[0][1]
                           , 0
                           );

        AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[3]));
        AoActUpdate(main_work.act[ACT_WIN_LINE], 0.0f);

        AoActAcmPop();
#endif //!_IPHONE

        AoActAcmPush();

        for (int k = 0; k < 3; k++)
        {
            AoActAcmInit();

            AoActAcmApplyTrans(dm_stgslct_win_act_pos_tbl[k + 1][0]
                               , dm_stgslct_win_act_pos_tbl[k + 1][1]
                               , 0
                               );

            if (k == 0)
            {   // 戻るテキストの言語別の長さをずらす
                AoActAcmApplyTrans(dm_stgslct_back_text_length_tbl[GsEnvGetLanguage()]
                                   , 0
                                   , 0
                                   );
#if _WII       // Wii版のみ左へ10ピクセルさらにずらせる                                
				AoActAcmApplyTrans(-10.f, 0, 0);
#endif
            }

            AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[1]));
            AoActUpdate(main_work.act[ACT_BTN_CANCEL2 + k], 0.0f);
        }
        AoActAcmPop();

        AoActAcmPush();
        for (int k = 0; k < 5; k++)
        {
            AoActAcmInit();

            AoActAcmApplyTrans(dm_stgslct_win_act_pos_tbl[4 + k][0]
                               , dm_stgslct_win_act_pos_tbl[4 + k][1]
                               , 0
                               );

#if _WII || _IPHONE
			AoActAcmApplyScale(DMD_STGSLCT_DISP_SCALE_TEXT
							   , DMD_STGSLCT_DISP_SCALE_TEXT);
#endif

            AoActSetTexture(AoTexGetTexList(&main_work.tex[1]));
            AoActUpdate(main_work.act[ACT_WIN_TEX_MSG + k], 0.0f);
        }
        AoActAcmPop();

        AoActAcmPush();
#if !_IPHONE
        for (int k = 0; k < 5; k++)
        {
#else //!_IPHONE
		const int ACT_TEX_WINTITLE = ACT_TEX_BACK1; //ACT_TEX_WINTITLEのダミーを作成
		for (int k = 0; k < (ACT_TEX_NO - ACT_TEX_BACK1 + 1); k++) {
#endif //!_IPHONE
            AoActAcmInit();

#if _WII
			if (k != ACT_TEX_BACK1 - ACT_TEX_WINTITLE) {
				AoActAcmApplyScale(DMD_STGSLCT_DISP_SCALE_TEXT
								   , DMD_STGSLCT_DISP_SCALE_TEXT);
			}
#endif //_WII || _IPHONE

#if _WII
			if (k == ACT_TEX_OK - ACT_TEX_WINTITLE) {
				AoActAcmApplyTrans(0
								   , 32.f
								   , 0
								   );
			}
#endif //_WII

#if !_IPHONE
            AoActAcmApplyTrans(dm_stgslct_win_act_pos_tbl[9 + k][0]
                               , dm_stgslct_win_act_pos_tbl[9 + k][1]
                               , 0
                               );
#else //!_IPHONE
			AoActAcmApplyTrans(dm_stgslct_win_act_pos_tbl[11+k][0]
							   , dm_stgslct_win_act_pos_tbl[11+k][1]
							   , 0
							   );
#endif //!_IPHONE

            if (main_work.win_mode == DME_STGSLCT_WIN_SSONIC_SLCT
                && k == 2)
            {
                AoActAcmApplyTrans(0
                                   , 16.f
                                   , 0
                                   );
            }

            AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[4]));
            AoActUpdate(main_work.act[ACT_TEX_WINTITLE + k], 0.0f);

#if _IPHONE //当たり判定処理          
			er::CTrgAoAction *answer;
			switch (ACT_TEX_WINTITLE + k) {
			case ACT_TEX_NO:
				answer = &main_work.trg_answer[0];

				AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[3]));
				for (AOS_ACTION **act = &main_work.act[ACT_NO_BTN_L], **act_end = &main_work.act[ACT_NO_BTN_R + 1]; act != act_end; ++act) {
					float update_frame = ((2.0f <= (*act).frame)? 1.0f: 0.0f);
					AoActUpdate(*act, update_frame);
				}
				break;
			case ACT_TEX_YES:
				answer = &main_work.trg_answer[1];

				AoActSetTexture(AoTexGetTexList(&main_work.cmn_tex[3]));
				for (AOS_ACTION **act = &main_work.act[ACT_YES_BTN_L], **act_end = &main_work.act[ACT_YES_BTN_R + 1]; act != act_end; ++act) {
					float update_frame = ((2.0f <= (*act).frame)? 1.0f: 0.0f);
					AoActUpdate(*act, update_frame);
				}
				break;
			default:
				answer = NULL;
				break;
			}
			if (answer) {
				answer.Update();
			}			
#endif //_IPHONE	//当たり判定処理
        }

        AoActAcmPop();

#if _IPHONE
	if (_am_sample_draw_enable) {
#endif //_IPHONE
        // ソート実行
        AoActSortExecute();

        // ソートバッファ描画
        AoActSortDraw();
#if _IPHONE
	}
#endif //_IPHONE

        // ソートバッファ全解除
        AoActSortUnregAll();
    }
}




// ==========================================================================
// dmStgSlctIsDataLoad
/*!
	データ読み込み完了チェック処理
 */
// ==========================================================================
s32 dmStgSlctIsDataLoad(DMS_STGSLCT_MAIN_WORK* main_work)
{

    for (int i = 0; i < 2; i++)
    {
        if (!amFsIsComplete(main_work.arc_amb_fs[i]))
        {
            return 0;
        }
    }

    // メニュー共通データ読み込み
    for (int i = 0; i < 5; i++)
    {
        if (!amFsIsComplete(main_work.arc_cmn_amb_fs[i]))
        {
            return 0;
        }
    }


    return 1;
}



// ==========================================================================
// dmStgSlctIsTexLoad
/*!
	テクスチャ構築完了チェック処理
 */
// ==========================================================================
s32 dmStgSlctIsTexLoad(DMS_STGSLCT_MAIN_WORK* main_work)
{

    for (int i = 0; i < DME_STGSLCT_DATA_TYPE_MAX; i++)
    {
        if (!AoTexIsLoaded(&main_work.tex[i]))
        {
            // フラグ扱いでON
            return 0;
        }
    }

    if (!GsFontIsBuilded())
    {
        return 0;
    }

    return 1;
}


// ==========================================================================
// dmStgSlctIsTexLoad2
/*!
	テクスチャ構築完了チェック処理		◆暫定対応
 */
// ==========================================================================
s32 dmStgSlctIsTexLoad2(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // メニュー共通データ
    for (int i = 0; i < 5; i++)
    {
        if (!AoTexIsLoaded(&main_work.cmn_tex[i]))
        {
            // フラグ扱いでON
            return 0;
        }
    }

    return 1;
}


// ==========================================================================
// dmStgSlctIsTexRelease
/*!
	テクスチャ解放完了チェック処理
 */
// ==========================================================================
s32 dmStgSlctIsTexRelease(DMS_STGSLCT_MAIN_WORK* main_work)
{

    for (int i = 0; i < DME_STGSLCT_DATA_TYPE_MAX; i++)
    {
        if (!AoTexIsReleased(&main_work.tex[i]))
        {
            // フラグ扱いでON
            return 0;
        }
    }

    // メニュー共通データ
    for (int i = 0; i < 5; i++)
    {
        if (!AoTexIsReleased(&main_work.cmn_tex[i]))
        {
            return 0;
        }
    }

    return 1;
}



// ==========================================================================
// dmStgSlctSetDecideZoneEfctPos
/*!
	ゾーン選択時のゾーンテーブル決定演出中処理
 */
// ==========================================================================
void dmStgSlctSetDecideZoneEfctPos(DMS_STGSLCT_MAIN_WORK* main_work)
{
    if (main_work.timer < DMD_STGSLCT_ZONE_DECIDE_EFCT_ON)
    {
        main_work.decide_zone_efct_dist_x = 0;
        main_work.decide_zone_efct_dist_y = DMD_STGSLCT_ZONE_DECIDE_EFCT_ON_POS_Y;
    }
    else
    {
        main_work.decide_zone_efct_dist_x = 0;
        main_work.decide_zone_efct_dist_y = 0;
    }
}



// ==========================================================================
// dmStgSlctIsDecideZoneEfctPos
/*!
	ゾーン選択時のゾーンテーブル決定演出中処理
 */
// ==========================================================================
BOOL dmStgSlctIsDecideZoneEfctPos(DMS_STGSLCT_MAIN_WORK* main_work)
{
    if (main_work.timer > DMD_STGSLCT_ZONE_DECIDE_EFCT_TIME)
    {
        return TRUE;
    }

    return FALSE;
}



// ==========================================================================
// dmStgSlctSetZonePosOutEfct
/*!
	ゾーン選択時のゾーンテーブル掃け演出中処理
 */
// ==========================================================================
void dmStgSlctSetZonePosOutEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    float move_dist[2] = { 0, 0 };
    float distance[2] = { 0, 0 };


    // 入り演出が終了したら
    //	if (dmStgSlctIsZonePosOutEfct(main_work)) {
    //		main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;
    //	}

    // 掃け演出分の座標更新
    for (u32 i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
    {
        if (main_work.efct_out_flag & (1 << i)
            && i != main_work.cur_zone)
        {
            if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
            {
                // ZONEテーブルの初期表示位置設定
                distance[0] = dm_stgslct_a_zone_nodisp_pos_tbl[i][0] - dm_stgslct_a_zone_disp_pos_tbl[i][0];
                distance[1] = dm_stgslct_a_zone_nodisp_pos_tbl[i][1] - dm_stgslct_a_zone_disp_pos_tbl[i][1];
            }

            else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
            {
                // ZONEテーブルの初期表示位置設定
                distance[0] = dm_stgslct_s_zone_nodisp_pos_tbl[i][0] - dm_stgslct_s_zone_disp_pos_tbl[i][0];
                distance[1] = dm_stgslct_s_zone_nodisp_pos_tbl[i][1] - dm_stgslct_s_zone_disp_pos_tbl[i][1];
            }

            else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
            {
                // ZONEテーブルの初期表示位置設定
                distance[0] = dm_stgslct_f_zone_nodisp_pos_tbl[i][0] - dm_stgslct_f_zone_disp_pos_tbl[i][0];
                distance[1] = dm_stgslct_f_zone_nodisp_pos_tbl[i][1] - dm_stgslct_f_zone_disp_pos_tbl[i][1];
            }
            else
            {
                // ZONEテーブルの初期表示位置設定
                distance[0] = dm_stgslct_n_zone_nodisp_pos_tbl[i][0] - dm_stgslct_n_zone_disp_pos_tbl[i][0];
                distance[1] = dm_stgslct_n_zone_nodisp_pos_tbl[i][1] - dm_stgslct_n_zone_disp_pos_tbl[i][1];
            }

            move_dist[0] = distance[0] / DMD_STGSLCT_ZONE_EFCT_TIME;
            move_dist[1] = distance[1] / DMD_STGSLCT_ZONE_EFCT_TIME;

            main_work.zone_pos[i][0] += move_dist[0];
            main_work.zone_pos[i][1] += move_dist[1];
        }
    }

    // 決定ZONEの移動演出分


    // エメラルドテーブルの掃け演出分
    dmStgSlctSetEmeTableOutEfct(main_work);
}




// ==========================================================================
// dmStgSlctIsZonePosOutEfct
/*!
	ゾーン選択時のゾーンテーブル掃け演出中処理
 */
// ==========================================================================
BOOL dmStgSlctIsZonePosOutEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{

    // 掃け演出終了チェック
    if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
    {
        if (main_work.zone_pos[0][0] > dm_stgslct_a_zone_nodisp_pos_tbl[0][0]
            || main_work.zone_pos[0][1] > dm_stgslct_a_zone_nodisp_pos_tbl[0][1])
        {
            if (main_work.cur_zone != 0)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[1][0] > dm_stgslct_a_zone_nodisp_pos_tbl[1][0]
            || main_work.zone_pos[1][1] > dm_stgslct_a_zone_nodisp_pos_tbl[1][1])
        {
            if (main_work.cur_zone != 1)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[2][0] < dm_stgslct_a_zone_nodisp_pos_tbl[2][0]
            || main_work.zone_pos[2][1] > dm_stgslct_a_zone_nodisp_pos_tbl[2][1])
        {
            if (main_work.cur_zone != 2)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[3][0] < dm_stgslct_a_zone_nodisp_pos_tbl[3][0]
            || main_work.zone_pos[3][1] < dm_stgslct_a_zone_nodisp_pos_tbl[3][1])
        {
            if (main_work.cur_zone != 3)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[4][0] > dm_stgslct_a_zone_nodisp_pos_tbl[4][0]
            || main_work.zone_pos[4][1] < dm_stgslct_a_zone_nodisp_pos_tbl[4][1])
        {
            if (main_work.cur_zone != 4)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[5][0] > dm_stgslct_a_zone_nodisp_pos_tbl[5][0]              // ◆
            || main_work.zone_pos[5][1] > dm_stgslct_a_zone_nodisp_pos_tbl[5][1])
        {
            if (main_work.cur_zone != 5)
            {
                return FALSE;
            }
        }
    }
    else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
    {
        if (main_work.zone_pos[0][0] > dm_stgslct_s_zone_nodisp_pos_tbl[0][0]
            || main_work.zone_pos[0][1] > dm_stgslct_s_zone_nodisp_pos_tbl[0][1])
        {
            if (main_work.cur_zone != 0)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[1][0] > dm_stgslct_s_zone_nodisp_pos_tbl[1][0]
            || main_work.zone_pos[1][1] > dm_stgslct_s_zone_nodisp_pos_tbl[1][1])
        {
            if (main_work.cur_zone != 1)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[2][0] < dm_stgslct_s_zone_nodisp_pos_tbl[2][0]
            || main_work.zone_pos[2][1] > dm_stgslct_s_zone_nodisp_pos_tbl[2][1])
        {
            if (main_work.cur_zone != 2)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[3][0] < dm_stgslct_s_zone_nodisp_pos_tbl[3][0]
            || main_work.zone_pos[3][1] < dm_stgslct_s_zone_nodisp_pos_tbl[3][1])
        {
            if (main_work.cur_zone != 3)
            {
                return FALSE;
            }
        }
        //		if (main_work.zone_pos[4][0] > dm_stgslct_s_zone_nodisp_pos_tbl[4][0]
        //			|| main_work.zone_pos[4][1] < dm_stgslct_s_zone_nodisp_pos_tbl[4][1]) {
        //			if (main_work.cur_zone != 4) {
        //				return FALSE;
        //			}
        //		}
        if (main_work.zone_pos[5][0] > dm_stgslct_s_zone_nodisp_pos_tbl[5][0]
            || main_work.zone_pos[5][1] < dm_stgslct_s_zone_nodisp_pos_tbl[5][1])
        {
            if (main_work.cur_zone != 5)
            {
                return FALSE;
            }
        }
    }
    else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
    {
        if (main_work.zone_pos[0][0] > dm_stgslct_f_zone_nodisp_pos_tbl[0][0]
            || main_work.zone_pos[0][1] > dm_stgslct_f_zone_nodisp_pos_tbl[0][1])
        {
            if (main_work.cur_zone != 0)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[1][0] > dm_stgslct_f_zone_nodisp_pos_tbl[1][0]
            || main_work.zone_pos[1][1] > dm_stgslct_f_zone_nodisp_pos_tbl[1][1])
        {
            if (main_work.cur_zone != 1)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[2][0] < dm_stgslct_f_zone_nodisp_pos_tbl[2][0]
            || main_work.zone_pos[2][1] > dm_stgslct_f_zone_nodisp_pos_tbl[2][1])
        {
            if (main_work.cur_zone != 2)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[3][0] < dm_stgslct_f_zone_nodisp_pos_tbl[3][0]
            || main_work.zone_pos[3][1] < dm_stgslct_f_zone_nodisp_pos_tbl[3][1])
        {
            if (main_work.cur_zone != 3)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[4][0] > dm_stgslct_f_zone_nodisp_pos_tbl[4][0]
            || main_work.zone_pos[4][1] < dm_stgslct_f_zone_nodisp_pos_tbl[4][1])
        {
            if (main_work.cur_zone != 4)
            {
                return FALSE;
            }
        }
    }
    else
    {
        if (main_work.zone_pos[0][0] > dm_stgslct_n_zone_nodisp_pos_tbl[0][0]
            || main_work.zone_pos[0][1] > dm_stgslct_n_zone_nodisp_pos_tbl[0][1])
        {
            if (main_work.cur_zone != 0)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[1][0] > dm_stgslct_n_zone_nodisp_pos_tbl[1][0]
            || main_work.zone_pos[1][1] < dm_stgslct_n_zone_nodisp_pos_tbl[1][1])
        {
            if (main_work.cur_zone != 1)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[2][0] < dm_stgslct_n_zone_nodisp_pos_tbl[2][0]
            || main_work.zone_pos[2][1] > dm_stgslct_n_zone_nodisp_pos_tbl[2][1])
        {
            if (main_work.cur_zone != 2)
            {
                return FALSE;
            }
        }
        if (main_work.zone_pos[3][0] < dm_stgslct_n_zone_nodisp_pos_tbl[3][0]
            || main_work.zone_pos[3][1] < dm_stgslct_n_zone_nodisp_pos_tbl[3][1])
        {
            if (main_work.cur_zone != 3)
            {
                return FALSE;
            }
        }
    }

    main_work.efct_out_flag = 0;


    if (!dmStgSlctIsEmeTableOutEfctEnd(main_work))
    {
        return FALSE;
    }

    return TRUE;
}


// ==========================================================================
// dmStgSlctSetStagePosInEfct
/*!
	ステージ選択時のステージテーブル入り演出中処理
 */
// ==========================================================================
void dmStgSlctSetStagePosInEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    float move_dist[2] = { 0, 0 };
    float distance[2] = { 0, 0 };
    u32 act_num_start = 0;
    u32 act_num_end = 0;

    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

    // 入り演出分の座標更新
    if (main_work.act_top_pos_x[act_num_start] > DMD_STGSLCT_ACT_TABLE_TOP_POS_X)
    {
        for (u32 i = act_num_start; i < act_num_end; i++)
        {
            distance[0] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X - DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;

            move_dist[0] = distance[0] / DMD_STGSLCT_ZONE_EFCT_TIME;

            main_work.act_top_pos_x[i] += move_dist[0];
        }
    }

    // 決定ZONEの移動演出分
    dmStgSlctSetDecideZonePosOutEfct(main_work);

    // ゲームモードテキストの入り演出分
    dmStgSlctSetModeTexInEfct(main_work);
}


// ==========================================================================
// dmStgSlctIsStagePosInEfct
/*!
	ステージ選択時のステージテーブル入り演出中処理
 */
// ==========================================================================
BOOL dmStgSlctIsStagePosInEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    //	u32 is_efct_end = 0;
    u32 act_num_start = 0;
    u32 act_num_end = 0;

    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

    if (!dmStgSlctIsModeTexInEfctEnd(main_work))
    {
        return FALSE;
    }

    // 掃け演出終了チェック
    if (main_work.act_top_pos_x[act_num_start] <= DMD_STGSLCT_ACT_TABLE_TOP_POS_X
        && main_work.zone_pos[main_work.cur_zone][0] < dm_stgslct_a_zone_nodisp_pos_tbl[0][0])
    {
        for (u32 i = act_num_start; i < act_num_end; i++)
        {
            main_work.act_top_pos_x[i] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;
        }
        main_work.zone_pos[main_work.cur_zone][0] = dm_stgslct_a_zone_nodisp_pos_tbl[0][0];

        return TRUE;
    }

    return FALSE;
}



// ==========================================================================
// dmStgSlctSetStagePosOutEfct
/*!
	ステージ選択時のステージテーブル掃け演出中処理
 */
// ==========================================================================
void dmStgSlctSetStagePosOutEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    float move_dist[2] = { 0, 0 };
    float distance[2] = { 0, 0 };
    u32 act_num_start = 0;
    u32 act_num_end = 0;

    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

    // 入り演出分の座標更新
    for (u32 i = act_num_start; i < act_num_end; i++)
    {
        //		if (main_work.efct_out_flag & (1 << i)) {
        distance[0] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X - DMD_STGSLCT_ACT_TABLE_TOP_POS_X;

        move_dist[0] = distance[0] / DMD_STGSLCT_ZONE_EFCT_TIME;

        main_work.act_top_pos_x[i] += move_dist[0];
        //		}
    }

    // ゲームモードテキストの掃け演出
    dmStgSlctSetModeTexOutEfct(main_work);
}



// ==========================================================================
// dmStgSlctIsStagePosOutEfct
/*!
	ステージ選択時のステージテーブル掃け演出中処理
 */
// ==========================================================================
BOOL dmStgSlctIsStagePosOutEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    //	u32 is_efct_end = 0;
    u32 act_num_start = 0;
    u32 act_num_end = 0;

    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

    // ゲームモードテキストの掃け演出終了チェック
    if (!dmStgSlctIsModeTexOutEfctEnd(main_work))
    {
        return FALSE;
    }

    // 掃け演出終了チェック
    if (main_work.act_top_pos_x[act_num_start] >= DMD_STGSLCT_STAGE_TAB_NODISP_POS_X)
    {
        for (u32 i = act_num_start; i < act_num_end; i++)
        {
            main_work.act_top_pos_x[i] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
        }
        //		main_work.act_top_pos_x[1] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
        //		main_work.act_top_pos_x[2] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
        //		main_work.act_top_pos_x[3] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;

        return TRUE;
    }

    return FALSE;
}



// ==========================================================================
// dmStgSlctSetZonePosInEfct
/*!
	ゾーン選択時のゾーンテーブル入り演出中処理
 */
// ==========================================================================
void dmStgSlctSetZonePosInEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    float move_dist[2] = { 0, 0 };
    float distance[2] = { 0, 0 };

    // 掃け演出分の座標更新
    for (int i = 0; i < DME_STGSLCT_ZONE_TYPE_NUM; i++)
    {
        if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
        {
            // ZONEテーブルの初期表示位置設定
            distance[0] = dm_stgslct_a_zone_disp_pos_tbl[i][0] - dm_stgslct_a_zone_nodisp_pos_tbl[i][0];
            distance[1] = dm_stgslct_a_zone_disp_pos_tbl[i][1] - dm_stgslct_a_zone_nodisp_pos_tbl[i][1];
        }

        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
        {
            // ZONEテーブルの初期表示位置設定
            distance[0] = dm_stgslct_s_zone_disp_pos_tbl[i][0] - dm_stgslct_s_zone_nodisp_pos_tbl[i][0];
            distance[1] = dm_stgslct_s_zone_disp_pos_tbl[i][1] - dm_stgslct_s_zone_nodisp_pos_tbl[i][1];
        }

        else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
        {
            // ZONEテーブルの初期表示位置設定
            distance[0] = dm_stgslct_f_zone_disp_pos_tbl[i][0] - dm_stgslct_f_zone_nodisp_pos_tbl[i][0];
            distance[1] = dm_stgslct_f_zone_disp_pos_tbl[i][1] - dm_stgslct_f_zone_nodisp_pos_tbl[i][1];
        }
        else
        {
            // ZONEテーブルの初期表示位置設定
            distance[0] = dm_stgslct_n_zone_disp_pos_tbl[i][0] - dm_stgslct_n_zone_nodisp_pos_tbl[i][0];
            distance[1] = dm_stgslct_n_zone_disp_pos_tbl[i][1] - dm_stgslct_n_zone_nodisp_pos_tbl[i][1];
        }

        move_dist[0] = distance[0] / DMD_STGSLCT_ZONE_EFCT_TIME;
        move_dist[1] = distance[1] / DMD_STGSLCT_ZONE_EFCT_TIME;

        main_work.zone_pos[i][0] += move_dist[0];
        main_work.zone_pos[i][1] += move_dist[1];
    }

    // エメラルドテーブルの入り演出設定
    dmStgSlctSetEmeTableInEfct(main_work);

    main_work.timer++;
}




// ==========================================================================
// dmStgSlctIsZonePosInEfct
/*!
	ゾーン選択時のゾーンテーブル入り演出中処理
 */
// ==========================================================================
BOOL dmStgSlctIsZonePosInEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 掃け演出終了チェック
    if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
    {
        if (main_work.zone_pos[0][0] < dm_stgslct_a_zone_disp_pos_tbl[0][0]
            || main_work.zone_pos[0][1] < dm_stgslct_a_zone_disp_pos_tbl[0][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[1][0] < dm_stgslct_a_zone_disp_pos_tbl[1][0]
            || main_work.zone_pos[1][1] < dm_stgslct_a_zone_disp_pos_tbl[1][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[2][0] > dm_stgslct_a_zone_disp_pos_tbl[2][0]
            || main_work.zone_pos[2][1] < dm_stgslct_a_zone_disp_pos_tbl[2][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[3][0] > dm_stgslct_a_zone_disp_pos_tbl[3][0]
            || main_work.zone_pos[3][1] > dm_stgslct_a_zone_disp_pos_tbl[3][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[4][0] < dm_stgslct_a_zone_disp_pos_tbl[4][0]
            || main_work.zone_pos[4][1] < dm_stgslct_a_zone_disp_pos_tbl[4][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[5][0] < dm_stgslct_a_zone_disp_pos_tbl[5][0]                // ◆
            || main_work.zone_pos[5][1] > dm_stgslct_a_zone_disp_pos_tbl[5][1])
        {
            return FALSE;
        }
    }
    else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
    {
        if (main_work.zone_pos[0][0] < dm_stgslct_s_zone_disp_pos_tbl[0][0]
            || main_work.zone_pos[0][1] < dm_stgslct_s_zone_disp_pos_tbl[0][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[1][0] < dm_stgslct_s_zone_disp_pos_tbl[1][0]
            || main_work.zone_pos[1][1] < dm_stgslct_s_zone_disp_pos_tbl[1][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[2][0] > dm_stgslct_s_zone_disp_pos_tbl[2][0]
            || main_work.zone_pos[2][1] < dm_stgslct_s_zone_disp_pos_tbl[2][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[3][0] > dm_stgslct_s_zone_disp_pos_tbl[3][0]
            || main_work.zone_pos[3][1] > dm_stgslct_s_zone_disp_pos_tbl[3][1])
        {
            return FALSE;
        }
        //		if (main_work.zone_pos[4][0] < dm_stgslct_s_zone_disp_pos_tbl[4][0]
        //			|| main_work.zone_pos[4][1] > dm_stgslct_s_zone_disp_pos_tbl[4][1]) {
        //			return FALSE;
        //		}
        if (main_work.zone_pos[5][0] < dm_stgslct_s_zone_disp_pos_tbl[5][0]
            || main_work.zone_pos[5][1] > dm_stgslct_s_zone_disp_pos_tbl[5][1])
        {
            return FALSE;
        }
    }
    else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
    {
        if (main_work.zone_pos[0][0] < dm_stgslct_f_zone_disp_pos_tbl[0][0]
            || main_work.zone_pos[0][1] < dm_stgslct_f_zone_disp_pos_tbl[0][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[1][0] < dm_stgslct_f_zone_disp_pos_tbl[1][0]
            || main_work.zone_pos[1][1] < dm_stgslct_f_zone_disp_pos_tbl[1][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[2][0] > dm_stgslct_f_zone_disp_pos_tbl[2][0]
            || main_work.zone_pos[2][1] < dm_stgslct_f_zone_disp_pos_tbl[2][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[3][0] > dm_stgslct_f_zone_disp_pos_tbl[3][0]
            || main_work.zone_pos[3][1] > dm_stgslct_f_zone_disp_pos_tbl[3][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[4][0] < dm_stgslct_f_zone_disp_pos_tbl[4][0]
            || main_work.zone_pos[4][1] > dm_stgslct_f_zone_disp_pos_tbl[4][1])
        {
            return FALSE;
        }
    }
    else
    {
        if (main_work.zone_pos[0][0] < dm_stgslct_n_zone_disp_pos_tbl[0][0]
            || main_work.zone_pos[0][1] < dm_stgslct_n_zone_disp_pos_tbl[0][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[1][0] < dm_stgslct_n_zone_disp_pos_tbl[1][0]
            || main_work.zone_pos[1][1] > dm_stgslct_n_zone_disp_pos_tbl[1][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[2][0] > dm_stgslct_n_zone_disp_pos_tbl[2][0]
            || main_work.zone_pos[2][1] < dm_stgslct_n_zone_disp_pos_tbl[2][1])
        {
            return FALSE;
        }
        if (main_work.zone_pos[3][0] > dm_stgslct_n_zone_disp_pos_tbl[3][0]
            || main_work.zone_pos[3][1] > dm_stgslct_n_zone_disp_pos_tbl[3][1])
        {
            return FALSE;
        }
    }

    // エメラルドテーブルの入り演出が終了したかどうか
    if (!dmStgSlctIsEmeTableInEfctEnd(main_work))
    {
        return FALSE;
    }


    return TRUE;
}



// ==========================================================================
// dmStgSlctSetDecideZonePosOutEfct
/*!
	ゾーン選択時の決定ゾーンテーブル掃け演出中処理
 */
// ==========================================================================
void dmStgSlctSetDecideZonePosOutEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    float move_dist[2] = { 0, 0 };
    float distance[2] = { 0, 0 };

    // 入り演出が終了したら
    //	if (dmStgSlctIsZonePosOutEfct(main_work)) {
    //		main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;
    //	}

    // 掃け演出分の座標更新
    if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
    {
        // ZONEテーブルの初期表示位置設定
        distance[0] = dm_stgslct_a_zone_nodisp_pos_tbl[0][0] - dm_stgslct_a_zone_disp_pos_tbl[main_work.cur_zone][0];
    }

    else if (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN)
    {
        // ZONEテーブルの初期表示位置設定
        distance[0] = dm_stgslct_s_zone_nodisp_pos_tbl[0][0] - dm_stgslct_s_zone_disp_pos_tbl[main_work.cur_zone][0];
    }

    else if (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN)
    {
        // ZONEテーブルの初期表示位置設定
        distance[0] = dm_stgslct_f_zone_nodisp_pos_tbl[0][0] - dm_stgslct_f_zone_disp_pos_tbl[main_work.cur_zone][0];
    }
    else
    {
        // ZONEテーブルの初期表示位置設定
        distance[0] = dm_stgslct_n_zone_nodisp_pos_tbl[0][0] - dm_stgslct_n_zone_disp_pos_tbl[main_work.cur_zone][0];
    }

    move_dist[0] = distance[0] / DMD_STGSLCT_ZONE_EFCT_TIME;

    main_work.zone_pos[main_work.cur_zone][0] += move_dist[0];

    // 決定ZONEの移動演出分

}



// ==========================================================================
// dmStgSlctSetStageZoneChangeEfct
/*!
	ステージ選択時のゾーン切り替え時演出中処理
 */
// ==========================================================================
void dmStgSlctSetStageZoneChangeEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    float move_dist[2] = { 0, 0 };
    float distance[2] = { 0, 0 };
    //	float move_src = 0;
    //	float move_dest = 0;
    u32 act_num_start = 0;
    u32 act_num_end = 0;

    // 画面外へ掃ける側
    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.chng_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.chng_zone][1] + act_num_start;

    // 入り演出分の座標更新
    for (u32 i = act_num_start; i < act_num_end; i++)
    {

        distance[0] = main_work.act_move_dest[0] - main_work.act_move_src[0];

        move_dist[0] = distance[0] / DMD_STGSLCT_ZONE_EFCT_TIME;

        main_work.act_top_pos_x[i] += move_dist[0];
    }

    // 画面中央へ入る側
    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

    // 入り演出分の座標更新
    for (u32 i = act_num_start; i < act_num_end; i++)
    {

        distance[1] = main_work.act_move_dest[1] - main_work.act_move_src[1];

        move_dist[1] = distance[1] / DMD_STGSLCT_ZONE_EFCT_TIME;

        main_work.act_top_pos_x[i] += move_dist[1];
    }
}



// ==========================================================================
// dmStgSlctIsStageZoneChangeEfct
/*!
	ステージ選択時のゾーン切り替え時演出中処理
 */
// ==========================================================================
BOOL dmStgSlctIsStageZoneChangeEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    u8 result = 0;
    //	u32 is_efct_end = 0;
    u32 act_num_start = 0;
    u32 act_num_end = 0;

    // 画面外へ掃ける側のチェック
    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.chng_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.chng_zone][1] + act_num_start;

    // 掃け演出終了チェック
    if (main_work.act_move_dest[0] > 0)
    {
        if (main_work.act_top_pos_x[act_num_start] >= main_work.act_move_dest[0])
        {
            for (u32 i = act_num_start; i < act_num_end; i++)
            {
                main_work.act_top_pos_x[i] = main_work.act_move_dest[0];
            }

            result |= 1 << 0;
        }

        // 画面外へ掃ける側のチェック
        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        if (main_work.act_top_pos_x[act_num_start] >= main_work.act_move_dest[1])
        {
            for (u32 i = act_num_start; i < act_num_end; i++)
            {
                main_work.act_top_pos_x[i] = main_work.act_move_dest[1];
            }

            result |= 1 << 1;
        }
    }
    else if (main_work.act_move_dest[0] < 0)
    {
        if (main_work.act_top_pos_x[act_num_start] <= main_work.act_move_dest[0])
        {
            for (u32 i = act_num_start; i < act_num_end; i++)
            {
                main_work.act_top_pos_x[i] = main_work.act_move_dest[0];
            }

            result |= 1 << 0;
        }

        // 画面外へ掃ける側のチェック
        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        if (main_work.act_top_pos_x[act_num_start] <= main_work.act_move_dest[1])
        {
            for (u32 i = act_num_start; i < act_num_end; i++)
            {
                main_work.act_top_pos_x[i] = main_work.act_move_dest[1];
            }

            result |= 1 << 1;
        }
    }

    if (result != 3)
    {
        return FALSE;
    }

    return TRUE;
}



// ==========================================================================
// dmStgSlctSetStageVrtclChangeEfct
/*!
	ステージ選択時の縦のACT切り替え時演出中処理
 */
// ==========================================================================
void dmStgSlctSetStageVrtclChangeEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    float move_dist[2] = { 0, 0 };
    float distance[2] = { 0, 0 };
    //	float move_src = 0;
    //	float move_dest = 0;
    u32 act_num_start = 0;
    u32 act_num_end = 0;

    // 画面外へ掃ける側
    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

    // 縦移動する場合
    for (u32 i = act_num_start; i < act_num_end; i++)
    {

        distance[1] = main_work.act_move_pos_dst[i] - main_work.act_move_pos_src[i];

        move_dist[1] = distance[1] / DMD_STGSLCT_ACT_VRTCL_MOVE_TIME;

        main_work.act_top_pos_y[i] += move_dist[1];
    }
}



// ==========================================================================
// dmStgSlctIsStageVrtclChangeEfct
/*!
	ステージ選択時の縦のACT切り替え時演出中処理
 */
// ==========================================================================
BOOL dmStgSlctIsStageVrtclChangeEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    u32 act_num_start = 0;
    u32 act_num_end = 0;

    // 画面外へ掃ける側のチェック
    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

    // 掃け演出終了チェック
    if (main_work.timer >= DMD_STGSLCT_ACT_VRTCL_MOVE_TIME)
    {

        for (u32 i = act_num_start; i < act_num_end; i++)
        {
            main_work.act_top_pos_y[i] = main_work.act_move_pos_dst[i];
        }

        return TRUE;
    }

    return FALSE;
}




// ==========================================================================
// dmStgSlctSetStageCrsrChangeEfct
/*!
	ステージ選択時の縦のACT切り替え時演出中処理
 */
// ==========================================================================
void dmStgSlctSetStageCrsrChangeEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    float move_dist = 0;
    float distance = 0;

    // 縦移動する場合
    distance = main_work.crsr_move_dst - main_work.crsr_move_src;

    move_dist = distance / DMD_STGSLCT_CRSR_MOVE_TIME;

    main_work.crsr_pos_y += move_dist;
}



// ==========================================================================
// dmStgSlctIsStageCrsrChangeEfct
/*!
	ステージ選択時の縦のACT切り替え時演出中処理
 */
// ==========================================================================
BOOL dmStgSlctIsStageCrsrChangeEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 掃け演出終了チェック
    if (main_work.timer >= DMD_STGSLCT_CRSR_MOVE_TIME)
    {
        main_work.crsr_pos_y = main_work.crsr_move_dst;

        return TRUE;
    }

    return FALSE;
}




// ==========================================================================
// dmStgSlctSetWinOpenEfct
/*!
	ウインドウ入り演出時のウインドウサイズ設定処理
 */
// ==========================================================================
void dmStgSlctSetWinOpenEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
#if _IPHONE
	if ((0.0f <= main_work.win_timer) && (main_work.win_timer < 1.0f)) {
		DmSoundPlaySE("Window");
	}
#endif //_IPHONE
    if (main_work.win_timer > DMD_STGSLCT_WIN_EFCT_TIME)
    {
        // ウインドウ演出終了
        main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT_END;

        main_work.win_timer = 0.0f;

        for (u32 i = 0; i < 2; i++)
        {
            main_work.win_size_rate[i] = DMD_STGSLCT_WIN_DEF_RATE;
        }
    }
    else
    {
        // タイマー更新(但しフレームレートが可変になるように修正すること)	◆
        main_work.win_timer++;
    }

    // 掃け演出分のサイズ更新
    for (u32 i = 0; i < 2; i++)
    {
        if (main_work.win_timer)
        {
            main_work.win_size_rate[i] = main_work.win_timer / DMD_STGSLCT_WIN_EFCT_TIME;
        }
        else
        {
            main_work.win_size_rate[i] = 1.0f;
        }

        if (main_work.win_size_rate[i] > 1.0f)
        {
            main_work.win_size_rate[i] = 1.0f;
        }
    }



}



// ==========================================================================
// dmStgSlctSetWinCloseEfct
/*!
	ウインドウ閉め演出時のウインドウサイズ設定処理
 */
// ==========================================================================
void dmStgSlctSetWinCloseEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{

    // 掃け演出分のサイズ更新
    for (u32 i = 0; i < 2; i++)
    {
        if (main_work.win_timer)
        {
            main_work.win_size_rate[i] = main_work.win_timer / DMD_STGSLCT_WIN_EFCT_TIME;
        }
        else
        {
            main_work.win_size_rate[i] = 0.0f;
        }
    }

    if (main_work.win_timer < 0.0f)
    {
        // ウインドウ演出終了
        main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT_END;

        main_work.win_timer = 0.0f;

        for (u32 i = 0; i < 2; i++)
        {
            main_work.win_size_rate[i] = 0.0f;
        }
    }
    else
    {
        // タイマー更新(但しフレームレートが可変になるように修正すること)	◆
        main_work.win_timer--;
    }
}



// ==========================================================================
// dmStgSlctSetActChngZonePosInit
/*!
	ACT移動演出時の座標初期化設定処理
 */
// ==========================================================================
void dmStgSlctSetActChngZonePosInit(DMS_STGSLCT_MAIN_WORK* main_work, s32 diff)
{
    u32 act_num_start = 0;
    u32 act_num_end = 0;
    u32 i = 0;

    // 画面外へ掃ける側
    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.chng_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.chng_zone][1] + act_num_start;

    // 入り演出分の座標更新
    for (i = act_num_start; i < act_num_end; i++)
    {
        main_work.act_top_pos_x[i] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;
    }

    main_work.act_move_src[0] = main_work.act_top_pos_x[act_num_start];

    // 画面中央へ入る側
    act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

    // 入り演出分の座標更新
    for (i = act_num_start; i < act_num_end; i++)
    {
        if (diff > 0)
        {
            main_work.act_top_pos_x[i] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
            main_work.act_top_pos_y[i] = dm_stgslct_act_disp_y_pos_tbl[i];
            //											+ (float)(dm_stgslct_act_disp_pos_y_tbl[i] * DMD_STGSLCT_ACT_TABLE_DIST_Y);
        }
        else
        {
            main_work.act_top_pos_x[i] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X * (-1);
            main_work.act_top_pos_y[i] = dm_stgslct_act_disp_y_pos_tbl[i];
            //											+ (float)(dm_stgslct_act_disp_pos_y_tbl[i] * DMD_STGSLCT_ACT_TABLE_DIST_Y);
        }
    }

    main_work.act_move_src[1] = main_work.act_top_pos_x[act_num_start];
}



// ==========================================================================
// dmStgSlctSetFocusChangeEfctData
/*!
	ステージ選択時の縦のACT切り替え時の設定処理
 */
// ==========================================================================
void dmStgSlctSetFocusChangeEfctData(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 上移動・下移動共に必要な変数の設定
    s32 chng_sign = 0;
    s32 disp_act_no = 0;
    u32 src = 0;
    u32 dst = 0;
    u32 act_comp = 0;

    src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
    dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];
    act_comp = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] - 1;

    if (main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR)
    {
        chng_sign = -1;
    }
    else if (main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR)
    {
        chng_sign = 1;
    }
#if !_IPHONE
    else
    {
        MTM_ASSERT(0);
    }
#endif //!_IPHONE

    // 選択ファイル切り替え
    main_work.prev_stage = main_work.cur_stage;

    main_work.prev_disp_no = main_work.focus_disp_no;
    //	main_work.crsr_prev_idx = main_work.crsr_idx;

    if (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL)
    {

#if !_IPHONE
        main_work.prev_disp_no = main_work.focus_disp_no;
        main_work.focus_disp_no = dmStgSlctGetRevisedStageVrtclNo(main_work.focus_disp_no
                                                                   , chng_sign
                                                                   , (s32)main_work.cur_zone
                                                                   , main_work.crsr_idx
                                                                   );

        for (u32 i = src; i < dst; i++)
        {
            main_work.act_move_pos_src[i] = main_work.act_top_pos_y[i];
            main_work.act_move_pos_dst[i] = dm_stgslct_act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
        }
#else //!_IPHONE
		switch (main_work.cur_zone) {
		case DME_STGSLCT_ZONE_TYPE_FINAL: //ファイナルステージ
			for (u32 i = src; i < dst; i++) {
				main_work.act_move_pos_src[i] = main_work.act_top_pos_y[i];
				main_work.act_move_pos_dst[i] = dm_stgslct_act_disp_y_pos_tbl[i];
			}
			break;
		default:
			for (u32 i = src; i < dst; i++) {
				main_work.act_move_pos_src[i] = main_work.act_top_pos_y[i];
				main_work.act_move_pos_dst[i] = dm_stgslct_act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
			}
			break;
		}
#endif //!_IPHONE

        if (main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR
            && (main_work.prev_disp_no == 0 && main_work.focus_disp_no == 4 - 1))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
        }

        if (main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR
            && (main_work.prev_disp_no == 4 - 1 && main_work.focus_disp_no == 0))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
        }

        if ((main_work.focus_disp_no == 3 && main_work.prev_disp_no == 0)
            || (main_work.focus_disp_no == 0 && main_work.prev_disp_no == 3))
        {
            main_work.crsr_prev_idx = main_work.crsr_idx;

            disp_act_no = main_work.prev_disp_no;

            main_work.crsr_idx = dmStgSlctGetRevisedStageCrsrNo(main_work.crsr_idx
                                                                 , chng_sign
                                                                 , (s32)main_work.cur_zone
                                                                 , main_work.prev_disp_no
                                                                 );

            main_work.crsr_move_src = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_prev_idx];
            main_work.crsr_move_dst = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_idx];

            main_work.flag &= ~DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
        }
        else if (main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR
                 && main_work.focus_disp_no == 0
                 && main_work.prev_disp_no == 0)
        {

        }
        else if (main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR
                 && main_work.focus_disp_no == 3
                 && main_work.prev_disp_no == 3)
        {

        }
        else
        {
            main_work.flag &= ~DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
        }

    }

    if (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_CRSR)
    {

        main_work.crsr_prev_idx = main_work.crsr_idx;

        disp_act_no = main_work.prev_disp_no;

        main_work.crsr_idx = dmStgSlctGetRevisedStageCrsrNo(main_work.crsr_idx
                                                             , chng_sign
                                                             , (s32)main_work.cur_zone
                                                             , main_work.prev_disp_no
                                                             );

        main_work.crsr_move_src = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_prev_idx];
        main_work.crsr_move_dst = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_idx];
    }

    main_work.cur_stage = (s32)(main_work.crsr_idx + main_work.focus_disp_no + src);
}




// ===========================================================================
//	dmStgSlctGetRevisedStageNo
/*!
	項目の場所を示す変数が最大・最小値を超えた際に補正した(回り込みさせた)値を取得する関数
	通常時のカーソル移動の選択項目に使用。

	@param id		[in] 項目番号
	@param diff		[in] 変化量(移動における)
	@return 補正された項目番号
*/
// ===========================================================================
/*
s32 dmStgSlctGetRevisedStageNo(s32 idx, s32 diff, s32 zone_no)
{
	s32 result;
	s32 src = 0;
	s32 dst = 0;
	
	result = (int)idx + diff;

	src = dm_stgslct_zone_act_num_tbl[zone_no][0];
	dst = src + dm_stgslct_zone_act_num_tbl[zone_no][1];
	
	// 先頭から一つ戻ると最後に移動
	if (result < src) {
		result = (int)(dst - 1);
	}
	
	// 最後から一つ進むと先頭に移動
	if (result >= dst) {
		result = src;
	}
	
	MTM_ASSERT(result >= src && result < dst);
	
	return result;
}
*/


// ===========================================================================
//	dmStgSlctGetRevisedStageVrtclNo
/*!
	項目の場所を示す変数が最大・最小値を超えた際に補正した(回り込みさせた)値を取得する関数
	通常時のカーソル移動の選択項目に使用。

	@param id		[in] 項目番号
	@param diff		[in] 変化量(移動における)
	@return 補正された項目番号
*/
// ===========================================================================
s32 dmStgSlctGetRevisedStageVrtclNo(s32 idx, s32 diff, s32 zone_no, s32 crsr_idx)
{
    s32 result;
    UNREFERENCED_PARAMETER(zone_no);

    result = (int)idx + diff;

    // 先頭から一つ戻ると最後に移動
    if (result < 0)
    {
        if (crsr_idx == 0)
        {
            result = 3;
        }
        else
        {
            result = 0;
        }
    }

    // 最後から一つ進むと先頭に移動
    if (result >= 4)
    {
        if (crsr_idx == 4 - 1)
        {
            result = 0;
        }
        else
        {
            result = 4 - 1;
        }
    }

    MTM_ASSERT(result >= 0 && result < 4);

    return result;
}



// ===========================================================================
//	dmStgSlctGetRevisedStageCrsrNo
/*!
	項目の場所を示す変数が最大・最小値を超えた際に補正した(回り込みさせた)値を取得する関数
	通常時のカーソル移動の選択項目に使用。

	@param id		[in] 項目番号
	@param diff		[in] 変化量(移動における)
	@return 補正された項目番号
*/
// ===========================================================================
s32 dmStgSlctGetRevisedStageCrsrNo(s32 idx, s32 diff, s32 zone_no, s32 disp_act)
{
    s32 result;
    s32 tmp_no = 0;
    u32 src = 0;
    u32 dst = 0;

    src = dm_stgslct_zone_act_num_tbl[zone_no][0];
    dst = src + dm_stgslct_zone_act_num_tbl[zone_no][1];
    result = (int)idx + diff;

    tmp_no = (s32)(dm_stgslct_zone_act_num_tbl[zone_no][1] - 4);

    if (tmp_no < 0)
    {
        tmp_no = 0;
    }

    // 先頭から一つ戻ると最後に移動
    if (result < 0)
    {
        if (disp_act == 0)
        {
            result = 3;
        }
        else
        {
            result = 0;
        }
    }

    // 最後から一つ進むと先頭に移動
    if (result > 3)
    {
        if (disp_act == tmp_no)
        {
            result = 0;
        }
        else
        {
            result = 3;
        }
    }

    MTM_ASSERT(result >= 0 && result <= 3);

    return result;
}



// ===========================================================================
//	dmStgSlctGetRevisedZoneNo
/*!
	項目の場所を示す変数が最大・最小値を超えた際に補正した(回り込みさせた)値を取得する関数
	通常時のカーソル移動の選択項目に使用。

	@param id		[in] 項目番号
	@param diff		[in] 変化量(移動における)
	@return 補正された項目番号
*/
// ===========================================================================
s32 dmStgSlctGetRevisedZoneNo(s32 idx, s32 diff, s32 is_final_open, s32 is_spe_open)
{
    s32 result;

    // FINALオープンならば
    if (is_final_open)
    {
        result = (int)idx + diff;

        // FINAL、スペステ共にオープン
        if (is_spe_open)
        {
            // 先頭から一つ戻ると最後に移動
            if (result < DME_STGSLCT_ACT_PAGE_1)
            {
                result = (int)(DME_STGSLCT_ACT_PAGE_NUM - 1);
            }

            // 最後から一つ進むと先頭に移動
            if (result >= DME_STGSLCT_ACT_PAGE_NUM)
            {
                result = DME_STGSLCT_ACT_PAGE_1;
            }
        }
        // FINALのみオープン
        else
        {
            // 先頭から一つ戻ると最後に移動
            if (result < DME_STGSLCT_ACT_PAGE_1)
            {
                result = (int)(DME_STGSLCT_ACT_PAGE_FINAL);
            }

            // 最後から一つ進むと先頭に移動
            if (result > DME_STGSLCT_ACT_PAGE_FINAL)
            {
                result = DME_STGSLCT_ACT_PAGE_1;
            }
        }
    }
    // スペステのみオープン
    else if (is_spe_open)
    {
        result = (int)idx + diff;

        if (result == DME_STGSLCT_ACT_PAGE_FINAL)
        {
            if (diff > 0)
            {
                result = DME_STGSLCT_ACT_PAGE_SPE;
            }
            else
            {
                result = DME_STGSLCT_ACT_PAGE_4;
            }
        }

        // 先頭から一つ戻ると最後に移動
        if (result < DME_STGSLCT_ACT_PAGE_1)
        {
            result = (int)(DME_STGSLCT_ACT_PAGE_NUM - 1);
        }

        // 最後から一つ進むと先頭に移動
        if (result >= DME_STGSLCT_ACT_PAGE_NUM)
        {
            result = DME_STGSLCT_ACT_PAGE_1;
        }
    }
    // FINAL,スペステ共にオープンなし
    else
    {
        result = (int)idx + diff;

        // 先頭から一つ戻ると最後に移動
        if (result < DME_STGSLCT_ACT_PAGE_1)
        {
            result = (int)(DME_STGSLCT_ACT_PAGE_4);
        }

        // 最後から一つ進むと先頭に移動
        if (result > DME_STGSLCT_ACT_PAGE_4)
        {
            result = DME_STGSLCT_ACT_PAGE_1;
        }
    }

    MTM_ASSERT(result >= DME_STGSLCT_ACT_PAGE_1 && result < DME_STGSLCT_ACT_PAGE_NUM);

    return result;
}



// ==========================================================================
// dmStgSlctSetTableActiveInfo
/*!
	各ACTテーブルのアクティブ情報設定処理
 */
// ==========================================================================
void dmStgSlctSetTableActiveInfo(DMS_STGSLCT_MAIN_WORK* main_work, u32 cnt)
{
    // 台紙の非アクティブ設定
    if (main_work.is_clear_stage[cnt] < 0)
    {
        for (u32 j = ACT_TAB_TABLE2; j <= ACT_TAB_TABLE3; j++)
        {
            AoActSetFrame(main_work.act[j], 1.f);
        }
        AoActSetFrame(main_work.act[ACT_TAB_SCR], 17.f);
        AoActSetFrame(main_work.act[ACT_TAB_MESS], 17.f);
    }
    // アクティブ
    else
    {
        for (u32 j = ACT_TAB_TABLE2; j <= ACT_TAB_TABLE3; j++)
        {
            AoActSetFrame(main_work.act[j], 0.f);
        }
        AoActSetFrame(main_work.act[ACT_TAB_SCR], dm_stgslct_disp_msg_id_table_tbl[cnt]);
        AoActSetFrame(main_work.act[ACT_TAB_MESS], dm_stgslct_disp_msg_id_table_tbl[cnt]);

        // スコアアタックをクリアしていない場合はタイムアタックを非アクティブ設定
        if (main_work.hi_score[cnt] == DMD_STGSLCT_INIT_SCORE_NUM
            && main_work.cur_game_mode == DME_STGSLCT_PLAY_MODE_TIME_ATK)
        {
            for (u32 j = ACT_TAB_TABLE2; j <= ACT_TAB_TABLE3; j++)
            {
                AoActSetFrame(main_work.act[j], 1.f);
            }
        }
        else
        {
            for (u32 j = ACT_TAB_TABLE2; j <= ACT_TAB_TABLE3; j++)
            {
                AoActSetFrame(main_work.act[j], 0.f);
            }
        }
    }

}



// ==========================================================================
// dmStgSlctIsCanSelectAct
/*!
	決定したステージがプレイ可能かどうかを返す処理
 */
// ==========================================================================
BOOL dmStgSlctIsCanSelectAct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // 解放されていないステージの場合
    //	if (main_work.cur_stage <= GSD_MAIN_STAGE_ID_FINAL_1) {
    if (main_work.is_clear_stage[main_work.cur_stage] == -1)
    {
        main_work.flag &= ~DMD_STGSLCT_FLAG_DECIDE;

        return FALSE;
    }
    //	}

    // スコアアタックをクリアしていないステージでタイムアタックを選ぼうとした場合
    if (main_work.hi_score[main_work.cur_stage] == DMD_STGSLCT_INIT_SCORE_NUM
        && main_work.cur_game_mode == DME_STGSLCT_PLAY_MODE_TIME_ATK)
    {

        return FALSE;
    }

    return TRUE;
}



// ===========================================================================
//	dmStgSlctGetClipStageNoForChngZone
/*!
	項目の場所を示す変数が最大・最小値を超えた際に補正した(回り込みさせた)値を取得する関数
	通常時のカーソル移動の選択項目に使用。

	@param id		[in] 項目番号
	@param diff		[in] 変化量(移動における)
	@return 補正された項目番号
*/
// ===========================================================================
/*
s32 dmStgSlctGetClipStageNoForChngZone(s32 prev_stage, s32 cur_zone, s32 prev_zone)
{
	s32 result;
	s32 prev_src = 0;
	s32 prev_dst = 0;
	s32 cur_src = 0;
	s32 cur_dst = 0;

	s32 up_list_no = 0;
	
	prev_src = dm_stgslct_zone_act_num_tbl[prev_zone][0];
	prev_dst = prev_src + dm_stgslct_zone_act_num_tbl[prev_zone][1];
	cur_src = dm_stgslct_zone_act_num_tbl[cur_zone][0];
	cur_dst = cur_src + dm_stgslct_zone_act_num_tbl[cur_zone][1];

	up_list_no = prev_stage - prev_src;

	result = up_list_no + cur_src;
	
	if (result >= cur_dst) {
		result = cur_dst - 1;		// 暫定
	}

	// 保険
	if (result < cur_src) {
		result = cur_src;
	}
	
	MTM_ASSERT(result >= cur_src && result < cur_dst);
	
	return result;
}
*/


// ==========================================================================
// dmStgSlctSetObiEfctPos
/*!
	帯アクションの演出用座標設定処理
 */
// ==========================================================================
void dmStgSlctSetObiEfctPos(DMS_STGSLCT_MAIN_WORK* main_work)
{
    for (u32 i = 0; i < 2; i++)
    {

        // 一定位置に来たら戻る
        if (main_work.obi_pos[i] < DMD_STGSLCT_OBI_MOVE_END_POS)
        {
            main_work.obi_pos[i] = DMD_STGSLCT_OBI_MOVE_START_POS;
        }

        // 移動分座標加算
        main_work.obi_pos[i] += DMD_STGSLCT_OBI_MOVE_SPEED;
    }
}



// ==========================================================================
// dmStgSlctMakeVertexAct
/*!
	ACTの上に被さる部分の背景描画タスク作成処理
 */
// ==========================================================================
void dmStgSlctMakeVertexAct(DMS_STGSLCT_MAIN_WORK* main_work, AMS_PARAM_DRAW_PRIMITIVE* param)
{
    f32 n = 0.f;

    // フォグ設定
    if (main_work.bg_fade.a > 0)
    {
        amDrawSetFogColor(120
                          , 1.0f
                          , 1.0f
                          , 1.0f
                          );

        n = 2.0f - ((f32)main_work.bg_fade.a / 255.f);

        amDrawSetFogRange(120, n, n + 1.0f);
        amDrawSetFog(120, 1);
    }
    else
    {
        amDrawSetFog(120, 0);
    }


    param.mtx = NULL;
    param.vtxPCT3D = (NNS_PRIM3D_PCT*)amDrawMallocDataBuffer(sizeof(NNS_PRIM3D_PCT) * 6);
    NNS_PRIM3D_PCT* v = param.vtxPCT3D;

    v[0].Pos.x = v[2].Pos.x = v[4].Pos.x = -160.0f;
    v[1].Pos.x = v[3].Pos.x = v[5].Pos.x = v[0].Pos.x + 1024.f;// + 1280.0f;
    v[0].Pos.y = v[1].Pos.y = 0.0f;
    v[2].Pos.y = v[3].Pos.y = v[0].Pos.y + 64.0f;
    v[4].Pos.y = v[5].Pos.y = v[0].Pos.y + 128.0f;
    v[0].Pos.z = v[1].Pos.z = v[2].Pos.z = v[3].Pos.z = v[4].Pos.z = v[5].Pos.z = -2.0f;
    v[0].Col = v[2].Col = AMD_RGBA8888(255, 255, 255, 255);
    v[1].Col = v[3].Col = AMD_RGBA8888(255, 255, 255, 255);
    v[4].Col = v[5].Col = AMD_RGBA8888(255, 255, 255, 0);

    v[0].Tex.u = v[2].Tex.u = v[4].Tex.u = 0.0f;
    v[1].Tex.u = v[3].Tex.u = v[5].Tex.u = 1.0f;
    v[0].Tex.v = v[1].Tex.v = 0.0f;
    v[2].Tex.v = v[3].Tex.v = 0.125f;
    v[4].Tex.v = v[5].Tex.v = 0.25f;


    param.format3D = NNE_PRIM3D_FMT_PCT;
    param.type = NNE_PRIM_TRIANGLE_STRIP;
    param.count = 6;

    param.texlist = AoTexGetTexList(&main_work.tex[0]);
#if !_IPHONE
    param.texId = (s32)(42 + main_work.cur_bg_id);
#else //!_IPHONE
	const u32 c_bg_tex_idx = 35; //テクスチャの並びが変わると、此処も変更しなければ成らない

	if (u32(-1) == main_work.cur_bg_id) {
		//ゾーンセレクト用特別背景なら透明にする
		for (int i = 0; i < 6; ++i) {
			u32 &color = v[i].Col;
			color &= 0xFFFFFF00;
		}
		param.texlist = NULL;
		param.texId = -1;
	} else {
		param.texId = (s32)(c_bg_tex_idx + main_work.cur_bg_id);
	}
#endif //!_IPHONE
    param.ablend = NNE_PRIM_ALPHABLEND_ON;
    param.zOffset = -1.0f;

    param.uwrap = NNE_PRIM_TEXWRAP_CLAMP;
    param.vwrap = NNE_PRIM_TEXWRAP_CLAMP;

    // ソート設定
    param.sortZ = 0.0f;

#if _PC | _XBOX
	param.bldSrc = NNE_BLENDMODE_SRCALPHA;
	param.bldDst = NNE_BLENDMODE_INVSRCALPHA;
	param.bldMode = NNE_BLENDOP_ADD;
#elif _PS3
	param.bldSrc = NND_BLENDFUNC_PS3_SRC_ALPHA;
	param.bldDst = NND_BLENDFUNC_PS3_ONE_MINUS_SRC_ALPHA;
	param.bldMode = NND_BLENDOP_PS3_FUNC_ADD;
#elif _WII
	param.bldSrc = GX_BL_SRCALPHA;
	param.bldDst = GX_BL_INVSRCALPHA;
	param.bldMode = GX_BM_BLEND;
#endif
    param.aTest = 0;
    param.zMask = 1;
    param.zTest = 0;

    // ソートしない
    param.noSort = 1;

    AoActDrawCorWide(v, 6, AOD_ACT_CORW_CENTER);

    amDrawPrimitive3D(120, param);




    param.mtx = NULL;
    param.vtxPCT3D = (NNS_PRIM3D_PCT*)amDrawMallocDataBuffer(sizeof(NNS_PRIM3D_PCT) * 6);
    v = param.vtxPCT3D;

    v[0].Pos.x = v[2].Pos.x = v[4].Pos.x = -160.0f;
    v[1].Pos.x = v[3].Pos.x = v[5].Pos.x = v[0].Pos.x + 1024.f;//1280.0f;
    v[0].Pos.y = v[1].Pos.y = 720.0f;
    v[2].Pos.y = v[3].Pos.y = v[0].Pos.y - 128.0f;
    v[4].Pos.y = v[5].Pos.y = v[0].Pos.y - 192.0f;
    v[0].Pos.z = v[1].Pos.z = v[2].Pos.z = v[3].Pos.z = v[4].Pos.z = v[5].Pos.z = -2.0f;
    v[0].Col = v[2].Col = AMD_RGBA8888(255, 255, 255, 255);
    v[1].Col = v[3].Col = AMD_RGBA8888(255, 255, 255, 255);
    v[4].Col = v[5].Col = AMD_RGBA8888(255, 255, 255, 0);

    v[0].Tex.u = v[2].Tex.u = v[4].Tex.u = 0.0f;
    v[1].Tex.u = v[3].Tex.u = v[5].Tex.u = 1.0f;
    v[0].Tex.v = v[1].Tex.v = 0.8125f;
    v[2].Tex.v = v[3].Tex.v = 0.3125f;
    v[4].Tex.v = v[5].Tex.v = 0.0625f;

    param.format3D = NNE_PRIM3D_FMT_PCT;
    param.type = NNE_PRIM_TRIANGLE_STRIP;
    param.count = 6;

    param.texlist = AoTexGetTexList(&main_work.tex[0]);
#if !_IPHONE
    param.texId = (s32)(48 + main_work.cur_bg_id);
#else //!_IPHONE
	if (u32(-1) == main_work.cur_bg_id) {	//ゾーンセレクト用特別背景なら透明にする
		for (int i = 0; i < 6; ++i) {
			u32 &color = v[i].Col;
			color &= 0xFFFFFF00;
		}
		param.texlist = NULL;
		param.texId = -1;
	} else {
		param.texId = (s32)(c_bg_tex_idx + 6 + main_work.cur_bg_id);
	}
#endif //!_IPHONE
    param.ablend = NNE_PRIM_ALPHABLEND_ON;
    param.zOffset = -1.0f;

    param.uwrap = NNE_PRIM_TEXWRAP_CLAMP;
    param.vwrap = NNE_PRIM_TEXWRAP_CLAMP;

    // ソート設定
    param.sortZ = 0.0f;


#if _PC | _XBOX
	param.bldSrc = NNE_BLENDMODE_SRCALPHA;
	param.bldDst = NNE_BLENDMODE_INVSRCALPHA;
	param.bldMode = NNE_BLENDOP_ADD;
#elif _PS3
	param.bldSrc = NND_BLENDFUNC_PS3_SRC_ALPHA;
	param.bldDst = NND_BLENDFUNC_PS3_ONE_MINUS_SRC_ALPHA;
	param.bldMode = NND_BLENDOP_PS3_FUNC_ADD;
#elif _WII
	param.bldSrc = GX_BL_SRCALPHA;
	param.bldDst = GX_BL_INVSRCALPHA;
	param.bldMode = GX_BM_BLEND;
#endif
    param.aTest = 0;
    param.zMask = 1;
    param.zTest = 0;

    // ソートしない
    param.noSort = 1;


    AoActDrawCorWide(v, 6, AOD_ACT_CORW_CENTER);


    amDrawPrimitive3D(120, param);

    // 
    main_work.tex_u[0] -= 0.024f;
    main_work.tex_u[1] -= 0.024f;
    main_work.tex_v[0] -= 0.006f;
    main_work.tex_v[1] -= 0.006f;


    amDrawMakeTask(dmStgSlctDrawVertexAct, (u16)(0x2800), (u32)0);



    param.mtx = NULL;
    param.vtxPCT3D = (NNS_PRIM3D_PCT*)amDrawMallocDataBuffer(sizeof(NNS_PRIM3D_PCT) * 6);
    v = param.vtxPCT3D;

    v[0].Pos.x = v[2].Pos.x = v[4].Pos.x = 1024.f - 160.0f;
    v[1].Pos.x = v[3].Pos.x = v[5].Pos.x = 1280.f - 160.0f;
    v[0].Pos.y = v[1].Pos.y = 0.0f;
    v[2].Pos.y = v[3].Pos.y = v[0].Pos.y + 64.0f;
    v[4].Pos.y = v[5].Pos.y = v[0].Pos.y + 128.0f;
    v[0].Pos.z = v[1].Pos.z = v[2].Pos.z = v[3].Pos.z = v[4].Pos.z = v[5].Pos.z = -2.0f;
    v[0].Col = v[2].Col = AMD_RGBA8888(255, 255, 255, 255);
    v[1].Col = v[3].Col = AMD_RGBA8888(255, 255, 255, 255);
    v[4].Col = v[5].Col = AMD_RGBA8888(255, 255, 255, 0);

    v[0].Tex.u = v[2].Tex.u = v[4].Tex.u = 0.0f;
    v[1].Tex.u = v[3].Tex.u = v[5].Tex.u = 1.0f;
    v[0].Tex.v = v[1].Tex.v = 0.0f;
    v[2].Tex.v = v[3].Tex.v = 0.125f;
    v[4].Tex.v = v[5].Tex.v = 0.25f;


    param.format3D = NNE_PRIM3D_FMT_PCT;
    param.type = NNE_PRIM_TRIANGLE_STRIP;
    param.count = 6;

    param.texlist = AoTexGetTexList(&main_work.tex[0]);
#if !_IPHONE
    param.texId = (s32)(54 + main_work.cur_bg_id);
#else //!_IPHONE
	if (u32(-1) == main_work.cur_bg_id) {	//ゾーンセレクト用特別背景なら透明にする
		for (int i = 0; i < 6; ++i) {
			u32 &color = v[i].Col;
			color &= 0xFFFFFF00;
		}
		param.texlist = NULL;
		param.texId = -1;
	} else {
		param.texId = (s32)(c_bg_tex_idx + 12 + main_work.cur_bg_id);
	}
#endif //!_IPHONE
    param.ablend = NNE_PRIM_ALPHABLEND_ON;
    param.zOffset = -1.0f;

    param.uwrap = NNE_PRIM_TEXWRAP_CLAMP;
    param.vwrap = NNE_PRIM_TEXWRAP_CLAMP;

    // ソート設定
    param.sortZ = 0.0f;

#if _PC | _XBOX
	param.bldSrc = NNE_BLENDMODE_SRCALPHA;
	param.bldDst = NNE_BLENDMODE_INVSRCALPHA;
	param.bldMode = NNE_BLENDOP_ADD;
#elif _PS3
	param.bldSrc = NND_BLENDFUNC_PS3_SRC_ALPHA;
	param.bldDst = NND_BLENDFUNC_PS3_ONE_MINUS_SRC_ALPHA;
	param.bldMode = NND_BLENDOP_PS3_FUNC_ADD;
#elif _WII
	param.bldSrc = GX_BL_SRCALPHA;
	param.bldDst = GX_BL_INVSRCALPHA;
	param.bldMode = GX_BM_BLEND;
#endif
    param.aTest = 0;
    param.zMask = 1;
    param.zTest = 0;

    // ソートしない
    param.noSort = 1;

    AoActDrawCorWide(v, 6, AOD_ACT_CORW_CENTER);

    amDrawPrimitive3D(120, param);


    param.mtx = NULL;
    param.vtxPCT3D = (NNS_PRIM3D_PCT*)amDrawMallocDataBuffer(sizeof(NNS_PRIM3D_PCT) * 6);
    v = param.vtxPCT3D;

    v[0].Pos.x = v[2].Pos.x = v[4].Pos.x = 1024.f - 160.0f;
    v[1].Pos.x = v[3].Pos.x = v[5].Pos.x = 1280.f - 160.0f;
    v[0].Pos.y = v[1].Pos.y = 720.0f;
    v[2].Pos.y = v[3].Pos.y = v[0].Pos.y - 128.0f;
    v[4].Pos.y = v[5].Pos.y = v[0].Pos.y - 192.0f;
    v[0].Pos.z = v[1].Pos.z = v[2].Pos.z = v[3].Pos.z = v[4].Pos.z = v[5].Pos.z = -2.0f;
    v[0].Col = v[2].Col = AMD_RGBA8888(255, 255, 255, 255);
    v[1].Col = v[3].Col = AMD_RGBA8888(255, 255, 255, 255);
    v[4].Col = v[5].Col = AMD_RGBA8888(255, 255, 255, 0);

    v[0].Tex.u = v[2].Tex.u = v[4].Tex.u = 0.0f;
    v[1].Tex.u = v[3].Tex.u = v[5].Tex.u = 1.0f;
    v[0].Tex.v = v[1].Tex.v = 0.8125f;
    v[2].Tex.v = v[3].Tex.v = 0.3125f;
    v[4].Tex.v = v[5].Tex.v = 0.0625f;

    param.format3D = NNE_PRIM3D_FMT_PCT;
    param.type = NNE_PRIM_TRIANGLE_STRIP;
    param.count = 6;

    param.texlist = AoTexGetTexList(&main_work.tex[0]);
#if !_IPHONE
    param.texId = (s32)(60 + main_work.cur_bg_id);
#else //!_IPHONE
	if (u32(-1) == main_work.cur_bg_id) {	//If it's a special background for zone selection, make it transparent
		for (int i = 0; i < 6; ++i) {
			u32 &color = v[i].Col;
			color &= 0xFFFFFF00;
		}
		param.texlist = NULL;
		param.texId = -1;
	} else {
		param.texId = (s32)(c_bg_tex_idx + 18 + main_work.cur_bg_id);
	}
#endif //!_IPHONE
    param.ablend = NNE_PRIM_ALPHABLEND_ON;
    param.zOffset = -1.0f;

    param.uwrap = NNE_PRIM_TEXWRAP_CLAMP;
    param.vwrap = NNE_PRIM_TEXWRAP_CLAMP;

    // ソート設定
    param.sortZ = 0.0f;


#if _PC | _XBOX
	param.bldSrc = NNE_BLENDMODE_SRCALPHA;
	param.bldDst = NNE_BLENDMODE_INVSRCALPHA;
	param.bldMode = NNE_BLENDOP_ADD;
#elif _PS3
	param.bldSrc = NND_BLENDFUNC_PS3_SRC_ALPHA;
	param.bldDst = NND_BLENDFUNC_PS3_ONE_MINUS_SRC_ALPHA;
	param.bldMode = NND_BLENDOP_PS3_FUNC_ADD;
#elif _WII
	param.bldSrc = GX_BL_SRCALPHA;
	param.bldDst = GX_BL_INVSRCALPHA;
	param.bldMode = GX_BM_BLEND;
#endif
    param.aTest = 0;
    param.zMask = 1;
    param.zTest = 0;


    AoActDrawCorWide(v, 6, AOD_ACT_CORW_CENTER);

    // ソートしない
    param.noSort = 1;


    amDrawPrimitive3D(120, param);

    // 
    main_work.tex_u[0] -= 0.024f;
    main_work.tex_u[1] -= 0.024f;
    main_work.tex_v[0] -= 0.006f;
    main_work.tex_v[1] -= 0.006f;


    amDrawMakeTask(dmStgSlctDrawVertexAct, (u16)(0x2800), (u32)0);


}



// ==========================================================================
// dmStgSlctDrawVertexAct
/*!
	ACTの上に被さる部分の描画設定処理
 */
// ==========================================================================
void dmStgSlctDrawVertexAct(AMS_TCB* tcb_p)
{
    UNREFERENCED_PARAMETER(tcb_p);

    // 前処理
    AoActDrawPre();
    amDrawExecCommand(120);

    // シーン描画終了(半透明描画開始)
    amDrawEndScene();

}




// ==========================================================================
// dmStgSlctSetEmeTableInEfct
/*!
	ファイルテーブル掃け演出用座標設定処理

  	ここでは基準点(一つ目のファイル)となる座標のみを移動させて
  	それ以外は一定の距離で表示位置をずらすようにする
 */
// ==========================================================================
void dmStgSlctSetEmeTableInEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    float move_dist = 0;
    float distance = 0;

    // 帯移動分
    distance = DMD_STGSLCT_DOWN_ACT_DISP_POS - DMD_STGSLCT_DOWN_ACT_NODISP_POS;

    move_dist = distance / DMD_STGSLCT_ZONE_EFCT_TIME;

    main_work.chaos_eme_pos_y += move_dist;

}



// ==========================================================================
// dmStgSlctIsEmeTableInEfctEnd
/*!
	帯掃け演出終了チェック処理
 */
// ==========================================================================
BOOL dmStgSlctIsEmeTableInEfctEnd(DMS_STGSLCT_MAIN_WORK* main_work)
{
    if (main_work.chaos_eme_pos_y <= DMD_STGSLCT_DOWN_ACT_DISP_POS)
    {

        main_work.chaos_eme_pos_y = DMD_STGSLCT_DOWN_ACT_DISP_POS;

        return TRUE;
    }

    return FALSE;
}



// ==========================================================================
// dmStgSlctSetEmeTableOutEfct
/*!
	ファイルテーブル掃け演出用座標設定処理

  	ここでは基準点(一つ目のファイル)となる座標のみを移動させて
  	それ以外は一定の距離で表示位置をずらすようにする
 */
// ==========================================================================
void dmStgSlctSetEmeTableOutEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    float move_dist = 0;
    float distance = 0;

    // 帯移動分
    distance = DMD_STGSLCT_DOWN_ACT_NODISP_POS - DMD_STGSLCT_DOWN_ACT_DISP_POS;

    move_dist = distance / DMD_STGSLCT_ZONE_EFCT_TIME;

    main_work.chaos_eme_pos_y += move_dist;

}



// ==========================================================================
// dmStgSlctIsEmeTableOutEfctEnd
/*!
	帯掃け演出終了チェック処理
 */
// ==========================================================================
BOOL dmStgSlctIsEmeTableOutEfctEnd(DMS_STGSLCT_MAIN_WORK* main_work)
{
    if (main_work.chaos_eme_pos_y >= DMD_STGSLCT_DOWN_ACT_NODISP_POS)
    {

        main_work.chaos_eme_pos_y = DMD_STGSLCT_DOWN_ACT_NODISP_POS;

        return TRUE;
    }

    return FALSE;
}




// ==========================================================================
// dmStgSlctSetModeTexInEfct
/*!
	ファイルテーブル掃け演出用座標設定処理

  	ここでは基準点(一つ目のファイル)となる座標のみを移動させて
  	それ以外は一定の距離で表示位置をずらすようにする
 */
// ==========================================================================
void dmStgSlctSetModeTexInEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    float move_dist = 0;
    float distance = 0;

    // 帯移動分
    distance = DMD_STGSLCT_DOWN_ACT_DISP_POS - DMD_STGSLCT_DOWN_ACT_NODISP_POS;

    move_dist = distance / DMD_STGSLCT_ZONE_EFCT_TIME;

    main_work.mode_tex_pos_y += move_dist;

}



// ==========================================================================
// dmStgSlctIsModeTexInEfctEnd
/*!
	帯掃け演出終了チェック処理
 */
// ==========================================================================
BOOL dmStgSlctIsModeTexInEfctEnd(DMS_STGSLCT_MAIN_WORK* main_work)
{
    if (main_work.mode_tex_pos_y <= DMD_STGSLCT_DOWN_ACT_DISP_POS)
    {

        main_work.mode_tex_pos_y = DMD_STGSLCT_DOWN_ACT_DISP_POS;

        return TRUE;
    }

    return FALSE;
}



// ==========================================================================
// dmStgSlctSetModeTexOutEfct
/*!
	ファイルテーブル掃け演出用座標設定処理

  	ここでは基準点(一つ目のファイル)となる座標のみを移動させて
  	それ以外は一定の距離で表示位置をずらすようにする
 */
// ==========================================================================
void dmStgSlctSetModeTexOutEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    float move_dist = 0;
    float distance = 0;

    // 帯移動分
    distance = DMD_STGSLCT_DOWN_ACT_NODISP_POS - DMD_STGSLCT_DOWN_ACT_DISP_POS;

    move_dist = distance / DMD_STGSLCT_ZONE_EFCT_TIME;

    main_work.mode_tex_pos_y += move_dist;

}



// ==========================================================================
// dmStgSlctIsModeTexOutEfctEnd
/*!
	帯掃け演出終了チェック処理
 */
// ==========================================================================
BOOL dmStgSlctIsModeTexOutEfctEnd(DMS_STGSLCT_MAIN_WORK* main_work)
{
    if (main_work.mode_tex_pos_y >= DMD_STGSLCT_DOWN_ACT_NODISP_POS)
    {

        main_work.mode_tex_pos_y = DMD_STGSLCT_DOWN_ACT_NODISP_POS;

        return TRUE;
    }

    return FALSE;
}



#if defined (MTD_DEBUG)
// ==========================================================================
// dmStgSlctSetAllOpenStage
/*!
	全ステージをオープン状態に設定する処理
 */
// ==========================================================================
void dmStgSlctSetAllOpenStage(DMS_STGSLCT_MAIN_WORK *main_work)
{
	u32 save_hi_score = 1000;
	bool sonic_type = 0;
	
	UNREFERENCED_PARAMETER(main_work);
	
	// 通常ステージデータインスタンス作成
	gs::backup::SStage &data
		= gs::backup::SStage::CreateInstance();
	
	// スペシャルステージデータインスタンス作成
	gs::backup::SSpecial &spe_data
		= gs::backup::SSpecial::CreateInstance();
	
	
	for (u32 i = 0; i <= GSD_MAIN_STAGE_ID_FINAL_1; i++) {
		// 通常ステージのハイスコア取得
		data[i].SetHighScore(save_hi_score, sonic_type);
		
	}
	
	for (u32 i = 0; i < 7; i++) {
		// スペシャルステージのハイスコア取得
		spe_data[i].SetHighScore(save_hi_score);
		
		spe_data[i].SetEmeraldStage((gs::backup::SSpecialSolo::EEmeraldStage::Type)(i + 1));
	}
}
#endif


// ==========================================================================
// dmStgSlctSetBgFadeEfct
/*!
	BGフェード演出設定処理
 */
// ==========================================================================
void dmStgSlctSetBgFadeEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    s32 tmp_bg_fade_a = 0;

    // ビット拡張して0以下と255以上を比較できるようにする
    tmp_bg_fade_a = (s32)main_work.bg_fade.a;

    // 現在選択しているZONEとBGのゾーンを比較
    if (main_work.cur_zone != main_work.cur_bg_id)
    {
        // 演出開始フラグON
        main_work.flag |= DMD_STGSLCT_FLAG_BG_FADE_EFCT;

        // 切り替えるBGのIDを設定
        main_work.next_bg_id = main_work.cur_zone;
    }


    // BGのフォグ設定
    if (main_work.flag & DMD_STGSLCT_FLAG_BG_FADE_EFCT)
    {

        if (tmp_bg_fade_a < 255)
        {
            tmp_bg_fade_a += DMD_STGSLCT_BG_FADE_SPEED;
        }
        else
        {
            tmp_bg_fade_a = 255;

            // ここでBGを切り替える
            main_work.cur_bg_id = main_work.next_bg_id;

            main_work.flag &= ~DMD_STGSLCT_FLAG_BG_FADE_EFCT;
        }
    }

    else
    {
        if (tmp_bg_fade_a > 0)
        {
            // ここでは常に下げる
            tmp_bg_fade_a -= DMD_STGSLCT_BG_FADE_SPEED;
        }
        else
        {
            tmp_bg_fade_a = 0;
        }
    }

    tmp_bg_fade_a = MTM_MATH_CLIP(tmp_bg_fade_a, 0, 255);

    // 計算結果を元の変数に戻す
    main_work.bg_fade.a = (u8)tmp_bg_fade_a;

}



// ==========================================================================
// dmStgSlctSetZoneScrChangeEfct
/*!
	BGフェード演出設定処理
 */
// ==========================================================================
void dmStgSlctSetZoneScrChangeEfct(DMS_STGSLCT_MAIN_WORK* main_work)
{
    // タイマー更新
    //	main_work.zone_scr_timer++;

    // 切り替え時間が経ったとき
    if (main_work.zone_scr_id > DMD_STGSLCT_ZONE_SCR_CHANGE_ALL_FRM)
    {
        // ゾーンスクリーンID切り替え
        main_work.zone_scr_id = 0;
    }
    else
    {
        main_work.zone_scr_id++;
    }
}



// ==========================================================================
// dmStgSlctSetNextFocusAct
/*!
	ACTクリア後に次のACTへFOCUSを変更する設定処理
 */
// ==========================================================================
s32 dmStgSlctSetNextFocusAct(DMS_STGSLCT_MAIN_WORK* main_work, s32 set_stage_id)
{
    UNREFERENCED_PARAMETER(main_work);

    u32 tmp_stage_id = 0;
    u32 tmp_focus_zone = 0;
    u32 tmp_zone_act_id = 0;

    u32 tmp_check_zone = 0;
    u32 tmp_check_act = 0;


    // スペステの場合、変更なし
    if (set_stage_id >= GSD_MAIN_STAGE_ID_SS1)
    {
        tmp_stage_id = dm_stgslct_zone_act_num_tbl[DME_STGSLCT_ACT_PAGE_SPE][0]
                        + (set_stage_id - GSD_MAIN_STAGE_ID_SS1);

        return (s32)tmp_stage_id;
    }

    if (set_stage_id >= GSD_MAIN_STAGE_ID_FINAL_1)
    {
        tmp_stage_id = GSD_MAIN_STAGE_ID_FINAL_1;

        return (s32)tmp_stage_id;
    }

    // ここでACT番号を取得(1-1から4-BOSSまでの間になる)
    tmp_stage_id = (u32)set_stage_id;

#if 1
	// 初回プレイでない場合、切り替えなし
	if (g_gs_main_sys_info.is_first_play == FALSE) {
		return (s32)tmp_stage_id;
	}
	
	// タイムアタックの場合、切り替えなし(初回プレイじゃない時点でOKかもしれない)
	if (g_gs_main_sys_info.game_mode == GSD_GAME_MODE_TIME_ATTACK) {
		return (s32)tmp_stage_id;
	}
	
	// 一つ前のイベントがランキングだった場合、ACTはそのまま
	if (main_work.flag & DMD_STGSLCT_FLAG_PREV_EVT_RANKING) {
		return (s32)tmp_stage_id;
	}
#endif

    // 初めにチェックするZONEを取得
    tmp_focus_zone = dm_stgslct_act_zone_no_tbl[tmp_stage_id];
    tmp_zone_act_id = dm_stgslct_zone_array_act_tbl[tmp_stage_id];

    // 初めにチェックするZONEのクリア状況をチェック
    for (u32 i = tmp_stage_id - tmp_zone_act_id;
             i < tmp_stage_id - tmp_zone_act_id + 4;
             i++)
    {
        // 指定ゾーンに若い番号から見て、未クリアACTがあればそれを指定
        if (GsMainSysIsStageClear(i) == FALSE)
        {
            return (s32)i;
        }
    }

    // その他のゾーンのクリア状況をチェック
    for (u32 i = 0; i < 4; i++)
    {
        tmp_check_zone = i;

        tmp_check_act = dm_stgslct_zone_act_num_tbl[tmp_check_zone][0];

        for (u32 i = 0; i < 4; i++)
        {
            if (GsMainSysIsStageClear(tmp_check_act + i) == FALSE)
            {
                return (s32)(tmp_check_act + i);
            }
        }
    }

    // 初回プレイステージクリアかつ、他のACT全てクリア済みの場合、FINALへ
    tmp_stage_id = GSD_MAIN_STAGE_ID_FINAL_1;

    // スコアアタックにて、全てクリア済みの状態のとき
    return (s32)tmp_stage_id;
}

#if _IPHONE
// ==========================================================================
// dmStgSlctIsBossFocus
/*!
	ボスフォーカス状態確認

	@return ボスフォーカス状態である
 */
// ==========================================================================
bool dmStgSlctIsBossFocus(DMS_STGSLCT_MAIN_WORK *main_work)
{
	bool result = false;
	switch (main_work.cur_zone) {
	case DME_STGSLCT_ZONE_TYPE_FINAL: //ファイナルゾーン
	case DME_STGSLCT_ZONE_TYPE_SPE: //スペシャルステージ
		//何もしない
		break;
	default: //通常ゾーン
		{
			if (GSD_GAME_MODE_STORY == main_work.cur_game_mode) {
				//スコアアタックなら
				u32 act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
				u32 act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;
				//クリア数の判定
				u32 clear_stage_num = 0;
				for (u32 i = act_num_start; i < act_num_end; ++i) {
					if (1 == main_work.is_clear_stage[i]) {
						++clear_stage_num;
					}
				}
				//クリア数が3ならボスにフォーカス
				if (3 == clear_stage_num) {
					result = true;
				}
			}
		}
		break;
	}
	return result;
}

// ==========================================================================
// dmStgSlctStageSelectChngZoneSetInZoneScroll
/*!
	アクトセレクト時のゾーン切り換えに於ける入り演出側ゾーンのスクロール量設定
 */
// ==========================================================================
void dmStgSlctStageSelectChngZoneSetInZoneScroll(DMS_STGSLCT_MAIN_WORK *main_work, s32 stage)
{
	u32 act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
	u32 act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;
	
	switch (main_work.cur_zone) {
	case DME_STGSLCT_ZONE_TYPE_FINAL: //ファイナルゾーン
		main_work.focus_disp_no = 0;
		for (u32 i = act_num_start; i < act_num_end; ++i) {
			main_work.act_top_pos_y[i] = dm_stgslct_act_disp_y_pos_tbl[i];
		}
		break;
	case DME_STGSLCT_ZONE_TYPE_SPE: //スペシャルステージ
		{
			s32 focus_disp_no = stage - DME_STGSLCT_STAGE_S_2;
			main_work.focus_disp_no = MTM_MATH_CLIP(focus_disp_no, 0, 4);
			for (u32 i = act_num_start; i < act_num_end; ++i) {
				main_work.act_top_pos_y[i] = dm_stgslct_act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
			}
		}
		break;
	default: //通常ゾーン
		if (dmStgSlctIsBossFocus(main_work)) {
			main_work.focus_disp_no = 1;
		} else switch (stage) {
		case DME_STGSLCT_STAGE_1_B:
		case DME_STGSLCT_STAGE_2_B:
		case DME_STGSLCT_STAGE_3_B:
		case DME_STGSLCT_STAGE_4_B:
			main_work.focus_disp_no = 1;
			break;
		default:
			main_work.focus_disp_no = 0;
			break;
		}
		for (u32 i = act_num_start; i < act_num_end; ++i) {
			main_work.act_top_pos_y[i] = dm_stgslct_act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
		}
		break;
	}	
}
#endif _IPHONE


// ==========================================================================
// _pt
/*!
 *	@param	tcb	[in]	TCB
 */
// ==========================================================================

}