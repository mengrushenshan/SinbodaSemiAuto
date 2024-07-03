using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Infrastructure.Model;
using Sinboda.Framework.MainWindow.Blue;
using Sinboda.SemiAuto.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SinbodaSemiAuto
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        private IBootStrapper bootStrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            Process current = Process.GetCurrentProcess();
            // 必须调用 BootStrapper.RegisterBootStrapper
            bootStrapper = BootStrapper.RegisterBootStrapper(new DefaultBootStrapper(current.ProcessName));

            //bootStrapper.SetDefaultView("SysLogPageView");    // 这是启动后默认加载页面，不设置则显示主页
            // 关闭程序前触发，可取消
            bootStrapper.AppClosing += BootStrapper_AppClosing;
            // 切换用户前触发，可取消
            bootStrapper.UserChanging += BootStrapper_UserChanging;


            bootStrapper.MianWindowCreated += BootStrapper_MianWindowCreated;

            // 添加自定义初始化任务
            // 自定义任务必须在调用 Run() 方法之前添加
            // 正在初始化功能模块
            bootStrapper.AddInitTask(new CustomTask<InitTaskResult>(SystemResources.Instance.LanguageArray[6475], () =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    bootStrapper.ModuleManager.ModuleInitDic.Add("Sinboda.Framework.View.SystemManagement.dll#Sinboda.Framework.View.SystemManagement.InitializeModule", new Sinboda.Framework.View.SystemManagement.InitializeModule());
                    bootStrapper.ModuleManager.ModuleInitDic.Add("Sinboda.Framework.View.SystemAlarm.dll#Sinboda.Framework.View.SystemAlarm.InitializeModule", new Sinboda.Framework.View.SystemAlarm.InitializeModule());
                    bootStrapper.ModuleManager.ModuleInitDic.Add("Sinboda.Framework.Print.dll#Sinboda.Framework.Print.InitializeModule", new Sinboda.Framework.Print.InitializeModule());
                    bootStrapper.ModuleManager.ModuleInitDic.Add("Sinboda.Framework.View.SystemSetup.dll#Sinboda.Framework.View.SystemSetup.InitializeModule", new Sinboda.Framework.View.SystemSetup.InitializeModule());

                    //产品部分
                    bootStrapper.ModuleManager.ModuleInitDic.Add("Sinboda.SemiAuto.Core.dll#Sinboda.SemiAuto.Core.InitializeModule", new Sinboda.SemiAuto.Core.InitializeModule());
                    bootStrapper.ModuleManager.ModuleInitDic.Add("Sinboda.SemiAuto.View.dll#Sinboda.SemiAuto.View.InitializeModule", new Sinboda.SemiAuto.View.InitializeModule());
                    bootStrapper.ModuleManager.ModuleInitDic.Add("Sinboda.SemiAuto.View.Samples.dll#Sinboda.SemiAuto.View.Samples.InitializeModule", new Sinboda.SemiAuto.View.Samples.InitializeModule());
                    bootStrapper.ModuleManager.ModuleInitDic.Add("Sinboda.SemiAuto.View.Results.dll#Sinboda.SemiAuto.View.Results.InitializeModule", new Sinboda.SemiAuto.View.Results.InitializeModule());
                    bootStrapper.ModuleManager.ModuleInitDic.Add("Sinboda.SemiAuto.View.MachineryDebug.dll#Sinboda.SemiAuto.View.MachineryDebug.InitializeModule", new Sinboda.SemiAuto.View.MachineryDebug.InitializeModule());
                    bootStrapper.SetDefaultView("SamplesRegisterPageView");
                });
                return new InitTaskResult();
            }), 3);

            bootStrapper.AddInitTask(new CustomTask<InitTaskResult>(SystemResources.Instance.LanguageArray[6475], () =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    bootStrapper.ModuleManager.ConfigInitDic.Add("Sinboda.Framework.View.SystemSetup.View.SoftWareCommonSettingPageView", new Sinboda.Framework.View.SystemSetup.View.SoftWareCommonSettingPageView());
                    bootStrapper.ModuleManager.ConfigInitDic.Add("Sinboda.Framework.View.SystemSetup.View.SysInfoManagePageView", new Sinboda.Framework.View.SystemSetup.View.SysInfoManagePageView());
                    bootStrapper.ModuleManager.ConfigInitDic.Add("Sinboda.Framework.View.SystemSetup.View.LISCommunicationSettingPageView", new Sinboda.Framework.View.SystemSetup.View.LISCommunicationSettingPageView());
                    bootStrapper.ModuleManager.ConfigInitDic.Add("Sinboda.Framework.View.SystemSetup.View.SysUsersManageSettingPageView", new Sinboda.Framework.View.SystemSetup.View.SysUsersManageSettingPageView());
                    bootStrapper.ModuleManager.ConfigInitDic.Add("Sinboda.Framework.View.SystemSetup.View.SysPermissionManageSettingPageView", new Sinboda.Framework.View.SystemSetup.View.SysPermissionManageSettingPageView());
                    bootStrapper.ModuleManager.ConfigInitDic.Add("Sinboda.Framework.Print.ReportSetting", new Sinboda.Framework.Print.ReportSetting());
                });
                return new InitTaskResult();
            }));


            // 启动程序
            bootStrapper.Run();


        }

        private void BootStrapper_MianWindowCreated(object sender, System.EventArgs e)
        {
            //var frame = (NavigationFrame)NavigationServiceExBase.CurrentService.Frame;
            //frame.NavBarVisbility = Visibility.Collapsed;
        }

        /// <summary>
        /// 切换用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BootStrapper_UserChanging(object sender, UserChangeEventArgs e)
        {
            // 返回出 true 取消切换用户操作
            e.Cancel = NotificationService.Instance.ShowMessage("询问", "确定要切换用户吗？", MessageBoxButton.YesNo, SinMessageBoxImage.Question) == MessageBoxResult.No; //TODO 翻译
            if (!e.Cancel)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    bootStrapper.ModuleManager.ConfigInitDic["Sinboda.Framework.View.SystemSetup.View.SoftWareCommonSettingPageView"] = null;
                    bootStrapper.ModuleManager.ConfigInitDic["Sinboda.Framework.View.SystemSetup.View.SysInfoManagePageView"] = null;
                    bootStrapper.ModuleManager.ConfigInitDic["Sinboda.Framework.View.SystemSetup.View.LISCommunicationSettingPageView"] = null;
                    bootStrapper.ModuleManager.ConfigInitDic["Sinboda.Framework.View.SystemSetup.View.SysUsersManageSettingPageView"] = null;
                    bootStrapper.ModuleManager.ConfigInitDic["Sinboda.Framework.View.SystemSetup.View.SysPermissionManageSettingPageView"] = null;
                    bootStrapper.ModuleManager.ConfigInitDic["Sinboda.Framework.Print.ReportSetting"] = null;

                    bootStrapper.ModuleManager.ConfigInitDic["Sinboda.Framework.View.SystemSetup.View.SoftWareCommonSettingPageView"] = new Sinboda.Framework.View.SystemSetup.View.SoftWareCommonSettingPageView();
                    bootStrapper.ModuleManager.ConfigInitDic["Sinboda.Framework.View.SystemSetup.View.SysInfoManagePageView"] = new Sinboda.Framework.View.SystemSetup.View.SysInfoManagePageView();
                    bootStrapper.ModuleManager.ConfigInitDic["Sinboda.Framework.View.SystemSetup.View.LISCommunicationSettingPageView"] = new Sinboda.Framework.View.SystemSetup.View.LISCommunicationSettingPageView();
                    bootStrapper.ModuleManager.ConfigInitDic["Sinboda.Framework.View.SystemSetup.View.SysUsersManageSettingPageView"] = new Sinboda.Framework.View.SystemSetup.View.SysUsersManageSettingPageView();
                    bootStrapper.ModuleManager.ConfigInitDic["Sinboda.Framework.View.SystemSetup.View.SysPermissionManageSettingPageView"] = new Sinboda.Framework.View.SystemSetup.View.SysPermissionManageSettingPageView();
                    bootStrapper.ModuleManager.ConfigInitDic["Sinboda.Framework.Print.ReportSetting"] = new Sinboda.Framework.Print.ReportSetting();
                });
            }
        }

        /// <summary>
        /// 应用程序关闭前触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BootStrapper_AppClosing(object sender, AppCancelEventArgs e)
        {
            // 返回出 true 取消关闭操作
            e.Cancel = NotificationService.Instance.ShowMessage("关机", "确定要关闭 平台 软件吗？", MessageBoxButton.YesNo, SinMessageBoxImage.Question) == MessageBoxResult.No;
            if(!e.Cancel)
            {
                //释放相机资源
                PVCamHelper.Instance.Dispose();

                //释放通讯资源
                TcpCmdActuators.Instance.Dispose();
            }
        }
    }
}
