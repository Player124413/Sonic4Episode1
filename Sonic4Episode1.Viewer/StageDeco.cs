using System;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sonic4Episode1.Viewer;

// what a decoration id draws with, taken off the game's own dispatch table rather than a list of
// ids copied by hand
internal enum StageDecoKind
{
    None,
    Model,
    Fall,
}

// one .dc record placed, the decoration twin of StageBlock
internal sealed class StageDecoration
{
    public int Id;
    public int ModelIndex;
    public StageDecoKind Kind;
    public Vector3 Position;
    public Matrix World;

    // in the map plane, for culling, the same thing StageBlock.Centre is
    public Vector2 Centre;
}

// The decorations for one stage, loaded the way gmGameDatLoadProcPostDeco and GmDecoBuildData do,
// out of a plain directory. Parse only, like StageMap. See docs/console-deco.md.
internal sealed class StageDeco
{
    public string Path { get; }

    public AppMain.AMS_AMB_HEADER Archive { get; private set; }
    public AppMain.AMS_AMB_HEADER ModelArchive { get; private set; }
    public AppMain.AMS_AMB_HEADER TextureArchive { get; private set; }
    public AppMain.AMS_AMB_HEADER FallModelArchive { get; private set; }
    public AppMain.AMS_AMB_HEADER FallTextureArchive { get; private set; }

    public StageModelFormat ModelFormat { get; private set; }

    public int ModelCount => ModelArchive?.file_num ?? 0;
    public int FallModelCount => FallModelArchive?.file_num ?? 0;

    public StageDeco(string path)
    {
        Path = path;
    }

    public void Load(string root)
    {
        var full = System.IO.Path.Combine(root, Path.Replace('/', System.IO.Path.DirectorySeparatorChar));
        if (!File.Exists(full))
            throw new FileNotFoundException($"{Path} is not in {root}", full);

        Archive = NnArchive.Open(full, out _);

        ModelArchive = Nested(AppMain.GMD_DECO_DATA_INDEX_AMB_MDL);
        TextureArchive = Nested(AppMain.GMD_DECO_DATA_INDEX_AMB_TEX);
        FallModelArchive = Nested(AppMain.GMD_DECO_DATA_INDEX_AMB_MDL_RENDER);
        FallTextureArchive = Nested(AppMain.GMD_DECO_DATA_INDEX_AMB_TEX_RENDER);

        ModelFormat = DetectFormat(ModelArchive);
    }

    private AppMain.AMS_AMB_HEADER Nested(int member)
    {
        return member < Archive.file_num
            ? NnArchive.ReadNested(AppMain.amBindGet(Archive, member, out _))
            : null;
    }

    // console ships NN objects here, mobile ships baked TVX, the same split ZONEx_M.AMB has
    private static StageModelFormat DetectFormat(AppMain.AMS_AMB_HEADER models)
    {
        for (int i = 0; models != null && i < models.file_num; i++)
        {
            var chunk = AppMain.amBindGet(models, i, out _);
            if (chunk.length < 4)
                continue;

            return NnArchive.ReadObject(chunk) != null
                ? StageModelFormat.NnObject
                : StageModelFormat.Tvx;
        }

        return StageModelFormat.Unknown;
    }

    // g_gm_decorate_tbl is the game's dispatch, and it has no _IPHONE fork, so the init function an
    // id is registered with says what it draws with on either build
    public static StageDecoKind KindOf(int id)
    {
        if (id < 0 || id >= AppMain.g_gm_decorate_tbl.Length)
            return StageDecoKind.None;

        var init = AppMain.g_gm_decorate_tbl[id];
        if (init == null)
            return StageDecoKind.None;

        switch (init.Method.Name)
        {
            case "GmDecoInitModel":
            case "GmDecoInitModelMaterial":
            case "GmDecoInitModelMotion":
            case "GmDecoInitModelMotionTouch":
            case "GmDecoInitModelMotionMaterial":
            case "GmDecoInitModelMotionMaterialTouch":
            case "GmDecoInitModelLoop":
            case "GmDecoInitModelEffect":
                return StageDecoKind.Model;

            case "GmDecoInitFall":
                return StageDecoKind.Fall;

            // Primitive3D is the glare, the Effect family spawns an effect and no model of its own
            default:
                return StageDecoKind.None;
        }
    }

    // gmDecoLoadObj: the .dc position plus the table offset, and the table is fx32. Y is negated
    // because game space counts down and the renderer flips it, which GmDecoGlareDraw does in as
    // many words at GmDecoGlare.cs:46. Nothing rotates: g_gm_deco_rot_z is 184 zeroes in this build
    // and in SEGA's source too.
    public static Vector3 WorldPosition(Vector2 placed, int id)
    {
        var offset = AppMain.g_gm_deco_pos[id];

        return new Vector3(
            placed.X + FX(offset[0]),
            placed.Y - FX(offset[1]),
            FX(offset[2]));
    }

    // ObjDrawAction3DNN scales the model by g_obj.draw_scale unless OBD_DISP_NODRAWSCALE is set,
    // and GmMain sets draw_scale to GMD_OBJ_DRAW_SCALE once for the whole game. The blocks bake the
    // same 3.2 into their own matrix instead, because they suppress the automatic one.
    // OBD_DISP_FLIP_X and _Y become a negative scale in gmDecoInitNomodel, which then clears them.
    public static Vector3 Scale(int id)
    {
        uint disp = AppMain.g_gm_deco_disp_flag[id];
        float scale = (disp & OBD_DISP_NODRAWSCALE) != 0 ? 1f : AppMain.GMD_OBJ_DRAW_SCALE;

        return new Vector3(
            (disp & OBD_DISP_FLIP_X) != 0 ? -scale : scale,
            (disp & OBD_DISP_FLIP_Y) != 0 ? -scale : scale,
            scale);
    }

    private const uint OBD_DISP_FLIP_X = 1;
    private const uint OBD_DISP_FLIP_Y = 2;
    private const uint OBD_DISP_NODRAWSCALE = 1 << 20;

    private static float FX(int value) => value / 4096f;

    // what the stage's .dc records resolve to, without a graphics device. The ids that come back
    // as None are the glare and the effects, which have no model of their own.
    public string PlacementSummary(System.Collections.Generic.List<StageMarker> markers)
    {
        var text = new StringBuilder();
        var byId = new System.Collections.Generic.SortedDictionary<int, int>();

        foreach (var marker in markers)
        {
            if (marker.Kind != StageMarkerKind.Decoration)
                continue;

            byId.TryGetValue(marker.Id, out int n);
            byId[marker.Id] = n + 1;
        }

        int model = 0, fall = 0, none = 0;

        foreach (var pair in byId)
        {
            var kind = KindOf(pair.Key);
            int index = AppMain.g_gm_deco_model_index[pair.Key][0];
            var archive = kind == StageDecoKind.Model ? ModelArchive
                : kind == StageDecoKind.Fall ? FallModelArchive
                : null;

            var member = archive != null && index >= 0 && index < archive.file_num
                ? System.IO.Path.GetFileName(archive.files[index].Replace('\\', '/'))
                : "";

            switch (kind)
            {
                case StageDecoKind.Model: model += pair.Value; break;
                case StageDecoKind.Fall: fall += pair.Value; break;
                default: none += pair.Value; break;
            }

            text.AppendLine($"      id {pair.Key,3}  x{pair.Value,-4} {kind,-5}"
                + $" model {index,3}  {member}");
        }

        text.AppendLine($"      {model} model, {fall} fall, {none} without a model of their own");
        return text.ToString();
    }

    public string Summary()
    {
        var text = new StringBuilder();

        text.AppendLine($"    {Path,-34} {Archive.file_num,4} member(s)"
            + (Archive.big_endian ? ", big endian" : ""));
        text.AppendLine($"      models: {ModelFormat}, {ModelCount} member(s),"
            + $" {TextureArchive?.file_num ?? 0} texture(s)");
        text.AppendLine($"      falls:  {FallModelCount} member(s),"
            + $" {FallTextureArchive?.file_num ?? 0} texture(s)");

        return text.ToString();
    }
}
