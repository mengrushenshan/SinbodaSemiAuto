using Sinboda.Framework.Common.ResourceExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Sinboda.Framework.Common.FileOperateHelper
{
    class SerializeHelper
    {
        /// <summary>
        /// 将一个对象序列化为二进制流
        /// </summary>
        /// <param name="p_value">欲序列化的对象</param>
        /// <returns>byte[] 二进制流</returns>
        public static byte[] BinarySerialize<T>(T p_value)
        {
            if (p_value == null) return null;
            byte[] serializedObject;
            MemoryStream ms = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(ms, p_value);
            ms.Seek(0, 0);
            serializedObject = ms.ToArray();
            ms.Close();
            return serializedObject;
        }

        /// <summary>
        /// 将二进制流反序列化为一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_serializedObject"></param>
        /// <returns></returns>
        public static T BinaryDeserialize<T>(byte[] p_serializedObject)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(p_serializedObject, 0, p_serializedObject.Length);
                ms.Seek(0, 0);
                BinaryFormatter b = new BinaryFormatter();
                return (T)b.Deserialize(ms);
            }
        }

        /// <summary>
        /// 将一个对象进行Xml序列化
        /// </summary>
        /// <typeparam name="T">欲序列化对象类型</typeparam>
        /// <param name="value">欲序列化的对象</param>
        /// <returns>>Xml数据流</returns>
        public static string XmlSerialize<T>(T value)
        {
            if (value == null) return null;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false);
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }

        /// <summary>
        /// 反序列化Xml数据流
        /// </summary>
        /// <typeparam name="T">返回对象的类型</typeparam>
        /// <param name="xml">Xml数据流</param>
        /// <returns>对象</returns>
        public static T XmlDeserialize<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return default(T);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlReaderSettings settings = new XmlReaderSettings();

            using (StringReader textReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }

        /// <summary>
        /// 将一个对象序列化成一个文件
        /// </summary>
        /// <param name="value">欲序列化的对象</param>
        /// <param name="filePath">文件及路径</param>
        /// <returns>是否序列化成功</returns>
        public static bool FileBinarySerialize<T>(T value, string filePath)
        {
            if (value == null) return false;
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Create);
                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(fileStream, value);
            }
            catch
            {
                return false;
                throw;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
            return true;
        }

        /// <summary>
        /// 将一个文件反序列化(Binary)为一个对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="filePath">文件及路径</param>
        /// <returns>对象</returns>
        public static T FileBinaryDeserialize<T>(string filePath)
        {
            FileStream fileStream = null;
            try
            {
                if (File.Exists(filePath) == false)
                    throw new FileNotFoundException(StringResourceExtension.GetLanguage(2692, "文件不存在"), filePath); //TODO 翻译
                fileStream = new FileStream(filePath, FileMode.Open);
                BinaryFormatter b = new BinaryFormatter();
                return (T)b.Deserialize(fileStream);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        /// <summary>
        /// 将一个对象序列化(Xml)成一个文件
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="value">欲序列化的对象</param>
        /// <param name="filePath">文件及路径</param>
        /// <returns>是否序列化成功</returns>
        public static bool FileXmlSerialize<T>(T value, string filePath)
        {
            if (value == null) return false;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;

            using (StringWriterWithEncoding textWriter = new StringWriterWithEncoding(Encoding.UTF8))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine(textWriter.ToString());
                }
            }
            return true;
        }

        /// <summary>
        /// 将一个文件反序列化(Xml)为一个对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="filePath">文件及路径</param>
        /// <returns>对象</returns>
        public static T FileXmlDeserialize<T>(string filePath)
        {
            if (!File.Exists(filePath)) return default(T);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlReaderSettings settings = new XmlReaderSettings();

            using (StreamReader sr = new StreamReader(filePath))
            {
                using (StringReader textReader = new StringReader(sr.ReadToEnd()))
                {
                    using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                    {
                        return (T)serializer.Deserialize(xmlReader);
                    }
                }
            }
        }
    }
}
