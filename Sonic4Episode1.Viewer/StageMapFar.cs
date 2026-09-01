using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sonic4Episode1.Viewer;

// Which outer members a zone's *_MAPFAR.AMB holds and where. gmMapFarBuildData dispatches on zone
// type into six builders precisely because the order differs between zones, so this is the table
// that dispatch implies. -1 is a member the zone does not ship.
internal readonly struct StageMapFarLayout
{
    public int Models { get; }
    public int Textures { get; }
    public int MaterialMotions { get; }
    public int Motions { get; }
    public int RenderModels { get; }

    private StageMapFarLayout(int models, int textures, int materialMotions, int motions, int renderModels)
    {
        Models = models;
        Textures = textures;
        MaterialMotions = materialMotions;
        Motions = motions;
        RenderModels = renderModels;
    }

    // zone type 3 loads ZONE1_MAPFAR.AMB and reads nothing out of it: gmMapFarZone4Build is empty
    // on both platforms and GmCamera never makes it a far camera
    public bool Drawn => Models >= 0;

    public static StageMapFarLayout For(int zoneType) => zoneType switch
    {
        0 => new StageMapFarLayout(0, 1, 2, -1, 3),
        1 => new StageMapFarLayout(0, 3, 2, 1, -1),
        2 or 4 or 5 => new StageMapFarLayout(0, 1, 2, -1, -1),
        _ => new StageMapFarLayout(-1, -1, -1, -1, -1),
    };
}

// Mobile ships .INO, XBLA .ENO, PC .ZNO. The console arms carry whole models mobile dropped, so
// this picks which object table a background is read against. See docs/map-far.md.
internal enum StageMapFarArm
{
    Unknown,
    Mobile,
    Console,
}

// One stage's background, loaded the way GmMapFarBuildData does. Parse only, like StageMap: no
// graphics device, no camera. The outer archive is a nested #AMB, so every member is itself one.
internal sealed class StageMapFar
{
    public string ArchivePath { get; }
    public int ZoneType { get; }
    public StageMapFarLayout Layout { get; }

    public AppMain.AMS_AMB_HEADER Archive { get; private set; }
    public AppMain.AMS_AMB_HEADER Models { get; private set; }
    public AppMain.AMS_AMB_HEADER Textures { get; private set; }
    public AppMain.AMS_AMB_HEADER MaterialMotions { get; private set; }
    public AppMain.AMS_AMB_HEADER Motions { get; private set; }
    public AppMain.AMS_AMB_HEADER RenderModels { get; private set; }

    public StageMapFarArm Arm { get; private set; }

    public int ModelCount => Models?.file_num ?? 0;

    public StageMapFar(string archivePath, int zoneType)
    {
        ArchivePath = archivePath;
        ZoneType = zoneType;
        Layout = StageMapFarLayout.For(zoneType);
    }

    public void Load(AppMain.AMS_AMB_HEADER archive)
    {
        Archive = archive;

        if (!Layout.Drawn)
            return;

        Models = Inner(Layout.Models);
        Textures = Inner(Layout.Textures);
        MaterialMotions = Inner(Layout.MaterialMotions);
        Motions = Inner(Layout.Motions);
        RenderModels = Inner(Layout.RenderModels);
        Arm = DetectArm(Models);
    }

    private AppMain.AMS_AMB_HEADER Inner(int member)
    {
        if (member < 0 || member >= Archive.file_num || Archive.lengths[member] == 0)
            return null;

        return AppMain.readAMBFile(AppMain.amBindGet(Archive, member, out _));
    }

    private static StageMapFarArm DetectArm(AppMain.AMS_AMB_HEADER models)
    {
        if (models == null)
            return StageMapFarArm.Unknown;

        for (int i = 0; i < models.file_num; i++)
        {
            var chunk = AppMain.amBindGet(models, i, out _);
            if (chunk?.array == null || chunk.length < 4)
                continue;

            uint id = BinaryPrimitives.ReadUInt32LittleEndian(chunk.array.AsSpan(chunk.offset));

            if (id == AppMain.NND_CHUNK_HEADER_GLES11)
                return StageMapFarArm.Mobile;

            if (id == AppMain.NND_CHUNK_HEADER_DXG20
                || BinaryPrimitives.ReverseEndianness(id) == AppMain.NND_CHUNK_HEADER_XE)
                return StageMapFarArm.Console;
        }

        return StageMapFarArm.Unknown;
    }

    public string Summary()
    {
        var text = new StringBuilder();

        text.AppendLine($"  {ArchivePath,-34} {Archive.file_num,4} member(s)"
            + (Archive.big_endian ? ", big endian" : ""));

        if (!Layout.Drawn)
        {
            text.AppendLine("    zone type 3 loads it and never reads it");
            return text.ToString();
        }

        text.AppendLine($"    {Arm} arm");
        Describe(text, "models", Models);
        Describe(text, "textures", Textures);
        Describe(text, "mat motions", MaterialMotions);
        Describe(text, "motions", Motions);
        Describe(text, "render models", RenderModels);

        return text.ToString();
    }

    private static void Describe(StringBuilder text, string label, AppMain.AMS_AMB_HEADER amb)
    {
        if (amb == null)
            return;

        text.AppendLine($"    {label,-14} {amb.file_num,3}: {string.Join(", ", MemberNames(amb))}");
    }

    private static IEnumerable<string> MemberNames(AppMain.AMS_AMB_HEADER amb)
    {
        for (int i = 0; i < amb.file_num; i++)
        {
            var name = amb.files?[i];
            yield return string.IsNullOrEmpty(name)
                ? "?"
                : Path.GetFileName(name.Replace('\\', '/'));
        }
    }
}
