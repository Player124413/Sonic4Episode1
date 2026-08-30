using static AppMain;

using static StgSlct.DME_STGSLCT_DATA_TYPE;

internal static partial class StgSlct
{
    private static void dmStgSlctLoadFontData(DMS_STGSLCT_MAIN_WORK main_work)
    {
        GsFontBuild();

        main_work.proc_menu_update = dmStgSlctIsLoadFontData;
    }

    private static void dmStgSlctIsLoadFontData(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (GsFontIsBuilded())
            main_work.proc_menu_update = dmStgSlctLoadRequest;
    }

    private static void dmStgSlctLoadRequest(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var variant = dm_stgslct_variant.fs_variant;
        var lng = GsEnvGetLanguage();

        main_work.arc_amb_fs[0] = amFsReadBackgroundAs(DMD_STGSLCT_DATA_FILENAME, variant);
        main_work.arc_amb_fs[1] = amFsReadBackgroundAs(dm_stgslct_main_lng_amb_name_tbl[lng], variant);

        for (int i = 0; i < 4; i++)
            main_work.arc_cmn_amb_fs[i] = amFsReadBackgroundAs(dm_stgslct_menu_cmn_amb_name_tbl[i], variant);

        main_work.arc_cmn_amb_fs[4] = amFsReadBackgroundAs(dm_stgslct_menu_cmn_lng_amb_name_tbl[lng], variant);

        main_work.proc_menu_update = dmStgSlctProcLoadWait;
    }

    private static void dmStgSlctProcLoadWait(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (!dmStgSlctIsDataLoad(main_work))
            return;

        for (int i = 0; i < 2; i++)
        {
            dmStgSlctBindArchive(
                main_work.arc_amb_fs, main_work.arc_amb, main_work.ama, main_work.amb, main_work.tex, i);

            main_work.arc_amb_fs[i] = null;
        }

        GsFontBuild();
        DmSndBgmPlayerInit();

        main_work.proc_menu_update = dmStgSlctProcLoadWait2;
    }

    private static void dmStgSlctProcLoadWait2(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (!dmStgSlctIsTexLoad(main_work) || !DmSndBgmPlayerIsSndSysBuild())
            return;

        for (int i = 0; i < 5; i++)
        {
            dmStgSlctBindArchive(
                main_work.arc_cmn_amb_fs, main_work.arc_cmn_amb, main_work.cmn_ama, main_work.cmn_amb,
                main_work.cmn_tex, i);

            main_work.arc_cmn_amb_fs[i] = null;
        }

        main_work.proc_menu_update = dmStgSlctProcTexBuildWait;
    }

    private static void dmStgSlctBindArchive(
        AMS_FS[] request,
        AMS_AMB_HEADER[] archive,
        A2S_AMA_HEADER[] ama,
        AMS_AMB_HEADER[] amb,
        AOS_TEXTURE[] tex,
        int index)
    {
        archive[index] = readAMBFile(request[index]);

        ama[index] = readAMAFile(amBindGet(archive[index], 0));
        amb[index] = readAMBFile(amBindGet(archive[index], 1, out var dir));
        amb[index].dir = dir;

        amFsClearRequest(request[index]);

        AoTexBuild(tex[index], amb[index]);
        AoTexLoad(tex[index]);
    }

    private static void dmStgSlctProcTexBuildWait(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (!dmStgSlctIsTexLoad2(main_work) || !DmSndBgmPlayerIsSndSysBuild())
            return;

        main_work.proc_menu_update = dmStgSlctProcCheckLoadingEnd;

        DmSaveMenuStart(true, false);
    }

    private static void dmStgSlctProcCheckLoadingEnd(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (!DmSaveIsExit())
            return;

        main_work.proc_menu_update = dmStgSlctProcCreateAct;

        DmSndBgmPlayerPlayBgm(DME_SND_BGM_PLAYER_IDX_MENU);

        main_work.flag |= DMD_STGSLCT_FLAG_SIGN_OUT_EXIT;
    }

    private static bool dmStgSlctIsDataLoad(DMS_STGSLCT_MAIN_WORK main_work)
    {
        for (int i = 0; i < 2; i++)
        {
            if (!amFsIsComplete(main_work.arc_amb_fs[i]))
                return false;
        }

        for (int i = 0; i < 5; i++)
        {
            if (!amFsIsComplete(main_work.arc_cmn_amb_fs[i]))
                return false;
        }

        return true;
    }

    private static bool dmStgSlctIsTexLoad(DMS_STGSLCT_MAIN_WORK main_work)
    {
        for (int i = 0; i < (int)DME_STGSLCT_DATA_TYPE_MAX; i++)
        {
            if (!AoTexIsLoaded(main_work.tex[i]))
                return false;
        }

        return GsFontIsBuilded();
    }

    private static bool dmStgSlctIsTexLoad2(DMS_STGSLCT_MAIN_WORK main_work)
    {
        for (int i = 0; i < 5; i++)
        {
            if (!AoTexIsLoaded(main_work.cmn_tex[i]))
                return false;
        }

        return true;
    }

    private static bool dmStgSlctIsTexRelease(DMS_STGSLCT_MAIN_WORK main_work)
    {
        for (int i = 0; i < (int)DME_STGSLCT_DATA_TYPE_MAX; i++)
        {
            if (!AoTexIsReleased(main_work.tex[i]))
                return false;
        }

        for (int i = 0; i < 5; i++)
        {
            if (!AoTexIsReleased(main_work.cmn_tex[i]))
                return false;
        }

        return true;
    }
}
