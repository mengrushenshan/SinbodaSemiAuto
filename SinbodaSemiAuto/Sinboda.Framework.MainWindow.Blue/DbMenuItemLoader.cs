using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using Sinboda.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.MainWindow.Blue
{
    /// <summary>
    /// 从 Db 文件中加载 <see cref="ModuleMenuItem"/> 类型集合
    /// </summary>
    public class DbMenuItemLoader : IMenuItemLoader
    {
        /// <summary>
        /// 创建菜单数据源
        /// </summary>
        /// <returns></returns>
        public List<ModuleMenuItem> CreateMenuItemSource()
        {
            // TODO：只有模块状态为成功才能显示菜单项
            IPermission permission = new PermissionOperation();
            var list = permission.GetModuleMenuItemList(SystemResources.Instance.CurrentUserName);
            CheckModuleState(list);
            return list;
        }

        /// <summary>
        /// 检查菜单模块状态
        /// </summary>
        /// <param name="modules"></param>
        private void CheckModuleState(List<ModuleMenuItem> modules)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].ChildMenus.Count > 0)
                {
                    CheckModuleState(modules[i].ChildMenus);
                }
                else
                {
                    // 只检查子菜单模块状态
                    ModuleInfo mInfo = InterfaceMagager.ModuleManager.FindModuleInfo(modules[i].ModuleName);
                    if (mInfo == null || mInfo.State != ModuleState.Initialized)
                    {
                        modules.Remove(modules[i]);
                        continue;
                    }
                }
            }
        }
    }
}
