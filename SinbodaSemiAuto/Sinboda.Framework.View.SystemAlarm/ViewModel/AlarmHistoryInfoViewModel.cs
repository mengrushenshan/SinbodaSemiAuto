using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Business.SystemAlarm;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Control.DataPage;
using Sinboda.Framework.Control.Loading;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.Framework.View.SystemAlarm.ViewModel
{
    /// <summary>
    /// 报警历史信息业务实现类
    /// </summary>
    public class AlarmHistoryInfoViewModel : NavigationViewModelBase
    {
        /// <summary>
        /// 报警历史信息处理
        /// </summary>
        private AlarmHistoryInfoBusiness opera = new AlarmHistoryInfoBusiness();
        /// <summary>
        /// ViewMode构造函数：初始化控件、命令、DataSource
        /// </summary>
        public AlarmHistoryInfoViewModel()
        {
            PageTitle = SystemResources.Instance.LanguageArray[1714];
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
            StartDate = DateTime.Now.Date.AddDays(-7);
            SearchCommand = new RelayCommand(SearchByCodeMethodNew);


            QueryCommand = new RelayCommand(QueryAlarmHitory);

            //QueryAlarmHitory();

            //LoadedCommand = new RelayCommand(LoadedMethod);

            //OnPageIndexChangedCommand = new RelayCommand<PageControlTestEventHandler>(OnPageIndexChangedMethod);

            //LoadedMethod();

            QueryAlarmHitory();
        }

        #region 属性
        private int _TotalCount = 1;
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalCount
        {
            get { return _TotalCount; }
            set { Set(ref _TotalCount, value); }
        }

        private int _PageRecordCount = 100;
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageRecordCount
        {
            get { return _PageRecordCount; }
            set { Set(ref _PageRecordCount, value); }
        }
        /// <summary>
        /// 当前跳过数量
        /// </summary>
        private int CurrentSkipCount { get; set; }


        private List<AlarmHistoryInfoModel> getHistoryIns = new List<AlarmHistoryInfoModel>();
        /// <summary>
        /// 报警历史数据集
        /// </summary>
        public List<AlarmHistoryInfoModel> GetHistoryIns
        {
            get { return getHistoryIns; }
            set { Set(ref getHistoryIns, value); }
        }
        private DateTime startDate = DateTime.Now.Date;
        //private DateTime startDateTmp = DateTime.Now.Date;
        /// <summary>
        /// 开始时间选择
        /// </summary>
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                //if (value > EndDate)
                //{
                //    NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[2298], MessageBoxButton.OK, DrMessageBoxImage.Warning);
                //    return;
                //}
                Set(ref startDate, value);
                //startDateTmp = startDate;
                //QueryAlarmHitory(CurrentSkipCount, PageRecordCount);
            }
        }
        private DateTime endDate = DateTime.Now.Date;
        //private DateTime endDateTmp = DateTime.Now.Date;
        /// <summary>
        /// 结束时间选择
        /// </summary>
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                //value = value.Date.AddDays(86399F / 86400);
                //if (value < StartDate)
                //{
                //    NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[2298], MessageBoxButton.OK, DrMessageBoxImage.Warning);
                //    return;
                //}
                Set(ref endDate, value);
                //endDateTmp = endDate;
                //QueryAlarmHitory(CurrentSkipCount, PageRecordCount);
            }
        }
        private List<SystemTypeValueModel> lookUpEditSourceType = new List<SystemTypeValueModel>();
        /// <summary>
        /// 报警类型集合：全部、数据报警、故障报警
        /// </summary>
        public List<SystemTypeValueModel> LookUpEditSourceType
        {
            get { return lookUpEditSourceType; }
            set { Set(ref lookUpEditSourceType, value); }
        }
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
            }
        }
        private List<SystemTypeValueModel> lookUpEditSourcelevel = new List<SystemTypeValueModel>();
        /// <summary>
        ///报警级别集合：注意（数据/故障）、加样停止、停止 
        /// </summary>
        public List<SystemTypeValueModel> LookUpEditSourceLevel
        {
            get { return lookUpEditSourcelevel; }
            set { Set(ref lookUpEditSourcelevel, value); }
        }
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
        private List<AlarmHistoryInfoModel> gridCtrlSource;
        /// <summary>
        /// GridCtrl关联的数据表数据
        /// </summary>
        public List<AlarmHistoryInfoModel> GridCtrlSource
        {
            get { return gridCtrlSource; }
            set
            {
                Set(ref gridCtrlSource, value);
                if (GridCtrlSource != null)
                    StrCodeTimes = GridCtrlSource.Count.ToString();
            }
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
                    if (SelectRow.ModuleType == 0)
                    {
                        model = SystemResources.Instance.AlarmOrignalInfos.FirstOrDefault(o => o.Code == SelectRow.Code);
                    }
                    else
                    {
                        model = SystemResources.Instance.AlarmOrignalInfos.FirstOrDefault(o => o.ModuleType == SelectRow.ModuleType && o.Code == SelectRow.Code);
                    }
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

        private string strCode;
        /// <summary>
        /// 动态输入的报警码
        /// </summary>
        public string StrCode
        {
            get { return strCode; }
            set { Set(ref strCode, value); }
        }
        private string strCodeTimes;
        /// <summary>
        /// 统计该报警产生的次数
        /// </summary>
        public string StrCodeTimes
        {
            get { return strCodeTimes; }
            set { Set(ref strCodeTimes, value); }
        }
        #endregion

        #region 命令

        public RelayCommand QueryCommand { get; set; }

        /// <summary>
        /// 加载命令
        /// </summary>
        public RelayCommand LoadedCommand { get; set; }
        /// <summary>
        /// 翻页命令
        /// </summary>
        public RelayCommand<PageControlTestEventHandler> OnPageIndexChangedCommand { get; set; }
        /// <summary>
        /// 按照报警码搜索
        /// </summary>
        public RelayCommand SearchCommand { get; set; }

        /// <summary>
        /// 通过报警码搜索报警历史信息
        /// </summary>
        private void SearchByCodeMethod()
        {
            //if (!string.IsNullOrEmpty(StrCode) && strCode.Contains("-") && strCode.Split('-')[1].Length > 0)
            if (!string.IsNullOrEmpty(StrCode) && !string.IsNullOrEmpty(StrCode.Trim()))
            {
                GridCtrlSource = new List<AlarmHistoryInfoModel>(GridCtrlSource.Where(p => p.Code.Contains(StrCode)).ToList());
                if (GridCtrlSource == null || GridCtrlSource.Count <= 0)
                    SelectRow = null;
            }
            else
                OnSelectedInfoChanged();
        }
        /// <summary>
        /// 通过报警码搜索报警历史信息
        /// </summary>
        private void SearchByCodeMethodNew()
        {
            OnSelectedInfoChanged();
            SearchByCodeMethod();
        }
        #endregion

        #region 其他方法
        /// <summary>
        /// 初始化报警级别列表
        /// </summary>
        private void IntiLookUpEdit()
        {
            GetAlarmLevelInfo();
            GetAlarmTypeInfo();
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
        /// 进入界面后显示总页数及基础数据
        /// </summary>
        private void LoadedMethod()
        {
            int totalcount = 0;
            var result = opera.GetAlarmHitoryInfosAllCount(StartDate, EndDate);

            if (result.ResultBool)
                totalcount = result.Results;

            TotalCount = totalcount;

            QueryAlarmHitory(0, PageRecordCount);
        }
        /// <summary>
        /// 翻页查询数据
        /// </summary>
        /// <param name="parameters"></param>
        private void OnPageIndexChangedMethod(PageControlTestEventHandler parameters)
        {
            CurrentSkipCount = parameters.SkipCount;
            QueryAlarmHitory(parameters.SkipCount, PageRecordCount);
        }
        /// <summary>
        /// 报警查询
        /// </summary>
        private void QueryAlarmHitory(int skipCount, int takeCount)
        {
            if (startDate > endDate)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[3017]);
                startDate = endDate;
                return;
            }

            OperationResult<List<AlarmHistoryInfoModel>> operationResult = opera.GetAlarmHitoryInfosAll(StartDate, EndDate, skipCount, takeCount);
            if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
            {
                GetHistoryIns = new List<AlarmHistoryInfoModel>(operationResult.Results);
                OnSelectedInfoChanged();
                if (!string.IsNullOrEmpty(StrCode))
                    SearchByCodeMethod();
            }
            else
            {
                ShowMessageError(SystemResources.Instance.LanguageArray[6370]); //查询失败
            }
        }

        /// <summary>
        /// 报警查询
        /// </summary>
        private void QueryAlarmHitory()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (StartDate > EndDate)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[3017]);
                StartDate = EndDate;
                return;
            }

            OperationResult<List<AlarmHistoryInfoModel>> operationResult = new OperationResult<List<AlarmHistoryInfoModel>>();
            LoadingHelper.Instance.ShowLoadingWindow(a =>
            {
                a.Title = "...";
                operationResult = opera.GetAlarmHitoryInfosAll(StartDate, EndDate);
            }, 0, a =>
            {
                if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    GetHistoryIns = new List<AlarmHistoryInfoModel>(operationResult.Results);
                    OnSelectedInfoChanged();
                    if (!string.IsNullOrEmpty(StrCode))
                        SearchByCodeMethod();
                }
                else
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6370]); //查询失败
                }
            });


            sw.Stop();
            LogHelper.logSoftWare.Debug($"报警历史信息显示处理时间为：{sw.ElapsedMilliseconds}");
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
                    GridCtrlSource = GetHistoryIns.Where(p => p.AlarmStyle == (AlarmStyleEnum)SelectAlarmsTypeIndex.Code && p.CodeLevel == (AlarmLevelEnum)SelectAlarmsLevelIndex.Code).ToList();
                else if (SelectAlarmsTypeIndex.Code != 0 && SelectAlarmsLevelIndex.Code == 0)
                    GridCtrlSource = GetHistoryIns.Where(p => p.AlarmStyle == (AlarmStyleEnum)SelectAlarmsTypeIndex.Code).ToList();
                else if (SelectAlarmsTypeIndex.Code == 0 && SelectAlarmsLevelIndex.Code != 0)
                    GridCtrlSource = GetHistoryIns.Where(p => p.CodeLevel == (AlarmLevelEnum)SelectAlarmsLevelIndex.Code).ToList();
                else
                    GridCtrlSource = GetHistoryIns;
            }
            else
            {
                if (SelectAlarmsTypeIndex.Code != 0 && SelectAlarmsLevelIndex.Code != 0)
                    GridCtrlSource = GetHistoryIns.Where(p => p.AlarmStyle == (AlarmStyleEnum)SelectAlarmsTypeIndex.Code && p.CodeLevel == (AlarmLevelEnum)SelectAlarmsLevelIndex.Code && p.ModuleID == SelectedModuleInfo.ModuleID).ToList();
                else if (SelectAlarmsTypeIndex.Code != 0 && SelectAlarmsLevelIndex.Code == 0)
                    GridCtrlSource = GetHistoryIns.Where(p => p.AlarmStyle == (AlarmStyleEnum)SelectAlarmsTypeIndex.Code && p.ModuleID == SelectedModuleInfo.ModuleID).ToList();
                else if (SelectAlarmsTypeIndex.Code == 0 && SelectAlarmsLevelIndex.Code != 0)
                    GridCtrlSource = GetHistoryIns.Where(p => p.CodeLevel == (AlarmLevelEnum)SelectAlarmsLevelIndex.Code && p.ModuleID == SelectedModuleInfo.ModuleID).ToList();
                else
                    GridCtrlSource = GetHistoryIns.Where(p => p.ModuleID == SelectedModuleInfo.ModuleID).ToList();
            }

            if (GridCtrlSource == null || GridCtrlSource.Count <= 0)
                SelectRow = null;
        }
    }
    #endregion
}
