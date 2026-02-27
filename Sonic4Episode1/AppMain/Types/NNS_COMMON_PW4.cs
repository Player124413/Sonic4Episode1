internal partial class AppMain
{
    internal class NNS_COMMON_PW4
    {
        public readonly NNS_VECTOR Pos = GlobalPool<NNS_VECTOR>.Alloc();
        public readonly NNS_COMMON_WEIGHT[] Wgt = New<NNS_COMMON_WEIGHT>(4);
    }
}
