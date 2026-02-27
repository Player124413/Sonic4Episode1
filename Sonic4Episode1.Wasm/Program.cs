using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

public static partial class Program
{
    private static Sonic4Ep1 _game;

    internal static async Task Main(string[] args)
    {
        using var client = new HttpClient() { BaseAddress = new Uri(args[0]) };

        ReportProgress("Checking for updates...", 1.0);

        if (await CheckForUpdates(client))
            await InstallLatestVersion(client);

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);

        _game = new Sonic4Ep1();
        SetMainLoop(MainLoop);
        Launch();

        await Task.Delay(-1);
    }

    private static async Task<bool> CheckForUpdates(HttpClient client)
    {
        // if the file doesnt exist an update is guaranteed to be required
        // that said, if it does, we can assume that something is installed and not require
        // an internet connection
        if (!File.Exists("/content/version.sha256"))
            return true;

        try
        {
            var latestVersion = await client.GetStringAsync("version.sha256");
            var localVersion = await File.ReadAllTextAsync("/content/version.sha256");
            if (localVersion == latestVersion)
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static async Task InstallLatestVersion(HttpClient client)
    {
        var latestVersion = await client.GetStringAsync("version.sha256");

        using var message = new HttpRequestMessage(HttpMethod.Get, "Content.tar.gz");
        message.SetBrowserResponseStreamingEnabled(true);
        message.SetBrowserRequestMode(BrowserRequestMode.SameOrigin);

        using var response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var downloadStream = await response.Content.ReadAsStreamAsync();
        var stream = new GZipStream(downloadStream, CompressionMode.Decompress, true);
        var outputDir = "/content";

        try
        {
            byte[] buffer = new byte[32 * 1024];
            byte[] header = new byte[512];
            long bytesLeft = 0;
            long files = 0;
            while (true)
            {
                int bytesRead = 0;
                do
                {
                    bytesRead += await stream.ReadAsync(new Memory<byte>(header, bytesRead, 512 - bytesRead));
                } while (bytesRead != 0 && bytesRead < 512);

                if (bytesRead == 0) // End of archive
                    break;

                string fileName = Encoding.ASCII.GetString(header, 0, 100).Trim('\0');
                string fileSizeStr = Encoding.ASCII.GetString(header, 124, 12).Trim('\0');
                long fileSize = Convert.ToInt64(fileSizeStr, 8);
                byte fileType = header[156];

                int paddingSize = (int)(512 - (fileSize % 512)) % 512;

                if (fileType == 0 || fileType == 48) // Regular file
                {
                    if (fileName.StartsWith("./"))
                        fileName = fileName[2..];

                    fileName = fileName.ToLowerInvariant();

                    string filePath = Path.Combine(outputDir, fileName);
                    string directoryName = Path.GetDirectoryName(filePath);

                    // shut up it's single threaded it's fucking webassembly
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    using FileStream outputFile = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 32 * 1024, true);
                    outputFile.Seek(0, SeekOrigin.Begin);
                    outputFile.SetLength(fileSize);

                    bytesLeft = fileSize;

                    while (bytesLeft > 0)
                    {
                        int bytesToRead = (int)Math.Min(buffer.Length, bytesLeft);
                        int bytesReadFromArchive = await stream.ReadAsync(buffer.AsMemory(0, bytesToRead));
                        await outputFile.WriteAsync(buffer.AsMemory(0, bytesReadFromArchive));
                        bytesLeft -= bytesReadFromArchive;
                    }

                    files++;
                }

                bytesLeft = paddingSize;
                while (bytesLeft > 0)
                {
                    int bytesToRead = (int)Math.Min(buffer.Length, bytesLeft);
                    int bytesReadFromArchive = await stream.ReadAsync(buffer.AsMemory(0, bytesToRead));
                    bytesLeft -= bytesReadFromArchive;
                }

                if (files % 10 == 0)
                {
                    ReportProgress($"Installing files ({(files / (double)(756) * 100):N2}%)...", files / (double)(756));
                }
            }
        }
        catch (Exception ex) { }

        ReportProgress("Preparing to launch...", 1.0);

        await File.WriteAllTextAsync("/content/version.sha256", latestVersion);
        await SyncFS();
    }

    private static void MainLoop()
    {
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

    [JSImport("setMainLoop", "main.js")]
    internal static partial void SetMainLoop([JSMarshalAs<JSType.Function>] Action cb);

    [JSImport("reportProgress", "main.js")]
    internal static partial void ReportProgress(string status, double progress);

    [JSImport("syncFS", "main.js")]
    internal static partial Task SyncFS();

    [JSImport("go", "main.js")]
    internal static partial void Launch();
}