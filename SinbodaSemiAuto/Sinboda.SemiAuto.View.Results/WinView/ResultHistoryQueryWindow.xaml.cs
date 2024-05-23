using Sinboda.Framework.Control.Controls;
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
    /// ResultHistoryQueryWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ResultHistoryQueryWindow : SinWindow
    {
        public ResultHistoryQueryWindow(ResultQueryPageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = new ResultHistoryQueryViewModel(viewModel);
        }
    }
}
