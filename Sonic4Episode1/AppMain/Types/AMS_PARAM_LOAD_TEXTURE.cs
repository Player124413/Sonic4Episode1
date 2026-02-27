using Microsoft.Xna.Framework.Graphics;

internal partial class AppMain
{
    internal class AMS_PARAM_LOAD_TEXTURE
    {
        public NNS_TEXINFO pTexInfo;
        public Texture2D tex;
        public ushort minfilter;
        public ushort magfilter;
        public uint globalIndex;
        public uint bank;
        public uint flag;
        public uint size;
        public byte[] buf_delete;
    }
}
