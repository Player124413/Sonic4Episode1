using System.Linq;

namespace Sonic4Episode1.Tests;

// The tables PC's Lost Labyrinth act 2 needs: the widened gimmick dwork block, the stage 9 loadout,
// and the 54-row effect table with the first model rows any zone table in this tree has had
public class Zone3PcTableTests
{
    private static readonly int[] PcOnlyAnims =
    {
        0, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 43, 44, 45, 46, 48, 49, 50,
    };

    // the array is sized from the two constants and indexed data_no - GMK_START, so a slot outside
    // the block reads off the end. This is the pin for the nine that went in at 1011
    [Fact]
    public void TheGimmickDworkBlockCoversItsArray()
    {
        Assert.Equal(
            AppMain.GMD_DWORK_NO_GMK_END - AppMain.GMD_DWORK_NO_GMK_START,
            AppMain.g_gm_gamedat_gimmick.Length);
    }

    [Fact]
    public void TheNewGimmickSlotsAreInsideTheBlock()
    {
        int[] slots =
        {
            AppMain.GMD_DWORK_NO_GMK_TORCH_MDL,
            AppMain.GMD_DWORK_NO_GMK_TORCH_TEX,
            AppMain.GMD_DWORK_NO_GMK_CANDLE_MDL,
            AppMain.GMD_DWORK_NO_GMK_CANDLE_TEX,
            AppMain.GMD_DWORK_NO_GMK_RAILEND_MDL,
            AppMain.GMD_DWORK_NO_GMK_RAILEND_TEX,
            AppMain.GMD_DWORK_NO_GMK_DYNAMITE_MDL,
            AppMain.GMD_DWORK_NO_GMK_DYNAMITE_TEX,
            AppMain.GMD_DWORK_NO_GMK_DYNAMITE_MAT,
        };

        Assert.All(slots, slot => Assert.InRange(slot, AppMain.GMD_DWORK_NO_GMK_START, AppMain.GMD_DWORK_NO_GMK_END - 1));
        Assert.Equal(slots.Length, slots.Distinct().Count());
    }

    [Fact]
    public void EveryLoadTableSlotIsInsideTheAllocation()
    {
        var tables = new[]
        {
            AppMain.gm_gamedat_tbl_gimmick03_02,
            AppMain.gm_gamedat_tbl_gimmick03_02_pc,
        };

        foreach (var table in tables)
            Assert.All(table, row => Assert.InRange(row.user_data, 0, AppMain.GMD_DWORK_NO_MAX - 1));
    }

    // the leak subtracts the wrong block start in two of the three, which lands the model's texture
    // AMB in the model's own slot. Nothing caught it because no mobile-line table has a model row
    [Fact]
    public void TheModelDworkHelpersStayInTheirOwnBlocks()
    {
        for (int slot = AppMain.GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_START;
             slot < AppMain.GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_END;
             ++slot)
        {
            int offset = slot - AppMain.GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_START;

            Assert.Equal(AppMain.GMD_DWORK_NO_EFFECT_ZONE_OBJECT_START + offset, AppMain.gmEfctZoneGetObjectDworkNo(slot));
            Assert.Equal(AppMain.GMD_DWORK_NO_EFFECT_ZONE_MDL_AMBTEX_START + offset, AppMain.gmEfctZoneGetMdlAmbtexDworkNo(slot));
            Assert.Equal(AppMain.GMD_DWORK_NO_EFFECT_ZONE_MDL_TEXLIST_START + offset, AppMain.gmEfctZoneGetMdlTexlistDworkNo(slot));
        }
    }

    [Fact]
    public void TheFourModelBlocksAreTheSameWidthAndDoNotOverlap()
    {
        int width = AppMain.GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_END - AppMain.GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_START;

        Assert.Equal(AppMain.GMD_DWORK_NO_EFFECT_ZONE_OBJECT_END - AppMain.GMD_DWORK_NO_EFFECT_ZONE_OBJECT_START, width);
        Assert.Equal(AppMain.GMD_DWORK_NO_EFFECT_ZONE_MDL_AMBTEX_END - AppMain.GMD_DWORK_NO_EFFECT_ZONE_MDL_AMBTEX_START, width);
        Assert.Equal(AppMain.GMD_DWORK_NO_EFFECT_ZONE_MDL_TEXLIST_END - AppMain.GMD_DWORK_NO_EFFECT_ZONE_MDL_TEXLIST_START, width);

        Assert.Equal(AppMain.GMD_DWORK_NO_EFFECT_ZONE_OBJECT_START, AppMain.GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_END);
        Assert.Equal(AppMain.GMD_DWORK_NO_EFFECT_ZONE_MDL_AMBTEX_START, AppMain.GMD_DWORK_NO_EFFECT_ZONE_OBJECT_END);
        Assert.Equal(AppMain.GMD_DWORK_NO_EFFECT_ZONE_MDL_TEXLIST_START, AppMain.GMD_DWORK_NO_EFFECT_ZONE_MDL_AMBTEX_END);
    }

    // the load loop walks one list entry per model row, so a table with more model rows than the
    // block is wide indexes past the end of both reg id lists
    [Fact]
    public void NoZoneTableHasMoreModelRowsThanTheBlockIsWide()
    {
        int width = AppMain.GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_END - AppMain.GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_START;

        var tables = AppMain.gm_efct_zone_create_info
            .Select(info => info.zone_create_param)
            .Append(AppMain.gm_efct_zone03_pc_create_param_tbl);

        foreach (var table in tables)
            Assert.True(table.Count(row => row.create_param.model_idx != -1) <= width);
    }

    [Fact]
    public void EveryModelRowNamesASlotInTheBlock()
    {
        var model_rows = AppMain.gm_efct_zone03_pc_create_param_tbl
            .Where(row => row.create_param.model_idx != -1);

        Assert.All(model_rows, row =>
        {
            Assert.InRange(row.model_dwork_no, AppMain.GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_START, AppMain.GMD_DWORK_NO_EFFECT_ZONE_MODELDAT_END - 1);
            Assert.NotEqual(-1, row.mdl_ambtex_idx);
        });
    }

    // PC's own table is 54 rows with ame_idx == row. Ours keeps mobile's row order for the first 36
    // so every existing spawn site's index still means what it did, then appends PC's other 18
    [Fact]
    public void ThePcTableIsPcsFiftyFourAnimsInOurRowOrder()
    {
        var pc = AppMain.gm_efct_zone03_pc_create_param_tbl;

        Assert.Equal(54, pc.Length);
        Assert.Equal(pc.Length, AppMain.gm_efct_zone03_pc_create_info.num);
        Assert.Same(pc, AppMain.gm_efct_zone03_pc_create_info.zone_create_param);

        var ame = pc.Select(row => row.create_param.ame_idx).ToArray();
        Assert.Equal(Enumerable.Range(0, 54), ame.OrderBy(i => i));
    }

    [Fact]
    public void TheFirstThirtySixRowsAreTheMobileOnesRemapped()
    {
        var mobile = AppMain.gm_efct_zone03_create_param_tbl;
        var pc = AppMain.gm_efct_zone03_pc_create_param_tbl;

        for (int row = 0; row < mobile.Length; ++row)
        {
            Assert.Equal(AppMain.gm_efct_zone03_pc_ame_idx[row], pc[row].create_param.ame_idx);
            Assert.Equal(mobile[row].create_param.init_flag, pc[row].create_param.init_flag);
            Assert.Equal(mobile[row].create_param.pos_type, pc[row].create_param.pos_type);
            Assert.Equal(mobile[row].create_param.main_func, pc[row].create_param.main_func);
        }
    }

    [Fact]
    public void TheAppendedRowsAreTheEighteenPcOnlyAnims()
    {
        var appended = AppMain.gm_efct_zone03_pc_create_param_tbl
            .Skip(AppMain.gm_efct_zone03_create_param_tbl.Length)
            .Select(row => row.create_param.ame_idx)
            .ToArray();

        Assert.Equal(PcOnlyAnims, appended);
        Assert.Empty(PcOnlyAnims.Intersect(AppMain.gm_efct_zone03_pc_ame_idx));
    }

    [Fact]
    public void EveryAnimSlotFitsTheZoneThreeDworkBlock()
    {
        int block = AppMain.GMD_DWORK_NO_EFFECT_ZONE3_AME_END - AppMain.GMD_DWORK_NO_EFFECT_ZONE3_AME_START + 1;

        Assert.True(AppMain.gm_efct_zone03_pc_create_param_tbl.Length <= block);
        Assert.All(
            AppMain.gm_efct_zone03_pc_create_param_tbl,
            row => Assert.InRange(row.create_param.ame_idx, 0, block - 1));
    }

    // the mobile permutation is what every existing zone 3 spawn resolves through
    [Fact]
    public void TheMobilePermutationIsStrictlyIncreasing()
    {
        var map = AppMain.gm_efct_zone03_pc_ame_idx;

        Assert.Equal(AppMain.gm_efct_zone03_create_param_tbl.Length, map.Length);
        Assert.Equal(map, map.OrderBy(i => i));
        Assert.Equal(map.Length, map.Distinct().Count());
    }

    [Fact]
    public void ThePcLoadoutDropsWhatPcDoesNotLoad()
    {
        var paths = AppMain.gm_gamedat_tbl_gimmick03_02_pc.Select(row => row.path).ToArray();

        Assert.DoesNotContain("G_ZONE3/MSG/G_MSG_Z3.AMB", paths);
        Assert.DoesNotContain(paths, path => path.EndsWith("_TVX.AMB"));
        Assert.DoesNotContain(paths, path => path.Contains("PRESSWALL"));
        Assert.Equal(paths.Length, paths.Distinct().Count());
    }

    [Fact]
    public void ThePcLoadoutCarriesTheFourNewFamilies()
    {
        var slots = AppMain.gm_gamedat_tbl_gimmick03_02_pc.Select(row => row.user_data).ToArray();

        Assert.Contains(AppMain.GMD_DWORK_NO_GMK_TORCH_MDL, slots);
        Assert.Contains(AppMain.GMD_DWORK_NO_GMK_CANDLE_MDL, slots);
        Assert.Contains(AppMain.GMD_DWORK_NO_GMK_RAILEND_MDL, slots);
        Assert.Contains(AppMain.GMD_DWORK_NO_GMK_DYNAMITE_MAT, slots);
        Assert.Equal(30, AppMain.gm_gamedat_tbl_gimmick03_02_pc.Length);
    }

    // GmPlySeqMain indexes the state data table by seq_state every frame, so the dismount throws on
    // its first frame without a row. Read out of PC's table at 0x57F2B0 row 68: MOVE_JUMP, accept 0
    [Fact]
    public void EveryCharacterHasARowForTheTruckGetOffState()
    {
        Assert.True(AppMain.GME_PLY_SEQ_STATE_GMK_TRUCK_GET_OFF < AppMain.GME_PLY_SEQ_STATE_MAX);

        foreach (var states in AppMain.g_gm_ply_seq_state_data_tbl)
        {
            Assert.Equal(AppMain.GME_PLY_SEQ_STATE_MAX, states.Length);

            var get_off = states[AppMain.GME_PLY_SEQ_STATE_GMK_TRUCK_GET_OFF];
            Assert.Equal(AppMain.GMD_PLY_SEQ_STATE_CHECK_ATTR_MOVE_JUMP, get_off.check_attr);
            Assert.Equal(0U, get_off.accept_attr);
        }
    }
}
