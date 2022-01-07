using Cider.IO;
using Cider.Server.Data.Model;

namespace Cider.Server.Data
{
    public class DataProvider : IDataProvider
    {
        protected IServerDb Db;

        public DataProvider()
        {
            Db = Core.Single<ServerDb>.Instance;
        }

        public string[] FindNotExistHash(string[] hashs)
        {
            throw new NotImplementedException();
        }

        public void SetFile(BlockedFile file)
        {
            Files f = new Files()
            {
                FileName = file.FileName,
                BlockHash = file.BlockHashList
            };
            Db.InsertFile(f);
        }

        public void SetBlockMessage(FileBlock fb, string fbname)
        {
            FileBlocks fbs = new FileBlocks()
            {
                BlockFileName = fbname,
                BlockHash = fb.HashCode
            };
            Db.InsertBlock(fbs);
        }

        #pragma warning disable 8601
        public BlockedFile? ReadFileMessage(string fileName)
        {
            var f = Db.GetFile(fileName);
            return f is null 
                        ? null 
                        : new BlockedFile()
                        {
                            FileName = fileName,
                            BlockHashList = f.BlockHash
                        };
        }

        public string[]? ReadFileBlocksFileName(string fileName)
        {
            return Db.CombineFilesAndBlocks(fileName);
        }
    }
}