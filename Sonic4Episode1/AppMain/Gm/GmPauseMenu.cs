internal partial class AppMain
{
    // two pause menus: the console one in gm/PauseMenu (gmPauseMenu.cpp, a cursor menu) and the
    // touch one the WP7 build shipped (gmPauseMenu_i.cpp, decompiled as CMain_PauseMenu). They have
    // the same eleven functions, so the only thing that has to agree is which one answered
    // GmPauseMenuLoadStart. see docs/pause-action-ids.md
    private static bool gm_pause_menu_console;

    public static void GmPauseMenuLoadStart()
    {
        gm_pause_menu_console = GmPauseMenu.GmPauseMenuIsConsoleDataComplete();

        if (gm_pause_menu_console)
        {
            GmPauseMenu.GmPauseMenuLoadStart();
            return;
        }

        CMain_PauseMenu instance = CMain_PauseMenu.CreateInstance();
        instance.Create();
        instance.LoadFile();
    }

    public static bool GmPauseMenuLoadIsFinished()
    {
        return gm_pause_menu_console
            ? GmPauseMenu.GmPauseMenuLoadIsFinished()
            : CMain_PauseMenu.CreateInstance().IsLoadFile();
    }

    public static void GmPauseMenuBuildStart()
    {
        if (gm_pause_menu_console)
            GmPauseMenu.GmPauseMenuBuildStart();
        else
            CMain_PauseMenu.CreateInstance().CreateTexture();
    }

    public static bool GmPauseMenuBuildIsFinished()
    {
        return gm_pause_menu_console
            ? GmPauseMenu.GmPauseMenuBuildIsFinished()
            : CMain_PauseMenu.CreateInstance().IsCreatedTexture();
    }

    public static void GmPauseMenuFlushStart()
    {
        if (gm_pause_menu_console)
            GmPauseMenu.GmPauseMenuFlushStart();
        else
            CMain_PauseMenu.CreateInstance().ReleaseTexture();
    }

    public static bool GmPauseMenuFlushIsFinished()
    {
        return gm_pause_menu_console
            ? GmPauseMenu.GmPauseMenuFlushIsFinished()
            : CMain_PauseMenu.CreateInstance().IsReleasedTexture();
    }

    public static void GmPauseMenuRelease()
    {
        if (gm_pause_menu_console)
            GmPauseMenu.GmPauseMenuRelease();
        else
            CMain_PauseMenu.CreateInstance().Release();
    }

    public static void GmPauseMenuStart(uint prio)
    {
        if (gm_pause_menu_console)
            GmPauseMenu.GmPauseMenuStart(prio);
        else
            CMain_PauseMenu.CreateInstance().Start(prio);
    }

    public static void GmPauseMenuCancel()
    {
        if (gm_pause_menu_console)
            GmPauseMenu.GmPauseMenuCancel();
        else
            CMain_PauseMenu.CreateInstance().Cancel();
    }

    public static bool GmPauseMenuIsFinished()
    {
        return gm_pause_menu_console
            ? GmPauseMenu.GmPauseMenuIsFinished()
            : CMain_PauseMenu.CreateInstance().IsPlay();
    }

    // CMain_PauseMenu keeps the mobile numbering internally because c_return_table and
    // local_create_action_table are indexed by it; only the boundary is named
    private static readonly GmPauseMenu.GME_PMENU_RESULT[] gm_pause_menu_mobile_result =
    {
        GmPauseMenu.GME_PMENU_RESULT.GME_PMENU_RESULT_RETRY,
        GmPauseMenu.GME_PMENU_RESULT.GME_PMENU_RESULT_OPTION,
        GmPauseMenu.GME_PMENU_RESULT.GME_PMENU_RESULT_BACK,
        GmPauseMenu.GME_PMENU_RESULT.GME_PMENU_RESULT_MAINMENU,
        GmPauseMenu.GME_PMENU_RESULT.GME_PMENU_RESULT_CANCEL,
    };

    public static GmPauseMenu.GME_PMENU_RESULT GmPauseMenuGetResult()
    {
        if (gm_pause_menu_console)
            return GmPauseMenu.GmPauseMenuGetResult();

        var result = CMain_PauseMenu.CreateInstance().GetResult();

        return result >= 0 && result < gm_pause_menu_mobile_result.Length
            ? gm_pause_menu_mobile_result[result]
            : GmPauseMenu.GME_PMENU_RESULT.GME_PMENU_RESULT_NONE;
    }
}
