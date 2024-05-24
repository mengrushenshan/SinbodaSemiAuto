using Sinboda.SemiAuto.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Command
{
    /// <summary>
    /// 指令
    /// </summary>
    internal interface ICmd
    {
        /// <summary>
        /// 执行
        /// </summary>
        bool Execute();

        /// <summary>
        /// 执行
        /// </summary>
        bool ExecuteAsync();

        /// <summary>
        /// 获取应答信息
        /// </summary>
        /// <returns></returns>
        IResponse GetResponse();
    }
}
