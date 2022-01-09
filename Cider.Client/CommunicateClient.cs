using Cider.Net;
using Cider.IO;
using Cider.Global;
using System.Net.Sockets;
using System.Net;
using Cider.Math;
using Cider.Hash;

namespace Cider.Client
{
    public class CommunicateClient
    {
        protected IPEndPoint iPEndPoint;

        protected FileBlockPadding padding;

        public CommunicateClient(string ip)
        {
            var ipAddress = IPAddress.Parse(ip);
            padding = new FileBlockPadding();
            iPEndPoint = new IPEndPoint(ipAddress, RuntimeArgs.Config.ServerPort);
        }
        public async Task Upload(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("{0}:文件不存在", path);
                return;
            }
            try
            {
                // 计算分块哈希值
                string[] hashs = ReadBlockHash(path);
                // 建立连接
                TcpClient client = new();
                client.Connect(iPEndPoint);
                using ApplicationLayer appClient = new ApplicationService(client);

                // 1.请求上传
                appClient.RequestUpload();
                // 2.上传文件名
                appClient.SendFileName(CutFileName(path));
                // 3.分块哈希上传
                appClient.SendHashList(hashs);
                // 4.接收返回值
                int returnNumber = appClient.ReceiveReturnNumber();
                // 5.异步计算线性表达式
                Stream stream = await ComputeLinearAsync(path, returnNumber);
                // 6.上传计算结果
                appClient.SendLinearResult(stream);
            }
            catch
            {

            }
        }

        public void Download(string fileName, string? path)
        {
            path ??= "./";
            path = HasSeparatorEnd(path) ? (path + fileName) : path;
            // 检查文件存在性
            if (File.Exists(path))
            {
                Console.Write("{0}:该文件已存在，是否覆盖文件？[y/n]", path);
                string? isCover = Console.ReadLine();
                isCover?.ToLower();
                // 不覆盖文件
                if (isCover is null || (isCover != "yes" && isCover != "y"))
                    return;     // 返回
            }

            try
            {
                using var client = new TcpClient();
                client.Connect(iPEndPoint);
                using ApplicationLayer appClient = new ApplicationService(client);

                // 1.请求下载
                appClient.RequestDownload();

                // 2.上传文件名
                appClient.SendFileName(fileName);

                // 3.接收文件
                Stream? stream = appClient.ReceiveFile();
                if (stream is null || !stream.CanRead)
                {
                    Console.WriteLine("{0}:文件不存在", fileName);
                }
                else
                {
                    try
                    {
                        // 创建新文件
                        using FileStream fs = File.Open(path, FileMode.OpenOrCreate);
                        // 设置缓冲区
                        byte[] buffer = new byte[Max(8192, RuntimeArgs.Config.BlockLength)];
                        int count;  // 保存每次读取的字节数
                        while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            fs.Write(buffer, 0 ,count);
                        }
                    }
                    catch (DirectoryNotFoundException)
                    {
                        Console.WriteLine("{0}:目录不存在", path);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("{0}:没有访问权限", path);
                    }
                    catch (Exception e)
                    {
                        #if DEBUG
                        Console.WriteLine(e.StackTrace);
                        #endif
                        Console.WriteLine("IO错误");
                    }
                }
            }
            catch (SocketException se)
            {
                #if DEBUG
                Console.WriteLine("Socket错误代码:{0}", se.ErrorCode);
                #endif
                string errMes = MatchSocketErrorCode(se.ErrorCode);
                Console.WriteLine(errMes);
            }
            catch (Exception e)
            {
                #if DEBUG
                Console.WriteLine(e.StackTrace);
                #endif
                Console.WriteLine("网络错误");
            }
        }

        protected static string MatchSocketErrorCode(int errCode)
        {
            return errCode switch
            {
                10013 => "权限被拒绝",
                10035 => "资源暂时不可用",
                10054 => "连接被对方重置",
                10061 => "连接被拒绝",
                10064 => "主机关闭",
                _ => "网络错误",
            };
        }

        protected string[] ReadBlockHash(string path)
        {
            byte[] buffer = new byte[RuntimeArgs.Config.BlockLength];
            using FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);
            int count;
            List<string> hashs = new();
            var hashHelper = new HashHelper(RuntimeArgs.Config.HashAlgorithm);
            while ((count = fs.Read(buffer, 0, buffer.Length)) != RuntimeArgs.Config.BlockLength)
            {
                hashs.Add(hashHelper.Compute(buffer));
            }
            if (count != 0)
            {
                padding.Padding(buffer);
                hashs.Add(hashHelper.Compute(buffer));
            }
            return hashs.ToArray();
        }

        /// <summary>
        /// 异步计算线性表达式
        /// </summary>
        /// <param name="path"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected async Task<Stream> ComputeLinearAsync(string path, int count)
        {
            return await Task.Run(() => ComputeLinearAsync(path, count));
        }

        /// <summary>
        /// 计算线性表达式
        /// </summary>
        /// <param name="path"></param>
        /// <param name="count"></param>
        /// <returns>结果写入内存流中</returns>
        protected Stream ComputeLinear(string path, int count)
        {
            // 耗时操作
            // 使用线程安全的流保存计算结果
            // 保证异步操作的线程安全性
            Stream stream = new ThreadSafeMemoryStream(count * RuntimeArgs.Config.BlockLength);
            using FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);

            // 完整的块个数
            var completeBlockCount = fs.Length / (RuntimeArgs.Config.BlockLength >> 3);
            // 是否能被完整分块
            // 若文件字节数能整除块长 则文件能被完整分块
            var IsComplete = fs.Length % (RuntimeArgs.Config.BlockLength >> 3) == 0;
            // 文件块数
            // 若不能被完整分块 则在完整块数上+1
            var blockCount = IsComplete ? completeBlockCount : completeBlockCount + 1;

            VandermondeEnumerator vdmd = new(count, blockCount, (uint)RuntimeArgs.Config.BlockLength);
            for (int i = 0; i < count; i++)
            {
                byte[] buffer = new byte[RuntimeArgs.Config.BlockLength];
                var linearItem = new GF((uint)RuntimeArgs.Config.BlockLength, 0);   // 保存当前线性表达式计算的中间结果
                for (int j = 0; j < blockCount ; j++)
                {
                    int c = fs.Read(buffer, 0, RuntimeArgs.Config.BlockLength); // 从文件中读取字节
                    if (c < buffer.Length)
                        padding.Padding(buffer);
                    vdmd.MoveNext();
                    var gf = buffer * vdmd.Current; // 矩阵对应元素相乘
                    linearItem += gf;   // 加上元素乘法结果
                }
                stream.Write(linearItem, 0, (int)linearItem.BitLength); // 当前线性表达式计算完成 写入流
                fs.Seek(0, SeekOrigin.Begin);   // 回到文件头部重新计算下一个线性表达式
            }
            return stream;
        }

        protected GFMatrix ConvertToMatrix(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        protected static bool HasSeparatorEnd(string path)
        {
            return path.EndsWith(RuntimeArgs.Separator);
        }

        protected static string CutFileName(string path)
        {
            int i = path.LastIndexOf(RuntimeArgs.Separator);
            return path.Substring(i + 1, path.Length - i - 1);
        }

        private static int Max(int value1, int value2)
        {
            return value1 > value2 ? value1 : value2;
        }
    }
}