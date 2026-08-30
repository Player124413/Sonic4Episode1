using static AppMain;

internal static partial class GmScore
{
    // one step of the shake per frame, walked by (vib_timer >> FX32_SHIFT) & GMD_SCORE_VIB_TBL_MASK
    internal static readonly int[][] gm_score_vib_tbl =
    [
        [-1 * FX32_ONE, 0 * FX32_ONE],
        [0 * FX32_ONE, -1 * FX32_ONE],
        [1 * FX32_ONE, 0 * FX32_ONE],
        [0 * FX32_ONE, 1 * FX32_ONE],
        [-1 * FX32_ONE, 0 * FX32_ONE],
        [0 * FX32_ONE, 1 * FX32_ONE],
        [1 * FX32_ONE, 0 * FX32_ONE],
        [0 * FX32_ONE, -1 * FX32_ONE],
    ];

    // indexed by GME_SCORE_VIB_LEVEL: none, 1x, 1.5x, 2x
    internal static readonly int[] gm_score_vib_scale_tbl =
    [
        0,
        (int)(FX32_ONE * 1.0),
        (int)(FX32_ONE * 1.5),
        (int)(FX32_ONE * 2.0),
    ];

    internal static readonly int[] gm_score_digit_divisor_tbl = [10000, 1000, 100, 10, 1];
}
