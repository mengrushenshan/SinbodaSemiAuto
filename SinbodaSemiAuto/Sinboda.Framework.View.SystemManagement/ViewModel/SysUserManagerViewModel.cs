using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Business.SystemManagement;
using Sinboda.Framework.Common.CommonFunc;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.View.SystemManagement.Win;

namespace Sinboda.Framework.View.SystemManagement.ViewModel
{
    /// <summary>
    /// 权限设置界面逻辑实现类
    /// </summary>
    public class SysUserManagerViewModel : NavigationViewModelBase
    {
        /// <summary>
        /// 用户管理业务逻辑接口
        /// </summary>
        PermissionManagerBusiness pBusiness = new PermissionManagerBusiness();
        /// <summary>
        /// 
        /// </summary>
        ManagementBusiness mBusiness = new ManagementBusiness();
        /// <summary>
        /// 当前用户
        /// </summary>
        string LogUser;
        /// <summary>
        /// 当前角色
        /// </summary>
        private string currentUserRoleID = string.Empty;
        /// <summary>
        /// 构造函数
        /// </summary>
        public SysUserManagerViewModel()
        {
            PageTitle = SystemResources.Instance.LanguageArray[1955];
            LogUser = SystemResources.Instance.CurrentUserName;
            OperationResult<string> operationResult = pBusiness.GetRoleIDByUserID(SystemResources.Instance.CurrentUserName);
            if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                currentUserRoleID = operationResult.Results;

            InitializeRoleCommand();
            InitializeModuleCommand();
            InitializeSysTypeCommand();
            InitializeSysDataCommand();
            InitializeModuleTypeCommand();
            InitializeModuleInfoCommand();
        }

        #region 公共部分
        private object _TabSelectedValue;
        /// <summary>
        /// 选中的tab页
        /// </summary>
        public object TabSelectedValue
        {
            get { return _TabSelectedValue; }
            set
            {
                _TabSelectedValue = value;
                RaisePropertyChanged("TabSelectedValue");
                switch (((TabItem)TabSelectedValue).Name)
                {
                    case "tabRole":
                        GetRoleList();
                        break;
                    case "tabModule":
                        GetModuleList();
                        ModuleTypeColletion.Clear();
                        var itemdicnull = new DataDictionaryInfoModel()
                        {
                            Code = "0",
                            Values = SystemResources.Instance.LanguageArray[6373],//非模块菜单
                        };
                        ModuleTypeColletion.Add(itemdicnull);
                        GetModuleTypeList();
                        foreach (var item in ModuleTypeList)
                        {
                            var itemdic = new DataDictionaryInfoModel()
                            {
                                Code = item.ModuleTypeCode.ToString(),
                                Values = item.ModuleTypeName,
                            };
                            ModuleTypeColletion.Add(itemdic);
                        }
                        break;
                    case "tabModuleInfo":
                        GetModuleTypeList();
                        break;
                    case "tabSysDataDictionary":
                        GetSysDataDictionaryType();
                        if (_SelectSysDataDictionaryType != null && !string.IsNullOrEmpty(_SelectSysDataDictionaryType.Code))
                            GetSysDataDictionaryInfo(_SelectSysDataDictionaryType.Code);
                        break;
                    default:
                        GetRoleList();
                        break;
                }
            }
        }
        /// <summary>
        /// 角色列表
        /// </summary>
        ObservableCollection<SysRoleModel> _RoleList = new ObservableCollection<SysRoleModel>();
        /// <summary>
        /// 角色列表属性
        /// </summary>
        public ObservableCollection<SysRoleModel> RoleList
        {
            get { return _RoleList; }
            set { Set(ref _RoleList, value); }
        }
        #endregion

        #region 角色设置
        #region 属性
        private string _RoleID;
        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleID
        {
            get { return _RoleID; }
            set { Set(ref _RoleID, value); }
        }
        private string _RoleDescription;
        /// <summary>
        /// 角色描述
        /// </summary>
        public string RoleDescription
        {
            get { return _RoleDescription; }
            set { Set(ref _RoleDescription, value); }
        }
        private string _RoleLanguageID;
        /// <summary>
        /// 角色描述语言编码
        /// </summary>
        public string RoleLanguageID
        {
            get { return _RoleLanguageID; }
            set { Set(ref _RoleLanguageID, value); }
        }
        private string _Level;
        /// <summary>
        /// 角色所在级别
        /// </summary>
        public string Level
        {
            get { return _Level; }
            set { Set(ref _Level, value); }
        }
        private bool _RoleIDEnabled = true;
        /// <summary>
        /// 角色编码可用不可用
        /// </summary>
        public bool RoleIDEnabled
        {
            get { return _RoleIDEnabled; }
            set { Set(ref _RoleIDEnabled, value); }
        }
        /// <summary>
        /// 角色列表选中项
        /// </summary>
        SysRoleModel _SelectRole;
        /// <summary>
        /// 角色列表选中项属性
        /// </summary>
        public SysRoleModel SelectRole
        {
            get { return _SelectRole; }
            set
            {
                Set(ref _SelectRole, value);
                if (SelectRole != null)
                {
                    RoleID = SelectRole.RoleID;
                    RoleDescription = SelectRole.Description;
                    RoleLanguageID = SelectRole.LangID.ToString();
                    Level = SelectRole.Level.ToString();
                    RoleIDEnabled = false;
                }
                else
                {
                    RoleID = string.Empty;
                    RoleDescription = string.Empty;
                    RoleLanguageID = string.Empty;
                    Level = string.Empty;
                    RoleIDEnabled = true;
                }
            }
        }
        #endregion

        #region 命令
        private void InitializeRoleCommand()
        {
            AddRoleCommand = new RelayCommand(AddRoleMethod);
            ModifyRoleCommand = new RelayCommand(ModifyRoleMethod);
            DelRoleCommand = new RelayCommand(DelRoleMethod);
            RoleClearCommand = new RelayCommand(RoleClearMethod);
        }
        /// <summary>
        /// 添加角色命令
        /// </summary>
        public RelayCommand AddRoleCommand { get; set; }
        private void AddRoleMethod()
        {
            if (ValidateRole())
            {
                OperationResult<PermissionErrorCode> operationResult = pBusiness.AddRole(RoleID, RoleDescription, StringParseHelper.ParseByDefault(RoleLanguageID, 0), StringParseHelper.ParseByDefault(Level, 0));
                PermissionErrorCode result = new PermissionErrorCode();
                if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                    result = operationResult.Results;
                else
                {
                    ShowMessageError(operationResult.Message);
                    return;
                }
                if (result == PermissionErrorCode.OK)
                {
                    GetRoleList();
                    SelectRole = null;
                    ShowMessageComplete(SystemResources.Instance.LanguageArray[6383]);//"添加角色成功！");                
                }
                else if (result == PermissionErrorCode.RoleExist)
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6384]);//"角色已存在，添加失败！");
                }
                else
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6385]);//"发生其他错误，添加失败！");
                }
            }
        }
        /// <summary>
        /// 修改角色命令
        /// </summary>
        public RelayCommand ModifyRoleCommand { get; set; }
        private void ModifyRoleMethod()
        {
            if (ValidateRole())
            {
                if (SelectRole == null)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！"); //TODO 翻译
                    return;
                }

                OperationResult<PermissionErrorCode> operationResult = pBusiness.ModifyRole(RoleID, RoleDescription, StringParseHelper.ParseByDefault(RoleLanguageID, 0), StringParseHelper.ParseByDefault(Level, 0));
                PermissionErrorCode result = new PermissionErrorCode();
                if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                    result = operationResult.Results;
                else
                {
                    ShowMessageError(operationResult.Message);//SystemResources.Instance.LanguageArray[]);
                    return;
                }
                if (result == PermissionErrorCode.OK)
                {
                    GetRoleList();
                    ShowMessageComplete(SystemResources.Instance.LanguageArray[6387]); //"更新角色成功！
                }
                else if (result == PermissionErrorCode.RoleNotExist)
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6388]); //"角色不存在，更新失败
                }
                else
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6389]); //"发生其他错误，更新失败！");
                }
            }
        }
        /// <summary>
        /// 删除角色命令
        /// </summary>
        public RelayCommand DelRoleCommand { get; set; }
        private void DelRoleMethod()
        {

            if (NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[2197], SystemResources.Instance.LanguageArray[351], MessageBoxButton.YesNo, SinMessageBoxImage.Question, false) == MessageBoxResult.No)//"提示", "您确认要删除选中的项目吗?"
                return;

            if (ValidateRole())
            {
                if (SelectRole == null)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！")
                    return;
                }

                OperationResult<PermissionErrorCode> operationResult = pBusiness.DelRole(RoleID);
                PermissionErrorCode result = new PermissionErrorCode();
                if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                    result = operationResult.Results;
                else
                {
                    ShowMessageError(operationResult.Message);//SystemResources.Instance.LanguageArray[]);
                    return;
                }
                if (result == PermissionErrorCode.OK)
                {
                    GetRoleList();
                    SelectRole = null;
                    ShowMessageComplete(SystemResources.Instance.LanguageArray[754]);
                }
                else if (result == PermissionErrorCode.RoleNotExist)
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6390]); //"角色不存在，删除失败！");
                }
                else
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6391]); //"发生其他错误，删除失败！");
                }
            }
        }
        /// <summary>
        /// 清除输入信息命令
        /// </summary>
        public RelayCommand RoleClearCommand { get; set; }
        private void RoleClearMethod()
        {
            if (NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[2197], SystemResources.Instance.LanguageArray[6392], MessageBoxButton.YesNo, SinMessageBoxImage.Question) == MessageBoxResult.No)
                return;
            SelectRole = null;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 校验角色方法
        /// </summary>
        /// <returns></returns>
        private bool ValidateRole()
        {
            if (string.IsNullOrEmpty(RoleID))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6393]);//"角色编码不能为空！
                return false;
            }

            if (string.IsNullOrEmpty(RoleDescription))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6394]); //"描述不能为空！"
                return false;
            }

            if (string.IsNullOrEmpty(RoleLanguageID))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6395]); //"语言编码不能为空！"
                return false;
            }

            if (!DataValidateHelper.IsComposeOnlyByNum(RoleLanguageID))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6396]); //"语言编码只能为整数数字！");
                return false;
            }

            if (string.IsNullOrEmpty(Level))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6397]); //"级别不能为空！"
                return false;
            }
            if (!DataValidateHelper.IsComposeOnlyByNum(Level))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6398]); //"级别只能为整数数字！")
                return false;
            }
            if (StringParseHelper.ParseByDefault(Level, 0) <= 0)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6399]); //"级别需大于0！
                return false;
            }

            return true;
        }
        /// <summary>
        /// 获取角色列表数据
        /// </summary>
        private void GetRoleList()
        {
            List<SysRoleModel> list = new List<SysRoleModel>();
            OperationResult<List<SysRoleModel>> operationResult = pBusiness.GetRoleList();
            if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                list = operationResult.Results;
            else
                ShowMessageError(operationResult.Message);//3903角色设置
            RoleList = new ObservableCollection<SysRoleModel>(list);
        }
        #endregion
        #endregion

        #region 业务模块设置
        #region 属性
        private string _ModuleID;
        /// <summary>
        /// 模块编码
        /// </summary>
        public string ModuleID
        {
            get { return _ModuleID; }
            set { Set(ref _ModuleID, value); }
        }
        private string _ModuleIDKey;
        /// <summary>
        /// 唯一标识码
        /// </summary>
        public string ModuleIDKey
        {
            get { return _ModuleIDKey; }
            set { Set(ref _ModuleIDKey, value); }
        }
        private string _ModuleDescription;
        /// <summary>
        /// 模块描述
        /// </summary>
        public string ModuleDescription
        {
            get { return _ModuleDescription; }
            set { Set(ref _ModuleDescription, value); }
        }
        private string _ModuleLanguageID;
        /// <summary>
        /// 模块语言编码
        /// </summary>
        public string ModuleLanguageID
        {
            get { return _ModuleLanguageID; }
            set { Set(ref _ModuleLanguageID, value); }
        }
        private List<DataDictionaryInfoModel> _ParentDicList = new List<DataDictionaryInfoModel>();
        /// <summary>
        /// 上级模块列表
        /// </summary>
        public List<DataDictionaryInfoModel> ParentDicList
        {
            get { return _ParentDicList; }
            set { Set(ref _ParentDicList, value); }
        }
        private DataDictionaryInfoModel _ParentDic;
        /// <summary>
        /// 选中的上级模块
        /// </summary>
        public DataDictionaryInfoModel ParentDic
        {
            get { return _ParentDic; }
            set
            {
                Set(ref _ParentDic, value);
                SysModuleModel model = ModuleList.Where(p => p.ParentID == ParentDic.Values).OrderByDescending(p => p.ShowOrder).FirstOrDefault();
                int count = model == null ? 0 : model.ShowOrder;
                ShowOrder = (count + 1).ToString();
            }
        }
        private string _DllName;
        /// <summary>
        /// 动态库名称
        /// </summary>
        public string DllName
        {
            get { return _DllName; }
            set { Set(ref _DllName, value); }
        }
        private string _NameSpace;
        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace
        {
            get { return _NameSpace; }
            set { Set(ref _NameSpace, value); }
        }
        private string _ShowOrder;
        /// <summary>
        /// 显示顺序
        /// </summary>
        public string ShowOrder
        {
            get { return _ShowOrder; }
            set { Set(ref _ShowOrder, value); }
        }
        private string _IconCommon;
        /// <summary>
        /// 正常时图标
        /// </summary>
        public string IconCommon
        {
            get { return _IconCommon; }
            set { Set(ref _IconCommon, value); }
        }
        private List<DataDictionaryInfoModel> _ModuleTypeCollection = new List<DataDictionaryInfoModel>();
        /// <summary>
        /// 菜单类别集合
        /// </summary>
        public List<DataDictionaryInfoModel> ModuleTypeColletion
        {
            get { return _ModuleTypeCollection; }
            set { Set(ref _ModuleTypeCollection, value); }
        }
        private DataDictionaryInfoModel _ModuleType;
        /// <summary>
        /// 菜单类别
        /// </summary>
        public DataDictionaryInfoModel ModuleType
        {
            get { return _ModuleType; }
            set { Set(ref _ModuleType, value); }
        }
        private bool _IsMenuShow = true;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsMenuShow
        {
            get { return _IsMenuShow; }
            set { Set(ref _IsMenuShow, value); }
        }
        private bool _IsDisplayEnable = false;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsDisplayEnable
        {
            get { return _IsDisplayEnable; }
            set { Set(ref _IsDisplayEnable, value); }
        }
        private ObservableCollection<SysModuleModel> _ModuleList = new ObservableCollection<SysModuleModel>();
        /// <summary>
        /// 模块列表
        /// </summary>
        public ObservableCollection<SysModuleModel> ModuleList
        {
            get { return _ModuleList; }
            set
            {
                Set(ref _ModuleList, value);
                ParentDicList.Clear();
                var itemdicnull = new DataDictionaryInfoModel()
                {
                    Code = "",
                    Values = "",
                };
                ParentDicList.Add(itemdicnull);
                foreach (var item in ModuleList)
                {
                    var itemdic = new DataDictionaryInfoModel()
                    {
                        Code = item.ModuleIDKey,
                        Values = item.Description,
                    };
                    ParentDicList.Add(itemdic);
                }
            }
        }
        private SysModuleModel _SelectModule;
        /// <summary>
        /// 选中的模块
        /// </summary>
        public SysModuleModel SelectModule
        {
            get { return _SelectModule; }
            set
            {
                Set(ref _SelectModule, value);

                if (SelectModule != null)
                {
                    ModuleID = SelectModule.ModuleID;
                    ModuleIDKey = SelectModule.ModuleIDKey;
                    ModuleDescription = SelectModule.Description;
                    ModuleLanguageID = SelectModule.LangID.ToString();
                    ParentDic = ParentDicList.Where(o => o.Code == SelectModule.ParentID).FirstOrDefault();
                    DllName = SelectModule.DllName;
                    NameSpace = SelectModule.NameSpace;
                    ShowOrder = SelectModule.ShowOrder.ToString();
                    IconCommon = SelectModule.IconCommon;
                    IsMenuShow = SelectModule.IsMenuShow;
                    IsDisplayEnable = SelectModule.IsDisplayEnable;
                    if (SelectModule.ModuleType != 0)
                    {
                        ModuleType = ModuleTypeColletion.Where(p => p.Code == SelectModule.ModuleType.ToString()).FirstOrDefault();
                    }
                    else
                        ModuleType = ModuleTypeColletion.FirstOrDefault();
                }
                else
                {
                    ModuleID = string.Empty;
                    ModuleIDKey = string.Empty;
                    ModuleDescription = string.Empty;
                    ModuleLanguageID = string.Empty;
                    ParentDic = ParentDicList.Where(o => o.Code == "").FirstOrDefault(); ;
                    DllName = string.Empty;
                    NameSpace = string.Empty;
                    ShowOrder = string.Empty;
                    IconCommon = string.Empty;
                    IsMenuShow = true;
                    IsDisplayEnable = false;
                    ModuleType = ModuleTypeColletion.FirstOrDefault();
                }
            }
        }
        #endregion

        #region 命令
        private void InitializeModuleCommand()
        {
            AddModuleCommand = new RelayCommand(AddModuleMethod);
            ModifyModuleCommand = new RelayCommand(ModifyModuleMethod);
            DelModuleCommand = new RelayCommand(DelModuleMethod);
            ModuleClearCommand = new RelayCommand(ModuleClearMethod);

            TopwardCommand = new RelayCommand(TopwardMethod);
            BottomwardCommand = new RelayCommand(BottomwardMethod);
            BeforewardCommand = new RelayCommand(BeforewardMethod);
            NextwardCommand = new RelayCommand(NextwardMethod);
        }
        /// <summary>
        /// 添加模块命令
        /// </summary>
        public RelayCommand AddModuleCommand { get; set; }
        private void AddModuleMethod()
        {
            if (ValidateModule(true, true))
            {
                int moduleTypeID = 0;
                if (ModuleTypeColletion.FirstOrDefault() == ModuleType)
                    moduleTypeID = 0;
                else if (ModuleTypeList.Count > 0)
                    moduleTypeID = ModuleTypeList.Where(p => p.ModuleTypeName == ModuleType.Values).FirstOrDefault().ModuleTypeCode;
                OperationResult<PermissionErrorCode> operationResult = pBusiness.AddModule(new SysModuleModel(ModuleID, ModuleIDKey, ParentDic.Code, ModuleDescription, StringParseHelper.ParseByDefault(ModuleLanguageID, 0),
                                                DllName, NameSpace, StringParseHelper.ParseByDefault(ShowOrder, 0), IconCommon, false, IsMenuShow, moduleTypeID, IsDisplayEnable));
                PermissionErrorCode result = new PermissionErrorCode();
                if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                    result = operationResult.Results;
                else
                {
                    ShowMessageError(operationResult.Message);//1656模块设置
                    return;
                }
                if (result == PermissionErrorCode.OK)
                {
                    GetModuleList();
                    SelectModule = null;
                    ShowMessageComplete(SystemResources.Instance.LanguageArray[429]);//添加数据成功
                }
                else if (result == PermissionErrorCode.ModuleExist)
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6400]); //"数据已存在，添加失败！
                }
                else
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6385]); //"发生其他错误，添加失败！
                }
            }
        }
        /// <summary>
        /// 更新模块命令
        /// </summary>
        public RelayCommand ModifyModuleCommand { get; set; }
        private void ModifyModuleMethod()
        {
            bool isCheckModuleIDKey = false;
            bool isCheckShowOrder = false;
            if (SelectModule == null)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作
                return;
            }
            if (ModuleIDKey != SelectModule.ModuleIDKey)
                isCheckModuleIDKey = true;
            if (ShowOrder != SelectModule.ShowOrder.ToString())
                isCheckShowOrder = true;
            if (ValidateModule(isCheckModuleIDKey, isCheckShowOrder))
            {
                int moduleTypeID = 0;
                if (ModuleTypeColletion.FirstOrDefault() == ModuleType)
                    moduleTypeID = 0;
                else if (ModuleType != ModuleTypeColletion.FirstOrDefault() && ModuleTypeList.Count > 0)
                    moduleTypeID = ModuleTypeList.Where(p => p.ModuleTypeName == ModuleType.Values).FirstOrDefault().ModuleTypeCode;
                OperationResult<PermissionErrorCode> operationResult = pBusiness.ModifyModule(SelectModule.ModuleIDKey, new SysModuleModel(ModuleID, ModuleIDKey, ParentDic.Code, ModuleDescription, StringParseHelper.ParseByDefault(ModuleLanguageID, 0),
                                                    DllName, NameSpace, StringParseHelper.ParseByDefault(ShowOrder, 0), IconCommon, false, IsMenuShow, moduleTypeID, IsDisplayEnable));
                PermissionErrorCode result = new PermissionErrorCode();
                if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                    result = operationResult.Results;
                else
                {
                    ShowMessageError(operationResult.Message);//1656模块设置
                    return;
                }
                if (result == PermissionErrorCode.OK)
                {
                    GetModuleList();
                    ShowMessageComplete(SystemResources.Instance.LanguageArray[6401]); //"更新成功！
                }
                else if (result == PermissionErrorCode.ModuleNotExist)
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6402]); //"数据已存在，更新失败")
                }
                else
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6389]); //"发生其他错误，更新失败
                }
            }
        }
        /// <summary>
        /// 删除模块命令
        /// </summary>
        public RelayCommand DelModuleCommand { get; set; }
        private void DelModuleMethod()
        {
            if (SelectModule == null)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]);//"未选择任何数据，无法执行操作！
                return;
            }

            if (NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[2197], SystemResources.Instance.LanguageArray[351], MessageBoxButton.YesNo, SinMessageBoxImage.Question) == MessageBoxResult.No)//"提示", "您确认要删除选中的项目吗?"
                return;

            if (string.IsNullOrEmpty(ModuleIDKey))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6403]);//"资源编码不能为空！
                return;
            }

            OperationResult<PermissionErrorCode> operationResult = pBusiness.DelModule(ModuleIDKey);
            PermissionErrorCode result = new PermissionErrorCode();
            if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                result = operationResult.Results;
            else
            {
                ShowMessageError(operationResult.Message);//1656模块设置
                return;
            }
            if (result == PermissionErrorCode.OK)
            {
                GetModuleList();
                SelectModule = null;
                ShowMessageComplete(SystemResources.Instance.LanguageArray[754]); //"删除成功！
            }
            else if (result == PermissionErrorCode.ModuleNotExist)
            {
                ShowMessageError(SystemResources.Instance.LanguageArray[6404]); //"模块不存在，删除失败
            }
            else
            {
                ShowMessageError(SystemResources.Instance.LanguageArray[6391]); //"发生其他错误，删除失败！
            }
        }
        /// <summary>
        /// 清除输入信息命令
        /// </summary>
        public RelayCommand ModuleClearCommand { get; set; }
        private void ModuleClearMethod()
        {
            if (NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[2197], SystemResources.Instance.LanguageArray[6392], MessageBoxButton.YesNo, SinMessageBoxImage.Question) == MessageBoxResult.No)
                return;
            SelectModule = null;
        }

        /// <summary>
        /// 置顶命令
        /// </summary>
        public RelayCommand TopwardCommand { get; set; }
        private void TopwardMethod()
        {
            if (SelectModule != null)
            {
                if (SelectModule.ShowOrder != 0)
                {
                    if (SelectModule.ShowOrder != 1)
                    {
                        string parentID = SelectModule.ParentID;
                        List<SysModuleModel> moduleListTmp = ModuleList.Where(p => p.ParentID == parentID && p.ModuleIDKey != SelectModule.ModuleIDKey).OrderBy(p => p.ShowOrder).ToList();
                        OperationResult result = pBusiness.HandlerModuleShowOrderTop(SelectModule.ModuleIDKey, moduleListTmp);
                        if (result.ResultEnum == OperationResultEnum.FAILED)
                            ShowMessageWarning(SystemResources.Instance.LanguageArray[6405]);//"置顶失败")
                        else
                            GetModuleList();
                    }
                    else
                    {
                        //ShowMessageWarning("不可以置顶"); //TODO 翻译
                        return;
                    }
                }
                else
                {
                    //ShowMessageWarning("不可以置顶"); //TODO 翻译
                    return;
                }
            }
            else
            {
                //ShowMessageWarning("不可以置顶"); //TODO 翻译
                return;
            }
        }
        /// <summary>
        /// 置底命令
        /// </summary>
        public RelayCommand BottomwardCommand { get; set; }
        private void BottomwardMethod()
        {
            if (SelectModule != null)
            {
                if (SelectModule.ShowOrder != 0)
                {
                    if (SelectModule.ShowOrder != ModuleList.Where(p => p.ParentID == SelectModule.ParentID).Count())
                    {
                        string parentID = SelectModule.ParentID;
                        List<SysModuleModel> moduleListTmp = ModuleList.Where(p => p.ParentID == parentID && p.ModuleIDKey != SelectModule.ModuleIDKey).OrderBy(p => p.ShowOrder).ToList();
                        OperationResult result = pBusiness.HandlerModuleShowOrderBottom(SelectModule.ModuleIDKey, moduleListTmp);
                        if (result.ResultEnum == OperationResultEnum.FAILED)
                            ShowMessageWarning(SystemResources.Instance.LanguageArray[6406]); //"置底失败"); 
                        else
                            GetModuleList();
                    }
                    else
                    {
                        //ShowMessageWarning("不可以置底"); //TODO 翻译
                        return;
                    }
                }
                else
                {
                    //ShowMessageWarning("不可以置底"); //TODO 翻译
                    return;
                }
            }
            else
            {
                //ShowMessageWarning("不可以置底"); //TODO 翻译
                return;
            }
        }
        /// <summary>
        /// 向上命令
        /// </summary>
        public RelayCommand BeforewardCommand { get; set; }
        private void BeforewardMethod()
        {
            if (SelectModule != null)
            {
                if (SelectModule.ShowOrder != 0)
                {
                    if (SelectModule.ShowOrder != 1)
                    {
                        string parentID = SelectModule.ParentID;
                        SysModuleModel moduleTmp = ModuleList.Where(p => p.ParentID == parentID && p.ShowOrder == SelectModule.ShowOrder - 1).FirstOrDefault();
                        OperationResult result = pBusiness.HandlerModuleShowOrderBefore(SelectModule, moduleTmp);
                        if (result.ResultEnum == OperationResultEnum.FAILED)
                            ShowMessageWarning(SystemResources.Instance.LanguageArray[6407]); //"上移失败");
                        else
                            GetModuleList();
                    }
                    else
                    {
                        //ShowMessageWarning("不可以向上"); //TODO 翻译
                        return;
                    }
                }
                else
                {
                    //ShowMessageWarning("不可以向上"); //TODO 翻译
                    return;
                }
            }
            else
            {
                //ShowMessageWarning("不可以向上"); //TODO 翻译
                return;
            }
        }
        /// <summary>
        /// 向下命令
        /// </summary>
        public RelayCommand NextwardCommand { get; set; }
        private void NextwardMethod()
        {
            if (SelectModule != null)
            {
                if (SelectModule.ShowOrder != 0)
                {
                    if (SelectModule.ShowOrder != ModuleList.Where(p => p.ParentID == SelectModule.ParentID).Count())
                    {
                        string parentID = SelectModule.ParentID;
                        SysModuleModel moduleTmp = ModuleList.Where(p => p.ParentID == parentID && p.ShowOrder == SelectModule.ShowOrder + 1).FirstOrDefault();
                        OperationResult result = pBusiness.HandlerModuleShowOrderNext(SelectModule, moduleTmp);
                        if (result.ResultEnum == OperationResultEnum.FAILED)
                            ShowMessageWarning(SystemResources.Instance.LanguageArray[6408]); //"下移失败");
                        else
                            GetModuleList();
                    }
                    else
                    {
                        //ShowMessageWarning("不可以向下"); //TODO 翻译
                        return;
                    }
                }
                else
                {
                    //ShowMessageWarning("不可以向下"); //TODO 翻译
                    return;
                }
            }
            else
            {
                //ShowMessageWarning("不可以向下"); //TODO 翻译
                return;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 校验方法
        /// </summary>
        /// <returns></returns>
        private bool ValidateModule(bool isCheckModuleIDKey, bool isCheckShowOrder)
        {
            //if (string.IsNullOrEmpty(ModuleID))
            //{
            //    ShowMessageWarning("模块编码不能为空！");//1656模块设置 //TODO 翻译
            //    return false;
            //}

            if (string.IsNullOrEmpty(ModuleIDKey))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6403]); //"资源编码不能为空
                return false;
            }

            if (isCheckModuleIDKey)
            {
                OperationResult operationResultKey = pBusiness.ExistModuleIDKey(ModuleIDKey);
                bool keyResult = false;
                if (operationResultKey.ResultEnum == OperationResultEnum.SUCCEED)
                    keyResult = true;
                if (keyResult)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6409]); //"资源编码已存在！
                    return false;
                }
            }

            if (string.IsNullOrEmpty(ModuleDescription))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6410]); //"资源名称不能为空！
                return false;
            }

            if (string.IsNullOrEmpty(ModuleLanguageID))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6395]); //"语言编号不能为空！
                return false;
            }

            if (!DataValidateHelper.IsComposeOnlyByNum(ModuleLanguageID))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6396]); //"语言编号只能为整数数字！");//
                return false;
            }

            //if (string.IsNullOrEmpty(DllName))
            //{
            //    ShowMessageWarning("动态库名称不能为空！");//1656模块设置 //TODO 翻译
            //    return false;
            //}

            //if (string.IsNullOrEmpty(NameSpace))
            //{
            //    ShowMessageWarning("命名空间不能为空！");//1656模块设置 //TODO 翻译
            //    return false;
            //}

            if (string.IsNullOrEmpty(ShowOrder))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6411]); //"显示顺序不能为空！
                return false;
            }

            if (!DataValidateHelper.IsComposeOnlyByNum(ShowOrder))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6412]); //"显示顺序只能为整数数字！");
                return false;
            }

            if (isCheckShowOrder)
            {
                bool showOrderResult = false;
                string ParentID = ParentDic == null ? "" : ParentDic.Code;
                OperationResult operationResultShowOrder = pBusiness.ExistSameModuleShowOrder(ParentID, StringParseHelper.ParseByDefault(ShowOrder, 0));
                if (operationResultShowOrder.ResultEnum == OperationResultEnum.SUCCEED)
                    showOrderResult = true;
                if (!string.IsNullOrEmpty(ParentID) && showOrderResult)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6413]); //"显示顺序已存在请重新填写
                    return false;
                }
            }

            //if (string.IsNullOrEmpty(IconCommon))
            //{
            //    ShowMessageWarning("正常时图标不能为空！");//1656模块设置 //TODO 翻译
            //    return false;
            //}

            return true;
        }
        /// <summary>
        /// 获取当前模块列表信息
        /// </summary>
        private void GetModuleList()
        {
            List<SysModuleModel> list = new List<SysModuleModel>();
            OperationResult<List<SysModuleModel>> operationResult = pBusiness.GetModuleList();
            if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                list = operationResult.Results;
            else
                ShowMessageError(operationResult.Message);//1656模块设置
            ModuleList = new ObservableCollection<SysModuleModel>(list);
        }
        #endregion
        #endregion

        #region 模块设置
        #region 模块类型设置
        #region 属性
        private ObservableCollection<ModuleTypeModel> _ModuleTypeList = new ObservableCollection<ModuleTypeModel>();
        /// <summary>
        /// 模块类型列表
        /// </summary>
        public ObservableCollection<ModuleTypeModel> ModuleTypeList
        {
            get { return _ModuleTypeList; }
            set
            {
                Set(ref _ModuleTypeList, value);

                if (ModuleTypeList != null)
                    SelectModuleType = ModuleTypeList.FirstOrDefault();
                else
                    SelectModuleType = null;
            }
        }
        private ModuleTypeModel _SelectModuleType = new ModuleTypeModel();
        /// <summary>
        /// 模块类型列表选中项
        /// </summary>
        public ModuleTypeModel SelectModuleType
        {
            get { return _SelectModuleType; }
            set
            {
                Set(ref _SelectModuleType, value);

                if (SelectModuleType != null)
                {
                    ModuleTypeName = SelectModuleType.ModuleTypeName;
                    ModuleTypeCode = SelectModuleType.ModuleTypeCode;
                    GetModuleInfoList(SelectModuleType);
                }
                else
                {
                    ModuleTypeName = string.Empty;
                    ModuleTypeCode = 0;
                    ModuleInfoList = null;
                }
            }
        }
        private string _ModuleTypeName;
        /// <summary>
        /// 模块类型名称
        /// </summary>
        public string ModuleTypeName
        {
            get { return _ModuleTypeName; }
            set { Set(ref _ModuleTypeName, value); }
        }
        private int _ModuleTypeCode;
        /// <summary>
        /// 模块类型编码
        /// </summary>
        public int ModuleTypeCode
        {
            get { return _ModuleTypeCode; }
            set { Set(ref _ModuleTypeCode, value); }
        }
        #endregion

        #region 命令
        private void InitializeModuleTypeCommand()
        {
            AddModuleTypeCommand = new RelayCommand(AddModuleTypeMethod);
            InsertModuleTypeCommand = new RelayCommand(InsertModuleTypeMethod);
            UpdateModuleTypeCommand = new RelayCommand(UpdateModuleTypeMethod);
            DeleteModuleTypeCommand = new RelayCommand(DeleteModuleTypeMethod);
        }
        /// <summary>
        /// 打开模块类型添加窗口
        /// </summary>
        public RelayCommand AddModuleTypeCommand { get; set; }
        /// <summary>
        /// 添加类型方法
        /// </summary>
        private void AddModuleTypeMethod()
        {
            ModuleTypeSetWin win = new ModuleTypeSetWin();
            win.DataContext = this;
            win.ShowDialog();
            GetModuleTypeList();
            foreach (var item in ModuleTypeList)
            {
                var itemdic = new DataDictionaryInfoModel()
                {
                    Code = item.ModuleTypeCode.ToString(),
                    Values = item.ModuleTypeName,
                };
                ModuleTypeColletion.Add(itemdic);
            }
        }
        /// <summary>
        /// 添加命令
        /// </summary>
        public RelayCommand InsertModuleTypeCommand { get; set; }
        /// <summary>
        /// 添加方法
        /// </summary>
        private void InsertModuleTypeMethod()
        {
            if (ValidationModuleType())
            {
                if (ModuleTypeList.Where(p => p.ModuleTypeName == ModuleTypeName).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6414]); //"模块类型名称已经存在"); 
                    return;
                }
                if (ModuleTypeList.Where(p => p.ModuleTypeCode == ModuleTypeCode).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6415]); //"模块类型编码已经存在"
                    return;
                }
                ModuleTypeModel model = new ModuleTypeModel();
                model.ModuleTypeName = ModuleTypeName;
                model.ModuleTypeCode = ModuleTypeCode;
                OperationResult result = mBusiness.OperateT(model, Core.ModelsOperation.OperationEnum.Add);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                    GetModuleTypeList();
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }
        /// <summary>
        /// 修改命令
        /// </summary>
        public RelayCommand UpdateModuleTypeCommand { get; set; }
        /// <summary>
        /// 修改方法
        /// </summary>
        private void UpdateModuleTypeMethod()
        {
            if (ValidationModuleType())
            {
                if (SelectModuleType == null)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); ///"未选择任何数据，无法执行操作！
                    return;
                }
                if (ModuleTypeList.Where(p => p.ModuleTypeName == ModuleTypeName).Count() > 0 &&
                    ModuleTypeList.Where(p => p.ModuleTypeName == ModuleTypeName).FirstOrDefault().Id != SelectModuleType.Id)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6414]); //"模块类型名称已经存在"); 
                    return;
                }
                if (ModuleTypeList.Where(p => p.ModuleTypeCode == ModuleTypeCode).Count() > 0 &&
                    ModuleTypeList.Where(p => p.ModuleTypeCode == ModuleTypeCode).FirstOrDefault().Id != SelectModuleType.Id)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6415]); //"模块类型编码已经存在
                    return;
                }
                ModuleTypeModel model = new ModuleTypeModel();
                model.Id = SelectModuleType.Id;
                model.ModuleTypeName = ModuleTypeName;
                model.ModuleTypeCode = ModuleTypeCode;
                OperationResult result = mBusiness.OperateT(model, Core.ModelsOperation.OperationEnum.Modify);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                    GetModuleTypeList();
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }
        /// <summary>
        /// 删除命令
        /// </summary>
        public RelayCommand DeleteModuleTypeCommand { get; set; }
        /// <summary>
        /// 删除方法
        /// </summary>
        private void DeleteModuleTypeMethod()
        {
            if (ValidationModuleType())
            {
                if (SelectModuleType == null)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); ///"未选择任何数据，无法执行操作！
                    return;
                }

                ModuleTypeModel model = new ModuleTypeModel();
                model.Id = SelectModuleType.Id;
                model.ModuleTypeName = ModuleTypeName;
                OperationResult result = mBusiness.OperateT(model, Core.ModelsOperation.OperationEnum.Delete);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                    GetModuleTypeList();
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 校验模块类型设置
        /// </summary>
        /// <returns></returns>
        private bool ValidationModuleType()
        {
            if (string.IsNullOrEmpty(ModuleTypeName))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6416]); //"模块类型名称不能为空！");
                return false;
            }
            if (ModuleTypeCode <= 0)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6417]); //"模块类型编码需大于0！
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取模块类型列表
        /// </summary>
        private void GetModuleTypeList()
        {
            OperationResult<List<ModuleTypeModel>> result = mBusiness.GetModuleTypeList();
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
            {
                DataDictionaryService.Instance.InitializeModuleInfoDictionary();
                ModuleTypeList = new ObservableCollection<ModuleTypeModel>(result.Results);
            }
            else
                ShowMessageError(result.Message);
        }
        #endregion
        #endregion

        #region 模块设置
        #region 属性
        private ObservableCollection<ModuleInfoModel> _ModuleInfoList = new ObservableCollection<ModuleInfoModel>();
        /// <summary>
        /// 模块列表
        /// </summary>
        public ObservableCollection<ModuleInfoModel> ModuleInfoList
        {
            get { return _ModuleInfoList; }
            set
            {
                Set(ref _ModuleInfoList, value);
                if (ModuleInfoList == null)
                    SelectModuleInfo = null;
            }
        }
        private ModuleInfoModel _SelectModuleInfo = new ModuleInfoModel();
        /// <summary>
        /// 模块列表选中项
        /// </summary>
        public ModuleInfoModel SelectModuleInfo
        {
            get { return _SelectModuleInfo; }
            set
            {
                Set(ref _SelectModuleInfo, value);

                if (SelectModuleInfo != null)
                {
                    ModuleInfoName = SelectModuleInfo.ModuleName;
                    ModuleInfoID = SelectModuleInfo.ModuleID;
                }
                else
                {
                    ModuleInfoName = string.Empty;
                    ModuleInfoID = 0;
                }
            }
        }
        private string _ModuleInfoName;
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleInfoName
        {
            get { return _ModuleInfoName; }
            set { Set(ref _ModuleInfoName, value); }
        }
        private int _ModuleInfoID;
        /// <summary>
        /// 模块编码
        /// </summary>
        public int ModuleInfoID
        {
            get { return _ModuleInfoID; }
            set { Set(ref _ModuleInfoID, value); }
        }
        #endregion

        #region 命令
        private void InitializeModuleInfoCommand()
        {
            InsertModuleInfoCommand = new RelayCommand(InsertModuleInfoMethod);
            UpdateModuleInfoCommand = new RelayCommand(UpdateModuleInfoMethod);
            DeleteModuleInfoCommand = new RelayCommand(DeleteModuleInfoMethod);
        }
        /// <summary>
        /// 添加命令
        /// </summary>
        public RelayCommand InsertModuleInfoCommand { get; set; }
        /// <summary>
        /// 添加方法
        /// </summary>
        private void InsertModuleInfoMethod()
        {
            if (ValidationModuleInfo())
            {
                if (ModuleInfoList.Where(p => p.ModuleName == ModuleInfoName).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6418]); //"模块名称已经存在
                    return;
                }
                if (ModuleInfoList.Where(p => p.ModuleID == ModuleInfoID).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6419]); //模块编码已经存在
                    return;
                }
                ModuleInfoModel model = new ModuleInfoModel();
                if (ModuleTypeList.Count == 0)
                    GetModuleTypeList();
                model.ModuleType = SelectModuleType.Id;
                model.ModuleName = ModuleInfoName;
                model.ModuleID = ModuleInfoID;
                OperationResult result = mBusiness.OperateT(model, Core.ModelsOperation.OperationEnum.Add);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    if (SelectModuleType != null)
                        GetModuleInfoList(SelectModuleType);
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }
        /// <summary>
        /// 修改命令
        /// </summary>
        public RelayCommand UpdateModuleInfoCommand { get; set; }
        /// <summary>
        /// 修改方法
        /// </summary>
        private void UpdateModuleInfoMethod()
        {
            if (ValidationModuleInfo())
            {
                if (SelectModuleInfo == null)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！"); 
                    return;
                }

                if (ModuleInfoList.Where(p => p.ModuleName == ModuleInfoName).Count() > 0 &&
                    ModuleInfoList.Where(p => p.ModuleName == ModuleInfoName).FirstOrDefault().Id != SelectModuleInfo.Id)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6418]); //"模块名称已经存在"); 
                    return;
                }

                if (ModuleInfoList.Where(p => p.ModuleID == ModuleInfoID).Count() > 0 &&
                    ModuleInfoList.Where(p => p.ModuleID == ModuleInfoID).FirstOrDefault().Id != SelectModuleInfo.Id)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6419]); //"模块编码已经存在"
                    return;
                }
                ModuleInfoModel model = new ModuleInfoModel();
                model.Id = SelectModuleInfo.Id;
                model.ModuleName = ModuleInfoName;
                model.ModuleID = ModuleInfoID;
                model.ModuleType = SelectModuleType.Id;
                OperationResult result = mBusiness.OperateT(model, Core.ModelsOperation.OperationEnum.Modify);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    if (SelectModuleType != null)
                        GetModuleInfoList(SelectModuleType);
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }
        /// <summary>
        /// 删除命令
        /// </summary>
        public RelayCommand DeleteModuleInfoCommand { get; set; }
        /// <summary>
        /// 删除方法
        /// </summary>
        private void DeleteModuleInfoMethod()
        {
            if (ValidationModuleInfo())
            {
                if (SelectModuleInfo == null)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]);//"未选择任何数据，无法执行操作！")
                    return;
                }

                ModuleInfoModel model = new ModuleInfoModel();
                model.Id = SelectModuleInfo.Id;
                model.ModuleName = ModuleInfoName;
                model.ModuleID = ModuleInfoID;
                OperationResult result = mBusiness.OperateT(model, Core.ModelsOperation.OperationEnum.Delete);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    if (SelectModuleType != null)
                        GetModuleInfoList(SelectModuleType);
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 校验模块设置
        /// </summary>
        /// <returns></returns>
        private bool ValidationModuleInfo()
        {
            if (string.IsNullOrEmpty(ModuleInfoName))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6416]); //"模块名称不能为空！"); 
                return false;
            }
            if (ModuleInfoID <= 0)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6417]); //"模块编码需大于0！！"); 
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取模块列表
        /// </summary>
        private void GetModuleInfoList(ModuleTypeModel moduleTypeID)
        {
            OperationResult<List<ModuleInfoModel>> result = mBusiness.GetModuleInfoList(moduleTypeID);
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
            {
                DataDictionaryService.Instance.InitializeModuleInfoDictionary();
                ModuleInfoList = new ObservableCollection<ModuleInfoModel>(result.Results);
            }
            else
                ShowMessageError(result.Message);
        }
        #endregion
        #endregion
        #endregion

        #region 系统字典设置
        #region 系统基础信息类型设置
        #region 属性
        private ObservableCollection<SystemTypeModel> _SysDataDictionaryTypeList = new ObservableCollection<SystemTypeModel>();
        /// <summary>
        ///基础信息类型列表
        /// </summary>
        public ObservableCollection<SystemTypeModel> SysDataDictionaryTypeList
        {
            get { return _SysDataDictionaryTypeList; }
            set { Set(ref _SysDataDictionaryTypeList, value); }
        }
        private SystemTypeModel _SelectSysDataDictionaryType = new SystemTypeModel();
        /// <summary>
        /// 基础信息类型表选中项
        /// </summary>
        public SystemTypeModel SelectSysDataDictionaryType
        {
            get { return _SelectSysDataDictionaryType; }
            set
            {
                Set(ref _SelectSysDataDictionaryType, value);

                if (_SelectSysDataDictionaryType != null)
                {
                    SysDataDicTypeCode = _SelectSysDataDictionaryType.Code;
                    SysDataDicTypeValue = _SelectSysDataDictionaryType.Values;
                    if (_SelectSysDataDictionaryType != null && !string.IsNullOrEmpty(_SelectSysDataDictionaryType.Code))
                        GetSysDataDictionaryInfo(_SelectSysDataDictionaryType.Code);
                }
                else
                {
                    SysDataDicTypeCode = string.Empty;
                    SysDataDicTypeValue = string.Empty;
                }
            }
        }
        private string _SysDataDicTypeCode;
        /// <summary>
        /// 基础信息类型编码
        /// </summary>
        public string SysDataDicTypeCode
        {
            get { return _SysDataDicTypeCode; }
            set { Set(ref _SysDataDicTypeCode, value); }
        }
        private string _SysDataDicTypeValue;
        /// <summary>
        /// 基础信息类型名称
        /// </summary>
        public string SysDataDicTypeValue
        {
            get { return _SysDataDicTypeValue; }
            set { Set(ref _SysDataDicTypeValue, value); }
        }
        #endregion

        #region 命令
        private void InitializeSysTypeCommand()
        {
            AddSysTypeCommand = new RelayCommand(AddSysTypeMethod);
            InsertSysDataDicTypeCommand = new RelayCommand(InsertSysDataDicTypeMethod);
            UpdateSysDataDicTypeCommand = new RelayCommand(UpdateSysDataDicTypeMethod);
            DeleteSysDataDicTypeCommand = new RelayCommand(DeleteSysDataDicTypeMethod);
        }

        /// <summary>
        /// 添加类型命令
        /// </summary>
        public RelayCommand AddSysTypeCommand { get; set; }
        /// <summary>
        /// 添加类型方法
        /// </summary>
        private void AddSysTypeMethod()
        {
            SysDataDictionayTypeSetWin win = new SysDataDictionayTypeSetWin();
            win.DataContext = this;
            win.ShowDialog();
            GetSysDataDictionaryType();
        }
        /// <summary>
        /// 添加命令
        /// </summary>
        public RelayCommand InsertSysDataDicTypeCommand { get; set; }
        /// <summary>
        /// 添加方法
        /// </summary>
        private void InsertSysDataDicTypeMethod()
        {
            if (ValidationSysDataDictionaryType())
            {
                if (SysDataDictionaryTypeList.Where(p => p.Code == SysDataDicTypeCode).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6418]);
                    return;
                }
                if (SysDataDictionaryTypeList.Where(p => p.Values == SysDataDicTypeValue).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6419]);
                    return;
                }
                SystemTypeModel model = new SystemTypeModel();
                model.Code = SysDataDicTypeCode;
                model.Values = SysDataDicTypeValue;
                model.IsEnable = true;
                OperationResult result = mBusiness.SaveSystemType(model, Core.ModelsOperation.OperationEnum.Add);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    SelectSysDataDictionaryType = null;
                    GetSysDataDictionaryType();
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }
        /// <summary>
        /// 修改命令
        /// </summary>
        public RelayCommand UpdateSysDataDicTypeCommand { get; set; }
        /// <summary>
        /// 修改方法
        /// </summary>
        private void UpdateSysDataDicTypeMethod()
        {
            if (SelectSysDataDictionaryType == null || string.IsNullOrEmpty(SelectSysDataDictionaryType.Code))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！
                return;
            }

            if (ValidationSysDataDictionaryType())
            {
                if (SysDataDictionaryTypeList.Where(p => p.Code == SysDataDicTypeCode).Count() > 0 &&
                    SelectSysDataDictionaryType.Code != SysDataDicTypeCode)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6419]); //"编码已经存在请重新填写"); 
                    return;
                }
                if (SysDataDictionaryTypeList.Where(p => p.Values == SysDataDicTypeValue).Count() > 0 &&
                    SelectSysDataDictionaryType.Values != SysDataDicTypeValue)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6418]); //"名称已经存在请重新填写"); 
                    return;
                }
                SystemTypeModel model = new SystemTypeModel();
                model.Id = SelectSysDataDictionaryType.Id;
                model.IsEnable = SelectSysDataDictionaryType.IsEnable;
                model.Order = SelectSysDataDictionaryType.Order;
                model.Code = SysDataDicTypeCode;
                model.Values = SysDataDicTypeValue;
                OperationResult result = mBusiness.SaveSystemType(model, Core.ModelsOperation.OperationEnum.Modify);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    SelectSysDataDictionaryType = null;
                    GetSysDataDictionaryType();
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }
        /// <summary>
        /// 删除命令
        /// </summary>
        public RelayCommand DeleteSysDataDicTypeCommand { get; set; }
        /// <summary>
        /// 删除方法
        /// </summary>
        private void DeleteSysDataDicTypeMethod()
        {
            if (SelectSysDataDictionaryType == null || string.IsNullOrEmpty(SelectSysDataDictionaryType.Code))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！
                return;
            }

            if (ValidationSysDataDictionaryType())
            {
                SystemTypeModel model = new SystemTypeModel();
                model.Id = SelectSysDataDictionaryType.Id;
                model.Code = SysDataDicTypeCode;
                model.Values = SysDataDicTypeValue;
                OperationResult result = mBusiness.SaveSystemType(model, Core.ModelsOperation.OperationEnum.Delete);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    SelectSysDataDictionaryType = null;
                    GetSysDataDictionaryType();
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 校验基础信息类型设置
        /// </summary>
        /// <returns></returns>
        private bool ValidationSysDataDictionaryType()
        {
            if (string.IsNullOrEmpty(SysDataDicTypeCode))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6420]); //"请填写编码
                return false;
            }
            if (string.IsNullOrEmpty(SysDataDicTypeValue))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6421]); //"请填写名称"
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取基础信息类型
        /// </summary>
        private void GetSysDataDictionaryType()
        {
            OperationResult<List<SystemTypeModel>> result = mBusiness.GetSysDataDictionaryTypeList();
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
            {
                DataDictionaryService.Instance.InitializeSystemTypeDictionary();
                SysDataDictionaryTypeList = new ObservableCollection<SystemTypeModel>(result.Results.Where(o => o.IsEnable));
            }
        }
        #endregion
        #endregion

        #region 系统基础信息信息设置
        #region 属性
        private ObservableCollection<SystemTypeValueModel> _SysDataDictionaryInfoList = new ObservableCollection<SystemTypeValueModel>();
        /// <summary>
        /// 基础信息信息列表
        /// </summary>
        public ObservableCollection<SystemTypeValueModel> SysDataDictionaryInfoList
        {
            get { return _SysDataDictionaryInfoList; }
            set { Set(ref _SysDataDictionaryInfoList, value); }
        }
        private SystemTypeValueModel _SelectSysDataDictionaryInfo = new SystemTypeValueModel();
        /// <summary>
        /// 基础信息信息选中项
        /// </summary>
        public SystemTypeValueModel SelectSysDataDictionaryInfo
        {
            get { return _SelectSysDataDictionaryInfo; }
            set
            {
                Set(ref _SelectSysDataDictionaryInfo, value);

                if (_SelectSysDataDictionaryInfo != null)
                {
                    SysDataDicCode = _SelectSysDataDictionaryInfo.Code;
                    SysDataDicValue = _SelectSysDataDictionaryInfo.DisplayValue;
                    SysDataDicLanguageID = _SelectSysDataDictionaryInfo.LanguageID;
                    SysDataDicIsEnable = _SelectSysDataDictionaryInfo.IsEnable;
                    SysDataDicOrder = _SelectSysDataDictionaryInfo.Order.ToString();
                }
                else
                {
                    SysDataDicCode = 0;
                    SysDataDicValue = string.Empty;
                    SysDataDicLanguageID = 0;
                    SysDataDicIsEnable = true;
                    SysDataDicOrder = string.Empty;
                }
            }
        }
        private int _SysDataDicCode;
        /// <summary>
        /// 基础信息编码
        /// </summary>
        public int SysDataDicCode
        {
            get { return _SysDataDicCode; }
            set { Set(ref _SysDataDicCode, value); }
        }
        private string _SysDataDicValue;
        /// <summary>
        /// 基础信息名称
        /// </summary>
        public string SysDataDicValue
        {
            get { return _SysDataDicValue; }
            set { Set(ref _SysDataDicValue, value); }
        }
        private int _SysDataDicLanguageID;
        /// <summary>
        /// 基础信息语言编号
        /// </summary>
        public int SysDataDicLanguageID
        {
            get { return _SysDataDicLanguageID; }
            set { Set(ref _SysDataDicLanguageID, value); }
        }
        private string _SysDataDicOrder;
        /// <summary>
        /// 显示顺序
        /// </summary>
        public string SysDataDicOrder
        {
            get { return _SysDataDicOrder; }
            set { Set(ref _SysDataDicOrder, value); }
        }
        private bool _SysDataDicIsEnable;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool SysDataDicIsEnable
        {
            get { return _SysDataDicIsEnable; }
            set { Set(ref _SysDataDicIsEnable, value); }
        }
        #endregion

        #region 命令
        private void InitializeSysDataCommand()
        {
            InsertSysDataDicCommand = new RelayCommand(InsertSysDataDicMethod);
            UpdateSysDataDicCommand = new RelayCommand(UpdateSysDataDicMethod);
            DeleteSysDataDicCommand = new RelayCommand(DeleteSysDataDicMethod);
        }
        /// <summary>
        /// 添加命令
        /// </summary>
        public RelayCommand InsertSysDataDicCommand { get; set; }
        /// <summary>
        /// 添加方法
        /// </summary>
        private void InsertSysDataDicMethod()
        {
            if (ValidationSysDataDictionary())
            {
                if (SysDataDictionaryInfoList.Where(p => p.Code == SysDataDicCode).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6419]); //"编码已经存在请重新填写"); 
                    return;
                }
                if (SysDataDictionaryInfoList.Where(p => p.DisplayValue == SysDataDicValue).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6418]); //"名称已经存在请重新填写"); 
                    return;
                }
                if (SysDataDictionaryInfoList.Where(p => p.Order == StringParseHelper.ParseByDefault(SysDataDicOrder, 0)).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6413]); //"显示顺序已存在请重新填写"); 
                    return;
                }
                SystemTypeValueModel model = new SystemTypeValueModel();
                model.Code = SysDataDicCode;
                model.DisplayValue = SysDataDicValue;
                model.LanguageID = SysDataDicLanguageID;
                model.IsEnable = SysDataDicIsEnable;
                model.Order = StringParseHelper.ParseByDefault(SysDataDicOrder, 0);
                model.CodeGroupID = SelectSysDataDictionaryType.Id;
                OperationResult result = mBusiness.SaveSystemType(model, Core.ModelsOperation.OperationEnum.Add);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    SelectSysDataDictionaryInfo = null;
                    if (_SelectSysDataDictionaryType != null && !string.IsNullOrEmpty(_SelectSysDataDictionaryType.Code))
                        GetSysDataDictionaryInfo(_SelectSysDataDictionaryType.Code);
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }
        /// <summary>
        /// 修改命令
        /// </summary>
        public RelayCommand UpdateSysDataDicCommand { get; set; }
        /// <summary>
        /// 修改方法
        /// </summary>
        private void UpdateSysDataDicMethod()
        {
            if (SelectSysDataDictionaryInfo == null)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！
                return;
            }

            if (ValidationSysDataDictionary())
            {
                if (SysDataDictionaryInfoList.Where(p => p.Code == SysDataDicCode).Count() > 0 &&
                    SelectSysDataDictionaryInfo.Code != SysDataDicCode)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6419]); //"编码已经存在请重新填写"); 
                    return;
                }
                if (SysDataDictionaryInfoList.Where(p => p.DisplayValue == SysDataDicValue).Count() > 0 &&
                    SelectSysDataDictionaryInfo.DisplayValue != SysDataDicValue)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6418]); //"名称已经存在请重新填写"); 
                    return;
                }
                if (SysDataDictionaryInfoList.Where(p => p.Order == StringParseHelper.ParseByDefault(SysDataDicOrder, 0) && p.Id != SelectSysDataDictionaryInfo.Id).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6413]); //"显示顺序已存在请重新填写");
                    return;
                }
                SystemTypeValueModel model = new SystemTypeValueModel();
                model.Id = SelectSysDataDictionaryInfo.Id;
                model.Code = SysDataDicCode;
                model.DisplayValue = SysDataDicValue;
                model.LanguageID = SysDataDicLanguageID;
                model.IsEnable = SysDataDicIsEnable;
                model.Order = StringParseHelper.ParseByDefault(SysDataDicOrder, 0);
                model.CodeGroupID = SelectSysDataDictionaryType.Id;
                model.Create_user = "TODO"; // TODO 这里要取真实的用户
                OperationResult result = mBusiness.SaveSystemType(model, Core.ModelsOperation.OperationEnum.Modify);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    SelectSysDataDictionaryInfo = null;
                    if (_SelectSysDataDictionaryType != null && !string.IsNullOrEmpty(_SelectSysDataDictionaryType.Code))
                        GetSysDataDictionaryInfo(_SelectSysDataDictionaryType.Code);
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }
        /// <summary>
        /// 删除命令
        /// </summary>
        public RelayCommand DeleteSysDataDicCommand { get; set; }

        /// <summary>
        /// 删除方法
        /// </summary>
        private void DeleteSysDataDicMethod()
        {
            if (SelectSysDataDictionaryInfo == null)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！
                return;
            }

            if (ValidationSysDataDictionary())
            {
                SystemTypeValueModel model = new SystemTypeValueModel();
                model.Id = SelectSysDataDictionaryInfo.Id;
                model.Code = SysDataDicCode;
                model.DisplayValue = SysDataDicValue;
                model.LanguageID = SysDataDicLanguageID;
                model.CodeGroupID = SelectSysDataDictionaryType.Id;
                OperationResult result = mBusiness.SaveSystemType(model, Core.ModelsOperation.OperationEnum.Delete);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    SelectSysDataDictionaryInfo = null;
                    if (_SelectSysDataDictionaryType != null && !string.IsNullOrEmpty(_SelectSysDataDictionaryType.Code))
                        GetSysDataDictionaryInfo(_SelectSysDataDictionaryType.Code);
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 校验基础信息设置
        /// </summary>
        /// <returns></returns>
        private bool ValidationSysDataDictionary()
        {
            if (SysDataDicCode < 0)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6420]); //"请填写编码"); 
                return false;
            }
            if (string.IsNullOrEmpty(SysDataDicValue))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6421]); //"请填写名称"); 
                return false;
            }
            if (SysDataDicLanguageID < 0)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6422]); //"请填写正确的语言编号
                return false;
            }
            if (!DataValidateHelper.IsComposeOnlyByNum(SysDataDicOrder))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6412]); //"显示顺序只能为整数数字！
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取基础信息信息
        /// </summary>
        private void GetSysDataDictionaryInfo(string codeGroup)
        {
            if (SelectSysDataDictionaryType != null && !string.IsNullOrEmpty(SelectSysDataDictionaryType.Code))
            {
                OperationResult<List<SystemTypeValueModel>> result = mBusiness.GetSysDataDictionaryInfoList(SelectSysDataDictionaryType.Id);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    DataDictionaryService.Instance.InitializeSystemTypeDictionary();
                    SysDataDictionaryInfoList = new ObservableCollection<SystemTypeValueModel>(result.Results.OrderBy(o => o.Order));
                }
            }
        }
        #endregion
        #endregion
        #endregion
    }
}
