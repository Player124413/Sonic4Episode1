using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Viewer;

// prints what one motion is made of, for when it looks wrong and its neighbours look fine
internal static class MotionDescribe
{
    public static int Run(string archivePath, int first, int last)
    {
        if (!File.Exists(archivePath))
        {
            Console.Error.WriteLine($"no such archive: {archivePath}");
            return 1;
        }

        foreach (var (index, motion, error) in NnArchive.ReadMotions(archivePath))
        {
            if (index < first || index > last)
                continue;

            if (error != null)
            {
                Console.WriteLine($"motion {index}: FAILED {error}");
                continue;
            }

            Describe(index, motion);
        }

        return 0;
    }

    public static int RunModel(string archivePath)
    {
        if (!File.Exists(archivePath))
        {
            Console.Error.WriteLine($"no such archive: {archivePath}");
            return 1;
        }

        var objects = NnArchive.ReadObjects(archivePath);
        Console.WriteLine($"{objects.Count} object(s) in {Path.GetFileName(archivePath)}");

        for (int i = 0; i < objects.Count; ++i)
        {
            var obj = objects[i];
            Console.WriteLine($"  object {i}: {obj.nNode} node(s), {obj.nMtxPal} palette entries,"
                + $" {obj.nMaterial} material(s), {obj.nSubobj} subobject(s), {obj.nVtxList} vertex list(s)");
        }

        return 0;
    }

    private static void Describe(int index, AppMain.NNS_MOTION motion)
    {
        uint category = motion.fType & AppMain.NND_MOTIONTYPE_CATEGORY_MASK;
        uint repeat = motion.fType & AppMain.NND_MOTIONTYPE_REPEAT_MASK;

        Console.WriteLine(
            $"motion {index}: fType 0x{motion.fType:X} category 0x{category:X} repeat {RepeatName(repeat)}"
            + $" frames {motion.StartFrame}..{motion.EndFrame} at {motion.FrameRate}fps"
            + $" {motion.nSubmotion} submotion(s)");

        if (motion.pSubmotion == null)
            return;

        var shapes = new Dictionary<string, int>();
        int lowestNode = int.MaxValue;
        int highestNode = -1;
        var repeats = new Dictionary<string, int>();

        for (int i = 0; i < motion.nSubmotion; ++i)
        {
            var sub = motion.pSubmotion[i];
            lowestNode = Math.Min(lowestNode, sub.Id);
            highestNode = Math.Max(highestNode, sub.Id);

            string keys = sub.pKeyList == null
                ? "null"
                : sub.pKeyList.GetType().GetElementType()?.Name?.Replace("NNS_MOTION_KEY_", "") ?? "?";

            string shape = $"fType 0x{sub.fType:X} ip 0x{sub.fIPType & AppMain.NND_SMOTIPTYPE_IP_MASK:X}"
                + $" KeySize {sub.KeySize} {keys}";
            shapes[shape] = shapes.TryGetValue(shape, out int n) ? n + 1 : 1;

            string sr = RepeatName(sub.fIPType & AppMain.NND_SMOTIPTYPE_REPEAT_MASK);
            repeats[sr] = repeats.TryGetValue(sr, out int m) ? m + 1 : 1;

            // an empty key list evaluates to something finite and wrong, not a failure
            if (sub.nKeyFrame <= 0)
                Console.WriteLine($"    submotion {i} node {sub.Id} has {sub.nKeyFrame} keys");
        }

        Console.WriteLine($"  nodes {lowestNode}..{highestNode}");
        Console.WriteLine($"  submotion repeat: {string.Join(", ", repeats.Select(r => $"{r.Key} x{r.Value}"))}");
        foreach (var shape in shapes.OrderByDescending(s => s.Value))
            Console.WriteLine($"  {shape.Value,5}x {shape.Key}");
    }

    private static string RepeatName(uint repeat)
    {
        if (repeat == 0) return "none";

        var parts = new List<string>();
        if ((repeat & AppMain.NND_MOTIONTYPE_NOREPEAT) != 0) parts.Add("NOREPEAT");
        if ((repeat & AppMain.NND_MOTIONTYPE_CONSTREPEAT) != 0) parts.Add("CONSTREPEAT");
        if ((repeat & AppMain.NND_MOTIONTYPE_REPEAT) != 0) parts.Add("REPEAT");
        if ((repeat & AppMain.NND_SMOTIPTYPE_MIRROR) != 0) parts.Add("MIRROR");
        if ((repeat & AppMain.NND_SMOTIPTYPE_OFFSET) != 0) parts.Add("OFFSET");

        return parts.Count == 0 ? $"0x{repeat:X}" : string.Join("|", parts);
    }
}
