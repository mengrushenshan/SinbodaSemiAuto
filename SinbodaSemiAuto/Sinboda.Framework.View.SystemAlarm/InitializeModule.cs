using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.View.SystemAlarm
{
    /// <summary>
    /// 实现模块初始化接口类
    /// </summary>
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
            };
        }
    }
}
