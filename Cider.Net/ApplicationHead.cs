using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Net
{
    /// <summary>
    /// 应用层头部
    /// </summary>
    public class ApplicationHead
    {
        public byte Option { get; set; }

        public uint DataLength { get; set; }

        public void SetOption(ApplicationOption opion)
        {
            Option = (byte)opion;
        }

        /// <summary>
        /// 将头部信息转化为字节序列
        /// </summary>
        public byte[] GetBytes()
        {
            byte[] byteArray = new byte[5];
            byteArray[0] = Option;
            // 小端法存储长度信息
            // 低字节存放低位数据
            uint length = DataLength;
            for (int i = 0; i < 4; i++)
            {
                byteArray[i + 1] = (byte)(length & 0xFF);
                length >>= 8;    // 下一个高位字节
            }

            return byteArray;
        }

        /// <summary>
        /// 从字节数组中读取头部信息
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static ApplicationHead CreateFromBytes(byte[] bArray)
        {
            ApplicationHead head = new ApplicationHead();
            if (bArray.Length >= 5)
            {
                head.Option = bArray[0];
                head.DataLength = 0;
                for (int i = 4; i > 0; i--)
                {
                    head.DataLength |= bArray[i];
                    head.DataLength <<= 8;
                }
            }
            else
            {
                throw new ArgumentException("需要的数组长度至少为5");
            }

            return head;
        }
    }
}
