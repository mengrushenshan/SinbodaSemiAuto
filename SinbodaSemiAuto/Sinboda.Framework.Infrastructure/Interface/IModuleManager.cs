using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 包含模块的加载，卸载，查询，初始化
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// 各个模块初始化字典
        /// </summary>
        Dictionary<string, IModule> ModuleInitDic { get; set; }
        /// <summary>
        /// 系统设置初始化字典
        /// </summary>
        Dictionary<string, object> ConfigInitDic { get; set; }

        /// <summary>
        /// 模块列表
        /// </summary>
        IEnumerable<ModuleInfo> ModuleInfoSource { get; }
        /// <summary>
        /// 已完成模块加载过程触发
        /// </summary>
        event EventHandler<ModuleCompletedEventArgs> LoadModuleCompleted;
        /// <summary>
        /// 已完成模块初始化过程触发
        /// </summary>
        event EventHandler<ModuleCompletedEventArgs> InitModuleCompleted;
        /// <summary>
        /// 加载模块类型
        /// </summary>
        /// <param name="moduleInfo"></param>
        void LoadModuleType(ModuleInfo moduleInfo);
        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="moduleInfo"></param>
        void AddModule(ModuleInfo moduleInfo);
        /// <summary>
        /// 执行模块初始化
        /// </summary>
        /// <param name="moduleInfo"></param>
        bool InitModule(ModuleInfo moduleInfo);
        /// <summary>
        /// 根据模块名查询 <see cref="ModuleInfo">ModuleInfo</see>
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        ModuleInfo FindModuleInfo(string moduleName);
    }

    /// <summary>
    /// 模块加载，初始化的事件数据
    /// </summary>
    public class ModuleCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// 模块信息
        /// </summary>
        public ModuleInfo ModuleInfo { get; private set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Error { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleInfo"></param>
        /// <param name="error"></param>
        public ModuleCompletedEventArgs(ModuleInfo moduleInfo, Exception error)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException("moduleInfo");

            ModuleInfo = moduleInfo;
            Error = error;
        }
    }
}
