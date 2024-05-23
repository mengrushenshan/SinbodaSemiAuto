using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 模块管理
    /// </summary>
    public class ModuleManager : IModuleManager
    {
        private IModuleInfoLoader _ModuleInfoLoader;
        private List<ModuleInfo> _ModuleInfoSource = new List<ModuleInfo>();

        /// <summary>
        /// 执行模块加载后触发
        /// </summary>
        public event EventHandler<ModuleCompletedEventArgs> LoadModuleCompleted;
        /// <summary>
        /// 执行模块初始化后触发
        /// </summary>
        public event EventHandler<ModuleCompletedEventArgs> InitModuleCompleted;
        /// <summary>
        /// 已加载模块
        /// </summary>
        public IEnumerable<ModuleInfo> ModuleInfoSource { get { return _ModuleInfoSource; } }
        private Dictionary<string, IModule> moduleInitDic = new Dictionary<string, IModule>();
        private Dictionary<string, object> configInitDic = new Dictionary<string, object>();
        public Dictionary<string, IModule> ModuleInitDic
        {
            get
            {
                return moduleInitDic;
            }
            set
            {
                moduleInitDic = value;
            }
        }
        public Dictionary<string, object> ConfigInitDic
        {
            get
            {
                return configInitDic;
            }

            set
            {
                configInitDic = value;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleInfoLoader"></param>
        public ModuleManager(IModuleInfoLoader moduleInfoLoader)
        {
            _ModuleInfoLoader = moduleInfoLoader;
            _ModuleInfoSource.AddRange(_ModuleInfoLoader.CreateModuleInfoSource());
        }

        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="moduleInfo"></param>
        public void AddModule(ModuleInfo moduleInfo)
        {
            if (moduleInfo != null)
            {
                _ModuleInfoSource.Add(moduleInfo);
            }
        }

        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="moduleInfo">模块名称</param>
        /// <returns></returns>
        public void LoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                return;

            try
            {
                //moduleInfo.ModuleAssembly = Assembly.LoadFile(moduleInfo.Ref);
                //Type moduleType = moduleInfo.ModuleAssembly.GetType(moduleInfo.ModuleType);

                //if (moduleType == null)
                //    throw new Exception(string.Format("模块 {0} 不包含类型 {1}.", moduleInfo.ModuleName, moduleInfo.ModuleType)); //TODO 翻译

                //if (moduleType.GetInterface("IModule") != null)
                //{
                //    IModule modul = Activator.CreateInstance(moduleType) as IModule;
                //    moduleInfo.Module = modul;
                //    RaiseLoadModuleCompleted(moduleInfo, null);
                //}
                //else
                //    throw new Exception(string.Format("类型 {0} 未实现 IModule 接口.", moduleInfo.ModuleType)); //TODO 翻译
                if (!ModuleInitDic.Keys.Contains(moduleInfo.ModuleName + "#" + moduleInfo.ModuleType))
                {
                    throw new Exception(string.Format(StringResourceExtension.GetLanguage(155, "模块 {0} 不包含类型 {1}."), moduleInfo.ModuleName, moduleInfo.ModuleType)); //TODO 翻译
                }
                else
                {
                    moduleInfo.ModuleAssembly = System.Reflection.Assembly.GetAssembly(ModuleInitDic[moduleInfo.ModuleName + "#" + moduleInfo.ModuleType].GetType());
                    moduleInfo.Module = ModuleInitDic[moduleInfo.ModuleName + "#" + moduleInfo.ModuleType];
                    RaiseLoadModuleCompleted(moduleInfo, null);
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("LoadModuelType", ex);
                moduleInfo.State = ModuleState.Failure;
                RaiseLoadModuleCompleted(moduleInfo, ex);
            }
        }

        /// <summary>
        /// 根据模块名查询 <see cref="ModuleInfo">ModuleInfo</see>
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public ModuleInfo FindModuleInfo(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
                return ModuleInfo.CreateEmptyModuleInfo();

            return _ModuleInfoSource.FirstOrDefault(o => o.ModuleName.Equals(moduleName));
        }

        /// <summary>
        /// 执行模块初始化
        /// </summary>
        /// <param name="moduleInfo"></param>
        public bool InitModule(ModuleInfo moduleInfo)
        {
            try
            {
                if (moduleInfo.State == ModuleState.Failure)
                    return false;

                if (moduleInfo.State == ModuleState.Initialized)
                    return true;

                moduleInfo.State = ModuleState.Initializing;
                InitTaskResult result = moduleInfo.Module.InitializeResource();
                moduleInfo.State = result.Succeed ? ModuleState.Initialized : ModuleState.Failure;
                RaiseLoadModuleCompleted(moduleInfo, null);
                return result.Succeed;
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("InitModule", ex);
                moduleInfo.State = ModuleState.Failure;
                RaiseInitModuleCompleted(moduleInfo, ex);
                return false;
            }
        }

        #region 私有方法 

        /// <summary>
        /// 触发 LoadModuleCompleted 事件
        /// </summary>
        /// <param name="e"></param>
        private void RaiseLoadModuleCompleted(ModuleCompletedEventArgs e)
        {
            if (LoadModuleCompleted != null)
            {
                LoadModuleCompleted(this, e);
            }
        }
        /// <summary>
        /// 触发 LoadModuleCompleted 事件
        /// </summary>
        /// <param name="moduleInfo">模块信息</param>
        /// <param name="error">错误信息</param>
        private void RaiseLoadModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            RaiseLoadModuleCompleted(new ModuleCompletedEventArgs(moduleInfo, error));
        }

        /// <summary>
        /// 触发 InitModuleCompleted 事件
        /// </summary>
        private void RaiseInitModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            RaiseInitModuleCompleted(new ModuleCompletedEventArgs(moduleInfo, error));
        }
        /// <summary>
        /// 触发 InitModuleCompleted 事件
        /// </summary>
        /// <param name="e"></param>
        private void RaiseInitModuleCompleted(ModuleCompletedEventArgs e)
        {
            if (InitModuleCompleted != null)
            {
                InitModuleCompleted(this, e);
            }
        }
        #endregion
    }
}
