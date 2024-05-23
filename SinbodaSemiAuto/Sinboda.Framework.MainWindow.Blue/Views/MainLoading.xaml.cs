using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.MainWindow.Blue.ViewModels;
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
    /// MainLoading.xaml 的交互逻辑
    /// </summary>
    public partial class MainLoading : Window
    {
        private MainLoadingViewModel vm;

        /// <summary>
        /// 
        /// </summary>
        public MainLoading()
        {
            InitializeComponent();
            vm = DataContext as MainLoadingViewModel;
            Title = SystemResources.Instance.AnalyzerInfoName;
        }
        /// <summary>
        /// 设定最大值
        /// </summary>
        /// <param name="count"></param>
        public void SetMaximum(int count)
        {
            vm.Maximum = count;
        }
        /// <summary>
        /// 设置描述
        /// </summary>
        /// <param name="taskDescribe"></param>
        public void SetTaskDescribe(string taskDescribe)
        {
            vm.TaskDescribe = taskDescribe;
        }
        /// <summary>
        /// 值增长
        /// </summary>
        public void Increment()
        {
            vm.CurrentValue = vm.CurrentValue + 1;
        }
    }
}
