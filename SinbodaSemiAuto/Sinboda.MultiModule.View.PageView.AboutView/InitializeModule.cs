using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Infrastructure.Model;
using Sinboda.SemiAuto.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.View
{
    public class InitializeModule : IModule
    {
        public void FinalizeResource()
        {

        }

        public List<ModuleMenuItem> GetMenus()
        {
            throw new NotImplementedException();
        }

        public InitTaskResult InitializeResource()
        {
            ClearVersionInfo();

            VirtualModuleCacheManager.AddData();
            SoftWareVersionCacheManager.AddData();

            return new InitTaskResult();
        }

        /// <summary>
        /// 软件启动后清空原有版本信息
        /// </summary>
        internal void ClearVersionInfo()
        {
            List<ModuleVersionModel> eqDbList = ModuleVersionOperation.Instance.Query(o => o.Id != null).ToList();
            foreach (var item in eqDbList)
            {
                item.Create_user = item.Create_user;
                item.UpdateTime = DateTime.Now;
                item.VersionInfo = "";
            }
            if (SystemResources.Instance.CurrentUserName == null)
                SystemResources.Instance.CurrentUserName = "sinboda";
            ModuleVersionOperation.Instance.Update(eqDbList);
        }
    }
}
