using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sonic4Episode1.Content;

// emits one output tree per profile: the base tree's archives, normalised to #AMB with their
// member list untouched, plus variants as sibling files rather than edits to a base archive
internal sealed class BuildReport
{
    public int Emitted;
    public int AssetsCopied;
    public int OverlaysEmitted;
    public int VariantsEmitted;
    public int VariantsWithoutBase;
    public List<string> Variants = new List<string>();
    public int TexturesConverted;
    public int TexturesFailed;
    public int Patched;
    public int Appended;
    public long TextureBytesBefore;
    public long TextureBytesAfter;
    public List<string> Problems = new List<string>();
    public List<string> Notes = new List<string>();

    public sealed class Move
    {
        public int Count;
        public long Before;
        public long After;
    }

    public Dictionary<(TextureFormat From, TextureFormat To), Move> Moves =
        new Dictionary<(TextureFormat, TextureFormat), Move>();

    public void Record(TextureFormat from, TextureFormat to, long before, long after)
    {
        if (!Moves.TryGetValue((from, to), out var move))
            Moves[(from, to)] = move = new Move();

        move.Count++;
        move.Before += before;
        move.After += after;
    }

    // each worker fills its own report and folds it in here, so nothing in the build needs a lock
    // around the counters themselves
    public void Merge(BuildReport other)
    {
        Emitted += other.Emitted;
        OverlaysEmitted += other.OverlaysEmitted;
        VariantsEmitted += other.VariantsEmitted;
        VariantsWithoutBase += other.VariantsWithoutBase;
        TexturesConverted += other.TexturesConverted;
        TexturesFailed += other.TexturesFailed;
        Patched += other.Patched;
        Appended += other.Appended;
        TextureBytesBefore += other.TextureBytesBefore;
        TextureBytesAfter += other.TextureBytesAfter;

        Variants.AddRange(other.Variants);
        Problems.AddRange(other.Problems);
        Notes.AddRange(other.Notes);

        foreach (var pair in other.Moves)
        {
            if (!Moves.TryGetValue(pair.Key, out var move))
                Moves[pair.Key] = move = new Move();

            move.Count += pair.Value.Count;
            move.Before += pair.Value.Before;
            move.After += pair.Value.After;
        }
    }
}

internal sealed class CheckReport
{
    public List<string> Problems = new List<string>();
    public List<string> Notes = new List<string>();
}

internal static class ContentBuild
{
    private static Dictionary<string, string> Archives(ContentSource source)
    {
        var found = Archives(source.Root);
        if (string.IsNullOrEmpty(source.DropSiblingSuffix))
            return found;

        foreach (var key in found.Keys.ToList())
        {
            var stem = Path.GetFileNameWithoutExtension(key);
            if (!stem.EndsWith(source.DropSiblingSuffix, StringComparison.OrdinalIgnoreCase))
                continue;

            var directory = Path.GetDirectoryName(key)?.Replace('\\', '/');
            var sibling = stem.Substring(0, stem.Length - source.DropSiblingSuffix.Length)
                          + Path.GetExtension(key);

            if (found.ContainsKey(string.IsNullOrEmpty(directory) ? sibling : directory + "/" + sibling))
                found.Remove(key);
        }

        return found;
    }

    // loose .AMA is in here because the pause menu is the one screen that reads an action file off
    // the content root instead of out of an archive. Only G_PAUSE.AMA and G_PAUSE_L.AMA exist.
    private static Dictionary<string, string> Archives(string root)
    {
        var found = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pattern in new[] { "*.AMB", "*.AMA" })
        {
            foreach (var path in Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories))
                found[Path.GetRelativePath(root, path).Replace('\\', '/')] = path;
        }

        return found;
    }

    // an .AMA is an action file, not a container: nothing inside it to transcode, repack or check
    private static bool IsLooseAma(string relative)
    {
        return string.Equals(Path.GetExtension(relative), ".AMA", StringComparison.OrdinalIgnoreCase);
    }

    // authored replacements, keyed like `unpack` lays a tree out but with the extension dropped so
    // a PNG can stand in for a PVR; everything after this is the ordinary encode path
    private static Dictionary<string, string> PatchIndex(ContentSources sources, ContentProfile profile)
    {
        var index = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var name in profile.Patches)
        {
            var root = sources.Source(name).Root;
            foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            {
                if (string.Equals(Path.GetFileName(path), ArchiveDescriptor.FileName,
                        StringComparison.OrdinalIgnoreCase))
                    continue;

                var relative = Path.GetRelativePath(root, path).Replace('\\', '/');
                var key = relative.Substring(0, relative.Length - Path.GetExtension(relative).Length);

                index[key] = path;
            }
        }

        return index;
    }

    public static BuildReport Run(ContentSources sources, ContentProfile profile)
    {
        var report = new BuildReport();
        var baseArchives = Archives(sources.Source(profile.Base));
        var patches = PatchIndex(sources, profile);

        Directory.CreateDirectory(profile.Output);

        Each(baseArchives.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase), report, (pair, local) =>
        {
            var output = Path.Combine(profile.Output, pair.Key);
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllBytes(output, Emit(pair.Value, pair.Key, local, profile, patches));
            local.Emitted++;
        });

        foreach (var overlay in profile.Overlays)
            EmitOverlay(sources.Source(overlay), baseArchives, profile, report, patches);

        foreach (var variant in profile.Variants)
        {
            var source = sources.Source(variant.Source);
            EmitVariant(variant, source, baseArchives, profile, report);
            EmitMemberVariant(variant, source, baseArchives, profile, report);
        }

        foreach (var asset in sources.Assets)
            CopyAssets(asset, sources.Source(asset.Source), profile, report);

        if (report.VariantsWithoutBase > 0)
            report.Notes.Add($"{report.VariantsWithoutBase} variant archives the base has no counterpart for");

        // parallel emission makes arrival order arbitrary; sorting keeps two runs comparable
        report.Problems.Sort(StringComparer.OrdinalIgnoreCase);

        WriteManifest(profile, report);
        WriteShipMarker(profile);

        return report;
    }

    // verbatim, and deliberately not parallel: these are big and mostly one stream each, so the
    // archive emission above is where the cores are worth spending
    private static void CopyAssets(AssetSpec asset, ContentSource source, ContentProfile profile,
        BuildReport report)
    {
        var into = asset.Into == null
            ? profile.Output
            : Path.Combine(profile.Output, asset.Into);

        if (asset.Paths.Count == 0)
        {
            report.AssetsCopied += CopyTree(source.Root, into);
            return;
        }

        foreach (var path in asset.Paths)
        {
            var from = Path.Combine(source.Root, path);
            var to = Path.Combine(into, path);

            if (Directory.Exists(from))
            {
                report.AssetsCopied += CopyTree(from, to);
            }
            else if (File.Exists(from))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(to));
                File.Copy(from, to, true);
                report.AssetsCopied++;
            }
            else
            {
                report.Problems.Add($"asset '{path}' is not in source '{asset.Source}' ({from})");
            }
        }
    }

    private static int CopyTree(string from, string to)
    {
        var copied = 0;
        Directory.CreateDirectory(to);

        foreach (var file in Directory.EnumerateFiles(from, "*", SearchOption.AllDirectories))
        {
            var target = Path.Combine(to, Path.GetRelativePath(from, file));
            Directory.CreateDirectory(Path.GetDirectoryName(target));
            File.Copy(file, target, true);
            copied++;
        }

        return copied;
    }

    // MSBuild cannot read the TOML, so the ship flag has to reach Content.targets as a file. Its
    // presence is what the Release gate tests for
    public const string NotShippedName = ".notshipped";

    private static void WriteShipMarker(ContentProfile profile)
    {
        var path = Path.Combine(profile.Output, NotShippedName);
        if (profile.Ship)
        {
            File.Delete(path);
            return;
        }

        File.WriteAllText(path,
            $"'{profile.Name}' is a diagnostic tree and must not ship. It is the only profile that " +
            "may carry PVR." + Environment.NewLine);
    }

    // archives are independent: each reads its own source and writes its own output, so the only
    // shared state is the report, and every worker gets its own to fold in at the end
    private static void Each<T>(IEnumerable<T> items, BuildReport report, Action<T, BuildReport> body)
    {
        Parallel.ForEach(
            items,
            () => new BuildReport(),
            (item, _, local) =>
            {
                body(item, local);
                return local;
            },
            local =>
            {
                lock (report)
                    report.Merge(local);
            });
    }

    // an overlay archive with no counterpart in the base lands at its own path; colliding with a
    // base archive would be a silent replacement, which the design forbids
    private static void EmitOverlay(
        ContentSource source,
        Dictionary<string, string> baseArchives,
        ContentProfile profile,
        BuildReport report,
        Dictionary<string, string> patches)
    {
        var all = Archives(source).OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase).ToList();
        var emit = all.Where(p => !baseArchives.ContainsKey(p.Key)).ToList();

        Each(emit, report, (pair, local) =>
        {
            var output = Path.Combine(profile.Output, pair.Key);
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllBytes(output, Emit(pair.Value, pair.Key, local, profile, patches));
            local.OverlaysEmitted++;
        });

        // an overlay mostly duplicating the base is normal; count collisions rather than flag them
        var collided = all.Count - emit.Count;
        if (collided > 0)
            report.Notes.Add($"overlay '{source.Name}': {collided} archives the base already has");
    }

    private static byte[] Emit(string path, string relative, BuildReport report,
        ContentProfile profile, Dictionary<string, string> patches, bool flip = false)
    {
        return Emit(File.ReadAllBytes(path), relative, report, profile, patches, flip);
    }

    private static byte[] Emit(byte[] bytes, string relative, BuildReport report,
        ContentProfile profile, Dictionary<string, string> patches, bool flip = false)
    {
        if (IsLooseAma(relative))
            return bytes;

        try
        {
            var normalised = AmbTree.Normalise(bytes);
            return Transcode(normalised, relative, report, profile, patches, flip) ?? normalised;
        }
        catch (Exception ex)
        {
            report.Problems.Add($"{relative}: {ex.Message}, copied verbatim");
            return bytes;
        }
    }

    // rebuilds texture members in place, only the bytes and extension change so no index moves;
    // returns null when nothing changed, so the caller keeps the normalised bytes as-is
    private static byte[] Transcode(byte[] archive, string relative, BuildReport report,
        ContentProfile profile, Dictionary<string, string> patches, bool flip = false)
    {
        if (!AmbTree.TryReadNested(archive, out var header))
            return null;

        var entries = new List<AppMain.AmbEntry>(header.file_num);
        var changed = false;

        // mirrors what unpack writes, so a patch tree is just an unpacked tree with edits in it
        var taken = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var consumed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < header.file_num; i++)
        {
            var name = header.files[i];
            var diskName = AmbTree.DiskName(name, taken);
            var data = new byte[header.lengths[i]];
            Buffer.BlockCopy(header.data, header.offsets[i], data, 0, header.lengths[i]);

            if (AmbTree.LooksLikeArchive(header.data, header.offsets[i], header.lengths[i], name, out _))
            {
                var rebuilt = Transcode(data, relative + "/" + diskName, report, profile, patches, flip);
                if (rebuilt != null)
                {
                    data = rebuilt;
                    changed = true;
                }
            }
            else if (TextureTranscode.IsTexture(name))
            {
                var key = relative + "/" + Path.GetFileNameWithoutExtension(diskName);
                if (patches.TryGetValue(key, out var patch))
                {
                    consumed.Add(key);
                    data = File.ReadAllBytes(patch);

                    // extension has to follow the payload or amTextureDecode dispatches on the wrong one
                    name = TextureFormats.NameFor(name, TextureFormats.Probe(data).Format);
                    changed = true;
                    report.Patched++;
                }

                if (Retarget(ref data, ref name, relative, report, profile, flip))
                    changed = true;
            }

            entries.Add(new AppMain.AmbEntry
            {
                Name = name,
                Data = data,
                Type = header.types[i],
                User0 = header.user0[i],
                User1 = header.user1[i],
            });

        }

        if (Append(entries, consumed, relative, report, patches))
            changed = true;

        return changed ? AppMain.writeAmbArchive(entries, header.big_endian) : null;
    }

    // a patch file whose member doesn't exist is an addition, not a replacement; appended members
    // land after every existing one (so no index moves) and are emitted verbatim, exempt from the
    // profile's encode and accept rules, since the author has already chosen the format
    private static bool Append(List<AppMain.AmbEntry> entries, HashSet<string> consumed,
        string relative, BuildReport report, Dictionary<string, string> patches)
    {
        var prefix = relative + "/";
        var added = false;

        foreach (var pair in patches.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (!pair.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                continue;

            // a stem can legitimately be both a replacement and an addition at once
            if (pair.Key.Substring(prefix.Length).Contains('/') || consumed.Contains(pair.Key))
                continue;

            entries.Add(new AppMain.AmbEntry
            {
                Name = Path.GetFileName(pair.Value),
                Data = File.ReadAllBytes(pair.Value),
                Type = ArchiveDescriptor.DefaultType,
            });

            report.Appended++;
            added = true;
        }

        return added;
    }

    private static bool Retarget(ref byte[] data, ref string name, string relative,
        BuildReport report, ContentProfile profile, bool flip = false)
    {
        var info = TextureFormats.Probe(data);
        var bytes = data;

        // stays lazy: a texture already in the target format decodes zero times, as before
        TextureTranscode.Decoded decoded = null;
        try
        {
            var plan = TexturePlanner.Choose(
                info, () => (decoded ??= TextureTranscode.Decode(bytes, info, flip)).Alpha(),
                profile.Encode, profile.Accept, out var problem);

            if (problem != null)
            {
                report.Problems.Add($"{relative}/{name}: {problem}");
                return false;
            }

            // a flip has to happen even when the format is already the target, so it forces the
            // re-encode the equal-format case would otherwise skip
            if (plan.Target == info.Format && !flip)
            {
                report.Record(info.Format, info.Format, data.Length, data.Length);
                return false;
            }

            try
            {
                decoded ??= TextureTranscode.Decode(bytes, info, flip);
                var encoded = decoded.Save(plan.Target, name);
                report.Record(info.Format, plan.Target, data.Length, encoded.Length);
                report.TextureBytesBefore += data.Length;
                report.TextureBytesAfter += encoded.Length;
                report.TexturesConverted++;
                data = encoded;
                name = TextureFormats.NameFor(name, plan.Target);
                return true;
            }
            catch (Exception ex)
            {
                report.TexturesFailed++;
                report.Problems.Add($"{relative}/{name}: {ex.Message}");
                return false;
            }
        }
        finally
        {
            decoded?.Dispose();
        }
    }

    // a patch tree is authored against one build's members, so it must not travel to a variant
    // sibling cut from a different one. D_STGSLCT_TAB09 is the example: it repairs alpha the iPad
    // set shipped wrong, and xbla's own copy never had that defect, so applying it there replaced
    // a good texture with one that then got flipped as well.
    private static readonly Dictionary<string, string> NoPatches = new Dictionary<string, string>();

    // refuses anything the profile named but can't honour, rather than dropping it quietly
    private static void EmitVariant(
        VariantSpec spec,
        ContentSource source,
        Dictionary<string, string> baseArchives,
        ContentProfile profile,
        BuildReport report)
    {
        var variantArchives = Archives(source);

        Each(spec.Archives, report, (key, local) =>
        {
            // a variant may bring an archive the base has no counterpart for, which is how the
            // console pause menu gets G_PAUSE_P* and G_PAUSE_WIN. It still ships as a sibling, so
            // amFsResolvedVariant gates it with the rest of the screen and it is absent otherwise
            if (!baseArchives.ContainsKey(key))
                local.VariantsWithoutBase++;

            if (!variantArchives.TryGetValue(key, out var path))
            {
                local.Problems.Add($"{key}: source '{spec.Source}' does not have it");
                return;
            }

            var output = Path.Combine(profile.Output, VariantName(key, spec.Name));
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllBytes(output, Emit(path, key, local, profile, NoPatches, spec.Flip));
            local.VariantsEmitted++;
            local.Variants.Add(key + " " + spec.Name);
        });
    }

    // a sparse sibling holding only the named members, in ascending base index. the base decides
    // the index, the sibling only supplies bytes, so nothing the game addresses by IDB_* moves
    private static void EmitMemberVariant(
        VariantSpec spec,
        ContentSource source,
        Dictionary<string, string> baseArchives,
        ContentProfile profile,
        BuildReport report)
    {
        var variantArchives = Archives(source);
        var byArchive = spec.Members
            .GroupBy(m => m.Archive, StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        Each(byArchive, report, (group, local) =>
        {
            var key = group.Key;

            if (!baseArchives.TryGetValue(key, out var basePath))
            {
                local.Problems.Add($"{key}: variant '{spec.Source}' names an archive the base does not have");
                return;
            }

            if (!variantArchives.TryGetValue(key, out var variantPath))
            {
                local.Problems.Add($"{key}: source '{spec.Source}' does not have it");
                return;
            }

            if (!AmbTree.TryReadNested(AmbTree.Normalise(File.ReadAllBytes(basePath)), out var expected))
            {
                local.Problems.Add($"{key}: base does not parse");
                return;
            }

            var variantBytes = AmbTree.Normalise(File.ReadAllBytes(variantPath));
            if (!AmbTree.TryReadNested(variantBytes, out var actual))
            {
                local.Problems.Add($"{key}: source '{spec.Source}' copy does not parse");
                return;
            }

            var picked = new SortedDictionary<int, AppMain.AmbEntry>();

            foreach (var member in group)
            {
                var index = IndexOf(expected, member.Member);
                if (index < 0)
                {
                    local.Problems.Add($"{key}: base has no member '{member.Member}'");
                    continue;
                }

                // the whole point of the sparse sibling is that the index is the base's, so a
                // source whose member list has shifted must be refused rather than renumbered
                if (index >= actual.file_num || !SameMember(expected.files[index], actual.files[index]))
                {
                    var found = index < actual.file_num ? actual.files[index] : "nothing";
                    local.Problems.Add(
                        $"{key}: '{member.Member}' is index {index} in the base but '{found}' in " +
                        $"'{spec.Source}'");
                    continue;
                }

                picked[index] = new AppMain.AmbEntry
                {
                    Name = actual.files[index],
                    Data = Slice(actual, index),
                    Type = actual.types[index],
                    User0 = actual.user0[index],
                    User1 = actual.user1[index],
                };
            }

            if (picked.Count == 0)
                return;

            var sparse = AppMain.writeAmbArchive(picked.Values.ToList(), actual.big_endian);

            var output = Path.Combine(profile.Output, VariantName(key, spec.Name));
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllBytes(output, Emit(sparse, key, local, profile, NoPatches, spec.Flip));

            local.VariantsEmitted++;
            foreach (var index in picked.Keys)
                local.Variants.Add(key + MemberSuffix(index) + " " + spec.Name);
        });
    }

    // "<archive>#<index>", the manifest key a member-level lookup uses
    public static string MemberSuffix(int index)
    {
        return "#" + index.ToString(CultureInfo.InvariantCulture);
    }

    // transcoding renames PNG to DDS, so members compare without the extension, as Check does
    private static bool SameMember(string was, string now)
    {
        return string.Equals(
            Path.GetFileNameWithoutExtension(was),
            Path.GetFileNameWithoutExtension(now),
            StringComparison.OrdinalIgnoreCase);
    }

    // exact, because G_START.AMA and G_START.AMB differ only by extension and SameMember would
    // hand both of them the first one's index
    private static int IndexOf(AppMain.AMS_AMB_HEADER header, string member)
    {
        for (int i = 0; i < header.file_num; i++)
        {
            if (string.Equals(header.files[i], member, StringComparison.OrdinalIgnoreCase))
                return i;
        }

        return -1;
    }

    private static byte[] Slice(AppMain.AMS_AMB_HEADER header, int index)
    {
        var data = new byte[header.lengths[index]];
        Buffer.BlockCopy(header.data, header.offsets[index], data, 0, header.lengths[index]);
        return data;
    }

    // the game can't enumerate its content directory on every head, so this is read back at load time
    public const string ManifestName = "variants.txt";

    private static void WriteManifest(ContentProfile profile, BuildReport report)
    {
        var path = Path.Combine(profile.Output, ManifestName);
        if (report.Variants.Count == 0)
        {
            File.Delete(path);
            return;
        }

        report.Variants.Sort(StringComparer.OrdinalIgnoreCase);
        File.WriteAllLines(path, report.Variants);
    }

    // checks rather than trusts the invariant the whole design rests on: an index that moved would
    // otherwise show up as the wrong texture rather than as a failure
    public static CheckReport Check(ContentSources sources, ContentProfile profile)
    {
        var report = new CheckReport();
        var problems = report.Problems;
        var baseArchives = Archives(sources.Source(profile.Base));
        var variantSuffixes = profile.Variants
            .SelectMany(v => new[] { "." + v.Name + ".AMB", "." + v.Name + ".AMA" })
            .ToArray();
        var appended = 0;

        if (!Directory.Exists(profile.Output))
        {
            problems.Add($"{profile.Output} does not exist; run build first");
            return report;
        }

        foreach (var pair in baseArchives.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
        {
            var output = Path.Combine(profile.Output, pair.Key);
            if (!File.Exists(output))
            {
                problems.Add($"{pair.Key}: missing from the output");
                continue;
            }

            // a loose .AMA has no members to hold in place; being present is the whole check
            if (IsLooseAma(pair.Key))
                continue;

            if (!AmbTree.TryReadNested(File.ReadAllBytes(pair.Value), out var expected))
            {
                problems.Add($"{pair.Key}: base does not parse");
                continue;
            }

            if (!AmbTree.TryReadNested(File.ReadAllBytes(output), out var actual))
            {
                problems.Add($"{pair.Key}: output does not parse");
                continue;
            }

            // appending is fine, losing a member is not
            if (actual.file_num < expected.file_num)
            {
                problems.Add($"{pair.Key}: {expected.file_num} members became {actual.file_num}");
                continue;
            }

            if (actual.file_num > expected.file_num)
                appended += actual.file_num - expected.file_num;

            // compared without the extension: transcoding renames PNG to DDS on purpose
            for (int i = 0; i < expected.file_num; i++)
            {
                var was = Path.GetFileNameWithoutExtension(expected.files[i]);
                var now = Path.GetFileNameWithoutExtension(actual.files[i]);
                if (!string.Equals(was, now, StringComparison.OrdinalIgnoreCase))
                {
                    problems.Add(
                        $"{pair.Key}: index {i} was '{expected.files[i]}', is '{actual.files[i]}'");
                    break;
                }
            }
        }

        foreach (var spec in profile.Variants)
            CheckMemberVariant(spec, baseArchives, profile, report);

        var overlayArchives = profile.Overlays
            .SelectMany(name => Archives(sources.Source(name)).Keys)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // an output file that is none of base, declared variant sibling, or declared overlay
        foreach (var pattern in new[] { "*.AMB", "*.AMA" })
        foreach (var path in Directory.EnumerateFiles(profile.Output, pattern, SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(profile.Output, path).Replace('\\', '/');
            if (baseArchives.ContainsKey(relative) || overlayArchives.Contains(relative))
                continue;
            if (variantSuffixes.Any(s => relative.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
                continue;

            problems.Add($"{relative}: not a base archive, a declared variant or a declared overlay");
        }

        if (appended > 0)
            report.Notes.Add($"{appended} appended members, after every base index");

        CheckTextureFormats(profile, report, PatchIndex(sources, profile));

        return report;
    }

    // the sparse sibling carries no indices of its own: the runtime pairs its members positionally
    // with the ascending manifest indices, so that pairing is what has to hold
    private static void CheckMemberVariant(
        VariantSpec spec,
        Dictionary<string, string> baseArchives,
        ContentProfile profile,
        CheckReport report)
    {
        var byArchive = spec.Members
            .GroupBy(m => m.Archive, StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

        foreach (var group in byArchive)
        {
            var key = group.Key;
            var sibling = Path.Combine(profile.Output, VariantName(key, spec.Name));

            if (!baseArchives.TryGetValue(key, out var basePath))
            {
                report.Problems.Add($"{key}: variant '{spec.Name}' names an archive the base does not have");
                continue;
            }

            if (!File.Exists(sibling))
            {
                report.Problems.Add($"{key}: variant '{spec.Name}' member sibling was not emitted");
                continue;
            }

            if (!AmbTree.TryReadNested(File.ReadAllBytes(basePath), out var expected)
                || !AmbTree.TryReadNested(File.ReadAllBytes(sibling), out var actual))
            {
                report.Problems.Add($"{key}: variant '{spec.Name}' member sibling does not parse");
                continue;
            }

            var indices = group
                .Select(m => IndexOf(expected, m.Member))
                .Where(i => i >= 0)
                .Distinct()
                .OrderBy(i => i)
                .ToList();

            if (indices.Count != actual.file_num)
            {
                report.Problems.Add(
                    $"{key}: variant '{spec.Name}' declares {indices.Count} members, the sibling has " +
                    $"{actual.file_num}");
                continue;
            }

            for (int i = 0; i < indices.Count; i++)
            {
                if (SameMember(expected.files[indices[i]], actual.files[i]))
                    continue;

                report.Problems.Add(
                    $"{key}: variant '{spec.Name}' sibling member {i} is '{actual.files[i]}', base index " +
                    $"{indices[i]} is '{expected.files[indices[i]]}'");
                break;
            }

            report.Notes.Add(
                $"{key}: {indices.Count} of {expected.file_num} members from '{spec.Name}'");
        }
    }

    // every texture in the output, base and variants and overlays alike
    private static void CheckTextureFormats(ContentProfile profile, CheckReport report,
        Dictionary<string, string> patches)
    {
        var pitchAndLinear = 0;
        var textures = 0;
        var authored = 0;
        long gpuBytes = 0;

        foreach (var path in Directory.EnumerateFiles(profile.Output, "*.AMB", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(profile.Output, path).Replace('\\', '/');

            foreach (var member in AmbTree.EnumerateMembers(File.ReadAllBytes(path), relative + "/"))
            {
                if (!TextureTranscode.IsTexture(member.Name))
                    continue;

                var info = TextureFormats.Probe(member.Data);
                textures++;
                gpuBytes += TextureFormats.GpuBytes(info);

                if (info.Format == TextureFormat.Unknown)
                {
                    report.Problems.Add($"{member.Path}: unrecognised texture ({info.Detail ?? "no header"})");
                    continue;
                }

                // an authored member carries the format its author chose, exempt from the accept set
                var stem = member.Path.Substring(0, member.Path.Length - Path.GetExtension(member.Path).Length);
                if (patches.ContainsKey(stem))
                {
                    authored++;
                    continue;
                }

                if (!profile.Accept.Contains(info.Format))
                {
                    report.Problems.Add(
                        $"{member.Path}: {TextureFormats.Token(info.Format)} is not accepted by " +
                        $"'{profile.Name}' ({string.Join(", ", profile.Accept.OrderBy(f => f).Select(TextureFormats.Token))})");
                    continue;
                }

                // amTextureDecode dispatches on extension, so a name/payload mismatch throws at runtime
                var declared = Path.GetExtension(member.Name);
                var actual = TextureFormats.Extension(info.Format);
                if (!string.Equals(declared, actual, StringComparison.OrdinalIgnoreCase))
                {
                    report.Problems.Add(
                        $"{member.Path}: named {declared} but the payload is {TextureFormats.Token(info.Format)}");
                }

                if (info.PitchAndLinear)
                    pitchAndLinear++;
            }
        }

        report.Notes.Add($"{textures} textures, {gpuBytes / 1024.0 / 1024.0:N1} MiB uploaded");

        if (authored > 0)
            report.Notes.Add($"{authored} authored members, exempt from the accept set");

        // passthrough PC bytes we deliberately don't rewrite; FNA needs mppNormaliseDdsFlags to load them
        if (pitchAndLinear > 0)
            report.Notes.Add($"{pitchAndLinear} DDS set both DDSD_PITCH and DDSD_LINEARSIZE");
    }

    // G_COM/RING/RING_MDL.AMB plus variant "pc" -> G_COM/RING/RING_MDL.pc.AMB
    public static string VariantName(string relative, string variant)
    {
        var directory = Path.GetDirectoryName(relative);
        var stem = Path.GetFileNameWithoutExtension(relative);
        var extension = Path.GetExtension(relative);
        var name = $"{stem}.{variant}{extension}";

        return string.IsNullOrEmpty(directory) ? name : Path.Combine(directory, name);
    }
}
