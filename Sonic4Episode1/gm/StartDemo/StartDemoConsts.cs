internal static partial class GmStartDemo
{
    public const int GMD_START_DEMO_INVLID_ID = -1;
    public const int GMD_START_DEMO_ACT_NO_NUM = 4;
    public const float GMD_START_DEMO_FADEIN_TIME = 30.0f;

    public const uint GMD_START_DEMO_FLAG_EXIT = 1 << 0;

    // load once this many display list slots are free
    public const int GMD_START_DEMO_BUILD_REG_ALLOWANCE_NUM = 64;

    public const uint GMD_START_DEMO_COUNTER_IN_END = 39;
    public const uint GMD_START_DEMO_COUNTER_WAIT_END = 160;
    public const uint GMD_START_DEMO_COUNTER_OUT_END = 230;

    // where the in-animation is parked once it has played
    public const float GMD_START_DEMO_FRAME_HELD = 40.0f;

    // shared archive: also used by the HUD, results, game over and special stage screens
    public const string GMD_START_DEMO_DATA_FILENAME = "G_COM/CPIT/CPIT_MAIN.AMB";

    // mobile pushes every part one unit forward so the card clears the 3D scene; console does not
    public const float GMD_START_DEMO_MOBILE_POS_Z = 10.0f;
}
