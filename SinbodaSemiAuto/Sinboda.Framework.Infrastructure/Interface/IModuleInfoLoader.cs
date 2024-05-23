using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    ///提供加载 <see cref="IModuleInfoLoader"/> 类型集合的所需的方法
    /// </summary>
    public interface IModuleInfoLoader
    {
        /// <summary>
        /// 创建 <see cref="ModuleInfo"/> 集合
        /// </summary>
        /// <returns></returns>
        IEnumerable<ModuleInfo> CreateModuleInfoSource();
    }
}
