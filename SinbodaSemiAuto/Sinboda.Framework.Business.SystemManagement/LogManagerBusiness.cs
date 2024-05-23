using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.ModelsOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Business.SystemManagement
{
    /// <summary>
    /// 日志业务处理类
    /// </summary>
    public class LogManagerBusiness : BusinessBase<LogManagerBusiness>
    {
        SystemLogModelOperations operations = new SystemLogModelOperations();
        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="type">日志类型</param>
        /// <param name="user">用户</param>
        /// <returns>日志模型</returns>
        public OperationResult<int> QueryLogCount(DateTime startDate, DateTime endDate, SysLogType type, string user)
        {
            int result = 0;
            try
            {
                Expression<Func<SysLogModel, bool>> exp = null;
                endDate = endDate.AddDays(1);
                exp = o => o.Datetime < endDate && o.Datetime > startDate && o.Type == (int)type;
                if (!string.IsNullOrEmpty(user))
                    exp = o => o.Datetime < endDate && o.Datetime > startDate && o.Type == (int)type && o.UserID == user;
                result = operations.QueryLogCount(exp);
                return Result(result);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("查询日志发生错误", e);
                return Result<int>(e);
            }
        }
        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="type">日志类型</param>
        /// <param name="user">用户</param>
        /// <returns>日志模型</returns>
        public OperationResult<List<SysLogModel>> QueryLog(DateTime startDate, DateTime endDate, SysLogType type, string user)
        {
            OperationResult<List<SysLogModel>> result = new OperationResult<List<SysLogModel>>();
            try
            {
                List<SysLogModel> list = new List<SysLogModel>();
                Expression<Func<SysLogModel, bool>> exp = null;
                //endDate = endDate.AddDays(1);
                exp = o => o.Datetime < endDate && o.Datetime > startDate && o.Type == (int)type;
                if (!string.IsNullOrEmpty(user))
                    exp = o => o.Datetime < endDate && o.Datetime > startDate && o.Type == (int)type && o.UserID == user;
                list = operations.QueryLog(exp);
                return Result(list);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("查询日志发生错误", e);
                return Result<List<SysLogModel>>(e);
            }
        }
        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="type">日志类型</param>
        /// <param name="user">用户</param>
        /// <returns>日志模型</returns>
        public OperationResult<List<SysLogModel>> QueryLog(DateTime startDate, DateTime endDate, SysLogType type, string user, int skipCount, int takeCount)
        {
            OperationResult<List<SysLogModel>> result = new OperationResult<List<SysLogModel>>();
            try
            {
                List<SysLogModel> list = new List<SysLogModel>();
                Expression<Func<SysLogModel, bool>> exp = null;
                endDate = endDate.AddDays(1);
                exp = o => o.Datetime < endDate && o.Datetime > startDate && o.Type == (int)type;
                if (!string.IsNullOrEmpty(user))
                    exp = o => o.Datetime < endDate && o.Datetime > startDate && o.Type == (int)type && o.UserID == user;
                list = operations.QueryLog(exp, skipCount, takeCount);
                return Result(list);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("查询日志发生错误", e);
                return Result<List<SysLogModel>>(e);
            }
        }

        /// <summary>
        /// 日志写入数据库
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="type">日志类型</param>
        /// <returns></returns>
        public OperationResult WriteLogDb(string message, SysLogType type)
        {
            try
            {
                bool result = operations.WriteLogDb(message, type);
                if (result)
                    return Result(OperationResultEnum.SUCCEED);
                else return Result(OperationResultEnum.FAILED);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetSysDataDictionaryTypeList error", e);
                return Result(OperationResultEnum.FAILED, e.Message);
            }
        }
    }
}
