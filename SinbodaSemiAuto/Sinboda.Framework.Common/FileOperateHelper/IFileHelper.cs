using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sinboda.Framework.Common.FileOperateHelper
{
    /// <summary>
    /// INI文件帮助接口
    /// </summary>
    public interface IFileHelper
    {
        /// <summary>
        /// xml文件保存
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="p_entity">传入实体</param>
        /// <param name="p_savePath">文件路径</param>
        /// <returns>是否保存成功</returns>
        bool SaveXML<T>(T p_entity, string p_savePath) where T : class;

        /// <summary>
        /// xml文件读取
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="p_readPath">文件路径</param>
        /// <returns>返回XML文档实体类实例</returns>
        T ReadXML<T>(string p_readPath) where T : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="nodePath"></param>
        /// <param name="xmlNodeNameAttrName"></param>
        /// <param name="xmlNodeNameAttrValue"></param>
        /// <returns></returns>
        XmlNodeList XMLGetXmlNodeByName(string xmlFileName, string nodePath, string xmlNodeNameAttrName, string xmlNodeNameAttrValue);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlNodeName"></param>
        /// <param name="xmlNodeNameAttrName"></param>
        /// <param name="xmlNodeNameAttrValue"></param>
        /// <param name="sourceNode"></param>
        /// <returns></returns>
        bool XMLUpdateXmlNodeByName(string xmlFileName, string xpath, string xmlNodeName, string xmlNodeNameAttrName, string xmlNodeNameAttrValue, XmlNode sourceNode);

        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="p_section">区域符</param>
        /// <param name="p_key">键</param>
        /// <param name="p_iValue">值</param>
        /// <param name="p_writePath">文件路径</param>
        void SaveINI(string p_section, string p_key, string p_iValue, string p_writePath);

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="p_section">区域符</param>
        /// <param name="p_key">键</param>
        /// <param name="p_readPath">文件路径</param>
        /// <returns>值</returns>
        string ReadINI(string p_section, string p_key, string p_readPath);

        /// <summary>
        /// 删除INI文件章节
        /// </summary>
        /// <param name="p_section">区域符</param>
        /// <param name="p_key">键</param>
        /// <param name="p_delPath">文件路径</param>
        void DeleteINI(string p_section, string p_key, string p_delPath);

        /// <summary>
        /// 读取Txt文件
        /// </summary>
        /// <param name="p_readPath">文件路径</param>
        /// <returns></returns>
        string ReadTXT(string p_readPath);

        /// <summary>
        /// 保存Txt文件
        /// </summary>
        /// <param name="p_savePath">文件路径</param>
        /// <param name="p_contents">内容</param>
        void SaveTXT(string p_savePath, string p_contents);

        /// <summary>
        /// 删除Txt文件
        /// </summary>
        /// <param name="p_delPath">文件路径</param>
        /// <returns>是否删除成功</returns>
        bool DeleteTXT(string p_delPath);
    }
}
