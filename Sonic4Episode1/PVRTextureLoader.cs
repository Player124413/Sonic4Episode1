#if HAS_PVR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Pvrtc;
using CSharp_PVR;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Buffers.Binary;

/// <summary>
/// A very nieve texture loader for PVR textures used in iOS versions of Sonic 4: Episode 1
/// It works, but it's slow, inefficient and bad
/// </summary>
internal class PVRTextureLoader
{
    public static Texture2D LoadTexture(GraphicsDevice graphicsDevice, Stream stream)
    {
        // disable in release because performance is bad, we shouldn't be loading these
#if !DEBUG || !FNA
        throw new InvalidOperationException("Attempt to load PVR texture in Release build");
#else
        var container = new PVRContainerV2(stream);
        var header = container.GetHeader();
        var data = container.GetTextureData();
        var width = (int)header.GetWidth();
        var height = (int)header.GetHeight();

        var pixelFormat = header.GetPixelFormat();

        Debug.WriteLine("{0}", header.GetPixelFormat());

        var format = pixelFormat switch
        {
            PixelFormatV2.ARGB4444 => SurfaceFormat.Bgra4444,
            PixelFormatV2.ARGB4444_2 => SurfaceFormat.Bgra4444,
            PixelFormatV2.AI88 => SurfaceFormat.Color,
            PixelFormatV2.I8 => SurfaceFormat.Alpha8,
            PixelFormatV2.I8_2 => SurfaceFormat.Alpha8,
            PixelFormatV2.RGB565 => SurfaceFormat.Bgr565,
            PixelFormatV2.RGB565_2 => SurfaceFormat.Bgr565,
            PixelFormatV2.ARGB1555 => SurfaceFormat.Bgra5551,
            PixelFormatV2.ARGB1555_2 => SurfaceFormat.Bgra5551,
            PixelFormatV2.PVRTC4_2 => SurfaceFormat.ColorBgraEXT,
            PixelFormatV2.PVRTC2_2 => SurfaceFormat.ColorBgraEXT,
            _ => SurfaceFormat.Color
        };

        if (pixelFormat == PixelFormatV2.PVRTC4_2)
        {
            var decoder = new PvrtcDecoder();
            data = decoder.DecompressPVRTC(data, width, height, false);
        }

        if (pixelFormat == PixelFormatV2.PVRTC2_2)
        {
            var decoder = new PvrtcDecoder();
            data = decoder.DecompressPVRTC(data, width, height, true);
        }

        if (pixelFormat == PixelFormatV2.ARGB1555 || pixelFormat == PixelFormatV2.ARGB1555)
        {
            void UNPACK_RGBA5551(ushort pixel, out byte r, out byte g, out byte b, out byte a)
            {
                r = (byte)((pixel & 0xF800) >> 8);
                g = (byte)((pixel & 0x07C0) >> 3);
                b = (byte)((pixel & 0x003E) << 2);
                a = (byte)((pixel & 0x0001) == 1 ? 0xFF : 0x00);
            }

            // Pack BGRA5551 pixel format
            ushort PACK_BGRA5551(byte r, byte g, byte b, byte a)
            {
                ushort pixel = (ushort)(((b >> 2) & 0x003E) | ((g << 3) & 0x07C0) | ((r << 8) & 0xF800));
                if (a > 0x7F) pixel |= 0x0001; // Set alpha bit if alpha is > 0x7F
                return pixel;
            }

            unsafe
            {
                fixed (byte* dataPtr = data)
                {
                    var span = new Span<ushort>(dataPtr, data.Length / 2);
                    for (int x = 0; x < span.Length; x++)
                    {
                        byte a, r, g, b;

                        UNPACK_RGBA5551(span[x], out r, out g, out b, out a);
                        span[x] = PACK_BGRA5551(r, g, b, a);
                    }
                }
            }
        }

        if (pixelFormat == PixelFormatV2.ARGB4444 || pixelFormat == PixelFormatV2.ARGB4444_2)
        {
            // TODO: i am SO sorry for your CPU
            for (int i = 0; i < data.Length; i += 2)
            {
                byte firstByte = data[i];
                byte secondByte = data[i + 1];

                byte a = (byte)(firstByte & 0x0F);
                byte g = (byte)((firstByte & 0xF0) >> 4);
                byte r = (byte)(secondByte & 0x0F);
                byte b = (byte)((secondByte & 0xF0) >> 4);

                data[i] = (byte)((r << 4) | (g & 0x0F));
                data[i + 1] = (byte)((a << 4) | (b & 0x0F));
            }
        }

        if (pixelFormat == PixelFormatV2.AI88 || pixelFormat == PixelFormatV2.AI88_2)
        {
            var newData = new byte[data.Length * 2];
            for (int i = 0; i < data.Length; i += 2)
            {
                byte firstByte = data[i];
                byte secondByte = data[i + 1];

                newData[i * 2] = firstByte;
                newData[i * 2 + 1] = firstByte;
                newData[i * 2 + 2] = firstByte;
                newData[i * 2 + 3] = secondByte;
            }

            data = newData;
        }

        var texbuf = new Texture2D(graphicsDevice, width, height, false, format);
        texbuf.SetData(data);

        return texbuf;
#endif
    }
}
#endif