using static AppMain;

using static DmSave.DME_SAVE_ACT;

internal static partial class DmSave
{
    private const int ACT_COUNT = (int)ACT_NUM;

    internal static readonly string[] dm_save_menu_cmn_lng_amb_name_tbl =
    [
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
        "DEMO/CMN/D_CMN_MSG_HK.AMB",
    ];

    internal static readonly float[][] dm_save_win_act_pos_tbl =
    [
        [DMD_SAVE_SIZE_HALF_WIDTH + 42f, 280f],
        [DMD_SAVE_SIZE_HALF_WIDTH - 80f, 274f],
        [DMD_SAVE_SIZE_HALF_WIDTH, 360f],
        [DMD_SAVE_SIZE_HALF_WIDTH, 360f],
        [DMD_SAVE_SIZE_HALF_WIDTH, 360f],
        [DMD_SAVE_SIZE_HALF_WIDTH, 420f],
    ];

    // how much taller the window gets so the language's message fits. the C++ table is six wide
    // against GSD_LANGUAGE_NUM; the five WP7 additions all reuse the 72 row
    internal static readonly int[] dm_save_win_size_y_tbl =
    [
        0,
        32,
        72,
        72,
        104,
        72,
        72,
        72,
        72,
        72,
        72,
    ];

    // mobile has no win_line, tex_wintitle or tex_ok at all; the C++ maps all three to 0 there
    // (dmSave.cpp:298-317). tex_msg01..03 exist in both but sit at different ids.
    private static readonly uint[] dm_save_act_id_tbl_mobile =
    [
        0,
        0,
        IDA_D_CMN_MSG_JP_ACT_TEX_MSG01,
        IDA_D_CMN_MSG_JP_ACT_TEX_MSG02,
        IDA_D_CMN_MSG_JP_ACT_TEX_MSG03,
        0,
    ];

    private static readonly uint[] dm_save_act_id_tbl_console =
    [
        IDA_D_CMN_WIN_ACT_WIN_LINE_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_WINTITLE_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_MSG01_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_MSG02_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_MSG03_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_OK_PC,
    ];

    // these two must stay in this file, below every table they name. static field initialisers run
    // in textual order within a file, but the order across a partial class's files is not defined
    internal static readonly DMS_SAVE_VARIANT dm_save_variant_mobile = new DMS_SAVE_VARIANT
    {
        act_id_tbl = dm_save_act_id_tbl_mobile,

        text_scale = DMD_SAVE_DISP_SCALE_TEXT,

        ge_widens_window = true,

        fs_variant = AMD_FS_VARIANT_BASE,
    };

    internal static readonly DMS_SAVE_VARIANT dm_save_variant_console = new DMS_SAVE_VARIANT
    {
        act_id_tbl = dm_save_act_id_tbl_console,

        text_scale = 1f,

        ge_widens_window = false,

        fs_variant = AMD_FS_VARIANT_CONSOLE,
    };
}
