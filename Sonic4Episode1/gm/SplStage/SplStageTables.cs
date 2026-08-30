internal static partial class GmSplStage
{
    // one row per special stage, SS1 to SS7. The clock counts down, so these are the starting times.
    internal static readonly uint[] gm_spl_stage_init_time =
        [5400U, 5400U, 1800U, 5400U, 1800U, 5400U, 5400U];

    // ring count each gate opens at, GMD_SS_RINGGATE_NONE where the stage has no more gates
    internal static readonly ushort[][] gm_spl_stage_ringgate_num =
    [
        [35, 67, 255, 255, 255, 255, 255, 255, 255],
        [55, 255, 255, 255, 255, 255, 255, 255, 255],
        [80, 255, 255, 255, 255, 255, 255, 255, 255],
        [25, 50, 52, 255, 255, 255, 255, 255, 255],
        [30, 99, 99, 99, 99, 255, 255, 255, 255],
        [10, 28, 30, 45, 57, 70, 75, 93, 99],
        [99, 99, 99, 99, 255, 255, 255, 255, 255],
    ];

    // the gimmick layer sets and reads these by number; cleared at the start of every stage
    internal static readonly byte[] gm_gmk_ss_switch = new byte[GMD_SPL_STG_SW_MAX];
}
