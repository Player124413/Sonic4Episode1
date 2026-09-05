using System;
using System.IO;
using System.Linq;
using Sonic4Episode1.Viewer;

namespace Sonic4Episode1.Tests;

// A stage is one index into gm_gamedat_tbl_map_info_tbl and nothing else. These pin that the viewer
// reads its archive names out of that table rather than formatting zone and act together, and that
// the loader reproduces gmGameDatLoadProcPostMap's slot assignment.
public class StageLoadTests
{
    // in gamedat order, which is what g_gs_main_sys_info.stage_id holds
    private static readonly string[] StageNames =
    {
        "ZONE11", "ZONE12", "ZONE13", "ZONE1BOSS",
        "ZONE21", "ZONE22", "ZONE23", "ZONE2BOSS",
        "ZONE31", "ZONE32", "ZONE33", "ZONE3BOSS",
        "ZONE41", "ZONE42", "ZONE43", "ZONE4BOSS",
        "ZONEFB", "ZONEFB", "ZONEFB", null, "ZONEFB",
        "SS1", "SS2", "SS3", "SS4", "SS5", "SS6", "SS7",
        "ZONE1E",
    };

    [Fact]
    public void EveryStageIsNamedAfterTheArchiveItLoads()
    {
        Assert.Equal(StageNames.Length, StageTable.Count);

        for (int i = 0; i < StageTable.Count; i++)
            Assert.Equal(StageNames[i], StageTable.Name(i));
    }

    // stage 19 is gm_gamedat_tbl_map_final_BOSS04, a single /bb/gmstage.bb entry
    [Fact]
    public void OnlyTheBossBattlePlaceholderHasNoBlockMap()
    {
        for (int i = 0; i < StageTable.Count; i++)
            Assert.Equal(i != 19, StageTable.HasBlockMap(i));

        Assert.Single(StageTable.MapPaths(19));
    }

    [Fact]
    public void EveryStageWithABlockMapLoadsFourArchivesInDataNoOrder()
    {
        for (int i = 0; i < StageTable.Count; i++)
        {
            if (!StageTable.HasBlockMap(i))
                continue;

            var paths = StageTable.MapPaths(i);
            Assert.EndsWith("_MAP.AMB", paths[0]);
            Assert.EndsWith("_M.AMB", paths[1]);
            Assert.EndsWith("_T.AMB", paths[2]);
            Assert.EndsWith("_ATTR.AMB", paths[3]);
        }
    }

    // dmStgSlctSetNextEvt adds 4 to reach a special stage, so the stage select's id space is the
    // gamedat one with the seven SS entries shifted past the five final boss phases
    [Theory]
    [InlineData("DME_STGSLCT_STAGE_1_1", 0)]
    [InlineData("DME_STGSLCT_STAGE_4_B", 15)]
    [InlineData("DME_STGSLCT_STAGE_F_1", 16)]
    public void ActIdsAreTheGamedatIdsUnchanged(string act, int stageId)
    {
        Assert.Equal(stageId, ActId(act));
        Assert.True(StageTable.HasBlockMap(stageId));
    }

    [Theory]
    [InlineData("DME_STGSLCT_STAGE_S_1", 21, "SS1")]
    [InlineData("DME_STGSLCT_STAGE_S_7", 27, "SS7")]
    public void SpecialStageIdsAreTheActIdPlusFour(string act, int stageId, string name)
    {
        Assert.Equal(stageId, ActId(act) + 4);
        Assert.Equal(name, StageTable.Name(stageId));
    }

    private static int ActId(string name)
    {
        return (int)Enum.Parse(typeof(StgSlct.DME_STGSLCT_STAGE), name);
    }

    [Fact]
    public void ZoneTypeCoversEveryStage()
    {
        Assert.Equal(StageTable.Count, AppMain.g_gm_gamedat_zone_type_tbl.Length);

        for (int i = 0; i < StageTable.Count; i++)
            Assert.InRange(StageTable.AddLayerCount(i), 1, 5);
    }

    [Fact]
    public void LoadsTheStagedZone1Act1()
    {
        var map = Load(0);

        Assert.Equal("ZONE11", map.Name);
        Assert.Equal(StageModelFormat.Tvx, map.ModelFormat);
        Assert.Equal(214, map.ModelCount);
        Assert.Equal(588u, map.At.block_num);
        Assert.Equal(588u, map.Df.block_num);
        Assert.Equal(588u, map.Di.block_num);

        // 15 members is three (.MP, .MD) pairs past the primary nine, so five layers in all
        Assert.Equal(5, map.Layers.Count);
        Assert.Equal(new[] { "A", "B", "N", "M", "MIPHONE" }, map.Layers.Select(l => l.Name));
        Assert.Equal(new[] { 128f, -128f, 256f, -256f, -384f }, map.Layers.Select(l => l.PosZ));

        foreach (var layer in map.Layers.Take(2))
        {
            Assert.Equal(280, layer.Width);
            Assert.Equal(32, layer.Height);
        }

        Assert.Equal(280, map.AttrA.map_w);
        Assert.Equal(280, map.AttrB.map_w);
    }

    [Fact]
    public void EveryStagedStageLoadsWithMatchingGrids()
    {
        var root = StagedRoot();

        for (int i = 0; i < StageTable.Count; i++)
        {
            if (!StageTable.HasBlockMap(i))
                continue;

            var map = new StageMap(root, i);
            map.Load();

            Assert.True(map.Layers[0].Present && map.Layers[1].Present, $"{map.Name} is missing A or B");

            foreach (var layer in map.Layers)
            {
                if (!layer.Present)
                {
                    Assert.True(layer.Mp == null && layer.Md == null, $"{map.Name} {layer.Name} is half a layer");
                    continue;
                }

                Assert.Equal(layer.Mp.map_w, layer.Md.map_w);
                Assert.Equal(layer.Mp.map_h, layer.Md.map_h);
            }
        }
    }

    // the mobile TVX arm indexes tvxamb[id - 1], so an id may reach file_num but never pass it.
    // console indexes by id straight and keeps member 0 blank, which is the same ids one along
    [Fact]
    public void NoStagedBlockIdOutrunsItsModelArchive()
    {
        var root = StagedRoot();

        for (int i = 0; i < StageTable.Count; i++)
        {
            if (!StageTable.HasBlockMap(i))
                continue;

            var map = new StageMap(root, i);
            map.Load();

            foreach (var layer in map.Layers)
            {
                if (!layer.Present)
                    continue;

                foreach (var block in layer.Mp.blocks)
                    Assert.True(block.id <= map.ModelCount,
                        $"{map.Name} {layer.Name} references block {block.id} of {map.ModelCount}");
            }
        }
    }

    // user0 tags a block model as a waterfall or as taking the extra light; it is zero throughout
    // every mobile archive, which is why this port has never drawn a waterfall
    [Fact]
    public void NoStagedBlockModelIsTagged()
    {
        var root = StagedRoot();

        for (int i = 0; i < StageTable.Count; i++)
        {
            if (!StageTable.HasBlockMap(i))
                continue;

            var map = new StageMap(root, i);
            map.Load();

            Assert.All(map.ModelArchive.user0, tag => Assert.Equal(0, tag));
            Assert.All(map.ModelArchive.user1, tag => Assert.Equal(0, tag));
        }
    }

    private static StageMap Load(int stageId)
    {
        var map = new StageMap(StagedRoot(), stageId);
        map.Load();
        return map;
    }

    private static string StagedRoot()
    {
        var here = Path.GetDirectoryName(typeof(StageLoadTests).Assembly.Location);
        var root = Path.GetFullPath(Path.Combine(here, "..", "..", "..", "..", "Content", "bin", "DesktopGL", "Content"));

        if (!Directory.Exists(root))
            throw Xunit.Sdk.SkipException.ForSkip("Content/bin/DesktopGL is not present");

        return root;
    }
}
