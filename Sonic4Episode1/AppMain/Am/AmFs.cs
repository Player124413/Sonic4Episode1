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
        public Task readTask;
        public FsBackgroundReadComplete callback;
        public AMS_AMB_HEADER amb_header;
        public bool is_real_amb = false;
        public bool big_endian = false;

        // null follows am_fs_variant; anything else pins this request to that variant
        public string variant;

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
            amPreLoadAmbItems(this.amb_header);
        }
    }

    public static bool amFsIsComplete(AMS_FS cdfsp)
    {
        return cdfsp.stat == 3;
    }

    public static AMS_FS amFsReadBackground(string file_name)
    {
        FsReadSpeedBytesPerFrame = 32768;
        return amFsReadBackground(file_name, (FsBackgroundReadComplete)null);
    }

    public static AMS_FS amFsReadBackgroundAs(string file_name, string variant)
    {
        FsReadSpeedBytesPerFrame = 32768;
        return amFsReadBackground(file_name, null, variant);
    }

    public static AMS_FS amFsReadBackground(string file_name, int BytesPerFrame)
    {
        FsReadSpeedBytesPerFrame = BytesPerFrame;
        return amFsReadBackground(file_name, null);
    }

    public static AMS_FS amFsReadBackground(
        string file_name,
        FsBackgroundReadComplete callback)
    {
        return amFsReadBackground(file_name, callback, null);
    }

    private static AMS_FS amFsReadBackground(
        string file_name,
        FsBackgroundReadComplete callback,
        string variant)
    {
        //Console.WriteLine(file_name);

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
            amsFs.callback = callback;
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
        byte[] array = buf.array;
        int offset = buf.offset;
        AMS_AMB_HEADER amsAmbHeader = searchPreloadedAmb(buf.amb, buf.offset);
        if (amsAmbHeader != null)
            return amsAmbHeader;
        AMS_AMB_HEADER amb = new AMS_AMB_HEADER();
        using (Stream input = new MemoryStream(array, offset, buf.length))
        {
            // Console.WriteLine($"{offset}-{offset + buf.length}");
            if (offset == 0)
            {
                amb.data = new byte[input.Length];
                input.Read(amb.data, 0, amb.data.Length);
            }
            else if (buf.amb.is_real_amb)
            {
                amb.data = new byte[buf.length];
                input.Read(amb.data, 0, amb.data.Length);
            }
            else
            {
                amb.data = array;
            }

            input.Position = 0L;
            readAmbHeader(input, ref amb);

            amb.parent = buf.amb;
            amPreLoadAmbItems(amb);
        }

        return amb;
    }

    public const int AMB_FILE_ID = 0x424D4123;
    public const int AMB_HEADER_SIZE = 0x20;

    public static void readAmbHeader(Stream stream, ref AMS_AMB_HEADER amb)
    {
        using (BinaryReader binaryReader = new BinaryReader(stream))
        {
            var fileId = binaryReader.ReadInt32();
            if (fileId != AMB_FILE_ID)
            {
                readLegacyAmbHeader(binaryReader, ref amb, fileId);
                return;
            }

            var headerSize = binaryReader.ReadInt32();
            if (headerSize == AMB_HEADER_SIZE)
            {
                readRealAmbHeader(binaryReader, ref amb);
                return;
            }

            if (BinaryPrimitives.ReverseEndianness(headerSize) == AMB_HEADER_SIZE)
            {
                amb.big_endian = true;
                readRealAmbHeader(new EndianBinaryReader(stream, bigEndian: true), ref amb);
                return;
            }

            throw new InvalidDataException($"#AMB header size 0x{headerSize:X8} is not 0x20 in either byte order");
        }
    }

    private static void readRealAmbHeader(BinaryReader reader, ref AMS_AMB_HEADER amb)
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
        amb.flag = new sbyte[4] {127, 127, 127, 127};
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
            if (amb.offsets[index] == offset && amb.buf[index] != null)
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
        AmbChunk ambChunk = new AmbChunk(amb.data, 0, 0, amb);
        for (int index = 0; index < amb.files.Length; ++index)
        {
            ambChunk.offset = amb.offsets[index];
            ambChunk.length = amb.lengths[index];
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

    // "<path> <variant>" lines, one per archive the content build emitted an alt version of
    private static Dictionary<string, string> am_fs_variants;

    // names the asset mode, not the dump it came from: console is assembled from more than one
    public const string AMD_FS_VARIANT_CONSOLE = "console";

    // what a screen asks for when its act tables only describe the base archives. a screen that
    // has not been ported to console yet has to pin this, or a globally flipped shared archive
    // reaches it with ids it cannot index
    public const string AMD_FS_VARIANT_BASE = "";

    public static string am_fs_variant = AMD_FS_VARIANT_CONSOLE;

    private static Dictionary<string, string> amFsGetVariants()
    {
        if (am_fs_variants != null)
            return am_fs_variants;

        am_fs_variants = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            using var stream = TitleContainer.OpenStream("Content\\variants.txt");
            using var reader = new StreamReader(stream);
            for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                var space = line.LastIndexOf(' ');
                if (space > 0)
                    am_fs_variants[line.Substring(0, space)] = line.Substring(space + 1);
            }
        }
        catch (Exception)
        {
        }

        return am_fs_variants;
    }

    // null when this path comes back as the base asset, which is the only way to tell:
    // the request is identical either way, only the payload differs
    public static string amFsResolvedVariant(string path)
    {
        return amFsVariantFor(path, am_fs_variant);
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

    public static string amFsResolve(string path)
    {
        return amFsResolveAs(path, am_fs_variant);
    }

    public static string amFsResolveAs(string path, string variant)
    {
        var key = path.Replace('\\', '/');
        var declared = amFsVariantFor(key, variant);
        if (declared == null)
            return key;

        var dot = key.LastIndexOf('.');
        return key.Substring(0, dot) + "." + declared + key.Substring(dot);
    }

    public static void amFsExecuteBackgroundRead()
    {
        if (ams_fsList.Count > 0)
        {
            AMS_FS value = ams_fsList.First.Value;

            if (value.stream == null)
            {
                var resolved = value.variant == null
                    ? amFsResolve(value.file_name)
                    : amFsResolveAs(value.file_name, value.variant);
                var stream = TitleContainer.OpenStream("Content\\" + resolved);

                value.stream = new MemoryStream();
#if ASYNC_IO
#if ASYNC_TARGETING_PACK
                value.readTask = Task.Factory.StartNew(() => stream.CopyTo(value.stream));
#else
                value.readTask = stream.CopyToAsync(value.stream);
#endif
#else
                stream.CopyTo(value.stream);
#endif
            }

            if (value.readTask != null && !value.readTask.IsCompleted)
            {
                return;
            }

            value.data = (value.stream as MemoryStream).ToArray();
            value.stream.Seek(0, SeekOrigin.Begin);

            //Console.WriteLine("{0}", value.data.Length);

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

            value.makeAmbHeader();
            value.stat = 3;
            value.stream?.Dispose();
            value.stream = null;
            ams_fsList.RemoveFirst();
            if (value.callback != null)            
                value.callback(value);            
        }
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