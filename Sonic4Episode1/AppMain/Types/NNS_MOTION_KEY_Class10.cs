internal partial class AppMain
{
    // NN key family SISplineA32_1: A32_1 plus an SI spline handle
    internal class NNS_MOTION_KEY_Class10
    {
        public readonly NNS_MOTION_SI_SPLINE_HANDLE Shandle = new NNS_MOTION_SI_SPLINE_HANDLE();
        public float Frame;
        public int Value;

        public NNS_MOTION_KEY_Class10()
        {
        }

        public NNS_MOTION_KEY_Class10(NNS_MOTION_KEY_Class10 motionKey)
        {
            this.Frame = motionKey.Frame;
            this.Value = motionKey.Value;
            this.Shandle.Assign(motionKey.Shandle);
        }

        public NNS_MOTION_KEY_Class10 Assign(NNS_MOTION_KEY_Class10 motionKey)
        {
            if (this != motionKey)
            {
                this.Frame = motionKey.Frame;
                this.Value = motionKey.Value;
                this.Shandle.Assign(motionKey.Shandle);
            }
            return this;
        }
    }
}
