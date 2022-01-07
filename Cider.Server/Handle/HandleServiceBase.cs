using Cider.Math;
using Cider.IO;

namespace Cider.Server.Handle
{
    public abstract class HandleServiceBase
    {
        /// <summary>
        /// 处理哈希值列表
        /// </summary>
        /// <returns>服务器不存在的哈希值列表</returns>
        public abstract string[]? HandleHashList(string[] hashs);

        /// <summary>
        /// 处理线性表达式结果
        /// </summary>
        /// <param name="matrix">线性表达式结果矩阵</param>
        public abstract FileBlock[] HandleLinearResult(GFMatrix matrix);

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