using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using Sinboda.Framework.MainWindow.Blue.Views;
using System.Diagnostics;
using System.Configuration;
using Sinboda.Framework.Common;
using System.IO;
using Sinboda.Framework.Common.FileOperateHelper;
using Microsoft.Practices.ServiceLocation;
using Sinboda.Framework.Core.Interface;
using System.Data.Entity;
using Sinboda.Framework.Core.PeriodTimer;
using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Business.SystemSetup;
using Sinboda.Framework.Core;

namespace Sinboda.Framework.MainWindow.Blue
{
    /// <summary>
    /// 平台提供的 <see cref="BootStrapperBase"/> 默认实现类
    /// </summary>
    public class DefaultBootStrapper : BootStrapperBase
    {
        private string[] safeSoftwareProgressNames = { "360tray", "360sd" };

        private string _processName;
        private MainLoading mainLoading;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DefaultBootStrapper(string processName) : base(processName)
        {
            _processName = processName;
            // 注册初始化任务事件
            InitTaskManager.AllCustomTaskExecution += InitTaskManager_AllCustomTaskExecution;
            InitTaskManager.BeginExecution += InitTaskManager_BeginExecution;
            InitTaskManager.EndExecution += InitTaskManager_EndExecution;

            // 当前语言及语言资源初始化
            SystemInitialize.InitializeLanguage();

            // 创建任务
            CreateSystemInitTask();
            // 设置主页
            NavigationHelper.Cuurrent.CreateRootItem(SystemResources.Instance.LanguageArray[6472], typeof(HomeView)); //主页
        }

        /// <summary>
        /// 创建系统初始化任务
        /// </summary>
        private void CreateSystemInitTask()
        {
            // 检查数据库是否有更新
            AddInitTask(new CustomTask<InitTaskResult>(SystemResources.Instance.LanguageArray[6473], () =>
            {
                // 检查是否存在杀毒软件进程
                foreach (var pname in safeSoftwareProgressNames)
                {
                    var ps = Process.GetProcessesByName(pname);
                    if (ps != null && ps.Length > 0)
                    {
                        return new InitTaskResult
                        {
                            IsCancel = true,
                            Succeed = false,
                            Message = StringResourceExtension.GetLanguage(279, "请卸载或关闭杀毒软件后，再启动程序")
                        };
                    }
                }


                // 获得当前数据库路径，决定是否创建检查数据库更新任务
                string dbFileLocationAndName = string.Empty;
                string name = ConfigurationManager.ConnectionStrings["DBConnectionStr"].ConnectionString;
                if (name.ToLower().Contains("fdb") && (name.ToLower().Contains("localhost") || name.Contains("127.0.0.1") || name.ToLower().Contains("server type=1")))
                {
                    string[] connectStrArray = name.Split(';');
                    foreach (var item in connectStrArray)
                    {
                        if (item.Contains("DataBase="))
                        {
                            dbFileLocationAndName = MapPath.DataBasePath + item.Split('=')[1];
                            break;
                        }
                        if (item.Contains("initial catalog="))
                        {
                            dbFileLocationAndName = MapPath.DataBasePath + item.Split('=')[1];
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(dbFileLocationAndName) && File.Exists(dbFileLocationAndName) && File.Exists(MapPath.UpdatePath + "TEMPLATE.FDB"))
                {
                    Process pro = new Process();
                    pro.StartInfo.FileName = "DBBackup2000.exe";
                    pro.StartInfo.Arguments = "/u";
                    pro.StartInfo.UseShellExecute = true;
                    pro.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    pro.Start();
                    pro.WaitForExit();
                    //获取执行结果 不为0 则为失败
                    string result = INIHelper.Read("Upgrade", "Error", MapPath.AppDir + "DBConfig.ini");
                    if (string.IsNullOrEmpty(result) || !result.Equals("0"))
                    {
                        //获取执行错误信息
                        // string resultError = INIHelper.Read("Upgrade", "ErrorMsg", MapPath.AppDir + "DBConfig.ini");
                        return new InitTaskResult()
                        {
                            Succeed = false,
                            Message = StringResourceExtension.GetLanguage(3053, "数据库升级失败"),
                            IsCancel = true,
                        };
                    }
                    else
                        return new InitTaskResult();
                }
                else return new InitTaskResult();
            }));

            // 初始化数据库连接
            AddInitTask(new CustomTask<InitTaskResult>(SystemResources.Instance.LanguageArray[2141], () =>
            {
                //处理目前使用的数据库
                var result = ServiceLocator.Current.GetInstance<IDbContextInitialize>();
                Type moduleType = result.DatabaseType;
                var instance = (DbContext)Activator.CreateInstance(moduleType);
                //判断数据库是否存在
                if (instance.Database.Exists())
                {
                    //创建数据库
                    result.IfExistIgnoreCreate();
                }
                else
                {
                    //初始化数据库
                    result.InitializeDB();

                    //初始化数据信息
                    DataDictionaryService.Instance.InitializeVersionInfo();
                    DataDictionaryService.Instance.InitializeSystemTypeDictionaryInfo();
                    DataDictionaryService.Instance.InitialzeBusinessDictionaryTypeInfo();

                    //初始化默认数据
                    result.InitializeData();
                }

                return new InitTaskResult();
            }));

            // 初始化平台资源
            AddInitTask(new CustomTask<InitTaskResult>(SystemResources.Instance.LanguageArray[6474], () =>
            {
                return SystemInitialize.InitializeResource();
            }));

            // 正在初始化功能模块
            AddInitTask(new CustomTask<InitTaskResult>(SystemResources.Instance.LanguageArray[6475], () =>
            {
                foreach (var module in ModuleManager.ModuleInfoSource)
                {
                    ModuleManager.LoadModuleType(module);
                    ModuleManager.InitModule(module);
                }
                return new InitTaskResult(true);
            }));
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void RunCore()
        {
            try
            {
                SoftWareInterfaceSettingBusiness business = new SoftWareInterfaceSettingBusiness();
                try
                {
                    //LogHelper.logSoftWare.Debug("GetSettingInfo RunCore");
                    //获取设置的皮肤和字体大小
                    OperationResult<SoftWareInterfaceModel> result = business.GetSettingInfo();
                    if (result.ResultEnum == OperationResultEnum.SUCCEED)
                    {
                        StyleResourceManager.SetTheme(result.Results.CurrentTheme, result.Results.CurrentFontSize);
                    }
                    else
                    {
                        StyleResourceManager.SetTheme("Green", "Normal");
                    }
                }
                catch (Exception ex)
                {
                    StyleResourceManager.SetTheme("Green", "Normal");
#if DEBUG
                    NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[6467], ex.Message, MessageBoxButton.OK, SinMessageBoxImage.Error); //
#endif
                    LogHelper.logSoftWare.Error(ex);
                    ShutdownCore();
                }

                //加载软件中中性信息显示部分（公司logo、仪器名称、仪器型号）
                business.GetAnalyzerInfo();

                mainLoading = new MainLoading();
                mainLoading.SetMaximum(InitTaskManager.Count);
                InitTaskManager.Execute();
                mainLoading.ShowDialog();
            }
            catch (Exception e)
            {
#if DEBUG
                NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[6467], e.ToStringEx(), MessageBoxButton.OK, SinMessageBoxImage.Error); //
#endif
                LogHelper.logSoftWare.Error(e);
                ShutdownCore();
            }
        }

        /// <summary>
        /// 一个初始化任务执行结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitTaskManager_EndExecution(object sender, CustomTaskEventArgs<InitTaskResult> e)
        {
            if (e.Result.Succeed)
            {
                mainLoading.Increment();
            }
            else
            {
                NotificationService.Instance.ShowError(e.Result.Message);

                if (e.Result.IsCancel)
                    InitTaskManager.Cancel();
                else
                    mainLoading.Increment();
            }
        }

        /// <summary>
        /// 开始一个初始化任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitTaskManager_BeginExecution(object sender, CustomTaskEventArgs<InitTaskResult> e)
        {
            mainLoading.SetTaskDescribe(e.CustomTask.TaskDescribe);
        }

        /// <summary>
        /// 初始化执行完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitTaskManager_AllCustomTaskExecution(object sender, CustomTaskCollectionEventArgs<InitTaskResult> e)
        {
            if (e.Status == CustomTaskStatus.Canceled || e.Status == CustomTaskStatus.Faulted)
            {

                if (e.Error != null)
                    NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[6477], e.Error.ToStringEx(), MessageBoxButton.OK, SinMessageBoxImage.Error);

                ShutdownCore();
                return;
            }

            CurrentApp.Dispatcher.Invoke(() =>
            {
                mainLoading.Close();
                mainLoading = null;
                MainLogin.Current.Show();
            });
        }
        /// <summary>
        /// 切换用户
        /// </summary>
        protected override void ChangeUserCore()
        {
            CurrentApp.Dispatcher.Invoke(() =>
            {
                // 清空导航历史，导航缓存，导航到主页
                NavigationServiceExBase.CurrentService.ClearNavigationHistory();
                NavigationServiceExBase.CurrentService.ClearNavigationCache();

                PeriodManager.Instance.StopLogoutPeriodTask();

                AppWindow.Current.HideEx();
                MainLogin.Current.Show();
                Messenger.Default.Send("ChangeUserRefreshUserNmame", "ChangeUserRefreshUserNmame");
                MainLogin.Current.cbxUserName.Text = "";
                MainLogin.Current.tbxPassWord.Password = "";
                MainLogin.Current.cbxUserName.Focus();
            });
        }

        /// <summary>
        /// 应用程序发生未处理的异常时执行（只捕获发生在主线程的异常）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // 有未处理的异常时显示提示窗口
            Log(StringResourceExtension.GetLanguage(65, "应用程序异常"), e.Exception);
            NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[4000], e.Exception.ToStringEx(), MessageBoxButton.OK, SinMessageBoxImage.Error); //TODO 翻译
        }

        /// <summary>
        /// 模块执行初始化结束
        /// </summary>
        /// <param name="moduleInfo"></param>
        /// <param name="error"></param>
        protected override void InitModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            ModuleExceptionHandle(moduleInfo, error);
        }

        /// <summary>
        /// 加载模块结束
        /// </summary>
        /// <param name="moduleInfo"></param>
        /// <param name="error"></param>
        protected override void LoadModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            ModuleExceptionHandle(moduleInfo, error);
        }

        /// <summary>
        /// 模块异常处理
        /// </summary>
        /// <param name="moduleInfo"></param>
        /// <param name="error"></param>
        private void ModuleExceptionHandle(ModuleInfo moduleInfo, Exception error)
        {
            if (error != null)
            {
                string message = StringResourceExtension.GetLanguage(66, "初始化化模块 : {0} 时发生异常 \r {1}", moduleInfo.ModuleName, error.ToStringEx());
                Log(message, error);
                throw new Exception(message); // 抛出异常，交给 InitTaskManager 处理
            }
        }

        /// <summary>
        /// 设置默认页面
        /// </summary>
        /// <param name="viewName"></param>
        public override void SetDefaultView(string viewName)
        {
            NavigationHelper.Cuurrent.DefaultViewName = viewName;
        }
    }
}
