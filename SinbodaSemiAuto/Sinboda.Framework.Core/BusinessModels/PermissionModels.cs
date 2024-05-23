using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.BusinessModels
{
    /// <summary>
    /// 角色数据定义类
    /// </summary>
    public class SysRoleModel
    {
        /// <summary>
        /// 标识
        /// </summary>
        public string RoleID { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 语言编号
        /// </summary>
        public int LangID { get; set; }
        /// <summary>
        /// 级别
        /// </summary>
        public int Level { get; set; }
    }
    /// <summary>
    /// 用户数据定义类
    /// </summary>
    public class SysUserModel
    {
        /// <summary>
        /// 用户标识 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 所属角色标识
        /// </summary>
        public string RoleID { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 签名路径
        /// </summary>
        public string SignPath { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SysUserModel()
        {

        }

        /// <summary>
        /// 用户定义类构造函数
        /// </summary>
        /// <param name="userName">用户标识</param>
        /// <param name="password">密码</param>
        /// <param name="roleID">所属角色标识</param>
        /// <param name="roleName">角色名称</param>
        public SysUserModel(string userName, string password, string roleID, string signPath, string roleName)
        {
            this.UserName = userName;
            this.Password = password;
            this.RoleID = roleID;
            this.SignPath = signPath;
            this.RoleName = roleName;
        }
    }
    /// <summary>
    /// 模块数据定义类
    /// </summary>
    public class SysModuleModel
    {
        /// <summary>
        /// 模块标识
        /// </summary>
        public string ModuleID { get; set; }
        /// <summary>
        /// 唯一标识码
        /// </summary>
        public string ModuleIDKey { get; set; }
        /// <summary>
        /// 模块描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 语言标识
        /// </summary>
        public int LangID { get; set; }
        /// <summary>
        /// 上级模块标识 
        /// </summary>
        public string ParentID { get; set; }
        /// <summary>
        /// Dll文件名，不含路径
        /// </summary>
        public string DllName { get; set; }
        /// <summary>
        /// 名称空间，不含类名
        /// </summary>
        public string NameSpace { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int ShowOrder { get; set; }
        /// <summary>
        /// 正常时图标
        /// </summary>
        public string IconCommon { get; set; }
        /// <summary>
        /// 是否拥有权限
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 模块类型
        /// </summary>
        public int ModuleType { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsMenuShow { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsDisplayEnable { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SysModuleModel()
        {

        }

        /// <summary>
        /// 模块数据定义类构造函数
        /// </summary>
        /// <param name="moduleID">模块标识</param>
        /// <param name="moduleIDKey">唯一标识</param>
        /// <param name="parentID">上级模块标识</param>
        /// <param name="description">模块描述</param>
        /// <param name="langID">语言标识</param>
        /// <param name="dllName">Dll文件名</param>
        /// <param name="nameSpace">名称空间</param>
        /// <param name="showOrder">显示顺序</param>
        /// <param name="iconCommon">显示顺序</param>
        /// <param name="enable">是否拥有权限</param>
        /// <param name="isMenuShow">是否显示</param>
        /// <param name="moduleType">菜单类型</param>
        public SysModuleModel(string moduleID, string moduleIDKey, string parentID, string description, int langID, string dllName, string nameSpace,
            int showOrder, string iconCommon, bool enable, bool isMenuShow, int moduleType = 0, bool isDisplayEnable = false)
        {
            ModuleID = moduleID;
            ModuleIDKey = moduleIDKey;
            ParentID = parentID;
            Description = description;
            LangID = langID;
            DllName = dllName;
            NameSpace = nameSpace;
            ShowOrder = showOrder;
            IconCommon = iconCommon;
            Enable = enable;
            IsMenuShow = isMenuShow;
            ModuleType = moduleType;
            IsDisplayEnable = isDisplayEnable;
        }
    }
    /// <summary>
    /// 权限数据定义类
    /// </summary>
    public class SysPermissionModel
    {
        /// <summary>
        /// 角色标识
        /// </summary>
        public string RoleID { get; set; }
        /// <summary>
        /// 模块标识
        /// </summary>
        public string ModuleID { get; set; }
        /// <summary>
        /// 上级模块标识
        /// </summary>
        public string ParentID { get; set; }
        /// <summary>
        /// 是否拥有权限
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SysPermissionModel()
        {

        }

        /// <summary>
        /// 模块数据定义类构造函数
        /// </summary>
        /// <param name="roleID">角色标识</param>
        /// <param name="moduleID">模块标识</param>
        /// <param name="parentID">上级模块标识</param>
        /// <param name="enable">是否拥有权限</param>
        public SysPermissionModel(string roleID, string moduleID, string parentID, bool enable)
        {
            this.RoleID = roleID;
            this.ModuleID = moduleID;
            this.ParentID = parentID;
            this.Enable = enable;
        }
    }
    /// <summary>
    /// 常用功能
    /// </summary>
    public class SysCommonFuncModel
    {
        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleID { get; set; }
        /// <summary>
        /// 模块编码
        /// </summary>
        public string ModuleID { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable { get; set; }
    }
}
