using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Models
{
    /// <summary>
    /// 验证级别
    /// </summary>
    public enum VerificationLevel
    {
        /// <summary>
        /// 忽略级别，完全忽略，正确
        /// </summary>
        Ignore,

        /// <summary>
        /// 警告级别，不影响测试，上位机弹出警告
        /// </summary>
        Warning,

        /// <summary>
        /// 报警级别，影响测试，上位机提示报警
        /// </summary>
        Alarm,

        /// <summary>
        /// 熔断级别，达到熔断条件时，停止验证及测试，弹出熔断前（包括熔断）的所有错误提示，否则继续校验及测试  
        /// </summary>
        Fusing,

        /// <summary>
        /// 错误级别，无法测试，上位机弹出当前的错误提示
        /// </summary>
        Error


    }
}
