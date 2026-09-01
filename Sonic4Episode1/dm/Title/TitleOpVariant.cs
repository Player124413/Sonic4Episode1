using static AppMain;

internal static partial class DmTitleOp
{
    // MAPFAR_ZONE1_MDL is the same archive path either way and amFsResolve decides which build's
    // members land there. Console ships the mirrored _B_D group in front of the real _T_D one, so
    // every index moves; mobile has no mirrored geometry at all and fakes the lower half.
    internal sealed class DMS_TITLEOP_VARIANT
    {
        public int sky_top_index;
        public int sky_bottom_index;

        // the rock A entry; B and C follow it
        public int rock_top_index;
        public int rock_bottom_index;

        // mobile has no lower rock: dmTitleOp.cpp:1600-1633 is entirely #if !_IPHONE
        public bool draws_bottom_rock;

        // OBD_DISP_NOCLIP is an iPhone-only flag, so this is the arm that sets it
        public bool rock_top_no_clip;

        // GMD_MAP_FAR_SEAMODE, as in the far map: the sea refracts the scene behind it
        public bool sea_samples_the_scene;
    }

    private static DMS_TITLEOP_VARIANT dm_titleop_variant = dm_titleop_variant_mobile;

    // the group is one archive, so the gate and amFsReadBackground cannot disagree: there is no
    // partial set to fall back from, and both read the same global
    private static bool dmTitleOpIsConsoleDataComplete()
    {
        return amFsAllVariant(AMD_FS_VARIANT_CONSOLE, dm_titleop_com_fileinfo_list[0].file_path);
    }

    private static void dmTitleOpSelectVariant()
    {
        dm_titleop_variant = dmTitleOpIsConsoleDataComplete()
            ? dm_titleop_variant_console
            : dm_titleop_variant_mobile;
    }

    internal static DMS_TITLEOP_VARIANT TitleOpVariantForTest(bool console)
    {
        return console ? dm_titleop_variant_console : dm_titleop_variant_mobile;
    }

    // MAPFAR_ZONE1_MDL is Z1_SKYL_T_D, Z1_ROCK_A/B/C_T_D, Z1_BOSS_FAR
    private static readonly DMS_TITLEOP_VARIANT dm_titleop_variant_mobile = new DMS_TITLEOP_VARIANT
    {
        sky_top_index = IDB_MAPFAR_ZONE1_MDL_Z1_SKYL_T_D_INO,

        // no mirrored sky exists, so the lower half draws the upper one again
        sky_bottom_index = IDB_MAPFAR_ZONE1_MDL_Z1_SKYL_T_D_INO,

        rock_top_index = IDB_MAPFAR_ZONE1_MDL_Z1_ROCK_A_T_D_INO,
        rock_bottom_index = -1,
        draws_bottom_rock = false,

        rock_top_no_clip = true,

        sea_samples_the_scene = false,
    };

    // MAPFAR_ZONE1_MDL is Z1_SKYL_B_D, Z1_ROCK_A/B/C_B_D, Z1_SKYL_T_D, Z1_ROCK_A/B/C_T_D
    private static readonly DMS_TITLEOP_VARIANT dm_titleop_variant_console = new DMS_TITLEOP_VARIANT
    {
        sky_top_index = 4,
        sky_bottom_index = 0,

        rock_top_index = 5,
        rock_bottom_index = 1,
        draws_bottom_rock = true,

        rock_top_no_clip = false,

        sea_samples_the_scene = true,
    };
}
