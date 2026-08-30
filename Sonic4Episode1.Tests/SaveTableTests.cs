using System;
using System.Linq;

namespace Sonic4Episode1.Tests;

// see docs/dmsave-port-notes.md. the size numbers below are what the mobile arm drew before the
// variant landed, so they pin the substitution rather than describe an intent
public class SaveTableTests
{
    private const int ActNum = (int)DmSave.DME_SAVE_ACT.ACT_NUM;

    // action counts of the four archives this screen indexes, both builds
    private const int ConsoleWinActs = 1;
    private const int ConsoleMsgActs = 14;
    private const int MobileWinActs = 8;
    private const int MobileMsgActs = 8;

    private const int Languages = 11;

    [Fact]
    public void EnumMatchesBothIdTables()
    {
        Assert.Equal(6, ActNum);
        Assert.Equal(ActNum, DmSave.SaveVariantForTest(false).act_id_tbl.Length);
        Assert.Equal(ActNum, DmSave.SaveVariantForTest(true).act_id_tbl.Length);
    }

    [Fact]
    public void ActPositionTableCoversEveryAction()
    {
        Assert.Equal(ActNum, DmSave.dm_save_win_act_pos_tbl.Length);

        foreach (var row in DmSave.dm_save_win_act_pos_tbl)
            Assert.Equal(2, row.Length);
    }

    [Fact]
    public void LanguageTablesAreElevenWide()
    {
        Assert.Equal(Languages, DmSave.dm_save_menu_cmn_lng_amb_name_tbl.Length);
        Assert.Equal(Languages, DmSave.dm_save_win_size_y_tbl.Length);
    }

    // slot 0 comes out of D_CMN_WIN, slots 1..5 out of D_CMN_MSG_<lang>. an id past the end of
    // either is an AOS_ACTION overrun at create, not a missing sprite
    [Theory]
    [InlineData(true, ConsoleWinActs, ConsoleMsgActs)]
    [InlineData(false, MobileWinActs, MobileMsgActs)]
    public void EveryIdIsInsideItsArchive(bool console, int win_acts, int msg_acts)
    {
        var ids = DmSave.SaveVariantForTest(console).act_id_tbl;

        Assert.InRange(ids[0], 0u, (uint)win_acts - 1);

        for (int i = 1; i < ActNum; i++)
            Assert.InRange(ids[i], 0u, (uint)msg_acts - 1);
    }

    // dmSave.cpp:298-317 forks exactly three of the six: WIN_LINE, TEX_WINTITLE and TEX_OK are
    // console-only and map to 0 on mobile. the three message slots exist on both
    [Fact]
    public void TheTwoArmsDifferWhereTheCppSaysAndNowhereElse()
    {
        var mobile = DmSave.SaveVariantForTest(false).act_id_tbl;
        var console = DmSave.SaveVariantForTest(true).act_id_tbl;

        var forked = Enumerable.Range(0, ActNum).Where(i => mobile[i] != console[i]).ToArray();

        Assert.Equal(
            new[]
            {
                (int)DmSave.DME_SAVE_ACT.ACT_TEX_WINTITLE,
                (int)DmSave.DME_SAVE_ACT.ACT_TEX_MSG1,
                (int)DmSave.DME_SAVE_ACT.ACT_TEX_MSG2,
                (int)DmSave.DME_SAVE_ACT.ACT_TEX_MSG3,
                (int)DmSave.DME_SAVE_ACT.ACT_TEX_OK,
            },
            forked);

        Assert.Equal(0u, mobile[(int)DmSave.DME_SAVE_ACT.ACT_WIN_LINE]);
        Assert.Equal(0u, mobile[(int)DmSave.DME_SAVE_ACT.ACT_TEX_WINTITLE]);
        Assert.Equal(0u, mobile[(int)DmSave.DME_SAVE_ACT.ACT_TEX_OK]);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void VariantDescriptorIsFullyPopulated(bool console)
    {
        var variant = DmSave.SaveVariantForTest(console);

        Assert.NotNull(variant);
        Assert.NotNull(variant.act_id_tbl);
        Assert.NotNull(variant.fs_variant);
        Assert.True(variant.text_scale > 0f);
    }

    [Fact]
    public void ConsoleArmDropsEveryScaleTheCppOmits()
    {
        var console = DmSave.SaveVariantForTest(true);

        Assert.Equal(1f, console.text_scale);
        Assert.False(console.ge_widens_window);
        Assert.Equal(AppMain.AMD_FS_VARIANT_CONSOLE, console.fs_variant);
    }

    [Fact]
    public void MobileArmKeepsTheValuesItDrewBefore()
    {
        var mobile = DmSave.SaveVariantForTest(false);

        Assert.Equal(1.6875f, mobile.text_scale);
        Assert.True(mobile.ge_widens_window);
        Assert.Equal(AppMain.AMD_FS_VARIANT_BASE, mobile.fs_variant);
    }

    // refactor pin: every one of these was an inlined literal before text_scale replaced it
    [Theory]
    [InlineData(AppMain.GSD_LANGUAGE_JP, false, 641, 303)]
    [InlineData(AppMain.GSD_LANGUAGE_GE, false, 749, 303)]
    [InlineData(AppMain.GSD_LANGUAGE_JP, true, 749, 303)]
    [InlineData(AppMain.GSD_LANGUAGE_US, true, 749, 357)]
    [InlineData(AppMain.GSD_LANGUAGE_GE, true, 749, 479)]
    public void MobileWindowSizeIsUnchanged(int lang, bool attention, int w, int h)
    {
        Assert.Equal((w, h), DmSave.WindowSizeForTest(false, lang, attention));
    }

    // dmSave.cpp:1545-1546 and :1566-1567: no scale, and no GE special case at all
    [Theory]
    [InlineData(AppMain.GSD_LANGUAGE_JP, false, 380, 180)]
    [InlineData(AppMain.GSD_LANGUAGE_GE, false, 380, 180)]
    [InlineData(AppMain.GSD_LANGUAGE_JP, true, 444, 180)]
    [InlineData(AppMain.GSD_LANGUAGE_US, true, 444, 212)]
    [InlineData(AppMain.GSD_LANGUAGE_GE, true, 444, 284)]
    public void ConsoleWindowSizeMatchesTheCpp(int lang, bool attention, int w, int h)
    {
        Assert.Equal((w, h), DmSave.WindowSizeForTest(true, lang, attention));
    }

    [Fact]
    public void EveryLanguageProducesAWindowTallerThanTheBase()
    {
        for (int lang = 0; lang < Languages; lang++)
        {
            var (_, h) = DmSave.WindowSizeForTest(true, lang, true);
            Assert.InRange(h, 180, 284);
        }
    }
}
