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

namespace Sinboda.SemiAuto.View.Samples.UserControls
{
    /// <summary>
    /// SampleRoundMonitorControl.xaml 的交互逻辑
    /// </summary>
    public partial class SampleRoundMonitorControl : UserControl
    {
        public SampleRoundMonitorControl()
        {
            InitializeComponent();
        }

        public OrderEventHandler orderEventHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SampleControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            orderEventHandler?.Invoke((sender as SampleControl).Tag.ToString());
        }
    }

    /// <summary>
    /// 事件
    /// </summary>
    /// <param name="Tag"></param>
    public delegate void OrderEventHandler(string Tag);
}
