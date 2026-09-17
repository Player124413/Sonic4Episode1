using mpp;

internal partial class AppMain
{
    // 0 is pitch black, 256 turns the whole effect off. The map's own lamps follow the same level
    // through gm_map_dark_mmtn_frame; this file owns the light map and the composite
    internal static int gm_dark_target;
    internal static int gm_dark_level;
    internal static float gm_map_dark_mmtn_frame;

    private static NNS_RGBA_U8 gm_dark_color;
    private static AMS_RENDER_TARGET gm_ply_postef_rt;
    private static MTS_TASK_TCB gm_ply_postef_tcb;

    // the synthetic preset the darkness draws with, against the four deco ones. White, five times
    // the size, and the caller supplies the alpha
    private static readonly GMS_DECOGLARE_PARAM gm_dark_light_sprite_param =
        new GMS_DECOGLARE_PARAM(uint.MaxValue, 200f, 1f, 1);

    internal const int GMD_DARK_LEVEL_MAX = 256;
    private const int GMD_DARK_LEVEL_STEP = 32;

    internal static void GmPlyPostEffectInit()
    {
        gm_dark_target = 0;
        gm_dark_level = 0;
        gm_dark_color = new NNS_RGBA_U8(0, 0, 0, 0);
        gm_map_dark_mmtn_frame = 0.0f;

        // a retry re-runs this without ever reaching GmPlyPostEffectRelease, so without the guard
        // every restart left another task queueing its own composite and the level got darker each
        // time. PC guards the same way at 0x4EF123
        if (gm_ply_postef_tcb != null)
            return;

        // ahead of the player, the enemies and the gimmicks, because the camera command it appends
        // to the light map bucket has to land before any blob they queue into the same bucket
        gm_ply_postef_tcb = MTM_TASK_MAKE_TCB(
            new GSF_TASK_PROCEDURE(gmPlyPostEffectMain), null, 0U, 0,
            GMD_TASK_PRIO_MAIN_PRE, 5, null, "GM_PLY_POSTEF");
    }

    internal static void GmPlyPostEffectRelease()
    {
        if (gm_ply_postef_tcb != null)
        {
            mtTaskClearTcb(gm_ply_postef_tcb);
            gm_ply_postef_tcb = null;
        }
        gm_ply_postef_rt = null;
    }

    internal static bool gmPlyPostEffectIsDark()
    {
        return gmStageIsLostLabyrinth2() && gm_dark_level < GMD_DARK_LEVEL_MAX;
    }

    private static void gmPlyPostEffectMain(MTS_TASK_TCB tcb)
    {
        if (!gmStageIsLostLabyrinth2())
            return;

        int target = gm_dark_target;
        GMS_PLAYER_WORK ply_work = g_gm_main_system.ply_work[GSD_MAIN_PLAYER_1P];
        if (ply_work != null && ((int)ply_work.player_flag & GMD_PLF_SUPER_SONIC) != 0)
            target = GMD_DARK_LEVEL_MAX;

        int delta = target - gm_dark_level;
        if (MTM_MATH_ABS(delta) < GMD_DARK_LEVEL_STEP / 2)
            gm_dark_level = target;
        else
            gm_dark_level += delta > 0 ? GMD_DARK_LEVEL_STEP : -GMD_DARK_LEVEL_STEP;

        int level = MTM_MATH_CLIP(gm_dark_level, 0, 255);
        gm_map_dark_mmtn_frame = level / 255f * 59f;

        byte grey = (byte)(gm_dark_level >= GMD_DARK_LEVEL_MAX ? 255 : level);
        gm_dark_color.r = gm_dark_color.g = gm_dark_color.b = gm_dark_color.a = grey;

        if (grey == byte.MaxValue || !GmMainIsDrawEnable())
            return;

        ObjDraw3DNNSetCameraEx(g_obj.glb_camera_id, g_obj.glb_camera_type, OBD_DRAW_CMD_STATE_LIGHTMAP);
        gmPlyPostEffectDrawPlayerLight(ply_work);
        _ObjDraw3DNNUserFunc(gmPlyPostEffectDarkRenderLightMap, null, OBD_DRAW_CMD_STATE_PRE_MAPFAR);
        _ObjDraw3DNNUserFunc(gmPlyPostEffectDarkComposite, null, OBD_DRAW_CMD_STATE_3DFIX);
    }

    // the torch light follows Sonic whether or not he is carrying one; the stage is what decides
    private static void gmPlyPostEffectDrawPlayerLight(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work == null)
            return;

        VecFx32 pos = ply_work.obj_work.pos;
        GmDarkLightSpriteDraw(
            pos.x >> 12, -((pos.y - 0x8000) >> 12), 256f,
            OBD_DRAW_CMD_STATE_LIGHTMAP, 1f, 1f, byte.MaxValue);
    }

    // list 1, before the scene: clear a half-res target to the darkness grey and run the private
    // bucket into it. Every blob in there is additive, so the result is the light for this frame
    private static void gmPlyPostEffectDarkRenderLightMap(object param)
    {
        if (gm_ply_postef_rt == null)
            gm_ply_postef_rt = new AMS_RENDER_TARGET();

        if (gm_ply_postef_rt.width == 0)
        {
            amRenderCreate(gm_ply_postef_rt, (int)AMD_SCREEN_WIDTH / 2, (int)AMD_SCREEN_HEIGHT / 2,
                AMD_RENDER_FLAG_RESOLVE_COLOR0 | AMD_RENDER_FLAG_USE_DEPTH);
            if (gm_ply_postef_rt.width == 0)
                return;
        }

        amDrawEndScene();
        AMS_RENDER_TARGET prev = amRenderSetTarget(gm_ply_postef_rt, 7U, gm_dark_color, 1f, 0);
        amDrawExecCommand(OBD_DRAW_CMD_STATE_LIGHTMAP, g_obj.drawflag);
        amDrawEndScene();
        amRenderSetTarget(prev);
    }

    // list 10, after the scene and before the HUD. dst = dst * src, so black in the light map is
    // black on screen and white leaves the pixel alone
    private static void gmPlyPostEffectDarkComposite(object param)
    {
        if (gm_ply_postef_rt == null || gm_ply_postef_rt.width == 0 || gm_ply_postef_rt.target == null)
            return;

        amDrawEndScene();
        OpenGL.mppglBlitMultiply(gm_ply_postef_rt.target);
    }
}
