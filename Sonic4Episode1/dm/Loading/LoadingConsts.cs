internal static partial class DmLoading
{
    // D_LOAD.AMA's own action ids. one set, not two: the dump has no file/iPhone/ace/D_LOAD.HMA
    public const uint IDA_D_LOAD_ACT_BG_WHITE = 0;
    public const uint IDA_D_LOAD_ACT_BG_BOTTOM = 1;
    public const uint IDA_D_LOAD_ACT_OBI = 2;
    public const uint IDA_D_LOAD_ACT_RUN = 3;
    public const uint IDA_D_LOAD_ACT_STOP = 4;
    public const uint IDA_D_LOAD_ACT_PERIOD1 = 5;
    public const uint IDA_D_LOAD_ACT_PERIOD2 = 6;
    public const uint IDA_D_LOAD_ACT_PERIOD3 = 7;
    public const uint IDA_D_LOAD_ACT_TEXT_LOAD = 8;

    public const ushort DMD_LOADING_TASK_PAUSELEVEL = 0x7fff;
    public const uint DMD_LOADING_TASK_PRIO_MAIN = 0x2000;
    public const int DMD_LOADING_TASK_GROUP_MAIN = 10;

    public const int DMD_LOADING_FILE_PATH_NUM_MAX = 60;

    public const string DMD_LOADING_CMN_DATA_FILENAME = "DEMO/CMN/D_CMN_WIN.AMB";
    public const string DMD_LOADING_DATA_FILENAME = "DEMO/OPTION/D_OPTION_USER.AMB";

    public const float DMD_LOADING_SIZE_WIDTH = 960.0f;
    public const float DMD_LOADING_SIZE_HEIGHT = 720.0f;
    public const float DMD_LOADING_SIZE_HALF_WIDTH = 480.0f;
    public const float DMD_LOADING_SIZE_HALF_HEIGHT = 360.0f;

    public const uint DMD_LOADING_DRAW_PRIO_CHAR = 0x3000;
    public const uint DMD_LOADING_DRAW_PRIO_BG = 0x2000;
    public const uint DMD_LOADING_DRAW_PRIO_FIX = 0x2800;

    public const int DMD_LOADING_DRAW_STATE_ID = 10;

    public const float DMD_LOADING_FADEIN_TIME = 32.0f;
    public const float DMD_LOADING_FADEOUT_TIME = 32.0f;

    public const float DMD_LOADING_LOAD_LOOP_TIME = 12.0f;
    public const float DMD_LOADING_LOADED_WAIT_TIME = 32.0f;

    public const float DMD_LOADING_SONIC_MOVE_INIT_SPD = 50.0f;
    public const float DMD_LOADING_SONIC_MAIN_INIT_POS = 0.0f;
    public const float DMD_LOADING_SONIC_OTHER_INIT_POS = 0.0f;

    public const uint DMD_LOADING_FLAG_EXIT = 1 << 0;
    public const uint DMD_LOADING_FLAG_CANCEL = 1 << 1;
    public const uint DMD_LOADING_FLAG_DECIDE = 1 << 2;

    // WP7 nudges the running Sonic up where the C++ leaves him at 0. It ships in a 800x480 window
    // against the console's 960x720
    public const float DMD_LOADING_SONIC_POS_Y = -10.0f;

    // the draw task prio, which is amDrawMakeTask's own u16 and not one of the AoAct prios above
    public const ushort DMD_LOADING_DRAW_TASK_PRIO = 0x8000;

    // iOS never shipped FI/PT/RU/CN/HK, so their "NOW LOADING" text is not a frame of the action's
    // own strip the way the first six are. WP7 appended the textures and picks them by id instead
    public const int DMD_LOADING_LANG_TEX_FIRST = 6;
    public const int DMD_LOADING_LANG_TEX_ID = 9;
}
