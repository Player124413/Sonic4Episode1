using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// packs an AMS_AMB_HEADER back to #AMB; the legacy magic-less layout is read only
internal partial class AppMain
{
    internal struct AmbEntry
    {
        public string Name;
        public byte[] Data;
        public int Type;
        public short User0;
        public short User1;

        public AmbEntry(string name, byte[] data)
        {
            Name = name;
            Data = data;
            Type = -1;
            User0 = 0;
            User1 = 0;
        }
    }

    public const int AMB_ENTRY_SIZE = 0x10;
    public const int AMB_NAME_SIZE = 0x20;
    // 32 predicts member offsets far better than 16 across shipped archives
    public const int AMB_ALIGN = 0x20;
    private const short AMB_VER_MAJOR = 0;
    private const short AMB_VER_MINOR = 4;
    // amBind.h:49
    public const byte AMD_AMB_FLAG0_ENDIAN_BIG = 0x01;

#if NET40
    internal static byte[] writeAmbArchive(IList<AmbEntry> entries, bool bigEndian)
#else
    internal static byte[] writeAmbArchive(IReadOnlyList<AmbEntry> entries, bool bigEndian)
#endif
    {
        var stream = new MemoryStream();
        writeAmbArchive(stream, entries, bigEndian);
        return stream.ToArray();
    }

#if NET40
    internal static void writeAmbArchive(Stream output, IList<AmbEntry> entries, bool bigEndian)
#else
    internal static void writeAmbArchive(Stream output, IReadOnlyList<AmbEntry> entries, bool bigEndian)
#endif
    {
        var writer = bigEndian
            ? new EndianBinaryWriter(output, bigEndian: true)
#if NET40
            : new BinaryWriter(output, Encoding.ASCII);
#else
            : new BinaryWriter(output, Encoding.ASCII, leaveOpen: true);
#endif

        var count = entries.Count;
        var entryTable = AMB_HEADER_SIZE;
        var dataStart = align(entryTable + (AMB_ENTRY_SIZE * count), AMB_ALIGN);

        var offsets = new int[count];
        var cursor = dataStart;
        for (int i = 0; i < count; i++)
        {
            offsets[i] = cursor;
            cursor = align(cursor + entries[i].Data.Length, AMB_ALIGN);
        }

        var stringTable = cursor;

        // literal bytes, not through the endian writer, so it reads the same on either platform
        output.WriteByte((byte)'#');
        output.WriteByte((byte)'A');
        output.WriteByte((byte)'M');
        output.WriteByte((byte)'B');
        writer.Write(AMB_HEADER_SIZE);
        writer.Write(AMB_VER_MAJOR);
        writer.Write(AMB_VER_MINOR);
        output.WriteByte(bigEndian ? AMD_AMB_FLAG0_ENDIAN_BIG : (byte)0);
        output.WriteByte(0);
        output.WriteByte(0);
        output.WriteByte(0);
        writer.Write(count);
        writer.Write(entryTable);
        writer.Write(dataStart);
        writer.Write(stringTable);

        for (int i = 0; i < count; i++)
        {
            writer.Write(offsets[i]);
            writer.Write(entries[i].Data.Length);
            writer.Write(entries[i].Type);
            writer.Write(entries[i].User0);
            writer.Write(entries[i].User1);
        }

        for (int i = 0; i < count; i++)
        {
            padTo(output, offsets[i]);
            output.Write(entries[i].Data, 0, entries[i].Data.Length);
        }

        padTo(output, stringTable);

        var name = new byte[AMB_NAME_SIZE];
        for (int i = 0; i < count; i++)
        {
            Array.Clear(name, 0, name.Length);
            var text = entries[i].Name ?? string.Empty;
            // NUL terminated slot; refuse rather than silently truncate and alias two members
            if (text.Length >= AMB_NAME_SIZE)
                throw new InvalidDataException(
                    $"AMB member name '{text}' is {text.Length} characters, the limit is {AMB_NAME_SIZE - 1}");

            for (int j = 0; j < text.Length; j++)
                name[j] = (byte)text[j];

            output.Write(name, 0, name.Length);
        }

        writer.Flush();
    }

    internal static List<AmbEntry> readAmbEntries(AMS_AMB_HEADER amb)
    {
        var entries = new List<AmbEntry>(amb.file_num);
        for (int i = 0; i < amb.file_num; i++)
        {
            var data = new byte[amb.lengths[i]];
            Buffer.BlockCopy(amb.data, amb.data_base + amb.offsets[i], data, 0, amb.lengths[i]);

            entries.Add(new AmbEntry
            {
                Name = amb.files[i],
                Data = data,
                Type = amb.types != null ? amb.types[i] : -1,
                User0 = amb.user0 != null ? amb.user0[i] : (short)0,
                User1 = amb.user1 != null ? amb.user1[i] : (short)0,
            });
        }

        return entries;
    }

    private static int align(int value, int to)
    {
        var remainder = value % to;
        return remainder == 0 ? value : value + (to - remainder);
    }

    private static void padTo(Stream stream, int position)
    {
        while (stream.Position < position)
            stream.WriteByte(0);
    }
}
