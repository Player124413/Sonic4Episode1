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
// base can diverge in member count/order for an archive that must never be selected as a sibling
internal sealed class VariantSpec
{
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

    public static MemberRef Parse(string profile, string source, string declared)
    {
        var hash = declared.IndexOf('#');
        if (hash <= 0 || hash == declared.Length - 1)
        {
            throw new InvalidDataException(
                $"profile '{profile}' variant '{source}': '{declared}' is not <archive>#<member>");
        }

        return new MemberRef(
            declared.Substring(0, hash).Replace('\\', '/'),
            declared.Substring(hash + 1));
    }
}

internal sealed class ContentProfile
{
    public string Name;
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

    public string Audio = "passthrough";
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
// can't read; only fna reads anything beyond PNG right now
internal static class HeadCapabilities
{
    private static readonly TextureFormat[] FnaFormats =
    {
        TextureFormat.Png, TextureFormat.Bc1, TextureFormat.Bc2, TextureFormat.Bc3,
        TextureFormat.Rgba32, TextureFormat.Bgra32,
    };

    private static readonly TextureFormat[] PngOnly = { TextureFormat.Png };

    public static readonly string[] Known = { "fna", "xna", "wp7", "uwp", "android" };

    public static bool Reads(string head, TextureFormat format)
    {
        var formats = string.Equals(head, "fna", StringComparison.OrdinalIgnoreCase)
            ? FnaFormats
            : PngOnly;

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

    public ContentSource Source(string name)
    {
        if (!Sources.TryGetValue(name, out var source))
            throw new InvalidDataException($"no source '{name}' in {FileName}");

        if (!Directory.Exists(source.Root))
            throw new DirectoryNotFoundException($"source '{name}' root does not exist: {source.Root}");

        return source;
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

    private static VariantSpec ReadVariant(string profile, TomlTable body)
    {
        if (!body.TryGetValue("source", out var source))
            throw new InvalidDataException($"profile '{profile}' has a variant with no source");

        var spec = new VariantSpec { Source = (string)source };
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
                spec.Members.Add(MemberRef.Parse(profile, spec.Source, (string)member));
        }

        if (spec.Archives.Count == 0 && spec.Members.Count == 0)
            throw new InvalidDataException(
                $"profile '{profile}' variant '{spec.Source}' names no archives or members; list them " +
                "explicitly");

        // a whole-archive swap and a member swap of the same archive would both want the same
        // sibling filename, and the member one would lose
        foreach (var member in spec.Members)
        {
            if (spec.Archives.Contains(member.Archive, StringComparer.OrdinalIgnoreCase))
                throw new InvalidDataException(
                    $"profile '{profile}' variant '{spec.Source}': {member.Archive} is named as both a " +
                    "whole archive and a member swap");
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
        foreach (var format in profile.Encode)
        {
            if (!profile.Accept.Contains(format))
                throw new InvalidDataException(
                    $"profile '{profile.Name}' would encode {TextureFormats.Token(format)} but does " +
                    "not accept it");
        }

        foreach (var head in profile.Heads)
        {
            if (Array.IndexOf(HeadCapabilities.Known, head.ToLowerInvariant()) < 0)
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

        if (table.TryGetValue("profile", out var profiles))
        {
            foreach (var pair in (TomlTable)profiles)
            {
                var body = (TomlTable)pair.Value;
                var profile = new ContentProfile { Name = pair.Key };

                if (!body.TryGetValue("base", out var profileBase))
                    throw new InvalidDataException($"profile '{pair.Key}' has no base");
                profile.Base = (string)profileBase;

                if (body.TryGetValue("variants", out var variants))
                {
                    foreach (var variant in (TomlArray)variants)
                        profile.Variants.Add(ReadVariant(pair.Key, (TomlTable)variant));
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

                if (body.TryGetValue("texture", out _))
                    throw new InvalidDataException(
                        $"profile '{pair.Key}' still uses the old scalar 'texture'; replace it with " +
                        "textures = { encode = [...], accept = [...] }");

                ReadTextures(pair.Key, body, profile);

                if (body.TryGetValue("heads", out var heads))
                {
                    foreach (var head in (TomlArray)heads)
                        profile.Heads.Add((string)head);
                }

                Validate(profile);

                if (body.TryGetValue("audio", out var audio))
                    profile.Audio = (string)audio;

                profile.Output = body.TryGetValue("output", out var output)
                    ? Path.GetFullPath((string)output, root)
                    : Path.Combine(root, "Content.Build", pair.Key);

                sources.Profiles[pair.Key] = profile;
            }
        }

        return sources;
    }
}
