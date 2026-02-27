using System.Runtime.InteropServices;

internal partial class AppMain
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct AMS_RGBA8888
    {
        [FieldOffset(0)]
        public byte r;
        [FieldOffset(1)]
        public byte g;
        [FieldOffset(2)]
        public byte b;
        [FieldOffset(3)]
        public byte a;
        [FieldOffset(0)]
        public uint color;
    }
}
