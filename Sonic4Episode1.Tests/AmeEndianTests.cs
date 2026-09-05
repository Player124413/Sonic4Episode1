using System;
using System.Collections.Generic;

namespace Sonic4Episode1.Tests;

public class AmeEndianTests
{
    private const int AmeVersion = 0x00020000;
    private const short DragType = 0x0304;

    private sealed class Writer
    {
        private readonly List<byte> bytes = new List<byte>();
        private readonly bool bigEndian;

        public Writer(bool bigEndian) => this.bigEndian = bigEndian;

        public int Length => bytes.Count;

        public void Raw(params byte[] value) => bytes.AddRange(value);

        public void I32(int value) => Ordered(BitConverter.GetBytes(value));

        public void U32(uint value) => Ordered(BitConverter.GetBytes(value));

        public void I16(short value) => Ordered(BitConverter.GetBytes(value));

        public void F32(float value) => Ordered(BitConverter.GetBytes(value));

        public void Pad(int count) => bytes.AddRange(new byte[count]);

        public byte[] ToArray() => bytes.ToArray();

        private void Ordered(byte[] value)
        {
            if (bigEndian)
                Array.Reverse(value);
            bytes.AddRange(value);
        }
    }

    // 64 byte header then one drag node: 32 byte node header, a vec4 and a magnitude
    private static byte[] Build(bool bigEndian, uint magic = 0x454D4123, int version = AmeVersion)
    {
        var w = new Writer(bigEndian);

        w.Raw((byte)(magic & 0xFF), (byte)(magic >> 8 & 0xFF),
              (byte)(magic >> 16 & 0xFF), (byte)(magic >> 24 & 0xFF));
        w.I32(version);
        w.I32(1);
        w.U32(64);
        w.F32(1f);
        w.F32(2f);
        w.F32(3f);
        w.F32(4f);
        w.F32(5f);
        w.F32(6f);
        w.Pad(64 - w.Length);

        w.I16(7);
        w.I16(DragType);
        w.U32(0x01000000);
        w.Raw((byte)'D', (byte)'R', (byte)'A', (byte)'G', 0, 0, 0, 0, 0, 0, 0, 0);
        w.I32(0);
        w.I32(0);
        w.I32(0);

        w.F32(10f);
        w.F32(20f);
        w.F32(30f);
        w.F32(40f);
        w.F32(50f);

        return w.ToArray();
    }

    // a sprite node, for the colour: 32 byte node header, a vec4, a quat, 13 floats, the two
    // colours, blend, two texture shorts, 6 floats, then an empty tex anim
    private static byte[] BuildSprite(bool bigEndian)
    {
        var w = new Writer(bigEndian);

        w.Raw(0x23, 0x41, 0x4D, 0x45);
        w.I32(AmeVersion);
        w.I32(1);
        w.U32(64);
        w.Pad(64 - w.Length);

        w.I16(1);
        w.I16(0x0201);
        w.U32(0);
        w.Pad(12);
        w.I32(0);
        w.I32(0);
        w.I32(0);

        for (int i = 0; i < 8; ++i)
            w.F32(0f);
        for (int i = 0; i < 13; ++i)
            w.F32(0f);

        // r,g,b,a as four bytes, deliberately all different so a reversal cannot hide
        w.Raw(0x11, 0x22, 0x33, 0x44);
        w.Raw(0xAA, 0xBB, 0xCC, 0xDD);

        w.I32(3);
        w.I16(4);
        w.I16(5);
        for (int i = 0; i < 6; ++i)
            w.F32(0f);
        w.F32(0f);
        w.I32(0);

        return w.ToArray();
    }

    private static AppMain.AMS_AME_HEADER Read(byte[] bytes)
    {
        return AppMain.readAMEfile(new AppMain.AmbChunk(bytes, 0, bytes.Length, null));
    }

    [Fact]
    public void BothByteOrdersDecodeIdentically()
    {
        var little = Read(Build(bigEndian: false));
        var big = Read(Build(bigEndian: true));

        Assert.Equal(AmeVersion, little.file_version);
        Assert.Equal(AmeVersion, big.file_version);

        Assert.Equal(1, little.node_num);
        Assert.Equal(1, big.node_num);
        Assert.Equal(64U, little.node_ofst);
        Assert.Equal(64U, big.node_ofst);

        Assert.Equal(5f, little.bounding.radius);
        Assert.Equal(5f, big.bounding.radius);
        Assert.Equal(6f, little.bounding.radius2);
        Assert.Equal(6f, big.bounding.radius2);
    }

    [Fact]
    public void NodeBodyDecodesIdentically()
    {
        var little = (AppMain.AMS_AME_NODE_DRAG)Read(Build(bigEndian: false)).node[0];
        var big = (AppMain.AMS_AME_NODE_DRAG)Read(Build(bigEndian: true)).node[0];

        Assert.Equal(7, little.id);
        Assert.Equal(7, big.id);
        Assert.Equal(DragType, little.type);
        Assert.Equal(DragType, big.type);
        Assert.Equal(0x01000000U, little.flag);
        Assert.Equal(0x01000000U, big.flag);

        Assert.Equal(10f, big.position.x);
        Assert.Equal(20f, big.position.y);
        Assert.Equal(30f, big.position.z);
        Assert.Equal(40f, big.position.w);
        Assert.Equal(50f, big.magnitude);

        Assert.Equal(little.position.x, big.position.x);
        Assert.Equal(little.magnitude, big.magnitude);
    }

    [Fact]
    public void RejectsAnUnknownMagic()
    {
        Assert.Throws<System.IO.InvalidDataException>(
            () => Read(Build(bigEndian: false, magic: 0x424D4123)));
    }

    [Fact]
    public void ColourIsAByteQuadAndDoesNotReverse()
    {
        var little = (AppMain.AMS_AME_NODE_SPRITE)Read(BuildSprite(bigEndian: false)).node[0];
        var big = (AppMain.AMS_AME_NODE_SPRITE)Read(BuildSprite(bigEndian: true)).node[0];

        foreach (var sprite in new[] { little, big })
        {
            Assert.Equal(0x11, sprite.color_start.r);
            Assert.Equal(0x22, sprite.color_start.g);
            Assert.Equal(0x33, sprite.color_start.b);
            Assert.Equal(0x44, sprite.color_start.a);

            Assert.Equal(0xAA, sprite.color_end.r);
            Assert.Equal(0xBB, sprite.color_end.g);
            Assert.Equal(0xCC, sprite.color_end.b);
            Assert.Equal(0xDD, sprite.color_end.a);
        }

        // the fields either side of the colours are genuine integers and do reverse
        Assert.Equal(3, big.blend);
        Assert.Equal(4, big.texture_slot);
        Assert.Equal(5, big.texture_id);
        Assert.Equal(little.blend, big.blend);
        Assert.Equal(little.texture_slot, big.texture_slot);
    }

    // the wrong-order value, in case a future change reaches for "version == 0x200 means big"
    [Fact]
    public void RejectsAVersionThatIsNeitherOrder()
    {
        Assert.Throws<System.IO.InvalidDataException>(
            () => Read(Build(bigEndian: false, version: 0x00030000)));
    }
}
