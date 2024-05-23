using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Configurations
{
    /// <summary>
    /// 表示配置文件中 module 元素的集合
    /// </summary>
    public class ModuleConfigurationElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 
        /// </summary>
        protected override bool ThrowOnDuplicate
        {
            get { return true; }
        }
        /// <summary>
        /// 节点集合类型
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }
        /// <summary>
        /// 节点标记名称
        /// </summary>
        protected override string ElementName
        {
            get { return "module"; }
        }
        /// <summary>
        /// 索引节点信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ModuleConfigurationElement this[int index]
        {
            get { return (ModuleConfigurationElement)BaseGet(index); }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModuleConfigurationElementCollection()
        { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="modules"></param>
        public ModuleConfigurationElementCollection(ModuleConfigurationElement[] modules)
        {
            if (modules == null)
            {
                throw new ArgumentNullException("modules");
            }
            for (int i = 0; i < modules.Length; i++)
            {
                ModuleConfigurationElement element = modules[i];
                BaseAdd(element);
            }
        }
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="module"></param>
        public void Add(ModuleConfigurationElement module)
        {
            BaseAdd(module);
        }
        /// <summary>
        /// 是否包含节点
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public bool Contains(string moduleName)
        {
            return BaseGet(moduleName) != null;
        }
        /// <summary>
        /// 寻找所有符合条件节点
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public IList<ModuleConfigurationElement> FindAll(Predicate<ModuleConfigurationElement> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            IList<ModuleConfigurationElement> list = new List<ModuleConfigurationElement>();
            foreach (ModuleConfigurationElement moduleConfigurationElement in this)
            {
                if (match(moduleConfigurationElement))
                {
                    list.Add(moduleConfigurationElement);
                }
            }
            return list;
        }
        /// <summary>
        /// 创建新节点
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ModuleConfigurationElement();
        }
        /// <summary>
        /// 获取节点信息
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModuleConfigurationElement)element).AssemblyFile;
        }
    }
}
