using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sonic4Episode1.Viewer;

internal sealed class StageLayer
{
    public string Name;
    public float PosZ;
    public AppMain.MP_HEADER Mp;
    public AppMain.MD_HEADER Md;

    // ZONE4BOSS has no _N layer and pads members 9 and 10 to nothing rather than shifting the rest
    // along, so a slot can be empty. gmMapInit only raises a layer's draw flag when both are there.
    public bool Present => Mp != null && Md != null;

    public int Width => Mp.map_w;
    public int Height => Mp.map_h;

    // gmMapInit: map_size is the grid in world units, and scrl_scale is how far this layer travels
    // for a unit of main camera travel. Both are meaningless on a layer drawn by the main camera.
    public Vector2 MapSize;
    public Vector2 ScrollScale;

    // gm_map_addmap_camera_tbl[1] is camera 0, so A, B and the M layer all draw with the main one
    public bool Parallax;
}

// Which model format ZONEx_M.AMB holds. Mobile ships baked TVX geometry indexed id-1; console ships
// one NN object per block indexed by id, with member 0 blank.
internal enum StageModelFormat
{
    Unknown,
    Tvx,
    NnObject,
}

// Loads one stage the way gmGameDatLoadProcPostMap does, out of a plain directory rather than
// through amFs, so mobile, XBLA and PC trees can be opened side by side. Parse only: no graphics
// device, no window, no camera. StageScene builds on top of this.
internal sealed class StageMap
{
    public string Root { get; }
    public int StageId { get; }
    public string Name { get; private set; }
    public int ZoneType { get; private set; }

    public AppMain.AMS_AMB_HEADER MapArchive { get; private set; }
    public AppMain.AMS_AMB_HEADER ModelArchive { get; private set; }
    public AppMain.AMS_AMB_HEADER TextureArchive { get; private set; }
    public AppMain.AMS_AMB_HEADER AttrArchive { get; private set; }

    // A and B first, then the parallax layers in the order gmMapInit walks them
    public List<StageLayer> Layers { get; } = new List<StageLayer>();

    // the two ATTR_A/ATTR_B grids, members 4 and 5, which carry per-cell collision block ids
    public AppMain.MP_HEADER AttrA { get; private set; }
    public AppMain.MP_HEADER AttrB { get; private set; }

    // what every parallax layer's scroll is measured against
    public Vector2 MainMapSize { get; private set; }

    public AppMain.AT_HEADER At { get; private set; }
    public AppMain.DF_HEADER Df { get; private set; }
    public AppMain.DI_HEADER Di { get; private set; }

    // members 6, 7 and 8: events, decorations and rings, all on the 256 unit grid
    public AppMain.EV_HEADER Ev { get; private set; }
    public AppMain.DC_HEADER Dc { get; private set; }
    public AppMain.RG_HEADER Rg { get; private set; }

    public StageModelFormat ModelFormat { get; private set; }
    public int ModelCount => ModelArchive?.file_num ?? 0;

    // the background. A separate subsystem with its own camera and draw buckets, not a layer.
    public StageMapFar MapFar { get; private set; }

    // the decorations .dc places. Null when the stage loads none, which is every special stage.
    public StageDeco Deco { get; private set; }

    public StageMap(string root, int stageId)
    {
        Root = root;
        StageId = stageId;
    }

    public void Load()
    {
        if (!StageTable.HasBlockMap(StageId))
            throw new InvalidOperationException($"stage {StageId} has no block map");

        Name = StageTable.Name(StageId);
        ZoneType = StageTable.ZoneType(StageId);

        var paths = StageTable.MapPaths(StageId);
        MapArchive = ReadArchive(paths[0]);
        ModelArchive = ReadArchive(paths[1]);
        TextureArchive = ReadArchive(paths[2]);
        AttrArchive = ReadArchive(paths[3]);

        LoadLayers();
        LoadAttr();
        LoadMapFar();
        LoadDeco();
        ModelFormat = DetectModelFormat(ModelArchive);
    }

    private void LoadDeco()
    {
        var path = StageTable.DecoPath(StageId);
        if (path == null)
            return;

        Deco = new StageDeco(path);
        Deco.Load(Root);
    }

    private void LoadMapFar()
    {
        var path = StageTable.MapFarPath(StageId);
        if (path == null)
            return;

        MapFar = new StageMapFar(path, ZoneType);
        MapFar.Load(ReadArchive(path));
    }

    // mirrors gmGameDatLoadProcPostMap case 0: members 0..8 are the primary set, then up to five
    // (.MP, .MD) pairs from index 9 on, each only present if the archive is long enough for both
    private void LoadLayers()
    {
        Layers.Add(new StageLayer
        {
            Name = LayerName(0, "A"),
            PosZ = AppMain.GMD_MAP_A_POS_Z,
            Mp = AppMain.readMPFile(AppMain.amBindGet(MapArchive, 0, out _)),
            Md = AppMain.readMDFile(AppMain.amBindGet(MapArchive, 2, out _)),
        });

        Layers.Add(new StageLayer
        {
            Name = LayerName(1, "B"),
            PosZ = AppMain.GMD_MAP_B_POS_Z,
            Mp = AppMain.readMPFile(AppMain.amBindGet(MapArchive, 1, out _)),
            Md = AppMain.readMDFile(AppMain.amBindGet(MapArchive, 3, out _)),
        });

        AttrA = AppMain.readMPFile(AppMain.amBindGet(MapArchive, 4, out _));
        AttrB = AppMain.readMPFile(AppMain.amBindGet(MapArchive, 5, out _));

        Ev = AppMain.readEVFile(AppMain.amBindGet(MapArchive, 6, out _));
        Dc = AppMain.readDCFile(AppMain.amBindGet(MapArchive, 7, out _));
        Rg = AppMain.readRGFile(AppMain.amBindGet(MapArchive, 8, out _));

        for (int i = 0; i < 5; i++)
        {
            int member = 9 + i * 2;
            if (MapArchive.file_num < member + 2)
                break;

            Layers.Add(new StageLayer
            {
                Name = LayerName(member, "add" + i),
                PosZ = PosZFor(i),
                Parallax = AppMain.gm_map_addmap_camera_tbl[i] != 0,
                Mp = AppMain.readMPFile(AppMain.amBindGet(MapArchive, member, out _)),
                Md = AppMain.readMDFile(AppMain.amBindGet(MapArchive, member + 1, out _)),
            });
        }

        // GmMapBuildColData takes the main map size off ATTR_A, not off layer A, though for every
        // stage in the corpus the two are the same grid
        MainMapSize = new Vector2(AttrA.map_w * StagePlacement.BlockSize, AttrA.map_h * StagePlacement.BlockSize);

        foreach (var layer in Layers)
        {
            if (!layer.Present)
                continue;

            layer.MapSize = new Vector2(
                layer.Width * StagePlacement.BlockSize, layer.Height * StagePlacement.BlockSize);
            layer.ScrollScale = StageParallax.ScrollScale(layer.MapSize, MainMapSize);
        }
    }

    // gmMapInit seeds every parallax layer from gm_map_addmap_pos_z_tbl and then overrides zone 3
    // act 2 and the final boss by hand
    private float PosZFor(int add)
    {
        float z = AppMain.gm_map_addmap_pos_z_tbl[add];

        if (StageId == 9)
        {
            if (add == 2) return 160f;
            if (add == 3) return -96f;
        }
        else if (StageId == 16 && add >= 1 && add <= 3)
        {
            return z - 64f;
        }

        return z;
    }

    private void LoadAttr()
    {
        At = AppMain.readATFile(AppMain.amBindGet(AttrArchive, 0, out _));
        Df = AppMain.readDFFile(AppMain.amBindGet(AttrArchive, 1, out _));
        Di = AppMain.readDIFile(AppMain.amBindGet(AttrArchive, 2, out _));
    }

    private string LayerName(int member, string fallback)
    {
        var name = MapArchive.files?[member];
        if (string.IsNullOrEmpty(name) || MapArchive.lengths[member] == 0)
            return fallback;

        var file = Path.GetFileNameWithoutExtension(name.Replace('\\', '/'));
        return file.StartsWith(Name + "_", StringComparison.OrdinalIgnoreCase)
            ? file.Substring(Name.Length + 1)
            : file;
    }

    // member 0 is blank on console, so member 1 is the first thing worth looking at either way
    private static StageModelFormat DetectModelFormat(AppMain.AMS_AMB_HEADER models)
    {
        for (int i = 0; i < models.file_num; i++)
        {
            var chunk = AppMain.amBindGet(models, i, out _);
            if (chunk == null || chunk.length < 4)
                continue;

            uint id = (uint)(chunk.array[chunk.offset]
                | chunk.array[chunk.offset + 1] << 8
                | chunk.array[chunk.offset + 2] << 16
                | chunk.array[chunk.offset + 3] << 24);

            if (id == AppMain.NND_CHUNK_HEADER_GLES11
                || id == AppMain.NND_CHUNK_HEADER_DXG20
                || System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(id) == AppMain.NND_CHUNK_HEADER_XE)
                return StageModelFormat.NnObject;

            return StageModelFormat.Tvx;
        }

        return StageModelFormat.Unknown;
    }

    private static int Count<T>(T[][] cells)
    {
        int total = 0;
        foreach (var cell in cells)
            total += cell.Length;
        return total;
    }

    private AppMain.AMS_AMB_HEADER ReadArchive(string relative)
    {
        var path = Path.Combine(Root, relative.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(path))
            throw new FileNotFoundException($"{relative} is not in {Root}", path);

        var bytes = File.ReadAllBytes(path);
        var header = new AppMain.AMS_AMB_HEADER { data = bytes };

        using var stream = new MemoryStream(bytes);
        AppMain.readAmbHeader(stream, ref header);

        return header;
    }

    public string Summary()
    {
        var text = new StringBuilder();
        var paths = StageTable.MapPaths(StageId);

        text.AppendLine($"stage {StageId} {Name}, zone type {ZoneType}, root {Root}");
        for (int i = 0; i < paths.Length; i++)
        {
            var amb = i == 0 ? MapArchive : i == 1 ? ModelArchive : i == 2 ? TextureArchive : AttrArchive;
            text.AppendLine($"  {paths[i],-34} {amb.file_num,4} member(s)"
                + (amb.big_endian ? ", big endian" : ""));
        }

        text.AppendLine($"  models: {ModelFormat}, {ModelCount} member(s)");
        text.AppendLine($"  collision: {At.block_num} at, {Df.block_num} df, {Di.block_num} di");
        text.AppendLine($"  placed on a {Ev.map_w} x {Ev.map_h} grid of {AppMain.GMD_MAP_EVENT_BLOCK_SIZE}:"
            + $" {Count(Ev.cells)} event(s), {Count(Dc.cells)} decoration(s), {Count(Rg.cells)} ring(s)");
        text.AppendLine($"  {Layers.Count - 2} parallax layer(s) present, zone type {ZoneType}"
            + $" draws {StageTable.AddLayerCount(StageId)}:");

        for (int i = 0; i < Layers.Count; i++)
        {
            var layer = Layers[i];
            if (!layer.Present)
            {
                text.AppendLine($"    {layer.Name,-8} empty");
                continue;
            }

            var drawn = i < 2 || i - 2 < StageTable.AddLayerCount(StageId) ? "" : "  (not drawn)";
            var scroll = layer.Parallax
                ? $" scroll {layer.ScrollScale.X,6:0.000} x {layer.ScrollScale.Y,6:0.000}"
                : " main camera                ";

            text.AppendLine($"    {layer.Name,-8} {layer.Width,4} x {layer.Height,-4}"
                + $" z {layer.PosZ,7}{scroll}{drawn}");
        }

        text.AppendLine("  background:");
        text.Append(MapFar == null ? "    none\n" : MapFar.Summary());

        text.AppendLine("  decorations:");
        if (Deco == null)
        {
            text.AppendLine("    none");
        }
        else
        {
            text.Append(Deco.Summary());
            text.Append(Deco.PlacementSummary(StageMarkers.Build(this)));
        }

        return text.ToString();
    }
}
