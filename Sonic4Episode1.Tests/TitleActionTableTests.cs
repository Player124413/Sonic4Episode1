using System;
using System.Linq;

namespace Sonic4Episode1.Tests;

// see docs/dmtitle-action-names.md for the ACT_TEX_START/ACT_LANG aliasing bug this guards against
public class TitleActionTableTests
{
    [Fact]
    public void MobileEnumMatchesItsIdTable()
    {
        Assert.Equal(
            (int)AppMain.DME_TITLE_ACT.ACT_NUM,
            DmTitle.dm_title_g_dm_act_id_tbl_iphone.Length);
    }

    [Fact]
    public void ConsoleEnumMatchesItsIdTable()
    {
        Assert.Equal(
            (int)AppMain.DME_TITLE_ACT_REG.ACT_NUM,
            DmTitle.dm_title_g_dm_act_id_tbl_pc.Length);
    }

    // regression: ACT_TEX_NO/ACT_TEX_BACK used to read past the end of AOS_ACTION[41]
    [Fact]
    public void EveryReferencedMobileActionIsInRange()
    {
        var referenced = new[]
        {
            AppMain.DME_TITLE_ACT.ACT_TEX_YES, AppMain.DME_TITLE_ACT.ACT_TEX_NO,
            AppMain.DME_TITLE_ACT.ACT_TEX_BACK, AppMain.DME_TITLE_ACT.ACT_WIN_TEX_MSG1,
            AppMain.DME_TITLE_ACT.ACT_WIN_TEX_MSG2, AppMain.DME_TITLE_ACT.ACT_BTN_CANCEL,
            AppMain.DME_TITLE_ACT.ACT_WIN_NO_BTN_L, AppMain.DME_TITLE_ACT.ACT_WIN_NO_BTN_C,
            AppMain.DME_TITLE_ACT.ACT_WIN_YES_BTN_C, AppMain.DME_TITLE_ACT.ACT_WIN_YES_BTN_R,
        };

        foreach (var act in referenced)
            Assert.InRange((int)act, 0, (int)AppMain.DME_TITLE_ACT.ACT_NUM - 1);
    }

    [Fact]
    public void MobileAliasesSurvive()
    {
        Assert.Equal(AppMain.DME_TITLE_ACT.ACT_LANG, AppMain.DME_TITLE_ACT.ACT_TEX_START);
        Assert.Equal(AppMain.DME_TITLE_ACT.ACT_NONE, AppMain.DME_TITLE_ACT.ACT_TEX_GAME);
    }

    [Fact]
    public void MobileIdTableAgreesWithTheLanguageArchive()
    {
        var expected = new (AppMain.DME_TITLE_ACT Act, uint Id)[]
        {
            (AppMain.DME_TITLE_ACT.ACT_TEX_START, 6u),
            (AppMain.DME_TITLE_ACT.ACT_TEX_TUDUKI, 1u),
            (AppMain.DME_TITLE_ACT.ACT_TEX_OPTION, 2u),
            (AppMain.DME_TITLE_ACT.ACT_TEX_KANZENBAN, 3u),
        };

        foreach (var (act, id) in expected)
            Assert.Equal(id, DmTitle.dm_title_g_dm_act_id_tbl_iphone[(int)act]);
    }

    // regression: the descriptors were declared above the tables they reference, and static field
    // initialisers run in textual order, so act_id_tbl bound to null and only blew up in game
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void VariantDescriptorIsFullyPopulated(bool iphone)
    {
        var variant = DmTitle.TitleVariantForTest(iphone);

        Assert.NotNull(variant);
        Assert.NotNull(variant.act_id_tbl);
        Assert.NotNull(variant.act_slot_tbl);
        Assert.NotNull(variant.win_act_pos_tbl);
        Assert.NotNull(variant.proc_win_input);

        Assert.Equal(variant.act_id_tbl.Length, variant.act_slot_tbl.Length);
    }
}
