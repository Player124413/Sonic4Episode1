internal partial class AppMain
{
    internal class DF_BLOCK
    {
        public readonly DF_CELL[,] df = new DF_CELL[8, 8];

        internal struct DF_CELL
        {
            public byte[] Data;
            public int Offset;
        }
    }
}
