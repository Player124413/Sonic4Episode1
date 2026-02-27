// Decompiled with JetBrains decompiler
// Type: mpp.MppBitConverter
// Assembly: Sonic4 ep I, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 093CE2FC-33E2-4332-B0FE-1EA1E44D3AE7
// Assembly location: C:\Users\wamwo\Documents\GitHub\Sonic4Ep1-WP7-Decompilation\XAP\Sonic4 ep I.dll

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace mpp;

internal class MppBitConverter
{
#if !WINDOWS_PHONE_7
    [StructLayout(LayoutKind.Explicit, Size = sizeof(int))]
    private struct _BitConverter
    {
        public _BitConverter(int val)
            => Int32Val = val;
        public _BitConverter(uint val)
            => Uint32Val = val;
        public _BitConverter(float val)
            => FloatVal = val;
        public unsafe _BitConverter(byte[] data, int offset)
        {
            fixed (byte* ptr = data)
                Int32Val = *(int*)(ptr + offset);
        }

        [FieldOffset(0)]
        public int Int32Val;
        [FieldOffset(0)]
        public uint Uint32Val;
        [FieldOffset(0)]
        public float FloatVal;
    }
#endif

    private static readonly int[] Int32_buf = new int[1];
    private static readonly uint[] UInt32_buf = new uint[1];
    private static readonly float[] float_buf = new float[1];

    public static void GetBytes(float value, byte[] dst, int offset)
    {
        Debug.Assert(offset + 4 < dst.Length);
#if !WINDOWS_PHONE_7
        unsafe
        {
            fixed (byte* ptr = dst)
            {
                *(float*)(ptr + offset) = value;
            }
        }
#else
        float_buf[0] = value;
        Buffer.BlockCopy(float_buf, 0, dst, offset, 4);
#endif
    }

    public static void GetBytes(uint value, byte[] dst, int offset)
    {
        Debug.Assert(offset + 4 < dst.Length);
#if !WINDOWS_PHONE_7
        unsafe
        {
            fixed (byte* ptr = dst)
            {
                *(uint*)(ptr + offset) = value;
            }
        }
#else
        UInt32_buf[0] = value;
        Buffer.BlockCopy(UInt32_buf, 0, dst, offset, 4);
#endif
    }

    public static void GetBytes(int value, byte[] dst, int offset)
    {
        Debug.Assert(offset + 4 < dst.Length);
#if !WINDOWS_PHONE_7
        unsafe
        {
            fixed (byte* ptr = dst)
            {
                *(int*)(ptr + offset) = value;
            }
        }
#else
        Int32_buf[0] = value;
        Buffer.BlockCopy(Int32_buf, 0, dst, offset, 4);
#endif
    }

    public static float Int32ToSingle(int value)
    {
#if !WINDOWS_PHONE_7
        return new _BitConverter(value).FloatVal;
#else
        Int32_buf[0] = value;
        Buffer.BlockCopy(Int32_buf, 0, float_buf, 0, 4);
        return float_buf[0];
#endif
    }

    public static int SingleToInt32(float value)
    {
#if !WINDOWS_PHONE_7
        return new _BitConverter(value).Int32Val;
#else
        float_buf[0] = value;
        Buffer.BlockCopy(float_buf, 0, Int32_buf, 0, 4);
        return Int32_buf[0];
#endif
    }

    public static float UInt32ToSingle(uint value)
    {
#if !WINDOWS_PHONE_7
        return new _BitConverter(value).FloatVal;
#else
        UInt32_buf[0] = value;
        Buffer.BlockCopy(UInt32_buf, 0, float_buf, 0, 4);
        return float_buf[0];
#endif
    }

    public static uint SingleToUInt32(float value)
    {
#if !WINDOWS_PHONE_7
        return new _BitConverter(value).Uint32Val;
#else
        float_buf[0] = value;
        Buffer.BlockCopy(float_buf, 0, UInt32_buf, 0, 4);
        return UInt32_buf[0];
#endif
    }

    public static uint ToUInt32(byte[] src, int offset)
    {
#if !WINDOWS_PHONE_7
        return new _BitConverter(src, offset).Uint32Val;
#else
        Buffer.BlockCopy(src, offset, UInt32_buf, 0, 4);
        return UInt32_buf[0];
#endif
    }

    public static int ToInt32(byte[] src, int offset)
    {
#if !WINDOWS_PHONE_7
        return new _BitConverter(src, offset).Int32Val;
#else
        Buffer.BlockCopy(src, offset, Int32_buf, 0, 4);
        return Int32_buf[0];
#endif
    }

    public static float ToSingle(byte[] src, int offset)
    {
#if !WINDOWS_PHONE_7
        return new _BitConverter(src, offset).FloatVal;
#else
        Buffer.BlockCopy(src, offset, float_buf, 0, 4);
        return float_buf[0];
#endif
    }
}
