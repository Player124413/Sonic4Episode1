internal partial class AppMain
{
    // the mobile arm names tables that AppMain.cctor.cs assigns in its body, so these are built on
    // first use rather than in a field initialiser: order across a partial class's files is not
    // defined, and a descriptor built too early would bind to null
    private static GMS_MAP_FAR_VARIANT gm_map_far_variant_mobile_built;
    private static GMS_MAP_FAR_VARIANT gm_map_far_variant_console_built;

    private static GMS_MAP_FAR_VARIANT gm_map_far_variant;

    private static GMS_MAP_FAR_VARIANT gmMapFarVariantMobile()
    {
        return gm_map_far_variant_mobile_built ??= new GMS_MAP_FAR_VARIANT
        {
            zone_1_obj_data = g_map_far_zone_1_scene_obj_data,
            zone_1_obj_data_mat_motion = g_map_far_zone_1_scene_obj_data_mat_motion,
            zone_1_obj_func_main = g_map_far_zone_1_scene_obj_func_main,
            zone_1_obj_func_out = g_map_far_zone_1_scene_obj_func_out,

            zone_1_render_index = 0,

            // SEA is index 0, so the PRE_MAPFAR arm is unreachable and everything lands in MAPFAR
            zone_1_pre_mapfar_below = 0,

            // mobile has no mirrored group to draw
            zone_1_draws_mirror_group = false,

            zone_1_sea_is_hidden = gmMapFarSeaIsHiddenMobile,

            zone_1_sky_hidden_at_boss = true,

            zone_1_rocks_are_hidden = gmMapFarRocksAreHiddenMobile,
            zone_2_night_view_is_hidden = gmMapFarNightViewIsHiddenMobile,

            zone_1_draws_extra_boss_rocks = false,

            // the flat backdrop is drawn here rather than skipped
            zone_1_skip_index = -1,

            zone_1_sky_index = (int)GMD_MAP_FAR_OBJ_INDEX.GMD_MAP_FAR_OBJ_INDEX_ZONE_1_SKY,
            zone_1_sea_index = (int)GMD_MAP_FAR_OBJ_INDEX.GMD_MAP_FAR_OBJ_INDEX_ZONE_1_SEA,

            zone_1_sea_samples_the_scene = false,

            zone_2_obj_data = g_map_far_zone_2_scene_obj_data,
            zone_2_obj_data_motion = g_map_far_zone_2_scene_obj_data_motion,
            zone_2_obj_data_mat_motion = g_map_far_zone_2_scene_obj_data_mat_motion,
            zone_2_obj_func_main = g_map_far_zone_2_scene_obj_func_main,
            zone_2_obj_func_out = g_map_far_zone_2_scene_obj_func_out,

            // no sky object to scale; index 0 is the flat backdrop
            zone_2_scales_sky = false,

            zone_2_scenery_hidden_at_boss = true,

            zone_2_scroll_x = g_map_far_zone_2_scroll_x,
            zone_2_scroll_y = g_map_far_zone_2_scroll_y,
            zone_2_boss_scroll_y = g_map_far_zone_2_boss_scroll_y,

            zone_3_obj_data_mat_motion = g_map_far_zone_3_scene_obj_data_mat_motion,

            zone_3_scroll_x = g_map_far_zone_3_scroll_x,
            zone_3_scroll_y = g_map_far_zone_3_scroll_y,

            zone_final_obj_data = g_map_far_zone_final_scene_obj_data,
            zone_final_obj_func_main = g_map_far_zone_final_scene_obj_func_main,
            zone_final_obj_func_out = g_map_far_zone_final_scene_obj_func_out,

            zone_final_rotates_per_object = false,

            clear_color = g_gm_map_far_clear_color,

            fog_enabled = new[] { true, false, false, false, false, true },

            fog_off_at_zone_1_boss = true,

            applies_ss_startframe_hack = true,
        };
    }

    private static GMS_MAP_FAR_VARIANT gmMapFarVariantConsole()
    {
        return gm_map_far_variant_console_built ??= new GMS_MAP_FAR_VARIANT
        {
            // SKYB, ROCKA_B, ROCKB_B, ROCKC_B, SEA, SKY, ROCKA, ROCKB, ROCKC. GMD_MAP_FAR_2DBG is
            // 0 on console so there is no tenth entry. index 4 is into the render list, which is
            // why it repeats 0
            zone_1_obj_data = new uint[] { 0, 1, 2, 3, 0, 4, 5, 6, 7 },

            // the mirrored sky reuses the real sky's material motion
            zone_1_obj_data_mat_motion = new uint[]
            {
                0, uint.MaxValue, uint.MaxValue, uint.MaxValue, 1,
                0, uint.MaxValue, uint.MaxValue, uint.MaxValue,
            },

            zone_1_obj_func_main = new MPP_VOID_OBS_OBJECT_WORK[9],

            zone_1_obj_func_out = new MPP_VOID_OBS_OBJECT_WORK[]
            {
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawRotate),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawRockA),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawRockB),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawRockC),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawSea),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawRotate),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawRockA),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawRockB),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawRockC),
            },

            zone_1_render_index = 4,

            zone_1_pre_mapfar_below = 4,

            // faithful. console hides these behind gmMapFarDrawSeaUserFunc, which ends the scene and
            // switches render target before MAPFAR, and that is the deferred sea shader path. if
            // they ever show up directly, that is why
            zone_1_draws_mirror_group = true,

            zone_1_skip_index = -1,

            zone_1_sea_is_hidden = gmMapFarSeaIsHiddenConsole,

            // no flat backdrop to replace it, so the sky draws at the boss like anywhere else
            zone_1_sky_hidden_at_boss = false,

            zone_1_rocks_are_hidden = gmMapFarRocksAreHiddenConsole,
            zone_2_night_view_is_hidden = gmMapFarNightViewIsHiddenConsole,

            zone_1_draws_extra_boss_rocks = true,

            zone_1_sky_index = 5,
            zone_1_sea_index = 4,

            zone_1_sea_samples_the_scene = true,

            // SKY, GROUND, WHEEL, GLARE, SLIGHT
            zone_2_obj_data = new uint[] { 0, 1, 2, 3, 4 },
            zone_2_obj_data_motion = new uint[]
            {
                uint.MaxValue, uint.MaxValue, 0, uint.MaxValue, 1,
            },
            zone_2_obj_data_mat_motion = new uint[]
            {
                uint.MaxValue, 0, 1, 2, uint.MaxValue,
            },
            zone_2_obj_func_main = new MPP_VOID_OBS_OBJECT_WORK[5],
            zone_2_obj_func_out = new MPP_VOID_OBS_OBJECT_WORK[]
            {
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawRotate),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawBg),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawWheel),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawBg),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawSLight),
            },

            zone_2_scales_sky = true,

            zone_2_scenery_hidden_at_boss = false,

            zone_2_scroll_x = new GMS_MAP_FAR_SCROLL[] { new GMS_MAP_FAR_SCROLL(0, 300, 1) },
            zone_2_scroll_y = new GMS_MAP_FAR_SCROLL[] { new GMS_MAP_FAR_SCROLL(0, 50, 1) },
            zone_2_boss_scroll_y = new GMS_MAP_FAR_SCROLL[] { new GMS_MAP_FAR_SCROLL(0, 50, 1) },

            // Z3_FAR_ZNV, the light shafts from the sky
            zone_3_obj_data_mat_motion = new uint[] { 0 },

            zone_3_scroll_x = new GMS_MAP_FAR_SCROLL[] { new GMS_MAP_FAR_SCROLL(0, 600, 1) },
            zone_3_scroll_y = new GMS_MAP_FAR_SCROLL[] { new GMS_MAP_FAR_SCROLL(0, 100, 1) },

            // SPACE, EARTH, CLOUD, FILM_A. no material motion archive members at all
            zone_final_obj_data = new uint[] { 0, 1, 2, 3 },
            zone_final_obj_func_main = new MPP_VOID_OBS_OBJECT_WORK[4],
            zone_final_obj_func_out = new MPP_VOID_OBS_OBJECT_WORK[]
            {
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawBg),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawRotate),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawRotate),
                new MPP_VOID_OBS_OBJECT_WORK(gmMapFarSceneObjFuncDrawRotate),
            },

            zone_final_rotates_per_object = true,

            clear_color = new NNS_RGBA_U8[]
            {
                new NNS_RGBA_U8(0x44, 0x66, 0xFF, byte.MaxValue),
                new NNS_RGBA_U8(0, 0, 0, byte.MaxValue),
                new NNS_RGBA_U8(85, 118, 109, byte.MaxValue),
                new NNS_RGBA_U8(0x44, 0xFF, 0xFF, byte.MaxValue),
                new NNS_RGBA_U8(0, 0, 0, byte.MaxValue),
                new NNS_RGBA_U8(0xE0, 0xE0, 0xE0, 0xE0),
            },

            fog_enabled = new[] { true, true, true, false, true, true },

            fog_off_at_zone_1_boss = false,

            applies_ss_startframe_hack = false,
        };
    }

    internal static GMS_MAP_FAR_VARIANT MapFarVariantForTest(bool console)
    {
        return console ? gmMapFarVariantConsole() : gmMapFarVariantMobile();
    }

    // the cull windows read the live camera position, which is only reachable once a stage is
    // loaded, so a test needs somewhere to put a position
    internal static bool NightViewIsHiddenForTest(GMS_MAP_FAR_VARIANT variant, int stage_id, float y, float z)
    {
        GMS_MAP_FAR_DATA saved_data = g_map_far_data;
        int saved_stage = g_gs_main_sys_info.stage_id;

        g_map_far_data = g_map_far_data_real;
        g_map_far_data.pos.y = y;
        g_map_far_data.pos.z = z;
        g_gs_main_sys_info.stage_id = (ushort)stage_id;

        bool hidden = variant.zone_2_night_view_is_hidden();

        g_gs_main_sys_info.stage_id = (ushort)saved_stage;
        g_map_far_data = saved_data;
        return hidden;
    }
}
