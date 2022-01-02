using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Cider.Net
{
    public enum ApplicationOption : byte
    {
        /// <summary>
        /// 发送哈希值列表
        /// </summary>
        SendHashList = 128,
        /// <summary>
        /// 服务端返回上传的数量
        /// </summary>
        ReturnNumber = 64,
        /// <summary>
        /// 发送线性表达式计算结果
        /// </summary>
        SendLinearResult = 32,
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

    }

    public abstract class ApplicationLayer
    {
        public abstract ApplicationHead Head { get; protected set; }

        public abstract void SetHead(ApplicationOption option);

        public abstract void Send(byte[] data);

        public abstract void SendHashList(string[] hashs);

        public abstract void SendReturnNumber(int number);

        public abstract void SendLinearResult(byte[] data);

        public abstract int Receive(byte[] data);

        public abstract string[] ReceiveHashList();

        public abstract int ReceiveReturnNumber();

        public abstract byte[] ReceiveLinearResult();

    }
}
