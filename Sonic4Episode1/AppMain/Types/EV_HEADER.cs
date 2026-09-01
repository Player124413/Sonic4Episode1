internal partial class AppMain
{
    internal struct EV_HEADER
    {
        public ushort map_w;
        public ushort map_h;

        // one entry per 256x256 cell, empty where the file stored a zero offset. The on-disk
        // EV_OFFSET and EV_BLOCK are folded into this, since neither survives the read.
        public EV_INFO[][] cells;
    }
}
