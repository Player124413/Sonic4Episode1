using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#if HAS_PVR
using CSharp_PVR;
using Pvrtc;
#endif

internal partial class AppMain
{
    public static int amTextureLoad(
        NNS_TEXLIST texlist,
        NNS_TEXFILELIST texfilelist,
        string filepath)
    {
        mppAssertNotImpl();
        return amTextureLoad(texlist, texfilelist, filepath, null);
    }

    public static int amTextureLoad(
        NNS_TEXLIST texlist,
        NNS_TEXFILELIST texfilelist,
        string filepath,
        AMS_AMB_HEADER amb)
    {
        int num = 0;
        ArrayPointer<NNS_TEXINFO> arrayPointer1 = new ArrayPointer<NNS_TEXINFO>(texlist.pTexInfoList);
        ArrayPointer<NNS_TEXFILE> arrayPointer2 = new ArrayPointer<NNS_TEXFILE>(texfilelist.pTexFileList);
        int nTex = texfilelist.nTex;
        while (nTex > 0)
        {
            if (amb == null)
            {
                num = amTextureLoad(arrayPointer1, arrayPointer2, filepath, null, 0);
            }
            else
            {
                Texture2D texbuf;
                string fName = (~arrayPointer2).Filename;

                var index = -1;
                var ext = "";
                var fileNames = new string[] {
                    fName,
                    Path.ChangeExtension(fName, ".png"),
                    Path.ChangeExtension(fName, ".dds"), // we leave these formats in even if they're not supported so we can at least
                    Path.ChangeExtension(fName, ".pvr"), // throw an error if the game tries to load them, instead of just silently failing
                    Path.ChangeExtension(fName, ".ast")
                };

                for (int i = 0; i < amb.files.Length; i++)
                {
                    for (int j = 0; j < fileNames.Length; j++)
                    {
                        if (string.Compare(mppAmbMemberName(amb.files[i]), fileNames[j],
                            StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            ext = Path.GetExtension(fileNames[j]);
                            index = i;
                            break;
                        }
                    }

                    if (index != -1)
                        break;
                }

                if (index == -1)
                {
                    System.Diagnostics.Debug.WriteLine($"Didn't find {fName}");
                    texbuf = new Texture2D(m_game.GraphicsDevice, 1, 1);
                    texbuf.SetData([Color.Magenta]);
                }
                else
                {
                    if (amb.buf[index] != null)
                    {
                        texbuf = (Texture2D)amb.buf[index];
                    }
                    else
                    {
                        var chunk = amBindChunk(amb, index);
                        texbuf = amTextureDecode(chunk.array, chunk.offset, chunk.length, ext);
                        amb.buf[index] = texbuf;
                    }
                }

                num = amTextureLoad(arrayPointer1, arrayPointer2, filepath, texbuf, 0);
            }

            --nTex;
            ++arrayPointer1;
            ++arrayPointer2;
        }

        return num;
    }

    public static int amTextureLoad(
        NNS_TEXINFO texinfo,
        NNS_TEXFILE texfile,
        string filepath,
        Texture2D texbuf,
        int size)
    {
        AMS_PARAM_LOAD_TEXTURE paramLoadTexture = new AMS_PARAM_LOAD_TEXTURE();
        paramLoadTexture.pTexInfo = texinfo;
        paramLoadTexture.tex = texbuf;
        paramLoadTexture.size = (uint)size;
        _amTextureSetupLoadParam(ref paramLoadTexture, ref texfile, texbuf);
        return amDrawRegistCommand(1, paramLoadTexture);
    }

    private const uint MPP_DDS_MAGIC = 0x20534444;
    private const uint MPP_DDSD_PITCH = 0x8;
    private const uint MPP_DDSD_LINEARSIZE = 0x80000;

    private static void mppNormaliseDdsFlags(byte[] data, int offset)
    {
        if (offset + 12 > data.Length || BitConverter.ToUInt32(data, offset) != MPP_DDS_MAGIC)
            return;

        uint flags = BitConverter.ToUInt32(data, offset + 8);
        if ((flags & (MPP_DDSD_PITCH | MPP_DDSD_LINEARSIZE)) != (MPP_DDSD_PITCH | MPP_DDSD_LINEARSIZE))
            return;

        BitConverter.GetBytes(flags & ~MPP_DDSD_PITCH).CopyTo(data, offset + 8);
    }

#if MONOGAME
    // FNA has Texture2D.ASTCFromStreamEXT for this; MonoGame carries the surface formats but no
    // loader, so the header walk is here instead. Keep it in step with FNA's Texture.ParseASTC
    private const uint MPP_ASTC_MAGIC = 0x5CA1AB13;
    private const int MPP_ASTC_HEADER_SIZE = 16;

    private static Texture2D mppAstcDecode(byte[] data, int offset, int length)
    {
        if (length < MPP_ASTC_HEADER_SIZE || BitConverter.ToUInt32(data, offset) != MPP_ASTC_MAGIC)
            throw new InvalidDataException("Not an ASTC!");

        int blockX = data[offset + 4];
        int blockY = data[offset + 5];
        int blockZ = data[offset + 6];

        SurfaceFormat format;
        if (blockX == 4 && blockY == 4 && blockZ == 1)
            format = SurfaceFormat.Astc4X4Rgba;
        else if (blockX == 6 && blockY == 6 && blockZ == 1)
            format = SurfaceFormat.Astc6X6Rgba;
        else
            throw new InvalidDataException(
                $"Unsupported ASTC block footprint: {blockX}x{blockY}x{blockZ}");

        int width = mppAstcRead24(data, offset + 7);
        int height = mppAstcRead24(data, offset + 10);

        // one level, so whatever is left after the header is the whole surface
        var texture = new Texture2D(m_game.GraphicsDevice, width, height, false, format);
        texture.SetData(0, null, data, offset + MPP_ASTC_HEADER_SIZE, length - MPP_ASTC_HEADER_SIZE);
        return texture;
    }

    private static int mppAstcRead24(byte[] data, int offset)
    {
        return data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16);
    }
#endif

    public static Texture2D amTextureDecode(byte[] data, int offset, int length, string ext)
    {
        if (ext.Equals(".dds", StringComparison.InvariantCultureIgnoreCase))
            mppNormaliseDdsFlags(data, offset);

        using var memoryStream = new MemoryStream(data, offset, length);
        if (ext.Equals(".dds", StringComparison.InvariantCultureIgnoreCase))
        {
#if FNA
            return Texture2D.DDSFromStreamEXT(m_game.GraphicsDevice, memoryStream);
#elif THIRD_PARTY_DDS
            return (Texture2D)DdsKtxXna.DdsKtxLoader.FromStream(m_game.GraphicsDevice, memoryStream);
#else
            throw new InvalidOperationException("DDS textures are not supported on this platform.");
#endif
        }

        if (ext.Equals(".ast", StringComparison.InvariantCultureIgnoreCase))
        {
#if FNA
            return Texture2D.ASTCFromStreamEXT(m_game.GraphicsDevice, memoryStream);
#elif MONOGAME
            return mppAstcDecode(data, offset, length);
#else
            throw new InvalidOperationException("ASTC textures are not supported on this platform.");
#endif
        }

        if (ext.Equals(".pvr", StringComparison.InvariantCultureIgnoreCase))
        {
#if DEBUG && HAS_PVR
            return PVRTextureLoader.LoadTexture(m_game.GraphicsDevice, memoryStream);
#else
            throw new InvalidOperationException("PVR textures are not supported on this platform.");
#endif
        }

        return Texture2D.FromStream(m_game.GraphicsDevice, memoryStream);
    }

    // dst doubles as the C++ (void **)&tex out-pointer: the handler writes dst.texture
    public static int amTextureLoad(
        AMS_PARAM_LOAD_TEXTURE_IMAGE dst,
        AmbChunk image,
        string ext,
        int minfilter,
        int magfilter,
        int u_wrap,
        int v_wrap)
    {
        dst.image = image?.array;
        dst.offset = image != null ? image.offset : 0;
        dst.size = image != null ? image.length : 0;
        dst.ext = ext;
        dst.minfilter = (short)minfilter;
        dst.magfilter = (short)magfilter;
        dst.u_wrap = (short)u_wrap;
        dst.v_wrap = (short)v_wrap;
        return amDrawRegistCommand(AMD_REGIST_LOAD_TEXTURE_IMAGE, dst);
    }

    public static int amTextureRelease(NNS_TEXLIST texlist)
    {
        return amDrawRegistCommand(2, new AMS_PARAM_RELEASE_TEXTURE()
        {
            texlist = texlist
        });
    }

    public static int amTextureRelease(byte[] texture)
    {
        mppAssertNotImpl();
        return 0;
    }

    public static byte[] _amTextureConvertHeader(ref NNS_TEXINFO texinfo, ref byte[] texbuf)
    {
        mppAssertNotImpl();
        return texbuf;
    }

    public static void _amTextureSetupLoadParam(
        ref AMS_PARAM_LOAD_TEXTURE param,
        ref NNS_TEXFILE texfile,
        Texture2D texload)
    {
        if (((int)texfile.fType & 512) != 0)
        {
            param.minfilter = 0;
            param.magfilter = 0;
        }
        else
        {
            param.minfilter = texfile.MinFilter;
            param.magfilter = texfile.MagFilter;
        }

        param.globalIndex = ((int)texfile.fType & 1024) == 0 ? 0U : texfile.GlobalIndex;
        if (((int)texfile.fType & 2048) != 0)
            param.bank = texfile.Bank;
        else
            param.bank = 0U;
    }
}