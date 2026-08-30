using static AppMain;

using static GmPauseMenu.GME_PMENU_ACT;

internal static partial class GmPauseMenu
{
    // measured out of the xbla archives, not the generated headers: the leak ships only Wii and
    // iPhone id sets and the Wii ordering is wrong for console. see docs/pause-action-ids.md
    private static class ConsoleIds
    {
        public const int IDA_G_PAUSE_ACT_TITLE_BASE = 0;
        public const int IDA_G_PAUSE_ACT_BTN_BASE = 1;
        public const int IDA_G_PAUSE_ACT_BTN_BASE_S = 2;
        public const int IDA_G_PAUSE_ACT_BACK = 5;

        public const int IDA_G_PAUSE_L_ACT_TITLE_PAUSE = 0;
        public const int IDA_G_PAUSE_L_ACT_BTN_BACK = 1;
        public const int IDA_G_PAUSE_L_ACT_DLG_SEL_YES = 4;
        public const int IDA_G_PAUSE_L_ACT_DLG_SEL_YES_S = 5;
        public const int IDA_G_PAUSE_L_ACT_DLG_SEL_NO = 6;
        public const int IDA_G_PAUSE_L_ACT_DLG_SEL_NO_S = 7;
        public const int IDA_G_PAUSE_L_ACT_MSG_RETRY = 8;
        public const int IDA_G_PAUSE_L_ACT_MSG_WORLD = 9;
        public const int IDA_G_PAUSE_L_ACT_MSG_RETRY2 = 10;
        public const int IDA_G_PAUSE_L_ACT_MSG_MAINMENU = 11;

        // one glyph where Wii and PS3 have the O/X pair, so both act slots map here
        public const int IDA_G_PAUSE_P_ACT_MARK_BACK = 0;

        public const int IDA_G_PAUSE_P_L_ACT_BTN_RETRY = 0;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_RETRY_S = 1;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_OPTION = 2;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_OPTION_S = 3;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_RANK = 4;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_RANK_S = 5;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_TROPHY = 6;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_TROPHY_S = 7;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_WORLD = 8;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_WORLD_S = 9;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_MMENU = 12;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_MMENU_S = 13;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_RESUME = 14;
        public const int IDA_G_PAUSE_P_L_ACT_BTN_RESUME_S = 15;
    }

    public const string GMD_PMENU_FILE_CMN_AMA = GMD_PMENU_FILE_DIR_PATH + "G_PAUSE.AMA";
    public const string GMD_PMENU_FILE_CMN_AMB = GMD_PMENU_FILE_DIR_PATH + "G_PAUSE.AMB";
    public const string GMD_PMENU_FILE_PLF_AMA = GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_P.AMA";
    public const string GMD_PMENU_FILE_PLF_AMB = GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_P.AMB";
    public const string GMD_PMENU_FILE_LNG_AMA = GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_L.AMA";
    public const string GMD_PMENU_FILE_PLNG_AMA = GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_P_L.AMA";
    public const string GMD_PMENU_FILE_WIN_AMB = GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_WIN.AMB";

    // six rows, because that is what xbla ships. The five WP7 languages have no console pause
    // archives at all, so gmPmIsConsoleDataComplete drops the whole screen back to the touch menu
    // rather than letting an index run off the end.
    internal static readonly string[] gm_pm_file_lng_amb_name_tbl =
    [
        GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_JP.AMB",
        GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_US.AMB",
        GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_FR.AMB",
        GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_IT.AMB",
        GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_GE.AMB",
        GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_SP.AMB",
    ];

    internal static readonly string[] gm_pm_file_plng_amb_name_tbl =
    [
        GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_P_JP.AMB",
        GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_P_US.AMB",
        GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_P_FR.AMB",
        GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_P_IT.AMB",
        GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_P_GE.AMB",
        GMD_PMENU_FILE_DIR_PATH + "G_PAUSE_P_SP.AMB",
    ];

    // indexed by GME_PMENU_ACT. The six button plates are the same two ids over and over: one
    // plate, one selected plate, drawn six times at different heights.
    internal static readonly int[] gm_pm_act_id_tbl =
    [
        ConsoleIds.IDA_G_PAUSE_ACT_TITLE_BASE,

        ConsoleIds.IDA_G_PAUSE_ACT_BTN_BASE, ConsoleIds.IDA_G_PAUSE_ACT_BTN_BASE_S,
        ConsoleIds.IDA_G_PAUSE_ACT_BTN_BASE, ConsoleIds.IDA_G_PAUSE_ACT_BTN_BASE_S,
        ConsoleIds.IDA_G_PAUSE_ACT_BTN_BASE, ConsoleIds.IDA_G_PAUSE_ACT_BTN_BASE_S,
        ConsoleIds.IDA_G_PAUSE_ACT_BTN_BASE, ConsoleIds.IDA_G_PAUSE_ACT_BTN_BASE_S,
        ConsoleIds.IDA_G_PAUSE_ACT_BTN_BASE, ConsoleIds.IDA_G_PAUSE_ACT_BTN_BASE_S,
        ConsoleIds.IDA_G_PAUSE_ACT_BTN_BASE, ConsoleIds.IDA_G_PAUSE_ACT_BTN_BASE_S,

        ConsoleIds.IDA_G_PAUSE_ACT_BACK,

        ConsoleIds.IDA_G_PAUSE_L_ACT_TITLE_PAUSE,
        ConsoleIds.IDA_G_PAUSE_L_ACT_BTN_BACK,
        ConsoleIds.IDA_G_PAUSE_L_ACT_DLG_SEL_YES,
        ConsoleIds.IDA_G_PAUSE_L_ACT_DLG_SEL_YES_S,
        ConsoleIds.IDA_G_PAUSE_L_ACT_DLG_SEL_NO,
        ConsoleIds.IDA_G_PAUSE_L_ACT_DLG_SEL_NO_S,
        ConsoleIds.IDA_G_PAUSE_L_ACT_MSG_RETRY,
        ConsoleIds.IDA_G_PAUSE_L_ACT_MSG_WORLD,
        ConsoleIds.IDA_G_PAUSE_L_ACT_MSG_MAINMENU,

        ConsoleIds.IDA_G_PAUSE_P_ACT_MARK_BACK,
        ConsoleIds.IDA_G_PAUSE_P_ACT_MARK_BACK,

        ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_RETRY,
        ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_RETRY_S,
        ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_OPTION,
        ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_OPTION_S,
        ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_RANK,
        ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_RANK_S,
        ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_TROPHY,
        ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_TROPHY_S,
        ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_RESUME,
        ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_RESUME_S,
        ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_WORLD,
        ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_WORLD_S,
    ];

    // top to bottom. Ranking and trophies sit between options and world map in the C++ and are
    // left out here; the enum entries stay so putting them back is an edit to this list.
    internal static readonly GME_PMENU_ACT[] gm_pm_sel_act_tbl =
    [
        ACT_PL_BTN_RESUME,
        ACT_PL_BTN_RETRY,
        ACT_PL_BTN_OPTION,
        ACT_PL_BTN_WORLD,
    ];

    internal static readonly GME_PMENU_RESULT[] gm_pm_sel_result_tbl =
    [
        GME_PMENU_RESULT.GME_PMENU_RESULT_CANCEL,
        GME_PMENU_RESULT.GME_PMENU_RESULT_RETRY,
        GME_PMENU_RESULT.GME_PMENU_RESULT_OPTION,
        GME_PMENU_RESULT.GME_PMENU_RESULT_BACK,
    ];

    // width of the back-button glyph plus its label, per language, so the pair centres as one unit
    internal static readonly float[] gm_pmenu_bb_width_tbl =
        [74.0f, 90.0f, 106.0f, 114.0f, 108.0f, 92.0f];
}
