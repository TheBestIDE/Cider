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
        public void ByteArrayTest()
        {
            byte[] bytes = new byte[16];
            for (int i = 0; i < 8; i++)
            {
                bytes[i] = (byte)i;
            }
            bytes[15] = 0b10011;

            string expectedHex = "";
            for (int i = 0; i < 16; i++)
            {
                expectedHex += bytes[i].ToString("X2");
            }

            LongBytes one = new LongBytes(bytes);
            Assert.AreEqual(expectedHex, one.ToString());
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

            string expectedHex = "";
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

            string expectedHex = "";
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
            string expectedHex = "";
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
            string expectedHex = "";
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

            string expectedHex = "";
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
            Assert.AreEqual(expected.ToString("X16"), rslt.ToString());
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
            string expectedHex = "";
            for (int i = 0; i < 16; i++)
            {
                expectedHex += bytes[i].ToString("X2");
            }

            Assert.AreEqual(expectedHex, rslt.ToString());
        }
    }
}
