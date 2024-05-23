using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.Interface;
using Sinboda.Framework.Core.ModelsOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Services
{
    /// <summary>
    /// 系统日志服务
    /// </summary>
    public class SystemLogService : ISystemLogService
    {
        SystemLogModelOperations operation = new SystemLogModelOperations();
        /// <summary>
        /// 日志写入数据库
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="type">日志类型</param>
        /// <returns></returns>
        public bool WriteLogDb(string message, SysLogType type)
        {
            return operation.WriteLogDb(message, type);
        }

        /// <summary>
        /// 清空日志信息
        /// </summary>
        /// <returns></returns>
        public bool ClearLog()
        {
            return operation.ClearLog();
        }
    }
}
