// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

using System;

namespace Neliva
{
    public static partial class DataFormat
    {
        /// <summary>
        /// The Base32 alphabet.
        /// </summary>
        private static byte[] Base32Alphabet = new byte[]
        {
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,  // 01234567
            0x38, 0x39, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66,  // 89abcdef
            0x67, 0x68, 0x6a, 0x6b, 0x6d, 0x6e, 0x70, 0x71,  // ghjkmnpq
            0x72, 0x73, 0x74, 0x76, 0x77, 0x78, 0x79, 0x7a,  // rstvwxyz
        };

        /// <summary>
        /// The map to decode bytes from Base32 <c>0123456789abcdefghjkmnpqrstvwxyz</c> characters.
        /// </summary>
        private static byte[] Base32Map = new byte[MC]
        {
            MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC,
            MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC,
            MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC, MC,
            00, 01, 02, 03, 04, 05, 06, 07, 08, 09, MC, MC, MC, MC, MC, MC,
            MC, 10, 11, 12, 13, 14, 15, 16, 17, MC, 18, 19, MC, 20, 21, MC,
            22, 23, 24, 25, 26, MC, 27, 28, 29, 30, 31, MC, MC, MC, MC, MC,
            MC, 10, 11, 12, 13, 14, 15, 16, 17, MC, 18, 19, MC, 20, 21, MC,
            22, 23, 24, 25, 26, MC, 27, 28, 29, 30, 31, MC, MC, MC, MC, MC,
        };

        /// <summary>
        /// Convert the provided <paramref name="value"/> to
        /// Base32 representation.
        /// </summary>
        /// <param name="value">
        /// The value to convert to Base32 representation.</param>
        /// <returns>
        /// The Base32 representation of the <paramref name="value"/>.
        /// </returns>
        public static string ToBase32(byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            int length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            int maxLength = ((length * 8) + 4) / 5;

            return string.Create<byte[]>(maxLength, value, (dest, src) =>
            {
                var alphabet = Base32Alphabet;

                int srcLen = src.Length;
                int buffer = src[0];
                int count = 0;
                int next = 1;
                int bitsLeft = 8;

                while (bitsLeft > 0 || next < srcLen)
                {
                    if (bitsLeft < 5)
                    {
                        if (next < srcLen)
                        {
                            buffer = (buffer << 8) | (src[next++] & 0xff);
                            bitsLeft += 8;
                        }
                        else
                        {
                            int pad = 5 - bitsLeft;
                            buffer <<= pad;
                            bitsLeft += pad;
                        }
                    }

                    int index = 0x1f & (buffer >> (bitsLeft - 5));
                    bitsLeft -= 5;

                    dest[count++] = (char)alphabet[index];
                }
            });
        }

        /// <summary>
        /// Convert the provided <paramref name="value"/> from
        /// Base32 representation.
        /// </summary>
        /// <param name="value">
        /// The value to convert from Base32 representation.</param>
        /// <returns>
        /// The byte array representation of the provided Base32 <paramref name="value"/>.
        /// If the <paramref name="value"/> parameter is an empty string
        /// then a zero length byte array is returned.
        /// </returns>
        public static byte[] FromBase32(string value)
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

            switch (length % 8)
            {
                case 1:
                case 3:
                case 6:
                    throw new ArgumentException("Invalid length of Base32 string.", nameof(value));
            }

            byte[] output = new byte[length * 5 / 8];

            int buffer = 0;
            int bitsLeft = 0;
            int count = 0;

            for (int i = 0; i < value.Length; i++)
            {
                int ch = value[i];

                if (ch >= MC || ((ch = Base32Map[ch]) >= MC))
                {
                    throw new ArgumentException("Base32 string has invalid chars.", nameof(value));
                }

                buffer = (buffer << 5) | ch;
                bitsLeft += 5;

                if (bitsLeft >= 8)
                {
                    output[count++] = (byte)(buffer >> (bitsLeft - 8));
                    bitsLeft -= 8;
                }
            }

            return output;
        }
    }
}