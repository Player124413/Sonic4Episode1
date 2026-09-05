internal static partial class GmFix
{
    public const uint GMD_FIX_MGR_FLAG_CLEAR = 1 << 0;
    public const uint GMD_FIX_MGR_FLAG_HIDE = 1 << 1;
    public const uint GMD_FIX_MGR_FLAG_TIMER_COUNTDOWN = 1 << 2;

    public const uint GMD_FIX_MGR_FLAG_HIDE_VIRTUAL_PAD_PART_SUPER_SONIC = 1 << 9;
    public const uint GMD_FIX_MGR_FLAG_HIDE_VIRTUAL_PAD_PART_PAUSE = 1 << 10;
    public const uint GMD_FIX_MGR_FLAG_HIDE_VIRTUAL_PAD_PART_ACTION = 1 << 11;
    public const uint GMD_FIX_MGR_FLAG_HIDE_VIRTUAL_PAD_PART_MOVE_PAD = 1 << 12;

    public const uint GMD_FIX_MGR_REQUEST_FLAG_TIMER_FLASH_ACT = 1 << 0;

    public const uint GMD_FIX_PART_FLAG_ACTIVE = 1 << 0;
    public const uint GMD_FIX_PART_FLAG_NODISP = 1 << 1;
    public const uint GMD_FIX_PART_FLAG_BLINK = 1 << 2;
    public const uint GMD_FIX_PART_FLAG_BLINK_OFF = 1 << 3;

    public const int GMD_FIX_PART_RINGCOUNT_DIGIT_NUM = 3;
    public const uint GMD_FIX_PART_RINGCOUNT_DIGIT_BLINK_ON_TIME = 10;
    public const uint GMD_FIX_PART_RINGCOUNT_DIGIT_BLINK_OFF_TIME = 10;

    public const int GMD_FIX_PART_SCORE_DIGIT_NUM = 9;

    public const int GMD_FIX_PART_SCORE_S22_TA_NARROWSCREEN_ADJUST_OFST_X = 0x40000;

    public const uint GMD_FIX_PART_TIMER_FLAG_RED_BY_TEX_FRAME = 1 << 0;
    public const uint GMD_FIX_PART_TIMER_FLAG_FLASH_ACT_ACTIVE = 1 << 1;

    public const int GMD_FIX_PART_TIMER_MIN_DIGIT_NUM = 1;
    public const int GMD_FIX_PART_TIMER_SEC_DIGIT_NUM = 2;
    public const int GMD_FIX_PART_TIMER_MSEC_DIGIT_NUM = 2;
    public const int GMD_FIX_PART_TIMER_ALL_DIGIT_NUM = 5;
    public const int GMD_FIX_PART_TIMER_DECO_CHAR_NUM = 2;

    public const float GMD_FIX_PART_TIMER_DIGIT_RED_FRAME_OFST = 10.0f;
    public const float GMD_FIX_PART_TIMER_DECO_CHAR_RED_FRAME_OFST = 1.0f;

    public const uint GMD_FIX_PART_TIMER_DIGIT_BLINK_ON_TIME = 10;
    public const uint GMD_FIX_PART_TIMER_DIGIT_BLINK_OFF_TIME = 10;
    public const uint GMD_FIX_PART_TIMER_DIGIT_BLINK_REMAIN_SEC = 20;

    public const int GMD_FIX_PART_TIMER_FLASH_ACT_SCALE_MAX = 6144;
    public const int GMD_FIX_PART_TIMER_FLASH_ACT_SCALE_DEF = 4096;

    public const int GMD_FIX_PART_CHALLENGE_DIGIT_NUM = 3;

    public const ushort GMD_FIX_TASK_PAUSELEVEL = 0;

    // shared with the results screen, game over and the title card
    public const string GMD_FIX_DATA_FILENAME = "G_COM/CPIT/CPIT_MAIN.AMB";

    // 35999 frames is 9:59:98, the display's ceiling
    public const uint GMD_FIX_GAME_TIME_MAX = 35999;
    public const int GMD_FIX_CHALLENGE_NUM_MAX = 999;

    // "0" through "9" are frames 0-9 of the digit action; the ring counter's blank is frame 10
    public const int GMD_FIX_DIGIT_BLANK = 10;
}
