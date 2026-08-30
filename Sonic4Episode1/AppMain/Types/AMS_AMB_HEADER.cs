internal partial class AppMain
{
    internal class AMS_AMB_HEADER
    {
        public char[] file_id = new char[4]
        {
      '#',
      'A',
      'M',
      'B'
        };
        public string dir = "";
        public sbyte[] flag;
        public int file_num;
        public string[] files;
        // Sint32 in the #AMB entry, a single byte in the legacy layout
        public int[] types;
        public short[] user0;
        public short[] user1;
        public object[] buf;
        public int[] offsets;
        public int[] lengths;
        public byte[] data;
        public AMS_AMB_HEADER parent;
        public bool is_real_amb = false;

        // xbla archives are big-endian throughout, members included. nothing inside an .AMA says
        // which way round it is, so the container has to carry it
        public bool big_endian = false;
    }
}
