using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cider.Math.Tests
{
    [TestClass()]
    public class LongBytesTests
    {
        [TestMethod()]
        public void BitPositionTest()
        {
            LongBytes num = 0xFF010;
            Assert.AreEqual(1, num.WordCount);
            Assert.AreEqual(20, num.HighestBitPosition);
        }

        [TestMethod()]
        public void CompareTest()
        {
            LongBytes num1 = 0xFF010;
            LongBytes num2 = 0xFF101;
            Assert.AreEqual(-1, LongBytes.Compare(num1, num2));

            LongBytes num3 = 0xFF000;
            Assert.AreEqual(1, LongBytes.Compare(num1, num3));

            LongBytes num4 = 0xFF010;
            Assert.AreEqual(0, LongBytes.Compare(num1, num4));

            byte[] bs = new byte[16];
            bs[7] = 0x01;
            bs[15] = 0x10;
            bs[14] = 0xF0;
            bs[13] = 0x0F;
            LongBytes num5 = new LongBytes(bs);
            Assert.AreEqual(1, LongBytes.Compare(num5, num1));
            Assert.IsTrue(num5 > num1);

            bs[7] = 0;
            LongBytes num6 = new LongBytes(bs);
            Assert.AreEqual(0, LongBytes.Compare(num6, num1));
            Assert.IsTrue(num6 == num1);

            bs[15] = 0x00;
            LongBytes num7 = new LongBytes(bs);
            Assert.AreEqual(-1, LongBytes.Compare(num7, num1));
            Assert.IsTrue(num7 < num1);
        }

        [TestMethod()]
        public void ByteArrayTest()
        {
            byte[] bytes = new byte[16];
            for (int i = 0; i < 8; i++)
            {
                bytes[i] = (byte)i;
            }
            bytes[15] = 0b10011;

            string expectedHex = "0x";
            for (int i = 0; i < 16; i++)
            {
                expectedHex += bytes[i].ToString("X2");
            }

            LongBytes one = new LongBytes(bytes);
            Assert.AreEqual(expectedHex, one.ToString());
        }

        [TestMethod()]
        public void ImplicitArrayTest1()
        {
            byte[] expected = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                expected[i] = (byte)i;
            }

            LongBytes num = expected;
            byte[] rslt = num;

            for (int i = 0; i < 32; i++)
            {
                Assert.AreEqual(expected[i], rslt[i]);
            }
        }

        [TestMethod()]
        public void ImplicitArrayTest2()
        {
            byte[] expected = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                expected[i] = (byte)i;
            }

            LongBytes num = expected;
            byte[] rslt = num;

            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(expected[i], rslt[i]);
            }
        }

        [TestMethod()]
        public void EqualTest()
        {
            LongBytes num1 = 0xFF1F01;
            LongBytes num2 = 0xFF1F01;
            Assert.IsTrue(num1 == num2);
        }

        [TestMethod()]
        public void NotEqualTest()
        {
            LongBytes num1 = 0xFF1F01;
            LongBytes num2 = 0xFF1F02;
            Assert.IsTrue(num1 != num2);
        }

        [TestMethod()]
        public void LeftShiftTest()
        {
            byte[] bytes = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                bytes[i] = (byte)i;
            }

            LongBytes one = new LongBytes(bytes);
            one <<= 3;

            string expectedHex = "0x";
            for (int i = 0; i < 32; i++)
            {
                bytes[i] <<= 3;
                expectedHex += bytes[i].ToString("X2");
            }

            Assert.AreEqual(expectedHex, one.ToString());
        }

        [TestMethod()]
        public void RightShitTest()
        {
            byte[] bytes = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                bytes[i] = (byte)(i << 3);
            }

            LongBytes one = new LongBytes(bytes);
            one >>= 3;

            string expectedHex = "0x";
            for (int i = 0; i < 32; i++)
            {
                bytes[i] >>= 3;
                expectedHex += bytes[i].ToString("X2");
            }

            Assert.AreEqual(expectedHex, one.ToString());
        }

        [TestMethod()]
        public void AndTest1()
        {
            byte[] bytes = new byte[16];
            for (int i = 0; i < 8; i++)
            {
                bytes[i] = (byte)i;
            }
            bytes[15] = 0b10011;

            LongBytes num1 = new LongBytes(bytes);
            LongBytes num2 = 0b10101;
            var rslt = num1 & num2;

            bytes[15] &= 0b10101;
            string expectedHex = "0x";
            for (int i = 0; i < 15; i++)
            {
                expectedHex += "00";
            }
            expectedHex += bytes[15].ToString("X2");

            Assert.AreEqual(expectedHex, rslt.ToString());
        }

        [TestMethod()]
        public void OrTest1()
        {
            byte[] bytes = new byte[16];
            for (int i = 0; i < 8; i++)
            {
                bytes[i] = (byte)i;
            }
            bytes[15] = 0b10011;

            LongBytes num1 = new LongBytes(bytes);
            LongBytes num2 = 0b10101;
            var rslt = num1 | num2;

            bytes[15] |= 0b10101;
            string expectedHex = "0x";
            for (int i = 0; i < 16; i++)
            {
                expectedHex += bytes[i].ToString("X2");
            }

            Assert.AreEqual(expectedHex, rslt.ToString());
        }

        [TestMethod()]
        public void NotTest1()
        {
            byte[] bytes = new byte[16];
            for (int i = 0; i < 8; i++)
            {
                bytes[i] = (byte)i;
            }
            bytes[15] = 0b10011;

            LongBytes num1 = new LongBytes(bytes);
            var rslt = ~num1;

            string expectedHex = "0x";
            for (int i = 0; i < 16; i++)
            {
                bytes[i] = (byte)~bytes[i];
                expectedHex += bytes[i].ToString("X2");
            }

            Assert.AreEqual(expectedHex, rslt.ToString());
        }

        [TestMethod()]
        public void XorTest1()
        {
            LongBytes num1 = 0b10011;
            LongBytes num2 = 0b10101;
            ulong expected = 0b10011 ^ 0b10101;
            var rslt = num1 ^ num2;
            Assert.AreEqual("0x" + expected.ToString("X16"), rslt.ToString());
        }

        [TestMethod()]
        public void XorTest2()
        {
            byte[] bytes = new byte[16];
            for (int i = 0; i < 8; i++)
            {
                bytes[i] = (byte)i;
            }
            bytes[15] = 0b10011;

            LongBytes num1 = new LongBytes(bytes);
            LongBytes num2 = 0b10101;
            var rslt = num1 ^ num2;

            bytes[15] ^= 0b10101;
            string expectedHex = "0x";
            for (int i = 0; i < 16; i++)
            {
                expectedHex += bytes[i].ToString("X2");
            }

            Assert.AreEqual(expectedHex, rslt.ToString());
        }
    }
}
