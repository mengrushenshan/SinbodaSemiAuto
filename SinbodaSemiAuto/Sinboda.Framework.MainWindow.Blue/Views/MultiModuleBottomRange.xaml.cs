using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Sinboda.Framework.MainWindow.Blue.Views
{
    /// <summary>
    /// MultiModuleBottomRange.xaml 的交互逻辑
    /// </summary>
    public partial class MultiModuleBottomRange : UserControl
    {
        private Timer timer;
        /// <summary>
        /// 
        /// </summary>
        public MultiModuleBottomRange()
        {
            Unloaded += MultiModuleBottomRange_Unloaded;
            InitializeComponent();

            if (timer != null)
            {
                LogHelper.logSoftWare.Debug($"重置定时器 {DateTime.Now.ToString()}");
                timer.Dispose();
                timer = null;
            }
            timer = new Timer(RefTime, null, 0, 1000);
        }

        private void MultiModuleBottomRange_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 注销后关闭定时器
                if (timer != null)
                    timer.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Debug("图标刷新注销消息：" + ex);
            }
        }

        private DateTime lastDataTime = DateTime.Now;
        private void RefTime(object s)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // 跨夜刷新样本
                if (lastDataTime.Day != DateTime.Now.Day)
                {
                    //1、刷新时钟显示
                    lastDataTime = DateTime.Now;
                }


                DateTime newDate = DateTime.Now;
                txtTime.Text = newDate.ToLongTimeString();
                txtDate.Text = newDate.ToShortDateString();
            }));

        }
    }
}
