using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sonic4Episode1.Viewer;

namespace Sonic4Episode1.Tests;

// The background is a nested #AMB whose member order differs per zone, which is why gmMapFarBuildData
// has six builders. These pin the table that dispatch implies, and that the viewer reads the archive
// name out of gm_gamedat_tbl_map_info_tbl rather than formatting it.
public class StageMapFarTests
{
    private static byte[] Chunk(uint header, bool bigEndian, int length = 64)
    {
        var bytes = new byte[length];

        if (bigEndian)
            BinaryPrimitives.WriteUInt32BigEndian(bytes, header);
        else
            BinaryPrimitives.WriteUInt32LittleEndian(bytes, header);

        return bytes;
    }

    private static byte[] Nested(bool bigEndian, params string[] names)
    {
        uint header = bigEndian ? AppMain.NND_CHUNK_HEADER_XE : AppMain.NND_CHUNK_HEADER_GLES11;
        var entries = names
            .Select(name => new AppMain.AmbEntry(name, Chunk(header, bigEndian)))
            .ToList();

        return AppMain.writeAmbArchive(entries, bigEndian);
    }

    private static AppMain.AMS_AMB_HEADER Outer(bool bigEndian, params (string Name, byte[] Data)[] members)
    {
        var entries = members.Select(m => new AppMain.AmbEntry(m.Name, m.Data)).ToList();
        var bytes = AppMain.writeAmbArchive(entries, bigEndian);

        var header = new AppMain.AMS_AMB_HEADER { data = bytes };
        using var stream = new MemoryStream(bytes);
        AppMain.readAmbHeader(stream, ref header);

        return header;
    }

    [Fact]
    public void EveryStageWithABlockMapNamesOneMapFarArchive()
    {
        for (int i = 0; i < StageTable.Count; i++)
        {
            var path = StageTable.MapFarPath(i);

            if (!StageTable.HasBlockMap(i))
            {
                Assert.Null(path);
                continue;
            }

            Assert.NotNull(path);
            Assert.EndsWith("_MAPFAR.AMB", path);
        }
    }

    // gmMapFarZone4Build is empty on both platforms and GmCamera never makes a far camera for zone
    // type 3, so Mad Gear borrows zone 1's archive and reads nothing out of it
    [Fact]
    public void ZoneFourBorrowsZoneOnesBackgroundAndNeverReadsIt()
    {
        var borrowed = Enumerable.Range(0, StageTable.Count)
            .Where(i => StageTable.HasBlockMap(i) && StageTable.ZoneType(i) == 3)
            .ToList();

        Assert.NotEmpty(borrowed);

        foreach (int stage in borrowed)
            Assert.EndsWith("G_ZONE1/MAPFAR/ZONE1_MAPFAR.AMB", StageTable.MapFarPath(stage));

        Assert.False(StageMapFarLayout.For(3).Drawn);
    }

    // measured across mobile, xbla and pc, and the same in all three
    [Fact]
    public void TheMemberOrderIsPerZoneType()
    {
        var zone1 = StageMapFarLayout.For(0);
        Assert.Equal((0, 1, 2, -1, 3),
            (zone1.Models, zone1.Textures, zone1.MaterialMotions, zone1.Motions, zone1.RenderModels));

        var zone2 = StageMapFarLayout.For(1);
        Assert.Equal((0, 3, 2, 1, -1),
            (zone2.Models, zone2.Textures, zone2.MaterialMotions, zone2.Motions, zone2.RenderModels));

        foreach (int zoneType in new[] { 2, 4, 5 })
        {
            var layout = StageMapFarLayout.For(zoneType);
            Assert.Equal((0, 1, 2, -1, -1),
                (layout.Models, layout.Textures, layout.MaterialMotions, layout.Motions, layout.RenderModels));
        }
    }

    [Fact]
    public void ZoneOneLoadsModelsTexturesMaterialMotionsAndARenderModel()
    {
        var archive = Outer(false,
            ("MAPFAR_ZONE1_MDL.AMB", Nested(false, "Z1_SKYL_T_D.INO", "Z1_ROCK_A_T_D.INO")),
            ("MAPFAR_TEX.AMB", Nested(false, "SKY_IPHONE.PVR")),
            ("MAPFAR_ZONE1_MAT.AMB", Nested(false, "Z1_SKYL_T_D.INV")),
            ("MAPFAR_ZONE1_RENDER_MDL.AMB", Nested(false, "Z1_SEA_D.INO")));

        var far = new StageMapFar("G_ZONE1/MAPFAR/ZONE1_MAPFAR.AMB", 0);
        far.Load(archive);

        Assert.Equal(2, far.ModelCount);
        Assert.Equal(1, far.Textures.file_num);
        Assert.Equal(1, far.MaterialMotions.file_num);
        Assert.Equal(1, far.RenderModels.file_num);
        Assert.Null(far.Motions);
    }

    [Fact]
    public void ZoneTwoTakesItsTexturesFromMemberThreeAndHasMotions()
    {
        var archive = Outer(true,
            ("MAPFAR_ZONE2_MDL.AMB", Nested(true, "BG_02_SKY.ENO", "BG_02_GROUND.ENO", "BG_02_WHEEL.ENO")),
            ("MAPFAR_ZONE2_MTN.AMB", Nested(true, "BG_02_WHEEL.ENM", "BG_02_SLIGHT.ENM")),
            ("MAPFAR_ZONE2_MAT.AMB", Nested(true, "BG_02_GROUND.ENV")),
            ("MAPFAR_TEX.AMB", Nested(true, "Z2_SKY.DDS")));

        var far = new StageMapFar("G_ZONE2/MAPFAR/ZONE2_MAPFAR.AMB", 1);
        far.Load(archive);

        Assert.Equal(3, far.ModelCount);
        Assert.Equal(2, far.Motions.file_num);
        Assert.Equal(1, far.MaterialMotions.file_num);
        Assert.Equal(1, far.Textures.file_num);
        Assert.Null(far.RenderModels);
    }

    [Fact]
    public void TheArmComesFromTheModelChunkHeader()
    {
        var mobile = new StageMapFar("x", 2);
        mobile.Load(Outer(false,
            ("MAPFAR_ZONE3_MDL.AMB", Nested(false, "Z3_FAR.INO")),
            ("MAPFAR_TEX.AMB", Nested(false, "T.PVR")),
            ("MAPFAR_ZONE3_MAT.AMB", Nested(false, "M.INV"))));

        Assert.Equal(StageMapFarArm.Mobile, mobile.Arm);

        var console = new StageMapFar("x", 2);
        console.Load(Outer(true,
            ("MAPFAR_ZONE3_MDL.AMB", Nested(true, "Z3_FAR.ENO")),
            ("MAPFAR_TEX.AMB", Nested(true, "T.DDS")),
            ("MAPFAR_ZONE3_MAT.AMB", Nested(true, "M.ENV"))));

        Assert.Equal(StageMapFarArm.Console, console.Arm);
    }

    [Fact]
    public void AZoneFourArchiveIsLoadedButNotOpened()
    {
        var archive = Outer(false,
            ("MAPFAR_ZONE1_MDL.AMB", Nested(false, "Z1_SKYL_T_D.INO")),
            ("MAPFAR_TEX.AMB", Nested(false, "SKY_IPHONE.PVR")),
            ("MAPFAR_ZONE1_MAT.AMB", Nested(false, "Z1_SKYL_T_D.INV")),
            ("MAPFAR_ZONE1_RENDER_MDL.AMB", Nested(false, "Z1_SEA_D.INO")));

        var far = new StageMapFar("G_ZONE1/MAPFAR/ZONE1_MAPFAR.AMB", 3);
        far.Load(archive);

        Assert.NotNull(far.Archive);
        Assert.Null(far.Models);
        Assert.Equal(0, far.ModelCount);
        Assert.Equal(StageMapFarArm.Unknown, far.Arm);
    }
}
