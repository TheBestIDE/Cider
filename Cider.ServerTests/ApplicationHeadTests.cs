using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Net.Tests
{
    [TestClass()]
    public class ApplicationHeadTests
    {
        [TestMethod()]
        public void GetBytesTest()
        {
            ApplicationHead head = new ApplicationHead();
            byte expct_opt = 0x01;
            head.Option = expct_opt;
            head.DataLength = 0x010056;
            var bytes = head.GetBytes();
            Assert.AreEqual(expct_opt, bytes[0]);
            Assert.AreEqual((byte)0x56, bytes[1]);
            Assert.AreEqual((byte)0x00, bytes[2]);
            Assert.AreEqual((byte)0x01, bytes[3]);
        }

        [TestMethod()]
        public void CreateFromBytesTest()
        {
            byte[] bytes = new byte[5];
            bytes[0] = 0x60;
            bytes[1] = 0x56;
            bytes[2] = 0x00;
            bytes[3] = 0x01;
            bytes[4] = 0x00;
            var head = ApplicationHead.CreateFromBytes(bytes);
            Assert.IsNotNull(head);
            Assert.AreEqual(0x60, head.Option);
            Assert.AreEqual(0x00010056U, head.DataLength);
        }
    }
}