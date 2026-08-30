internal static partial class DmSave
{
    public const int DMD_SAVE_TASK_PAUSELEVEL = 0x7fff;
    public const int DMD_SAVE_TASK_PRIO_MAIN = 0x2000;
    public const int DMD_SAVE_TASK_GROUP_MAIN = 0;

    public const int DMD_SAVE_FILE_PATH_NUM_MAX = 60;

    public const float DMD_SAVE_SIZE_WIDTH = 960f;
    public const float DMD_SAVE_SIZE_HEIGHT = 720f;
    public const float DMD_SAVE_SIZE_HALF_WIDTH = 480f;
    public const float DMD_SAVE_SIZE_HALF_HEIGHT = 360f;

    public const int DMD_SAVE_DRAW_PRIO_BG = 0x0100;
    public const int DMD_SAVE_DRAW_PRIO_WIN = 0xEFFF;
    public const int DMD_SAVE_DRAW_PRIO_WIN_FIX = 0xEFFF;
    public const int DMD_SAVE_DRAW_TASK_PRIO = 0xEFFF;

    public const int DMD_SAVE_DRAW_STATE_ID = 100;

    public const float DMD_SAVE_WINDOW_SIZE_W = 380f;
    public const float DMD_SAVE_WINDOW_SIZE_H = 180f;
    public const float DMD_SAVE_WIN_DEF_RATE = 1f;

    public const int DMD_SAVE_LOAD_MIN_TIME = 60;
    public const int DMD_SAVE_SAVE_MIN_TIME = 60;

    public const float DMD_SAVE_FADEIN_TIME = 32f;
    public const float DMD_SAVE_FADEOUT_TIME = 32f;

    public const int DMD_SAVE_BGM_FADEIN_TIME = 32;
    public const int DMD_SAVE_BGM_FADEOUT_TIME = 32;

    public const int DMD_SAVE_SE_END_WAIT_TIME = 60;

    public const int DMD_SAVE_WIN_EFCT_TIME = 8;

    public const uint DMD_SAVE_FLAG_EXIT = 1 << 0;
    public const uint DMD_SAVE_FLAG_CANCEL = 1 << 1;
    public const uint DMD_SAVE_FLAG_DECIDE = 1 << 2;
    public const uint DMD_SAVE_FLAG_DISP_MENU = 1 << 3;
    public const uint DMD_SAVE_FLAG_WIN_EFCT_END = 1 << 4;
    public const uint DMD_SAVE_FLAG_WIN_MSG_END = 1 << 5;

    public const uint DMD_SAVE_FLAG_SIGN_OUT_EXIT = 1U << 31;

    public const uint DMD_SAVE_DISP_FLAG_WIN_ACT = 1 << 0;
    public const uint DMD_SAVE_DISP_FLAG_SAVE_MENU = 1 << 1;
    public const uint DMD_SAVE_DISP_FLAG_SAVE_VIEW = 1 << 2;
    public const uint DMD_SAVE_DISP_FLAG_MENU_TAB = 1 << 3;
    public const uint DMD_SAVE_DISP_FLAG_SAVE_LIST = 1 << 4;
    public const uint DMD_SAVE_DISP_FLAG_NOW_UPDATE = 1 << 5;

    public const float DMD_SAVE_DISP_SCALE_TEXT = 1.5f * 1.125f;
}
