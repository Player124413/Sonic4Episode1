using static AoPad;

using static AppMain;

using static GmPauseMenu.GME_PMENU_ACT;
using static GmPauseMenu.GME_PMENU_RESULT;
using static GmPauseMenu.GME_PMENU_SE;
using static GmPauseMenu.GME_PMENU_STATE;

// the console pause menu, gmPauseMenu.cpp, which is wrapped entirely in #if !_IPHONE and was never
// decompiled. AppMain/Gm/GmPauseMenu.cs routes between this and the touch menu the WP7 build had.
// see docs/pause-action-ids.md
internal static partial class GmPauseMenu
{
    private static readonly GMS_PMENU_GLOBAL gm_pmenu_work = new GMS_PMENU_GLOBAL();

    private static GMS_PMENU_GLOBAL gmPmGetGlbWork()
    {
        return gm_pmenu_work;
    }

    // all of the screen's archives or none: the console and mobile builds are different screens
    // here, not different art, and xbla ships six languages against this build's eleven
    public static bool GmPauseMenuIsConsoleDataComplete()
    {
        var lng = GsEnvGetLanguage();
        if (lng < 0 || lng >= gm_pm_file_lng_amb_name_tbl.Length)
            return false;

        return amFsAllVariant(
            AMD_FS_VARIANT_CONSOLE,
            GMD_PMENU_FILE_CMN_AMA,
            GMD_PMENU_FILE_CMN_AMB,
            GMD_PMENU_FILE_LNG_AMA,
            GMD_PMENU_FILE_PLF_AMA,
            GMD_PMENU_FILE_PLF_AMB,
            GMD_PMENU_FILE_PLNG_AMA,
            GMD_PMENU_FILE_WIN_AMB,
            gm_pm_file_lng_amb_name_tbl[lng],
            gm_pm_file_plng_amb_name_tbl[lng]);
    }

    private static AMS_FS gmPmReadRequest(string name)
    {
        return amFsReadBackground(name);
    }

    public static void GmPauseMenuLoadStart()
    {
        var work = gmPmGetGlbWork();
        if (GmPauseMenuLoadIsFinished() || work.state == GMD_PMENU_STATE_LOADING)
            return;

        work.file_cmn_ama = null;
        work.file_cmn_amb = null;
        work.file_lng_ama = null;
        work.file_lng_amb = null;
        work.file_plf_ama = null;
        work.file_plf_amb = null;
        work.file_plng_ama = null;
        work.file_plng_amb = null;
        work.file_win_amb = null;
        work.fs = null;
        work.se_handle = null;

        work.state = GMD_PMENU_STATE_LOADING;

        // the C++ chains nine background reads and steals fs->buf raw. amFs parses what it reads as
        // an archive, which an .AMA is not, so the four action files come in synchronously instead.
        // They are small and this runs behind the stage load.
        work.file_cmn_ama = readAMAFile(GMD_PMENU_FILE_CMN_AMA);
        work.file_lng_ama = readAMAFile(GMD_PMENU_FILE_LNG_AMA);
        work.file_plf_ama = readAMAFile(GMD_PMENU_FILE_PLF_AMA);
        work.file_plng_ama = readAMAFile(GMD_PMENU_FILE_PLNG_AMA);

        work.fs = gmPmReadRequest(GMD_PMENU_FILE_CMN_AMB);

        work.tcb = amTaskMake(gmPmLoadTaskWait, null, 0U, 0U, 0U, "gmPauseMenu::Load");
        amTaskStart(work.tcb);
    }

    public static bool GmPauseMenuLoadIsFinished()
    {
        var state = gmPmGetGlbWork().state;

        return state != GMD_PMENU_STATE_RELEASED && state != GMD_PMENU_STATE_LOADING;
    }

    public static void GmPauseMenuBuildStart()
    {
        var work = gmPmGetGlbWork();
        if (!GmPauseMenuLoadIsFinished() || GmPauseMenuBuildIsFinished())
            return;
        if (work.state == GMD_PMENU_STATE_BUILDING)
            return;

        work.state = GMD_PMENU_STATE_BUILDING;

        AoTexBuild(work.tex_cmn, work.file_cmn_amb);
        AoTexLoad(work.tex_cmn);
        AoTexBuild(work.tex_lng, work.file_lng_amb);
        AoTexLoad(work.tex_lng);
        AoTexBuild(work.tex_plf, work.file_plf_amb);
        AoTexLoad(work.tex_plf);
        AoTexBuild(work.tex_plng, work.file_plng_amb);
        AoTexLoad(work.tex_plng);
        AoTexBuild(work.tex_win, work.file_win_amb);
        AoTexLoad(work.tex_win);

        work.tcb = amTaskMake(gmPmBuildTaskWait, null, 0U, 0U, 0U, "gmPauseMenu::Build");
        amTaskStart(work.tcb);
    }

    public static bool GmPauseMenuBuildIsFinished()
    {
        var state = gmPmGetGlbWork().state;

        return state == GMD_PMENU_STATE_BUILDED || state == GMD_PMENU_STATE_EXECUTE;
    }

    public static void GmPauseMenuFlushStart()
    {
        var work = gmPmGetGlbWork();
        if (!GmPauseMenuBuildIsFinished())
            return;

        if (!GmPauseMenuIsFinished())
            GmPauseMenuCancel();

        if (GmPauseMenuFlushIsFinished() || work.state == GMD_PMENU_STATE_FLUSHING)
            return;

        work.state = GMD_PMENU_STATE_FLUSHING;

        AoTexRelease(work.tex_cmn);
        AoTexRelease(work.tex_lng);
        AoTexRelease(work.tex_plf);
        AoTexRelease(work.tex_plng);
        AoTexRelease(work.tex_win);

        work.tcb = amTaskMake(gmPmFlushTaskWait, null, 0U, 0U, 0U, "gmPauseMenu::Flush");
        amTaskStart(work.tcb);
    }

    public static bool GmPauseMenuFlushIsFinished()
    {
        var state = gmPmGetGlbWork().state;

        return state != GMD_PMENU_STATE_BUILDING
            && state != GMD_PMENU_STATE_BUILDED
            && state != GMD_PMENU_STATE_EXECUTE
            && state != GMD_PMENU_STATE_FLUSHING;
    }

    public static void GmPauseMenuRelease()
    {
        var work = gmPmGetGlbWork();
        if (!GmPauseMenuFlushIsFinished())
            return;

        work.file_cmn_ama = null;
        work.file_cmn_amb = null;
        work.file_lng_ama = null;
        work.file_lng_amb = null;
        work.file_plf_ama = null;
        work.file_plf_amb = null;
        work.file_plng_ama = null;
        work.file_plng_amb = null;
        work.file_win_amb = null;

        work.state = GMD_PMENU_STATE_RELEASED;
        work.tcb = null;
        work.fs = null;
    }

    public static void GmPauseMenuStart(uint prio)
    {
        var gwork = gmPmGetGlbWork();
        if (!GmPauseMenuBuildIsFinished() || !GmPauseMenuIsFinished())
            return;

        gwork.state = GMD_PMENU_STATE_EXECUTE;
        gwork.result = GME_PMENU_RESULT_NONE;
        gwork.se_handle = GsSoundAllocSeHandle();

        gwork.tcb = amTaskMake(
            gmPmTask00, gmPmTaskDestructor, prio, 0U, 0U, "gmPauseMenu::Execute");

        gwork.tcb.work = new GMS_PMENU_TASK_WORK
        {
            work = new GMS_PMENU_WORK(),
            proc_count = 0U,
            proc = gmPmProcBegin,
        };

        amTaskStart(gwork.tcb);
        gmPmSndPlaySe(GMD_PMENU_SE_START);
    }

    public static void GmPauseMenuCancel()
    {
        var work = gmPmGetGlbWork();
        if (GmPauseMenuIsFinished())
            return;

        amTaskDelete(work.tcb);
        work.tcb = null;
        work.result = GME_PMENU_RESULT_CANCEL;
    }

    public static bool GmPauseMenuIsFinished()
    {
        return gmPmGetGlbWork().state != GMD_PMENU_STATE_EXECUTE;
    }

    public static GME_PMENU_RESULT GmPauseMenuGetResult()
    {
        return gmPmGetGlbWork().result;
    }

    // ---- resource wait tasks

    private static void gmPmLoadTaskWait(AMS_TCB tcb)
    {
        var work = gmPmGetGlbWork();
        if (!amFsIsComplete(work.fs))
            return;

        var amb = readAMBFile(work.fs);
        amFsClearRequest(work.fs);
        work.fs = null;

        if (work.file_cmn_amb == null)
        {
            work.file_cmn_amb = amb;
            work.fs = gmPmReadRequest(gm_pm_file_lng_amb_name_tbl[GsEnvGetLanguage()]);
        }
        else if (work.file_lng_amb == null)
        {
            work.file_lng_amb = amb;
            work.fs = gmPmReadRequest(GMD_PMENU_FILE_PLF_AMB);
        }
        else if (work.file_plf_amb == null)
        {
            work.file_plf_amb = amb;
            work.fs = gmPmReadRequest(gm_pm_file_plng_amb_name_tbl[GsEnvGetLanguage()]);
        }
        else if (work.file_plng_amb == null)
        {
            work.file_plng_amb = amb;
            work.fs = gmPmReadRequest(GMD_PMENU_FILE_WIN_AMB);
        }
        else
        {
            work.file_win_amb = amb;
            amTaskDelete(tcb);
            work.state = GMD_PMENU_STATE_LOADED;
            work.tcb = null;
        }
    }

    private static void gmPmBuildTaskWait(AMS_TCB tcb)
    {
        var work = gmPmGetGlbWork();
        if (!AoTexIsLoaded(work.tex_cmn) || !AoTexIsLoaded(work.tex_lng)
            || !AoTexIsLoaded(work.tex_plf) || !AoTexIsLoaded(work.tex_plng)
            || !AoTexIsLoaded(work.tex_win))
        {
            return;
        }

        amTaskDelete(tcb);
        work.state = GMD_PMENU_STATE_BUILDED;
        work.tcb = null;
    }

    // back to LOADED, not RELEASED: the textures go, the files stay until GmPauseMenuRelease
    private static void gmPmFlushTaskWait(AMS_TCB tcb)
    {
        var work = gmPmGetGlbWork();
        if (!AoTexIsReleased(work.tex_cmn) || !AoTexIsReleased(work.tex_lng)
            || !AoTexIsReleased(work.tex_plf) || !AoTexIsReleased(work.tex_plng)
            || !AoTexIsReleased(work.tex_win))
        {
            return;
        }

        amTaskDelete(tcb);
        work.state = GMD_PMENU_STATE_LOADED;
        work.tcb = null;
    }

    // ---- the execute task

    private static GMS_PMENU_TASK_WORK gmPmGetTaskWork()
    {
        return (GMS_PMENU_TASK_WORK)amTaskGetWork(gmPmGetGlbWork().tcb);
    }

    private static uint gmPmGetProcCount()
    {
        return gmPmGetTaskWork().proc_count;
    }

    private static void gmPmSetProc(GMS_PMENU_PROC proc)
    {
        gmPmGetTaskWork().proc = proc;
    }

    // the count resets whenever the proc changes, so a proc's first frame always sees zero. Every
    // one of the nineteen procs relies on that.
    private static void gmPmTask00(AMS_TCB tcb)
    {
        var twork = (GMS_PMENU_TASK_WORK)amTaskGetWork(tcb);
        if (twork.proc == null)
        {
            amTaskSetProcedure(tcb, gmPmTask01);
            return;
        }

        var prev_proc = twork.proc;
        twork.proc(twork.work);

        if (twork.proc_count < uint.MaxValue)
            twork.proc_count += 1;

        if (twork.proc != prev_proc)
            twork.proc_count = 0;
    }

    private static void gmPmTask01(AMS_TCB tcb)
    {
        amTaskDelete(tcb);
    }

    private static void gmPmTaskDestructor(AMS_TCB tcb)
    {
        var gwork = gmPmGetGlbWork();
        var twork = (GMS_PMENU_TASK_WORK)amTaskGetWork(tcb);

        if (gwork.se_handle != null)
        {
            GsSoundFreeSeHandle(gwork.se_handle);
            gwork.se_handle = null;
        }

        if (twork.work != null)
        {
            for (int i = 0; i < (int)ACT_NUM; ++i)
            {
                if (twork.work.act[i] != null)
                {
                    AoActDelete(twork.work.act[i]);
                    twork.work.act[i] = null;
                }
            }

            twork.work = null;
        }

        gwork.state = GMD_PMENU_STATE_BUILDED;
        gwork.tcb = null;
    }

    // ---- util

    private static void gmPmUtilSetTexture(GME_PMENU_ACT act_no)
    {
        var gwork = gmPmGetGlbWork();

        AOS_TEXTURE tex;
        if (act_no >= ACT_PLNG_S)
            tex = gwork.tex_plng;
        else if (act_no >= ACT_PLF_S)
            tex = gwork.tex_plf;
        else if (act_no >= ACT_LNG_S)
            tex = gwork.tex_lng;
        else
            tex = gwork.tex_cmn;

        AoActSetTexture(AoTexGetTexList(tex));
    }

    private static readonly string[] gm_pm_se_name_tbl = ["Cursol", "Ok", "Cancel", "Pause"];

    private static void gmPmSndPlaySe(GME_PMENU_SE se)
    {
        if (se == GMD_PMENU_SE_START && AoSysIsShowPlatformUI())
            return;

        GmSoundPlaySE(gm_pm_se_name_tbl[(int)se], gmPmGetGlbWork().se_handle);
    }

    // ---- pad

    private static BUTTON gmPmPadStand()
    {
        return AoPadStand();
    }

    private static BUTTON gmPmPadMStand()
    {
        return AoPadMStand();
    }

    private static BUTTON gmPmPadMRepeat()
    {
        return AoPadMRepeat();
    }

    // ---- what the menu asks about the game

    private static bool gmPmGlbIsBackEnable()
    {
        return GsMainSysIsStageClear(GSD_MAIN_STAGE_ID_1_1);
    }

    private static bool gmPmGlbIsSpecialStage()
    {
        var id = GsGetMainSysInfo().stage_id;

        return id >= GSD_MAIN_STAGE_ID_SS1 && id <= GSD_MAIN_STAGE_ID_SS7;
    }
}
