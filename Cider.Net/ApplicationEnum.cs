using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
