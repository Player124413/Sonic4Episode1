using System;
using System.IO;

internal partial class AppMain
{
    internal class NNS_MATERIALPTR
    {
        public uint fType;
        public object pMaterial;

        public NNS_MATERIALPTR()
        {
        }

        public NNS_MATERIALPTR(NNS_MATERIALPTR materialPtr)
        {
            this.fType = materialPtr.fType;
            this.pMaterial = materialPtr.pMaterial;
        }

        public NNS_MATERIALPTR Assign(NNS_MATERIALPTR materialPtr)
        {
            this.fType = materialPtr.fType;
            this.pMaterial = materialPtr.pMaterial;
            return this;
        }

        public static NNS_MATERIALPTR Read(
          BinaryReader reader,
          long data0Pos,
          NNE_PLATFORM platform,
          out bool transparentMaterial)
        {
            transparentMaterial = false;
            NNS_MATERIALPTR nnsMaterialptr = new NNS_MATERIALPTR();
            nnsMaterialptr.fType = reader.ReadUInt32();
            uint num = reader.ReadUInt32();
            if (num != 0U)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(data0Pos + num, SeekOrigin.Begin);
                nnsMaterialptr.pMaterial = ReadMaterial(reader, data0Pos, platform, out transparentMaterial);
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
            }
            return nnsMaterialptr;
        }

        private static object ReadMaterial(
          BinaryReader reader,
          long data0Pos,
          NNE_PLATFORM platform,
          out bool transparentMaterial)
        {
            transparentMaterial = false;
            switch (platform)
            {
                case NNE_PLATFORM.GLES11:
                    return NNS_MATERIAL_GLES11_DESC.Read(reader, data0Pos, out transparentMaterial);
                case NNE_PLATFORM.DXG20:
                    return NNS_MATERIAL_STDSHADER_DESC.Read(reader, data0Pos);
                default:
                    throw new NotSupportedException($"no material reader for {platform}");
            }
        }
    }
}
