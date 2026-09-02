using System.Linq;

namespace Sonic4Episode1.Tests;

// Zone 2 moved to PC as a whole set, because ZONE2_M.AMB is shared by all four acts and the variant
// system resolves one provenance per archive path. So acts 1, 3 and boss now load PC layouts against
// PC block models where they used to load xbla ones, and the question this file answers is whether
// our per-stage tables still cover what those layouts place.
//
// Measured by parsing the .EV and .DC out of both source trees and diffing (the xbla maps are big
// endian, which is why a first pass read them as empty). The answer: act 1 and the boss are
// identical to xbla id for id, and act 3 differs by exactly one addition. Numbers below are that
// measurement, so a future content change that breaks the assumption fails here rather than in game.
public class Zone2CohortTests
{
    private const int Z2A1 = 4;
    private const int Z2A2 = 5;
    private const int Z2A3 = 6;
    private const int Z2BOSS = 7;

    // PC's ZONE23 adds 24 placements of the special stage arrow, which no xbla zone 2 act places.
    // It is the only event id anywhere in the cohort outside act 2 that the move to PC introduced
    private const int SS_ARROW_EVENT_ID = 291;

    // every decoration id the four acts place between them, outside act 2's own 184-199 block
    public static TheoryData<int> SharedDecoIds => new() { 14, 15, 16, 180, 181 };

    [Fact]
    public void TheArrowActThreeAddsHasAMaker()
    {
        Assert.NotNull(AppMain.g_gm_event_tbl[SS_ARROW_EVENT_ID]);
    }

    // a maker with no archive behind it is a null model list at spawn, not a load failure, so this
    // asks the loadout rather than the dispatch table
    [Fact]
    public void ActThreeLoadsTheArrowArchives()
    {
        var paths = ActGimmickPaths(Z2A3);

        Assert.Contains("G_SS/GMK/GMK_SS_ARROW_MDL.AMB", paths);
        Assert.Contains("G_SS/GMK/GMK_SS_ARROW_TEX.AMB", paths);
        Assert.Contains("G_SS/GMK/GMK_SS_ARROW_MAT.AMB", paths);
    }

    // Act 1 and the boss place exactly what xbla placed, so their loadouts were already right and
    // must stay that way. These are the families their layouts actually reference
    [Theory]
    [InlineData(Z2A1, "GMK_LAND_2_MDL")]
    [InlineData(Z2A1, "GMK_BUMPER_MDL")]
    [InlineData(Z2A1, "GMK_BOBBIN_MDL")]
    [InlineData(Z2A1, "GMK_FLIPPER_MDL")]
    [InlineData(Z2A1, "GMK_STOPPER_MDL")]
    [InlineData(Z2A1, "GMK_SLOT_MDL")]
    [InlineData(Z2A1, "GMK_SP_CTPLT_MDL")]
    [InlineData(Z2A3, "GMK_LAND_2_MDL")]
    [InlineData(Z2A3, "GMK_BOBBIN_MDL")]
    [InlineData(Z2A3, "GMK_CANNON_MDL")]
    [InlineData(Z2BOSS, "GMK_SHUTTER_MDL")]
    [InlineData(Z2BOSS, "GMK_CAPSULE_MDL")]
    public void EachActLoadsTheFamiliesItPlaces(int stage_id, string archive)
    {
        Assert.Contains(ActGimmickPaths(stage_id), path => path.Contains(archive));
    }

    // PC's own act 1 and act 3 tables additionally load STOPPER and SLOT for act 3, and B_WALL2 for
    // act 1, that the layouts never place. We differ from PC here in both directions and it is
    // deliberate: an archive nobody places is a wasted load, where a missing one is a null head
    [Fact]
    public void ActThreeSkipsThePinballFamiliesItDoesNotPlace()
    {
        var paths = ActGimmickPaths(Z2A3);

        Assert.DoesNotContain(paths, p => p.Contains("GMK_STOPPER"));
        Assert.DoesNotContain(paths, p => p.Contains("GMK_SLOT"));
    }

    // Decoration ids the whole cohort shares. DECO_ZONE2 is PC's eleven-member archive now, so a
    // model index still pointing where mobile's two-member one had it would draw the wrong thing.
    // Values read from the retail PC binary's g_gm_deco_model_index at 0x57B2A0, two ints per id
    [Theory]
    [InlineData(14, 0)]
    [InlineData(15, 0)]
    [InlineData(16, 0)]
    [InlineData(180, 0)]
    [InlineData(181, 1)]
    public void TheSharedDecoIdsPointAtPcsModels(int id, int member)
    {
        Assert.Equal(member, AppMain.g_gm_deco_model_index[id][0]);
        Assert.Equal(-1, AppMain.g_gm_deco_model_index[id][1]);
    }

    // none of the shared five animates; the dice and card spins are act 2's 184-199 only
    [Theory]
    [MemberData(nameof(SharedDecoIds))]
    public void TheSharedDecoIdsHaveNoMotion(int id)
    {
        Assert.Equal(-1, AppMain.g_gm_deco_node_motion_index[id][0]);
        Assert.Equal(-1, AppMain.g_gm_deco_mat_motion_index[id][0]);
    }

    // Bit 22 is what suppresses the automatic quarter turn about Y in ObjDrawAction3DNN. Without it
    // a decoration drawn from a console-grade model comes out edge on, which is the same failure as
    // the map far rotation. PC sets it on all five, read from g_gm_deco_disp_flag at 0x57B8E0
    [Theory]
    [MemberData(nameof(SharedDecoIds))]
    public void TheSharedDecoIdsSuppressTheQuarterTurn(int id)
    {
        Assert.Equal(AppMain.OBD_DISP_NODIRFLIP, AppMain.g_gm_deco_disp_flag[id] & AppMain.OBD_DISP_NODIRFLIP);
    }

    private static string[] ActGimmickPaths(int stage_id)
    {
        var info = AppMain.gmGameDatGimmickInfoFor(stage_id);

        return Enumerable.Range(0, info.num)
            .Select(i => info.data_tbl[i].path)
            .ToArray();
    }
}
