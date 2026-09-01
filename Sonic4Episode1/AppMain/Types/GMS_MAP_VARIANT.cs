internal partial class AppMain
{
    internal sealed class GMS_MAP_VARIANT
    {
        public bool blocks_as_model;

        // AMD_REGISTLIST_NUM, which the registration gate takes 64 off
        public int registlist_num;

        // what every archive this stage loads gets pinned to. the global am_fs_variant cannot be
        // trusted here: it stays "console" even when the stage has fallen back to mobile
        public string fs_variant;
    }
}
