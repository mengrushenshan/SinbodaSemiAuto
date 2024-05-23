using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.CommonModels
{
    /// <summary>
    /// 打印日志所用model信息
    /// </summary>
    public class LogModelPrint
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string Datetime { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        ///  日志类型 字符型
        /// </summary>
        public string Typestr { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; }
    }
}
