using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Configurations
{
    /// <summary>
    /// 表示 region 集合
    /// </summary>
    public class RegionConfigurationElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 是否可添加重复元素
        /// </summary>
        protected override bool ThrowOnDuplicate
        {
            get { return true; }
        }
        /// <summary>
        /// 节点标记名称
        /// </summary>
        protected override string ElementName
        {
            get { return "region"; }
        }
        /// <summary>
        /// 节点集合类型
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public RegionConfigurationElementCollection()
        { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="modules"></param>
        public RegionConfigurationElementCollection(RegionConfigurationElement[] modules)
        {
            if (modules == null)
            {
                throw new ArgumentNullException("regions");
            }
            for (int i = 0; i < modules.Length; i++)
            {
                RegionConfigurationElement element = modules[i];
                BaseAdd(element);
            }
        }
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="module"></param>
        public void Add(RegionConfigurationElement module)
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
        /// 索引节点信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RegionConfigurationElement this[int index]
        {
            get { return (RegionConfigurationElement)BaseGet(index); }
        }
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new RegionConfigurationElement();
        }
        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RegionConfigurationElement)element).RegionName;
        }
    }
}
