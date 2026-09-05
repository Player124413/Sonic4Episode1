using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Sonic4Episode1.Content;

internal static class Program
{
    private const string Usage = @"s4content - Sonic 4 Episode I content pipeline

  unpack <archive|tree> <outdir>   extract to editable directories plus _archive.toml
         [--decode]                write textures as PNG rather than their shipped format
  pack   <dir> <out>               rebuild; a directory holding _archive.toml, or a tree of them
  packflat <dir> <out>             pack a plain directory of files into one archive, no descriptor
         [--filter <glob>]         only files matching, e.g. *.fxb
  diff   <treeA> <treeB>           compare two content trees by archive and member
         [--ignore-extension]      match members by stem, for comparing across texture formats
  normalise <tree> <outdir>        rewrite every archive, nested ones included, as #AMB
  build  <profile>                 emit a profile's tree: base archives plus variant siblings
  check  <profile>                 assert no base archive's member list moved
  texcheck <archiveA> <archiveB>   compare mean channel values member by member, for colour drift
  texformats <tree>                count every texture member by format, with disk and GPU bytes
  profiles                         list what content-sources.toml declares
  roundtrip <tree>                 unpack, pack, unpack again and check nothing moved
  xenonenums [pc] [xbla]           pair render state enums across two sources' shared models

Output is always #AMB. The legacy magic-less layout is read only.";

    private static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine(Usage);
            return 1;
        }

        try
        {
            switch (args[0].ToLowerInvariant())
            {
                case "unpack": return Unpack(args);
                case "pack": return Pack(args);
                case "packflat": return PackFlat(args);
                case "diff": return Diff(args);
                case "normalise":
                case "normalize": return Normalise(args);
                case "build": return Build(args);
                case "check": return Check(args);
                case "texcheck": return TexCheck(args);
                case "texformats": return TexFormats(args);
                case "profiles": return Profiles(args);
                case "roundtrip": return RoundTrip(args);
                case "xenonenums": return XenonEnums.Run(args);
                default:
                    Console.Error.WriteLine($"unknown verb '{args[0]}'");
                    Console.WriteLine(Usage);
                    return 1;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"error: {ex.Message}");
            return 1;
        }
    }

    private static IEnumerable<string> ArchivesUnder(string root)
    {
        return Directory
            .EnumerateFiles(root, "*.AMB", SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase);
    }

    private static string RelativeTo(string root, string path)
    {
        return Path.GetRelativePath(root, path).Replace('\\', '/');
    }

    private static int Unpack(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("usage: unpack <archive|tree> <outdir> [--decode]");
            return 1;
        }

        var source = args[1];
        var target = args[2];
        var decode = args.Any(a => string.Equals(a, "--decode", StringComparison.OrdinalIgnoreCase));

        if (File.Exists(source))
        {
            var name = Path.GetFileName(source);
            AmbTree.Unpack(File.ReadAllBytes(source), Path.Combine(target, name), name, decode);
            Console.WriteLine($"unpacked {name}");
            return 0;
        }

        if (!Directory.Exists(source))
        {
            Console.Error.WriteLine($"no such archive or directory: {source}");
            return 1;
        }

        var count = 0;
        foreach (var archive in ArchivesUnder(source))
        {
            var relative = RelativeTo(source, archive);
            AmbTree.Unpack(File.ReadAllBytes(archive), Path.Combine(target, relative), relative, decode);
            count++;
        }

        Console.WriteLine($"unpacked {count} archives from {source}");
        return 0;
    }

    private static bool IsArchiveDirectory(string path)
    {
        return File.Exists(Path.Combine(path, ArchiveDescriptor.FileName));
    }

    private static int Pack(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("usage: pack <dir> <out>");
            return 1;
        }

        var source = args[1];
        var target = args[2];

        if (!Directory.Exists(source))
        {
            Console.Error.WriteLine($"no such directory: {source}");
            return 1;
        }

        if (IsArchiveDirectory(source))
        {
            var directory = Path.GetDirectoryName(Path.GetFullPath(target));
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllBytes(target, AmbTree.Pack(source));
            Console.WriteLine($"packed {target}");
            return 0;
        }

        var count = 0;
        foreach (var directory in Directory
            .EnumerateDirectories(source, "*.AMB", SearchOption.AllDirectories)
            .Where(IsArchiveDirectory)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            // a nested archive's directory sits inside its parent's; only the outermost ones are emitted
            var parent = Path.GetDirectoryName(directory);
            if (parent != null && IsArchiveDirectory(parent))
                continue;

            var output = Path.Combine(target, RelativeTo(source, directory));
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllBytes(output, AmbTree.Pack(directory));
            count++;
        }

        Console.WriteLine($"packed {count} archives into {target}");
        return 0;
    }

    // for directories that were never an unpacked archive: the shader permutations and the sound
    // effects are loose files the runtime opens one at a time, and an archive is how the engine
    // already knows to hold a set of them
    private static int PackFlat(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("usage: packflat <dir> <out> [--filter <glob>]");
            return 1;
        }

        var source = args[1];
        var target = args[2];
        var filter = "*";

        for (int i = 3; i < args.Length - 1; i++)
        {
            if (args[i] == "--filter")
                filter = args[i + 1];
        }

        if (!Directory.Exists(source))
        {
            Console.Error.WriteLine($"no such directory: {source}");
            return 1;
        }

        // ordinal so two runs on the same directory produce identical bytes
        var files = Directory
            .EnumerateFiles(source, filter, SearchOption.TopDirectoryOnly)
            .OrderBy(Path.GetFileName, StringComparer.Ordinal)
            .ToList();

        if (files.Count == 0)
        {
            Console.Error.WriteLine($"no files matching '{filter}' in {source}");
            return 1;
        }

        var entries = files
            .Select(path => new AppMain.AmbEntry(Path.GetFileName(path), File.ReadAllBytes(path)))
            .ToList();

        var directory = Path.GetDirectoryName(Path.GetFullPath(target));
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var packed = AppMain.writeAmbArchive(entries, false);
        File.WriteAllBytes(target, packed);

        Console.WriteLine($"packed {entries.Count} files into {target} ({packed.Length} bytes)");
        return 0;
    }

    private static ContentSources LoadSources()
    {
        var path = ContentSources.Locate(Directory.GetCurrentDirectory());
        if (path == null)
            throw new FileNotFoundException(
                $"no {ContentSources.FileName} in this directory or any parent");

        return ContentSources.Load(path);
    }

    private static int Build(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("usage: build <profile>");
            return 1;
        }

        var sources = LoadSources();
        var profile = sources.Profile(args[1]);

        if (profile.Abstract)
        {
            Console.Error.WriteLine(
                $"'{profile.Name}' declares no textures; it is an assembly for other profiles to " +
                $"extend. Build one of: {string.Join(", ", sources.Extending(profile.Name))}");
            return 1;
        }

        var unsupported = profile.Encode.Where(f => !TextureTranscode.CanEncode(f)).ToList();
        if (unsupported.Count > 0)
        {
            Console.Error.WriteLine(
                $"'{profile.Name}' encodes to {string.Join(", ", unsupported.Select(TextureFormats.Token))}, " +
                "which nothing in the pipeline can produce yet. Building would emit a tree with every " +
                "texture left in its source format.");
            return 1;
        }

        var report = ContentBuild.Run(sources, profile);

        Console.WriteLine($"{profile.Name} -> {profile.Output}");
        Console.WriteLine($"  base archives     : {report.Emitted} from '{profile.Base}'");
        if (report.OverlaysEmitted > 0)
            Console.WriteLine($"  overlay archives  : {report.OverlaysEmitted}");
        Console.WriteLine($"  variant siblings  : {report.VariantsEmitted}");
        if (report.AssetsCopied > 0)
            Console.WriteLine($"  loose assets      : {report.AssetsCopied}");

        if (report.Moves.Count > 0)
        {
            Console.WriteLine("  textures");
            foreach (var move in report.Moves.OrderBy(m => m.Key.From).ThenBy(m => m.Key.To))
            {
                var from = TextureFormats.Token(move.Key.From);
                var to = TextureFormats.Token(move.Key.To);
                if (move.Key.From == move.Key.To)
                {
                    Console.WriteLine($"    {from,-8} {"",10} {move.Value.Count,6}  passthrough");
                    continue;
                }

                Console.WriteLine($"    {from,-8} -> {to,-7} {move.Value.Count,6}  " +
                                  $"{Megabytes(move.Value.Before)} -> {Megabytes(move.Value.After)}");
            }

            if (report.TexturesFailed > 0)
                Console.WriteLine($"    {report.TexturesFailed} failed");
        }

        if (report.Patched > 0 || report.Appended > 0)
        {
            Console.WriteLine($"  patched members   : {report.Patched}, appended {report.Appended}");
        }

        foreach (var note in report.Notes)
            Console.WriteLine($"  {note}");

        foreach (var problem in report.Problems.Take(20))
            Console.WriteLine($"  {problem}");
        if (report.Problems.Count > 20)
            Console.WriteLine($"  ... and {report.Problems.Count - 20} more");

        return 0;
    }

    private static int Check(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("usage: check <profile>");
            return 1;
        }

        var sources = LoadSources();
        var profile = sources.Profile(args[1]);

        if (profile.Abstract)
        {
            Console.Error.WriteLine($"'{profile.Name}' is an assembly; there is no tree to check");
            return 1;
        }

        var report = ContentBuild.Check(sources, profile);
        var problems = report.Problems;

        Console.WriteLine($"{profile.Name} against base '{profile.Base}'");
        Console.WriteLine($"  {profile.DescribeTextures()}");
        foreach (var note in report.Notes)
            Console.WriteLine($"  {note}");

        if (problems.Count == 0)
        {
            Console.WriteLine("  every base archive keeps its member count and order, " +
                              "and every texture is an accepted format");
            return 0;
        }

        Console.WriteLine($"  {problems.Count} problems");
        foreach (var problem in problems.Take(25))
            Console.WriteLine($"  {problem}");
        if (problems.Count > 25)
            Console.WriteLine($"  ... and {problems.Count - 25} more");

        return 1;
    }

    // BC compression moves the mean channel value by well under a unit; a gamma mistake moves it by tens
    private static int TexCheck(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("usage: texcheck <archiveA> <archiveB>");
            return 1;
        }

        var left = FlattenTextures(File.ReadAllBytes(args[1]), "");
        var right = FlattenTextures(File.ReadAllBytes(args[2]), "");
        var worst = 0.0;
        var worstPsnr = double.PositiveInfinity;
        long flipped = 0;

        foreach (var pair in left.Zip(right, (a, b) => (A: a, B: b)))
        {
            if (pair.A.Image == null || pair.B.Image == null)
                continue;

            var label = Path.GetFileNameWithoutExtension(pair.A.Name);
            try
            {
                var a = TextureTranscode.Describe(pair.A.Image);
                var b = TextureTranscode.Describe(pair.B.Image);
                var drift = Math.Max(Math.Max(Math.Abs(a.R - b.R), Math.Abs(a.G - b.G)), Math.Abs(a.B - b.B));
                var compare = TextureTranscode.Compare(pair.A.Image, pair.B.Image);
                worstPsnr = Math.Min(worstPsnr, compare.RgbPsnr);
                flipped += compare.AlphaFlipped;
                worst = Math.Max(worst, drift);

                Console.WriteLine($"  {label,-32} " +
                                  $"{a.R,6:0.0} {a.G,6:0.0} {a.B,6:0.0}  ->  " +
                                  $"{b.R,6:0.0} {b.G,6:0.0} {b.B,6:0.0}   drift {drift,5:0.0}" +
                                  $"   psnr {Psnr(compare.RgbPsnr)}  aflip {compare.AlphaFlipped}");
            }
            catch (Exception ex)
            {
                // one unreadable member should not hide the drift on everything after it
                Console.WriteLine($"  {label,-32} unreadable: {ex.Message}");
            }
        }

        Console.WriteLine($"  worst mean drift: {worst:0.0}   worst psnr: {Psnr(worstPsnr)}   " +
                          $"alpha flipped: {flipped}");

        // PSNR is the one that answers whether visible pixels moved; drift alone can't tell a
        // shifted alpha filter from a real colour change
        return worst < 4.0 && worstPsnr >= 30.0 ? 0 : 1;
    }

    private static string Psnr(double value)
    {
        return double.IsPositiveInfinity(value)
            ? "  exact"
            : value.ToString("00.0", CultureInfo.InvariantCulture) + " dB";
    }

    private sealed class FormatTally
    {
        public int Count;
        public long DiskBytes;
        public long GpuBytes;
        public long Texels;
    }

    private static int TexFormats(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("usage: texformats <tree>");
            return 1;
        }

        var root = args[1];
        if (!Directory.Exists(root))
        {
            Console.Error.WriteLine($"no such directory: {root}");
            return 1;
        }

        var tally = new SortedDictionary<TextureFormat, FormatTally>();
        var unknown = new List<string>();
        var mismatched = new List<string>();
        var archives = 0;

        foreach (var path in ArchivesUnder(root))
        {
            archives++;
            var relative = RelativeTo(root, path);

            foreach (var member in AmbTree.EnumerateMembers(File.ReadAllBytes(path), relative + "/"))
            {
                if (!TextureTranscode.IsTexture(member.Name))
                    continue;

                var info = TextureFormats.Probe(member.Data);

                if (!tally.TryGetValue(info.Format, out var entry))
                    tally[info.Format] = entry = new FormatTally();

                entry.Count++;
                entry.DiskBytes += member.Data.Length;
                entry.GpuBytes += TextureFormats.GpuBytes(info);
                entry.Texels += info.Texels;

                if (info.Format == TextureFormat.Unknown)
                    unknown.Add($"{member.Path}: {info.Detail ?? "unrecognised"}");
                else if (!string.Equals(Path.GetExtension(member.Name),
                             TextureFormats.Extension(info.Format), StringComparison.OrdinalIgnoreCase))
                    mismatched.Add($"{member.Path}: named {Path.GetExtension(member.Name)} " +
                                   $"but the payload is {TextureFormats.Token(info.Format)}");
            }
        }

        Console.WriteLine($"{root}: {archives} archives");
        Console.WriteLine();
        Console.WriteLine($"  {"format",-8} {"count",7} {"disk",12} {"gpu",12} {"texels",14}");

        var totalCount = 0;
        long totalDisk = 0, totalGpu = 0, totalTexels = 0;
        foreach (var pair in tally)
        {
            Console.WriteLine($"  {TextureFormats.Token(pair.Key),-8} {pair.Value.Count,7} " +
                              $"{Megabytes(pair.Value.DiskBytes),12} {Megabytes(pair.Value.GpuBytes),12} " +
                              $"{pair.Value.Texels,14:N0}");
            totalCount += pair.Value.Count;
            totalDisk += pair.Value.DiskBytes;
            totalGpu += pair.Value.GpuBytes;
            totalTexels += pair.Value.Texels;
        }

        Console.WriteLine($"  {"total",-8} {totalCount,7} {Megabytes(totalDisk),12} " +
                          $"{Megabytes(totalGpu),12} {totalTexels,14:N0}");

        Report("unrecognised", unknown);
        Report("extension disagrees with payload", mismatched);

        return unknown.Count == 0 && mismatched.Count == 0 ? 0 : 1;
    }

    private static void Report(string heading, List<string> lines)
    {
        if (lines.Count == 0)
            return;

        Console.WriteLine();
        Console.WriteLine($"  {heading}: {lines.Count}");
        foreach (var line in lines.Take(20))
            Console.WriteLine($"    {line}");

        if (lines.Count > 20)
            Console.WriteLine($"    ... and {lines.Count - 20} more");
    }

    private static string Megabytes(long bytes)
    {
        return (bytes / 1024.0 / 1024.0).ToString("N1", CultureInfo.InvariantCulture) + " MiB";
    }

    private static List<(string Name, byte[] Image)> FlattenTextures(byte[] archive, string prefix)
    {
        var found = new List<(string, byte[])>();
        if (!AmbTree.TryReadNested(archive, out var header))
            return found;

        for (int i = 0; i < header.file_num; i++)
        {
            var data = new byte[header.lengths[i]];
            Buffer.BlockCopy(header.data, header.offsets[i], data, 0, header.lengths[i]);

            if (AmbTree.LooksLikeArchive(header.data, header.offsets[i], header.lengths[i], header.files[i], out _))
                found.AddRange(FlattenTextures(data, prefix + header.files[i] + "/"));
            else if (TextureTranscode.IsTexture(header.files[i]))
                found.Add((prefix + header.files[i], data));
        }

        return found;
    }

    private static int Profiles(string[] args)
    {
        var path = ContentSources.Locate(Directory.GetCurrentDirectory());
        if (path == null)
        {
            Console.Error.WriteLine($"no {ContentSources.FileName} in this directory or any parent");
            return 1;
        }

        var sources = ContentSources.Load(path);
        Console.WriteLine(path);
        Console.WriteLine();
        Console.WriteLine("sources");
        foreach (var source in sources.Sources.Values)
        {
            var archives = Directory.Exists(source.Root)
                ? ArchivesUnder(source.Root).Count().ToString(CultureInfo.InvariantCulture) + " archives"
                : "MISSING";

            Console.WriteLine($"  {source.Name,-10} {archives,-14} {source.Root}");
        }

        Console.WriteLine();
        Console.WriteLine("profiles");
        foreach (var profile in sources.Profiles.Values)
        {
            var variants = profile.Variants.Count == 0
                ? "none"
                : string.Join(", ", profile.Variants.Select(v => v.Members.Count == 0
                    ? $"{v.Group}({v.Archives.Count})"
                    : $"{v.Group}({v.Archives.Count}+{v.Members.Count}m)"));
            var heads = profile.Heads.Count == 0 ? "none" : string.Join(",", profile.Heads);
            var overlays = profile.Overlays.Count == 0 ? "none" : string.Join(",", profile.Overlays);
            var patches = profile.Patches.Count == 0 ? "none" : string.Join(",", profile.Patches);

            Console.WriteLine($"  {profile.Name,-16} base={profile.Base,-10} " +
                              $"{(profile.Extends == null ? "" : "extends=" + profile.Extends + " ")}" +
                              $"{(profile.Abstract ? "ASSEMBLY" : profile.Ship ? "ships" : "NOT SHIPPED")}");
            Console.WriteLine($"  {"",-16} overlays={overlays} patches={patches}");
            Console.WriteLine($"  {"",-16} variants={variants}");
            if (!profile.Abstract)
            {
                Console.WriteLine($"  {"",-16} {profile.DescribeTextures()} heads={heads}");
                Console.WriteLine($"  {"",-16} -> {profile.Output}");
            }
        }

        return 0;
    }

    private static int Normalise(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("usage: normalise <tree> <outdir>");
            return 1;
        }

        var source = args[1];
        var target = args[2];

        if (!Directory.Exists(source))
        {
            Console.Error.WriteLine($"no such directory: {source}");
            return 1;
        }

        int converted = 0, copied = 0, failed = 0;
        foreach (var archive in ArchivesUnder(source))
        {
            var relative = RelativeTo(source, archive);
            var output = Path.Combine(target, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(output));

            var original = File.ReadAllBytes(archive);
            try
            {
                var normalised = AmbTree.Normalise(original);
                File.WriteAllBytes(output, normalised);

                if (normalised.AsSpan().SequenceEqual(original))
                    copied++;
                else
                    converted++;
            }
            catch (Exception ex)
            {
                failed++;
                Console.WriteLine($"  FAIL {relative}: {ex.Message}");
                File.WriteAllBytes(output, original);
            }
        }

        Console.WriteLine($"{source} -> {target}");
        Console.WriteLine($"  rewritten       : {converted}");
        Console.WriteLine($"  already #AMB    : {copied}");
        Console.WriteLine($"  failed, copied  : {failed}");

        return failed == 0 ? 0 : 1;
    }

    private sealed class Member
    {
        public string Name;
        public int Length;
        public string Hash;
    }

    private static List<Member> Flatten(byte[] bytes, string name)
    {
        var members = new List<Member>();
        if (AmbTree.TryReadNested(bytes, out var header))
            Walk(header, name, members, 0);

        return members;
    }

    private static void Walk(AppMain.AMS_AMB_HEADER header, string prefix, List<Member> into, int depth)
    {

        for (int i = 0; i < header.file_num; i++)
        {
            var name = $"{prefix}/{header.files[i]}";

            // matches a legacy nested archive against its #AMB counterpart instead of diffing them as one opaque member
            if (depth < 8 && AmbTree.LooksLikeArchive(
                    header.data, header.offsets[i], header.lengths[i], header.files[i], out var nested))
            {
                Walk(nested, name, into, depth + 1);
                continue;
            }

            var data = new byte[header.lengths[i]];
            Buffer.BlockCopy(header.data, header.offsets[i], data, 0, header.lengths[i]);

            into.Add(new Member
            {
                Name = name,
                Length = data.Length,
                Hash = Convert.ToHexString(MD5.HashData(data)),
            });
        }
    }

    // only the member's own extension is dropped, never the .AMB segments above it, so a PNG tree
    // and a DDS tree of the same content line up when diffed
    private static string KeyOf(string name, bool ignoreExtension)
    {
        if (!ignoreExtension)
            return name;

        var slash = name.LastIndexOf('/');
        var directory = slash < 0 ? string.Empty : name.Substring(0, slash + 1);
        var leaf = slash < 0 ? name : name.Substring(slash + 1);
        var dot = leaf.LastIndexOf('.');

        return dot <= 0 ? name : directory + leaf.Substring(0, dot);
    }

    private static Dictionary<string, Member> Index(string root, bool ignoreExtension)
    {
        var index = new Dictionary<string, Member>(StringComparer.OrdinalIgnoreCase);
        foreach (var archive in ArchivesUnder(root))
        {
            var relative = RelativeTo(root, archive);
            foreach (var member in Flatten(File.ReadAllBytes(archive), relative))
            {
                // names repeat inside an archive, so disambiguate rather than silently collapsing
                var stem = KeyOf(member.Name, ignoreExtension);
                var key = stem;
                for (int n = 1; index.ContainsKey(key); n++)
                    key = $"{stem}#{n.ToString(CultureInfo.InvariantCulture)}";

                index[key] = member;
            }
        }

        return index;
    }

    private static int Diff(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("usage: diff <treeA> <treeB>");
            return 1;
        }

        var ignoreExtension = args.Any(a =>
            string.Equals(a, "--ignore-extension", StringComparison.OrdinalIgnoreCase));

        var left = Index(args[1], ignoreExtension);
        var right = Index(args[2], ignoreExtension);

        var onlyLeft = left.Keys.Where(k => !right.ContainsKey(k)).OrderBy(k => k).ToList();
        var onlyRight = right.Keys.Where(k => !left.ContainsKey(k)).OrderBy(k => k).ToList();
        var common = left.Keys.Where(right.ContainsKey).OrderBy(k => k).ToList();
        var changed = common.Where(k => left[k].Hash != right[k].Hash).ToList();

        Console.WriteLine($"A {args[1]}: {left.Count} members");
        Console.WriteLine($"B {args[2]}: {right.Count} members");
        Console.WriteLine($"  only in A : {onlyLeft.Count}");
        Console.WriteLine($"  only in B : {onlyRight.Count}");
        Console.WriteLine($"  identical : {common.Count - changed.Count}");
        Console.WriteLine($"  differing : {changed.Count}");

        foreach (var key in onlyLeft.Take(20))
            Console.WriteLine($"  A only  {key}");
        foreach (var key in onlyRight.Take(20))
            Console.WriteLine($"  B only  {key}");
        foreach (var key in changed.Take(20))
            Console.WriteLine($"  differs {key}  {left[key].Length} vs {right[key].Length} bytes");

        return 0;
    }

    private static int RoundTrip(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("usage: roundtrip <tree>");
            return 1;
        }

        var root = args[1];
        int total = 0, identical = 0, failed = 0, legacy = 0;

        foreach (var archive in ArchivesUnder(root))
        {
            var relative = RelativeTo(root, archive);
            var original = File.ReadAllBytes(archive);

            total++;
            try
            {
                var before = AmbTree.Read(original);
                if (!before.is_real_amb)
                    legacy++;

                var packed = AppMain.writeAmbArchive(AppMain.readAmbEntries(before), before.big_endian);
                var after = AmbTree.Read(packed);

                var problem = Compare(before, after);
                if (problem != null)
                {
                    failed++;
                    Console.WriteLine($"  FAIL {relative}: {problem}");
                    continue;
                }

                if (packed.AsSpan().SequenceEqual(original))
                    identical++;
            }
            catch (Exception ex)
            {
                failed++;
                Console.WriteLine($"  FAIL {relative}: {ex.Message}");
            }
        }

        Console.WriteLine($"{root}");
        Console.WriteLine($"  archives        : {total} ({legacy} legacy, upgraded to #AMB on write)");
        Console.WriteLine($"  member-level ok : {total - failed}");
        Console.WriteLine($"  byte-identical  : {identical} ({Percent(identical, total)})");
        Console.WriteLine($"  failed          : {failed}");

        return failed == 0 ? 0 : 1;
    }

    private static string Percent(int part, int whole)
    {
        return whole == 0 ? "n/a" : (100.0 * part / whole).ToString("0.0", CultureInfo.InvariantCulture) + "%";
    }

    private static string Compare(AppMain.AMS_AMB_HEADER before, AppMain.AMS_AMB_HEADER after)
    {
        if (before.file_num != after.file_num)
            return $"file_num {before.file_num} became {after.file_num}";

        for (int i = 0; i < before.file_num; i++)
        {
            if (!string.Equals(before.files[i], after.files[i], StringComparison.Ordinal))
                return $"member {i} name '{before.files[i]}' became '{after.files[i]}'";
            if (before.lengths[i] != after.lengths[i])
                return $"member {i} length {before.lengths[i]} became {after.lengths[i]}";
            if (before.types[i] != after.types[i])
                return $"member {i} type {before.types[i]} became {after.types[i]}";
            if (before.user0[i] != after.user0[i] || before.user1[i] != after.user1[i])
                return $"member {i} user data changed";

            var lhs = before.data.AsSpan(before.offsets[i], before.lengths[i]);
            var rhs = after.data.AsSpan(after.offsets[i], after.lengths[i]);
            if (!lhs.SequenceEqual(rhs))
                return $"member {i} '{before.files[i]}' bytes differ";
        }

        return null;
    }
}
