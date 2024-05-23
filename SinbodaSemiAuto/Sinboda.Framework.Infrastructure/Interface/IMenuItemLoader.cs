using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 提供加载 <see cref="ModuleMenuItem"/> 类型集合的所需的方法
    /// </summary>
    public interface IMenuItemLoader
    {
        /// <summary>
        /// 创建 <see cref="ModuleMenuItem"/> 集合
        /// </summary>
        /// <returns></returns>
        List<ModuleMenuItem> CreateMenuItemSource();
    }
}
