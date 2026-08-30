using System;

using static AppMain;

internal static partial class GmClearDemo
{
    // Everything gmClearDemo.cpp puts behind an #ifdef, picked once in gmClearDemoInit. 77
    // conditional blocks in the file, but 47 are cosmetic: 42 of them write scale.x/y = 1.0f *
    // FX32_ONE, which ObjObjectCreate already did. Only these ten change behaviour, and none of
    // them changes the shape of a table, so unlike GMS_FIX_VARIANT nothing here is an array.
    // see docs/cleardemo-port-notes.md
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

        // console re-derives the time-attack Sonic icon every frame where mobile sets it once at
        // action load. Same result while the player is frozen, which they are.
        public bool time_sonic_icon_per_frame;

        public bool is_console;
    }

    private static GMS_CLRDM_VARIANT gm_clrdm_variant = gm_clrdm_variant_mobile;

    // G_RSLT ids are a strict prefix, so the console arms would run on either archive. This gates
    // on the assets anyway, for the same reason every other screen does: the two builds' art is not
    // a shared vocabulary and mixing them is what the whole variant mechanism exists to stop.
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
        return (AoPad.AoPadStand() & (ControllerConsts.CONFIRM
                                      | ControllerConsts.CANCEL
                                      | ControllerConsts.START)) != 0;
    }

    private static bool gmClearDemoIsTimeSkipPressedPad()
    {
        return (AoPad.AoPadStand() & ControllerConsts.CONFIRM) != 0;
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
