using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.Framework.Print
{
    /// <summary>
    /// 接口实现类
    /// </summary>
    public class InitializeModule : IModule
    {
        /// <summary>
        /// 销毁
        /// </summary>
        public void FinalizeResource()
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public InitTaskResult InitializeResource()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ReportSetting set = new ReportSetting();

                set.Reload();
            });
            return new InitTaskResult();
        }
        /// <summary>
        /// 获得菜单
        /// </summary>
        /// <returns></returns>
        public List<ModuleMenuItem> GetMenus()
        {
            return new List<ModuleMenuItem>
            {
            };
        }
    }
 }
