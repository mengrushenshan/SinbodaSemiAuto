using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Business.SystemAlarm;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Sinboda.Framework.View.SystemAlarm.ViewModel
{
    /// <summary>
    /// 报警信息设置业务实现类
    /// </summary>
    public class AlarmSettingViewModel : NavigationViewModelBase
    {
        /// <summary>
        /// 报警设置业务层对象
        /// </summary>
        private AlarmSettingBusiness operation = new AlarmSettingBusiness();
        /// <summary>
        /// 所有数据
        /// </summary>
        List<AlarmOrignalInfoModel> AllAlarmInfos = new List<AlarmOrignalInfoModel>();
        /// <summary>
        /// 用于存储当前原始信息表中的各单元对应的报警码
        /// </summary>
        Dictionary<string, List<string>> dicOfCode = new Dictionary<string, List<string>>();


        private bool isDataError;
        public bool IsDataError
        {
            get { return isDataError; }
            set { Set(ref isDataError, value); }
        }

        /// <summary>
        /// ViewModel 初始化命令、数据源、控件
        /// </summary>
        public AlarmSettingViewModel()
        {
            PageTitle = SystemResources.Instance.LanguageArray[1715];
            SaveVisibility = Visibility.Collapsed;

            if (DataDictionaryService.Instance.GetIsMultiModuleMode)
            {
                ModuleInfoVisibility = Visibility.Visible;
                ModuleInfoVisible = true;
                ModuleInfoSource.Add(new ModuleTypeModel() { ModuleTypeCode = 0, ModuleTypeName = SystemResources.Instance.LanguageArray[1719] });
                foreach (var item in DataDictionaryService.Instance.ModuleTypeInfo)
                {
                    item.Value.ModuleTypeName = item.Value.LanguageID != 0 ? SystemResources.Instance.LanguageArray[item.Value.LanguageID] : item.Value.ModuleTypeName;

                    if (item.Value.IsShow)
                        ModuleInfoSource.Add(item.Value);
                }
                SelectedModuleInfo = ModuleInfoSource.FirstOrDefault();
            }
            else
            {
                ModuleInfoSource = new List<ModuleTypeModel>();
                SelectedModuleInfo = new ModuleTypeModel();
                ModuleInfoVisibility = Visibility.Collapsed;
                ModuleInfoVisible = false;
            }

            GetAllInfs(true);
            ResetCommand = new RelayCommand(ResetMethod);
            SaveEnabledCommnad = new RelayCommand(SaveEnabledMethod);
            ChangeVisibleFlagCommand = new RelayCommand(ChangeVisibleFlagMethod);
            SearchCommand = new RelayCommand(RefreshDataSource);
            GetCautionCommnad = new RelayCommand(GetCautionAlarmSound);
            GetStopCommnad = new RelayCommand(GetStopAlarmSound);
            GetWarningCommnad = new RelayCommand(GetWarningAlarmSound);
        }

        #region 属性

        private int tabSelectedIndex;
        /// <summary>
        /// 选项卡切换选中
        /// </summary>
        public int TabSelectedIndex
        {
            get { return tabSelectedIndex; }
            set
            {
                Set(ref tabSelectedIndex, value);
                if (TabSelectedIndex == 0)
                {
                    RefreshDataSource();
                    SaveVisibility = Visibility.Collapsed;
                }
                if (TabSelectedIndex == 1)
                {
                    SetEnabledMehod(false);
                    SaveVisibility = Visibility.Visible;
                }
            }
        }

        private List<AlarmOrignalInfoModel> getAlarmIns = new List<AlarmOrignalInfoModel>();
        /// <summary>
        /// 报警信息数据集
        /// </summary>
        public List<AlarmOrignalInfoModel> GetAlarmIns
        {
            get { return getAlarmIns; }
            set { Set(ref getAlarmIns, value); }
        }


        private List<ModuleTypeModel> moduleInfoSource = new List<ModuleTypeModel>();
        /// <summary>
        /// 模块下拉列表集合
        /// </summary>
        public List<ModuleTypeModel> ModuleInfoSource
        {
            get { return moduleInfoSource; }
            set { Set(ref moduleInfoSource, value); }
        }

        private ModuleTypeModel selectedModuleInfo = new ModuleTypeModel();
        /// <summary>
        /// 模块下拉列表选中项
        /// </summary>
        public ModuleTypeModel SelectedModuleInfo
        {
            get { return selectedModuleInfo; }
            set
            {
                Set(ref selectedModuleInfo, value);
                RefreshDataSource();
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

        private List<string> strCodePreSource = new List<string>();
        /// <summary>
        ///报警码前半段集合
        /// </summary>
        public List<string> StrCodePreSource
        {
            get { return strCodePreSource; }
            set { Set(ref strCodePreSource, value); }
        }

        private string strCodePre;
        /// <summary>
        /// 报警码前半段
        /// </summary>
        public string StrCodePre
        {
            get { return strCodePre; }
            set
            {
                Set(ref strCodePre, value);
                List<string> listtmp = new List<string>();
                listtmp.Add(SystemResources.Instance.LanguageArray[1719]);
                int tmp;
                if (int.TryParse(StrCodePre, out tmp))
                {
                    listtmp.AddRange(dicOfCode[StrCodePre]);
                }
                StrCodeAfterSource = listtmp;
                StrCodeAfter = StrCodeAfterSource.FirstOrDefault();
                RefreshDataSource();
            }
        }

        private List<string> strCodeAfterSource = new List<string>();
        /// <summary>
        ///报警码后半段集合
        /// </summary>
        public List<string> StrCodeAfterSource
        {
            get { return strCodeAfterSource; }
            set { Set(ref strCodeAfterSource, value); }
        }

        private string strCodeAfter;
        /// <summary>
        /// 报警码后半段
        /// </summary>
        public string StrCodeAfter
        {
            get { return strCodeAfter; }
            set
            {
                Set(ref strCodeAfter, value);
                RefreshDataSource();
            }
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
        private AlarmOrignalInfoModel selectRow;
        /// <summary>
        /// 选中行
        /// </summary>
        public AlarmOrignalInfoModel SelectRow
        {
            get { return selectRow; }
            set { Set(ref selectRow, value); }
        }

        private bool warningLevelVisibleUse;
        /// <summary>
        /// 注意级别
        /// </summary>
        public bool WarningLevelVisibleUse
        {
            get { return warningLevelVisibleUse; }
            set { Set(ref warningLevelVisibleUse, value); }
        }

        private bool sampleStopLevelVisibleUse;
        /// <summary>
        /// 加样停止级别
        /// </summary>
        public bool SampleStopLevelVisibleUse
        {
            get { return sampleStopLevelVisibleUse; }
            set { Set(ref sampleStopLevelVisibleUse, value); }
        }
        private bool stopLevelVisibleUse;
        /// <summary>
        /// 停止级别
        /// </summary>
        public bool StopLevelVisibleUse
        {
            get { return stopLevelVisibleUse; }
            set { Set(ref stopLevelVisibleUse, value); }
        }

        private bool cautionLevelHaveSound;
        /// <summary>
        /// 注意级别
        /// </summary>
        public bool CautionLevelHaveSound
        {
            get { return cautionLevelHaveSound; }
            set { Set(ref cautionLevelHaveSound, value); }
        }

        private bool warningLevelHaveSound;
        /// <summary>
        /// 加样停止级别
        /// </summary>
        public bool WarningLevelHaveSound
        {
            get { return warningLevelHaveSound; }
            set { Set(ref warningLevelHaveSound, value); }
        }
        private bool stopLevelHaveSound;
        /// <summary>
        /// 停止级别
        /// </summary>
        public bool StopLevelHaveSound
        {
            get { return stopLevelHaveSound; }
            set { Set(ref stopLevelHaveSound, value); }
        }

        private Visibility saveVisibility;
        /// <summary>
        /// 保存按钮可见状态
        /// </summary>
        public Visibility SaveVisibility
        {
            get { return saveVisibility; }
            set { Set(ref saveVisibility, value); }
        }

        private string cautionAlarmSoundPath;
        /// <summary>
        /// 列表中模块显示与否
        /// </summary>
        public string CautionAlarmSoundPath
        {
            get { return cautionAlarmSoundPath; }
            set { Set(ref cautionAlarmSoundPath, value); }
        }

        private string stopAlarmSoundPath;
        /// <summary>
        /// 列表中模块显示与否
        /// </summary>
        public string StopAlarmSoundPath
        {
            get { return stopAlarmSoundPath; }
            set { Set(ref stopAlarmSoundPath, value); }
        }

        private string warningAlarmSoundPath;
        /// <summary>
        /// 列表中模块显示与否
        /// </summary>
        public string WarningAlarmSoundPath
        {
            get { return warningAlarmSoundPath; }
            set { Set(ref warningAlarmSoundPath, value); }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 传递原事件参数
        /// </summary>
        public RelayCommand ChangeVisibleFlagCommand { get; set; }
        /// <summary>
        /// 报警是否显示复选框响应函数
        /// </summary>
        public void ChangeVisibleFlagMethod()
        {
            string code = SelectRow.Code;
            bool flag = SelectRow.CodeVisibility;
            operation.SetAlarmCodeVisibility(code, flag);//将显示标志置为与之前相反的状态
            GetAllInfs(false);
            RefreshDataSource();
        }

        /// <summary>
        /// 报警是否显示复选框响应函数
        /// </summary>
        public void ChangeHaveSoundFlagMethod()
        {
            string code = SelectRow.Code;
            bool flag = SelectRow.HaveSound;
            AlarmLevelEnum alarmLevelEnum = SelectRow.CodeLevel;
            
            GetAllInfs(false);
            RefreshDataSource();
        }
        /// <summary>
        /// 恢复默认
        /// </summary>
        public RelayCommand ResetCommand { get; set; }
        /// <summary>
        /// 恢复默认
        /// </summary>
        private void ResetMethod()
        {
            //"提示", "您确认要恢复默认设置吗? "
            if (NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[2197], SystemResources.Instance.LanguageArray[79], MessageBoxButton.YesNo, SinMessageBoxImage.Question) == MessageBoxResult.No)
                return;
            OperationResult operationResult = operation.ChangeVisiblFlags(new long[] { 1, 2, 3 }, new bool[] { true, true, true });
            if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
            {
                WriteOperateLog(string.Format("{0}  :  {1}", SystemResources.Instance.LanguageArray[1715], SystemResources.Instance.LanguageArray[3784]));
                GetAllInfs(true);
                RefreshDataSource();
                SetEnabledMehod(true);
                ShowMessageComplete(SystemResources.Instance.LanguageArray[3784]);
            }
            else
                //输出错误信息
                ShowMessageError(operationResult.Message);//29提示
        }
        /// <summary>
        /// 保存命令
        /// </summary>
        public RelayCommand SaveEnabledCommnad { get; set; }

        /// <summary>
        /// 保存方法
        /// </summary>
        private void SaveEnabledMethod()
        {
            OperationResult operationResult = operation.ChangeVisiblFlags(new long[] { 1, 2, 3 }, new bool[] { warningLevelVisibleUse, sampleStopLevelVisibleUse, stopLevelVisibleUse });
            if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
            {
                GetAllInfs(true);
                WriteOperateLog(string.Format("{0}  :  {1}", SystemResources.Instance.LanguageArray[1715], SystemResources.Instance.LanguageArray[609]));//29提示609保存成功 
                ShowMessageComplete(SystemResources.Instance.LanguageArray[368]);
            }
            else
                ShowMessageError(SystemResources.Instance.LanguageArray[610]);   //29提示610保存失败！
        }

        /// <summary>
        /// 保存命令
        /// </summary>
        public RelayCommand GetCautionCommnad { get; set; }

        /// <summary>
        /// 保存方法
        /// </summary>
        private void GetCautionAlarmSound()
        {
            string path = GetSoundFileMethod();
            if (!string.IsNullOrEmpty(path))
            {

                CautionAlarmSoundPath = path;
            }
        }
        /// <summary>
        /// 保存命令
        /// </summary>
        public RelayCommand GetStopCommnad { get; set; }

        /// <summary>
        /// 保存方法
        /// </summary>
        private void GetStopAlarmSound()
        {
            string path = GetSoundFileMethod();
            if (!string.IsNullOrEmpty(path))
            {

                StopAlarmSoundPath = path;
            }

        }

        /// <summary>
        /// 保存命令
        /// </summary>
        public RelayCommand GetWarningCommnad { get; set; }

        /// <summary>
        /// 保存方法
        /// </summary>
        private void GetWarningAlarmSound()
        {
            string path = GetSoundFileMethod();
            if (!string.IsNullOrEmpty(path))
            {

                WarningAlarmSoundPath = path;
            }

        }

        /// <summary>
        /// 获取备份路径方法
        /// </summary>
        private string GetSoundFileMethod()
        {
            OpenFileDialog FBD = new OpenFileDialog();
            FBD.Title = SystemResources.Instance.LanguageArray[6442];//"请选择路径";
            FBD.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            FBD.Multiselect = false;
            FBD.Filter = "(*.wav)|*.wav";
            if (FBD.ShowDialog() == DialogResult.OK)
            {

                if (!string.IsNullOrEmpty(FBD.FileName))
                {
                    return FBD.FileName;
                }
                else
                {
                    //  ShowMessageError(SystemResources.Instance.LanguageArray[3543]);
                }

            }
            return string.Empty;
        }

        /// <summary>
        /// 按照报警码搜索
        /// </summary>
        public RelayCommand SearchCommand { get; set; }
        #endregion

        #region 其他方法
        /// <summary>
        /// 获取报警信息数据集合
        /// </summary>
        /// <returns></returns>
        private void GetAllInfs(bool isAll)
        {
            OperationResult<List<AlarmOrignalInfoModel>> operationResult = operation.GetAlarmInfos();
            if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
            {
                AllAlarmInfos = operationResult.Results;
                if (SystemResources.Instance.CurrentUserName != "dryf")
                    AllAlarmInfos = AllAlarmInfos.Where(o => o.CodeLevel != AlarmLevelEnum.Debug).ToList();
                if (AllAlarmInfos.Count > 0)
                {
                    foreach (var item in AllAlarmInfos)
                    {
                        string[] str = item.Code.Split('-');
                        if (!dicOfCode.ContainsKey(str[0]))
                            dicOfCode.Add(str[0], new List<string>() { str[1] });
                        else
                        {
                            if (!dicOfCode[str[0]].Contains(str[1]))
                            {
                                dicOfCode[str[0]].Add(str[1]);
                            }
                        }
                    }
                    if (isAll)
                    {
                        //StrCodePreSource.Clear();
                        //StrCodePreSource.Add(SystemResources.Instance.LanguageArray[1719]);
                        //StrCodePreSource.AddRange(dicOfCode.Keys);
                        //StrCodePre = StrCodePreSource.FirstOrDefault();

                        //StrCodeAfterSource.Clear();
                        //StrCodeAfterSource.Add(SystemResources.Instance.LanguageArray[1719]);
                        //StrCodeAfter = StrCodeAfterSource.FirstOrDefault();
                        StrCode = string.Empty;
                        RefreshDataSource();
                    }
                }
            }
            else
            {
                //输出错误信息
                ShowMessageError(operationResult.Message);//29提示
            }
        }
        /// <summary>
        /// 重新装载所有数据
        /// </summary>
        private void RefreshDataSource()
        {
            if (IsDataError) return;

            GetAlarmIns.Clear();
            List<AlarmOrignalInfoModel> listtmp = new List<AlarmOrignalInfoModel>();
            if (!string.IsNullOrEmpty(StrCode))
            {
                #region
                //int tmp;
                //if (int.TryParse(StrCodePre, out tmp))
                //{
                //    if (int.TryParse(StrCodeAfter, out tmp))
                //    {
                //        if (DataDictionaryService.Instance.GetIsMultiModuleMode && SelectedModuleInfo != ModuleInfoSource.FirstOrDefault())
                //            listtmp.AddRange(AllAlarmInfos.Where(o => o.ModuleType == selectedModuleInfo.ModuleTypeCode && o.Code == StrCodePre + "-" + StrCodeAfter));
                //        else
                //            listtmp.AddRange(AllAlarmInfos.Where(o => o.Code == StrCodePre + "-" + StrCodeAfter));
                //    }
                //    else
                //    {
                //        if (DataDictionaryService.Instance.GetIsMultiModuleMode && SelectedModuleInfo != ModuleInfoSource.FirstOrDefault())
                //        {
                //            for (int i = 0; i < dicOfCode[StrCodePre].Count; i++)
                //            {
                //                listtmp.AddRange(AllAlarmInfos.Where(o => o.ModuleType == selectedModuleInfo.ModuleTypeCode && o.Code == StrCodePre + "-" + dicOfCode[StrCodePre][i]));
                //            }
                //        }
                //        else
                //        {
                //            for (int i = 0; i < dicOfCode[StrCodePre].Count; i++)
                //            {
                //                listtmp.AddRange(AllAlarmInfos.Where(o => o.Code == StrCodePre + "-" + dicOfCode[StrCodePre][i]));
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    if (DataDictionaryService.Instance.GetIsMultiModuleMode && SelectedModuleInfo != ModuleInfoSource.FirstOrDefault())
                //        listtmp.AddRange(AllAlarmInfos.Where(o => o.ModuleType == selectedModuleInfo.ModuleTypeCode));
                //    else
                //        listtmp.AddRange(AllAlarmInfos);
                //}
                #endregion
                if (DataDictionaryService.Instance.GetIsMultiModuleMode && SelectedModuleInfo != ModuleInfoSource.FirstOrDefault())
                    listtmp.AddRange(AllAlarmInfos.Where(o => o.ModuleType == selectedModuleInfo.ModuleTypeCode && o.Code.Contains(StrCode)));
                else
                    listtmp.AddRange(AllAlarmInfos.Where(o => o.Code.Contains(StrCode)));
            }
            else
            {
                if (DataDictionaryService.Instance.GetIsMultiModuleMode && SelectedModuleInfo != ModuleInfoSource.FirstOrDefault())
                    listtmp.AddRange(AllAlarmInfos.Where(o => o.ModuleType == selectedModuleInfo.ModuleTypeCode));
                else
                    listtmp.AddRange(AllAlarmInfos);
            }

            GetAlarmIns = listtmp;
        }
        /// <summary>
        /// 恢复默认显示标志设置
        /// </summary>
        private void SetEnabledMehod(bool isReset)
        {
            if (isReset)
            {
                WarningLevelVisibleUse = AllAlarmInfos.Where(p => p.CodeLevel == AlarmLevelEnum.Caution && p.CodeVisibility == false).Count() == 0 ? true : false;
                SampleStopLevelVisibleUse = AllAlarmInfos.Where(p => p.CodeLevel == AlarmLevelEnum.SampleAdding && p.CodeVisibility == false).Count() == 0 ? true : false;
                StopLevelVisibleUse = AllAlarmInfos.Where(p => p.CodeLevel == AlarmLevelEnum.Stop && p.CodeVisibility == false).Count() == 0 ? true : false;
            }
            else
            {
                WarningLevelVisibleUse = AllAlarmInfos.Where(p => p.CodeLevel == AlarmLevelEnum.Caution && p.CodeVisibility == true).Count() > 0 ? true : false;
                SampleStopLevelVisibleUse = AllAlarmInfos.Where(p => p.CodeLevel == AlarmLevelEnum.SampleAdding && p.CodeVisibility == true).Count() > 0 ? true : false;
                StopLevelVisibleUse = AllAlarmInfos.Where(p => p.CodeLevel == AlarmLevelEnum.Stop && p.CodeVisibility == true).Count() > 0 ? true : false;
            }
        }
        #endregion
    }
}
