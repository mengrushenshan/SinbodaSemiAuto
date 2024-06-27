using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.Enum
{
    #region 产品类型 ProductType
    /// <summary>
    /// 产品类型
    /// </summary>
    [EnumAnnotate("产品类型", 7904)]
    public enum ProductType
    {
        /// <summary>
        /// 默认值，表示通用类型
        /// </summary>
        [EnumAnnotate("默认值", 5523)]
        Common = 0,

        /// <summary>
        /// 服务器
        /// </summary>
        [EnumAnnotate("服务器", 7906)]
        Server,

        /// <summary>
        /// 样本台
        /// </summary>
        [EnumAnnotate("样本台", 6713)]
        Platform,

        /// <summary>
        /// 凝血 3000
        /// </summary>
        [EnumAnnotate("辛伯至 001", 0)]
        Sinboda001

    }
    #endregion

    #region 测试状态 TestState
    /// <summary>
    /// 测试状态
    /// </summary>
    [Serializable]
    [EnumAnnotate("测试状态", 6703)]
    public enum TestState
    {
        //校准，质控状态 成功，失败
        //样本状态 未测试，测试中，完成，失败
        //测试项目，未测试，测试中，完成，失败
        None,
        /// <summary>
        /// 未完成
        /// </summary>
        [EnumAnnotate("未完成", 292)]
        Untested,
        /// <summary>
        /// 测试中
        /// </summary>
        [EnumAnnotate("测试中", 1237)]
        Testing,
        /// <summary>
        /// 成功
        /// </summary>
        [EnumAnnotate("成功", 1118)]
        Success,
        /// <summary>
        /// 完成
        /// </summary>
        [EnumAnnotate("完成", 283)]
        Complete,
        /// <summary>
        /// 失败
        /// </summary>
        [EnumAnnotate("失败", 1137)]
        Failed,
        /// <summary>
        /// 过期
        /// </summary>
        [EnumAnnotate("过期", 2870)]
        Expired
    }
    #endregion

    #region 项目类型 ItemType

    /// <summary>
    /// 项目类型
    /// </summary>
    [EnumAnnotate("项目类型", 1912)]
    public enum ItemType
    {

        /// <summary>
        ///
        /// </summary>
        [EnumAnnotate("单分子", 1888)]
        SingleMolecule = 0,
        /// <summary>
        ///衍生项目
        /// </summary>
        [EnumAnnotate("衍生项目", 2279)]
        MoleculeRim,
        /// <summary>
        ///凝血计算项目
        /// </summary>
        [EnumAnnotate("计算项目", 1478)]
        CalCulation,
    }

    #endregion

    #region 性别 Sex

    /// <summary>
    /// 性别
    /// </summary>
    [EnumAnnotate("性别", 371)]
    public enum Sex
    {
        /// <summary>
        /// 空
        /// </summary>
        [EnumAnnotate("", 0)]
        None,
        /// <summary>
        /// 男
        /// </summary>
        [EnumAnnotate("男", 69)]
        Male,
        /// <summary>
        /// 女
        /// </summary>
        [EnumAnnotate("女", 70)]
        Female,
        /// <summary>
        /// 未知
        /// </summary>
        [EnumAnnotate("未知", 71)]
        Unknow
    }

    #endregion

    #region 年龄的单位 AgeUnit

    /// <summary>
    /// 年龄的单位
    /// </summary>
    [Serializable]
    [EnumAnnotate("年龄单位", 1255)]
    public enum AgeUnit
    {
        /// <summary>
        /// 岁
        /// </summary>
        [EnumAnnotate("岁", 1727)]
        Annum,

        /// <summary>
        /// 月
        /// </summary>
        [EnumAnnotate("月", 73)]
        Month,

        /// <summary>
        /// 天
        /// </summary>
        [EnumAnnotate("天", 813)]
        Day,

        /// <summary>
        /// 小时
        /// </summary>
        [EnumAnnotate("小时", 812)]
        Hour
    }

    #endregion

    #region 测试结果类型 TestResultType

    /// <summary>
    /// 测试结果类型
    /// </summary>
    [Serializable]
    [EnumAnnotate("分子测试结果类型", 7935)]
    public enum TestResultType
    {
        /// <summary>
        /// 无类型
        /// </summary>
        None,
        /// <summary>
        /// 正常
        /// </summary>
        [EnumAnnotate("常规测试", 735)]
        Normal,
        /// <summary>
        /// 手工复查
        /// </summary>
        [EnumAnnotate("手工复查测试", 7937)]
        ManualRecheck,
        /// <summary>
        /// 自动复查
        /// </summary>
        [EnumAnnotate("自动复查测试", 7938)]
        AutoRecheck
    }

    #endregion

    #region 异常结果标志类型 ResultRangeMark

    /// <summary>
    /// 结果范围标志
    /// </summary>
    [EnumAnnotate("结果范围标志")]
    public enum ResultRangeMark
    {
        /// <summary>
        /// 正常结果
        /// </summary>
        [EnumAnnotate("正常结果", 2813)]
        None,
        /// <summary>
        /// 结果超出参考区间上限
        /// </summary>
        [EnumAnnotate("结果超出参考区间上限", 2814)]
        ResultHighRange,
        /// <summary>
        /// 结果超出参考区间下限
        /// </summary>
        [EnumAnnotate("结果超出参考区间下限", 2815)]
        ResultLowRange,
        /// <summary>
        /// 浓度超出线性范围上限(0-5)
        /// </summary>
        [EnumAnnotate("浓度超出线性范围上限", 8217)]
        TiterHighRange,
        /// <summary>
        /// 浓度超出线性范围下限(0-5)
        /// </summary>
        [EnumAnnotate("浓度超出线性范围下限", 8218)]
        TiterLowRange,
        /// <summary>
        /// 结果超出危急值上限(0-76)
        /// </summary>
        [EnumAnnotate("结果超出危急值上限", 2820)]
        ResultDangerousHighRange,
        /// <summary>
        /// 结果超出危急值下限(0-76)
        /// </summary>
        [EnumAnnotate("结果超出危急值下限", 2821)]
        ResultDangerousLowRange,
        /// <summary>
        /// 吸光度线性超出(0-6)
        /// </summary>
        [EnumAnnotate("吸光度线性超出", 8219)]
        OverAbsLinear,
        /// <summary>
        /// 超出前带检查(0-8)
        /// </summary>
        [EnumAnnotate("超出前带检查", 8221)]
        OverProzoneCheck,
        /// <summary>
        /// 液面误探
        /// </summary>
        [EnumAnnotate("液面探测受到干扰", 19843)]
        LiquidFalseLevel,
    }

    /// <summary>
    /// 结果异常标志
    /// </summary>
    [EnumAnnotate("结果异常标志", 2431)]
    public enum ResultErrorMark
    {
        /// <summary>
        /// 正常结果
        /// </summary>
        [EnumAnnotate("正常结果", 2813)]
        None,
        /// <summary>
        /// 可信度低
        /// </summary>
        [EnumAnnotate("可信度低", 3006)]              // 可信度低
        LowCredibility,
        /// <summary>
        /// 异常结果
        /// </summary>
        [EnumAnnotate("异常结果", 1502)]                 // 异常结果
        WrongResult,
        /// <summary>
        /// 超出测试时间
        /// </summary>
        [EnumAnnotate("超出测试时间", 6141)]
        EstimateLowerLimit,
        /// 校准过期
        /// </summary>
        [EnumAnnotate("校准过期", 162)]
        CalibExpired,
        /// <summary>
        /// 分析失败，结果无意义
        /// </summary>
        AnalysisFailed,
        /// <summary>
        /// 分析错误，无结果
        /// </summary>
        AnalysisWrong,

        /// <summary>
        /// 紧急停止(仪器导致)
        /// </summary>
        [EnumAnnotate("仪器紧急停止", 3248)]
        EmergencyStop,
        /// <summary>
        /// 异常停机
        /// </summary>
        [EnumAnnotate("异常停机", 2450)]
        AbnormalDowntime,
        /// <summary>
        /// 手动急停(人工导致)
        /// </summary>
        EmergencyStopByHand,

        /// <summary>
        /// 发送测试取消测试
        /// </summary>
        CancelSendTest,
        /// <summary>
        /// 吸光度超出反应界限(0-9)
        /// </summary>
        [EnumAnnotate("吸光度超出反应界限", 2823)]
        OverReactionAbs,
        /// <summary>
        /// 样本跳过
        /// </summary>
        [EnumAnnotate("样本跳过", 2451)]
        SampleSkip,
        /// <summary>
        /// 试剂跳过
        /// </summary>
        [EnumAnnotate("试剂跳过", 2452)]
        ReagentSkip,
        
    }

    #endregion

    #region 电机相关
    /// <summary>
    /// 电机id
    /// </summary>
    public enum MotorId : int
    {
        Xaxis,
        Yaxis
    }

    /// <summary>
    /// 方向
    /// </summary>
    [EnumAnnotate("方向", 0)]
    public enum Direction : int
    {
        [EnumAnnotate("反向", 0)]
        Backward,

        [EnumAnnotate("正向", 0)]
        Forward
    }

    /// <summary>
    /// 速率
    /// </summary>
    [EnumAnnotate("速率", 0)]
    public enum Rate : int
    {
        [EnumAnnotate("慢速", 0)]
        slow,

        [EnumAnnotate("快速", 0)]
        fast
    }

    /// <summary>
    /// 速率
    /// </summary>
    [EnumAnnotate("移动", 0)]
    public enum Motion : int
    {
        [EnumAnnotate("停止", 0)]
        stop,

        [EnumAnnotate("运动", 0)]
        run
    }
    #endregion

    #region 测试类型
    [EnumAnnotate("测试类型", 0)]
    public enum TestType
    {
        [EnumAnnotate("", 0)]
        None,
        [EnumAnnotate("样本", 0)]
        Sample,
        [EnumAnnotate("聚焦", 0)]
        Focus,
        [EnumAnnotate("质控", 0)]
        QualityControl,
        [EnumAnnotate("校准", 0)]
        Calibration
    }
    #endregion

    #region 测试项目
    [EnumAnnotate("测试项目", 0)]
    public enum TestItem
    {
        [EnumAnnotate("AD", 0)]
        AD,
        [EnumAnnotate("PD", 0)]
        PD
    }
    #endregion

    [AttributeUsage(AttributeTargets.All)]
    public sealed class EnumAnnotate : Attribute
    {
        /// <summary>
        /// 枚举类型注释
        /// </summary>
        public string Annotate { get; }

        /// <summary>
        /// 多语言ID
        /// </summary>
        public int Lid { get; }

        public EnumAnnotate(string annotate)
        {
            Annotate = annotate;
        }

        public EnumAnnotate(string annotate, int lid)
        {
            Annotate = annotate;
            Lid = lid;
        }


        public static string Get(Type tp, string name)
        {
            MemberInfo[] mi = tp.GetMember(name);
            if (mi != null && mi.Length > 0)
            {
                EnumAnnotate attr = GetCustomAttribute(mi[0], typeof(EnumAnnotate)) as EnumAnnotate;
                if (attr != null)
                {
                    return attr.Annotate;
                }
            }
            return null;
        }
        public static EnumAnnotate Get(object enm)
        {
            if (enm != null)
            {
                MemberInfo[] mi = enm.GetType().GetMember(enm.ToString());
                if (mi != null && mi.Length > 0)
                {
                    EnumAnnotate attr = GetCustomAttribute(mi[0], typeof(EnumAnnotate)) as EnumAnnotate;
                    if (attr != null)
                    {
                        return attr;
                    }
                }
            }
            return null;
        }
        public static EnumAnnotate Get(Type enm)
        {
            if (enm != null)
            {
                Attribute[] attrs = enm.GetCustomAttributes() as Attribute[];
                foreach (Attribute attr in attrs)
                    if (attr is EnumAnnotate)
                        return (EnumAnnotate)attr;
            }
            return null;
        }
        public static List<Type> GetEnum()
        {
            List<Type> lt = new List<Type>();
            Type[] ts = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type t in ts)
            {
                if (t.IsEnum)
                {
                    lt.Add(t);
                }
            }
            return lt;
        }

    }
}
