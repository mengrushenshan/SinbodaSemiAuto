using Sinboda.Framework.Business.SystemSetup;
using Sinboda.Framework.Common.FileOperateHelper;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.PeriodTimer;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Core.Services;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;

namespace Sinboda.Framework.View.SystemSetup.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class SoftWareCommonSettingViewModel : NavigationViewModelBase
    {
        SoftWareInterfaceSettingBusiness sBusiness = new SoftWareInterfaceSettingBusiness();
        /// <summary>
        /// 
        /// </summary>
        public SoftWareCommonSettingViewModel()
        {
            if (DesignHelper.IsInDesignMode) return;

            SaveCommand = new RelayCommand(SaveMethod);
            GetBackupCommand = new RelayCommand(GetBackupMethod);
            //GetReBackupCommand = new RelayCommand(GetReBackupMethod);
            ActionBackupCommand = new RelayCommand(ActionBackupMethod);
        }


        public void Init()
        {
            InitIsEnabled();
            InitItemsSource();
            GetCurrentModuleInfoList();
        }

        #region 属性
        #region 仪器信息设置属性
        private string _CompanyLogo;
        /// <summary>
        /// 公司logo
        /// </summary>
        public string CompanyLogo
        {
            get { return _CompanyLogo; }
            set { Set(ref _CompanyLogo, value); }
        }
        private string _AnalyzerType;
        /// <summary>
        /// 型号
        /// </summary>
        public string AnalyzerType
        {
            get { return _AnalyzerType; }
            set { Set(ref _AnalyzerType, value); }
        }
        private string _AnalyzerTypeName;
        /// <summary>
        /// 型号
        /// </summary>
        public string AnalyzerTypeName
        {
            get { return _AnalyzerTypeName; }
            set { Set(ref _AnalyzerTypeName, value); }
        }
        private string _AnalyzerName;
        /// <summary>
        /// 名称
        /// </summary>
        public string AnalyzerName
        {
            get { return _AnalyzerName; }
            set { Set(ref _AnalyzerName, value); }
        }
        private int _AnalyzerNameLanguangeID;
        /// <summary>
        /// 名称对应的语言编号
        /// </summary>
        public int AnalyzerNameLanguangeID
        {
            get { return _AnalyzerNameLanguangeID; }
            set { Set(ref _AnalyzerNameLanguangeID, value); }
        }
        private bool _AnalyzerTypeIsEnabled = false;
        /// <summary>
        /// 型号可用状态
        /// </summary>
        public bool AnalyzerTypeIsEnabled
        {
            get { return _AnalyzerTypeIsEnabled; }
            set { Set(ref _AnalyzerTypeIsEnabled, value); }
        }
        private bool _AnalyzerNameIsEnabled = false;
        /// <summary>
        /// 名称可用状态
        /// </summary>
        public bool AnalyzerNameIsEnabled
        {
            get { return _AnalyzerNameIsEnabled; }
            set { Set(ref _AnalyzerNameIsEnabled, value); }
        }
        OperationResult<SoftWareInterfaceModel> result = new OperationResult<SoftWareInterfaceModel>();
        #endregion

        #region 界面显示设置属性
        private List<DataDictionaryInfoDetail> languagesItemsSource = new List<DataDictionaryInfoDetail>();
        /// <summary>
        /// 语言列表
        /// </summary>
        public List<DataDictionaryInfoDetail> LanguagesItemsSource
        {
            get { return languagesItemsSource; }
            set { Set(ref languagesItemsSource, value); }
        }
        private DataDictionaryInfoDetail selectedLanguage = new DataDictionaryInfoDetail();
        /// <summary>
        /// 选中的语言
        /// </summary>
        public DataDictionaryInfoDetail SelectedLanguage
        {
            get { return selectedLanguage; }
            set { Set(ref selectedLanguage, value); }
        }

        private List<DataDictionaryInfoDetail> themesItemsSource = new List<DataDictionaryInfoDetail>();
        /// <summary>
        /// 主题列表
        /// </summary>
        public List<DataDictionaryInfoDetail> ThemesItemsSource
        {
            get { return themesItemsSource; }
            set { Set(ref themesItemsSource, value); }
        }
        private DataDictionaryInfoDetail selectedTheme = new DataDictionaryInfoDetail();
        /// <summary>
        /// 选中的主题
        /// </summary>
        public DataDictionaryInfoDetail SelectedTheme
        {
            get { return selectedTheme; }
            set { Set(ref selectedTheme, value); }
        }

        private List<DataDictionaryInfoDetail> fontsItemsSource = new List<DataDictionaryInfoDetail>();
        /// <summary>
        /// 字体列表
        /// </summary>
        public List<DataDictionaryInfoDetail> FontsItemsSource
        {
            get { return fontsItemsSource; }
            set { Set(ref fontsItemsSource, value); }
        }
        private DataDictionaryInfoDetail selectedFont = new DataDictionaryInfoDetail();
        /// <summary>
        /// 选中的字体
        /// </summary>
        public DataDictionaryInfoDetail SelectedFont
        {
            get { return selectedFont; }
            set
            {
                Set(ref selectedFont, value);
                if (selectedTheme != null && selectedTheme.Code != null && selectedFont != null && selectedFont.Code != null)
                {
                    ResourceDictionary resourceDictionary = new ResourceDictionary()
                    {
                        Source = new Uri(string.Format("/Sinboda.Theme.{0};component/Themes/Fonts/{1}Font.xaml", selectedTheme.Code, selectedFont.Code), UriKind.RelativeOrAbsolute)
                    };

                    TitleFontSizeValue = double.Parse(resourceDictionary["TitleFontSize"].ToString());
                    ContentFontSizeValue = double.Parse(resourceDictionary["ContentFontSize"].ToString());
                }
            }
        }
        private double _TitleFontSizeValue = 21;
        /// <summary>
        /// 标题字体大小
        /// </summary>
        public double TitleFontSizeValue
        {
            get { return _TitleFontSizeValue; }
            set { Set(ref _TitleFontSizeValue, value); }
        }
        private double _ContentFontSizeValue = 18;
        /// <summary>
        /// 内容字体大小
        /// </summary>
        public double ContentFontSizeValue
        {
            get { return _ContentFontSizeValue; }
            set { Set(ref _ContentFontSizeValue, value); }
        }

        #endregion

        #region 备份还原设置属性
        private bool _BackupMaintanceByExit;
        /// <summary>
        /// 退出时提示备份
        /// </summary>
        public bool BackupMaintanceByExit
        {
            get { return _BackupMaintanceByExit; }
            set { Set(ref _BackupMaintanceByExit, value); }
        }

        private bool _BackupMaintanceByTime;
        /// <summary>
        /// 定时提醒备份
        /// </summary>
        public bool BackupMaintanceByTime
        {
            get { return _BackupMaintanceByTime; }
            set { Set(ref _BackupMaintanceByTime, value); }
        }

        private DateTime _BackupTime;
        /// <summary>
        /// 定时备份时间
        /// </summary>
        public DateTime BackupTime
        {
            get { return _BackupTime; }
            set { Set(ref _BackupTime, value); }
        }

        private string _BackupLocation;
        /// <summary>
        /// 备份路径
        /// </summary>
        public string BackupLocation
        {
            get { return _BackupLocation; }
            set { Set(ref _BackupLocation, value); }
        }

        private string _ReBackupLocation;
        /// <summary>
        /// 还原路径
        /// </summary>
        public string ReBackupLocation
        {
            get { return _ReBackupLocation; }
            set { Set(ref _ReBackupLocation, value); }
        }

        //private bool _AutoBackup;
        ///// <summary>
        ///// 自动备份
        ///// </summary>
        //public bool AutoBackup
        //{
        //    get { return _AutoBackup; }
        //    set { Set(ref _AutoBackup, value); }
        //}

        private string _PrintSysLog;
        public string PrintSysLog
        {
            get { return _PrintSysLog; }
            set { Set(ref _PrintSysLog, value); }
        }
        #endregion

        #region 待机注销
        /// <summary>
        /// 待机注销是否开启
        /// </summary>
        private bool _Logout4StandyEnable;
        public bool Logout4StandyEnable
        {
            get { return _Logout4StandyEnable; }
            set
            {
                Set(ref _Logout4StandyEnable, value);
                if (!_Logout4StandyEnable)
                {
                    if (Logout4StandyByTimeStr == "0")
                        Logout4StandyByTimeStr = "0";
                }
            }
        }

        private string _Logout4StandyByTimeStr;
        public string Logout4StandyByTimeStr
        {
            get { return _Logout4StandyByTimeStr; }
            set { Set(ref _Logout4StandyByTimeStr, value); }
        }
        /// <summary>
        /// 待机功能是否显示
        /// </summary>
        private Visibility logoutVisible;
        public Visibility LogoutVisible
        {
            get { return logoutVisible; }
            set { Set(ref logoutVisible, value); }
        }
        #endregion
        #endregion

        #region  命令

        /// <summary>
        /// 获取备份路径命令
        /// </summary>
        public RelayCommand GetBackupCommand { get; set; }
        /// <summary>
        /// 获取备份路径方法
        /// </summary>
        private void GetBackupMethod()
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.Description = SystemResources.Instance.LanguageArray[6442];//"请选择路径";
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BackupLocation = FBD.SelectedPath;
            }

        }

        public RelayCommand GetReBackupCommand { get; set; }

        private void GetReBackupMethod()
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.Description = SystemResources.Instance.LanguageArray[6442];//"请选择路径";
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ReBackupLocation = FBD.SelectedPath;
            }
        }
        /// <summary>
        /// 执行备份还原命令
        /// </summary>
        public RelayCommand ActionBackupCommand { get; set; }
        /// <summary>
        /// 执行备份还原方法
        /// </summary>
        private void ActionBackupMethod()
        {
            if (MessageBoxResult.OK == NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[6443], MessageBoxButton.OK, SinMessageBoxImage.Warning, false))//如执行还原操作，请先手动关闭上位机软件！
            {
                Process p = new Process();
                ProcessStartInfo process = new ProcessStartInfo(MapPath.AppDir + "DBBackup2000.exe");
                //process.Arguments = "/L " + SystemResources.Instance.CurrentLanguage.ToLower();
                p.StartInfo = process;
                p.Start();
                //p.WaitForExit();
            }
        }

        /// <summary>
        /// 保存命令
        /// </summary>
        public RelayCommand SaveCommand { get; set; }
        /// <summary>
        /// 保存方法
        /// </summary>
        private void SaveMethod()
        {
            InterfaceSaveMethod();
        }
        #endregion

        #region  方法
        /// <summary>
        /// 初始化控件状态
        /// </summary>
        private void InitIsEnabled()
        {
            //if (SystemResources.Instance.CurrentUserName == "dryf")
            //{
            //    AnalyzerTypeIsEnabled = true;
            //    AnalyzerNameIsEnabled = true;
            //}

        }
        /// <summary>
        /// 初始化资源信息
        /// </summary>
        private void InitItemsSource()
        {
            OperationResult<DataDictionaryParentInfo> result = sBusiness.GetShowSettingInfo();
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
            {
                LanguagesItemsSource = result.Results.DataDictionaryInfos.Where(o => o.ParentID == "languageType").FirstOrDefault().DataDictionaryInfoDetails;// DataDictionaryService.Instance.ListTypeAndInfo["languageType"];
                FontsItemsSource = result.Results.DataDictionaryInfos.Where(o => o.ParentID == "fontType").FirstOrDefault().DataDictionaryInfoDetails;// DataDictionaryService.Instance.ListTypeAndInfo["fontType"];
                ThemesItemsSource = result.Results.DataDictionaryInfos.Where(o => o.ParentID == "themeType").FirstOrDefault().DataDictionaryInfoDetails;//DataDictionaryService.Instance.ListTypeAndInfo["themeType"];
            }
            else
            { }
        }

        private void InterfaceSaveMethod()
        {
            if (ValidationCurrentModuleInfo())
            {
                SoftWareInterfaceModel model = new SoftWareInterfaceModel();
                model.CompanyLogoPath = CompanyLogo;
                model.AnalyzerName = AnalyzerName;
                model.AnalyzerType = AnalyzerType;
                model.AnalyzerTypeName = AnalyzerTypeName;
                model.LanguageID = AnalyzerNameLanguangeID;

                model.CurrentLanguage = SelectedLanguage.Code;
                model.CurrentFontSize = SelectedFont.Code;
                model.CurrentTheme = selectedTheme.Code;

                model.BackupMaintanceByExit = BackupMaintanceByExit;
                model.BackupMaintanceByTime = BackupMaintanceByTime;
                model.BackupTime = BackupTime.ToDbString("HH:mm");
                model.BackupLocation = BackupLocation;
                model.ReBackupLocation = ReBackupLocation;
                model.PrintSysLog = PrintSysLog;

                //model.Logout4StandyByTime = Logout4StandyByTime;
                model.Logout4StandyByTime = string.IsNullOrEmpty(Logout4StandyByTimeStr) ? 0 : Convert.ToInt32(Logout4StandyByTimeStr);
                model.Logout4StandyEnable = Logout4StandyEnable;
                model.LogoutEnableDisplay = SystemResources.Instance.softModel.LogoutEnableDisplay;
                //model.AutoBackup = AutoBackup;

                OperationResult result1 = sBusiness.SetSettingInfo(model);
                if (result1.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    INIHelper.Write("DatabaseBackup", "Path", model.BackupLocation, MapPath.AppDir + "DBConfig.ini");

                    SystemResources.Instance.BackupMaintanceByTime = SystemResources.Instance.softModel.BackupMaintanceByTime = model.BackupMaintanceByTime;
                    SystemResources.Instance.BackupTime = SystemResources.Instance.softModel.BackupTime = model.BackupTime;
                    SystemResources.Instance.Logout4StandyByTime = SystemResources.Instance.softModel.Logout4StandyByTime = model.Logout4StandyByTime;
                    SystemResources.Instance.Logout4StandyEnable = SystemResources.Instance.softModel.Logout4StandyEnable = model.Logout4StandyEnable;

                    if (SystemResources.Instance.Logout4StandyEnable)
                    {
                        PeriodManager.Instance.StartLogoutPeroidTimer();
                    }
                    else
                    {
                        PeriodManager.Instance.StopLogoutPeriodTimer();
                    }

                    SystemInitialize.SetCurrentLanguage(model.CurrentLanguage);
                    SystemInitialize.InitializeLanguage();
                    StyleResourceManager.SetTheme(model.CurrentTheme, model.CurrentFontSize);
                    WriteOperateLog(string.Format("{0} : {1}", SystemResources.Instance.LanguageArray[4345], SystemResources.Instance.LanguageArray[609]));//29提示609保存成功 
                    if (oldLanguage != SelectedLanguage.Code)
                    {
                        GlobalClass.ChangeLanguage();
                        InitItemsSource();
                        GetCurrentModuleInfoList();
                        SystemInitialize.InitializeResource();
                        ShowMessageComplete(SystemResources.Instance.LanguageArray[6444]);// "保存成功，切换语言需要重启后生效！"); 
                    }
                    else
                        ShowMessageComplete(SystemResources.Instance.LanguageArray[368]); //"保存成功！"); 
                }
                else
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[610]);
                }
            }
        }

        /// <summary>
        /// 校验模块设置
        /// </summary>
        /// <returns></returns>
        private bool ValidationCurrentModuleInfo()
        {
            if (string.IsNullOrEmpty(AnalyzerName))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6445]); //"名称不能为空！
                return false;
            }
            if (string.IsNullOrEmpty(BackupLocation))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6446]); //"备份路径不能为空！
                return false;
            }
            //if (string.IsNullOrEmpty(ReBackupLocation))
            //{
            //    ShowMessageWarning("还原路径不能为空！"); //TODO 翻译
            //    return false;
            //}
            if (BackupMaintanceByTime && string.IsNullOrEmpty(BackupTime.ToDbString()))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6447]); //"定时时间不能为空！
                return false;
            }

            if (Logout4StandyEnable && string.IsNullOrEmpty(Logout4StandyByTimeStr))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[225]); //"待机注销时间不能为空！
                return false;
            }

            if (Logout4StandyEnable && Logout4StandyByTimeStr == "0")
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[227]); //"待机注销时间不能为0！
                return false;
            }

            return true;
        }


        private string oldLanguage = string.Empty;
        /// <summary>
        /// 获取信息
        /// </summary>
        private void GetCurrentModuleInfoList()
        {
            oldLanguage = SystemResources.Instance.CurrentLanguage;
            result = sBusiness.GetSettingInfo();// aBusiness.GetAnalyzerInfo();
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
            {
                if (result.Results != null)
                {
                    SoftWareInterfaceModel model = result.Results;
                    SelectedLanguage = LanguagesItemsSource.FirstOrDefault(o => o.Code == SystemResources.Instance.CurrentLanguage);
                    SelectedTheme = ThemesItemsSource.FirstOrDefault(o => o.Code == model.CurrentTheme);
                    SelectedFont = FontsItemsSource.FirstOrDefault(o => o.Code == model.CurrentFontSize);

                    AnalyzerName = model.LanguageID != 0 ? SystemResources.Instance.LanguageArray[model.LanguageID] : model.AnalyzerName;
                    AnalyzerType = model.AnalyzerType;
                    AnalyzerTypeName = model.AnalyzerTypeName;
                    CompanyLogo = model.CompanyLogoPath;
                    AnalyzerNameLanguangeID = model.LanguageID;

                    BackupMaintanceByExit = model.BackupMaintanceByExit;
                    BackupMaintanceByTime = model.BackupMaintanceByTime;
                    BackupTime = model.BackupTime.ToDateTime();
                    BackupLocation = INIHelper.Read("DatabaseBackup", "Path", MapPath.AppDir + "DBConfig.ini");  //model.BackupLocation;
                    ReBackupLocation = model.ReBackupLocation;

                    Logout4StandyEnable = model.Logout4StandyEnable;
                    //Logout4StandyByTime = model.Logout4StandyByTime;
                    Logout4StandyByTimeStr = model.Logout4StandyByTime == 0 ? string.Empty : model.Logout4StandyByTime.ToString();
                    LogoutVisible = model.LogoutEnableDisplay ? Visibility.Visible : Visibility.Collapsed;

                    //AutoBackup = model.AutoBackup;
                    PrintSysLog = model.PrintSysLog;
                }
                else
                {
                    SelectedLanguage = LanguagesItemsSource.FirstOrDefault();
                    SelectedTheme = ThemesItemsSource.FirstOrDefault();
                    SelectedFont = FontsItemsSource.FirstOrDefault();

                    AnalyzerName = string.Empty;
                    AnalyzerType = "0";
                    AnalyzerTypeName = string.Empty;
                    CompanyLogo = string.Empty;
                    AnalyzerNameLanguangeID = 0;

                    BackupMaintanceByExit = false;
                    BackupMaintanceByTime = false;
                    BackupTime = DateTime.MinValue;
                    BackupLocation = string.Empty;
                    ReBackupLocation = string.Empty;

                    Logout4StandyEnable = false;
                    //Logout4StandyByTime = int.MaxValue;
                    Logout4StandyByTimeStr = string.Empty;
                    //AutoBackup = false;
                    PrintSysLog = "true";
                }
            }
            else
                ShowMessageError(result.Message);
        }
        #endregion
    }
}
