using Cider.Global;
namespace Cider.Server.Handle
{
    public class FileBlocksReadStream : Stream
    {
        protected readonly long length;

        protected long position;

        protected Queue<string>? filePathsQueue;

        FileStream? fileStream;

        public FileBlocksReadStream(IEnumerable<string>? filePaths)
        {
            if (filePaths is null || filePaths.Count() == 0)
            {
                filePathsQueue = null;
            }
            else
            {
                filePathsQueue = new Queue<string>();
                foreach(var path in filePaths)
                {
                    // 只写入存在的文件
                    if (File.Exists(path))
                        filePathsQueue.Enqueue(path);
                }
                length = filePathsQueue.Count * RuntimeArgs.Config.BlockLength;
                GetNextExist();
            }
        }

        public override bool CanRead => filePathsQueue is not null;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => length;

        public override long Position { get => position; set => throw new NotSupportedException(); }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // 不可读
            if (!CanRead)
                throw new NotSupportedException();
            // 没有流可以读取
            if (fileStream is null)
                return 0;
            // 从文件中打开流
            int c = fileStream.Read(buffer, offset, count);
            // 文件流已经读取到末尾
            if (c == 0)
            {
                // 释放文件
                fileStream.Dispose();
                // 读取下一个文件
                GetNextExist();
                // 没有下一个文件
                if (fileStream is null)
                    return 0;
                // 有下一个文件 读取
                return fileStream.Read(buffer, offset, count);
            }
            else
            {
                return c;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                fileStream?.Dispose();
            filePathsQueue = null;
        }

        #pragma warning disable 8602
        protected void GetNextExist()
        {
            string? path = null;
            // 队列中还有文件
            while (filePathsQueue.Count > 0)
            {
                // 读取队首
                var p = filePathsQueue.Dequeue();
                // 文件存在
                if (File.Exists(p))
                {
                    path = p;
                    break;
                }
            }
            // 读取到了存在的文件就打开然后返回
            fileStream = (path is null) ? null : File.Open(path, FileMode.Open);
        }
    }
}