using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Math
{
    public struct LongBytes
    {
        #region Field

        private readonly ulong[] _dat;

        #endregion

        #region Property

        /// <summary>
        /// 实际存储的信息的以字节为单位的长度
        /// </summary>
        public long ByteLength { get; private set; } = 0;

        /// <summary>
        /// 字数量
        /// </summary>
        public int WordCount { get => _dat.Length; }

        /// <summary>
        /// 最高有效比特位的位置
        /// <para>即：为1的最高比特位置，位置从1开始计数</para>
        /// <para>若为全0 则返回0</para>
        /// </summary>
        public long HighestBitPosition
        {
            get
            {
                for (int i = WordCount - 1; i >= 0; i--)
                {
                    if (_dat[i] == 0)
                        continue;
                    for (int j = 63; j >= 0; j--)
                    {
                        if ((_dat[i] >> j & 1) != 0)
                        {
                            return (i << 6) + j + 1;
                        }
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// 以Byte为单位的字长
        /// </summary>
        public static int WordByteLength { get => sizeof(ulong); }

        #endregion

        #region Constructor

        public LongBytes()
        {
            ByteLength = 8;
            _dat = new ulong[1];
        }

        public LongBytes(long byteLength)
        {
            this.ByteLength = byteLength;
            long arrLength = byteLength >> 3;
            long rmdLength = byteLength & 7;
            _dat = new ulong[rmdLength == 0 ? arrLength : arrLength + 1];
        }

        public LongBytes(long byteLength, ulong num)
        {
            ByteLength = byteLength;
            long arrLength = byteLength >> 3;
            long rmdLength = byteLength & 7;
            _dat = new ulong[rmdLength == 0 ? arrLength : arrLength + 1];
            _dat[0] = num;
        }

        public LongBytes(ulong[] data)
        {
            _dat = data;
            ByteLength = data.Length << 3;
        }

        public LongBytes(LongBytes bytes)
        {
            ByteLength = bytes.ByteLength;
            this._dat = new ulong[bytes.WordCount];
            Array.Copy(bytes._dat, this._dat, bytes.WordCount);
        }

        #endregion

        #region Method

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            // 为空不相等
            if (obj is null)
                return false;
            // 是LongBytes类型 比较是否相等
            if (obj is LongBytes num)
                return this == num;
            // 不是该类型 不相等
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach (var item in _dat)
            {
                hashCode ^= item.GetHashCode();
            }
            return hashCode;
        }

        public override string ToString()
        {
            string str = "0x";
            for (int i = _dat.Length - 1; i >= 0; i--)
            {
                str += _dat[i].ToString("X16");
            }
            return str;
        }

        #region Static Method

        /// <summary>
        /// 左移
        /// </summary>
        /// <param name="longBytes">操作数</param>
        /// <param name="len">左移位数</param>
        /// <returns>左移结果</returns>
        /// <exception cref="ArgumentException"></exception>
        public static LongBytes LeftShift(LongBytes longBytes, int len)
        {
            // 移位小于0 异常
            if (len < 0)
                throw new ArgumentException("左移数必须大于等于0", nameof(len));

            // 移位为0 不变
            if (len == 0)
                return new LongBytes(longBytes);

            // 移位大于0
            ulong[] bits;
            int leftShiftWord = len >> 6;   // 左移的数组索引数
            int leftShiftbit = len & 0x3F;  // 除去字节数后需要左移的位数

            // 有小移位
            if (leftShiftbit != 0)
            {
                // 左移溢出位都为0 则无需扩展
                if (longBytes._dat[^1] >> (64 - leftShiftbit) == 0)
                {
                    bits = new ulong[longBytes.WordCount + leftShiftWord];
                }
                else
                {
                    bits = new ulong[longBytes.WordCount + leftShiftWord + 1];
                    bits[^1] = longBytes._dat[^1] >> (64 - leftShiftbit);
                }
                for (int i = leftShiftWord + 1; i < longBytes.WordCount + leftShiftWord; i++)
                {
                    bits[i] = (longBytes._dat[i - leftShiftWord] << leftShiftbit) | (longBytes._dat[i - leftShiftWord - 1] >> (64 - leftShiftbit));
                }
                bits[leftShiftWord] = longBytes._dat[0] << leftShiftbit;
            }
            else
            {
                bits = new ulong[longBytes.WordCount + leftShiftWord];
                // 左移索引数
                for (int i = leftShiftWord; i < bits.Length; i++)
                {
                    bits[i] = longBytes._dat[i - leftShiftWord];
                }
            }

            return new LongBytes(bits);
        }

        /// <summary>
        /// 右移
        /// </summary>
        /// <param name="longBytes">操作数</param>
        /// <param name="len">右移位数</param>
        /// <returns>右移结果</returns>
        /// <exception cref="ArgumentException"></exception>
        public static LongBytes RightShift(LongBytes longBytes, int len)
        {
            // 移位小于0 异常
            if (len < 0)
                throw new ArgumentException("右移数必须大于等于0", nameof(len));

            // 移位为0 不变
            if (len == 0)
                return new LongBytes(longBytes);

            // 移位大于0
            int rightShiftWord = len >> 6;   // 右移的数组索引数
            int rightShiftbit = len & 0x3F;  // 除去字节数后需要右移的位数
            // 移位溢出 全0
            if (rightShiftWord >= longBytes._dat.Length)
                return new LongBytes(new ulong[] { 0 });

            ulong[] bits = new ulong[longBytes.WordCount - rightShiftWord];
            // 有小移位
            if (rightShiftbit != 0)
            {
                for (int i = 0; i < bits.Length - 1; i++)
                {
                    bits[i] = (longBytes._dat[i + rightShiftWord] >> rightShiftbit) | (longBytes._dat[i + rightShiftWord + 1] << (64 - rightShiftbit));
                }
                bits[^1] = longBytes._dat[^1] >> rightShiftbit;
            }
            else
            {
                // 右移索引数
                for (int i = 0; i < bits.Length; i++)
                {
                    bits[i] = longBytes._dat[i + rightShiftWord];
                }
            }

            return new LongBytes(bits);
        }

        /// <summary>
        /// 按位与
        /// </summary>
        public static LongBytes And(LongBytes left, LongBytes right)
        {
            var longer = left.WordCount >= right.WordCount ? left : right;
            var shorter = left.WordCount < right.WordCount ? left : right;
            var longs = new ulong[longer.WordCount];

            for (int i = 0; i < shorter.WordCount; i++)
            {
                longs[i] = shorter._dat[i] & longer._dat[i];
            }

            return new LongBytes(longs);
        }

        /// <summary>
        /// 按位或
        /// </summary>
        public static LongBytes Or(LongBytes left, LongBytes right)
        {
            var longer = left.WordCount >= right.WordCount ? left : right;
            var shorter = left.WordCount < right.WordCount ? left : right;
            var longs = new ulong[longer.WordCount];

            Array.Copy(longer._dat, longs, longer.WordCount);
            for (int i = 0; i < shorter.WordCount; i++)
            {
                longs[i] |= shorter._dat[i];
            }

            return new LongBytes(longs);
        }

        /// <summary>
        /// 按位异或
        /// </summary>
        public static LongBytes Xor(LongBytes left, LongBytes right)
        {
            var longer = left.WordCount >= right.WordCount ? left : right;
            var shorter = left.WordCount < right.WordCount ? left : right;
            var longs = new ulong[longer.WordCount];

            Array.Copy(longer._dat, longs, longer.WordCount);
            for (int i = 0; i < shorter.WordCount; i++)
            {
                longs[i] ^= shorter._dat[i];
            }

            return new LongBytes(longs);
        }

        /// <summary>
        /// 按位取反
        /// </summary>
        public static LongBytes Not(LongBytes left)
        {
            var longs = new ulong[left.WordCount];

            for (int i = 0; i < left.WordCount; i++)
            {
                longs[i] = ~left._dat[i];
            }

            return new LongBytes(longs);
        }

        public static long Max(long left, long right)
        {
            return left > right ? left : right;
        }

        public static long Min(long left, long right)
        {
            return left < right ? left : right;
        }

        /// <summary>
        /// 比较两个数大小
        /// </summary>
        /// <param name="longer">比特长度较长的数</param>
        /// <param name="shorter">比特长度较短的数</param>
        /// <returns>大于返回正数 小于返回负数 相等返回0</returns>
        public static int Compare(LongBytes longer, LongBytes shorter)
        {
            // 检查长比特数的高位是否为0
            for (int i = longer.WordCount - 1; i >= shorter.WordCount; i--)
            {
                // 长比特数长出的部分若不为0 则必然长比特数大
                if (longer._dat[i] != 0)
                    return 1;
            }

            // 检查对齐部分的大小
            for (int i = shorter.WordCount - 1; i >= 0; i--)
            {
                if (longer._dat[i] > shorter._dat[i])
                    return 1;
                else if (longer._dat[i] < shorter._dat[i])
                    return -1;
            }

            return 0;
        }

        private static ulong[] BytesToLongs(byte[] bytes)
        {
            int arrLength = bytes.Length >> 3;
            int rmdLength = bytes.Length & 7;
            ulong[] longs = new ulong[rmdLength == 0 ? arrLength : arrLength + 1];

            for (int i = 0; i < longs.Length - 1; i++)
            {
                int j = i << 3;
                for (int k = j + WordByteLength - 1; k >= j; k--)
                {
                    longs[i] <<= 8;
                    longs[i] |= bytes[k];
                }
            }

            for (int k = bytes.Length - 1; k >= (longs.Length - 1) << 3; k--)
            {
                longs[^1] <<= 8;
                longs[^1] |= bytes[k];
            }

            return longs;
        }

        private static byte[] LongsToBytes(ulong[] longs)
        {
            byte[] bytes = new byte[longs.Length << 3];
            for (int i = 0; i < longs.Length; i++)
            {
                int j = i << 3;
                ulong ul = longs[i];
                for (int k = j; k < j + WordByteLength; k++)
                {
                    bytes[k] = (byte)ul;
                    ul >>= 8;
                }
            }
            return bytes;
        }

        #endregion

        #region Type Conversion

        // implicit

        public static implicit operator LongBytes(long data)
        {
            return (ulong)data;
        }

        public static implicit operator LongBytes(ulong data)
        {
            var bs = new LongBytes();
            bs._dat[0] = data;
            return bs;
        }

        public static implicit operator LongBytes(int data)
        {
            return (uint)data;
        }

        public static implicit operator LongBytes(uint data)
        {
            var bs = new LongBytes();
            bs._dat[0] = data;
            return bs;
        }

        public static implicit operator LongBytes(byte[] data)
        {
            ulong[] longs = BytesToLongs(data);

            return new LongBytes(longs);
        }

        public static implicit operator byte[](LongBytes bytes)
        {
            byte[] bs = LongsToBytes(bytes._dat);
            return bs;
        }

        // explicit

        public static explicit operator byte(LongBytes longBytes)
        {
            return (byte)longBytes._dat[^1];
        }

        public static explicit operator short(LongBytes longBytes)
        {
            return (short)longBytes._dat[^1];
        }

        public static explicit operator ushort(LongBytes longBytes)
        {
            return (ushort)longBytes._dat[^1];
        }

        public static explicit operator int(LongBytes longBytes)
        {
            return (int)longBytes._dat[^1];
        }

        public static explicit operator uint(LongBytes longBytes)
        {
            return (uint)longBytes._dat[^1];
        }

        public static explicit operator long(LongBytes longBytes)
        {
            return (long)longBytes._dat[^1];
        }

        public static explicit operator ulong(LongBytes longBytes)
        {
            return longBytes._dat[^1];
        }

        #endregion

        #region Operator Overloading

        public static bool operator >(LongBytes left, LongBytes right)
        {
            return left.WordCount >= right.WordCount 
                   ? Compare(left, right) > 0
                   : Compare(right, left) < 0;
        }

        public static bool operator <(LongBytes left, LongBytes right)
        {
            return left.WordCount >= right.WordCount
                   ? Compare(left, right) < 0
                   : Compare(right, left) > 0;
        }

        public static bool operator ==(LongBytes left, LongBytes right)
        {
            return left.WordCount >= right.WordCount
                   ? Compare(left, right) == 0
                   : Compare(right, left) == 0;
        }

        public static bool operator !=(LongBytes left, LongBytes right)
        {
            return !(left == right);
        }

        public static LongBytes operator <<(LongBytes longBytes, int len)
        {
            return LeftShift(longBytes, len);
        }

        public static LongBytes operator >>(LongBytes longBytes, int len)
        {
            return RightShift(longBytes, len);
        }

        public static LongBytes operator &(LongBytes left, LongBytes right)
        {
            return And(left, right);
        }

        public static LongBytes operator |(LongBytes left, LongBytes right)
        {
            return Or(left, right);
        }

        public static LongBytes operator ^(LongBytes left, LongBytes right)
        {
            return Xor(left, right);
        }

        public static LongBytes operator ~(LongBytes left)
        {
            return Not(left);
        }

        #endregion

        #endregion
    }
}
