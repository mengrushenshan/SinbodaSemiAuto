using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using Sinboda.Framework.Business.SystemSetup;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using Sinboda.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// SystemMenuView.xaml 的交互逻辑
    /// </summary>
    public partial class SystemMenuView : UserControl
    {
        /// <summary>
        /// 标识 <seealso cref="MaxMenuNumber"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty MaxMenuNumberProperty =
            DependencyProperty.Register("MaxMenuNumber", typeof(int), typeof(SystemMenuView), new PropertyMetadata(8, MaxMenuNumberChanged));

        /// <summary>
        /// 标识 <seealso cref="SelectedItem"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(ModuleMenuItem), typeof(SystemMenuView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 获取或设置系统显示的最大菜单数（默认值 8）
        /// </summary>
        public int MaxMenuNumber
        {
            get { return (int)GetValue(MaxMenuNumberProperty); }
            set { SetValue(MaxMenuNumberProperty, value); }
        }

        /// <summary>
        /// 获取或设置系统菜单选中项
        /// </summary>
        public ModuleMenuItem SelectedItem
        {
            get { return (ModuleMenuItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// 系统菜单
        /// </summary>
        public ObservableCollection<ModuleMenuItem> ModuleMenuItemSource { get; set; }

        /// <summary>
        /// 获取关联的 <see cref="IMenuItemLoader"/>
        /// </summary>
        public IMenuItemLoader MenuItemLoader
        {
            get { return ServiceLocator.Current.GetInstance<IMenuItemLoader>(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public SystemMenuView()
        {
            InitializeComponent();
            Messenger.Default.Register<bool>(this, "ClearTopMenuSelectedStatus", ClearTopMenuSelectedStatus);
        }

        private void SystemMenuView_Loaded(object sender, RoutedEventArgs e)
        {
            // 菜单重复显示问题 haosd 2019/05/17
            lbMenu.Items.Clear();
            cboMore.Items.Clear();

            //2020.01.13  讨论决定右侧留出100的空间
            double width = ActualWidth - 100;

            //if (width < 800 || width > 1100)
            //{

            //}
            int count = (int)Math.Round((width - 100) / 100, 0);
            MaxMenuNumber = count - 1;

            int? nMaxMenu = LoginSettingBusiness.Instance.GetMaxMenuCount();
            if (null == nMaxMenu)
            {
                LoginSettingBusiness.Instance.SetMaxMenuCount(MaxMenuNumber);
            }
            else
            {
                MaxMenuNumber = nMaxMenu.Value > MaxMenuNumber ? MaxMenuNumber : nMaxMenu.Value;
            }

            if (DesignHelper.IsInDesignMode) return;

            var sysMenuList = MenuItemLoader.CreateMenuItemSource();
            NavigationHelper.Cuurrent.SetSystemMenus(sysMenuList);
            NavigationHelper.Cuurrent.CreateNavigationItemSource(sysMenuList);

            //找到为上方菜单的菜单，同时过滤为显示的
            var defauleMenus = sysMenuList.Where(o => o.ModuleType == 0 && o.IsMenuShow == true).ToList();

            for (int i = 0; i < defauleMenus.Count; i++)
            {
                if (i < MaxMenuNumber)
                    lbMenu.Items.Add(defauleMenus[i]);
                else
                {
                    if (cboMore.Visibility == Visibility.Collapsed)
                        cboMore.Visibility = Visibility.Visible;

                    cboMore.Items.Add(defauleMenus[i]);
                }
            }

            if (string.IsNullOrEmpty(NavigationHelper.Cuurrent.DefaultViewName))
                NavigationServiceExBase.CurrentService.GoHome();
            else
                NavigationServiceExBase.CurrentService.GoDefaultView();
        }

        private static void MaxMenuNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        { }
        /// <summary>
        /// 选中变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
                return;

            var element = sender as ItemsControl;
            if (element.Name == "cboMore")
                lbMenu.SelectedIndex = -1;
            else
                cboMore.SelectedIndex = -1;

            SelectedItem = e.AddedItems[0] as ModuleMenuItem;
        }

        public void Select(string id)
        {
            foreach (var item in lbMenu.Items)
            {
                var i = item as ModuleMenuItem;
                if (i == null)
                    continue;

                if (i.Id.Equals(id))
                {
                    lbMenu.SelectedItem = i;
                    return;
                }
            }

            foreach (var item in cboMore.Items)
            {
                var i = item as ModuleMenuItem;
                if (i == null)
                    continue;

                if (i.Id.Equals(id))
                {
                    cboMore.SelectedItem = i;
                    return;
                }
            }
        }

        /// <summary>
        /// 清空菜单选中
        /// </summary>
        /// <param name="flag"></param>
        public void ClearTopMenuSelectedStatus(bool flag)
        {
            lbMenu.SelectedIndex = -1;
            cboMore.SelectedIndex = -1;
        }
    }
}
