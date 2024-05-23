using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sinboda.Framework.Common.CommonFunc
{
    /// <summary>
    /// 字符串处理帮助工具
    /// </summary>
    public static class StringOperateHelper
    {
        /// <summary>
        /// 返回正则表达式匹配的所有结果.
        /// </summary>
        /// <param name="strIn">字符串</param>
        /// <param name="regexStr">正则表达式</param>
        /// <returns>返回匹配的所有结果</returns>  
        public static List<string> MatchResults(string strIn, string regexStr)
        {
            List<string> results = new List<string>();
            if (strIn == null || regexStr == null)
            {
                return results;
            }
            // 定义一个Regex对象实例 
            Regex r = new Regex(regexStr);
            MatchCollection mc = r.Matches(strIn);
            // 在输入字符串中找到所有匹配 
            for (int i = 0; i < mc.Count; i++)
            {
                // 将匹配的字符串添在字符串列表中 
                results.Add(mc[i].Value);
            }
            return results;
        }

        /// <summary>
        /// 返回正则表达式匹配的所有位置.
        /// </summary>
        /// <param name="strIn">原始字符串</param>
        /// <param name="regexStr">正则表达式</param>
        /// <returns>返回匹配的所有位置 </returns>  
        public static List<int> MatchPositiones(string strIn, string regexStr)
        {
            List<int> matchposition = new List<int>();
            if (strIn == null || regexStr == null)
            {
                return matchposition;
            }
            // 定义一个Regex对象实例 
            Regex r = new Regex(regexStr);
            MatchCollection mc = r.Matches(strIn);
            // 在输入字符串中找到所有匹配 
            for (int i = 0; i < mc.Count; i++)
            {
                // 记录匹配字符的位置 
                matchposition.Add(mc[i].Index);
            }
            return matchposition;
        }

        /// <summary>
        /// 检验重复的单词（相重复的单词，中间以空格隔开）
        /// </summary>
        /// <param name="strIn">原字符串</param>
        /// <returns>以Dictionary（重复的单词, 第一个单词的位置）形式返回所有重复单词</returns>
        public static Dictionary<string, int> RepeatWords(string strIn)
        {
            Dictionary<string, int> resultDic = new Dictionary<string, int>();
            if (strIn == null)
            {
                return resultDic;
            }
            Regex rx = new Regex(@"\b(?<word>\w+)\s+(\k<word>)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(strIn);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                resultDic.Add(groups["word"].Value, groups[0].Index);
            }
            return resultDic;
        }

        /// <summary>
        /// 返回正则表达式第一次匹配的位置.
        /// </summary>     
        /// <param name="strIn">原字符串</param>
        /// <param name="regexStr">正则表达式</param>
        /// <returns>返回第一次匹配的位置</returns>
        public static int FirstMatchIndex(string strIn, string regexStr)
        {
            if (strIn == null || regexStr == null)
            {
                return -1;
            }
            // 定义一个Regex对象实例 
            Regex r = new Regex(regexStr);
            // 在字符串中匹配 
            Match m = r.Match(strIn);
            if (m.Success)
            {
                return m.Index;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 返回最后一次匹配的位置.
        /// </summary>
        /// <param name="strIn">字符串</param>
        /// <param name="regexStr">正则表达式</param>
        /// <returns>返回最后一次匹配的位置</returns>  
        public static int LastMatchPosition(string strIn, string regexStr)
        {
            List<int> matchposition = new List<int>();
            if (strIn == null || regexStr == null)
            {
                return -1;
            }
            // 定义一个Regex对象实例 
            Regex r = new Regex(regexStr);
            MatchCollection mc = r.Matches(strIn);
            // 在输入字符串中找到所有匹配 
            for (int i = 0; i < mc.Count; i++)
            {
                // 记录匹配字符的位置 
                matchposition.Add(mc[i].Index);
            }
            return matchposition[mc.Count - 1];
        }

        /// <summary>
        /// 清除掉除 @、-（连字符）和 .（句点）以外的所有非字母数字字符
        /// </summary>
        /// <param name="strIn">原字符串</param>
        /// <returns>处理后的字符串</returns>
        public static string CleanInput(string strIn)
        {
            if (strIn == null)
            {
                return string.Empty;
            }
            return Regex.Replace(strIn, @"[^\w\.@-]", "");
        }

        /// <summary>
        /// 清除特定的字符串
        /// </summary>
        /// <param name="strIn1">原字符串</param>
        /// <param name="strIn2">需清除的字符串</param>
        /// <returns>完成消除操作的字符串</returns>
        public static string DelSpecStr(string strIn1, string strIn2)
        {
            if (strIn1 == null || strIn2 == null)
            {
                return string.Empty;
            }
            return Regex.Replace(strIn1, @strIn2, "");
        }

        /// <summary>
        /// 用一个字符串替代某个字符串的子字符串
        /// </summary>
        /// <param name="strIn1">原字符串</param>
        /// <param name="strIn2">被替代的子字符串</param>
        /// <param name="strIn3">替代字符串</param>
        /// <returns>完成替代操作的字符串</returns>
        public static string StrReplace(string strIn1, string strIn2, string strIn3)
        {
            if (strIn1 == null || strIn2 == null || strIn3 == null)
            {
                return string.Empty;
            }
            return Regex.Replace(strIn1, @strIn2, strIn3);
        }

        /// <summary>
        /// 从完整路径中提取文件名
        /// </summary>
        /// <param name="fullName">文件的完整路径</param>
        /// <returns>返回文件名</returns>
        public static string GetFileName(string fullName)
        {
            if (fullName == null)
            {
                return string.Empty;
            }
            Regex r = new Regex(@"\\[^\\]*$");
            Match m = r.Match(fullName);
            if (m.Success)
            {
                return m.Value.Substring(1);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 从完整路径中提取文件名前面的路径
        /// </summary>
        /// <param name="fullName">文件的完整路径</param>
        /// <returns>返回文件名前面的路径</returns>
        public static string GetFilePath(string fullName)
        {
            if (fullName == null)
            {
                return string.Empty;
            }
            Regex r = new Regex(@"\\[^\\]*$");
            Match m = r.Match(fullName);
            if (m.Success)
            {
                return fullName.Substring(0, m.Index);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 返回第一个匹配的值.
        /// </summary>
        /// <param name="strIn">字符串</param>
        /// <param name="regexStr">正则表达式</param>
        /// <returns>返回第一个匹配的值</returns>  
        public static string FirstMatchValue(string strIn, string regexStr)
        {
            if (strIn == null || regexStr == null)
            {
                return null;
            }
            // 定义一个Regex对象实例 
            Regex r = new Regex(regexStr);
            // 在字符串中匹配 
            Match m = r.Match(strIn);
            if (m.Success)
            {
                return m.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 返回最后一次匹配的值.
        /// </summary>
        /// <param name="strIn">字符串</param>
        /// <param name="regexStr">正则表达式</param>
        /// <returns>返回最后一次匹配的值</returns>  
        public static string LastMatchValue(string strIn, string regexStr)
        {
            if (strIn == null || regexStr == null)
            {
                return string.Empty;
            }
            List<string> results = new List<string>();
            // 定义一个Regex对象实例 
            Regex r = new Regex(regexStr);
            MatchCollection mc = r.Matches(strIn);
            // 在输入字符串中找到所有匹配 
            for (int i = 0; i < mc.Count; i++)
            {
                // 将匹配的字符串添在字符串列表中 
                results.Add(mc[i].Value);
            }
            return results[mc.Count - 1];
        }

        /// <summary>
        /// 截取第一个匹配项的前面的子字符串.
        /// </summary>
        /// <param name="strIn">字符串</param>
        /// <param name="regexStr">正则表达式</param>
        /// <returns>第一个匹配项的前面的子字符串</returns>  
        public static string FindFrontSubstring(string strIn, string regexStr)
        {
            if (strIn == null || regexStr == null)
            {
                return string.Empty;
            }
            // 定义一个Regex对象实例 
            Regex r = new Regex(regexStr);
            // 在字符串中匹配 
            Match m = r.Match(strIn);
            if (m.Success)
            {
                return strIn.Substring(0, m.Index);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 截取第一个匹配项的后面的子字符串.
        /// </summary>
        /// <param name="strIn">字符串</param>
        /// <param name="regexStr">正则表达式</param>
        /// <returns>第一个匹配项的后面的子字符串</returns>  
        public static string FindAfterSubstring(string strIn, string regexStr)
        {
            if (strIn == null || regexStr == null)
            {
                return string.Empty;
            }
            // 定义一个Regex对象实例 
            Regex r = new Regex(regexStr);
            // 在字符串中匹配 
            Match m = r.Match(strIn);
            if (m.Success && strIn.Length - m.Index - 1 > 0)
            {
                return strIn.Substring(m.Index + 1, strIn.Length - m.Index - 1);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 截取第一个匹配项和最后一个匹配项之间的子字符串.
        /// </summary>
        /// <param name="strIn">字符串</param>
        /// <param name="regexStr">正则表达式</param>
        /// <returns>第一个匹配项和最后一个匹配项之间的子字符串</returns>  
        public static string FindInnerSubstring(string strIn, string regexStr)
        {
            if (strIn == null || regexStr == null)
            {
                return string.Empty;
            }
            // 定义一个Regex对象实例 
            Regex r = new Regex(regexStr);
            MatchCollection mc = r.Matches(strIn);
            if (mc.Count > 1)
            {
                if (mc[mc.Count - 1].Index - mc[0].Index - 1 > 0)
                {
                    string result = strIn.Substring(mc[0].Index + 1, mc[mc.Count - 1].Index - mc[0].Index - 1);
                    return result;
                }
            }
            return "";
        }

        /// <summary>
        /// 判断一个字符串是否在一个字符串数组中
        /// </summary>
        /// <param name="Strings">字符串数组</param>
        /// <param name="key">字符串</param>
        /// <returns>是否存在</returns>
        public static bool Contains(string[] Strings, string key)
        {
            if (Strings != null && !string.IsNullOrEmpty(key))
            {
                foreach (string item in Strings)
                {
                    if (item == key)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断一个对象是否在一个对象数组中
        /// </summary>
        /// <param name="Objs">对象数组</param>
        /// <param name="obj">对象</param>
        /// <returns>是否存在</returns>
        public static bool Contains(Array Objs, object obj)
        {
            if (Objs != null && obj != null)
            {
                foreach (object item in Objs)
                {
                    if (item.Equals(obj))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 按设置保留测试值小数位数
        /// </summary>
        /// <param name="itemvalue">测试值</param>
        /// <param name="digit">小数位数</param>
        /// <returns>保留小数后的值</returns>
        public static string ShowValueByDigits(string itemvalue, int digit)
        {
            if (digit > 15 || digit < 0)
            {
                return itemvalue;
            }
            string value = Math.Round(Convert.ToDouble(itemvalue), digit).ToString();
            string[] valuedigit = value.Split('.');
            //value带几位小数
            int valueleng = 0;
            if (valuedigit.Count() == 1)
            {
                valueleng = 0;
                if (digit != 0)
                    value += ".";
            }
            else
            {
                valueleng = valuedigit[1].Length;
            }
            if (valueleng != digit)
            {
                for (int zeroleng = 0; zeroleng < (digit - valueleng); zeroleng++)
                {
                    value += "0";
                }
            }
            return value;
        }
    }
}
