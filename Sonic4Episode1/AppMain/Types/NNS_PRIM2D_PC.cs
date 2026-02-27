internal partial class AppMain
{
    internal class NNS_PRIM2D_PC : NNS_PRIM2D_P
    {
        public uint Col;

        public override void Clear()
        {
            this.Col = 0U;
            base.Clear();
        }
    }
}
