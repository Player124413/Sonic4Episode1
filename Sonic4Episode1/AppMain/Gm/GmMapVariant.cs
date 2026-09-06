using System;
using System.Linq;

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

    // identical to console in every engine respect - the pc tier is console-grade art and takes the
    // same block-model arm. It exists only to pin the stage's loads to the pc tier, because
    // GmGameData.cs:374 loads map archives as gm_map_variant.fs_variant. Pinning "console" for a
    // stage whose archives are declared pc silently falls back to the base archive, and the base is
    // #TVX, which the NN model loader rejects as a bad header
    internal static readonly GMS_MAP_VARIANT gm_map_variant_pc = new GMS_MAP_VARIANT
    {
        blocks_as_model = true,

        registlist_num = 128,

        fs_variant = AMD_FS_VARIANT_PC,
    };

    // resolved once per stage in GmGameDatLoadInit, never per frame
    private static GMS_MAP_VARIANT gm_map_variant = gm_map_variant_mobile;

    internal static GMS_MAP_VARIANT MapVariantForTest(bool console)
    {
        return console ? gm_map_variant_console : gm_map_variant_mobile;
    }

    // resolved once per stage by gmMapSelectVariant. True when this stage's levels are the PC-era
    // ones rather than the mobile-era layouts both mobile and xbla ship, which is what decides the
    // things that differ per LEVEL rather than per asset kind
    internal static bool gmStageIsPCLevel()
    {
        return gm_map_variant == gm_map_variant_pc;
    }

    // The two map motion sidecars are the console arm's: the leak guards their table rows with
    // `#if !defined GMD_GAMEDAT_NO_CREATE_MAP_MOTION` and defines that macro, so the mobile build
    // never had them. Console and pc are both console-grade art and both take them.
    //
    // Asked of the resolved variant rather than of the stage, because gmMapSelectVariant has
    // already run by the time the load list is built, and because the extra rows must not feed
    // back into the all-or-none tier check that decided it
    internal static bool gmMapHasMotionData()
    {
        return gm_map_variant != gm_map_variant_mobile;
    }

    // The sidecars sit at GMD_GAMEDAT_MAP_MTN and _MMTN, which is positions 4 and 5, because
    // gmGameDatLoadProcPostMap indexes g_gm_gamedat_map by the row's position in the table. Every
    // row after them shifts, which is harmless: MAPFAR and DECO carry their own post procs and
    // never look at the position
    internal static GMS_GAMEDAT_LOAD_DATA[] gmMapTableWithMotion(
      GMS_GAMEDAT_LOAD_DATA[] table,
      string mtn_path,
      string mmtn_path)
    {
        GMS_GAMEDAT_LOAD_DATA[] motion =
        {
            new GMS_GAMEDAT_LOAD_DATA(mtn_path, gmGameDatLoadAllocHeadSub, null, gmGameDatLoadProcPostMap, -1),
            new GMS_GAMEDAT_LOAD_DATA(mmtn_path, gmGameDatLoadAllocHeadSub, null, gmGameDatLoadProcPostMap, -1),
        };

        return table.Take(GMD_GAMEDAT_MAP_MTN)
            .Concat(motion)
            .Concat(table.Skip(GMD_GAMEDAT_MAP_MTN))
            .ToArray();
    }

    // zone 4 loads zone 1's map far archive, so following the global variant would hand it a console
    // background against mobile blocks. these load pinned to what the stage actually resolved to
    internal static bool gmMapOwnsLoad(GMS_GAMEDAT_LOAD_DATA load_data)
    {
        return load_data.proc_post == gmGameDatLoadProcPostMap
            || load_data.proc_post == gmGameDatLoadProcPostMapFar;
    }

    // every map and map far archive the stage loads, named off the load table rather than spelled
    // out, so a stage cannot end up with console blocks against a mobile background.
    // Console-grade rather than exactly "console": the pc tier is console-grade too, and this gate
    // only decides the engine arm (block models vs TVX), which both tiers share
    internal static bool gmMapIsConsoleDataComplete(int stage_id)
    {
        return gmMapAllMapArchives(stage_id, amFsIsConsoleGrade);
    }

    // the narrower question: is every one of this stage's map archives the PC tier, meaning we have
    // the PC-era LEVEL and not just console-grade art. Drives the things that differ per level
    // rather than per asset kind - Z2A2's card gimmicks, its loadout, no pinball, no start message.
    // All-or-none for the same reason as above: a PC layout against xbla block models is silently
    // the wrong blocks, not an error
    internal static bool gmMapIsPcLevelDataComplete(int stage_id)
    {
        return gmMapAllMapArchives(stage_id,
            v => string.Equals(v, AMD_FS_VARIANT_PC, StringComparison.OrdinalIgnoreCase));
    }

    private static bool gmMapAllMapArchives(int stage_id, Func<string, bool> accept)
    {
        var info = gm_gamedat_tbl_map_info_tbl[stage_id];
        var found = false;

        for (int index = 0; index < info.num; ++index)
        {
            var load_data = info.data_tbl[index];
            if (!gmMapOwnsLoad(load_data))
                continue;

            found = true;
            if (!accept(amFsResolvedVariant(load_data.path)))
                return false;
        }

        return found;
    }

    // map and map far are one group, so they move together
    private static void gmMapSelectVariant(int stage_id)
    {
        // three tiers, best first. pc before console: where both exist the PC-era level wins
        bool console = gmMapIsConsoleDataComplete(stage_id);

        gm_map_variant = gmMapIsPcLevelDataComplete(stage_id) ? gm_map_variant_pc
            : console ? gm_map_variant_console
            : gm_map_variant_mobile;

        gm_map_far_variant = console ? gmMapFarVariantConsole() : gmMapFarVariantMobile();
    }
}
