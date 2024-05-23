using Sinboda.Framework.Common.CommonFunc;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.Interface;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Services
{
    /// <summary>
    /// 报警信息处理下位机报警类
    /// </summary>
    public class SystemAlarmService : ISystemAlarmService
    {
        /// <summary>
        /// 获取报警原始信息
        /// </summary>
        /// <returns></returns>
        public List<AlarmOrignalInfoModel> GetAlarmInfos()
        {
            return SystemAlarmModelOperations.Instance.GetAlarmInfos();
        }

        /// <summary>
        /// 报警码
        /// </summary>
        public string AlarmCode { get; internal set; }
        /// <summary>
        /// 报警码状态
        /// </summary>
        public bool AlarmCodeVisibility { get; internal set; }
        /// <summary>
        /// 报警码级别
        /// </summary>
        public AlarmLevelEnum AlarmCodeLevel { get; internal set; }

        /// <summary>
        /// 下位机报警处理（生化、发光使用）（8字节结构体）
        /// </summary>
        /// <param name="data">报警内容，不包含数据包头</param>
        /// <returns></returns>
        public bool AnalyzerAlarmHandler(AlarmStruct tempinstance)
        {
            string parameters = BytesToString(tempinstance.Param);
            LogHelper.debugAlarm.Debug($"Original Data : {tempinstance.Module} {tempinstance.Level} {tempinstance.Unit} {tempinstance.Code} ; {parameters}");

            try
            {
                bool result = false;
                // 组合报警码
                AlarmCode = tempinstance.Unit.ToString() + "-" + tempinstance.Code.ToString();
                // 转换报警级别
                int level = TranslateLevel(tempinstance.Level);
                if (level != -1)
                {
                    int moduleTypeCode = 0;
                    // 查找对应报警信息
                    AlarmOrignalInfoModel alarminfomodel = null;
                    if (DataDictionaryService.Instance.GetIsMultiModuleMode && tempinstance.Module != 0)
                    {
                        moduleTypeCode = DataDictionaryService.Instance.ModuleInfoList.FirstOrDefault(o => o.ModuleID == tempinstance.Module).ModuleTypeCode;
                        alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == moduleTypeCode && p.Code == AlarmCode).FirstOrDefault();
                    }
                    else
                    {
                        alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == 0 && p.Code == AlarmCode).FirstOrDefault();
                    }
                    // 与报警信息进行比较判断
                    AlarmLevelEnum strLevel = JudgeInfo(tempinstance, alarminfomodel, level);
                    // 判断是否为调试报警，非调试报警存储数据库
                    if (strLevel != AlarmLevelEnum.Debug)
                    {
                        LogHelper.debugAlarm.Debug($"Not Debug Alarm：Module：{tempinstance.Module}，Code:{AlarmCode}，Para:{parameters}");
                        if (parameters.Length > 50)
                            parameters = parameters.Substring(0, 49);
                        result = SystemAlarmModelOperations.Instance.OperateAlarmInfoData(moduleTypeCode, tempinstance.Module, AlarmCode, strLevel, alarminfomodel, parameters, tempinstance.Param);
                    }
                    else
                    {
                        LogHelper.debugAlarm.Debug($"Debug Alarm：Module：{tempinstance.Module}，Code:{AlarmCode}，Para:{parameters}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.debugAlarm.Error($"报警异常：\r\n{ex.Message} \r\n调用堆栈: \r\n{ex.ToString()}");
                return false;
            }
        }
        /// <summary>
        /// 下位机报警处理（生化、发光使用）（4字节结构体）
        /// </summary>
        /// <param name="data">报警内容，不包含数据包头</param>
        /// <returns></returns>
        public bool AnalyzerAlarmHandler(AlarmMiniStruct tempinstance, byte[] dataPara)
        {
            string parameters = BytesToString(dataPara);
            LogHelper.debugAlarm.Debug($"Original Data : {tempinstance.Module} {tempinstance.Level} {tempinstance.Unit} {tempinstance.Code} ; {parameters}");

            try
            {
                bool result = false;
                // 组合报警码
                AlarmCode = tempinstance.Unit.ToString() + "-" + tempinstance.Code.ToString();
                // 转换报警级别
                int level = TranslateLevel(tempinstance.Level);
                if (level != -1)
                {
                    int moduleTypeCode = 0;
                    // 查找对应报警信息
                    AlarmOrignalInfoModel alarminfomodel = null;
                    if (DataDictionaryService.Instance.GetIsMultiModuleMode && tempinstance.Module != 0)
                    {
                        moduleTypeCode = DataDictionaryService.Instance.ModuleInfoList.FirstOrDefault(o => o.ModuleID == tempinstance.Module).ModuleTypeCode;
                        alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == moduleTypeCode && p.Code == AlarmCode).FirstOrDefault();
                    }
                    else
                    {
                        alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == 0 && p.Code == AlarmCode).FirstOrDefault();
                    }
                    // 与报警信息进行比较判断
                    AlarmLevelEnum strLevel = JudgeInfo(tempinstance, alarminfomodel, level);
                    // 将参数转为明文
                    string parametersASCII = string.Empty;
                    if (dataPara.Where(o => o != 0).Count() > 0)
                        parametersASCII = BinaryUtilHelper.ByteToASCII(dataPara.Where(o => o != 0).ToArray());
                    // 判断是否为调试报警，非调试报警存储数据库
                    if (strLevel != AlarmLevelEnum.Debug)
                    {
                        LogHelper.debugAlarm.Debug($"Not Debug Alarm：Module：{tempinstance.Module}，Code:{AlarmCode}，Para:{parametersASCII}");
                        if (parametersASCII.Length > 50)
                            parametersASCII = parametersASCII.Substring(0, 49);
                        result = SystemAlarmModelOperations.Instance.OperateAlarmInfoData(moduleTypeCode, tempinstance.Module, AlarmCode, strLevel, alarminfomodel, parametersASCII, dataPara);
                    }
                    else
                    {
                        LogHelper.debugAlarm.Debug($"Debug Alarm：Module：{tempinstance.Module}，Code:{AlarmCode}，Para:{parametersASCII}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.debugAlarm.Error($"报警异常：\r\n{ex.Message} \r\n调用堆栈: \r\n{ex.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 仪器报警处理
        /// </summary>
        /// <param name="data">报警数据</param>
        /// <param name="moduleID">模块编码</param>
        /// <returns></returns>
        public bool AnalyzerAlarmHandler(AlarmMiniStruct tempinstance, byte[] dataPara, int moduleID)
        {
            string parameters = BytesToString(dataPara);
            LogHelper.debugAlarm.Debug($"Original Data : {tempinstance.Module} {tempinstance.Level} {tempinstance.Unit} {tempinstance.Code} ; {parameters} ; {moduleID}");

            try
            {
                bool result = false;
                // 组合报警码
                AlarmCode = tempinstance.Unit.ToString() + "-" + tempinstance.Code.ToString();
                // 转换报警级别
                int level = TranslateLevel(tempinstance.Level);
                if (level != -1)
                {
                    int moduleTypeCode = 0;
                    // 查找对应报警信息
                    AlarmOrignalInfoModel alarminfomodel = null;
                    if (DataDictionaryService.Instance.GetIsMultiModuleMode && moduleID != 0)
                    {
                        moduleTypeCode = DataDictionaryService.Instance.ModuleInfoList.FirstOrDefault(o => o.ModuleID == moduleID).ModuleTypeCode;
                        alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == moduleTypeCode && p.Code == AlarmCode).FirstOrDefault();
                    }
                    else
                    {
                        alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == 0 && p.Code == AlarmCode).FirstOrDefault();
                    }
                    // 与报警信息进行比较判断
                    AlarmLevelEnum strLevel = JudgeInfo(tempinstance, alarminfomodel, level);
                    // 将参数转为明文
                    string parametersASCII = string.Empty;
                    if (dataPara.Where(o => o != 0).Count() > 0)
                        parametersASCII = BinaryUtilHelper.ByteToASCII(dataPara.Where(o => o != 0).ToArray());
                    // 判断是否为调试报警，非调试报警存储数据库
                    if (strLevel != AlarmLevelEnum.Debug)
                    {
                        LogHelper.debugAlarm.Debug($"Not Debug Alarm：Module：{tempinstance.Module}，Code:{AlarmCode}，Para:{parametersASCII}");
                        if (parametersASCII.Length > 50)
                            parametersASCII = parametersASCII.Substring(0, 49);
                        result = SystemAlarmModelOperations.Instance.OperateAlarmInfoData(moduleTypeCode, moduleID, AlarmCode, strLevel, alarminfomodel, parametersASCII, dataPara);
                    }
                    else
                    {
                        LogHelper.debugAlarm.Debug($"Debug Alarm：Module：{tempinstance.Module}，Code:{AlarmCode}，Para:{parametersASCII}");
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.debugAlarm.Error($"报警异常：\r\n{ex.Message} \r\n调用堆栈: \r\n{ex.ToString()} \r\n模块号：{moduleID}");
                return false;
            }
        }

        /// <summary>
        /// 仪器报警处理（尿分使用）
        /// </summary>
        /// <param name="data">报警数据</param>
        /// <param name="moduleTypeCode">模块类型编码</param>
        /// <param name="moduleID">模块编码</param>
        /// <param name="highPosCode">高位code值</param>
        /// <returns></returns>
        public bool AnalyzerAlarmHandler(AlarmMiniStruct tempinstance, byte[] dataPara, int moduleTypeCode, int moduleID, byte highPosCode = 0x00)
        {
            string parameters = BytesToString(dataPara);
            LogHelper.debugAlarm.Debug($"Original Data : {tempinstance.Module} {tempinstance.Level} {tempinstance.Unit} {tempinstance.Code} ; {parameters} ; {moduleTypeCode} {moduleID}");

            try
            {
                bool result = false;
                int nCode = (highPosCode == 0x00 ? tempinstance.Code : ((highPosCode << 8) | tempinstance.Code));

                // 组合报警码
                AlarmCode = tempinstance.Unit.ToString() + "-" + nCode.ToString();
                // 转换报警级别
                int level = TranslateLevel(tempinstance.Level);
                if (level != -1)
                {
                    // 查找对应报警信息
                    AlarmOrignalInfoModel alarminfomodel = null;
                    if (DataDictionaryService.Instance.GetIsMultiModuleMode && moduleTypeCode != 0)
                    {
                        alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == moduleTypeCode && p.Code == AlarmCode).FirstOrDefault();
                    }
                    else
                    {
                        alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == 0 && p.Code == AlarmCode).FirstOrDefault();
                    }
                    // 与报警信息进行比较判断
                    AlarmLevelEnum strLevel = JudgeInfo(tempinstance, alarminfomodel, level);
                    // 将参数转为明文
                    string parametersASCII = string.Empty;
                    if (dataPara.Where(o => o != 0).Count() > 0)
                        parametersASCII = BinaryUtilHelper.ByteToASCII(dataPara.Where(o => o != 0).ToArray());
                    // 判断是否为调试报警，非调试报警存储数据库
                    if (strLevel != AlarmLevelEnum.Debug)
                    {
                        LogHelper.debugAlarm.Debug($"Not Debug Alarm：Module：{tempinstance.Module}，Code:{AlarmCode}，Para:{parametersASCII}");
                        if (parametersASCII.Length > 50)
                            parametersASCII = parametersASCII.Substring(0, 49);
                        result = SystemAlarmModelOperations.Instance.OperateAlarmInfoData(moduleTypeCode, tempinstance.Module, AlarmCode, strLevel, alarminfomodel, parametersASCII, dataPara, null, moduleID);
                    }
                    else
                    {
                        LogHelper.debugAlarm.Debug($"Debug Alarm：Module：{tempinstance.Module}，Code:{AlarmCode}，Para:{parametersASCII}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.debugAlarm.Error($"报警异常：\r\n{ex.Message} \r调用堆栈: \r\n{ex.ToString()} \r\n模块号：{moduleID}");
                return false;
            }
        }

        /// <summary>
        /// 仪器报警处理（血球专用使用）（不转换报警级别）
        /// </summary>
        /// <param name="tempinstance">AlarmMiniStruct 实例</param>
        /// <param name="dataPara">dataPara 参数</param>
        /// <param name="moduleTypeCode">模块类型</param>
        /// <param name="moduleID">模块编码</param>
        /// <param name="alarmTime">报警时间</param>
        /// <returns></returns>
        public bool AnalyzerAlarmHandler(AlarmMiniStruct tempinstance, byte[] dataPara, int moduleTypeCode, int moduleID, DateTime alarmTime)
        {
            string parameters = BytesToString(dataPara);
            LogHelper.debugAlarm.Debug($"Original Data : {tempinstance.Module} {tempinstance.Level} {tempinstance.Unit} {tempinstance.Code} ; {parameters} ; {moduleTypeCode} {moduleID}");

            try
            {
                bool result = false;
                // 组合报警码
                AlarmCode = tempinstance.Unit.ToString() + "-" + tempinstance.Code.ToString();
                // 转换报警级别
                int level = tempinstance.Level;
                if (level != -1)
                {
                    // 查找对应报警信息
                    AlarmOrignalInfoModel alarminfomodel = null;
                    if (DataDictionaryService.Instance.GetIsMultiModuleMode && moduleTypeCode != 0)
                    {
                        alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == moduleTypeCode && p.Code == AlarmCode).FirstOrDefault();
                    }
                    else
                    {
                        alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == 0 && p.Code == AlarmCode).FirstOrDefault();
                    }
                    // 与报警信息进行比较判断
                    AlarmLevelEnum strLevel = JudgeInfo(tempinstance, alarminfomodel, level);
                    // 将参数转为明文
                    string parametersASCII = string.Empty;
                    if (dataPara.Where(o => o != 0).Count() > 0)
                        parametersASCII = BinaryUtilHelper.ByteToASCII(dataPara.Where(o => o != 0).ToArray());
                    // 判断是否为调试报警，非调试报警存储数据库
                    if (strLevel != AlarmLevelEnum.Debug)
                    {
                        LogHelper.debugAlarm.Debug($"Not Debug Alarm：Module：{tempinstance.Module}，Code:{AlarmCode}，Para:{parametersASCII}");
                        if (parametersASCII.Length > 50)
                            parametersASCII = parametersASCII.Substring(0, 49);
                        result = SystemAlarmModelOperations.Instance.OperateAlarmInfoData(moduleTypeCode, moduleID, AlarmCode, strLevel, alarminfomodel, parametersASCII, dataPara, alarmTime);
                    }
                    else
                    {
                        LogHelper.debugAlarm.Debug($"Debug Alarm：Module：{tempinstance.Module}，Code:{AlarmCode}，Para:{parametersASCII}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.debugAlarm.Error($"报警异常：\r\n{ex.Message} \r调用堆栈: \r\n{ex.ToString()} \r\n模块号：{moduleID}");
                return false;
            }
        }

        #region 私有方法
        /// <summary>
        /// 级别转换
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private int TranslateLevel(byte level)
        {
            if (level >= 0 && level <= 9)
                return 3;
            else if (level >= 10 && level <= 99)
                return 2;
            else if (level >= 100 && level <= 149)
                return 1;
            else if (level >= 150 && level <= 180)
                return 4;
            else
            {
                LogHelper.debugAlarm.Debug($"Level Error！Code：{AlarmCode}，Level：{level}。");
                return -1;
            }
        }
        /// <summary>
        /// 判断报警码、级别与原始信息是否有差别（8字节结构体）
        /// </summary>
        /// <param name="tempinstance"></param>
        /// <param name="alarminfomodel"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private AlarmLevelEnum JudgeInfo(AlarmStruct tempinstance, AlarmOrignalInfoModel alarminfomodel, int level)
        {
            AlarmLevelEnum strLevel = (AlarmLevelEnum)level;
            AlarmCodeLevel = strLevel;
            if (alarminfomodel == null)
                LogHelper.debugAlarm.Debug($"Unknown Alarm！Code：{AlarmCode}，Level：{tempinstance.Level}。");
            else
            {
                AlarmCodeVisibility = alarminfomodel.CodeVisibility;
                if (strLevel != alarminfomodel.CodeLevel)
                    LogHelper.debugAlarm.Debug($"Level Error！Code：{AlarmCode}，Original Level：{alarminfomodel.CodeLevel}，Upload Level：{tempinstance.Level}。");
            }

            return strLevel;
        }
        /// <summary>
        /// 判断报警码、级别与原始信息是否有差别（4字节结构体）
        /// </summary>
        /// <param name="tempinstance"></param>
        /// <param name="alarminfomodel"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private AlarmLevelEnum JudgeInfo(AlarmMiniStruct tempinstance, AlarmOrignalInfoModel alarminfomodel, int level)
        {
            AlarmLevelEnum strLevel = (AlarmLevelEnum)level;
            AlarmCodeLevel = strLevel;
            if (alarminfomodel == null)
                LogHelper.debugAlarm.Debug($"Unknown Alarm！Code：{AlarmCode}，Level：{tempinstance.Level}。");
            else
            {
                AlarmCodeVisibility = alarminfomodel.CodeVisibility;
                if (strLevel != alarminfomodel.CodeLevel)
                    LogHelper.debugAlarm.Debug($"Level Error！Code：{AlarmCode}，Original Level：{alarminfomodel.CodeLevel}，Upload Level：{tempinstance.Level}。");
            }

            return strLevel;
        }
        /// <summary>
        /// 参数转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string BytesToString(byte[] data)
        {
            string result = string.Empty;
            foreach (var b in data)
                result += b.ToString("X2") + " ";
            return result;
        }
        #endregion

        /// <summary>
        /// 上位机软件报警处理（生化、免疫、凝血、血球）
        /// </summary>
        /// <param name="alarmCode">报警码</param>
        /// <param name="alarmInfo">报警信息</param>
        /// <param name="moduleTypeCode">模块类型编码</param>
        /// <param name="isMention">是否提示用户（图标闪烁）</param>
        /// <param name="moduleID">模块编号</param>
        /// <param name="isDebug">是否为调试报警</param>
        /// <returns></returns>
        public bool SoftWareAlarmHandler(string alarmCode, string alarmInfo, int moduleType, bool isMention = true, int moduleID = 0, bool isDebug = false)
        {
            bool result = false;
            string printInfo = string.Empty;
            printInfo += " <<< ";
            AlarmOrignalInfoModel alarminfomodel = null;
            if (DataDictionaryService.Instance.GetIsMultiModuleMode && moduleType != 0)
                alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == moduleType && p.Code == alarmCode).FirstOrDefault();
            else
                alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == 0 && p.Code == alarmCode).FirstOrDefault();
            if (alarminfomodel == null)
            {
                printInfo += string.Format($"Unknown Software Alarm！Code：{AlarmCode}。");
                LogHelper.debugAlarm.Debug(printInfo);

                alarminfomodel = new AlarmOrignalInfoModel();
                alarminfomodel.CodeVisibility = true;
            }
            alarminfomodel.Info = alarmInfo;
            if (!isDebug)
            {
                result = SystemAlarmModelOperations.Instance.OperateAlarmInfoData(moduleType, alarmCode, alarminfomodel, isMention, moduleID);
            }
            else
                LogHelper.debugAlarm.Debug($"Software Debug Alarm：Module：{moduleType}，Code:{AlarmCode}，Info:{alarmInfo}");
            return result;
        }
        /// <summary>
        /// 上位机软件报警处理（尿分使用）
        /// </summary>
        /// <param name="tempinstance">报警结构体</param>
        /// <param name="alarmInfo">报警信息</param>
        /// <param name="moduleTypeCode">模块类型编码</param>
        /// <param name="moduleID">模块号</param>
        /// <param name="resolveRule"></param>
        /// <returns></returns>
        public bool SoftWareAlarmHandler(AlarmMiniStruct tempinstance, string alarmInfo, int moduleTypeCode, int moduleID, int resolveRule = 0)
        {
            LogHelper.debugAlarm.Debug($"Original Data : {tempinstance.Module} {tempinstance.Level} {tempinstance.Unit} {tempinstance.Code} ; {alarmInfo} ; {moduleTypeCode} {moduleID}");

            try
            {
                bool result = false;
                // 组合报警码
                AlarmCode = tempinstance.Unit.ToString() + "-" + tempinstance.Code.ToString();
                // 转换报警级别
                int level = TranslateLevel(tempinstance.Level);
                if (level != -1)
                {
                    // 查找对应报警信息
                    AlarmOrignalInfoModel alarminfomodel = null;
                    if (DataDictionaryService.Instance.GetIsMultiModuleMode && moduleTypeCode != 0)
                    {
                        alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == moduleTypeCode && p.Code == AlarmCode).FirstOrDefault();
                    }
                    else
                    {
                        alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == 0 && p.Code == AlarmCode).FirstOrDefault();
                    }
                    // 与报警信息进行比较判断
                    AlarmLevelEnum strLevel = JudgeInfo(tempinstance, alarminfomodel, level);
                    // 判断是否为调试报警，非调试报警存储数据库
                    if (strLevel != AlarmLevelEnum.Debug)
                    {
                        LogHelper.debugAlarm.Debug($"Not Debug Alarm：Module：{tempinstance.Module}，Code:{AlarmCode}，Para:{alarmInfo}");
                        if (alarmInfo.Length > 50)
                            alarmInfo = alarmInfo.Substring(0, 49);
                        result = SystemAlarmModelOperations.Instance.OperateAlarmInfoData(moduleTypeCode, moduleID, AlarmCode, strLevel, alarminfomodel, alarmInfo, null);
                    }
                    else
                    {
                        LogHelper.debugAlarm.Debug($"Debug Alarm：Module：{tempinstance.Module}，Code:{AlarmCode}，Para:{alarmInfo}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.debugAlarm.Error($"报警异常：\r\n{ex.Message} \r调用堆栈: \r\n{ex.ToString()} \r\n模块号：{moduleID}");
                return false;
            }
        }

        /// <summary>
        /// 上位机软件报警处理（尿生化）
        /// </summary>
        /// <param name="alarmCode">报警码</param>
        /// <param name="alarmInfo">报警信息</param>
        /// <param name="moduleTypeCode">模块类型编码</param>
        /// <param name="dataPara">参数信息</param>
        /// <param name="isMention">是否提示用户（图标闪烁）</param>
        /// <param name="moduleID">模块编号</param>
        /// <param name="isDebug">是否为调试报警</param>
        /// <returns></returns>
        public bool SoftWareAlarmHandler(string alarmCode, string alarmInfo, int moduleType, byte[] dataPara, bool isMention = true, int moduleID = 0, bool isDebug = false)
        {
            bool result = false;
            string printInfo = string.Empty;
            printInfo += " <<< ";
            AlarmOrignalInfoModel alarminfomodel = null;
            if (DataDictionaryService.Instance.GetIsMultiModuleMode && moduleType != 0)
                alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == moduleType && p.Code == alarmCode).FirstOrDefault();
            else
                alarminfomodel = SystemResources.Instance.AlarmOrignalInfos.Where(p => p.ModuleType == 0 && p.Code == alarmCode).FirstOrDefault();
            if (alarminfomodel == null)
            {
                printInfo += string.Format($"Unknown Software Alarm！Code：{AlarmCode}。");
                LogHelper.debugAlarm.Debug(printInfo);

                alarminfomodel = new AlarmOrignalInfoModel();
                alarminfomodel.CodeVisibility = true;
            }
            alarminfomodel.Info = alarmInfo;
            // 将参数转为明文
            string parametersASCII = string.Empty;
            if (dataPara.Where(o => o != 0).Count() > 0)
                parametersASCII = BinaryUtilHelper.ByteToASCII(dataPara.Where(o => o != 0).ToArray());
            if (!isDebug)
            {
                result = SystemAlarmModelOperations.Instance.OperateAlarmInfoData(moduleType, alarmCode, alarminfomodel, isMention, moduleID, dataPara, parametersASCII);
            }
            else
                LogHelper.debugAlarm.Debug($"Software Debug Alarm：Module：{moduleType}，Code:{AlarmCode}，Info:{alarmInfo}");
            return result;
        }

        /// <summary>
        /// 清空报警信息
        /// </summary>
        /// <returns></returns>
        public bool ClearAlarmInfo()
        {
            return SystemAlarmModelOperations.Instance.ClearAlarmInfo();
        }
        /// <summary>
        /// 清空当天所有报警app
        /// </summary>
        /// <returns></returns>
        public bool AlarmClearAllMethod()
        {
            return SystemAlarmModelOperations.Instance.DeleteAllShowInfos();
        }

        /// <summary>
        /// 获取当天报警信息app
        /// </summary>
        /// <returns></returns>
        public List<AlarmHistoryInfoModel> GetAlarmHitoryInfosToday()
        {
            List<AlarmHistoryInfoModel> results = new List<AlarmHistoryInfoModel>();
            results = SystemAlarmModelOperations.Instance.GetAlarmTodayInfo(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
            return results;
        }
    }
}
