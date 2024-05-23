using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sinboda.Framework.LIS.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// 去掉包头包尾
        /// </summary>
        public static string HL7RemoveBlockChar(string HL7Text, string StartBlockChar, string EndBlockChar)
        {
            return HL7Text.Replace(System.Convert.ToString((char)System.Convert.ToByte(StartBlockChar, 16)), "").Replace(
                System.Convert.ToString((char)System.Convert.ToByte(EndBlockChar, 16)) + System.Convert.ToString((char)System.Convert.ToByte(0x0d)), "").Replace(
                System.Convert.ToString((char)System.Convert.ToByte(0x0a)), "");
        }
        public static string ASTMRemoveBlockChar(string HL7Text, string StartBlockChar, string EndBlockChar)
        {
            return HL7Text.Replace(System.Convert.ToString((char)System.Convert.ToByte(StartBlockChar, 16)), "").Replace(
                System.Convert.ToString((char)System.Convert.ToByte(EndBlockChar, 16)), "");
        }
        #region 字符串与字节数组之间转化

        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        public static byte[] StrToBytes(string str)
        {
            return System.Text.Encoding.Default.GetBytes(str);
        }

        /// <summary>
        /// 字节数组转字符串
        /// </summary>
        public static string BytesToStr(byte[] bytes)
        {
            return System.Text.Encoding.Default.GetString(bytes);
        }

        #endregion

        #region 字符串、字节数组与数据流Stream之间转化
        /// <summary>
        /// 字节数组转数据流Stream
        /// </summary>
        public static Stream BytesToStream(byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        /// <summary>
        /// 字符串转数据流Stream
        /// </summary>
        public static Stream StrToStream(string str)
        {
            return BytesToStream(StrToBytes(str));
        }

        /// <summary>
        /// 数据流Stream转字节数组
        /// </summary>
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary>
        /// 数据流Stream转字符串
        /// </summary>
        public static string StreamToStr(Stream stream)
        {
            return BytesToStr(StreamToBytes(stream));
        }

        #endregion

        #region Base64字符与数据流Stream之间转化

        /// <summary>
        /// 数据流Stream转Base64字符
        /// </summary>
        public static string StreamToBase64(Stream stream)
        {
            return System.Convert.ToBase64String(StreamToBytes(stream));
        }

        /// <summary>
        /// Base64字符转数据流Stream
        /// </summary>
        public static Stream Base64ToStream(string base64Str)
        {
            return BytesToStream(System.Convert.FromBase64String(base64Str));
        }

        /// <summary>
        /// 本地文件转Base64字符
        /// </summary>
        public static string FileToBase64(string filePath)
        {
            FileStream fileStream = new FileStream(filePath,
                         FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                return StreamToBase64(fileStream);
            }
            catch
            {
                return "";
            }
            finally
            {
                fileStream.Close();
            }
        }


        /// <summary>
        /// Base64字符转本地文件
        /// </summary>
        public static bool Base64ToFile(string base64Str, string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            try
            {
                Stream stream = Base64ToStream(base64Str);
                fileStream.Write(StreamToBytes(stream), 0, (int)stream.Length);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                fileStream.Close();
            }
        }

        static System.Text.Encoding getEncoding(Encoding encoding)
        {
            string s = "";
            switch (encoding)
            {
                case Encoding.Default: s = "gb2312"; break;
                case Encoding.ASCII: s = "us-ascii"; break;
                case Encoding.BigEndianUnicode: s = "utf-16BE"; break;
                case Encoding.Unicode: s = "utf-16"; break;
                case Encoding.UTF32: s = "utf-32"; break;
                case Encoding.UTF7: s = "utf-7"; break;
                case Encoding.UTF8: s = "utf-8"; break;
            }
            return System.Text.Encoding.GetEncoding(s);
        }


        public static byte[] ToEncoding(Encoding srcEncoding, Encoding dstEncoding, byte[] bytes)
        {
            return System.Text.Encoding.Convert(getEncoding(srcEncoding), getEncoding(dstEncoding), bytes);
        }
        /// <summary>
        /// 将16进制字符串转成ASII码对应的字符串
        /// 例如："0x02"=>"\u0002"
        /// </summary>
        /// <param name="xStr">16进制字符串</param>
        /// <returns></returns>
        public static string GetASIIString(string xStr)
        {
            return System.Convert.ToString((char)System.Convert.ToByte(xStr, 16));
        }
        #endregion

    }
}
