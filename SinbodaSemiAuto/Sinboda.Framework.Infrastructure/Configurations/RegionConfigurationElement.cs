using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Configurations
{
    /// <summary>
    /// 表示配置文件中的 region 元素
    /// </summary>
    public class RegionConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        [ConfigurationProperty("regionName", IsRequired = true)]
        public string RegionName
        {
            get
            {
                return (string)base["regionName"];
            }
            set
            {
                base["regionName"] = value;
            }
        }
        /// <summary>
        /// 模块名称
        /// </summary>
        [ConfigurationProperty("assemblyFile", IsRequired = true)]
        public string AssemblyFile
        {
            get
            {
                return (string)base["assemblyFile"];
            }
            set
            {
                base["assemblyFile"] = value;
            }
        }
        /// <summary>
        /// 区域使用的界面类型
        /// </summary>
        [ConfigurationProperty("viewType", IsRequired = true)]
        public string ViewType
        {
            get
            {
                return (string)base["viewType"];
            }
            set
            {
                base["viewType"] = value;
            }
        }

        /// <summary>
        /// 区域搜索范围
        /// </summary>
        [ConfigurationProperty("searchRange", IsRequired = false)]
        public SearchRange SearchRange
        {
            get
            {
                return (SearchRange)base["searchRange"];
            }
            set
            {
                base["searchRange"] = value;
            }
        }
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public RegionConfigurationElement()
        { }
        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="regionName"></param>
        /// <param name="assemblyFile"></param>
        /// <param name="viewType"></param>
        /// <param name="searchRange"></param>
        public RegionConfigurationElement(string regionName, string assemblyFile, string viewType, SearchRange searchRange)
        {
            base["regionName"] = regionName;
            base["assemblyFile"] = assemblyFile;
            base["viewType"] = viewType;
            base["searchRange"] = searchRange;
        }
    }
}
