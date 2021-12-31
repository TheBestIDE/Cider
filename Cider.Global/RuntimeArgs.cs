using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Global
{
    public static class RuntimeArgs
    {
        static RuntimeArgs()
        {
            Config = ConfigurationFactory.CreateInstance();
            Config.EnCoding = Encoding.UTF8;
            Config.HashAlgorithm = "SHA256";
            Config.BlockLength = 1024;
            Config.ServerPort = 8088;
        }

        public static Configuration Config { get; set; }
    }
}
