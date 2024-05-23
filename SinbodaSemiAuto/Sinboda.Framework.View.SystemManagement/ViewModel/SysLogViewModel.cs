using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Sinboda.Framework.Business.SystemManagement;
using Sinboda.Framework.Common;
using Sinboda.Framework.Common.ExportImportHelper;
using Sinboda.Framework.Control.DataPage;
using Sinboda.Framework.Control.Loading;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.CommonModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Print;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.View.SystemManagement.ViewModel
{
    /// <summary>
    /// 日志界面处理
    /// </summary>
    public class SysLogViewModel : NavigationViewModelBase
    {
        /// <summary>
        /// 系统日志业务逻辑接口
        /// </summary>
        LogManagerBusiness business = new LogManagerBusiness();
        /// <summary>
        /// 权限管理业务逻辑接口
        /// </summary>
        PermissionManagerBusiness permissbusiness = new PermissionManagerBusiness();
        /// <summary>
        /// 基础信息业务逻辑接口
        /// </summary>
        ManagementBusiness managebusiness = new ManagementBusiness();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SysLogViewModel()
        {
            PageTitle = SystemResources.Instance.LanguageArray[1456];
            GetLogTypeInfo();
            PrintLogCommand = new RelayCommand(PrintLogMethod);
            ExportLogCommand = new RelayCommand(ExportLogMethod);
            LoadedCommand = new RelayCommand(LoadedMethod);

            QueryCommand = new RelayCommand(RefreshData);

            OnPageIndexChangedCommand = new RelayCommand<PageControlTestEventHandler>(OnPageIndexChangedMethod);
        }

        /// <summary>
        /// 加载事件
        /// </summary>
        public void LoadedPage()
        {
            //应加入多国语言处理
            var _userList = permissbusiness.GetUserList();
            if (_userList.ResultEnum == OperationResultEnum.SUCCEED)
            {
                var _list = new List<String>();
                _userList.Results.ForEach((result =>
                {
                    _list.Add(result.UserName);
                }));
                string all = SystemResources.Instance.LanguageArray[1719];//全部

                _list.Insert(0, all);
                UserList = _list;
            }
            int _selectIndex = UserList.IndexOf(SystemResources.Instance.CurrentUserName);
            if (_selectIndex > 0)
            {
                SelectUserIndex = _selectIndex;
            }
            else
            {
                SelectUserIndex = 0;
            }
            LoadedMethod();
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
        /// <summary>
        /// 日志类型列表
        /// </summary>
        private List<SystemTypeValueModel> logTypeList = new List<SystemTypeValueModel>();
        /// <summary>
        /// 日志类型列表属性
        /// </summary>
        public List<SystemTypeValueModel> LogTypeList
        {
            get { return logTypeList; }
            set { Set(ref logTypeList, value); }
        }
        /// <summary>
        /// 选中日志类型索引
        /// </summary>
        private SystemTypeValueModel selectLogTypeIndex = new SystemTypeValueModel();
        /// <summary>
        /// 选中日志类型索引属性
        /// </summary>
        public SystemTypeValueModel SelectLogTypeIndex
        {
            get { return selectLogTypeIndex; }
            set
            {
                Set(ref selectLogTypeIndex, value);
                //if (selectLogTypeIndex.Code != -1)
                //{
                //    LoadedMethod();
                //}
            }
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        private List<string> userList = new List<string>();
        /// <summary>
        /// 日志类型列表属性
        /// </summary>
        public List<string> UserList
        {
            get { return userList; }
            set { Set(ref userList, value); }
        }
        /// <summary>
        /// 选中日志类型索引
        /// </summary>
        private int selectUserIndex = 0;
        /// <summary>
        /// 选中日志类型索引属性
        /// </summary>
        public int SelectUserIndex
        {
            get { return selectUserIndex; }
            set
            {
                Set(ref selectUserIndex, value);
                //if (selectUserIndex != -1)
                //{
                //    LoadedMethod();
                //}
            }
        }
        /// <summary>
        /// 日志列表
        /// </summary>
        private List<SysLogModel> logList = new List<SysLogModel>();
        /// <summary>
        /// 日志列表属性
        /// </summary>
        public List<SysLogModel> LogList
        {
            get { return logList; }
            set { Set(ref logList, value); }
        }
        /// <summary>
        /// 日志列表选中项
        /// </summary>
        private SysLogModel selectLog;
        /// <summary>
        /// 日志列表选中项属性
        /// </summary>
        public SysLogModel SelectLog
        {
            get { return selectLog; }
            set { Set(ref selectLog, value); }
        }
        /// <summary>
        /// 查询条件中的开始日期
        /// </summary>
        private DateTime startDate = DateTime.Today.Date;
        private DateTime startDateTmp = DateTime.Now.Date;
        /// <summary>
        /// 查询条件中的开始日期属性
        /// </summary>
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                //if (value > endDate)
                //{
                //    ShowMessageWarning(SystemResources.Instance.LanguageArray[8644]);
                //    return;
                //}

                Set(ref startDate, value);
                //if (startDate >= endDate.AddDays(-30))
                //{
                //    startDateTmp = startDate;
                //    LoadedMethod();
                //}
                //else
                //{
                //    ShowMessageError(SystemResources.Instance.LanguageArray[6369]); //TODO 翻译"查询时间跨度不能超过30天
                //    StartDate = startDateTmp;
                //}
                //RefreshData(CurrentSkipCount, PageRecordCount);
            }
        }
        /// <summary>
        /// 查询条件中的结束日期
        /// </summary>
        private DateTime endDate = DateTime.Today;
        private DateTime endDateTmp = DateTime.Now.Date;
        /// <summary>
        /// 查询条件中的结束日期属性
        /// </summary>
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                //value = value.Date.AddDays(86399F / 86400);
                //if (value < startDate)
                //{
                //    ShowMessageWarning(SystemResources.Instance.LanguageArray[2298]);
                //    return;
                //}

                Set(ref endDate, value);
                //if (endDate <= startDate.AddDays(30))
                //{
                //    endDateTmp = endDate;
                //    LoadedMethod();
                //}
                //else
                //{
                //    ShowMessageError(SystemResources.Instance.LanguageArray[6369]); //TODO 翻译"查询时间跨度不能超过30天
                //    EndDate = endDateTmp;
                //}
                //RefreshData(CurrentSkipCount, PageRecordCount);
            }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 加载命令
        /// </summary>
        public RelayCommand LoadedCommand { get; set; }
        /// <summary>
        /// 翻页命令
        /// </summary>
        public RelayCommand<PageControlTestEventHandler> OnPageIndexChangedCommand { get; set; }
        /// <summary>
        /// 打印日志命令
        /// </summary>
        public RelayCommand PrintLogCommand { get; set; }
        /// <summary>
        /// 导出日志命令
        /// </summary>
        public RelayCommand ExportLogCommand { get; set; }

        public RelayCommand QueryCommand { get; set; }


        protected override void OnParameterChanged(object parameter)
        {
            base.OnParameterChanged(parameter);

            StartDate = DateTime.Today.Date;
            EndDate = DateTime.Today;

        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 进入界面后显示总页数及基础数据
        /// </summary>
        private void LoadedMethod()
        {
            string user;
            if (userList != null && SelectUserIndex >= 1)
                user = userList[SelectUserIndex];
            else
                user = "";
            if (startDate > endDate)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[2298]);//29提示2298结束日期不可早于起始日期！
                return;
            }

            RefreshData();

            //int totalcount = 0;
            //var result = business.QueryLogCount(startDate, endDate, (SysLogType)(selectLogTypeIndex.Code), user);

            //if (result.ResultBool)
            //    totalcount = result.Results;

            //TotalCount = totalcount;

            //RefreshData(0, PageRecordCount);
        }
        /// <summary>
        /// 刷新列表
        /// </summary>
        private void RefreshData()
        {
            string user;
            if (userList != null && SelectUserIndex >= 1)
                user = userList[SelectUserIndex];
            else
                user = "";
            if (startDate > endDate)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[2298]);//29提示2298结束日期不可早于起始日期！
                return;
            }
            OperationResult<List<SysLogModel>> result = new OperationResult<List<SysLogModel>>();
            LoadingHelper.Instance.ShowLoadingWindow(a =>
            {
                a.Title = "...";
                result = business.QueryLog(startDate, endDate.AddDays(86399F / 86400), (SysLogType)(selectLogTypeIndex.Code), user);//1登录日志 2操作日志 
            }, 0, a =>
            {
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    List<SysLogModel> tmp = new List<SysLogModel>();
                    foreach (var item in userList)
                    {
                        tmp.AddRange(result.Results.OrderByDescending(o => o.Datetime).Where(o => o.UserID == item).ToList());
                    }
                    LogList = tmp.OrderByDescending(o => o.Datetime).ToList();
                }
                else
                {
                    ShowMessageError(result.Message);
                }
            });

        }

        #region 废弃
        /// <summary>
        /// 翻页查询数据
        /// </summary>
        /// <param name="parameters"></param>
        private void OnPageIndexChangedMethod(PageControlTestEventHandler parameters)
        {
            CurrentSkipCount = parameters.SkipCount;
            RefreshData(parameters.SkipCount, PageRecordCount);
        }
        /// <summary>
        /// 刷新列表
        /// </summary>
        private void RefreshData(int skipCount, int takeCount)
        {
            string user;
            if (userList != null && SelectUserIndex >= 1)
                user = userList[SelectUserIndex];
            else
                user = "";
            if (startDate > endDate)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[2298]);//29提示2298结束日期不可早于起始日期！
                return;
            }
            var result = business.QueryLog(startDate, endDate, (SysLogType)(selectLogTypeIndex.Code), user, skipCount, takeCount);//1登录日志 2操作日志 
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
            {
                List<SysLogModel> tmp = new List<SysLogModel>();
                foreach (var item in userList)
                {
                    tmp.AddRange(result.Results.OrderByDescending(o => o.Datetime).Where(o => o.UserID == item).ToList());
                }
                LogList = tmp.OrderByDescending(o => o.Datetime).ToList();
            }
            else
            {
                ShowMessageError(result.Message);
            }
        }
        #endregion

        /// <summary>
        /// 获取日志类型
        /// </summary>
        private void GetLogTypeInfo()
        {
            DataDictionaryService.Instance.InitializeSystemTypeDictionary();
            LogTypeList = DataDictionaryService.Instance.SystemTypeValueDictionary["logType"];

            if (SystemResources.Instance.CurrentPermissionList.Keys.Contains("ResultPrint"))
            {
                if (!SystemResources.Instance.CurrentPermissionList["ResultPrint"])
                    LogTypeList.RemoveAll(o => o.Code != 1);
            }

            SelectLogTypeIndex = LogTypeList.LastOrDefault();
        }
        #endregion


        /// <summary>
        /// 检查
        /// </summary>
        public bool Check()
        {
            if (LogList.Count == 0)
            {
                ShowMessageError(SystemResources.Instance.LanguageArray[3127]);//导出数据失败
                return false;
            }
            return true;
        }
        /// <summary>
        /// 打印日志的操作
        /// </summary>
        public void PrintLogMethod()
        {
            if (!Check())
                return;
            List<LogModelPrint> list = new List<LogModelPrint>();
            foreach (var item in logList)
            {
                LogModelPrint temp = new LogModelPrint();
                temp.Id = item.Id;
                temp.Message = item.Message;
                temp.Type = item.Type;
                temp.Datetime = item.Datetime.ToString();
                temp.Typestr = temp.Type == 1 ? SystemResources.Instance.LanguageArray[1952] : (temp.Type == 3 ? SystemResources.Instance.LanguageArray[27] : "");
                temp.UserID = item.UserID;
                list.Add(temp);
            }
            new SinReport(PageTitle, list, "SystemLog.repx", null, false, false);
        }
        /// <summary>
        /// 导出日志的操作
        /// </summary>
        public void ExportLogMethod()
        {
            if (!Check())
                return;
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "csv file(*.csv)|*.csv|xls file(*.xls)|*.xls"; Directory.GetDirectoryRoot(MapPath.AppDir);
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            dlg.FileName = DateTime.Now.ToString("yyyyMMdd") + "LogData";
            if (dlg.ShowDialog() == true)
            {
                ExportData<SysLogModel> datas = new ExportData<SysLogModel>();
                datas.Datas = logList;
                datas.SheetName = DateTime.Now.ToString("yyyyMMdd") + "LogData";
                datas.PropertiesToColumnHeads.Add("Datetime", SystemResources.Instance.LanguageArray[1142]);
                datas.PropertiesToColumnHeads.Add("UserID", SystemResources.Instance.LanguageArray[1129]);
                datas.PropertiesToColumnHeads.Add("Message", SystemResources.Instance.LanguageArray[1953]);
                IExportAndImport exportAndImport = new ExportAndImport();
                if (exportAndImport.ExportFile(dlg.FileName, datas))
                {
                    ShowMessageComplete(SystemResources.Instance.LanguageArray[4100]);
                }
                else
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6382]);//导出数据失败
                }
            }
        }
    }
}
