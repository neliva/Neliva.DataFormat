// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

using System;

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
    }
}