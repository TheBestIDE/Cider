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
        private string _hashAlgorithm;

        public Configuration()
        {
            this.EnCoding = Encoding.UTF8;
            this._hashAlgorithm = "SHA256";
            HashAlgorithm = "SHA256";
            BlockLength = 1024;
        }

        #region Property

        public override Encoding EnCoding { get; internal set; }

        /// <exception cref="ArgumentException"></exception>
        public override string HashAlgorithm
        { 
            get => _hashAlgorithm; 
            internal set
            {
                var nameList = Enum.GetNames(typeof(SupportHash));
                if (value.IsInArray(nameList))
                    _hashAlgorithm = value;
                else
                    throw new ArgumentException("哈希算法不支持");
            } 
        }

        public override int BlockLength { get; internal set; }

        public override int ServerPort { get; internal set; }

        #endregion
    }
}