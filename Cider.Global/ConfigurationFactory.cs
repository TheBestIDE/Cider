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
            return CreateInstance(Encoding.UTF8, Core.SupportHash.SHA256, 1024);
        }

        public static Configuration CreateInstance(Encoding en, Core.SupportHash hashAlgorithm, int blockLength)
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
