using Cider.Global;
using Cider.Hash;
using Cider.IO;
using Cider.Math;
using Cider.Net;
using Cider.Server.Data;

namespace Cider.Server.Handle
{
    public class HandleService : HandleServiceBase
    {
        protected static readonly Random rand = new ();
        protected IDataProvider dataProvider = Core.Single<DataProvider>.Instance;

        public override void HandleDirtyBlock(BlockedFile file)
        {
            throw new NotImplementedException();
        }

        public override int[]? HandleHashList(string[] hashs)
        {
            return dataProvider.FindNotExistHash(hashs);
        }

        #pragma warning disable 8602
        public override void HandleLinearResult(BlockedFile file, Stream stream)
        {
            int k = file.BlockHashList.Count;
            int m = file.DifferentBlockPositionList?.Count ?? 0;
            // 服务器已经有了所有需要的信息
            // 去重系统丢弃接收到的数据
            if (m == 0)
                return;

            long bitLength = RuntimeArgs.Config.BlockLength << 3;   // 分块比特长度
            GFSquareMatrix coefficientMatrix = new GFSquareMatrix((ulong)k, bitLength);   // 系数矩阵
            GFMatrix rsltMatrix = new GFMatrix((ulong)k, 1, bitLength);   // 结果矩阵

            // 从文件系统中读取 k - m 个文件块
            int readCnt = 0;    // 已读取的文件块个数
            for (int i = 0; i < k; i++)
            {
                // 已读取到 k - m 个文件块
                // 结束读取
                if (readCnt == k - m)
                {
                    break;
                }

                // 文件块存在
                if (!file.DifferentBlockPositionList.Contains(i))
                {
                    var blockName = dataProvider.ReadBlockFileName(file.BlockHashList[i]);  // 读取文件块物理存储文件名
                    if (blockName is null)  // 文件块存在
                        throw new IOException("File IO Error: Block " + file.BlockHashList[i] + " Not Exist.");
                    var path = FileIOHelper.GetFullSaveFilePath(blockName);     // 获取物理存储路径
                    byte[] buffer = new byte[RuntimeArgs.Config.BlockLength];   // 存储文件块缓冲区
                    FileIOHelper.ReadFile(path, buffer, 0);     // 将文件块读取到缓冲区内

                    // 系数矩阵前 k - m 行为单位向量
                    var idtVector = GFMatrix.CreateIdentityRowVector((ulong)i, (ulong)k, bitLength);   // k维单位行向量 第i列为1
                    coefficientMatrix.SendRow((ulong)readCnt, idtVector, 0);    // 单位行向量写入系数矩阵

                    // 结果矩阵前 k - m 行为已存在的文件块
                    rsltMatrix[(ulong)readCnt, 0] = buffer;

                    // 读取文件块数+1
                    readCnt++;
                }
            }

            List<FileBlock> blocks = new List<FileBlock>(); // 临时存储不存在文件块列表
            // 添加后 m 行
            for (int i = 0; i < m; i++)
            {
                ulong row_num = (ulong)(i + k - m);
                var vdmdRow = VandermondeMatrix.CreateRowInstance((ulong)i, (ulong)k, bitLength);

                // 系数矩阵后 m 行为范德蒙德矩阵行
                coefficientMatrix.SendRow(row_num, vdmdRow, 0);

                // 结果矩阵后 m 行为线性表达式的计算结果
                var buffer = new byte[RuntimeArgs.Config.BlockLength];  // 存储结果缓冲区
                stream.Read(buffer, 0, buffer.Length);
                rsltMatrix[row_num, 0] = buffer;
            }

            var unknownMatrix = coefficientMatrix.GetInverseMatrix() * rsltMatrix;  // 未知数矩阵
            // 验证文件块的哈希值
            for (int i = 0; i < k; i++)
            {
                byte[] buffer = unknownMatrix[(ulong)i, 0];
                var hashHepler = new HashHelper(RuntimeArgs.Config.HashAlgorithm);
                if (hashHepler.Compute(buffer) != file.BlockHashList[i])
                    throw new HashVerifyException();
                // 索引位于服务器不存在块索引列表中
                if (file.DifferentBlockPositionList.Contains(i))
                    blocks.Add(new FileBlock(file.BlockHashList[i], buffer));
            }

            // 添加到不存在文件块列表中
            file.DifferentFileBlocks = blocks.Count > 0 ? blocks.ToArray() : null;
        }

        public override void HandleReadVerifyBlocks(BlockedFile file, Stream stream)
        {
            byte[] buffer = new byte[RuntimeArgs.Config.BlockLength];
            int i;
            var hashHelper = new HashHelper(RuntimeArgs.Config.HashAlgorithm);
            var diffBlocks = new List<FileBlock>();
            for (i = 0; i < file.BlockHashList.Count && stream.Read(buffer, 0, buffer.Length) != 0; i++)
            {
                // 哈希值验证失败
                if (hashHelper.Compute(buffer) != file.BlockHashList[i])
                    throw new HashVerifyException();
                // 为服务器中不存在的块
                if (file.DifferentBlockPositionList?.Contains(i) ?? false)
                {
                    FileBlock block = new FileBlock(file.BlockHashList[i], buffer);
                    diffBlocks.Add(block);
                }
            }

            // 客户端上传的块数小于服务器返回的数量
            if (i < file.BlockHashList.Count)
                throw new LackFileBlocksException();

            // 不存在的块写入file对象
            file.DifferentFileBlocks = diffBlocks.Count > 0 ? diffBlocks.ToArray() : null;
        }

        public override int HandleConfuseNumber(int number)
        {
            return number == 0 ? rand.Next(1, 3) : rand.Next(number, number + 2);   // [number, number + 1]闭区间上随机混淆
        }

        public override void HandleWriteFile(BlockedFile file)
        {
            Dictionary<string, string> hash_name_dic = new Dictionary<string, string>();
            // 不存在的块写入文件系统
            if (file.DifferentFileBlocks is not null)
            {
                foreach (var block in file.DifferentFileBlocks)
                {
                    var tlong = DateTime.Now.ToUniversalTime().Ticks;   // 获取当前时间戳作为随机名
                    string path = FileIOHelper.GetFullSaveFilePath(tlong.ToString());
                    FileIOHelper.WriteFile(path, block.Data);
                    hash_name_dic[block.HashCode] = tlong.ToString();
                }
                dataProvider.SetBlocksMessage(hash_name_dic);
            }
            // 文件信息写入数据库
            dataProvider.SetFile(file);
        }

        public override FileBlocksReadStream HandleReadFile(string fileName)
        {
            var fbsn = dataProvider.ReadFileBlocksFileName(fileName);
            List<string> fpaths = new List<string>();
            if (fbsn is not null)
            {
                foreach (var f in fbsn)
                {
                    fpaths.Add(FileIOHelper.GetFullSaveFilePath(f));
                }
            }
            return new FileBlocksReadStream(fpaths);
        }
    }
}