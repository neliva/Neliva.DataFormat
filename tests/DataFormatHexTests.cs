// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void HexTestPasses()
        {
            var expected = "dd1d91b7d90b2bd3138533ce92b272fbf8a369316aefe242e659cc0ae238afe0";

            var value = DataFormat.FromHex(expected);
            var actual = DataFormat.ToHex(value);

            Assert.AreEqual(expected, actual);
        }
    }
}