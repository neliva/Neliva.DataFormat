// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Neliva
{
    /// <summary>
    /// Provides support to encode and decode data in hex and base32.
    /// </summary>
    public static partial class DataFormat
    {
        /// <summary>
        /// Represents the max number of elements in a map table
        /// and an invalid character in a map table.
        /// </summary>
        private const int MC = 128;

        /// <summary>
        /// The base32 and hex alphabet.
        /// </summary>
        private static ReadOnlySpan<byte> HexAndBase32Alphabet => new byte[]
        {
            (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7',  //  base32 + hex
            (byte)'8', (byte)'9', (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f',  //  base32 + hex
            (byte)'g', (byte)'h', (byte)'j', (byte)'k', (byte)'m', (byte)'n', (byte)'p', (byte)'q',  //  base32
            (byte)'r', (byte)'s', (byte)'t', (byte)'v', (byte)'w', (byte)'x', (byte)'y', (byte)'z',  //  base32
        };

        private static Guid ReadGuidBigEndian(Span<byte> value)
        {
            return new Guid(
                BinaryPrimitives.ReadUInt32BigEndian(value.Slice(0, 4)),
                BinaryPrimitives.ReadUInt16BigEndian(value.Slice(4, 2)),
                BinaryPrimitives.ReadUInt16BigEndian(value.Slice(6, 2)),
                value[8],
                value[9],
                value[10],
                value[11],
                value[12],
                value[13],
                value[14],
                value[15]);
        }

        private static void WriteGuidBigEndian(Guid value, Span<byte> destination)
        {
            if (!value.TryWriteBytes(destination))
            {
                throw new ArgumentOutOfRangeException(nameof(destination));
            }

            // Fix broken GUID endianness
            Swap(destination, 0, 3);
            Swap(destination, 1, 2);
            Swap(destination, 4, 5);
            Swap(destination, 6, 7);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap(Span<byte> value, int idx1, int idx2)
        {
            var tmp = value[idx1];

            value[idx1] = value[idx2];
            value[idx2] = tmp;
        }
    }
}