using System.Linq;

namespace Sonic4Episode1.Tests;

public class GmFixVariantTests
{
    private static int[] Mobile => GmFix.gm_fix_variant_mobile.timer_timeattack_act_id_tbl;
    private static int[] Console => GmFix.gm_fix_variant_console.timer_timeattack_act_id_tbl;

    // common/ace/G_FIX.HMA has 45 ids, iPhone/ace/G_FIX.HMA has 64
    private const int ConsoleActNum = 45;

    [Fact]
    public void TheTimeAttackTablesDifferOnlyAtTheTransposedPair()
    {
        Assert.Equal(Mobile.Length, Console.Length);

        for (int i = 0; i < Mobile.Length; i++)
        {
            if (i == 2 || i == 3)
                continue;

            Assert.Equal(Mobile[i], Console[i]);
        }
    }

    [Fact]
    public void TheTransposedPairIsSwappedAndNotReordered()
    {
        Assert.Equal(29, Mobile[2]);
        Assert.Equal(30, Mobile[3]);

        Assert.Equal(30, Console[2]);
        Assert.Equal(29, Console[3]);
    }

    [Fact]
    public void TheConsoleVariantNamesNothingPastItsArchive()
    {
        Assert.All(Console, id => Assert.True(id < ConsoleActNum, $"action {id} is not in console G_FIX"));
    }

    [Fact]
    public void TheVirtualPadIsEntirelyMobile()
    {
        var ids = GmFix.gm_fix_virtual_pad_act_id_tbl.SelectMany(row => row).Where(id => id >= 0);

        Assert.NotEmpty(ids);
        Assert.All(ids, id => Assert.True(id >= ConsoleActNum, $"action {id} is not mobile-only"));

        Assert.True(GmFix.gm_fix_variant_mobile.has_virtual_pad);
        Assert.False(GmFix.gm_fix_variant_console.has_virtual_pad);
    }

    // the plan is a mobile-only concept: the C++ has no GME_FIX_PLAN and no plan-indexed table on
    // console, so nothing on the console path may consult it
    [Fact]
    public void MobilePicksItsLayoutFromTheInputMode()
    {
        Assert.Equal(
            GmFix.GME_FIX_PLAN.GME_FIX_PLAN_VIRTUAL_PAD,
            GmFix.GmFixPlanFor(false, AppMain.GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC));

        Assert.Equal(
            GmFix.GME_FIX_PLAN.GME_FIX_PLAN_FLICK,
            GmFix.GmFixPlanFor(true, AppMain.GSD_MAINSYS_GAME_FLAG_INPUT_IS_FLICK));

        Assert.Equal(
            GmFix.GME_FIX_PLAN.GME_FIX_PLAN_TILT,
            GmFix.GmFixPlanFor(false, 0U));
    }

    [Fact]
    public void TheConsoleVariantSuppliesItsOwnPlanFreeTables()
    {
        Assert.NotNull(GmFix.gm_fix_variant_console.score_stage22_act_id_tbl);
        Assert.NotNull(GmFix.gm_fix_variant_console.challenge_act_id_tbl);

        Assert.Null(GmFix.gm_fix_variant_mobile.score_stage22_act_id_tbl);
        Assert.Null(GmFix.gm_fix_variant_mobile.challenge_act_id_tbl);
    }

    // this is the one place borrowing plan 0 would have been silently wrong
    [Fact]
    public void ConsoleDrawsTheScoreIconOnActTwoTwoWhereMobileDrawsNothing()
    {
        Assert.Equal(35, GmFix.gm_fix_variant_console.score_stage22_act_id_tbl[0]);
        Assert.Equal(-1, GmFix.gm_fix_score_stage22_act_id_tbl[0][0]);

        Assert.Equal(
            GmFix.gm_fix_variant_console.score_stage22_act_id_tbl.Skip(1),
            GmFix.gm_fix_score_stage22_act_id_tbl[0].Skip(1));
    }

    [Fact]
    public void TheChallengeTablesAgreeBetweenBuilds()
    {
        Assert.Equal(
            GmFix.gm_fix_variant_console.challenge_act_id_tbl,
            GmFix.gm_fix_challenge_act_id_tbl[0]);
    }

    [Fact]
    public void EveryConsoleArmStaysInsideTheConsoleArchive()
    {
        var console = GmFix.gm_fix_variant_console;

        var ids = console.timer_timeattack_act_id_tbl
            .Concat(console.score_stage22_act_id_tbl)
            .Concat(console.challenge_act_id_tbl)
            .Where(id => id >= 0);

        Assert.All(ids, id => Assert.True(id < ConsoleActNum, $"action {id} is not in console G_FIX"));
    }

    // the mobile-only plans are what reach the DA_* block, and console must never see them
    [Fact]
    public void TheMobilePlansReachPastTheConsoleArchive()
    {
        foreach (var plan in new[] { 1, 2 })
        {
            Assert.Contains(GmFix.gm_fix_score_stage22_act_id_tbl[plan], id => id >= ConsoleActNum);
            Assert.Contains(GmFix.gm_fix_challenge_act_id_tbl[plan], id => id >= ConsoleActNum);
        }
    }

    [Fact]
    public void TheBehaviourFlagsAreOppositeWhereTheCppSaysTheyAre()
    {
        Assert.True(GmFix.gm_fix_variant_mobile.timer_timeattack_x_offset);
        Assert.False(GmFix.gm_fix_variant_console.timer_timeattack_x_offset);

        Assert.False(GmFix.gm_fix_variant_mobile.score_narrowscreen_offset);
        Assert.True(GmFix.gm_fix_variant_console.score_narrowscreen_offset);
    }
}
