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

        // key layout comes from the axis bits and angle type alone, matching nnCalcMotionTranslate/
        // Scale/Rotate; interpolation type must not enter into it
        private static void ReadNodeMotionKeyFrames(
          BinaryReader reader,
          NNS_SUBMOTION submotion)
        {
            uint fType = submotion.fType;
            uint interpType = submotion.fIPType & NND_SMOTIPTYPE_IP_MASK;

            // spline/Bezier keys are wider records, none of the classes below describe them
            if (interpType == NND_SMOTIPTYPE_SPLINE || interpType == NND_SMOTIPTYPE_BEZIER
                || interpType == NND_SMOTIPTYPE_SI_SPLINE)
            {
                throw new NotImplementedException(
                    $"submotion fType 0x{fType:X} interpolation 0x{interpType:X} keys carry handles we do not read");
            }

            uint translation = fType & NND_SMOTTYPE_TRANSLATION_MASK;
            if (translation != 0)
            {
                ReadVectorOrScalar(reader, submotion, translation == NND_SMOTTYPE_TRANSLATION_MASK);
                return;
            }

            uint scaling = fType & NND_SMOTTYPE_SCALING_MASK;
            if (scaling != 0)
            {
                ReadVectorOrScalar(reader, submotion, scaling == NND_SMOTTYPE_SCALING_MASK);
                return;
            }

            uint rotation = fType & NND_SMOTTYPE_ROTATION_MASK;
            if (rotation != 0)
            {
                ReadRotation(reader, submotion, rotation);
                return;
            }

            if ((fType & NND_SMOTTYPE_USER_MASK) != 0)
                throw new NotImplementedException($"submotion fType 0x{fType:X} carries user keys we do not read");

            if ((fType & NND_SMOTTYPE_NODEHIDE) != 0)
            {
                submotion.pKeyList = NNS_MOTION_KEY_Class11.Read(reader, submotion.nKeyFrame);
                CheckKeySize(submotion, 8);
                return;
            }

            throw new NotImplementedException($"submotion fType 0x{fType:X} has no known key layout");
        }

        // F3 when all three axes are animated together, F1 for a single axis
        private static void ReadVectorOrScalar(BinaryReader reader, NNS_SUBMOTION submotion, bool allAxes)
        {
            if (allAxes)
            {
                submotion.pKeyList = NNS_MOTION_KEY_Class5.Read(reader, submotion.nKeyFrame);
                CheckKeySize(submotion, 16);
            }
            else
            {
                submotion.pKeyList = NNS_MOTION_KEY_Class1.Read(reader, submotion.nKeyFrame);
                CheckKeySize(submotion, 8);
            }
        }

        private static void ReadRotation(BinaryReader reader, NNS_SUBMOTION submotion, uint rotation)
        {
            if (rotation == NND_SMOTTYPE_QUATERNION)
            {
                throw new NotImplementedException(
                    $"submotion fType 0x{submotion.fType:X} carries quaternion keys (Class7) we do not read");
            }

            const uint allAxesMask =
                NND_SMOTTYPE_ROTATION_X | NND_SMOTTYPE_ROTATION_Y | NND_SMOTTYPE_ROTATION_Z;
            bool allAxes = rotation == allAxesMask;

            if ((submotion.fType & NND_SMOTTYPE_ANGLE_MASK) == NND_SMOTTYPE_ANGLE_ANGLE16)
            {
                if (allAxes)
                {
                    submotion.pKeyList = NNS_MOTION_KEY_Class16.Read(reader, submotion.nKeyFrame);
                    CheckKeySize(submotion, 8);
                }
                else
                {
                    submotion.pKeyList = NNS_MOTION_KEY_Class14.Read(reader, submotion.nKeyFrame);
                    CheckKeySize(submotion, 4);
                }
                return;
            }

            if (allAxes)
            {
                submotion.pKeyList = NNS_MOTION_KEY_Class13.Read(reader, submotion.nKeyFrame);
                CheckKeySize(submotion, 16);
                return;
            }

            submotion.pKeyList = NNS_MOTION_KEY_Class8.Read(reader, submotion.nKeyFrame);
            CheckKeySize(submotion, 8);
        }

        private static void CheckKeySize(NNS_SUBMOTION submotion, int expected)
        {
            if (submotion.KeySize != 0 && submotion.KeySize != expected)
            {
                throw new InvalidDataException(
                    $"submotion fType 0x{submotion.fType:X} states KeySize {submotion.KeySize}"
                    + $" but the layout chosen for it is {expected} bytes");
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

        // dispatches on fType & 0xFFFFFF00, matching nnCalcMaterialMotion's per-field switch
        private static void ReadMaterialMotionKeyFrames(
          BinaryReader reader,
          NNS_SUBMOTION submotion)
        {
            // Bezier of a single float is Class2; nothing describes Bezier/spline for multi-component fields
            uint interpType = submotion.fIPType & NND_SMOTIPTYPE_IP_MASK;
            bool bezier = interpType == NND_SMOTIPTYPE_BEZIER;

            if (interpType == NND_SMOTIPTYPE_SPLINE || interpType == NND_SMOTIPTYPE_SI_SPLINE)
            {
                throw new NotImplementedException(
                    $"material submotion fType 0x{submotion.fType:X} interpolation 0x{interpType:X}"
                    + " keys carry handles we do not read");
            }

            switch (submotion.fType & 0xFFFFFF00U)
            {
                case NND_SMOTTYPE_DIFFUSE_MASK:
                case NND_SMOTTYPE_SPECULAR_MASK:
                case NND_SMOTTYPE_AMBIENT_MASK:
                    submotion.pKeyList = NNS_MOTION_KEY_Class5.Read(reader, submotion.nKeyFrame);
                    CheckKeySize(submotion, 16);
                    return;

                case NND_SMOTTYPE_OFFSET_MASK:
                    submotion.pKeyList = NNS_MOTION_KEY_Class4.Read(reader, submotion.nKeyFrame);
                    CheckKeySize(submotion, 12);
                    return;

                // hide and the texture index are the two integer valued fields
                case NND_SMOTTYPE_HIDE:
                case NND_SMOTTYPE_TEXTURE_INDEX:
                    submotion.pKeyList = NNS_MOTION_KEY_Class11.Read(reader, submotion.nKeyFrame);
                    CheckKeySize(submotion, 8);
                    return;

                case NND_SMOTTYPE_MATCLBK_USER:
                    throw new NotImplementedException(
                        $"material submotion fType 0x{submotion.fType:X} carries user keys (Class12)"
                        + " we do not read");

                case NND_SMOTTYPE_DIFFUSE_R:
                case NND_SMOTTYPE_DIFFUSE_G:
                case NND_SMOTTYPE_DIFFUSE_B:
                case NND_SMOTTYPE_ALPHA:
                case NND_SMOTTYPE_SPECULAR_R:
                case NND_SMOTTYPE_SPECULAR_G:
                case NND_SMOTTYPE_SPECULAR_B:
                case NND_SMOTTYPE_SPECULAR_LEVEL:
                case NND_SMOTTYPE_SPECULAR_GLOSS:
                case NND_SMOTTYPE_AMBIENT_R:
                case NND_SMOTTYPE_AMBIENT_G:
                case NND_SMOTTYPE_AMBIENT_B:
                case NND_SMOTTYPE_TEXTURE_BLEND:
                case NND_SMOTTYPE_OFFSET_U:
                case NND_SMOTTYPE_OFFSET_V:
                    if (bezier)
                    {
                        submotion.pKeyList = NNS_MOTION_KEY_Class2.Read(reader, submotion.nKeyFrame);
                        CheckKeySize(submotion, 24);
                        return;
                    }

                    submotion.pKeyList = NNS_MOTION_KEY_Class1.Read(reader, submotion.nKeyFrame);
                    CheckKeySize(submotion, 8);
                    return;

                default:
                    throw new NotImplementedException(
                        $"material submotion fType 0x{submotion.fType:X} names no field the"
                        + " evaluator handles");
            }
        }

    }
}
