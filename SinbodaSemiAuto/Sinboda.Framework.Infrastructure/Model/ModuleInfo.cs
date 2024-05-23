using Sinboda.Framework.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Model
{
    /// <summary>
    /// 表示一个模块的基本信息
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// 加载地址
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// 模块初始化状态
        /// </summary>
        public ModuleState State { get; internal set; }

        /// <summary>
        /// 模块版本号（模块必须初始化完成）
        /// </summary>
        public string Version
        {
            get
            {
                if (ModuleAssembly != null)
                {
                    return ModuleAssembly.GetName().Version.ToString();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 模块程序集
        /// </summary>
        public Assembly ModuleAssembly { get; internal set; }

        /// <summary>
        /// 模块初始化
        /// </summary>
        public IModule Module { get; internal set; }

        /// <summary>
        /// 创建一个 ModuleInfo
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="type">类型名称</param>
        /// <param name="_ref">加载地址</param>
        /// <param name="initialize">模块初始化状态</param>
        public ModuleInfo(string name, string type, string _ref, Func<bool> initialize = null)
        {
            ModuleName = name;
            ModuleType = type;
            Ref = _ref;
            State = ModuleState.NotStarted;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ModuleInfo()
        {

        }

        /// <summary>
        /// 创建一个空的 <see cref="ModuleInfo"/> 实例，<seealso cref="ModuleInfo.State"/> 为 <seealso cref="ModuleState.Failure"/>
        /// </summary>
        /// <returns></returns>
        public static ModuleInfo CreateEmptyModuleInfo()
        {
            return new ModuleInfo() { State = ModuleState.Failure };
        }
    }
}
