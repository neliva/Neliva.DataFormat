// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

using System;

namespace Neliva
{
    public static partial class DataFormat
    {
        /// <summary>
        /// The hex alphabet 0-9 and a-f.
        /// </summary>
        private static byte[] HexAlphabet = new byte[16]
        {
            //
            // https://datatracker.ietf.org/doc/html/rfc4648
            //
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,  // 01234567
            0x38, 0x39, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66,  // 89abcdef
        };

        /// <summary>
        /// The map to decode bytes from hex characters.
        /// </summary>
        private static byte[] HexMap = new byte[MC] {
            MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC,
            MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC,
            MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC,
            00, 01, 02, 03, 04, 05, 06, 07, 08, 09, MC, MC, MC, MC, MC, MC,
            MC, 10, 11, 12, 13, 14, 15, MC, MC, MC, MC, MC, MC, MC, MC, MC,
            MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC,
            MC, 10, 11, 12, 13, 14, 15, MC, MC, MC, MC, MC, MC, MC, MC, MC,
            MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC,
        };

        /// <summary>
        /// Converts a byte array to a hexadecimal representation.
        /// </summary>
        /// <param name="value">
        /// The byte array to convert.
        /// </param>
        /// <returns>
        /// The hexadecimal representation of the provided byte array.
        /// If the <paramref name="value"/> parameter has no elements
        /// then an empty string is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// This function does not allocate any extra memory
        /// besides the returned string object.
        /// </remarks>
        public static unsafe string ToHex(ReadOnlySpan<byte> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Length > (int.MaxValue / 2))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            int length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            fixed (byte* bytesPtr = value)
            {
                return string.Create(length << 1, (Ptr: (IntPtr)bytesPtr, value.Length), (dest, args) =>
                {
                    var alphabet = HexAlphabet;

                    var src = new ReadOnlySpan<byte>((byte*)args.Ptr, args.Length);

                    for (int i = 0; i < args.Length; i++)
                    {
                        int b = src[i];
                        int c = i << 1;

                        dest[c] = (char)alphabet[b >> 4];
                        dest[c + 1] = (char)alphabet[b & 0x0F];
                    }
                });
            }
        }

        /// <summary>
        /// Converts a hexadecimal string to a byte array representation.
        /// </summary>
        /// <param name="value">
        /// The string to convert.
        /// </param>
        /// <returns>
        /// The byte array representation of the provided hexadecimal string.
        /// If the <paramref name="value"/> parameter is an empty string
        /// then a zero length byte array is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>
        /// The <paramref name="value"/> parameter does not contain
        /// an even number of characters.
        /// </para>
        /// <para>
        /// OR
        /// </para>
        /// <para>
        /// The <paramref name="value"/> parameter contains characters
        /// that are not hexadecimal digits.
        /// </para>
        /// </exception>
        public static byte[] FromHex(ReadOnlySpan<char> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            int length = value.Length;

            if (length == 0)
            {
                return Array.Empty<byte>();
            }

            if ((length & 1) != 0)
            {
                throw new ArgumentException("Odd length of hex string.", nameof(value));
            }

            byte[] output = new byte[length / 2];

            for (int i = 0; i < value.Length; i += 2)
            {
                int c1 = value[i];
                int c2 = value[i + 1];

                if ((c1 >= MC) || ((c1 = HexMap[c1]) >= MC) || (c2 >= MC) || ((c2 = HexMap[c2]) >= MC))
                {
                    throw new ArgumentException("Hex string has invalid chars.", nameof(value));
                }

                output[i >> 1] = (byte)((c1 << 4) | c2);
            }

            return output;
        }

        /// <summary>
        /// Verifies if the provided <paramref name="value"/> is
        /// a valid hexadecimal string.
        /// </summary>
        /// <param name="value">
        /// The string to verify.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <paramref name="value"/> is not <c>null</c> and
        /// has an even number of hexadecimal digits; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method returns <c>true</c> for an empty string.
        /// </remarks>
        public static bool IsHex(ReadOnlySpan<char> value)
        {
            if ((value == null) || ((value.Length & 1) != 0))
            {
                return false;
            }

            for (int i = 0; i < value.Length; i += 2)
            {
                int c1 = value[i];
                int c2 = value[i + 1];

                if ((c1 >= MC) || (HexMap[c1] >= MC) || (c2 >= MC) || (HexMap[c2] >= MC))
                {
                    return false;
                }
            }

            return true;
        }
    }
}