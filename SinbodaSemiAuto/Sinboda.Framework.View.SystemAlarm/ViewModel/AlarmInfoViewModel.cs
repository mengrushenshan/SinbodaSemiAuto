using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Business.SystemAlarm;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Control.Controls.Navigation;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.View.SystemAlarm.Win;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.Framework.View.SystemAlarm.ViewModel
{
    /// <summary>
    /// 报警信息业务实现类
    /// </summary>
    class AlarmInfoViewModel : NavigationViewModelBase
    {
        /// <summary>
        /// 业务逻辑层中对数据增、删、改、查操作
        /// </summary>
        private AlarmHistoryInfoBusiness operation = new AlarmHistoryInfoBusiness();
        //private AlarmInfoSelectRow alarmInfoSelectRow;
        /// <summary>
        /// ViewModel 构造函数
        /// </summary>
        public AlarmInfoViewModel()
        {
            //alarmInfoSelectRow = new AlarmInfoSelectRow();
            if (DataDictionaryService.Instance.GetIsMultiModuleMode)
            {
                ModuleInfoVisibility = Visibility.Visible;
                ModuleInfoVisible = true;
                ModuleInfoSource.Add(new ModuleInfoModel() { ModuleID = 0, ModuleName = SystemResources.Instance.LanguageArray[1719] });
                foreach (var item in DataDictionaryService.Instance.ModuleTypeAndInfo)
                {
                    foreach (var subItem in item.Value)
                        if (subItem.IsShow)
                            ModuleInfoSource.Add(subItem);
                }
                SelectedModuleInfo = ModuleInfoSource.FirstOrDefault();
            }
            else
            {
                ModuleInfoSource = new List<ModuleInfoModel>();
                SelectedModuleInfo = new ModuleInfoModel();
                ModuleInfoVisibility = Visibility.Collapsed;
                ModuleInfoVisible = false;
            }
            IntiLookUpEdit();

            AlarmSettingCommand = new RelayCommand(AlarmSettingMethod);
            AlarmHistoryInfoCommand = new RelayCommand(AlarmHistoryInfoMethod);
            //AlarmRefreshCommand = new RelayCommand(AlarmRefreshMethod);
            AlarmDeleteOneCommand = new RelayCommand(AlarmDeleteOneMethod);
            AlarmClearAllCommand = new RelayCommand(AlarmClearAllMethod);

            Messenger.Default.Register<string>(this, "AlarmRefreshByEvent", AlarmRefreshByEvent);
        }

        public void LoadedPage()
        {
            AlarmRefreshMethod();
        }

        public void UnLoadedPage()
        {
        }
        /// <summary>
        /// 自动刷新报警信息
        /// </summary>
        /// <param name="msg"></param>
        private void AlarmRefreshByEvent(string msg)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AlarmRefreshMethod();
            });
        }

        #region 属性
        private List<SystemTypeValueModel> lookUpEditSourceType = new List<SystemTypeValueModel>();
        /// <summary>
        /// 报警类型集合：全部、数据报警、故障报警
        /// </summary>
        public List<SystemTypeValueModel> LookUpEditSourceType
        {
            get { return lookUpEditSourceType; }
            set { Set(ref lookUpEditSourceType, value); }
        }
        /// <summary>
        /// 查看报警类型数据列表选中项索引
        /// </summary>
        private SystemTypeValueModel selectAlarmsTypeIndex = new SystemTypeValueModel();
        /// <summary>
        /// 查看报警类型数据列表选中项索引
        /// </summary>
        public SystemTypeValueModel SelectAlarmsTypeIndex
        {
            get { return selectAlarmsTypeIndex; }
            set
            {
                Set(ref selectAlarmsTypeIndex, value);
                OnSelectedInfoChanged();
            }
        }
        /// <summary>
        ///报警级别集合：注意（数据/故障）、加样停止、停止 
        /// </summary>
        private List<SystemTypeValueModel> lookUpEditSourcelevel = new List<SystemTypeValueModel>();
        /// <summary>
        ///报警级别集合：注意（数据/故障）、加样停止、停止 
        /// </summary>
        public List<SystemTypeValueModel> LookUpEditSourceLevel
        {
            get { return lookUpEditSourcelevel; }
            set { Set(ref lookUpEditSourcelevel, value); }
        }
        /// <summary>
        /// 查看报警类型数据列表选中项索引
        /// </summary>
        private SystemTypeValueModel selectAlarmsLevelIndex = new SystemTypeValueModel();
        /// <summary>
        /// 查看报警类型数据列表选中项索引
        /// </summary>
        public SystemTypeValueModel SelectAlarmsLevelIndex
        {
            get { return selectAlarmsLevelIndex; }
            set
            {
                Set(ref selectAlarmsLevelIndex, value);
                OnSelectedInfoChanged();
            }
        }

        private List<ModuleInfoModel> moduleInfoSource = new List<ModuleInfoModel>();
        /// <summary>
        /// 模块下拉列表集合
        /// </summary>
        public List<ModuleInfoModel> ModuleInfoSource
        {
            get { return moduleInfoSource; }
            set { Set(ref moduleInfoSource, value); }
        }

        private ModuleInfoModel selectedModuleInfo = new ModuleInfoModel();
        /// <summary>
        /// 模块下拉列表选中项
        /// </summary>
        public ModuleInfoModel SelectedModuleInfo
        {
            get { return selectedModuleInfo; }
            set
            {
                Set(ref selectedModuleInfo, value);
                OnSelectedInfoChanged();
            }
        }

        private Visibility moduleInfoVisibility;
        /// <summary>
        /// 模块下拉列表显示与否
        /// </summary>
        public Visibility ModuleInfoVisibility
        {
            get { return moduleInfoVisibility; }
            set { Set(ref moduleInfoVisibility, value); }
        }

        private bool moduleInfoVisible;
        /// <summary>
        /// 列表中模块显示与否
        /// </summary>
        public bool ModuleInfoVisible
        {
            get { return moduleInfoVisible; }
            set { Set(ref moduleInfoVisible, value); }
        }

        /// <summary>
        /// 报警当天历史数据集
        /// </summary>
        private List<AlarmHistoryInfoModel> _HistoryInfos = new List<AlarmHistoryInfoModel>();
        /// <summary>
        /// 报警当天历史数据集
        /// </summary>
        public List<AlarmHistoryInfoModel> HistoryInfos
        {
            get { return _HistoryInfos; }
            set { Set(ref _HistoryInfos, value); }
        }
        /// <summary>
        /// GridCtrl关联的数据表数据
        /// </summary> 
        private List<AlarmHistoryInfoModel> _GridCtrlSource = new List<AlarmHistoryInfoModel>();
        /// <summary>
        /// GridCtrl关联的数据表数据
        /// </summary> 
        public List<AlarmHistoryInfoModel> GridCtrlSource
        {
            get { return _GridCtrlSource; }
            set { Set(ref _GridCtrlSource, value); }
        }

        int moduleType;
        string code;
        /// <summary>
        /// 选中行
        /// </summary>
        private AlarmHistoryInfoModel selectRow;
        /// <summary>
        /// 选中行
        /// </summary>
        public AlarmHistoryInfoModel SelectRow
        {
            get { return selectRow; }
            set
            {
                Set(ref selectRow, value);
                AlarmOrignalInfoModel model = new AlarmOrignalInfoModel();
                if (selectRow != null)
                {
                    moduleType = selectRow.ModuleType;
                    code = SelectRow.Code;
                    model = SystemResources.Instance.AlarmOrignalInfos.FirstOrDefault(o => o.ModuleType == SelectRow.ModuleType && o.Code == SelectRow.Code);
                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(SelectRow.Parameters) && SelectRow.Parameters.Contains("{") && SelectRow.Parameters.Contains("}"))
                            DetailInfo = SystemAlarmModelOperations.Instance.SetParamterInfo(model.DetailInfo, SelectRow.Parameters);
                        else
                            DetailInfo = model.DetailInfo;
                        Solution = model.Solution;
                    }
                    else
                    {
                        if (SelectRow.Info == StringResourceExtension.GetLanguage(138, "厂家保留报警信息"))
                        {
                            DetailInfo = StringResourceExtension.GetLanguage(139, "请联系厂家售后工程师");
                            Solution = StringResourceExtension.GetLanguage(140, "请联系厂家售后工程师");
                        }
                        else
                        {
                            DetailInfo = "";
                            Solution = "";
                        }
                    }
                }
                else
                {
                    moduleType = 0;
                    code = string.Empty;
                    model = null;
                    DetailInfo = "";
                    Solution = "";
                }
            }
        }
        private string _DetailInfo;
        /// <summary>
        /// 详细信息
        /// </summary>
        public string DetailInfo
        {
            get { return _DetailInfo; }
            set { Set(ref _DetailInfo, value); }
        }
        private string _Solution;
        /// <summary>
        /// 解决办法
        /// </summary>
        public string Solution
        {
            get { return _Solution; }
            set { Set(ref _Solution, value); }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 绑定 报警历史信息 按钮Command属性
        /// </summary>
        public RelayCommand AlarmHistoryInfoCommand { get; set; }

        /// <summary>
        /// 报警历史窗体show方法
        /// </summary>
        private void AlarmHistoryInfoMethod()
        {
            AlarmsHistoryInfoWin histroyFrm = new AlarmsHistoryInfoWin();
            histroyFrm.ShowDialog();
        }

        /// <summary>
        /// 报警设置按钮命令绑定
        /// </summary>
        public RelayCommand AlarmSettingCommand { get; set; }

        /// <summary>
        /// 报警设置窗体显示方法
        /// </summary>
        private void AlarmSettingMethod()
        {
            AlarmsSettingWin SettingFrm = new AlarmsSettingWin();
            SettingFrm.ShowDialog();
        }
        /// <summary>
        /// 报警删除按钮命令绑定
        /// </summary>
        public RelayCommand AlarmDeleteOneCommand { get; set; }
        /// <summary>
        /// 删除选中的报警信息方法----删除 不删数据库信息
        /// </summary>
        private void AlarmDeleteOneMethod()
        {
            if (selectRow != null)
            {
                OperationResult result = operation.DeleteOneAlarmHistoryInfo(selectRow.Id);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    WriteOperateLog(string.Format("{0}：{1}", SystemResources.Instance.LanguageArray[2176], SystemResources.Instance.LanguageArray[228] + SystemResources.Instance.LanguageArray[1118]));//29提示609清除成功 //TODO 翻译
                }
                moduleType = 0;
                code = string.Empty;
                SelectRow = null;
                AlarmRefreshMethod();

                /* 尿分清除报警时，通知屏幕使用 */
                Messenger.Default.Send<object>(selectRow, "NotifyClearAlarm");
            }
        }
        /// <summary>
        /// 报警全部删除按钮命令绑定
        /// </summary>
        public RelayCommand AlarmClearAllCommand { get; set; }
        /// <summary>
        /// 清除所有报警信息方法- ----删除 不删数据库信息
        /// </summary>
        public void AlarmClearAllMethod()
        {
            OperationResult result = operation.DeleteAllShowInfos();
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
            {
                WriteOperateLog(string.Format("{0}：{1}", SystemResources.Instance.LanguageArray[2176], SystemResources.Instance.LanguageArray[3182] + SystemResources.Instance.LanguageArray[1118]));//29提示609清除成功 //TODO 翻译
            }
            moduleType = 0;
            code = string.Empty;
            SelectRow = null;
            AlarmRefreshMethod();

            Messenger.Default.Send<string>("ClearAllAlarmFromPlatFormByEvent", "ClearAllAlarmFromPlatFormByEvent");
            /* 尿分清除报警时，通知屏幕使用 */
            Messenger.Default.Send<object>(null, "NotifyClearAlarm");
        }
        /// <summary>
        /// 刷新报警信息列表按钮命令绑定
        /// </summary>
        public RelayCommand AlarmRefreshCommand { get; set; }
        /// <summary>
        /// 刷新报警信息列表方法
        /// </summary>
        public void AlarmRefreshMethod()
        {
            GetInfs();
            OnSelectedInfoChanged();

            var s = NavigationServiceExBase.CurrentService.Current as JournalEntry;
            var item = s.Source as NavigationItem;
            if (item.Id == "AlarmsInfoPageView")
                SystemResources.Instance.AnalyzerAlarmLevel = AlarmLevel.None;
        }
        #endregion

        #region 其他方法
        /// <summary>
        /// 初始化下拉列框：报警级别、报警类型
        /// </summary>
        private void IntiLookUpEdit()
        {
            GetAlarmTypeInfo();
            GetAlarmLevelInfo();
        }

        /// <summary>
        /// 获取当天报警历史信息原始数据
        /// </summary>
        /// <returns></returns>
        private void GetInfs()
        {
            OperationResult<List<AlarmHistoryInfoModel>> result = operation.GetAlarmHitoryInfosToday(DateTime.Now.Date, DateTime.Now.Date);
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
                HistoryInfos = result.Results;
        }
        /// <summary>
        /// 对外获取当天报警历史信息原始数据
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<AlarmHistoryInfoModel>> GetAlarmHitoryInfosToday()
        {
            OperationResult<List<AlarmHistoryInfoModel>> result = operation.GetAlarmHitoryInfosToday(DateTime.Now.Date, DateTime.Now.Date);
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
                HistoryInfos = result.Results;
            return result;
        }
        /// <summary>
        /// 获取报警类型
        /// </summary>
        private void GetAlarmTypeInfo()
        {
            DataDictionaryService.Instance.InitializeSystemTypeDictionary();
            LookUpEditSourceType = DataDictionaryService.Instance.SystemTypeValueDictionary["alarmStyle"];
            SelectAlarmsTypeIndex = LookUpEditSourceType.FirstOrDefault();
        }
        /// <summary>
        /// 获取报警级别
        /// </summary>
        private void GetAlarmLevelInfo()
        {
            DataDictionaryService.Instance.InitializeSystemTypeDictionary();
            LookUpEditSourceLevel = DataDictionaryService.Instance.SystemTypeValueDictionary["alarmLevel"];
            SelectAlarmsLevelIndex = LookUpEditSourceLevel.FirstOrDefault();
        }
        /// <summary>
        /// 切换过滤条件
        /// </summary>
        private void OnSelectedInfoChanged()
        {
            if (SelectAlarmsTypeIndex == null || SelectAlarmsLevelIndex == null || SelectedModuleInfo == null) return;

            if (SelectedModuleInfo.ModuleID == 0)
            {
                if (SelectAlarmsTypeIndex.Code != 0 && SelectAlarmsLevelIndex.Code != 0)
                {
                    GridCtrlSource = HistoryInfos.Where(p => p.AlarmStyle == (AlarmStyleEnum)SelectAlarmsTypeIndex.Code && p.CodeLevel == (AlarmLevelEnum)SelectAlarmsLevelIndex.Code).ToList();
                    SelectRowShow();
                }
                else if (SelectAlarmsTypeIndex.Code != 0 && SelectAlarmsLevelIndex.Code == 0)
                {
                    GridCtrlSource = HistoryInfos.Where(p => p.AlarmStyle == (AlarmStyleEnum)SelectAlarmsTypeIndex.Code).ToList();
                    SelectRowShow();
                }
                else if (SelectAlarmsTypeIndex.Code == 0 && SelectAlarmsLevelIndex.Code != 0)
                {
                    GridCtrlSource = HistoryInfos.Where(p => p.CodeLevel == (AlarmLevelEnum)SelectAlarmsLevelIndex.Code).ToList();
                    SelectRowShow();
                }
                else
                {
                    GridCtrlSource = HistoryInfos;
                    SelectRowShow();
                }
            }
            else
            {
                if (SelectAlarmsTypeIndex.Code != 0 && SelectAlarmsLevelIndex.Code != 0)
                {
                    GridCtrlSource = HistoryInfos.Where(p => p.AlarmStyle == (AlarmStyleEnum)SelectAlarmsTypeIndex.Code && p.CodeLevel == (AlarmLevelEnum)SelectAlarmsLevelIndex.Code && p.ModuleID == SelectedModuleInfo.ModuleID).ToList();
                    SelectRowShow();
                }
                else if (SelectAlarmsTypeIndex.Code != 0 && SelectAlarmsLevelIndex.Code == 0)
                {
                    GridCtrlSource = HistoryInfos.Where(p => p.AlarmStyle == (AlarmStyleEnum)SelectAlarmsTypeIndex.Code && p.ModuleID == SelectedModuleInfo.ModuleID).ToList();
                    SelectRowShow();
                }
                else if (SelectAlarmsTypeIndex.Code == 0 && SelectAlarmsLevelIndex.Code != 0)
                {
                    GridCtrlSource = HistoryInfos.Where(p => p.CodeLevel == (AlarmLevelEnum)SelectAlarmsLevelIndex.Code && p.ModuleID == SelectedModuleInfo.ModuleID).ToList();
                    SelectRowShow();
                }
                else
                {
                    GridCtrlSource = HistoryInfos.Where(p => p.ModuleID == SelectedModuleInfo.ModuleID).ToList();
                    SelectRowShow();
                }
            }
        }
        /// <summary>
        /// 显示选中数据
        /// </summary>
        private void SelectRowShow()
        {
            if (moduleType != 0 && code != string.Empty)
            {
                SelectRow = GridCtrlSource.FirstOrDefault(o => o.ModuleType == moduleType && o.Code == code);
            }
            else
            {
                SelectRow = null;
            }
        }
        #endregion
    }
}
