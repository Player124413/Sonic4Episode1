using System;
using System.Diagnostics;
using System.IO;

namespace Sonic4Episode1.Content;

// astcenc as a subprocess, PNG in and PNG out, because it is the interchange DirectXTex already
// reads and writes here. See docs/astc-encoding.md for why this is not a managed encoder.
internal static class AstcTool
{
    private static readonly string Quality =
        Environment.GetEnvironmentVariable("ASTCENC_QUALITY") ?? "-medium";

    private static readonly Lazy<string> Executable = new Lazy<string>(Locate);

    private static string Locate()
    {
        var overridden = Environment.GetEnvironmentVariable("ASTCENC");
        if (!string.IsNullOrEmpty(overridden))
        {
            if (!File.Exists(overridden))
                throw new FileNotFoundException($"ASTCENC points at nothing: {overridden}");

            return overridden;
        }

        var sources = ContentSources.Locate(Directory.GetCurrentDirectory());
        if (sources != null)
        {
            var root = Path.GetDirectoryName(sources);
            foreach (var name in new[] { "astcenc-avx2.exe", "astcenc-sse4.1.exe", "astcenc-sse2.exe" })
            {
                var candidate = Path.Combine(root, "Dependencies", "astcenc", name);
                if (File.Exists(candidate))
                    return candidate;
            }
        }

        throw new FileNotFoundException(
            "no astcenc. Put the ARM release in Dependencies/astcenc, or set ASTCENC to it. " +
            "https://github.com/ARM-software/astc-encoder/releases");
    }

    public static byte[] Encode(byte[] png, TextureFormat target, string name)
    {
        var block = TextureFormats.AstcBlock(target);
        return Run(png, ".png", ".astc", "-cl", $"{block.X}x{block.Y}", name);
    }

    public static byte[] Decode(byte[] astc, string name)
    {
        return Run(astc, ".astc", ".png", "-dl", null, name);
    }

    // astcenc is file in, file out, so every call costs two temp files and a process. That is
    // still three orders of magnitude ahead of encoding in-process, measured in docs/astc-encoding.md
    private static byte[] Run(byte[] input, string inputExtension, string outputExtension,
        string mode, string block, string name)
    {
        var stem = Path.Combine(Path.GetTempPath(), "s4astc-" + Guid.NewGuid().ToString("N"));
        var inputPath = stem + inputExtension;
        var outputPath = stem + outputExtension;

        try
        {
            File.WriteAllBytes(inputPath, input);

            var arguments = $"{mode} \"{inputPath}\" \"{outputPath}\"";
            if (block != null)
                arguments += $" {block} {Quality}";

            arguments += " -silent";

            var start = new ProcessStartInfo(Executable.Value, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            using var process = Process.Start(start);
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new InvalidDataException(
                    $"{name}: astcenc exited {process.ExitCode}: {Trim(error)}{Trim(output)}");

            if (!File.Exists(outputPath))
                throw new InvalidDataException($"{name}: astcenc wrote nothing: {Trim(output)}");

            return File.ReadAllBytes(outputPath);
        }
        finally
        {
            Delete(inputPath);
            Delete(outputPath);
        }
    }

    private static string Trim(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? "" : text.Trim();
    }

    private static void Delete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (IOException)
        {
        }
    }
}
