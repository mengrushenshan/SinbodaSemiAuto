using System.Text.RegularExpressions;

namespace Sinboda.Framework.Common.CommonFunc
{
    /// <summary>
    /// 数据校验帮助工具
    /// </summary>
    public static class DataValidateHelper
    {
        /// <summary>
        /// 验证是否为合法的日期表示格式。 
        /// </summary>
        /// <param name="strIn">匹配字符串</param>
        ///  <param name="type">日期表示形式（0 不指定格式即连接符号可以是“-”，“/”，“.”其中的任何一种；1 连接符号是“-”；2 连接符号是“/”；3 连接符号是“.”</param>
        /// <returns>符合格式要求返回TRUE</returns>
        public static bool IsValidDate(string strIn, int type)
        {
            switch (type)
            {
                case 0:
                    return Regex.IsMatch(strIn, @"^((\d{4})|(\d{2}))(?<a>[-/.])((1[0-2])|(0?\d))\k<a>(([12]\d)|(3[01])|(0?\d))$");
                case 1:
                    return Regex.IsMatch(strIn, @"^((\d{4})|(\d{2}))(?<a>[-])((1[0-2])|(0?\d))\k<a>(([12]\d)|(3[01])|(0?\d))$");
                case 2:
                    return Regex.IsMatch(strIn, @"^((\d{4})|(\d{2}))(?<a>[/])((1[0-2])|(0?\d))\k<a>(([12]\d)|(3[01])|(0?\d))$");
                case 3:
                    return Regex.IsMatch(strIn, @"^((\d{4})|(\d{2}))(?<a>[.])((1[0-2])|(0?\d))\k<a>(([12]\d)|(3[01])|(0?\d))$");
                default:
                    return false;
            }
        }

        /// <summary>
        /// 验证是否为有效的时间格式 hh:mm:ss
        /// </summary>
        /// <param name="strIn">匹配字符串</param>
        /// <returns>符合格式要求返回TRUE</returns>
        public static bool IsValidTime(string strIn)
        {
            return Regex.IsMatch(strIn, @"^(((0\d)|(1[0-2]))(:[0-5]\d){2} ?(a|p)m)|((([0-1]\d)|(2[0-3]))(:[0-5]\d){2})$");
        }

        /// <summary>
        /// 验证是否为有效的IP格式  
        /// </summary>
        /// <param name="strIn">匹配字符串</param>
        /// <returns>符合格式要求返回TRUE</returns>
        public static bool IsValidIp(string strIn)
        {
            return Regex.IsMatch(strIn, @"^(((25[0-5])|(2[0-4]\d)|([01]?\d\d?))\.){3}((25[0-5])|(2[0-4]\d)|([01]?\d\d?))$");
        }

        /// <summary>
        /// 验证N位只由数字、字母组成的字符串
        /// </summary>
        /// <param name="strIn">匹配字符串</param>
        /// <param name="n">匹配字符串的位数</param>
        /// <returns>符合格式要求返回TRUE</returns>
        public static bool IsValidNumAndChar(string strIn, int n)
        {
            if (Regex.IsMatch(strIn, @"\."))
            {
                return false;
            }
            bool t;
            if (n > 0)
            {
                return t = Regex.IsMatch(strIn, @"^[a-zA-Z0-9]{" + n + "}$");
            }
            else
            {
                return t = Regex.IsMatch(strIn, @"^[a-zA-Z0-9]{0,}$");
            }
        }

        /// <summary>
        /// 验证字符串是否只有数字组成
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsComposeOnlyByNum(string strIn)
        {
            return Regex.IsMatch(strIn, @"^\d{0,}$");
        }

        /// <summary>
        /// 验证N位只由数字组成的符串
        /// </summary>
        /// <param name="strIn">匹配字符串</param>
        /// <param name="n">匹配字符串的位数</param>
        /// <returns>符合格式要求返回TRUE</returns>
        public static bool IsComposeByNum(string strIn, int n)
        {
            if (Regex.IsMatch(strIn, @"\."))
            {
                return false;
            }

            bool t = Regex.IsMatch(strIn, @"^[0-9]{" + n + "}$");
            return t;
        }

        /// <summary>
        /// 验证N位只由(大写或者小写)字母组成的字符串
        /// </summary>
        /// <param name="strIn">匹配字符串</param>
        /// <param name="n">匹配字符串的位数</param>
        /// <returns>符合格式要求返回TRUE</returns>
        public static bool IsComposeByChar(string strIn, int n)
        {
            if (Regex.IsMatch(strIn, @"\."))
            {
                return false;
            }

            bool t = Regex.IsMatch(strIn, @"^[a-zA-Z]{" + n + "}$");
            return t;
        }

        /// <summary>
        /// 验证是否包含数字
        /// </summary>
        /// <param name="strIn">匹配字符串</param>
        /// <returns>符合格式要求返回TRUE</returns>
        public static bool IsIncludeNum(string strIn)
        {
            Regex r = new Regex(@"\d+", RegexOptions.IgnoreCase);
            Match m = r.Match(strIn);
            return m.Success;
        }

        /// <summary>
        /// 验证是否含汉字
        /// </summary>
        /// <param name="strIn">匹配字符串</param>
        /// <returns>符合格式要求返回TRUE</returns>
        public static bool IsIncludeChinese(string strIn)
        {
            Regex r = new Regex(@"[\u4e00-\u9fa5]", RegexOptions.IgnoreCase);
            Match m = r.Match(strIn);
            return m.Success;
        }

        /// <summary>
        /// 只能输入汉字
        /// </summary>
        /// <param name="strIn">匹配字符串</param>
        /// <returns>符合格式要求返回TRUE</returns>
        public static bool IsOnlyComposeByChinese(string strIn)
        {
            return Regex.IsMatch(strIn, @"[\u4e00-\u9fff]+$");
        }

        /// <summary>
        /// 验证是否含特殊符号
        /// </summary>
        /// <param name="strIn">匹配字符串</param>
        /// <returns>符合格式要求返回TRUE</returns>
        public static bool IsIncludeSymb(string strIn)
        {
            Regex r = new Regex(@"\W+", RegexOptions.IgnoreCase);
            Match m = r.Match(strIn);
            return m.Success;
        }

        /// <summary>
        /// 验证是否为有效浮点数(小数点后最多保留m位)
        /// </summary>
        /// <param name="strIn">匹配字符串</param>
        /// <param name="m">小数点后最多保留m位</param>
        /// <returns>符合格式要求返回TRUE</returns> 
        public static bool IsValidFloutMaxMedian(ref string strIn, int m)
        {
            if (Regex.IsMatch(strIn, @"\d+\.\d{1," + m + "}$").ToString().Equals("True"))
            {
                strIn = string.Format("{0:N" + m + "}", System.Convert.ToDecimal(strIn)).ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 验证是否为有效浮点数(小数点后不限制位数)
        /// </summary>
        /// <param name="strIn">匹配字符串</param>
        /// <returns>符合格式要求返回TRUE</returns> 
        public static bool IsValidFlout(string strIn)
        {
            return Regex.IsMatch(strIn, @"\d+\.\d+$");
        }

        /// <summary>
        /// 匹配制定长度的浮点型(可以仅为整数)
        /// </summary>
        /// <param name="strIn"></param>
        /// <param name="m">整数位最大长度</param>
        /// <param name="n">小数位最大长度</param>
        /// <returns>符合格式要求返回TRUE</returns>
        public static bool IsValidFloatLen(string strIn, int m, int n)
        {
            string pattern = string.Format(@"(^0\.\d{{1,{1}}}[%,‰]?$)|(^[1-9]\d{{0,{0}}}[%,‰]?$)|^0[%,‰]?$|(^[1-9]\d{{0,{0}}}\.\d{{1,{1}}}[%,‰]?$)", m - 1, n);
            return Regex.IsMatch(strIn, pattern);
        }
    }
}
