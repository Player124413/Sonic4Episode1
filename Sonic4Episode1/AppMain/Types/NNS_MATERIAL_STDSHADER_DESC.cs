using System.IO;

internal partial class AppMain
{
    internal class NNS_MATERIAL_STDSHADER_DESC
    {
        public uint fFlag;
        public uint User;
        public NNS_MATERIAL_STDSHADER_COLOR pColor;
        public NNS_MATERIAL_STDSHADER_LOGIC pLogic;
        public uint fTexType;
        public int nTex;
        public NNS_MATERIAL_STDSHADER_TEXMAP_DESC[] pTexDesc;

        // 28 bytes on disk.
        public static NNS_MATERIAL_STDSHADER_DESC Read(BinaryReader reader, long data0Pos)
        {
            NNS_MATERIAL_STDSHADER_DESC desc = new NNS_MATERIAL_STDSHADER_DESC();
            desc.fFlag = reader.ReadUInt32();
            desc.User = reader.ReadUInt32();

            uint colorOffset = reader.ReadUInt32();
            uint logicOffset = reader.ReadUInt32();

            desc.fTexType = reader.ReadUInt32();
            desc.nTex = reader.ReadInt32();
            uint texDescOffset = reader.ReadUInt32();

            if (colorOffset != 0U)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(data0Pos + colorOffset, SeekOrigin.Begin);
                desc.pColor = NNS_MATERIAL_STDSHADER_COLOR.Read(reader);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
            }

            if (logicOffset != 0U)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(data0Pos + logicOffset, SeekOrigin.Begin);
                desc.pLogic = NNS_MATERIAL_STDSHADER_LOGIC.Read(reader);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
            }

            if (texDescOffset != 0U)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(data0Pos + texDescOffset, SeekOrigin.Begin);
                desc.pTexDesc = new NNS_MATERIAL_STDSHADER_TEXMAP_DESC[desc.nTex];
                for (int index = 0; index < desc.nTex; ++index)
                    desc.pTexDesc[index] = NNS_MATERIAL_STDSHADER_TEXMAP_DESC.Read(reader, data0Pos);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
            }

            return desc;
        }

        public NNS_MATERIAL_STDSHADER_DESC()
        {
        }

        public NNS_MATERIAL_STDSHADER_DESC(NNS_MATERIAL_STDSHADER_DESC desc)
        {
            this.fFlag = desc.fFlag;
            this.User = desc.User;
            this.pColor = desc.pColor;
            this.pLogic = desc.pLogic;
            this.fTexType = desc.fTexType;
            this.nTex = desc.nTex;
            this.pTexDesc = desc.pTexDesc;
        }

        public NNS_MATERIAL_STDSHADER_DESC Assign(
          NNS_MATERIAL_STDSHADER_DESC desc)
        {
            this.fFlag = desc.fFlag;
            this.User = desc.User;
            this.pColor = desc.pColor;
            this.pLogic = desc.pLogic;
            this.fTexType = desc.fTexType;
            this.nTex = desc.nTex;
            this.pTexDesc = desc.pTexDesc;
            return this;
        }
    }
}
