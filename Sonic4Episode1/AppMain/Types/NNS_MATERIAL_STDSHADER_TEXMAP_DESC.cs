using System.IO;

internal partial class AppMain
{
    internal class NNS_MATERIAL_STDSHADER_TEXMAP_DESC
    {
        public NNS_RGBA? pBorderColor = new NNS_RGBA?();
        public uint fType;
        public int iTexIdx;
        public int TexCoord;
        public float Blend;
        public NNS_TEXCOORD Offset;
        public NNS_TEXCOORD Scale;
        public int WrapS;
        public int WrapT;
        public NNS_TEXTURE_FILTERMODE pFilterMode;
        public NNS_TEXTURE_LOD_PARAM pLODParam;
        public object pTexInfo;
        public uint Reserved1;
        public uint Reserved0;

        // 64 bytes on disk, mapped field by field since the on-disk layout doesn't match the fields above
        public static NNS_MATERIAL_STDSHADER_TEXMAP_DESC Read(BinaryReader reader, long data0Pos)
        {
            NNS_MATERIAL_STDSHADER_TEXMAP_DESC desc = new NNS_MATERIAL_STDSHADER_TEXMAP_DESC();
            desc.fType = reader.ReadUInt32();
            desc.iTexIdx = reader.ReadInt32();
            desc.TexCoord = reader.ReadInt32();
            desc.Blend = reader.ReadSingle();
            desc.Offset = new NNS_TEXCOORD(reader.ReadSingle(), reader.ReadSingle());
            desc.Scale = new NNS_TEXCOORD(reader.ReadSingle(), reader.ReadSingle());
            bool xenon = reader is EndianBinaryReader endian && endian.IsBigEndian;

            desc.WrapS = reader.ReadUInt16();
            desc.WrapT = reader.ReadUInt16();

            if (xenon)
            {
                desc.WrapS = mppXenonTextureAddress((ushort)desc.WrapS);
                desc.WrapT = mppXenonTextureAddress((ushort)desc.WrapT);
            }

            uint borderColor = reader.ReadUInt32();
            desc.pBorderColor = new NNS_RGBA(
                ((borderColor >> 16) & 0xFF) / 255f,
                ((borderColor >> 8) & 0xFF) / 255f,
                (borderColor & 0xFF) / 255f,
                ((borderColor >> 24) & 0xFF) / 255f);

            // pFilterMode and pLODParam, not followed yet; nothing reads them on this path
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();

            desc.Reserved0 = reader.ReadUInt32();
            desc.Reserved1 = reader.ReadUInt32();
            reader.ReadUInt32();

            return desc;
        }

        public void Assign(NNS_MATERIAL_STDSHADER_TEXMAP_DESC pPtr)
        {
            mppAssertNotImpl();
        }

        public NNS_MATERIAL_STDSHADER_TEXMAP_DESC()
        {
        }

        public NNS_MATERIAL_STDSHADER_TEXMAP_DESC(NNS_MATERIAL_STDSHADER_TEXMAP_DESC desc)
        {
            this.fType = desc.fType;
            this.iTexIdx = desc.iTexIdx;
            this.TexCoord = desc.TexCoord;
            this.Blend = desc.Blend;
            this.Offset = desc.Offset;
            this.Scale = desc.Scale;
            this.WrapS = desc.WrapS;
            this.WrapT = desc.WrapT;
            this.pBorderColor = desc.pBorderColor;
            this.pFilterMode = desc.pFilterMode;
            this.pLODParam = desc.pLODParam;
            this.pTexInfo = desc.pTexInfo;
            this.Reserved1 = desc.Reserved1;
            this.Reserved0 = desc.Reserved0;
        }
    }
}
