// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

using System;

namespace Neliva
{
    public static partial class DataFormat
    {
        /// <summary>
        /// The map to decode bytes from base32 <c>0123456789abcdefghjkmnpqrstvwxyz</c> characters.
        /// </summary>
        private static ReadOnlySpan<byte> Base32Map => new byte[MC]
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
        /// Converts the span <paramref name="value"/> to lowercase base32 representation
        /// using the <c>0123456789abcdefghjkmnpqrstvwxyz</c> alphabet.
        /// </summary>
        /// <param name="value">
        /// The span to convert.
        /// </param>
        /// <returns>
        /// The string representation in base32 of the provided span <paramref name="value"/>.
        /// If the <paramref name="value"/> span is empty then an empty string is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is too large to be encoded.
        /// </exception>
        public static unsafe string ToBase32(ReadOnlySpan<byte> value)
        {
            int length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            if (length > (int)(((long)int.MaxValue * 5) / 8))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Input is too large to be processed.");
            }

            fixed (byte* bytesPtr = value)
            {
                return string.Create((int)((((long)length * 8) + 4) / 5), (Ptr: (IntPtr)bytesPtr, Length: length), static (dest, args) =>
                {
                    var src = new ReadOnlySpan<byte>((byte*)args.Ptr, args.Length);

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

                        dest[count++] = (char)HexAndBase32Alphabet[index];
                    }
                });
            }
        }

        /// <summary>
        /// Converts the base32 encoded span to a byte array representation.
        /// </summary>
        /// <param name="value">
        /// The span to convert.
        /// </param>
        /// <returns>
        /// The byte array representation of the provided base32 span <paramref name="value"/>.
        /// If the <paramref name="value"/> span is empty
        /// then a zero length byte array is returned.
        /// </returns>
        /// <exception cref="FormatException">
        /// <para>
        /// The length of <paramref name="value"/> is not correct.
        /// </para>
        /// <para>
        /// OR
        /// </para>
        /// <para>
        /// The <paramref name="value"/> contains a non-base32 character.
        /// </para>
        /// </exception>
        public static byte[] FromBase32(ReadOnlySpan<char> value)
        {
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
                    throw new FormatException("The input is not a valid base32 string as its length is not correct.");
            }

            byte[] output = GC.AllocateUninitializedArray<byte>((int)((long)length * 5 / 8));

            FromBase32(value, output);

            return output;
        }

        private static void FromBase32(ReadOnlySpan<char> value, Span<byte> destination)
        {
            int buffer = 0;
            int bitsLeft = 0;
            int count = 0;

            for (int i = 0; i < value.Length; i++)
            {
                int ch = value[i];

                if (ch >= MC || ((ch = Base32Map[ch]) >= MC))
                {
                    throw new FormatException("The input is not a valid base32 string as it contains a non-base32 character.");
                }

                buffer = (buffer << 5) | ch;
                bitsLeft += 5;

                if (bitsLeft >= 8)
                {
                    destination[count++] = (byte)(buffer >> (bitsLeft - 8));
                    bitsLeft -= 8;
                }
            }
        }

        /// <summary>
        /// Converts the <paramref name="value"/> to lowercase base32 representation
        /// using the <c>0123456789abcdefghjkmnpqrstvwxyz</c> alphabet.
        /// </summary>
        /// <param name="value">
        /// The <see cref="Guid"/> to convert.
        /// </param>
        /// <returns>
        /// The string representation in base32 of the provided <paramref name="value"/>.
        /// The length of the returned string is 26 characters.
        /// </returns>
        public static string ToBase32Guid(Guid value)
        {
            Span<byte> guidBytes = stackalloc byte[16];

            WriteGuidBigEndian(value, guidBytes);

            return ToBase32(guidBytes);
        }

        /// <summary>
        /// Converts the base32 encoded <paramref name="value"/> to a <see cref="Guid"/> structure.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/> representation of the provided base32 span <paramref name="value"/>.
        /// </returns>
        /// <exception cref="FormatException">
        /// <para>
        /// The length of <paramref name="value"/> is not 26 characters.
        /// </para>
        /// <para>
        /// OR
        /// </para>
        /// <para>
        /// The <paramref name="value"/> contains a non-base32 character.
        /// </para>
        /// </exception>
        public static Guid FromBase32Guid(ReadOnlySpan<char> value)
        {
            if (value.Length != 26)
            {
                throw new FormatException("The input is not a valid base32 GUID as its length is not 26 characters.");
            }

            Span<byte> guidBytes = stackalloc byte[16];

            FromBase32(value, guidBytes);

            return ReadGuidBigEndian(guidBytes);
        }
    }
}