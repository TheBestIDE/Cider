using Cider.IO;
using Cider.Math;
using Cider.Server.Data;

namespace Cider.Server.Handle
{
    public class HandleService : HandleServiceBase
    {
        protected static Random rand = new Random();
        protected IDataProvider dataProvider = Core.Single<DataProvider>.Instance;

        public override void HandleDirtyBlock(BlockedFile file)
        {
            throw new NotImplementedException();
        }

        public override string[]? HandleHashList(string[] hashs)
        {
            return dataProvider.FindNotExistHash(hashs);
        }

        public override FileBlock[] HandleLinearResult(GFMatrix matrix)
        {
            throw new NotImplementedException();
        }

        public override int HandleConfuseNumber(int number)
        {
            return number == 0 ? 1 : rand.Next(number, number + 2); // [number, number + 1]闭区间上随机混淆
        }

        public override void HandleWriteFile(BlockedFile file)
        {
            Dictionary<string, string> hash_name_dic = new Dictionary<string, string>();
            // 不存在的块写入文件系统
            if (file.DifferentFileBlocks is not null)
            {
                foreach (var block in file.DifferentFileBlocks)
                {
                    var tlong = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();    // 获取当前时间戳作为随机名
                    string path = FileIOHelper.GetFullSaveFilePath(tlong.ToString());
                    FileIOHelper.WriteFile(path, block.Data);
                    dataProvider.SetBlockMessage(block, tlong.ToString());
                }
            }
            // 文件信息写入数据库
            dataProvider.SetFile(file);
        }
    }
}