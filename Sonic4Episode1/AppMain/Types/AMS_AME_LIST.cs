internal partial class AppMain
{
    internal class AMS_AME_LIST
    {
        public AMS_AME_LIST next;
        public AMS_AME_LIST prev;
        public bool in_use;

        public virtual void Clear()
        {
            this.next = null;
            this.prev = null;
        }
    }
}
