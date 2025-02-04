﻿
using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Common.Log;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Core.Resources;
using Sinboda.SemiAuto.View.MachineryDebug.ViewModel;
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

namespace Sinboda.SemiAuto.View.MachineryDebug.PageView
{
    /// <summary>
    /// MachineryDebugPageView.xaml 的交互逻辑
    /// </summary>
    public partial class MachineryDebugPageView : UserControl
    {
        MachineryDebugPageViewModel vm;
        public MachineryDebugPageView()
        {
            InitializeComponent();
            this.PreviewMouseWheel += img_PreviewMouseWheel;
            DataContext = vm = new MachineryDebugPageViewModel();
        }

        /// <summary>
        /// 鼠标滚轮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MouseWheelEvent mouseWheel = null;
            // 获取滚动方向
            if (e.Delta > 0) // 向上滚动
            {
                mouseWheel = new MouseWheelEvent(false, e.Delta);
                //滚轮事件通知
                
            }
            else if (e.Delta < 0) // 向下滚动
            {
                mouseWheel = new MouseWheelEvent(true, e.Delta);
                //滚轮事件通知
            }
            vm.TMainWinMouseWheelEvent(mouseWheel);
        }

        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image im = sender as Image;
            Point point = e.GetPosition(im);
            if (point == null) return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                vm.SetRoiRange(point, true);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                vm.SetRoiRange(point, false);
            }
            
        }

        /// <summary>
        /// x轴一直左移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnXmotorLeft_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.AlwaysLeftMove(vm.MotorList[0]);
        }

        /// <summary>
        /// x轴一直右移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnXmotorRight_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.AlwaysRightMove(vm.MotorList[0]);
        }

        /// <summary>
        /// y轴一直左移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnYmotorLeft_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.AlwaysLeftMove(vm.MotorList[1]);
        }

        /// <summary>
        /// y轴一直右移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnYmotorRight_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.AlwaysRightMove(vm.MotorList[1]);
        }

        /// <summary>
        /// z轴一直左移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZmotorLeft_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.AlwaysLeftMove(vm.MotorList[2]);
        }

        /// <summary>
        /// z轴一直右移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZmotorRight_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.AlwaysRightMove(vm.MotorList[2]);
        }
    }
}
