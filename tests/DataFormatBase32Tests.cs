// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Neliva.Tests
{
    [TestClass]
    public class DataFormatBase32Tests
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

            string hex = DataFormat.ToBase32((byte[])original.Clone());

            byte[] result = DataFormat.FromBase32(hex);

            CollectionAssert.AreEqual(original, result, "Roundtrip values don't match.");

            byte[] resultUpper = DataFormat.FromBase32(hex.ToUpperInvariant());

            CollectionAssert.AreEqual(original, resultUpper, "Roundtrip values don't match (Upper).");
        }

        [TestMethod]
        public void FromBase32EmptyString_Pass()
        {
            var actual = DataFormat.FromBase32(string.Empty);

            CollectionAssert.AreEqual(new byte[0], actual);
        }

        [TestMethod]
        public void FromBase32NullString_Pass()
        {
            var actual = DataFormat.FromBase32(null);

            CollectionAssert.AreEqual(new byte[0], actual);
        }

        [TestMethod]
        public void ToBase32EmptyArray_Pass()
        {
            var actual = DataFormat.ToBase32(Array.Empty<byte>());

            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void ToBase32NullArray_Pass()
        {
            var actual = DataFormat.ToBase32(null);

            Assert.AreEqual(string.Empty, actual);
        }
    }
}