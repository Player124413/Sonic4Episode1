using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;

namespace Sonic4Episode1.Viewer;

// reading NN chunk files out of an #AMB, shared by the viewer and the motion harness
internal static class NnArchive
{
    // every chunk that carries an NNS_MOTION, across GLES11, DXG20 and Xenon
    private static readonly HashSet<uint> MotionChunks = new HashSet<uint>
    {
        AppMain.NND_CHUNK_MOTION_GLES11, AppMain.NND_CHUNK_CAMERA_MOTION_GLES11,
        AppMain.NND_CHUNK_LIGHT_MOTION_GLES11, AppMain.NND_CHUNK_MORPH_MOTION_GLES11,
        AppMain.NND_CHUNK_MATERIAL_MOTION_GLES11,

        AppMain.NND_CHUNK_MOTION_DXG20, AppMain.NND_CHUNK_CAMERA_MOTION_DXG20,
        AppMain.NND_CHUNK_LIGHT_MOTION_DXG20, AppMain.NND_CHUNK_MORPH_MOTION_DXG20,
        AppMain.NND_CHUNK_MATERIAL_MOTION_DXG20,

        AppMain.NND_CHUNK_MOTION_XE, AppMain.NND_CHUNK_CAMERA_MOTION_XE,
        AppMain.NND_CHUNK_LIGHT_MOTION_XE, AppMain.NND_CHUNK_MORPH_MOTION_XE,
        AppMain.NND_CHUNK_MATERIAL_MOTION_XE,
    };

    private static readonly HashSet<uint> EndChunks = new HashSet<uint>
    {
        AppMain.NND_CHUNK_NEND_GLES11, AppMain.NND_CHUNK_NEND_DXG20, AppMain.NND_CHUNK_NEND_XE,
    };

    public static AppMain.AMS_AMB_HEADER Open(string path, out byte[] raw)
    {
        raw = File.ReadAllBytes(path);
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = raw;
        using (var stream = new MemoryStream(raw))
            AppMain.readAmbHeader(stream, ref header);

        return header;
    }

    public static BinaryReader ReaderFor(byte[] raw, int offset, MemoryStream member, out bool known)
    {
        uint little = BitConverter.ToUInt32(raw, offset);
        uint big = BinaryPrimitives.ReverseEndianness(little);

        known = true;
        if (little == AppMain.NND_CHUNK_HEADER_GLES11 || little == AppMain.NND_CHUNK_HEADER_DXG20)
            return new BinaryReader(member);
        if (big == AppMain.NND_CHUNK_HEADER_XE)
            return new EndianBinaryReader(member, bigEndian: true);

        known = false;
        return new BinaryReader(member);
    }

    public static List<AppMain.NNS_OBJECT> ReadObjects(string path)
    {
        var objects = new List<AppMain.NNS_OBJECT>();
        var header = Open(path, out byte[] raw);

        for (int i = 0; i < header.file_num; ++i)
        {
            if (header.lengths[i] < 32)
                continue;

            using var member = new MemoryStream(raw, header.offsets[i], header.lengths[i]);
            using BinaryReader reader = ReaderFor(raw, header.offsets[i], member, out bool known);
            if (!known)
                continue;

            AppMain.amObjectSetup(out var obj, out _, reader);
            if (obj != null && obj.nNode > 0)
                objects.Add(obj);
        }

        return objects;
    }

    // each motion is read on its own so a NNS_SUBMOTION throw doesn't end the whole archive
    public static IEnumerable<(int Index, AppMain.NNS_MOTION Motion, string Error)> ReadMotions(string path)
    {
        var header = Open(path, out byte[] raw);

        int index = 0;
        for (int i = 0; i < header.file_num; ++i)
        {
            if (header.lengths[i] < 32)
                continue;

            using var member = new MemoryStream(raw, header.offsets[i], header.lengths[i]);
            using BinaryReader reader = ReaderFor(raw, header.offsets[i], member, out bool known);
            if (!known)
                continue;

            foreach (var (at, chunkId) in MotionChunkOffsets(reader, out long data0Pos))
            {
                AppMain.NNS_MOTION motion = null;
                string error = null;
                try
                {
                    reader.BaseStream.Seek(at, SeekOrigin.Begin);
                    motion = AppMain.NNS_MOTION.Read(reader, data0Pos);
                }
                catch (Exception ex)
                {
                    // several readers share the message "Investigation needed", so name the chunk
                    error = $"{ChunkName(chunkId)}: {ex.GetType().Name}: {ex.Message}";
                }

                yield return (index++, motion, error);
            }
        }
    }

    // count down and bound by the stream rather than relying only on hitting NEND
    private static List<(long At, uint ChunkId)> MotionChunkOffsets(BinaryReader reader, out long data0Pos)
    {
        var offsets = new List<(long, uint)>();

        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        var file = AppMain.NNS_BINCNK_FILEHEADER.Read(reader);
        long ofsData = file.OfsData;
        data0Pos = ofsData;

        reader.BaseStream.Seek(ofsData, SeekOrigin.Begin);
        var chunk = AppMain.NNS_BINCNK_DATAHEADER.Read(reader);

        for (int remaining = file.nChunk; remaining > 0; --remaining)
        {
            if (EndChunks.Contains(chunk.Id))
                break;

            if (MotionChunks.Contains(chunk.Id))
                offsets.Add((ofsData + chunk.OfsMainData, chunk.Id));

            ofsData += 8 + chunk.OfsNextId;
            if (ofsData + 16 > reader.BaseStream.Length)
                break;

            reader.BaseStream.Seek(ofsData, SeekOrigin.Begin);
            chunk = AppMain.NNS_BINCNK_DATAHEADER.Read(reader);
        }

        return offsets;
    }

    // the four characters the chunk id spells, which say which of the five motion kinds this is
    public static string ChunkName(uint id)
    {
        var bytes = BitConverter.GetBytes(id);
        var text = new char[4];
        for (int i = 0; i < 4; ++i)
            text[i] = bytes[i] >= 0x20 && bytes[i] < 0x7F ? (char)bytes[i] : '?';

        return new string(text);
    }
}
