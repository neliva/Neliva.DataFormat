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
            var actual = DataFormat.FromHex(string.Empty);

            Assert.AreEqual(Array.Empty<byte>(), actual);
        }

        [TestMethod]
        public void FromHexNullStringPass()
        {
            var actual = DataFormat.FromHex(null);

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
}