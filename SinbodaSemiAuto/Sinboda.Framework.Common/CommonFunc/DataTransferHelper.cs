using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Sinboda.Framework.Common.CommonFunc
{
    /// <summary>
    /// 数据转换帮助类
    /// </summary>
    public static class DataTransferHelper
    {
        /// <summary>
        /// 将List转换成DataTable
        /// </summary>
        /// <typeparam name="T">List数据的实体类型</typeparam>
        /// <param name="data">List数据</param>
        /// <returns>转换后的DataTable</returns>
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                dt.Columns.Add(property.Name, property.PropertyType);
            }
            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item) == null ? DBNull.Value : properties[i].GetValue(item);
                }
                dt.Rows.Add(values);
            }
            return dt;
        }

        /// <summary>
        /// 将DataTable转换成List
        /// </summary>
        /// <typeparam name="T">转换后的List实体类型</typeparam>
        /// <param name="dt">DataTable数据</param>
        /// <returns>转换后的List</returns>
        public static IList<T> ConvertToModel<T>(DataTable dt)
        {
            IList<T> ts = new List<T>();// 定义集合
            Type type = typeof(T); // 获得此模型的类型
            string tempName = "";
            foreach (DataRow dr in dt.Rows)
            {
                T t = default(T);
                PropertyInfo[] propertys = t.GetType().GetProperties();// 获得此模型的公共属性
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;
                    if (dt.Columns.Contains(tempName))
                    {
                        if (!pi.CanWrite) continue;
                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                ts.Add(t);
            }
            return ts;
        }

        /// <summary> 
        /// 数据表转键值对集合
        /// 把DataTable转成 List集合, 存每一行 
        /// 集合中放的是键值对字典,存每一列 
        /// </summary> 
        /// <param name="dt">数据表</param> 
        /// <returns>哈希表数组</returns> 
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            List<Dictionary<string, object>> list
                 = new List<Dictionary<string, object>>();

            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    dic.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                list.Add(dic);
            }
            return list;
        }

        /// <summary> 
        /// 数据集转键值对数组字典 
        /// </summary> 
        /// <param name="ds">数据集</param> 
        /// <returns>键值对数组字典</returns> 
        public static Dictionary<string, List<Dictionary<string, object>>> DataSetToDic(DataSet ds)
        {
            Dictionary<string, List<Dictionary<string, object>>> result = new Dictionary<string, List<Dictionary<string, object>>>();

            foreach (DataTable dt in ds.Tables)
                result.Add(dt.TableName, DataTableToList(dt));

            return result;
        }
        /// <summary>
        /// 数组转化为结构体
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="bytes">转换的数据</param>
        /// <param name="source">结构体</param>
        /// <param name="isBiggerBefore">是否大端在前</param>
        /// <returns></returns>
        public static bool BytesToStruct<T>(byte[] bytes, out T source, bool isBiggerBefore)
        {
            Type type = typeof(T);
            int nSize = Marshal.SizeOf(type);
            source = default(T);

            if (nSize > bytes.Length)
                return false;

            IntPtr pStruct = Marshal.AllocHGlobal(nSize);
            try
            {
                Marshal.Copy(bytes, 0, pStruct, nSize);
                source = (T)Marshal.PtrToStructure(pStruct, type);

                if (isBiggerBefore)
                    source = (T)ReverseTool.ReverseRecord(source);
            }
            finally
            {
                Marshal.FreeHGlobal(pStruct);
            }

            return true;
        }

        /// <summary>
        /// 结构体转化为数组
        /// </summary>
        /// <param name="obj">结构体</param>
        /// <param name="isBiggerBefore">是否大端在前</param>
        /// <returns></returns>
        public static byte[] StructToBytes(object obj, bool isBiggerBefore)
        {
            if (obj == null)
                return new byte[0];
            int nSize = Marshal.SizeOf(obj);
            byte[] bytes = new byte[nSize];
            IntPtr pStruct;
            pStruct = Marshal.AllocHGlobal(nSize);
            try
            {
                if (isBiggerBefore)
                    obj = ReverseTool.ReverseRecord(obj);

                Marshal.StructureToPtr(obj, pStruct, false);
                Marshal.Copy(pStruct, bytes, 0, nSize);
            }
            finally
            {
                Marshal.FreeHGlobal(pStruct);
            }

            return bytes;
        }

        /// <summary>
        /// 遍历结构体
        /// </summary>
        /// <param name="type">结构类型</param>
        /// <returns>结构体内所有字段长度list</returns>
        private static List<int> GetLengthOfEveryDataOfStruct(Type type)
        {
            List<int> list = new List<int>();
            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                if (fieldInfo.FieldType.Name.EndsWith("[]"))
                {
                    string typename = fieldInfo.FieldType.FullName;
                    typename = typename.Substring(0, typename.Length - 2);
                    int len = Marshal.SizeOf(Type.GetType(typename));
                    string strc = fieldInfo.CustomAttributes.ElementAt<CustomAttributeData>(0).NamedArguments[2].ToString();
                    string[] temp = strc.Split('=');
                    int count = int.Parse(temp[1]);
                    for (int i = 0; i < count; i++)
                    {
                        list.Add(len);
                    }
                }
                else
                {
                    list.Add(Marshal.SizeOf(fieldInfo.FieldType));
                }
            }
            return list;
        }

        /// <summary>
        /// 根据结构体类型，byte数组高低位转换
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <param name="list">结构体内所有字段长度list</param>
        /// <returns></returns>
        private static byte[] HighLowReverse(byte[] bytes, List<int> list)
        {
            int index = 0;
            foreach (int item in list)
            {
                for (int i = 0; i < item / 2; i++)
                {
                    byte t;
                    t = bytes[index + i];
                    bytes[index + i] = bytes[index + item - 1 - i];
                    bytes[index + item - 1 - i] = t;
                }
                index += item;
            }
            return bytes;
        }

        /// <summary>
        /// 指针转化为数组
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        public static byte[] IntPtrToBytes(IntPtr buff, int nSize)
        {
            if (nSize <= 0)
            {
                return null;
            }

            byte[] bytes = new byte[nSize];
            Marshal.Copy(buff, bytes, 0, nSize);

            return bytes;
        }

        /// <summary>
        /// 获取Windows\System32目录
        /// </summary>
        /// <returns></returns>
        public static string GetSystem32Path()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.System);
        }

        /// <summary>
        /// 获取Windows目录
        /// </summary>
        /// <returns></returns>
        public static string GetWindowPath()
        {
            return Path.GetDirectoryName(GetSystem32Path());
        }

        /// <summary>
        /// 获取应用程序路径
        /// </summary>
        /// <returns></returns>
        public static string GetAppPath()
        {
            return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        }

        /// <summary>
        /// 获取当前类库全路径名
        /// </summary>
        /// <returns></returns>
        public static string GetModuleFullName()
        {
            string strModuleFileName = Assembly.GetExecutingAssembly().CodeBase;
            int nStart = 8;// 去除file:///  
            strModuleFileName = strModuleFileName.Substring(nStart, strModuleFileName.Length - nStart).Replace("/", "\\");

            return strModuleFileName;
        }

        /// <summary>
        /// 获取当前类库路径
        /// </summary>
        /// <returns></returns>
        public static string GetModulePath()
        {
            return Path.GetDirectoryName(GetModuleFullName());
        }

        /// <summary>
        /// 检查文件路径并自动建立目录
        /// </summary>
        /// <param name="fileName"></param>
        public static void CheckFilePath(string fileName)
        {
            List<string> list = new List<string>();

            string strPath = Path.GetDirectoryName(fileName);

            while (!Directory.Exists(strPath))
            {
                list.Add(strPath);
                int nEnd = strPath.LastIndexOf('\\');

                if (nEnd < 3) //C:\
                {
                    break;
                }

                strPath = strPath.Substring(0, nEnd);
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                Directory.CreateDirectory(list[i]);
            }
        }

        /// <summary>
        /// 获取当前时区（秒）
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentTimeZone()
        {
            TimeZone zone = TimeZone.CurrentTimeZone;
            TimeSpan ts = zone.GetUtcOffset(DateTime.Now);
            return (long)ts.TotalSeconds;
        }

        /// <summary>
        /// C#日期类型转化为C++日期类型       
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToTime_t(DateTime dateTime)
        {
            long time_t;

            DateTime dtZero = new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan ts = dateTime - dtZero;
            time_t = ts.Ticks / 10000000 - GetCurrentTimeZone()/*28800*/;

            return time_t;
        }

        /// <summary>
        /// C++日期类型转化C#日期类型
        /// </summary>
        /// <param name="time_t"></param>
        /// <returns></returns>
        public static DateTime Time_tToDateTime(long time_t)
        {
            //time_t是世界时间， 比本地时间 少8小时(即28800秒)
            double seconds = time_t + GetCurrentTimeZone()/*28800*/;
            double secs = Convert.ToDouble(seconds);

            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(secs);

            return dt;
        }

        /// <summary>
        /// 读取文件版本号（迪瑞版本号格式）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileVersionToString(string fileName)
        {
            FileVersionInfo verInfo = FileVersionInfo.GetVersionInfo(fileName);
            return string.Format("v{0}.{1:D3}.{2:D3}", verInfo.FileMajorPart, verInfo.FileMinorPart, verInfo.FileBuildPart);
        }

        /// <summary>
        /// 获取应用程序版本号
        /// </summary>
        /// <returns></returns>
        public static string GetAppVersionToString()
        {
            return GetFileVersionToString(Process.GetCurrentProcess().MainModule.FileName);
        }

        /// <summary>
        /// 获取当前模块库版本号
        /// </summary>
        /// <returns></returns>
        public static string GetModuleVersionToString()
        {
            return GetFileVersionToString(GetModuleFullName());
        }

        /// <summary>
        /// 打开指定目录并定位到文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void OpenFolderAndSelectFile(string fileName)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + fileName;
            System.Diagnostics.Process.Start(psi);
        }

        /// <summary>
        /// 获取FtpURL的文件名
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string GetFtpFileName(string strUrl)
        {
            return Path.GetFileName(strUrl);
        }
    }
}
