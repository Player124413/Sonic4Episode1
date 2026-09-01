using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using SDL3;

public static partial class Program
{
    private static Sonic4Ep1 _game;

    internal static async Task Main(string[] args)
    {
        // we're on SDL3 now!
        // Environment.SetEnvironmentVariable("FNA_PLATFORM_BACKEND", "SDL2");

        try
        {
            using var client = new HttpClient() { BaseAddress = new Uri(args[0]) };

            // the host picks this from the player's SD/HD/Console choice and a WebGL probe; see
            // lib/host/src/content.ts
            var profile = args.Length > 1 ? args[1] : "mobile-hd-dxt";

            ReportProgress("Checking for updates...", 1.0);

            if (await CheckForUpdates(client, profile))
                await InstallLatestVersion(client, profile);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);

            _game = new Sonic4Ep1();
            Launch(MainLoop);

            await Task.Delay(-1);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
    }

    // what is on disk, as "<profile> <hash>". The profile has to be part of it: two sets have
    // different hashes but comparing hash alone would still match a set we do not have installed
    private const string InstalledPath = "/content/installed.txt";

    private static async Task<bool> CheckForUpdates(HttpClient client, string profile)
    {
        // if the file doesnt exist an update is guaranteed to be required
        // that said, if it does, we can assume that something is installed and not require
        // an internet connection
        if (!File.Exists(InstalledPath))
            return true;

        try
        {
            var latest = await client.GetStringAsync($"sets/{profile}.sha256");
            var installed = await File.ReadAllTextAsync(InstalledPath);

            return installed != Installed(profile, latest);
        }
        catch
        {
            return false;
        }
    }

    private static string Installed(string profile, string version)
    {
        return profile + " " + version.Trim();
    }

    // only when the set actually changes: a same-set update overwrites in place, and wiping 800
    // files out of IDBFS is not free
    private static void ClearContent(string profile, string latestVersion)
    {
        if (!File.Exists(InstalledPath))
            return;

        try
        {
            var installed = File.ReadAllText(InstalledPath);
            if (installed.StartsWith(profile + " ", StringComparison.Ordinal))
                return;

            ReportProgress("Removing the previous content set...", 1.0);
            foreach (var entry in Directory.EnumerateFileSystemEntries("/content"))
            {
                if (Directory.Exists(entry))
                    Directory.Delete(entry, true);
                else
                    File.Delete(entry);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
    }

    private static async Task InstallLatestVersion(HttpClient client, string profile)
    {
        var latestVersion = await client.GetStringAsync($"sets/{profile}.sha256");

        // "<hash> <count>"; the count is the progress denominator
        var parts = latestVersion.Trim().Split(' ');
        var expectedFiles = parts.Length > 1 && int.TryParse(parts[1], out var count) ? count : 0;

        // the extractor only ever writes, so switching sets would otherwise leave a union of two
        // texture formats on disk
        ClearContent(profile, latestVersion);

        using var message = new HttpRequestMessage(HttpMethod.Get, $"sets/{profile}.tar.gz");
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
            long bytesWritten = 0;
            while (true)
            {
                int bytesRead = 0;
                while (bytesRead < 512)
                {
                    int got = await stream.ReadAsync(new Memory<byte>(header, bytesRead, 512 - bytesRead));
                    if (got == 0)
                        break;

                    bytesRead += got;
                }

                if (bytesRead == 0) // End of archive
                    break;

                // a short header is the stream ending mid-entry, which is not the same thing
                if (bytesRead < 512)
                    throw new EndOfStreamException(
                        $"stream ended {bytesRead} bytes into a header after {files} files, {bytesWritten} bytes");

                string fileName = Encoding.ASCII.GetString(header, 0, 100).Trim('\0');

                // the numeric fields end in a space or a NUL, both legal. GNU tar writes NUL,
                // libarchive (which is Windows' tar.exe) writes a space
                string fileSizeStr = Encoding.ASCII.GetString(header, 124, 12).Trim('\0', ' ');
                long fileSize = fileSizeStr.Length == 0 ? 0 : Convert.ToInt64(fileSizeStr, 8);
                byte fileType = header[156];

                int paddingSize = (int)(512 - (fileSize % 512)) % 512;

                if (fileType == 0 || fileType == 48) // Regular file
                {
                    if (fileName.StartsWith("./"))
                        fileName = fileName[2..];

                    fileName = fileName.ToLowerInvariant();

                    string filePath = Path.Combine(outputDir, fileName);
                    string directoryName = Path.GetDirectoryName(filePath);

                    Directory.CreateDirectory(directoryName);

                    using FileStream outputFile = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 32 * 1024, true);
                    outputFile.Seek(0, SeekOrigin.Begin);
                    outputFile.SetLength(fileSize);

                    bytesLeft = fileSize;

                    while (bytesLeft > 0)
                    {
                        int bytesToRead = (int)Math.Min(buffer.Length, bytesLeft);
                        int bytesReadFromArchive = await stream.ReadAsync(buffer.AsMemory(0, bytesToRead));
                        if (bytesReadFromArchive == 0)
                            throw new EndOfStreamException(
                                $"stream ended {bytesLeft} bytes short of '{fileName}' " +
                                $"after {files} files, {bytesWritten} bytes");

                        await outputFile.WriteAsync(buffer.AsMemory(0, bytesReadFromArchive));
                        bytesLeft -= bytesReadFromArchive;
                        bytesWritten += bytesReadFromArchive;
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

                if (files % 10 == 0 && expectedFiles > 0)
                {
                    var fraction = files / (double)expectedFiles;
                    ReportProgress($"Installing files ({(fraction * 100):N2}%)...", fraction);
                }
            }

            Console.WriteLine($"installed {files} files, {bytesWritten} bytes, expected {expectedFiles} files");
            if (expectedFiles > 0 && files != expectedFiles)
                throw new EndOfStreamException(
                    $"archive held {files} files, the manifest says {expectedFiles}");
        }
        catch (Exception ex)
        {
            // a half extracted tree is worse than none: it would look installed and then fail on a
            // texture nobody can explain
            Console.Error.WriteLine(ex);
            ReportProgress("Content download failed, see the console.", 1.0);
            return;
        }

        ReportProgress("Preparing to launch...", 1.0);

        await File.WriteAllTextAsync(InstalledPath, Installed(profile, latestVersion));
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

    [JSImport("reportProgress", "main.js")]
    internal static partial void ReportProgress(string status, double progress);

    [JSImport("syncFS", "main.js")]
    internal static partial Task SyncFS();

    [JSImport("go", "main.js")]
    internal static partial void Launch([JSMarshalAs<JSType.Function>] Action cb);
}