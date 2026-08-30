using System.IO;
using Microsoft.Xna.Framework.Graphics;

internal partial class AppMain
{
    internal class NNS_VTXLIST_DX_DESC
    {
        public uint Format;
        public uint VertexFVF;
        public uint Stride;
        public uint nVertex;
        public ByteBuffer pList;
        public int nBoneMtx;
        public uint[] pBoneMtxList;
        public VertexBuffer pD3DVertexBuffer;
        public VertexDeclaration pD3DVertexDecl;
        public uint Offset;
        public uint fType;

        // pList is raw vertex bytes; Xenon data stays big-endian until decode
        public bool mppBigEndian;

        public static NNS_VTXLIST_DX_DESC Read(BinaryReader reader, long data0Pos)
        {
            NNS_VTXLIST_DX_DESC desc = new NNS_VTXLIST_DX_DESC();
            desc.mppBigEndian = reader is EndianBinaryReader endian && endian.IsBigEndian;
            desc.Format = reader.ReadUInt32();
            desc.VertexFVF = reader.ReadUInt32();
            desc.Stride = reader.ReadUInt32();
            desc.nVertex = reader.ReadUInt32();

            uint listOffset = reader.ReadUInt32();
            desc.nBoneMtx = reader.ReadInt32();
            uint boneMtxListOffset = reader.ReadUInt32();

            // pD3DVertexBuffer and pD3DVertexDecl, assigned at runtime by NN, always zero on disk
            reader.ReadUInt32();
            reader.ReadUInt32();

            desc.Offset = reader.ReadUInt32();
            desc.fType = reader.ReadUInt32();
            reader.ReadUInt32();

            if (listOffset != 0U)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(data0Pos + listOffset, SeekOrigin.Begin);
                byte[] vertices = new byte[desc.Stride * desc.nVertex];
                reader.Read(vertices, 0, vertices.Length);
                desc.pList = ByteBuffer.Wrap(vertices);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
            }

            if (boneMtxListOffset != 0U)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(data0Pos + boneMtxListOffset, SeekOrigin.Begin);
                desc.pBoneMtxList = new uint[desc.nBoneMtx];
                for (int index = 0; index < desc.nBoneMtx; ++index)
                    desc.pBoneMtxList[index] = reader.ReadUInt32();
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
            }

            return desc;
        }

        public NNS_VTXLIST_DX_DESC Assign(NNS_VTXLIST_DX_DESC desc)
        {
            this.Format = desc.Format;
            this.VertexFVF = desc.VertexFVF;
            this.Stride = desc.Stride;
            this.nVertex = desc.nVertex;
            this.pList = desc.pList;
            this.nBoneMtx = desc.nBoneMtx;
            this.pBoneMtxList = desc.pBoneMtxList;
            this.Offset = desc.Offset;
            this.fType = desc.fType;
            return this;
        }
    }
}
