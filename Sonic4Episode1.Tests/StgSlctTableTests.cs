using System;
using System.Linq;
using System.Reflection;

namespace Sonic4Episode1.Tests;

// The tables these cover were checked element by element against dm/DmStgSlct.cs while it still
// existed; that file is gone, so what is left here pins the invariants that hold on their own.
public class StgSlctTableTests
{
    private static object Table(Type owner, string name)
    {
        var field = owner.GetField(name, BindingFlags.NonPublic | BindingFlags.Static)
            ?? owner.GetField(name, BindingFlags.Public | BindingFlags.Static);

        Assert.True(field != null, $"{owner.Name}.{name} not found");
        return field.GetValue(null);
    }

    [Theory]
    [InlineData(typeof(StgSlct.DME_STGSLCT_ACT), "dm_stgslct_act_id_tbl_mobile", 116)]
    [InlineData(typeof(StgSlct.DME_STGSLCT_ACT_REG), "dm_stgslct_act_id_tbl_console", 130)]
    public void ActEnumAndItsIdTableStayTheSameLength(Type actEnum, string table, int expected)
    {
        var actNum = (int)Enum.Parse(actEnum, "ACT_NUM");
        var ids = (uint[])Table(typeof(StgSlct), table);

        Assert.Equal(expected, actNum);
        Assert.Equal(expected, ids.Length);
    }

    // ACT_NONE deliberately aliases a real action rather than sitting past the end
    [Theory]
    [InlineData(typeof(StgSlct.DME_STGSLCT_ACT))]
    [InlineData(typeof(StgSlct.DME_STGSLCT_ACT_REG))]
    public void ActAliasesLandWhereSegaPutThem(Type actEnum)
    {
        Assert.Equal(Enum.Parse(actEnum, "ACT_TAB_TABLE2"), Enum.Parse(actEnum, "ACT_TAB_START"));
        Assert.Equal(Enum.Parse(actEnum, "ACT_TAB_S_NUM9"), Enum.Parse(actEnum, "ACT_TAB_END"));
        Assert.Equal(Enum.Parse(actEnum, "ACT_TAB_COVER2"), Enum.Parse(actEnum, "ACT_NONE"));
    }

    // the descriptor instances live below the tables they name, in the same file, because
    // initialiser order across a partial class's files is undefined. this is what notices if
    // someone moves them.
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void VariantDescriptorIsFullyPopulated(bool console)
    {
        var variant = StgSlct.StgSlctVariantForTest(console);

        Assert.NotNull(variant);
        Assert.NotNull(variant.act_id_tbl);
        Assert.NotNull(variant.act_index);
        Assert.NotNull(variant.act_slot_tbl);
        Assert.NotNull(variant.zone_disp_pos_tbl);
        Assert.NotNull(variant.zone_nodisp_pos_tbl);
        Assert.NotNull(variant.act_disp_y_pos_tbl);
        Assert.NotNull(variant.act_tab_disp_y_pos_tbl);
        Assert.NotNull(variant.win_act_pos_tbl);
        Assert.NotNull(variant.input_zone_select);
        Assert.NotNull(variant.input_stage_select);
        Assert.NotNull(variant.input_win_disp_idle);

        Assert.Equal(console, variant.is_console);
        Assert.Equal(variant.act_num, variant.act_id_tbl.Length);
        Assert.Equal(4, variant.zone_disp_pos_tbl.Length);
        Assert.Equal(4, variant.zone_nodisp_pos_tbl.Length);
        Assert.Equal(24, variant.act_disp_y_pos_tbl.Length);
        Assert.Equal(14, variant.win_act_pos_tbl.Length);
        Assert.Equal(console ? 4 : 5, variant.act_tab_disp_y_pos_tbl.Length);
        Assert.NotEqual(0, variant.draw_prio_win);
    }

    // shared draw code names actions with the console enum and maps them through act_index, so a
    // shared action must resolve in both variants and a mobile-only one must not resolve at all
    [Theory]
    [InlineData("ACT_TAB_TABLE2")]
    [InlineData("ACT_TAB_SCR")]
    [InlineData("ACT_TAB_MESS")]
    [InlineData("ACT_TAB_S_NUM1")]
    [InlineData("ACT_TEX_ZONE_UP")]
    [InlineData("ACT_TEX_FIX_BACK")]
    [InlineData("ACT_BTN_CANCEL1")]
    [InlineData("ACT_WIN_TEX_MSG")]
    public void SharedActionsResolveInBothVariants(string name)
    {
        var key = (int)Enum.Parse(typeof(StgSlct.DME_STGSLCT_ACT_REG), name);

        foreach (var console in new[] { true, false })
        {
            var variant = StgSlct.StgSlctVariantForTest(console);

            var target = console
                ? typeof(StgSlct.DME_STGSLCT_ACT_REG)
                : typeof(StgSlct.DME_STGSLCT_ACT);

            Assert.InRange(variant.act_index[key], 0, variant.act_num - 1);

            // by value, not by name: aliases like ACT_TAB_START share a value with ACT_TAB_TABLE2
            // and Enum.GetName is free to hand back either
            Assert.Equal((int)Enum.Parse(target, name), variant.act_index[key]);
        }
    }

    // -1 rather than a wrong index, so touching one of these on mobile throws instead of drawing
    // the wrong sprite
    [Theory]
    [InlineData("ACT_WIN_LINE")]
    [InlineData("ACT_TEX_OK")]
    [InlineData("ACT_TEX_WINTITLE")]
    [InlineData("ACT_OBI_C")]
    [InlineData("ACT_TEX_SONIC")]
    [InlineData("ACT_TAB_ZONE_CURSOR1")]
    [InlineData("ACT_TAB_CURSOR_1")]
    public void ConsoleOnlyActionsAreAbsentOnMobile(string name)
    {
        var key = (int)Enum.Parse(typeof(StgSlct.DME_STGSLCT_ACT_REG), name);

        Assert.InRange(StgSlct.StgSlctVariantForTest(true).act_index[key], 0, 129);
        Assert.Equal(-1, StgSlct.StgSlctVariantForTest(false).act_index[key]);
    }

    // the four direction tables folded onto is_final_open, which indexes them 0 none,
    // 1 FINAL_OPEN, 2 SPECIAL_OPEN, 3 ALL_OPEN. Columns are up, down, left, right.
    [Theory]
    [InlineData(0, 0, 1, "DME_STGSLCT_ZONE_TYPE_2")]
    [InlineData(0, 0, 3, "DME_STGSLCT_ZONE_TYPE_3")]
    [InlineData(0, 4, 0, "DME_STGSLCT_ZONE_TYPE_NONE")]
    [InlineData(1, 0, 3, "DME_STGSLCT_ZONE_TYPE_FINAL")]
    [InlineData(2, 0, 3, "DME_STGSLCT_ZONE_TYPE_SPE")]
    [InlineData(3, 5, 0, "DME_STGSLCT_ZONE_TYPE_FINAL")]
    [InlineData(3, 5, 3, "DME_STGSLCT_ZONE_TYPE_4")]
    public void FoldedDirectionTableKeepsItsOpenStateOrder(
        int open, int zone, int dir, string expected)
    {
        var tbl = (StgSlct.DME_STGSLCT_ZONE_TYPE[][][])
            Table(typeof(StgSlct), "dm_stgslct_zone_input_dir_tbl");

        Assert.Equal(4, tbl.Length);
        Assert.All(tbl, open_state => Assert.Equal(6, open_state.Length));
        Assert.All(tbl, open_state => Assert.All(open_state, z => Assert.Equal(4, z.Length)));

        Assert.Equal(expected, tbl[open][zone][dir].ToString());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EverySlotNamesAnArchiveThatIsActuallyLoaded(bool console)
    {
        var variant = StgSlct.StgSlctVariantForTest(console);

        Assert.Equal(variant.act_num, variant.act_slot_tbl.Length);
        Assert.All(variant.act_slot_tbl, slot => Assert.InRange(slot, 0, 6));
    }

    // every id has to be a valid index into the archive it comes from, and the console main
    // archive only has 89 actions where mobile has 80
    [Theory]
    [InlineData("dm_stgslct_act_id_tbl_mobile", 80)]
    [InlineData("dm_stgslct_act_id_tbl_console", 89)]
    public void NoIdReachesPastTheLargestArchive(string table, int mainArchiveActions)
    {
        var ids = (uint[])Table(typeof(StgSlct), table);

        Assert.All(ids, id => Assert.InRange(id, 0u, (uint)mainArchiveActions - 1));
    }
}
