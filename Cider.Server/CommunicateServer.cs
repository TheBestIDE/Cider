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

namespace Cider.Server
{
    internal class CommunicateServer : IDisposable
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

        #endregion

        public CommunicateServer() : this(RuntimeArgs.Config.ServerPort)
        {
        }

        public CommunicateServer(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            clientDict = new Dictionary<int, ApplicationLayer>();
        }

        public void Run()
        {
            InitApplicationService();
            RunAsync().Wait();
        }

        public void Dispose()
        {
            foreach (var item in clientDict)
            {
                item.Value.Dispose(); // 关闭所有连接
            }
        }

        #pragma warning disable 8600
        #pragma warning disable 8602
        protected async Task RunAsync()
        {
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
                await HandleConnectionAsync(service);
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
            BlockedFile file = new ();  // 保存当前处理的文件信息上下文
            var handle = Core.Single<HandleService>.Instance;    // 获取处理实例 单例模式
            try
            {
                // 1.接收文件名
                file.FileName = appClient.ReceiveFileName();

                // 2.接收哈希列表
                string[] hashs = appClient.ReceiveHashList();   // 接收到哈希列表
                string[]? diffHash = handle.HandleHashList(hashs);  // 获取服务器不存在的哈希值
                file.BlockHashList = hashs.ToList();    // 写入哈希值列表
                file.DifferentBlockList = diffHash?.ToList();   // 写入不存在列表

                // 3.返回上传的线性表达式结果数值
                int number = diffHash?.Length ?? 0;     // 服务器上不存在的哈希值数量
                number = handle.HandleConfuseNumber(number);    // 混淆数量
                appClient.SendReturnNumber(number);     // 发送返回数值

                // 4.处理线性表达式结果
                var result = appClient.ReceiveLinearResult();   // 接收线性表达式结果
                var matrix = ConvertToMatrix(result);   // 转化为矩阵
                if (matrix.Row != (ulong)number)        // 上传的线性表达式计算结果数量与请求的不一致
                    throw new LackLinearResultException();
                file.DifferentFileBlocks = handle.HandleLinearResult(matrix);   // 处理线性表达式结果

                // 5.写入文件
                handle.HandleWriteFile(file);

                Console.WriteLine("Task Exit normally.");
            }
            catch (ArgumentException)
            {
                // 未初始化应用层服务
                Console.WriteLine("Application Layer Serivce Not Initialized.");
            }
            catch (LackLinearResultException)
            {
                // 缺失线性表达式的结果
                Console.WriteLine("Lack of Linear Result.");
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
                clientDict.Remove(appClient.GetHashCode()); // 移除出字典
                appClient.Dispose();    // 释放非托管资源
                Console.WriteLine("Task Compelted.");
            }
        }

        protected static void InitApplicationService()
        {
            ApplicationService.EnCode = RuntimeArgs.Config.EnCoding;
            ApplicationService.HashLength = (int)RuntimeArgs.Config.HashAlgorithm;
        }

        protected static GFMatrix ConvertToMatrix(byte[] bytes)
        {
            int row = bytes.Length / RuntimeArgs.Config.BlockLength;
            var matrix = new GFMatrix((ulong)row, 1, (uint)RuntimeArgs.Config.BlockLength);
            throw new NotImplementedException();
        }
    }
}
