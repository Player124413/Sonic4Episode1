internal partial class AppMain
{
    // NN key family Quat_4: float frame, a quaternion. Not read by NNS_SUBMOTION yet, no fixture has any.
    internal class NNS_MOTION_KEY_Class7
    {
        public float Frame;
        public NNS_QUATERNION Value;

        public NNS_MOTION_KEY_Class7()
        {
        }

        public NNS_MOTION_KEY_Class7(NNS_MOTION_KEY_Class7 motionKey)
        {
            this.Frame = motionKey.Frame;
            this.Value = motionKey.Value;
        }

        public NNS_MOTION_KEY_Class7 Assign(NNS_MOTION_KEY_Class7 motionKey)
        {
            if (this != motionKey)
            {
                this.Frame = motionKey.Frame;
                this.Value = motionKey.Value;
            }
            return this;
        }
    }
}
