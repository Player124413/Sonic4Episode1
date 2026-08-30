using System.IO;

internal partial class AppMain
{
    // NN key family F2: float frame, two floats. Texture offset when U/V animate together; either alone is Class1.
    internal class NNS_MOTION_KEY_Class4
    {
        public float Frame;
        public NNS_TEXCOORD Value;

        public NNS_MOTION_KEY_Class4()
        {
        }

        public NNS_MOTION_KEY_Class4(NNS_MOTION_KEY_Class4 motionKey)
        {
            this.Frame = motionKey.Frame;
            this.Value = motionKey.Value;
        }

        public NNS_MOTION_KEY_Class4 Assign(NNS_MOTION_KEY_Class4 motionKey)
        {
            if (this != motionKey)
            {
                this.Frame = motionKey.Frame;
                this.Value = motionKey.Value;
            }
            return this;
        }

        public static NNS_MOTION_KEY_Class4 Read(BinaryReader reader)
        {
            return new NNS_MOTION_KEY_Class4()
            {
                Frame = reader.ReadSingle(),
                Value = new NNS_TEXCOORD { u = reader.ReadSingle(), v = reader.ReadSingle() },
            };
        }

        public static NNS_MOTION_KEY_Class4[] Read(BinaryReader reader, int count)
        {
            NNS_MOTION_KEY_Class4[] keys = new NNS_MOTION_KEY_Class4[count];
            for (int index = 0; index < count; ++index)
                keys[index] = Read(reader);
            return keys;
        }
    }
}
