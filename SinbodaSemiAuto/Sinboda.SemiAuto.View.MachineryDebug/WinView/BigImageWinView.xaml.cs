using Sinboda.Framework.Control.Controls;
using Sinboda.SemiAuto.Core.Models;
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

namespace Sinboda.SemiAuto.View.MachineryDebug.WinView
{
    /// <summary>
    /// BigImageWinView.xaml 的交互逻辑
    /// </summary>
    public partial class BigImageWinView : SinWindow
    {
        MachineryDebugPageViewModel vm;
        public BigImageWinView(MachineryDebugPageViewModel viewModel)
        {
            this.PreviewMouseWheel += img_PreviewMouseWheel;
            this.PreviewKeyDown += Grid_PreviewKeyDown;
            this.PreviewKeyUp += Grid_PreviewKeyUp;
            DataContext = vm = viewModel;
            InitializeComponent();
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                vm.MWinKeyEvent(keyEvent);
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
                vm.MWinKeyEvent(keyEvent);
            }
        }
    }
}
