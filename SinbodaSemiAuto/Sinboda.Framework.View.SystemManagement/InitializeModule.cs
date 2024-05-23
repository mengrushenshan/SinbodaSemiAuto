using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.View.SystemManagement
{
    public class InitializeModule : IModule
    {
        /// <summary>
        /// 销毁资源
        /// </summary>
        public void FinalizeResource()
        {
        }
        /// <summary>
        /// 初始化资源
        /// </summary>
        /// <returns></returns>
        public InitTaskResult InitializeResource()
        {
            return new InitTaskResult();
        }
        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <returns></returns>
        public List<ModuleMenuItem> GetMenus()
        {
            return new List<ModuleMenuItem>
            {
                new ModuleMenuItem
                {
                    Id= "89E36254447A499B8BC7839B5AC72C31",
                    ModuleName = "Sinboda.Framework.View.SystemManagement.dll", Name = "系统管理", Source = "Sinboda.Framework.View.SystemManagement.View.SysUserManagePageView",
                    ChildMenus = new List<ModuleMenuItem>
                    {
                        new ModuleMenuItem { ModuleName = "Sinboda.Framework.View.SystemManagement.dll", Name = "权限管理", Source = "Sinboda.Framework.View.SystemManagement.View.SysUserManagePageView",Id="35AED3B077F84077A7EEDACDE5124F90" }
                    }
                },
                new ModuleMenuItem
                {
                    Id="861E09A434C6405FA07950FFC259A962",
                    ModuleName = "Sinboda.Framework.View.SystemManagement.dll", Name = "日志管理", Source = "Sinboda.Framework.View.SystemManagement.View.SysLogPageView",
                    ChildMenus = new List<ModuleMenuItem>
                    {
                        new ModuleMenuItem { ModuleName = "Sinboda.Framework.View.SystemManagement.dll", Name = "权限管理", Source = "Sinboda.Framework.View.SystemManagement.View.SysLogPageView" ,Id="861E09A434C6405FA07950FFC249A932",}
                    }
                },
            };
        }
    }
}
