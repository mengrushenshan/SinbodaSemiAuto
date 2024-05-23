using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto
{
    /// <summary>
    /// 患者信息表
    /// </summary>
    [Serializable]
    public partial class Sin_Patient : EntityModelBase
    {
        /// <summary>
        /// 姓名    
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别编号
        /// </summary>
        public Sex Sex { get; set; }

        private int? age;
        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age
        {
            get { return age; }
            set
            {
                if (value != null && value != 0)
                    Set(ref age, value);
                else
                    Set(ref age, null);
            }
        }

        /// <summary>
        /// 年龄单位 1-岁 2-月 3-天 4-小时
        /// </summary>
        public AgeUnit Age_unit { get; set; }

        private string medical_num;
        /// <summary>
        /// 病例号
        /// </summary>
        public string Medical_num
        {
            get { return medical_num; }
            set
            {
                if (string.IsNullOrEmpty(value) || Regex.IsMatch(value, @"^[A-Z0-9a-z]*$"))
                    Set(ref medical_num, value);
            }
        }

        /// <summary>
        /// 就诊类别
        /// </summary>
        public string Treatment_type { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }

        /// <summary>
        /// 收费类别
        /// </summary>
        public string Charge_type { get; set; }

        /// <summary>
        /// 病区编号
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 病房
        /// </summary>
        public string Ward { get; set; }

        /// <summary>
        /// 病床
        /// </summary>
        public string Bed { get; set; }

        /// <summary>
        /// 登记日期
        /// </summary>
        public DateTime? Regist_date { get; set; }

        /// <summary>
        /// 登记日期（包含时间）
        /// </summary>
        public DateTime? Regist_datetime { get; set; }

        /// <summary>
        /// 删除标志
        /// </summary>
        public bool? Delete_flag { get; set; }
    }
}
