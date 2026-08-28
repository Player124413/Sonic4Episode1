internal partial class AppMain
{
    internal class GMS_DECO_PRIM_DRAW_WORK
    {
        public readonly GMS_DECO_PRIM_DRAW_STACK[] stack = New<GMS_DECO_PRIM_DRAW_STACK>(GMD_DECO_PRIM_DRAW_STACK_NUM);
        public AOS_TEXTURE tex;
        public int tex_id;
        public uint command;
        public ushort all_vtx_num;
        public ushort stack_num;
    }
}
