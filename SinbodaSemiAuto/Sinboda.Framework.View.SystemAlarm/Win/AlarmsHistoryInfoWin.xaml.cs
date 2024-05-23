using Sinboda.Framework.Control.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sinboda.Framework.View.SystemAlarm.Win
{
    /// <summary>
    /// AlarmsHistoryInfoWin.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmsHistoryInfoWin : SinWindow
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AlarmsHistoryInfoWin()
        {
            InitializeComponent();
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
    }
}
