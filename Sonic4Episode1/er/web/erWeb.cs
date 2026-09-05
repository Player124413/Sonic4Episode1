namespace er.web
{
    internal class erWeb
    {
        public static void StartWeb(string url)
        {
#if WINDOWS_UWP
            _ = Windows.System.Launcher.LaunchUriAsync(new System.Uri(url));
#elif WINDOWSPHONE7_5 || WASM
            (new Microsoft.Phone.Tasks.WebBrowserTask() { Uri = new System.Uri(url) }).Show();
#else
            System.Diagnostics.Process.Start(url);
#endif
        }
    }
}
