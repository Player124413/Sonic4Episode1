using System;

internal partial class AppMain
{
    internal sealed class GMS_MAP_FAR_VARIANT
    {
        // mobile draws the sea everywhere on acts 1 to 3 and not at all at the boss; console culls
        // it per stage against the camera position and does draw it at the boss
        public Func<bool> zone_1_sea_is_hidden;

        // the sky is suppressed at the zone 1 boss only where the flat backdrop replaces it
        public bool zone_1_sky_hidden_at_boss;

        // hand-tuned windows that hide scenery the terrain already covers. the two arms were tuned
        // against different cameras, so they are separate sets rather than the same set retuned
        public Func<bool> zone_1_rocks_are_hidden;
        public Func<bool> zone_2_night_view_is_hidden;

        // one more rock each in DrawRockA and DrawRockB at the zone 1 boss, where mobile draws the
        // flat backdrop instead and suppresses the rocks entirely
        public bool zone_1_draws_extra_boss_rocks;
        public uint[] zone_1_obj_data;
        public uint[] zone_1_obj_data_mat_motion;
        public MPP_VOID_OBS_OBJECT_WORK[] zone_1_obj_func_main;
        public MPP_VOID_OBS_OBJECT_WORK[] zone_1_obj_func_out;

        // the sea is built out of the render model list rather than the model list
        public int zone_1_render_index;

        // indices below this draw into PRE_MAPFAR, so the sea samples them before it draws over
        public int zone_1_pre_mapfar_below;

        // the mirrored group is reflection source geometry, not scenery. console never shows it
        // directly: the sea user func ends the scene and switches render target before MAPFAR
        public bool zone_1_draws_mirror_group;

        // the flat backdrop slot, which retail draws only on a stage our load table does not have.
        // -1 where the arm has no slot to skip
        public int zone_1_skip_index;

        public int zone_1_sky_index;
        public int zone_1_sea_index;

        // GMD_MAP_FAR_SEAMODE. the sea samples the scene it has already drawn into, so on console it
        // picks a permutation with a user sampler instead of drawing as ordinary geometry
        public bool zone_1_sea_samples_the_scene;

        public uint[] zone_2_obj_data;
        public uint[] zone_2_obj_data_motion;
        public uint[] zone_2_obj_data_mat_motion;
        public MPP_VOID_OBS_OBJECT_WORK[] zone_2_obj_func_main;
        public MPP_VOID_OBS_OBJECT_WORK[] zone_2_obj_func_out;

        // console has a sky object at all here, and doubles it on every axis
        public bool zone_2_scales_sky;

        // the wheel and the searchlight. mobile bails at the boss, which is what makes their
        // 2_BOSS transform unreachable there
        public bool zone_2_scenery_hidden_at_boss;

        // gmMapFarCameraGetPos clamps to the scroll width, so these define the coordinate range the
        // per-stage cull windows above are expressed in. zone 2 and zone 3 are the only zones that
        // fork, and getting them from the wrong arm silently moves every window
        public GMS_MAP_FAR_SCROLL[] zone_2_scroll_x;
        public GMS_MAP_FAR_SCROLL[] zone_2_scroll_y;
        public GMS_MAP_FAR_SCROLL[] zone_2_boss_scroll_y;

        // the whole of zone 3's fork: mobile ships an empty material archive and animates nothing
        public uint[] zone_3_obj_data_mat_motion;

        public GMS_MAP_FAR_SCROLL[] zone_3_scroll_x;
        public GMS_MAP_FAR_SCROLL[] zone_3_scroll_y;

        public uint[] zone_final_obj_data;
        public MPP_VOID_OBS_OBJECT_WORK[] zone_final_obj_func_main;
        public MPP_VOID_OBS_OBJECT_WORK[] zone_final_obj_func_out;

        // earth, cloud and film each turn at their own rate, picked off user_work
        public bool zone_final_rotates_per_object;

        public NNS_RGBA_U8[] clear_color;

        // by zone type; zone 4 gets no call at all on either arm
        public bool[] fog_enabled;

        public bool fog_off_at_zone_1_boss;

        // bends the special stage material motion's start frame to work around broken iPhone
        // content, which console does not have
        public bool applies_ss_startframe_hack;
    }
}
