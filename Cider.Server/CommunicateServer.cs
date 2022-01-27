using Cider.Global;
using Cider.Net;
using Cider.Server.Handle;
using Cider.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cider.IO;
using Cider.Server.Core;

namespace Cider.Server
{
    public class CommunicateServer : IDisposable
    {
        #region Field

        /// <summary>
        /// Tcp服务
        /// </summary>
        protected TcpListener server;

        /// <summary>
        /// 已连接的Tcp客户主机
        /// </summary>
        protected Dictionary<int, ApplicationLayer> clientDict;

        protected Dictionary<int, Task> taskDict;

        #endregion

        public CommunicateServer() : this(RuntimeArgs.Config.ServerPort)
        {
        }

        public CommunicateServer(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            clientDict = new Dictionary<int, ApplicationLayer>();
            taskDict = new Dictionary<int, Task>();
        }

        public void Dispose()
        {
            foreach (var item in clientDict)
            {
                item.Value.Dispose(); // 关闭所有连接
            }
            foreach (var task in taskDict)
            {
                task.Value.Dispose();
            }
        }

        #pragma warning disable 8600
        #pragma warning disable 8602
        public void Run()
        {
            InitApplicationService();
            server.Start();

            Console.WriteLine("Start Server.");
            while (true)
            {
                Console.WriteLine("Waiting for connection...");
                // 监听等待连接
                TcpClient client = server.AcceptTcpClient();

                // 收到客户端连接
                Console.WriteLine("Connected from {0}:{1}", 
                                  ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), 
                                  ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString());
                ApplicationService service = new ApplicationService(client);    // 使用Tcp基础连接创建应用层对象
                clientDict[service.GetHashCode()] = service;  // 写入字典

                // 异步处理连接请求
                // 直接返回监听
                var task = HandleConnectionAsync(service);
                taskDict[task.GetHashCode()] = task;
                Console.WriteLine("Task submitted.");
            }
        }

        /// <summary>
        /// 处理连接异步方法
        /// </summary>
        protected async Task HandleConnectionAsync(ApplicationLayer appClient)
        {
            await Task.Run(() => HandleConnection(appClient));
        }

        /// <summary>
        /// 处理连接同步方法
        /// </summary>
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
                clientDict.Remove(appClient.GetHashCode()); // 移除出字典
                appClient.Dispose();    // 释放非托管资源
                Console.WriteLine("Task Compelted.");
            }
        }

        /// <summary>
        /// 处理上传请求
        /// </summary>
        /// <exception cref="LackDataBytesException"></exception>
        /// <exception cref="LackHeadBytesException"></exception>
        /// <exception cref="Hash.HashVerifyException"></exception>
        /// <exception cref="LackFileBlocksException"></exception>
        /// <exception cref="OperationMatchException"></exception>
        /// <exception cref="ArgumentException">未初始化应用层服务</exception>
        protected void HanldeUpload(ApplicationLayer appClient)
        {
            BlockedFile file = new ();  // 保存当前处理的文件信息上下文
            var handle = Core.Single<HandleService>.Instance;    // 获取处理实例 单例模式

            try
            {
                // 1.接收文件名
                file.FileName = appClient.ReceiveFileName();

                // 2.接收哈希列表
                string[] hashs = appClient.ReceiveHashList();   // 接收到哈希列表
                int[]? diffHash = handle.HandleHashList(hashs);  // 获取服务器不存在的哈希值
                file.BlockHashList = hashs.ToList();    // 写入哈希值列表
                file.DifferentBlockPositionList = diffHash?.ToList();   // 写入不存在列表

                // 3.返回上传的线性表达式结果数值
                int number = handle.CheckDirtyBlocks(hashs) // 检查哈希值是否有位于脏块列表中
                             ? hashs.Length                 // 返回r = k
                             : (diffHash?.Length ?? 0);     // 服务器上不存在的哈希值数量
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
            catch (Hash.HashVerifyException)
            {
                Console.WriteLine("Fail to Verify Received Data's Hash.");
                // 哈希验证失败
                // 处理脏块
                handle.HandleDirtyBlock(file);
            }
            catch (LackFileBlocksException)
            {
                // 上传的文件块数量与请求的不一致
                Console.WriteLine("Lack of File Blocks.");
                // 处理脏块
                handle.HandleDirtyBlock(file);
            }
        }

        /// <summary>
        /// 处理下载请求
        /// </summary>
        /// <exception cref="LackDataBytesException"></exception>
        /// <exception cref="LackHeadBytesException"></exception>
        /// <exception cref="OperationMatchException"></exception>
        /// <exception cref="ArgumentException">未初始化应用层服务</exception>
        protected void HandleDownload(ApplicationLayer appClient)
        {
            var handle = Core.Single<HandleService>.Instance;
            string fileName = appClient.ReceiveFileName();
            using Stream f = handle.HandleReadFile(fileName);
            appClient.SendFile(f);
        }

        protected static void InitApplicationService()
        {
            ApplicationService.EnCode = RuntimeArgs.Config.EnCoding;
            ApplicationService.HashByteLength = (int)RuntimeArgs.Config.HashAlgorithm / 8;
            ApplicationService.BlockLength = RuntimeArgs.Config.BlockLength;
        }

        protected static GFMatrix ConvertToMatrix(Stream stream)
        {
            // 由于上传前客户端已填充
            // 故认为客户端上传的字节流是能被完整分块的
            // 这里直接整除
            long row = stream.Length / RuntimeArgs.Config.BlockLength;
            var matrix = new GFMatrix((ulong)row, 1, (uint)RuntimeArgs.Config.BlockLength);
            byte[] buffer = new byte[RuntimeArgs.Config.BlockLength];
            for (long i = 0; i < row && (stream.Read(buffer, 0, buffer.Length) != 0); i++)
            {
                matrix[(ulong)i, 0] = buffer;
            }
            return matrix;
        }
    }
}
