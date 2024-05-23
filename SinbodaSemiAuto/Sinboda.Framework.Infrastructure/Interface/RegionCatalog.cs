using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRegionCatalog
    {
        /// <summary>
        /// 区域集合
        /// </summary>
        List<Region> Regions { get; }
        /// <summary>
        /// 添加区域
        /// </summary>
        /// <param name="region"></param>
        void AddRegion(Region region);
    }

    /// <summary>
    /// 区域处理
    /// </summary>
    public class RegionCatalog : IRegionCatalog
    {
        private readonly IRegionLoader _RegionLoader;
        private List<Region> _RegionCollection = new List<Region>();
        /// <summary>
        /// 区域集合
        /// </summary>
        public List<Region> Regions
        {
            get { return _RegionCollection; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="regionLoader"></param>
        public RegionCatalog(IRegionLoader regionLoader)
        {
            if (regionLoader == null)
                throw new ArgumentNullException("regionLodfer");

            _RegionLoader = regionLoader;
            Regions.AddRange(_RegionLoader.CreateRegionSource());
        }
        /// <summary>
        /// 添加区域
        /// </summary>
        /// <param name="region"></param>
        public void AddRegion(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            Regions.Add(region);
        }
    }
}
