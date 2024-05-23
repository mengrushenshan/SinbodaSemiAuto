using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Common.FileOperateHelper
{
    /// <summary>
    /// ini 文件读写帮助类
    /// </summary>
    public class INIHelper
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, byte[] retVal, int size, string filePath);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="section">区域符</param>
        /// <param name="key">键</param>
        /// <param name="iValue">值</param>
        /// <param name="path">文件路径</param>
        public static void Write(string section, string key, string iValue, string path)
        {
            WritePrivateProfileString(section, key, iValue, path);
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="section">区域符</param>
        /// <param name="key">键</param>
        /// <param name="path">文件路径</param>
        /// <returns>值</returns>
        public static string Read(string section, string key, string path)
        {
            StringBuilder stringBuilder = new StringBuilder(255);
            int privateProfileString = GetPrivateProfileString(section, key, "", stringBuilder, 255, path);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="nSize">The number of characters in the substring</param>
        /// <returns></returns>
        public static string Read(string section, string key, string path, int nSize)
        {
            StringBuilder stringBuilder = new StringBuilder(nSize);
            int privateProfileString = GetPrivateProfileString(section, key, "", stringBuilder, nSize, path);
            string tmp = stringBuilder.ToString();
            return tmp;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="section">区域符</param>
        /// <param name="key">键</param>
        /// <param name="path">文件路径</param>
        public static void Delete(string section, string key, string path)
        {
            Write(section, key, null, path);
        }
    }
}
