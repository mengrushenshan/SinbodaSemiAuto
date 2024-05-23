using System;
using Sinboda.Framework.Common.Log;

namespace Sinboda.Framework.Common.CommonFunc
{
    /// <summary>
    /// 时间格式转换工具类
    /// </summary>
    public static class DateTimeFormatHelper
    {
        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="dttm">传入的时间</param>
        /// <param name="dtt">转换的显示类型</param>
        /// <returns>转换后的字符串</returns>
        public static string ToStringByDatetime(this DateTime dttm, DateTimeType dtt)
        {
            string datetimetype = "";
            switch (dtt)
            {
                case DateTimeType.StandardyyyyMMddHHmmss:
                    datetimetype = "yyyy-MM-dd HH:mm:ss";
                    break;
                case DateTimeType.StandardyyyyMMddHHmm:
                    datetimetype = "yyyy-MM-dd HH:mm";
                    break;
                case DateTimeType.StandardyyyyMMdd:
                    datetimetype = "yyyy-MM-dd";
                    break;
                case DateTimeType.StandardyyyyMM:
                    datetimetype = "yyyy-MM";
                    break;
                case DateTimeType.Standardyyyy:
                    datetimetype = "yyyy";
                    break;
                case DateTimeType.StandardMMdd:
                    datetimetype = "MM-dd";
                    break;
                case DateTimeType.StandardHHmmss:
                    datetimetype = "HH:mm:ss";
                    break;
                case DateTimeType.StandardHHmm:
                    datetimetype = "HH:mm";
                    break;
                case DateTimeType.Standardmmss:
                    datetimetype = "mm:ss";
                    break;
                case DateTimeType.yyyyMMddHHmmss:
                    datetimetype = "yyyyMMddHHmmss";
                    break;
                case DateTimeType.yyyyMMdd:
                    datetimetype = "yyyyMMdd";
                    break;
                case DateTimeType.yyyyMM:
                    datetimetype = "yyyyMM";
                    break;
                case DateTimeType.yyMMdd:
                    datetimetype = "yyMMdd";
                    break;
                case DateTimeType.yyMM:
                    datetimetype = "yyMM";
                    break;
                case DateTimeType.HHmmss:
                    datetimetype = "HHmmss";
                    break;
                case DateTimeType.HHmm:
                    datetimetype = "HHmm";
                    break;
                case DateTimeType.MM:
                    datetimetype = "MM";
                    break;
                case DateTimeType.dd:
                    datetimetype = "dd";
                    break;
                case DateTimeType.HH:
                    datetimetype = "HH";
                    break;
                case DateTimeType.mm:
                    datetimetype = "mm";
                    break;
                case DateTimeType.ss:
                    datetimetype = "ss";
                    break;
                case DateTimeType.yyMMddHHmmss:
                    datetimetype = "yyMMddHHmmss";
                    break;
                case DateTimeType.yyMMddHHmm:
                    datetimetype = "yyMMddHHmm";
                    break;
                case DateTimeType.yyyyMMddHHmm:
                    datetimetype = "yyyyMMddHHmm";
                    break;
                default:
                    datetimetype = "";
                    break;
            }
            return dttm.ToString(datetimetype);
        }

        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="str">传入的字符串</param>
        /// <param name="dtt">转换的显示类型</param>
        /// <returns>转换后的时间</returns>
        public static DateTime ToDateTime(this string str, DateTimeType dtt)
        {
            DateTime result = new DateTime(1900, 1, 1);
            switch (dtt)
            {
                case DateTimeType.StandardyyyyMMddHHmmss:
                    break;
                case DateTimeType.StandardyyyyMMddHHmm:
                    break;
                case DateTimeType.StandardyyyyMMdd:
                    break;
                case DateTimeType.StandardyyyyMM:
                    break;
                case DateTimeType.Standardyyyy:
                    break;
                case DateTimeType.StandardMMdd:
                    break;
                case DateTimeType.StandardHHmmss:
                    break;
                case DateTimeType.StandardHHmm:
                    break;
                case DateTimeType.Standardmmss:
                    break;
                case DateTimeType.yyyyMMddHHmmss:
                    break;
                case DateTimeType.yyyyMMdd:
                    break;
                case DateTimeType.yyyyMM:
                    str = str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-01";
                    break;
                case DateTimeType.yyMMdd:
                    break;
                case DateTimeType.yyMM:
                    break;
                case DateTimeType.HHmmss:
                    break;
                case DateTimeType.HHmm:
                    break;
                case DateTimeType.MM:
                    break;
                case DateTimeType.dd:
                    break;
                case DateTimeType.HH:
                    break;
                case DateTimeType.mm:
                    break;
                case DateTimeType.ss:
                    break;
                case DateTimeType.yyMMddHHmmss:
                    break;
                case DateTimeType.yyMMddHHmm:
                    break;
                case DateTimeType.yyyyMMddHHmm:
                    break;
                default:
                    break;
            }
            if (!DateTime.TryParse(str, out result))
            {
                try
                {
                    result = Convert.ToDateTime(str);
                }
                catch (Exception ex)
                {
                    result = new DateTime(1900, 1, 1);
                    LogHelper.logSoftWare.Error("DateTimeFormatHelper error", ex);
                }
            }
            return result;
        }
    }
    /// <summary>
    /// 时间格式枚举
    /// </summary>
    public enum DateTimeType
    {
        /// <summary>
        /// 标准日期时间格式
        /// </summary>
        StandardyyyyMMddHHmmss = 1,
        /// <summary>
        /// 标准日期时间格式（不包含秒）
        /// </summary>
        StandardyyyyMMddHHmm = 2,
        /// <summary>
        /// 标准日期格式
        /// </summary>
        StandardyyyyMMdd = 3,
        /// <summary>
        /// 标准日期格式（不包含日）
        /// </summary>
        StandardyyyyMM = 4,
        /// <summary>
        /// 标准日期格式（不包含月日）
        /// </summary>
        Standardyyyy = 5,
        /// <summary>
        /// 标准日期格式（不包含年）
        /// </summary>
        StandardMMdd = 6,
        /// <summary>
        /// 标准时间格式
        /// </summary>
        StandardHHmmss = 7,
        /// <summary>
        /// 标准时间格式（不包含秒）
        /// </summary>
        StandardHHmm,
        /// <summary>
        /// 标准时间格式（不包含时）
        /// </summary>
        Standardmmss,
        /// <summary>
        /// 日期时间格式
        /// </summary>
        yyyyMMddHHmmss,
        /// <summary>
        /// 日期格式
        /// </summary>
        yyyyMMdd,
        /// <summary>
        /// 年月
        /// </summary>
        yyyyMM,
        /// <summary>
        /// 短格式年月日
        /// </summary>
        yyMMdd,
        /// <summary>
        /// 短格式年月
        /// </summary>
        yyMM,
        /// <summary>
        /// 时间格式
        /// </summary>
        HHmmss,
        /// <summary>
        /// 时间格式（不包含秒）
        /// </summary>
        HHmm,
        /// <summary>
        /// 月
        /// </summary>
        MM,
        /// <summary>
        /// 日
        /// </summary>
        dd,
        /// <summary>
        /// 时
        /// </summary>
        HH,
        /// <summary>
        /// 分
        /// </summary>
        mm,
        /// <summary>
        /// 秒
        /// </summary>
        ss,
        /// <summary>
        /// 短格式日期时间
        /// </summary>
        yyMMddHHmmss,
        /// <summary>
        /// 短格式日期时间（不包含秒）
        /// </summary>
        yyMMddHHmm,
        /// <summary>
        /// 长格式日期时间（不包含秒）
        /// </summary>
        yyyyMMddHHmm
    }
}
