internal partial class AppMain
{
    internal class NNS_VTXLIST_COMMON_ARRAY
    {
        public uint fType;
        public int Number;
        public uint Size;
        public object pList;

        public NNS_VTXLIST_COMMON_ARRAY Assign(NNS_VTXLIST_COMMON_ARRAY array)
        {
            this.fType = array.fType;
            this.Number = array.Number;
            this.Size = array.Size;
            this.pList = array.pList;
            return this;
        }
    }
}
