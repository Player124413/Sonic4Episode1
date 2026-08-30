using System;
using System.Collections.Generic;

// flattens the triangle strips of an NNS_PRIMTYPE_DX_STRIPLIST into a triangle list
internal static class DXStripList
{
    public static ushort[] ToTriangleList(AppMain.NNS_PRIMTYPE_DX_STRIPLIST strip)
    {
        if (strip.pList == null || strip.pLengthList == null)
            throw new ArgumentException("strip list has no data", nameof(strip));

        var indices = new List<ushort>((int)strip.nIndex * 3);
        bool startsRight = (strip.Format & AppMain.NND_PRIMMEMBER_DX_ONLY_STRIP_R) != 0u;
        int at = 0;

        for (int s = 0; s < strip.nPrimitive; ++s)
        {
            int length = (int)strip.pLengthList[s];
            if (at + length > strip.pList.Length)
                throw new InvalidOperationException($"strip {s} runs past the {strip.pList.Length} index list");

            AppendStrip(indices, strip.pList, at, length, startsRight);
            at += length;
        }

        return indices.ToArray();
    }

    private static void AppendStrip(List<ushort> indices, ushort[] list, int at, int length, bool startsRight)
    {
        for (int i = 0; i + 2 < length; ++i)
        {
            ushort a = list[at + i];
            ushort b = list[at + i + 1];
            ushort c = list[at + i + 2];

            // skip the degenerate triangles strips stitch with
            if (a == b || b == c || a == c)
                continue;

            bool flip = (i & 1) == 0 ? startsRight : !startsRight;
            indices.Add(a);
            if (flip)
            {
                indices.Add(c);
                indices.Add(b);
            }
            else
            {
                indices.Add(b);
                indices.Add(c);
            }
        }
    }
}
