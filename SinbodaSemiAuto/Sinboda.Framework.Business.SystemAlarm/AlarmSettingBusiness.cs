using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Business.SystemAlarm
{
    /// <summary>
    /// 报警原始信息处理类
    /// </summary>
    public class AlarmSettingBusiness : BusinessBase<AlarmSettingBusiness>
    {
        /// <summary>
        /// 获取报警信息
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<AlarmOrignalInfoModel>> GetAlarmInfos()
        {
            try
            {
                List<AlarmOrignalInfoModel> results = new List<AlarmOrignalInfoModel>();
                results = SystemAlarmModelOperations.Instance.GetAlarmInfos();
                return Result(OperationResultEnum.SUCCEED, results);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetAlarmInfos error", e);
                return Result<List<AlarmOrignalInfoModel>>(e);
            }
        }
        /// <summary>
        /// 获取单个报警信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public OperationResult<AlarmOrignalInfoModel> GetAlarmInfoByCode(string code)
        {
            try
            {
                return Result<AlarmOrignalInfoModel>(OperationResultEnum.SUCCEED, SystemAlarmModelOperations.Instance.GetAlarmInfoByCode(code));
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetAlarmInfoByCode error", e);
                return Result<AlarmOrignalInfoModel>(e);
            }
        }
        /// <summary>
        /// 获得报警码显示状态
        /// </summary>
        /// <param name="code">报警码</param>
        /// <returns></returns>
        public OperationResult GetAlarmCodeVisibility(string code)
        {
            try
            {
                if (SystemAlarmModelOperations.Instance.GetCodeVisibleByCode(code))
                    return Result(OperationResultEnum.SUCCEED);
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetAlarmCodeVisibility error", e);
                return Result(OperationResultEnum.FAILED, e);
            }

        }
        /// <summary>
        /// 获取某一级别报警码显示状态
        /// </summary>
        /// <param name="level">报警级别</param>
        /// <returns></returns>
        public OperationResult GetAlarmCodeVisibilityByLevel(string level)
        {
            try
            {
                if (SystemAlarmModelOperations.Instance.GetCodeVisibleByLevel(level))
                    return Result(OperationResultEnum.SUCCEED);
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetAlarmCodeVisibilityByLevel error", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 设置某一报警码显示状态
        /// </summary>
        /// <param name="alarmCode">报警码</param>
        /// <param name="flag">状态</param>
        /// <returns></returns>
        public OperationResult SetAlarmCodeVisibility(string alarmCode, bool flag)
        {
            try
            {
                if (SystemAlarmModelOperations.Instance.SetCodeVisibilityByCode(alarmCode, flag))
                {
                    SystemInitialize.InitializeAlarmOrignalInfo();
                    return Result(OperationResultEnum.SUCCEED);
                }
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("SetAlarmCodeVisibility error", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 设置某一级别的报警码显示状态
        /// </summary>
        /// <param name="level">级别</param>
        /// <param name="flag">状态</param>
        /// <returns></returns>
        public OperationResult ChangeVisiblFlags(long[] level, bool[] flag)
        {
            try
            {
                if (SystemAlarmModelOperations.Instance.SetCodeVisibilityByLevel(level, flag))
                {
                    SystemInitialize.InitializeAlarmOrignalInfo();
                    return Result(OperationResultEnum.SUCCEED);
                }
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("ChangeAlarmLevelVisibleFlag error", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 恢复报警码状态
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public OperationResult SetAlarmCodeVisibilityToDefault(int index)
        {
            try
            {
                if (SystemAlarmModelOperations.Instance.SetAlarmCodeVisibilityToDefault(index))
                {
                    SystemInitialize.InitializeAlarmOrignalInfo();
                    return Result(OperationResultEnum.SUCCEED);
                }
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("SetDefaultAlarmCode error", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
    }
}
