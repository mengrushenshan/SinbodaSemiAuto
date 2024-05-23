using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Common.DBOperateHelper;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sinboda.Framework.Common.CommonFunc;
using System.IO;

namespace Sinboda.Framework.Core.ModelsOperation
{
    public delegate void AlarmEvent(AlarmEventOprateType oprateType, AlarmHistoryInfoModel alarmInfo);

    public enum AlarmEventOprateType
    {
        Add,
        ClearOne,
        ClearAll,
    }

    /// <summary>
    /// 报警信息数据操作类
    /// </summary>
    public class SystemAlarmModelOperations : EFDataOperationBase<SystemAlarmModelOperations, AlarmHistoryInfoModel, DBContextBase>
    {
        Thread alarmHandlerThread = null;
        ConcurrentQueue<AlarmMessage> alarmQueue = new ConcurrentQueue<AlarmMessage>();

        public static event AlarmEvent AlarmMentionEvent;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SystemAlarmModelOperations()
        {
            string _ConnectString = Path.Combine(_Directory, _FileName);
            bool result = iDBHelper.Init(_DataSource + _ConnectString);
            if (!result)
                iDBHelper = null;


            if (alarmHandlerThread == null)
            {
                alarmHandlerThread = new Thread(new ThreadStart(AlarmHanlder));
                alarmHandlerThread.IsBackground = true;
                alarmHandlerThread.Start();
            }
        }

        #region 报警原始信息处理
        /// <summary>
        /// 数据源
        /// </summary>
        string _DataSource = @"Data Source=";
        /// <summary>
        /// 程序路径
        /// </summary>
        string _Directory = MapPath.AppDir;
        /// <summary>
        /// 数据库
        /// </summary>
        string _FileName = @"Data\\AlarmInfo.db";
        /// <summary>
        /// db操作帮助类
        /// </summary>
        IDBHelper iDBHelper = new DBHelper(DBProvider.SQLite);

        private bool ConvertoBool(object value)
        {
            if (value == null) return false;

            bool result = false;

            if (bool.TryParse(value.ToString(), out result))
            {

            }

            return result;
        }

        /// <summary>
        /// 获取报警原始信息
        /// </summary>
        /// <returns></returns>
        public List<AlarmOrignalInfoModel> GetAlarmInfos()
        {
            string tableName = "AlarmOrignalInfo_" + SystemResources.Instance.CurrentLanguage.ToLower();
            List<AlarmOrignalInfoModel> results = new List<AlarmOrignalInfoModel>();
            string sql = "select " + tableName + ".[Code], " + tableName + ".[CodeLevel]," + tableName + ".[Info]," + tableName + ".[DetailInfo]," + tableName + ".[Solution]," + tableName + ".[ModuleType],AlarmCodeVisibility.[CodeVisibility] " +
            "from " + tableName + " left join AlarmCodeVisibility on " + tableName + ".[ModuleType] = AlarmCodeVisibility.[ModuleType] and " + tableName + ".[Code] = AlarmCodeVisibility.[Code]";
            //string sql = string.Format("select * from " + tableName);
            DataTable dt = new DataTable();
            dt = iDBHelper.ExcuteQueryDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRowView drv in dt.DefaultView)
                {
                    AlarmOrignalInfoModel model = new AlarmOrignalInfoModel()
                    {
                        Code = drv["Code"].ToString(),
                        CodeLevel = (AlarmLevelEnum)drv["CodeLevel"],
                        Info = drv["Info"].ToString(),
                        DetailInfo = drv["DetailInfo"].ToString(),
                        Solution = drv["Solution"].ToString(),
                        CodeVisibility = ConvertoBool(drv["CodeVisibility"]),
                        ModuleType = int.Parse(drv["ModuleType"].ToString()),

                    };
                    if (!DataDictionaryService.Instance.GetIsMultiModuleMode)
                        model.ModuleTypeName = string.Empty;
                    else
                    {
                        int moduleTypeCode = int.Parse(drv["ModuleType"].ToString());
                        if (moduleTypeCode == 0)
                            model.ModuleTypeName = string.Empty;
                        else
                        {
                            ModuleTypeModel mType = null;
                            if (DataDictionaryService.Instance.ModuleTypeInfo.TryGetValue(moduleTypeCode, out mType))
                            {
                                model.ModuleTypeName = mType.LanguageID != 0 ? SystemResources.Instance.LanguageArray[mType.LanguageID] : mType.ModuleTypeName;
                            }
                        }
                    }

                    // 区分是否为多模块联机
                    if (DataDictionaryService.Instance.GetIsMultiModuleMode && DataDictionaryService.Instance.ModuleTypeInfo.Count > 1)
                    {
                        // 模块类型能在模块类型表中查找到
                        if (DataDictionaryService.Instance.ModuleTypeInfo[model.ModuleType] != null)
                        {
                            // 当前模块类型设置为可见
                            if (DataDictionaryService.Instance.ModuleTypeInfo[model.ModuleType].IsShow)
                                results.Add(model);
                        }
                        else
                            results.Add(model);
                    }
                    else
                        results.Add(model);
                }
            }
            return results;
        }
        /// <summary>
        /// 获取单个报警原始信息
        /// </summary>
        /// <param name="code">报警码</param>
        /// <returns></returns>
        public AlarmOrignalInfoModel GetAlarmInfoByCode(string code)
        {
            string tableName = "AlarmOrignalInfo_" + SystemResources.Instance.CurrentLanguage.ToLower();
            AlarmOrignalInfoModel results = new AlarmOrignalInfoModel();
            string sql = "select " + tableName + ".[Code], " + tableName + ".[CodeLevel]," + tableName + ".[Info]," + tableName + ".[DetailInfo]," + tableName + ".[Solution]," + tableName + ".[ModuleType],AlarmCodeVisibility.[CodeVisibility] " +
                        "from " + tableName + " left join AlarmCodeVisibility on " + tableName + ".[ModuleType] = AlarmCodeVisibility.[ModuleType] and " + tableName + ".[Code] = AlarmCodeVisibility.[Code]  where Code = " + code;
            //string sql = string.Format("select * from " + tableName + " where Code='{0}'", code);
            DataTable dt = new DataTable();
            dt = iDBHelper.ExcuteQueryDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                results.Code = dt.Rows[0]["Code"].ToString();
                results.CodeLevel = (AlarmLevelEnum)dt.Rows[0]["CodeLevel"];
                results.Info = dt.Rows[0]["Info"].ToString();
                results.DetailInfo = dt.Rows[0]["DetailInfo"].ToString();
                results.Solution = dt.Rows[0]["Solution"].ToString();
                results.CodeVisibility = (bool)dt.Rows[0]["CodeVisibility"];
                results.ModuleType = int.Parse(dt.Rows[0]["ModuleType"].ToString());
            }
            return results;
        }



        /// <summary>
        /// 获得报警码显示状态
        /// </summary>
        /// <param name="code">报警码</param>
        /// <returns></returns>
        public bool GetCodeVisibleByCode(string code)
        {
            string sql = string.Format("select CodeVisibility from AlarmCodeVisibility where Code='{0}'", code);
            bool result = false;
            object dbResult = iDBHelper.ExcuteQueryObject(sql);
            if (dbResult != null)
                return (bool)dbResult;
            else
                return result;
        }
        /// <summary>
        /// 获取某一级别报警码显示状态
        /// </summary>
        /// <param name="level">报警级别</param>
        /// <returns></returns>
        public bool GetCodeVisibleByLevel(string level)
        {
            string sql = string.Format("select CodeVisibility from AlarmCodeVisibility where CodeLevel='{0}'", level);
            int result = 0;
            DataTable dt = iDBHelper.ExcuteQueryDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRowView drv in dt.Rows)
                {
                    var tem = ConvertoBool(drv["CodeVisibility"]);
                    if (tem) result++;
                }

                if (result == dt.Rows.Count)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        /// <summary>
        /// 设置某一报警码显示状态
        /// </summary>
        /// <param name="code">报警码</param>
        /// <param name="flag">状态</param>
        /// <returns></returns>
        public bool SetCodeVisibilityByCode(string code, bool flag)
        {
            string sql = string.Format("update AlarmCodeVisibility set CodeVisibility='{0}' where Code='{1}'", flag, code);
            int result = iDBHelper.ExcuteNonQueryInt(sql);
            if (result > 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 设置某一级别的报警码显示状态
        /// </summary>
        /// <param name="level">级别</param>
        /// <param name="flag">状态</param>
        /// <returns></returns>
        public bool SetCodeVisibilityByLevel(long[] level, bool[] flag)
        {
            List<string> sqls = new List<string>();
            for (int i = 0; i < level.Length; i++)
            {
                string sql = string.Format("update AlarmCodeVisibility set CodeVisibility='{0}' where CodeLevel='{1}'", flag[i], level[i]);
                sqls.Add(sql);
            }
            int result = iDBHelper.ExecuteNonQueryIntTransaction(sqls);
            if (result > 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 恢复报警码状态
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SetAlarmCodeVisibilityToDefault(int index)
        {
            string sql = string.Empty;
            if (index == 0)//如果 全选 则将所有报警的可显标志置位true
                sql = string.Format("update AlarmCodeVisibility set CodeVisibility= 'true'");
            else
                sql = string.Format("update AlarmCodeVisibility set CodeVisibility= 'true' where CodeLevel='{0}'", index);
            int result = iDBHelper.ExcuteNonQueryInt(sql);
            if (result > 0)
                return true;
            else
                return false;
        }
        #endregion

        /// <summary>
        /// 处理插入报警记录信息表数据(上位机)
        /// </summary>
        /// <param name="moduleID">模块类型</param>
        /// <param name="strCode">报警码</param>
        /// <param name="modelInResults">原始信息</param>
        /// <param name="isMention">报警图标是否闪烁</param>
        /// <returns></returns>
        public bool OperateAlarmInfoData(int moduleType, string strCode, AlarmOrignalInfoModel modelInResults, bool isMention = true, int moduleID = 0, byte[] bPara = null, string parameters = "")
        {
            try
            {
                alarmQueue.Enqueue(
                    new AlarmMessage()
                    {
                        IsSoftware = true,
                        SoftwareInfo = modelInResults.Info,
                        moduleTypeCode = moduleType,
                        strCode = strCode,
                        modelInResults = modelInResults,
                        isMention = isMention,
                        moduleID = moduleID,
                        bPara = bPara,
                        parameters = parameters,
                    });
                return true;
            }
            catch (Exception e)
            {
                LogHelper.debugAlarm.Error($"OperateAlarmInfoData up error ", e);
                return false;
            }
        }

        /// <summary>
        /// 处理插入报警记录信息表数据(下位机)
        /// </summary>
        /// <param name="moduleID">模块号</param>
        /// <param name="strCode">报警码</param>
        /// <param name="strLevel">报警级别</param>
        /// <param name="modelInResults">原始信息</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public bool OperateAlarmInfoData(int moduleTypeCode, int moduleID, string strCode, AlarmLevelEnum strLevel,
                                        AlarmOrignalInfoModel modelInResults, string parameters,
                                        byte[] bPara = null, DateTime? time = null, int? instrumentModuleId = null)
        {
            try
            {
                alarmQueue.Enqueue(
                new AlarmMessage()
                {
                    IsSoftware = false,
                    moduleTypeCode = moduleTypeCode,
                    moduleID = moduleID,
                    strCode = strCode,
                    AlarmTime = time,
                    strLevel = strLevel,
                    modelInResults = modelInResults,
                    parameters = parameters,
                    bPara = bPara,
                    InstrumentModuleId = instrumentModuleId
                });
                return true;
            }
            catch (Exception e)
            {
                LogHelper.debugAlarm.Error($"OperateAlarmInfoData down error ", e);
                return false;
            }
        }

        /// <summary>
        /// 报警信息处理线程
        /// </summary>
        public void AlarmHanlder()
        {
            while (true)
            {
                try
                {
                    if (alarmQueue.Count > 0)
                    {
                        AlarmMessage message = new AlarmMessage();

                        if (alarmQueue.TryDequeue(out message))
                        {

                            OperateAlarmInfoData(message);
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
                catch (ThreadAbortException tae)
                { }
                catch (Exception e)
                {
                    LogHelper.logSoftWare.Error("alarmHandlerThread 线程出现异常", e);
                    //throw e;
                }
            }
        }
        /// <summary>
        /// 数据最终存储
        /// </summary>
        /// <param name="message"></param>
        public void OperateAlarmInfoData(AlarmMessage message)
        {
            try
            {
                DateTime dtToday = DateTime.Now.Date;
                if (message.IsSoftware)
                {
                    AlarmHistoryInfoModel alarmItem = new AlarmHistoryInfoModel();
                    //插入新的报警
                    alarmItem.ModuleID = message.moduleID;
                    alarmItem.ModuleType = message.moduleTypeCode;
                    //更新历史信息表中存在的数据的删除标识
                    string strSqlInfo = message.SoftwareInfo;
                    if (SystemResources.Instance.CurrentLanguage != "CN")
                    {
                        strSqlInfo = strSqlInfo.Replace("'", "''");
                    }

                    string sql = @"update alarm_history_info set alarm_history_info.delete_flag='1' where alarm_history_info.delete_flag='0' and alarm_history_info.code=@P1 and alarm_history_info.info=@P2 and alarm_history_info.module_id=@P3 and alarm_history_info.alarm_time>=@P0";

                    int result = ExecuteSqlCommand(sql, dtToday, message.strCode, strSqlInfo, alarmItem.ModuleID);

                    alarmItem.Code = message.strCode;
                    // 上位机的报警有可能给下位机使用，例如尿生化，所以需要使用数据库中存储的级别来显示 sunch 20210726
                    alarmItem.CodeLevel = message.modelInResults != null ? message.modelInResults.CodeLevel : AlarmLevelEnum.Caution;
                    //LogHelper.debugAlarm.Debug($"software Alarm：Code：{message.strCode}，Module：{message.moduleTypeCode}，OginalInfo is null：{message.modelInResults != null}，Level：{alarmItem.CodeLevel}");

                    if (null != message.bPara)
                        alarmItem.Info = SetParamterInfo(message.SoftwareInfo, message.bPara);
                    else
                        alarmItem.Info = message.SoftwareInfo;

                    alarmItem.Parameters = message.parameters;
                    alarmItem.AlarmTime = message.AlarmTime ?? DateTime.Now;
                    alarmItem.AlarmStyle = AlarmStyleEnum.Data;
                    alarmItem.InstrumentModuleId = message.InstrumentModuleId;

                    if (message.modelInResults != null && message.modelInResults.CodeVisibility)
                        alarmItem.DeletedFlag = false;
                    else if (message.modelInResults == null && SystemResources.Instance.CurrentUserName == "dryf")
                        alarmItem.DeletedFlag = false;
                    else
                        alarmItem.DeletedFlag = true;

                    LogHelper.debugAlarm.Debug(alarmItem.ToString());

                    Insert(alarmItem);
                    Messenger.Default.Send<string>("AlarmRefreshByEvent", "AlarmRefreshByEvent");
                    // 尿份产线，添加报警跑马灯，需要报警信息，添加此消息通知
                    Messenger.Default.Send<object>(alarmItem, "NotifyAlarmRefresh");

                    //当前是注意级别报警，图标的状态不是空就需要赋值为注意，如果是故障级别，不能将当前故障再赋值成注意
                    if (message.modelInResults != null && message.modelInResults.CodeVisibility && message.isMention && SystemResources.Instance.AnalyzerAlarmLevel == AlarmLevel.None)
                        SystemResources.Instance.AnalyzerAlarmLevel = AlarmLevel.Warning;
                    //当前是注意级别报警，图标的状态不是空就需要赋值为注意，如果是故障级别，不能将当前故障再赋值成注意
                    else if (message.modelInResults == null && SystemResources.Instance.CurrentUserName == "dryf" && SystemResources.Instance.AnalyzerAlarmLevel == AlarmLevel.None)
                        SystemResources.Instance.AnalyzerAlarmLevel = AlarmLevel.Warning;

                    // 通知业务模块
                    if (AlarmMentionEvent != null)
                    {
                        if (!string.IsNullOrEmpty(message.modelInResults.DetailInfo))
                            alarmItem.DetailInfo = message.modelInResults == null ? string.Empty : Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(message.modelInResults.DetailInfo));
                        if (!string.IsNullOrEmpty(message.modelInResults.Solution))
                            alarmItem.Solution = message.modelInResults == null ? string.Empty : Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(message.modelInResults.Solution));
                        AlarmMentionEvent(AlarmEventOprateType.Add, alarmItem);
                    }
                }
                else
                {
                    //插入新的报警
                    AlarmHistoryInfoModel alarmItem = new AlarmHistoryInfoModel();
                    alarmItem.ModuleID = message.moduleID;
                    alarmItem.ModuleType = message.moduleTypeCode;
                    alarmItem.InstrumentModuleId = message.InstrumentModuleId;
                    alarmItem.Code = message.strCode;
                    alarmItem.CodeLevel = message.strLevel;
                    if (message.modelInResults != null)
                    {
                        if (null != message.bPara)
                        {
                            alarmItem.Info = SetParamterInfo(message.modelInResults.Info, message.bPara);
                        }
                        else
                        {
                            alarmItem.Info = SetParamterInfo(message.modelInResults.Info, message.parameters);
                        }
                    }
                    else
                    {
                        alarmItem.Info = StringResourceExtension.GetLanguage(138, "厂家保留报警信息");
                    }
                    alarmItem.AlarmTime = message.AlarmTime ?? DateTime.Now;
                    alarmItem.Parameters = message.parameters;
                    alarmItem.AlarmStyle = AlarmStyleEnum.Error;

                    //更新历史信息表中存在的数据的删除标识
                    string strSqlInfo = alarmItem.Info;
                    if (SystemResources.Instance.CurrentLanguage != "CN")
                    {
                        strSqlInfo = strSqlInfo.Replace("'", "''");
                    }

                    string sql = "update alarm_history_info set alarm_history_info.delete_flag='1' where alarm_history_info.delete_flag='0' and alarm_history_info.code='" + alarmItem.Code + "' and alarm_history_info.info='" + strSqlInfo + "' and alarm_history_info.module_id='" + alarmItem.ModuleID + "' and alarm_history_info.alarm_time>=@P0";

                    int result = ExecuteSqlCommand(sql, dtToday);

                    if (message.modelInResults != null && message.modelInResults.CodeVisibility)
                        alarmItem.DeletedFlag = false;
                    else if (message.modelInResults == null && SystemResources.Instance.CurrentUserName == "dryf")
                        alarmItem.DeletedFlag = false;
                    else
                        alarmItem.DeletedFlag = true;

                    LogHelper.debugAlarm.Debug(alarmItem.ToString());

                    Insert(alarmItem);
                    Messenger.Default.Send<string>("AlarmRefreshByEvent", "AlarmRefreshByEvent");
                    // 尿份产线，添加报警跑马灯，需要报警信息，添加此消息通知
                    Messenger.Default.Send<object>(alarmItem, "NotifyAlarmRefresh");

                    if (message.modelInResults != null && message.modelInResults.CodeVisibility)
                    {
                        //当前是注意级别报警，图标的状态不是空就需要赋值为注意，如果是故障级别，不能将当前故障再赋值成注意
                        if (message.strLevel == AlarmLevelEnum.Caution && SystemResources.Instance.AnalyzerAlarmLevel == AlarmLevel.None)
                            SystemResources.Instance.AnalyzerAlarmLevel = AlarmLevel.Warning;
                        //当前是故障级别报警，图标的状态不是故障就需要赋值为故障
                        else if ((message.strLevel == AlarmLevelEnum.SampleAdding || message.strLevel == AlarmLevelEnum.Stop) && SystemResources.Instance.AnalyzerAlarmLevel != AlarmLevel.Stop)
                            SystemResources.Instance.AnalyzerAlarmLevel = AlarmLevel.Stop;
                    }
                    else if (message.modelInResults == null && SystemResources.Instance.CurrentUserName == "dryf")
                    {
                        //当前是注意级别报警，图标的状态不是空就需要赋值为注意，如果是故障级别，不能将当前故障再赋值成注意
                        if (message.strLevel == AlarmLevelEnum.Caution && SystemResources.Instance.AnalyzerAlarmLevel == AlarmLevel.None)
                            SystemResources.Instance.AnalyzerAlarmLevel = AlarmLevel.Warning;
                        //当前是故障级别报警，图标的状态不是故障就需要赋值为故障
                        else if ((message.strLevel == AlarmLevelEnum.SampleAdding || message.strLevel == AlarmLevelEnum.Stop) && SystemResources.Instance.AnalyzerAlarmLevel != AlarmLevel.Stop)
                            SystemResources.Instance.AnalyzerAlarmLevel = AlarmLevel.Stop;
                    }
                    // 通知业务模块
                    if (AlarmMentionEvent != null)
                    {
                        if (!string.IsNullOrEmpty(message.modelInResults.DetailInfo))
                            alarmItem.DetailInfo = message.modelInResults == null ? string.Empty : Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(message.modelInResults.DetailInfo));
                        if (!string.IsNullOrEmpty(message.modelInResults.Solution))
                            alarmItem.Solution = message.modelInResults == null ? string.Empty : Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(message.modelInResults.Solution));
                        AlarmMentionEvent(AlarmEventOprateType.Add, alarmItem);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.debugAlarm.Error($"报警空引用问题监控(异常):{ex.ToString()}");
                throw ex;
            }
        }


        #region 报警记录信息处理
        /// <summary>
        /// 从数据库中获取当天报警相关信息：将所有信息加载到内存中
        /// </summary>
        /// <returns></returns>
        public List<AlarmHistoryInfoModel> GetAlarmTodayInfo(DateTime dtBegin, DateTime dtEnd)
        {
            List<AlarmHistoryInfoModel> result = new List<AlarmHistoryInfoModel>();
            using (DBContextBase _db = new DBContextBase())
            {
                if (SystemResources.Instance.CurrentUserName == "dryf")
                    result = _db.AlarmHistoryInfoModel.AsNoTracking().Where(p => p.AlarmTime >= dtBegin.Date && p.AlarmTime <= dtEnd.Date && p.DeletedFlag == false).OrderByDescending(o => o.AlarmTime).ToList();
                else
                    result = _db.AlarmHistoryInfoModel.AsNoTracking().Where(p => p.AlarmTime >= dtBegin.Date && p.AlarmTime <= dtEnd.Date && p.DeletedFlag == false && p.CodeLevel != AlarmLevelEnum.Debug).OrderByDescending(o => o.AlarmTime).ToList();
            }

            foreach (var item in result)
            {
                if (!DataDictionaryService.Instance.GetIsMultiModuleMode)
                    continue;

                var alarmMsg = SystemResources.Instance.AlarmOrignalInfos.FirstOrDefault(o => o.Code.Equals(item.Code));
                if (alarmMsg != null)
                {
                    item.DetailInfo = alarmMsg.DetailInfo;
                    item.Solution = alarmMsg.Solution;
                }


                ModuleTypeModel mType = null;
                if (DataDictionaryService.Instance.ModuleTypeInfo.TryGetValue(item.ModuleType, out mType))
                {
                    item.ModuleTypeName = mType.LanguageID != 0 ? SystemResources.Instance.LanguageArray[mType.LanguageID] : mType.ModuleTypeName;
                    var module = DataDictionaryService.Instance.ModuleInfoList.FirstOrDefault(o => o.ModuleID == item.ModuleID);
                    item.ModuleName = module == null ? string.Empty : module.ModuleName;
                }
                else
                {
                    LogHelper.debugAlarm.Debug($"报警未知键问题监控:未知模块类型: {item.ModuleType}, 模块号：{item.ModuleID}");
                }
            }
            return result;
        }
        /// <summary>
        /// 从数据库中获取所有报警相关信息：将所有信息加载到内存中
        /// </summary>
        /// <returns></returns>
        public List<AlarmHistoryInfoModel> GetAlarmHitoryInfo(DateTime dtBegin, DateTime dtEnd)
        {
            List<AlarmHistoryInfoModel> result = new List<AlarmHistoryInfoModel>();
            using (DBContextBase _db = new DBContextBase())
            {
                if (SystemResources.Instance.CurrentUserName == "dryf")
                    result = _db.AlarmHistoryInfoModel.AsNoTracking().Where(p => p.AlarmTime >= dtBegin.Date && p.AlarmTime <= dtEnd.Date).OrderByDescending(o => o.AlarmTime).ToList();
                else
                    result = _db.AlarmHistoryInfoModel.AsNoTracking().Where(p => p.AlarmTime >= dtBegin.Date && p.AlarmTime <= dtEnd.Date && p.CodeLevel != AlarmLevelEnum.Debug).OrderByDescending(o => o.AlarmTime).ToList();
            }

            foreach (var item in result)
            {
                if (!DataDictionaryService.Instance.GetIsMultiModuleMode)
                    continue;

                ModuleTypeModel mType = null;
                if (DataDictionaryService.Instance.ModuleTypeInfo.TryGetValue(item.ModuleType, out mType))
                {
                    item.ModuleTypeName = mType.LanguageID != 0 ? SystemResources.Instance.LanguageArray[mType.LanguageID] : mType.ModuleTypeName;
                    var module = DataDictionaryService.Instance.ModuleInfoList.FirstOrDefault(o => o.ModuleID == item.ModuleID);
                    item.ModuleName = module == null ? string.Empty : module.ModuleName;
                }
                else
                {
                    LogHelper.debugAlarm.Debug($"报警未知键问题监控:未知模块类型: {item.ModuleType}, 模块号：{item.ModuleID}");
                }
            }
            return result;
        }
        /// <summary>
        /// 从数据库中获取当天报警相关信息：将所有信息加载到内存中
        /// </summary>
        /// <returns></returns>
        public int GetAlarmHitoryInfoCount(DateTime dtBegin, DateTime dtEnd)
        {
            int result = 0;
            using (DBContextBase _db = new DBContextBase())
            {
                if (SystemResources.Instance.CurrentUserName == "dryf")
                    result = _db.AlarmHistoryInfoModel.AsNoTracking().Where(p => p.AlarmTime >= dtBegin.Date && p.AlarmTime <= dtEnd.Date).Count();
                else
                    result = _db.AlarmHistoryInfoModel.AsNoTracking().Where(p => p.AlarmTime >= dtBegin.Date && p.AlarmTime <= dtEnd.Date && p.CodeLevel != AlarmLevelEnum.Debug).Count();
            }

            return result;
        }
        /// <summary>
        /// 从数据库中获取当天报警相关信息：将所有信息加载到内存中
        /// </summary>
        /// <returns></returns>
        public List<AlarmHistoryInfoModel> GetAlarmHitoryInfo(DateTime dtBegin, DateTime dtEnd, int skipCount, int takeCount)
        {
            List<AlarmHistoryInfoModel> result = new List<AlarmHistoryInfoModel>();
            using (DBContextBase _db = new DBContextBase())
            {
                if (SystemResources.Instance.CurrentUserName == "dryf")
                    result = _db.AlarmHistoryInfoModel.AsNoTracking().Where(p => p.AlarmTime >= dtBegin.Date && p.AlarmTime <= dtEnd.Date).OrderByDescending(o => o.AlarmTime).Skip(skipCount).Take(takeCount).ToList();
                else
                    result = _db.AlarmHistoryInfoModel.AsNoTracking().Where(p => p.AlarmTime >= dtBegin.Date && p.AlarmTime <= dtEnd.Date && p.CodeLevel != AlarmLevelEnum.Debug).OrderByDescending(o => o.AlarmTime).Skip(skipCount).Take(takeCount).ToList();
            }
            foreach (var item in result)
            {
                item.ModuleTypeName = DataDictionaryService.Instance.GetIsMultiModuleMode == true ?
                                item.ModuleType == 0 ? string.Empty : DataDictionaryService.Instance.ModuleTypeInfo[item.ModuleType].ModuleTypeName :
                                string.Empty;
                item.ModuleName = DataDictionaryService.Instance.GetIsMultiModuleMode == true ?
                            item.ModuleID == 0 ?
                            (item.ModuleType == 0 ? string.Empty : DataDictionaryService.Instance.ModuleTypeInfo[item.ModuleType].ModuleTypeName) :
                            DataDictionaryService.Instance.ModuleTypeAndInfo[item.ModuleType].FirstOrDefault(o => o.ModuleID == item.ModuleID).ModuleName :
                            string.Empty;
            }
            return result;
        }
        /// <summary>
        /// 将选中的报警码的显示标志置为false
        /// </summary>
        /// <param name="id"></param>
        public bool DeleteOneAlarmHistoryInfo(Guid id)
        {
            using (DBContextBase _db = new DBContextBase())
            {
                AlarmHistoryInfoModel model = _db.AlarmHistoryInfoModel.FirstOrDefault(p => p.Id == id);

                if (model != null)
                {
                    model.DeletedFlag = true;
                    int result = _db.SaveChanges();
                    if (result > 0)
                    {
                        if (AlarmMentionEvent != null)
                            AlarmMentionEvent(AlarmEventOprateType.ClearOne, model);
                        return true;
                    }
                    else return false;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// 将报警列表中的报警显示标志全部置为False
        /// </summary>
        public bool DeleteAllShowInfos()
        {
            DateTime dtbegin = DateTime.Now.Date;
            DateTime dtend = DateTime.Now.Date.AddDays(1);

            string sql = string.Empty;
            sql = string.Format("UPDATE ALARM_HISTORY_INFO SET DELETE_FLAG=1 WHERE ALARM_TIME>=@P0 AND ALARM_TIME<=@P1 ");
            int result = ExecuteSqlCommand(sql, dtbegin, dtend);
            if (result > 0)
            {
                Messenger.Default.Send<string>("ClearAllAlarmFromPlatFormByEvent", "ClearAllAlarmFromPlatFormByEvent");

                if (AlarmMentionEvent != null)
                    AlarmMentionEvent(AlarmEventOprateType.ClearAll, null);
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 初始化报警统计数据表
        /// </summary>
        /// <returns></returns>
        public List<AlarmStatisticInfo> GetAlarmCodeStatistic()
        {
            List<AlarmStatisticInfo> result = new List<AlarmStatisticInfo>();
            using (DBContextBase _db = new DBContextBase())
            {
                List<AlarmHistoryInfoModel> list = new List<AlarmHistoryInfoModel>();
                list = _db.AlarmHistoryInfoModel.ToList();

                var resultTMP = from history in list
                                group history.Code by history.Code into g
                                select new AlarmStatisticInfo()
                                {
                                    Code = g.Key,
                                    Count = g.Count()
                                };
                result = resultTMP.ToList();
            }
            return result;
        }
        /// <summary>
        /// 清空报警信息
        /// </summary>
        /// <returns></returns>
        public bool ClearAlarmInfo()
        {
            DeleteAll();
            return true;
        }
        #endregion

        /// <summary>
        /// 将报警列表中的参数赋值
        /// </summary>
        public string SetParamterInfo(string msgInfo, byte[] bPara)
        {
            string strMsg = "";
            string strTemp = msgInfo;
            string strPara = "";

            if (bPara == null)
            {
                return msgInfo;
            }

            strPara = BinaryUtilHelper.ByteToASCII(bPara);

            if (msgInfo.IndexOf("{") == -1)
            {
                return msgInfo;
            }

            if (strPara.IndexOf("{") == -1)
            {
                return msgInfo;
            }

            string strNum = msgInfo.Substring(msgInfo.LastIndexOf("{") + 1, msgInfo.LastIndexOf("}") - msgInfo.LastIndexOf("{") - 1);
            if (string.IsNullOrEmpty(strNum))
            {
                return msgInfo;
            }

            int nParaNum = int.Parse(msgInfo.Substring(msgInfo.LastIndexOf("{") + 1, msgInfo.LastIndexOf("}") - msgInfo.LastIndexOf("{") - 1));
            for (int i = 0; i <= nParaNum; i++)
            {
                strMsg += strTemp.Substring(0, strTemp.IndexOf("{")) + strPara.Substring(strPara.IndexOf("{") + 1, strPara.IndexOf("}") - strPara.IndexOf("{") - 1);
                strTemp = strTemp.Remove(0, strTemp.IndexOf("}") + 1);
                strPara = strPara.Remove(0, strPara.IndexOf("}") + 1);
            }
            strMsg += strTemp;
            return strMsg;
        }

        /// <summary>
        /// 将报警列表中的参数赋值
        /// </summary>
        public string SetParamterInfo(string msgInfo, string bPara)
        {
            string strMsg = "";
            string strTemp = msgInfo;
            string strPara = "";

            if (string.IsNullOrEmpty(bPara) || string.IsNullOrEmpty(msgInfo))
            {
                return msgInfo;
            }

            strPara = bPara;

            if (msgInfo.IndexOf("{") == -1)
            {
                return msgInfo;
            }

            if (strPara.IndexOf("{") == -1)
            {
                return msgInfo;
            }

            string strNum = msgInfo.Substring(msgInfo.LastIndexOf("{") + 1, msgInfo.LastIndexOf("}") - msgInfo.LastIndexOf("{") - 1);
            if (string.IsNullOrEmpty(strNum))
            {
                return msgInfo;
            }

            int nParaNum = int.Parse(msgInfo.Substring(msgInfo.LastIndexOf("{") + 1, msgInfo.LastIndexOf("}") - msgInfo.LastIndexOf("{") - 1));
            for (int i = 0; i <= nParaNum; i++)
            {
                strMsg += strTemp.Substring(0, strTemp.IndexOf("{")) + strPara.Substring(strPara.IndexOf("{") + 1, strPara.IndexOf("}") - strPara.IndexOf("{") - 1);
                strTemp = strTemp.Remove(0, strTemp.IndexOf("}") + 1);
                strPara = strPara.Remove(0, strPara.IndexOf("}") + 1);
            }
            strMsg += strTemp;
            return strMsg;
        }
    }

    public class AlarmMessage
    {
        /// <summary>
        /// 是否为软件报警
        /// </summary>
        public bool IsSoftware { get; set; }
        /// <summary>
        /// 软件报警信息
        /// </summary>
        public string SoftwareInfo { get; set; }
        /// <summary>
        /// 模块类型编码
        /// </summary>
        public int moduleTypeCode { get; set; }
        /// <summary>
        /// 模块编号
        /// </summary>
        public int moduleID { get; set; }
        /// <summary>
        /// 报警码
        /// </summary>
        public string strCode { get; set; }
        /// <summary>
        /// 报警时间
        /// </summary>
        public DateTime? AlarmTime { get; set; }
        /// <summary>
        /// 报警级别
        /// </summary>
        public AlarmLevelEnum strLevel { get; set; }
        /// <summary>
        /// 原始报警信息
        /// </summary>
        public AlarmOrignalInfoModel modelInResults { get; set; }
        /// <summary>
        /// 转换后的参数
        /// </summary>
        public string parameters { get; set; }
        /// <summary>
        /// 下位机上传参数
        /// </summary>
        public byte[] bPara { get; set; }
        /// <summary>
        /// 仪器模块ID，用于统计
        /// </summary>
        public int? InstrumentModuleId { get; set; }
        /// <summary>
        /// 是否闪烁
        /// </summary>
        public bool isMention { get; set; }
    }
}
