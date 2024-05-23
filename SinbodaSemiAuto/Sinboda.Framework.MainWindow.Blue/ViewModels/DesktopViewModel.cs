using Microsoft.Practices.ServiceLocation;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.MainWindow.Blue.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class DesktopViewModel : NavigationViewModelBase
    {
        /// <summary>
        /// 菜单集合
        /// </summary>
        public ObservableCollection<ModuleMenuItem> MenuItemSource { get; set; }

        private ModuleMenuItem _SelectedItem;
        /// <summary>
        /// 选中菜单
        /// </summary>
        public ModuleMenuItem SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                Set(ref _SelectedItem, value);
                Navigate(SelectedItem.Id);
            }
        }

        /// <summary>
        /// 获取关联的 <see cref="IMenuItemLoader"/>
        /// </summary>
        public IMenuItemLoader MenuItemLoader
        {
            get { return ServiceLocator.Current.GetInstance<IMenuItemLoader>(); }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public DesktopViewModel()
        {
            MenuItemSource = new ObservableCollection<ModuleMenuItem>();

        }
        /// <summary>
        /// 参数变更
        /// </summary>
        /// <param name="parameter"></param>
        protected override void OnParameterChanged(object parameter)
        {

            ModuleMenuItem menuItem = parameter as ModuleMenuItem;
            if (menuItem == null) return;

            MenuItemSource.Clear();
            foreach (var item in menuItem.ChildMenus)
            {
                MenuItemSource.Add(item);
            }
        }
    }
}
