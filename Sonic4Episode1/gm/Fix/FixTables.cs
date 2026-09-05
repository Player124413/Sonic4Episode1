using static AppMain;

using static GmFix.GME_FIX_CHALLENGE_SUBPART;
using static GmFix.GME_FIX_RING_SUBPART;
using static GmFix.GME_FIX_SCORE_SUBPART;
using static GmFix.GME_FIX_TIMER_SUBPART;

internal static partial class GmFix
{
    // G_FIX's two builds agree on 0-28 and 31-44, transpose 29 and 30, and the mobile one appends
    // 45-63. only the time-attack counter and the virtual pad reach any of that.
    private static class MobileIds
    {
        public const int TA_NUM_C_A = 29;
        public const int TA_NUM_C_1000 = 30;

        public const int ICON_SUPER01 = 45;
        public const int START = 46;
        public const int CROSS = 47;
        public const int JUMP = 48;
        public const int DA_ICON_SONIC = 49;
        public const int DA_ICON_SUPER01 = 63;
    }

    private static class ConsoleIds
    {
        public const int TA_NUM_C_1000 = 29;
        public const int TA_NUM_C_A = 30;
    }

    private static readonly int[][] gm_fix_tex_amb_idx_tbl =
    [
        [IDB_CPIT_MAIN_G_FIX_AMB, IDB_CPIT_MAIN_SPST_AMB, IDB_CPIT_MAIN_SPST_JP_AMB],
        [IDB_CPIT_MAIN_G_FIX_AMB, IDB_CPIT_MAIN_SPST_AMB, IDB_CPIT_MAIN_SPST_US_AMB],
        [IDB_CPIT_MAIN_G_FIX_AMB, IDB_CPIT_MAIN_SPST_AMB, IDB_CPIT_MAIN_SPST_FR_AMB],
        [IDB_CPIT_MAIN_G_FIX_AMB, IDB_CPIT_MAIN_SPST_AMB, IDB_CPIT_MAIN_SPST_IT_AMB],
        [IDB_CPIT_MAIN_G_FIX_AMB, IDB_CPIT_MAIN_SPST_AMB, IDB_CPIT_MAIN_SPST_GE_AMB],
        [IDB_CPIT_MAIN_G_FIX_AMB, IDB_CPIT_MAIN_SPST_AMB, IDB_CPIT_MAIN_SPST_SP_AMB],
        [IDB_CPIT_MAIN_G_FIX_AMB, IDB_CPIT_MAIN_SPST_AMB, IDB_CPIT_MAIN_SPST_FI_AMB],
        [IDB_CPIT_MAIN_G_FIX_AMB, IDB_CPIT_MAIN_SPST_AMB, IDB_CPIT_MAIN_SPST_PT_AMB],
        [IDB_CPIT_MAIN_G_FIX_AMB, IDB_CPIT_MAIN_SPST_AMB, IDB_CPIT_MAIN_SPST_RU_AMB],
        [IDB_CPIT_MAIN_G_FIX_AMB, IDB_CPIT_MAIN_SPST_AMB, IDB_CPIT_MAIN_SPST_CN_AMB],
        [IDB_CPIT_MAIN_G_FIX_AMB, IDB_CPIT_MAIN_SPST_AMB, IDB_CPIT_MAIN_SPST_HK_AMB],
    ];

    private static readonly int[][] gm_fix_ama_amb_idx_tbl =
    [
        [IDB_CPIT_MAIN_G_FIX_AMA, IDB_CPIT_MAIN_SPST_AMA, IDB_CPIT_MAIN_SPST_JP_AMA],
        [IDB_CPIT_MAIN_G_FIX_AMA, IDB_CPIT_MAIN_SPST_AMA, IDB_CPIT_MAIN_SPST_US_AMA],
        [IDB_CPIT_MAIN_G_FIX_AMA, IDB_CPIT_MAIN_SPST_AMA, IDB_CPIT_MAIN_SPST_FR_AMA],
        [IDB_CPIT_MAIN_G_FIX_AMA, IDB_CPIT_MAIN_SPST_AMA, IDB_CPIT_MAIN_SPST_IT_AMA],
        [IDB_CPIT_MAIN_G_FIX_AMA, IDB_CPIT_MAIN_SPST_AMA, IDB_CPIT_MAIN_SPST_GE_AMA],
        [IDB_CPIT_MAIN_G_FIX_AMA, IDB_CPIT_MAIN_SPST_AMA, IDB_CPIT_MAIN_SPST_SP_AMA],
        [IDB_CPIT_MAIN_G_FIX_AMA, IDB_CPIT_MAIN_SPST_AMA, IDB_CPIT_MAIN_SPST_FI_AMA],
        [IDB_CPIT_MAIN_G_FIX_AMA, IDB_CPIT_MAIN_SPST_AMA, IDB_CPIT_MAIN_SPST_PT_AMA],
        [IDB_CPIT_MAIN_G_FIX_AMA, IDB_CPIT_MAIN_SPST_AMA, IDB_CPIT_MAIN_SPST_RU_AMA],
        [IDB_CPIT_MAIN_G_FIX_AMA, IDB_CPIT_MAIN_SPST_AMA, IDB_CPIT_MAIN_SPST_CN_AMA],
        [IDB_CPIT_MAIN_G_FIX_AMA, IDB_CPIT_MAIN_SPST_AMA, IDB_CPIT_MAIN_SPST_HK_AMA],
    ];

    // SPST has seven actions and every SPST_<lang> exactly one, so this was eleven identical rows
    private static readonly int[] gm_fix_timer_ss_act_id_tbl =
    [
        IDA_SPST_LANG_ACT_TEX_TIME, IDA_SPST_LANG_ACT_TEX_TIME,
        IDA_SPST_ACT_2, IDA_SPST_ACT_3, IDA_SPST_ACT_4,
        IDA_SPST_ACT_5, IDA_SPST_ACT_6, IDA_SPST_ACT_7,
    ];

    public const int IDA_SPST_LANG_ACT_TEX_TIME = 0;
    public const int IDA_SPST_ACT_2 = 1;
    public const int IDA_SPST_ACT_3 = 2;
    public const int IDA_SPST_ACT_4 = 3;
    public const int IDA_SPST_ACT_5 = 4;
    public const int IDA_SPST_ACT_6 = 5;
    public const int IDA_SPST_ACT_7 = 6;

    private static readonly int[] gm_fix_ringcount_act_id_tbl =
    [
        IDA_G_FIX_ACT_ICON_RING,
        IDA_G_FIX_ACT_NUM_A_100,
        IDA_G_FIX_ACT_NUM_A_10,
        IDA_G_FIX_ACT_NUM_A_1,
    ];

    private static readonly int[] gm_fix_score_act_id_tbl =
    [
        4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
    ];

    // console declares this flat and puts the score icon at subpart 0, where mobile puts -1, "no
    // subpart". The digit index table never reaches slot 0, so the icon is the only difference.
    internal static readonly int[] gm_fix_score_stage22_act_id_tbl_console =
    [
        IDA_G_FIX_ACT_ICON_SCORE, 36, 37, 38, 39, 40, 41, 42, 43, 44,
    ];

    internal static readonly int[][] gm_fix_score_stage22_act_id_tbl =
    [
        [-1, 36, 37, 38, 39, 40, 41, 42, 43, 44],
        [-1, 54, 55, 56, 57, 58, 59, 60, 61, 62],
        [-1, 54, 55, 56, 57, 58, 59, 60, 61, 62],
    ];

    private static readonly int[] gm_fix_timer_act_id_tbl =
    [
        14, 15, 16, 17, 18, 19, 20, 21,
    ];

    // the only table that straddles the transposed pair. subparts are positional, so the console
    // arm names the same two sprites at the indices that build gives them
    private static readonly int[] gm_fix_timer_timeattack_act_id_tbl_mobile =
    [
        27, 28, MobileIds.TA_NUM_C_A, MobileIds.TA_NUM_C_1000, 31, 32, 33, 34,
    ];

    private static readonly int[] gm_fix_timer_timeattack_act_id_tbl_console =
    [
        27, 28, ConsoleIds.TA_NUM_C_A, ConsoleIds.TA_NUM_C_1000, 31, 32, 33, 34,
    ];

    // console declares this flat too, and its contents are the mobile TILT row exactly
    internal static readonly int[] gm_fix_challenge_act_id_tbl_console =
    [
        IDA_G_FIX_ACT_ICON_SONIC, IDA_G_FIX_ACT_TEX_SONIC,
        IDA_G_FIX_ACT_NUM_D_100, IDA_G_FIX_ACT_NUM_D_10, IDA_G_FIX_ACT_NUM_D_1,
    ];

    internal static readonly int[][] gm_fix_challenge_act_id_tbl =
    [
        gm_fix_challenge_act_id_tbl_console,
        [49, 50, 51, 52, 53],
        [49, 50, 51, 52, 53],
    ];

    internal static readonly int[][] gm_fix_virtual_pad_act_id_tbl =
    [
        [MobileIds.ICON_SUPER01, MobileIds.START, -1, -1],
        [MobileIds.DA_ICON_SUPER01, MobileIds.START, MobileIds.JUMP, -1],
        [MobileIds.DA_ICON_SUPER01, MobileIds.START, MobileIds.JUMP, MobileIds.CROSS],
    ];

    private static readonly float[] gm_fix_part_common_digit_type_frame_tbl =
    [
        0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f,
    ];

    // one longer than the common one: the ring counter blanks its digits at zero
    private static readonly float[] gm_fix_part_ring_count_digit_type_frame_tbl =
    [
        0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f,
    ];

    // digits arrive most-significant first, the subparts run the other way
    private static readonly int[] gm_fix_part_ring_count_digit_subpart_idx_tbl =
    [
        (int)GME_FIX_RING_SUBPART_DIGIT_1,
        (int)GME_FIX_RING_SUBPART_DIGIT_10,
        (int)GME_FIX_RING_SUBPART_DIGIT_100,
    ];

    private static readonly int[] gm_fix_part_score_digit_subpart_idx_tbl =
    [
        (int)GME_FIX_SCORE_SUBPART_NUM_9, (int)GME_FIX_SCORE_SUBPART_NUM_8,
        (int)GME_FIX_SCORE_SUBPART_NUM_7, (int)GME_FIX_SCORE_SUBPART_NUM_6,
        (int)GME_FIX_SCORE_SUBPART_NUM_5, (int)GME_FIX_SCORE_SUBPART_NUM_4,
        (int)GME_FIX_SCORE_SUBPART_NUM_3, (int)GME_FIX_SCORE_SUBPART_NUM_2,
        (int)GME_FIX_SCORE_SUBPART_NUM_1,
    ];

    private static readonly int[] gm_fix_part_timer_digit_subpart_idx_tbl =
    [
        (int)GME_FIX_TIMER_SUBPART_NUM_1, (int)GME_FIX_TIMER_SUBPART_NUM_10,
        (int)GME_FIX_TIMER_SUBPART_NUM_100, (int)GME_FIX_TIMER_SUBPART_NUM_1000,
        (int)GME_FIX_TIMER_SUBPART_NUM_10000,
    ];

    private static readonly int[] gm_fix_part_timer_deco_char_subpart_idx_tbl =
    [
        (int)GME_FIX_TIMER_SUBPART_NUM_AP1,
        (int)GME_FIX_TIMER_SUBPART_NUM_AP2,
    ];

    private static readonly int[] gm_fix_part_challenge_digit_subpart_idx_tbl =
    [
        (int)GME_FIX_CHALLENGE_SUBPART_NUM_1,
        (int)GME_FIX_CHALLENGE_SUBPART_NUM_10,
        (int)GME_FIX_CHALLENGE_SUBPART_NUM_100,
    ];

    // msec, sec, min, in the order the digit list is filled
    private static readonly int[] gm_fix_timer_digit_num_tbl =
    [
        GMD_FIX_PART_TIMER_MSEC_DIGIT_NUM,
        GMD_FIX_PART_TIMER_SEC_DIGIT_NUM,
        GMD_FIX_PART_TIMER_MIN_DIGIT_NUM,
    ];

    private static readonly SKeyToFrame[] c_key_to_frame_table =
    [
        new SKeyToFrame { key = 8, frame = 4f },
        new SKeyToFrame { key = 4, frame = 3f },
        new SKeyToFrame { key = 2, frame = 2f },
        new SKeyToFrame { key = 1, frame = 1f },
    ];

    // these two must stay in this file, below every table they name. static field initialisers run
    // in textual order within a file, but the order across a partial class's files is not defined
    // mobile's two plan-indexed tables are resolved through gmFixGetPlan at use, not here
    internal static readonly GMS_FIX_VARIANT gm_fix_variant_mobile = new GMS_FIX_VARIANT
    {
        timer_timeattack_act_id_tbl = gm_fix_timer_timeattack_act_id_tbl_mobile,
        score_stage22_act_id_tbl = null,
        challenge_act_id_tbl = null,

        has_virtual_pad = true,
        timer_timeattack_x_offset = true,
        score_narrowscreen_offset = false,
        is_console = false,
    };

    internal static readonly GMS_FIX_VARIANT gm_fix_variant_console = new GMS_FIX_VARIANT
    {
        timer_timeattack_act_id_tbl = gm_fix_timer_timeattack_act_id_tbl_console,
        score_stage22_act_id_tbl = gm_fix_score_stage22_act_id_tbl_console,
        challenge_act_id_tbl = gm_fix_challenge_act_id_tbl_console,

        has_virtual_pad = false,
        timer_timeattack_x_offset = false,
        score_narrowscreen_offset = true,
        is_console = true,
    };
}
