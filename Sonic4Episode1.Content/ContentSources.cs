using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tomlyn;
using Tomlyn.Model;

namespace Sonic4Episode1.Content;

// content-sources.toml: where each provenance lives and what each profile emits. A profile names
// one `base`, never reordered, and any number of `variants`, only ever appended
internal sealed class ContentSource
{
    public string Name;
    public string Root;

    // drops the never-loaded X_M.AMB when X.AMB exists; must not be a glob, since _M means
    // something else entirely on the map archives (ZONE1_M.AMB is a collision map, no sibling)
    public string DropSiblingSuffix;
}

// an alternative build of specific archives, emitted as <STEM>.<source>.AMB beside the base. The
// archive list is explicit rather than "everything the source also has", since a source and the
// base can diverge in member count/order for an archive that must never be selected as a sibling.
// Declared once as a [group.*] and referenced by name, so two profiles cannot drift apart
internal sealed class VariantSpec
{
    public string Group;
    public string Source;
    // what the emitted sibling is called, and what am_fs_variant has to be set to. defaults to the
    // source, but a mode can be assembled from more than one provenance
    public string Name;

    // console DDS is stored the other way up from what the GL layer's UVs expect. The 2D sprite
    // path samples through that layer and needs the flip; models are drawn by the stdshader path
    // and must not be flipped. Nothing in the archive says which, hence declaring it here.
    public bool Flip;

    public List<string> Archives = new List<string>();

    // `<archive>#<member>` for a shared archive where only some members may move, written to a
    // sparse sibling instead of a whole one. CPIT_MAIN is the case: G_START can go console while
    // G_FIX and G_RSLT, whose ids differ between builds, stay on the base
    public List<MemberRef> Members = new List<MemberRef>();
}

internal readonly struct MemberRef
{
    public readonly string Archive;
    public readonly string Member;

    public MemberRef(string archive, string member)
    {
        Archive = archive;
        Member = member;
    }

    public static MemberRef Parse(string group, string declared)
    {
        var hash = declared.IndexOf('#');
        if (hash <= 0 || hash == declared.Length - 1)
        {
            throw new InvalidDataException(
                $"group '{group}': '{declared}' is not <archive>#<member>");
        }

        return new MemberRef(
            declared.Substring(0, hash).Replace('\\', '/'),
            declared.Substring(hash + 1));
    }
}

// loose files copied into every profile's tree untouched. Declared once at the top level rather
// than per profile: what a head needs beside the archives does not vary with the texture set
internal sealed class AssetSpec
{
    public string Source;

    // named entries under the source root, each a directory copied recursively or a single file.
    // empty means the whole root, which is what `into` is for
    public List<string> Paths = new List<string>();

    // where the copy lands in the tree, relative to its root. null puts each path at its own name
    public string Into;
}

internal sealed class ContentProfile
{
    public string Name;

    // the assembly this profile takes its base, overlays, patches and variants from; only the
    // texture set, heads and Ship differ between a profile and what it extends
    public string Extends;

    public string Base;
    public List<VariantSpec> Variants = new List<VariantSpec>();
    // sources whose archives are emitted at their own paths, for content that has no counterpart in
    // the base rather than an alternative to one
    public List<string> Overlays = new List<string>();

    // Encode is preference order and may be empty (the identity profile, passthrough); Accept
    // bounds what the output tree may contain regardless
    public List<TextureFormat> Encode = new List<TextureFormat>();
    public HashSet<TextureFormat> Accept = new HashSet<TextureFormat>();
    public List<string> Heads = new List<string>();

    // authored replacements laid out exactly as `unpack` writes a tree; a patch replaces bytes only, the member keeps its index
    public List<string> Patches = new List<string>();

    // an assembly declares no textures of its own and only exists to be extended; building one
    // would emit an untranscoded tree nobody asked for
    public bool Abstract;

    // false keeps a profile out of a Release build. The raw passthrough profile is the only one,
    // and it is the only place PVR is allowed to survive
    public bool Ship = true;

    public string Output;

    public string DescribeTextures()
    {
        var encode = Encode.Count == 0 ? "none" : string.Join(",", Encode.Select(TextureFormats.Token));
        var accept = Accept.Count == 0 ? "none"
            : string.Join(",", Accept.OrderBy(f => f).Select(TextureFormats.Token));

        return $"encode={encode} accept={accept}";
    }
}

// what each head can actually hand to the GPU, so a profile can't declare a format its consumers
// can't read. wasm lists both sets because it is the one head that meets devices with only BC and
// devices with only ASTC; which tree it gets is a packaging choice, not a capability
// (docs/webgl-texture-capabilities.md)
internal static class HeadCapabilities
{
    private static readonly TextureFormat[] Bc =
    {
        TextureFormat.Png, TextureFormat.Bc1, TextureFormat.Bc2, TextureFormat.Bc3,
        TextureFormat.Rgba32, TextureFormat.Bgra32,
    };

    private static readonly TextureFormat[] BcAndAstc =
    {
        TextureFormat.Png, TextureFormat.Bc1, TextureFormat.Bc2, TextureFormat.Bc3,
        TextureFormat.Rgba32, TextureFormat.Bgra32,
        TextureFormat.Astc4x4, TextureFormat.Astc6x6,
    };

    private static readonly TextureFormat[] PngAndAstc =
    {
        TextureFormat.Png, TextureFormat.Astc4x4, TextureFormat.Astc6x6,
    };

    private static readonly TextureFormat[] PngOnly = { TextureFormat.Png };

    private static readonly Dictionary<string, TextureFormat[]> ByHead =
        new Dictionary<string, TextureFormat[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "fna", Bc },
            { "wasm", BcAndAstc },
            // measured, not assumed: the Mali row in docs/webgl-texture-capabilities.md has no
            // s3tc, and Adreno is the same. android reads DDS through DdsKtxXna, but reading it
            // and uploading it are different questions and this table is about the second
            { "android", PngAndAstc },
            { "ios", PngAndAstc },
            { "xna", PngOnly },
            { "wp7", PngOnly },
            { "uwp", PngOnly },
        };

    public static readonly string[] Known = ByHead.Keys.OrderBy(k => k, StringComparer.Ordinal).ToArray();

    public static bool Knows(string head)
    {
        return ByHead.ContainsKey(head);
    }

    public static bool Reads(string head, TextureFormat format)
    {
        var formats = ByHead.TryGetValue(head, out var known) ? known : PngOnly;

        return Array.IndexOf(formats, format) >= 0;
    }
}

internal sealed class ContentSources
{
    public const string FileName = "content-sources.toml";

    public Dictionary<string, ContentSource> Sources =
        new Dictionary<string, ContentSource>(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, ContentProfile> Profiles =
        new Dictionary<string, ContentProfile>(StringComparer.OrdinalIgnoreCase);

    public List<AssetSpec> Assets = new List<AssetSpec>();

    public ContentSource Source(string name)
    {
        if (!Sources.TryGetValue(name, out var source))
            throw new InvalidDataException($"no source '{name}' in {FileName}");

        if (!Directory.Exists(source.Root))
            throw new DirectoryNotFoundException(
                $"source '{name}' root does not exist: {source.Root}{Environment.NewLine}" +
                "see Sources/README.md for what to copy in there");

        return source;
    }

    public IEnumerable<string> Extending(string name)
    {
        return Profiles.Values
            .Where(p => string.Equals(p.Extends, name, StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Name)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase);
    }

    public ContentProfile Profile(string name)
    {
        if (!Profiles.TryGetValue(name, out var profile))
            throw new InvalidDataException(
                $"no profile '{name}' in {FileName}; known: {string.Join(", ", Profiles.Keys)}");

        return profile;
    }

    // Walk up from the working directory so the tool can be run from anywhere in the tree.
    public static string Locate(string start)
    {
        var directory = new DirectoryInfo(Path.GetFullPath(start));
        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, FileName);
            if (File.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        return null;
    }

    private static AssetSpec ReadAsset(TomlTable body)
    {
        if (!body.TryGetValue("source", out var source))
            throw new InvalidDataException("an [[assets]] entry has no source");

        var spec = new AssetSpec { Source = (string)source };
        spec.Into = body.TryGetValue("into", out var into) ? (string)into : null;

        if (body.TryGetValue("paths", out var paths))
        {
            foreach (var path in (TomlArray)paths)
                spec.Paths.Add(((string)path).Replace('\\', '/'));
        }

        if (spec.Paths.Count == 0 && spec.Into == null)
            throw new InvalidDataException(
                $"[[assets]] on '{spec.Source}' needs either paths or into, or it would copy the " +
                "whole source root over the tree");

        return spec;
    }

    private static VariantSpec ReadGroup(string group, TomlTable body)
    {
        if (!body.TryGetValue("source", out var source))
            throw new InvalidDataException($"group '{group}' has no source");

        var spec = new VariantSpec { Group = group, Source = (string)source };
        spec.Name = body.TryGetValue("name", out var name) ? (string)name : spec.Source;
        spec.Flip = body.TryGetValue("flip", out var flip) && (bool)flip;

        if (body.TryGetValue("archives", out var archives))
        {
            foreach (var archive in (TomlArray)archives)
                spec.Archives.Add(((string)archive).Replace('\\', '/'));
        }

        if (body.TryGetValue("members", out var members))
        {
            foreach (var member in (TomlArray)members)
                spec.Members.Add(MemberRef.Parse(group, (string)member));
        }

        if (spec.Archives.Count == 0 && spec.Members.Count == 0)
            throw new InvalidDataException(
                $"group '{group}' names no archives or members; list them explicitly");

        // a whole-archive swap and a member swap of the same archive would both want the same
        // sibling filename, and the member one would lose
        foreach (var member in spec.Members)
        {
            if (spec.Archives.Contains(member.Archive, StringComparer.OrdinalIgnoreCase))
                throw new InvalidDataException(
                    $"group '{group}': {member.Archive} is named as both a whole archive and a " +
                    "member swap");
        }

        return spec;
    }

    private static void ReadTextures(string name, TomlTable body, ContentProfile profile)
    {
        if (!body.TryGetValue("textures", out var textures))
            throw new InvalidDataException(
                $"profile '{name}' has no textures = {{ encode = [...], accept = [...] }}");

        var table = (TomlTable)textures;
        profile.Encode.AddRange(ReadFormats(name, table, "encode"));
        foreach (var format in ReadFormats(name, table, "accept"))
            profile.Accept.Add(format);
    }

    private static IEnumerable<TextureFormat> ReadFormats(string name, TomlTable table, string key)
    {
        if (!table.TryGetValue(key, out var declared))
            yield break;

        foreach (var token in (TomlArray)declared)
        {
            if (!TextureFormats.TryParse((string)token, out var format))
                throw new InvalidDataException(
                    $"profile '{name}' textures.{key}: '{token}' is not a texture format");

            yield return format;
        }
    }

    private static void Validate(ContentProfile profile)
    {
        if (profile.Abstract)
        {
            if (profile.Heads.Count > 0)
                throw new InvalidDataException(
                    $"profile '{profile.Name}' declares no textures, so it is an assembly to be " +
                    "extended and cannot name heads");

            return;
        }

        foreach (var format in profile.Encode)
        {
            if (!profile.Accept.Contains(format))
                throw new InvalidDataException(
                    $"profile '{profile.Name}' would encode {TextureFormats.Token(format)} but does " +
                    "not accept it");
        }

        // the one format that must never reach a player. PVR only survives in the passthrough
        // profile, which exists to isolate a texture bug from the rest of the pipeline
        if (profile.Ship && profile.Accept.Contains(TextureFormat.Pvr))
            throw new InvalidDataException(
                $"profile '{profile.Name}' accepts pvr but is a shipping profile; set ship = false " +
                "or drop pvr from accept");

        foreach (var head in profile.Heads)
        {
            if (!HeadCapabilities.Knows(head))
                throw new InvalidDataException(
                    $"profile '{profile.Name}' names an unknown head '{head}'; known: " +
                    string.Join(", ", HeadCapabilities.Known));

            foreach (var format in profile.Accept)
            {
                if (!HeadCapabilities.Reads(head, format))
                    throw new InvalidDataException(
                        $"profile '{profile.Name}' accepts {TextureFormats.Token(format)} but head " +
                        $"'{head}' cannot load it");
            }
        }
    }

    public static ContentSources Load(string path)
    {
        var table = Toml.ToModel(File.ReadAllText(path), path);
        var sources = new ContentSources();
        var root = Path.GetDirectoryName(Path.GetFullPath(path));

        if (table.TryGetValue("source", out var declared))
        {
            foreach (var pair in (TomlTable)declared)
            {
                var body = (TomlTable)pair.Value;
                if (!body.TryGetValue("root", out var sourceRoot))
                    throw new InvalidDataException($"source '{pair.Key}' has no root");

                sources.Sources[pair.Key] = new ContentSource
                {
                    Name = pair.Key,
                    Root = Path.GetFullPath((string)sourceRoot, root),
                    DropSiblingSuffix = body.TryGetValue("drop_sibling_suffix", out var drop)
                        ? (string)drop
                        : null,
                };
            }
        }

        var groups = new Dictionary<string, VariantSpec>(StringComparer.OrdinalIgnoreCase);
        if (table.TryGetValue("group", out var declaredGroups))
        {
            foreach (var pair in (TomlTable)declaredGroups)
                groups[pair.Key] = ReadGroup(pair.Key, (TomlTable)pair.Value);
        }

        if (table.TryGetValue("assets", out var declaredAssets))
        {
            foreach (var entry in (TomlTableArray)declaredAssets)
                sources.Assets.Add(ReadAsset(entry));
        }

        if (!table.TryGetValue("profile", out var profiles))
            return sources;

        // two passes: a profile may extend one declared after it
        var bodies = new Dictionary<string, TomlTable>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in (TomlTable)profiles)
            bodies[pair.Key] = (TomlTable)pair.Value;

        foreach (var pair in bodies)
        {
            var profile = ReadProfile(pair.Key, pair.Value, bodies, groups, root);
            Validate(profile);
            sources.Profiles[pair.Key] = profile;
        }

        return sources;
    }

    private static ContentProfile ReadProfile(string name, TomlTable body,
        Dictionary<string, TomlTable> bodies, Dictionary<string, VariantSpec> groups, string root)
    {
        var profile = new ContentProfile { Name = name };
        var assembly = body;

        if (body.TryGetValue("extends", out var extends))
        {
            profile.Extends = (string)extends;

            if (!bodies.TryGetValue(profile.Extends, out assembly))
                throw new InvalidDataException(
                    $"profile '{name}' extends '{profile.Extends}', which is not declared");

            if (assembly.ContainsKey("extends"))
                throw new InvalidDataException(
                    $"profile '{name}' extends '{profile.Extends}', which itself extends something; " +
                    "one level only");
        }

        ReadAssembly(name, assembly, groups, profile);

        if (body.TryGetValue("texture", out _))
            throw new InvalidDataException(
                $"profile '{name}' still uses the old scalar 'texture'; replace it with " +
                "textures = { encode = [...], accept = [...] }");

        if (body.TryGetValue("textures", out _))
            ReadTextures(name, body, profile);
        else
            profile.Abstract = true;

        if (body.TryGetValue("heads", out var heads))
        {
            foreach (var head in (TomlArray)heads)
                profile.Heads.Add((string)head);
        }

        if (body.TryGetValue("ship", out var ship))
            profile.Ship = (bool)ship;

        profile.Output = body.TryGetValue("output", out var output)
            ? Path.GetFullPath((string)output, root)
            : Path.Combine(root, "Content.Build", name);

        return profile;
    }

    // base, overlays, patches and variants all come from the extended profile when there is one,
    // so two flavours of the same tree cannot describe different content
    private static void ReadAssembly(string name, TomlTable body,
        Dictionary<string, VariantSpec> groups, ContentProfile profile)
    {
        if (!body.TryGetValue("base", out var profileBase))
            throw new InvalidDataException($"profile '{name}' has no base");

        profile.Base = (string)profileBase;

        if (body.TryGetValue("variants", out var variants))
        {
            foreach (var variant in (TomlArray)variants)
            {
                if (variant is TomlTable)
                    throw new InvalidDataException(
                        $"profile '{name}' declares a variant inline; variants now name [group.*] " +
                        "entries so two profiles cannot drift apart");

                var group = (string)variant;
                if (!groups.TryGetValue(group, out var spec))
                    throw new InvalidDataException(
                        $"profile '{name}' names variant group '{group}', which is not declared; " +
                        $"known: {string.Join(", ", groups.Keys)}");

                profile.Variants.Add(spec);
            }
        }

        if (body.TryGetValue("overlays", out var overlays))
        {
            foreach (var overlay in (TomlArray)overlays)
                profile.Overlays.Add((string)overlay);
        }

        if (body.TryGetValue("patches", out var patches))
        {
            foreach (var patch in (TomlArray)patches)
                profile.Patches.Add((string)patch);
        }
    }
}
