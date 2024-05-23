using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels
{
    /// <summary>
    /// 报警结构体
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AlarmStruct
    {
        /// <summary>
        /// 模块号
        /// </summary>
        public byte Module;
        /// <summary>
        /// 级别
        /// </summary>
        public byte Level;
        /// <summary>
        /// 单元
        /// </summary>
        public byte Unit;
        /// <summary>
        /// 报警码
        /// </summary>
        public byte Code;
        /// <summary>
        /// 参数
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Param;
    }

    /// <summary>
    /// 报警结构体
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AlarmMiniStruct
    {
        /// <summary>
        /// 模块号
        /// </summary>
        public byte Module;
        /// <summary>
        /// 级别
        /// </summary>
        public byte Level;
        /// <summary>
        /// 单元
        /// </summary>
        public byte Unit;
        /// <summary>
        /// 报警码
        /// </summary>
        public byte Code;
    }

    /// <summary>
    /// 报警原始信息
    /// </summary>
    public class AlarmOrignalInfoModel : EntityModelBase
    {
        /// <summary>
        /// 报警码
        /// </summary>       
        public string Code { get; set; }
        /// <summary>
        /// 报警级别
        /// </summary>
        public AlarmLevelEnum CodeLevel { get; set; }
        /// <summary>
        /// 报警内容
        /// </summary>        
        public string Info { get; set; }
        /// <summary>
        /// 报警详细信息
        /// </summary>
        public string DetailInfo { get; set; }
        /// <summary>
        /// 用户报警解决方法
        /// </summary>
        public string Solution { get; set; }
        /// <summary>
        /// 报警显示标志
        /// </summary>
        public bool CodeVisibility { get; set; }
        /// <summary>
        /// 模块类型
        /// </summary>
        public int ModuleType { get; set; }
        /// <summary>
        /// 模块类型名称
        /// </summary>
        public string ModuleTypeName { get; set; }
        /// <summary>
        /// 报警提示音
        /// </summary>
        public bool HaveSound { get; set; }
    }
    /// <summary>
    /// 报警记录信息
    /// </summary>
    public partial class AlarmHistoryInfoModel : EntityModelBase
    {
        /// <summary>
        /// 报警码 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 报警类型
        /// </summary>
        [NotMapped]
        public AlarmModuleType AlarmModuleType { get; set; }
        /// <summary>
        /// 报警级别
        /// </summary>
        public AlarmLevelEnum CodeLevel { get; set; }
        /// <summary>
        /// 报警内容
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// 报警详细信息
        /// </summary>
        [NotMapped]
        public string DetailInfo { get; set; }
        /// <summary>
        /// 解决方法
        /// </summary>
        [NotMapped]
        public string Solution { get; set; }
        /// <summary>
        /// 报警时间
        /// </summary>        
        public DateTime AlarmTime { get; set; }
        /// <summary>
        /// 报警类型：1数据报警、2故障报警
        /// </summary>        
        public AlarmStyleEnum AlarmStyle { get; set; }
        /// <summary>
        /// 是否已删除标志 true已删除
        /// </summary>
        public bool? DeletedFlag { get; set; }
        /// <summary>
        /// 模块编号
        /// </summary>
        public int ModuleID { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        [NotMapped]
        public string ModuleName { get; set; }
        /// <summary>
        /// 模块类型
        /// </summary>
        public int ModuleType { get; set; }
        /// <summary>
        /// 模块类型名称
        /// </summary>
        [NotMapped]
        public string ModuleTypeName { get; set; }
        /// <summary>
        /// 下位机上传参数
        /// </summary>
        public string Parameters { get; set; }
        /// <summary>
        /// 仪器模块ID
        /// </summary>
        public int? InstrumentModuleId { get; set; }

        public override string ToString()
        {
            return $"Code:{Code} CodeLevel:{CodeLevel} Info:{Info} AlarmTime:{AlarmTime} AlarmStyle:{AlarmStyle} DeletedFlag:{DeletedFlag} ModuleID:{ModuleID} ModuleType:{ModuleType} Parameters:{Parameters}";
        }
    }
    /// <summary>
    /// 报警统计信息
    /// </summary>
    public class AlarmStatisticInfo
    {
        /// <summary>
        /// 报警码 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 报警级别
        /// </summary>
        public int Count { get; set; }
    }
}
