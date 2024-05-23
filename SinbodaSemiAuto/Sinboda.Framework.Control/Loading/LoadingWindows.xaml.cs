using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Control.ProgressBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Sinboda.Framework.Control.Loading
{
    /// <summary>
    /// 
    /// </summary>
    public class LoadingHelper
    {
        private readonly static LoadingHelper instance = new LoadingHelper();

        /// <summary>
        /// 返回 <see cref="LoadingHelper"/> 的唯一实例
        /// </summary>
        public static LoadingHelper Instance { get { return instance; } }
        private LoadingHelper() { }


        /// <summary>
        /// 显示进度条
        /// </summary>
        /// <param name="action"></param>
        /// <param name="milliseconds">延时（毫秒）</param>
        /// <param name="callback">完成时回调</param>
        public void ShowLoadingWindow(Action<AsynNotify> action, int milliseconds, Action<AsynNotify> callback)
        {
            ShowLoadingWindow(action, new TimeSpan(10000 * milliseconds), callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delay"></param>
        /// <param name="callback"></param>
        public void ShowLoadingWindow(Action<AsynNotify> action, TimeSpan delay, Action<AsynNotify> callback)
        {
            AsynNotify asynNotify = new AsynNotify();
            DispatcherTimer _displayAfterTimer = new DispatcherTimer();
            _displayAfterTimer.Interval = delay;
            _displayAfterTimer.Tick += (s, e) =>
            {
                var timer = s as DispatcherTimer;
                timer.Stop();

                if (LoadingWindows.Instance.IsColsed)
                    LoadingWindows.Instance.CreateLoadingWindows();

                LoadingWindows.Instance.ShowLoading(asynNotify);
            };

            action.BeginInvoke(asynNotify, o =>
            {
                try
                {
                    action.EndInvoke(o);
                }
                catch (Exception ex)
                {
                    LogHelper.logSoftWare.Error($"[ShowLoadingWindow] [异常] {ex.Message}", ex);  // 先写日志，避免出现主线程阻塞无法写日志问题
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SinMessageBox win = new SinMessageBox("Error", ex.Message, SinMessageBoxImage.Error);
                        win.Owner = LoadingWindows.Instance;
                        win.Activate();
                        win.AddButtons(new List<Button> { win.CreateButton("OK", true, false, MessageBoxResult.OK) });
                        win.ShowDialog();
                    });

                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        if (_displayAfterTimer.IsEnabled)
                            _displayAfterTimer.Stop();

                        try
                        {
                            callback(asynNotify);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.logSoftWare.Error($"[ShowLoadingWindow 完成回调] [异常] {ex.Message}", ex);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                SinMessageBox win = new SinMessageBox("Error", ex.Message, SinMessageBoxImage.Error);
                                win.Owner = LoadingWindows.Instance;
                                win.Activate();
                                win.AddButtons(new List<Button> { win.CreateButton("OK", true, false, MessageBoxResult.OK) });
                                win.ShowDialog();
                            });

                        }

                        LoadingWindows.Instance.HideLoading();
                    });
                }
            }, null);
            _displayAfterTimer.Start();
        }
    }


    /// <summary>
    /// LoadingWindows.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingWindows : Window
    {
        private int taskCount = 0;
        private bool isClosed = false;
        private static LoadingWindows instance;
        /// <summary>
        /// 返回 <see cref="LoadingWindows"/> 的唯一实例
        /// </summary>
        public static LoadingWindows Instance
        {
            get { return instance; }
        }

        public static readonly DependencyProperty AsynNotifyProperty
            = DependencyProperty.Register("AsynNotify", typeof(AsynNotify), typeof(LoadingWindows), new FrameworkPropertyMetadata(new AsynNotify()));

        static LoadingWindows()
        {
            instance = new LoadingWindows();
        }

        public void CreateLoadingWindows()
        {
            instance = new LoadingWindows();
        }

        /// <summary>
        /// 是否关闭
        /// </summary>
        public bool IsColsed
        {
            get { return isClosed; }
        }

        public AsynNotify AsynNotify
        {
            get { return (AsynNotify)GetValue(AsynNotifyProperty); }
            set { SetValue(AsynNotifyProperty, value); }
        }

        /// <summary>
        /// 显示的加载信息
        /// </summary>
        public string Text
        {
            get { return txtMessage.Text; }
            set { txtMessage.Text = value; }
        }

        private LoadingWindows()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            DataContext = this;
            ShowActivated = false;
            // Visibility = Visibility.Hidden;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            LogHelper.logSoftWare.Debug($"[LoadingWindows.OnClosed] 关闭等待窗口");
            isClosed = true;
        }

        public void ShowLoading(AsynNotify an)
        {
            try
            {
                Interlocked.Increment(ref taskCount);
                Text = an.Title;
                Debug.WriteLine($"[ShowLoading 计数] {taskCount} {Text}");
                AsynNotify = an;
                progressBar.IsIndeterminate = AsynNotify.Maximum == 0;

                if (Visibility != Visibility.Visible && !isClosed)
                {
                    LogHelper.logSoftWare.Debug($"[ShowDialog 显示等待窗口] 线程={Thread.CurrentThread.ManagedThreadId}");
                    ShowDialog();
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Debug($"[LoadingWindows.ShowLoading] [异常] {ex.Message} 线程={Thread.CurrentThread.ManagedThreadId} Visibility={Visibility} isClosed={isClosed}");
                throw;
            }
        }

        public void HideLoading()
        {
            if (taskCount == 0)
                return;

            Interlocked.Decrement(ref taskCount);
            Debug.WriteLine($"[HideLoading 计数] {taskCount}");
            if (taskCount == 0)
            {
                Hide();
                progressBar.IsIndeterminate = false;
                LogHelper.logSoftWare.Debug($"[Hide 隐藏等待窗口] 线程={Thread.CurrentThread.ManagedThreadId}");
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ControlHelper
    {
        //从Handle中获取Window对象
        private static Window GetWindowFromHwnd(IntPtr hwnd)
        {
            return (Window)HwndSource.FromHwnd(hwnd).RootVisual;
        }

        //GetForegroundWindow API
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        /////调用GetForegroundWindow然后调用GetWindowFromHwnd

        /// <summary>
        /// 获取当前顶级窗体，若获取失败则返回主窗体
        /// </summary>
        public static Window GetTopWindow()
        {
            var hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
                return Application.Current.MainWindow;

            return GetWindowFromHwnd(hwnd);
        }
    }
}
