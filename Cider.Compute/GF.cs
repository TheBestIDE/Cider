using System.Collections.Generic;

namespace Cider.Math
{
    /// <summary>
    /// 伽罗华域
    /// </summary>
    public class GF
    {
        protected ulong dat;

        /// <summary>
        /// 不可约多项式字典
        /// </summary>
        public static Dictionary<uint, ulong> PrimitiveDict { get; }

        /// <summary>
        /// 比特数
        /// </summary>
        public uint BitLenght { get; protected set; }

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

        public GF(uint bitnum)
        {
            dat = 0x0;
            BitLenght = bitnum;
        }

        public GF(uint bitnum, ulong b)
        {
            dat = b;
            BitLenght = bitnum;
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

        public static explicit operator byte(GF gf)
        {
            return (byte)gf.dat;
        }

        public static explicit operator ushort(GF gf)
        {
            return (ushort)gf.dat;
        }

        public static explicit operator uint(GF gf)
        {
            return (uint)gf.dat;
        }

        public static explicit operator ulong(GF gf)
        {
            return (ulong)gf.dat;
        }

        public static GF Add(GF left, GF right)
        {
            if (left.BitLenght != right.BitLenght)
                throw new ArgumentException("The left and right is not in the same Galois Field");
            ulong res = left.dat ^ right.dat;
            return new GF(left.BitLenght, res);
        }

        public static GF operator+(GF left, GF right)
        {
            return GF.Add(left, right);
        }

        public static GF operator-(GF left, GF right)
        {
            return GF.Add(left, right);
        }

        public static GF Multify(GF left, GF right)
        {
            if (left.BitLenght != right.BitLenght)
                throw new ArgumentException("The left and right is not same Field");
            ulong l_bits = left.dat, r_bits = right.dat;
            uint len = left.BitLenght;
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

            return result;
        }

        public static GF operator*(GF left, GF right)
        {
            return GF.Multify(left, right);
        }
    }
}
