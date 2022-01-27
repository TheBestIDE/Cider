using Cider.IO;
using System.Collections.Generic;

namespace Cider.Server.Data
{
    public interface IDataProvider
    {
        /// <summary>查找服务器中不存在的文件块哈希索引列表</summary>
        public int[]? FindNotExistHash(string[] hashs);

        /// <summary>写入文件信息</summary>
        public void SetFile(BlockedFile file);

        /// <summary>写入文件块信息</summary>
        public void SetBlockMessage(FileBlock fb, string fbname);

        /// <summary>
        /// 批量写入文件块
        /// </summary>
        public void SetBlocksMessage(Dictionary<string, string> hash_name_dic);

        /// <summary>读取文件信息</summary>
        public BlockedFile? ReadFileMessage(string fileName);

        /// <summary>
        /// 读取文件块对应的物理存储文件名
        /// </summary>
        /// <param name="hash">文件块的哈希值</param>
        public string? ReadBlockFileName(string hash);

        /// <summary>读取文件块的物理存储文件名</summary>
        public string[]? ReadFileBlocksFileName(string fileName);

        public void InsertToDirtyList(IEnumerable<string> hashs);

        public bool CheckIsInDirtyList(IEnumerable<string> hashs);
    }
}