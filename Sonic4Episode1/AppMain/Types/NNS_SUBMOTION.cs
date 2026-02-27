using System;
using System.IO;

internal partial class AppMain
{
    internal class NNS_SUBMOTION
    {
        public uint fType;
        public uint fIPType;
        public int Id;
        public float StartFrame;
        public float EndFrame;
        public float StartKeyFrame;
        public float EndKeyFrame;
        public int nKeyFrame;
        public int KeySize;
        public object pKeyList;

        public short Id0
        {
            get => (short)(this.Id & ushort.MaxValue);
            set => this.Id = this.Id & -65536 | value & ushort.MaxValue;
        }

        public short Id1
        {
            get => (short)(this.Id >> 16 & ushort.MaxValue);
            set => this.Id = this.Id & ushort.MaxValue | value << 16;
        }

        public NNS_SUBMOTION()
        {
        }

        public NNS_SUBMOTION(NNS_SUBMOTION subMotion)
        {
            this.fType = subMotion.fType;
            this.fIPType = subMotion.fIPType;
            this.Id = subMotion.Id;
            this.StartFrame = subMotion.StartFrame;
            this.EndFrame = subMotion.EndFrame;
            this.StartKeyFrame = subMotion.StartKeyFrame;
            this.EndKeyFrame = subMotion.EndKeyFrame;
            this.nKeyFrame = subMotion.nKeyFrame;
            this.KeySize = subMotion.KeySize;
            this.pKeyList = subMotion.pKeyList;
        }

        public NNS_SUBMOTION Assign(NNS_SUBMOTION subMotion)
        {
            if (this != subMotion)
            {
                this.fType = subMotion.fType;
                this.fIPType = subMotion.fIPType;
                this.Id = subMotion.Id;
                this.StartFrame = subMotion.StartFrame;
                this.EndFrame = subMotion.EndFrame;
                this.StartKeyFrame = subMotion.StartKeyFrame;
                this.EndKeyFrame = subMotion.EndKeyFrame;
                this.nKeyFrame = subMotion.nKeyFrame;
                this.KeySize = subMotion.KeySize;
                this.pKeyList = subMotion.pKeyList;
            }
            return this;
        }

        public static NNS_SUBMOTION Read(
          BinaryReader reader,
          uint motionType,
          long data0Pos)
        {
            NNS_SUBMOTION submotion = new NNS_SUBMOTION();
            submotion.fType = reader.ReadUInt32();
            submotion.fIPType = reader.ReadUInt32();
            submotion.Id = reader.ReadInt32();
            submotion.StartFrame = reader.ReadSingle();
            submotion.EndFrame = reader.ReadSingle();
            submotion.StartKeyFrame = reader.ReadSingle();
            submotion.EndKeyFrame = reader.ReadSingle();
            submotion.nKeyFrame = reader.ReadInt32();
            submotion.KeySize = reader.ReadInt32();
            uint num = reader.ReadUInt32();
            if (num != 0U)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(data0Pos + num, SeekOrigin.Begin);
                switch (motionType)
                {
                    case NND_MOTIONTYPE_NODE:
                        ReadNodeMotionKeyFrames(reader, submotion);
                        break;
                    case NND_MOTIONTYPE_CAMERA:
                        ReadCameraMotionKeyFrames(reader, submotion);
                        break;
                    case NND_MOTIONTYPE_LIGHT:
                        ReadLightMotionKeyFrames(reader, submotion);
                        break;
                    case NND_MOTIONTYPE_MORPH:
                        ReadMorphMotionKeyFrames(reader, submotion);
                        break;
                    case NND_MOTIONTYPE_MATERIAL:
                        ReadMaterialMotionKeyFrames(reader, submotion);
                        break;
                }
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
            }
            return submotion;
        }

        private static void ReadNodeMotionKeyFrames(
          BinaryReader reader,
          NNS_SUBMOTION submotion)
        {
            uint frameType = submotion.fType & NND_SMOTTYPE_FRAME_MASK; // 16 bit integer or floating point
            uint animType = submotion.fType & NND_SMOTTYPE_VALUETYPE_MASK;
            uint interpType = submotion.fIPType & NND_SMOTIPTYPE_IP_MASK;
            if (((int)animType & NND_SMOTTYPE_TRANSLATION_MASK) != 0)
            {
                if (NND_SMOTTYPE_FRAME_FLOAT == frameType)
                {
                    switch (interpType)
                    {
                        case NND_SMOTIPTYPE_SPLINE:
                        case NND_SMOTIPTYPE_BEZIER:
                            throw new NotImplementedException("Investigation needed");
                        default:
                            submotion.pKeyList = NNS_MOTION_KEY_Class1.Read(reader, submotion.nKeyFrame);
                            break;
                    }
                }
                else
                {
                    if (NND_SMOTTYPE_FRAME_SINT16 == frameType)
                        throw new NotImplementedException("Investigation needed");
                    throw new NotImplementedException("Investigation needed");
                }
            }
            else if (((int)animType & NND_SMOTTYPE_ROTATION_MASK) != 0)
            {
                if (NND_SMOTTYPE_FRAME_FLOAT == frameType)
                {
                    submotion.pKeyList = NNS_MOTION_KEY_Class13.Read(reader, submotion.nKeyFrame);
                }
                else if (NND_SMOTTYPE_FRAME_SINT16 != frameType)
                {
                    throw new NotImplementedException("Investigation needed");
                }
                else if (interpType == NND_SMOTIPTYPE_SPLINE)
                {
                    throw new NotImplementedException("Investigation needed");
                }
                else if (interpType == NND_SMOTIPTYPE_QUAT_LERP)
                {
                    submotion.pKeyList = NNS_MOTION_KEY_Class16.Read(reader, submotion.nKeyFrame);
                }
                else
                {
                    submotion.pKeyList = NNS_MOTION_KEY_Class14.Read(reader, submotion.nKeyFrame);
                }

            }
            else if (((int)animType & NND_SMOTTYPE_SCALING_MASK) != 0)
            {
                if (NND_SMOTTYPE_FRAME_FLOAT == frameType)
                {
                    switch (interpType)
                    {
                        case 1:
                        case 16:
                            throw new NotImplementedException("Investigation needed");
                        default:
                            submotion.pKeyList = NNS_MOTION_KEY_Class1.Read(reader, submotion.nKeyFrame);
                            break;
                    }
                }
                else
                {
                    if (NND_SMOTTYPE_FRAME_SINT16 == frameType)
                        throw new NotImplementedException("Investigation needed");
                    throw new NotImplementedException("Investigation needed");
                }
            }
            else
            {
                if (((int)animType & NND_SMOTTYPE_USER_MASK) != 0)
                {
                    if (NND_SMOTTYPE_FRAME_FLOAT == frameType)
                        throw new NotImplementedException("Investigation needed");
                    if (NND_SMOTTYPE_FRAME_SINT16 == frameType)
                        throw new NotImplementedException("Investigation needed");
                    throw new NotImplementedException("Investigation needed");
                }

                if (((int)animType & NND_SMOTIPTYPE_OFFSET) == 0)
                    throw new NotImplementedException("Investigation needed");

                if (NND_SMOTTYPE_FRAME_FLOAT == frameType)
                {
                    submotion.pKeyList = NNS_MOTION_KEY_Class11.Read(reader, submotion.nKeyFrame);
                }
                else
                {
                    if (NND_SMOTTYPE_FRAME_SINT16 != frameType)
                        throw new NotImplementedException("Investigation needed");

                    if (submotion.KeySize != 8)
                        throw new NotImplementedException("Investigation needed");
                    submotion.pKeyList = NNS_MOTION_KEY_Class16.Read(reader, submotion.nKeyFrame);
                }
            }
        }

        private static void ReadCameraMotionKeyFrames(
          BinaryReader reader,
          NNS_SUBMOTION submotion)
        {
            int fType1 = (int)submotion.fType;
            int fType2 = (int)submotion.fType;
            int fType3 = (int)submotion.fType;
            int fIpType = (int)submotion.fIPType;
            throw new NotImplementedException();
        }

        private static void ReadLightMotionKeyFrames(
          BinaryReader reader,
          NNS_SUBMOTION submotion)
        {
            int fType1 = (int)submotion.fType;
            int fType2 = (int)submotion.fType;
            int fType3 = (int)submotion.fType;
            int fIpType = (int)submotion.fIPType;
            throw new NotImplementedException();
        }

        private static void ReadMorphMotionKeyFrames(
          BinaryReader reader,
          NNS_SUBMOTION submotion)
        {
            int fType1 = (int)submotion.fType;
            int fType2 = (int)submotion.fType;
            int fType3 = (int)submotion.fType;
            int fIpType = (int)submotion.fIPType;
            throw new NotImplementedException();
        }

        private static void ReadMaterialMotionKeyFrames(
          BinaryReader reader,
          NNS_SUBMOTION submotion)
        {
            int fType1 = (int)submotion.fType;
            int fType2 = (int)submotion.fType;
            uint animType = submotion.fType & NND_SMOTTYPE_VALUETYPE_MASK;
            uint interpType = submotion.fIPType & NND_SMOTIPTYPE_IP_MASK;

            if (((int)animType & 256) != 0)
                throw new NotImplementedException("Investigation needed");
            if (((int)animType & NND_SMOTTYPE_DIFFUSE_MASK) != 0)
            {
                switch (interpType)
                {
                    case 1:
                    case 16:
                    label_8:
                        throw new NotImplementedException("Investigation needed");
                    default:
                        switch (submotion.KeySize)
                        {
                            case 8:
                                submotion.pKeyList = NNS_MOTION_KEY_Class1.Read(reader, submotion.nKeyFrame);
                                return;
                            case 16:
                                submotion.pKeyList = NNS_MOTION_KEY_Class5.Read(reader, submotion.nKeyFrame);
                                return;
                            default:
                                goto label_8;
                        }
                }
            }
            else
            {
                if (((int)animType & 4096) != 0)
                    throw new NotImplementedException("Investigation needed");
                if (((int)animType & NND_SMOTTYPE_SPECULAR_MASK) != 0)
                {
                    switch (interpType)
                    {
                        case 1:
                        case 16:
                            throw new NotImplementedException("Investigation needed");
                        default:
                            submotion.pKeyList = NNS_MOTION_KEY_Class5.Read(reader, submotion.nKeyFrame);
                            break;
                    }
                }
                else
                {
                    if (((int)animType & NND_MOTIONTYPE_NOREPEAT) != 0)
                        throw new NotImplementedException("Investigation needed");
                    if (((int)animType & NND_MOTIONTYPE_CONSTREPEAT) != 0)
                        throw new NotImplementedException("Investigation needed");
                    if (((int)animType & NND_SMOTTYPE_AMBIENT_MASK) != 0)
                    {
                        switch (interpType)
                        {
                            case 1:
                            case 16:
                                throw new NotImplementedException("Investigation needed");
                            default:
                                submotion.pKeyList = NNS_MOTION_KEY_Class5.Read(reader, submotion.nKeyFrame);
                                break;
                        }
                    }
                    else
                    {
                        if (((int)animType & 2097152) != 0)
                            throw new NotImplementedException("Investigation needed");
                        if (((int)animType & NND_SMOTTYPE_TEXTURE_BLEND) != 0)
                        {
                            switch (interpType)
                            {
                                case 1:
                                    break;
                                case 16:
                                    break;
                                default:
                                    if (submotion.KeySize != 8)
                                        break;
                                    submotion.pKeyList = NNS_MOTION_KEY_Class1.Read(reader, submotion.nKeyFrame);
                                    break;
                            }
                        }
                        else if (((int)animType & NND_SMOTTYPE_OFFSET_MASK) != 0)
                        {
                            switch (interpType)
                            {
                                case 1:
                                    throw new NotImplementedException("Investigation needed");
                                case 16:
                                    submotion.pKeyList = NNS_MOTION_KEY_Class2.Read(reader, submotion.nKeyFrame);
                                    break;
                                default:
                                    submotion.pKeyList = NNS_MOTION_KEY_Class1.Read(reader, submotion.nKeyFrame);
                                    break;
                            }
                        }
                        else
                        {
                            if (((int)animType & NND_SMOTTYPE_MATCLBK_USER) != 0)
                                throw new NotImplementedException("Investigation needed");
                            throw new NotImplementedException("Investigation needed");
                        }
                    }
                }
            }
        }
    }
}
