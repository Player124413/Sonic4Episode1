using System;
using System.Buffers.Binary;
using System.IO;

internal sealed class EndianBinaryReader : BinaryReader
{
    private readonly bool swap;

    public EndianBinaryReader(Stream input, bool bigEndian)
        : base(input)
    {
        swap = bigEndian == BitConverter.IsLittleEndian;
    }

    public bool IsBigEndian => swap == BitConverter.IsLittleEndian;

    public override short ReadInt16()
    {
        var value = base.ReadInt16();
        return swap ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    public override ushort ReadUInt16()
    {
        var value = base.ReadUInt16();
        return swap ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    public override int ReadInt32()
    {
        var value = base.ReadInt32();
        return swap ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    public override uint ReadUInt32()
    {
        var value = base.ReadUInt32();
        return swap ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    public override long ReadInt64()
    {
        var value = base.ReadInt64();
        return swap ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    public override ulong ReadUInt64()
    {
        var value = base.ReadUInt64();
        return swap ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    public override float ReadSingle()
    {
        if (!swap)
            return base.ReadSingle();
#if NET40
        return BitConverterEx.UInt32BitsToSingle(BinaryPrimitives.ReverseEndianness(base.ReadUInt32()));
#else
        return BitConverter.UInt32BitsToSingle(BinaryPrimitives.ReverseEndianness(base.ReadUInt32()));
#endif
    }

    public override double ReadDouble()
    {
        if (!swap)
            return base.ReadDouble();
#if NET40
        return BitConverterEx.UInt64BitsToDouble(BinaryPrimitives.ReverseEndianness(base.ReadUInt64()));
#else
        return BitConverter.UInt64BitsToDouble(BinaryPrimitives.ReverseEndianness(base.ReadUInt64()));
#endif
    }
}
