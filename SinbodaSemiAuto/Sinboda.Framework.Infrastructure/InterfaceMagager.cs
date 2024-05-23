using Autofac;
using Autofac.Configuration;
using Microsoft.Practices.ServiceLocation;
using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.RegionAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 提供框架使用的基础接口管理功能
    /// </summary>
    public sealed class InterfaceMagager
    {
        /// <summary>
        /// 模块管理
        /// </summary>
        public static IModuleManager ModuleManager { get; private set; }
        /// <summary>
        /// 区域集合
        /// </summary>
        public static IRegionCatalog RegionCatalog { get; private set; }
        /// <summary>
        /// 初始化任务
        /// </summary>
        public static ICustomTaskManager<InitTaskResult> InitTaskManager { get; private set; }

        static InterfaceMagager()
        {
            var builder = new ContainerBuilder();

            // 配置文件方式
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
            builder.RegisterType<InitTaskManager>().As<ICustomTaskManager<InitTaskResult>>().Named<ICustomTaskManager<InitTaskResult>>("InitTask");
            IContainer container = builder.Build();

            // 代码方式
            //builder.RegisterType<ConfigurationModuleLoader>().As<IModuleInfoLoader>();
            //builder.RegisterType<ConfigurationRegionLoader>().As<IRegionLoader>();
            //builder.RegisterType<ModuleManager>().As<IModuleManager>();
            //builder.RegisterType<RegionCatalog>().As<IRegionCatalog>();
            //builder.RegisterType<ModuleManageMenuItemLoader>().As<IMenuItemLoader>();
            //IContainer container = builder.Build();
            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));


            ModuleManager = ServiceLocator.Current.GetInstance<IModuleManager>();
            RegionCatalog = ServiceLocator.Current.GetInstance<IRegionCatalog>();
            InitTaskManager = ServiceLocator.Current.GetInstance<ICustomTaskManager<InitTaskResult>>("InitTask"); // 初始化任务

            // 添加区域适配器
            RegionAdapterMappings.RegisterMapping(typeof(ContentControl), new ContentControlRegionAdapter());
        }
    }
}
