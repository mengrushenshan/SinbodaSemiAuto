using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Utils
{
    /// <summary>
    /// 各种类型转换帮助类
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        /// 将 <see cref="double"/> 转换为统一格式浮点数字符串
        /// <para>只有在需要将 <see cref="double"/> 类型转换为 <see cref="string"/> 存储到数据库时调用</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToDbString(this double obj)
        {
            return obj.ToString(CultureInfo.InvariantCulture);
        }



        /// <summary>
        /// 将数据库中浮点数字符串转换为 <see cref="double"/>
        /// <para>只有在需要将数据库中存储的 <see cref="string"/> 转换为 <see cref="double"/> 时调用</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double ToDouble(this string obj)
        {
            return Convert.ToDouble(obj, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 将 <see cref="DateTime"/> 转换为统一格式时间字符串
        /// <para>只有在需要将 <see cref="DateTime"/> 类型转换为 <see cref="string"/> 存储到数据库时调用</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToDbString(this DateTime obj)
        {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 将 <see cref="DateTime"/> 转换为统一格式时间字符串
        /// <para>只有在需要将 <see cref="DateTime"/> 类型转换为 <see cref="string"/> 存储到数据库时调用</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="format">使用指定的格式将当前 DateTime 对象的值转换为它的等效字符串表示形式</param>
        /// <returns></returns>
        public static string ToDbString(this DateTime obj, string format)
        {
            return obj.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 将数据库中时间格式字符串转换为 <see cref="DateTime"/>
        /// <para>只有在需要将数据库中存储的 <see cref="string"/> 转换为 <see cref="DateTime"/> 时调用</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string obj)
        {
            return Convert.ToDateTime(obj, CultureInfo.InvariantCulture);
        }
    }
}
