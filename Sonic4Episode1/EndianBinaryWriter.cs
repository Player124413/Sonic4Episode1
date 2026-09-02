using System;
using System.Buffers.Binary;
using System.IO;

internal sealed class EndianBinaryWriter : BinaryWriter
{
    private readonly bool swap;

    public EndianBinaryWriter(Stream output, bool bigEndian)
        : base(output)
    {
        swap = bigEndian == BitConverter.IsLittleEndian;
    }

    public bool IsBigEndian => swap == BitConverter.IsLittleEndian;

    public override void Write(short value)
    {
        base.Write(swap ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    public override void Write(ushort value)
    {
        base.Write(swap ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    public override void Write(int value)
    {
        base.Write(swap ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    public override void Write(uint value)
    {
        base.Write(swap ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    public override void Write(long value)
    {
        base.Write(swap ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    public override void Write(ulong value)
    {
        base.Write(swap ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    public override void Write(float value)
    {
        if (!swap)
        {
            base.Write(value);
            return;
        }
#if NET40
        base.Write(BinaryPrimitives.ReverseEndianness(BitConverterEx.SingleToUInt32Bits(value)));
#else
        base.Write(BinaryPrimitives.ReverseEndianness(BitConverter.SingleToUInt32Bits(value)));
#endif
    }

    public override void Write(double value)
    {
        if (!swap)
        {
            base.Write(value);
            return;
        }
#if NET40
        base.Write(BinaryPrimitives.ReverseEndianness(BitConverterEx.DoubleToUInt64Bits(value)));
#else
        base.Write(BinaryPrimitives.ReverseEndianness(BitConverter.DoubleToUInt64Bits(value)));
#endif
    }
}
