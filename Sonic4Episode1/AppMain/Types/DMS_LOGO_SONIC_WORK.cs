internal partial class AppMain
{
    internal class DMS_LOGO_SONIC_WORK
    {
        public AOS_ACTION[] act = new AOS_ACTION[2];
        public uint flag;
        public int timer;
        public _func_ func;
        public _skip_check_ skip_check;

        public delegate void _func_(DMS_LOGO_SONIC_WORK work);

        public delegate bool _skip_check_();
    }
}
