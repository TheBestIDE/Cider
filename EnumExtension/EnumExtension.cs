using System;
using System.Collections.Generic;
using System.Text;

namespace Extension
{
    public static class EnumExtension
    {
        /// <summary>
        /// 输出枚举对应的字符串
        /// </summary>
        /// <param name="enumobj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToEnumString(this Enum enumobj)
        {
            if (enumobj == null)
                throw new ArgumentNullException(nameof(enumobj));
            return enumobj.ToString("g");
        }
    }
}
