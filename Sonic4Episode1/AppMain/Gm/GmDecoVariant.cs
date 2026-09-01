internal partial class AppMain
{
    // Decorations are a second genuine fork, not part of the map's. Console draws one NN model per
    // decoration; mobile draws baked TVX sprites off a single atlas and has no node motion at all.
    // They ship as their own amFsAllVariant group so a missing sibling on either side cannot drag
    // the other back, which is what MapVariantTests already asserts. See docs/console-deco.md.
    private static bool gm_deco_as_model;

    // named off the stage's own load table, the same way gmMapIsConsoleDataComplete is. There is
    // only ever one deco entry per stage, so this is all-or-none by construction rather than by
    // agreement, but the shape matches its sibling on purpose
    internal static bool gmDecoIsConsoleDataComplete(int stage_id)
    {
        var info = gm_gamedat_tbl_map_info_tbl[stage_id];
        var found = false;

        for (int index = 0; index < info.num; ++index)
        {
            var load_data = info.data_tbl[index];
            if (load_data.proc_post != gmGameDatLoadProcPostDeco)
                continue;

            found = true;
            if (amFsResolvedVariant(load_data.path) != AMD_FS_VARIANT_CONSOLE)
                return false;
        }

        return found;
    }

    // resolved once per stage beside gmMapSelectVariant, never per frame
    private static void gmDecoSelectVariant(int stage_id)
    {
        gm_deco_as_model = gmDecoIsConsoleDataComplete(stage_id);
    }
}
