using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 模块接口
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// 初始化资源
        /// </summary>
        InitTaskResult InitializeResource();
        /// <summary>
        /// 释放资源
        /// </summary>
        void FinalizeResource();
        /// <summary>
        /// 获取模块菜单
        /// </summary>
        /// <returns></returns>
        List<ModuleMenuItem> GetMenus();
    }
}
