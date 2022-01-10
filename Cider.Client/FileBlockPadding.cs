
namespace Cider.Client
{
    /// <summary>
    /// 使用ISO7816-4Padding方式
    /// 填充第一位为1 后面都为0
    /// </summary>
    public class FileBlockPadding
    {
        /// <summary>
        /// 填充块
        /// </summary>
        /// <param name="block">待填充的块</param>
        /// <param name="offset">填充的起点</param>
        public void Padding(byte[] block, int offset)
        {
            if (offset < block.Length)
                block[offset] = 0x80;
            for (int i = offset + 1; i < block.Length; i++)
            {
                block[i] = 0x00;
            }
        }

        /// <summary>
        /// 查找最后一个块的填充
        /// </summary>
        /// <exception cref="PaddingEndException"></exception>
        /// <returns>块有效字节数量</returns>
        public int DePadding(byte[] block)
        {
            int i;
            for (i = block.Length - 1; i >= 0; i--)
            {
                if (block[i] == 0x00)
                    continue;
                if (block[i] == 0x80)
                    break;
                throw new PaddingEndException();
            }

            return i;
        }
    }

    /// <summary>
    /// 填充尾部出现异常字符
    /// </summary>
    public class PaddingEndException : Exception
    {

    }
}