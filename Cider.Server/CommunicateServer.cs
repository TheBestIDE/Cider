using Cider.Global;
using Cider.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Server
{
    internal class CommunicateServer : ApplicationLayer
    {
        protected TcpListener server;

        public override ApplicationHead Head { get; protected set; }

        public CommunicateServer()
        {
            Head = new ApplicationHead();
            server = new TcpListener(IPAddress.Any, RuntimeArgs.Config.ServerPort);
        }

        public void Listen()
        {
            server.Start();
            Console.WriteLine("Start Listening");
        }

        public override int Receive(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void Send(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void SetHead(ApplicationOption option)
        {
            Head.Option = (byte)option;
        }
    }
}
