using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sonic4Episode1.Content;

// Unpacking an archive to a directory and packing it back. Nested archives become nested
// directories, each with its own _archive.toml.
internal static class AmbTree
{
    public static AppMain.AMS_AMB_HEADER Read(byte[] bytes)
    {
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = bytes;

        using var stream = new MemoryStream(bytes);
        AppMain.readAmbHeader(stream, ref header);

        return header;
    }

    // the bounds check matters here: the legacy layout has no magic, so a run of plausible-looking
    // integers at the head of a texture could otherwise pass as an archive
    public static bool TryReadNested(byte[] bytes, out AppMain.AMS_AMB_HEADER nested)
    {
        return LooksLikeArchive(bytes, 0, bytes.Length, ".AMB", allowEmpty: true, out nested);
    }

    // a nested #AMB is self-contained, its offsets relative to itself; a nested legacy archive's
    // offsets are relative to the whole containing file, and getting this wrong is silent
    public static bool LooksLikeArchive(byte[] container, int offset, int length, string name,
        out AppMain.AMS_AMB_HEADER nested)
    {
        return LooksLikeArchive(container, offset, length, name, allowEmpty: false, out nested);
    }

    // allowEmpty is for whole files only: accepting a 12-byte empty legacy header for a *member*
    // would let any short run of zeros read as an empty nested archive and lose its bytes
    public static bool LooksLikeArchive(byte[] container, int offset, int length, string name,
        bool allowEmpty, out AppMain.AMS_AMB_HEADER nested)
    {
        nested = null;
        if (offset + length > container.Length)
            return false;

        var minimum = allowEmpty ? 12 : 0x20;
        if (length < minimum)
            return false;

        var slice = new byte[length];
        Buffer.BlockCopy(container, offset, slice, 0, length);

        var selfContained = slice[0] == '#' && slice[1] == 'A' && slice[2] == 'M' && slice[3] == 'B';

        // legacy archives carry no magic at all, so only the extension suggests one
        if (!selfContained && !name.EndsWith(".AMB", StringComparison.OrdinalIgnoreCase))
            return false;

        try
        {
            var header = new AppMain.AMS_AMB_HEADER();
            using (var stream = new MemoryStream(slice))
                AppMain.readAmbHeader(stream, ref header);

            // a legacy child indexes into the parent, a real one into itself
            header.data = selfContained ? slice : container;

            if (!Validate(header, allowEmpty))
                return false;

            nested = header;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool Validate(AppMain.AMS_AMB_HEADER header, bool allowEmpty)
    {
        if (header.file_num < 0 || header.file_num > 100000)
            return false;
        if (header.file_num == 0 && !allowEmpty)
            return false;

        for (int i = 0; i < header.file_num; i++)
        {
            if (header.offsets[i] < 0 || header.lengths[i] < 0)
                return false;
            if ((long)header.offsets[i] + header.lengths[i] > header.data.Length)
                return false;
        }

        return true;
    }

    public static bool LooksLikeArchive(byte[] bytes, string name, out AppMain.AMS_AMB_HEADER nested)
    {
        return LooksLikeArchive(bytes, 0, bytes.Length, name, out nested);
    }

    // every leaf member of an archive, nested ones walked through rather than yielded
    public static IEnumerable<(string Path, int Index, string Name, byte[] Data)> EnumerateMembers(
        byte[] archive, string prefix)
    {
        if (!TryReadNested(archive, out var header))
            return Array.Empty<(string, int, string, byte[])>();

        return EnumerateMembers(header, prefix);
    }

    private static IEnumerable<(string Path, int Index, string Name, byte[] Data)> EnumerateMembers(
        AppMain.AMS_AMB_HEADER header, string prefix)
    {
        for (int i = 0; i < header.file_num; i++)
        {
            var name = header.files[i] ?? i.ToString(CultureInfo.InvariantCulture);

            if (LooksLikeArchive(header.data, header.offsets[i], header.lengths[i], name, out var nested))
            {
                foreach (var member in EnumerateMembers(nested, prefix + name + "/"))
                    yield return member;
            }
            else
            {
                var data = new byte[header.lengths[i]];
                Buffer.BlockCopy(header.data, header.offsets[i], data, 0, header.lengths[i]);
                yield return (prefix + name, i, name, data);
            }
        }
    }

    public static void Unpack(byte[] bytes, string directory, string origin, bool decode = false)
    {
        var header = Read(bytes);
        Unpack(header, directory, origin, decode);
    }

    private static void Unpack(AppMain.AMS_AMB_HEADER header, string directory, string origin,
        bool decode)
    {
        Directory.CreateDirectory(directory);

        var descriptor = new ArchiveDescriptor
        {
            BigEndian = IsBigEndian(header),
            Origin = origin,
        };

        var taken = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < header.file_num; i++)
        {
            var name = header.files[i] ?? i.ToString(CultureInfo.InvariantCulture);

            var isArchive = LooksLikeArchive(
                header.data, header.offsets[i], header.lengths[i], name, out var nested);

            byte[] data = null;
            if (!isArchive)
            {
                data = new byte[header.lengths[i]];
                Buffer.BlockCopy(header.data, header.offsets[i], data, 0, header.lengths[i]);

                // rename the member too, or a PNG payload under a .PVR name sends amTextureDecode to the wrong decoder
                if (decode && TextureTranscode.IsTexture(name))
                {
                    var info = TextureFormats.Probe(data);
                    if (info.Format != TextureFormat.Unknown && info.Format != TextureFormat.Png)
                    {
                        data = TextureTranscode.Encode(data, info, TextureFormat.Png, name);
                        name = TextureFormats.NameFor(name, TextureFormat.Png);
                    }
                }
            }

            descriptor.Files.Add(name);

            var meta = new MemberMeta();
            if (header.types[i] != ArchiveDescriptor.DefaultType)
                meta.Type = header.types[i];
            if (header.user0[i] != 0)
                meta.User0 = header.user0[i];
            if (header.user1[i] != 0)
                meta.User1 = header.user1[i];

            var diskName = DiskName(name, taken);

            if (!meta.IsEmpty)
                descriptor.Members[i] = meta;

            var target = Path.Combine(directory, diskName);
            if (isArchive)
            {
                Unpack(nested, target, null, decode);
            }
            else
            {
                File.WriteAllBytes(target, data);
            }
        }

        descriptor.Save(Path.Combine(directory, ArchiveDescriptor.FileName));
    }

    // rebuilds an archive and everything nested inside it as #AMB, collapsing both nesting
    // conventions into one; member bytes are otherwise untouched
    public static byte[] Normalise(byte[] container, int offset, int length, bool topLevel = true)
    {
        if (!LooksLikeArchive(container, offset, length, ".AMB", topLevel, out var header))
            throw new InvalidDataException("not an archive");

        var entries = new List<AppMain.AmbEntry>(header.file_num);
        for (int i = 0; i < header.file_num; i++)
        {
            byte[] data;
            if (LooksLikeArchive(header.data, header.offsets[i], header.lengths[i], header.files[i], out _))
            {
                data = Normalise(header.data, header.offsets[i], header.lengths[i], topLevel: false);
            }
            else
            {
                data = new byte[header.lengths[i]];
                Buffer.BlockCopy(header.data, header.offsets[i], data, 0, header.lengths[i]);
            }

            entries.Add(new AppMain.AmbEntry
            {
                Name = header.files[i],
                Data = data,
                Type = header.types[i],
                User0 = header.user0[i],
                User1 = header.user1[i],
            });
        }

        return AppMain.writeAmbArchive(entries, IsBigEndian(header));
    }

    public static byte[] Normalise(byte[] archive)
    {
        return Normalise(archive, 0, archive.Length);
    }

    public static byte[] Pack(string directory)
    {
        var descriptorPath = Path.Combine(directory, ArchiveDescriptor.FileName);
        if (!File.Exists(descriptorPath))
            throw new FileNotFoundException($"no {ArchiveDescriptor.FileName} in {directory}");

        var descriptor = ArchiveDescriptor.Load(descriptorPath);
        var entries = new List<AppMain.AmbEntry>(descriptor.Files.Count);
        var diskNames = ResolveDiskNames(descriptor);

        for (int i = 0; i < descriptor.Files.Count; i++)
        {
            var name = descriptor.Files[i];
            var meta = descriptor.MetaFor(i);
            var path = Path.Combine(directory, diskNames[i]);

            byte[] data;
            if (Directory.Exists(path))
                data = Pack(path);
            else if (File.Exists(path))
                data = File.ReadAllBytes(path);
            else
                throw new FileNotFoundException($"member '{name}' of {directory} is missing at {path}");

            entries.Add(new AppMain.AmbEntry
            {
                Name = name,
                Data = data,
                Type = meta?.Type ?? ArchiveDescriptor.DefaultType,
                User0 = meta?.User0 ?? 0,
                User1 = meta?.User1 ?? 0,
            });
        }

        return AppMain.writeAmbArchive(entries, descriptor.BigEndian);
    }

    // the reader decides byte order from the header size at 0x04 but doesn't keep the answer
    public static bool IsBigEndian(AppMain.AMS_AMB_HEADER header)
    {
        if (!header.is_real_amb || header.data.Length < 8)
            return false;

        return BitConverter.ToInt32(header.data, 4) != AppMain.AMB_HEADER_SIZE;
    }

    // unpack and pack derive the on-disk name the same way from the same ordered list, so the
    // descriptor doesn't have to record it
    public static List<string> ResolveDiskNames(ArchiveDescriptor descriptor)
    {
        var taken = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var resolved = new List<string>(descriptor.Files.Count);

        for (int i = 0; i < descriptor.Files.Count; i++)
        {
            var over = descriptor.MetaFor(i)?.Path;
            if (over != null)
            {
                taken.Add(over);
                resolved.Add(over);
                continue;
            }

            resolved.Add(DiskName(descriptor.Files[i], taken));
        }

        return resolved;
    }

    // member names aren't always usable as filenames (e.g. ".\RING.ENO"); the real name stays in
    // the descriptor and a flattened one goes on disk
    public static string DiskName(string name, HashSet<string> taken)
    {
        var candidate = name.Replace('\\', '/');
        var slash = candidate.LastIndexOf('/');
        if (slash >= 0)
            candidate = candidate.Substring(slash + 1);

        foreach (var invalid in Path.GetInvalidFileNameChars())
            candidate = candidate.Replace(invalid, '_');

        if (candidate.Length == 0)
            candidate = "_";

        if (taken.Add(candidate))
            return candidate;

        var stem = Path.GetFileNameWithoutExtension(candidate);
        var extension = Path.GetExtension(candidate);
        for (int n = 1; ; n++)
        {
            var next = $"{stem}~{n.ToString(CultureInfo.InvariantCulture)}{extension}";
            if (taken.Add(next))
                return next;
        }
    }
}
