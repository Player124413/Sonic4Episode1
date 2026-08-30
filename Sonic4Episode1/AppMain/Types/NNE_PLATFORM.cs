internal partial class AppMain
{
    // DXG20 covers both Win32 and Xbox 360, which differ in byte order, not in layout
    internal enum NNE_PLATFORM
    {
        GLES11,
        DXG20,
    }
}
