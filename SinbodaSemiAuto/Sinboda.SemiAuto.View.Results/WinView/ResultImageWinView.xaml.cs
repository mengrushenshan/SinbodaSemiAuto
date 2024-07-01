using Sinboda.Framework.Control.Controls;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.View.Results.ViewModel;
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

namespace Sinboda.SemiAuto.View.Results.WinView
{
    /// <summary>
    /// ResultImageWinView.xaml 的交互逻辑
    /// </summary>
    public partial class ResultImageWinView : SinWindow
    {
        ResultImageViewModel viewModel;
        public ResultImageWinView(Sin_Sample sample, int imagePos)
        {
            DataContext = viewModel = new ResultImageViewModel(sample, imagePos);
            this.PreviewMouseWheel += img_PreviewMouseWheel;
            InitializeComponent();
        }

        /// <summary>
        /// 鼠标滚轮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 获取滚动方向
            if (e.Delta > 0) // 向上滚动
            {
                viewModel.BeforImage();
            }
            else if (e.Delta < 0) // 向下滚动
            {
                viewModel.NextImage();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
