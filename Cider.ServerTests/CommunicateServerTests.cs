using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.Server;
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
using Cider.Server.Handle;

namespace Cider.Server.Tests
{
    [TestClass()]
    public class CommunicateServerTests
    {
        [TestMethod()]
        public void NormalCommunicateTest()
        {
            BlockedFile file = new ();  // 保存当前处理的文件信息上下文
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
            Assert.AreEqual(0, diffHash.Length);

            // 3.返回上传的线性表达式结果数值
            int number = handle.CheckDirtyBlocks(hashs) // 检查哈希值是否有位于脏块列表中
                            ? hashs.Length                 // 返回r = k
                            : (diffHash?.Length ?? 0);     // 服务器上不存在的哈希值数量
            // r < k的情况
            if (number != hashs.Length)
                number = handle.HandleConfuseNumber(number);    // 混淆数量
            // r = k无需混淆 保证 r <= k
            // Assert.AreEqual(hashs.Length, number);

            // handle.HandleWriteFile(file);
        }

        [TestMethod()]
        public void VerifyCommunicateTest()
        {

        }
    }
}