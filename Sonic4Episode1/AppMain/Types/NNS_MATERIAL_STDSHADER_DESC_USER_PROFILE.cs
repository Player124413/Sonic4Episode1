using System.IO;

internal partial class AppMain
{
    internal class NNS_MATERIAL_STDSHADER_DESC_USER_PROFILE : NNS_MATERIAL_STDSHADER_DESC
    {
        public uint UserProfile;

        // nnfmaterialdxg20.h:489, the base descriptor's 28 bytes and then one more word. The
        // material's fType carries NND_MATTYPE_USERPROFILE when this is the shape on disk.
        public static NNS_MATERIAL_STDSHADER_DESC_USER_PROFILE Read(BinaryReader reader, long data0Pos)
        {
            var desc = new NNS_MATERIAL_STDSHADER_DESC_USER_PROFILE();
            desc.Assign(NNS_MATERIAL_STDSHADER_DESC.Read(reader, data0Pos));
            desc.UserProfile = reader.ReadUInt32();
            return desc;
        }

        public NNS_MATERIAL_STDSHADER_DESC_USER_PROFILE()
        {
        }

        public NNS_MATERIAL_STDSHADER_DESC_USER_PROFILE(
          NNS_MATERIAL_STDSHADER_DESC_USER_PROFILE desc)
          : base(desc)
        {
            this.UserProfile = desc.UserProfile;
        }

        public NNS_MATERIAL_STDSHADER_DESC_USER_PROFILE Assign(
          NNS_MATERIAL_STDSHADER_DESC_USER_PROFILE desc)
        {
            this.Assign((NNS_MATERIAL_STDSHADER_DESC)desc);
            this.UserProfile = desc.UserProfile;
            return this;
        }
    }
}
