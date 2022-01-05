using Cider.IO;

namespace Cider.Server.Data
{
    public interface IDataProvider
    {
        /// <summary>查找服务器中不存在的文件块哈希列表</summary>
        public string[] FindNotExistHash(string[] hashs);

        /// <summary>写入文件信息</summary>
        public void SetFile(BlockedFile file);

        /// <summary>写入文件块信息</summary>
        public void SetBlockMessage(FileBlock fb, string fbname);

        /// <summary>读取文件信息</summary>
        public BlockedFile ReadFileMessage(string fileName);
    }
}