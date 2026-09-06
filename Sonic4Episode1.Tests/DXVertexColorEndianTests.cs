using Microsoft.Xna.Framework;

namespace Sonic4Episode1.Tests;

// zone 3's darkened wall bricks carry a grey vertex tint with full alpha. read in the wrong byte
// order that tint lands in alpha and the bricks draw see-through
public class DXVertexColorEndianTests
{
    // ARGB 0xFFC0C0C0, as ZONE3_M.AMB stores it on each platform
    private static readonly byte[] LittleEndian = { 0xC0, 0xC0, 0xC0, 0xFF };
    private static readonly byte[] BigEndian = { 0xFF, 0xC0, 0xC0, 0xC0 };

    private static Color Decode(byte[] vertex, bool bigEndian)
    {
        var desc = new AppMain.NNS_VTXLIST_DX_DESC
        {
            Format = AppMain.NND_VTXMEMBER_DX_COL_U32,
            nVertex = 1,
            Stride = 4,
            pList = ByteBuffer.Wrap(vertex),
            mppBigEndian = bigEndian,
        };

        return DXVertexArrays.Decode(desc, bigEndian).Colors[0];
    }

    [Fact]
    public void BothByteOrdersDecodeTheSameTint()
    {
        var expected = new Color(0xC0, 0xC0, 0xC0, 0xFF);

        Assert.Equal(expected, Decode(LittleEndian, bigEndian: false));
        Assert.Equal(expected, Decode(BigEndian, bigEndian: true));
    }

    [Fact]
    public void BigEndianReadAsLittleMovesTheTintIntoAlpha()
    {
        var wrong = Decode(BigEndian, bigEndian: false);

        Assert.Equal(0xC0, wrong.A);
        Assert.Equal(0xFF, wrong.B);
    }
}
