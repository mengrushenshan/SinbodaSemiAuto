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
    /// SysInfoManagePageView.xaml 的交互逻辑
    /// </summary>
    public partial class SysInfoManagePageView : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SysInfoManagePageView()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                var context = DataContext as SysInfoManageViewModel;
                context.Init();
            };
        }

        private void CheckBox_IsEable_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((SysInfoManageViewModel)this.DataContext).UpdateDataDicIsEnable();
        }

        private void CheckBox_IsDefault_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((SysInfoManageViewModel)this.DataContext).UpdateDataDicIsDefault();
        }

        private void CheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //if(!(((SysInfoManageViewModel)this.DataContext).IsEnable))
            //{
            //    ((SysInfoManageViewModel)this.DataContext).IsDefault = false;
            //}
        }

        private void GridControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((SysInfoManageViewModel)this.DataContext).DoubleClickSelectTypeItem();
        }
    }
}
