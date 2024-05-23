using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Interface
{
    /// <summary>
    /// 获取原始报警信息接口
    /// </summary>
    public interface ISystemAlarmService
    {
        /// <summary>
        /// 获取原始报警信息
        /// </summary>
        /// <returns>原始报警信息</returns>
        List<AlarmOrignalInfoModel> GetAlarmInfos();

        /// <summary>
        /// 报警码
        /// </summary>
        string AlarmCode { get; }
        /// <summary>
        /// 报警码状态
        /// </summary>
        bool AlarmCodeVisibility { get; }
        /// <summary>
        /// 报警码级别
        /// </summary>
        AlarmLevelEnum AlarmCodeLevel { get; }

        /// <summary>
        /// 仪器报警处理（生化、发光、凝血使用）（8字节结构体）
        /// </summary>
        /// <param name="data">报警结构体</param>
        /// <returns></returns>
        bool AnalyzerAlarmHandler(AlarmStruct data);
        /// <summary>
        /// 仪器报警处理（生化、发光、凝血使用）（4字节结构体）
        /// </summary>
        /// <param name="data">报警结构体</param>
        /// <param name="dataPara">参数信息</param>
        /// <returns></returns>
        bool AnalyzerAlarmHandler(AlarmMiniStruct data, byte[] dataPara);
        /// <summary>
        /// 仪器报警处理（暂无使用）
        /// </summary>
        /// <param name="data">报警结构体</param>
        /// <param name="dataPara">参数信息</param>
        /// <param name="moduleID">模块编码</param>
        /// <returns></returns>
        bool AnalyzerAlarmHandler(AlarmMiniStruct data, byte[] dataPara, int moduleID);
        /// <summary>
        /// 仪器报警处理（尿分使用）
        /// </summary>
        /// <param name="data">报警结构体</param>
        /// <param name="dataPara">参数信息</param>
        /// <param name="moduleTypeCode">模块类型编码</param>
        /// <param name="moduleID">模块编码</param>
        /// <param name="highPosCode">高位code值</param>
        /// <returns></returns>
        bool AnalyzerAlarmHandler(AlarmMiniStruct data, byte[] dataPara, int moduleTypeCode, int moduleID, byte highPosCode = 0x00);

        /// <summary>
        /// 仪器报警处理（血球使用）
        /// </summary>
        /// <param name="data">报警结构体</param>
        /// <param name="dataPara">参数信息</param>
        /// <param name="moduleTypeCode">模块类型编码</param>
        /// <param name="moduleID">模块编码</param>
        /// <param name="alarmTime">报警时间</param>
        /// <returns></returns>
        bool AnalyzerAlarmHandler(AlarmMiniStruct data, byte[] dataPara, int moduleTypeCode, int moduleID, DateTime alarmTime);

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
        bool SoftWareAlarmHandler(string alarmCode, string alarmInfo, int moduleTypeCode, bool isMention = true, int moduleID = 0, bool isDebug = false);
        /// <summary>
        /// 位机软件报警处理（尿分使用）
        /// </summary>
        /// <param name="tempinstance">报警结构体</param>
        /// <param name="alarmInfo">报警信息</param>
        /// <param name="moduleTypeCode">模块类型编码</param>
        /// <param name="moduleID">模块号</param>
        /// <param name="resolveRule"></param>
        /// <returns></returns>
        bool SoftWareAlarmHandler(AlarmMiniStruct tempinstance, string alarmInfo, int moduleTypeCode, int moduleID, int resolveRule = 0);
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
        bool SoftWareAlarmHandler(string alarmCode, string alarmInfo, int moduleType, byte[] dataPara, bool isMention = true, int moduleID = 0, bool isDebug = false);

        /// <summary>
        /// 清空报警信息
        /// </summary>
        /// <returns>是否清除成功</returns>
        bool ClearAlarmInfo();
        /// <summary>
        /// 清空所有报警
        /// </summary>
        /// <returns>是否清除成功</returns>
        bool AlarmClearAllMethod();
        /// <summary>
        /// 获取当天报警信息
        /// </summary>
        /// <returns>报警信息集合</returns>
        List<AlarmHistoryInfoModel> GetAlarmHitoryInfosToday();
    }
}
