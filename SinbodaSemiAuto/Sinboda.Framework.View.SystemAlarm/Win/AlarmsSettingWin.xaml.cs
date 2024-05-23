using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.View.SystemAlarm.ViewModel;
using System.Windows;

namespace Sinboda.Framework.View.SystemAlarm.Win
{
    /// <summary>
    /// AlarmsSettingPageView.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmsSettingWin : SinWindow
    {
        AlarmSettingViewModel viewModel = new AlarmSettingViewModel();

        /// <summary>
        /// 报警信息设置界面窗口类
        /// </summary>
        public AlarmsSettingWin()
        {
            InitializeComponent();
            DataContext = viewModel;
        }
        /// <summary>
        /// 关闭按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeVisibleFlagMethod();
        }

        private void CheckSound_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeHaveSoundFlagMethod();
        }
    }
}
