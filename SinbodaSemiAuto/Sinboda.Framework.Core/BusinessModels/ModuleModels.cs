using Sinboda.Framework.Core.AbstractClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels
{
    /// <summary>
    /// 模块类型表
    /// </summary>
    public partial class ModuleTypeModel : EntityModelBase
    {
        /// <summary>
        /// 语言编号
        /// </summary>
        public int LanguageID { get; set; }
        /// <summary>
        /// 模块类型
        /// </summary>
        public string ModuleTypeName { get; set; }
        /// <summary>
        /// 模块类型编码
        /// </summary>
        public int ModuleTypeCode { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; } = true;
    }

    /// <summary>
    /// 模块信息表
    /// </summary>
    public partial class ModuleInfoModel : EntityModelBase
    {
        /// <summary>
        /// 语言编号
        /// </summary>
        public int LanguageID { get; set; }
        /// <summary>
        /// 模块编号
        /// </summary>
        public int ModuleID { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 模块类型
        /// </summary>
        public Guid ModuleType { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; } = true;

        /// <summary>
        /// 模块类型编号
        /// </summary>
        [NotMapped]
        public int ModuleTypeCode { get; set; }
        /// <summary>
        /// 模块类型名称
        /// </summary>
        [NotMapped]
        public string ModuleTypeName { get; set; }

    }
}
