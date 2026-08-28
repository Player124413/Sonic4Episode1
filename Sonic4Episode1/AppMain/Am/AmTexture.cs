using System;
using System.IO;
using CSharp_PVR;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pvrtc;

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
                    Path.ChangeExtension(fName, ".pvr")  // throw an error if the game tries to load them, instead of just silently failing 
                };

                for (int i = 0; i < amb.files.Length; i++)
                {
                    for (int j = 0; j < fileNames.Length; j++)
                    {
                        if (string.Compare(Path.GetFileName(amb.files[i]), fileNames[j],
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
                        texbuf = amTextureDecode(amb.data, amb.offsets[index],
                            amb.data.Length - amb.offsets[index], ext);
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

    public static Texture2D amTextureDecode(byte[] data, int offset, int length, string ext)
    {
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

        if (ext.Equals(".pvr", StringComparison.InvariantCultureIgnoreCase))
        {
#if DEBUG
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