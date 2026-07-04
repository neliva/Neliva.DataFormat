// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Neliva.Tests
{
    [ExcludeFromCodeCoverage]
    public class DataFormatHexTests
    {
        [Fact]
        public void FullRangeRoundTripPass()
        {
            string hexStrLower =
                "000102030405060708090a0b0c0d0e0f" +
                "101112131415161718191a1b1c1d1e1f" +
                "202122232425262728292a2b2c2d2e2f" +
                "303132333435363738393a3b3c3d3e3f" +
                "404142434445464748494a4b4c4d4e4f" +
                "505152535455565758595a5b5c5d5e5f" +
                "606162636465666768696a6b6c6d6e6f" +
                "707172737475767778797a7b7c7d7e7f" +
                "808182838485868788898a8b8c8d8e8f" +
                "909192939495969798999a9b9c9d9e9f" +
                "a0a1a2a3a4a5a6a7a8a9aaabacadaeaf" +
                "b0b1b2b3b4b5b6b7b8b9babbbcbdbebf" +
                "c0c1c2c3c4c5c6c7c8c9cacbcccdcecf" +
                "d0d1d2d3d4d5d6d7d8d9dadbdcdddedf" +
                "e0e1e2e3e4e5e6e7e8e9eaebecedeeef" +
                "f0f1f2f3f4f5f6f7f8f9fafbfcfdfeff";

            string hexStrUpper = hexStrLower.ToUpperInvariant();

            Assert.True(DataFormat.IsHex(hexStrLower));
            Assert.True(DataFormat.IsHex(hexStrUpper));

            var hexLower = DataFormat.FromHex(hexStrLower);
            var hexUpper = DataFormat.FromHex(hexStrUpper);

            Assert.Equal(256, hexLower.Length);
            Assert.Equal(256, hexUpper.Length);

            for (int i = 0; i < 256; i++)
            {
                Assert.Equal(i, hexLower[i]);
                Assert.Equal(i, hexUpper[i]);
            }

            string hexResut = DataFormat.ToHex(hexLower);

            Assert.Equal(hexStrLower, hexResut);
        }

        [Fact]
        public void FromHexEmptyStringPass()
        {
            Assert.True(DataFormat.IsHex(string.Empty));

            var actual = DataFormat.FromHex(string.Empty);

            Assert.Equal(Array.Empty<byte>(), actual);
        }

        [Fact]
        public void FromHexNullStringPass()
        {
            Assert.True(DataFormat.IsHex((string)null));

            var actual = DataFormat.FromHex((string)null);

            Assert.Equal(Array.Empty<byte>(), actual);
        }

        [Fact]
        public void ToHexEmptyArrayPass()
        {
            var actual = DataFormat.ToHex(Array.Empty<byte>());

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        public void ToHexNullArrayPass()
        {
            var actual = DataFormat.ToHex(null);

            Assert.Equal(string.Empty, actual);
        }

        // 6E656C697661
        [Theory]
        [InlineData("6")]
        [InlineData("6E6")]
        [InlineData("6E656")]
        [InlineData("6E656C69766")]
        public void FromHexInvalidInputLengthFail(string invalidLengthHex)
        {
            Assert.False(DataFormat.IsHex(invalidLengthHex));

            var ex = Assert.Throws<FormatException>(() => DataFormat.FromHex(invalidLengthHex));
            Assert.Equal("The input is not a valid hex string as its length is not a multiple of 2.", ex.Message);
        }

        // Uppercase
        [Theory]
        [InlineData("  6E656C697661")]
        [InlineData("6E656C697661  ")]
        [InlineData("6E-656C69-7661")]
        [InlineData("6E656C697661\u0061\u0300")]
        [InlineData("\u0061\u03006E656C697661")]
        [InlineData("\u200b6E656C6976611")]
        [InlineData("0X")]
        [InlineData("0\u0308")]
        [InlineData("N0")]
        // Lowercase
        [InlineData("  6e656c697661")]
        [InlineData("6e656c697661  ")]
        [InlineData("6e-656c69-7661")]
        [InlineData("6e656c697661\u0061\u0300")]
        [InlineData("\u0061\u03006e656c697661")]
        [InlineData("\u200b6e656c6976611")]
        [InlineData("0x")]
        [InlineData("n0")]
        public void FromHexInvalidInputCharFail(string invalidCharInHex)
        {
            Assert.False(DataFormat.IsHex(invalidCharInHex));

            var ex = Assert.Throws<FormatException>(() => DataFormat.FromHex(invalidCharInHex));
            Assert.Equal("The input is not a valid hex string as it contains a non-hex character.", ex.Message);
        }

        // Boundary: char 127 must map to invalid via the table; char 128 (== MC) and above must
        // short-circuit on the ch >= MC guard before indexing the 128-entry map (no out-of-range read).
        [Theory]
        [InlineData("\u007f0")] // 127 at c1 -> table miss
        [InlineData("0\u007f")] // 127 at c2 -> table miss
        [InlineData("\u00800")] // 128 at c1 -> ch >= MC guard
        [InlineData("0\u0080")] // 128 at c2 -> ch >= MC guard
        [InlineData("\uffff0")] // max char at c1 -> ch >= MC guard
        [InlineData("0\uffff")] // max char at c2 -> ch >= MC guard
        public void FromHexCharMapBoundaryFails(string input)
        {
            Assert.False(DataFormat.IsHex(input));

            var ex = Assert.Throws<FormatException>(() => DataFormat.FromHex(input));
            Assert.Equal("The input is not a valid hex string as it contains a non-hex character.", ex.Message);
        }

        [Theory]
        [InlineData(int.MaxValue)]
        [InlineData(int.MaxValue / 2 + 1)]
        public unsafe void ToHexInputTooLargeFail(int inputSize)
        {
            Assert.Throws<ArgumentException>(() => DataFormat.ToHex(new ReadOnlySpan<byte>((void*)0, inputSize)));
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        [InlineData("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
        [InlineData("1C0FCF80-5B4E-4FC9-944A-7AA4549D7CF7")]
        public void ToHexGuidPasses(string guidStr)
        {
            var expected = Guid.Parse(guidStr);

            var guidHex = DataFormat.ToHexGuid(expected);

            Assert.Equal(expected.ToString("N"), guidHex);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        [InlineData("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        [InlineData("1C0FCF80-5B4E-4FC9-944A-7AA4549D7CF7")]
        [InlineData("1c0fcf80-5b4e-4fc9-944a-7aa4549d7cf7")]
        public void FromHexGuidPasses(string guidStr)
        {
            var expected = Guid.Parse(guidStr);

            var actual = DataFormat.FromHexGuid(guidStr.Replace("-", string.Empty));

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData("0000000000000000000000000000000")]
        [InlineData("000000000000000000000000000000000")]
        public void FromHexGuidInvalidInputLengthFail(string invalidLengthHex)
        {
            var ex = Assert.Throws<FormatException>(() => DataFormat.FromHexGuid(invalidLengthHex));
            Assert.Equal("The input is not a valid hex GUID as its length is not 32 characters.", ex.Message);
        }

        // Uppercase
        [Theory]
        [InlineData("000000000000N0000000000000000000")]
        [InlineData("000000000000000000000x0000000000")]
        [InlineData("00000000000000000z00000000000000")]
        [InlineData("00000000p00000000000000000000000")]
        public void FromHexGuidInvalidInputCharFail(string invalidCharInHex)
        {
            var ex = Assert.Throws<FormatException>(() => DataFormat.FromHexGuid(invalidCharInHex));
            Assert.Equal("The input is not a valid hex string as it contains a non-hex character.", ex.Message);
        }

        // --- Limits & bounds: mixed-case decoding and exhaustive round trips ---

        [Theory]
        [InlineData("0aB3Cd", new byte[] { 0x0A, 0xB3, 0xCD })]
        [InlineData("DeAdBeEf", new byte[] { 0xDE, 0xAD, 0xBE, 0xEF })]
        public void FromHexMixedCaseDecodesCorrectly(string mixed, byte[] expected)
        {
            Assert.True(DataFormat.IsHex(mixed));
            Assert.Equal(expected, DataFormat.FromHex(mixed));
        }

        [Fact]
        public void HexRoundTripAllLengthsPass()
        {
            for (int length = 0; length <= 64; length++)
            {
                var data = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    data[i] = (byte)((i * 31) + (length * 7));
                }

                string hex = DataFormat.ToHex(data);

                Assert.Equal(length * 2, hex.Length);
                Assert.True(DataFormat.IsHex(hex));
                Assert.Equal(data, DataFormat.FromHex(hex));
            }
        }

        [Fact]
        public void HexGuidRoundTripExhaustivePass()
        {
            for (int i = 0; i < 256; i++)
            {
                var bytes = new byte[16];
                for (int b = 0; b < 16; b++)
                {
                    bytes[b] = (byte)(i + (b * 17));
                }

                var expected = new Guid(bytes);

                string hex = DataFormat.ToHexGuid(expected);

                Assert.Equal(32, hex.Length);
                Assert.True(DataFormat.IsHex(hex));
                Assert.Equal(expected, DataFormat.FromHexGuid(hex));
            }
        }

        // IsHex and FromHex use separate validation logic; this guards the contract
        // that IsHex(value) is true exactly when FromHex(value) succeeds.
        [Theory]
        [InlineData("")]
        [InlineData("00")]
        [InlineData("6e656c697661")]
        [InlineData("abcdefABCDEF")]
        [InlineData("6")]
        [InlineData("000")]
        [InlineData("0g")]
        [InlineData("g0")]
        [InlineData(" 0")]
        [InlineData("0 ")]
        [InlineData("\u007f0")]
        [InlineData("0\u007f")]
        [InlineData("\u00800")]
        [InlineData("0\u0080")]
        [InlineData("\uffff0")]
        public void IsHexAgreesWithFromHexOutcome(string value)
        {
            bool isValid = DataFormat.IsHex(value);

            bool decodes;
            try
            {
                DataFormat.FromHex(value);
                decodes = true;
            }
            catch (FormatException)
            {
                decodes = false;
            }

            Assert.Equal(isValid, decodes);
        }
    }

    [ExcludeFromCodeCoverage]
    public class DataFormatBase32Tests
    {
        [Fact]
        public void FullRangeRoundTripPass()
        {
            string base32StrLower =
                "000g40r40m30e209185gr38e1w8124gk" +
                "2gahc5rr34d1p70x3rfj08924cj2a9h7" +
                "50mjmasc5mq2yc1h68sk8d9p6ww3jehv" +
                "7gykwft085146h258s3mgjaa9d64tkjf" +
                "a18n4mtmanb5ep2sb9dnrqaybxg62rk3" +
                "chjpcsv8d5n6pv3ddsqq0wbjedt7axkq" +
                "f1wqmyvwfnz7z041ga1r91c6gy48k2mb" +
                "hj6rx3wgj699754njtbsh6ctkee9v7mz" +
                "m2gt58x4mpkafa59nantsbdenyrb3cnk" +
                "pjtvddxrq6xbqf5xqtzw1ge2rf2cbhp7" +
                "s34wnjycsq7czm6htb9x9neptzcdkppv" +
                "vkexxqz0w7he7s75wvkyhtfaxfpevvqf" +
                "y3rz5wzmyqvffy7szbxzszfyzw";

            string base32StrUpper = base32StrLower.ToUpperInvariant();

            Assert.True(DataFormat.IsBase32(base32StrLower));
            Assert.True(DataFormat.IsBase32(base32StrUpper));

            var base32Lower = DataFormat.FromBase32(base32StrLower);
            var base32Upper = DataFormat.FromBase32(base32StrUpper);

            Assert.Equal(256, base32Lower.Length);
            Assert.Equal(256, base32Upper.Length);

            for (int i = 0; i < 256; i++)
            {
                Assert.Equal(i, base32Lower[i]);
                Assert.Equal(i, base32Upper[i]);
            }

            string base32Resut = DataFormat.ToBase32(base32Lower);

            Assert.Equal(base32StrLower, base32Resut);
        }

        [Fact]
        public void FromBase32EmptyStringPass()
        {
            Assert.True(DataFormat.IsBase32(string.Empty));

            var actual = DataFormat.FromBase32(string.Empty);

            Assert.Equal(Array.Empty<byte>(), actual);
        }

        [Fact]
        public void FromBase32NullStringPass()
        {
            Assert.True(DataFormat.IsBase32((string)null));

            var actual = DataFormat.FromBase32((string)null);

            Assert.Equal(Array.Empty<byte>(), actual);
        }

        [Fact]
        public void ToBase32EmptyArrayPass()
        {
            var actual = DataFormat.ToBase32(Array.Empty<byte>());

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        public void ToBase32NullArrayPass()
        {
            var actual = DataFormat.ToBase32(null);

            Assert.Equal(string.Empty, actual);
        }

        [Theory]
        [InlineData(new byte[] { 0x66 }, "cr")]
        [InlineData(new byte[] { 0x66, 0x6F }, "csqg")]
        [InlineData(new byte[] { 0x66, 0x6F, 0x6F }, "csqpy")]
        [InlineData(new byte[] { 0x66, 0x6F, 0x6F, 0x62 }, "csqpyrg")]
        [InlineData(new byte[] { 0x66, 0x6F, 0x6F, 0x62, 0x61 }, "csqpyrk1")]
        [InlineData(new byte[] { 0x66, 0x6F, 0x6F, 0x62, 0x61, 0x72 }, "csqpyrk1e8")]
        public void Base32KnownTextVectorsPass(byte[] value, string expected)
        {
            string actual = DataFormat.ToBase32(value);

            Assert.Equal(expected, actual);
            Assert.True(DataFormat.IsBase32(actual));
            Assert.Equal(value, DataFormat.FromBase32(actual));
            Assert.Equal(value, DataFormat.FromBase32(actual.ToUpperInvariant()));
        }

        // DSJPRTBPC4
        [Theory]
        [InlineData("D")]
        [InlineData("DSJ")]
        [InlineData("DSJPRT")]
        [InlineData("DSJPRTBPC")]
        [InlineData("DSJPRTBPC40")]
        public void FromBase32InvalidInputLengthFail(string invalidLengthBase32)
        {
            Assert.False(DataFormat.IsBase32(invalidLengthBase32));

            var ex = Assert.Throws<FormatException>(() => DataFormat.FromBase32(invalidLengthBase32));
            Assert.Equal("The input is not a valid base32 string as its length is not correct.", ex.Message);
        }

        // Uppercase
        [Theory]
        [InlineData(" DSJPRTBPC")]
        [InlineData("SJPRTBPC4 ")]
        [InlineData("DS-PRTBPC4")]
        [InlineData("DSJPRT\u0061\u0300C4")]
        [InlineData("\u0061\u0300JPRTBPC4")]
        [InlineData("D\u200bJPRTBPC4")]
        [InlineData("LA")]
        [InlineData("AO")]
        [InlineData("AI")]
        [InlineData("AU")]
        [InlineData("DSJPRTBLC4")]
        [InlineData("DSJPOTBPC4")]
        [InlineData("DSJIRTBPC4")]
        [InlineData("DSJPRTBPCU")]
        [InlineData("D\u0308JPRTBPC4")]
        // Lowercase
        [InlineData(" dsjprtbpc")]
        [InlineData("sjprtbpc4 ")]
        [InlineData("ds-prtbpc4")]
        [InlineData("dsjprt\u0061\u0300c4")]
        [InlineData("\u0061\u0300jprtbpc4")]
        [InlineData("d\u200bjprtbpc4")]
        [InlineData("la")]
        [InlineData("ao")]
        [InlineData("ai")]
        [InlineData("au")]
        [InlineData("dsjprtblc4")]
        [InlineData("dsjpotbpc4")]
        [InlineData("dsjirtbpc4")]
        [InlineData("dsjprtbpcu")]
        [InlineData("d\u0308jprtbpc4")]
        public void FromBase32InvalidInputCharFail(string invalidCharInBase32)
        {
            Assert.False(DataFormat.IsBase32(invalidCharInBase32));

            var ex = Assert.Throws<FormatException>(() => DataFormat.FromBase32(invalidCharInBase32));
            Assert.Equal("The input is not a valid base32 string as it contains a non-base32 character.", ex.Message);
        }

        // Boundary: char 127 must map to invalid via the table; char 128 (== MC) and above must
        // short-circuit on the ch >= MC guard before indexing the 128-entry map (no out-of-range read).
        [Theory]
        [InlineData("\u007f0")] // 127 at first char -> table miss
        [InlineData("0\u007f")] // 127 at second char -> table miss
        [InlineData("\u00800")] // 128 at first char -> ch >= MC guard
        [InlineData("0\u0080")] // 128 at second char -> ch >= MC guard
        [InlineData("\uffff0")] // max char at first char -> ch >= MC guard
        [InlineData("0\uffff")] // max char at second char -> ch >= MC guard
        public void FromBase32CharMapBoundaryFails(string input)
        {
            Assert.False(DataFormat.IsBase32(input));

            var ex = Assert.Throws<FormatException>(() => DataFormat.FromBase32(input));
            Assert.Equal("The input is not a valid base32 string as it contains a non-base32 character.", ex.Message);
        }

        [Theory]
        [InlineData(int.MaxValue)]
        [InlineData(((int)(((long)int.MaxValue * 5) / 8)) + 1)]
        public unsafe void ToBase32InputTooLargeFail(int inputSize)
        {
            Assert.Throws<ArgumentException>(() => DataFormat.ToBase32(new ReadOnlySpan<byte>((void*)0, inputSize)));
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", "00000000000000000000000000")]
        [InlineData("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", "zzzzzzzzzzzzzzzzzzzzzzzzzw")]
        [InlineData("1C0FCF80-5B4E-4FC9-944A-7AA4549D7CF7", "3g7wz02v9s7wk52afaj597bwyw")]
        public void ToBase32GuidPasses(string guidStr, string base32Guid)
        {
            var expected = Guid.Parse(guidStr);

            var actual = DataFormat.ToBase32Guid(expected);

            Assert.Equal(base32Guid, actual);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", "00000000000000000000000000")]
        [InlineData("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", "zzzzzzzzzzzzzzzzzzzzzzzzzw")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff", "zzzzzzzzzzzzzzzzzzzzzzzzzw")]
        [InlineData("1C0FCF80-5B4E-4FC9-944A-7AA4549D7CF7", "3g7wz02v9s7wk52afaj597bwyw")]
        [InlineData("1c0fcf80-5b4e-4fc9-944a-7aa4549d7cf7", "3g7wz02v9s7wk52afaj597bwyw")]
        public void FromBase32GuidPasses(string guidStr, string base32Guid)
        {
            var expected = Guid.Parse(guidStr);

            var actual = DataFormat.FromBase32Guid(base32Guid);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData("zzzzzzzzzzzzzzzzzzzzzzzzz")]
        [InlineData("zzzzzzzzzzzzzzzzzzzzzzzzzww")]
        public void FromBase32GuidInvalidInputLengthFail(string invalidLengthBase32)
        {
            var ex = Assert.Throws<FormatException>(() => DataFormat.FromBase32Guid(invalidLengthBase32));
            Assert.Equal("The input is not a valid base32 GUID as its length is not 26 characters.", ex.Message);
        }

        [Theory]
        [InlineData("zzzzzzzzzzzzzzzzzzuzzzzzzw")]
        [InlineData("zzzzzzzzzzzzzzlzzzzzzzzzzw")]
        [InlineData("zzzzzzzozzzzzzzzzzzzzzzzzw")]
        [InlineData("zzzzzzzzzzzzzzzzz zzzzzzzw")]
        public void FromBase32GuidInvalidInputCharFail(string invalidCharInBase32)
        {
            var ex = Assert.Throws<FormatException>(() => DataFormat.FromBase32Guid(invalidCharInBase32));
            Assert.Equal("The input is not a valid base32 string as it contains a non-base32 character.", ex.Message);
        }

        // Covers ToBase32/FromBase32 across all (length % 5) input residues so every
        // padding / bitsLeft branch in the encoder and decoder is exercised.
        [Theory]
        [InlineData(1)]   // -> 2 chars
        [InlineData(2)]   // -> 4 chars
        [InlineData(3)]   // -> 5 chars
        [InlineData(4)]   // -> 7 chars
        [InlineData(5)]   // -> 8 chars
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(17)]
        public void Base32RoundTripVariableLengthsPass(int length)
        {
            var data = new byte[length];
            for (int i = 0; i < length; i++)
            {
                data[i] = (byte)(i * 7 + 1);
            }

            string encoded = DataFormat.ToBase32(data);

            Assert.True(DataFormat.IsBase32(encoded));

            var decoded = DataFormat.FromBase32(encoded);

            Assert.Equal(data, decoded);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(7)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(33)]
        public void HexRoundTripVariableLengthsPass(int length)
        {
            var data = new byte[length];
            for (int i = 0; i < length; i++)
            {
                data[i] = (byte)(i * 11 + 3);
            }

            string encoded = DataFormat.ToHex(data);

            Assert.True(DataFormat.IsHex(encoded));
            Assert.Equal(length * 2, encoded.Length);

            var decoded = DataFormat.FromHex(encoded);

            Assert.Equal(data, decoded);
        }

        // Explicitly verifies IsBase32 returns true for every valid (length % 8) residue.
        [Theory]
        [InlineData("")]                  // 0
        [InlineData("00")]                // 2
        [InlineData("0000")]              // 4
        [InlineData("00000")]             // 5
        [InlineData("0000000")]           // 7
        [InlineData("00000000")]          // 8 (0)
        [InlineData("0000000000")]        // 10 (2)
        public void IsBase32ValidLengthsReturnsTrue(string value)
        {
            Assert.True(DataFormat.IsBase32(value));
        }

        [Theory]
        [InlineData("00")]
        [InlineData("abcdef")]
        [InlineData("0123456789abcdefABCDEF")]
        public void IsHexValidReturnsTrue(string value)
        {
            Assert.True(DataFormat.IsHex(value));
        }

        // --- Limits & bounds: mixed/upper case, non-canonical trailing bits, exhaustive round trips ---

        [Theory]
        [InlineData("3G7wZ02V9s7Wk52AfAj597BwYw", "3g7wz02v9s7wk52afaj597bwyw")]
        [InlineData("0123456789ABCDEFGHJKMNPQRSTVWXYZ", "0123456789abcdefghjkmnpqrstvwxyz")]
        public void FromBase32MixedCaseDecodesAsLowercase(string mixed, string lower)
        {
            Assert.True(DataFormat.IsBase32(mixed));
            Assert.Equal(DataFormat.FromBase32(lower), DataFormat.FromBase32(mixed));
        }

        // The encoder always emits zero trailing bits; the decoder rejects any input whose
        // final character carries non-zero padding bits (non-canonical encoding).
        [Theory]
        [InlineData("10", new byte[] { 0x08 })]
        [InlineData("zw", new byte[] { 0xFF })]
        [InlineData("0g", new byte[] { 0x04 })]
        [InlineData("0000g", new byte[] { 0x00, 0x00, 0x08 })]
        public void FromBase32CanonicalTrailingBitsDecodes(string input, byte[] expected)
        {
            Assert.True(DataFormat.IsBase32(input));

            var decoded = DataFormat.FromBase32(input);

            Assert.Equal(expected, decoded);
            Assert.Equal(input, DataFormat.ToBase32(decoded));
        }

        [Theory]
        [InlineData("11")]
        [InlineData("12")]
        [InlineData("13")]
        [InlineData("zz")]
        [InlineData("zx")]
        [InlineData("zy")]
        [InlineData("0001")]
        [InlineData("000h")]
        [InlineData("00001")]
        [InlineData("0000001")]
        public void FromBase32NonZeroTrailingBitsThrows(string input)
        {
            Assert.False(DataFormat.IsBase32(input));

            var ex = Assert.Throws<FormatException>(() => DataFormat.FromBase32(input));
            Assert.Equal("The input is not a valid base32 string as it contains non-zero trailing bits.", ex.Message);
        }

        // Exhaustive: for every length that has trailing bits, sweep all 32 possible final
        // characters and assert only canonical (zero low-bit) endings decode; the rest throw.
        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(10)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(15)]
        public void FromBase32TrailingBitsAllFinalCharVariations(int length)
        {
            const string alphabet = "0123456789abcdefghjkmnpqrstvwxyz";
            int trailingBits = (5 * (length % 8)) % 8;
            int mask = (1 << trailingBits) - 1;

            string prefix = new string('0', length - 1);

            for (int v = 0; v < 32; v++)
            {
                string s = prefix + alphabet[v];
                bool canonical = (v & mask) == 0;

                Assert.Equal(canonical, DataFormat.IsBase32(s));

                if (canonical)
                {
                    var decoded = DataFormat.FromBase32(s);
                    Assert.Equal(s, DataFormat.ToBase32(decoded));
                }
                else
                {
                    var ex = Assert.Throws<FormatException>(() => DataFormat.FromBase32(s));
                    Assert.Equal("The input is not a valid base32 string as it contains non-zero trailing bits.", ex.Message);
                }
            }
        }

        [Theory]
        [InlineData("3G7WZ02V9S7WK52AFAJ597BWYW", "1c0fcf80-5b4e-4fc9-944a-7aa4549d7cf7")]
        [InlineData("ZZZZZZZZZZZZZZZZZZZZZZZZZW", "ffffffff-ffff-ffff-ffff-ffffffffffff")]
        public void FromBase32GuidUppercaseDecodesPass(string upperBase32Guid, string guidStr)
        {
            var expected = Guid.Parse(guidStr);

            Assert.Equal(expected, DataFormat.FromBase32Guid(upperBase32Guid));
        }

        [Fact]
        public void FromBase32GuidCanonicalTrailingBitsDecodes()
        {
            var allOnes = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

            string canonical = new string('z', 25) + "w";

            Assert.True(DataFormat.IsBase32(canonical));
            Assert.Equal(allOnes, DataFormat.FromBase32Guid(canonical));
        }

        // A 26-character base32 GUID has 2 trailing bits, so any final character whose
        // low 2 bits are set is non-canonical and must be rejected.
        [Theory]
        [InlineData('z')]
        [InlineData('x')]
        [InlineData('y')]
        public void FromBase32GuidNonZeroTrailingBitsThrows(char lastChar)
        {
            string nonCanonical = new string('z', 25) + lastChar;

            Assert.False(DataFormat.IsBase32(nonCanonical));

            var ex = Assert.Throws<FormatException>(() => DataFormat.FromBase32Guid(nonCanonical));
            Assert.Equal("The input is not a valid base32 string as it contains non-zero trailing bits.", ex.Message);
        }

        [Fact]
        public void Base32RoundTripAllLengthsPass()
        {
            for (int length = 0; length <= 64; length++)
            {
                var data = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    data[i] = (byte)((i * 31) + (length * 7));
                }

                string b32 = DataFormat.ToBase32(data);

                Assert.True(DataFormat.IsBase32(b32));
                Assert.Equal(data, DataFormat.FromBase32(b32));
            }
        }

        [Fact]
        public void Base32GuidRoundTripExhaustivePass()
        {
            for (int i = 0; i < 256; i++)
            {
                var bytes = new byte[16];
                for (int b = 0; b < 16; b++)
                {
                    bytes[b] = (byte)(i + (b * 17));
                }

                var expected = new Guid(bytes);

                string b32 = DataFormat.ToBase32Guid(expected);

                Assert.Equal(26, b32.Length);
                Assert.True(DataFormat.IsBase32(b32));
                Assert.Equal(expected, DataFormat.FromBase32Guid(b32));
            }
        }

        // IsBase32 and FromBase32 use separate validation logic; this guards the contract
        // that IsBase32(value) is true exactly when FromBase32(value) succeeds.
        [Theory]
        [InlineData("")]
        [InlineData("00")]
        [InlineData("00000")]
        [InlineData("dsjprtbpc4")]
        [InlineData("0123456789abcdefghjkmnpqrstvwxyz")]
        [InlineData("D")]
        [InlineData("DSJ")]
        [InlineData("DSJPRT")]
        [InlineData("ai")]
        [InlineData("al")]
        [InlineData("ao")]
        [InlineData("au")]
        [InlineData(" 0")]
        [InlineData("11")]
        [InlineData("zz")]
        [InlineData("0001")]
        [InlineData("\u00800")]
        [InlineData("\uffff0")]
        public void IsBase32AgreesWithFromBase32Outcome(string value)
        {
            bool isValid = DataFormat.IsBase32(value);

            bool decodes;
            try
            {
                DataFormat.FromBase32(value);
                decodes = true;
            }
            catch (FormatException)
            {
                decodes = false;
            }

            Assert.Equal(isValid, decodes);
        }
    }

    [ExcludeFromCodeCoverage]
    public class DataFormatSpanTests
    {
        // ---- ToHex(ReadOnlySpan<byte>, Span<char>) ----

        [Fact]
        public void ToHexSpanEmptyInputWritesNothingReturnsZero()
        {
            Span<char> destination = stackalloc char[4];
            destination.Fill('#');

            int written = DataFormat.ToHex(ReadOnlySpan<byte>.Empty, destination);

            Assert.Equal(0, written);
            Assert.Equal("####", new string(destination));
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, "00")]
        [InlineData(new byte[] { 0x0A, 0xB3, 0xCD }, "0ab3cd")]
        [InlineData(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }, "deadbeef")]
        public void ToHexSpanWritesExpectedAndReturnsCount(byte[] value, string expected)
        {
            Span<char> destination = stackalloc char[expected.Length];

            int written = DataFormat.ToHex(value, destination);

            Assert.Equal(expected.Length, written);
            Assert.Equal(expected, new string(destination));
            Assert.Equal(DataFormat.ToHex(value), new string(destination));
        }

        [Fact]
        public void ToHexSpanOversizedDestinationLeavesTailUntouched()
        {
            var value = new byte[] { 0x0A, 0xB3, 0xCD };

            Span<char> destination = stackalloc char[10];
            destination.Fill('#');

            int written = DataFormat.ToHex(value, destination);

            Assert.Equal(6, written);
            Assert.Equal("0ab3cd", new string(destination.Slice(0, written)));
            Assert.Equal("####", new string(destination.Slice(written)));
        }

        [Fact]
        public void ToHexSpanDestinationTooSmallThrows()
        {
            var value = new byte[] { 0x01, 0x02 };
            var destination = new char[3];

            var ex = Assert.Throws<ArgumentException>(() => DataFormat.ToHex(value, destination));
            Assert.Equal("destination", ex.ParamName);
        }

        [Fact]
        public unsafe void ToHexSpanInputTooLargeThrows()
        {
            var ex = Assert.Throws<ArgumentException>(() => DataFormat.ToHex(new ReadOnlySpan<byte>((void*)0, int.MaxValue), Span<char>.Empty));
            Assert.Equal("value", ex.ParamName);
        }

        // ---- ToBase32(ReadOnlySpan<byte>, Span<char>) ----

        [Fact]
        public void ToBase32SpanEmptyInputWritesNothingReturnsZero()
        {
            Span<char> destination = stackalloc char[4];
            destination.Fill('#');

            int written = DataFormat.ToBase32(ReadOnlySpan<byte>.Empty, destination);

            Assert.Equal(0, written);
            Assert.Equal("####", new string(destination));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(16)]
        [InlineData(17)]
        public void ToBase32SpanMatchesStringOverload(int length)
        {
            var value = new byte[length];
            for (int i = 0; i < length; i++)
            {
                value[i] = (byte)((i * 7) + 1);
            }

            string expected = DataFormat.ToBase32(value);

            Span<char> destination = stackalloc char[expected.Length];

            int written = DataFormat.ToBase32(value, destination);

            Assert.Equal(expected.Length, written);
            Assert.Equal(expected, new string(destination));
        }

        [Fact]
        public void ToBase32SpanOversizedDestinationLeavesTailUntouched()
        {
            var value = new byte[] { 0x66, 0x6F, 0x6F };
            string expected = DataFormat.ToBase32(value);

            Span<char> destination = stackalloc char[10];
            destination.Fill('#');

            int written = DataFormat.ToBase32(value, destination);

            Assert.Equal(expected.Length, written);
            Assert.Equal(expected, new string(destination.Slice(0, written)));
            Assert.Equal(new string('#', destination.Length - written), new string(destination.Slice(written)));
        }

        [Fact]
        public void ToBase32SpanDestinationTooSmallThrows()
        {
            var value = new byte[] { 0x66, 0x6F, 0x6F };
            var destination = new char[4];

            var ex = Assert.Throws<ArgumentException>(() => DataFormat.ToBase32(value, destination));
            Assert.Equal("destination", ex.ParamName);
        }

        [Fact]
        public unsafe void ToBase32SpanInputTooLargeThrows()
        {
            int tooLarge = (int)(((long)int.MaxValue * 5) / 8) + 1;

            var ex = Assert.Throws<ArgumentException>(() => DataFormat.ToBase32(new ReadOnlySpan<byte>((void*)0, tooLarge), Span<char>.Empty));
            Assert.Equal("value", ex.ParamName);
        }

        // ---- FromHex(ReadOnlySpan<char>, Span<byte>) ----

        [Fact]
        public void FromHexSpanEmptyInputWritesNothingReturnsZero()
        {
            Span<byte> destination = stackalloc byte[4];

            int written = DataFormat.FromHex(ReadOnlySpan<char>.Empty, destination);

            Assert.Equal(0, written);
        }

        [Theory]
        [InlineData("00", new byte[] { 0x00 })]
        [InlineData("0aB3Cd", new byte[] { 0x0A, 0xB3, 0xCD })]
        [InlineData("deadbeef", new byte[] { 0xDE, 0xAD, 0xBE, 0xEF })]
        public void FromHexSpanWritesExpectedAndReturnsCount(string value, byte[] expected)
        {
            Span<byte> destination = stackalloc byte[expected.Length];

            int written = DataFormat.FromHex(value, destination);

            Assert.Equal(expected.Length, written);
            Assert.Equal(expected, destination.ToArray());
        }

        [Fact]
        public void FromHexSpanOversizedDestinationLeavesTailUntouched()
        {
            Span<byte> destination = stackalloc byte[8];
            destination.Fill(0xEE);

            int written = DataFormat.FromHex("deadbeef", destination);

            Assert.Equal(4, written);
            Assert.Equal(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }, destination.Slice(0, written).ToArray());
            Assert.Equal(new byte[] { 0xEE, 0xEE, 0xEE, 0xEE }, destination.Slice(written).ToArray());
        }

        [Fact]
        public void FromHexSpanDestinationTooSmallThrows()
        {
            var destination = new byte[1];

            var ex = Assert.Throws<ArgumentException>(() => DataFormat.FromHex("deadbeef", destination));
            Assert.Equal("destination", ex.ParamName);
        }

        [Fact]
        public void FromHexSpanOddLengthThrows()
        {
            var destination = new byte[4];

            Assert.Throws<FormatException>(() => DataFormat.FromHex("abc", destination));
        }

        [Fact]
        public void FromHexSpanInvalidCharThrows()
        {
            var destination = new byte[1];

            Assert.Throws<FormatException>(() => DataFormat.FromHex("0g", destination));
        }

        // ---- FromBase32(ReadOnlySpan<char>, Span<byte>) ----

        [Fact]
        public void FromBase32SpanEmptyInputWritesNothingReturnsZero()
        {
            Span<byte> destination = stackalloc byte[4];

            int written = DataFormat.FromBase32(ReadOnlySpan<char>.Empty, destination);

            Assert.Equal(0, written);
        }

        [Theory]
        [InlineData("cr", new byte[] { 0x66 })]
        [InlineData("00000", new byte[] { 0x00, 0x00, 0x00 })]
        public void FromBase32SpanWritesExpectedAndReturnsCount(string value, byte[] expected)
        {
            Span<byte> destination = stackalloc byte[expected.Length];

            int written = DataFormat.FromBase32(value, destination);

            Assert.Equal(expected.Length, written);
            Assert.Equal(expected, destination.ToArray());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(7)]
        [InlineData(16)]
        [InlineData(17)]
        public void FromBase32SpanMatchesByteArrayOverload(int length)
        {
            var data = new byte[length];
            for (int i = 0; i < length; i++)
            {
                data[i] = (byte)((i * 7) + 1);
            }

            string encoded = DataFormat.ToBase32(data);

            Span<byte> destination = stackalloc byte[length];

            int written = DataFormat.FromBase32(encoded, destination);

            Assert.Equal(length, written);
            Assert.Equal(data, destination.ToArray());
        }

        [Fact]
        public void FromBase32SpanOversizedDestinationLeavesTailUntouched()
        {
            Span<byte> destination = stackalloc byte[8];
            destination.Fill(0xEE);

            int written = DataFormat.FromBase32("00000", destination);

            Assert.Equal(3, written);
            Assert.Equal(new byte[] { 0x00, 0x00, 0x00 }, destination.Slice(0, written).ToArray());
            Assert.Equal(new byte[] { 0xEE, 0xEE, 0xEE, 0xEE, 0xEE }, destination.Slice(written).ToArray());
        }

        [Fact]
        public void FromBase32SpanDestinationTooSmallThrows()
        {
            var destination = new byte[2];

            var ex = Assert.Throws<ArgumentException>(() => DataFormat.FromBase32("00000", destination));
            Assert.Equal("destination", ex.ParamName);
        }

        [Theory]
        [InlineData("D")]
        [InlineData("DSJ")]
        [InlineData("DSJPRT")]
        public void FromBase32SpanInvalidLengthThrows(string value)
        {
            var destination = new byte[16];

            Assert.Throws<FormatException>(() => DataFormat.FromBase32(value, destination));
        }

        [Fact]
        public void FromBase32SpanInvalidCharThrows()
        {
            var destination = new byte[1];

            Assert.Throws<FormatException>(() => DataFormat.FromBase32("ai", destination));
        }

        [Fact]
        public void FromBase32SpanNonZeroTrailingBitsThrows()
        {
            var destination = new byte[1];

            Assert.Throws<FormatException>(() => DataFormat.FromBase32("11", destination));
        }
    }
}
