using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sinboda.Framework.Core.CommonModels
{
    /// <summary>
    /// 软件名称信息
    /// </summary>
    public class CurrentModuleInfo
    {
        /// <summary>
        /// 语言编号
        /// </summary>
        public int LanguageID { get; set; }
        /// <summary>
        /// 公司logo
        /// </summary>
        public string CompanyLogoPath { get; set; }
        /// <summary>
        /// 公司logo
        /// </summary>
        public BitmapImage CompanyLogo { get; set; }
        /// <summary>
        /// 仪器名称
        /// </summary>
        public string AnalyzerName { get; set; }
        /// <summary>
        /// 仪器型号
        /// </summary>
        public string AnalyzerType { get; set; }
    }
}
