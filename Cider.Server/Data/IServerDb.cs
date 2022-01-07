using Cider.Server.Data.Model;

namespace Cider.Server.Data
{
    public interface IServerDb
    {
        public Files? GetFile(string fileName);
        public void InsertFile(Files fb);
        public void DeleteFile(string fileName);
        
        public FileBlocks? GetBlock(string hash);
        public void InsertBlock(FileBlocks fb);
        public void DeleteBlock(string hash);
        public string[]? CombineFilesAndBlocks(string fileName);
    }
}