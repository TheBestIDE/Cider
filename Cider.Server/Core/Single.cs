using System.Collections.Generic;

namespace Cider.Server.Core
{
    /// <summary>
    /// 单个实例
    /// </summary>
    public class Single<T> where T : class, new()
    {
        private static Dictionary<Type, object> _tDict;

        static Single()
        {
            _tDict = new Dictionary<Type, object>();
        }

        public static T Instance
        {
            get
            {
                if (!_tDict.ContainsKey(typeof(T)))
                    _tDict[typeof(T)] = new T();    // 装箱
                return (T)_tDict[typeof(T)];        // 拆箱

            }
        }
    }
}