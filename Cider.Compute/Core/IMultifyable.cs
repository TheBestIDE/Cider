using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Math.Core
{
    /// <summary>
    /// 乘法接口
    /// </summary>
    public interface IMultifyable
    {
        public object Multify(object value);
    }

    /// <summary>
    /// 泛型乘法接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMultifyable<T> : IMultifyable
    {
        public T Multify(T value);
    }
}
