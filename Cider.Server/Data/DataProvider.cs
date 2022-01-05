using Cider.IO;

namespace Cider.Server.Data
{
    public class DataProvider : IDataProvider
    {
        public string[] FindNotExistHash(string[] hashs)
        {
            throw new NotImplementedException();
        }

        public void SetFile(BlockedFile file)
        {
            throw new NotImplementedException();
        }

        public void SetBlockMessage(FileBlock fb, string fbname)
        {
            throw new NotImplementedException();
        }

        public BlockedFile ReadFileMessage(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}