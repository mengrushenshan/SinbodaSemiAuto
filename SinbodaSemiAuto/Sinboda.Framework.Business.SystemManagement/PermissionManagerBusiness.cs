using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Business.SystemManagement
{
    /// <summary>
    /// 权限设置实现业务类
    /// </summary>
    public class PermissionManagerBusiness : BusinessBase<PermissionManagerBusiness>
    {
        /// <summary>
        /// 权限模块接口
        /// </summary>
        IPermission permission = new PermissionOperation();

        /// <summary>
        /// 根据用户名获取角色
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>角色编码</returns>
        public OperationResult<string> GetRoleIDByUserID(string userName)
        {
            try
            {
                string result = permission.GetRoleIDByUserName(userName);
                return Result<string>(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetRoleIDByUserID", e);
                return Result<string>(e);
            }
        }

        #region 角色处理
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<SysRoleModel>> GetRoleList()
        {
            try
            {
                List<SysRoleModel> result = permission.GetAllRoles(SystemResources.Instance.CurrentUserName);
                return Result<List<SysRoleModel>>(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetRoleList", e);
                return Result<List<SysRoleModel>>(e);
            }
        }
        /// <summary>
        /// 增加角色
        /// </summary>
        /// <param name="roleID">角色编码</param>
        /// <param name="decription">描述</param>
        /// <param name="langID">语言编码</param>
        /// <param name="level">级别</param>
        /// <returns></returns>
        public OperationResult<PermissionErrorCode> AddRole(string roleID, string decription, int langID, int level)
        {
            try
            {
                PermissionErrorCode permissionErrorCode = permission.AddRole(roleID, decription, langID, level);
                return Result<PermissionErrorCode>(OperationResultEnum.SUCCEED, permissionErrorCode);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("AddRole", e);
                return Result<PermissionErrorCode>(e);
            }
        }
        /// <summary>
        /// 修改角色名
        /// </summary>
        /// <param name="roleID">角色编码</param>
        /// <param name="decription">描述</param>
        /// <param name="langID">语言编码</param>
        /// <param name="level">级别</param>
        /// <returns></returns>
        public OperationResult<PermissionErrorCode> ModifyRole(string roleID, string decription, int langID, int level)
        {
            try
            {
                PermissionErrorCode permissionErrorCode = permission.ModifyRole(roleID, decription, langID, level);
                return Result<PermissionErrorCode>(OperationResultEnum.SUCCEED, permissionErrorCode);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("ModifyRole", e);
                return Result<PermissionErrorCode>(e);
            }
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleID">角色编码</param>
        /// <returns></returns>
        public OperationResult<PermissionErrorCode> DelRole(string roleID)
        {
            try
            {
                PermissionErrorCode permissionErrorCode = permission.DelRole(roleID);
                return Result<PermissionErrorCode>(OperationResultEnum.SUCCEED, permissionErrorCode);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("DelRole", e);
                return Result<PermissionErrorCode>(e);
            }
        }
        #endregion

        #region 用户处理
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<SysUserModel>> GetUserList()
        {
            try
            {
                List<SysUserModel> result = permission.GetAllUsersByRoleID(permission.GetRoleIDByUserName(SystemResources.Instance.CurrentUserName));
                return Result<List<SysUserModel>>(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetUserList", e);
                return Result<List<SysUserModel>>(e);
            }
        }

        /// <summary>
        /// 增加用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="SelectRole">所属角色</param>
        /// <returns></returns>
        public OperationResult<PermissionErrorCode> AddUser(string userName, string Password, string SelectRole, string signPath)
        {
            try
            {
                PermissionErrorCode permissionErrorCode = permission.AddUser(userName, Password, SelectRole, signPath);
                return Result<PermissionErrorCode>(OperationResultEnum.SUCCEED, permissionErrorCode);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("AddUser", e);
                return Result<PermissionErrorCode>(e);
            }
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="SelectRole">所属角色</param>
        /// <returns></returns>
        public OperationResult<PermissionErrorCode> ModifyUser(string userName, string Password, string SelectRole, string signPath)
        {
            try
            {
                PermissionErrorCode permissionErrorCode = permission.ModifyUser(userName, Password, SelectRole, signPath);
                return Result<PermissionErrorCode>(OperationResultEnum.SUCCEED, permissionErrorCode);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("ModifyUser", e);
                return Result<PermissionErrorCode>(e);
            }
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public OperationResult<PermissionErrorCode> DelUser(string userName)
        {
            try
            {
                PermissionErrorCode permissionErrorCode = permission.DelUser(userName);
                return Result<PermissionErrorCode>(OperationResultEnum.SUCCEED, permissionErrorCode);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("DelUser", e);
                return Result<PermissionErrorCode>(e);
            }
        }
        #endregion

        #region 业务模块处理
        /// <summary>
        /// 获取模块列表
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<SysModuleModel>> GetModuleList()
        {
            try
            {
                List<SysModuleModel> result = permission.GetModuleList();
                List<SysModuleModel> resutTest = GetModuleItemList(result);
                return Result<List<SysModuleModel>>(OperationResultEnum.SUCCEED, resutTest);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetModuleList error", e);
                return Result<List<SysModuleModel>>(e);
            }
        }
        /// <summary>
        /// 获取所有
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private List<SysModuleModel> GetModuleItemList(List<SysModuleModel> result)
        {
            List<SysModuleModel> listresult = new List<SysModuleModel>();
            List<SysModuleModel> listAll = result;//.Where(p => p.DllName != "").ToList();
            List<SysModuleModel> listTmp = listAll.Where(p => p.ParentID == "").ToList();
            foreach (var item in listTmp)
            {
                SysModuleModel menuItem = new SysModuleModel();
                menuItem.ModuleID = item.ModuleID;
                menuItem.ModuleIDKey = item.ModuleIDKey;
                menuItem.Description = item.Description;
                menuItem.LangID = item.LangID;
                menuItem.ParentID = item.ParentID;
                menuItem.DllName = item.DllName;
                menuItem.NameSpace = item.NameSpace;
                menuItem.ShowOrder = item.ShowOrder;
                menuItem.IconCommon = item.IconCommon;
                menuItem.IsMenuShow = item.IsMenuShow;
                menuItem.ModuleType = item.ModuleType;
                menuItem.IsDisplayEnable = item.IsDisplayEnable;
                listresult.Add(menuItem);
                listresult.AddRange(FindChildrenMenu(listAll, item.ModuleIDKey));
            }
            return listresult;
        }
        /// <summary>
        /// 递归获取并填充子集
        /// </summary>
        /// <param name="listtmp"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        private List<SysModuleModel> FindChildrenMenu(List<SysModuleModel> listtmp, string parentID)
        {
            List<SysModuleModel> listresult = new List<SysModuleModel>();
            List<SysModuleModel> listTmp = listtmp.Where(p => p.ParentID == parentID).ToList();
            if (listTmp.Count > 0)
            {
                foreach (var item in listTmp)
                {
                    SysModuleModel menuItem = new SysModuleModel();
                    menuItem.ModuleID = item.ModuleID;
                    menuItem.ModuleIDKey = item.ModuleIDKey;
                    menuItem.Description = item.Description;
                    menuItem.LangID = item.LangID;
                    menuItem.ParentID = item.ParentID;
                    menuItem.DllName = item.DllName;
                    menuItem.NameSpace = item.NameSpace;
                    menuItem.ShowOrder = item.ShowOrder;
                    menuItem.IconCommon = item.IconCommon;
                    menuItem.IsMenuShow = item.IsMenuShow;
                    menuItem.ModuleType = item.ModuleType;
                    menuItem.IsDisplayEnable = item.IsDisplayEnable;
                    listresult.Add(menuItem);
                    listresult.AddRange(FindChildrenMenu(listtmp, item.ModuleIDKey));
                }
            }
            return listresult;
        }
        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="moduleData">模块信息</param>
        /// <returns></returns>
        public OperationResult<PermissionErrorCode> AddModule(SysModuleModel moduleData)
        {
            try
            {
                PermissionErrorCode permissionErrorCode = permission.AddModule(moduleData);
                return Result<PermissionErrorCode>(OperationResultEnum.SUCCEED, permissionErrorCode);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("AddModule error", e);
                return Result<PermissionErrorCode>(e);
            }
        }
        /// <summary>
        /// 修改模块
        /// </summary>
        /// <param name="moduleIDKey">类名称   原模块编号 的资源编码</param>
        /// <param name="moduleData">模块信息</param>
        /// <returns></returns>
        public OperationResult<PermissionErrorCode> ModifyModule(string moduleIDKey, SysModuleModel moduleData)
        {
            try
            {
                PermissionErrorCode permissionErrorCode = permission.ModifyModule(moduleIDKey, moduleData);
                return Result<PermissionErrorCode>(OperationResultEnum.SUCCEED, permissionErrorCode);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("ModifyModule error", e);
                return Result<PermissionErrorCode>(e);
            }
        }
        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="moduleIDKey">类名称 原模块编码 的资源编码</param>
        /// <returns></returns>
        public OperationResult<PermissionErrorCode> DelModule(string moduleIDKey)
        {
            try
            {
                PermissionErrorCode permissionErrorCode = permission.DelModule(moduleIDKey);
                return Result<PermissionErrorCode>(OperationResultEnum.SUCCEED, permissionErrorCode);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("DelModule error", e);
                return Result<PermissionErrorCode>(e);
            }
        }
        /// <summary>
        /// 是否存在唯一标识码
        /// </summary>
        /// <param name="moduleIDKey"></param>
        /// <returns></returns>
        public OperationResult ExistModuleIDKey(string moduleIDKey)
        {
            try
            {
                bool result = permission.ExistModuleIDKey(moduleIDKey);
                if (result)
                    return Result(OperationResultEnum.SUCCEED);
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("ExistModuleIDKey error", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 存在相同显示顺序的模块
        /// </summary>
        /// <param name="parentID">上级模块编码</param>
        /// <param name="showOrder">显示顺序</param>
        /// <returns></returns>
        public OperationResult ExistSameModuleShowOrder(string parentID, int showOrder)
        {
            try
            {
                bool result = permission.ExistSameModuleShowOrder(parentID, showOrder);
                if (result)
                    return Result(OperationResultEnum.SUCCEED);
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("ExistSameModuleShowOrder error", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public OperationResult HandlerModuleShowOrderTop(string moduleID, List<SysModuleModel> others)
        {
            try
            {
                bool result = permission.HandlerModuleShowOrderTop(moduleID, others);
                if (result)
                    return Result(OperationResultEnum.SUCCEED);
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("HandlerModuleShowOrderTop error", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 置底
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public OperationResult HandlerModuleShowOrderBottom(string moduleID, List<SysModuleModel> others)
        {
            try
            {
                bool result = permission.HandlerModuleShowOrderBottom(moduleID, others);
                if (result)
                    return Result(OperationResultEnum.SUCCEED);
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("HandlerModuleShowOrderBottom error", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 向上
        /// </summary>
        /// <param name="module"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public OperationResult HandlerModuleShowOrderBefore(SysModuleModel module, SysModuleModel other)
        {
            try
            {
                bool result = permission.HandlerModuleShowOrderBefore(module, other);
                if (result)
                    return Result(OperationResultEnum.SUCCEED);
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("HandlerModuleShowOrderBefore error", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 向下
        /// </summary>
        /// <param name="module"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public OperationResult HandlerModuleShowOrderNext(SysModuleModel module, SysModuleModel other)
        {
            try
            {
                bool result = permission.HandlerModuleShowOrderNext(module, other);
                if (result)
                    return Result(OperationResultEnum.SUCCEED);
                else
                    return Result(OperationResultEnum.FAILED);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("HandlerModuleShowOrderNext error", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        #endregion

        #region 权限处理
        /// <summary>
        /// 按角色获取模块
        /// </summary>
        /// <param name="roleID">角色编码</param>
        /// <returns></returns>
        public OperationResult<List<SysModuleModel>> GetPermissionByRole(string roleID)
        {
            try
            {
                List<SysModuleModel> result = permission.GetPermissionByRoleID(roleID);

                if (result.Count == 0)
                {
                    int level = permission.GetRoleLevelByUserName(SystemResources.Instance.CurrentUserName);

                    if (level == 1)
                    {
                        result = permission.GetAllPermission();
                    }
                }

                return Result<List<SysModuleModel>>(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetPermissionByRole error", e);
                return Result<List<SysModuleModel>>(e);
            }
        }
        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="roleID">角色编码</param>
        /// <param name="moduleIDKey">类名称  原模块编码 的资源编码</param>
        /// <param name="parentID">上级模块编码</param>
        /// <param name="enable">可用与否</param>
        /// <returns></returns>
        public OperationResult<PermissionErrorCode> ModifyPermission(string roleID, string moduleIDKey, string parentID, bool enable)
        {
            try
            {
                //权限表结构未变  ModuleID值改为业务表ModuleIDKey值  因ModuleID值改成不唯一了  Modified by kanxd 20181105
                PermissionErrorCode permissionErrorCode = permission.ModifyPermission(roleID, moduleIDKey, parentID, enable);
                return Result<PermissionErrorCode>(OperationResultEnum.SUCCEED, permissionErrorCode);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("ModifyPermission error", e);
                return Result<PermissionErrorCode>(e);
            }
        }
        #endregion
    }
}
