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

        // members are addressed from here, not from data[0]: a nested archive shares its parent's
        // array rather than being copied out of it
        public int data_base;
        public AMS_AMB_HEADER parent;
        public bool is_real_amb = false;
        public bool big_endian = false;

        // the sibling archive supplying some of this one's members, and where in it each of our
        // indices lives, -1 where the base member stands. members are reached through the sibling
        // header rather than copied out of it, so they keep its byte order: an xbla .AMA spliced
        // into a little-endian base has nothing of its own that says which way round it is.
        public AMS_AMB_HEADER member_variant_amb;
        public int[] member_variant_index;
    }
}
