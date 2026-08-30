using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

internal partial class AppMain
{
    internal class NNS_PRIMTYPE_DX_STRIPLIST
    {
        public uint Format;
        public uint nIndex;
        public uint nPrimitive;
        public ushort[] pLengthList;
        public ushort[] pList;
        public IndexBuffer pD3DIndexBuffer;
        public uint fType;
        public uint BaseVertexIndex;

        public static NNS_PRIMTYPE_DX_STRIPLIST Read(BinaryReader reader, long data0Pos)
        {
            NNS_PRIMTYPE_DX_STRIPLIST strip = new NNS_PRIMTYPE_DX_STRIPLIST();
            strip.Format = reader.ReadUInt32();
            strip.nIndex = reader.ReadUInt32();
            strip.nPrimitive = reader.ReadUInt32();

            uint lengthListOffset = reader.ReadUInt32();
            uint listOffset = reader.ReadUInt32();

            // pD3DIndexBuffer, assigned at runtime by NN, always zero on disk
            reader.ReadUInt32();

            strip.fType = reader.ReadUInt32();
            strip.BaseVertexIndex = reader.ReadUInt32();

            if ((strip.Format & NND_PRIMMEMBER_DX_INDEXFMT_U32) != 0U)
                throw new NotSupportedException("32bit strip indices are not supported by NN");

            if (lengthListOffset != 0U)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(data0Pos + lengthListOffset, SeekOrigin.Begin);
                strip.pLengthList = new ushort[strip.nPrimitive];
                for (int index = 0; index < strip.nPrimitive; ++index)
                    strip.pLengthList[index] = reader.ReadUInt16();
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
            }

            if (listOffset != 0U)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(data0Pos + listOffset, SeekOrigin.Begin);
                strip.pList = new ushort[strip.nIndex];
                for (int index = 0; index < strip.nIndex; ++index)
                    strip.pList[index] = reader.ReadUInt16();
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
            }

            return strip;
        }

        public NNS_PRIMTYPE_DX_STRIPLIST Assign(NNS_PRIMTYPE_DX_STRIPLIST strip)
        {
            this.Format = strip.Format;
            this.nIndex = strip.nIndex;
            this.nPrimitive = strip.nPrimitive;
            this.pLengthList = strip.pLengthList;
            this.pList = strip.pList;
            this.fType = strip.fType;
            this.BaseVertexIndex = strip.BaseVertexIndex;
            return this;
        }
    }
}
