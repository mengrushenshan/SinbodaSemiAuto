using Sinboda.Framework.Core.CommonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Interface
{
    /// <summary>
    /// 仪器信息接口
    /// </summary>
    public interface IAnalyzerInfo
    {
        /// <summary>
        /// 获取信息
        /// </summary>
        CurrentModuleInfo GetAnalyzerInfo();

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <returns></returns>
        bool SetAnalyzerInfo(CurrentModuleInfo info);
    }
}
