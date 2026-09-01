internal partial class AppMain
{
    private class AMS_PARAM_DRAW_OBJECT
    {
        public NNS_OBJECT _object;
        public NNS_MATRIX mtx;
        public NNS_TEXLIST texlist;
        public uint sub_obj_type;
        public uint flag;

        // NNF_DRAWOBJ is 64 bit and this port keeps it as two words; everything from bit 32 up,
        // which is where the MATCTRL and SHADOWMAP bits live, arrives here
        public uint flag_ex;
        public NNS_MATERIALCALLBACK_FUNC material_func;
        public float scaleZ;
        public readonly object holder;

        public AMS_PARAM_DRAW_OBJECT()
        {
        }

        public AMS_PARAM_DRAW_OBJECT(object _holder)
        {
            this.holder = _holder;
        }

        public static explicit operator OBS_DRAW_PARAM_3DNN_MODEL(
          AMS_PARAM_DRAW_OBJECT ob)
        {
            return (OBS_DRAW_PARAM_3DNN_MODEL)ob.holder;
        }
    }
}
