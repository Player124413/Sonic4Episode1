using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Viewer;

// A stage is a single index into gm_gamedat_tbl_map_info_tbl, which is what g_gs_main_sys_info.stage_id
// holds and what GmGameDatLoadInit is handed. Nothing here builds an archive name by formatting a
// zone and act together: the game's own table is the only place those names live.
internal static class StageTable
{
    public const int MapArchiveCount = 4;

    public static int Count => AppMain.gm_gamedat_tbl_map_info_tbl.Length;

    // the entries gmGameDatLoadProcPostMap claims, in data_no order: map, models, textures, attr
    public static string[] MapPaths(int stageId)
    {
        var info = AppMain.gm_gamedat_tbl_map_info_tbl[stageId];
        var paths = new List<string>();

        for (int i = 0; i < info.num && paths.Count < MapArchiveCount; i++)
        {
            if (info.data_tbl[i].proc_post == AppMain.gmGameDatLoadProcPostMap)
                paths.Add(info.data_tbl[i].path);
        }

        return paths.ToArray();
    }

    // the background, one entry per stage right after the four map archives
    public static string MapFarPath(int stageId)
    {
        var info = AppMain.gm_gamedat_tbl_map_info_tbl[stageId];

        for (int i = 0; i < info.num; i++)
        {
            if (info.data_tbl[i].proc_post == AppMain.gmGameDatLoadProcPostMapFar)
                return info.data_tbl[i].path;
        }

        return null;
    }

    // the decorations, one entry per stage. the special stages and the stage 19 placeholder have none
    public static string DecoPath(int stageId)
    {
        var info = AppMain.gm_gamedat_tbl_map_info_tbl[stageId];

        for (int i = 0; i < info.num; i++)
        {
            if (info.data_tbl[i].proc_post == AppMain.gmGameDatLoadProcPostDeco)
                return info.data_tbl[i].path;
        }

        return null;
    }

    public static int ZoneType(int stageId)
    {
        return AppMain.g_gm_gamedat_zone_type_tbl[stageId];
    }

    // how many of the five parallax layers this zone type draws
    public static int AddLayerCount(int stageId)
    {
        return AppMain.gm_map_add_tbl_use_no[ZoneType(stageId)] + 1;
    }

    // named after the archive it loads, so the name is the game's rather than ours
    public static string Name(int stageId)
    {
        var paths = MapPaths(stageId);
        if (paths.Length == 0)
            return null;

        var file = Path.GetFileName(paths[0]);
        return file.EndsWith("_MAP.AMB", StringComparison.OrdinalIgnoreCase)
            ? file.Substring(0, file.Length - "_MAP.AMB".Length)
            : null;
    }

    // stage 19 is the boss battle placeholder, one /bb/gmstage.bb entry and no block map at all
    public static bool HasBlockMap(int stageId)
    {
        return Name(stageId) != null && MapPaths(stageId).Length == MapArchiveCount;
    }

    public static bool TryResolve(string arg, out int stageId)
    {
        if (int.TryParse(arg, out stageId))
            return stageId >= 0 && stageId < Count;

        for (stageId = 0; stageId < Count; stageId++)
        {
            if (string.Equals(Name(stageId), arg, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        stageId = -1;
        return false;
    }

    public static IEnumerable<string> Describe()
    {
        for (int i = 0; i < Count; i++)
        {
            var name = Name(i);
            yield return name == null
                ? $"{i,3}  (no block map, loads {string.Join(", ", MapPaths(i).Select(Path.GetFileName))})"
                : $"{i,3}  {name,-10} zone type {ZoneType(i)}, {AddLayerCount(i)} parallax layer(s)";
        }
    }
}
