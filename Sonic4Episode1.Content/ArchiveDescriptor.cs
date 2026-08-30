using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Tomlyn;
using Tomlyn.Model;

namespace Sonic4Episode1.Content;

// _archive.toml, the one file per archive that is actually checked in. Position in `files` is the
// member index that ~1700 inlined literals in the port depend on: reordering or removing breaks, appending doesn't.
internal sealed class MemberMeta
{
    public int? Type;
    public short? User0;
    public short? User1;
    // set only when the member's real name is not usable as a filename, e.g. ".\RING.ENO"
    public string Path;

    public bool IsEmpty => Type == null && User0 == null && User1 == null && Path == null;
}

internal sealed class ArchiveDescriptor
{
    public const string FileName = "_archive.toml";
    public const int DefaultType = -1;

    public bool BigEndian;
    public string Origin;
    public List<string> Files = new List<string>();

    // keyed by index, not name: member names aren't unique within an archive
    public Dictionary<int, MemberMeta> Members = new Dictionary<int, MemberMeta>();

    public MemberMeta MetaFor(int index)
    {
        return Members.TryGetValue(index, out var meta) ? meta : null;
    }

    public static ArchiveDescriptor Load(string path)
    {
        var text = File.ReadAllText(path);
        var table = Toml.ToModel(text, path);
        var descriptor = new ArchiveDescriptor();

        if (table.TryGetValue("endian", out var endian))
            descriptor.BigEndian = string.Equals((string)endian, "big", StringComparison.Ordinal);

        if (table.TryGetValue("origin", out var origin))
            descriptor.Origin = (string)origin;

        if (table.TryGetValue("files", out var files))
        {
            foreach (var entry in (TomlArray)files)
                descriptor.Files.Add((string)entry);
        }

        if (table.TryGetValue("file", out var members))
        {
            foreach (var pair in (TomlTable)members)
            {
                var meta = new MemberMeta();
                var body = (TomlTable)pair.Value;

                if (body.TryGetValue("type", out var type))
                    meta.Type = Convert.ToInt32(type, CultureInfo.InvariantCulture);
                if (body.TryGetValue("user0", out var user0))
                    meta.User0 = Convert.ToInt16(user0, CultureInfo.InvariantCulture);
                if (body.TryGetValue("user1", out var user1))
                    meta.User1 = Convert.ToInt16(user1, CultureInfo.InvariantCulture);
                if (body.TryGetValue("path", out var diskPath))
                    meta.Path = (string)diskPath;

                descriptor.Members[int.Parse(pair.Key, CultureInfo.InvariantCulture)] = meta;
            }
        }

        return descriptor;
    }

    // hand-emitted rather than model-serialised so the diff formatting is controlled; Load parses
    // it back with a real TOML parser, so a malformed emit fails loudly
    public void Save(string path)
    {
        var text = new StringBuilder();
        text.Append("endian = \"").Append(BigEndian ? "big" : "little").Append("\"\n");
        if (!string.IsNullOrEmpty(Origin))
            text.Append("origin = \"").Append(Escape(Origin)).Append("\"\n");

        text.Append('\n');
        if (Files.Count == 0)
        {
            text.Append("files = []\n");
        }
        else
        {
            text.Append("files = [\n");
            foreach (var name in Files)
                text.Append("  \"").Append(Escape(name)).Append("\",\n");
            text.Append("]\n");
        }

        for (int i = 0; i < Files.Count; i++)
        {
            var meta = MetaFor(i);
            if (meta == null || meta.IsEmpty)
                continue;

            // keyed by index, with the name only as a comment: names repeat within an archive
            text.Append("\n[file.").Append(i.ToString(CultureInfo.InvariantCulture))
                .Append("]  # ").Append(Files[i]).Append('\n');
            if (meta.Path != null)
                text.Append("path = \"").Append(Escape(meta.Path)).Append("\"\n");
            if (meta.Type != null)
                text.Append("type = ").Append(meta.Type.Value.ToString(CultureInfo.InvariantCulture)).Append('\n');
            if (meta.User0 != null)
                text.Append("user0 = 0x").Append(((ushort)meta.User0.Value).ToString("X4")).Append('\n');
            if (meta.User1 != null)
                text.Append("user1 = 0x").Append(((ushort)meta.User1.Value).ToString("X4")).Append('\n');
        }

        File.WriteAllText(path, text.ToString(), new UTF8Encoding(false));
    }

    private static string Escape(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
