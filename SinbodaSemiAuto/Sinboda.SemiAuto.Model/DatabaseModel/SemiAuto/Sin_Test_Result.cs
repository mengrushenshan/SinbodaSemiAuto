using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto
{
    /// <summary>
    /// 结果表
    /// </summary>
    [Serializable]
    public partial class Sin_Test_Result : EntityModelBase
    {
        /// <summary>
        /// 样本主键
        /// </summary>
        public Guid Sample_id { get; set; }

        /// <summary>
        /// 测试号
        /// </summary>
        public int? Test_num { get; set; }

        /// <summary>
        /// 检测项目
        /// </summary>
        public string Item_test_name { get; set; }

        /// <summary>
        /// 检测项目类型
        /// </summary>
        public ItemType Item_type { get; set; }

        /// <summary>
        /// 试剂批号
        /// </summary>
        public string Item_reagent_lotno { get; set; }

        /// <summary>
        /// 测试结果(浓度)
        /// </summary>
        public double? Result { get; set; }

        /// <summary>
        /// 原始结果(吸光度)
        /// </summary>
        public double? Result_original { get; set; }

        /// <summary>
        /// 参考值范围用|分割(手工项目除外)
        /// </summary>
        public string Reference_range { get; set; }

        /// <summary>
        /// 线性范围(检测值范围)
        /// </summary>
        public string Linear_range { get; set; }

        /// <summary>
        /// 危急值范围
        /// </summary>
        public string Crisis_range { get; set; }

        /// <summary>
        /// 测试发送时间，结果文件夹生成日期、反应曲线看曲线界面使用日期
        /// </summary>
        public DateTime Sample_send_date { get; set; }

        /// <summary>
        /// 测试时间
        /// </summary>
        public DateTime? Sample_test_time { get; set; }

        /// <summary>
        /// 结果测试完成时间
        /// </summary>
        public DateTime? Sample_test_end_time { get; set; }

        /// <summary>
        /// 测试标志
        /// </summary>
        public TestState Test_state { get; set; } = TestState.Untested;

        /// <summary>
        /// 使用标识
        /// </summary>
        public bool Using_flag { get; set; }

        private bool recheck_flag;
        /// <summary>
        /// 复查标志（表示要做复查，正在复查）
        /// </summary>
        public bool Recheck_flag
        {
            get { return recheck_flag; }
            set { Set(ref recheck_flag, value); }
        }

        /// <summary>
        /// 结果范围标志
        /// </summary>
        public ResultRangeMark ResultRangeMark { get; set; }

        /// <summary>
        /// 结果异常标志
        /// </summary>
        public ResultErrorMark ResultErrorMark { get; set; }

        /// <summary>
        /// 测试结果类型
        /// </summary>
        public TestResultType Test_result_type { get; set; } = TestResultType.Normal;

        /// <summary>
        /// 结果修改标记
        /// </summary>
        public bool Result_update_flag { get; set; } = false;

        /// <summary>
        /// 复查原因
        /// </summary>
        public string Recheck_reason { get; set; }

        /// <summary>
        /// 测试元数据文件名
        /// </summary>
        public string Test_file_name { get; set; }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int Digits { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 不可编辑
        /// </summary>
        public bool NonEditable { get; set; }

        public override string ToString()
        {
            return $"测试号 {Test_num} 项目 {Item_test_name} 原始结果 {Result_original} 测试结果 {Result} 测试状态 {Test_state} 复查标志 {Recheck_flag} 结果类型 {Test_result_type.ToString()} 结果范围标志 {ResultErrorMark} 结果异常标志 {ResultErrorMark} 文件名 {Test_file_name}";
        }
    }
}
