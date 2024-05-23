using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 表示 region 的装载
    /// </summary>
    public interface IRegionLoader
    {
        /// <summary>
        /// 返回 <see cref="IEnumerable"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<Region> CreateRegionSource();
    }
}
