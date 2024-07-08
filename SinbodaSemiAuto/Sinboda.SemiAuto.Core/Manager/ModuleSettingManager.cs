using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Manager
{
    /// <summary>
    /// 模块设置管理
    /// </summary>
    public class ModuleSettingManager : TBaseSingleton<ModuleSettingManager>
    {
        /// <summary>
        /// 模块设置信息
        /// </summary>
        private List<ModuleSettingInfo> Module_setting_info = new List<ModuleSettingInfo>();

        /// <summary>
        /// 所有模块的设置信息
        /// </summary>
        public List<ModuleSettingInfo> ModuleSetting => Module_setting_info;

        /// <summary>
        /// 可以做为发送测试目标模块的设置信息
        /// </summary>
        public List<ModuleSettingInfo> TargetModuleSetting => Module_setting_info.FindAll(msi => msi.IsModuleEnabled != null && msi.IsModuleEnabled.Value && !msi.IsShield);

        /// <summary>
        /// 新增或修改模块设置
        /// </summary>
        /// <param name="mss"></param>
        public void SetModuleSetting(ModuleSettingInfo msi)
        {
            if (msi != null)
            {
                ModuleSettingInfo m = Module_setting_info.FirstOrDefault(o => o.ModuleId == msi.ModuleId);
                if (m == null)
                    Module_setting_info.Add(msi);
                else
                    m.IsModuleEnabled = msi.IsModuleEnabled;
            }
        }

        /// <summary>
        /// 新增或修改ISE设置
        /// </summary>
        /// <param name="msi"></param>
        public void SetISESetting(ModuleSettingInfo msi)
        {
            if (msi != null)
            {
                ModuleSettingInfo m = Module_setting_info.FirstOrDefault(o => o.ModuleId == msi.ModuleId);
                if (m == null)
                    Module_setting_info.Add(msi);

            }
        }

        /// <summary>
        /// 模块是否启用
        /// </summary>
        /// <param name="module_id"></param>
        /// <returns></returns>
        public bool? IsModuleEnabled(int? module_id)
        {
            ModuleSettingInfo msi = ModuleSetting.FirstOrDefault(o => o.ModuleId == module_id);
            if (msi != null)
                return msi.IsModuleEnabled;
            return null;
        }

        /// <summary>
        /// 模块是否休眠
        /// </summary>
        /// <param name="module_id"></param>
        /// <returns></returns>
        public bool? IsModulSleep(int? module_id)
        {
            ModuleSettingInfo msi = ModuleSetting.FirstOrDefault(o => o.ModuleId == module_id);
            if (msi != null)
                return msi.IsSleep;
            return null;
        }

        /// <summary>
        /// 模块是否遮蔽
        /// </summary>
        /// <param name="module_id"></param>
        /// <returns></returns>
        public bool? IsModulShield(int? module_id)
        {
            ModuleSettingInfo msi = ModuleSetting.FirstOrDefault(o => o.ModuleId == module_id);
            if (msi != null)
                return msi.IsShield;
            return null;
        }


        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear() => ModuleSetting.Clear();
    }
}
