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
    public class ThreadSafeBufferStream : Stream
    {
        /// <summary>
        /// 内存缓冲区
        /// 保存数据
        /// </summary>
        protected readonly byte[] mBuffer;

        /// <summary>
        /// 当前读取到的缓冲区数组位置
        /// </summary>
        protected int rdpoint = 0;

        /// <summary>
        /// 当前写入到的缓冲区数组位置
        /// </summary>
        protected int wrtpoint = 0;

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        protected readonly int bufferSize;

        /// <summary>
        /// 缓冲区读写同步信号量
        /// 写入信号量
        /// </summary>
        protected Semaphore emptySemaphore;

        /// <summary>
        /// 缓冲区读写同步信号量
        /// 读取信号量
        /// </summary>
        protected Semaphore fullSemaphore;

        /// <summary>
        /// 缓冲区互斥信号量
        /// </summary>
        protected Mutex mMutex;

        private long _length;

        /// <summary>
        /// 读取位置
        /// </summary>
        private long _rdpos = 0;

        /// <summary>
        /// 写入位置
        /// </summary>
        private long _wrtpos = 0;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => _length;

        /// <summary>
        /// 返回读取位置
        /// </summary>
        public override long Position { get => _rdpos; set => throw new NotSupportedException(); }

        /// <summary>
        /// 读取位置
        /// </summary>
        public long ReadPosition { get => _rdpos; }

        /// <summary>
        /// 写入位置
        /// </summary>
        public long WritePosition { get => _wrtpos; }

        public ThreadSafeBufferStream(long length) : this(length, 4096)
        {
            
        }

        public ThreadSafeBufferStream(long length, int bufSize)
        {
            bufferSize = bufSize;
            mBuffer = new byte[bufferSize];
            mMutex = new Mutex();
            emptySemaphore = new Semaphore(bufferSize, bufferSize);
            fullSemaphore = new Semaphore(0, bufferSize);
            this._length = length;
        }

        public override void Flush()
        {
        }

        public override int ReadByte()
        {
            mMutex.WaitOne();
            bool hasNoByte = _rdpos == _length;
            mMutex.ReleaseMutex();
            if (hasNoByte)
                return -1;

            fullSemaphore.WaitOne();
            mMutex.WaitOne();
            // 进入临界区
            int b;
            if (_rdpos < _length)
            {
                b = mBuffer[rdpoint];
                rdpoint = (rdpoint + 1) % bufferSize;
                _rdpos++;
            }
            else
            {
                b = -1;
            }
            // 出临界区
            mMutex.ReleaseMutex();
            emptySemaphore.Release();

            return b;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int i;
            for (i = 0; i < count; i++)
            {
                int b = ReadByte();
                if (b == -1)
                    break;
                buffer[offset + i] = (byte)b;
            }
            return i;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            _length = value;
        }

        public override void WriteByte(byte b)
        {
            emptySemaphore.WaitOne();
            mMutex.WaitOne();
            // 进入临界区
            mBuffer[wrtpoint] = b;
            wrtpoint = (wrtpoint + 1) % bufferSize;
            _wrtpos++;
            mMutex.ReleaseMutex();
            // 出临界区
            fullSemaphore.Release();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                WriteByte(buffer[offset + i]);
            }
        }
    }
}
