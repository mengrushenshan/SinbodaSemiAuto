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

namespace Sinboda.Framework.View.SystemSetup.View
{
    /// <summary>
    /// SysPermissionManageSettingPageView.xaml 的交互逻辑
    /// </summary>
    public partial class SysPermissionManageSettingPageView : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public SysPermissionManageSettingPageView()
        {
            InitializeComponent();
        }

        private void PermissionSet_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SysPermissionManageSettingViewModel model = this.DataContext as ViewModel.SysPermissionManageSettingViewModel;
            if (model != null)
            {
                model.GetRoleList();
            }
        }
    }
}
