using Sinboda.Framework.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Interface
{
    /// <summary>
    /// 系统日志服务接口
    /// </summary>
    public interface ISystemLogService
    {
        /// <summary>
        /// 书写日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool WriteLogDb(string message, SysLogType type);

        /// <summary>
        /// 清空日志信息
        /// </summary>
        /// <returns></returns>
        bool ClearLog();
    }
}
