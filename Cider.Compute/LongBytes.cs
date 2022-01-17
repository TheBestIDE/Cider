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

        private ulong[] _dat;

        #endregion

        #region Property

        /// <summary>
        /// 实际存储的以字节为单位的长度
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
                for (int i = 0; i < WordCount; i++)
                {
                    if (_dat[i] == 0)
                        continue;
                    for (int j = (WordByteLength << 3) - 1; j >= 0; j--)
                    {
                        if ((_dat[i] >> j & 1) != 0)
                        {
                            return (WordCount - i - 1) * (WordByteLength << 3) + j + 1;
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
            _dat = new ulong[arrLength];
        }

        public LongBytes(long byteLength, ulong num)
        {
            ByteLength = byteLength;
            long arrLength = byteLength >> 3;
            _dat = new ulong[arrLength];
            _dat[^1] = num;
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
            for (int i = 0; i < _dat.Length; i++)
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
            var bytes = new LongBytes(longBytes.ByteLength);
            int leftShiftWord = len >> 6;   // 左移的数组索引数
            int leftShiftbit = len & 0x3F;  // 除去字节数后需要左移的位数
            // 移位溢出 全0
            if (leftShiftWord >= longBytes.WordCount)
                return bytes;

            // 左移索引数
            for (int i = 0; i < longBytes.WordCount - leftShiftWord; i++)
            {
                bytes._dat[i] = longBytes._dat[i + leftShiftWord];
            }
            for (int i = longBytes.WordCount - leftShiftWord; i < longBytes.WordCount; i++)
            {
                bytes._dat[i] = 0;
            }

            // 有小移位
            if (leftShiftbit != 0)
            {
                // 左移剩余的小移位
                for (int i = 0; i < longBytes.WordCount - leftShiftWord - 1; i++)
                {
                    bytes._dat[i] = (bytes._dat[i] << leftShiftbit) | (bytes._dat[i + 1] >> (WordByteLength * 8 - leftShiftbit));
                }
                bytes._dat[longBytes.WordCount - leftShiftWord - 1] <<= leftShiftbit;
            }

            return bytes;
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
            var bytes = new LongBytes(longBytes.ByteLength);
            int rightShiftWord = len >> 6;   // 右移的数组索引数
            int rightShiftbit = len & 0x3F;  // 除去字节数后需要右移的位数
            // 移位溢出 全0
            if (rightShiftWord >= longBytes._dat.Length)
                return bytes;

            // 右移索引数
            for (int i = longBytes.WordCount - 1; i >= rightShiftWord; i--)
            {
                bytes._dat[i] = longBytes._dat[i - rightShiftWord];
            }
            for (int i = rightShiftWord - 1; i >= 0; i--)
            {
                bytes._dat[i] = 0;
            }

            // 有小移位
            if (rightShiftbit != 0)
            {
                // 右移剩余的小移位
                for (int i = longBytes.WordCount - 1; i > rightShiftWord; i--)
                {
                    bytes._dat[i] = (bytes._dat[i] >> rightShiftbit) | (bytes._dat[i - 1] << (WordByteLength * 8 - rightShiftbit));
                }
                bytes._dat[rightShiftWord] >>= rightShiftbit;
            }

            return bytes;
        }

        /// <summary>
        /// 按位与
        /// </summary>
        public static LongBytes And(LongBytes left, LongBytes right)
        {
            var longer = left.ByteLength >= right.ByteLength ? left : right;
            var shorter = left.ByteLength < right.ByteLength ? left : right;
            var bytes = new LongBytes(longer.ByteLength);

            int i = longer.WordCount - 1;
            int j = shorter.WordCount - 1;
            for (; i >= 0 && j >= 0; i--, j--)
            {
                bytes._dat[i] = longer._dat[i] & shorter._dat[j];
            }

            for (; i >= 0; i--)
            {
                bytes._dat[i] = 0;
            }

            return bytes;
        }

        /// <summary>
        /// 按位或
        /// </summary>
        public static LongBytes Or(LongBytes left, LongBytes right)
        {
            var longer = left.ByteLength >= right.ByteLength ? left : right;
            var shorter = left.ByteLength < right.ByteLength ? left : right;
            var bytes = new LongBytes(longer.ByteLength);

            int i = longer.WordCount - 1;
            int j = shorter.WordCount - 1;
            for (; i >= 0 && j >= 0; i--, j--)
            {
                bytes._dat[i] = longer._dat[i] | shorter._dat[j];
            }

            for (; i >= 0; i--)
            {
                bytes._dat[i] = longer._dat[i];
            }

            return bytes;
        }

        /// <summary>
        /// 按位异或
        /// </summary>
        public static LongBytes Xor(LongBytes left, LongBytes right)
        {
            var longer = left.ByteLength >= right.ByteLength ? left : right;
            var shorter = left.ByteLength < right.ByteLength ? left : right;
            var bytes = new LongBytes(longer.ByteLength);

            int i = longer.WordCount - 1;
            int j = shorter.WordCount - 1;
            for (; i >= 0 && j >= 0; i--, j--)
            {
                bytes._dat[i] = longer._dat[i] ^ shorter._dat[j];
            }

            for (; i >= 0; i--)
            {
                bytes._dat[i] = longer._dat[i];
            }

            return bytes;
        }

        /// <summary>
        /// 按位取反
        /// </summary>
        public static LongBytes Not(LongBytes left)
        {
            var bytes = new LongBytes(left.ByteLength);

            for (int i = 0; i < left.WordCount; i++)
            {
                bytes._dat[i] = ~left._dat[i];
            }

            return bytes;
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
            // 长比特数比短比特数所占用存储空间的字数量差
            var byteDiff = longer.WordCount - shorter.WordCount;

            // 检查长比特数的高位是否为0
            for (int i = 0; i < byteDiff; i++)
            {
                // 长比特数长出的部分若不为0 则必然长比特数大
                if (longer._dat[i] != 0)
                    return 1;
            }

            // 检查对齐部分的大小
            for (int i = 0; i < shorter.WordCount; i++)
            {
                if (longer._dat[i + byteDiff] > shorter._dat[i])
                    return 1;
                else if (longer._dat[i + byteDiff] < shorter._dat[i])
                    return -1;
            }

            return 0;
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
            var bytes = new LongBytes(data.Length);

            for (int i = 0; i < bytes.WordCount; i++)
            {
                int j = i * WordByteLength;
                for (int k = j; k < j + WordByteLength; k++)
                {
                    bytes._dat[i] <<= 8;
                    bytes._dat[i] |= data[k];
                }
            }

            return bytes;
        }

        public static implicit operator byte[](LongBytes bytes)
        {
            byte[] bs = new byte[bytes.ByteLength];
            for (int i = 0; i < bytes.WordCount; i++)
            {
                int j = i * WordByteLength;
                for (int k = 0; k < WordByteLength; k++)
                {
                    bs[k + j] = (byte)(bytes._dat[i] >> ((WordByteLength - k - 1) << 3));
                }
            }
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
