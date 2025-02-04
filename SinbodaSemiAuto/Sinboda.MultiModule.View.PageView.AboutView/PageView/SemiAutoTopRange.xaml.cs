﻿using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.View.WinView;
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

namespace Sinboda.SemiAuto.View.PageView
{
    /// <summary>
    /// SemiAutoTopRange.xaml 的交互逻辑
    /// </summary>
    public partial class SemiAutoTopRange : UserControl
    {
        public SemiAutoTopRange()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 发送测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StartTestFlowWindow startTestFlowWindow = new StartTestFlowWindow();
            startTestFlowWindow.Show();
        }



        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NotificationService.Instance.ShowQuestion(SystemResources.Instance.GetLanguage(0, "是否停止测试")) == MessageBoxResult.Yes)
            {
                TestFlow.TestFlow.Instance.SetTestIsChannel(true);
            }
        }
    }
}
