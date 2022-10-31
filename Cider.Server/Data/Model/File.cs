
namespace Cider.Server.Data.Model
{
    public class File
    {
        /// <summary>文件名</summary>
        public string? FileName { get; set; }

        /// <summary>文件哈希值</summary>
        public IList<string>? BlockHash { get; set; }
    }
}