using Sinboda.Framework.Common.FileOperateHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sinboda.Framework.Control.GridColumnSetting
{
    /// <summary>
    /// 
    /// </summary>
    public class ColumnSetting : INotifyPropertyChanged
    {
        private string columnHeader;
        /// <summary>
        /// 表格列配置类
        /// </summary>
        public string ColumnHeader
        {
            get { return this.columnHeader; }
            set
            {
                this.columnHeader = value;
                OnPropertyChanged("ColumnHeader");
            }
        }
        private string columnField;
        /// <summary>
        /// 
        /// </summary>
        public string ColumnField
        {
            get { return this.columnField; }
            set
            {
                this.columnField = value;
                OnPropertyChanged("ColumnField");
            }
        }
        private int columnIndex;
        /// <summary>
        /// 
        /// </summary>
        public int ColumnIndex
        {
            get { return this.columnIndex; }
            set
            {
                this.columnIndex = value;
                OnPropertyChanged("ColumnIndex");
            }
        }
        private bool isVisible;
        /// <summary>
        /// 
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set
            {
                this.isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }
        private string columnWidth;
        /// <summary>
        /// 
        /// </summary>
        public string ColumnWidth
        {
            get { return this.columnWidth; }
            set
            {
                this.columnWidth = value;
                OnPropertyChanged("ColumnWidth");
            }
        }
        /// <summary>
        /// 属性值变化时发生
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
    /// <summary>
    /// 表格配置文件管理类
    /// </summary>
    public class GridConfigManager
    {
        /// <summary>
        /// 配置文件使用接口
        /// </summary>
        private FileHelper fileHelper = new FileHelper();
        /// <summary>
        /// 用于判断是否存在FrameWork.xml文件
        /// </summary>
        private bool isHaveFile = false;



        ///// <summary>
        ///// 配置文件名称
        ///// </summary>
        //private string strConfigFileName = @"/GridViewColumnConfig.xml";

        /// <summary>
        /// 配置文件名称
        /// </summary>
        private string strConfigFileFullPath = string.Empty;

        ///// <summary>
        ///// 默认配置
        ///// </summary>
        //private List<GridViewColumnConfig> DefaulConfig = new List<GridViewColumnConfig>();

        /// <summary>
        /// 当前配置
        /// </summary>
        private List<ColumnSetting> currentConfig = new List<ColumnSetting>();




        private string strGridName = string.Empty;



        /// <summary>
        /// 当前配置
        /// </summary>
        public List<ColumnSetting> CurrentConfig
        {
            get { return this.currentConfig; }
            set { this.currentConfig = value; }
        }

        /// <summary>
        /// 当前表名
        /// </summary>
        public string CurrentGridName
        {
            get { return this.strGridName; }
            set { this.strGridName = value; }
        }

        /// <summary>
        /// 配置路径
        /// </summary>
        public string ConfigPath
        {
            get { return this.strConfigFileFullPath; }
            set { this.strConfigFileFullPath = value; }
        }


        ///// <summary>
        ///// 初始化配置文件
        ///// </summary>
        ///// <returns></returns>
        //public bool InitGridConfig()
        //{
        //    if (string.IsNullOrEmpty(this.strConfigFileFullPath))
        //    {
        //        return false;
        //    }

        //    isHaveFile = File.Exists(this.strConfigFileFullPath);
        //    if (!isHaveFile)
        //    {
        //        iDrConfigFile.XMLCreateConfigFile(this.strConfigFileFullPath);
        //        iDrConfigFile.XMLSetFileName(this.strConfigFileFullPath);
        //    }

        //    return true;
        //}

        /// <summary>
        /// 获取当期表格列属性配置
        /// </summary>
        /// <param name="configPath"></param>
        /// <param name="strGridName"></param>
        /// <returns></returns>
        public List<ColumnSetting> GetCurrentSetting(string configPath, string strGridName)
        {
            if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
            {
                return null;
            }

            CurrentConfig.Clear();
            XmlNodeList xmlNodeList = fileHelper.XMLGetXmlNodeByName(configPath, "", "GridName", strGridName);

            if (null == xmlNodeList)
            {
                return null;
            }

            foreach (XmlNode node in xmlNodeList)
            {
                ColumnSetting config = new ColumnSetting();
                config.ColumnField = node.Attributes["FieldName"].Value;
                config.ColumnHeader = node.Attributes["Header"].Value;
                config.ColumnIndex = int.Parse(node.Attributes["Index"].Value);
                config.IsVisible = bool.Parse(node.Attributes["IsVisible"].Value);
                config.ColumnWidth = node.Attributes["Width"].Value;
                CurrentConfig.Add(config);
            }
            return CurrentConfig;
        }

        /// <summary>
        /// 设置当前表格列属性配置
        /// </summary>
        /// <param name="configPath"></param>
        /// <param name="strGridName"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool SetCurrentConfig(string configPath, string strGridName, List<ColumnSetting> config)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlGridEle = xmlDoc.CreateElement("Grid");
            XmlAttribute xmlAttr = xmlDoc.CreateAttribute("GridName");
            xmlAttr.Value = strGridName;
            xmlGridEle.Attributes.Append(xmlAttr);
            foreach (ColumnSetting configItem in config)
            {
                XmlElement xmlColumnEle = xmlDoc.CreateElement("Column");
                XmlAttribute xmlFieldNameAttr = xmlDoc.CreateAttribute("FieldName");
                XmlAttribute xmlHeaderAttr = xmlDoc.CreateAttribute("Header");
                XmlAttribute xmlIndexAttr = xmlDoc.CreateAttribute("Index");
                XmlAttribute xmlIsVisibleAttr = xmlDoc.CreateAttribute("IsVisible");
                XmlAttribute xmlWidthAttr = xmlDoc.CreateAttribute("Width");
                xmlFieldNameAttr.Value = configItem.ColumnField;
                xmlHeaderAttr.Value = configItem.ColumnHeader;
                xmlIndexAttr.Value = configItem.ColumnIndex.ToString();
                xmlIsVisibleAttr.Value = configItem.IsVisible.ToString();
                xmlWidthAttr.Value = configItem.ColumnWidth;

                xmlColumnEle.Attributes.Append(xmlFieldNameAttr);
                xmlColumnEle.Attributes.Append(xmlHeaderAttr);
                xmlColumnEle.Attributes.Append(xmlIndexAttr);
                xmlColumnEle.Attributes.Append(xmlIsVisibleAttr);
                xmlColumnEle.Attributes.Append(xmlWidthAttr);

                xmlGridEle.AppendChild(xmlColumnEle);
            }

            fileHelper.XMLUpdateXmlNodeByName(configPath, "", "Grid", "GridName", strGridName, xmlGridEle);

            return true;
        }
    }
}
