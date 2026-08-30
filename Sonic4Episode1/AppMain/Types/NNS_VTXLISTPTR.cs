using System;
using System.IO;

internal partial class AppMain
{
    internal class NNS_VTXLISTPTR
    {
        public uint fType;
        public object pVtxList;

        public NNS_VTXLISTPTR()
        {
        }

        public NNS_VTXLISTPTR(NNS_VTXLISTPTR vtxListPtr)
        {
            this.fType = vtxListPtr.fType;
            this.pVtxList = vtxListPtr.pVtxList;
        }

        public NNS_VTXLISTPTR Assign(NNS_VTXLISTPTR vtxListPtr)
        {
            this.fType = vtxListPtr.fType;
            this.pVtxList = vtxListPtr.pVtxList;
            return this;
        }

        public static NNS_VTXLISTPTR Read(BinaryReader reader, long data0Pos, NNE_PLATFORM platform)
        {
            NNS_VTXLISTPTR nnsVtxlistptr = new NNS_VTXLISTPTR();
            nnsVtxlistptr.fType = reader.ReadUInt32();
            uint num = reader.ReadUInt32();
            if (num != 0U)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(data0Pos + num, SeekOrigin.Begin);
                nnsVtxlistptr.pVtxList = ReadVtxList(reader, data0Pos, platform);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
            }
            return nnsVtxlistptr;
        }

        private static object ReadVtxList(BinaryReader reader, long data0Pos, NNE_PLATFORM platform)
        {
            switch (platform)
            {
                case NNE_PLATFORM.GLES11:
                    return NNS_VTXLIST_GL_DESC.Read(reader, data0Pos);
                case NNE_PLATFORM.DXG20:
                    return NNS_VTXLIST_DX_DESC.Read(reader, data0Pos);
                default:
                    throw new NotSupportedException($"no vertex list reader for {platform}");
            }
        }
    }
}
