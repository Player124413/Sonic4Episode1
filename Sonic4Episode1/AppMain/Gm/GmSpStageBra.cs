internal partial class AppMain
{
    // gmSpStageBra.cpp, not gmSplStage.cpp: the event that decides which special stage to enter.
    // Registered in the event table, which is why it stays on AppMain.
    public static void GmSpStageBranchInit(object arg)
    {
        UNREFERENCED_PARAMETER(arg);
        if (SyGetEvtInfo().old_evt_id == 5)
        {
            if (g_gs_main_sys_info.stage_id < 21 || g_gs_main_sys_info.stage_id > 27)
                g_gs_main_sys_info.stage_id = 21;
        }
        else
            g_gs_main_sys_info.stage_id = GsMainSysIsStageClear(21) ? (GsMainSysIsStageClear(22) ? (GsMainSysIsStageClear(23) ? (GsMainSysIsStageClear(24) ? (GsMainSysIsStageClear(25) ? (GsMainSysIsStageClear(26) ? (ushort)27 : (ushort)26) : (ushort)25) : (ushort)24) : (ushort)23) : (ushort)22) : (ushort)21;
        g_gs_main_sys_info.char_id[0] = 2;
        g_gs_main_sys_info.game_flag |= 128U;
        GmMainGSInit();
        SyChangeNextEvt();
    }
}
