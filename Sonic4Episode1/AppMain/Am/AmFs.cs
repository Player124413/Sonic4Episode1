using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

internal partial class AppMain
{
    internal class AMS_FS
    {
        public string dir;
        public int count;
        public string[] files;
        public int[] types;
        public short[] user0;
        public short[] user1;
        public sbyte[] flag;
        public sbyte type;
        public byte[] data;
        public int[] offsets;
        public int[] lengths;
        public sbyte stat;
        public string file_name;
        public Stream stream;
        public byte[] read_buf;
        public Task<byte[]> readTask;

        // set when a read failed and this request is parked; amFsRetryRequest clears it
        public Exception error;
        public AMS_AMB_HEADER amb_header;
        public bool is_real_amb = false;
        public bool big_endian = false;

        // null follows am_fs_variant; anything else pins this request to that variant
        public string variant;

        // 0 not started, 1 parent read, 2 sibling read, 3 done
        public int phase;

        public int[] member_variant;
        public string member_variant_name;
        public AMS_AMB_HEADER member_variant_amb;
        public int[] member_variant_index;

        public static explicit operator AMS_AMB_HEADER(AMS_FS fs)
        {
            return readAMBFile(fs);
        }

        public void makeAmbHeader()
        {
            this.amb_header = new AMS_AMB_HEADER();
            this.amb_header.dir = this.dir;
            this.amb_header.file_num = this.count;
            this.amb_header.files = new string[this.files.Length];
            this.amb_header.types = new int[this.types.Length];
            this.amb_header.user0 = new short[this.files.Length];
            this.amb_header.user1 = new short[this.files.Length];
            this.amb_header.flag = new sbyte[this.flag.Length];
            this.amb_header.offsets = new int[this.files.Length];
            this.amb_header.lengths = new int[this.files.Length];
            this.amb_header.data = this.data;
            this.amb_header.buf = new object[this.count];
            Array.Copy(files, 0, amb_header.files, 0, this.files.Length);
            Array.Copy(types, 0, amb_header.types, 0, this.types.Length);
            if (user0 != null)
                Array.Copy(user0, 0, amb_header.user0, 0, this.user0.Length);
            if (user1 != null)
                Array.Copy(user1, 0, amb_header.user1, 0, this.user1.Length);
            Buffer.BlockCopy(flag, 0, amb_header.flag, 0, this.flag.Length);
            Buffer.BlockCopy(offsets, 0, amb_header.offsets, 0, this.offsets.Length * 4);
            Buffer.BlockCopy(lengths, 0, amb_header.lengths, 0, this.lengths.Length * 4);
            this.amb_header.is_real_amb = is_real_amb;
            this.amb_header.big_endian = big_endian;
            this.amb_header.member_variant_amb = this.member_variant_amb;
            this.amb_header.member_variant_index = this.member_variant_index;
            amPreLoadAmbItems(this.amb_header);
        }
    }

    public static bool amFsIsComplete(AMS_FS cdfsp)
    {
        return cdfsp.stat == 3;
    }

    public static AMS_FS amFsReadBackground(string file_name)
    {
        return amFsReadBackground(file_name, null);
    }

    public static AMS_FS amFsReadBackgroundAs(string file_name, string variant)
    {
        return amFsReadBackground(file_name, variant);
    }

    private static AMS_FS amFsReadBackground(string file_name, string variant)
    {
        AMS_FS amsFs;
        if (ams_fsList.Count > 0 && lastReadAMS_FS != null && lastReadAMS_FS.file_name == file_name
            && lastReadAMS_FS.variant == variant)
        {
            amsFs = ams_fsList.First.Value;
            amFsExecuteBackgroundRead();
        }
        else
        {
            amsFs = new AMS_FS();
            amsFs.file_name = file_name;
            amsFs.variant = variant;
            amsFs.stat = 2;
            ams_fsList.AddLast(amsFs);
            lastReadAMS_FS = amsFs;
        }

        return amsFs;
    }

    public static AMS_AMB_HEADER readAMBFile(AMS_FS fs)
    {
        if (fs.amb_header == null)
            fs.makeAmbHeader();
        return fs.amb_header;
    }

    public static AMS_AMB_HEADER readAMBFile(object data)
    {
        switch (data)
        {
            case AMS_AMB_HEADER _:
                return (AMS_AMB_HEADER) data;
            case AmbChunk chunk:
                return readAMBFile(chunk);
            default:
                return readAMBFile((AMS_FS) data);
        }
    }

    internal static AMS_AMB_HEADER readAMBFile(AmbChunk buf)
    {
        AMS_AMB_HEADER amsAmbHeader = searchPreloadedAmb(buf.amb, buf.offset);
        if (amsAmbHeader != null)
            return amsAmbHeader;
        AMS_AMB_HEADER amb = new AMS_AMB_HEADER();
        using (Stream input = new MemoryStream(buf.array, buf.offset, buf.length))
        {
            amb.data = buf.array;

            // the legacy layout's offsets already reach across the whole parent, only #AMB numbers
            // its members from its own start
            amb.data_base = buf.amb.is_real_amb ? buf.offset : 0;

            readAmbHeader(input, ref amb);

            amb.parent = buf.amb;
            amPreLoadAmbItems(amb);
        }

        return amb;
    }

    public const int AMB_FILE_ID = 0x424D4123;
    public const int AMB_HEADER_SIZE = 0x20;

    // amBindConv stamps file_id[0] with '!' once it has converted an archive in place, and asserts
    // only on file_id[1..3] == "AMB". so must we
    private const int AMB_FILE_ID_MASK = unchecked((int)0xFFFFFF00);

    // flag is a byte array precisely so it can be read before the byte order is known, which is why
    // amBindConv's swap list covers every scalar except it
    private const int AMB_FLAG_OFFSET = 0x0C;

    // the caller owns `stream`, so nothing here is disposed: a BinaryReader holds no resource of
    // its own, and disposing one would close a stream that outlives this call
    public static void readAmbHeader(Stream stream, ref AMS_AMB_HEADER amb)
    {
        var reader = new BinaryReader(stream);

        var fileId = reader.ReadInt32();
        if ((fileId & AMB_FILE_ID_MASK) != (AMB_FILE_ID & AMB_FILE_ID_MASK))
        {
            readLegacyAmbHeader(reader, ref amb, fileId);
            return;
        }

        var headerSize = reader.ReadInt32();
        var sizeSaysBig = headerSize != AMB_HEADER_SIZE;
        if (sizeSaysBig && BinaryPrimitives.ReverseEndianness(headerSize) != AMB_HEADER_SIZE)
            throw new InvalidDataException($"#AMB header size 0x{headerSize:X8} is not 0x20 in either byte order");

        stream.Position = AMB_FLAG_OFFSET;
        var flag = reader.ReadBytes(4);
        var bigEndian = (flag[0] & AMD_AMB_FLAG0_ENDIAN_BIG) != 0;

        // 0x20 and 0x20000000 cannot both be the header size, so a disagreement means we have
        // misparsed the header rather than merely mislabelled it
        if (bigEndian != sizeSaysBig)
            throw new InvalidDataException($"#AMB flag[0] 0x{flag[0]:X2} disagrees with header size 0x{headerSize:X8}");

        amb.big_endian = bigEndian;
        readRealAmbHeader(bigEndian ? new EndianBinaryReader(stream, bigEndian: true) : reader, ref amb, flag);
    }

    private static void readRealAmbHeader(BinaryReader reader, ref AMS_AMB_HEADER amb, byte[] flag)
    {
        reader.BaseStream.Position = 0x10;
        amb.file_num = reader.ReadInt32();
        var entryTableOffset = reader.ReadInt32();
        reader.BaseStream.Position = 0x1C;
        var stringTableOffset = reader.ReadInt32();

        amb.is_real_amb = true;
        amb.files = new string[amb.file_num];
        amb.types = new int[amb.file_num];
        amb.user0 = new short[amb.file_num];
        amb.user1 = new short[amb.file_num];
        amb.offsets = new int[amb.file_num];
        amb.lengths = new int[amb.file_num];
        amb.buf = new object[amb.file_num];
        amb.flag = new sbyte[flag.Length];
        for (int i = 0; i < flag.Length; i++)
        {
            amb.flag[i] = (sbyte)flag[i];
        }
        for (int i = 0; i < amb.file_num; i++)
        {
            reader.BaseStream.Seek(entryTableOffset + (i * 0x10), SeekOrigin.Begin);
            amb.offsets[i] = reader.ReadInt32();
            amb.lengths[i] = reader.ReadInt32();
            amb.types[i] = reader.ReadInt32();
            amb.user0[i] = reader.ReadInt16();
            amb.user1[i] = reader.ReadInt16();
            if (stringTableOffset != 0)
            {
                reader.BaseStream.Seek(stringTableOffset + (i * 0x20), SeekOrigin.Begin);
                amb.files[i] = readChars(reader);
            }
            else
            {
                amb.files[i] = i.ToString();
            }
        }
    }

    private static void readLegacyAmbHeader(BinaryReader reader, ref AMS_AMB_HEADER amb, int fileNum)
    {
        amb.file_num = fileNum;
        amb.files = new string[amb.file_num];
        amb.types = new int[amb.file_num];
        // legacy layout has no per-file user data, these just stay zero
        amb.user0 = new short[amb.file_num];
        amb.user1 = new short[amb.file_num];
        amb.offsets = new int[amb.file_num];
        amb.lengths = new int[amb.file_num];
        amb.buf = new object[amb.file_num];
        int num = reader.ReadInt32();
        amb.flag = new sbyte[num];
        for (int i = 0; i < num; i++)
        {
            amb.flag[i] = reader.ReadSByte();
        }

        for (int j = 0; j < amb.file_num; j++)
        {
            amb.files[j] = reader.ReadString();
            amb.types[j] = reader.ReadSByte();
        }

        for (int k = 0; k < amb.file_num; k++)
        {
            amb.offsets[k] = reader.ReadInt32();
        }

        for (int l = 0; l < amb.file_num; l++)
        {
            amb.lengths[l] = reader.ReadInt32();
        }
    }


    public static AMS_AMB_HEADER searchPreloadedAmb(
        AMS_AMB_HEADER amb,
        int offset)
    {
        for (int index = 0; index < amb.file_num; ++index)
        {
            if (amb.data_base + amb.offsets[index] == offset && amb.buf[index] != null)
                return (AMS_AMB_HEADER) amb.buf[index];
        }

        for (int index = 0; index < amb.file_num; ++index)
        {
            if (amb.buf[index] is AMS_AMB_HEADER)
            {
                AMS_AMB_HEADER amsAmbHeader = searchPreloadedAmb((AMS_AMB_HEADER) amb.buf[index], offset);
                if (amsAmbHeader != null)
                    return amsAmbHeader;
            }
        }

        return null;
    }

    // member names carry Windows separators (".\RING.ENO"), and Path only splits on '\' when it
    // happens to be running on Windows
    private static readonly char[] mpp_amb_name_separators = new char[] { '/', '\\' };

    internal static string mppAmbMemberName(string member)
    {
        if (member == null)
            return null;

        int cut = member.LastIndexOfAny(mpp_amb_name_separators);
        return cut < 0 ? member : member.Substring(cut + 1);
    }

    internal static string mppAmbMemberBaseName(string member)
    {
        string name = mppAmbMemberName(member);
        if (name == null)
            return null;

        int dot = name.LastIndexOf('.');
        return dot < 0 ? name : name.Substring(0, dot);
    }

    private static bool mppIsNnMotionMember(string extension)
    {
        switch (extension.ToUpperInvariant())
        {
            case ".INM":
            case ".INV":
            case ".ZNM":
            case ".ZNV":
            case ".ENM":
            case ".ENV":
                return true;
            default:
                return false;
        }
    }

    public static void amPreLoadAmbItems(AMS_AMB_HEADER amb)
    {
        for (int index = 0; index < amb.files.Length; ++index)
        {
            // an absent member is offset 0 and size 0, and has nothing to parse. ZONE2_M_M gets
            // away with reaching here today only because its absent members have empty names too
            AmbChunk ambChunk = amBindGet(amb, index, out string _);
            if (ambChunk == null)
                continue;

            string extension = Path.GetExtension(amb.files[index]);
            if (mppIsNnMotionMember(extension))
            {
                NNS_MOTION motion;
                amMotionSetup(out motion, ambChunk);
                amb.buf[index] = motion;
            }
            else if (extension.Equals(g_dm_buy_screen_amb_ext, StringComparison.OrdinalIgnoreCase))
            {
                amb.buf[index] = readAMBFile(ambChunk);
                amPreLoadAmbItems((AMS_AMB_HEADER) amb.buf[index]);
            }
            else if (extension.Equals(".AME", StringComparison.OrdinalIgnoreCase))
                amb.buf[index] = readAMEfile(ambChunk);
        }
    }

    // "<path> <variant>" lines, one per archive the content build emitted an alt version of.
    // "<path>#<index> <variant>" for a shared archive only some of whose members moved
    private static Dictionary<string, string> am_fs_variants;

    private static Dictionary<string, int[]> am_fs_member_variants;

    // console-grade assets from the xbla dump. Note this tier still carries MOBILE-ERA LEVELS:
    // xbla's ZONE22 is the pinball minigame, cell for cell identical to mobile's. Console-grade
    // asset mode, not a statement about which levels you get
    public const string AMD_FS_VARIANT_CONSOLE = "console";

    // console-grade assets from the retail PC dump, AND the PC-era levels that go with them, which
    // are a different thing from the mobile ones on some stages (ZONE22 is the card stage, not the
    // minigame). Preferred over console where present; see am_fs_variant_order
    public const string AMD_FS_VARIANT_PC = "pc";

    // what a screen asks for when its act tables only describe the base archives. a screen that
    // has not been ported to console yet has to pin this, or a globally flipped shared archive
    // reaches it with ids it cannot index
    public const string AMD_FS_VARIANT_BASE = "";

    public static string am_fs_variant = AMD_FS_VARIANT_CONSOLE;

    // resolution order, best first: an archive present as .pc takes it, else .console, else base.
    // This is what lets PC stages come in a few at a time the way console ones did, rather than
    // all or nothing across the whole game
    public static string[] am_fs_variant_order = { AMD_FS_VARIANT_PC, AMD_FS_VARIANT_CONSOLE };

    // both console-grade tiers. Asks "are these assets the NN/DDS console kind", which is what
    // drives the engine arm (block models, registlist size); it says nothing about level vintage
    public static bool amFsIsConsoleGrade(string variant)
    {
        return string.Equals(variant, AMD_FS_VARIANT_CONSOLE, StringComparison.OrdinalIgnoreCase)
            || string.Equals(variant, AMD_FS_VARIANT_PC, StringComparison.OrdinalIgnoreCase);
    }

    private static Dictionary<string, string> amFsGetVariants()
    {
        if (am_fs_variants != null)
            return am_fs_variants;

        try
        {
            using var stream = TitleContainer.OpenStream("Content\\variants.txt");
            using var reader = new StreamReader(stream);
            amFsLoadVariants(reader);
        }
        catch (Exception)
        {
            amFsLoadVariants(TextReader.Null);
        }

        return am_fs_variants;
    }

    internal static void amFsLoadVariants(TextReader reader)
    {
        am_fs_variants = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var members = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
        {
            var space = line.LastIndexOf(' ');
            if (space <= 0)
                continue;

            var key = line.Substring(0, space);
            var declared = line.Substring(space + 1);

            var hash = key.LastIndexOf('#');
            if (hash <= 0 || !int.TryParse(key.Substring(hash + 1), out var index))
            {
                am_fs_variants[key] = declared;
                continue;
            }

            var path = amFsMemberKey(key.Substring(0, hash), declared);
            if (!members.TryGetValue(path, out var indices))
                members[path] = indices = new List<int>();

            indices.Add(index);
        }

        am_fs_member_variants = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in members)
        {
            // ascending, because the sparse sibling's members are paired with these positionally
            pair.Value.Sort();
            am_fs_member_variants[pair.Key] = pair.Value.ToArray();
        }
    }

    private static string amFsMemberKey(string path, string variant)
    {
        return path.Replace('\\', '/') + "\n" + variant;
    }

    // the base indices this archive supplies from `variant`, or null for none. the archive itself
    // still resolves to the base path: only these members come from the sibling
    public static int[] amFsMemberVariantIndices(string path, string variant)
    {
        if (string.IsNullOrEmpty(variant))
            return null;

        amFsGetVariants();
        return am_fs_member_variants.TryGetValue(amFsMemberKey(path, variant), out var indices)
            ? indices
            : null;
    }

    // the tier whose sibling supplies members of this archive, best first, or null for none. Same
    // walk as amFsResolvedVariant, so a .pc sibling can sit over a .console archive
    public static string amFsMemberVariantResolved(string path)
    {
        foreach (var variant in am_fs_variant_order)
        {
            if (amFsMemberVariantIndices(path, variant) != null)
                return variant;
        }

        return null;
    }

    // the member-level counterpart of amFsAllVariant: a screen that names its own members of a
    // shared archive, and falls back wholesale if any of them is missing
    public static bool amFsAllMemberVariant(string variant, string path, params int[] indices)
    {
        // an archive that moved wholesale supplies every member, so it answers this question too.
        // a screen must not care which way the manifest happens to declare its assets
        if (amFsAllVariant(variant, path))
            return true;

        var declared = amFsMemberVariantIndices(path, variant);
        if (declared == null)
            return false;

        foreach (var index in indices)
        {
            if (Array.IndexOf(declared, index) < 0)
                return false;
        }

        return true;
    }

    // null when this path comes back as the base asset, which is the only way to tell:
    // the request is identical either way, only the payload differs
    public static string amFsResolvedVariant(string path)
    {
        // walk the tiers best first, so an archive that exists as .pc takes it and one that only
        // exists as .console still resolves. The build declares one tier per archive, so this is a
        // ranked accept rather than a search; if an archive is ever emitted into two tiers at once
        // the manifest loader has to keep both, not just the last line
        foreach (var variant in am_fs_variant_order)
        {
            var declared = amFsVariantFor(path, variant);
            if (declared != null)
                return declared;
        }

        return null;
    }

    // "at least console-grade", for the gates that are really asking about the asset kind rather
    // than about which levels shipped. amFsAllVariant stays an exact match, for callers that mean it
    public static bool amFsAllConsoleGrade(params string[] paths)
    {
        foreach (var path in paths)
        {
            if (!amFsIsConsoleGrade(amFsResolvedVariant(path)))
                return false;
        }

        return true;
    }

    private static string amFsVariantFor(string path, string variant)
    {
        if (string.IsNullOrEmpty(variant))
            return null;

        if (!amFsGetVariants().TryGetValue(path.Replace('\\', '/'), out var declared)
            || !string.Equals(declared, variant, StringComparison.OrdinalIgnoreCase))
            return null;

        return declared;
    }

    // a screen's act tables index one build's archives, so a partial variant set has to fall back
    // wholesale rather than mix the two
    public static bool amFsAllVariant(string variant, params string[] paths)
    {
        foreach (var path in paths)
        {
            if (!string.Equals(amFsResolvedVariant(path), variant, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    // must use the same tier order as amFsResolvedVariant. If this pinned a single variant while
    // the gates walked the chain, a stage would take the console arm (NN block models) and then be
    // handed the base archive, which is #TVX - the model loader rejects it as a bad NN header
    public static string amFsResolve(string path)
    {
        var key = path.Replace('\\', '/');
        var declared = amFsResolvedVariant(key);

        return declared == null ? key : amFsVariantPath(key, declared);
    }

    public static string amFsResolveAs(string path, string variant)
    {
        var key = path.Replace('\\', '/');
        var declared = amFsVariantFor(key, variant);

        return declared == null ? key : amFsVariantPath(key, declared);
    }

    // G_COM/CPIT/CPIT_MAIN.AMB plus "console" -> G_COM/CPIT/CPIT_MAIN.console.AMB
    public static string amFsVariantPath(string path, string variant)
    {
        var key = path.Replace('\\', '/');
        var dot = key.LastIndexOf('.');

        return key.Substring(0, dot) + "." + variant + key.Substring(dot);
    }

    // true once the whole file is in value.read_buf, with value.stream positioned at 0 over it.
    // false means the read is still running and the caller should come back next frame
    private static bool amFsCopyInto(AMS_FS value, string resolved)
    {
        if (value.stream != null)
            return true;

        // parked after a failed read. Sticky, or the next frame would just issue the request again
        // and turn one dead file into a request storm
        if (value.error != null)
            return false;

        if (value.read_buf == null && value.readTask == null)
        {
#if ASYNC_IO
            value.readTask = ContentSource.Current.ReadAsync(resolved);
#else
            value.read_buf = ContentSource.Current.Read(resolved);
#endif
        }

        if (value.readTask != null)
        {
            if (!value.readTask.IsCompleted)
                return false;

            var completed = value.readTask;
            value.readTask = null;

            // the C++ leaves a failed read at the head of the queue as AMD_FS_STAT_CANTOPEN and
            // waits to be told to retry. Rethrowing here would take the frame loop with it
            if (completed.IsFaulted)
            {
                value.error = completed.Exception?.GetBaseException();
                Console.Error.WriteLine($"amFs: '{resolved}' failed: {value.error?.Message}");
                return false;
            }

            value.read_buf = completed.GetAwaiter().GetResult();
        }

        value.stream = new MemoryStream(value.read_buf, false);
        return true;
    }

    // the request the queue is stuck on, or null. amFs only ever services the head, so a parked
    // one blocks everything behind it, which is what makes it worth surfacing
    public static AMS_FS amFsFailedRequest()
    {
        if (ams_fsList.Count == 0)
            return null;

        var head = ams_fsList.First.Value;
        return head.error != null ? head : null;
    }

    public static void amFsRetryRequest(AMS_FS cdfsp)
    {
        cdfsp.error = null;
        cdfsp.readTask = null;
        cdfsp.read_buf = null;
    }

    public static byte[] amFsReadAll(Stream source)
    {
        using (source)
        {
            if (!source.CanSeek)
            {
                using (var grow = new MemoryStream())
                {
                    source.CopyTo(grow);
                    return grow.ToArray();
                }
            }

            var buffer = new byte[source.Length];
            var read = 0;
            while (read < buffer.Length)
            {
                var got = source.Read(buffer, read, buffer.Length - read);
                if (got <= 0)
                    throw new EndOfStreamException(
                        $"wanted {buffer.Length} bytes, stream ended {buffer.Length - read} short");

                read += got;
            }

            return buffer;
        }
    }

    private static void amFsFinishCopy(AMS_FS value)
    {
        value.stream?.Dispose();
        value.stream = null;
        value.read_buf = null;
    }

    public static void amFsExecuteBackgroundRead()
    {
        if (ams_fsList.Count == 0)
            return;

        AMS_FS value = ams_fsList.First.Value;

        if (value.phase == 0)
        {
            var resolved = value.variant == null
                ? amFsResolve(value.file_name)
                : amFsResolveAs(value.file_name, value.variant);

            if (!amFsCopyInto(value, resolved))
                return;

            value.data = value.read_buf;

            AMS_AMB_HEADER parsed = new AMS_AMB_HEADER();
            readAmbHeader(value.stream, ref parsed);
            value.count = parsed.file_num;
            value.files = parsed.files;
            value.types = parsed.types;
            value.user0 = parsed.user0;
            value.user1 = parsed.user1;
            value.offsets = parsed.offsets;
            value.lengths = parsed.lengths;
            value.flag = parsed.flag;
            value.is_real_amb = parsed.is_real_amb;
            value.big_endian = parsed.big_endian;

            amFsFinishCopy(value);

            value.member_variant_name = value.variant ?? amFsMemberVariantResolved(value.file_name);
            value.member_variant = amFsMemberVariantIndices(
                value.file_name, value.member_variant_name);
            value.phase = value.member_variant == null ? 2 : 1;
        }

        // the sibling holds only the declared members, in the same ascending order the manifest
        // gave them, so pairing is positional
        if (value.phase == 1)
        {
            var sibling_path = amFsVariantPath(value.file_name, value.member_variant_name);
            if (!amFsCopyInto(value, sibling_path))
                return;

            var sibling = new AMS_AMB_HEADER();
            readAmbHeader(value.stream, ref sibling);
            sibling.data = value.read_buf;
            sibling.dir = sibling_path;
            sibling.buf = new object[sibling.file_num];
            amPreLoadAmbItems(sibling);

            value.member_variant_amb = sibling;
            value.member_variant_index = new int[value.count];
            for (int i = 0; i < value.count; i++)
                value.member_variant_index[i] = -1;

            for (int i = 0; i < value.member_variant.Length && i < sibling.file_num; i++)
            {
                var index = value.member_variant[i];
                if (index >= 0 && index < value.count)
                    value.member_variant_index[index] = i;
            }

            amFsFinishCopy(value);
            value.phase = 2;
        }

        value.makeAmbHeader();
        value.stat = 3;
        value.phase = 3;
        ams_fsList.RemoveFirst();
    }


    public AMS_FS amFsReadBackground(int file_id, byte[] buf, int cache)
    {
        mppAssertNotImpl();
        return null;
    }

    private AMS_FS amFsReadBackground(
        int afs_id,
        string file_name,
        byte[] buf,
        int cache)
    {
        mppAssertNotImpl();
        return null;
    }

    private AMS_FS amFsReadFileList(int afs_id, string file_name, byte[] buf)
    {
        mppAssertNotImpl();
        return null;
    }

    public static int amFsRead(string file_name, out byte[] buf)
    {
        int count = 0;
        using (Stream stream = TitleContainer.OpenStream(file_name))
        {
            count = (int) stream.Length;
            buf = new byte[count];
            stream.Read(buf, 0, count);
        }

        return count;
    }

    public static byte[] amFsRead(string file_name)
    {
        byte[] buffer = null;
        using (Stream stream = TitleContainer.OpenStream("Content\\" + file_name))
        {
            int length = (int) stream.Length;
            buffer = new byte[length];
            stream.Read(buffer, 0, length);
        }

        return buffer;
    }

    public static int amFsRead(string file_name, out Texture2D buf)
    {
        mppAssertNotImpl();
        buf = null;
        return 0;
    }

    public static void amFsClearRequest(AMS_FS cdfsp)
    {
        ams_fsList.Remove(cdfsp);
    }

    private int amFsSearchPartition(string pname)
    {
        return -1;
    }

    private int amFsSearchName(string fname)
    {
        return -1;
    }

    private int amFsReadStart()
    {
        return 1;
    }

    private void amFsReadListNext(AMS_FS cdfsp)
    {
    }

    private void amFsConvertPath(ref string out_name, ref string in_name)
    {
    }

    private string amFsGetColumn(string cp)
    {
        return cp;
    }

    private string amFsGetNextColumn(string cp)
    {
        return this.amFsGetColumn(cp);
    }

    private string amFsGetNumber(string cp, int num)
    {
        return cp;
    }

    public string amFsGetString(string cp, string str)
    {
        return cp;
    }
}