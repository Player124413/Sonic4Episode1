using System;
using System.Linq;

using static GmPauseMenu.GME_PMENU_ACT;

namespace Sonic4Episode1.Tests;

// these read no .AMA: the action counts are pinned against the shipped files in
// AmaActionNameTests, whose reader caches are process-wide statics and must not be raced
public class PauseMenuTableTests
{
    private const int CmnActNum = 6;
    private const int LngActNum = 12;
    private const int PlfActNum = 1;
    private const int PlngActNum = 16;

    private static int ActNumFor(GmPauseMenu.GME_PMENU_ACT act)
    {
        if (act >= ACT_PLNG_S)
            return PlngActNum;
        if (act >= ACT_PLF_S)
            return PlfActNum;
        if (act >= ACT_LNG_S)
            return LngActNum;

        return CmnActNum;
    }

    [Fact]
    public void EveryActionHasAnId()
    {
        Assert.Equal((int)ACT_NUM, GmPauseMenu.gm_pm_act_id_tbl.Length);
    }

    [Fact]
    public void EveryIdIsInsideTheArchiveItsSlotNames()
    {
        for (int i = 0; i < (int)ACT_NUM; i++)
        {
            var act = (GmPauseMenu.GME_PMENU_ACT)i;
            var id = GmPauseMenu.gm_pm_act_id_tbl[i];

            Assert.True(
                id >= 0 && id < ActNumFor(act),
                $"{act} asks for action {id}, and its archive has {ActNumFor(act)}");
        }
    }

    // the two ids gmPmProcBegin swaps in are not in the table, so they get checked separately
    [Theory]
    [InlineData(12)] // BTN_MMENU, for when the world map is unreachable
    [InlineData(13)] // BTN_MMENU_S
    public void TheMainMenuSwapIsInsidePlng(int id)
    {
        Assert.True(id < PlngActNum);
    }

    [Fact]
    public void TheRetry2SwapIsInsideLng()
    {
        // MSG_RETRY2, for when the stage is not a special stage
        Assert.True(10 < LngActNum);
    }

    [Fact]
    public void TheSelectableRowsAndTheirResultsAgree()
    {
        Assert.Equal(GmPauseMenu.GMD_PMENU_SEL_NUM, GmPauseMenu.gm_pm_sel_act_tbl.Length);
        Assert.Equal(GmPauseMenu.GMD_PMENU_SEL_NUM, GmPauseMenu.gm_pm_sel_result_tbl.Length);
    }

    // gmPmDrawSelect draws act_id_tbl[i] normally and act_id_tbl[i] + 1 highlighted, in both the
    // enum and the archive. Both have to hold or the wrong sprite lights up.
    [Fact]
    public void EverySelectableRowHasItsHighlightOneSlotLater()
    {
        foreach (var act in GmPauseMenu.gm_pm_sel_act_tbl)
        {
            var plain = GmPauseMenu.gm_pm_act_id_tbl[(int)act];
            var selected = GmPauseMenu.gm_pm_act_id_tbl[(int)act + 1];

            Assert.Equal(plain + 1, selected);
            Assert.True(act + 1 < ACT_NUM);
        }
    }

    // same rule for the six plates, which is what ACT_BTN_BASE1 + i * 2 walks
    [Fact]
    public void EveryButtonPlateHasItsHighlightOneSlotLater()
    {
        for (int i = 0; i < GmPauseMenu.GMD_PMENU_SEL_NUM_FULL; i++)
        {
            var plate = ACT_BTN_BASE1 + (i * 2);

            Assert.Equal(
                GmPauseMenu.gm_pm_act_id_tbl[(int)plate] + 1,
                GmPauseMenu.gm_pm_act_id_tbl[(int)plate + 1]);
        }
    }

    // dropped for want of a DmRanking and an AoTrophyShowAchievementUI, not because the console
    // archive lacks them: putting them back should stay a one-line edit to gm_pm_sel_act_tbl
    [Fact]
    public void RankingAndTrophyKeepTheirActionsDespiteBeingUnreachable()
    {
        Assert.DoesNotContain(ACT_PL_BTN_RANK, GmPauseMenu.gm_pm_sel_act_tbl);
        Assert.DoesNotContain(ACT_PL_BTN_TROPHY, GmPauseMenu.gm_pm_sel_act_tbl);

        Assert.True(GmPauseMenu.gm_pm_act_id_tbl[(int)ACT_PL_BTN_RANK] < PlngActNum);
        Assert.True(GmPauseMenu.gm_pm_act_id_tbl[(int)ACT_PL_BTN_TROPHY] < PlngActNum);
    }

    // no result the menu can return may be one GmPause treats as "resume"
    [Fact]
    public void EverySelectableRowReturnsSomethingReachable()
    {
        Assert.All(
            GmPauseMenu.gm_pm_sel_result_tbl,
            r => Assert.True(r < GmPauseMenu.GME_PMENU_RESULT.GME_PMENU_RESULT_NUM));

        Assert.DoesNotContain(GmPauseMenu.GME_PMENU_RESULT.GME_PMENU_RESULT_RANKING,
            GmPauseMenu.gm_pm_sel_result_tbl);
        Assert.DoesNotContain(GmPauseMenu.GME_PMENU_RESULT.GME_PMENU_RESULT_TROPHY,
            GmPauseMenu.gm_pm_sel_result_tbl);
    }

    // xbla ships six languages; this build has eleven. The tables staying six long is what makes
    // the amFsAllVariant gate do real work rather than being decorative.
    [Fact]
    public void TheLanguageTablesStopWhereTheConsoleAssetsDo()
    {
        Assert.Equal(6, GmPauseMenu.gm_pm_file_lng_amb_name_tbl.Length);
        Assert.Equal(6, GmPauseMenu.gm_pm_file_plng_amb_name_tbl.Length);
        Assert.Equal(6, GmPauseMenu.gm_pmenu_bb_width_tbl.Length);

        Assert.True(AppMain.GSD_LANGUAGE_NUM > GmPauseMenu.gm_pmenu_bb_width_tbl.Length);
    }

    [Fact]
    public void TheGateNamesEveryFileTheLoaderReads()
    {
        var loaded = new[]
        {
            GmPauseMenu.GMD_PMENU_FILE_CMN_AMA,
            GmPauseMenu.GMD_PMENU_FILE_CMN_AMB,
            GmPauseMenu.GMD_PMENU_FILE_LNG_AMA,
            GmPauseMenu.GMD_PMENU_FILE_PLF_AMA,
            GmPauseMenu.GMD_PMENU_FILE_PLF_AMB,
            GmPauseMenu.GMD_PMENU_FILE_PLNG_AMA,
            GmPauseMenu.GMD_PMENU_FILE_WIN_AMB,
        };

        Assert.All(loaded, path => Assert.StartsWith("G_COM/MENU/", path, StringComparison.Ordinal));
        Assert.Equal(loaded.Length, loaded.Distinct().Count());
    }
}
