using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.MenuItemLoader
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleManageMenuItemLoader : IMenuItemLoader
    {
        /// <summary>
        /// 生成菜单集合
        /// </summary>
        /// <returns></returns>
        public List<ModuleMenuItem> CreateMenuItemSource()
        {
            List<ModuleMenuItem> result = new List<ModuleMenuItem>();

            foreach (var menuItem in InterfaceMagager.ModuleManager.ModuleInfoSource)
            {
                if (menuItem.Module != null && menuItem.State == ModuleState.Initialized)
                {
                    var menus = menuItem.Module.GetMenus();
                    if (menus != null)
                        result.AddRange(menus);
                }
            }
            return result;
        }
    }
}
