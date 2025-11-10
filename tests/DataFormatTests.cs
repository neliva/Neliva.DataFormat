// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neliva.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DataFormatHexTests
    {
        [TestMethod]
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

            Assert.IsTrue(DataFormat.IsHex(hexStrLower));
            Assert.IsTrue(DataFormat.IsHex(hexStrUpper));

            var hexLower = DataFormat.FromHex(hexStrLower);
            var hexUpper = DataFormat.FromHex(hexStrUpper);

            Assert.AreEqual(256, hexLower.Length);
            Assert.AreEqual(256, hexUpper.Length);

            for (int i = 0; i < 256; i++)
            {
                Assert.AreEqual(i, hexLower[i], "lower");
                Assert.AreEqual(i, hexUpper[i], "upper");
            }

            string hexResut = DataFormat.ToHex(hexLower);

            Assert.AreEqual(hexStrLower, hexResut);
        }

        [TestMethod]
        public void FromHexEmptyStringPass()
        {
            Assert.IsTrue(DataFormat.IsHex(string.Empty));

            var actual = DataFormat.FromHex(string.Empty);

            Assert.AreEqual(Array.Empty<byte>(), actual);
        }

        [TestMethod]
        public void FromHexNullStringPass()
        {
            Assert.IsTrue(DataFormat.IsHex((string)null));

            var actual = DataFormat.FromHex((string)null);

            Assert.AreEqual(Array.Empty<byte>(), actual);
        }

        [TestMethod]
        public void ToHexEmptyArrayPass()
        {
            var actual = DataFormat.ToHex(Array.Empty<byte>());

            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void ToHexNullArrayPass()
        {
            var actual = DataFormat.ToHex(null);

            Assert.AreEqual(string.Empty, actual);
        }

        // 6E656C697661
        [TestMethod]
        [DataRow("6")]
        [DataRow("6E6")]
        [DataRow("6E656")]
        [DataRow("6E656C69766")]
        public void FromHexInvalidInputLengthFail(string invalidLengthHex)
        {
            Assert.IsFalse(DataFormat.IsHex(invalidLengthHex));

            var ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromHex(invalidLengthHex));
            Assert.AreEqual("The input is not a valid hex string as its length is not a multiple of 2.", ex.Message);
        }

        // Uppercase
        [TestMethod]
        [DataRow("  6E656C697661")]
        [DataRow("6E656C697661  ")]
        [DataRow("6E-656C69-7661")]
        [DataRow("6E656C697661\u0061\u0300")]
        [DataRow("\u0061\u03006E656C697661")]
        [DataRow("\u200b6E656C6976611")]
        [DataRow("0X")]
        [DataRow("0\u0308")]
        [DataRow("N0")]
        // Lowercase
        [DataRow("  6e656c697661")]
        [DataRow("6e656c697661  ")]
        [DataRow("6e-656c69-7661")]
        [DataRow("6e656c697661\u0061\u0300")]
        [DataRow("\u0061\u03006e656c697661")]
        [DataRow("\u200b6e656c6976611")]
        [DataRow("0x")]
        [DataRow("0\u0308")]
        [DataRow("n0")]
        public void FromHexInvalidInputCharFail(string invalidCharInHex)
        {
            Assert.IsFalse(DataFormat.IsHex(invalidCharInHex));

            var ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromHex(invalidCharInHex));
            Assert.AreEqual("The input is not a valid hex string as it contains a non-hex character.", ex.Message);
        }

        [TestMethod]
        [DataRow(int.MaxValue)]
        [DataRow(int.MaxValue / 2 + 1)]
        public unsafe void ToHexInputTooLargeFail(int inputSize)
        {
            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(() => DataFormat.ToHex(new ReadOnlySpan<byte>((void*)0, inputSize)));
        }

        [TestMethod]
        [DataRow("00000000-0000-0000-0000-000000000000")]
        [DataRow("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
        [DataRow("1C0FCF80-5B4E-4FC9-944A-7AA4549D7CF7")]
        public void ToHexGuidPasses(string guidStr)
        {
            var expected = Guid.Parse(guidStr);

            var guidHex = DataFormat.ToHexGuid(expected);

            Assert.AreEqual(expected.ToString("N"), guidHex);
        }

        [TestMethod]
        [DataRow("00000000-0000-0000-0000-000000000000")]
        [DataRow("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
        [DataRow("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        [DataRow("1C0FCF80-5B4E-4FC9-944A-7AA4549D7CF7")]
        [DataRow("1c0fcf80-5b4e-4fc9-944a-7aa4549d7cf7")]
        public void FromHexGuidPasses(string guidStr)
        {
            var expected = Guid.Parse(guidStr);

            var actual = DataFormat.FromHexGuid(guidStr.Replace("-", string.Empty));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("0000000000000000000000000000000")]
        [DataRow("000000000000000000000000000000000")]
        public void FromHexGuidInvalidInputLengthFail(string invalidLengthHex)
        {
            var ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromHexGuid(invalidLengthHex));
            Assert.AreEqual("The input is not a valid hex GUID as its length is not 32 characters.", ex.Message);
        }

        // Uppercase
        [TestMethod]
        [DataRow("000000000000N0000000000000000000")]
        [DataRow("000000000000000000000x0000000000")]
        [DataRow("00000000000000000z00000000000000")]
        [DataRow("00000000p00000000000000000000000")]
        public void FromHexGuidInvalidInputCharFail(string invalidCharInHex)
        {
            var ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromHexGuid(invalidCharInHex));
            Assert.AreEqual("The input is not a valid hex string as it contains a non-hex character.", ex.Message);
        }
    }

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DataFormatBase32Tests
    {
        [TestMethod]
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

            Assert.IsTrue(DataFormat.IsBase32(base32StrLower));
            Assert.IsTrue(DataFormat.IsBase32(base32StrUpper));

            var base32Lower = DataFormat.FromBase32(base32StrLower);
            var base32Upper = DataFormat.FromBase32(base32StrUpper);

            Assert.AreEqual(256, base32Lower.Length);
            Assert.AreEqual(256, base32Upper.Length);

            for (int i = 0; i < 256; i++)
            {
                Assert.AreEqual(i, base32Lower[i], "lower");
                Assert.AreEqual(i, base32Upper[i], "upper");
            }

            string base32Resut = DataFormat.ToBase32(base32Lower);

            Assert.AreEqual(base32StrLower, base32Resut);
        }

        [TestMethod]
        public void FromBase32EmptyStringPass()
        {
            Assert.IsTrue(DataFormat.IsBase32(string.Empty));

            var actual = DataFormat.FromBase32(string.Empty);

            Assert.AreEqual(Array.Empty<byte>(), actual);
        }

        [TestMethod]
        public void FromBase32NullStringPass()
        {
            Assert.IsTrue(DataFormat.IsBase32((string)null));

            var actual = DataFormat.FromBase32((string)null);

            Assert.AreEqual(Array.Empty<byte>(), actual);
        }

        [TestMethod]
        public void ToBase32EmptyArrayPass()
        {
            var actual = DataFormat.ToBase32(Array.Empty<byte>());

            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void ToBase32NullArrayPass()
        {
            var actual = DataFormat.ToBase32(null);

            Assert.AreEqual(string.Empty, actual);
        }

        // DSJPRTBPC4
        [TestMethod]
        [DataRow("D")]
        [DataRow("DSJ")]
        [DataRow("DSJPRT")]
        [DataRow("DSJPRTBPC")]
        [DataRow("DSJPRTBPC40")]
        public void FromBase32InvalidInputLengthFail(string invalidLengthBase32)
        {
            Assert.IsFalse(DataFormat.IsBase32(invalidLengthBase32));

            var ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromBase32(invalidLengthBase32));
            Assert.AreEqual("The input is not a valid base32 string as its length is not correct.", ex.Message);
        }

        // Uppercase
        [TestMethod]
        [DataRow(" DSJPRTBPC")]
        [DataRow("SJPRTBPC4 ")]
        [DataRow("DS-PRTBPC4")]
        [DataRow("DSJPRT\u0061\u0300C4")]
        [DataRow("\u0061\u0300JPRTBPC4")]
        [DataRow("D\u200bJPRTBPC4")]
        [DataRow("LA")]
        [DataRow("AO")]
        [DataRow("AI")]
        [DataRow("AU")]
        [DataRow("DSJPRTBLC4")]
        [DataRow("DSJPOTBPC4")]
        [DataRow("DSJIRTBPC4")]
        [DataRow("DSJPRTBPCU")]
        [DataRow("D\u0308JPRTBPC4")]
        // Lowercase
        [DataRow(" dsjprtbpc")]
        [DataRow("sjprtbpc4 ")]
        [DataRow("ds-prtbpc4")]
        [DataRow("dsjprt\u0061\u0300c4")]
        [DataRow("\u0061\u0300jprtbpc4")]
        [DataRow("d\u200bjprtbpc4")]
        [DataRow("la")]
        [DataRow("ao")]
        [DataRow("ai")]
        [DataRow("au")]
        [DataRow("dsjprtblc4")]
        [DataRow("dsjpotbpc4")]
        [DataRow("dsjirtbpc4")]
        [DataRow("dsjprtbpcu")]
        [DataRow("d\u0308jprtbpc4")]
        public void FromBase32InvalidInputCharFail(string invalidCharInBase32)
        {
            Assert.IsFalse(DataFormat.IsBase32(invalidCharInBase32));

            var ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromBase32(invalidCharInBase32));
            Assert.AreEqual("The input is not a valid base32 string as it contains a non-base32 character.", ex.Message);
        }

        [TestMethod]
        [DataRow(int.MaxValue)]
        [DataRow(((int)(((long)int.MaxValue * 5) / 8)) + 1)]
        public unsafe void ToBase32InputTooLargeFail(int inputSize)
        {
            var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(() => DataFormat.ToBase32(new ReadOnlySpan<byte>((void*)0, inputSize)));
        }

        [TestMethod]
        [DataRow("00000000-0000-0000-0000-000000000000", "00000000000000000000000000")]
        [DataRow("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", "zzzzzzzzzzzzzzzzzzzzzzzzzw")]
        [DataRow("1C0FCF80-5B4E-4FC9-944A-7AA4549D7CF7", "3g7wz02v9s7wk52afaj597bwyw")]
        public void ToBase32GuidPasses(string guidStr, string base32Guid)
        {
            var expected = Guid.Parse(guidStr);

            var actual = DataFormat.ToBase32Guid(expected);

            Assert.AreEqual(base32Guid, actual);
        }

        [TestMethod]
        [DataRow("00000000-0000-0000-0000-000000000000", "00000000000000000000000000")]
        [DataRow("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", "zzzzzzzzzzzzzzzzzzzzzzzzzw")]
        [DataRow("ffffffff-ffff-ffff-ffff-ffffffffffff", "zzzzzzzzzzzzzzzzzzzzzzzzzw")]
        [DataRow("1C0FCF80-5B4E-4FC9-944A-7AA4549D7CF7", "3g7wz02v9s7wk52afaj597bwyw")]
        [DataRow("1c0fcf80-5b4e-4fc9-944a-7aa4549d7cf7", "3g7wz02v9s7wk52afaj597bwyw")]
        public void FromBase32GuidPasses(string guidStr, string base32Guid)
        {
            var expected = Guid.Parse(guidStr);

            var actual = DataFormat.FromBase32Guid(base32Guid);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("zzzzzzzzzzzzzzzzzzzzzzzzz")]
        [DataRow("zzzzzzzzzzzzzzzzzzzzzzzzzww")]
        public void FromBase32GuidInvalidInputLengthFail(string invalidLengthBase32)
        {
            var ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromBase32Guid(invalidLengthBase32));
            Assert.AreEqual("The input is not a valid base32 GUID as its length is not 26 characters.", ex.Message);
        }

        [TestMethod]
        [DataRow("zzzzzzzzzzzzzzzzzzuzzzzzzw")]
        [DataRow("zzzzzzzzzzzzzzlzzzzzzzzzzw")]
        [DataRow("zzzzzzzozzzzzzzzzzzzzzzzzw")]
        [DataRow("zzzzzzzzzzzzzzzzz zzzzzzzw")]
        public void FromBase32GuidInvalidInputCharFail(string invalidCharInBase32)
        {
            var ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromBase32Guid(invalidCharInBase32));
            Assert.AreEqual("The input is not a valid base32 string as it contains a non-base32 character.", ex.Message);
        }
    }
}