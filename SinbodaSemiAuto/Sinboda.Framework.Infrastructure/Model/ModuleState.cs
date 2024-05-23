using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Model
{
    /// <summary>
    /// 表示模块初始化状态
    /// </summary>
    public enum ModuleState
    {
        /// <summary>
        /// 已创建 MduleInfo，但尚未执行初始化
        /// </summary>
        NotStarted,

        /// <summary>
        /// 正在执行初始化
        /// </summary>
        Initializing,

        /// <summary>
        /// 已完成初始化，可以正常使用
        /// </summary>
        Initialized,

        /// <summary>
        /// 初始化加载失败，模块无法使用
        /// </summary>
        Failure
    }
}
