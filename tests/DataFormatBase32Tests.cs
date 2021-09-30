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
        public void RoundTripPass()
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
        public void FromBase32EmptyStringPass()
        {
            var actual = DataFormat.FromBase32(string.Empty);

            CollectionAssert.AreEqual(new byte[0], actual);
        }

        [TestMethod]
        public void FromBase32NullStringPass()
        {
            var actual = DataFormat.FromBase32(null);

            CollectionAssert.AreEqual(new byte[0], actual);
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
        public void FromBase32InvalidInputLengthFail()
        {
            var ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromBase32("D"));
            Assert.AreEqual("The input is not a valid base32 string as its length is not correct.", ex.Message);

            ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromBase32("DSJ"));
            Assert.AreEqual("The input is not a valid base32 string as its length is not correct.", ex.Message);

            ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromBase32("DSJPRT"));
            Assert.AreEqual("The input is not a valid base32 string as its length is not correct.", ex.Message);

            ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromBase32("DSJPRTBPC"));
            Assert.AreEqual("The input is not a valid base32 string as its length is not correct.", ex.Message);

            ex = Assert.ThrowsException<FormatException>(() => DataFormat.FromBase32("DSJPRTBPC40"));
            Assert.AreEqual("The input is not a valid base32 string as its length is not correct.", ex.Message);
        }
    }
}