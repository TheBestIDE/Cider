using System.Security.Cryptography;
using System.Text;
using Cider.Global.Core;

namespace Cider.Hash
{
    public class HashHelper
    {
        protected HashAlgorithm hashAlgorithm;

        public HashHelper(SupportHash hash)
        {
            hashAlgorithm = CreateHashInstance(hash);
        }

        public static HashAlgorithm CreateHashInstance(SupportHash hash)
        {
            switch (hash)
            {
                case SupportHash.MD5: return MD5.Create();
                case SupportHash.SHA256: return SHA256.Create();
                case SupportHash.SHA384: return SHA384.Create();
                case SupportHash.SHA512: return SHA512.Create();
                default: return SHA256.Create();
            }
        }

        public string Compute(byte[] buffer)
        {
            var bhash = hashAlgorithm.ComputeHash(buffer);
            return ToHexString(bhash);
        }

        protected static string ToHexString(byte[] bhash)
        {
            StringBuilder builder = new ();
            foreach (byte b in bhash)
            {
                builder.Append(string.Format("{0:X2}", b));
            }
            return builder.ToString().Trim();
        }
    }
}