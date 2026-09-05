internal static partial class GmPauseMenu
{
    public const int GMD_PMENU_EFCT_IN_TIME = 24;
    public const int GMD_PMENU_EFCT_OUT_TIME = 24;
    public const int GMD_PMENU_EFCT_WIN_TIME = 8;
    public const int GMD_PMENU_EFCT_DECIDE_TIME = 12;

    public const float GMD_PMENU_SCREEN_W = 960.0f;
    public const float GMD_PMENU_SCREEN_H = 720.0f;

    public const float GMD_PMENU_WIN_W = 430.0f;
    public const float GMD_PMENU_WIN_H = 196.0f;

    // the whole menu sits 32px below centre, which is also where the dimmer plate is drawn
    public const float GMD_PMENU_SEL_CENTER_Y = GMD_PMENU_SCREEN_H * 0.5f + 32.0f;
    public const float GMD_PMENU_SEL_PITCH_Y = 51.0f;

    public const int GMD_PMENU_SEL_NUM = 4;
    public const int GMD_PMENU_SEL_NUM_FULL = 6;

    // out of range on purpose: the decide flash asks for a row that cannot be highlighted
    public const int GMD_PMENU_SEL_NONE = 6;
    public const int GMD_PMENU_SEL_SUB_NONE = 2;

    public const string GMD_PMENU_FILE_DIR_PATH = "G_COM/MENU/";
}
