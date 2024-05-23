
using Sinboda.Framework.Common.Log;
using Sinboda.SemiAuto.Core.Helpers;
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
            DataContext = vm = new MachineryDebugPageViewModel();
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                LogHelper.logSoftWare.Info(e);
            }
            else
            {
                LogHelper.logSoftWare.Info(e);
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (MouseKeyBoardHelper.Is_VK_LEFT_Down())
            {
                LogHelper.logSoftWare.Info(e);
            }
            else
            {
                LogHelper.logSoftWare.Info(e);
            }

        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
