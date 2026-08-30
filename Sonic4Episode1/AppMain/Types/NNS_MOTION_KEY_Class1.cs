using System.IO;

internal partial class AppMain
{
    // NN key family F1: float frame, one float. Single axis; all three together is Class5.
    internal struct NNS_MOTION_KEY_Class1
    {
        public float Frame;
        public float Value;

        public NNS_MOTION_KEY_Class1(NNS_MOTION_KEY_Class1 motionKey)
        {
            this.Frame = motionKey.Frame;
            this.Value = motionKey.Value;
        }

        public NNS_MOTION_KEY_Class1 Assign(NNS_MOTION_KEY_Class1 motionKey)
        {
            this.Frame = motionKey.Frame;
            this.Value = motionKey.Value;
            return this;
        }

        public static NNS_MOTION_KEY_Class1 Read(BinaryReader reader)
        {
            return new NNS_MOTION_KEY_Class1()
            {
                Frame = reader.ReadSingle(),
                Value = reader.ReadSingle()
            };
        }

        public static NNS_MOTION_KEY_Class1[] Read(BinaryReader reader, int count)
        {
            NNS_MOTION_KEY_Class1[] nnsMotionKeyClass1Array = new NNS_MOTION_KEY_Class1[count];
            for (int index = 0; index < count; ++index)
                nnsMotionKeyClass1Array[index] = Read(reader);
            return nnsMotionKeyClass1Array;
        }
    }
}
