using System.Linq;

namespace Sonic4Episode1.Tests;

// gmSplStage.cpp has no platform fork and loads no archive, so the port is a straight move. What is
// worth pinning is the ring-gate table, because gmSplStageRingGateChk breaks out of the scan on the
// first GMD_SS_RINGGATE_NONE rather than skipping it: a gate configured after a sentinel is dead.
public class SplStageTableTests
{
    private const int SpecialStageNum = 7;

    [Fact]
    public void EverySpecialStageHasAStartingTime()
    {
        Assert.Equal(SpecialStageNum, GmSplStage.gm_spl_stage_init_time.Length);
        Assert.All(GmSplStage.gm_spl_stage_init_time, time => Assert.True(time > 0));
    }

    [Fact]
    public void EverySpecialStageHasAFullGateRow()
    {
        Assert.Equal(SpecialStageNum, GmSplStage.gm_spl_stage_ringgate_num.Length);
        Assert.All(
            GmSplStage.gm_spl_stage_ringgate_num,
            row => Assert.Equal(GmSplStage.GMD_SS_RINGGATE_MAX, row.Length));
    }

    // the sentinels have to be one trailing run, or the gates past the first one never open
    [Fact]
    public void NoGateSitsBehindASentinel()
    {
        for (int stage = 0; stage < SpecialStageNum; stage++)
        {
            var row = GmSplStage.gm_spl_stage_ringgate_num[stage];
            var first_none = System.Array.IndexOf(row, GmSplStage.GMD_SS_RINGGATE_NONE);
            if (first_none < 0)
                continue;

            Assert.All(
                row.Skip(first_none),
                gate => Assert.Equal(GmSplStage.GMD_SS_RINGGATE_NONE, gate));
        }
    }

    // GmSplStageSwSet masks the number down rather than bounds-checking it, so the mask and the
    // array have to agree exactly or a gimmick writes off the end or into the wrong slot
    [Fact]
    public void TheSwitchMaskCoversExactlyTheSwitchArray()
    {
        Assert.Equal(GmSplStage.GMD_SPL_STG_SW_MAX, GmSplStage.gm_gmk_ss_switch.Length);
        Assert.Equal(0, GmSplStage.GMD_SPL_STG_SW_MAX & (GmSplStage.GMD_SPL_STG_SW_MAX - 1));
    }

    // the three work flags are a bit field on GMS_SPL_STG_WORK, read by the gimmick layer
    [Fact]
    public void TheWorkFlagsDoNotOverlap()
    {
        var flags = new[]
        {
            GmSplStage.GMD_SPL_STAGE_BOUNCE_HIT,
            GmSplStage.GMD_SPL_STAGE_NUDGE_HIT,
            GmSplStage.GMD_SPL_STAGE_RESULT,
        };

        Assert.Equal(flags.Aggregate(0U, (a, f) => a | f), flags.Aggregate(0U, (a, f) => a + f));
    }
}
