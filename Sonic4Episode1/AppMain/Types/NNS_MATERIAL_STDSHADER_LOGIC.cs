using System.IO;

internal partial class AppMain
{
    // nnfmaterialdxg20.h:370, unrelated to the GL NNS_MATERIAL_LOGIC beyond the name: these enums are D3D9's
    internal class NNS_MATERIAL_STDSHADER_LOGIC
    {
        public uint fFlag;
        public ushort SrcFactorRGB;
        public ushort DstFactorRGB;
        public ushort SrcFactorA;
        public ushort DstFactorA;
        public uint BlendColor;
        public ushort BlendOp;
        public ushort LogicOp;
        public ushort AlphaFunc;
        public ushort DepthFunc;
        public uint AlphaRef;

        public NNS_MATERIAL_STDSHADER_LOGIC Assign(NNS_MATERIAL_STDSHADER_LOGIC logic)
        {
            this.fFlag = logic.fFlag;
            this.SrcFactorRGB = logic.SrcFactorRGB;
            this.DstFactorRGB = logic.DstFactorRGB;
            this.SrcFactorA = logic.SrcFactorA;
            this.DstFactorA = logic.DstFactorA;
            this.BlendColor = logic.BlendColor;
            this.BlendOp = logic.BlendOp;
            this.LogicOp = logic.LogicOp;
            this.AlphaFunc = logic.AlphaFunc;
            this.DepthFunc = logic.DepthFunc;
            this.AlphaRef = logic.AlphaRef;
            return this;
        }

        public static NNS_MATERIAL_STDSHADER_LOGIC Read(BinaryReader reader)
        {
            return new NNS_MATERIAL_STDSHADER_LOGIC
            {
                fFlag = reader.ReadUInt32(),
                SrcFactorRGB = reader.ReadUInt16(),
                DstFactorRGB = reader.ReadUInt16(),
                SrcFactorA = reader.ReadUInt16(),
                DstFactorA = reader.ReadUInt16(),
                BlendColor = reader.ReadUInt32(),
                BlendOp = reader.ReadUInt16(),
                LogicOp = reader.ReadUInt16(),
                AlphaFunc = reader.ReadUInt16(),
                DepthFunc = reader.ReadUInt16(),
                AlphaRef = reader.ReadUInt32(),
            };
        }
    }
}
