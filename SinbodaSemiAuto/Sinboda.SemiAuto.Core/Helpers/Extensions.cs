using Mapster;
using MapsterMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Helpers
{
    internal static class Extensions
    {
        /// <summary>
        /// 通过枚举类型获取所有枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems<T>() where T : Enum
        {
            foreach (object item in Enum.GetValues(typeof(T)))
            {
                yield return (T)item;
            }
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string JsonSerialize(this object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(this string data)
        {
            try
            { return JsonConvert.DeserializeObject<T>(data); }
            catch(Exception e) 
            {
                return JsonConvert.DeserializeObject<T>(null);
            }
            
        }

        /// <summary>
        /// 检查路径是否存在 不存在则创建
        /// </summary>
        /// <param name="dir"></param>
        public static void CheckAndCreateDirectory(this string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="path"></param>
        public static bool CheckPath(this string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// 检查文件是否存在 不存在则创建
        /// </summary>
        /// <param name="path"></param>
        public static void CheckAndCreatePath(this string path)
        {
            if (!File.Exists(path))
                File.Create(path);
        }

        /// <summary>
        /// 检查字符串数据是否为空
        /// </summary>
        /// <param name="path"></param>
        public static bool IsNullOrWhiteSpace(this string data)
        {
            return string.IsNullOrWhiteSpace(data);
        }

        /// <summary>
        /// 判空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        /// <summary>
        /// bytes转字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteTo16Str(this byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("0x");
            foreach (var item in bytes)
            {
                sb.Append($"{item.ToString("X2")} ");
            }
            return sb.ToString();
        }
    }
}
