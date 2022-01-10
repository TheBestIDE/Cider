using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

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

        protected TcpListener CreateListener()
        {
            return new TcpListener(IPAddress.Any, 8088);
        }

        protected TcpClient CreateClient()
        {
            return new TcpClient();
        }

        [TestMethod()]
        public void ApplicationServiceTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ApplicationServiceTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReceiveTest()
        {
            var listener = CreateListener();
            var client = CreateClient();
        }

        [TestMethod()]
        public void SendTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SendFileNameTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SendHashListTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SendReturnNumberTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SendLinearResultTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SendFileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReceiveFileNameTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReceiveHashListTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReceiveReturnNumberTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReceiveLinearResultTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReceiveFileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Assert.Fail();
        }
    }
}