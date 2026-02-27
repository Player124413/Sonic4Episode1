internal partial class AppMain
{
    internal class GSS_INIT_WORK
    {
        public int count;
        public AMS_FS fs;
        public ProcDelegate proc;

        public delegate void ProcDelegate(GSS_INIT_WORK work);
    }
}
