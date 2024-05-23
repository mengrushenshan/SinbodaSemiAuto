using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Control.Controls.Navigation
{
    /// <summary>
    /// 导航菜单项
    /// </summary>
    public class NavigationItem
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 导航源
        /// </summary>
        public object Source { get; set; }
        /// <summary>
        /// 模块名
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 菜单类型
        /// </summary>
        public int ModuleType { get; set; }
        /// <summary>
        /// 上级导航项
        /// </summary>
        public NavigationItem ParentItem { get; set; }
        /// <summary>
        /// 导航时的参数
        /// </summary>
        public object NavigationParameter { get; set; }
        /// <summary>
        /// 子导航项
        /// </summary>
        public List<NavigationItem> ChildMenus { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public NavigationItem()
        {
            ChildMenus = new List<NavigationItem>();
        }

        /// <summary>
        /// 批量添加子项
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(NavigationItem item)
        {
            item.ParentItem = this;
            ChildMenus.Add(item);
        }
    }
}
