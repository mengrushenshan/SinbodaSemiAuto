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
    /// AppError.xaml 的交互逻辑
    /// </summary>
    public partial class AppError : UserControl
    {
        public AppError()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 显示错误
        /// </summary>
        /// <param name="text"></param>
        public void SetErrorText(string text)
        {
            txtMsg.Text = text;
        }
    }
}
