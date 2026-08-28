using static SDL3.SDL;
using System;
using SDL3;

static class Program
{
    static int Main(string[] args)
    {
        SDL_SetHint("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");

        // Keep mouse and touch input separate.
        SDL_SetHint(SDL_HINT_MOUSE_TOUCH_EVENTS, "0");
        SDL_SetHint(SDL_HINT_TOUCH_MOUSE_EVENTS, "0");
        SDL_SetHint(SDL_HINT_PEN_TOUCH_EVENTS, "0");

        // Don't let the accelerometer take over controller slot 0.
        // SDL3.SDL.SDL_SetHint(SDL3.SDL.SDL_HINT_ACCELEROMETER_AS_JOYSTICK, "0");

        realArgs = args;
        return SDL_RunApp(0, IntPtr.Zero, FakeMain, IntPtr.Zero);
    }

    static string[] realArgs;

    [ObjCRuntime.MonoPInvokeCallback(typeof(SDL.SDL_main_func))]
    static int FakeMain(int argc, IntPtr argv)
    {
        RealMain(realArgs);
        return 0;
    }

    static void RealMain(string[] args)
    {
        using (var game = new Sonic4Ep1())
            game.Run();
    }
}