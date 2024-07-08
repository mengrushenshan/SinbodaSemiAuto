using Sinboda.Framework.Core.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Interfaces
{
    /// <summary>
    /// 模块管理接口
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="module">模块信息</param>
        /// <returns></returns>
        ModuleInfoModel AddModule(ModuleInfoModel module);


        bool CanTestModule(int moduleId);
    }
}
