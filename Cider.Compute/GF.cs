using System.Collections.Generic;

namespace Cider.Math
{
    /// <summary>
    /// 伽罗华域
    /// </summary>
    public class GF
    {
        #region Field

        private readonly LongBytes _dat;

        #endregion

        #region Property

        /// <summary>
        /// 不可约多项式字典
        /// </summary>
        public static Dictionary<long, LongBytes> PrimitiveDict { get; }

        /// <summary>
        /// 比特数
        /// </summary>
        public long BitLength { get; private set; }

        #endregion

        #region Constructor

        static GF()
        {
            PrimitiveDict = new Dictionary<long, LongBytes>
            {
                [4] = 0b10011,     // x^4 + x + 1
                [8] = 0b100011011, // x^8 + x^4 + x^3 + x^1 + 1
                [16] = (1UL << 16) | (1 << 12) | 0b1011,     // x^16 + x^12 + x^3 + x + 1
                [32] = (1UL << 32) | (1 << 22) | 0b111,      // x^32 + x^22 + x^2 + x + 1 
                [64] = (1UL << 64) | 0b11011  // x^64 + x^4 + x^3 + x + 1
            };
        }

        public GF(GF num)
        {
            _dat = new (num._dat);
            BitLength = num.BitLength;
        }

        public GF(long bitnum) : this(bitnum, 0x0)
        {
        }

        public GF(LongBytes longBytes)
        {
            _dat = new LongBytes(longBytes);
            BitLength = longBytes.ByteLength << 3;
        }

        public GF(long bitnum, LongBytes longBytes)
        {
            _dat = new LongBytes(longBytes);
            BitLength = bitnum;
        }

        #endregion

        #region Method

        #region Override

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (obj is GF gf)
                return (_dat == gf._dat) && (BitLength == gf.BitLength);
            return false;
        }

        public override int GetHashCode()
        {
            return BitLength.GetHashCode() ^ _dat.GetHashCode();
        }

        public override string ToString()
        {
            return _dat.ToString();
        }

        #endregion

        /// <summary>
        /// 扩展欧几里得算法求乘法逆
        /// </summary>
        /// <returns></returns>
        public GF Inverse()
        {
            GF x1 = One(BitLength);
            GF x2 = Zero(BitLength);
            GF a = new (BitLength, _dat);
            GF b = new (BitLength, PrimitiveDict[BitLength]);
            GF zero = Zero(b.BitLength);
            while (b != zero)
            {
                DivMod(a, b, out GF q, out GF r);
                (a, b) = (b, new (BitLength, r._dat));
                (x1, x2) = (x2, x1 + q * x2);
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
            return new GF(bitnum);
        }

        /// <summary>
        /// 获取有限域上的乘法单位元
        /// </summary>
        /// <returns></returns>
        public static GF One(long bitnum)
        {
            return new GF(bitnum, 0x1);
        }

        /// <summary>
        /// 有限域上的加法
        /// <para>即按位求异或</para>
        /// </summary>
        /// <returns>和</returns>
        /// <exception cref="ArgumentException"></exception>
        public static GF Add(GF left, GF right)
        {
            if (left.BitLength != right.BitLength)
                throw new ArgumentException("The left and right is not in the same Galois Field");
            var res = left._dat ^ right._dat;
            return new GF(left.BitLength, res);
        }

        /// <summary>
        /// 有限域上的减法
        /// <para>实际上与加法相同</para>
        /// </summary>
        public static GF Subtract(GF left, GF right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// 有限域上的乘法
        /// </summary>
        /// <returns>积</returns>
        /// <exception cref="ArgumentException"></exception>
        public static GF Multify(GF left, GF right)
        {
            if (left.BitLength != right.BitLength)
                throw new ArgumentException("The left and right is not same Field");
            LongBytes l_bits = left._dat, r_bits = right._dat;
            long len = left.BitLength;
            LongBytes result = new (left._dat.ByteLength);
            // 使用快速平方乘算法计算left*right
            while (r_bits != 0)
            {
                if ((r_bits & 1) != 0)
                {
                    result ^= l_bits;
                }

                LongBytes flag = l_bits & (new LongBytes(left._dat.ByteLength, 1UL) << (int)(len - 1));   // 记录最高位是否为1
                l_bits <<= 1;
                if (flag != 0)
                    l_bits ^= PrimitiveDict[len];   // 左移后溢出 需要减去不可约多项式以回到域内
                r_bits >>= 1;
            }

            return new GF(len, result);
        }

        /// <summary>
        /// 使用乘法逆元计算有限域上的除法
        /// <para>被除数乘上除数的乘法逆</para>
        /// </summary>
        /// <param name="left">被除数</param>
        /// <param name="right">除数</param>
        /// <returns>商</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DivideByZeroException"></exception>
        public static GF Divide(GF left, GF right)
        {
            if (left.BitLength != right.BitLength)
                throw new ArgumentException("The left and right is not same Field");
            if (right == Zero(right.BitLength))
                throw new DivideByZeroException();
            return left * right.Inverse();
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

            long byteLength = dividend._dat.ByteLength;
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
            quotient = new GF(dividend.BitLength, qtnt);
            remainder = new GF(dividend.BitLength, rmdr);
        }

        #region Type Conversion

        public static implicit operator GF(byte b)
        {
            return new GF(8, (ulong)b);
        }

        public static implicit operator GF(ushort b)
        {
            return new GF(16, (ulong)b);
        }

        public static implicit operator GF(uint b)
        {
            return new GF(32, (ulong)b);
        }

        public static implicit operator GF(ulong b)
        {
            return new GF(64, b);
        }

        public static implicit operator GF(LongBytes longBytes)
        {
            return new GF(longBytes);
        }

        public static implicit operator byte[](GF gf)
        {
            var bcount = gf.BitLength / 8u;
            byte[] b = new byte[bcount];
            var bits = new LongBytes(gf._dat);
            for (long i = bcount - 1; i >= 0; i--)
            {
                b[i] = (byte)(bits & 0xFF);
                bits >>= 8;
            }
            return b;
        }

        public static implicit operator GF(byte[] bs)
        {
            LongBytes bits = bs;
            return new (bs.Length << 3, bits);
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

        public static explicit operator LongBytes(GF gf)
        {
            return new LongBytes(gf._dat);
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

        public static GF operator/(GF left, GF right)
        {
            return Divide(left, right);
        }

        #endregion

        #endregion

        #endregion
    }
}
