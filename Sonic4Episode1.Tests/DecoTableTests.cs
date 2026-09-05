using Microsoft.Xna.Framework;
using Sonic4Episode1.Viewer;

namespace Sonic4Episode1.Tests;

public class DecoTableTests
{
    private const int Sunflower = 17;
    private const int WallA = 18;
    private const int WoodA = 26;

    // DECO_ZONE1_MDL.AMB member order, which is the same on both builds:
    // HIMAWARI, WALLA, WALLB, ASIHANAA/B/C, HANAA, HANAB, WOODA, WALLAA/AB/AC/AD, WALLBA
    [Theory]
    [InlineData(17, 0)]  // sunflower
    [InlineData(18, 1)]  // wall A
    [InlineData(19, 2)]  // wall B
    [InlineData(21, 3)]  // reed A
    [InlineData(22, 4)]
    [InlineData(23, 5)]
    [InlineData(24, 6)]  // flower A
    [InlineData(25, 7)]
    [InlineData(26, 8)]  // log
    [InlineData(28, 9)]  // wall AA
    [InlineData(29, 10)]
    [InlineData(30, 11)]
    [InlineData(31, 12)]
    [InlineData(32, 13)]
    public void TheModelIndexTableMatchesTheConsoleMemberOrder(int id, int member)
    {
        Assert.Equal(member, AppMain.g_gm_deco_model_index[id][0]);
    }

    // DECO_ZONE1_MTN.AMB has exactly two node motions, and exactly two zone 1 ids want one
    [Theory]
    [InlineData(17, 0)]  // DECO_Z1_HIMAWARI.ENM
    [InlineData(26, 1)]  // DECO_Z1_WOODA.ENM
    [InlineData(111, 0)] // the ending stage's twins
    [InlineData(114, 1)]
    public void TheNodeMotionTableMatchesTheConsoleMtnMemberOrder(int id, int member)
    {
        Assert.Equal(member, AppMain.g_gm_deco_node_motion_index[id][0]);
    }

    // every other zone 1 decoration has no node motion of its own
    [Theory]
    [InlineData(18)]
    [InlineData(19)]
    [InlineData(24)]
    [InlineData(25)]
    [InlineData(29)]
    [InlineData(30)]
    public void ZoneOneDecorationsWithoutMotionSayMinusOne(int id)
    {
        Assert.Equal(-1, AppMain.g_gm_deco_node_motion_index[id][0]);
    }

    // zone 1 sets no OBD_DISP_NODRAWSCALE override, so ObjDrawAction3DNN draws every decoration at the global GMD_OBJ_DRAW_SCALE
    [Theory]
    [InlineData(17)]
    [InlineData(18)]
    [InlineData(19)]
    [InlineData(20)]
    [InlineData(24)]
    [InlineData(25)]
    [InlineData(26)]
    [InlineData(29)]
    [InlineData(30)]
    [InlineData(40)]
    public void ZoneOneDecorationsDrawAtTheGlobalDrawScale(int id)
    {
        Assert.Equal(new Vector3(AppMain.GMD_OBJ_DRAW_SCALE), StageDeco.Scale(id));
    }

    [Fact]
    public void TheDispatchTableDecidesWhatADecorationDrawsWith()
    {
        // glare is a primitive with no model of its own, EFFECT_TAKI spawns an effect
        Assert.Equal(StageDecoKind.None, StageDeco.KindOf(14));
        Assert.Equal(StageDecoKind.None, StageDeco.KindOf(16));
        Assert.Equal(StageDecoKind.None, StageDeco.KindOf(99));

        Assert.Equal(StageDecoKind.Model, StageDeco.KindOf(17));
        Assert.Equal(StageDecoKind.Model, StageDeco.KindOf(18));
        Assert.Equal(StageDecoKind.Model, StageDeco.KindOf(26));

        Assert.Equal(StageDecoKind.Fall, StageDeco.KindOf(20));
        Assert.Equal(StageDecoKind.Fall, StageDeco.KindOf(45));
    }

    // g_gm_deco_rot_z is 184 zeroes here and in SEGA's source, so the viewer does not rotate at
    // all. If this ever fires, the decoration matrix needs a rotation putting back.
    [Fact]
    public void NothingRotates()
    {
        Assert.All(AppMain.g_gm_deco_rot_z, value => Assert.Equal(0, value));
    }

    // only five ids in the whole table offset off their .dc position, and none is in zone 1
    [Fact]
    public void OnlyAHandfulOfDecorationsSitOffTheirPlacedPosition()
    {
        var offset = new System.Collections.Generic.List<int>();

        for (int id = 0; id < AppMain.g_gm_deco_pos.Length; id++)
        {
            if (AppMain.g_gm_deco_pos[id][0] != 0 || AppMain.g_gm_deco_pos[id][1] != 0)
                offset.Add(id);
        }

        Assert.Equal(new[] { 68, 69, 100, 142, 182 }, offset);
    }
}
