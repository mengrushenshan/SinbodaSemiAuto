using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Configurations
{
    /// <summary>
    /// 表示配置文件中的 regions 节
    /// </summary>
    public class RegionsConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// 配置文件中的 region 集合
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true, IsKey = false)]
        public RegionConfigurationElementCollection Regions
        {
            get
            {
                return (RegionConfigurationElementCollection)base[""];
            }
            set
            {
                base[""] = value;
            }
        }
    }
}
