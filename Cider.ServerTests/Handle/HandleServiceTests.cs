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
using Cider.Server.Data;
using Cider.Math;

namespace Cider.Server.Handle.Tests
{
    [TestClass()]
    public class HandleServiceTests
    {
        HandleService _handleService = Core.Single<HandleService>.Instance;
        DataProvider provider = Core.Single<DataProvider>.Instance;
        ServerDb db = Core.Single<ServerDb>.Instance;

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
            BlockedFile file = new();  // 保存当前处理的文件信息上下文
            var handle = Core.Single<HandleService>.Instance;    // 获取处理实例 单例模式
            var hashHelper = new HashHelper(RuntimeArgs.Config.HashAlgorithm);
            byte[][] data = new byte[4][];  // 文件数据

            // 1.接收文件名
            file.FileName = "Testtest";

            // 2.接收哈希列表
            for (int i = 0; i < 4; i++)
            {
                byte[] bytes = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    bytes[j] = (byte)(i * j);
                }
                data[i] = bytes;
                file.BlockHashList.Add(hashHelper.Compute(bytes));
            }
            var hashs = file.BlockHashList.ToArray();
            int[]? diffHash = handle.HandleHashList(hashs);  // 获取服务器不存在的哈希值
            file.DifferentBlockPositionList = diffHash?.ToList();   // 写入不存在列表

            Assert.IsNotNull(diffHash);
            Assert.AreEqual(1, diffHash.Length);
            Assert.AreEqual(3, diffHash[0]);

            // 3.返回上传的线性表达式结果数值
            int number = diffHash?.Length ?? 0;     // 服务器上不存在的哈希值数量
            // r < k的情况
            if (number != hashs.Length)
                number = handle.HandleConfuseNumber(number);    // 混淆数量
            // r = k无需混淆 保证 r <= k
            //appClient.SendReturnNumber(number);     // 发送返回数值

            long bitLength = RuntimeArgs.Config.BlockLength << 3;
            GFMatrix co_mat = VandermondeMatrix.CreateInstance((ulong)number, 4, bitLength);
            GFMatrix uk_mat = new GFMatrix(4, 1, bitLength);
            for (ulong i = 0; i < 4; i++)
            {
                GF gf = data[i];
                Assert.AreEqual(bitLength, gf.BitLength);
                uk_mat[i, 0] = gf;
            }
            var linear_mat = co_mat * uk_mat;
            Assert.AreEqual(bitLength, linear_mat.BitLength);

            MemoryStream stream = new MemoryStream();
            for (ulong i = 0; i < (ulong)number; i++)
            {
                byte[] bs = linear_mat[i, 0];
                Assert.AreEqual(4, bs.Length);
                stream.Write(bs, 0, bs.Length);
            }
            stream.Position = 0;

            // 4.处理线性表达式结果
            using var result = stream;     // 接收线性表达式结果
            Assert.AreNotEqual(hashs.Length, number);
            // 需要上传的块数量等于请求的块数量
            if (number == hashs.Length)
            {
                // 客户端不需要使用矩阵编码
                // 上传的即为文件块
                handle.HandleReadVerifyBlocks(file, result);
            }
            else
            {
                handle.HandleLinearResult(file, result);    // 处理线性表达式结果
                Assert.IsNotNull(file.DifferentFileBlocks);
                Assert.AreEqual(1, file.DifferentFileBlocks.Length);
                foreach (var block in file.DifferentFileBlocks)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Assert.AreEqual((byte)(i * 3), block.Data[i]);
                    }
                }
            }

            // 5.写入文件
            handle.HandleWriteFile(file);
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
            ClearDbTest();

            BlockedFile file = new BlockedFile()
            {
                FileName = "Testtest"
            };

            var hashHelper = new HashHelper(RuntimeArgs.Config.HashAlgorithm);
            byte[][] data = new byte[3][];
            List<FileBlock> fbs = new List<FileBlock>();
            for (int i = 0; i < 3; i++)
            {
                byte[] bytes = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    bytes[j] = (byte)(i * j);
                }
                data[i] = bytes;
                var hash = hashHelper.Compute(bytes);
                file.BlockHashList.Add(hash);
                fbs.Add(new FileBlock(hash, bytes));
            }
            Assert.AreEqual(3, file.BlockHashList.Count);
            Assert.AreEqual(3, fbs.Count);

            file.DifferentFileBlocks = fbs.ToArray();
            file.DifferentBlockPositionList = new List<int>() { 0, 1, 2 };

            _handleService.HandleWriteFile(file);

            var rslt_f = provider.ReadFileMessage(file.FileName);
            Assert.IsNotNull(rslt_f);
            Assert.AreEqual(file.BlockHashList.Count, rslt_f.BlockHashList.Count);
            for (int i = 0; i < file.BlockHashList.Count; i++)
            {
                Assert.AreEqual(file.BlockHashList[i], rslt_f.BlockHashList[i]);
            }
        }

        [TestMethod()]
        public void HandleReadFileTest()
        {
            using Stream stream = _handleService.HandleReadFile("test.txt");
            Assert.AreEqual(20, stream.Length);
            //byte[] buffer = new byte[3];
            //int c, rdCnt = 0;
            //while ((c = stream.Read(buffer, 0, buffer.Length)) != 0)
            //{
            //    for (int i = 0; i < c; i++, rdCnt++)
            //    {
            //        byte exp = (byte)((rdCnt >> 2) * (rdCnt & 3));
            //        Assert.AreEqual(exp, buffer[i]);
            //    }
            //}
        }

        [TestMethod()]
        public void ClearDbTest()
        {
            db.ClearFiles();
            db.ClearBlocks();
        }
    }
}