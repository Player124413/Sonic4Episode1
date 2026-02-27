internal partial class AppMain
{

    private static void GsFontInit()
    {
    }

    private static void GsFontExit()
    {
    }

    public static void GsFontBuild()
    {
        GsFontBuild(true);
    }

    public static void GsFontBuild(bool use_mem2)
    {
        g_gs_font_builded = true;
    }

    public static bool GsFontIsBuilding()
    {
        return false;
    }

    public static bool GsFontIsBuilded()
    {
        return g_gs_font_builded;
    }

    public static void GsFontRelease()
    {
        g_gs_font_builded = false;
    }
}