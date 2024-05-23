using Sinboda.Framework.Business.SystemManagement;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Core.Services;

namespace Sinboda.Framework.View.SystemSetup.ViewModel
{
    /// <summary>
    /// 
    /// </summary>DelUserCommand
    public class SysUsersManageSettingViewModel : NavigationViewModelBase
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
        /// 当前角色
        /// </summary>
        private string currentUserRoleID = string.Empty;
        /// <summary>
        /// 构造函数
        /// </summary>   
        public SysUsersManageSettingViewModel()
        {
            PageTitle = SystemResources.Instance.LanguageArray[17];
            //InitUserAndRole();
            InitializeUserCommand();
        }
        #region 用户设置
        #region 属性
        /// <summary>
        /// 用户ID
        /// </summary>
        string _UserName;
        /// <summary>
        /// 用户ID属性
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { Set(ref _UserName, value); }
        }
        /// <summary>
        /// 原密码
        /// </summary>
        string _OriginalPassword;
        /// <summary>
        /// 原密码属性
        /// </summary>
        public string OriginalPassword
        {
            get { return _OriginalPassword; }
            set { Set(ref _OriginalPassword, value); }
        }
        /// <summary>
        /// 密码
        /// </summary>
        string _Password;
        /// <summary>
        /// 密码属性
        /// </summary>
        public string Password
        {
            get { return _Password; }
            set { Set(ref _Password, value); }
        }
        /// <summary>
        /// 确认密码
        /// </summary>
        string _PasswordEnsure;
        /// <summary>
        /// 确认密码属性
        /// </summary>
        public string PasswordEnsure
        {
            get { return _PasswordEnsure; }
            set { Set(ref _PasswordEnsure, value); }
        }
        /// <summary>
        /// 角色列表选中项
        /// </summary>
        SysRoleModel _SelectUserRole;
        /// <summary>
        /// 角色列表选中项属性
        /// </summary>
        public SysRoleModel SelectUserRole
        {
            get { return _SelectUserRole; }
            set { Set(ref _SelectUserRole, value); }
        }
        private bool _UserNameEnabled = true;
        /// <summary>
        /// 用户名可用状态
        /// </summary>
        public bool UserNameEnabled
        {
            get { return _UserNameEnabled; }
            set { Set(ref _UserNameEnabled, value); }
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        private ObservableCollection<SysUserModel> _UserList = new ObservableCollection<SysUserModel>();
        /// <summary>
        /// 用户列表属性
        /// </summary>
        public ObservableCollection<SysUserModel> UserList
        {
            get { return _UserList; }
            set { Set(ref _UserList, value); }
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
        /// <summary>
        /// 用户列表中的选中项
        /// </summary>
        private SysUserModel selectUser;
        /// <summary>
        /// 用户列表中的选中项属性
        /// </summary>
        public SysUserModel SelectUser
        {
            get { return selectUser; }
            set
            {
                Set(ref selectUser, value);
                if (SelectUser != null)
                {
                    UserName = selectUser.UserName;
                    Password = string.Empty;
                    PasswordEnsure = string.Empty;
                    OriginalPassword = string.Empty;
                    //Password = selectUser.Password;
                    //PasswordEnsure = selectUser.Password;
                    //OriginalPassword = selectUser.Password;
                    SelectUserRole = RoleList.FirstOrDefault(o => o.RoleID == SelectUser.RoleID);
                    //UserNameEnabled = false; bug  41116
                    SignPath = selectUser.SignPath;
                }
                else
                {
                    UserName = string.Empty;
                    Password = string.Empty;
                    PasswordEnsure = string.Empty;
                    OriginalPassword = string.Empty;
                    SelectUserRole = null;
                    UserNameEnabled = true;
                    SignPath = string.Empty;
                }
            }
        }
        /// <summary>
        /// 是否显示重置按钮
        /// </summary>
        public Visibility isVisibility;
        public Visibility IsVisibility
        {
            get { return isVisibility; }
            set { Set(ref isVisibility, value); }
        }

        private string signPath;
        public string SignPath
        {
            get { return signPath; }
            set { Set(ref signPath, value); }
        }
        #endregion

        #region 命令
        private void InitializeUserCommand()
        {
            AddUserCommand = new RelayCommand(AddUserMethod);
            ModifyUserCommand = new RelayCommand(ModifyUserMethod, CanModifyUserMethod);
            DelUserCommand = new RelayCommand(DelUserMethod, CanDelUserMethod);
            UserClearCommand = new RelayCommand(UserClearMethod);
            ResetPasswordCommand = new RelayCommand(ResetPasswordMethod, CanResetPasswordMethod);
            SetSignPathCommand = new RelayCommand(GetBackupMethod);
        }

        /// <summary>
        /// 添加用户命令
        /// </summary>
        public RelayCommand AddUserCommand { get; set; }
        /// <summary>
        /// 增加用户按钮响应函数
        /// </summary>
        public void AddUserMethod()
        {
            if (ValidateUser())
            {
                //同级别用户无法添加同级别的用户
                if (SystemResources.Instance.CurrentRole == SelectUserRole.RoleID && SystemResources.Instance.CurrentUserName != "admin")
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[3177]);//29提示 3177不能添加同级别用户
                    return;
                }

                OperationResult<PermissionErrorCode> operationResult = pBusiness.AddUser(UserName, Password, SelectUserRole.RoleID, SignPath);
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
                    WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[46], SystemResources.Instance.LanguageArray[17], UserName));//29提示609保存成功 //TODO 翻译
                    GetUserList();
                    SelectUser = null;
                    ShowMessageComplete(SystemResources.Instance.LanguageArray[609]);//29提示 609保存成功！
                }
                else if (result == PermissionErrorCode.UserExist)
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[2553]);//29提示 2553用户已存在
                }
                else
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[610]);//29提示610保存失败
                }
            }
        }

        /// <summary>
        /// 更新用户命令
        /// </summary>
        public RelayCommand ModifyUserCommand { get; set; }
        /// <summary>
        /// 修改用户按钮响应函数
        /// </summary>
        public void ModifyUserMethod()
        {
            if (SelectUser != null)
            {
                if (ValidateUser() && ValidateOriginalPassword())
                {
                    if (SelectUser.UserName != UserName)
                    {
                        ShowMessageWarning(SystemResources.Instance.LanguageArray[3178]);//用户名不能修改
                        return;
                    }
                    // 不能修改当前登陆用户的角色
                    if (SelectUser.UserName == SystemResources.Instance.CurrentUserName)
                    {
                        if (SelectUser.RoleID != SelectUserRole.RoleID)
                        {
                            ShowMessageWarning(SystemResources.Instance.LanguageArray[3179]);//29提示 3179不能修改当前登陆用户的角色
                            return;
                        }
                    }
                    else
                    {
                        if (Password != SelectUser.Password)
                        {
                            ShowMessageWarning(SystemResources.Instance.LanguageArray[209]);//29提示 3180:不能修改其他用户密码
                            return;
                        }
                        if (SystemResources.Instance.CurrentRole == SelectUserRole.RoleID)
                        {
                            ShowMessageWarning(SystemResources.Instance.LanguageArray[3180]);//29提示 3180:不能修改为同级别用户
                            return;
                        }
                    }

                    OperationResult<PermissionErrorCode> operationResult = pBusiness.ModifyUser(SelectUser.UserName, Password, SelectUserRole.RoleID, SignPath);
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
                        WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[47], SystemResources.Instance.LanguageArray[17], SelectUser.UserName));//29提示609保存成功 //TODO 翻译
                        GetUserList();
                        SelectUser = null;
                        ShowMessageComplete(SystemResources.Instance.LanguageArray[6401]);//29提示609保存成功！ 
                    }
                    else if (result == PermissionErrorCode.UserNotExist)
                    {
                        ShowMessageError(SystemResources.Instance.LanguageArray[6459]);//29提示2553用户不存在 
                    }
                    else
                    {
                        ShowMessageError(SystemResources.Instance.LanguageArray[6389]); //"发生其他错误，更新失败！
                    }
                }
            }
            else
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！
                return;
            }
        }
        private bool CanModifyUserMethod()
        {
            if (SelectUser == null)
                return false;

            // 设计模式可以
            if (SystemResources.Instance.CurrentRole == "DesignMode")
                return true;

            // 当前选中用户为当前登录用户可以
            if (SelectUser.UserName == SystemResources.Instance.CurrentUserName)
                return true;

            // 当前选中用户为admin，当前登录用户非admin，不可以
            if (SelectUser.UserName == "admin" && SystemResources.Instance.CurrentUserName != "admin")
                return false;

            // 当前登录角色为管理员，当前选中用户角色为操作人员
            if (SystemResources.Instance.CurrentRole == "ApplyAdminMode" && SelectUser.RoleID == "ApplyUseMode")
                return true;

            return false;
        }

        /// <summary>
        /// 删除用户命令
        /// </summary>
        public RelayCommand DelUserCommand { get; set; }
        /// <summary>
        /// 删除用户按钮响应函数
        /// </summary>
        public void DelUserMethod()
        {
            if (SelectUser != null)
            {
                if (SelectUser != null && SelectUser.UserName == SystemResources.Instance.CurrentUserName)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[3181]);//29提示 3181:不能删除当前登陆用户
                    return;
                }
                if (NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[2197], SystemResources.Instance.LanguageArray[351], MessageBoxButton.YesNo, SinMessageBoxImage.Question) == MessageBoxResult.No)//"提示", "您确认要删除选中的项目吗?"
                    return;


                OperationResult<PermissionErrorCode> operationResult = pBusiness.DelUser(SelectUser.UserName);
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
                    WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[48], SystemResources.Instance.LanguageArray[17], SelectUser.UserName));//29提示609保存成功 //TODO 翻译
                    GetUserList();
                    SelectUser = null;
                    ShowMessageComplete(SystemResources.Instance.LanguageArray[754]);//29提示754删除成功
                }
                else if (result == PermissionErrorCode.UserNotExist)
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6460]);//"用户不存在，删除失败！"); 
                }
                else
                {
                    ShowMessageError(SystemResources.Instance.LanguageArray[6391]); //"发生其他错误，删除失败！"); 
                }
            }
            else
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！
                return;
            }
        }

        private bool CanDelUserMethod()
        {
            if (SelectUser == null)
                return false;

            // 设计模式可以
            if (SystemResources.Instance.CurrentRole == "DesignMode")
                return true;

            // 当前选中用户为admin，不可以
            if (SelectUser.UserName == "admin")
                return false;

            if (SystemResources.Instance.CurrentUserName == "admin" && SystemResources.Instance.CurrentRole != "DesignMode")
                return true;

            // 当前登录角色为管理员，当前选中用户角色为操作人员
            if (SystemResources.Instance.CurrentRole == "ApplyAdminMode" && SelectUser.RoleID == "ApplyUseMode")
                return true;

            return false;
        }

        /// <summary>
        /// 清除输入信息命令
        /// </summary>
        public RelayCommand UserClearCommand { get; set; }
        private void UserClearMethod()
        {
            if (NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[2197], SystemResources.Instance.LanguageArray[6392], MessageBoxButton.YesNo, SinMessageBoxImage.Question) == MessageBoxResult.No)//"提示" 
                return;
            SelectUser = null;
        }

        public RelayCommand ResetPasswordCommand { get; set; }
        private void ResetPasswordMethod()
        {
            if (SelectUser != null)
            {
                if (NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[2197], SystemResources.Instance.LanguageArray[211], MessageBoxButton.YesNo, SinMessageBoxImage.Question) == MessageBoxResult.No)//"提示", "您确认要重置选中的用户密码吗? "
                    return;
                //if (ValidateUser())
                {

                    OperationResult<PermissionErrorCode> operationResult = pBusiness.ModifyUser(SelectUser.UserName, "1", SelectUser.RoleID, SignPath);
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
                        WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[47], SystemResources.Instance.LanguageArray[17], SelectUser.UserName));//29提示609保存成功 //TODO 翻译
                        GetUserList();
                        ShowMessageComplete(SystemResources.Instance.LanguageArray[62]);//重置成功，重置后的密码是1
                    }
                    else if (result == PermissionErrorCode.UserNotExist)
                    {
                        ShowMessageError(SystemResources.Instance.LanguageArray[6459]);//29提示2553用户不存在 
                    }
                    else
                    {
                        ShowMessageError(SystemResources.Instance.LanguageArray[6389]); //"发生其他错误，更新失败！
                    }
                }
            }
            else
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！
                return;
            }
        }
        private bool CanResetPasswordMethod()
        {
            if (SelectUser == null)
                return false;

            // 设计模式可以
            if (SystemResources.Instance.CurrentRole == "DesignMode")
                return true;

            // 当前选中用户为当前登录用户可以
            if (SelectUser.UserName == SystemResources.Instance.CurrentUserName)
                return true;

            // 当前选中用户为admin，当前登录用户非admin，不可以
            if (SelectUser.UserName == "admin" && SystemResources.Instance.CurrentUserName != "admin")
                return false;

            // 当前登录角色为管理员，当前选中用户角色为操作人员
            if (SystemResources.Instance.CurrentRole == "ApplyAdminMode" && SelectUser.RoleID == "ApplyUseMode")
                return true;

            return true;
        }

        /// <summary>
        /// 清除输入信息命令
        /// </summary>
        public RelayCommand SetSignPathCommand { get; set; }
        /// <summary>
        /// 获取备份路径方法
        /// </summary>
        private void GetBackupMethod()
        {
            OpenFileDialog FBD = new OpenFileDialog();
            FBD.Title = SystemResources.Instance.LanguageArray[6442];//"请选择路径";
            FBD.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            FBD.Multiselect = false;
            FBD.Filter = "(*.png)|*.png|(*.jpg)|*.jpg|(*.*)|*.*";
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                SignPath = FBD.FileName;
            }

        }
        #endregion

        #region 方法
        /// <summary>
        /// 验证函数
        /// </summary>
        /// <returns></returns>
        private bool ValidateUser()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[409]);//29提示409请输入用户名！
                return false;
            }
            //if (!IsLegalNumber(UserName))
            //{
            //    ShowMessageWarning("非法输入，请输入正确的用户名！");//29提示409请输入用户名！ //TODO 翻译
            //    return false;
            //}
            if (string.IsNullOrEmpty(Password))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[412]);//29提示412请输入密码！
                return false;
            }
            if (string.IsNullOrEmpty(PasswordEnsure))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6461]); //"请输入确认密码！");//29提示412请输入密码！ //TODO 翻译
                return false;
            }
            if (Password != PasswordEnsure)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[2558]);//29提示2558确认密码和密码必须相同！
                return false;
            }
            if (SelectUserRole == null)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[3922]);//29提示3922请选择角色
                return false;
            }
            return true;
        }
        /// <summary>
        /// 验证原始密码
        /// </summary>
        /// <returns></returns>
        private bool ValidateOriginalPassword()
        {
            if (string.IsNullOrEmpty(OriginalPassword))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[51]);//29提示412请输入原密码！
                return false;
            }
            if (SelectUser.Password != OriginalPassword)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[52]);//29提示2558 原密码错误！
                return false;
            }
            //if (Password == OriginalPassword)
            //{
            //    ShowMessageWarning(SystemResources.Instance.LanguageArray[53]);//29提示2558 原密码和新密码不能相同！
            //    return false;
            //}
            return true;
        }
        ///// <summary>
        ///// 校验是否为数字
        ///// </summary>
        ///// <param name="str"></param>
        ///// <returns></returns>
        //private static bool IsLegalNumber(string str)
        //{
        //    char[] charStr = str.ToLower().ToArray();
        //    for (int i = 0; i < charStr.Length; i++)
        //    {
        //        int num = Convert.ToInt32(charStr[i]);
        //        if (!(IsChineaseLetter(num) || (num >= 48 && num <= 57) || (num >= 97 && num <= 123) || (num >= 65 && num <= 90) || num == 45))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
        ///// <summary>
        ///// 校验是否为中文字符
        ///// </summary>
        ///// <param name="code"></param>
        ///// <returns></returns>
        //private static bool IsChineaseLetter(int code)
        //{
        //    int chbegin = Convert.ToInt32("4e00", 16);
        //    int chend = Convert.ToInt32("9fff", 16);
        //    if (code >= chbegin && code <= chend)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        /// <summary>
        /// 获取用户列表信息
        /// </summary>
        private void GetUserList()
        {
            List<SysUserModel> list = new List<SysUserModel>();
            OperationResult<List<SysUserModel>> operationResult = pBusiness.GetUserList();
            if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                list = operationResult.Results;
            else
                ShowMessageError(operationResult.Message);
            UserList = new ObservableCollection<SysUserModel>(list);
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
            if (SystemResources.Instance.CurrentRole == "DesignMode" || SystemResources.Instance.CurrentRole == "ApplyAdminMode")
            {
                IsVisibility = Visibility.Visible;
            }
            else
            {
                IsVisibility = Visibility.Collapsed;
            }

        }
        public void InitUserAndRole()
        {
            GetUserList();
            GetRoleList();
        }
        #endregion
        #endregion
    }
}
