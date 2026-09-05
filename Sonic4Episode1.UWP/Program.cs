using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace Sonic4Episode1.UWP;

class Program
{
    static void Main(string[] args)
    {
        // Set hints for how we want SDL to behave: https://wiki.libsdl.org/CategoryHints
        SDL_SetHint("SDL_WINRT_HANDLE_BACK_BUTTON", "1");

        // Delegate to SDL to allow it to perform all the necessary tasks needed to
        // boostrap a UWP app, so we don't have to! Once it's ready it will fire SDLMain.
        SDL_main_func mainFunction = SDLMain;
        SDL_WinRTRunApp(mainFunction, IntPtr.Zero);
    }

    private static int SDLMain(int argc, IntPtr argv)
    {
        var game = new Sonic4Ep1();
        game.Run();

        return 0;
    }
}
