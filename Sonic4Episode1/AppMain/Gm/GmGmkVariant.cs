internal partial class AppMain
{
    // The gimmick archives are member for member identical between the two builds, which is why
    // they ship as content alone with no format fork. The gimmick *modules* are a different matter:
    // they carry _IPHONE forks about lighting and about how a thing is drawn, and those are only
    // correct for the models that are actually loaded. See docs/console-gimmicks.md.
    private static bool gm_gmk_as_console;

    // named off the stage's own gimmick table. The G_COM gimmicks are deliberately not part of any
    // console group, because gm_gamedat_tbl_gimmick_common loads them for every stage in the game,
    // so only the per-stage ones are asked about here
    internal static bool gmGmkIsConsoleDataComplete(int stage_id)
    {
        var info = gm_gamedat_tbl_gimmick_info_tbl[stage_id];
        var found = false;

        for (int index = 0; index < info.num; ++index)
        {
            var path = info.data_tbl[index].path;
            if (path == null || path.StartsWith("G_COM/"))
                continue;

            found = true;
            // console-grade, not exactly "console": the pc tier is console-grade too and this only
            // decides the model-vs-TVX arm
            if (!amFsIsConsoleGrade(amFsResolvedVariant(path)))
                return false;
        }

        return found;
    }

    private static void gmGmkSelectVariant(int stage_id)
    {
        gm_gmk_as_console = gmGmkIsConsoleDataComplete(stage_id);
    }
}
