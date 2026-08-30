using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Viewer;

// reports which submotion key formats a set of motion archives actually uses; reads only the
// fixed 40 byte header rather than calling NNS_SUBMOTION.Read, so one unsupported submotion
// doesn't hide every combination behind it in the same file
internal static class MotionSurvey
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

    private sealed class Combination
    {
        public uint MotionType;
        public uint ValueType;
        public uint FrameType;
        public uint IpType;
        public int KeySize;

        public int Count;
        public readonly SortedSet<string> Files = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public static int Run(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Console.Error.WriteLine($"no such directory: {directory}");
            return 1;
        }

        // every archive, not just *_MTN.AMB: camera and cutscene motion lives elsewhere too
        string[] archives = Directory.GetFiles(directory, "*.AMB", SearchOption.AllDirectories);
        Console.WriteLine($"{archives.Length} archive(s) under {directory}");

        var combinations = new Dictionary<string, Combination>();
        int motions = 0;
        int submotions = 0;
        var failures = new List<string>();

        foreach (string archive in archives)
        {
            try
            {
                SurveyArchive(archive, combinations, ref motions, ref submotions);
            }
            catch (Exception ex)
            {
                failures.Add($"{Path.GetFileName(archive)}: {ex.Message}");
            }
        }

        Report(combinations, motions, submotions, failures);
        return 0;
    }

    private static void SurveyArchive(
        string path, Dictionary<string, Combination> combinations, ref int motions, ref int submotions)
    {
        byte[] raw = File.ReadAllBytes(path);
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = raw;
        using (var stream = new MemoryStream(raw))
            AppMain.readAmbHeader(stream, ref header);

        // skip members that don't open with an NN file header (2D layouts, sound banks, etc), and
        // skip a failing member on its own so it can't hide the motion sitting after it
        string name = Path.GetFileName(path);
        for (int i = 0; i < header.file_num; ++i)
        {
            if (header.lengths[i] < 32 || header.offsets[i] + 4 > raw.Length)
                continue;

            uint little = BitConverter.ToUInt32(raw, header.offsets[i]);
            uint big = BinaryPrimitives.ReverseEndianness(little);

            bool bigEndian;
            if (little == AppMain.NND_CHUNK_HEADER_GLES11 || little == AppMain.NND_CHUNK_HEADER_DXG20)
                bigEndian = false;
            else if (big == AppMain.NND_CHUNK_HEADER_XE)
                bigEndian = true;
            else
                continue;

            try
            {
                using var member = new MemoryStream(raw, header.offsets[i], header.lengths[i]);
                using BinaryReader reader = bigEndian
                    ? new EndianBinaryReader(member, bigEndian: true)
                    : new BinaryReader(member);

                SurveyMember(reader, name, combinations, ref motions, ref submotions);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"  {name}[{i}] {header.files[i]}: {ex.Message}");
            }
        }
    }

    private static void SurveyMember(
        BinaryReader reader,
        string archive,
        Dictionary<string, Combination> combinations,
        ref int motions,
        ref int submotions)
    {
        var file = AppMain.NNS_BINCNK_FILEHEADER.Read(reader);
        long ofsData = file.OfsData;
        reader.BaseStream.Seek(ofsData, SeekOrigin.Begin);
        var chunk = AppMain.NNS_BINCNK_DATAHEADER.Read(reader);

        // count down and bound by the stream rather than relying only on hitting NEND
        for (int remaining = file.nChunk; remaining > 0; --remaining)
        {
            if (EndChunks.Contains(chunk.Id))
                return;

            if (MotionChunks.Contains(chunk.Id))
            {
                reader.BaseStream.Seek(ofsData + chunk.OfsMainData, SeekOrigin.Begin);
                ReadMotionHeaders(reader, ofsData, archive, combinations, ref motions, ref submotions);
            }

            ofsData += 8 + chunk.OfsNextId;
            if (ofsData + 16 > reader.BaseStream.Length)
                return;

            reader.BaseStream.Seek(ofsData, SeekOrigin.Begin);
            chunk = AppMain.NNS_BINCNK_DATAHEADER.Read(reader);
        }
    }

    private static void ReadMotionHeaders(
        BinaryReader reader,
        long data0Pos,
        string archive,
        Dictionary<string, Combination> combinations,
        ref int motions,
        ref int submotions)
    {
        uint motionType = reader.ReadUInt32();
        reader.ReadSingle();
        reader.ReadSingle();
        int nSubmotion = reader.ReadInt32();
        uint offset = reader.ReadUInt32();
        if (offset == 0U || nSubmotion <= 0)
            return;

        ++motions;
        uint category = motionType & AppMain.NND_MOTIONTYPE_CATEGORY_MASK;
        reader.BaseStream.Seek(data0Pos + offset, SeekOrigin.Begin);

        for (int i = 0; i < nSubmotion; ++i)
        {
            uint fType = reader.ReadUInt32();
            uint fIPType = reader.ReadUInt32();
            reader.ReadInt32();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadInt32();
            int keySize = reader.ReadInt32();
            reader.ReadUInt32();

            ++submotions;
            Record(combinations, archive, category, fType, fIPType, keySize);
        }
    }

    private static void Record(
        Dictionary<string, Combination> combinations,
        string archive,
        uint category,
        uint fType,
        uint fIPType,
        int keySize)
    {
        uint frameType = fType & AppMain.NND_SMOTTYPE_FRAME_MASK;
        uint valueType = ValueClassOf(category, fType);
        uint ipType = fIPType & AppMain.NND_SMOTIPTYPE_IP_MASK;

        string key = $"{category}|{valueType}|{frameType}|{ipType}|{keySize}";
        if (!combinations.TryGetValue(key, out var combination))
        {
            combination = new Combination
            {
                MotionType = category,
                ValueType = valueType,
                FrameType = frameType,
                IpType = ipType,
                KeySize = keySize,
            };
            combinations[key] = combination;
        }

        ++combination.Count;
        combination.Files.Add(archive);
    }

    // groups by which of translation/rotation/scaling/user a submotion is, not the axis bits
    private static uint ValueClassOf(uint category, uint fType)
    {
        if (category != AppMain.NND_MOTIONTYPE_NODE)
            return fType & ~AppMain.NND_SMOTTYPE_FRAME_MASK & ~AppMain.NND_SMOTTYPE_ANGLE_MASK;

        const uint translation = AppMain.NND_SMOTTYPE_TRANSLATION_X
            | AppMain.NND_SMOTTYPE_TRANSLATION_Y | AppMain.NND_SMOTTYPE_TRANSLATION_Z;
        const uint rotation = AppMain.NND_SMOTTYPE_ROTATION_X
            | AppMain.NND_SMOTTYPE_ROTATION_Y | AppMain.NND_SMOTTYPE_ROTATION_Z
            | AppMain.NND_SMOTTYPE_QUATERNION;
        const uint scaling = AppMain.NND_SMOTTYPE_SCALING_X
            | AppMain.NND_SMOTTYPE_SCALING_Y | AppMain.NND_SMOTTYPE_SCALING_Z;

        if ((fType & translation) != 0) return translation;
        if ((fType & rotation) != 0) return rotation;
        if ((fType & scaling) != 0) return scaling;
        return fType & ~AppMain.NND_SMOTTYPE_FRAME_MASK & ~AppMain.NND_SMOTTYPE_ANGLE_MASK;
    }

    private static void Report(
        Dictionary<string, Combination> combinations,
        int motions,
        int submotions,
        List<string> failures)
    {
        Console.WriteLine($"{motions} motion(s), {submotions} submotion(s), "
            + $"{combinations.Count} distinct combination(s)");
        Console.WriteLine();

        foreach (var c in combinations.Values.OrderByDescending(c => c.Count))
        {
            Console.WriteLine($"{c.Count,6}  {MotionTypeName(c.MotionType),-9} {ValueName(c.MotionType, c.ValueType),-12}"
                + $" {FrameName(c.FrameType),-7} {IpName(c.IpType),-22} KeySize {c.KeySize,-3}"
                + $" {(Supported(c) ? "ported" : "MISSING")}");

            if (!Supported(c))
                Console.WriteLine($"        in {string.Join(", ", c.Files.Take(4))}{(c.Files.Count > 4 ? ", ..." : "")}");
        }

        Console.WriteLine();
        int missing = combinations.Values.Count(c => !Supported(c));
        Console.WriteLine(missing == 0
            ? "every combination in these archives is already ported"
            : $"{missing} combination(s) the reader throws on");

        foreach (string failure in failures)
            Console.Error.WriteLine("failed: " + failure);
    }

    // Mirrors the branches in NNS_SUBMOTION that do not throw.
    private static bool Supported(Combination c)
    {
        if (c.MotionType != AppMain.NND_MOTIONTYPE_NODE && c.MotionType != AppMain.NND_MOTIONTYPE_MATERIAL)
            return false;

        bool spline = c.IpType == AppMain.NND_SMOTIPTYPE_SPLINE || c.IpType == AppMain.NND_SMOTIPTYPE_BEZIER;
        bool isRotation = (c.ValueType & (AppMain.NND_SMOTTYPE_ROTATION_X | AppMain.NND_SMOTTYPE_QUATERNION)) != 0;

        if (isRotation)
        {
            if (c.FrameType == AppMain.NND_SMOTTYPE_FRAME_FLOAT)
                return true;
            if (c.FrameType != AppMain.NND_SMOTTYPE_FRAME_SINT16)
                return false;
            return c.IpType != AppMain.NND_SMOTIPTYPE_SPLINE;
        }

        return c.FrameType == AppMain.NND_SMOTTYPE_FRAME_FLOAT && !spline;
    }

    private static string MotionTypeName(uint type)
    {
        if (type == AppMain.NND_MOTIONTYPE_NODE) return "node";
        if (type == AppMain.NND_MOTIONTYPE_CAMERA) return "camera";
        if (type == AppMain.NND_MOTIONTYPE_LIGHT) return "light";
        if (type == AppMain.NND_MOTIONTYPE_MORPH) return "morph";
        if (type == AppMain.NND_MOTIONTYPE_MATERIAL) return "material";
        return $"0x{type:X}";
    }

    private static string ValueName(uint motionType, uint valueType)
    {
        if (motionType != AppMain.NND_MOTIONTYPE_NODE)
            return $"0x{valueType:X}";

        if ((valueType & AppMain.NND_SMOTTYPE_TRANSLATION_X) != 0) return "translation";
        if ((valueType & AppMain.NND_SMOTTYPE_ROTATION_X) != 0) return "rotation";
        if ((valueType & AppMain.NND_SMOTTYPE_QUATERNION) != 0) return "quaternion";
        if ((valueType & AppMain.NND_SMOTTYPE_SCALING_X) != 0) return "scaling";
        if ((valueType & AppMain.NND_SMOTTYPE_NODEHIDE) != 0) return "nodehide";
        return $"0x{valueType:X}";
    }

    private static string FrameName(uint frameType)
    {
        if (frameType == AppMain.NND_SMOTTYPE_FRAME_FLOAT) return "float";
        if (frameType == AppMain.NND_SMOTTYPE_FRAME_SINT16) return "sint16";
        return $"0x{frameType:X}";
    }

    private static string IpName(uint ipType)
    {
        if (ipType == AppMain.NND_SMOTIPTYPE_SPLINE) return "spline";
        if (ipType == AppMain.NND_SMOTIPTYPE_LINEAR) return "linear";
        if (ipType == AppMain.NND_SMOTIPTYPE_CONSTANT) return "constant";
        if (ipType == AppMain.NND_SMOTIPTYPE_BEZIER) return "bezier";
        if (ipType == AppMain.NND_SMOTIPTYPE_SI_SPLINE) return "si_spline";
        if (ipType == AppMain.NND_SMOTIPTYPE_TRIGGER) return "trigger";
        if (ipType == AppMain.NND_SMOTIPTYPE_QUAT_LERP) return "quat_lerp";
        if (ipType == AppMain.NND_SMOTIPTYPE_QUAT_SLERP) return "quat_slerp";
        if (ipType == AppMain.NND_SMOTIPTYPE_QUAT_SQUAD) return "quat_squad";
        return $"0x{ipType:X}";
    }
}
