using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Global
{
    /// <summary>
    /// Configuration工厂模式
    /// </summary>
    public static class ConfigurationFactory
    {
        public static Configuration CreateInstance()
        {
            return new Configuration() 
            { 
                EnCoding = Encoding.UTF8, 
                HashAlgorithm = "SHA256", 
                BlockLength = 1024
            };
        }

        public static Configuration CreateInstance(Encoding en, string hashAlgorithm, int blockLength)
        {

            return new Configuration()
            {
                EnCoding = en,
                HashAlgorithm = hashAlgorithm,
                BlockLength = blockLength
            };
        }
    }
}
