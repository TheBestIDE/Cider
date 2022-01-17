using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.Server.Handle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cider.IO;
using Cider.Hash;
using Cider.Global;
using System.IO;

namespace Cider.Server.Handle.Tests
{
    [TestClass()]
    public class HandleServiceTests
    {
        HandleService _handleService = Core.Single<HandleService>.Instance;


        [TestMethod()]
        public void HandleHashListTest()
        {
            List<string> hashs = new List<string>();
            // 添加已有的hash
            for (int i = 6; i < 10; i++)
            {
                hashs.Add("BlockHash" + i);
            }
            // 添加不存在的hash
            for (int i = 20; i < 25; i++)
            {
                hashs.Add("BlockHash" + i);
            }

            var diff = _handleService.HandleHashList(hashs.ToArray());
            Assert.IsNotNull(diff);
            Assert.AreEqual(5, diff.Length);

            for (int i = 0; i < diff.Length; i++)
            {
                Assert.AreEqual(i + 4, diff[i]);
            }
        }

        [TestMethod()]
        public void HandleLinearResultTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void HandleReadVerifyBlocksTest()
        {
            BlockedFile file = new BlockedFile()
            {
                FileName = "Testtest"
            };

            var hashHelper = new HashHelper(RuntimeArgs.Config.HashAlgorithm);
            byte[][] data = new byte[3][];
            for (int i = 0; i < 3; i++)
            {
                byte[] bytes = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    bytes[j] = (byte)(i * j);
                }
                data[i] = bytes;
                file.BlockHashList.Add(hashHelper.Compute(bytes));
            }
            Assert.AreEqual(3, file.BlockHashList.Count);

            file.DifferentBlockPositionList = new List<int>() { 0, 1, 2 };

            using MemoryStream stream = new MemoryStream();
            foreach (var bytes in data)
            {
                foreach (var b in bytes)
                {
                    stream.WriteByte(b);
                }
            }
            stream.Seek(0, SeekOrigin.Begin);

            _handleService.HandleReadVerifyBlocks(file, stream);
            Assert.IsNotNull(file.DifferentFileBlocks);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Assert.AreEqual(data[i][j], file.DifferentFileBlocks[i].Data[j]);
                }
            }
        }

        [TestMethod()]
        public void HandleConfuseNumberTest()
        {;
            int num1 = _handleService.HandleConfuseNumber(0);
            Assert.IsTrue(num1 == 1 || num1 == 2);

            int num2 = _handleService.HandleConfuseNumber(1);
            Assert.IsTrue(num2 == 1 || num2 == 2);

            int num3 = _handleService.HandleConfuseNumber(10);
            Assert.IsTrue(num3 == 10 || num3 == 11);
        }

        [TestMethod()]
        public void HandleWriteFileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void HandleReadFileTest()
        {
            Assert.Fail();
        }
    }
}