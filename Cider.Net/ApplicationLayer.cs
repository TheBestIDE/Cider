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
    /// 应用层抽象类
    /// </summary>
    public abstract class ApplicationLayer : IDisposable
    {
        #region Property

        public abstract ApplicationHead Head { get; protected set; }

        #endregion

        #region Method

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
            if (head.Option != (byte)option)
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
        public abstract void SendLinearResult(Stream stream);

        /// <summary>
        /// 使用流发送文件
        /// </summary>
        public abstract void SendFile(Stream stream);

        /// <summary>
        /// 接收比特流
        /// </summary>
        public int Receive(byte[] data)
        {
            return Receive(data, 0, data.Length);
        }

        public abstract int Receive(byte[] data, int offset, int count);

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
        public abstract Stream ReceiveLinearResult();

        /// <summary>
        /// 接收文件
        /// </summary>
        public abstract uint ReceiveFile(out Stream? stream);


        public abstract void Dispose();

        #endregion
    }
}
