using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Interface
{
    /// <summary>
    /// 区域初始化接口
    /// </summary>
    public interface IRegionAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="regionTarget"></param>
        /// <param name="region"></param>
        void Initialize(object regionTarget, object region);
    }
}
