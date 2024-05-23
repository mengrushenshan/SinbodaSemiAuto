using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using Sinboda.Framework.Infrastructure.RegionAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 区域管理
    /// </summary>
    public class RegionManager
    {
        #region 属性
        /// <summary>
        /// 区域名称属性
        /// </summary>
        public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(RegionManager), new PropertyMetadata(RegionNameChangedCallback));

        /// <summary>
        /// 获取区域名称
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetRegionName(DependencyObject obj)
        {
            return (string)obj.GetValue(RegionNameProperty);
        }

        /// <summary>
        /// 设置区域名称
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetRegionName(DependencyObject obj, string value)
        {
            obj.SetValue(RegionNameProperty, value);
        }
        #endregion

        /// <summary>
        /// 创建区域元素
        /// </summary>
        /// <param name="element">关联元素</param>
        /// <param name="regionName">区域名称</param>
        public static void CreateRegion(DependencyObject element, string regionName)
        {
            if (DesignHelper.IsInDesignMode)
                return;

            IRegionAdapter regionAdapter = RegionAdapterMappings.GetMapping(element.GetType());
            Region region = InterfaceMagager.RegionCatalog.Regions.FirstOrDefault(o => o.RegionName.Equals(regionName));
            if (region == null)
            {
                return;
                // TODO：未查询到区域
                //throw new ArgumentNullException("region");
            }

            if (region.SearchRange == SearchRange.Module)
            {
                ModuleInfo minfo = InterfaceMagager.ModuleManager.FindModuleInfo(region.ModuleName);
                if (minfo != null && minfo.State == ModuleState.Initialized)
                {
                    Type regionType = minfo.ModuleAssembly.GetType(region.ViewType);
                    region.View = Activator.CreateInstance(regionType);
                }
            }
            else if (region.SearchRange == SearchRange.All)
            {
                string moduleName = region.ModuleName.Substring(0, region.ModuleName.Length - 4);
                Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(o => o.GetName().Name == moduleName);
                if (assembly == null)
                    return;

                Type regionType = assembly.GetType(region.ViewType);
                region.View = Activator.CreateInstance(regionType);
            }

            if (region.View != null)
                regionAdapter.Initialize(element, region.View);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void RegionNameChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CreateRegion(d, e.NewValue.ToString());
        }
    }
}
