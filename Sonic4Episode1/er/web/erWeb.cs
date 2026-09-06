#if WASM
using System.Runtime.InteropServices.JavaScript;
#endif

namespace er.web
{
    internal partial class erWeb
    {
#if WASM
        [JSImport("WebBrowserTask_Open", "main.js")]
        internal static partial void WebBrowserTask_Open(string url);
#endif

        public static void StartWeb(string url)
        {
#if WINDOWS_UWP
            _ = Windows.System.Launcher.LaunchUriAsync(new System.Uri(url));
#elif WINDOWSPHONE7_5
            (new Microsoft.Phone.Tasks.WebBrowserTask() { Uri = new System.Uri(url) }).Show();
#elif WASM
            WebBrowserTask_Open(url);
#else
            System.Diagnostics.Process.Start(url);
#endif
        }
    }
}
