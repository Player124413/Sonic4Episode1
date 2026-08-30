using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Viewer;

// runs real motion through the real evaluation path with nothing drawn, so a report is a motion
// bug rather than a shader/vertex/texture one. Both failure modes here are silent otherwise:
// mppAssertNotImpl and Matrix.CheckForNaNs only fire under DEBUG.
internal static class MotionHarness
{
    private enum Verdict
    {
        Clean,
        NotExercised,
        ThrewOnRead,
        ThrewOnEvaluate,
        BadMatrix,
    }

    private sealed class Result
    {
        public string Archive;
        public int Index;
        public Verdict Verdict;
        public string Detail;
    }

    // half-frame steps so interpolation between keys gets hit, not just the keys themselves
    private const float FrameStep = 0.5f;
    private const int MaxFramesPerMotion = 4000;

    public static int Run(string target)
    {
        string[] archives = File.Exists(target)
            ? new[] { target }
            : Directory.GetFiles(target, "*.AMB", SearchOption.AllDirectories);

        Console.WriteLine($"{archives.Length} archive(s)");
        var results = new List<Result>();
        int withMotion = 0;

        foreach (string archive in archives)
        {
            if (RunArchive(archive, results))
                ++withMotion;
        }

        Console.WriteLine($"{withMotion} archive(s) carried motion");
        Report(results);
        return 0;
    }

    // try the objects in this same archive first, only look for a sibling model if it has none
    private static bool RunArchive(string motionPath, List<Result> results)
    {
        string archive = Path.GetFileName(motionPath);

        List<(int Index, AppMain.NNS_MOTION Motion, string Error)> motions;
        try
        {
            motions = NnArchive.ReadMotions(motionPath).ToList();
        }
        catch (Exception ex)
        {
            results.Add(new Result
            {
                Archive = archive, Index = -1, Verdict = Verdict.ThrewOnRead,
                Detail = $"archive: {ex.GetType().Name} {ex.Message}",
            });
            return true;
        }

        if (motions.Count == 0)
            return false;

        List<AppMain.NNS_OBJECT> objects;
        try
        {
            objects = NnArchive.ReadObjects(motionPath);
            if (objects.Count == 0)
            {
                string model = SiblingModel(motionPath);
                if (model != null)
                    objects = NnArchive.ReadObjects(model);
            }
        }
        catch (Exception ex)
        {
            results.Add(new Result
            {
                Archive = archive, Index = -1, Verdict = Verdict.ThrewOnRead,
                Detail = $"model: {ex.GetType().Name} {ex.Message}",
            });
            return true;
        }

        if (objects.Count == 0)
        {
            results.Add(new Result
            {
                Archive = archive, Index = -1, Verdict = Verdict.NotExercised,
                Detail = "no object to animate, in this archive or beside it",
            });
            return true;
        }

        foreach (var (index, motion, readError) in motions)
        {
            if (readError != null)
            {
                results.Add(new Result
                {
                    Archive = archive, Index = index,
                    Verdict = Verdict.ThrewOnRead, Detail = readError,
                });
                continue;
            }

            results.Add(Evaluate(ObjectFor(objects, motion), motion, archive, index));
        }

        return true;
    }

    // _MTN/_MAT motion animates the object in the _MDL beside it; a numbered variant like
    // GMK_TP_TURN_8_MAT.AMB shares GMK_TP_TURN_MDL.AMB, so drop a trailing number and retry
    private static string SiblingModel(string motionPath)
    {
        string directory = Path.GetDirectoryName(motionPath);
        string name = Path.GetFileName(motionPath);

        foreach (string tag in new[] { "_MTN", "_MAT" })
        {
            int at = name.IndexOf(tag, StringComparison.OrdinalIgnoreCase);
            if (at < 0)
                continue;

            string tail = name.Substring(at + tag.Length);
            foreach (string stem in Stems(name.Substring(0, at)))
            {
                string model = Path.Combine(directory, stem + "_MDL" + tail);
                if (File.Exists(model))
                    return model;

                // one material motion can drive a whole family of models; any exercises the same code
                string[] family = Directory.GetFiles(directory, stem + "*_MDL*.AMB");
                if (family.Length > 0)
                    return family[0];
            }
        }

        // maps don't use _MDL at all, e.g. ZONE2_M.AMB vs ZONE2_M_M.AMB; a short trailing tag separates them
        string bare = Path.GetFileNameWithoutExtension(name);
        int tagAt = bare.LastIndexOf('_');
        if (tagAt > 0 && bare.Length - tagAt - 1 <= 2)
        {
            string model = Path.Combine(directory, bare.Substring(0, tagAt) + ".AMB");
            if (File.Exists(model))
                return model;
        }

        return null;
    }

    private static IEnumerable<string> Stems(string stem)
    {
        yield return stem;

        int at = stem.LastIndexOf('_');
        if (at > 0 && stem.Skip(at + 1).All(char.IsDigit) && at + 1 < stem.Length)
            yield return stem.Substring(0, at);
    }

    // submotion indices are the only clue which object a motion belongs to; falls back to the
    // first object so a genuine overrun still gets reported rather than throwing here
    private static AppMain.NNS_OBJECT ObjectFor(
        List<AppMain.NNS_OBJECT> objects, AppMain.NNS_MOTION motion)
    {
        if (motion?.pSubmotion == null)
            return objects[0];

        // node motion indexes nodes through Id, material motion indexes materials through Id0
        bool material = (motion.fType & AppMain.NND_MOTIONTYPE_NODE) == 0
            && (motion.fType & AppMain.NND_MOTIONTYPE_MATERIAL) != 0;

        int highest = -1;
        for (int i = 0; i < motion.nSubmotion; ++i)
        {
            var sub = motion.pSubmotion[i];
            highest = Math.Max(highest, material ? sub.Id0 : sub.Id);
        }

        // tightest fit, not first: a motion for a small object can fit inside a bigger one too
        AppMain.NNS_OBJECT best = null;
        int bestSize = int.MaxValue;

        foreach (var candidate in objects)
        {
            int size = material ? candidate.nMaterial : candidate.nNode;
            if (highest < size && size < bestSize)
            {
                best = candidate;
                bestSize = size;
            }
        }

        return best ?? objects[0];
    }

    private static Result Evaluate(AppMain.NNS_OBJECT obj, AppMain.NNS_MOTION motion, string archive, int index)
    {
        var result = new Result { Archive = archive, Index = index, Verdict = Verdict.Clean };
        if (motion == null)
        {
            result.Verdict = Verdict.ThrewOnRead;
            result.Detail = "motion read as null";
            return result;
        }

        // nnCalcMatrixPaletteMotion no-ops on anything but node motion, route the rest ourselves
        if ((motion.fType & AppMain.NND_MOTIONTYPE_NODE) == 0)
        {
            if ((motion.fType & AppMain.NND_MOTIONTYPE_MATERIAL) != 0)
                return EvaluateMaterial(obj, motion, archive, index);

            result.Verdict = Verdict.NotExercised;
            result.Detail = $"motion category 0x{motion.fType & AppMain.NND_MOTIONTYPE_CATEGORY_MASK:X}"
                + " has no evaluation path here";
            return result;
        }

        int count = obj.nNode;
        var palette = new NNS_MATRIX[count];
        for (int i = 0; i < count; ++i)
            palette[i] = NNS_MATRIX.CreateIdentity();

        var nstat = new uint[count];
        var stack = new AppMain.NNS_MATRIXSTACK();

        // a node index past the object's count means we paired this motion with the wrong object
        int highestNode = -1;
        for (int i = 0; i < motion.nSubmotion && motion.pSubmotion != null; ++i)
            highestNode = Math.Max(highestNode, motion.pSubmotion[i].Id);

        if (highestNode >= count)
        {
            result.Verdict = Verdict.BadMatrix;
            result.Detail = $"drives node {highestNode} but the object has {count}";
            return result;
        }

        float start = motion.StartFrame;
        float end = motion.EndFrame;
        if (!(start <= end))
        {
            result.Verdict = Verdict.BadMatrix;
            result.Detail = $"frame range is {start}..{end}";
            return result;
        }

        int steps = Math.Min((int)((end - start) / FrameStep) + 1, MaxFramesPerMotion);
        for (int step = 0; step < steps; ++step)
        {
            float frame = start + step * FrameStep;
            try
            {
                AppMain.nnSetUpNodeStatusList(nstat, count, AppMain.NND_NODESTATUS_NONE);
                AppMain.nnCalcMatrixPaletteMotion(
                    palette, nstat, obj, motion, frame, null, stack,
                    AppMain.NND_SETNODESTATUS_CLIP_HIDE);
                AppMain.nnCalcNodeHideMotion(new AppMain.ArrayPointer<uint>(nstat), motion, frame);
            }
            catch (Exception ex)
            {
                result.Verdict = Verdict.ThrewOnEvaluate;
                result.Detail = $"frame {frame}: {ex.GetType().Name}: {ex.Message}";
                return result;
            }

            string bad = FirstBadEntry(palette, obj, out int badNode);
            if (bad != null)
            {
                result.Verdict = Verdict.BadMatrix;
                result.Detail = $"frame {frame}: {bad}; {DescribeSubmotions(motion, badNode)}";
                return result;
            }
        }

        return result;
    }

    // material motion writes into a shadow object holding the animated copies, not the palette
    private static Result EvaluateMaterial(
        AppMain.NNS_OBJECT obj, AppMain.NNS_MOTION motion, string archive, int index)
    {
        var result = new Result { Archive = archive, Index = index, Verdict = Verdict.Clean };

        // Id0 selects the material; an overrun means the wrong object, same as the node index check
        int highestMaterial = -1;
        for (int i = 0; i < motion.nSubmotion && motion.pSubmotion != null; ++i)
            highestMaterial = Math.Max(highestMaterial, motion.pSubmotion[i].Id0);

        if (highestMaterial >= obj.nMaterial)
        {
            result.Verdict = Verdict.BadMatrix;
            result.Detail = $"drives material {highestMaterial} but the object has {obj.nMaterial}";
            return result;
        }

        var shadow = new AppMain.NNS_OBJECT();
        float start = motion.StartFrame;
        float end = motion.EndFrame;
        if (!(start <= end))
        {
            result.Verdict = Verdict.BadMatrix;
            result.Detail = $"frame range is {start}..{end}";
            return result;
        }

        int steps = Math.Min((int)((end - start) / FrameStep) + 1, MaxFramesPerMotion);
        for (int step = 0; step < steps; ++step)
        {
            float frame = start + step * FrameStep;
            try
            {
                AppMain.nnInitMaterialMotionObject(shadow, obj, motion);
                AppMain.nnCalcMaterialMotion(shadow, obj, motion, frame);
            }
            catch (Exception ex)
            {
                result.Verdict = Verdict.ThrewOnEvaluate;
                result.Detail = $"frame {frame}: {ex.GetType().Name}: {ex.Message}";
                return result;
            }

            string bad = FirstBadMaterial(shadow);
            if (bad != null)
            {
                result.Verdict = Verdict.BadMatrix;
                result.Detail = $"frame {frame}: {bad}";
                return result;
            }
        }

        return result;
    }

    private static string FirstBadMaterial(AppMain.NNS_OBJECT shadow)
    {
        if (shadow.pMatPtrList == null)
            return null;

        for (int i = 0; i < shadow.pMatPtrList.Length; ++i)
        {
            if (!(shadow.pMatPtrList[i]?.pMaterial is AppMain.NNS_MATERIAL_STDSHADER_DESC material))
                continue;

            var c = material.pColor;
            if (c == null)
                continue;

            foreach (float f in new[]
                     {
                         c.Diffuse.r, c.Diffuse.g, c.Diffuse.b, c.Diffuse.a,
                         c.Ambient.r, c.Ambient.g, c.Ambient.b,
                         c.Specular.r, c.Specular.g, c.Specular.b,
                         c.Emission.r, c.Emission.g, c.Emission.b,
                         c.Shininess, c.SpecularIntensity,
                     })
            {
                if (float.IsNaN(f) || float.IsInfinity(f))
                    return $"material {i} colour is {(float.IsNaN(f) ? "NaN" : "infinite")}";
            }
        }

        return null;
    }

    // only entries a node actually claims are checked; NN never writes the palette's spare tail
    private static string FirstBadEntry(NNS_MATRIX[] palette, AppMain.NNS_OBJECT obj, out int badNode)
    {
        for (int i = 0; i < obj.nNode; ++i)
        {
            int at = obj.pNodeList[i].iMatrix;
            if (at < 0 || at >= palette.Length)
                continue;

            string fault = FaultIn(palette[at]);
            if (fault != null)
            {
                badNode = i;
                return $"node {i} (fType 0x{obj.pNodeList[i].fType:X}) matrix {at} {fault}";
            }
        }

        badNode = -1;
        return null;
    }

    private static string DescribeSubmotions(AppMain.NNS_MOTION motion, int nodeIdx)
    {
        if (motion.pSubmotion == null || nodeIdx < 0)
            return "no submotions";

        var parts = new List<string>();
        for (int i = 0; i < motion.nSubmotion; ++i)
        {
            var sub = motion.pSubmotion[i];
            if (sub.Id != nodeIdx)
                continue;

            string keys = sub.pKeyList == null
                ? "null"
                : sub.pKeyList.GetType().GetElementType()?.Name ?? sub.pKeyList.GetType().Name;

            parts.Add($"fType 0x{sub.fType:X}/ip 0x{sub.fIPType:X} keys {keys} n {sub.nKeyFrame}");
        }

        return parts.Count == 0 ? "no submotions for this node" : string.Join(" | ", parts);
    }

    private static string FaultIn(NNS_MATRIX m)
    {
        float[] all =
        {
            m.M00, m.M01, m.M02, m.M03, m.M10, m.M11, m.M12, m.M13,
            m.M20, m.M21, m.M22, m.M23, m.M30, m.M31, m.M32, m.M33,
        };

        foreach (float f in all)
        {
            if (float.IsNaN(f))
                return "is NaN";
            if (float.IsInfinity(f))
                return "is infinite";
        }

        // a zero 3x3 with a live translation row is the shape an unfilled sin/cos table produces
        float det =
            m.M00 * (m.M11 * m.M22 - m.M12 * m.M21)
            - m.M01 * (m.M10 * m.M22 - m.M12 * m.M20)
            + m.M02 * (m.M10 * m.M21 - m.M11 * m.M20);

        if (det != 0f)
            return null;

        // a singular matrix isn't automatically a bug, a legit zero-scale axis produces one too;
        // only a wholly dead 3x3 (every column zero) is corruption
        double x = Math.Sqrt(m.M00 * m.M00 + m.M10 * m.M10 + m.M20 * m.M20);
        double y = Math.Sqrt(m.M01 * m.M01 + m.M11 * m.M11 + m.M21 * m.M21);
        double z = Math.Sqrt(m.M02 * m.M02 + m.M12 * m.M12 + m.M22 * m.M22);

        if (x != 0.0 || y != 0.0 || z != 0.0)
            return null;

        return $"has a dead 3x3 (column lengths {x:0.###}, {y:0.###}, {z:0.###})";
    }

    private static void Report(List<Result> results)
    {
        Console.WriteLine($"{results.Count} motion(s) exercised");
        Console.WriteLine();

        foreach (Verdict verdict in new[]
                 { Verdict.ThrewOnRead, Verdict.ThrewOnEvaluate, Verdict.BadMatrix, Verdict.NotExercised })
        {
            var of = results.Where(r => r.Verdict == verdict).ToList();
            if (of.Count == 0)
                continue;

            Console.WriteLine($"{verdict}: {of.Count}");
            foreach (var group in of.GroupBy(r => r.Detail).OrderByDescending(g => g.Count()).Take(12))
            {
                var first = group.First();
                Console.WriteLine($"  {group.Count(),5}x  {group.Key}");
                Console.WriteLine($"           first: {first.Archive} motion {first.Index}");
            }
            Console.WriteLine();
        }

        int clean = results.Count(r => r.Verdict == Verdict.Clean);
        int skipped = results.Count(r => r.Verdict == Verdict.NotExercised);
        Console.WriteLine($"clean: {clean} of {results.Count} ({skipped} not exercised)");
    }
}
