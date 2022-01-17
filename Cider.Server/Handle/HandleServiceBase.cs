using Cider.Math;
using Cider.IO;
using Cider.Hash;
using Cider.Net;

namespace Cider.Server.Handle
{
    public abstract class HandleServiceBase
    {
        /// <summary>
        /// 处理哈希值列表
        /// </summary>
        /// <returns>服务器不存在的哈希值索引列表</returns>
        public abstract int[]? HandleHashList(string[] hashs);

        /// <summary>
        /// 处理线性表达式结果
        /// </summary>
        /// <param name="file">分块文件对象</param>
        /// <param name="stream">待处理字节流</param>
        public abstract void HandleLinearResult(BlockedFile file, Stream stream);

        /// <summary>
        /// 接受和验证文件块
        /// </summary>
        /// <remarks>当且仅当文件块未编码时使用此方法 即客户端请求上传的文件块数量等于服务器返回的数量</remarks>
        /// <param name="file">分块文件对象</param>
        /// <param name="stream">待处理字节流</param>
        /// <exception cref="HashVerifyException"></exception>
        /// <exception cref="LackFileBlocksException"></exception>
        public abstract void HandleReadVerifyBlocks(BlockedFile file, Stream stream);

        /// <summary>
        /// 处理脏块
        /// </summary>
        /// <param name="file"></param>
        public abstract void HandleDirtyBlock(BlockedFile file);

        /// <summary>
        /// 混淆数量
        /// </summary>
        /// <param name="number">原来的数量</param>        
        public abstract int HandleConfuseNumber(int number);

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="file">文件信息</param>
        public abstract void HandleWriteFile(BlockedFile file);

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="fileName">文件名</param>
        public abstract FileBlocksReadStream HandleReadFile(string fileName);
    }
}