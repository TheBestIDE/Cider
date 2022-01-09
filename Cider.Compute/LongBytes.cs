using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cider.Math.Core;

namespace Cider.Math
{
    public struct LongBytes
    {
        private byte[] bytes;

        public long BitLength { get; private set; } = 0;

        public LongBytes(long bitLength)
        {
            this.BitLength = bitLength;
            bytes = new byte[bitLength];
        }

        public LongBytes(byte[] bs)
        {
            BitLength = bs.Length;
            bytes = bs;
        }

        #region Method

        #region Static Operator

        public static implicit operator LongBytes(long data)
        {
            return (ulong)data;
        }

        public static implicit operator LongBytes(ulong data)
        {
            var bs = new LongBytes(8);
            for (int i = 0, j = 7; i < 8; i++, j--)
            {
                bs.bytes[i] = (byte)(data >> (j << 3) & 0xFF); // 左移到对应位
            }
            return bs;
        }

        public static implicit operator LongBytes(int data)
        {
            return (uint)data;
        }

        public static implicit operator LongBytes(uint data)
        {
            var bs = new LongBytes(4);
            for (int i = 0, j = 3; i < 4; i++, j--)
            {
                bs.bytes[i] = (byte)(data >> (j << 3) & 0xFF); // 左移到对应位
            }
            return bs;
        }

        public static LongBytes operator <<(LongBytes longBytes, int len)
        {
            var bytes = new LongBytes(longBytes.BitLength);
            int leftShiftByte = len >> 3;   // 左移的字节数
            int leftShiftbit = len % 8;     // 除去字节数后需要左移的位数
            if (leftShiftByte >= longBytes.BitLength)
                return bytes;
            if (leftShiftbit == 0)
            {
                for (int i = 0; i < longBytes.BitLength - leftShiftByte; i++)
                {

                }
            }
            return bytes;
        }

        public static LongBytes operator >>(LongBytes longBytes, int len)
        {
            var bytes = new LongBytes(longBytes.BitLength);

            return bytes;
        }

        #endregion

        #endregion
    }
}
