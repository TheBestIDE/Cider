using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cider.Server.Data.Model;

namespace Cider.Server.Data.Tests
{
    [TestClass()]
    public class ServerDbTests
    {
        [TestMethod()]
        public void StaticServerDbTest()
        {
            string dbPath = "./" + ServerDb.DbFileDirectory + "/" + ServerDb.DbFileName;
            Assert.IsTrue(System.IO.File.Exists(dbPath));
        }

        [TestMethod()]
        public void ServerDbTest()
        {
            using ServerDb db = new ServerDb();
        }

        [TestMethod()]
        public void GetFileTest()
        {
            using var db = new ServerDb();
            var rcvFile = db.GetFile("Test");
            Assert.IsNotNull(rcvFile);
            Assert.IsNotNull(rcvFile.BlockHash);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual("BlockHash" + i, rcvFile.BlockHash[i]);
            }
        }

        [TestMethod()]
        public void InsertFileTest()
        {
            using var db = new ServerDb();
            Model.File? file = new Model.File()
            {
                FileName = "Test"
            };

            IList<string> hashs = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                hashs.Add("BlockHash" + i);
            }
            file.BlockHash = hashs;

            db.InsertFile(file);

            var rcvFile = db.GetFile(file.FileName);
            Assert.IsNotNull(rcvFile);
            Assert.IsNotNull(rcvFile.BlockHash);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(hashs[i], rcvFile.BlockHash[i]);
            }
        }

        [TestMethod()]
        public void DeleteFileTest()
        {
            using var db = new ServerDb();
            db.DeleteFile("Test");
            var rcvFile = db.GetFile("Test");
            Assert.IsNull(rcvFile);
        }

        [TestMethod()]
        public void GetBlockTest()
        {
            using var db = new ServerDb();
            for (int i = 0; i < 10; i++)
            {
                var block = db.GetBlock("BlockHash" + i);
                Assert.IsNotNull(block);
                Assert.AreEqual("BlockHash" + i, block.BlockHash);
                Assert.AreEqual(i.ToString(), block.BlockFileName);
            }
        }

        [TestMethod()]
        public void InsertBlockTest()
        {
            using var db = new ServerDb();
            IList<FileBlock> fileBlocks = new List<FileBlock>();
            for (int i = 0; i < 10; i++)
            {
                fileBlocks.Add(new FileBlock()
                {
                    BlockHash = "BlockHash" + i,
                    BlockFileName = i.ToString(),
                });
            }

            foreach (var block in fileBlocks)
            {
                db.InsertBlock(block);
            }

            for (int i = 0; i < 10; i++)
            {
                var block = db.GetBlock("BlockHash" + i);
                Assert.IsNotNull(block);
                Assert.AreEqual(fileBlocks[i].BlockHash, block.BlockHash);
                Assert.AreEqual(fileBlocks[i].BlockFileName, block.BlockFileName);
            }
        }

        [TestMethod()]
        public void InsertBlocksTest()
        {
            using var db = new ServerDb();
            IList<FileBlock> fileBlocks = new List<FileBlock>();
            for (int i = 0; i < 10; i++)
            {
                fileBlocks.Add(new FileBlock()
                {
                    BlockHash = "BlockHash" + i,
                    BlockFileName = i.ToString(),
                });
            }

            db.InsertBlocks(fileBlocks);

            for (int i = 0; i < 10; i++)
            {
                var block = db.GetBlock("BlockHash" + i);
                Assert.IsNotNull(block);
                Assert.AreEqual(fileBlocks[i].BlockHash, block.BlockHash);
                Assert.AreEqual(fileBlocks[i].BlockFileName, block.BlockFileName);
            }
        }

        [TestMethod()]
        public void DeleteBlockTest()
        {
            using var db = new ServerDb();
            for (int i = 0; i < 10; i++)
            {
                db.DeleteBlock("BlockHash" + i);
                Assert.IsNull(db.GetBlock("BlockHash" + i));
            }
        }

        [TestMethod()]
        public void DeleteBlocksTest()
        {
            using var db = new ServerDb();
            IList<string> hashs = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                hashs.Add("BlockHash" + i);
            }
            db.DeleteBlocks(hashs);

            foreach (var hash in hashs)
            {
                Assert.IsNull(db.GetBlock(hash));
            }
        }

        [TestMethod()]
        public void CombineFilesAndBlocksTest()
        {
            using var db = new ServerDb();
            var hashFileName = db.CombineFilesAndBlocks("Test");
            Assert.IsNotNull(hashFileName);
            Assert.AreEqual(5, hashFileName.Length);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(i.ToString(), hashFileName[i]);
            }
        }

        [TestMethod()]
        public void DirtyBlocksTest()
        {
            using var db = new ServerDb();
            List<string> dthashs = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                dthashs.Add("BlockHash" + i);
            }

            db.InsertDirtyBlocksIfNotExist(dthashs);

            List<string> thashs = new List<string>();
            for (int i = 4; i < 10; i++)
            {
                thashs.Add("BlockHash" + i);
            }

            bool rst = db.IsInDirtyBlocks(thashs);
            Assert.IsTrue(rst);
        }

        [TestMethod()]
        public void DirtyBlocksTest1()
        {
            using var db = new ServerDb();
            List<string> dthashs = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                dthashs.Add("BlockHash" + i);
            }

            db.InsertDirtyBlocksIfNotExist(dthashs);

            List<string> thashs = new List<string>();
            for (int i = 5; i < 10; i++)
            {
                thashs.Add("BlockHash" + i);
            }

            bool rst = db.IsInDirtyBlocks(thashs);
            Assert.IsFalse(rst);
        }
    }
}