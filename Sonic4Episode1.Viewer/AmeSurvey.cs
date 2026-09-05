using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Viewer;

// parses every .AME under a tree and reports what node types it holds, which order it is in, and
// anything readAMEfile could not take. The switch in AmAme has no default arm, so an unhandled type
// leaves a null in node[] and the stream mid-node; both show up here rather than as a crash later.
internal static class AmeSurvey
{
    private const uint AmbFileId = 0x424D4123;
    private const uint AmeFileId = 0x454D4123;

    private sealed class Tally
    {
        public int Files;
        public int BigEndian;
        public int Nodes;
        public int NullNodes;
        public int NullAtEnd;
        public readonly Dictionary<ushort, int> Types = new Dictionary<ushort, int>();
        public readonly List<string> Failures = new List<string>();
        public readonly SortedSet<string> UnhandledIn = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public static int Run(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Console.Error.WriteLine($"no such directory: {directory}");
            return 1;
        }

        string[] archives = Directory.GetFiles(directory, "*.AMB", SearchOption.AllDirectories);
        var tally = new Tally();

        foreach (string archive in archives)
        {
            try
            {
                var header = NnArchive.Open(archive, out byte[] raw);
                WalkArchive(header, raw, Path.GetFileName(archive), tally);
            }
            catch (Exception ex)
            {
                tally.Failures.Add($"{Path.GetFileName(archive)}: {ex.Message}");
            }
        }

        Report(directory, archives.Length, tally);
        return tally.Failures.Count == 0 && tally.NullNodes == 0 ? 0 : 1;
    }

    private static void WalkArchive(
        AppMain.AMS_AMB_HEADER header, byte[] raw, string trail, Tally tally)
    {
        for (int i = 0; i < header.file_num; ++i)
        {
            int offset = header.offsets[i];
            int length = header.lengths[i];
            if (length < 8 || offset + 8 > raw.Length)
                continue;

            uint id = BitConverter.ToUInt32(raw, offset);
            string name = $"{trail}/{header.files[i]}";
            var chunk = new AppMain.AmbChunk(raw, offset, length, header);

            if (id == AmbFileId)
            {
                var nested = NnArchive.ReadNested(chunk);
                if (nested != null)
                    WalkArchive(nested, nested.data, name, tally);
                continue;
            }

            if (id != AmeFileId)
                continue;

            SurveyAme(chunk, name, tally);
        }
    }

    private static void SurveyAme(AppMain.AmbChunk chunk, string name, Tally tally)
    {
        try
        {
            // raw, so the report says which order the file is in even when the parse then fails
            int rawVersion = BitConverter.ToInt32(chunk.array, chunk.offset + 4);
            bool bigEndian = rawVersion != 0x00020000;

            var ame = AppMain.readAMEfile(chunk);

            tally.Files++;
            if (bigEndian)
                tally.BigEndian++;

            for (int i = 0; i < ame.node.Length; ++i)
            {
                var node = ame.node[i];
                tally.Nodes++;
                if (node == null)
                {
                    tally.NullNodes++;
                    if (i == ame.node.Length - 1)
                        tally.NullAtEnd++;
                    else
                        tally.UnhandledIn.Add($"{name} node {i} of {ame.node.Length}");
                    continue;
                }

                ushort type = (ushort)node.type;
                tally.Types.TryGetValue(type, out int count);
                tally.Types[type] = count + 1;
            }
        }
        catch (Exception ex)
        {
            tally.Failures.Add($"{name}: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void Report(string directory, int archives, Tally tally)
    {
        Console.WriteLine($"{directory}");
        Console.WriteLine($"  {archives} archive(s), {tally.Files} AME parsed, "
            + $"{tally.BigEndian} big endian, {tally.Files - tally.BigEndian} little");
        Console.WriteLine($"  {tally.Nodes} nodes, {tally.NullNodes} null "
            + $"({tally.NullAtEnd} trailing, {tally.NullNodes - tally.NullAtEnd} mid-list)");
        Console.WriteLine();

        Console.WriteLine("  node types");
        foreach (var pair in tally.Types.OrderBy(p => p.Key))
        {
            string label = Enum.IsDefined(typeof(AppMain.AME_AME_NODE_TYPE), pair.Key)
                ? ((AppMain.AME_AME_NODE_TYPE)pair.Key).ToString()
                : "UNKNOWN";
            Console.WriteLine($"    0x{pair.Key:X4}  {pair.Value,6}  {label}");
        }

        if (tally.UnhandledIn.Count != 0)
        {
            Console.WriteLine();
            Console.WriteLine($"  {tally.UnhandledIn.Count} file(s) with an unhandled node type");
            foreach (string file in tally.UnhandledIn)
                Console.WriteLine($"    {file}");
        }

        if (tally.Failures.Count != 0)
        {
            Console.WriteLine();
            Console.WriteLine($"  {tally.Failures.Count} failure(s)");
            foreach (string failure in tally.Failures)
                Console.WriteLine($"    {failure}");
        }
    }
}
