using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel
{
    /// <summary>
    /// 模块设置信息
    /// </summary>
    public class ModuleSettingInfo : EntityModelBase
    {
        public override string ToString()
        {
            return $"module id {ModuleId}     is module enabled {IsModuleEnabled}    is module shield {IsShield}    product type {productType}";
        }

        /// <summary>
        /// 产品类型
        /// </summary>
        public ProductType productType { get; set; }

        /// <summary>
        /// 模块号
        /// </summary>
        public ushort ModuleId { get; set; }

        private bool? isModuleEnabled = null;
        /// <summary>
        /// 是否启用模块
        /// </summary>
        public bool? IsModuleEnabled
        {
            get { return isModuleEnabled; }
            set { Set(ref isModuleEnabled, value); }
        }

        private bool? isSleep = null;
        /// <summary>
        /// 模块是否休眠
        /// </summary>
        public bool? IsSleep
        {
            get { return isSleep; }
            set { Set(ref isSleep, value); }
        }

        private bool isShield;
        /// <summary>
        /// 是否遮蔽
        /// </summary>
        public bool IsShield
        {
            get { return isShield; }
            set { Set(ref isShield, value); }
        }

        public string moduleName;
        [NotMapped]
        public string ModuleName
        {
            get { return moduleName; }
            set { Set(ref moduleName, value); }
        }

    }
}
