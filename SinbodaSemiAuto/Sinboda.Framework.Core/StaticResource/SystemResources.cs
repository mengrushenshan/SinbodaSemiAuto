using GalaSoft.MvvmLight;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.Interface;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Sinboda.Framework.Core.StaticResource
{
    /// <summary>
    /// 系统资源
    /// </summary>
    public class SystemResources : ObservableObject
    {
        #region 单例

        private static readonly SystemResources instance = new SystemResources();
        /// <summary>
        /// 静态实例
        /// </summary>
        public static SystemResources Instance
        {
            get { return instance; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SystemResources()
        {
            if (DesignHelper.IsInDesignMode)
                return;
        }
        #endregion

        #region 私有变量

        private bool analyzerConnectionState;   // 仪器连接状态
        private bool lisConnectionState;        // LIS 连接状态
        private bool printerConnectionState;    // 打印机状态
        private AlarmLevel alarmLevel;          // 报警级别
        private string currentUserName = "dr";         // 用户名
        private string currentRole;             // 用户角色
        private string currentSoftwareVersion;  // 软件版本
        private string currentState;            // 当前状态
        private string cpuId = string.Empty;         // 电脑CPUId
        //private StringResourceExtension
        #endregion

        #region 系统开放属性

        /// <summary>
        /// 注册密钥
        /// </summary>
        public string Secretkey { get; set; } = string.Empty;

        /// <summary>
        /// 获取仪器编码(‘;’分割)
        /// </summary>
        public string AnalyzerCode
        {
            get
            {
                if (AnalyzerIdCode.Count <= 0)
                    return string.Empty;

                var er = AnalyzerIdCode.Values.GetEnumerator();
                er.MoveNext();
                string code = er.Current;
                while (er.MoveNext())
                {
                    code += $";{er.Current}";
                }

                return code;
            }
        }

        /// <summary>
        /// 仪器编码数据
        /// </summary>
        public Dictionary<int, string> AnalyzerIdCode
        {
            get;
            private set;
        } = new Dictionary<int, string>();

        /// <summary>
        /// 公司logo
        /// </summary>
        public BitmapImage AnalyzerInfoLogo { get; set; }
        /// <summary>
        /// 仪器名称
        /// </summary>
        public string AnalyzerInfoName { get; set; }
        /// <summary>
        /// 仪器型号
        /// </summary>
        public string AnalyzerInfoType { get; set; }
        /// <summary>
        /// 仪器型号名称
        /// </summary>
        public string AnalyzerInfoTypeName { get; set; }

        /// <summary>
        /// 是否依照时间进行自动备份
        /// </summary>
        public bool BackupMaintanceByTime { get; set; }

        /// <summary>
        /// 自动备份时间
        /// </summary>
        public string BackupTime { get; set; }

        /// <summary>
        /// 是否开启待机注销功能
        /// </summary>
        public bool Logout4StandyEnable { get; set; }
        /// <summary>
        /// 待机注销设置的时间间隔 单位分钟
        /// </summary>
        public int Logout4StandyByTime { get; set; }

        /// <summary>
        /// 基础设置界面设置信息
        /// </summary>
        public SoftWareInterfaceModel softModel = new SoftWareInterfaceModel();

        /// <summary>
        /// 是否自动打印
        /// </summary>
        public bool IsAutoPrint { get; set; }

        /// <summary>
        /// 是否自动打印时需要样本已审核
        /// </summary>
        public bool IsAutoPrintByAudit { get; set; }

        /// <summary>
        /// 是否自动打印时需要带有患者姓名
        /// </summary>
        public bool IsAutoPrintByName { get; set; }

        /// <summary>
        /// 打印模版使用的阳性标识
        /// </summary>
        public string PrintTemplateHighMark { get; set; }
        /// <summary>
        /// 打印模版使用的阴性标识
        /// </summary>
        public string PrintTemplateLowMark { get; set; }

        /// <summary>
        /// 仪器报警级别
        /// </summary>
        public AlarmLevel AnalyzerAlarmLevel
        {
            get { return alarmLevel; }
            set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Set("AnalyzerAlarmLevel", ref alarmLevel, value);
                });
            }
        }

        /// <summary>
        /// 仪器连接状态
        /// </summary>
        public bool AnalyzerConnectionState
        {
            get { return analyzerConnectionState; }
            set { Set("AnalyzerConnectionState", ref analyzerConnectionState, value); }
        }
        /// <summary>
        /// LIS连接状态
        /// </summary>
        public bool LISConnectionState
        {
            get { return lisConnectionState; }
            set { Set("LISConnectionState", ref lisConnectionState, value); }
        }
        /// <summary>
        /// 软件状态
        /// </summary>
        public string CurrentState
        {
            get { return currentState; }
            set { Set(nameof(CurrentState), ref currentState, value); }
        }

        /// <summary>
        /// 打印机连接状态
        /// </summary>
        public bool PrinterConnectionState
        {
            get { return printerConnectionState; }
            set { Set("PrinterConnectionState", ref printerConnectionState, value); }
        }

        /// <summary>
        /// 当前语言
        /// </summary>
        public string CurrentLanguage { get; internal set; }
        /// <summary>
        /// 语言
        /// </summary>
        public string[] LanguageArray
        {
            get { return StringResourceExtension.LanguageArray; }
            //internal set { }
        }
        /// <summary>
        /// 当前登录用户
        /// </summary>
        public string CurrentUserName
        {
            get { return currentUserName; }
            set { Set("CurrentUserName", ref currentUserName, value); }
        }
        /// <summary>
        /// 当前登录用户角色
        /// </summary>
        public string CurrentRole
        {
            get { return currentRole; }
            set { Set("CurrentRole", ref currentRole, value); }
        }
        /// <summary>
        /// 当前软件版本
        /// </summary>
        public string CurrentSoftwareVersion
        {
            get { return currentSoftwareVersion; }
            set { Set("CurrentSoftwareVersion", ref currentSoftwareVersion, value); }
        }
        /// <summary>
        /// CPUId
        /// </summary>
        public string CPUId
        {
            get { return cpuId; }
            set { Set("CPUId", ref cpuId, value); }
        }
        #endregion

        #region 系统开放接口
        /// <summary>
        /// 当前登录用户权限表
        /// </summary>
        public Dictionary<string, bool> CurrentPermissionList { get; internal set; }
        /// <summary>
        /// 报警信息原始表
        /// </summary>
        public List<AlarmOrignalInfoModel> AlarmOrignalInfos { get; internal set; }
        /// <summary>
        /// 系统日志接口
        /// </summary>
        public ISystemLogService SysLogInstance { get; internal set; }
        /// <summary>
        /// 系统报警接口
        /// </summary>
        public ISystemAlarmService SysAlarmInstance { get; internal set; }
        #endregion

        /// <summary>
        /// 获取枚举集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public List<SystemTypeValue<T>> GetSystemTypeValueEnum<T>(string typeName) where T : struct
        {
            return DataDictionaryService.Instance.GetSystemTypeValueEnum<T>(typeName);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public List<SysUserModel> GetUserListByRoleID(string roleID)
        {
            IPermission iPermission = new PermissionOperation();
            return iPermission.GetUsersByRoleID(roleID);
        }

        /// <summary>
        /// 获取系统词条
        /// </summary>
        /// <param name="lid">语言ID</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="args">词条参数</param>
        /// <returns></returns>
        public string GetLanguage(int lid, string defaultValue = "", params object[] args)
        {
            try
            {
                string lvalue = string.Empty;
                if (lid >= 0 && lid < StringResourceExtension.LanguageArray.Length)
                {
                    lvalue = StringResourceExtension.LanguageArray[lid];
                }

                string value = string.IsNullOrEmpty(lvalue) ? defaultValue : lvalue;
                if (args == null || args.Length == 0)
                    return value;
                else
                    return string.Format(value, args);
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Debug($"获取系统词条异常：语言编号={lid}", ex);
                return defaultValue;
            }
        }

        /// <summary>
        /// 添加仪器ID
        /// </summary>
        /// <param name="mid"></param>
        /// <param name="aid"></param>
        public void AddAnalyzerCode(int mid, string aid)
        {
            string value;
            if (!AnalyzerIdCode.TryGetValue(mid, out value))
            {
                AnalyzerIdCode.Add(mid, aid);
            }
            else
            {
                AnalyzerIdCode[mid] = aid;
            }
        }

        /// <summary>
        /// 设定仪器编码
        /// </summary>
        /// <param name="codes"></param>
        public void SetAnalyzerCode(string codes)
        {
            AnalyzerIdCode.Clear();
            var arr = codes.Split(';');
            if (arr.Length <= 0)
                return;

            for (int i = 0; i < arr.Length; i++)
            {
                AddAnalyzerCode(i, arr[i]);
            }
        }
    }
}
