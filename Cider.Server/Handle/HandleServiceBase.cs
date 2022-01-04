using Cider.Math;

namespace Cider.Server.Handle
{
    public abstract class HandleServiceBase
    {
        /// <summary>
        /// 处理哈希值列表
        /// </summary>
        /// <returns>需要对方上传的线性表达式结果数量</returns>
        public abstract int HandleHashList(string[] hashs);

        /// <summary>
        /// 处理线性表达式结果
        /// </summary>
        /// <param name="matrix">线性表达式结果矩阵</param>
        public abstract void HandleLinearResult(GFMatrix matrix);
    }
}