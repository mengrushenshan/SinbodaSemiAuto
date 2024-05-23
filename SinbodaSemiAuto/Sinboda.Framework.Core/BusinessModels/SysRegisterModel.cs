using Sinboda.Framework.Core.AbstractClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels
{
    /// <summary>
    /// 系统注册实体
    /// </summary>
    public partial class SysRegisterModel : EntityModelBase
    {
        /// <summary>
        /// 注册码
        /// </summary>
        public string RegisterCode { get; set; }
        /// <summary>
        /// 医院名称
        /// </summary>
        public string HospitalName { get; set; }
        /// <summary>
        /// 医院地址
        /// </summary>
        public string HospitalAddr { get; set; }
        /// <summary>
        /// 医院联系方式
        /// </summary>
        public string HospitalPhone { get; set; }
        /// <summary>
        /// 售后人员名称
        /// </summary>
        public string SaleName { get; set; }
        /// <summary>
        /// 代理商名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 售后联系方式
        /// </summary>
        public string SalePhone { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }
    }
}
