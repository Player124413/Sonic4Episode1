using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Content;

// Harvests (D3D9, Xenon) render state pairs out of the corpus. The same model shipped in both the
// Steam and XBLA builds describes the same material with different numbers, because
// nnfmaterialdxg20.h aliases every NND_*_STDSHADER_* onto that platform's D3D macro. One sample
// would be guesswork; this walks every model present in both trees and reports what actually occurs.
//
// See docs/nn-asset-loading-paths.md for the worked example this generalises.
internal static class XenonEnums
{
    private sealed class Field
    {
        public readonly string Name;
        public readonly Dictionary<uint, Dictionary<uint, int>> Pairs = new Dictionary<uint, Dictionary<uint, int>>();

        public Field(string name) => Name = name;

        public void Add(uint pc, uint xenon)
        {
            if (!Pairs.TryGetValue(pc, out var seen))
                Pairs[pc] = seen = new Dictionary<uint, int>();

            seen.TryGetValue(xenon, out int count);
            seen[xenon] = count + 1;
        }
    }

    private sealed class Harvest
    {
        public readonly Dictionary<string, Field> Fields = new Dictionary<string, Field>(StringComparer.Ordinal);

        public int Archives;
        public int Models;
        public int Materials;
        public int SkippedShape;
        public int SkippedAnchor;

        public Field this[string name]
        {
            get
            {
                if (!Fields.TryGetValue(name, out var field))
                    Fields[name] = field = new Field(name);
                return field;
            }
        }
    }

    public static int Run(string[] args)
    {
        var sources = ContentSources.Load(ContentSources.Locate(Directory.GetCurrentDirectory()));
        string pcName = args.Length > 1 ? args[1] : "pc";
        string xenonName = args.Length > 2 ? args[2] : "xbla";

        var pcRoot = sources.Source(pcName).Root;
        var xenonRoot = sources.Source(xenonName).Root;

        if (!Directory.Exists(pcRoot) || !Directory.Exists(xenonRoot))
        {
            Console.Error.WriteLine($"need both trees: {pcRoot} and {xenonRoot}");
            return 1;
        }

        var harvest = new Harvest();

        foreach (var relative in CommonArchives(pcRoot, xenonRoot))
        {
            try
            {
                Walk(harvest, Path.Combine(pcRoot, relative), Path.Combine(xenonRoot, relative));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"  {relative}: {ex.Message}");
            }
        }

        Report(harvest, pcName, xenonName);
        Census(pcName, pcRoot);
        Census(xenonName, xenonRoot);
        return 0;
    }

    // What the harvest covers is only what both trees ship. This is the other half: every distinct
    // value the Xenon tree actually uses, so a mapping can be called complete rather than hoped at.
    private static void Census(string name, string root)
    {
        var seen = new Dictionary<string, SortedSet<uint>>(StringComparer.Ordinal);
        var failures = new Dictionary<string, int>(StringComparer.Ordinal);
        int members = 0;
        int objects = 0;

        void Note(string field, uint value)
        {
            if (!seen.TryGetValue(field, out var values))
                seen[field] = values = new SortedSet<uint>();
            values.Add(value);
        }

        foreach (var path in Directory.EnumerateFiles(root, "*.AMB", SearchOption.AllDirectories))
        {
            AppMain.AMS_AMB_HEADER archive;
            try
            {
                archive = ReadArchive(path);
            }
            catch (Exception)
            {
                continue;
            }

            for (int i = 0; i < archive.file_num; i++)
            {
                members++;

                AppMain.NNS_OBJECT obj;
                try
                {
                    obj = ReadObject(AppMain.amBindGet(archive, i, out _));
                }
                catch (Exception ex)
                {
                    // a member that will not parse is the interesting case, not a skippable one:
                    // an unmapped enum lands here and would otherwise vanish from the census
                    var key = ex.GetType().Name + ": " + ex.Message;
                    failures.TryGetValue(key, out int count);
                    failures[key] = count + 1;
                    continue;
                }

                if (obj == null)
                    continue;

                objects++;

                for (int m = 0; m < obj.nMaterial; m++)
                {
                    if (!(obj.pMatPtrList[m].pMaterial is AppMain.NNS_MATERIAL_STDSHADER_DESC desc))
                        continue;

                    var logic = desc.pLogic;
                    if (logic != null)
                    {
                        bool blends = (logic.fFlag & 1) != 0;
                        bool separateAlpha = (logic.fFlag & 2) != 0;

                        if (blends)
                        {
                            Note("D3DBLEND", logic.SrcFactorRGB);
                            Note("D3DBLEND", logic.DstFactorRGB);
                            Note("D3DBLENDOP", logic.BlendOp);
                        }

                        if (blends && separateAlpha)
                        {
                            Note("D3DBLEND", logic.SrcFactorA);
                            Note("D3DBLEND", logic.DstFactorA);
                        }

                        if ((logic.fFlag & 8) != 0)
                            Note("D3DCMPFUNC", logic.AlphaFunc);
                        if ((logic.fFlag & 0x10) != 0)
                            Note("D3DCMPFUNC", logic.DepthFunc);
                    }

                    foreach (var tex in desc.pTexDesc ?? Array.Empty<AppMain.NNS_MATERIAL_STDSHADER_TEXMAP_DESC>())
                    {
                        Note("D3DTEXTUREADDRESS", (uint)tex.WrapS);
                        Note("D3DTEXTUREADDRESS", (uint)tex.WrapT);
                    }
                }
            }
        }

        Console.WriteLine($"every value {name} actually uses, live fields only"
            + $" ({objects} object(s) out of {members} member(s), {failures.Values.Sum()} unreadable):");

        foreach (var field in seen.Keys.OrderBy(k => k, StringComparer.Ordinal))
            Console.WriteLine($"  {field,-18} {string.Join(", ", seen[field])}");

        foreach (var failure in failures.OrderByDescending(kv => kv.Value).Take(10))
            Console.WriteLine($"  x{failure.Value} {failure.Key}");
    }

    private static IEnumerable<string> CommonArchives(string pcRoot, string xenonRoot)
    {
        var xenon = new HashSet<string>(
            Directory.EnumerateFiles(xenonRoot, "*.AMB", SearchOption.AllDirectories)
                .Select(p => Path.GetRelativePath(xenonRoot, p).Replace('\\', '/')),
            StringComparer.OrdinalIgnoreCase);

        return Directory.EnumerateFiles(pcRoot, "*.AMB", SearchOption.AllDirectories)
            .Select(p => Path.GetRelativePath(pcRoot, p).Replace('\\', '/'))
            .Where(xenon.Contains)
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase);
    }

    private static void Walk(Harvest harvest, string pcPath, string xenonPath)
    {
        var pc = ReadArchive(pcPath);
        var xenon = ReadArchive(xenonPath);

        harvest.Archives++;

        // by member name where there is one, since a build can insert or drop a member
        var xenonByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < xenon.file_num; i++)
        {
            var name = MemberName(xenon, i);
            if (name != null)
                xenonByName[name] = i;
        }

        for (int i = 0; i < pc.file_num; i++)
        {
            var name = MemberName(pc, i);
            int j = i;

            if (name != null && xenonByName.TryGetValue(name, out int found))
                j = found;
            else if (name != null && xenonByName.Count > 0)
                continue;

            if (j >= xenon.file_num)
                continue;

            var left = ReadObject(AppMain.amBindGet(pc, i, out _));
            var right = ReadObject(AppMain.amBindGet(xenon, j, out _));
            if (left == null || right == null)
                continue;

            Pair(harvest, left, right);
        }
    }

    // by stem: the same member is .ZNO on PC and .ENO on Xenon, which is 110 of the 370 shared
    // archives per docs/content-catalog.md
    private static string MemberName(AppMain.AMS_AMB_HEADER header, int index)
    {
        var name = header.files?[index];
        return string.IsNullOrEmpty(name) ? null : Path.GetFileNameWithoutExtension(name.Replace('\\', '/'));
    }

    private static void Pair(Harvest harvest, AppMain.NNS_OBJECT pc, AppMain.NNS_OBJECT xenon)
    {
        if (pc.nMaterial != xenon.nMaterial || pc.nSubobj != xenon.nSubobj || pc.nNode != xenon.nNode)
        {
            harvest.SkippedShape++;
            return;
        }

        harvest.Models++;

        for (int m = 0; m < pc.nMaterial; m++)
        {
            if (!(pc.pMatPtrList[m].pMaterial is AppMain.NNS_MATERIAL_STDSHADER_DESC a))
                continue;
            if (!(xenon.pMatPtrList[m].pMaterial is AppMain.NNS_MATERIAL_STDSHADER_DESC b))
                continue;

            // fFlag, fTexType and nTex are platform-neutral bit sets, so a disagreement means the
            // two materials are not the same material and nothing below them can be trusted
            if (a.fFlag != b.fFlag || a.fTexType != b.fTexType || a.nTex != b.nTex)
            {
                harvest.SkippedAnchor++;
                continue;
            }

            harvest.Materials++;
            PairLogic(harvest, a.pLogic, b.pLogic);
            PairTexMaps(harvest, a, b);
        }
    }

    private static void PairLogic(
        Harvest harvest,
        AppMain.NNS_MATERIAL_STDSHADER_LOGIC pc,
        AppMain.NNS_MATERIAL_STDSHADER_LOGIC xenon)
    {
        if (pc == null || xenon == null)
            return;

        // LogicOp and fFlag are the offset anchors from the worked example: both are the same value
        // on both platforms, so if they disagree the struct is being read at the wrong offset
        if (pc.fFlag != xenon.fFlag || pc.LogicOp != xenon.LogicOp
            || pc.AlphaRef != xenon.AlphaRef || pc.BlendColor != xenon.BlendColor)
        {
            harvest.SkippedAnchor++;
            return;
        }

        bool blends = (pc.fFlag & 1) != 0;
        bool separateAlpha = (pc.fFlag & 2) != 0;
        bool alphaTests = (pc.fFlag & 8) != 0;
        bool depthTests = (pc.fFlag & 0x10) != 0;

        Add(harvest, "D3DBLEND", blends, pc.SrcFactorRGB, xenon.SrcFactorRGB);
        Add(harvest, "D3DBLEND", blends, pc.DstFactorRGB, xenon.DstFactorRGB);
        Add(harvest, "D3DBLEND", blends && separateAlpha, pc.SrcFactorA, xenon.SrcFactorA);
        Add(harvest, "D3DBLEND", blends && separateAlpha, pc.DstFactorA, xenon.DstFactorA);
        Add(harvest, "D3DBLENDOP", blends, pc.BlendOp, xenon.BlendOp);
        Add(harvest, "D3DCMPFUNC", alphaTests, pc.AlphaFunc, xenon.AlphaFunc);
        Add(harvest, "D3DCMPFUNC", depthTests, pc.DepthFunc, xenon.DepthFunc);
    }

    private static void PairTexMaps(
        Harvest harvest,
        AppMain.NNS_MATERIAL_STDSHADER_DESC pc,
        AppMain.NNS_MATERIAL_STDSHADER_DESC xenon)
    {
        if (pc.pTexDesc == null || xenon.pTexDesc == null)
            return;

        for (int t = 0; t < pc.pTexDesc.Length && t < xenon.pTexDesc.Length; t++)
        {
            var a = pc.pTexDesc[t];
            var b = xenon.pTexDesc[t];

            if (a.fType != b.fType || a.iTexIdx != b.iTexIdx)
            {
                harvest.SkippedAnchor++;
                continue;
            }

            Add(harvest, "D3DTEXTUREADDRESS", true, (uint)a.WrapS, (uint)b.WrapS);
            Add(harvest, "D3DTEXTUREADDRESS", true, (uint)a.WrapT, (uint)b.WrapT);
        }
    }

    // a field the flags say is switched off still holds whatever the exporter wrote, which is worth
    // seeing but not worth trusting, so the two are counted apart
    private static void Add(Harvest harvest, string field, bool live, uint pc, uint xenon)
    {
        harvest[live ? field : field + " (unused)"].Add(pc, xenon);
    }

    private static AppMain.NNS_OBJECT ReadObject(AppMain.AmbChunk chunk)
    {
        if (chunk == null || chunk.length < 32)
            return null;

        uint id = BinaryPrimitives.ReadUInt32LittleEndian(chunk.array.AsSpan(chunk.offset));
        bool bigEndian;

        if (id == AppMain.NND_CHUNK_HEADER_GLES11 || id == AppMain.NND_CHUNK_HEADER_DXG20)
            bigEndian = false;
        else if (BinaryPrimitives.ReverseEndianness(id) == AppMain.NND_CHUNK_HEADER_XE)
            bigEndian = true;
        else
            return null;

        var bytes = new byte[chunk.length];
        Array.Copy(chunk.array, chunk.offset, bytes, 0, chunk.length);

        using var stream = new MemoryStream(bytes);
        using BinaryReader reader = bigEndian
            ? new EndianBinaryReader(stream, bigEndian: true)
            : new BinaryReader(stream);

        AppMain.amObjectSetup(out var obj, out _, reader);
        return obj;
    }

    private static AppMain.AMS_AMB_HEADER ReadArchive(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var header = new AppMain.AMS_AMB_HEADER { data = bytes };

        using var stream = new MemoryStream(bytes);
        AppMain.readAmbHeader(stream, ref header);

        return header;
    }

    private static void Report(Harvest harvest, string pcName, string xenonName)
    {
        Console.WriteLine($"{harvest.Archives} archive(s) in both trees, {harvest.Models} model pair(s),"
            + $" {harvest.Materials} material pair(s)");
        Console.WriteLine($"skipped: {harvest.SkippedShape} on model shape, {harvest.SkippedAnchor} on anchor");
        Console.WriteLine();

        foreach (var field in harvest.Fields.Values.OrderBy(f => f.Name, StringComparer.Ordinal))
        {
            Console.WriteLine($"{field.Name}  ({pcName} -> {xenonName})");

            foreach (var pc in field.Pairs.Keys.OrderBy(v => v))
            {
                var seen = field.Pairs[pc];
                var total = seen.Values.Sum();

                if (seen.Count == 1)
                {
                    var only = seen.First();
                    Console.WriteLine($"  {pc,3} -> {only.Key,-3}  {total} sample(s)");
                    continue;
                }

                var split = string.Join(", ", seen
                    .OrderByDescending(kv => kv.Value)
                    .Select(kv => $"{kv.Key} x{kv.Value.ToString(CultureInfo.InvariantCulture)}"));

                Console.WriteLine($"  {pc,3} -> AMBIGUOUS  {split}");
            }

            Console.WriteLine();
        }
    }
}
