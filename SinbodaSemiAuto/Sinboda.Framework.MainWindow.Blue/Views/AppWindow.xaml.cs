using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Control.Controls.Navigation;
using Sinboda.Framework.Control.Utils;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.MainWindow.Blue.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Sinboda.Framework.MainWindow.Blue.Views
{
    /// <summary>
    /// AppWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AppWindow : Window
    {
        private static AppWindow current = new AppWindow();

        /// <summary>
        /// 是否正在切换用户
        /// </summary>
        private bool isUserChanging = false;

        /// <summary>
        /// 唯一主窗体
        /// </summary>
        public static AppWindow Current
        {
            get
            {
                if (current == null)
                    current = new AppWindow();
                return current;
            }
            private set { current = value; }
        }

        public AppWindow()
        {
            InitializeComponent();

            NavigationServiceExBase.CurrentService.Frame = frame;
            DataContext = new AppWindowViewModel();
            Closing += AppWindow_Closing;
            PreviewKeyDown += AppWindow_PreviewKeyDown;
            Title = SystemResources.Instance.AnalyzerInfoName;
            InitMessageWindow();

        }

        /// <summary>
        /// 屏蔽组合键功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyStates == Keyboard.GetKeyStates(Key.F) && Keyboard.Modifiers == ModifierKeys.Control)//屏蔽Control+F
                e.Handled = true;
        }

        internal void HideEx()
        {
            isUserChanging = true;
            Current.Close();
            Current = null;
        }

        /// <summary>
        /// 切换用户
        /// </summary>
        internal void ShowEx()
        {
            Current.Show();
            isUserChanging = false;
        }



        /// <summary>
        /// 程序关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isUserChanging) return;

            e.Cancel = !BootStrapper.Current.Shutdown();
        }
        /// <summary>
        /// 导航失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            // 直接显示错误界面
            AppError error = new AppError();
            error.SetErrorText(e.Exception.ToString());
            NavigationServiceExBase.CurrentService.Frame.Navigate(error, null);
        }
        /// <summary>
        /// 导航结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frame_Navigated(object sender, NavigatedEventArgs e)
        {
            if (e.Source == null)
                return;

            var source = e.Source as NavigationItem;
            if (source == null)
                return;

            if (source.ModuleType != 0)
                Messenger.Default.Send(true, "ClearTopMenuSelectedStatus");
            if (source.ModuleType == -1)
                Messenger.Default.Send<string>("HomePageMessage", "HomePageMessage");
            if (!string.IsNullOrEmpty(source.Id))
            {
                // 如果当前页面与菜单选中项不一致
                if (upMenus.SelectedItem == null || upMenus.SelectedItem.Id != source.Id)
                {
                    upMenus.Select(source.Id);
                }

            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null && menuItem.Header != null)
            {
                OperationLogBusiness.Instance.WriteOperationLogToDb(menuItem.Header.ToString(), 1);
            }
        }
        MessageWindow messageWin = null;
        private void InitMessageWindow()
        {
            if (messageWin == null)
            {
                messageWin = new MessageWindow();
                messageWin.Show();
            }
        }

        private void CommandBinding_ShowHelp(object sender, ExecutedRoutedEventArgs e)
        {
            ((dynamic)this.DataContext).ShowHelpByKeyF1();
        }
    }
}
