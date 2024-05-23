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
    /// SoftWareCommonSettingPageView.xaml 的交互逻辑
    /// </summary>
    public partial class SoftWareCommonSettingPageView : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public SoftWareCommonSettingPageView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var context = DataContext as SoftWareCommonSettingViewModel;
                context.Init();
            };
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Process p = new Process();
            //string connectStr = ConfigurationManager.ConnectionStrings["DBConnectionStr"].ConnectionString;
            //string name = string.Empty;
            //string[] splitConnectStr = connectStr.Split(';');
            //foreach (var item in splitConnectStr)
            //{
            //    if (item.Split('=')[0].ToLower() == "database")
            //    {
            //        name = item.Split('=')[1];
            //    }
            //}
            //string location = backup_location.Text;
            //string arguments = string.Empty;
            //arguments = name + " " + location;
            //ProcessStartInfo process = new ProcessStartInfo(@"F:\Desktop\backup_database.bat", arguments);
            //p.StartInfo = process;
            //p.Start();
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            //Process p = new Process();
            //string connectStr = ConfigurationManager.ConnectionStrings["DBConnectionStr"].ConnectionString;
            //string name = string.Empty;
            //string[] splitConnectStr = connectStr.Split(';');
            //foreach (var item in splitConnectStr)
            //{
            //    if (item.Split('=')[0].ToLower() == "database")
            //    {
            //        name = item.Split('=')[1];
            //    }
            //}
            //string location = rebackup_location.Text;
            //string arguments = string.Empty;
            //arguments = name + " " + location;
            //ProcessStartInfo process = new ProcessStartInfo(@"F:\Desktop\rebackup_database.bat", arguments);
            //p.StartInfo = process;
            //BootStrapper.Current.Shutdown();
            //p.Start();
        }
    }
}

