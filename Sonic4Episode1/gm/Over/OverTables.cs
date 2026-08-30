using static AppMain;

internal static partial class GmOver
{
    // G_OVER's ids are build-independent: the dump has file/common/ace/G_OVER*.HMA and no iPhone
    // counterpart. see docs/per-member-variants.md
    public const uint IDA_G_OVER_ACT_T_FADEOUT = 0;
    public const uint IDA_G_OVER_ACT_G_FADEOUT = 1;

    // and every G_OVER_<lang> lays its four out the same way, which is why one row serves all
    // eleven where the decompile had eleven identical copies
    public const uint IDA_G_OVER_LANG_ACT_T_TEX_TIME = 0;
    public const uint IDA_G_OVER_LANG_ACT_T_TEX_OVER = 1;
    public const uint IDA_G_OVER_LANG_ACT_G_TEX_GAME = 2;
    public const uint IDA_G_OVER_LANG_ACT_G_TEX_OVER = 3;

    private static readonly int[][] gm_over_tex_amb_idx_tbl =
    [
        [IDB_CPIT_MAIN_G_OVER_AMB, IDB_CPIT_MAIN_G_OVER_JP_AMB],
        [IDB_CPIT_MAIN_G_OVER_AMB, IDB_CPIT_MAIN_G_OVER_US_AMB],
        [IDB_CPIT_MAIN_G_OVER_AMB, IDB_CPIT_MAIN_G_OVER_FR_AMB],
        [IDB_CPIT_MAIN_G_OVER_AMB, IDB_CPIT_MAIN_G_OVER_IT_AMB],
        [IDB_CPIT_MAIN_G_OVER_AMB, IDB_CPIT_MAIN_G_OVER_GE_AMB],
        [IDB_CPIT_MAIN_G_OVER_AMB, IDB_CPIT_MAIN_G_OVER_SP_AMB],
        [IDB_CPIT_MAIN_G_OVER_AMB, IDB_CPIT_MAIN_G_OVER_FI_AMB],
        [IDB_CPIT_MAIN_G_OVER_AMB, IDB_CPIT_MAIN_G_OVER_PT_AMB],
        [IDB_CPIT_MAIN_G_OVER_AMB, IDB_CPIT_MAIN_G_OVER_RU_AMB],
        [IDB_CPIT_MAIN_G_OVER_AMB, IDB_CPIT_MAIN_G_OVER_CN_AMB],
        [IDB_CPIT_MAIN_G_OVER_AMB, IDB_CPIT_MAIN_G_OVER_HK_AMB],
    ];

    private static readonly int[][] gm_over_ama_amb_idx_tbl =
    [
        [IDB_CPIT_MAIN_G_OVER_AMA, IDB_CPIT_MAIN_G_OVER_JP_AMA],
        [IDB_CPIT_MAIN_G_OVER_AMA, IDB_CPIT_MAIN_G_OVER_US_AMA],
        [IDB_CPIT_MAIN_G_OVER_AMA, IDB_CPIT_MAIN_G_OVER_FR_AMA],
        [IDB_CPIT_MAIN_G_OVER_AMA, IDB_CPIT_MAIN_G_OVER_IT_AMA],
        [IDB_CPIT_MAIN_G_OVER_AMA, IDB_CPIT_MAIN_G_OVER_GE_AMA],
        [IDB_CPIT_MAIN_G_OVER_AMA, IDB_CPIT_MAIN_G_OVER_SP_AMA],
        [IDB_CPIT_MAIN_G_OVER_AMA, IDB_CPIT_MAIN_G_OVER_FI_AMA],
        [IDB_CPIT_MAIN_G_OVER_AMA, IDB_CPIT_MAIN_G_OVER_PT_AMA],
        [IDB_CPIT_MAIN_G_OVER_AMA, IDB_CPIT_MAIN_G_OVER_RU_AMA],
        [IDB_CPIT_MAIN_G_OVER_AMA, IDB_CPIT_MAIN_G_OVER_CN_AMA],
        [IDB_CPIT_MAIN_G_OVER_AMA, IDB_CPIT_MAIN_G_OVER_HK_AMA],
    ];

    private static readonly uint[] gm_over_string_act_id_tbl =
    [
        IDA_G_OVER_LANG_ACT_G_TEX_GAME,
        IDA_G_OVER_LANG_ACT_G_TEX_OVER,
        IDA_G_OVER_LANG_ACT_T_TEX_TIME,
        IDA_G_OVER_LANG_ACT_T_TEX_OVER,
    ];

    private static readonly uint[] gm_over_fadeout_act_id_tbl =
    [
        IDA_G_OVER_ACT_G_FADEOUT,
        IDA_G_OVER_ACT_T_FADEOUT,
    ];
}
