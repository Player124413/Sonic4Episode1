using System;
using System.Buffers.Binary;
using System.IO;

namespace Sonic4Episode1.Tests;

public class EndianBinaryReaderTests
{
    private const short Int16Value = unchecked((short)0xF00D);
    private const ushort UInt16Value = 0xBEEF;
    private const int Int32Value = unchecked((int)0xDEADBEEF);
    private const uint UInt32Value = 0x12345678;
    private const long Int64Value = unchecked((long)0x0123456789ABCDEF);
    private const ulong UInt64Value = 0xFEDCBA9876543210;
    private const float SingleValue = -12345.678f;
    private const double DoubleValue = 9876.54321e12;

    private static byte[] BuildBigEndian()
    {
        var buffer = new byte[2 + 2 + 4 + 4 + 8 + 8 + 4 + 8];
        var span = buffer.AsSpan();

        BinaryPrimitives.WriteInt16BigEndian(span.Slice(0, 2), Int16Value);
        BinaryPrimitives.WriteUInt16BigEndian(span.Slice(2, 2), UInt16Value);
        BinaryPrimitives.WriteInt32BigEndian(span.Slice(4, 4), Int32Value);
        BinaryPrimitives.WriteUInt32BigEndian(span.Slice(8, 4), UInt32Value);
        BinaryPrimitives.WriteInt64BigEndian(span.Slice(12, 8), Int64Value);
        BinaryPrimitives.WriteUInt64BigEndian(span.Slice(20, 8), UInt64Value);
        BinaryPrimitives.WriteSingleBigEndian(span.Slice(28, 4), SingleValue);
        BinaryPrimitives.WriteDoubleBigEndian(span.Slice(32, 8), DoubleValue);

        return buffer;
    }

    private static byte[] BuildLittleEndian()
    {
        var buffer = new byte[2 + 2 + 4 + 4 + 8 + 8 + 4 + 8];
        var span = buffer.AsSpan();

        BinaryPrimitives.WriteInt16LittleEndian(span.Slice(0, 2), Int16Value);
        BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(2, 2), UInt16Value);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(4, 4), Int32Value);
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(8, 4), UInt32Value);
        BinaryPrimitives.WriteInt64LittleEndian(span.Slice(12, 8), Int64Value);
        BinaryPrimitives.WriteUInt64LittleEndian(span.Slice(20, 8), UInt64Value);
        BinaryPrimitives.WriteSingleLittleEndian(span.Slice(28, 4), SingleValue);
        BinaryPrimitives.WriteDoubleLittleEndian(span.Slice(32, 8), DoubleValue);

        return buffer;
    }

    private static void AssertReadsSampleValues(BinaryReader reader)
    {
        Assert.Equal(Int16Value, reader.ReadInt16());
        Assert.Equal(UInt16Value, reader.ReadUInt16());
        Assert.Equal(Int32Value, reader.ReadInt32());
        Assert.Equal(UInt32Value, reader.ReadUInt32());
        Assert.Equal(Int64Value, reader.ReadInt64());
        Assert.Equal(UInt64Value, reader.ReadUInt64());
        Assert.Equal(SingleValue, reader.ReadSingle());
        Assert.Equal(DoubleValue, reader.ReadDouble());
    }

    [Fact]
    public void ReadsBigEndianData()
    {
        using var stream = new MemoryStream(BuildBigEndian());
        using var reader = new EndianBinaryReader(stream, bigEndian: true);

        AssertReadsSampleValues(reader);
    }

    [Fact]
    public void PassesThroughLittleEndianData()
    {
        using var stream = new MemoryStream(BuildLittleEndian());
        using var reader = new EndianBinaryReader(stream, bigEndian: false);

        AssertReadsSampleValues(reader);
    }

    [Fact]
    public void MatchesPlainBinaryReaderWhenLittleEndian()
    {
        var data = BuildLittleEndian();

        using var expectedStream = new MemoryStream(data);
        using var expected = new BinaryReader(expectedStream);
        using var actualStream = new MemoryStream(data);
        using var actual = new EndianBinaryReader(actualStream, bigEndian: false);

        AssertReadsSampleValues(expected);
        AssertReadsSampleValues(actual);
    }

    [Fact]
    public void LeavesByteReadsUnswapped()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };

        using var stream = new MemoryStream(data);
        using var reader = new EndianBinaryReader(stream, bigEndian: true);

        Assert.Equal(1, reader.ReadByte());
        Assert.Equal(new byte[] { 2, 3, 4, 5 }, reader.ReadBytes(4));
    }

    [Fact]
    public void ReportsItsOwnEndianness()
    {
        using var stream = new MemoryStream(new byte[8]);
        using var big = new EndianBinaryReader(stream, bigEndian: true);

        Assert.True(big.IsBigEndian);

        using var littleStream = new MemoryStream(new byte[8]);
        using var little = new EndianBinaryReader(littleStream, bigEndian: false);

        Assert.False(little.IsBigEndian);
    }
}
