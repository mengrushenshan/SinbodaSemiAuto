using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels
{
    /// <summary>
    /// 软件界面设置实体
    /// </summary>
    public class SoftWareInterfaceModel
    {
        #region 仪器信息
        /// <summary>
        /// 语言编号
        /// </summary>
        public int LanguageID { get; set; }
        /// <summary>
        /// 公司logo
        /// </summary>
        public string CompanyLogoPath { get; set; }
        /// <summary>
        /// 仪器名称
        /// </summary>
        public string AnalyzerName { get; set; }
        /// <summary>
        /// 仪器型号
        /// </summary>
        public string AnalyzerType { get; set; }
        /// <summary>
        /// 仪器型号名称
        /// </summary>
        public string AnalyzerTypeName { get; set; }
        #endregion

        #region 显示设置
        /// <summary>
        /// 当前语言
        /// </summary>
        public string CurrentLanguage { get; set; }
        /// <summary>
        /// 当前主题
        /// </summary>
        public string CurrentTheme { get; set; }
        /// <summary>
        /// 当前字体大小
        /// </summary>
        public string CurrentFontSize { get; set; }
        #endregion


        #region 备份还原设置
        /// <summary>
        /// 退出时备份
        /// </summary>
        public bool BackupMaintanceByExit { get; set; }
        /// <summary>
        /// 定时备份
        /// </summary>
        public bool BackupMaintanceByTime { get; set; }
        /// <summary>
        /// 定时备份时间
        /// </summary>
        public string BackupTime { get; set; }

        /// <summary>
        /// 备份路径
        /// </summary>
        public string BackupLocation { get; set; }
        /// <summary>
        /// 还原路径
        /// </summary>
        public string ReBackupLocation { get; set; }
        #endregion

        #region 待机注销
        /// <summary>
        /// 是否开启待机注销功能
        /// </summary>
        public bool Logout4StandyEnable { get; set; }
        /// <summary>
        /// 待机注销设置的时间间隔 单位分钟
        /// </summary>
        public int Logout4StandyByTime { get; set; }
        /// <summary>
        /// 待机注销功能显示
        /// </summary>
        public bool LogoutEnableDisplay { get; set; }
        #endregion

        /// <summary>
        /// 是否输出点击跟踪日志
        /// </summary>
        public string PrintSysLog { get; set; }

        public SoftWareInterfaceModel()
        {
            Logout4StandyEnable = false;
            Logout4StandyByTime = 30;
            LogoutEnableDisplay = true;
        }
    }
}
