using Microsoft.Xna.Framework.Graphics;

internal partial class AppMain
{
    // A class, not a struct: the C++ passes (void **)&dst so the handler can write the created
    // texture back (gmWaterSurface.cpp:1964). Boxing a struct into AMS_REGISTLIST.param loses
    // that, so the param object itself is the destination holder.
    internal class AMS_PARAM_LOAD_TEXTURE_IMAGE
    {
        public Texture2D texture;
        public byte[] image;
        public int offset;
        public int size;
        public string ext;
        public short minfilter;
        public short magfilter;
        public short u_wrap;
        public short v_wrap;
    }
}
