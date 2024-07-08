using Sinboda.Framework.Core.Services;
using Sinboda.SemiAuto.Core.Manager;
using Sinboda.SemiAuto.TestFlow;
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

namespace Sinboda.SemiAuto.View.PageView
{
    /// <summary>
    /// BCAModuleView.xaml 的交互逻辑
    /// </summary>
    public partial class SemiAutoModuleView : ModuleViewUserControl
    {
        public SemiAutoModuleView()
        {
            InitializeComponent();
        }

        protected override void SetModuleContext(object newValue)
        {
            var context = DataContext as SemiAutoModuleContext;
            var setting = ModuleSettingManager.Instance.ModuleSetting.FirstOrDefault(o => o.ModuleId == context.ModuleID);
        }


        /// <summary>
        /// 双击质控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            switch (e.ClickCount)
            {
                case 1:
                    break;
                case 2:
                    var context = DataContext as SemiAutoModuleContext;
                    if (context == null)
                        return;

                    var navItem = NavigationHelper.Cuurrent.GetNavigationItem("QCManagePageView");
                    if (navItem != null)
                    {
                        navItem.NavigationParameter = context.QC_PilotInfo.Tag;
                        NavigationServiceExBase.CurrentService.Navigate(navItem);
                    }
                    break;
            }
        }

        /// <summary>
        /// 双击试剂
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (e.ClickCount)
            {
                case 1:
                    break;
                case 2:
                    var context = DataContext as SemiAutoModuleContext;
                    if (context == null)
                        return;

                    var navItem = NavigationHelper.Cuurrent.GetNavigationItem("ReagentMainPageView");
                    if (navItem != null)
                    {
                        navItem.NavigationParameter = context.QC_PilotInfo.Tag;
                        NavigationServiceExBase.CurrentService.Navigate(navItem);
                    }
                    break;
            }

        }

        /// <summary>
        /// 双击校准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown_2(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (e.ClickCount)
            {
                case 1:
                    break;
                case 2:
                    var context = DataContext as SemiAutoModuleContext;
                    if (context == null)
                        return;

                    var navItem = NavigationHelper.Cuurrent.GetNavigationItem("CalibrationManagePageView");
                    if (navItem != null)
                    {
                        navItem.NavigationParameter = context.QC_PilotInfo.Tag;
                        NavigationServiceExBase.CurrentService.Navigate(navItem);
                    }
                    break;
            }
        }

        /// <summary>
        /// 双击维护
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown_3(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (e.ClickCount)
            {
                case 1:
                    break;
                case 2:
                    var context = DataContext as SemiAutoModuleContext;
                    if (context == null)
                        return;

                    var navItem = NavigationHelper.Cuurrent.GetNavigationItem("Maintaince");
                    if (navItem != null)
                    {
                        navItem.NavigationParameter = context.QC_PilotInfo.Tag;

                        NavigationServiceExBase.CurrentService.Navigate(navItem);
                    }
                    break;
            }
        }

        /// <summary>
        /// 双击耗材监控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown_4(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (e.ClickCount)
            {
                case 1:
                    break;
                case 2:
                    var context = DataContext as SemiAutoModuleContext;
                    if (context == null)
                        return;

                    var navItem = NavigationHelper.Cuurrent.GetNavigationItem("Monitoring");
                    if (navItem != null)
                    {
                        navItem.NavigationParameter = context.Waste_PilotInfo.Tag;
                        NavigationServiceExBase.CurrentService.Navigate(navItem);
                    }
                    break;
            }
        }
    }
}
