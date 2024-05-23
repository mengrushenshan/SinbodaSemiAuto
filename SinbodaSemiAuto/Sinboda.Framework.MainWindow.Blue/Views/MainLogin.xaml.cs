using Sinboda.Framework.Core.StaticResource;
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

namespace Sinboda.Framework.MainWindow.Blue.Views
{
    /// <summary>
    /// MainLogin.xaml 的交互逻辑
    /// </summary>
    public partial class MainLogin : Window
    {
        private static readonly MainLogin current = new MainLogin();
        /// <summary>
        /// 
        /// </summary>
        public static MainLogin Current
        {
            get { return current; }
        }
        public MainLogin()
        {
            InitializeComponent();
            Title = SystemResources.Instance.AnalyzerInfoName;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
