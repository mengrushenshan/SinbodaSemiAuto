using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Business.SystemSetup;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.PeriodTimer;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.MainWindow.Blue.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.Framework.MainWindow.Blue.ViewModels
{
    /// <summary>
    /// 页面 <seealso cref="Views.MainLogin"/> 的 ViewModel
    /// </summary>
    public class MainLoginViewModel : ViewModelBase
    {
        IPermission iPermission = new PermissionOperation();
        /// <summary>
        /// 
        /// </summary>
        public MainLoginViewModel()
        {
            Messenger.Default.Register<string>(this, "ChangeUserRefreshUserNmame", ChangeUserRefreshUserNmameEvent);
            List<SysUserModel> list = iPermission.GetAllLoginUsers();
            UserNameList = list.Select(p => p.UserName).ToList();

            string rememberName = LoginSettingBusiness.Instance.GetRemeberName();
            if (!string.IsNullOrEmpty(rememberName) && UserNameList.Contains(rememberName))
            {
                UserName = rememberName;
            }
#if DEBUG
            UserName = "dryf";
            PassWord = "drrj";
#endif

            LoginCommand = new RelayCommand(LoginMethod);
            ExitCommand = new RelayCommand(ExitMethod);
        }
        /// <summary>
        /// 刷新用户名列表
        /// </summary>
        /// <param name="flag"></param>
        public void ChangeUserRefreshUserNmameEvent(string flag)
        {
            List<SysUserModel> list = iPermission.GetAllLoginUsers();
            UserNameList = list.Select(p => p.UserName).ToList();
        }
        private List<string> _UserNameList = new List<string>();
        /// <summary>
        /// 用户列表
        /// </summary>
        public List<string> UserNameList
        {
            get { return _UserNameList; }
            set { Set(ref _UserNameList, value); }
        }


        private string _UserName = string.Empty;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get
            {
                return _UserName.Trim();
            }
            set
            {
                if (string.IsNullOrEmpty(value) || Regex.IsMatch(value, "^[\u4E00-\u9FA5A-Za-z0-9]+$"))
                    Set(ref _UserName, value);
            }
        }


        private string _PassWord = string.Empty;
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord
        {
            get { return _PassWord; }
            set { Set(ref _PassWord, value); }
        }

        /// <summary>
        /// 登录命令
        /// </summary>
        public RelayCommand LoginCommand { get; set; }

        int failNum = 0;
        /// <summary>
        /// 登录方法
        /// </summary>
        private void LoginMethod()
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(PassWord))
            {
                if (iPermission.Login(UserName, PassWord))
                {
                    //调用核心模块接口初始化用户及用户权限资源
                    SystemInitialize.InitializePermission(UserName);
                    GlobalClass.ChangeLanguage();
                    //登录日志
                    SystemResources.Instance.SysLogInstance.WriteLogDb(StringResourceExtension.GetLanguage(2694, "登录系统"), SysLogType.Login); //TODO 翻译
                    failNum = 0;

                    PeriodManager.Instance.StartLogoutPeriodTask();

                    AppWindow.Current.ShowEx();
                    BootStrapper.Current.CurrentApp.MainWindow = AppWindow.Current;
                    MainLogin.Current.Hide();

                    LoginSettingBusiness.Instance.SetLoginName(UserName);
                }
                else
                {
                    failNum++;
                    if (failNum == 3)
                    {
                        NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[816], SystemResources.Instance.LanguageArray[2806], MessageBoxButton.OK, SinMessageBoxImage.Error); //29提示 2806用户名或密码错误输入错误三次，程序退出！
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[816], SystemResources.Instance.LanguageArray[2807], MessageBoxButton.OK, SinMessageBoxImage.Error);
                    }
                }
            }
            else
            {
                NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[816], SystemResources.Instance.LanguageArray[6469], MessageBoxButton.OK, SinMessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 退出命令
        /// </summary>
        public RelayCommand ExitCommand { get; set; }
        /// <summary>
        /// 退出方法
        /// </summary>
        private void ExitMethod()
        {
            //TODO ： 通知主框架退出程序
            //Messenger.Default.Send<string>("ExitProgram", "ExitProgram");
            Messenger.Default.Send<string>("ExitApplication", "ExitApplication");

            Application.Current.Shutdown();
        }
    }
}
