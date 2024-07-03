using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Models
{
    /// <summary>
    /// 仪器模块版本号对象
    /// </summary>
    public class BoardInfo
    {
        /// <summary>
        /// 板号
        /// </summary>
        public byte BoardId { get; set; }
        /// <summary>
        /// 版名称
        /// </summary>
        public string BoardName { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 所属模块号
        /// </summary>
        public int ModuleId { get; set; }
    }
}
