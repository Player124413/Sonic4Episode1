internal partial class AppMain
{
    internal class A2S_AMA_HEADER
    {
        public uint version;
        public uint node_num;
        public uint act_num;
        public int node_tbl_offset;
        public A2S_AMA_NODE[] node_tbl;
        public int act_tbl_offset;
        public A2S_AMA_ACT[] act_tbl;
        public int node_name_tbl_offset;
        public int act_name_tbl_offset;

        // null when the file carries no name table
        public string[] act_names;
        public string[] node_names;

        public int FindAct(string name)
        {
            return IndexOf(act_names, name);
        }

        public int FindNode(string name)
        {
            return IndexOf(node_names, name);
        }

        private static int IndexOf(string[] names, string name)
        {
            if (names == null)
                return -1;

            for (int index = 0; index < names.Length; ++index)
            {
                if (string.Equals(names[index], name, System.StringComparison.OrdinalIgnoreCase))
                    return index;
            }

            return -1;
        }
    }
}
