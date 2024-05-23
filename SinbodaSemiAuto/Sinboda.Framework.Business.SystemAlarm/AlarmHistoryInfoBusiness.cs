using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.ModelsOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Business.SystemAlarm
{
    /// <summary>
    /// 报警历史信息处理类
    /// </summary>
    public class AlarmHistoryInfoBusiness : BusinessBase<AlarmHistoryInfoBusiness>
    {
        /// <summary>
        /// 获取当天报警信息
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<AlarmHistoryInfoModel>> GetAlarmHitoryInfosToday(DateTime dtBegin, DateTime dtEnd)
        {
            try
            {
                List<AlarmHistoryInfoModel> results = new List<AlarmHistoryInfoModel>();
                results = SystemAlarmModelOperations.Instance.GetAlarmTodayInfo(dtBegin, dtEnd.AddDays(1));
                return Result(OperationResultEnum.SUCCEED, results);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetAllAlarmInfoByToday error", e);
                return Result<List<AlarmHistoryInfoModel>>(e);
            }
        }
        /// <summary>
        /// 获取所有报警信息
        /// </summary>
        /// <returns></returns>
        public OperationResult<int> GetAlarmHitoryInfosAllCount(DateTime dtBegin, DateTime dtEnd)
        {
            try
            {
                var results = SystemAlarmModelOperations.Instance.GetAlarmHitoryInfoCount(dtBegin, dtEnd.AddDays(1));
                return Result(OperationResultEnum.SUCCEED, results);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetAllAlarmInfos", e);
                return Result<int>(e);
            }
        }
        /// <summary>
        /// 获取所有报警信息
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<AlarmHistoryInfoModel>> GetAlarmHitoryInfosAll(DateTime dtBegin, DateTime dtEnd)
        {
            try
            {
                List<AlarmHistoryInfoModel> results = new List<AlarmHistoryInfoModel>();
                results = SystemAlarmModelOperations.Instance.GetAlarmHitoryInfo(dtBegin, dtEnd.AddDays(1));
                return Result(OperationResultEnum.SUCCEED, results);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetAllAlarmInfos", e);
                return Result<List<AlarmHistoryInfoModel>>(e);
            }
        }
        /// <summary>
        /// 获取所有报警信息
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<AlarmHistoryInfoModel>> GetAlarmHitoryInfosAll(DateTime dtBegin, DateTime dtEnd, int skipCount, int takeCount)
        {
            try
            {
                List<AlarmHistoryInfoModel> results = new List<AlarmHistoryInfoModel>();
                results = SystemAlarmModelOperations.Instance.GetAlarmHitoryInfo(dtBegin, dtEnd.AddDays(1), skipCount, takeCount);
                return Result(OperationResultEnum.SUCCEED, results);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetAllAlarmInfos", e);
                return Result<List<AlarmHistoryInfoModel>>(e);
            }
        }
        /// <summary>
        /// 删除一个报警信息项
        /// </summary>
        /// <param name="id"></param>
        public OperationResult DeleteOneAlarmHistoryInfo(Guid id)
        {
            try
            {
                if (SystemAlarmModelOperations.Instance.DeleteOneAlarmHistoryInfo(id))
                    return Result(OperationResultEnum.SUCCEED);
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("DeleteOneAlarmInfo error", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 清空所有报警历史信息项
        /// </summary>
        public OperationResult DeleteAllShowInfos()
        {
            try
            {
                if (SystemAlarmModelOperations.Instance.DeleteAllShowInfos())
                    return Result(OperationResultEnum.SUCCEED);
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("DeleteAllAlarmInfos error", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 获取报警统计项
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<AlarmStatisticInfo>> GetAlarmCodeStatistic()
        {
            try
            {
                List<AlarmStatisticInfo> results = SystemAlarmModelOperations.Instance.GetAlarmCodeStatistic();
                return Result(OperationResultEnum.SUCCEED, results);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetAlarmCodeStatistic error", e);
                return Result<List<AlarmStatisticInfo>>(e);
            }
        }
    }
}
