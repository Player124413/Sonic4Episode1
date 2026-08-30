using System.IO;

internal partial class AppMain
{
    // NN key family A32_1: float frame, one Angle32. Single axis rotation, the Angle32 counterpart of Class14.
    internal class NNS_MOTION_KEY_Class8
    {
        public float Frame;
        public int Value;

        public NNS_MOTION_KEY_Class8()
        {
        }

        public NNS_MOTION_KEY_Class8(NNS_MOTION_KEY_Class8 motionKey)
        {
            this.Frame = motionKey.Frame;
            this.Value = motionKey.Value;
        }

        public NNS_MOTION_KEY_Class8 Assign(NNS_MOTION_KEY_Class8 motionKey)
        {
            if (this != motionKey)
            {
                this.Frame = motionKey.Frame;
                this.Value = motionKey.Value;
            }
            return this;
        }

        public static NNS_MOTION_KEY_Class8 Read(BinaryReader reader)
        {
            return new NNS_MOTION_KEY_Class8()
            {
                Frame = reader.ReadSingle(),
                Value = reader.ReadInt32(),
            };
        }

        public static NNS_MOTION_KEY_Class8[] Read(BinaryReader reader, int count)
        {
            NNS_MOTION_KEY_Class8[] keys = new NNS_MOTION_KEY_Class8[count];
            for (int index = 0; index < count; ++index)
                keys[index] = Read(reader);
            return keys;
        }
    }
}
