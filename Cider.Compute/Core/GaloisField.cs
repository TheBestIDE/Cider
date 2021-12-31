using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Math.Core
{
    /// <summary>
    /// 伽罗华域抽象类
    /// </summary>
    public abstract class GaloisField
    {
        /// <summary>
        /// 加法单位元
        /// </summary>
        public abstract int AddE { get; protected set; }

        /// <summary>
        /// 乘法单位元
        /// </summary>
        public abstract int MultipyE { get; protected set; }

        /// <summary>
        /// 2^N上的伽罗华域
        /// </summary>
        public abstract int N { get; protected set; }

        /// <summary>
        /// 求逆元
        /// </summary>
        /// <returns></returns>
        public abstract GaloisField GetInverseElement(GaloisField galoisField);

        /// <summary>
        /// 加法运算
        /// </summary>
        /// <param name="onther"></param>
        /// <returns></returns>
        public abstract GaloisField Add(GaloisField left, GaloisField right);

        /// <summary>
        /// 乘法运算
        /// </summary>
        /// <param name="onther"></param>
        /// <returns></returns>
        public abstract GaloisField Multipy(GaloisField left, GaloisField right);
    }
}
