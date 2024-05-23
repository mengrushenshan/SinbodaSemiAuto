using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Control.Controls.Navigation;
using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Infrastructure.Model;
using Sinboda.Framework.MainWindow.Blue.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.MainWindow.Blue
{
    /// <summary>
    /// 从模块程序集中加载相应内容
    /// </summary>
    internal class ModuleNavigationContentProvider : NavigationContentProvider
    {
        public override object Load(object source)
        {
            var moduleMenuItem = source as NavigationItem;

            if (moduleMenuItem != null)
            {
                // 没有模块名称
                if (string.IsNullOrEmpty(moduleMenuItem.ModuleName))
                    return base.Load(moduleMenuItem.Source);

                // 根据模块状态决定导航行为
                ModuleInfo moduleInfo = InterfaceMagager.ModuleManager.FindModuleInfo(moduleMenuItem.ModuleName);
                if (moduleInfo != null && moduleInfo.State == ModuleState.Initialized)
                {
                    object content = base.Load(moduleInfo.ModuleAssembly.GetType(moduleMenuItem.Source.ToString()));
                    if (content != null)
                        return base.Load(content);
                    else
                    {
                        string errMsg = StringResourceExtension.GetLanguage(150, "无法显示模块 {0} \r  原因 : 模块 {0} 中未找到类型 {1} ", moduleMenuItem.ModuleName, moduleMenuItem.Source.ToString());
                        AppError appError = new AppError();
                        appError.SetErrorText(errMsg);
                        return appError;
                    }
                }
                else
                {
                    string errMsg = StringResourceExtension.GetLanguage(153, "无法显示模块 {0} \r  原因 : 未完成初始化或初始化失败", moduleMenuItem.ModuleName);
                    AppError appError = new AppError();
                    appError.SetErrorText(errMsg);
                    return base.Load(appError);
                }
            }
            else
            {
                return base.Load(source);
            }
        }
    }
}
