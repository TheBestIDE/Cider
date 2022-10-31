using Cider.Server.Data.Model;

namespace Cider.Server.Data
{
    public interface IServerDb
    {
        public Model.File? GetFile(string fileName);
        public void InsertFile(Model.File fb);
        public void DeleteFile(string fileName);
        
        public FileBlock? GetBlock(string hash);
        public void InsertBlock(FileBlock fb);
        public void InsertBlocks(IEnumerable<FileBlock> fbs);
        public void DeleteBlock(string hash);
        public void DeleteBlocks(IEnumerable<string> hashs);

        public string[]? CombineFilesAndBlocks(string fileName);

        public bool IsInDirtyBlocks(IEnumerable<string> hashs);
        public void InsertDirtyBlocksIfNotExist(IEnumerable<string> hashs);
    }
}