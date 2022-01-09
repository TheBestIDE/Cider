using System.Collections.Generic;

namespace Cider.Math
{
    /// <summary>
    /// 伽罗华域
    /// </summary>
    public class GF
    {
        private readonly ulong _dat;

        /// <summary>
        /// 不可约多项式字典
        /// </summary>
        public static Dictionary<uint, ulong> PrimitiveDict { get; }

        /// <summary>
        /// 比特数
        /// </summary>
        public uint BitLength { get; private set; }

        static GF()
        {
            PrimitiveDict = new Dictionary<uint, ulong>
            {
                [4] = 0b10011,     // x^4 + x + 1
                [8] = (1UL << 8) | 0b11101,  // x^8 + x^4 + x^3 + x^2 + 1
                [16] = (1UL << 16) | (1 << 12) | 0b1011,     // x^16 + x^12 + x^3 + x + 1
                [32] = (1UL << 32) | (1 << 22) | 0b111,      // x^32 + x^22 + x^2 + x + 1 
                [64] = (1UL << 64) | 0b11011  // x^64 + x^4 + x^3 + x + 1
            };
        }

        public GF(uint bitnum) : this(bitnum, 0x0)
        {
        }

        public GF(uint bitnum, ulong b)
        {
            _dat = b;
            BitLength = bitnum;
        }
        public override string ToString()
        {
            return _dat.ToString();
        }

        public static implicit operator GF(byte b)
        {
            return new GF(8, b);
        }

        public static implicit operator GF(ushort b)
        {
            return new GF(16, b);
        }

        public static implicit operator GF(uint b)
        {
            return new GF(32, b);
        }

        public static implicit operator GF(ulong b)
        {
            return new GF(64, b);
        }

        public static implicit operator byte[](GF gf)
        {
            var bcount = gf.BitLength / 8u;
            byte[] b = new byte[bcount];
            var bits = gf._dat;
            for (uint i = bcount - 1; i >= 0; i--)
            {
                b[i] = (byte)(bits & 0xFF);
                bits >>= 8;
            }
            return b;
        }

        public static implicit operator GF(byte[] bs)
        {
            ulong bits = 0;
            for (int i = 0; i < bs.Length; i++)
            {
                bits |= bs[i];
                bits <<= 8;
            }
            return new ((uint)bs.Length << 3, bits);
        }

        public static explicit operator byte(GF gf)
        {
            return (byte)gf._dat;
        }

        public static explicit operator ushort(GF gf)
        {
            return (ushort)gf._dat;
        }

        public static explicit operator uint(GF gf)
        {
            return (uint)gf._dat;
        }

        public static explicit operator ulong(GF gf)
        {
            return (ulong)gf._dat;
        }

        public static GF Add(GF left, GF right)
        {
            if (left.BitLength != right.BitLength)
                throw new ArgumentException("The left and right is not in the same Galois Field");
            ulong res = left._dat ^ right._dat;
            return new GF(left.BitLength, res);
        }

        public static GF operator+(GF left, GF right)
        {
            return Add(left, right);
        }

        public static GF operator-(GF left, GF right)
        {
            return Add(left, right);
        }

        public static GF Multify(GF left, GF right)
        {
            if (left.BitLength != right.BitLength)
                throw new ArgumentException("The left and right is not same Field");
            ulong l_bits = left._dat, r_bits = right._dat;
            uint len = left.BitLength;
            ulong result = 0UL;
            // 使用快速平方乘算法计算left*right
            while (r_bits != 0)
            {
                if ((r_bits & 1) != 0)
                {
                    result ^= l_bits;
                }

                ulong flag = l_bits & (1UL << (int)(len - 1));   // 记录最高位是否为1
                l_bits <<= 1;
                if (flag != 0)
                    l_bits ^= PrimitiveDict[len];   // 左移后溢出 需要减去不可约多项式以回到域内
                r_bits >>= 1;
            }

            return new GF(left.BitLength, result);
        }

        public static GF operator*(GF left, GF right)
        {
            return Multify(left, right);
        }
    }
}
