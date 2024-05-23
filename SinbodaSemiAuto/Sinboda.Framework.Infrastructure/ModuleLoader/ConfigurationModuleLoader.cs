using Sinboda.Framework.Infrastructure.Configurations;
using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 读取 config 中的 modules 节创建 ModuleInfo
    /// </summary>
    public class ConfigurationModuleLoader : IModuleInfoLoader
    {
        /// <summary>
        /// 读取 config 文件创建模块数据源
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModuleInfo> CreateModuleInfoSource()
        {
            List<ModuleInfo> result = new List<ModuleInfo>();
            ModulesConfigurationSection modulesConfigurationSection = ConfigurationManager.GetSection("modules") as ModulesConfigurationSection;
            if (modulesConfigurationSection != null)
            {
                foreach (ModuleConfigurationElement element in modulesConfigurationSection.Modules)
                {
                    ModuleInfo moduleInfo = new ModuleInfo(element.AssemblyFile, element.ModuleType, GetFileAbsoluteUri(element.AssemblyFile));
                    result.Add(moduleInfo);
                }
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetFileAbsoluteUri(string filePath)
        {
            return Path.GetFullPath(filePath);
        }
    }
}
