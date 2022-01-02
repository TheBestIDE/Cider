using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Net
{
    public class ApplicationService : ApplicationLayer
    {
        protected TcpClient client;

        public override ApplicationHead Head { get; protected set; }

        public ApplicationService()
        {
            Head = new ApplicationHead();
            client = new TcpClient();
        }

        public ApplicationService(TcpClient tcp)
        {
            Head = new ApplicationHead();
            client = tcp;
        }

        public override int Receive(byte[] data)
        {
            var stream = client.GetStream();
            return stream.Read(data);
        }

        public override void Send(byte[] data)
        {
            var stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        public override void SendHashList(string[] hashs)
        {
            MemoryStream ms = new MemoryStream();
        }

        public override void SendReturnNumber(int number)
        {
            throw new NotImplementedException();
        }

        public override void SendLinearResult(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void SetHead(ApplicationOption option)
        {
            throw new NotImplementedException();
        }

        public override string[] ReceiveHashList()
        {
            throw new NotImplementedException();
        }

        public override int ReceiveReturnNumber()
        {
            throw new NotImplementedException();
        }

        public override byte[] ReceiveLinearResult()
        {
            throw new NotImplementedException();
        }
    }
}
