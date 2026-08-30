using System.IO;

internal partial class AppMain
{
    internal class NNS_MESHSET
    {
        public readonly NNS_VECTOR Center = GlobalPool<NNS_VECTOR>.Alloc();
        public float Radius;
        public int iNode;
        public int iMatrix;
        public int iMaterial;
        public int iVtxList;
        public int iPrimList;
        public int iShader;
        public uint Reserved2;
        public uint Reserved1;
        public uint Reserved0;

        public NNS_MESHSET()
        {
        }

        public NNS_MESHSET(NNS_MESHSET meshSet)
        {
            this.Center.Assign(meshSet.Center);
            this.Radius = meshSet.Radius;
            this.iNode = meshSet.iNode;
            this.iMatrix = meshSet.iMatrix;
            this.iMaterial = meshSet.iMaterial;
            this.iVtxList = meshSet.iVtxList;
            this.iPrimList = meshSet.iPrimList;
            this.iShader = meshSet.iShader;
            this.Reserved2 = meshSet.Reserved2;
            this.Reserved1 = meshSet.Reserved1;
            this.Reserved0 = meshSet.Reserved0;
        }

        public NNS_MESHSET Assign(NNS_MESHSET meshSet)
        {
            if (this != meshSet)
            {
                this.Center.Assign(meshSet.Center);
                this.Radius = meshSet.Radius;
                this.iNode = meshSet.iNode;
                this.iMatrix = meshSet.iMatrix;
                this.iMaterial = meshSet.iMaterial;
                this.iVtxList = meshSet.iVtxList;
                this.iPrimList = meshSet.iPrimList;
                this.iShader = meshSet.iShader;
                this.Reserved2 = meshSet.Reserved2;
                this.Reserved1 = meshSet.Reserved1;
                this.Reserved0 = meshSet.Reserved0;
            }
            return this;
        }

        public static NNS_MESHSET Read(BinaryReader reader, NNE_PLATFORM platform)
        {
            NNS_MESHSET meshSet = new NNS_MESHSET();
            meshSet.Center.x = reader.ReadSingle();
            meshSet.Center.y = reader.ReadSingle();
            meshSet.Center.z = reader.ReadSingle();
            meshSet.Radius = reader.ReadSingle();
            meshSet.iNode = reader.ReadInt32();
            meshSet.iMatrix = reader.ReadInt32();
            meshSet.iMaterial = reader.ReadInt32();
            meshSet.iVtxList = reader.ReadInt32();
            meshSet.iPrimList = reader.ReadInt32();

            if (platform == NNE_PLATFORM.DXG20)
            {
                meshSet.iShader = reader.ReadInt32();
                return meshSet;
            }

            meshSet.Reserved2 = reader.ReadUInt32();
            meshSet.Reserved1 = reader.ReadUInt32();
            meshSet.Reserved0 = reader.ReadUInt32();
            return meshSet;
        }
    }
}
