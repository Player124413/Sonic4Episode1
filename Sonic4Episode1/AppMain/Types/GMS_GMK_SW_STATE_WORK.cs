internal partial class AppMain
{
    private class GMS_GMK_SW_STATE_WORK : IClearable
    {
        public bool sw;
        public int time;
        public bool gear;
        public int per;

        // PC widened this record from four fields to six so the Lost Labyrinth darkness could use
        // the same rows: a region id and a switch id are one namespace, and lighting a room is
        // literally turning on the switch of that number. sw doubles as "the region is lit" and
        // time as the light accumulator, threshold 16
        public int counter;
        public bool reset;

        public void Clear()
        {
            this.sw = this.gear = this.reset = false;
            this.time = this.per = this.counter = 0;
        }
    }
}
