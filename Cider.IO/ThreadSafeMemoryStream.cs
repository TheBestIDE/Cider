using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cider.IO
{
    /// <summary>
    /// 线程安全的读写流
    /// 字节流存储在内存中
    /// </summary>
    public class ThreadSafeMemoryStream : Stream
    {
        protected readonly MemoryStream _memoryStream;

        protected Semaphore mSemaphore;

        private long _length;

        private long _position;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => _length;

        public override long Position { get => _position; set => throw new NotSupportedException(); }

        public ThreadSafeMemoryStream(long length)
        {
            _memoryStream = new MemoryStream();
            mSemaphore = new Semaphore(1, 1);
            this._length = length;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            mSemaphore.WaitOne();
            int c = _memoryStream.Read(buffer, offset, count);
            _position += c;
            mSemaphore.Release();
            return c;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            _length = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            mSemaphore.WaitOne();
            _memoryStream.Write(buffer, offset, count);
            mSemaphore.Release();
        }
    }
}
