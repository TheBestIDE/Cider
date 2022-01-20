using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Cider.Global.Core;
using Cider.Hash;
using System.IO;
using Cider.IO;

namespace Cider.Net.Tests
{
    [TestClass()]
    public class ApplicationServiceTests
    {
        static IPEndPoint iPEndPoint;

        static ApplicationServiceTests()
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            iPEndPoint = new IPEndPoint(ipAddress, 8088);
        }

        protected int blockLength = 4;

        protected TcpListener CreateListener()
        {
            return new TcpListener(IPAddress.Any, 8088);
        }

        protected TcpClient CreateClient()
        {
            return new TcpClient();
        }

        protected void InitApplicationService()
        {
            ApplicationService.EnCode = Encoding.UTF8;
            ApplicationService.HashByteLength = (int)SupportHash.SHA256 / 8;
            ApplicationService.BlockLength = blockLength;
        }

        [TestMethod()]
        public void RequestCommandTest() 
        {
            var listener = CreateListener();
            var client = CreateClient();
            listener.Start();
            var conn_trd = listener.AcceptTcpClientAsync();

            client.Connect(iPEndPoint);
            using var appConn = new ApplicationService(conn_trd.Result);
            using var appClient = new ApplicationService(client);

            // 异步测试
            var task1 = Task.Run(() => appClient.RequestUpload());
            var cmd1 = appConn.ReceiveRequestCommand();
            task1.Wait();
            var task2 = Task.Run(() => appClient.RequestDownload());
            var cmd2 = appConn.ReceiveRequestCommand();
            task2.Wait();

            listener.Stop();
            Assert.AreEqual(ApplicationRequestCommand.Upload, cmd1);
            Assert.AreEqual(ApplicationRequestCommand.Download, cmd2);

        }

        [TestMethod()]
        public void SendRcvTest()
        {
            var listener = CreateListener();
            var client = CreateClient();
            listener.Start();
            var conn_trd = listener.AcceptTcpClientAsync();

            client.Connect(iPEndPoint);
            using var appConn = new ApplicationService(conn_trd.Result);
            using var appClient = new ApplicationService(client);

            byte[] send_buffer = new byte[2];
            send_buffer[0] = 0x01; send_buffer[1] = 0x02;
            // 异步测试
            var task = Task.Run(() => appClient.Send(send_buffer));
            byte[] rcv_buffer = new byte[2];
            appConn.Receive(rcv_buffer);
            task.Wait();

            listener.Stop();
            Assert.AreEqual((byte)0x01, rcv_buffer[0]);
            Assert.AreEqual((byte)0x02, rcv_buffer[1]);
        }

        [TestMethod()]
        public void SendRcvFileNameTest()
        {
            InitApplicationService();
            var listener = CreateListener();
            var client = CreateClient();
            listener.Start();
            var conn_trd = listener.AcceptTcpClientAsync();

            client.Connect(iPEndPoint);
            using var appConn = new ApplicationService(conn_trd.Result);
            using var appClient = new ApplicationService(client);

            string expected = "信号.jpg"; 
            // 异步测试
            var task = Task.Run(() => appClient.SendFileName(expected));
            string rcv = appConn.ReceiveFileName();
            task.Wait();

            listener.Stop();
            Assert.AreEqual(expected, rcv);
        }

        [TestMethod()]
        [Timeout(10000)]
        public void SndRcvHashListTest()
        {
            var hashHelper = new HashHelper(SupportHash.SHA256);
            InitApplicationService();

            var listener = CreateListener();
            var client = CreateClient();
            string[] msgs = new string[] { "信号.jpg", "123456", "test123" };
            List<string> hashs = new List<string>();
            foreach (var msg in msgs)
            {
                var hash = hashHelper.Compute(Encoding.UTF8.GetBytes(msg));
                hashs.Add(hash);
            }

            listener.Start();
            var conn_trd = listener.AcceptTcpClientAsync();

            client.Connect(iPEndPoint);
            using var appConn = new ApplicationService(conn_trd.Result);
            using var appClient = new ApplicationService(client);
            // 异步测试
            var task = Task.Run(() => appClient.SendHashList(hashs.ToArray()));
            string[] rcv = appConn.ReceiveHashList();
            task.Wait();

            listener.Stop();
            Assert.AreEqual(hashs[0], rcv[0]);
            Assert.AreEqual(hashs[1], rcv[1]);
            Assert.AreEqual(hashs[2], rcv[2]);
        }

        [TestMethod()]
        public void ToByte()
        {
            int expt = 0x00100056;
            byte[] rst = ApplicationService.ToBytes(expt);
            Assert.AreEqual(0x56, rst[0]);
            Assert.AreEqual(0x00, rst[1]);
            Assert.AreEqual(0x10, rst[2]);
            Assert.AreEqual(0x00, rst[3]);
        }

        [TestMethod()]
        public void ToInt()
        {
            int expt = 0x00100056;
            byte[] rst = ApplicationService.ToBytes(expt);
            int rst_int = ApplicationService.ToInt(rst, 0);
            Assert.AreEqual(expt, rst_int);
        }

        [TestMethod()]
        public void SndRcvReturnNumberTest()
        {
            InitApplicationService();
            var listener = CreateListener();
            var client = CreateClient();
            listener.Start();
            var conn_trd = listener.AcceptTcpClientAsync();

            client.Connect(iPEndPoint);
            using var appConn = new ApplicationService(conn_trd.Result);
            using var appClient = new ApplicationService(client);

            int expected = 8;
            // 异步测试
            Task task = Task.Run(() => appClient.SendReturnNumber(expected));
            int rcv = appConn.ReceiveReturnNumber();
            task.Wait();

            listener.Stop();
            Assert.AreEqual(expected, rcv);
        }

        [TestMethod()]
        public void SndRcvLinearResultTest()
        {
            int length = 32;
            ThreadSafeBufferStream sndStream = new (length);
            byte[] sndBytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                sndBytes[i] = (byte)i;
            }
            sndStream.Write(sndBytes, 0, sndBytes.Length);

            InitApplicationService();
            var listener = CreateListener();
            var client = CreateClient();
            listener.Start();
            var conn_trd = listener.AcceptTcpClientAsync();

            client.Connect(iPEndPoint);
            using var appConn = new ApplicationService(conn_trd.Result);
            using var appClient = new ApplicationService(client);
            // 异步测试
            Action action = new (() => appClient.SendLinearResult(sndStream));
            Task task = new (action);

            task.Start();
            using var rcvStream = appConn.ReceiveLinearResult();
            task.Wait();
            
            for (int i = 0; i < rcvStream.Length; i++)
            {
                var rcvByte = rcvStream.ReadByte();
                Assert.AreEqual(sndBytes[i], rcvByte);
            }

            listener.Stop();
        }

        [TestMethod()]
        public void SndRcvFileTest()
        {
            int length = 1024;
            ThreadSafeBufferStream sndStream = new(length);
            byte[] sndBytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                sndBytes[i] = (byte)i;
            }
            sndStream.Write(sndBytes, 0, sndBytes.Length);
            // 初始化
            InitApplicationService();
            var listener = CreateListener();
            var client = CreateClient();
            // 等待连接
            listener.Start();
            var conn_trd = listener.AcceptTcpClientAsync();
            // 开始连接
            client.Connect(iPEndPoint);
            using var appConn = new ApplicationService(conn_trd.Result);
            using var appClient = new ApplicationService(client);
            // 异步测试
            Task task = Task.Run(() => appClient.SendFile(sndStream));
            var rcvNumber = appConn.ReceiveFile(out Stream? rcvStream);
            task.Wait();

            Assert.IsNotNull(rcvNumber);
            int rcvCnt = (int)rcvNumber * blockLength;
            Assert.AreEqual(length, rcvCnt);

            for (int i = 0; i < rcvCnt; i++)
            {
                var rcvByte = rcvStream.ReadByte();
                Assert.AreEqual(sndBytes[i], rcvByte);
            }

            listener.Stop();
        }
    }
}