using System;
using Microsoft.Xna.Framework;

namespace Sonic4Episode1.Viewer;

// The per-stage placement functions. Each emits nothing, one, or several instances of its model at
// hardcoded offsets, gated on where the background camera is; gmMapFarSceneObjFuncDraw* in the
// oracle. FlatBg is Draw2DBG, the single quad mobile substitutes for real geometry.
internal enum StageMapFarDraw
{
    Bg,
    FlatBg,
    Rotate,
    RockA,
    RockB,
    RockC,
    Wheel,
    SearchLight,
    Sea,
}

// zone 1's sea is the lone member of RENDER_MDL; everything else comes out of MDL
internal enum StageMapFarSource
{
    Models,
    RenderModels,
}

internal readonly struct StageMapFarObject
{
    public StageMapFarSource Source { get; }
    public int Model { get; }
    public int Motion { get; }
    public int MaterialMotion { get; }
    public uint Bucket { get; }
    public StageMapFarDraw Draw { get; }

    public StageMapFarObject(
        StageMapFarSource source, int model, int motion, int materialMotion,
        uint bucket, StageMapFarDraw draw)
    {
        Source = source;
        Model = model;
        Motion = motion;
        MaterialMotion = materialMotion;
        Bucket = bucket;
        Draw = draw;
    }
}

// GMS_MAP_FAR_SCROLL. gmMapFarCameraGetPos walks these per axis and clamps flat past the last one.
internal readonly struct StageMapFarScroll
{
    public int Pos { get; }
    public int Width { get; }
    public int LoopNum { get; }

    public StageMapFarScroll(int pos, int width, int loopNum)
    {
        Pos = pos;
        Width = width;
        LoopNum = loopNum;
    }
}

// gmMapFarTcbProcPreDraw's per-zone fog. Console turns it on for every zone type; mobile leaves it
// on only for zone 1 and the special stage, so this is one of the visible arm differences.
internal readonly struct StageMapFarFog
{
    public Vector3 Colour { get; }
    public float Near { get; }
    public float Far { get; }

    private StageMapFarFog(Vector3 colour, float near, float far)
    {
        Colour = colour;
        Near = near;
        Far = far;
    }

    // the uniforms have to be real either way, so "off" is pushed past anything we draw
    private static StageMapFarFog Off => new(Vector3.Zero, 100000f, 200000f);

    public static StageMapFarFog For(int zoneType, int stageId, StageMapFarArm arm)
    {
        bool console = arm == StageMapFarArm.Console;

        switch (zoneType)
        {
            case 0:
                if (!console && stageId == 3)
                    return Off;
                return new StageMapFarFog(
                    stageId == 2 || stageId == 3
                        ? new Vector3(0.85f, 0.5f, 0.25f)
                        : new Vector3(0.7f, 0.95f, 1f),
                    100f, 500f);

            case 1:
                return console ? new StageMapFarFog(new Vector3(0f, 0f, 0.3f), 450f, 650f) : Off;

            case 2:
                return console
                    ? new StageMapFarFog(new Vector3(0.3333333f, 0.4627451f, 0.427451f), 300f, 1300f)
                    : Off;

            case 4:
                return console ? new StageMapFarFog(new Vector3(0.1f, 0.08f, 0.22f), 160f, 1100f) : Off;

            case 5:
                return new StageMapFarFog(Vector3.Zero, 100f, 1000f);

            default:
                return Off;
        }
    }
}

// g_map_far_zone_N_scene_obj_data and its _motion / _mat_motion / _func_out siblings, both arms,
// transcribed from gmMapFar.cpp:142-203 and :575-976. Console entries are the #else arm.
internal static class StageMapFarTable
{
    private const int None = -1;

    private const uint Pre = AppMain.OBD_DRAW_CMD_STATE_PRE_MAPFAR;
    private const uint Far = AppMain.OBD_DRAW_CMD_STATE_MAPFAR;

    private static StageMapFarObject Model(
        int model, int materialMotion, uint bucket, StageMapFarDraw draw, int motion = None) =>
        new(StageMapFarSource.Models, model, motion, materialMotion, bucket, draw);

    private static StageMapFarObject Render(
        int model, int materialMotion, uint bucket, StageMapFarDraw draw) =>
        new(StageMapFarSource.RenderModels, model, None, materialMotion, bucket, draw);

    private static readonly StageMapFarObject[] Zone1Mobile =
    {
        Render(0, 1, Far, StageMapFarDraw.Sea),
        Model(0, 0, Far, StageMapFarDraw.Rotate),
        Model(1, None, Far, StageMapFarDraw.RockA),
        Model(2, None, Far, StageMapFarDraw.RockB),
        Model(3, None, Far, StageMapFarDraw.RockC),
        Model(4, 2, Far, StageMapFarDraw.FlatBg),
    };

    // the four _B models are the pre-mirrored reflection, which is why they draw a bucket earlier:
    // gmMapFar.cpp:4721 is `i >= ZONE_1_SEA ? MAPFAR : PRE_MAPFAR` and SEA is 4 here
    private static readonly StageMapFarObject[] Zone1Console =
    {
        Model(0, 0, Pre, StageMapFarDraw.Rotate),
        Model(1, None, Pre, StageMapFarDraw.RockA),
        Model(2, None, Pre, StageMapFarDraw.RockB),
        Model(3, None, Pre, StageMapFarDraw.RockC),
        Render(0, 1, Far, StageMapFarDraw.Sea),
        Model(4, 0, Far, StageMapFarDraw.Rotate),
        Model(5, None, Far, StageMapFarDraw.RockA),
        Model(6, None, Far, StageMapFarDraw.RockB),
        Model(7, None, Far, StageMapFarDraw.RockC),
    };

    private static readonly StageMapFarObject[] Zone2Mobile =
    {
        Model(0, None, Pre, StageMapFarDraw.FlatBg),
        Model(1, 0, Pre, StageMapFarDraw.Wheel, motion: 0),
        Model(2, None, Pre, StageMapFarDraw.SearchLight, motion: 1),
    };

    private static readonly StageMapFarObject[] Zone2Console =
    {
        Model(0, None, Pre, StageMapFarDraw.Rotate),
        Model(1, 0, Pre, StageMapFarDraw.Bg),
        Model(2, 1, Pre, StageMapFarDraw.Wheel, motion: 0),
        Model(3, 2, Pre, StageMapFarDraw.Bg),
        Model(4, None, Pre, StageMapFarDraw.SearchLight, motion: 1),
    };

    private static readonly StageMapFarObject[] Zone3Mobile =
    {
        Model(0, None, Far, StageMapFarDraw.FlatBg),
    };

    private static readonly StageMapFarObject[] Zone3Console =
    {
        Model(0, 0, Far, StageMapFarDraw.Bg),
    };

    private static readonly StageMapFarObject[] FinalMobile =
    {
        Model(0, None, Far, StageMapFarDraw.FlatBg),
    };

    // the three rotating members read their own index back out of user_work so earth, cloud and
    // film can turn at different rates
    private static readonly StageMapFarObject[] FinalConsole =
    {
        Model(0, None, Far, StageMapFarDraw.Bg),
        Model(1, None, Far, StageMapFarDraw.Rotate),
        Model(2, None, Far, StageMapFarDraw.Rotate),
        Model(3, None, Far, StageMapFarDraw.Rotate),
    };

    private static readonly StageMapFarObject[] SpecialStage =
    {
        Model(0, 0, Pre, StageMapFarDraw.Bg),
    };

    private static readonly StageMapFarObject[] None3d = Array.Empty<StageMapFarObject>();

    public static StageMapFarObject[] Objects(int zoneType, StageMapFarArm arm)
    {
        bool console = arm == StageMapFarArm.Console;

        return zoneType switch
        {
            0 => console ? Zone1Console : Zone1Mobile,
            1 => console ? Zone2Console : Zone2Mobile,
            2 => console ? Zone3Console : Zone3Mobile,
            4 => console ? FinalConsole : FinalMobile,
            5 => SpecialStage,
            _ => None3d,
        };
    }

    // zone 1's boss is stage 3 and zone 2's is stage 7; every other zone has one pair
    public static StageMapFarScroll ScrollX(int zoneType, StageMapFarArm arm, bool boss)
    {
        bool console = arm == StageMapFarArm.Console;

        return zoneType switch
        {
            0 => boss ? new StageMapFarScroll(0, 10, 1) : new StageMapFarScroll(0, 600, 1),
            1 when boss => new StageMapFarScroll(0, 20, 1),
            1 => console ? new StageMapFarScroll(0, 300, 1) : new StageMapFarScroll(0, 90, 1),
            2 => console ? new StageMapFarScroll(0, 600, 1) : new StageMapFarScroll(0, 678, 1),
            4 => new StageMapFarScroll(0, 50, 1),
            5 => new StageMapFarScroll(0, 0, 1),
            _ => new StageMapFarScroll(0, 0, 0),
        };
    }

    public static StageMapFarScroll ScrollY(int zoneType, StageMapFarArm arm, bool boss)
    {
        bool console = arm == StageMapFarArm.Console;

        return zoneType switch
        {
            0 => boss ? new StageMapFarScroll(0, 0, 0) : new StageMapFarScroll(0, 20, 1),
            1 => console ? new StageMapFarScroll(0, 50, 1) : new StageMapFarScroll(0, 40, 1),
            2 => console ? new StageMapFarScroll(0, 100, 1) : new StageMapFarScroll(0, 51, 1),
            4 => new StageMapFarScroll(0, 10, 1),
            _ => new StageMapFarScroll(0, 0, 0),
        };
    }

    public static bool IsBossStage(int stageId) => stageId == 3 || stageId == 7;

    // ObjDrawAction3DNN turns every 3D object 90 degrees about Y unless OBD_DISP_NODIRFLIP is set
    // (ObjDraw.cs:371-378). The block path sets it and escapes; map far's placement functions
    // mostly do not, so most of the background is authored expecting that turn. The ones that OR in
    // 13705472 rather than 9502720 do set it: the sky, the sea, and the final zone's backdrop.
    public static bool DirFlip(StageMapFarDraw draw, int zoneType) => draw switch
    {
        StageMapFarDraw.Sea => false,
        StageMapFarDraw.Rotate => false,
        StageMapFarDraw.Bg => zoneType != 4,
        _ => true,
    };

    // g_gm_map_far_clear_color. Zone 1 and zone 4 are the two entries the arms disagree on, and
    // zone 1's is the visible one: mobile fills the horizon brown, console sky blue.
    public static Color ClearColour(int zoneType, StageMapFarArm arm)
    {
        bool console = arm == StageMapFarArm.Console;

        return zoneType switch
        {
            0 when console => new Color(0x44, 0x66, 0xff),
            0 => new Color(0x5a, 0x30, 0x10),
            2 => new Color(0x55, 0x76, 0x6d),
            3 when console => new Color(0x44, 0xff, 0xff),
            5 => Color.White,
            _ => Color.Black,
        };
    }
}
