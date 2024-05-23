using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Enums
{
    public enum PermissionErrorCode
    {
        /// <summary>
        /// 没有错误
        /// </summary>
        OK,
        /// <summary>
        /// 用户已存在
        /// </summary>
        UserExist,
        /// <summary>
        /// 用户不存在
        /// </summary>
        UserNotExist,
        /// <summary>
        /// 角色已存在
        /// </summary>
        RoleExist,
        /// <summary>
        /// 角色不存在
        /// </summary>
        RoleNotExist,
        /// <summary>
        /// 模块已存在
        /// </summary>
        ModuleExist,
        /// <summary>
        /// 模块不存在
        /// </summary>
        ModuleNotExist,
        /// <summary>
        /// 权限已存在
        /// </summary>
        PermissionExist,
        /// <summary>
        /// 权限不存在
        /// </summary>
        PermissionNotExist,
        /// <summary>
        /// 其它错误
        /// </summary>
        OtherError
    };
}
