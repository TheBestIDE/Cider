using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.IO
{
    public class BufferQueue<T>
    {
        protected Queue<T> dataQueue;

        protected Queue<string> bufferQueue = new();

        /// <summary>
        /// 保存在内存中的个数
        /// </summary>
        public int MemoryNumber { get; protected set; }

        public int Count { get; protected set; }

        public BufferQueue() : this(10)
        {

        }

        public BufferQueue(int memoryNumber)
        {
            MemoryNumber = memoryNumber;
            dataQueue = new Queue<T>(memoryNumber);
        }

        public void Enqueue(T item)
        {
            if (dataQueue.Count < MemoryNumber)
            {
                dataQueue.Enqueue(item);
            }
            else
            {
                string path = "";   // 保存到临时文件
                bufferQueue.Enqueue(path);
            }
            Count++;
        }

        public T Dequeue()
        {
            if (bufferQueue.Count > 0)
            {
                T item = default(T);  // 从临时文件读取
                dataQueue.Enqueue(item);
            }
            Count--;
            return dataQueue.Dequeue();
        }
    }
}
