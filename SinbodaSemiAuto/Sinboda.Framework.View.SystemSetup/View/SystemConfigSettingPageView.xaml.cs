using Sinboda.Framework.View.SystemSetup.ViewModel;
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
    /// SystemConfigSettingPageView.xaml 的交互逻辑
    /// </summary>
    public partial class SystemConfigSettingPageView : UserControl
    {
        private object selectedItem = null;

        /// <summary>
        /// 
        /// </summary>
        public SystemConfigSettingPageView()
        {
            InitializeComponent();
            InitDefaultModel();
        }
        private void InitDefaultModel()
        {
            if (listView.Items.Count > 0)
            {
                listView.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// 左侧菜单选中后界面显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SystemConfigSettingViewModel model = this.DataContext as SystemConfigSettingViewModel;
            if (model != null)
            {
                selectedItem = listView.SelectedItem;
                UserControl uc = model.InitTreeItemUserControl(listView.SelectedItem) as UserControl;
                uc.Margin = new Thickness(10, 10, 0, 0);
                setupBorder.Child = uc;
            }
        }

        private void systemSetting_Loaded(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null)
            {
                listView_SelectionChanged(this, null);
            }
        }
    }
}
