using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.RegexLC
{
    /// <summary>
    /// 正则表达式限制
    /// </summary>
    public static class RegexLCClass
    {
        /// <summary>
        /// 单条码号限制，不带|
        /// </summary>
        public static string BarcodeRegexStr = @"^[_A-Z0-9a-z\>\<\;\:\]\[\-\=\!\@\#\$\*]*$";
        public static Regex BarcodeRegex = new Regex(BarcodeRegexStr);

        /// <summary>
        /// 多条码号限制，带|
        /// </summary>
        public static string MultipleBarcodeRegexStr = @"^[_A-Z0-9a-z\>\<\;\:\]\[\-\=\!\?\@\#\$\*\|]*$";
        public static Regex MultipleBarcodeRegex = new Regex(MultipleBarcodeRegexStr);

        /// <summary>
        /// 年龄限制 1-200
        /// </summary>
        public static string AgeRegexStr = "([1-9][0-9]{0,1}|[1][0-9][0-9]|200)";
        public static Regex AgeRegex = new Regex("^([1-9][0-9]{0,1}|[1][0-9][0-9]|200)$");

        /// <summary>
        /// 样本号范围限制 9999-9999
        /// </summary>
        public static string SampleNumberRangeRegexStr = @"^[1-9]{1}[0-9]{0,3}((-{1})|(-{1}[1-9]{1}[0-9]{0,3}))?$";
        public static Regex SampleNumberRangeRegex = new Regex(SampleNumberRangeRegexStr);

        /// <summary>
        /// 试剂盘外圈盘号范围限制 43-43
        /// </summary>
        public static string ReagentDiskOuterRangeRegexStr = @"^([1-9]{1}|[1-3]{1}[0-9]{0,1}|[4]{1}[0-3]{0,1})((-{1})|(-{1}[1-9]{1}|-{1}[1-3]{1}[0-9]{0,1}|-{1}[4]{1}[0-3]{0,1}))?$";
        public static Regex ReagentDiskOuterRangeRegex = new Regex(ReagentDiskOuterRangeRegexStr);

        /// <summary>
        /// 试剂盘内圈盘号范围限制 45-45
        /// </summary>
        public static string ReagentDiskInnerRangeRegexStr = @"^([1-9]{1}|[1-3]{1}[0-9]{0,1}|[4]{1}[0-5]{0,1})((-{1})|(-{1}[1-9]{1}|-{1}[1-3]{1}[0-9]{0,1}|-{1}[4]{1}[0-5]{0,1}))?$";
        public static Regex ReagentDiskInnerRangeRegex = new Regex(ReagentDiskInnerRangeRegexStr);

        /// <summary>
        /// 次数 1-1000
        /// </summary>
        public static string TimesRegexStr = "^([1-9][0-9]{0,2}|1000)?$";
        public static Regex TimesRegex = new Regex("^([1-9][0-9]{0,2}|1000)$");

        public static string FileNameRegexStr = @"^[^\\\/\>\<\|\:\*\?]*$";
        /// <summary>
        /// 文件名称输入限制
        /// </summary>
        public static Regex FileNameRegex = new Regex(FileNameRegexStr);
    }
}
