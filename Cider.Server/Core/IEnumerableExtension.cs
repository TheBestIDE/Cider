using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Server.Core
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> RemoveDuplication<T>(this IEnumerable<T> source)
        {
            List<T> list = new List<T>();
            foreach (T item in source)
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            return list;
        }
    }
}
