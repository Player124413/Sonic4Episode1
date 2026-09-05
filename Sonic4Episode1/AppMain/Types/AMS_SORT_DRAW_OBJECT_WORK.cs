internal partial class AppMain
{
    private class AMS_SORT_DRAW_OBJECT_WORK
    {
        public readonly AMS_COMMAND_HEADER cmd_header = new AMS_COMMAND_HEADER();
        public readonly AMS_PARAM_SORT_DRAW_OBJECT param = new AMS_PARAM_SORT_DRAW_OBJECT();
        public readonly AMS_DRAWSTATE state = new AMS_DRAWSTATE();
        public NNS_MATRIX[] plt_mtx;
        public uint[] nstat;
    }
}
