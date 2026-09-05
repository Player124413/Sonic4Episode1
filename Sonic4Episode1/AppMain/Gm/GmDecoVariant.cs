using System;

internal partial class AppMain
{
    // Decorations are a second genuine fork, not part of the map's. Console draws one NN model per
    // decoration; mobile draws baked TVX sprites off a single atlas and has no node motion at all.
    // They ship as their own amFsAllVariant group so a missing sibling on either side cannot drag
    // the other back, which is what MapVariantTests already asserts.
    private static bool gm_deco_as_model;

    // The narrower question, the deco archive's own tier rather than the map's. PC reordered
    // DECO_ZONE1's members and rewrote the sunflower, so a handful of id-indexed table rows differ.
    // Not gmStageIsPcLevel(): that reads gm_map_variant, and zone 1's maps stay on console.
    internal static bool gm_deco_as_pc;

    // named off the stage's own load table, the same way gmMapIsConsoleDataComplete is. There is
    // only ever one deco entry per stage, so this is all-or-none by construction rather than by
    // agreement, but the shape matches its sibling on purpose
    internal static bool gmDecoIsConsoleDataComplete(int stage_id)
    {
        // console-grade, not exactly "console": the pc tier is console-grade too and this only
        // decides the model-vs-TVX arm
        return gmDecoAllDecoArchives(stage_id, amFsIsConsoleGrade);
    }

    internal static bool gmDecoIsPcDataComplete(int stage_id)
    {
        return gmDecoAllDecoArchives(stage_id,
            v => string.Equals(v, AMD_FS_VARIANT_PC, StringComparison.OrdinalIgnoreCase));
    }

    private static bool gmDecoAllDecoArchives(int stage_id, Func<string, bool> accept)
    {
        var info = gm_gamedat_tbl_map_info_tbl[stage_id];
        var found = false;

        for (int index = 0; index < info.num; ++index)
        {
            var load_data = info.data_tbl[index];
            if (load_data.proc_post != gmGameDatLoadProcPostDeco)
                continue;

            found = true;
            if (!accept(amFsResolvedVariant(load_data.path)))
                return false;
        }

        return found;
    }

    // resolved once per stage beside gmMapSelectVariant, never per frame
    private static void gmDecoSelectVariant(int stage_id)
    {
        gm_deco_as_model = gmDecoIsConsoleDataComplete(stage_id);
        gm_deco_as_pc = gmDecoIsPcDataComplete(stage_id);
    }
}
