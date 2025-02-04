﻿using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.View.Samples.UserControls;
using Sinboda.SemiAuto.View.Samples.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;


namespace Sinboda.SemiAuto.View.Samples.PageView
{
    /// <summary>
    /// SamplesRegisterPageView.xaml 的交互逻辑
    /// </summary>
    public partial class SamplesRegisterPageView : UserControl
    {
        SamplesRegisterPageViewModel viewModel;
        SampleRoundMonitorControl96 sampleRoundMonitorControl96;
        public SamplesRegisterPageView()
        {
            InitializeComponent();
            DataContext = viewModel = new SamplesRegisterPageViewModel();
            sampleRoundMonitorControl96 = new SampleRoundMonitorControl96();
            sampleRoundMonitorControl96.DataContext = viewModel;
            sampleRoundMonitorControl96.GetBoard = viewModel.ShowBoardInfo;
            sampleRoundMonitorControl96.SetRowBoard = viewModel.ShowRackBoard;
            sampleRoundMonitorControl96.SetColBoard = viewModel.ShowColBoard;
            sampleRoundMonitorControl96.InitBoardData();
            SampleGrid.Children.Add(sampleRoundMonitorControl96);
            viewModel.RefTemplateBoard = sampleRoundMonitorControl96.SetBoardData;
            viewModel.RefBoardCol = sampleRoundMonitorControl96.SetColData;
            viewModel.RefBoardRow = sampleRoundMonitorControl96.SetRowData;

            this.PreviewMouseWheel += img_PreviewMouseWheel;
            this.PreviewKeyDown += Grid_PreviewKeyDown;
            this.PreviewKeyUp += Grid_PreviewKeyUp;
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
            viewModel.TMainWinMouseWheelEvent(mouseWheel);
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl ||
                e.Key == Key.RightCtrl ||
                e.Key == Key.LeftShift ||
                e.Key == Key.RightShift ||
                e.Key == Key.Left ||
                e.Key == Key.Up ||
                e.Key == Key.Right ||
                e.Key == Key.Down
                )
            {
                KeyBoardEvent keyEvent = new KeyBoardEvent(true, e.Key);
                //键盘事件通知
                viewModel.MWinKeyEvent(keyEvent);
            }
        }

        /// <summary>
        /// 鼠标抬起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl ||
               e.Key == Key.RightCtrl ||
               e.Key == Key.LeftShift ||
               e.Key == Key.RightShift ||
               e.Key == Key.Left ||
               e.Key == Key.Up ||
               e.Key == Key.Right ||
               e.Key == Key.Down
               )
            {
                KeyBoardEvent keyEvent = new KeyBoardEvent(false, e.Key);
                //键盘事件通知
                viewModel.MWinKeyEvent(keyEvent);
            }
        }
    }
}
