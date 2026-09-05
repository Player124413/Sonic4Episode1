using System.Runtime.InteropServices;

namespace System
{
    // BitConverter's bit-reinterpret methods only exist from .NET Core 3.0. no unsafe here, WP7
    // won't take it.
    internal static class BitConverterEx
    {
        [StructLayout(LayoutKind.Explicit, Size = 4)]
        private struct Bits32
        {
            [FieldOffset(0)]
            public float Single;
            [FieldOffset(0)]
            public int Int32;
            [FieldOffset(0)]
            public uint UInt32;
        }

        [StructLayout(LayoutKind.Explicit, Size = 8)]
        private struct Bits64
        {
            [FieldOffset(0)]
            public double Double;
            [FieldOffset(0)]
            public ulong UInt64;
        }

        public static uint SingleToUInt32Bits(float value)
            => new Bits32 { Single = value }.UInt32;

        public static float UInt32BitsToSingle(uint value)
            => new Bits32 { UInt32 = value }.Single;

        public static float Int32BitsToSingle(int value)
            => new Bits32 { Int32 = value }.Single;

        public static ulong DoubleToUInt64Bits(double value)
            => new Bits64 { Double = value }.UInt64;

        public static double UInt64BitsToDouble(ulong value)
            => new Bits64 { UInt64 = value }.Double;
    }
}

namespace System.Buffers.Binary
{
    internal static class BinaryPrimitivesEx
    {
        public static float ReadSingleBigEndian(byte[] source, int offset)
        {
            uint bits = (uint)(
                source[offset] << 24 |
                source[offset + 1] << 16 |
                source[offset + 2] << 8 |
                source[offset + 3]
            );
            return BitConverterEx.UInt32BitsToSingle(bits);
        }

        public static float ReadSingleLittleEndian(byte[] source, int offset)
        {
            uint bits = (uint)(
                source[offset] |
                source[offset + 1] << 8 |
                source[offset + 2] << 16 |
                source[offset + 3] << 24
            );
            return BitConverterEx.UInt32BitsToSingle(bits);
        }
    }
}
