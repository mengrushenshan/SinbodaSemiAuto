using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Enums
{
    /// <summary>
    /// 报警级别
    /// </summary>
    public enum AlarmLevel
    {
        /// <summary>
        /// 无报警
        /// </summary>
        None,
        /// <summary>
        /// 注意级别
        /// </summary>
        Warning,
        /// <summary>
        /// 停止级别
        /// </summary>
        Stop,
    }
    /// <summary>
    /// 系统报警类别枚举
    /// </summary>
    public enum AlarmStyleEnum
    {
        /// <summary>
        /// 全部
        /// </summary>
        All,
        /// <summary>
        /// 数据
        /// </summary>
        Data,
        /// <summary>
        /// 故障
        /// </summary>
        Error,
    }
    /// <summary>
    /// 系统报警级别枚举
    /// </summary>
    public enum AlarmLevelEnum
    {
        /// <summary>
        /// 所有
        /// </summary>
        All,
        /// <summary>
        /// 注意
        /// </summary>
        Caution,
        /// <summary>
        /// 加样停止
        /// </summary>
        SampleAdding,
        /// <summary>
        /// 停止
        /// </summary>
        Stop,
        /// <summary>
        /// 调试
        /// </summary>
        Debug
    }
    /// <summary>
    /// 报警模块类型（尿线专用）
    /// </summary>
    public enum AlarmModuleType
    {
        /// <summary>
        /// 默认值 -1
        /// </summary>
        UNDEFINE = -1,
        /// <summary>
        /// 预留（根据平台人员建议有效值大于0）
        /// </summary>
        RESERVE = 0,
        /// <summary>
        /// 干化学模块(H)
        /// </summary>
        H,
        /// <summary>
        /// 形态学模块(F)
        /// </summary>
        F,
        /// <summary>
        /// 一体机模块(HF)
        /// </summary>
        HF,
        /// <summary>
        /// 轨道模块（还有MUS-9600）(ST)
        /// </summary>
        ST
    }
}
