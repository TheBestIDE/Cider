using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Net
{
    /// <summary>
    /// 数据报缺失字节
    /// </summary>
    public class LackBytesException : Exception
    {
        public LackBytesException() { }
        public LackBytesException(string mes) : base(mes) { }
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

    public class LackFileBlocksException : LackDataBytesException
    {
        public LackFileBlocksException() { }
        public LackFileBlocksException(string mes) : base(mes) { }
    }

    /// <summary>头部指示操作与请求操作不匹配</summary>
    public class OperationMatchException : Exception
    {
        public OperationMatchException() { }
        public OperationMatchException(string mes) : base(mes) { }
    }
}
