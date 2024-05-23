using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.RegionAdapter
{
    /// <summary>
    /// 区域适配器管理类
    /// </summary>
    public class RegionAdapterMappings
    {
        private static readonly Dictionary<Type, IRegionAdapter> mappings = new Dictionary<Type, IRegionAdapter>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="adapter"></param>
        public static void RegisterMapping(Type controlType, IRegionAdapter adapter)
        {
            if (controlType == null)
            {
                throw new ArgumentNullException("controlType");
            }
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }
            if (mappings.ContainsKey(controlType))
            {
                throw new InvalidOperationException(string.Format(StringResourceExtension.GetLanguage(98, "添加了相同的类型{0}"), controlType.Name)); //TODO 翻译
            }
            mappings.Add(controlType, adapter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        public static IRegionAdapter GetMapping(Type controlType)
        {
            Type type = controlType;
            while (type != null)
            {
                if (mappings.ContainsKey(type))
                {
                    return mappings[type];
                }
                type = type.BaseType;
            }
            throw new KeyNotFoundException(string.Format(StringResourceExtension.GetLanguage(99, "未找到键{0}"), controlType)); //TODO 翻译
        }
    }
}
