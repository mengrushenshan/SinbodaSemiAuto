using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Configurations
{
    /// <summary>
    /// 表示配置文件中的 modules 节
    /// </summary>
    public class ModulesConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// 配置文件中的 module 元素集合
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true, IsKey = false)]
        public ModuleConfigurationElementCollection Modules
        {
            get
            {
                return (ModuleConfigurationElementCollection)base[""];
            }
            set
            {
                base[""] = value;
            }
        }
    }
}
