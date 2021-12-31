using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cider.Math.Core;

namespace Cider.Math
{
    /// <summary>
    /// 长字节存储
    /// </summary>
    public class LongBytes : IComparable,
                             IComparable<LongBytes>,
                             IEquatable<LongBytes>

    {
        internal Integer block;
        internal int length;

        /// <summary>
        /// Object比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int CompareTo(object? obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (obj is int _int)
                return CompareTo(_int);
            if (obj is LongBytes bytes)
                return CompareTo(bytes);
            throw new ArgumentOutOfRangeException(nameof(obj));
        }

        public int CompareTo(LongBytes other)
        {
            return block.CompareTo(other.block);
        }

        /// <summary>
        /// Object类型比较
        /// 比较是否为相同引用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is int)
                return Equals((int)obj);
            if (obj is LongBytes)
                return Equals((LongBytes)obj);
            return false;
        }

        /// <summary>
        /// 与int类型数据比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(int other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// 与本类型数据比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(LongBytes other)
        {
            return CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return block.GetHashCode();
        }

        public static bool operator ==(LongBytes left, LongBytes right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LongBytes left, LongBytes right)
        {
            return !(left==right);
        }

        public static bool operator <(LongBytes left, LongBytes right)
        {
            return left.CompareTo(right)<0;
        }

        public static bool operator <=(LongBytes left, LongBytes right)
        {
            return left.CompareTo(right)<=0;
        }

        public static bool operator >(LongBytes left, LongBytes right)
        {
            return left.CompareTo(right)>0;
        }

        public static bool operator >=(LongBytes left, LongBytes right)
        {
            return left.CompareTo(right)>=0;
        }
    }
}
