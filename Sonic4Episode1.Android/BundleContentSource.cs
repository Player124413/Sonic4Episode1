using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

internal sealed class BundleContentSource : IContentSource
{
    private const int Attempts = 4;
    private const int FirstRetryMs = 250;

    private readonly string _root;
    private readonly string _set;
    private readonly string _streamed;
    private readonly bool _remote;
    private readonly HttpClient _http;
    private readonly Dictionary<string, string> _bundleOf = new(StringComparer.Ordinal);
    private readonly Dictionary<string, Task> _ingesting = new(StringComparer.Ordinal);

    // the set is <base>/sets/<profile>, and the oggs sit at <base>/Content beside it rather than in
    // any set, because they are byte identical across profiles
    private BundleContentSource(string root, string set)
    {
        _root = root;
        _set = set.TrimEnd('/', '\\');
        _remote = set.StartsWith("http", StringComparison.OrdinalIgnoreCase);
        _http = _remote ? new HttpClient() : null;

        var sets = _remote
            ? _set[.._set.LastIndexOf('/')]
            : Path.GetDirectoryName(_set);

        _streamed = _remote
            ? sets[..sets.LastIndexOf('/')] + "/Content"
            : Path.Combine(Path.GetDirectoryName(sets), "Content");
    }

    public string Root => _root;

    public bool CanOpenOnDemand => false;

    public byte[] Read(string path) => File.ReadAllBytes(Resolve(path));

    // amFs calls this from the game loop and polls the task, so it has to leave that thread at
    // once. Awaiting is not enough: a bundle read out of a local directory completes every await
    // synchronously, which runs the whole gunzip and untar inline on the caller
    public Task<byte[]> ReadAsync(string path) => Task.Run(() => Fetch(path));

    private async Task<byte[]> Fetch(string path)
    {
        var name = Normalize(path);
        var file = Path.Combine(_root, name);

        if (!File.Exists(file))
        {
            if (_bundleOf.TryGetValue(name, out var bundle))
                await Ingest(bundle);
            else
                await FetchLoose(path.Replace('\\', '/').TrimStart('/'), file);
        }

        return await File.ReadAllBytesAsync(file);
    }

    // set is either a URL or a directory holding the profile's sets tree. A directory is what an
    // `adb push` of Sonic4Episode1.Wasm/wwwroot/sets/<profile> gets you, and it needs no server
    public static async Task<BundleContentSource> InstallAsync(
        string root, string set, IProgress<double> progress)
    {
        var source = new BundleContentSource(root, set);
        await source.Install(progress);
        return source;
    }

    private async Task Install(IProgress<double> progress)
    {
        var manifest = ParseManifest(await ReadSetText("manifest.txt"));
        var stamp = Path.Combine(_root, ".version");

        if (File.Exists(stamp) && File.ReadAllText(stamp) == manifest.Version)
            return;

        // a version change invalidates every bundle, not just the ones that moved, and there is no
        // per file digest to be cleverer with
        if (Directory.Exists(_root))
            Directory.Delete(_root, true);

        Directory.CreateDirectory(_root);

        await Extract("boot", progress);
        File.WriteAllText(stamp, manifest.Version);
    }

    private Task Ingest(string bundle)
    {
        lock (_ingesting)
        {
            if (File.Exists(MarkerFor(bundle)))
                return Task.CompletedTask;

            if (_ingesting.TryGetValue(bundle, out var pending))
                return pending;

            var task = Run();
            _ingesting[bundle] = task;
            return task;
        }

        async Task Run()
        {
            try
            {
                await Extract(bundle, null);
            }
            finally
            {
                lock (_ingesting)
                    _ingesting.Remove(bundle);
            }
        }
    }

    private async Task Extract(string bundle, IProgress<double> progress)
    {
        using var packed = await OpenSet(bundle + ".tar.gz", progress);
        using var tar = new GZipStream(packed, CompressionMode.Decompress);
        using var reader = new TarReader(tar);

        while (await reader.GetNextEntryAsync() is { } entry)
        {
            if (entry.EntryType is not (TarEntryType.RegularFile or TarEntryType.V7RegularFile))
                continue;

            if (entry.DataStream == null)
                continue;

            var target = Path.GetFullPath(Path.Combine(_root, Normalize(entry.Name)));
            if (!target.StartsWith(_root, StringComparison.Ordinal))
                throw new InvalidDataException($"'{entry.Name}' in {bundle} escapes the content root");

            Directory.CreateDirectory(Path.GetDirectoryName(target));

            using var file = File.Create(target);
            await entry.DataStream.CopyToAsync(file);
        }

        var marker = MarkerFor(bundle);
        Directory.CreateDirectory(Path.GetDirectoryName(marker));
        File.WriteAllBytes(marker, []);
    }

    // BGM is in no bundle: it is fetched a track at a time, and a track with no Loop half is normal,
    // so a miss has to fault the task rather than retry into a hang
    private async Task FetchLoose(string name, string file)
    {
        using var stream = await Open(_streamed, name, null);

        Directory.CreateDirectory(Path.GetDirectoryName(file));

        using var target = File.Create(file);
        await stream.CopyToAsync(target);
    }

    private string MarkerFor(string bundle) => Path.Combine(_root, ".bundles", bundle);

    private string Resolve(string path) => Path.Combine(_root, Normalize(path));

    // the web head's key(): forward slashes, lower case, on the tree and on every lookup alike.
    // FNA's TitleContainer lowercases a name before opening it, and the case repair behind that
    // lists "/", which Android will not have, so the tree has to be laid out already lowered
    private static string Normalize(string path) =>
        path.Replace('\\', '/').TrimStart('/').ToLowerInvariant();

    private async Task<string> ReadSetText(string name)
    {
        using var stream = await Open(_set, name, null);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    private Task<Stream> OpenSet(string name, IProgress<double> progress) =>
        Open(_set, name, progress);

    private async Task<Stream> Open(string root, string name, IProgress<double> progress)
    {
        if (!_remote)
            return File.OpenRead(Path.GetFullPath(Path.Combine(root, name)));

        var url = root + "/" + name;
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                // a 404 is a packaging mistake, and retrying it turns a loud failure into a hang
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new FileNotFoundException($"'{url}' is not in the set", name);

                if (!response.IsSuccessStatusCode && !Retryable(response.StatusCode))
                    throw new IOException($"'{url}' returned {(int)response.StatusCode}");

                response.EnsureSuccessStatusCode();
                return await Read(response, progress);
            }
            catch (Exception ex) when (attempt < Attempts && ex is not FileNotFoundException)
            {
                await Task.Delay(FirstRetryMs << (attempt - 1));
            }
        }

        // buffered rather than handed over live, so a dropped connection mid bundle is a retry
        // rather than a half written tree
        static async Task<Stream> Read(HttpResponseMessage response, IProgress<double> progress)
        {
            var total = response.Content.Headers.ContentLength ?? 0;
            var buffer = total > 0 ? new MemoryStream((int)total) : new MemoryStream();

            using var body = await response.Content.ReadAsStreamAsync();
            var chunk = new byte[64 * 1024];
            int got;
            while ((got = await body.ReadAsync(chunk)) > 0)
            {
                buffer.Write(chunk, 0, got);
                if (total > 0)
                    progress?.Report((double)buffer.Length / total);
            }

            buffer.Position = 0;
            return buffer;
        }
    }

    private static bool Retryable(HttpStatusCode code) =>
        code == HttpStatusCode.RequestTimeout ||
        code == HttpStatusCode.TooManyRequests ||
        (int)code >= 500;

    private Manifest ParseManifest(string text)
    {
        var version = "";
        var bundle = "";

        foreach (var raw in text.Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0)
                continue;

            if (line.StartsWith("version ", StringComparison.Ordinal))
                version = line[8..];
            else if (line.StartsWith("boot ", StringComparison.Ordinal))
                bundle = "";
            else if (line.StartsWith("bundle ", StringComparison.Ordinal))
                bundle = line[7..].Split(' ')[0];
            else if (bundle.Length != 0)
                _bundleOf[Normalize(line)] = bundle;
        }

        if (version.Length == 0)
            throw new InvalidDataException("manifest has no version line");

        return new Manifest(version);
    }

    private readonly record struct Manifest(string Version);
}
