using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Common.CommonFunc
{
    internal class ReverseTool
    {
        private static readonly Dictionary<int, FieldInfo[]> OrsPropertys = new Dictionary<int, FieldInfo[]>();
        private static readonly object SynchHelper = new object();

        /// <summary>
        /// 无符号短整数反转
        /// </summary>
        /// <param name="value"></param>
        public static void ReverseUInt16(ref UInt16 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            value = BitConverter.ToUInt16(data, 0);
        }

        /// <summary>
        /// 短整数反转
        /// </summary>
        /// <param name="value"></param>
        public static void ReverseInt16(ref Int16 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            value = BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        /// 无符号整数反转
        /// </summary>
        /// <param name="value"></param>
        public static void ReverseUInt32(ref UInt32 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            value = BitConverter.ToUInt32(data, 0);
        }

        /// <summary>
        /// 整数反转
        /// </summary>
        /// <param name="value"></param>
        public static void ReverseInt32(ref Int32 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            value = BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// 无符号长整数反转
        /// </summary>
        /// <param name="value"></param>
        public static void ReverseUInt64(ref UInt64 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            value = BitConverter.ToUInt64(data, 0);
        }

        /// <summary>
        /// 长整数反转
        /// </summary>
        /// <param name="value"></param>
        public static void ReverseInt64(ref Int64 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            value = BitConverter.ToInt64(data, 0);
        }

        /// <summary>
        /// 单精度浮点数反转
        /// </summary>
        /// <param name="value"></param>
        public static void ReverseFloat(ref float value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            value = BitConverter.ToSingle(data, 0);
        }

        /// <summary>
        /// 双精度整数反转
        /// </summary>
        /// <param name="value"></param>
        public static void ReverseDouble(ref double value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            value = BitConverter.ToDouble(data, 0);
        }

        public static object ReverseRecord(object record)
        {
            foreach (var fieldInfo in GetFields(record))
            {
                if (fieldInfo.FieldType == typeof(string) 
                    || fieldInfo.FieldType == typeof(sbyte)
                    || fieldInfo.FieldType == typeof(char)
                    || fieldInfo.FieldType == typeof(char[])
                    || fieldInfo.FieldType == typeof(byte) 
                    || fieldInfo.FieldType == typeof(byte[]))
                {
                    continue;
                }
                else if (fieldInfo.FieldType == typeof(UInt16))
                {
                    var val = (UInt16)(fieldInfo.GetValue(record));
                    ReverseUInt16(ref val);
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(Int16))
                {
                    var val = (Int16)(fieldInfo.GetValue(record));
                    ReverseInt16(ref val);
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(UInt32))
                {
                    var val = (UInt32)(fieldInfo.GetValue(record));
                    ReverseUInt32(ref val);
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(Int32))
                {
                    var val = (Int32)(fieldInfo.GetValue(record));
                    ReverseInt32(ref val);
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(UInt64))
                {
                    var val = (UInt64)(fieldInfo.GetValue(record));
                    ReverseUInt64(ref val);
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(Int64))
                {
                    var val = (Int64)(fieldInfo.GetValue(record));
                    ReverseInt64(ref val);
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(float))
                {
                    var val = (float)(fieldInfo.GetValue(record));
                    ReverseFloat(ref val);
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(double))
                {
                    var val = (double)(fieldInfo.GetValue(record));
                    ReverseDouble(ref val);
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(UInt16[]))
                {
                    var val = (UInt16[])(fieldInfo.GetValue(record));
                    if (val == null)
                        continue;

                    for (int i = 0; i < val.Length; i++)
                    {
                        ReverseUInt16(ref val[i]);
                    }
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(Int16[]))
                {
                    var val = (Int16[])(fieldInfo.GetValue(record));
                    if (val == null)
                        continue;

                    for (int i = 0; i < val.Length; i++)
                    {
                        ReverseInt16(ref val[i]);
                    }
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(UInt32[]))
                {
                    var val = (UInt32[])(fieldInfo.GetValue(record));
                    if (val == null)
                        continue;

                    for (int i = 0; i < val.Length; i++)
                    {
                        ReverseUInt32(ref val[i]);
                    }
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(Int32[]))
                {
                    var val = (Int32[])(fieldInfo.GetValue(record));
                    for (int i = 0; i < val.Length; i++)
                    {
                        ReverseInt32(ref val[i]);
                    }
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(UInt64[]))
                {
                    var val = (UInt64[])(fieldInfo.GetValue(record));
                    if (val == null)
                        continue;

                    for (int i = 0; i < val.Length; i++)
                    {
                        ReverseUInt64(ref val[i]);
                    }
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(Int64[]))
                {
                    var val = (Int64[])(fieldInfo.GetValue(record));
                    if (val == null)
                        continue;

                    for (int i = 0; i < val.Length; i++)
                    {
                        ReverseInt64(ref val[i]);
                    }
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(float[]))
                {
                    var val = (float[])(fieldInfo.GetValue(record));
                    if (val == null)
                        continue;

                    for (int i = 0; i < val.Length; i++)
                    {
                        ReverseFloat(ref val[i]);
                    }
                    fieldInfo.SetValue(record, val);
                }
                else if (fieldInfo.FieldType == typeof(double[]))
                {
                    var val = (double[])(fieldInfo.GetValue(record));
                    if (val == null)
                        continue;

                    for (int i = 0; i < val.Length; i++)
                    {
                        ReverseDouble(ref val[i]);
                    }
                    fieldInfo.SetValue(record, val);
                }
                else
                {
                    var val = fieldInfo.GetValue(record);
                    FieldInfo[] fs = val.GetType().GetFields();
                    if ((null != fs) && (fs.Length > 0))
                    {
                        val = ReverseRecord(val);
                        fieldInfo.SetValue(record, val);
                    }
                }
            }
            return record;
        }

        private static FieldInfo[] GetFields(object obj)
        {
            Type type = obj.GetType();
            var key = type.GetHashCode();
            if (OrsPropertys.ContainsKey(key))
                return OrsPropertys[key];

            lock (SynchHelper)
            {
                if (OrsPropertys.ContainsKey(key))
                    return OrsPropertys[key];

                OrsPropertys.Add(key, type.GetFields());
            }

            return OrsPropertys[key];
        }

    }
}
