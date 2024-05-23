using Sinboda.Framework.Control.Controls.Navigation;
using Sinboda.Framework.Core.Interface;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Services
{
    /// <summary>
    /// <see cref="INavigationServiceEx"/> 实现的基类
    /// </summary>
    public class NavigationServiceExBase : INavigationServiceEx
    {
        /// <summary>
        /// 
        /// </summary>
        public static NavigationServiceExBase CurrentService = new NavigationServiceExBase();

        private NavigationServiceExBase()
        { }

        /// <summary>
        /// 
        /// </summary>
        public INavigationFrame Frame { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool CanGoBack
        {
            get { return Frame.CanGoBack; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool CanGoForward
        {
            get { return Frame.CanGoForward; }
        }
        /// <summary>
        /// 
        /// </summary>
        public object Current
        {
            get { return NavigationJournal.Current; }
        }
        /// <summary>
        /// 
        /// </summary>
        public IJournal NavigationJournal
        {
            get { return Frame.Journal; }
        }
        /// <summary>
        /// 清空导航历史
        /// </summary>
        public void ClearNavigationHistory()
        {
            NavigationJournal.ClearNavigationHistory();
        }
        /// <summary>
        /// 清空导航缓存
        /// </summary>
        public void ClearNavigationCache()
        {
            NavigationJournal.ClearNavigationCache();
        }
        /// <summary>
        /// 下一页
        /// </summary>
        public void GoBack()
        {
            Frame.GoBack();
        }

        /// <summary>
        /// 上一页
        /// </summary>
        public void GoForward()
        {
            Frame.GoForward();
        }

        /// <summary>
        /// 到主页
        /// </summary>
        public void GoHome()
        {
            Navigate(NavigationHelper.Cuurrent.RootItem);
        }

        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="target"></param>
        public void Navigate(NavigationItem target)
        {
            Frame.Navigate(target, null);
        }

        /// <summary>
        /// 导航到默认页（默认页名称必须是菜单项目）
        /// </summary>
        public void GoDefaultView()
        {
            if (string.IsNullOrEmpty(NavigationHelper.Cuurrent.DefaultViewName))
                return;

            var navItem = NavigationHelper.Cuurrent.GetNavigationItem(NavigationHelper.Cuurrent.DefaultViewName);
            Navigate(navItem);
        }
    }

    /// <summary>
    /// 导航帮助类
    /// </summary>
    public class NavigationHelper
    {
        private static readonly NavigationHelper current = new NavigationHelper();
        private static Dictionary<object, NavigationItem> _NavigationPaths = new Dictionary<object, NavigationItem>(); // 缓存个页面的导航路径

        /// <summary>
        /// 当前 <see cref="NavigationHelper"/> 实例
        /// </summary>
        public static NavigationHelper Cuurrent
        {
            get { return current; }
        }

        /// <summary>
        /// 返回 <see cref="IEnumerable{T}"/> 集合系统菜单
        /// </summary>
        public IEnumerable<ModuleMenuItem> SystemMenus { get; private set; }

        /// <summary>
        /// 导航目录根节点
        /// </summary>
        public NavigationItem RootItem { get; private set; }
        /// <summary>
        /// 默认导航项
        /// </summary>
        public string DefaultViewName { get; set; }

        /// <summary>
        /// 创建 <see cref="NavigationItem"/> 导航目录
        /// </summary>
        /// <param name="menuList"></param>
        /// <returns></returns>
        public NavigationItem CreateNavigationItemSource(List<ModuleMenuItem> menuList)
        {
            foreach (var item in menuList)
            {
                var citem = ConvertToNavigationItem(item);
                RootItem.AddItem(citem);
                Insert(citem.Id, citem);
            }
            return RootItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuList"></param>
        public void SetSystemMenus(List<ModuleMenuItem> menuList)
        {
            SystemMenus = menuList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public NavigationItem GetNavigationItem(string key)
        {
            NavigationItem result = null;
            if (_NavigationPaths.TryGetValue(key, out result))
            { }
            return result;
        }

        /// <summary>
        /// 创建根节点
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public NavigationItem CreateRootItem(string viewName, object source)
        {
            RootItem = new NavigationItem { Name = viewName, Source = source, ModuleType = -1 };
            return RootItem;
        }

        private void Insert(object key, NavigationItem item)
        {
            if (!_NavigationPaths.ContainsKey(key))
            {
                _NavigationPaths.Add(key, item);
            }
        }

        /// <summary>
        /// 将 <see cref="ModuleMenuItem"/> 转换为 <see cref="NavigationItem"/>
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns></returns>
        private NavigationItem ConvertToNavigationItem(ModuleMenuItem menuItem)
        {
            NavigationItem item = new NavigationItem
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Source = menuItem.Source,
                ModuleName = menuItem.ModuleName,
                ModuleType = menuItem.ModuleType.GetHashCode()
            };

            if (menuItem.ChildMenus != null && menuItem.ChildMenus.Count > 0)
            {
                foreach (var citem in menuItem.ChildMenus)
                {
                    var navitem = ConvertToNavigationItem(citem);
                    item.AddItem(navitem);
                    Insert(navitem.Id, navitem);
                }
            }
            return item;
        }
    }
}
