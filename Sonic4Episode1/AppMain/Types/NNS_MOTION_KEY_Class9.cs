internal partial class AppMain
{
    // NN key family BezierA32_1: A32_1 plus a Bezier handle
    internal class NNS_MOTION_KEY_Class9
    {
        public readonly NNS_MOTION_BEZIER_HANDLE Bhandle = new NNS_MOTION_BEZIER_HANDLE();
        public float Frame;
        public int Value;

        public NNS_MOTION_KEY_Class9()
        {
        }

        public NNS_MOTION_KEY_Class9(NNS_MOTION_KEY_Class9 motionKey)
        {
            this.Frame = motionKey.Frame;
            this.Value = motionKey.Value;
            this.Bhandle.Assign(motionKey.Bhandle);
        }

        public NNS_MOTION_KEY_Class9 Assign(NNS_MOTION_KEY_Class9 motionKey)
        {
            if (this != motionKey)
            {
                this.Frame = motionKey.Frame;
                this.Value = motionKey.Value;
                this.Bhandle.Assign(motionKey.Bhandle);
            }
            return this;
        }
    }
}
