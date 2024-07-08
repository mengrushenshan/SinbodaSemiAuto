using Sinboda.Framework.Control.Controls;
using Sinboda.SemiAuto.View.ViewModels;
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

namespace Sinboda.SemiAuto.View.WinView
{
    /// <summary>
    /// StartTestFlowWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartTestFlowWindow : SinWindow
    {
        public StartTestFlowWindow()
        {
            InitializeComponent();
            DataContext = new StartTestFlowViewModel();
        }

        private void SinButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
