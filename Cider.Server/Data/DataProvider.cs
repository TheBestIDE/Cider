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

        public int[]? FindNotExistHash(string[] hashs)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < hashs.Length; i++)
            {
                var rcv = Db.GetBlock(hashs[i]);
                if (rcv == null)
                    result.Add(i);
            }

            return result.Count > 0 ? result.ToArray() : null;
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

        public void SetBlocksMessage(Dictionary<string, string> hash_name_dic)
        {
            List<FileBlocks> blocks = new ();
            foreach (var trp in hash_name_dic)
            {
                blocks.Add(new FileBlocks()
                {
                    BlockFileName = trp.Value,
                    BlockHash = trp.Key
                });
            }
            Db.InsertBlocks(blocks);
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

        public string? ReadBlockFileName(string hash)
        {
            var block = Db.GetBlock(hash);
            return block?.BlockFileName ?? null;
        }

        public string[]? ReadFileBlocksFileName(string fileName)
        {
            return Db.CombineFilesAndBlocks(fileName);
        }

        public void InsertToDirtyList(IEnumerable<string> hashs)
        {
            Db.InsertDirtyBlocksIfNotExist(hashs);
        }

        public bool CheckIsInDirtyList(IEnumerable<string> hashs)
        {
            return Db.IsInDirtyBlocks(hashs);
        }
    }
}