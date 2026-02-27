internal partial class AppMain
{
    internal class AMS_AME_NODE_NOISE : AMS_AME_NODE
    {
        public readonly NNS_VECTOR4D axis = GlobalPool<NNS_VECTOR4D>.Alloc();
        public float magnitude;
    }
}
