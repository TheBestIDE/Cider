using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Server.Data.Tests
{
    [TestClass()]
    public class DataProviderTests
    {
        protected DataProvider dataProvider = new DataProvider();

        [TestMethod()]
        public void FindNotExistHashTest()
        {
            List<string> hashs = new List<string>();
            // 添加已有的hash
            for (int i = 6; i < 10; i++)
            {
                hashs.Add("BlockHash" + i);
            }
            // 添加不存在的hash
            for (int i = 20; i < 25; i ++)
            {
                hashs.Add("BlockHash" + i);
            }

            var diff = dataProvider.FindNotExistHash(hashs.ToArray());
            Assert.IsNotNull(diff);
            Assert.AreEqual(5, diff.Length);

            for (int i = 0; i < diff.Length; i++)
            {
                Assert.AreEqual("BlockHash" + (i + 20), diff[i]);
            }
        }

        [TestMethod()]
        public void SetBlocksMessageTest()
        {
            Dictionary<string, string> blocks = new Dictionary<string, string>();
            for (int i = 100; i < 105; i++)
            {
                blocks["BlockHash" + i] = i.ToString();
            }
            dataProvider.SetBlocksMessage(blocks);

            var db = Cider.Server.Core.Single<ServerDb>.Instance;
            for (int i = 100; i < 105; i++)
            {
                var hash = "BlockHash" + i;
                var block = db.GetBlock(hash);
                Assert.IsNotNull(block);
                Assert.AreEqual(blocks[hash], block.BlockFileName);
            }
        }

        [TestMethod()]
        public void ReadFileMessageTest()
        {
            var file = dataProvider.ReadFileMessage("Test");
            Assert.IsNotNull(file);
            Assert.AreEqual("Test", file.FileName);
            Assert.IsNotNull(file.BlockHashList);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual("BlockHash" + i, file.BlockHashList[i]);
            }
        }

        [TestMethod()]
        public void ReadFileBlocksFileNameTest()
        {
            var names = dataProvider.ReadFileBlocksFileName("Test");
            Assert.IsNotNull(names);
            for (int i = 0; i < names.Length; i++)
            {
                Assert.AreEqual(i.ToString(), names[i]);
            }
        }
    }
}