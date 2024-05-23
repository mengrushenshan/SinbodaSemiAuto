using Sinboda.Framework.Common.DBOperateHelper;
using Sinboda.Framework.Common.FileOperateHelper;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.Interface;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinboda.Framework.Infrastructure;
using System.IO;
using Sinboda.Framework.Common.CommonFunc;
using System.Management;

namespace Sinboda.Framework.Core.StaticResource
{
    /// <summary>
    /// 平台初始化
    /// </summary>
    public class SystemInitialize
    {
        /// <summary>
        /// 初始化资源
        /// </summary>
        public static InitTaskResult InitializeResource()
        {
            // 初始化字典信息
            DataDictionaryService.Instance.InitializeDataDictionary();

            // 系统报警接口创建
            SystemResources.Instance.SysAlarmInstance = new SystemAlarmService();

            // 系统日志接口创建
            SystemResources.Instance.SysLogInstance = new SystemLogService();

            // 当前报警原始信息初始化
            InitializeAlarmOrignalInfo();

            // 初始化软件版本信息
            InitializeSoftWareVersion();
            //初始化CPU id
            SystemResources.Instance.CPUId = GetCpuID();

            return new InitTaskResult();
        }

        /// <summary>
        /// 切换当前语言
        /// </summary>
        /// <param name="languangeCode"></param>
        /// <returns></returns>
        public static bool SetCurrentLanguage(string languangeCode)
        {
            //数据源
            string _DataSource = @"Data Source=";
            //程序路径
            string _Directory = MapPath.AppDir;
            //数据库
            string _FileName = @"Data\\lang.db";
            //db操作帮助类
            IDBHelper iDBHelper = new DBHelper(DBProvider.SQLite);
            string _ConnectString = Path.Combine(_Directory, _FileName);
            iDBHelper.Init(_DataSource + _ConnectString);
            int result = iDBHelper.ExcuteNonQueryInt(string.Format("update setlang set lang='{0}'", languangeCode));
            if (result > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 根据语言ID及语言类型修改词条显示
        /// </summary>
        /// <param name="languageID">语言ID</param>
        /// <param name="languageType">语言类型:CN\EN\FR\DE\IT\PL\PT\RU\ES\TK</param>
        /// <param name="languageValue">语言显示</param>
        /// <returns></returns>
        public static bool UpdateLanguageValue(int languageID, string languageType, string languageValue)
        {
            //数据源
            string _DataSource = @"Data Source=";
            //程序路径
            string _Directory = MapPath.AppDir;
            //数据库
            string _FileName = @"Data\\lang.db";
            //db操作帮助类
            IDBHelper iDBHelper = new DBHelper(DBProvider.SQLite);
            string _ConnectString = Path.Combine(_Directory, _FileName);
            iDBHelper.Init(_DataSource + _ConnectString);
            int result = iDBHelper.ExcuteNonQueryInt(string.Format("update lang set {0}='{1}' where id={2}", languageType, languageValue, languageID));
            if (result > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 当前语言及语言资源初始化
        /// </summary>
        public static void InitializeLanguage()
        {
            //数据源
            string _DataSource = @"Data Source=";
            //程序路径
            string _Directory = MapPath.AppDir;
            //数据库
            string _FileName = @"Data\\lang.db";
            //db操作帮助类
            IDBHelper iDBHelper = new DBHelper(DBProvider.SQLite);
            string _ConnectString = Path.Combine(_Directory, _FileName);
            iDBHelper.Init(_DataSource + _ConnectString);
            object str = iDBHelper.ExcuteQueryObject(string.Format("select lang from setlang"));
            SystemResources.Instance.CurrentLanguage = str.ToString();
            DataTable dt = iDBHelper.ExcuteQueryDataTable(string.Format("select * from lang"));
            int arrayLength = 0;
            if (dt != null)
            {
                if (dt.DefaultView != null && dt.DefaultView.Count > 0)
                {
                    for (int i = 0; i < dt.DefaultView.Count; i++)
                    {
                        int id = StringParseHelper.ParseByDefault(dt.DefaultView[i]["ID"].ToString(), 0);
                        if (id > arrayLength)
                            arrayLength = id;
                    }
                }
            }
            StringResourceExtension.LanguageArray = new string[arrayLength + 100];
            foreach (DataRowView item in dt.DefaultView)
            {
                StringResourceExtension.LanguageArray[StringParseHelper.ParseByDefault(item["ID"].ToString(), 0)] = item[SystemResources.Instance.CurrentLanguage].ToString();
            }
            IPermission iPermission = new PermissionOperation();
            SystemResources.Instance.CurrentPermissionList = iPermission.GetModulePermissionDic("dryf");
        }
        /// <summary>  
        /// 获取CpuID  
        /// </summary>  
        /// <returns>CpuID</returns>  
        private static string GetCpuID()
        {
            try
            {
                string strCpuID = string.Empty;
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    strCpuID = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return strCpuID;
            }
            catch
            {
                return "unknown";
            }
        }
        /// <summary>
        /// 当前用户及用户权限资源初始化
        /// </summary>
        /// <param name="loginUserName"></param>
        public static void InitializePermission(string loginUserName)
        {
            IPermission iPermission = new PermissionOperation();
            //用户初始化
            SystemResources.Instance.CurrentUserName = loginUserName;
            SystemResources.Instance.CurrentRole = iPermission.GetRoleIDByUserName(loginUserName);
            //权限字典初始化（主要为界面权限而使用）
            SystemResources.Instance.CurrentPermissionList = iPermission.GetModulePermissionDic(SystemResources.Instance.CurrentUserName);
        }

        /// <summary>
        /// 当前报警原始信息初始化
        /// </summary>
        public static void InitializeAlarmOrignalInfo()
        {
            SystemResources.Instance.AlarmOrignalInfos = SystemResources.Instance.SysAlarmInstance.GetAlarmInfos();
        }

        /// <summary>
        /// 初始化软件版本信息
        /// </summary>
        public static void InitializeSoftWareVersion()
        {
            SystemResources.Instance.CurrentSoftwareVersion = INIHelper.Read("Version", "CurrentVersion", MapPath.ConfigPath + "Version.ini");
        }
    }
}
