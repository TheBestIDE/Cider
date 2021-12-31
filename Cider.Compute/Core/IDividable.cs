using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Math.Core
{
    /// <summary>
    /// 除法
    /// </summary>
    public interface IDividable
    {
        public object Divide(object value);
    }

    /// <summary>
    /// 泛型除法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDividable<T> : IDividable
    {
        public T Divide(T value);
    }
}
