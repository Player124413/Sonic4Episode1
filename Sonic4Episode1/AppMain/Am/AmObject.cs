using System.Buffers.Binary;
using System.IO;

internal partial class AppMain
{
    public static void amObjectSetup(
      out NNS_OBJECT _object,
      out NNS_TEXFILELIST texfilelist,
      object _buf)
    {
        AmbChunk ambChunk = (AmbChunk)_buf;
        using (MemoryStream memoryStream = new MemoryStream(ambChunk.array, ambChunk.offset, ambChunk.array.Length - ambChunk.offset))
            amObjectSetup(out _object, out texfilelist, mppOpenNnChunk(memoryStream));
    }

    public static BinaryReader mppOpenNnChunk(Stream stream)
    {
        long start = stream.Position;
        var probe = new BinaryReader(stream);
        uint id = probe.ReadUInt32();
        stream.Position = start;

        if (id == NND_CHUNK_HEADER_GLES11 || id == NND_CHUNK_HEADER_DXG20)
            return probe;

        if (BinaryPrimitives.ReverseEndianness(id) == NND_CHUNK_HEADER_XE)
            return new EndianBinaryReader(stream, bigEndian: true);

        throw new InvalidDataException($"unrecognised NN file header 0x{id:X8}");
    }

    public static void amObjectSetup(
      out NNS_OBJECT _object,
      out NNS_TEXFILELIST texfilelist,
      BinaryReader reader)
    {
        _object = null;
        texfilelist = null;

        var bincnkFileheader = NNS_BINCNK_FILEHEADER.Read(reader);
        long ofsData;
        reader.BaseStream.Seek(ofsData = bincnkFileheader.OfsData, SeekOrigin.Begin);
        var bincnkDataheader = NNS_BINCNK_DATAHEADER.Read(reader);
        long data0Pos = ofsData;
        reader.BaseStream.Seek(bincnkFileheader.OfsNOF0, SeekOrigin.Begin);
        var nof0Header = NNS_BINCNK_NOF0HEADER.Read(reader);
        int nChunk = bincnkFileheader.nChunk;
        while (nChunk > 0)
        {
            switch (bincnkDataheader.Id)
            {
                case NND_CHUNK_OBJECT_GLES11:
                    reader.BaseStream.Seek(data0Pos + bincnkDataheader.OfsMainData, SeekOrigin.Begin);
                    _object = NNS_OBJECT.Read(reader, data0Pos, NNE_PLATFORM.GLES11);
                    break;
                case NND_CHUNK_OBJECT_DXG20:
                case NND_CHUNK_OBJECT_XE:
                    reader.BaseStream.Seek(data0Pos + bincnkDataheader.OfsMainData, SeekOrigin.Begin);
                    _object = NNS_OBJECT.Read(reader, data0Pos, NNE_PLATFORM.DXG20);
                    break;
                // NEND is the same four chars on GLES11 and DXG20, only XE is byte-swapped
                case NND_CHUNK_NEND_GLES11:
                case NND_CHUNK_NEND_XE:
                    return;
                case NND_CHUNK_TEXTURE_GLES11:
                case NND_CHUNK_TEXTURE_DXG20:
                case NND_CHUNK_TEXTURE_XE:
                    reader.BaseStream.Seek(data0Pos + bincnkDataheader.OfsMainData, SeekOrigin.Begin);
                    texfilelist = NNS_TEXFILELIST.Read(reader, data0Pos);
                    break;
            }
            --nChunk;
            reader.BaseStream.Seek(ofsData += 8 + bincnkDataheader.OfsNextId, SeekOrigin.Begin);
            bincnkDataheader = NNS_BINCNK_DATAHEADER.Read(reader);
        }
    }

    public static int amObjectLoad(
      out NNS_OBJECT _object,
      NNS_OBJECT obj_file,
      uint drawflag)
    {
        AMS_PARAM_VERTEX_BUFFER_OBJECT vertexBufferObject = new AMS_PARAM_VERTEX_BUFFER_OBJECT();
        _object = new NNS_OBJECT();
        vertexBufferObject.obj = _object;
        vertexBufferObject.srcobj = obj_file;
        vertexBufferObject.bindflag = 0U;
        vertexBufferObject.drawflag = drawflag;
        return amDrawRegistCommand(3, vertexBufferObject);
    }

    public static int amObjectLoad(
      out NNS_OBJECT _object,
      out NNS_TEXLIST texlist,
      out object texlistbuf,
      object buf,
      uint drawflag,
      string filepath,
      AMS_AMB_HEADER amb)
    {
        NNS_OBJECT _object1;
        NNS_TEXFILELIST texfilelist;
        amObjectSetup(out _object1, out texfilelist, buf);
        int nTex = texfilelist.nTex;
        texlistbuf = null;
        nnSetUpTexlist(out texlist, nTex, ref texlistbuf);
        int num = amObjectLoad(out _object, _object1, drawflag);
        if (filepath != null || amb != null)
            num = amTextureLoad(texlist, texfilelist, filepath, amb);
        return num;
    }

    private static int amObjectLoad(
      out NNS_OBJECT _object,
      NNS_TEXFILELIST txbfilelist,
      out NNS_TEXLIST texlist,
      out object texlistbuf,
      object buf,
      uint drawflag,
      string filepath,
      AMS_AMB_HEADER amb)
    {
        NNS_OBJECT _object1;
        amObjectSetup(out _object1, out NNS_TEXFILELIST _, buf);
        int nTex = txbfilelist.nTex;
        texlistbuf = null;
        nnSetUpTexlist(out texlist, nTex, ref texlistbuf);
        int num = amObjectLoad(out _object, _object1, drawflag);
        if (filepath != null || amb != null)
            num = amTextureLoad(texlist, txbfilelist, filepath, amb);
        return num;
    }

    private int amObjectLoadShader(ref NNS_OBJECT _object, uint drawflag)
    {
        mppAssertNotImpl();
        return 0;
    }

    private int amObjectLoadShader(ref NNS_OBJECT _object, int num, uint drawflag)
    {
        mppAssertNotImpl();
        return 0;
    }

    public static int amObjectRelease(NNS_OBJECT _object)
    {
        return amDrawRegistCommand(4, new AMS_PARAM_DELETE_VERTEX_OBJECT()
        {
            obj = _object
        });
    }

    private static int amObjectRelease(NNS_OBJECT _object, NNS_TEXLIST texlist)
    {
        amObjectRelease(_object);
        return amTextureRelease(texlist);
    }

}