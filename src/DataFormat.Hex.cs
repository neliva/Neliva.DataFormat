// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

using System;

namespace Neliva
{
    public static partial class DataFormat
    {
        /// <summary>
        /// The map to decode bytes from hex characters.
        /// </summary>
        private static ReadOnlySpan<byte> HexMap => new byte[MC]
        {
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
        /// Converts the span <paramref name="value"/> to lowercase hex representation.
        /// </summary>
        /// <param name="value">
        /// The span to convert.
        /// </param>
        /// <returns>
        /// The string representation in hex of the provided span <paramref name="value"/>.
        /// If the <paramref name="value"/> span is empty then an empty string is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is too large to be encoded.
        /// </exception>
        public static unsafe string ToHex(ReadOnlySpan<byte> value)
        {
            int length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            if (length > (int.MaxValue / 2))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Input is too large to be processed.");
            }

            fixed (byte* bytesPtr = value)
            {
                return string.Create(length * 2, (Ptr: (IntPtr)bytesPtr, Length: length), (dest, args) =>
                {
                    var src = new ReadOnlySpan<byte>((byte*)args.Ptr, args.Length);

                    for (int i = 0; i < args.Length; i++)
                    {
                        int b = src[i];
                        int c = i << 1;

                        dest[c] = (char)HexAndBase32Alphabet[b >> 4];
                        dest[c + 1] = (char)HexAndBase32Alphabet[b & 0x0F];
                    }
                });
            }
        }

        /// <summary>
        /// Converts the hex encoded span to a byte array representation.
        /// </summary>
        /// <param name="value">
        /// The span to convert.
        /// </param>
        /// <returns>
        /// The byte array representation of the provided hex span <paramref name="value"/>.
        /// If the <paramref name="value"/> span is empty
        /// then a zero length byte array is returned.
        /// </returns>
        /// <exception cref="FormatException">
        /// <para>
        /// The length of <paramref name="value"/> is not a multiple of 2.
        /// </para>
        /// <para>
        /// OR
        /// </para>
        /// <para>
        /// The <paramref name="value"/> contains a non-hex character.
        /// </para>
        /// </exception>
        public static byte[] FromHex(ReadOnlySpan<char> value)
        {
            int length = value.Length;

            if (length == 0)
            {
                return Array.Empty<byte>();
            }

            if ((length & 1) != 0)
            {
                throw new FormatException("The input is not a valid hex string as its length is not a multiple of 2.");
            }

            byte[] output = GC.AllocateUninitializedArray<byte>(length / 2);

            for (int i = 0; i < value.Length; i += 2)
            {
                int c1 = value[i];
                int c2 = value[i + 1];

                if ((c1 >= MC) || ((c1 = HexMap[c1]) >= MC) || (c2 >= MC) || ((c2 = HexMap[c2]) >= MC))
                {
                    throw new FormatException("The input is not a valid hex string as it contains a non-hex character.");
                }

                output[i >> 1] = (byte)((c1 << 4) | c2);
            }

            return output;
        }
    }
}