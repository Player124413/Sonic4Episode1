using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using SDL3;

public static partial class Program
{
    private static Sonic4Ep1 _game;

    internal static async Task Main(string[] args)
    {
        try
        {
            _clock = System.Diagnostics.Stopwatch.StartNew();

            // the host picks this from the player's SD/HD/Console choice and a WebGL probe; see
            // lib/host/src/content.ts
            var profile = args.Length > 0 ? args[0] : "mobile-hd-dxt";

            // a parked read leaves the engine half built, so the offer is a clean restart rather
            // than a retry into whatever state the level got to
            ContentFailure.Restart = ReloadPage;

            await BundleContentSource.InstallAsync(profile);
            ContentSource.Current = new BundleContentSource();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);

            _game = new Sonic4Ep1();
            Launch(MainLoop);

            await Task.Delay(-1);
        }
        catch (Exception ex)
        {
            // the type and the inner chain, because the JS side only surfaces the message
            Console.Error.WriteLine($"[boot] {ex.GetType().FullName}: {ex.Message}");
            for (var inner = ex.InnerException; inner != null; inner = inner.InnerException)
                Console.Error.WriteLine($"[boot]   inner {inner.GetType().FullName}: {inner.Message}");

            Console.Error.WriteLine(ex);
        }
    }

    [JSExport]
    internal static void SetViewSize(int width, int height) => _game?.SetViewSize(width, height);

    private static System.Diagnostics.Stopwatch _clock;

    private static void MainLoop()
    {
        if (_clock != null)
        {
            Console.WriteLine($"[content {_clock.ElapsedMilliseconds,6} ms] first frame");
            _clock = null;
        }

        try
        {
            _game?.RunOneFrame();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            throw;
        }
    }

    [JSImport("reloadPage", "main.js")]
    internal static partial void ReloadPage();

    [JSImport("go", "main.js")]
    internal static partial void Launch([JSMarshalAs<JSType.Function>] Action cb);
}
