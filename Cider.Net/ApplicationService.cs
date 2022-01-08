using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Net
{
    public class ApplicationService : ApplicationLayer
    {
        #region Field

        protected TcpClient client;

        protected NetworkStream nStream;

        #endregion

        #region Property

        public override ApplicationHead Head { get; protected set; }

        /// <summary>
        /// 编码方式
        /// </summary>
        public static Encoding? EnCode { get; set; }

        /// <summary>
        /// 哈希字段的长度
        /// </summary>
        public static int HashLength { get; set; }

        /// <summary>
        /// 服务是否可使用
        /// </summary>
        public static bool CanProvideService
        {
            get
            {
                return (EnCode is not null) && (HashLength != 0);
            }
        }

        /// <summary>
        /// 是否有数据可读
        /// </summary>
        protected bool HasData
        {
            get
            {
                return nStream.DataAvailable;
            }
        }

        #endregion

        #region Constructor

        public ApplicationService() : this(new TcpClient())
        {
        }

        public ApplicationService(TcpClient tcp)
        {
            Head = new ApplicationHead();
            client = tcp;
            nStream = client.GetStream();
        }

        #endregion

        #region Method

        public override int Receive(byte[] data)
        {
            if (nStream.CanRead)
                return nStream.Read(data);
            else
                return 0;
        }

        public override void Send(byte[] data, int offset, int count)
        {
            if (nStream.CanWrite)
                nStream.Write(data, offset, count);
        }

        #pragma warning disable 8602
        /// <summary>
        /// 发送文件名
        /// </summary>
        /// <param name="name">文件名</param>
        /// <exception cref="ArgumentException">未初始化应用层服务</exception>
        public override void SendFileName(string name)
        {
            CheckIfInit();
            byte[] bname = EnCode.GetBytes(name);
            Head.SetOption(ApplicationOption.SendFileName);
            Head.DataLength = (uint)bname.Length;
            Send(Head.GetBytes());
            Send(bname);
        }

        public override void SendHashList(string[] hashs)
        {
            Head.SetOption(ApplicationOption.SendHashList);
            Head.DataLength = (uint)hashs.Length;
            Send(Head.GetBytes());
            foreach(string str in hashs)
            {
                var buf = Encoding.ASCII.GetBytes(str); // 哈希字符串用ASCII编码传输
                Send(buf);
            }
        }

        public override void SendReturnNumber(int number)
        {
            Head.SetOption(ApplicationOption.SendReturnNumber);
            Head.DataLength = 1U;
            Send(Head.GetBytes());
            Send(ToBytes(number));
        }

        public override void SendLinearResult(byte[] data)
        {
            Head.SetOption(ApplicationOption.SendLinearResult);
            Head.DataLength = (uint)data.Length;
            Send(Head.GetBytes());
            Send(data);
        }

        public override void SendFile(Stream stream)
        {
            if (stream.CanRead)
            {
                Head.SetOption(ApplicationOption.SendFile);
                Head.DataLength = (uint)stream.Length;
                Send(Head.GetBytes());
                byte[] buffer = new byte[8192];
                int count;
                while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    Send(buffer, 0, count);
                }
            }
            else
            {
                Head.SetOption(ApplicationOption.SendFileNotExist);
                Head.DataLength = 0u;
                Send(Head.GetBytes());
            }
        }

        /// <summary>接收文件名</summary>
        /// <exception cref="LackDataBytesException"></exception>
        /// <exception cref="LackHeadBytesException"></exception>
        /// <exception cref="OperationMatchException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public override string ReceiveFileName()
        {
            CheckIfInit();
            var head = ReceiveHead(ApplicationOption.SendFileName);

            byte[] buf = new byte[1024];    // 缓冲区
            using MemoryStream mStream = new MemoryStream();
            int waitTimes = 0;  // 等待次数
            while (mStream.Length < head.DataLength)
            {
                if (HasData)    // 有数据可读取
                {
                    int count = Receive(buf);
                    mStream.Write(buf, 0, count);
                    waitTimes = 0;  // 重置等待次数
                }
                else if (waitTimes <= 10)   // 收到
                {
                    Thread.Sleep(1000); // 等待1s
                    waitTimes++;
                }
                else    // 等待超过10次 不再等待
                {
                    throw new LackDataBytesException();  // 收到的字节数有误
                }
            }

            return EnCode.GetString(mStream.GetBuffer());
        }

        /// <summary>接收哈希列表</summary>
        /// <exception cref="LackDataBytesException"></exception>
        /// <exception cref="LackHeadBytesException"></exception>
        /// <exception cref="OperationMatchException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public override string[] ReceiveHashList()
        {
            CheckIfInit();
            var head = ReceiveHead(ApplicationOption.SendHashList);

            string[] hashs = new string[head.DataLength];   // 哈希列表
            byte[] buf = new byte[HashLength << 1];     // 哈希一个字节用两个十六进制显示的字符表示
                                                        // 一个ASCII字符占一个字节
            int i = 0,  // 计数 已完整收到的哈希数量
                waitTimes = 0;  // 等待次数
            using MemoryStream mStream = new MemoryStream();
            while (i < head.DataLength)     // 需要接收到报头给定的哈希的数量才停止
            {
                int count = Receive(buf);   // 一次接收的字节数
                mStream.Write(buf, 0, count);   // 接收的字节写入内存缓冲区
                if (mStream.Length >= buf.Length)  // 已接收到一个哈希对应的字节数量
                {
                    mStream.Read(buf, 0, buf.Length);  // 读取出字节流
                    hashs[i] = Encoding.ASCII.GetString(buf);   // 转化为哈希字符串
                    waitTimes = 0;  // 重置等待次数
                    i++;    // 累加
                }
                else if (waitTimes > 10     // 等待次数超过10次 认为网络异常 不再等待
                      || count == 0)        // 返回字节为0 远程主机关闭了连接                                 
                {
                    throw new LackDataBytesException();  // 接收到的哈希字段数量有误
                }
                else    // 未读满一个哈希对应的字节数量
                {
                    Thread.Sleep(1000); // 等待1s
                    waitTimes++;
                }
            }

            return hashs;
        }

        /// <summary>接收返回的线性表达式数值</summary>
        /// <exception cref="LackDataBytesException"></exception>
        /// <exception cref="LackHeadBytesException"></exception>
        /// <exception cref="OperationMatchException"></exception>
        public override int ReceiveReturnNumber()
        {
            _ = ReceiveHead(ApplicationOption.SendReturnNumber);
            byte[] buf = new byte[4];
            if (Receive(buf) != 4)
                throw new LackDataBytesException();
            
            return ToInt(buf, 0);
        }

        /// <summary>
        /// 接收线性表达式计算结果
        /// </summary>
        /// <exception cref="LackLinearResultException"></exception>
        /// <exception cref="LackHeadBytesException"></exception>
        /// <exception cref="OperationMatchException"></exception>
        public override byte[] ReceiveLinearResult()
        {
            ApplicationHead head = ReceiveHead(ApplicationOption.SendLinearResult);
            
            byte[] buf = new byte[1024];    // 缓冲区
            using MemoryStream mStream = new ();
            int waitTimes = 0;  // 等待次数
            while (mStream.Length < head.DataLength)
            {
                if (HasData)    // 有数据可读取
                {
                    int count = Receive(buf);
                    mStream.Write(buf, 0, count);
                    waitTimes = 0;  // 重置等待次数
                }
                else if (waitTimes <= 10)   // 收到
                {
                    Thread.Sleep(1000); // 等待1s
                    waitTimes++;
                }
                else    // 等待超过10次 不再等待
                {
                    throw new LackLinearResultException();  // 收到的字节数有误
                }
            }

            return mStream.GetBuffer();
        }

        /// <summary>
        /// 接收文件内容
        /// </summary>
        /// <exception cref="LackHeadBytesException"></exception>
        /// <exception cref="OperationMatchException"></exception>
        /// <returns>读取文件的流</returns>
        public override Stream? ReceiveFile()
        {
            ApplicationHead head = ReceiveHead();
            if (head.Option == (byte)ApplicationOption.SendFileNotExist)
                return null;
            if (head.Option != (byte)ApplicationOption.SendFile)
                throw new OperationMatchException();
            return nStream;
        }

        public override void Dispose()
        {
            nStream.Close();
            client.Close();
        }

        /// <summary>
        /// 测试是否被初始化
        /// </summary>
        /// <exception cref="ArgumentException">未初始化抛出异常</exception>
        protected static void CheckIfInit()
        {
            if (!CanProvideService)
                throw new ArgumentException("HashLength is not init.");
        }

        protected static byte[] ToBytes(int number)
        {
            byte[] bs = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bs[i] = (byte)(number & 0xFF);
                number >>= 8;
            }
            return bs;
        }

        protected static int ToInt(byte[] bs, int offset)
        {
            int number = 0;
            for (int i = offset; i < offset + 4; i++)
            {
                number |= bs[i];
                number <<= 8;
            }
            return number;
        }

        #endregion
    }
}
