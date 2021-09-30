// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Neliva.Tests
{
    [TestClass]
    public class DataFormatHexTests
    {
        [TestMethod]
        public void RoundTrip_Pass()
        {
            byte[] original = new byte[byte.MaxValue * 2 + 8];

            byte val = 0;

            for (int i = 0; i < original.Length; i++)
            {
                original[i] = val;

                val++;
            }

            string hex = DataFormat.ToHex((byte[])original.Clone());

            byte[] result = DataFormat.FromHex(hex);

            CollectionAssert.AreEqual(original, result, "Roundtrip values don't match.");

            byte[] resultUpper = DataFormat.FromHex(hex.ToUpperInvariant());

            CollectionAssert.AreEqual(original, resultUpper, "Roundtrip values don't match (Upper).");
        }

        [TestMethod]
        public void FromHexEmptyString_Pass()
        {
            var actual = DataFormat.FromHex(string.Empty);

            CollectionAssert.AreEqual(new byte[0], actual);
        }

        [TestMethod]
        public void FromHexNullString_Pass()
        {
            var actual = DataFormat.FromHex(null);

            CollectionAssert.AreEqual(new byte[0], actual);
        }

        [TestMethod]
        public void ToHexEmptyArray_Pass()
        {
            var actual = DataFormat.ToHex(Array.Empty<byte>());

            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void ToHexNullArray_Pass()
        {
            var actual = DataFormat.ToHex(null);

            Assert.AreEqual(string.Empty, actual);
        }

    }
}