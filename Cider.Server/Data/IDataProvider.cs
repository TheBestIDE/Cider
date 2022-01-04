using Cider.IO;

namespace Cider.Server.Data
{
    public interface IDataProvider
    {
        public string[] FindNotExistHash(string[] hashs);

        public void SetFile(BlockedFile file);

        public BlockedFile ReadFileMessage();
    }
}