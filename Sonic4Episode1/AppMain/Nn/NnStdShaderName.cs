using System;
using System.Text;

internal partial class AppMain
{
    // NNS_STDSHADER_PROFILE, nnlshaderdxg20.h:164; array-backed since the key packer indexes fields by offset
    internal class NNS_STDSHADER_PROFILE
    {
        public const int FieldCount = 46;

        private readonly int[] fields = new int[FieldCount];

        public int this[int index]
        {
            get => fields[index];
            set => fields[index] = value;
        }

        public int bVtxCol { get => fields[0]; set => fields[0] = value; }
        public int bVtxColSpe { get => fields[1]; set => fields[1] = value; }
        public int bVtxNrm { get => fields[2]; set => fields[2] = value; }
        public int bVtxObjSpaceNrm { get => fields[3]; set => fields[3] = value; }
        public int bVtxReverseTan { get => fields[4]; set => fields[4] = value; }
        public int VtxTfmMode { get => fields[5]; set => fields[5] = value; }
        public int bLighting { get => fields[6]; set => fields[6] = value; }
        public int bSpecular { get => fields[7]; set => fields[7] = value; }
        public int bTwoSidedLighting { get => fields[8]; set => fields[8] = value; }
        public int nFragParallelLight { get => fields[9]; set => fields[9] = value; }
        public int nFragPointLight { get => fields[10]; set => fields[10] = value; }
        public int NormalMapType { get => fields[11]; set => fields[11] = value; }
        public int bBaseMap { get => fields[12]; set => fields[12] = value; }
        public int nDecalMap { get => fields[13]; set => fields[13] = value; }
        public int bSpecularMap { get => fields[14]; set => fields[14] = value; }
        public int bShininessMap { get => fields[15]; set => fields[15] = value; }
        public int bDualParaboloidMap { get => fields[16]; set => fields[16] = value; }
        public int bEnvMaskMap { get => fields[17]; set => fields[17] = value; }
        public int bModulateMap { get => fields[18]; set => fields[18] = value; }
        public int bAddMap { get => fields[19]; set => fields[19] = value; }
        public int bOpacityMap { get => fields[20]; set => fields[20] = value; }
        public int bUser1Map { get => fields[21]; set => fields[21] = value; }
        public int bUser2Map { get => fields[22]; set => fields[22] = value; }
        public int bUser3Map { get => fields[23]; set => fields[23] = value; }
        public int bUser4Map { get => fields[24]; set => fields[24] = value; }
        public int bUser5Map { get => fields[25]; set => fields[25] = value; }
        public int bUser6Map { get => fields[26]; set => fields[26] = value; }
        public int bUser7Map { get => fields[27]; set => fields[27] = value; }
        public int bUser8Map { get => fields[28]; set => fields[28] = value; }
        public int nShadowMap { get => fields[29]; set => fields[29] = value; }
        public int bUserSampler2D1 { get => fields[30]; set => fields[30] = value; }
        public int bUserSampler2D2 { get => fields[31]; set => fields[31] = value; }
        public int bUserSampler3D1 { get => fields[32]; set => fields[32] = value; }
        public int bUserSampler3D2 { get => fields[33]; set => fields[33] = value; }
        public int bUserSamplerCube1 { get => fields[34]; set => fields[34] = value; }
        public int bUserSamplerCube2 { get => fields[35]; set => fields[35] = value; }

        // bUserSampler2D1 through bUserSamplerCube2, in NNE_USER_SAMPLER order
        public int GetUserSampler(NNE_USER_SAMPLER sampler) => fields[30 + (int)sampler];

        // in the order the maps above are used, DualParaboloid and Shadow excluded
        public int GetTexCoord(int index) => fields[36 + index];
        public void SetTexCoord(int index, int value) => fields[36 + index] = value;

        public int UserProfile { get => fields[44]; set => fields[44] = value; }
        public int UserProfileDrawobj { get => fields[45]; set => fields[45] = value; }
    }

    // NNS_STDSHADER_NAME, nnlshaderdxg20.h:229. The Uint64 typedef above it is inside an #if 0.
    internal struct NNS_STDSHADER_NAME : IEquatable<NNS_STDSHADER_NAME>
    {
        public ulong low;
        public ulong high;

        public bool Equals(NNS_STDSHADER_NAME other) => low == other.low && high == other.high;
        public override bool Equals(object obj) => obj is NNS_STDSHADER_NAME other && Equals(other);
        public override int GetHashCode() => low.GetHashCode() ^ high.GetHashCode();

        // the 26 digit stem SHADER.AMB names its entries by, before .PSH or .VSH
        public override string ToString()
        {
            char[] digits = new char[NameDigits];
            ulong lo = low;
            ulong hi = high;

            for (int i = NameDigits - 1; i >= 0; --i)
            {
                digits[i] = Base32Alphabet[(int)(lo & 31)];
                lo = (lo >> 5) | (hi << 59);
                hi >>= 5;
            }

            return new string(digits);
        }
    }

    public const int NameDigits = 26;
    private const string Base32Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUV";

    // recovered from nnGetStdShaderName in libnndxg20.lib, see docs/nn-stdshader-ubershader.md
    private static readonly int[] mppStdShaderNameWidth =
    {
        1, 1, 1, 1, 1, 2, 1, 1, 1, 2, 2, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 6, 8,
    };

    private static readonly int[] mppStdShaderNameBias =
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -3, -3, -3, -3, -3, -3, -3, -3, 0, 0,
    };

    public static NNS_STDSHADER_NAME nnGetStdShaderName(NNS_STDSHADER_PROFILE profile)
    {
        ulong low = 0;
        ulong high = 0;

        // the last field lands in the most significant bits
        for (int i = NNS_STDSHADER_PROFILE.FieldCount - 1; i >= 0; --i)
        {
            int width = mppStdShaderNameWidth[i];
            ulong value = (uint)(profile[i] - mppStdShaderNameBias[i]) & ((1u << width) - 1);

            high = (high << width) | (low >> (64 - width));
            low = (low << width) | value;
        }

        return new NNS_STDSHADER_NAME { low = low, high = high };
    }

    public static void nnGetStdShaderProfile(NNS_STDSHADER_PROFILE profile, NNS_STDSHADER_NAME name)
    {
        ulong low = name.low;
        ulong high = name.high;

        for (int i = 0; i < NNS_STDSHADER_PROFILE.FieldCount; ++i)
        {
            int width = mppStdShaderNameWidth[i];
            profile[i] = (int)(low & ((1ul << width) - 1)) + mppStdShaderNameBias[i];

            low = (low >> width) | (high << (64 - width));
            high >>= width;
        }
    }

    public static NNS_STDSHADER_NAME mppStdShaderNameParse(string digits)
    {
        if (digits == null || digits.Length != NameDigits)
            throw new ArgumentException($"a shader name is {NameDigits} base 32 digits", nameof(digits));

        ulong low = 0;
        ulong high = 0;

        foreach (char digit in digits)
        {
            int value = Base32Alphabet.IndexOf(char.ToUpperInvariant(digit));
            if (value < 0)
                throw new ArgumentException($"'{digit}' is not a base 32 digit", nameof(digits));

            high = (high << 5) | (low >> 59);
            low = (low << 5) | (uint)value;
        }

        return new NNS_STDSHADER_NAME { low = low, high = high };
    }
}
