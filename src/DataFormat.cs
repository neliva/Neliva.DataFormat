// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

namespace Neliva
{
    /// <summary>
    /// Provides support to encode and decode data in hex and base32.
    /// </summary>
    public static partial class DataFormat
    {
        /// <summary>
        /// Represents the max number of elements in a map table
        /// and an invalid character in the map table.
        /// </summary>
        private const int MC = 128;

        /// <summary>
        /// The Base32 alphabet.
        /// </summary>
        private static char[] HexAndBase32Alphabet = new char[]
        {
            '\x30', '\x31', '\x32', '\x33', '\x34', '\x35', '\x36', '\x37',  // 01234567    | Hex + Base32
            '\x38', '\x39', '\x61', '\x62', '\x63', '\x64', '\x65', '\x66',  // 89abcdef    | Hex + Base32
            '\x67', '\x68', '\x6a', '\x6b', '\x6d', '\x6e', '\x70', '\x71',  // ghjkmnpq    | Base32
            '\x72', '\x73', '\x74', '\x76', '\x77', '\x78', '\x79', '\x7a',  // rstvwxyz    | Base32
        };
    }
}