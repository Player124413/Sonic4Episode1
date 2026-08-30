using System;
using System.IO;

internal partial class AppMain
{
    internal class NNS_PRIMLISTPTR
    {
        public uint fType;
        public object pPrimList;

        public NNS_PRIMLISTPTR()
        {
        }

        public NNS_PRIMLISTPTR(NNS_PRIMLISTPTR primListPtr)
        {
            this.fType = primListPtr.fType;
            this.pPrimList = primListPtr.pPrimList;
        }

        public NNS_PRIMLISTPTR Assign(NNS_PRIMLISTPTR primListPtr)
        {
            this.fType = primListPtr.fType;
            this.pPrimList = primListPtr.pPrimList;
            return this;
        }

        public static NNS_PRIMLISTPTR Read(BinaryReader reader, long data0Pos, NNE_PLATFORM platform)
        {
            NNS_PRIMLISTPTR nnsPrimlistptr = new NNS_PRIMLISTPTR();
            nnsPrimlistptr.fType = reader.ReadUInt32();
            uint num = reader.ReadUInt32();
            if (num != 0U)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(data0Pos + num, SeekOrigin.Begin);
                nnsPrimlistptr.pPrimList = ReadPrimList(reader, data0Pos, platform);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
            }
            return nnsPrimlistptr;
        }

        private static object ReadPrimList(BinaryReader reader, long data0Pos, NNE_PLATFORM platform)
        {
            switch (platform)
            {
                case NNE_PLATFORM.GLES11:
                    return NNS_PRIMLIST_GL_DESC.Read(reader, data0Pos);
                case NNE_PLATFORM.DXG20:
                    return NNS_PRIMTYPE_DX_STRIPLIST.Read(reader, data0Pos);
                default:
                    throw new NotSupportedException($"no primitive list reader for {platform}");
            }
        }
    }
}
