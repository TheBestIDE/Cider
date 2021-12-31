using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Global.Core
{
    /// <summary>
    /// 配置抽象类
    /// </summary>
    public abstract class AConfiguration
    {
        /// <summary>
        /// 编码
        /// </summary>
        public abstract Encoding EnCoding { get; internal set; }
        
        /// <summary>
        /// 哈希算法
        /// </summary>
        public abstract string HashAlgorithm { get; internal set; }

        /// <summary>
        /// 分块长度
        /// </summary>
        public abstract int BlockLength { get; internal set; }

        /// <summary>
        /// 服务端口
        /// </summary>
        public abstract int ServerPort { get; internal set; }
    }
}
