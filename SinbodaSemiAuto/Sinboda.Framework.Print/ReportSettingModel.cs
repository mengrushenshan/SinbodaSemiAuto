using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Print
{
    /// <summary>
    /// 定义报表格式实体（参考）
    /// </summary>
    public class ReportSettingModel
    {
        /// <summary>
        /// 标题头1
        /// </summary>
        public string FirstName { set; get; }

        /// <summary>
        /// 标题头2
        /// </summary>
        public string SecondName { set; get; }

        /// <summary>
        /// 是否使用结尾
        /// </summary>
        public string UseEndnotes { set; get; }

        /// <summary>
        /// 结尾1
        /// </summary>
        public string Endnotes1 { set; get; }

        /// <summary>
        /// 结尾2
        /// </summary>
        public string Endnotes2 { set; get; }

        /// <summary>
        /// 自动打印
        /// </summary>
        public string AutoPrint { set; get; }

        /// <summary>
        /// 打印审核的
        /// </summary>
        public string PrintAudit { set; get; }

        /// <summary>
        /// 打印带有患者姓名的
        /// </summary>
        public string PrintPatient { set; get; }

        /// <summary>
        /// 结果标志 
        /// </summary>
        public string ResultFlag { set; get; }

        /// <summary>
        /// 结果上限
        /// </summary>
        public string HighFlag { set; get; }

        /// <summary>
        /// 结果下限
        /// </summary>
        public string LowFlag { set; get; }
    }
}
