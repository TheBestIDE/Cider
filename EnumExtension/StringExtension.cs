using System;
using System.Collections.Generic;
using System.Text;

namespace Extension
{
    public static class StringExtension
    {
        /// <summary>
        /// 判断字符串是否在字符串数组中
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool IsInArray(this string str, string[] strs)
        {
            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i] == str)
                    return true;
            }
            return false;
        }
    }
}
