internal partial class AppMain
{
    internal class AMS_TASK
    {
        public readonly AMS_TCB tcb_head = new AMS_TCB();
        public readonly AMS_TCB tcb_tail = new AMS_TCB();
        public int tcb_max;
        public int tcb_work_size;
    }
}
