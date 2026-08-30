using System;
using System.Buffers.Binary;
using Microsoft.Xna.Framework;

// decodes the interleaved vertex buffer of an NNS_VTXLIST_DX_DESC into separate arrays,
// components in D3D FVF order
internal sealed class DXVertexArrays
{
    public Vector3[] Positions;
    public Vector3[] Normals;
    public Vector2[] TexCoords;
    public Color[] Colors;
    public Vector4[] BlendWeights;
    public byte[][] BlendIndices;

    public static DXVertexArrays Decode(AppMain.NNS_VTXLIST_DX_DESC desc, bool bigEndian)
    {
        if (desc.pList == null)
            throw new ArgumentException("vertex list has no data", nameof(desc));

        RejectUnsupported(desc.Format);

        int count = (int)desc.nVertex;
        int stride = (int)desc.Stride;
        byte[] data = desc.pList.Data;

        var arrays = new DXVertexArrays();
        int offset = 0;

        if ((desc.Format & AppMain.NND_VTXMEMBER_DX_POS_F32) != 0u)
        {
            arrays.Positions = ReadVector3(data, offset, stride, count, bigEndian);
            offset += 12;
        }

        int weights = StoredBlendWeights(desc.Format);
        if (weights > 0)
        {
            arrays.BlendWeights = ReadBlendWeights(data, offset, stride, count, weights, bigEndian);
            offset += weights * 4;
        }

        if ((desc.Format & AppMain.NND_VTXMEMBER_DX_BLENDFMT_I8) != 0u)
        {
            arrays.BlendIndices = ReadBlendIndices(data, offset, stride, count);
            offset += 4;
        }

        if ((desc.Format & AppMain.NND_VTXMEMBER_DX_NRM_F32) != 0u)
        {
            arrays.Normals = ReadVector3(data, offset, stride, count, bigEndian);
            offset += 12;
        }

        if ((desc.Format & AppMain.NND_VTXMEMBER_DX_COL_U32) != 0u)
        {
            arrays.Colors = ReadColors(data, offset, stride, count);
            offset += 4;
        }

        if ((desc.Format & AppMain.NND_VTXMEMBER_DX_COL2_U32) != 0u)
            offset += 4;

        int texCount = (int)((desc.Format & AppMain.NND_VTXMEMBER_DX_TEXNUM_MASK) >> AppMain.NND_VTXMEMBER_DX_TEXNUM_SHIFT);
        if (texCount > 0)
        {
            arrays.TexCoords = ReadVector2(data, offset, stride, count, bigEndian);
            offset += texCount * 8;
        }

        if (offset != stride)
            throw new InvalidOperationException($"decoded {offset} bytes per vertex but Stride is {stride}");

        return arrays;
    }

    private static void RejectUnsupported(uint format)
    {
        const uint unsupported =
            AppMain.NND_VTXMEMBER_DX_NRM_U32 |
            AppMain.NND_VTXMEMBER_DX_TAN_F32 |
            AppMain.NND_VTXMEMBER_DX_TAN_U32 |
            AppMain.NND_VTXMEMBER_DX_BNRM_F32 |
            AppMain.NND_VTXMEMBER_DX_BNRM_U32 |
            AppMain.NND_VTXMEMBER_DX_BLENDFMT_U32;

        if ((format & unsupported) != 0u)
            throw new NotSupportedException($"vertex format 0x{format:X8} uses a packed component we have never seen in this game's data");

        uint texFormat = format & AppMain.NND_VTXMEMBER_DX_TEXFMT_MASK;
        if (texFormat != AppMain.NND_VTXMEMBER_DX_TEXFMT_F32)
            throw new NotSupportedException($"vertex format 0x{format:X8} uses non-float texture coordinates");
    }

    // BLEND2/3/4FIXED are 5/6/7, i.e. bones = value - 3, and one weight is implied by the rest
    // summing to 1. The non-FIXED BLEND2/3/4 are marked "no support" in nnfobjectdxg20.h.
    private static int StoredBlendWeights(uint format)
    {
        int blend = (int)((format & AppMain.NND_VTXMEMBER_DX_BLEND_MASK) >> AppMain.NND_VTXMEMBER_DX_BLEND_SHIFT);
        if (blend == 0)
            return 0;

        if (blend < 5)
            throw new NotSupportedException($"envelope mode {blend} is not supported by NN");

        return blend - 3 - 1;
    }

    private static float ReadSingle(byte[] data, int at, bool bigEndian)
    {
        var span = data.AsSpan(at, 4);
        return bigEndian
            ? BinaryPrimitives.ReadSingleBigEndian(span)
            : BinaryPrimitives.ReadSingleLittleEndian(span);
    }

    private static Vector3[] ReadVector3(byte[] data, int offset, int stride, int count, bool bigEndian)
    {
        var values = new Vector3[count];
        for (int i = 0; i < count; ++i)
        {
            int at = i * stride + offset;
            values[i] = new Vector3(
                ReadSingle(data, at, bigEndian),
                ReadSingle(data, at + 4, bigEndian),
                ReadSingle(data, at + 8, bigEndian));
        }
        return values;
    }

    private static Vector2[] ReadVector2(byte[] data, int offset, int stride, int count, bool bigEndian)
    {
        var values = new Vector2[count];
        for (int i = 0; i < count; ++i)
        {
            int at = i * stride + offset;
            values[i] = new Vector2(
                ReadSingle(data, at, bigEndian),
                ReadSingle(data, at + 4, bigEndian));
        }
        return values;
    }

    private static Color[] ReadColors(byte[] data, int offset, int stride, int count)
    {
        var values = new Color[count];
        for (int i = 0; i < count; ++i)
        {
            int at = i * stride + offset;
            values[i] = new Color(data[at + 2], data[at + 1], data[at], data[at + 3]);
        }
        return values;
    }

    private static Vector4[] ReadBlendWeights(byte[] data, int offset, int stride, int count, int stored, bool bigEndian)
    {
        var values = new Vector4[count];
        for (int i = 0; i < count; ++i)
        {
            int at = i * stride + offset;
            float x = ReadSingle(data, at, bigEndian);
            float y = stored > 1 ? ReadSingle(data, at + 4, bigEndian) : 0f;
            float z = stored > 2 ? ReadSingle(data, at + 8, bigEndian) : 0f;
            values[i] = new Vector4(x, y, z, 1f - x - y - z);
        }
        return values;
    }

    private static byte[][] ReadBlendIndices(byte[] data, int offset, int stride, int count)
    {
        var values = new byte[count][];
        for (int i = 0; i < count; ++i)
        {
            int at = i * stride + offset;
            values[i] = new byte[4] { data[at], data[at + 1], data[at + 2], data[at + 3] };
        }
        return values;
    }
}
