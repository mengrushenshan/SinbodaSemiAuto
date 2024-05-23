using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sinboda.Framework.Common.FileOperateHelper
{
    /// <summary>
    /// XML操作类
    /// </summary>
    public class XMLHelper
    {
        #region 打印报告单字段属性名称
        public const string ATTR_CODE = "code";
        public const string ATTR_DIR = "dir";
        public const string ATTR_ISSHOW = "isShow";
        public const string ATTR_NAME = "name";
        public const string ATTR_FILE = "file";
        public const string ATTR_SEL = "defalutSelection";
        #endregion
        /// <summary>
        /// 根据节点创建节点
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlNodeName"></param>
        /// <param name="innerText"></param>
        /// <returns></returns>
        public static bool CreateXmlNodeByXPath(string xmlFileName, string xpath, string xmlNodeName, string innerText)
        {
            bool result = false;
            bool flag = false;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                XmlNode xmlNode = null;
                bool flag2 = xpath.Length > 0;
                if (flag2)
                {
                    xmlNode = documentElement.SelectSingleNode(xpath);
                }
                else
                {
                    xmlNode = documentElement;
                }
                bool flag3 = xmlNode != null;
                if (flag3)
                {
                    foreach (XmlNode xmlNode2 in xmlNode.ChildNodes)
                    {
                        bool flag4 = xmlNode2.Name.ToLower() == xmlNodeName.ToLower();
                        if (flag4)
                        {
                            flag = true;
                            break;
                        }
                    }
                    bool flag5 = !flag;
                    if (flag5)
                    {
                        XmlElement xmlElement = xmlDocument.CreateElement(xmlNodeName);
                        xmlElement.InnerXml = innerText;
                        xmlNode.AppendChild(xmlElement);
                    }
                    xmlDocument.Save(xmlFileName);
                    result = true;
                }
                bool flag6 = xmlNode == null;
                if (flag6)
                {
                    XmlElement newChild = xmlDocument.CreateElement(xpath);
                    documentElement.AppendChild(newChild);
                    xmlDocument.Save(xmlFileName);
                    xmlDocument.Load(xmlFileName);
                    XmlNode documentElement2 = xmlDocument.DocumentElement;
                    XmlElement newChild2 = xmlDocument.CreateElement(xmlNodeName);
                    documentElement2.SelectSingleNode(xpath).AppendChild(newChild2);
                    xmlDocument.Save(xmlFileName);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 根据节点更新节点
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlNodeName"></param>
        /// <param name="innerText"></param>
        /// <returns></returns>
        public static bool UpdateXmlNodeByXPath(string xmlFileName, string xpath, string xmlNodeName, string innerText)
        {
            bool result = false;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                bool flag = xpath.Length > 0;
                XmlNode xmlNode;
                if (flag)
                {
                    xmlNode = documentElement.SelectSingleNode(xpath);
                }
                else
                {
                    xmlNode = documentElement;
                }
                bool flag2 = xmlNode != null;
                if (flag2)
                {
                    foreach (XmlNode xmlNode2 in xmlNode.ChildNodes)
                    {
                        bool flag3 = xmlNode2.Name.ToLower() == xmlNodeName.ToLower();
                        if (flag3)
                        {
                            xmlNode2.InnerXml = innerText;
                            xmlDocument.Save(xmlFileName);
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 根据节点创建属性
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlNoleName"></param>
        /// <param name="xmlAttributeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool CreateXmlAttributeByXPath(string xmlFileName, string xpath, string xmlNoleName, string xmlAttributeName, string value)
        {
            bool result = false;
            bool flag = false;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                XmlNode xmlNode = documentElement.SelectSingleNode(xpath + xmlNoleName);
                bool flag2 = xmlNode == null;
                if (flag2)
                {
                    bool flag3 = xpath.Length > 0;
                    if (flag3)
                    {
                        CreateXmlNodeByXPath(xmlFileName, xpath.Substring(2, xpath.Length - 2), xmlNoleName.Substring(2, xmlNoleName.Length - 2), "");
                    }
                    else
                    {
                        CreateXmlNodeByXPath(xmlFileName, xpath, xmlNoleName, "");
                    }
                    xmlDocument.Load(xmlFileName);
                    bool flag4 = xpath.Length > 0;
                    XmlNode xmlNode2;
                    if (flag4)
                    {
                        xmlNode2 = xmlDocument.SelectSingleNode(xpath + xmlNoleName);
                    }
                    else
                    {
                        XmlNode documentElement2 = xmlDocument.DocumentElement;
                        xmlNode2 = documentElement2.SelectSingleNode(xpath + xmlNoleName);
                    }
                    XmlAttribute xmlAttribute = xmlDocument.CreateAttribute(xmlAttributeName);
                    xmlAttribute.Value = value;
                    xmlNode2.Attributes.Append(xmlAttribute);
                    xmlDocument.Save(xmlFileName);
                    result = true;
                }
                bool flag5 = xmlNode != null;
                if (flag5)
                {
                    foreach (XmlAttribute xmlAttribute2 in xmlNode.Attributes)
                    {
                        bool flag6 = xmlAttribute2.Name.ToLower() == xmlAttributeName.ToLower();
                        if (flag6)
                        {
                            flag = true;
                            break;
                        }
                    }
                    bool flag7 = !flag;
                    if (flag7)
                    {
                        XmlAttribute xmlAttribute3 = xmlDocument.CreateAttribute(xmlAttributeName);
                        xmlAttribute3.Value = value;
                        xmlNode.Attributes.Append(xmlAttribute3);
                        xmlDocument.Save(xmlFileName);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 根据节点更新属性
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlNoleName"></param>
        /// <param name="xmlAttributeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool UpdateXmlAttributeByXPath(string xmlFileName, string xpath, string xmlNoleName, string xmlAttributeName, string value)
        {
            bool result = false;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                XmlNode xmlNode = documentElement.SelectSingleNode(xpath + xmlNoleName);
                bool flag = xmlNode != null;
                if (flag)
                {
                    foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                    {
                        bool flag2 = xmlAttribute.Name.ToLower() == xmlAttributeName.ToLower();
                        if (flag2)
                        {
                            xmlAttribute.Value = value;
                            xmlDocument.Save(xmlFileName);
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 根据节点创建属性
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlNoleName"></param>
        /// <param name="xmlAttributeNameBase"></param>
        /// <param name="valueBase"></param>
        /// <param name="xmlAttributeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool CreateXmlAttributeByXPath(string xmlFileName, string xpath, string xmlNoleName, string xmlAttributeNameBase, string valueBase, string xmlAttributeName, string value)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            XmlDocument xmlDocument = new XmlDocument();
            bool result;
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                XmlNodeList xmlNodeList = documentElement.SelectNodes(xpath + xmlNoleName);
                int num;
                for (int i = 0; i < xmlNodeList.Count; i = num + 1)
                {
                    XmlNode xmlNode = xmlNodeList.Item(i);
                    bool flag4 = xmlNode != null;
                    if (flag4)
                    {
                        foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                        {
                            bool flag5 = xmlAttribute.Name.ToLower() == xmlAttributeNameBase.ToLower() && xmlAttribute.Value == valueBase;
                            if (flag5)
                            {
                                flag2 = true;
                                foreach (XmlAttribute xmlAttribute2 in xmlNode.Attributes)
                                {
                                    bool flag6 = xmlAttribute2.Name.ToLower() == xmlAttributeName.ToLower();
                                    if (flag6)
                                    {
                                        flag3 = true;
                                        break;
                                    }
                                }
                                bool flag7 = flag3;
                                if (flag7)
                                {
                                    break;
                                }
                            }
                        }
                        bool flag8 = !flag2;
                        if (flag8)
                        {
                            result = false;
                            return result;
                        }
                        bool flag9 = !flag3;
                        if (flag9)
                        {
                            XmlAttribute xmlAttribute3 = xmlDocument.CreateAttribute(xmlAttributeName);
                            xmlAttribute3.Value = value;
                            xmlNode.Attributes.Append(xmlAttribute3);
                            xmlDocument.Save(xmlFileName);
                            flag = true;
                        }
                    }
                    num = i;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            result = flag;
            return result;
        }
        /// <summary>
        /// 根据节点更新属性
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlNoleName"></param>
        /// <param name="xmlAttributeNameBase"></param>
        /// <param name="valueBase"></param>
        /// <param name="xmlAttributeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool UpdateXmlAttributeByXPath(string xmlFileName, string xpath, string xmlNoleName, string xmlAttributeNameBase, string valueBase, string xmlAttributeName, string value)
        {
            bool result = false;
            bool flag = false;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                XmlNodeList xmlNodeList = documentElement.SelectNodes(xpath + xmlNoleName);
                int num;
                for (int i = 0; i < xmlNodeList.Count; i = num + 1)
                {
                    XmlNode xmlNode = xmlNodeList.Item(i);
                    bool flag2 = xmlNode != null;
                    if (flag2)
                    {
                        foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                        {
                            bool flag3 = xmlAttribute.Name.ToLower() == xmlAttributeNameBase.ToLower() && xmlAttribute.Value == valueBase;
                            if (flag3)
                            {
                                foreach (XmlAttribute xmlAttribute2 in xmlNode.Attributes)
                                {
                                    bool flag4 = xmlAttribute2.Name.ToLower() == xmlAttributeName.ToLower();
                                    if (flag4)
                                    {
                                        xmlAttribute2.Value = value;
                                        flag = true;
                                        xmlDocument.Save(xmlFileName);
                                        result = true;
                                        break;
                                    }
                                }
                                bool flag5 = flag;
                                if (flag5)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    num = i;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 创建xml文件
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <returns></returns>
        public static bool CreateXMLConfigFile(string xmlFileName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlElement newChild2 = xmlDocument.CreateElement("root");
            xmlDocument.AppendChild(newChild);
            xmlDocument.AppendChild(newChild2);
            xmlDocument.Save(xmlFileName);
            return true;
        }
        /// <summary>
        /// 更新节点集合
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlNodeName"></param>
        /// <param name="xmlNodeNameAttrName"></param>
        /// <param name="xmlNodeNameAttrValue"></param>
        /// <param name="sourceNode"></param>
        /// <returns></returns>
        public static bool UpdateXMLChildNodeList(string xmlFileName, string xpath, string xmlNodeName, string xmlNodeNameAttrName, string xmlNodeNameAttrValue, XmlNode sourceNode)
        {
            bool flag = false;
            XmlDocument xmlDocument = new XmlDocument();
            bool result;
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                XmlNodeList xmlNodeList = documentElement.SelectNodes(xpath + xmlNodeName);
                int num;
                for (int i = 0; i < xmlNodeList.Count; i = num + 1)
                {
                    XmlNode xmlNode = xmlNodeList.Item(i);
                    bool flag2 = xmlNode != null && xmlNode.Attributes[xmlNodeNameAttrName] != null && xmlNode.Attributes[xmlNodeNameAttrName].Value == xmlNodeNameAttrValue;
                    if (flag2)
                    {
                        xmlNodeList.Item(i).RemoveAll();
                        XmlAttribute xmlAttribute = xmlDocument.CreateAttribute(xmlNodeNameAttrName);
                        xmlAttribute.Value = xmlNodeNameAttrValue;
                        xmlNodeList.Item(i).Attributes.Append(xmlAttribute);
                        foreach (XmlNode xmlNode2 in sourceNode.ChildNodes)
                        {
                            XmlElement xmlElement = xmlDocument.CreateElement(xmlNode2.Name);
                            foreach (XmlAttribute xmlAttribute2 in xmlNode2.Attributes)
                            {
                                XmlAttribute xmlAttribute3 = xmlDocument.CreateAttribute(xmlAttribute2.Name);
                                xmlAttribute3.Value = xmlAttribute2.Value;
                                xmlElement.Attributes.Append(xmlAttribute3);
                            }
                            xmlNodeList.Item(i).AppendChild(xmlElement);
                        }
                        xmlDocument.Save(xmlFileName);
                        flag = true;
                    }
                    num = i;
                }
                bool flag3 = !flag;
                if (flag3)
                {
                    XmlElement xmlElement2 = xmlDocument.CreateElement(xmlNodeName);
                    XmlAttribute xmlAttribute4 = xmlDocument.CreateAttribute(xmlNodeNameAttrName);
                    xmlAttribute4.Value = xmlNodeNameAttrValue;
                    xmlElement2.Attributes.Append(xmlAttribute4);
                    foreach (XmlNode xmlNode3 in sourceNode.ChildNodes)
                    {
                        XmlElement xmlElement3 = xmlDocument.CreateElement(xmlNode3.Name);
                        foreach (XmlAttribute xmlAttribute5 in xmlNode3.Attributes)
                        {
                            XmlAttribute xmlAttribute6 = xmlDocument.CreateAttribute(xmlAttribute5.Name);
                            xmlAttribute6.Value = xmlAttribute5.Value;
                            xmlElement3.Attributes.Append(xmlAttribute6);
                        }
                        xmlElement2.AppendChild(xmlElement3);
                    }
                    documentElement.AppendChild(xmlElement2);
                    xmlDocument.Save(xmlFileName);
                    flag = true;
                }
                result = flag;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 根据节点获取节点
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static XmlNode GetXmlNodeByXpath(string xmlFileName, string xpath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode result;
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                XmlNode xmlNode = documentElement.SelectSingleNode(xpath);
                result = xmlNode;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 根据节点获取节点集合
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static XmlNodeList GetXmlNodeListByXpath(string xmlFileName, string xpath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNodeList result;
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                bool flag = xpath.Length > 0;
                XmlNodeList xmlNodeList;
                if (flag)
                {
                    xmlNodeList = documentElement.SelectNodes(xpath);
                }
                else
                {
                    xmlNodeList = documentElement.ChildNodes;
                }
                result = xmlNodeList;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 根据节点获取属性
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlAttributeName"></param>
        /// <returns></returns>
        public static XmlAttribute GetXmlAttribute(string xmlFileName, string xpath, string xmlAttributeName)
        {
            string empty = string.Empty;
            XmlDocument xmlDocument = new XmlDocument();
            XmlAttribute result = null;
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                XmlNode xmlNode = documentElement.SelectSingleNode(xpath);
                bool flag = xmlNode != null;
                if (flag)
                {
                    bool flag2 = xmlNode.Attributes.Count > 0;
                    if (flag2)
                    {
                        result = xmlNode.Attributes[xmlAttributeName];
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 获取节点子集合
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlNodeNameAttrName"></param>
        /// <param name="xmlNodeNameAttrValue"></param>
        /// <returns></returns>
        public static XmlNodeList GetXMLChildNodeList(string xmlFileName, string xpath, string xmlNodeNameAttrName, string xmlNodeNameAttrValue)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNodeList result;
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                XmlNodeList xmlNodeList = null;
                bool flag = xpath.Length > 0;
                if (flag)
                {
                    XmlNodeList xmlNodeList2 = documentElement.SelectNodes(xpath);
                    foreach (XmlNode xmlNode in xmlNodeList2)
                    {
                        bool flag2 = xmlNode.Attributes[xmlNodeNameAttrName] != null && xmlNode.Attributes[xmlNodeNameAttrName].Value == xmlNodeNameAttrValue;
                        if (flag2)
                        {
                            xmlNodeList = xmlNode.ChildNodes;
                        }
                    }
                }
                else
                {
                    XmlNodeList xmlNodeList2 = documentElement.ChildNodes;
                    foreach (XmlNode xmlNode2 in xmlNodeList2)
                    {
                        bool flag3 = xmlNode2.Attributes[xmlNodeNameAttrName] != null && xmlNode2.Attributes[xmlNodeNameAttrName].Value == xmlNodeNameAttrValue;
                        if (flag3)
                        {
                            xmlNodeList = xmlNode2.ChildNodes;
                        }
                    }
                }
                result = xmlNodeList;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 根据节点删除节点
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static bool DeleteXmlNodeByXPath(string xmlFileName, string xpath)
        {
            bool result = false;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                XmlNode xmlNode = documentElement.SelectSingleNode(xpath);
                bool flag = xmlNode != null;
                if (flag)
                {
                    xmlNode.ParentNode.RemoveChild(xmlNode);
                    xmlDocument.Save(xmlFileName);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 根据节点删除属性
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="xpath"></param>
        /// <param name="xmlAttributeName"></param>
        /// <returns></returns>
        public static bool DeleteXmlAttributeByXPath(string xmlFileName, string xpath, string xmlAttributeName)
        {
            bool result = false;
            bool flag = false;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode documentElement = xmlDocument.DocumentElement;
                XmlNode xmlNode = documentElement.SelectSingleNode(xpath);
                XmlAttribute node = null;
                bool flag2 = xmlNode != null;
                if (flag2)
                {
                    foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                    {
                        bool flag3 = xmlAttribute.Name.ToLower() == xmlAttributeName.ToLower();
                        if (flag3)
                        {
                            node = xmlAttribute;
                            flag = true;
                            break;
                        }
                    }
                    bool flag4 = flag;
                    if (flag4)
                    {
                        xmlNode.Attributes.Remove(node);
                        xmlDocument.Save(xmlFileName);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 修改节点的属性值
        /// </summary>
        /// <param name="xmlFileName">文件路径</param>
        /// <param name="firstAttr">第一属性值（查找节点用）</param>
        /// <param name="secondAttr">第二属性值（查找节点用）</param>
        /// <param name="xmlAttrKey">属性名称（查找节点用）</param>
        /// <param name="chgAttrKey">要改修的属性名称</param>
        /// <param name="value">修改值</param>
        /// <returns></returns>
        public static bool UpdateXmlAttributeByPath(string xmlFileName, string firstAttr, string secondAttr, string xmlAttrKey, string chgAttrKey, string value)
        {
            bool result = false;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(xmlFileName);

                // 通过第一个属性获取节点
                XmlNode node = GetNode(xmlDocument.DocumentElement, firstAttr, xmlAttrKey);

                // 通过第二个属性获取节点
                if (!string.IsNullOrEmpty(secondAttr))
                {
                    node = GetNode(node, secondAttr, xmlAttrKey);
                }

                bool flag = node != null;
                if (flag)
                {
                    if (node.Attributes.GetNamedItem(xmlAttrKey).Value.ToLower() == secondAttr.ToLower())
                    {
                        node.Attributes.GetNamedItem(chgAttrKey).Value = value;
                        result = true;
                    }

                    xmlDocument.Save(xmlFileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 根据属性值获取节点
        /// </summary>
        /// <param name="xmlFileName">文件路径</param>
        /// <param name="firstAttr">第一个属性值</param>
        /// <param name="secondAttr">第二个属性值</param>
        /// <param name="xmlAttrKey">属性名称</param>
        /// <returns></returns>
        public static XmlNode GetNodeByByAttr(string xmlFileName, string firstAttr, string secondAttr, string xmlAttrKey)
        {
            XmlNode node = null;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(xmlFileName);
                node = GetNode(xmlDocument.DocumentElement, firstAttr, xmlAttrKey);

                if (!string.IsNullOrEmpty(secondAttr))
                {
                    node = GetNode(node, secondAttr, xmlAttrKey);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return node;
        }

        /// <summary>
        /// 通过属性值删除子节点
        /// </summary>
        /// <param name="xmlFileName">文件路径</param>
        /// <param name="firstAttr">第一个属性值</param>
        /// <param name="secondAttr">第二个属性值</param>
        /// <param name="xmlAttrKey">属性名称</param>
        /// <returns></returns>
        public static bool DeleteNodeByAttr(string xmlFileName, string firstAttr, string secondAttr, string xmlAttrKey)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(xmlFileName);

                // 通过第一个属性获取节点
                XmlNode node = GetNode(xmlDocument.DocumentElement, firstAttr, xmlAttrKey);

                // 通过第二个属性获取节点
                if (!string.IsNullOrEmpty(secondAttr))
                {
                    node = GetNode(node, secondAttr, xmlAttrKey);
                }

                if (null == node)
                {
                    return false;
                }

                node.ParentNode.RemoveChild(node);
                xmlDocument.Save(xmlFileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
        /// <summary>
        /// 向对应分类节点插入子节点
        /// </summary>
        /// <param name="xmlFileName">文件路径</param>
        /// <param name="firstAttr">第一个属性值</param>
        /// <param name="secondAttr">第二个属性值</param>
        /// <param name="xmlPathKey">属性名称</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="addAttr">新增节点需要添加的属性名称和值</param>
        /// <returns></returns>
        public static bool InsertNode2XmlByAttr(string xmlFileName, string firstAttr, string secondAttr, string xmlAttrKey, string nodeName, Dictionary<string, string> addAttr)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(xmlFileName);
                XmlNode xmlNode = GetNode(xmlDocument.DocumentElement, firstAttr, xmlAttrKey);

                if (!string.IsNullOrEmpty(secondAttr))
                {
                    xmlNode = GetNode(xmlNode, secondAttr, xmlAttrKey);
                }

                if (null == xmlNode)
                {
                    return false;
                }

                XmlElement xmlElement = xmlDocument.CreateElement(nodeName);
                if (null == xmlElement)
                {
                    return false;
                }

                foreach (var item in addAttr)
                {
                    xmlElement.SetAttribute(item.Key, item.Value);
                }

                xmlNode.AppendChild(xmlElement);
                xmlDocument.Save(xmlFileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        /// <summary>
        /// 根据属性查找结点
        /// </summary>
        /// <param name="curNode">节点</param>
        /// <param name="value">属性值</param>
        /// <param name="key">属性名称</param>
        /// <returns></returns>
        private static XmlNode GetNode(XmlNode curNode, string value, string key)
        {
            XmlNode node = null;
            if (null == curNode || null == curNode.Attributes)
            {
                return node;
            }

            if ((curNode.Attributes.Count > 0) && (value.ToLower() == curNode.Attributes.GetNamedItem(key).Value.ToLower()))
            {
                node = curNode;
            }

            foreach (XmlNode item in curNode.ChildNodes)
            {
                if (null != node)
                {
                    break;
                }

                node = GetNode(item, value, key);
            }

            return node;
        }
    }
}
