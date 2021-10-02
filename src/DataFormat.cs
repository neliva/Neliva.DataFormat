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
        /// and an invalid character in a map table.
        /// </summary>
        private const int MC = 128;

        /// <summary>
        /// The base32 alphabet.
        /// </summary>
        private static char[] HexAndBase32Alphabet = new char[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7',  //  base32 + hex
            '8', '9', 'a', 'b', 'c', 'd', 'e', 'f',  //  base32 + hex
            'g', 'h', 'j', 'k', 'm', 'n', 'p', 'q',  //  base32
            'r', 's', 't', 'v', 'w', 'x', 'y', 'z',  //  base32
        };
    }
}