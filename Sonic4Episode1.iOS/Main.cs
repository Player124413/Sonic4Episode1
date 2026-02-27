using SDL2;
using System; 

static class Program
{
    static void Main(string[] args)
    {
        // Enable high DPI "Retina" support. Trust us, you'll want this.
        SDL.SDL_SetHint("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");

        // Keep mouse and touch input separate.
        SDL.SDL_SetHint(SDL.SDL_HINT_MOUSE_TOUCH_EVENTS, "0");
        SDL.SDL_SetHint(SDL.SDL_HINT_TOUCH_MOUSE_EVENTS, "0");

        // Don't let the accelerometer take over controller slot 0.
        SDL.SDL_SetHint(SDL.SDL_HINT_ACCELEROMETER_AS_JOYSTICK, "0");

        realArgs = args;
        SDL.SDL_UIKitRunApp(0, IntPtr.Zero, FakeMain);
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