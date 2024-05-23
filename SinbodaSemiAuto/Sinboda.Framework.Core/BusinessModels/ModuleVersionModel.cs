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
    /// 模块版本信息表
    /// </summary>
    public partial class ModuleVersionModel : EntityModelBase
    {
        /// <summary>
        /// 模块编号
        /// </summary>
        public int ModuleID { get; set; }
        /// <summary>
        /// 模块显示显示名称
        /// </summary>
        [NotMapped]
        public string MachineShowName { get; set; }
        /// <summary>
        /// 序列号
        /// </summary>
        public string SerialNO { get; set; }
        /// <summary>
        /// 单元名称
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 单元显示名称
        /// </summary>
        [NotMapped]
        public string UnitShowName { get; set; }
        /// <summary>
        /// 单元顺序号
        /// </summary>
        [NotMapped]
        public int UnitOrder { get; set; }
        /// <summary>
        /// 版本信息
        /// </summary>
        public string VersionInfo { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 显示时间
        /// </summary>
        [NotMapped]
        public string CreatTimeShowInfo { get; set; }
    }
}
