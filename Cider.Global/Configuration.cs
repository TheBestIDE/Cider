using Cider.Global.Core;
using System.Text;

using Extension;

namespace Cider.Global
{ 
    /// <summary>
    /// 配置类
    /// </summary>
    public class Configuration : AConfiguration
    {

        public Configuration()
        {
            this.EnCoding = Encoding.UTF8;
            HashAlgorithm = SupportHash.SHA256;
            BlockLength = 1024;
        }

        #region Property

        public override Encoding EnCoding { get; internal set; }

        public override SupportHash HashAlgorithm { get; internal set; }

        public override int BlockLength { get; internal set; }

        public override int ServerPort { get; internal set; }

        #endregion
    }
}