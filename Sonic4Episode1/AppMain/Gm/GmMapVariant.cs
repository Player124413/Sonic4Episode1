internal partial class AppMain
{
    // these two must stay above gm_map_variant: static field initialisers run in textual order
    // within a file, so a consumer declared first binds to null
    internal static readonly GMS_MAP_VARIANT gm_map_variant_mobile = new GMS_MAP_VARIANT
    {
        blocks_as_model = false,

        registlist_num = 256,

        fs_variant = AMD_FS_VARIANT_BASE,
    };

    internal static readonly GMS_MAP_VARIANT gm_map_variant_console = new GMS_MAP_VARIANT
    {
        blocks_as_model = true,

        registlist_num = 128,

        fs_variant = AMD_FS_VARIANT_CONSOLE,
    };

    // resolved once per stage in GmGameDatLoadInit, never per frame
    private static GMS_MAP_VARIANT gm_map_variant = gm_map_variant_mobile;

    internal static GMS_MAP_VARIANT MapVariantForTest(bool console)
    {
        return console ? gm_map_variant_console : gm_map_variant_mobile;
    }

    // zone 4 loads zone 1's map far archive, so following the global variant would hand it a console
    // background against mobile blocks. these load pinned to what the stage actually resolved to
    internal static bool gmMapOwnsLoad(GMS_GAMEDAT_LOAD_DATA load_data)
    {
        return load_data.proc_post == gmGameDatLoadProcPostMap
            || load_data.proc_post == gmGameDatLoadProcPostMapFar;
    }

    // every map and map far archive the stage loads, named off the load table rather than spelled
    // out, so a stage cannot end up with console blocks against a mobile background
    internal static bool gmMapIsConsoleDataComplete(int stage_id)
    {
        var info = gm_gamedat_tbl_map_info_tbl[stage_id];
        var found = false;

        for (int index = 0; index < info.num; ++index)
        {
            var load_data = info.data_tbl[index];
            if (!gmMapOwnsLoad(load_data))
                continue;

            found = true;
            if (amFsResolvedVariant(load_data.path) != AMD_FS_VARIANT_CONSOLE)
                return false;
        }

        return found;
    }

    // map and map far are one group, so they move together
    private static void gmMapSelectVariant(int stage_id)
    {
        bool console = gmMapIsConsoleDataComplete(stage_id);

        gm_map_variant = console ? gm_map_variant_console : gm_map_variant_mobile;
        gm_map_far_variant = console ? gmMapFarVariantConsole() : gmMapFarVariantMobile();
    }
}
