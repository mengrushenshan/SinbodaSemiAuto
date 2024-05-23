using Sinboda.Framework.Control.Controls;
using Sinboda.SemiAuto.View.Samples.ViewModel;
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

namespace Sinboda.SemiAuto.View.Samples.WinView
{
    /// <summary>
    /// SampleDeleteWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SampleDeleteWindow : SinWindow
    {
        public SampleDeleteWindow()
        {
            InitializeComponent();
            DataContext = new SampleDeleteViewModel();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
