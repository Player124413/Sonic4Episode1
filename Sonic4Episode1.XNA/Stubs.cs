using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Sonic4ep1
{
    public static class Strings
    {
        public const string ID_STAGE1 = @"SPLASH HILL ZONE {0}";
        public const string ID_STAGE2 = @"CASINO STREET ZONE {0}";
        public const string ID_STAGE3 = @"LOST LABYRINTH ZONE {0}";
        public const string ID_STAGE4 = @"MAD GEAR ZONE {0}";
        public const string ID_SSTAGE = @"SPECIAL STAGE {0}";
        public const string ID_LOADING = @"Loading...";
        public const string ID_LB_DATE = @"Date";
        public const string ID_LB_GTAG = @"Gamertag";
        public const string ID_LB_RANK = @"Rank";
        public const string ID_YES = @"Yes";
        public const string ID_ACT = @"ACT {0}";
        public const string ID_LB_BUY = @"Please purchase the game in order to view the leaderboards";
        public const string ID_RESUME_TEXT = @"Do you want to resume from your most recent Star Post?";
        public const string ID_UPDATE_CAPTION = @"Title Update Available";
        public const string ID_BOSS = @"BOSS";
        public const string ID_LEADERBOARDS = @"Leaderboards";
        public const string ID_BESTSCORE = @"Best Score";
        public const string ID_A6_NAME = @"Contender";
        public const string ID_A6_DESC = @"Upload your recorded clear times for all Time Attack stages.";
        public const string ID_A5_NAME = @"All Stages Cleared!";
        public const string ID_A5_DESC = @"Defeat the final boss and view the ending.";
        public const string ID_A2_NAME = @"The First Chaos Emerald";
        public const string ID_A2_DESC = @"Acquire a Chaos Emerald.";
        public const string ID_A7_NAME = @"Ring Collector";
        public const string ID_A7_DESC = @"Collect all the Rings in 'Special Stage 1' and Clear the stage.";
        public const string ID_A4_NAME = @"Golden Flash";
        public const string ID_A4_DESC = @"Clear all Acts as Super Sonic.";
        public const string ID_A1_NAME = @"The Story Begins";
        public const string ID_A1_DESC = @"Clear Splash Hill Zone Act 1.";
        public const string ID_MUSIC_INTERRUPT_CAPTON = @"Interrupt Music";
        public const string ID_MUSIC_INTERRUPT_TEXT = @"Do you want to interrupt your music and use game background music?";
        public const string ID_SUPERSONIC = @"Super Sonic";
        public const string ID_A3_NAME = @"Enemy Hunter";
        public const string ID_A3_DESC = @"Defeat 1,000 enemies.";
        public const string ID_BESTTIME = @"Best Time";
        public const string ID_A9_DESC = @"Acquire all seven Chaos Emeralds.";
        public const string ID_A9_NAME = @"Super Sonic Genesis";
        public const string ID_A8_DESC = @"Build up 99 or more extra lives.";
        public const string ID_A8_NAME = @"Immortal";
        public const string ID_FINALZONE = @"E.G.G. STATION ZONE BOSS";
        public const string ID_OK = @"OK";
        public const string ID_NO = @"No";
        public const string ID_LB_UPDATE = @"Please update the game to view the Leaderboard";
        public const string ID_LB_UNABLE = @"You must be signed in to Xbox LIVE to access this feature.";
        public const string ID_RESUME_CAPTION = @"Resume game?";
        public const string ID_SONIC = @"Sonic";
        public const string ID_ACHIEVEMENTS = @"Achievements";
        public const string ID_A14_NAME = @"Casino Street Zone";
        public const string ID_A14_DESC = @"Finish all 3 Acts and boss round of Casino Street Zone";
        public const string ID_LB_OFFLINE = @"Please sign in to view the Leaderboard";
        public const string ID_A13_NAME = @"Splash Hill Zone";
        public const string ID_A13_DESC = @"Finish all 3 Acts and boss round of Splash Hill Zone";
        public const string ID_A16_NAME = @"Mad Gear Zone";
        public const string ID_A16_DESC = @"Finish all 3 Acts and boss round of Mad Gear Zone";
        public const string ID_A15_NAME = @"Lost Labyrinth Zone";
        public const string ID_A15_DESC = @"Finish all 3 Acts and boss round of Lost Labrinth Zone";
        public const string ID_A10_NAME = @"Speed's My Game";
        public const string ID_A10_DESC = @"Clear Splash Hill Zone Act 1 in less than a minute.";
        public const string ID_A12_NAME = @"Centurion";
        public const string ID_A12_DESC = @"Get 120 rings in Mad Gear Zone Act 1.";
        public const string ID_A11_NAME = @"Untouchable";
        public const string ID_A11_DESC = @"Clear the E.G.G. Station Zone without taking any damage.";
        public const string ID_LB_NORECORDS = @"No Records";
        public const string ID_UPDATE_TEXT = @"An update is available! This update is required to connect to Xbox LIVE. Update now?";

        public static CultureInfo Culture { get; internal set; }
    }
}

namespace System.Runtime.CompilerServices
{
    unsafe class UnsafeEx
    {
        public static TTo BitCast<TFrom, TTo>(TFrom source)
            where TFrom : struct
            where TTo : struct
        {
            if (sizeof(TFrom) != sizeof(TTo) || !typeof(TFrom).IsValueType || !typeof(TTo).IsValueType)
            {
                ThrowNotSupportedException();
            }
            return Unsafe.ReadUnaligned<TTo>(ref Unsafe.As<TFrom, byte>(ref source));
        }

        public static void ThrowNotSupportedException()
            => throw new NotSupportedException();
    }
}

namespace System
{
    unsafe class BitConverterEx
    {
        /// <summary>
        /// Converts the specified single-precision floating point number to a 32-bit unsigned integer.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>A 32-bit unsigned integer whose bits are identical to <paramref name="value"/>.</returns>
        public static uint SingleToUInt32Bits(float value) => UnsafeEx.BitCast<float, uint>(value);

        /// <summary>
        /// Converts the specified 32-bit unsigned integer to a single-precision floating point number.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>A single-precision floating point number whose bits are identical to <paramref name="value"/>.</returns>
        public static float UInt32BitsToSingle(uint value) => UnsafeEx.BitCast<uint, float>(value);

        /// <summary>
        /// Converts the specified 32-bit signed integer to a single-precision floating point number.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>A single-precision floating point number whose bits are identical to <paramref name="value"/>.</returns>
        public static float Int32BitsToSingle(int value) => UnsafeEx.BitCast<int, float>(value);
        /// <summary>
        /// Converts the specified double-precision floating point number to a 64-bit unsigned integer.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>A 64-bit unsigned integer whose bits are identical to <paramref name="value"/>.</returns>
        public static ulong DoubleToUInt64Bits(double value) => UnsafeEx.BitCast<double, ulong>(value);

        /// <summary>
        /// Converts the specified 64-bit unsigned integer to a double-precision floating point number.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>A double-precision floating point number whose bits are identical to <paramref name="value"/>.</returns>
        public static double UInt64BitsToDouble(ulong value) => UnsafeEx.BitCast<ulong, double>(value);
    }
}

namespace System.Buffers.Binary
{
    class BinaryPrimitivesEx
    {

        /// <summary>
        /// Reads a <see cref="float" /> from the beginning of a read-only span of bytes, as big endian.
        /// </summary>
        /// <param name="source">The read-only span to read.</param>
        /// <returns>The big endian value.</returns>
        /// <remarks>Reads exactly 4 bytes from the beginning of the span.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="source"/> is too small to contain a <see cref="float" />.
        /// </exception>
        public static float ReadSingleBigEndian(ReadOnlySpan<byte> source)
        {
            return BitConverter.IsLittleEndian ?
                BitConverterEx.UInt32BitsToSingle(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<uint>(source))) :
                MemoryMarshal.Read<float>(source);
        }

        /// <summary>
        /// Reads a <see cref="float" /> from the beginning of a read-only span of bytes, as little endian.
        /// </summary>
        /// <param name="source">The read-only span to read.</param>
        /// <returns>The little endian value.</returns>
        /// <remarks>Reads exactly 4 bytes from the beginning of the span.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="source"/> is too small to contain a <see cref="float" />.
        /// </exception>
        public static float ReadSingleLittleEndian(ReadOnlySpan<byte> source)
        {
            return !BitConverter.IsLittleEndian ?
                BitConverterEx.UInt32BitsToSingle(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<uint>(source))) :
                MemoryMarshal.Read<float>(source);
        }
    }
}