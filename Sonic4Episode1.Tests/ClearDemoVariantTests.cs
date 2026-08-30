using System.Linq;
using System.Reflection;

namespace Sonic4Episode1.Tests;

// G_RSLT is the one CPIT_MAIN archive whose ids are a strict prefix rather than a divergence:
// 0-99 identical between builds, iPhone appending 100-105 for the six touch-button pieces. So
// unlike GMS_FIX_VARIANT nothing here swaps a table, and what the descriptor carries is behaviour.
// see docs/cleardemo-port-notes.md
public class ClearDemoVariantTests
{
    private static GmClearDemo.GMS_CLRDM_VARIANT Mobile => GmClearDemo.gm_clrdm_variant_mobile;
    private static GmClearDemo.GMS_CLRDM_VARIANT Console => GmClearDemo.gm_clrdm_variant_console;

    // the console G_RSLT header stops at 99
    private const int ConsoleActNum = 100;

    [Fact]
    public void EveryFieldIsPopulatedOnBothArms()
    {
        foreach (var variant in new[] { Mobile, Console })
        {
            foreach (var field in typeof(GmClearDemo.GMS_CLRDM_VARIANT).GetFields(
                         BindingFlags.Public | BindingFlags.Instance))
            {
                Assert.True(
                    field.GetValue(variant) != null,
                    $"{field.Name} is null on {(variant.is_console ? "console" : "mobile")}");
            }
        }
    }

    [Fact]
    public void TheTwoArmsDisagreeAboutBeingConsole()
    {
        Assert.True(Console.is_console);
        Assert.False(Mobile.is_console);
    }

    // the touch buttons are actions 100-105, which is exactly what the console archive does not
    // have. Everything else on the retry screen is inside the shared prefix.
    [Fact]
    public void OnlyTheMobileArmReachesPastTheConsoleArchive()
    {
        Assert.True(Mobile.has_touch_buttons);
        Assert.False(Console.has_touch_buttons);

        // bg_retry, the two labels, and the seven record-time digits
        var console_uses = new[] { 99, 16, 17, 49, 50, 51, 52, 53, 54, 55 };
        Assert.All(console_uses, id => Assert.True(id < ConsoleActNum));

        // and the six the mobile arm adds are all past it
        var touch_only = new[] { 100, 101, 102, 103, 104, 105 };
        Assert.All(touch_only, id => Assert.True(id >= ConsoleActNum));
    }

    // console starts the cursor on RETRY, so that label is already lit; the touch build lights
    // neither because the buttons carry the state
    [Fact]
    public void OnlyTheConsoleArmPreHighlightsRetry()
    {
        Assert.Equal(1.0f, Console.retry_default_frame);
        Assert.Equal(0.0f, Mobile.retry_default_frame);
    }

    // the three sites that only exist on one side, each from a #ifdef with no counterpart arm
    [Fact]
    public void TheOneSidedBehavioursSitOnTheRightArm()
    {
        Assert.True(Mobile.hides_clear_time_on_retry);
        Assert.True(Mobile.clears_nn_command_state_tbl);
        Assert.True(Mobile.sets_bg_color_black);
        Assert.False(Mobile.time_sonic_icon_per_frame);

        Assert.False(Console.hides_clear_time_on_retry);
        Assert.False(Console.clears_nn_command_state_tbl);
        Assert.False(Console.sets_bg_color_black);
        Assert.True(Console.time_sonic_icon_per_frame);
    }

    // four sites take decide, cancel or start; the time-attack record display takes decide only.
    // That asymmetry is SEGA's, and collapsing the two predicates would quietly lose it.
    [Fact]
    public void TheTwoSkipPredicatesAreDistinctOnConsole()
    {
        Assert.NotSame(Console.is_skip_pressed, Console.is_time_skip_pressed);
        Assert.NotEqual(Console.is_skip_pressed.Method, Console.is_time_skip_pressed.Method);
    }

    // the retry screen is a whole-function fork with no shared code between the arms
    [Fact]
    public void TheRetryInputIsADifferentFunctionPerArm()
    {
        Assert.NotEqual(Mobile.proc_retry_input.Method, Console.proc_retry_input.Method);
        Assert.Equal("gmClearDemoSetRetryInputPad", Console.proc_retry_input.Method.Name);
        Assert.Equal("gmClearDemoSetRetryInputTouch", Mobile.proc_retry_input.Method.Name);
    }

    // cur_retry_slct is outside the #ifdef in the C++, so the console cursor needs no new field.
    // If it ever moved onto the mobile-only block this would catch it.
    [Fact]
    public void TheCursorFieldExistsOnTheSharedWork()
    {
        var field = typeof(GmClearDemo.GMS_CLRDM_MAIN_WORK).GetField(
            "cur_retry_slct", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(field);
    }
}
