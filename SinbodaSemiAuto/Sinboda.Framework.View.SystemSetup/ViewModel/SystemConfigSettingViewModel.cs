using Sinboda.Framework.Business.SystemSetup;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure.Model;
using Sinboda.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.View.SystemSetup.ViewModel
{
    /// <summary>
    /// 系统动态设置类
    /// </summary>
    public class SystemConfigSettingViewModel : NavigationViewModelBase
    {
        /// <summary>
        /// 系统设置业务逻辑接口
        /// </summary>
        SystemConfigSettingBusiness business = new SystemConfigSettingBusiness();

        /// <summary>
        /// 当前用户
        /// </summary>
        string LogUser;

        /// <summary>
        /// 程序路径
        /// </summary>
        string currentDirectory = MapPath.AppDir;
        /// <summary>
        /// 构造函数
        /// </summary>
        public SystemConfigSettingViewModel()
        {
            LogUser = SystemResources.Instance.CurrentUserName;
            OperationResult<List<ModuleMenuItem>> operationResult = business.GetModuleInfoList(SystemResources.Instance.CurrentUserName);
            if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
            {
                var temp = operationResult.Results.Find(p => p != null && p.Id == "SystemConfigSettingPageView");
                if (temp != null)
                {
                    //此处所用类型为各产品进行初始化的类型，同时也是安装时写入文件的类型，此类型用来筛选并显示设置的目录 不可更改 sunch 2020-02-03
                    ModuleType type = (ModuleType)Convert.ToInt32(SystemResources.Instance.AnalyzerInfoType);
                    if (type != ModuleType.None)
                        ModuleList = temp.ChildMenus.Where(o => (o.ModuleType == type || o.ModuleType == 0) && BootStrapper.Current.ModuleManager.ConfigInitDic.ContainsKey(o.Source.ToString())).ToList();
                    else
                        ModuleList = temp.ChildMenus.Where(o => BootStrapper.Current.ModuleManager.ConfigInitDic.ContainsKey(o.Source.ToString())).ToList();
                }
            }
        }

        private List<ModuleMenuItem> _ModuleList = new List<ModuleMenuItem>();
        /// <summary>
        /// 模块列表
        /// </summary>
        public List<ModuleMenuItem> ModuleList
        {
            get { return _ModuleList; }
            set
            {
                if (_ModuleList != value)
                {
                    _ModuleList = value;
                    RaisePropertyChanged("ModuleList");
                }
            }
        }

        /// <summary>
        /// 点击左侧菜单后初始化界面
        /// </summary>
        /// <param name="treeItem"></param>
        /// <returns></returns>
        public object InitTreeItemUserControl(object treeItem)
        {
            ModuleMenuItem item = treeItem as ModuleMenuItem;
            try
            {
                //Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(currentDirectory, item.ModuleName));
                //if (asm != null)
                //{
                //    dynamic obj = asm.CreateInstance(item.Source.ToString());
                //    return obj;
                //}
                if (BootStrapper.Current.ModuleManager.ConfigInitDic.Keys.Contains(item.Source.ToString()))
                {
                    LogHelper.logSoftWare.Info("设置加载：" + item.Source.ToString());
                    return BootStrapper.Current.ModuleManager.ConfigInitDic[item.Source.ToString()];
                }
                else
                {
                    LogHelper.logSoftWare.Info("设置未加载：" + item.Source.ToString());
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("系统设置初始化界面出错，错误信息：", e);
            }

            return null;
        }
    }
}
