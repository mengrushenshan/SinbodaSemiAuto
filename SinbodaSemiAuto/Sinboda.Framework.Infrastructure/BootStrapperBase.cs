using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 启动流程基类类，包含模块和任务接口
    /// </summary>
    public abstract class BootStrapperBase : IBootStrapper
    {
        /// <summary>
        /// 程序名称
        /// </summary>
        private string _processName;
        /// <summary>
        /// 是否正在执行关闭操作
        /// </summary>
        private bool isClosing = false;
        /// <summary>
        /// 是否正在切换用户
        /// </summary>
        private bool isUserChanging = false;

        /// <summary>
        /// 应用程序关闭前触发，可取消关闭
        /// </summary>
        public event EventHandler<AppCancelEventArgs> AppClosing;
        /// <summary>
        /// 注销用户关闭前触发，可取消注销
        /// </summary>
        public event EventHandler<UserChangeEventArgs> UserChanging;

        public event EventHandler MianWindowCreated;

        /// <summary>
        /// 返回当前应用程序的 <see cref="Application"/> 实例
        /// </summary>
        public Application CurrentApp
        {
            get { return Application.Current; }
        }

        /// <summary>
        /// 获取模块管理 <see cref="IModuleManager"/>
        /// </summary>
        public IModuleManager ModuleManager
        {
            get { return InterfaceMagager.ModuleManager; }
        }
        /// <summary>
        /// 获取初始化任务管理 <see cref="ICustomTaskManager{T}"/>
        /// </summary>
        public ICustomTaskManager<InitTaskResult> InitTaskManager
        {
            get { return InterfaceMagager.InitTaskManager; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public BootStrapperBase(string processName)
        {
            FixPopupBug();
            _processName = processName;
            CurrentApp.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            CurrentApp.DispatcherUnhandledException += CurrentApp_DispatcherUnhandledException;
            CurrentApp.Exit += CurrentApp_Exit;

            // 注册模块事件
            ModuleManager.InitModuleCompleted += ModuleManager_InitModuleCompleted;
            ModuleManager.LoadModuleCompleted += ModuleManager_LoadModuleCompleted;
        }

        private static void FixPopupBug()
        {
            var ifLeft = SystemParameters.MenuDropAlignment;
            if (ifLeft)
            {
                var t = typeof(SystemParameters);
                var field = t.GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
                field?.SetValue(null, false);
            }
        }

        /// <summary>
        /// 应用程序关闭时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentApp_Exit(object sender, ExitEventArgs e)
        {
            // 程序关闭释放模块资源
            foreach (var module in ModuleManager.ModuleInfoSource)
            {
                if (module.State == ModuleState.Initialized)
                    module.Module.FinalizeResource();
            }
        }

        /// <summary>
        /// 将 <see cref="CustomTask{T}"/> 添加到初始化列表的指定位置
        /// </summary>
        /// <param name="task">一个<see cref="CustomTask{T}"/>类型实例</param>
        /// <param name="index">插入位置</param>
        public void AddInitTask(CustomTask<InitTaskResult> task, int index)
        {
            if (task == null)
                return;

            InitTaskManager.AddInitTask(task, index);
        }

        /// <summary>
        /// 将 <see cref="CustomTask{T}"/> 添加到初始化列表的结尾
        /// </summary>
        /// <param name="task">一个<see cref="CustomTask{T}"/>类型实例</param>
        public void AddInitTask(CustomTask<InitTaskResult> task)
        {
            if (task == null)
                return;

            InitTaskManager.AddInitTask(task);
        }

        /// <summary>
        /// 应用程序发生未处理的异常时执行（只捕获发生在主线程的异常）
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">异常数据</param>
        private void CurrentApp_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // 基类只记录异常日志，如何通知用户留给子类实现
            DispatcherUnhandledException(sender, e);
            e.Handled = true;
        }


        /// <summary>
        /// 模块加载后触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleManager_LoadModuleCompleted(object sender, ModuleCompletedEventArgs e)
        {
            LoadModuleCompleted(e.ModuleInfo, e.Error);
        }

        /// <summary>
        /// 模块初始化后触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleManager_InitModuleCompleted(object sender, ModuleCompletedEventArgs e)
        {
            InitModuleCompleted(e.ModuleInfo, e.Error);
        }

        private static Semaphore singleInstanceWatcher;
        /// <summary>
        /// 判断程序是否新建
        /// </summary>
        private static bool createdNew;
        /// <summary>
        /// 启动
        /// </summary>
        public void Run()
        {
            // Initial count.   // Maximum count.   //_processName.   //createdNew.
            singleInstanceWatcher = new Semaphore(0, 1, _processName, out createdNew);

            if (createdNew)
            {
                RunCore();
            }
            else
            {
                //如果程序已打开 将显示原程序   并在OnStartup中对第二次打开操作进行退出
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(_processName))
                {
                    if (process.Id != current.Id)
                    {
                        NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                        NativeMethods.ShowWindow(process.MainWindowHandle, WindowShowStyle.Show);//原程序将以某种方式显示：最大、最小、默认...
                        break;
                    }
                }
                CurrentApp.Shutdown();
            }
        }

        /// <summary>
        /// 应用程序启动流程
        /// </summary>
        protected abstract void RunCore();

        /// <summary>
        /// 创建自定义模块 
        /// </summary>
        public virtual void AddModuleInfo(ModuleInfo moduleInfo)
        { }
        /// <summary>
        /// 模块加载后执行此方法
        /// </summary>
        /// <param name="moduleInfo">执行加载的模块信息</param>
        /// <param name="error">加载时发生的异常，如果未发生异常则未 NULL</param>
        protected virtual void LoadModuleCompleted(ModuleInfo moduleInfo, Exception error)
        { }
        /// <summary>
        /// 模块初始化后执行此方法
        /// </summary>
        /// <param name="moduleInfo">执行初始化的模块信息</param>
        /// <param name="error">初始化发生的异常，如果未发生异常则未 NULL</param>
        protected virtual void InitModuleCompleted(ModuleInfo moduleInfo, Exception error)
        { }
        /// <summary>
        /// 应用程序接收到未处理的异常时调用
        /// <para>基类已将异常记录到日志</para>
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">异常数据</param>
        protected virtual void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        { }

        /// <summary>
        /// 切换用户，继承类实现业务
        /// </summary>
        protected virtual void ChangeUserCore()
        { }

        /// <summary>
        ///   切换用户
        /// </summary>
        public virtual void ChangeUser(bool isAuto = false)
        {
            if (isUserChanging) return;

            isUserChanging = true;
            if (UserChanging != null)
            {
                var args = new UserChangeEventArgs();
                args.IsAutoAction = isAuto;
                UserChanging(this, args);
                if (args.Cancel)
                {
                    isUserChanging = false;
                    return;
                }
            }
            ChangeUserCore();
            isUserChanging = false;
        }

        public virtual void WindowCreated()
        {
            EventHandler eventHandler = MianWindowCreated;
            if (eventHandler != null)
            {
                eventHandler(this, new EventArgs());
            }
        }

        /// <summary>
        /// 关闭程序
        /// </summary>
        public bool Shutdown()
        {
            if (isClosing) return false;

            isClosing = true;
            if (AppClosing != null)
            {
                var args = new AppCancelEventArgs();
                AppClosing(this, args);
                if (args.Cancel)
                {
                    isClosing = false;
                    return false;
                }
            }
            return ShutdownCore();
        }

        /// <summary>
        /// 结束当前应用程序
        /// </summary>
        public bool ShutdownCore()
        {
            CurrentApp.Dispatcher.Invoke(() =>
            {
                //CurrentApp.Shutdown();
                //LogHelper.logSoftWare.Info("退出Environment.Exit(0)开始");
                //Environment.Exit(0);
                LogHelper.logSoftWare.Info("退出Process.GetCurrentProcess().Kill()开始");
                Process.GetCurrentProcess().Kill();
                //Environment.Exit(0);
                //LogHelper.logSoftWare.Info("退出Environment.Exit(0)结束");
            });
            return true;
        }
        /// <summary>
        /// 记录文件日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        protected void Log(string message, Exception ex)
        {
            LogHelper.logSoftWare.Error(message, ex);
        }

        /// <summary>
        /// 设置默认页面名称
        /// </summary>
        /// <param name="viewName"></param>
        public virtual void SetDefaultView(string viewName)
        {

        }
    }
    internal enum WindowShowStyle : uint
    {
        /// <summary>
        /// 隐藏窗口并激活其他窗口
        /// </summary>
        Hide = 0,
        /// <summary>
        /// 激活并显示一个窗口。如果窗口被最大化或最小化，系统将其恢复到原来的尺寸和大小。
        /// </summary>
        ShowNormal = 1,
        /// <summary>
        /// 激活窗体并将其最小化
        /// </summary>
        ShowMinimized = 2,
        /// <summary>
        /// 激活窗体并将其最大化
        /// </summary>
        ShowMaximized = 3,
        /// <summary>
        /// 以窗口最近一次的大小和状态显示窗口。此值与SW_SHOWNORMAL相似，只是窗口没有被激活
        /// </summary>
        ShowNoActivate = 4,
        /// <summary>
        /// 在窗口原来的位置以原来的尺寸激活和显示窗口   //若使用此枚举   窗体隐藏时不弹出显示
        /// </summary>
        Show = 5,
        /// <summary>
        /// 最小化指定的窗口并且激活在Z序中的下一个顶层窗口
        /// </summary>
        Minimize = 6,
        /// <summary>
        /// 最小化的方式显示窗口，此值与SW_SHOWMINIMIZED相似，只是窗口没有被激活
        /// </summary>
        ShowMinNoActivate = 7,
        /// <summary>
        /// 以窗口原来的状态显示窗口。此值与SW_SHOW相似，只是窗口没有被激活
        /// </summary>
        ShowNA = 8,
        /// <summary>
        /// 激活并显示窗口  如果窗口最大化或最小化 则系统将窗口恢复到原来的尺寸和位置  在恢复最小化窗口时，应用程序应该指定这个标志 
        /// </summary>
        Restore = 9,
        /// <summary>
        /// 依据在StartupInfo结构中指定的Flag标志设定显示状态  StartupInfo结构是有启动应用程序的程序传递给CreateProcess函数的
        /// </summary>
        ShowDefault = 10,
        /// <summary>
        /// 在WindowNT5.0中最小化窗口，即使拥有窗口的线程被挂起也会最小化
        /// </summary>
        ForceMinimized = 11
    }
    static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd,
            WindowShowStyle nCmdShow);
    }
}
