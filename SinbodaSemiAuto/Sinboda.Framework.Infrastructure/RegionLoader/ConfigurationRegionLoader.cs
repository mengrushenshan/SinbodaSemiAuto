using Sinboda.Framework.Infrastructure.Configurations;
using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 配置文件的 region 装载
    /// </summary>
    public class ConfigurationRegionLoader : IRegionLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Region> CreateRegionSource()
        {
            List<Region> result = new List<Region>();
            RegionsConfigurationSection section = ConfigurationManager.GetSection("regions") as RegionsConfigurationSection;
            if (section != null)
            {
                foreach (RegionConfigurationElement element in section.Regions)
                {
                    Region region = new Region(element.RegionName, element.AssemblyFile, element.ViewType, element.SearchRange);
                    result.Add(region);
                }
            }

            return result;
        }
    }
}
