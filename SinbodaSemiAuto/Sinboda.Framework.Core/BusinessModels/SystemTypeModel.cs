using Sinboda.Framework.Core.AbstractClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels
{
    /// <summary>
    /// ◆单元名称：系统类型-基础信息
    /// ◆功能描述：
    /// ◆创建人  ： sunch
    /// ◆创建日期： 2019/3/13 14:01
    /// ◆修改人  ：
    /// ◆修改日期：
    /// ◆修改原因：
    /// </summary>
    public partial class SystemTypeModel : EntityModelBase
    {
        /// <summary>
        /// 字典编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        public string Values { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 语言编号
        /// </summary>
        public string LanguageID { get; set; }
    }

    /// <summary>
    /// ◆单元名称：系统类型-基础信息
    /// ◆功能描述：
    /// ◆创建人  ： sunch
    /// ◆创建日期： 2019/3/13 14:01
    /// ◆修改人  ：
    /// ◆修改日期：
    /// ◆修改原因：
    /// </summary>
    [Serializable]
    public partial class SystemTypeValueModel : EntityModelBase
    {
        /// <summary>
        /// 语言编号
        /// </summary>
        public int LanguageID { get; set; }
        /// <summary>
        /// 字典编码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        public string DisplayValue { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 是否为默认项
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 字典类型
        /// </summary>
        public Guid CodeGroupID { get; set; }
    }

    /// <summary>
    /// <seealso cref="SystemTypeValueModel"/> 类型的泛型扩展
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SystemTypeValue<T> : SystemTypeValueModel where T : struct
    {
        /// <summary>
        /// 扩展值
        /// </summary>
        public T Value { get; set; }
    }
}
