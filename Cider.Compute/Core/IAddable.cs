using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Math.Core
{
    public interface IAddable
    {
        public object Add(object value);
    }

    public interface IAddable<T> : IAddable
    {
        public T Add(T value);
    }
}
