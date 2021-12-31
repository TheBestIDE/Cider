using Cider.Global;
using Cider.Global.Core;

//var conf = ConfigurationFactory.CreateInstance();
//Console.WriteLine("Test Configuration:{0}, {1}, {2}", conf.EnCoding, conf.HashAlgorithm, conf.BlockLength);
namespace Cider.Test
{
    public class Program
    {
        public static int Main(string[] args)
        {
            byte[] buffer = new byte[] {2, 3, 4, 5};
            Edit(buffer);
            foreach(byte b in buffer)
            {
                Console.WriteLine(b);
            }
            return 0;
        }
        public static void Edit(byte[] buffer)
        {
            if (buffer == null)
                return;
            buffer[0] = 1;
        }
    }
}