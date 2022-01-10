using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Client.Tests
{
    [TestClass()]
    public class FileBlockPaddingTests
    {
        [TestMethod()]
        public void PaddingTest()
        {
            var padding = new FileBlockPadding();
            var bs = new byte[1024];
            int dataEnd = 512;
            for (int i = 0; i < dataEnd; i++)
            {
                bs[i] = (byte)i;
            }
            padding.Padding(bs, dataEnd);
            Assert.AreEqual((byte)0x80, bs[dataEnd]);
            for (int i = dataEnd + 1; i < bs.Length; i++)
            {
                Assert.AreEqual(bs[i], (byte)0x00);
            }
        }

        [TestMethod()]
        public void DePaddingTest()
        {
            var padding = new FileBlockPadding();
            var bs = new byte[1024];
            int dataEnd = 512;
            for (int i = 0; i < dataEnd; i++)
            {
                bs[i] = (byte)i;
            }
            padding.Padding(bs, dataEnd);
            Assert.AreEqual(dataEnd, padding.DePadding(bs));
        }
    }
}