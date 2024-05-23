using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Common.CommonFunc
{
    public static class StringParseHelper
    {
        /// <summary>
        /// 转换成DateTime
        /// </summary>
        /// <param name="input">传入数据</param>
        /// <param name="defaultvalue">转换失败后输出的默认值</param>
        /// <returns>转换后结果</returns>
        public static DateTime ParseByDefault(this string input, DateTime defaultvalue)
        {
            return input.ParseStringToType<DateTime>(delegate (string e)
            {
                return Convert.ToDateTime(input);
            }, defaultvalue);
        }

        /// <summary>
        /// 转换成decimal
        /// </summary>
        /// <param name="input">传入数据</param>
        /// <param name="defaultvalue">转换失败后输出的默认值</param>
        /// <returns>转换后结果</returns>
        public static decimal ParseByDefault(this string input, decimal defaultvalue)
        {
            return input.ParseStringToType<decimal>(delegate (string e)
            {
                return Convert.ToDecimal(input);
            }, defaultvalue);
        }

        /// <summary>
        /// 转换成double
        /// </summary>
        /// <param name="input">传入数据</param>
        /// <param name="defaultvalue">转换失败后输出的默认值</param>
        /// <returns>转换后结果</returns>
        public static double ParseByDefault(this string input, double defaultvalue)
        {
            return input.ParseStringToType<double>(delegate (string e)
            {
                return Convert.ToDouble(input);
            }, defaultvalue);
        }

        /// <summary>
        /// 转换成float
        /// </summary>
        /// <param name="input">传入数据</param>
        /// <param name="defaultvalue">转换失败后输出的默认值</param>
        /// <returns>转换后结果</returns>
        public static float ParseByDefault(this string input, float defaultvalue)
        {
            return input.ParseStringToType<float>(delegate (string e)
            {
                return Convert.ToSingle(input);
            }, defaultvalue);
        }

        /// <summary>
        /// 转换成long
        /// </summary>
        /// <param name="input">传入数据</param>
        /// <param name="defaultvalue">转换失败后输出的默认值</param>
        /// <returns>转换后结果</returns>
        public static long ParseByDefault(this string input, long defaultvalue)
        {
            return input.ParseStringToType<long>(delegate (string e)
            {
                return Convert.ToInt64(input);
            }, defaultvalue);
        }

        /// <summary>
        /// 转换成int
        /// </summary>
        /// <param name="input">传入数据</param>
        /// <param name="defaultvalue">转换失败后输出的默认值</param>
        /// <returns>转换后结果</returns>
        public static int ParseByDefault(this string input, int defaultvalue)
        {
            return input.ParseStringToType<int>(delegate (string e)
            {
                return Convert.ToInt32(input);
            }, defaultvalue);
        }

        /// <summary>
        /// 转换成short
        /// </summary>
        /// <param name="input">传入数据</param>
        /// <param name="defaultvalue">转换失败后输出的默认值</param>
        /// <returns>转换后结果</returns>
        public static short ParseByDefault(this string input, short defaultvalue)
        {
            return input.ParseStringToType<short>(delegate (string e)
            {
                return Convert.ToInt16(input);
            }, defaultvalue);
        }

        /// <summary>
        /// 转换成string
        /// </summary>
        /// <param name="input">传入数据</param>
        /// <param name="defaultvalue">转换失败后输出的默认值</param>
        /// <returns>转换后结果</returns>
        public static string ParseByDefault(this string input, string defaultvalue)
        {
            if (string.IsNullOrEmpty(input))
            {
                return defaultvalue;
            }
            return input;
        }

        /// <summary>
        /// 私有转换函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">传入数据</param>
        /// <param name="action">转换函数</param>
        /// <param name="defaultvalue">转换失败后输出的默认值</param>
        /// <returns>转换后结果</returns>
        private static T ParseStringToType<T>(this string input, Func<string, T> action, T defaultvalue) where T : struct
        {
            if (string.IsNullOrEmpty(input))
            {
                return defaultvalue;
            }
            try
            {
                return action(input);
            }
            catch (Exception ex)
            {
                Log.LogHelper.logSoftWare.Error(null, ex);
                return defaultvalue;
            }
        }
    }
}
