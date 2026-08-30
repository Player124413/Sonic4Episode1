using System.IO;

internal partial class AppMain
{
    internal class NNS_BINCNK_NOF0HEADER
    {
        public uint Id;
        public int OfsNextId;
        public int nData;
        public int Pad;

        public static NNS_BINCNK_NOF0HEADER Read(BinaryReader reader)
        {
            return new NNS_BINCNK_NOF0HEADER()
            {
                Id = reader.ReadUInt32(),
                OfsNextId = readChunkOfsNextId(reader),
                nData = reader.ReadInt32(),
                Pad = reader.ReadInt32()
            };
        }
    }
}
