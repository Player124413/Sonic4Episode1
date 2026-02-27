internal partial class AppMain
{
    internal class ACHIEVEMENT_INFO
    {
        public string id;
        public string name;
        public string description;
        public int cost;

        public ACHIEVEMENT_INFO(string id, string name, string desc, int cost)
        {
            this.id = id;
            this.name = name;
            this.description = desc;
            this.cost = cost;
        }
    }
}
