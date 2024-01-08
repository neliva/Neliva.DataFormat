// This is free and unencumbered software released into the public domain.
// See the UNLICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neliva.Tests
{
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
            var actual = DataFormat.FromBase32(string.Empty);

            Assert.AreEqual(Array.Empty<byte>(), actual);
        }

        [TestMethod]
        public void FromBase32NullStringPass()
        {
            var actual = DataFormat.FromBase32(null);

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