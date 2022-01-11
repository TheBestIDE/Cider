using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.IO.Tests
{
    [TestClass()]
    public class ThreadSafeBufferStreamTests
    {
        [TestMethod()]
        public void RdWrtByteTest()
        {
            ThreadSafeBufferStream stream = new ThreadSafeBufferStream(8);
            byte[] buffer = new byte[8];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)i;
            }

            Action action = () =>
            {
                foreach (var b in buffer)
                {
                    stream.WriteByte(b);
                }
            };
            Task task = new Task(action);
            task.Start();

            for (int i = 0; i < 8; i++)
            {
                int b = stream.ReadByte();
                Assert.AreEqual(b, buffer[i]);
            }
            Assert.AreEqual(-1, stream.ReadByte());

            task.Wait();
        }

        [TestMethod()]
        public void ReadTest()
        {
            int length = 32;
            ThreadSafeBufferStream stream = new ThreadSafeBufferStream(length);
            byte[] buffer = new byte[length];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)i;
            }

            Action action = () =>
            {
                stream.Write(buffer);
            };
            Task task = new Task(action);
            task.Start();

            byte[] rst = new byte[length];
            int count = stream.Read(rst, 0, rst.Length);


            for (int i = 0; i < rst.Length; i++)
            {
                //int b = stream.ReadByte();
                Assert.AreEqual(buffer[i], rst[i]);
            }

            Assert.AreEqual(32, count);

            task.Wait();
        }

        [TestMethod()]
        public void WriteTest()
        {
            int length = 32;
            ThreadSafeBufferStream stream = new ThreadSafeBufferStream(length);
            byte[] buffer = new byte[length];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)i;
            }

            Action action = () =>
            {
                stream.Write(buffer);
            };
            Task task = new Task(action);
            task.Start();

            byte[] rst = new byte[length];

            for (int i = 0; i < length; i++)
            {
                int b = stream.ReadByte();
                Assert.AreEqual(buffer[i], (byte)b);
            }

            task.Wait();
        }
    }
}