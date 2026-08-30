internal static partial class StgSlct
{
    public const string GSS_BASE_PATH = "";

    public const int DMD_STGSLCT_TASK_PAUSELEVEL = 0;
    public const int DMD_STGSLCT_TASK_PRIO_MAIN = 0x2000;
    public const int DMD_STGSLCT_TASK_GROUP_MAIN = 0;

    public const int DMD_STGSLCT_FILE_PATH_NUM_MAX = 60;

    public const string DMD_STGSLCT_DATA_FILENAME = GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT.AMB";

    public const float DMD_STGSLCT_SIZE_WIDTH = 960.0f;
    public const float DMD_STGSLCT_SIZE_HEIGHT = 720.0f;
    public const float DMD_STGSLCT_SIZE_HALF_WIDTH = 480.0f;
    public const float DMD_STGSLCT_SIZE_HALF_HEIGHT = 360.0f;

    public const int DMD_STGSLCT_DISP_SCORE_DIGIT_NUM = 9;

    public const int DMD_STGSLCT_DRAW_PRIO_ZONE = 0x2000;
    public const int DMD_STGSLCT_DRAW_PRIO_STAGE = 0x2000;
    public const int DMD_STGSLCT_DRAW_PRIO_BG = 0x1000;
    public const int DMD_STGSLCT_DRAW_PRIO_FIX = 0x3000;
    public const int DMD_STGSLCT_DRAW_PRIO_WIN_FIX = 0x4000;

    public const int DMD_STGSLCT_INIT_SCORE_NUM = 1000000000;
    public const int DMD_STGSLCT_INIT_RECORD_TIME_NUM = 36000;

    public const int DMD_STGSLCT_CONV_SCORE_RATE = 10;

    public const int DMD_STGSLCT_DOWN_ACT_DISP_POS = 0;
    public const int DMD_STGSLCT_DOWN_ACT_NODISP_POS = 180;
    public const int DMD_STGSLCT_MODE_TEX_FRAME_MAX = 60;

    public const float DMD_STGSLCT_STAGE_TAB_NODISP_POS_X = 1120.0f;

    public const float DMD_STGSLCT_NO_ACTIVE_NUM_ID = 10.0f;
    public const float DMD_STGSLCT_TIME_COLON_ID = 11.0f;

    public const float DMD_STGSLCT_WINDOW_SIZE_H = 180.0f;
    public const float DMD_STGSLCT_WIN_DEF_RATE = 1.0f;

    public const float DMD_STGSLCT_FADEIN_TIME = 32.0f;
    public const float DMD_STGSLCT_FADEOUT_TIME = 32.0f;

    public const int DMD_STGSLCT_BGM_FADEIN_TIME = 32;
    public const int DMD_STGSLCT_BGM_FADEOUT_TIME = 32;

    public const int DMD_STGSLCT_BG_FADE_SPEED = 20;

    public const float DMD_STGSLCT_ZONE_EFCT_TIME = 16.0f;
    public const float DMD_STGSLCT_ACT_EFCT_TIME = 16.0f;
    public const float DMD_STGSLCT_WIN_EFCT_TIME = 8.0f;
    public const float DMD_STGSLCT_OBI_MOVE_START_POS = 1120.0f;
    public const float DMD_STGSLCT_OBI_MOVE_END_POS = -1120.0f;
    public const float DMD_STGSLCT_OBI_MOVE_SPEED = -3.0f;
    public const float DMD_STGSLCT_ACT_VRTCL_CHNG_DIST = 128.0f;
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
    public const uint DMD_STGSLCT_FLAG_SIGN_OUT_EXIT = 1u << 31;

    public const int DMD_STGSLCT_DISP_FLAG_WIN_ACT = 1 << 0;
    public const int DMD_STGSLCT_DISP_FLAG_LR_ARROW = 1 << 1;
    public const int DMD_STGSLCT_DISP_FLAG_ACT_CRSR = 1 << 2;

    // the four that differ between builds. picked once into the variant descriptor rather than
    // asked about at each use site.
    private const int DMD_STGSLCT_DRAW_PRIO_WIN_CONSOLE = 0x3000;
    private const int DMD_STGSLCT_DRAW_PRIO_WIN_MOBILE = 0x3500;

    private const float DMD_STGSLCT_ACT_TABLE_TOP_POS_X_CONSOLE = 170.0f;
    private const float DMD_STGSLCT_ACT_TABLE_TOP_POS_X_MOBILE = 100.0f;

    private const float DMD_STGSLCT_ACT_TABLE_DIST_Y_CONSOLE = 96.0f;
    private const float DMD_STGSLCT_ACT_TABLE_DIST_Y_MOBILE = 96.0f + 22.0f;

    private const float DMD_STGSLCT_WINDOW_SIZE_W_CONSOLE = 380.0f;
    private const float DMD_STGSLCT_WINDOW_SIZE_W_MOBILE = 420.0f;

    // these two have no counterpart on the other side: the C++ only defines DISP_SCALE_TEXT under
    // _WII/_IPHONE and OBI_MSG_SCALE_SIZE under _PS3/_XBOX/_PC, and only variant-local code reads
    // them, so neither belongs on the descriptor
    public const float DMD_STGSLCT_DISP_SCALE_TEXT_MOBILE = 1.5f * 1.125f;
    public const float DMD_STGSLCT_OBI_MSG_SCALE_SIZE_CONSOLE = 1.2f;
}
