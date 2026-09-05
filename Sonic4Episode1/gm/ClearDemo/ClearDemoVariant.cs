using System;

using static AppMain;

internal static partial class GmClearDemo
{
    // picked once in gmClearDemoInit; unlike GMS_FIX_VARIANT nothing here needs to be an array
    internal sealed class GMS_CLRDM_VARIANT
    {
        // four sites take decide, cancel or start; the one on the time-attack record display takes
        // decide only, which is SEGA's and not an oversight
        public Func<bool> is_skip_pressed;
        public Func<bool> is_time_skip_pressed;

        // a cursor over two labels, or two nine-slice touch buttons
        public GMS_CLRDM_MAIN_WORK._WorkDelegate proc_retry_input;
        public bool has_touch_buttons;
        public float retry_default_frame;

        public bool hides_clear_time_on_retry;
        public bool clears_nn_command_state_tbl;
        public bool sets_bg_color_black;

        // console re-derives the time-attack sonic icon every frame, mobile sets it once at load;
        // same result since the player is frozen
        public bool time_sonic_icon_per_frame;

        public bool is_console;
    }

    private static GMS_CLRDM_VARIANT gm_clrdm_variant = gm_clrdm_variant_mobile;

    // ids are a strict prefix, so this checks the actual assets instead: the two builds' art isn't
    // a shared vocabulary, and that's what the variant mechanism exists to stop
    private static bool gmClearDemoIsConsoleDataComplete()
    {
        var lng = gmClearDemoLanguage();

        return amFsAllMemberVariant(
            AMD_FS_VARIANT_CONSOLE,
            GmFix.GMD_FIX_DATA_FILENAME,
            IDB_CPIT_MAIN_G_RSLT_AMA,
            IDB_CPIT_MAIN_G_RSLT_AMB,
            g_gm_clear_demo_data_ama_id[lng],
            g_gm_clear_demo_data_amb_id[lng]);
    }

    private static bool gmClearDemoIsSkipPressedPad()
    {
        return (AoPad.AoPadStand() & (BUTTON.CONFIRM
                                      | BUTTON.CANCEL
                                      | BUTTON.START)) != 0;
    }

    private static bool gmClearDemoIsTimeSkipPressedPad()
    {
        return (AoPad.AoPadStand() & BUTTON.CONFIRM) != 0;
    }

    // the WP7 back key rides along with the touch check on this branch, which the C++ does not have
    private static bool gmClearDemoIsSkipPressedTouch()
    {
        return amTpIsTouchPush(0) || isBackKeyPressed();
    }

    private static bool gmClearDemoIsTimeSkipPressedTouch()
    {
        return amTpIsTouchPush(0);
    }

    internal static readonly GMS_CLRDM_VARIANT gm_clrdm_variant_mobile = new GMS_CLRDM_VARIANT
    {
        is_skip_pressed = gmClearDemoIsSkipPressedTouch,
        is_time_skip_pressed = gmClearDemoIsTimeSkipPressedTouch,
        proc_retry_input = gmClearDemoSetRetryInputTouch,
        has_touch_buttons = true,
        retry_default_frame = 0.0f,
        hides_clear_time_on_retry = true,
        clears_nn_command_state_tbl = true,
        sets_bg_color_black = true,
        time_sonic_icon_per_frame = false,
        is_console = false,
    };

    internal static readonly GMS_CLRDM_VARIANT gm_clrdm_variant_console = new GMS_CLRDM_VARIANT
    {
        is_skip_pressed = gmClearDemoIsSkipPressedPad,
        is_time_skip_pressed = gmClearDemoIsTimeSkipPressedPad,
        proc_retry_input = gmClearDemoSetRetryInputPad,
        has_touch_buttons = false,
        retry_default_frame = 1.0f,
        hides_clear_time_on_retry = false,
        clears_nn_command_state_tbl = false,
        sets_bg_color_black = false,
        time_sonic_icon_per_frame = true,
        is_console = true,
    };
}
