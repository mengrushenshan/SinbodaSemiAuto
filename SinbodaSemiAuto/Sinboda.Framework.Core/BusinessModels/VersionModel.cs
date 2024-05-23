using Sinboda.Framework.Core.AbstractClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels
{
    /// <summary>
    /// ◆单元名称：版本信息表
    /// ◆功能描述：
    /// ◆创建人  ： sunch
    /// ◆创建日期： 2019/3/13 13:55
    /// ◆修改人  ：
    /// ◆修改日期：
    /// ◆修改原因：
    /// </summary>
    public partial class VersionModel : EntityModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string DBI_NAME { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string DBI_VALUE { get; set; }
    }
}
