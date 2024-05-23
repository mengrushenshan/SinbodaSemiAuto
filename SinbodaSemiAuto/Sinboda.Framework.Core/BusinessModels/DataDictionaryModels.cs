using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sinboda.Framework.Core.BusinessModels
{
    /// <summary>
    /// 基础信息类型
    /// </summary>
    public partial class DataDictionaryTypeModel : EntityModelBase
    {
        /// <summary>
        /// 语言编号
        /// </summary>
        public int LanguageID { get; set; }
        /// <summary>
        /// 语言编号
        /// </summary>
        [NotMapped]
        public string LanguageIDString { get; set; }
        /// <summary>
        /// 字典编码
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        public string TypeValues { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int ShowOrder { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        [NotMapped]
        public string ShowOrderString { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 是否可以设置助记符
        /// </summary>
        public bool IsSetHotKey { get; set; }
        /// <summary>
        /// 是否可以设置默认项
        /// </summary>
        public bool IsSetDefault { get; set; }
        /// <summary>
        /// 是否可以设置上级编码
        /// </summary>
        public bool IsSetParentCode { get; set; }
        /// <summary>
        /// 上级编码
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public int MaxLengthValue { get; set; }

        /// <summary>
        /// 正则表达式
        /// </summary>
        public string RegexTextValue { get; set; }

        /// <summary>
        /// 子项是否可以删除
        /// </summary>
        public bool IsChildCanDelete { get; set; } = true;
    }

    /// <summary>
    /// 基础信息信息
    /// </summary>
    public partial class DataDictionaryInfoModel : EntityModelBase
    {
        /// <summary>
        /// 语言编号
        /// </summary>
        public int LanguageID { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        public string Values { get; set; }
        [NotMapped]
        public string DisplayValues
        {
            get
            {
                if (LanguageID <= 0)
                {
                    return Values;
                }
                else
                {
                    return SystemResources.Instance.LanguageArray[LanguageID];
                }
            }
        }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int ShowOrder { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 助记符
        /// </summary>
        public string HotKey { get; set; }
        /// <summary>
        /// 是否为默认项
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 上级编码
        /// </summary>
        public Guid ParentCode { get; set; }
        /// <summary>
        /// 上级名称
        /// </summary>
        [NotMapped]
        public string ParentName { get; set; }
        /// <summary>
        /// 字典类型编号
        /// </summary>
        public Guid CodeGroupID { get; set; }
    }


    [XmlRoot(ElementName = "DataDictionaryParentInfo")]
    public class DataDictionaryParentInfo
    {
        /// <summary>
        ///图片
        /// </summary>
        [XmlArray("DataDictionaryInfos"), XmlArrayItem("DataDictionaryInfo")]
        public List<DataDictionaryInfo> DataDictionaryInfos { get; set; }

    }

    [XmlRoot("DataDictionaryInfo")]
    public class DataDictionaryInfo
    {

        /// <summary>
        ///父节点名称  语言  字体  主题
        /// </summary>
        [XmlAttribute]
        public string ParentID { get; set; }
        /// <summary>
        ///图片
        /// </summary>
        [XmlArray("DataDictionaryInfoDetails")]
        [XmlArrayItem("DataDictionaryInfoDetail", typeof(DataDictionaryInfoDetail))]
        public List<DataDictionaryInfoDetail> DataDictionaryInfoDetails { get; set; }

    }

    /// <summary>
    /// 字典表描述
    /// </summary>
    [Serializable()]
    public class DataDictionaryInfoDetail
    {
        /// <summary>
        /// 语言编号
        /// </summary>
        [XmlAttribute]
        public int LanguageID { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        [XmlAttribute]
        public string Code { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        [XmlAttribute]
        public string Values { get; set; }
    }
}
