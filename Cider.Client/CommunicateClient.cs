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

        public CommunicateClient(string ip)
        {
            var ipAddress = IPAddress.Parse(ip);
            iPEndPoint = new IPEndPoint(ipAddress, RuntimeArgs.Config.ServerPort);
        }
        public void Upload(string path)
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
                TcpClient client = new TcpClient();
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
                // 5.计算线性表达式

                // 6.上传计算结果
            }
            catch
            {

            }
        }

        public void Download(string fileName, string? path)
        {
            path = path ?? "./";
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

        protected string MatchSocketErrorCode(int errCode)
        {
            switch (errCode)
            {
                case 10013: return "权限被拒绝";
                case 10035: return "资源暂时不可用";
                case 10054: return "连接被对方重置";
                case 10061: return "连接被拒绝";
                case 10064: return "主机关闭";
                default: return "网络错误";
            }
        }

        protected string[] ReadBlockHash(string path)
        {
            byte[] buffer = new byte[RuntimeArgs.Config.BlockLength];
            using FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);
            int count;
            List<string> hashs = new List<string>();
            var hashHelper = new HashHelper(RuntimeArgs.Config.HashAlgorithm);
            while ((count = fs.Read(buffer, 0, buffer.Length)) != RuntimeArgs.Config.BlockLength)
            {
                hashs.Add(hashHelper.Compute(buffer));
            }
            if (count != 0)
            {
                var padding = new FileBlockPadding();
                padding.Padding(buffer);
                hashs.Add(hashHelper.Compute(buffer));
            }
            return hashs.ToArray();
        }

        protected GFMatrix ConvertToMatrix(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        protected bool HasSeparatorEnd(string path)
        {
            return path.EndsWith(RuntimeArgs.Separator);
        }

        protected string CutFileName(string path)
        {
            int i = path.LastIndexOf(RuntimeArgs.Separator);
            return path.Substring(i + 1, path.Length - i - 1);
        }

        private int Max(int value1, int value2)
        {
            return value1 > value2 ? value1 : value2;
        }
    }
}