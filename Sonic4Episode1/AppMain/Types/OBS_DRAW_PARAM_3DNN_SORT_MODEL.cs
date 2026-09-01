internal partial class AppMain
{
    private class OBS_DRAW_PARAM_3DNN_SORT_MODEL
    {
        public readonly AMS_COMMAND_HEADER cmd_header = new AMS_COMMAND_HEADER();
        public readonly AMS_PARAM_SORT_DRAW_OBJECT param = new AMS_PARAM_SORT_DRAW_OBJECT();
        public readonly AMS_DRAWSTATE state = new AMS_DRAWSTATE();
        // the C++ allocates these at the tail of the struct: a deferred draw needs its own palette,
        // since the immediate path's scratch is long gone by the time the sort list replays
        public NNS_MATRIX[] plt_mtx;
        public uint[] nstat;
        public readonly AMS_PARAM_DRAW_OBJECT draw_object = new AMS_PARAM_DRAW_OBJECT();
        public MPP_VOID_OBJECT_DELEGATE user_func;
        public object user_param;
        public MPP_BOOL_NNSDRAWCALLBACKVAL_OBJECT_DELEGATE material_cb_func;
        public object material_cb_param;
        public uint use_light_flag;
    }
}
