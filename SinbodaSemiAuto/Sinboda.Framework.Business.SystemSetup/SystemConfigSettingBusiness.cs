using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Business.SystemSetup
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemConfigSettingBusiness : BusinessBase<SystemConfigSettingBusiness>
    {
        /// <summary>
        /// 权限模块接口
        /// </summary>
        IPermission permission = new PermissionOperation();
        /// <summary>
        /// 获取模块菜单列表
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public OperationResult<List<ModuleMenuItem>> GetModuleInfoList(string userName)
        {
            try
            {
                List<ModuleMenuItem> result = permission.GetModuleMenuItemList(userName);
                return Result<List<ModuleMenuItem>>(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetModuleInfoList", e);
                return Result<List<ModuleMenuItem>>(e);
            }
        }
    }
}
