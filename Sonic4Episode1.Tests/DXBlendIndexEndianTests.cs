using Microsoft.Xna.Framework;

namespace Sonic4Episode1.Tests;

public class DXBlendIndexEndianTests
{
    // one vertex of the rope's skinned mesh: 3 float weights, then the packed index quad
    private static byte[] Vertex(bool bigEndian)
    {
        var data = new byte[16];

        void Weight(int at, float value)
        {
            var bytes = System.BitConverter.GetBytes(value);
            if (bigEndian)
                System.Array.Reverse(bytes);
            System.Array.Copy(bytes, 0, data, at, 4);
        }

        Weight(0, 0.2f);
        Weight(4, 0.8f);
        Weight(8, 0f);

        // the quad is 0,1,0,0 in memory order on a little-endian file
        if (bigEndian)
        {
            data[12] = 0; data[13] = 0; data[14] = 1; data[15] = 0;
        }
        else
        {
            data[12] = 0; data[13] = 1; data[14] = 0; data[15] = 0;
        }

        return data;
    }

    private static (Vector4 Weights, byte[] Indices) Decode(bool bigEndian)
    {
        var desc = new AppMain.NNS_VTXLIST_DX_DESC
        {
            Format = AppMain.NND_VTXMEMBER_DX_BLEND4FIXED | AppMain.NND_VTXMEMBER_DX_BLENDFMT_I8,
            nVertex = 1,
            Stride = 16,
            pList = ByteBuffer.Wrap(Vertex(bigEndian)),
            mppBigEndian = bigEndian,
        };

        var arrays = DXVertexArrays.Decode(desc, bigEndian);
        return (arrays.BlendWeights[0], arrays.BlendIndices[0]);
    }

    [Fact]
    public void BothByteOrdersDecodeTheSameBoneIndices()
    {
        var little = Decode(bigEndian: false);
        var big = Decode(bigEndian: true);

        Assert.Equal(new byte[] { 0, 1, 0, 0 }, little.Indices);
        Assert.Equal(new byte[] { 0, 1, 0, 0 }, big.Indices);
    }

    [Fact]
    public void BothByteOrdersDecodeTheSameWeights()
    {
        var little = Decode(bigEndian: false);
        var big = Decode(bigEndian: true);

        Assert.Equal(little.Weights.X, big.Weights.X, 5);
        Assert.Equal(little.Weights.Y, big.Weights.Y, 5);
        Assert.Equal(little.Weights.Z, big.Weights.Z, 5);
        Assert.Equal(little.Weights.W, big.Weights.W, 5);
    }

    [Fact]
    public void TheReversedQuadWouldCollapseOntoASingleBone()
    {
        var big = Decode(bigEndian: true);
        var reversed = new byte[] { big.Indices[3], big.Indices[2], big.Indices[1], big.Indices[0] };

        Assert.Equal(new byte[] { 0, 0, 1, 0 }, reversed);

        // weights ride slots 0 and 1, and reversed both of those name bone 0
        Assert.Equal(0, reversed[0]);
        Assert.Equal(0, reversed[1]);
    }
}
