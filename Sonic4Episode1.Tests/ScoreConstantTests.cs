namespace Sonic4Episode1.Tests;

// gmScore.cpp has no platform fork, so porting it was purely a matter of putting the constants the
// decompiler inlined back where the C++ has them. That substitution is the only thing that could
// have changed behaviour, so every derived value is pinned here against the literal it replaced.
public class ScoreConstantTests
{
    [Fact]
    public void TheSpawnOffsetsMatchTheLiteralsTheyReplaced()
    {
        Assert.Equal(-65536, GmScore.GMD_SCORE_DEF_OFST_Y * AppMain.FX32_ONE);
        Assert.Equal(1179648, GmScore.GMD_SCORE_POS_Z);
        Assert.Equal(184320, GmScore.GMD_SCORE_DISP_TIME * AppMain.FX32_ONE);
        Assert.Equal(99999, GmScore.GMD_SCORE_MAX);
    }

    // rise_dist was -8 * (scale - 4096) - 131072
    [Theory]
    [InlineData(4096)]
    [InlineData(6144)]
    [InlineData(8192)]
    public void TheRiseDistanceMatchesTheLiteralForm(int scale)
    {
        var ported = GmScore.GMD_SCORE_DEF_RIZE_DIST * AppMain.FX32_ONE
                     + GmScore.GMD_SCORE_RIZE_DIST_SCALE * (scale - AppMain.FX32_ONE);

        Assert.Equal(-8 * (scale - 4096) - 131072, ported);
        Assert.Equal(30, GmScore.GMD_SCORE_RIZE_TIME);
    }

    // ofst_x was ((n * 11 + (n - 1)) * 4096 >> 1) - 22528, stepping by -49152
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void TheDigitLayoutMatchesTheLiteralForm(int score_num)
    {
        var ported = (score_num * GmScore.GMD_SCORE_DEF_SIZE_X
                      + (score_num - 1) * GmScore.GMD_SCORE_DEF_INT_X) * AppMain.FX32_ONE >> 1;
        ported -= GmScore.GMD_SCORE_DEF_SIZE_X * AppMain.FX32_ONE / 2;

        Assert.Equal(((score_num * 11 + (score_num - 1)) * 4096 >> 1) - 22528, ported);
        Assert.Equal(-49152, -GmScore.GMD_SCORE_DEF_OFST_X * AppMain.FX32_ONE);
    }

    [Fact]
    public void TheVibrationTablesAreUnchanged()
    {
        Assert.Equal(GmScore.GMD_SCORE_VIB_TBL_NUM, GmScore.gm_score_vib_tbl.Length);
        Assert.Equal(
            new[] { 0, 4096, 6144, 8192 },
            GmScore.gm_score_vib_scale_tbl);

        var expected = new[]
        {
            new[] { -4096, 0 }, new[] { 0, -4096 }, new[] { 4096, 0 }, new[] { 0, 4096 },
            new[] { -4096, 0 }, new[] { 0, 4096 }, new[] { 4096, 0 }, new[] { 0, -4096 },
        };

        Assert.Equal(expected, GmScore.gm_score_vib_tbl);
    }

    // the scale table is indexed by vib_level straight from GmPlayer's combo table, unbounded
    [Fact]
    public void EveryVibrationLevelHasAScale()
    {
        Assert.Equal(
            (int)GmScore.GME_SCORE_VIB_LEVEL.GME_SCORE_VIB_LEVEL_MAX,
            GmScore.gm_score_vib_scale_tbl.Length);
    }

    // the shake walks the table with (timer >> FX32_SHIFT) & MASK, so the mask has to cover it
    [Fact]
    public void TheVibrationMaskCoversExactlyTheTable()
    {
        Assert.Equal(GmScore.GMD_SCORE_VIB_TBL_NUM - 1, GmScore.GMD_SCORE_VIB_TBL_MASK);
    }

    [Fact]
    public void TheDigitDivisorsCoverTheWholeRange()
    {
        Assert.Equal(GmScore.GMD_SCORE_FIG_MAX, GmScore.gm_score_digit_divisor_tbl.Length);
        Assert.Equal(new[] { 10000, 1000, 100, 10, 1 }, GmScore.gm_score_digit_divisor_tbl);

        // the largest score has to fit in the digits the table can split it into
        Assert.True(GmScore.GMD_SCORE_MAX < GmScore.gm_score_digit_divisor_tbl[0] * 10);
    }
}
