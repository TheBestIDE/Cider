using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Cider.Net
{
    /// <summary>
    /// 8bits选项
    /// 前四bits:
    /// 1:命令 2:上传 3:下载 4:错误位
    /// 
    /// </summary>
    public enum ApplicationOption : byte
    {
        /// <summary>
        /// 发送文件名
        /// </summary>
        SendFileName = 0x60,

        /// <summary>
        /// 发送文件
        /// </summary>
        SendFile = 0x20,
        /// <summary>
        /// 发送文件不存在或损坏
        /// </summary>
        SendFileNotExist = 0x30,

        /// <summary>
        /// 发送哈希值列表
        /// </summary>
        SendHashList = 0x40,
        /// <summary>
        /// 服务端返回上传的数量
        /// </summary>
        SendReturnNumber = 0x41,
        /// <summary>
        /// 发送线性表达式计算结果
        /// </summary>
        SendLinearResult = 0x42,

        /// <summary>请求指令</summary>
        RequestCommand = 0x80,
    }

    public enum ApplicationRequestCommand : byte
    {
        None = 0,
        /// <summary>上载</summary>
        Upload = 1,
        /// <summary>下载</summary>
        Download = 2,
    }

    /// <summary>
    /// 应用层头部
    /// </summary>
    public class ApplicationHead
    {
        public byte Option { get; set; }

        public uint DataLength { get; set; }

        public void SetOption(ApplicationOption opion)
        {
            Option = (byte)opion;
        }

        /// <summary>
        /// 将头部信息转化为字节序列
        /// </summary>
        public byte[] GetBytes()
        {
            byte[] byteArray = new byte[5];
            byteArray[0] = Option;
            // 小端法存储长度信息
            // 低字节存放低位数据
            uint length = DataLength;
            for (int i = 0; i < 4; i++)
            {
                byteArray[i + 1] = (byte)(length & 0xFF);
                length >>= 8;    // 下一个高位字节
            }

            return byteArray;
        }

        /// <summary>
        /// 从字节数组中读取头部信息
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static ApplicationHead CreateFromBytes(byte[] bArray)
        {
            ApplicationHead head = new ApplicationHead(); 
            if (bArray.Length >= 5)
            {
                head.Option = bArray[0];
                head.DataLength = 0;
                for (int i = 4; i > 0; i--)
                {
                    head.DataLength |= bArray[i];
                    head.DataLength <<= 8;
                }
            }
            else
            {
                throw new ArgumentException("需要的数组长度至少为5");
            }

            return head;
        }
    }

    /// <summary>数据报缺失字节</summary>
    public class LackBytesException : Exception
    {
        public LackBytesException(){}
        public LackBytesException(string mes) : base(mes){}
    }

    /// <summary>数据报头部缺失字节</summary>
    public class LackHeadBytesException : LackBytesException
    {
        public LackHeadBytesException()
        {
        }

        public LackHeadBytesException(string mes) : base(mes)
        {
        }
    }

    /// <summary>数据报数据部分缺失字节</summary>
    public class LackDataBytesException : LackBytesException
    {
        public LackDataBytesException()
        {
        }

        public LackDataBytesException(string mes) : base(mes)
        {          
        }
    }

    public class LackLinearResultException : LackDataBytesException
    {
        public LackLinearResultException() { }
        public LackLinearResultException(string mes) : base(mes) { }
    }

    /// <summary>头部指示操作与请求操作不匹配</summary>
    public class OperationMatchException : Exception 
    {
        public OperationMatchException(){}
        public OperationMatchException(string mes) : base(mes){}
    }

    public abstract class ApplicationLayer : IDisposable
    {
        public abstract ApplicationHead Head { get; protected set; }

        /// <summary>
        /// 解析头部
        /// </summary>
        /// <exception cref="LackHeadBytesException">头部不完整</exception>
        /// <exception cref="OperationMatchException">数据报操作不匹配</exception>
        protected ApplicationHead ReceiveHead(ApplicationOption option)
        {
            byte[] bhead = new byte[5];
            if (Receive(bhead) != 5)
                throw new LackHeadBytesException();  // 收到的头部字节不完整

            var head = ApplicationHead.CreateFromBytes(bhead);
            if (head.Option != (byte)ApplicationOption.SendLinearResult)
                throw new OperationMatchException();  // 收到的数据报不是该操作

            return head;
        }

        /// <summary>
        /// 解析头部
        /// </summary>
        /// <exception cref="LackHeadBytesException">头部不完整</exception>
        protected ApplicationHead ReceiveHead()
        {
            byte[] bhead = new byte[5];
            if (Receive(bhead) != 5)
                throw new LackHeadBytesException();  // 收到的头部字节不完整

            return ApplicationHead.CreateFromBytes(bhead);
        }

        public virtual void RequestUpload()
        {
            Head.SetOption(ApplicationOption.RequestCommand);
            Head.DataLength = 1U;
            byte[] re = new byte[1] { (byte)ApplicationRequestCommand.Upload };
            Send(Head.GetBytes());
            Send(re);
        }

        public virtual void RequestDownload()
        {
            Head.SetOption(ApplicationOption.RequestCommand);
            Head.DataLength = 1U;
            byte[] re = new byte[1] { (byte)ApplicationRequestCommand.Download };
            Send(Head.GetBytes());
            Send(re);
        }

        /// <summary>接收请求命令</summary>
        /// <exception cref="LackDataBytesException"></exception>
        /// <exception cref="LackHeadBytesException"></exception>
        /// <exception cref="OperationMatchException"></exception>
        public virtual ApplicationRequestCommand ReceiveRequestCommand()
        {
            _ = ReceiveHead(ApplicationOption.RequestCommand);
            byte[] buf = new byte[1];
            if (Receive(buf) != 1)
                throw new LackDataBytesException();
            if (buf[0] == 1)
                return ApplicationRequestCommand.Upload;
            else if (buf[0] == 2)
                return ApplicationRequestCommand.Download;
            else
                return ApplicationRequestCommand.None;
        }

        public void SetHead(ApplicationOption option, uint length)
        {
            Head.SetOption(option);
            Head.DataLength = length;
        }

        /// <summary>
        /// 发送比特流
        /// </summary>
        public void Send(byte[] data)
        {
            Send(data, 0, data.Length);
        }

        public abstract void Send(byte[] data, int offset, int count);

        /// <summary>
        /// 发送文件名
        /// </summary>
        public abstract void SendFileName(string name);

        /// <summary>
        /// 发送文件分块的哈希值列表
        /// </summary>
        public abstract void SendHashList(string[] hashs);

        /// <summary>
        /// 发送返回的文件块数
        /// </summary>
        public abstract void SendReturnNumber(int number);

        /// <summary>
        /// 发送线性表达式计算结果
        /// </summary>
        public abstract void SendLinearResult(byte[] data);

        /// <summary>
        /// 使用流发送文件
        /// </summary>
        public abstract void SendFile(Stream stream);

        /// <summary>
        /// 接收比特流
        /// </summary>
        public abstract int Receive(byte[] data);

        /// <summary>
        /// 接收文件名
        /// </summary>
        public abstract string ReceiveFileName();

        /// <summary>
        /// 接收哈希列表
        /// </summary>
        public abstract string[] ReceiveHashList();

        /// <summary>
        /// 接收返回的文件块数
        /// </summary>
        public abstract int ReceiveReturnNumber();

        /// <summary>
        /// 接收线性表达式计算结果
        /// </summary>
        public abstract byte[] ReceiveLinearResult();

        /// <summary>
        /// 接收文件
        /// </summary>
        public abstract Stream? ReceiveFile();

        public abstract void Dispose();

    }
}
