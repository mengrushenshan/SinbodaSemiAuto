using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.ModelsOperation
{
    /// <summary>
    /// 信息管理数据操作类
    /// </summary>
    public class SystemLogModelOperations : EFDataOperationBase<SystemLogModelOperations, SysLogModel, DBContextBase>
    {
        #region 系统日志功能
        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns>日志模型</returns>
        public int QueryLogCount(Expression<Func<SysLogModel, bool>> exp)
        {
            return Where(exp).Count();
        }

        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns>日志模型</returns>
        public List<SysLogModel> QueryLog(Expression<Func<SysLogModel, bool>> exp)
        {
            return Where(exp).OrderByDescending(o => o.Datetime).ToList();
        }
        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns>日志模型</returns>
        public List<SysLogModel> QueryLog(Expression<Func<SysLogModel, bool>> exp, int skipCount, int takeCount)
        {
            return Where(exp).OrderByDescending(o => o.Datetime).Skip(skipCount).Take(takeCount).ToList();
        }

        /// <summary>
        /// 日志写入数据库
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="type">日志类型</param>
        /// <returns></returns>
        public bool WriteLogDb(string message, SysLogType type)
        {
            SysLogModel model = new SysLogModel()
            {
                Message = message,
                Type = (int)type,
                Datetime = DateTime.Now,
                UserID = SystemResources.Instance.CurrentUserName
            };
            Insert(model);
            return true;
        }
        /// <summary>
        /// 清空日志信息
        /// </summary>
        /// <returns></returns>
        public bool ClearLog()
        {
            DeleteAll();
            return true;
        }
        #endregion
    }
}
