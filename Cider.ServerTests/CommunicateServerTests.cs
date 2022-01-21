using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cider.Net;
using Cider.IO;
using Cider.Server.Handle;
using System.IO;
using Cider.Global;
using System.Net;
using System.Net.Sockets;
using Cider.Global.Core;
using Cider.Server.Core;

namespace Cider.Server.Tests
{
    [TestClass()]
    public class CommunicateServerTests
    {
        static IPEndPoint iPEndPoint;

        static CommunicateServerTests()
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            iPEndPoint = new IPEndPoint(ipAddress, 8088);
        }

        protected int blockLength = RuntimeArgs.Config.BlockLength;

        protected TcpListener CreateListener()
        {
            return new TcpListener(IPAddress.Any, 8088);
        }

        protected TcpClient CreateClient()
        {
            return new TcpClient();
        }

        protected void InitApplicationService()
        {
            ApplicationService.EnCode = Encoding.UTF8;
            ApplicationService.HashByteLength = (int)SupportHash.SHA256 / 8;
            ApplicationService.BlockLength = blockLength;
        }

        [TestMethod()]
        public void HandleConnectionTest()
        {
            InitApplicationService();
            var listener = CreateListener();
            listener.Start();
            var conn_trd = listener.AcceptTcpClient();
            using var appConn = new ApplicationService(conn_trd);

            HandleConnection(appConn);
        }

        [TestMethod()]
        public void UploadTest()
        {
            InitApplicationService();
            var listener = CreateListener();
            listener.Start();
            var conn_trd = listener.AcceptTcpClient();
            using var appConn = new ApplicationService(conn_trd);

            var request = appConn.ReceiveRequestCommand();
            Assert.AreEqual(ApplicationRequestCommand.Upload, request);

            BlockedFile file = new();  // 保存当前处理的文件信息上下文
            var handle = Core.Single<HandleService>.Instance;    // 获取处理实例 单例模式

            // 1.接收文件名
            file.FileName = appConn.ReceiveFileName();
            //Assert.AreEqual("test.txt", file.FileName);

            // 2.接收哈希列表
            string[] hashs = appConn.ReceiveHashList();   // 接收到哈希列表
            //Assert.AreEqual(5, hashs.Length);
            int[]? diffHash = handle.HandleHashList(hashs);  // 获取服务器不存在的哈希值
            file.BlockHashList = hashs.ToList();    // 写入哈希值列表
            file.DifferentBlockPositionList = diffHash?.ToList();   // 写入不存在列表

            // 3.返回上传的线性表达式结果数值
            int number = diffHash?.Length ?? 0;     // 服务器上不存在的哈希值数量
            // r < k的情况
            if (number != hashs.Length)
                number = handle.HandleConfuseNumber(number);    // 混淆数量
            // r = k无需混淆 保证 r <= k
            appConn.SendReturnNumber(number);     // 发送返回数值

            // 4.处理线性表达式结果
            using var result = appConn.ReceiveLinearResult();     // 接收线性表达式结果
            Assert.AreEqual(0, result.Position);
            // 需要上传的块数量等于请求的块数量
            if (number == hashs.Length)
            {
                //Assert.AreEqual(5 * blockLength, result.Length);
                // 客户端不需要使用矩阵编码
                // 上传的即为文件块
                handle.HandleReadVerifyBlocks(file, result);
            }
            else
            {
                Assert.AreEqual(number * blockLength, result.Length);
                handle.HandleLinearResult(file, result);    // 处理线性表达式结果
            }

            // 5.写入文件
            handle.HandleWriteFile(file);
        }

        [TestMethod()]
        public void DownloadTest()
        {
            InitApplicationService();
            var listener = CreateListener();
            listener.Start();
            var conn_trd = listener.AcceptTcpClient();
            using var appConn = new ApplicationService(conn_trd);

            var request = appConn.ReceiveRequestCommand();
            Assert.AreEqual(ApplicationRequestCommand.Download, request);

            var handle = Core.Single<HandleService>.Instance;
            string fileName = appConn.ReceiveFileName();
            //Assert.AreEqual("test.txt", fileName);

            using Stream f = handle.HandleReadFile(fileName);
            //Assert.AreEqual(5 * blockLength, f.Length);
            appConn.SendFile(f);
        }

        protected void HandleConnection(ApplicationLayer appClient)
        {
            try
            {
                var request = appClient.ReceiveRequestCommand();
                if (request == ApplicationRequestCommand.Upload)
                {
                    HanldeUpload(appClient);
                    Console.WriteLine("Request Upload");
                }
                else if (request == ApplicationRequestCommand.Download)
                {
                    HandleDownload(appClient);
                    Console.WriteLine("Request Download");
                }
                else
                {
                    Console.WriteLine("Unknow Command");
                }

                Console.WriteLine("Task Exit normally.");
            }
            catch (ArgumentException)
            {
                // 未初始化应用层服务
                Console.WriteLine("Application Layer Serivce Not Initialized.");
            }
            catch (Hash.HashVerifyException)
            {
                Console.WriteLine("Fail to Verify Received Data's Hash.");
            }
            catch (LackFileBlocksException)
            {
                // 上传的文件块数量与请求的不一致
                Console.WriteLine("Lack of File Blocks.");
                // 处理脏块
                // handle.HandleDirtyBlock(file);
            }
            catch (LackHeadBytesException)
            {
                // 头字节缺失
                Console.WriteLine("Lack of head bytes.");
            }
            catch (LackDataBytesException)
            {
                // 数据字节缺失
                Console.WriteLine("Lack of data bytes.");
            }
            catch (OperationMatchException)
            {
                // 接收到的操作异常
                Console.WriteLine("Head indicate different from Operation.");
            }
            catch (Exception e)
            {
                // 未知异常
                // 均接收
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                appClient.Dispose();    // 释放非托管资源
                Console.WriteLine("Task Compelted.");
            }
        }

        protected void HanldeUpload(ApplicationLayer appClient)
        {
            BlockedFile file = new();  // 保存当前处理的文件信息上下文
            var handle = Core.Single<HandleService>.Instance;    // 获取处理实例 单例模式

            // 1.接收文件名
            file.FileName = appClient.ReceiveFileName();

            // 2.接收哈希列表
            string[] hashs = appClient.ReceiveHashList();   // 接收到哈希列表
            int[]? diffHash = handle.HandleHashList(hashs);  // 获取服务器不存在的哈希值
            file.BlockHashList = hashs.ToList();    // 写入哈希值列表
            file.DifferentBlockPositionList = diffHash?.ToList();   // 写入不存在列表

            // 3.返回上传的线性表达式结果数值
            int number = diffHash?.Length ?? 0;     // 服务器上不存在的哈希值数量
            // r < k的情况
            if (number != hashs.Length)
                number = handle.HandleConfuseNumber(number);    // 混淆数量
            // r = k无需混淆 保证 r <= k
            appClient.SendReturnNumber(number);     // 发送返回数值

            // 4.处理线性表达式结果
            using var result = appClient.ReceiveLinearResult();     // 接收线性表达式结果
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
            }

            // 5.写入文件
            handle.HandleWriteFile(file);
        }

        protected void HandleDownload(ApplicationLayer appClient)
        {
            var handle = Core.Single<HandleService>.Instance;
            string fileName = appClient.ReceiveFileName();
            using Stream f = handle.HandleReadFile(fileName);
            byte[] buffer = new byte[RuntimeArgs.Config.BlockLength];
            int count;
            while ((count = f.Read(buffer, 0, buffer.Length)) != 0)
            {
                appClient.Send(buffer, 0, count);
            }
        }
    }
}