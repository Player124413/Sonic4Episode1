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
            (int)DmTitle.DME_TITLE_ACT.ACT_NUM,
            DmTitle.dm_title_g_dm_act_id_tbl_iphone.Length);
    }

    [Fact]
    public void ConsoleEnumMatchesItsIdTable()
    {
        Assert.Equal(
            (int)DmTitle.DME_TITLE_ACT_REG.ACT_NUM,
            DmTitle.dm_title_g_dm_act_id_tbl_pc.Length);
    }

    // regression: ACT_TEX_NO/ACT_TEX_BACK used to read past the end of AOS_ACTION[41]
    [Fact]
    public void EveryReferencedMobileActionIsInRange()
    {
        var referenced = new[]
        {
            DmTitle.DME_TITLE_ACT.ACT_TEX_YES, DmTitle.DME_TITLE_ACT.ACT_TEX_NO,
            DmTitle.DME_TITLE_ACT.ACT_TEX_BACK, DmTitle.DME_TITLE_ACT.ACT_WIN_TEX_MSG1,
            DmTitle.DME_TITLE_ACT.ACT_WIN_TEX_MSG2, DmTitle.DME_TITLE_ACT.ACT_BTN_CANCEL,
            DmTitle.DME_TITLE_ACT.ACT_WIN_NO_BTN_L, DmTitle.DME_TITLE_ACT.ACT_WIN_NO_BTN_C,
            DmTitle.DME_TITLE_ACT.ACT_WIN_YES_BTN_C, DmTitle.DME_TITLE_ACT.ACT_WIN_YES_BTN_R,
        };

        foreach (var act in referenced)
            Assert.InRange((int)act, 0, (int)DmTitle.DME_TITLE_ACT.ACT_NUM - 1);
    }

    [Fact]
    public void MobileAliasesSurvive()
    {
        Assert.Equal(DmTitle.DME_TITLE_ACT.ACT_LANG, DmTitle.DME_TITLE_ACT.ACT_TEX_START);
        Assert.Equal(DmTitle.DME_TITLE_ACT.ACT_NONE, DmTitle.DME_TITLE_ACT.ACT_TEX_GAME);
    }

    [Fact]
    public void MobileIdTableAgreesWithTheLanguageArchive()
    {
        var expected = new (DmTitle.DME_TITLE_ACT Act, uint Id)[]
        {
            (DmTitle.DME_TITLE_ACT.ACT_TEX_START, 6u),
            (DmTitle.DME_TITLE_ACT.ACT_TEX_TUDUKI, 1u),
            (DmTitle.DME_TITLE_ACT.ACT_TEX_OPTION, 2u),
            (DmTitle.DME_TITLE_ACT.ACT_TEX_KANZENBAN, 3u),
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
        Assert.NotNull(variant.act_index);
        Assert.NotNull(variant.fs_variant);

        Assert.Equal(variant.act_id_tbl.Length, variant.act_slot_tbl.Length);
        Assert.Equal(variant.act_id_tbl.Length, variant.act_num);
        Assert.Equal(iphone, !variant.is_console);
    }

    // both ACT enums alias a member onto the one before it (ACT_LANG/ACT_TEX_START on mobile,
    // ACT_LANG/ACT_TEX_START and ACT_NONE/ACT_TEX_GAME on console), so Enum.GetName can hand
    // act_index either name for those values. it only resolves correctly because the aliasing is
    // symmetric; this pins every name shared draw code actually looks up.
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ActIndexResolvesEverySharedName(bool iphone)
    {
        var variant = DmTitle.TitleVariantForTest(iphone);

        var shared = new[]
        {
            DmTitle.DME_TITLE_ACT_REG.ACT_TEX_YES, DmTitle.DME_TITLE_ACT_REG.ACT_TEX_NO,
            DmTitle.DME_TITLE_ACT_REG.ACT_WIN_TEX_MSG1, DmTitle.DME_TITLE_ACT_REG.ACT_WIN_TEX_MSG2,
            DmTitle.DME_TITLE_ACT_REG.ACT_TEX_START, DmTitle.DME_TITLE_ACT_REG.ACT_BTN_CANCEL,
        };

        foreach (var key in shared)
        {
            var idx = variant.act_index[(int)key];

            Assert.True(idx >= 0, $"{key} unresolved on {(iphone ? "mobile" : "console")}");
            Assert.InRange(idx, 0, variant.act_num - 1);
        }
    }

    // the three window actions console has and mobile does not. act_index reports them absent;
    // dmTitleWinSelectDraw still maps them to 0 rather than using that, see the TODO there
    [Fact]
    public void ActIndexReportsConsoleOnlyActionsAbsentOnMobile()
    {
        var mobile = DmTitle.TitleVariantForTest(true);
        var console = DmTitle.TitleVariantForTest(false);

        var console_only = new[]
        {
            DmTitle.DME_TITLE_ACT_REG.ACT_WIN_TEX_MSG3,
            DmTitle.DME_TITLE_ACT_REG.ACT_WIN_LINE,
            DmTitle.DME_TITLE_ACT_REG.ACT_TEX_WINTITLE,
            DmTitle.DME_TITLE_ACT_REG.ACT_TEX_OK,
        };

        foreach (var key in console_only)
        {
            Assert.Equal(-1, mobile.act_index[(int)key]);
            Assert.True(console.act_index[(int)key] >= 0);
        }
    }

    // only mobile has a second main-menu window size; the console pair is declared inside the
    // _IPHONE arm of dmTitle.cpp:161-174 and has no counterpart
    [Fact]
    public void SecondMainMenuWindowSizeIsMobileOnly()
    {
        Assert.NotNull(DmTitle.TitleVariantForTest(true).mmenu_win_size_w2);
        Assert.NotNull(DmTitle.TitleVariantForTest(true).mmenu_win_size_h2);

        Assert.Null(DmTitle.TitleVariantForTest(false).mmenu_win_size_w2);
        Assert.Null(DmTitle.TitleVariantForTest(false).mmenu_win_size_h2);
    }

    // every one of these took the wrong #if arm before the re-derive. 138 and 304 are the trailing
    // #else, which is Wii; the _PC || _XBOX values are 288 and 264. dmTitle.cpp:120-134, :161-175
    [Fact]
    public void ConsoleLayoutMatchesThePcArm()
    {
        var console = DmTitle.TitleVariantForTest(false);

        Assert.Equal(486f, console.mmenu_win_size_w);

        // the C++'s 288 sizes a seven-item menu; ours is five, so two 50-unit slots come off
        Assert.Equal(288f - 2f * 50f, console.mmenu_win_size_h);
        Assert.Equal(380f, console.window_size_w);
        Assert.Equal(1f, console.disp_scale_text);
        Assert.Equal(264f, console.crsr_dflt_pos_x);
    }

    [Fact]
    public void MobileLayoutValuesAreUnchanged()
    {
        var mobile = DmTitle.TitleVariantForTest(true);

        Assert.Equal(800f, mobile.mmenu_win_size_w);
        Assert.Equal(540f, mobile.mmenu_win_size_h);
        Assert.Equal(420f, mobile.window_size_w);
        Assert.Equal(1.6875f, mobile.disp_scale_text);
        Assert.Equal(304f, mobile.crsr_dflt_pos_x);
    }

    // decide_menu_frm is sized once and indexed by menu item. dropping RANK and LIBRARY is what
    // keeps the console menu at five and this array in range; the C++'s seven would overrun it
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MenuCountsFitTheDecideEffectArray(bool iphone)
    {
        var variant = DmTitle.TitleVariantForTest(iphone);
        var frames = new DmTitle.DMS_TITLE_MAIN_WORK().decide_menu_frm.Length;

        Assert.InRange(variant.slct_menu_num_first, 1, frames);
        Assert.InRange(variant.slct_menu_num_played, 1, frames);
        Assert.InRange(variant.slct_menu_num_trial, 1, frames);

        Assert.InRange((int)DmTitle.DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_NUM, 1, frames);
    }

    // slots 3 and 4 hold different items per build. before this existed, dmTitleSetNextEvent
    // switched on a bare index, so console's "Return to Game Library" ran the mobile arm's
    // leaderboards case and opened options with the Xbox Live flag set
    [Fact]
    public void MenuActionsSayWhatEachSlotDoes()
    {
        var mobile = DmTitle.TitleVariantForTest(true);
        var console = DmTitle.TitleVariantForTest(false);

        Assert.Equal(
            new[]
            {
                DmTitle.DME_TITLE_MENU_ACTION.NEW_GAME, DmTitle.DME_TITLE_MENU_ACTION.CONTINUE,
                DmTitle.DME_TITLE_MENU_ACTION.OPTION, DmTitle.DME_TITLE_MENU_ACTION.ACHIEVEMENTS,
                DmTitle.DME_TITLE_MENU_ACTION.LEADERBOARDS,
            },
            mobile.menu_actions);

        Assert.Equal(
            new[]
            {
                DmTitle.DME_TITLE_MENU_ACTION.NEW_GAME, DmTitle.DME_TITLE_MENU_ACTION.CONTINUE,
                DmTitle.DME_TITLE_MENU_ACTION.OPTION, DmTitle.DME_TITLE_MENU_ACTION.LIBRARY,
                DmTitle.DME_TITLE_MENU_ACTION.BUY,
            },
            console.menu_actions);
    }

    // the table is indexed by menu slot, and the cursor shift can push a slot one past its position
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MenuActionsCoverEverySlotTheCursorCanReach(bool iphone)
    {
        var variant = DmTitle.TitleVariantForTest(iphone);
        var reachable = new[]
        {
            variant.slct_menu_num_first, variant.slct_menu_num_played, variant.slct_menu_num_trial,
        };

        Assert.Equal((int)DmTitle.DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_NUM, variant.menu_actions.Length);

        // worst case is the highest cursor index plus the trial/first-play shift of one
        foreach (var count in reachable)
            Assert.True(count - 1 + 1 < variant.menu_actions.Length, $"{count} slots overruns menu_actions");
    }

    // the cursor y table is indexed by cur_slct_menu, which slct_menu_num bounds
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TrialCursorTableCoversEveryTrialMenuSlot(bool iphone)
    {
        var variant = DmTitle.TitleVariantForTest(iphone);

        Assert.NotNull(variant.crsr_trial_pos_y_tbl);
        Assert.True(
            variant.crsr_trial_pos_y_tbl.Length >= variant.slct_menu_num_trial,
            $"trial cursor table has {variant.crsr_trial_pos_y_tbl.Length} rows for {variant.slct_menu_num_trial} slots");
    }
}
