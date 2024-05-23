using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Configurations
{
    /// <summary>
    /// 表示配置文件中的 module 元素
    /// </summary>
    public class ModuleConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// 程序集名称
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
        /// 初始化模块类型
        /// </summary>
        [ConfigurationProperty("moduleType", IsRequired = true)]
        public string ModuleType
        {
            get
            {
                return (string)base["moduleType"];
            }
            set
            {
                base["moduleType"] = value;
            }
        }

        //[ConfigurationProperty("startupLoaded", IsRequired = false, DefaultValue = true)]
        //public bool StartupLoaded
        //{
        //    get
        //    {
        //        return (bool)base["startupLoaded"];
        //    }
        //    set
        //    {
        //        base["startupLoaded"] = value;
        //    }
        //}

        //[ConfigurationProperty("dependencies", IsDefaultCollection = true, IsKey = false)]
        //public ModuleDependencyCollection Dependencies
        //{
        //    get
        //    {
        //        return (ModuleDependencyCollection)base["dependencies"];
        //    }
        //    set
        //    {
        //        base["dependencies"] = value;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public ModuleConfigurationElement()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="moduleType"></param>
        /// <param name="startupLoaded"></param>
        public ModuleConfigurationElement(string assemblyFile, string moduleType, bool startupLoaded)
        {
            base["assemblyFile"] = assemblyFile;
            base["moduleType"] = moduleType;
            base["startupLoaded"] = startupLoaded;
        }
    }
}
