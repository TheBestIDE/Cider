using System.Collections.Generic;

namespace Cider.Math
{
    /// <summary>
    /// 伽罗华域
    /// </summary>
    public class GF
    {
        private readonly LongBytes _dat;

        /// <summary>
        /// 不可约多项式字典
        /// </summary>
        public static Dictionary<long, LongBytes> PrimitiveDict { get; }

        /// <summary>
        /// 比特数
        /// </summary>
        public long BitLength { get; private set; }

        static GF()
        {
            PrimitiveDict = new Dictionary<long, LongBytes>
            {
                [4] = 0b10011,     // x^4 + x + 1
                [8] = (1UL << 8) | 0b11101,  // x^8 + x^4 + x^3 + x^2 + 1
                [16] = (1UL << 16) | (1 << 12) | 0b1011,     // x^16 + x^12 + x^3 + x + 1
                [32] = (1UL << 32) | (1 << 22) | 0b111,      // x^32 + x^22 + x^2 + x + 1 
                [64] = (1UL << 64) | 0b11011  // x^64 + x^4 + x^3 + x + 1
            };
        }

        public GF(GF num)
        {
            _dat = num._dat;
            BitLength = num.BitLength;
        }

        public GF(long bitnum) : this(bitnum, 0x0)
        {
        }

        public GF(long bitnum, ulong b)
        {
            _dat = b;
            BitLength = bitnum;
        }

        public GF(LongBytes longBytes)
        {
            _dat = new LongBytes(longBytes);
            BitLength = longBytes.ByteLength << 3;
        }

        #region Method

        public override string ToString()
        {
            return _dat.ToString();
        }

        /// <summary>
        /// 扩展欧几里得算法求乘法逆
        /// </summary>
        /// <returns></returns>
        public GF Inverse()
        {
            var byteLength = BitLength >> 3;
            LongBytes x1 = new(byteLength, 1);
            LongBytes x2 = new(byteLength, 0);
            LongBytes a = new LongBytes(this._dat);
            LongBytes b = new (PrimitiveDict[BitLength]);
            LongBytes zero = new(b.ByteLength);
            while (b != zero)
            {
                GF q, r;
                DivMod(a, b, out q, out r);
                a = b;
                b = r._dat;
                x1 = x2;
                x2 = x1 ^ q * x2;
            }
            return new GF(x1);
        }

        #region Static Method

        /// <summary>
        /// 获取有限域上的加法单位元
        /// </summary>
        /// <returns></returns>
        public static GF Zero(long bitnum)
        {
            return new GF(bitnum, 0x0);
        }

        /// <summary>
        /// 获取有限域上的乘法单位元
        /// </summary>
        /// <returns></returns>
        public static GF One(uint bitnum)
        {
            return new GF(bitnum, 0x1);
        }

        public static GF Add(GF left, GF right)
        {
            if (left.BitLength != right.BitLength)
                throw new ArgumentException("The left and right is not in the same Galois Field");
            ulong res = (ulong)(left._dat ^ right._dat);
            return new GF(left.BitLength, res);
        }

        public static GF Multify(GF left, GF right)
        {
            if (left.BitLength != right.BitLength)
                throw new ArgumentException("The left and right is not same Field");
            LongBytes l_bits = left._dat, r_bits = right._dat;
            long len = left.BitLength;
            var result = new LongBytes(left.BitLength >> 3);
            // 使用快速平方乘算法计算left*right
            while (r_bits != 0)
            {
                if ((r_bits & 1) != 0)
                {
                    result ^= l_bits;
                }

                LongBytes flag = l_bits & (new LongBytes(left.BitLength >> 3, 1UL) << (int)(len - 1));   // 记录最高位是否为1
                l_bits <<= 1;
                if (flag != 0)
                    l_bits ^= PrimitiveDict[len];   // 左移后溢出 需要减去不可约多项式以回到域内
                r_bits >>= 1;
            }

            return new GF(result);
        }

        /// <summary>
        /// 有限域带余除法
        /// </summary>
        /// <param name="dividend">被除数</param>
        /// <param name="divisor">除数</param>
        /// <param name="quotient">商</param>
        /// <param name="remainder">余数</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DivideByZeroException"></exception>
        public static void DivMod(GF dividend, GF divisor, out GF quotient, out GF remainder)
        {
            if (dividend.BitLength != divisor.BitLength)
                throw new ArgumentException("The dividend and divisor is not in same Field");
            if (divisor == Zero(divisor.BitLength))
                throw new DivideByZeroException();

            long byteLength = dividend.BitLength >> 3;
            LongBytes qtnt = new(byteLength);      // 商
            LongBytes rmdr = new(dividend._dat);   // 余数
            LongBytes one = new(byteLength, 1);    // 长字节类型1
            var digit_dividend = rmdr.HighestBitPosition;
            var digit_divisor = divisor._dat.HighestBitPosition;

            while (!(rmdr < divisor._dat))
            {
                var rec = digit_dividend - digit_divisor;
                rmdr ^= divisor._dat << (int)rec;
                qtnt |= one << (int)rec;
                digit_dividend = rmdr.HighestBitPosition;
            }
            quotient = new GF(qtnt);
            remainder = new GF(rmdr);
        }

        #region Type Conversion

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

        public static implicit operator GF(LongBytes longBytes)
        {
            return new GF(longBytes);
        }

        public static implicit operator LongBytes(GF gf)
        {
            return new LongBytes(gf._dat);
        }

        public static implicit operator byte[](GF gf)
        {
            var bcount = gf.BitLength / 8u;
            byte[] b = new byte[bcount];
            var bits = gf._dat;
            for (long i = bcount - 1; i >= 0; i--)
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

        #endregion

        #region Operator Overloading

        public static bool operator ==(GF left, GF right)
        {
            return left._dat == right._dat;
        }

        public static bool operator !=(GF left, GF right)
        {
            return !(left == right);
        }

        public static bool operator >(GF left, GF right)
        {
            return left._dat > right._dat;
        }

        public static bool operator <(GF Left, GF right)
        {
            return Left._dat < right._dat;
        }


        public static GF operator+(GF left, GF right)
        {
            return Add(left, right);
        }

        public static GF operator-(GF left, GF right)
        {
            return Add(left, right);
        }

        public static GF operator*(GF left, GF right)
        {
            return Multify(left, right);
        }

        #endregion

        #endregion

        #endregion
    }
}
