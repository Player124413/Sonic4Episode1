using System;
using System.IO;

namespace Sonic4Episode1.Tests;

// NNS_SUBMOTION.Id is two halfwords and Id0 is the first of them in file order, so big-endian data
// carries them the other way round; nnt.h:321-324 switches the packing on NND_PLATFORM_GC for the
// same reason. Node motions use Id whole as a node index and must not be touched.
public class SubmotionIdEndianTests
{
    private const int IdOffset = 8;
    private const int SubmotionBytes = 40;

    private static byte[] Submotion(int id, bool bigEndian)
    {
        var bytes = new byte[SubmotionBytes];
        var span = bytes.AsSpan();

        // fType and fIPType are read first and their values do not matter here
        Write(span, IdOffset, (uint)id, bigEndian);
        return bytes;
    }

    private static void Write(Span<byte> span, int at, uint value, bool bigEndian)
    {
        if (bigEndian)
            System.Buffers.Binary.BinaryPrimitives.WriteUInt32BigEndian(span.Slice(at), value);
        else
            System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(at), value);
    }

    private static AppMain.NNS_SUBMOTION Read(int id, bool bigEndian, uint motionType)
    {
        var bytes = Submotion(id, bigEndian);
        using var stream = new MemoryStream(bytes);
        using BinaryReader reader = bigEndian
            ? new EndianBinaryReader(stream, bigEndian: true)
            : new BinaryReader(stream);

        return AppMain.NNS_SUBMOTION.Read(reader, motionType, 0);
    }

    // measured: the same sky material motion is Id 0x00010000 on mobile and 0x00000001 on xbla, and
    // both mean material 0, texmap 1
    [Fact]
    public void AMaterialSubmotionReadsTheSameIdsFromEitherByteOrder()
    {
        var mobile = Read(0x00010000, bigEndian: false, AppMain.NND_MOTIONTYPE_MATERIAL);
        var xbla = Read(0x00000001, bigEndian: true, AppMain.NND_MOTIONTYPE_MATERIAL);

        Assert.Equal(0, mobile.Id0);
        Assert.Equal(1, mobile.Id1);

        Assert.Equal(mobile.Id0, xbla.Id0);
        Assert.Equal(mobile.Id1, xbla.Id1);
    }

    // ZONE3_MAPFAR's Z3_FAR has 14 materials and animates the U offset of texmap 0 on six of them.
    // Read the little-endian way those become texmap indices 8..13 and overflow an 8 entry array in
    // nnInitMaterialMotionObject.
    [Fact]
    public void ZoneThreesLightShaftsAreMaterialIndicesNotTexmapIndices()
    {
        for (int material = 8; material <= 13; material++)
        {
            var submotion = Read(material << 16, bigEndian: true, AppMain.NND_MOTIONTYPE_MATERIAL);

            Assert.Equal(material, submotion.Id0);
            Assert.Equal(0, submotion.Id1);
        }
    }

    // node motions index a node with the whole word, so swapping the halves would break every
    // big-endian character and block animation
    [Fact]
    public void ANodeSubmotionKeepsItsIdWhole()
    {
        Assert.Equal(0x00080000, Read(0x00080000, bigEndian: true, AppMain.NND_MOTIONTYPE_NODE).Id);
        Assert.Equal(0x00080000, Read(0x00080000, bigEndian: false, AppMain.NND_MOTIONTYPE_NODE).Id);
    }
}
