using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.Model.DatabaseModel.RegexLC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto
{
    /// <summary>
    /// 样本表
    /// </summary>
    [Serializable]
    public partial class Sin_Sample : EntityModelBase
    {
        /// <summary>
        /// 患者编号
        /// </summary>
        public Guid Patient_id { get; set; }

        private int sampleCode;
        /// <summary>
        /// 样本编号
        /// </summary>
        public int SampleCode
        {
            get { return sampleCode; }
            set { Set(ref sampleCode, value); }
        }

        private string barcode;
        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode
        {
            get { return barcode; }
            set
            {
                if (string.IsNullOrEmpty(value) || RegexLCClass.BarcodeRegex.IsMatch(value))
                    Set(ref barcode, value);
            }
        }

        private int? rackDish;
        /// <summary>
        /// 样本架
        /// <para></para>
        /// </summary>
        public int? RackDish
        {
            get { return rackDish; }
            set { Set(ref rackDish, value); }
        }

        private int? position;
        /// <summary>
        /// 位置
        /// </summary>
        public int? Position
        {
            get { return position; }
            set { Set(ref position, value); }
        }

        /// <summary>
        /// 登记日期（按时间查询样本）
        /// </summary>
        public DateTime Sample_date { get; set; }

        private TestState test_state;
        /// <summary>
        /// 样本状态（未完成、待测试、测试中、已完成）
        /// </summary>
        public TestState Test_state
        {
            get { return test_state; }
            set { Set(ref test_state, value); }
        }

        /// <summary>
        /// 删除标志
        /// </summary>
        public bool Delete_flag { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime? Expiry_date { get; set; }

        /// <summary>
        /// 送检科室
        /// </summary>
        public string Send_office { get; set; }

        /// <summary>
        /// 送检时间
        /// </summary>
        public DateTime Send_time { get; set; }

        /// <summary>
        /// 送检医生
        /// </summary>
        public string Send_doctor { get; set; }

        private DateTime sampling_time;
        /// <summary>
        /// 采样时间
        /// </summary>
        public DateTime Sampling_time
        {
            get { return sampling_time; }
            set { Set(ref sampling_time, value); }
        }

        /// <summary>
        /// 检验者
        /// </summary>
        public string Test_doctor { get; set; }

        private DateTime? test_time;
        /// <summary>
        /// 检查完成时间
        /// </summary>
        public DateTime? Test_time
        {
            get { return test_time; }
            set { Set(ref test_time, value); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 实验板号
        /// </summary>
        public int BoardId { get; set; }

        [NotMapped]
        public Sin_Test_Result TestResult { get; set; }

        public override string ToString()
        {
            return $"样本编号 {SampleCode} 条码 {Barcode} 位置{Position} 架号{RackDish} 样本状态 {Test_state} 检查完成时间 {Test_time}";
        }
    }
}
