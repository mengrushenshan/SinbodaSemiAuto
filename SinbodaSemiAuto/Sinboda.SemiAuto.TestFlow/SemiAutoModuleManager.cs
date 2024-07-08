using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.SemiAuto.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.TestFlow
{
    public class SemiAutoModuleManager : TBaseSingleton<SemiAutoModuleManager>, IModuleManager
    {

        /// <summary>
        /// 保存活动模块
        /// </summary>
        public List<SemiAutoModuleContext> ModuleContexts { get; private set; } = new List<SemiAutoModuleContext>();

        public ModuleInfoModel AddModule(ModuleInfoModel module)
        {
            var mContext = new SemiAutoModuleContext(module);
            ModuleContexts.Add(mContext);

            foreach (var item in ModuleContexts)
            {
                LogHelper.logSoftWare.Debug($"当前发光模块信息,模块编号:{item.ModuleID},模块名称:{item.ModuleName},模块状态:{item.ModuleState},模块显示信息:{item.StatusDisplayValue}");
            }

            return mContext;
        }

        /// <summary>
        /// 清除模块
        /// </summary>
        public void ClearModule() => ModuleContexts.Clear();

        public bool CanTestModule(int moduleId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定模块的 <seealso cref="CMModuleContext"/> 实例
        /// </summary>
        /// <param name="moduleId">模块号</param>
        /// <returns></returns>
        public SemiAutoModuleContext GetModuleContext(int moduleId)
        {
            return ModuleContexts.FirstOrDefault(o => o.ModuleID == moduleId);
        }
    }
}
