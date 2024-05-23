using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using Sinboda.Framework.Common.Log;
using Sinboda.SemiAuto.Core.Models;

namespace Sinboda.SemiAuto.Core.Helpers
{
    internal class XmlHelper
    {
         /// <summary>
         /// 读取xml数据
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="fileName"></param>
         /// <returns></returns>
        public static T GetXmlData<T>(string fileName) where T : class
        {
            try
            {
                T result = null;
                string xmlString = GetXmlString(fileName);
                if (!xmlString.IsNullOrWhiteSpace())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    using (Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
                    {
                        using (XmlReader xmlReader = XmlReader.Create(xmlStream))
                        {
                            Object obj = xmlSerializer.Deserialize(xmlReader);
                            result = (T)obj;
                        }
                    }
                }
                else
                {
                    LogHelper.logSoftWare.Error($"file [{fileName}] is not exsisted !");
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 保存xml数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool SaveXml<T>(string fileName, T data) where T : class
        {
            bool result = false;
            try
            {
                string strXmlData = SerializeXml<T>(data);
                GlobalData.DicXml.CheckAndCreateDirectory();
                string nTempPath = $"{GlobalData.DicXml}\\{fileName}";
                //nTempPath.CheckAndCreatePath();
                //string nTempXML = EncodService.Encrypt(aXmlString, ModPlus.byteKey, ModPlus.byteIV);
                if (IOService.DocumentWrite(nTempPath, strXmlData) == true)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error(ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// 序列化为xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SerializeXml<T>(T data) where T : class
        {
            string xmlString = string.Empty;
            try
            {
                //定义忽略xsd和xsi的规则
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (MemoryStream ms = new MemoryStream())
                {
                    xmlSerializer.Serialize(ms, data, ns);
                    xmlString = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error(ex.ToString());
            }
            return xmlString;
        }

        /// <summary>
        /// 获取XML文件
        /// </summary>
        /// <returns></returns>
        private static string GetXmlString(string fileName)
        {
            string nTemlxml = null;
            try
            {
                GlobalData.DicXml.CheckAndCreateDirectory();
                string nTempPath = $"{GlobalData.DicXml}\\{fileName}";
                if (nTempPath.CheckPath())
                    nTemlxml = IOService.DocumentRead(nTempPath);
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error(ex.ToString());
            }
            return nTemlxml;
        }
    }
}
