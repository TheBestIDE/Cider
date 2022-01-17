using System.Reflection;
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
            Config.HashAlgorithm = Core.SupportHash.SHA256;
            Config.BlockLength = 4;
            Config.ServerPort = 8088;

            string location = Assembly.GetExecutingAssembly().Location;
            var platform = System.Environment.OSVersion.Platform;
            
            if (platform == PlatformID.Win32NT)
                Separator = '\\';   // Windows路径分隔符
            else
                Separator = '/';    // Unix路径分隔符
            int lastIndex = location.LastIndexOf(Separator);
            RuntimeDirectory = location.Substring(0, lastIndex + 1);
        }

        public static char Separator { get; }

        public static Configuration Config { get; set; }

        public static string RuntimeDirectory { get; }
    }
}
