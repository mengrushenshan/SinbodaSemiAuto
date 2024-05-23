using Sinboda.Framework.Common;
using Sinboda.Framework.Common.CommonFunc;
using Sinboda.Framework.Common.DBOperateHelper;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.ModelsOperation
{
    /// <summary>
    /// 权限设置接口
    /// </summary>
    public interface IPermission
    {
        #region 角色增删改查
        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns>返回角色集合</returns>
        List<SysRoleModel> GetAllRoles(string userName);
        /// <summary>
        /// 增加角色
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <param name="roleName">角色名称</param>
        /// <param name="langID">角色名称</param>
        /// <param name="level">角色名称</param>
        /// <returns>成功返回ok，角色存在返回RoleExist，其他错误返回OtherError</returns>
        PermissionErrorCode AddRole(string roleID, string roleName, int langID, int level);
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <param name="newRoleName">角色名称</param>
        /// <param name="newLangID">角色名称</param>
        /// <param name="newLevel">角色名称</param>
        /// <returns>成功返回ok，角色不存在返回RoleNotExist，其他错误返回OtherError</returns>
        PermissionErrorCode ModifyRole(string roleID, string newRoleName, int newLangID, int newLevel);
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleID">角色编号</param>
        /// <returns>成功返回ok，角色不存在返回RoleNotExist，其他错误返回OtherError</returns>
        PermissionErrorCode DelRole(string roleID);
        #endregion

        #region 用户增删改查
        /// <summary>
        /// 获取登录用户
        /// </summary>
        /// <returns>返回除管理员以外的所有登录用户</returns>
        List<SysUserModel> GetAllLoginUsers();
        /// <summary>
        /// 根绝角色查询所有用户
        /// </summary>
        /// <param name="roleID">角色标识</param>
        /// <returns>返回当前角色下所有用户</returns>
        List<SysUserModel> GetAllUsersByRoleID(string roleID);
        /// <summary>
        /// 根绝角色查询该角色所有用户
        /// </summary>
        /// <param name="roleID">角色标识</param>
        /// <returns>返回当前角色下所有用户</returns>
        List<SysUserModel> GetUsersByRoleID(string roleID);
        /// <summary>
        /// 根据用户获取角色
        /// </summary>
        /// <param name="userName">要查询角色的userName</param>
        /// <returns>返回角色信息</returns>
        string GetRoleIDByUserName(string userName);
        /// <summary>
        /// 根据用户获取角色级别
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns>角色级别</returns>
        int GetRoleLevelByUserName(string userName);
        /// <summary>
        /// 增加用户
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="password">密码</param>
        /// <param name="roleID">所属角色标识</param>
        /// <returns>成功返回ok，用户存在返回UerExist，其他错误返回OtherError</returns>
        PermissionErrorCode AddUser(string userName, string password, string roleID, string signPath);
        /// <summary>
        /// 修改用户 
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="password">密码</param>
        /// <param name="roleID">所属角色标识</param>
        /// <returns>成功返回ok，用户不存在返回UerNotExist，其他错误返回OtherError</returns>
        PermissionErrorCode ModifyUser(string userName, string password, string roleID, string signPath);
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns>成功返回ok，用户不存在返回UerNotExist，其他错误返回OtherError</returns>
        PermissionErrorCode DelUser(string userName);
        #endregion

        #region 模块增删改查
        /// <summary>
        /// 获取模块列表
        /// </summary>
        /// <returns>返回模块列表</returns>
        List<SysModuleModel> GetModuleList();
        /// <summary>
        /// 增加模块
        /// </summary>
        /// <param name="moduleData">模块信息</param>
        /// <returns>成功返回ok，模块存在返回moduleExist，其他错误返回otherError</returns>
        PermissionErrorCode AddModule(SysModuleModel moduleData);
        /// <summary>
        /// 修改模块 
        /// </summary>
        /// <param name="moduleIDKey">类名称  原模块编码 的资源编码</param>
        /// <param name="moduleData">模块信息</param>
        /// <returns>成功返回ok，模块不存在返回moduleNotExist，其他错误返回otherError</returns> 
        PermissionErrorCode ModifyModule(string moduleIDKey, SysModuleModel moduleData);
        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="moduleIDKey">类名称 【模块标识】  的资源编码</param>
        /// <returns>成功返回ok，模块不存在返回moduleNotExist，其他错误返回otherError</returns>
        PermissionErrorCode DelModule(string moduleIDKey);
        /// <summary>
        /// 查看当前模块唯一标识码是否存在
        /// </summary>
        /// <param name="moduleIDKey"></param>
        /// <returns></returns>
        bool ExistModuleIDKey(string moduleIDKey);
        /// <summary>
        /// 存在相同显示顺序的模块
        /// </summary>
        /// <param name="parentID">上级模块编码</param>
        /// <param name="showOrder">显示顺序</param>
        /// <returns></returns>
        bool ExistSameModuleShowOrder(string parentID, int showOrder);
        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="moduleIDKey">当前选中类名称【模块编号】的资源编码</param>
        /// <param name="others">被动移动项</param>
        /// <returns></returns>
        bool HandlerModuleShowOrderTop(string moduleIDKey, List<SysModuleModel> others);
        /// <summary>
        /// 置底
        /// </summary>
        /// <param name="moduleIDKey">当前选中类名称【模块编号】的资源编码</param>
        /// <param name="others">被动移动项</param>
        /// <returns></returns>
        bool HandlerModuleShowOrderBottom(string moduleIDKey, List<SysModuleModel> others);
        /// <summary>
        /// 向前
        /// </summary>
        /// <param name="module">当前选中</param>
        /// <param name="other">被动移动项</param>
        /// <returns></returns>
        bool HandlerModuleShowOrderBefore(SysModuleModel module, SysModuleModel other);
        /// <summary>
        /// 向后
        /// </summary>
        /// <param name="module">当前选中</param>
        /// <param name="other">被动移动项</param>
        /// <returns></returns>
        bool HandlerModuleShowOrderNext(SysModuleModel module, SysModuleModel other);
        #endregion

        #region 权限增删改查
        /// <summary>
        /// 根据角色查询所有权限
        /// </summary>
        /// <returns>权限集合</returns>
        List<SysModuleModel> GetAllPermission();
        /// <summary>
        /// 根据角色查询所有权限
        /// </summary>
        /// <param name="roleID">角色标识</param>
        /// <returns>权限集合</returns>
        List<SysModuleModel> GetPermissionByRoleID(string roleID);
        /// <summary>
        /// 按用户获取权限列表
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns>权限集合</returns>
        List<SysModuleModel> GetPermissionByUserName(string userName);
        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="roleID">角色标识</param>
        /// <param name="moduleIDKey">类名称 原模块标识 的资源编码</param>
        /// <param name="parentID">上级模块标识</param>
        /// <param name="enable">权限</param>
        /// <returns>成功返回ok，不存在权限时返回permissionNotExist，其他错误返回otherError</returns>
        PermissionErrorCode ModifyPermission(string roleID, string moduleIDKey, string parentID, bool enable);
        #endregion

        #region 用户登录
        /// <summary>
        /// 查询用户名密码是否正确
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="password">密码</param>
        /// <returns>是否成功</returns>
        bool Login(string userName, string password);
        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        List<ModuleMenuItem> GetModuleMenuItemList(string userName);
        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Dictionary<string, bool> GetModulePermissionDic(string userName);
        #endregion
    }

    /// <summary>
    /// 权限设置实现类
    /// </summary>
    public class PermissionOperation : IPermission
    {
        /// <summary>
        /// 数据源
        /// </summary>
        string _DataSource = @"Data Source=";
        /// <summary>
        /// 程序路径
        /// </summary>
        string _Directory = MapPath.AppDir;
        /// <summary>
        /// 数据库
        /// </summary>
        string _FileName = @"Config\\PerUsers.db";
        /// <summary>
        /// db操作帮助类
        /// </summary>
        IDBHelper iDBHelper = new DBHelper(DBProvider.SQLite);
        /// <summary>
        /// 构造函数
        /// </summary>
        public PermissionOperation()
        {
            string _ConnectString = Path.Combine(_Directory, _FileName);
            bool result = iDBHelper.Init(_DataSource + _ConnectString);
            if (!result)
                iDBHelper = null;
        }

        #region 角色增删改查
        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns>返回角色集合</returns>
        public List<SysRoleModel> GetAllRoles(string userName)
        {
            try
            {
                List<SysRoleModel> list = new List<SysRoleModel>();
                int level = -1;
                object ormObject = iDBHelper.ExcuteQueryObject(string.Format("select Roles.Level from Roles,Users where Roles.RoleID=Users.RoleID and UserName='{0}'", userName));
                if (ormObject != null)
                {
                    level = (int)ormObject;
                }
                else
                {
                    return list;
                }
                DataTable dt = new DataTable();
                dt = iDBHelper.ExcuteQueryDataTable(string.Format("select * from Roles where Level>='{0}'", level));
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRowView item in dt.DefaultView)
                    {
                        list.Add(new SysRoleModel { RoleID = item["RoleID"].ToString(), Description = item["Description"].ToString(), LangID = int.Parse(item["LangID"].ToString()), Level = int.Parse(item["Level"].ToString()) });
                    }
                    foreach (var item in list)
                    {
                        item.Description = item.LangID < SystemResources.Instance.LanguageArray.Length ? SystemResources.Instance.LanguageArray[item.LangID] : item.Description;
                    }
                    return list;
                }
                else
                {
                    return list;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 增加角色
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <param name="description">角色名称</param>
        /// <param name="langID">角色名称</param>
        /// <param name="level">角色名称</param>
        /// <returns>成功返回ok，角色存在返回RoleExist，其他错误返回OtherError</returns>
        public PermissionErrorCode AddRole(string roleID, string description, int langID, int level)
        {
            try
            {
                List<string> sqls = new List<string>();
                DataTable ormDT = iDBHelper.ExcuteQueryDataTable($"select * from Roles where RoleID='{roleID}'");
                if (ormDT != null && ormDT.Rows.Count > 0)
                {
                    return PermissionErrorCode.RoleExist;
                }

                // 添加角色
                sqls.Add(string.Format("insert into Roles values ('{0}','{1}','{2}','{3}')", roleID, description, langID, level));

                // 添加角色菜单
                var modules = iDBHelper.ExcuteQueryDataTable("select * from Modules");
                foreach (DataRow row in modules.Rows)
                {
                    sqls.Add($"insert into Permission values('{roleID}', '{row["ModuleIDKey"]}', '{row["ParentID"]}', 0)");
                }

                int ormInt = iDBHelper.ExecuteNonQueryIntTransaction(sqls);
                if (ormInt > 0)
                {
                    return PermissionErrorCode.OK;
                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <param name="newDescription">角色名称</param>
        /// <param name="langID">语言编码</param>
        /// <param name="level">级别</param>
        /// <returns>成功返回ok，角色不存在返回RoleNotExist，其他错误返回OtherError</returns>
        public PermissionErrorCode ModifyRole(string roleID, string newDescription, int langID, int level)
        {
            try
            {
                string sql = string.Empty;
                sql = string.Format("select * from Roles where RoleID='{0}'", roleID);
                DataTable ormDT = iDBHelper.ExcuteQueryDataTable(sql);
                if (ormDT != null && ormDT.Rows.Count > 0)
                {

                }
                else
                {
                    return PermissionErrorCode.RoleNotExist;
                }
                sql = string.Format("update Roles set Description= '{0}',LangID='{1}',Level='{2}' where RoleID = '{3}'", newDescription, langID, level, roleID);
                int ormInt = iDBHelper.ExcuteNonQueryInt(sql);
                if (ormInt > 0)
                {
                    return PermissionErrorCode.OK;
                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleID">角色编号</param>
        /// <returns>成功返回ok，角色不存在返回RoleNotExist，其他错误返回OtherError</returns>
        public PermissionErrorCode DelRole(string roleID)
        {
            try
            {
                string sql = string.Empty;
                sql = string.Format("select * from Roles where RoleID='{0}'", roleID);
                DataTable ormDT = iDBHelper.ExcuteQueryDataTable(sql);
                if (ormDT != null && ormDT.Rows.Count > 0)
                {

                }
                else
                {
                    return PermissionErrorCode.RoleNotExist;
                }
                List<string> excuteSqlList = new List<string>();
                //删除角色
                sql = string.Format("delete from Roles where RoleID='{0}'", roleID);
                excuteSqlList.Add(sql);
                //删除相关用户
                sql = string.Format("delete from Users where RoleID='{0}'", roleID);
                excuteSqlList.Add(sql);
                //删除相关权限
                sql = string.Format("delete from Permission where RoleID='{0}'", roleID);
                excuteSqlList.Add(sql);
                //删除相关常用功能
                //sql = string.Format("delete from CommonFunc where RoleID='{0}'", roleID);
                //excuteSqlList.Add(sql);
                int ormInt = iDBHelper.ExecuteNonQueryIntTransaction(excuteSqlList);
                if (ormInt > 0)
                {
                    return PermissionErrorCode.OK;
                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        #endregion

        #region 用户增删改查
        /// <summary>
        /// 获取登录用户
        /// </summary>
        /// <returns>返回管理员和普通用户的所有登录用户</returns>
        public List<SysUserModel> GetAllLoginUsers()
        {
            List<SysUserModel> list = new List<SysUserModel>();
            try
            {
                string sql = string.Format("select u.*, r.LangID from Users as u, Roles as r where u.RoleID = r.RoleID and level>=3");
                DataTable dt = new DataTable();
                dt = iDBHelper.ExcuteQueryDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRowView item in dt.DefaultView)
                    {
                        list.Add(new SysUserModel(
                            item["UserName"].ToString(),
                            DataEncryptionHelper.DecryptDES(item["Password"].ToString()),
                            item["RoleID"].ToString(),
                            item["SignPath"].ToString(),
                            item["LangID"].ToString()));
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 根绝角色查询所有用户
        /// </summary>
        /// <param name="roleID">角色标识</param>
        /// <returns>返回当前角色下所有用户</returns>
        public List<SysUserModel> GetAllUsersByRoleID(string roleID)
        {
            List<SysUserModel> list = new List<SysUserModel>();
            try
            {
                string sql = string.Format("select Level from Roles where RoleID='{0}'", roleID);
                int levelTmp = 0;
                object ormObject = iDBHelper.ExcuteQueryObject(sql);
                if (ormObject != null)
                {
                    levelTmp = (int)ormObject;
                }
                else
                {
                    return list;
                }
                sql = string.Format("select u.*, r.LangID from Users as u, Roles as r where u.RoleID = r.RoleID and u.RoleID in (select RoleID from Roles where Level>='{0}')",
                            levelTmp);
                DataTable dt = new DataTable();
                dt = iDBHelper.ExcuteQueryDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRowView item in dt.DefaultView)
                    {
                        list.Add(new SysUserModel(
                            item["UserName"].ToString(),
                            DataEncryptionHelper.DecryptDES(item["Password"].ToString()),
                            item["RoleID"].ToString(),
                            item["SignPath"].ToString(),
                            item["LangID"].ToString()));
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// 根绝角色查询该角色的所有用户
        /// </summary>
        /// <param name="roleID">角色标识</param>
        /// <returns>返回当前角色下所有用户</returns>
        public List<SysUserModel> GetUsersByRoleID(string roleID)
        {
            List<SysUserModel> list = new List<SysUserModel>();
            try
            {
                string sql = string.Format("select Level from Roles where RoleID='{0}'", roleID);
                int levelTmp = 0;
                object ormObject = iDBHelper.ExcuteQueryObject(sql);
                if (ormObject != null)
                {
                    levelTmp = (int)ormObject;
                }
                else
                {
                    return list;
                }
                sql = string.Format("select u.*, r.LangID from Users as u, Roles as r where u.RoleID = r.RoleID and u.RoleID in (select RoleID from Roles where Level='{0}')",
                            levelTmp);
                DataTable dt = new DataTable();
                dt = iDBHelper.ExcuteQueryDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRowView item in dt.DefaultView)
                    {
                        list.Add(new SysUserModel(
                            item["UserName"].ToString(),
                            DataEncryptionHelper.DecryptDES(item["Password"].ToString()),
                            item["RoleID"].ToString(),
                            item["SignPath"].ToString(),
                            item["LangID"].ToString()));
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// 根据用户获取角色
        /// </summary>
        /// <param name="userName">要查询角色的userName</param>
        /// <returns>返回角色信息</returns>
        public string GetRoleIDByUserName(string userName)
        {
            try
            {
                string sql = string.Format("select RoleID from Users where UserName='{0}'", userName);
                object ormObject = iDBHelper.ExcuteQueryObject(sql);
                if (ormObject != null)
                {
                    return (string)ormObject;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// 根据用户获取角色级别
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns>角色级别</returns>
        public int GetRoleLevelByUserName(string userName)
        {
            try
            {
                string sql = string.Format("select Level from Roles left join Users on Roles.RoleID=Users.RoleID where UserName='{0}'", userName);
                object ormObject = iDBHelper.ExcuteQueryObject(sql);
                if (ormObject != null)
                {
                    return (int)ormObject;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 根据用户获取角色级别
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns>角色级别</returns>
        public int GetRoleLevelByRoleID(string roleID)
        {
            try
            {
                string sql = string.Format("select Level from Roles where RoleID='{0}'", roleID);
                object ormObject = iDBHelper.ExcuteQueryObject(sql);
                if (ormObject != null)
                {
                    return (int)ormObject;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 增加用户
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="password">密码</param>
        /// <param name="roleID">所属角色标识</param>
        /// <returns>成功返回ok，用户存在返回UerExist，其他错误返回OtherError</returns>
        public PermissionErrorCode AddUser(string userName, string password, string roleID, string signPath)
        {
            try
            {
                string sql = string.Format("select * from Users where UserName='{0}'", userName);
                DataTable ormDT = iDBHelper.ExcuteQueryDataTable(sql);
                if (ormDT != null && ormDT.Rows.Count > 0)
                {
                    return PermissionErrorCode.UserExist;
                }
                sql = string.Format("insert into Users values ('{0}','{1}','{2}','{3}')", userName, DataEncryptionHelper.EncryptDES(password), roleID, signPath);
                int ormInt = iDBHelper.ExcuteNonQueryInt(sql);
                if (ormInt > 0)
                {
                    return PermissionErrorCode.OK;
                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 修改用户 
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="password">密码</param>
        /// <param name="roleID">所属角色标识</param>
        /// <returns>成功返回ok，用户不存在返回UerNotExist，其他错误返回OtherError</returns>
        public PermissionErrorCode ModifyUser(string userName, string password, string roleID, string signPath)
        {
            try
            {
                string sql = string.Format("select * from Users where UserName='{0}'", userName);
                DataTable ormDT = iDBHelper.ExcuteQueryDataTable(sql);
                if (ormDT != null && ormDT.Rows.Count > 0)
                {

                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
                sql = string.Format("update Users set Password='{0}',RoleID='{1}',SignPath='{3}' where UserName='{2}'", DataEncryptionHelper.EncryptDES(password), roleID, userName, signPath);
                int ormInt = iDBHelper.ExcuteNonQueryInt(sql);
                if (ormInt > 0)
                {
                    return PermissionErrorCode.OK;
                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns>成功返回ok，用户不存在返回UerNotExist，其他错误返回OtherError</returns>
        public PermissionErrorCode DelUser(string userName)
        {
            try
            {
                string sql = string.Format("select * from Users where UserName='{0}'", userName);
                DataTable ormDT = iDBHelper.ExcuteQueryDataTable(sql);
                if (ormDT != null && ormDT.Rows.Count > 0)
                {

                }
                else
                {
                    return PermissionErrorCode.RoleNotExist;
                }
                sql = string.Format("delete from Users where UserName='{0}'", userName);
                int ormInt = iDBHelper.ExcuteNonQueryInt(sql);
                if (ormInt > 0)
                {
                    return PermissionErrorCode.OK;
                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }

        }
        #endregion

        #region 模块增删改查
        /// <summary>
        /// 获取模块列表
        /// </summary>
        /// <returns>返回模块列表</returns>
        public List<SysModuleModel> GetModuleList()
        {
            List<SysModuleModel> list = new List<SysModuleModel>();
            try
            {
                DataTable dt = new DataTable();
                dt = iDBHelper.ExcuteQueryDataTable(string.Format("select * from Modules order by showorder"));
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRowView item in dt.DefaultView)
                    {
                        list.Add(new SysModuleModel(
                            item["ModuleID"].ToString(),
                            item["ModuleIDKey"].ToString(),
                            item["ParentID"].ToString(),
                            item["Description"].ToString(),
                            Int32.Parse(item["LangID"].ToString()),
                            item["DllName"].ToString(),
                            item["NameSpace"].ToString(),
                            Int32.Parse(item["ShowOrder"].ToString()),
                            item["IconCommon"].ToString(),
                            false,
                            (bool)item["IsMenuShow"],
                            Int32.Parse(item["ModuleType"].ToString()),
                            (bool)item["DisplayEnable"]
                            ));
                    }
                    return list;
                }
                else
                {
                    return list;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 增加模块
        /// </summary>
        /// <param name="moduleData">模块信息</param>
        /// <returns>成功返回ok，模块存在返回moduleExist，其他错误返回otherError</returns>
        public PermissionErrorCode AddModule(SysModuleModel moduleData)
        {
            try
            {
                //查询所有角色
                List<string> roleList = new List<string>();
                string sql = string.Format("select RoleID from Roles");
                DataTable ormDTRole = iDBHelper.ExcuteQueryDataTable(sql);
                if (ormDTRole != null && ormDTRole.Rows.Count > 0)
                {
                    foreach (DataRowView item in ormDTRole.DefaultView)
                    {
                        roleList.Add(item["RoleID"].ToString());
                    }
                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
                List<string> excuteSqlList = new List<string>();
                //添加模块
                sql = string.Format("insert into Modules values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                    moduleData.ModuleID, moduleData.ModuleIDKey, moduleData.Description, moduleData.LangID,
                    moduleData.ParentID, moduleData.DllName, moduleData.NameSpace, moduleData.ShowOrder, moduleData.IconCommon,
                    moduleData.ModuleType, moduleData.IsMenuShow, moduleData.IsDisplayEnable);

                excuteSqlList.Add(sql);

                foreach (var item in roleList)
                {
                    //添加到权限表（每个角色都添加一次） //不更改权限表字段  更改字段内容 因ModuleID字段在业务表里不唯一 ModuleIDKey是唯一的  Modified  by kanxd 20181105
                    sql = string.Format("insert into Permission values ('{0}','{1}','{2}','{3}')", item, moduleData.ModuleIDKey, moduleData.ParentID, true);
                    excuteSqlList.Add(sql);
                }

                int ormInt = iDBHelper.ExecuteNonQueryIntTransaction(excuteSqlList);
                if (ormInt > 0)
                {
                    return PermissionErrorCode.OK;
                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 修改模块 
        /// </summary>
        /// <param name="moduleIDKey">类名称   原模块编号 的资源编码</param>
        /// <param name="moduleData">模块信息</param>
        /// <returns>成功返回ok，模块不存在返回moduleNotExist，其他错误返回otherError</returns> 
        public PermissionErrorCode ModifyModule(string moduleIDKey, SysModuleModel moduleData)
        {
            try
            {
                //查询所有角色
                List<string> roleList = new List<string>();
                string sql = string.Format("select RoleID from Roles");
                DataTable ormDTRole = iDBHelper.ExcuteQueryDataTable(sql);
                if (ormDTRole != null && ormDTRole.Rows.Count > 0)
                {
                    foreach (DataRowView item in ormDTRole.DefaultView)
                    {
                        roleList.Add(item["RoleID"].ToString());
                    }
                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
                List<string> excuteSqlList = new List<string>();
                //更新模块
                sql = string.Format("update Modules set ModuleID='{0}',ModuleIDKey='{1}',Description='{2}',LangID='{3}',ParentID='{4}',DllName='{5}',NameSpace='{6}',ShowOrder='{7}'," +
                    "IconCommon='{8}',ModuleType='{9}',IsMenuShow='{10}',DisplayEnable='{11}' where ModuleIDKey='{12}'",
                    moduleData.ModuleID, moduleData.ModuleIDKey, moduleData.Description, moduleData.LangID, moduleData.ParentID, moduleData.DllName, moduleData.NameSpace, moduleData.ShowOrder,
                    moduleData.IconCommon, moduleData.ModuleType, moduleData.IsMenuShow, moduleData.IsDisplayEnable, moduleIDKey);
                excuteSqlList.Add(sql);
                //更新到权限表（更新即可）  //不更改权限表字段  更改字段内容 因ModuleID字段在业务表里不唯一 ModuleIDKey是唯一的  Modified  by kanxd 20181105
                sql = string.Format("update Permission set ParentID='{0}',ModuleID='{1}' where ModuleID='{2}'", moduleData.ParentID, moduleData.ModuleIDKey, moduleIDKey);
                excuteSqlList.Add(sql);
                int ormInt = iDBHelper.ExecuteNonQueryIntTransaction(excuteSqlList);
                if (ormInt > 0)
                {
                    return PermissionErrorCode.OK;
                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="moduleIDKey">类名称【模块标识】的资源编码</param>
        /// <returns>成功返回ok，模块不存在返回moduleNotExist，其他错误返回otherError</returns>
        public PermissionErrorCode DelModule(string moduleIDKey)
        {
            try
            {
                string sql = string.Format("select * from Modules where ModuleIDKey='{0}'", moduleIDKey);
                DataTable ormDT = iDBHelper.ExcuteQueryDataTable(sql);
                if (ormDT != null && ormDT.Rows.Count > 0)
                {

                }
                else
                {
                    return PermissionErrorCode.ModuleNotExist;
                }
                List<string> excuteSqlList = new List<string>();
                //删除模块
                sql = string.Format("delete from Modules  where ModuleIDKey='{0}'", moduleIDKey);
                excuteSqlList.Add(sql);
                //删除相关权限
                sql = string.Format("delete from Permission where ModuleID='{0}'", moduleIDKey);
                excuteSqlList.Add(sql);
                int ormInt = iDBHelper.ExecuteNonQueryIntTransaction(excuteSqlList);
                if (ormInt > 0)
                {

                    return PermissionErrorCode.OK;
                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 模块唯一标识码是否已存在
        /// </summary>
        /// <param name="moduleIDKey"></param>
        /// <returns></returns>
        public bool ExistModuleIDKey(string moduleIDKey)
        {
            try
            {
                string sql = string.Format("select * from Modules where ModuleIDKey='{0}'", moduleIDKey);
                DataTable ormDT = iDBHelper.ExcuteQueryDataTable(sql);
                if (ormDT != null && ormDT.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 存在相同显示顺序的模块
        /// </summary>
        /// <param name="parentID">上级模块编码</param>
        /// <param name="showOrder">显示顺序</param>
        /// <returns></returns>
        public bool ExistSameModuleShowOrder(string parentID, int showOrder)
        {
            try
            {
                string sql = string.Format("select * from Modules where ParentID='{0}' and ShowOrder='{1}'", parentID, showOrder);
                DataTable ormDT = iDBHelper.ExcuteQueryDataTable(sql);
                if (ormDT != null && ormDT.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="moduleIDKey">当前选中模块编号</param>
        /// <param name="others">被动移动项</param>
        /// <returns></returns>
        public bool HandlerModuleShowOrderTop(string moduleIDKey, List<SysModuleModel> others)
        {
            List<string> sqlList = new List<string>();
            string sql = string.Empty;
            sqlList.Add(string.Format("update Modules set ShowOrder='{0}' where ModuleIDKey='{1}'", 1, moduleIDKey));
            for (int i = 0; i < others.Count(); i++)
            {
                sqlList.Add(string.Format("update Modules set ShowOrder='{0}' where ModuleIDKey='{1}'", i + 2, others[i].ModuleIDKey));
            }
            int result = iDBHelper.ExecuteNonQueryIntTransaction(sqlList);
            if (result > 0)
                return true;
            else return false;
        }
        /// <summary>
        /// 置底
        /// </summary>
        /// <param name="moduleIDKey">当前选中模块编号</param>
        /// <param name="others">被动移动项</param>
        /// <returns></returns>
        public bool HandlerModuleShowOrderBottom(string moduleIDKey, List<SysModuleModel> others)
        {
            List<string> sqlList = new List<string>();
            string sql = string.Empty;
            sqlList.Add(string.Format("update Modules set ShowOrder='{0}' where ModuleIDKey='{1}'", others.Count() + 1, moduleIDKey));
            for (int i = 0; i < others.Count(); i++)
            {
                sqlList.Add(string.Format("update Modules set ShowOrder='{0}' where ModuleIDKey='{1}'", i + 1, others[i].ModuleIDKey));
            }
            int result = iDBHelper.ExecuteNonQueryIntTransaction(sqlList);
            if (result > 0)
                return true;
            else return false;
        }
        /// <summary>
        /// 向前
        /// </summary>
        /// <param name="module">当前选中</param>
        /// <param name="other">被动移动项</param>
        /// <returns></returns>
        public bool HandlerModuleShowOrderBefore(SysModuleModel module, SysModuleModel other)
        {
            List<string> sqlList = new List<string>();
            string sql = string.Empty;
            sqlList.Add(string.Format("update Modules set ShowOrder='{0}' where ModuleIDKey='{1}'", module.ShowOrder - 1, module.ModuleIDKey));
            sqlList.Add(string.Format("update Modules set ShowOrder='{0}' where ModuleIDKey='{1}'", other.ShowOrder + 1, other.ModuleIDKey));
            int result = iDBHelper.ExecuteNonQueryIntTransaction(sqlList);
            if (result > 0)
                return true;
            else return false;
        }
        /// <summary>
        /// 向后
        /// </summary>
        /// <param name="module">当前选中</param>
        /// <param name="other">被动移动项</param>
        /// <returns></returns>
        public bool HandlerModuleShowOrderNext(SysModuleModel module, SysModuleModel other)
        {
            List<string> sqlList = new List<string>();
            string sql = string.Empty;
            sqlList.Add(string.Format("update Modules set ShowOrder='{0}' where ModuleIDKey='{1}'", module.ShowOrder + 1, module.ModuleIDKey));
            sqlList.Add(string.Format("update Modules set ShowOrder='{0}' where ModuleIDKey='{1}'", other.ShowOrder - 1, other.ModuleIDKey));
            int result = iDBHelper.ExecuteNonQueryIntTransaction(sqlList);
            if (result > 0)
                return true;
            else return false;
        }
        #endregion

        #region 权限增删改查
        /// <summary>
        /// 根据角色查询所有权限
        /// </summary>
        /// <returns>权限集合</returns>
        public List<SysModuleModel> GetAllPermission()
        {
            List<SysModuleModel> list = new List<SysModuleModel>();
            try
            {
                string sql = string.Empty;
                sql = string.Format("select * from Modules");
                DataTable dt = new DataTable();
                dt = iDBHelper.ExcuteQueryDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRowView item in dt.DefaultView)
                    {
                        list.Add(new SysModuleModel(
                            item["ModuleID"].ToString(),
                            item["ModuleIDKey"].ToString(),
                            item["ParentID"].ToString(),
                            item["Description"].ToString(),
                            int.Parse(item["LangID"].ToString()),
                            item["DllName"].ToString(),
                            item["NameSpace"].ToString(),
                            int.Parse(item["ShowOrder"].ToString()),
                            item["IconCommon"].ToString(),
                            false,
                            (bool)item["IsMenuShow"],
                             int.Parse(item["ModuleType"].ToString())
                            ));
                    }
                    return list;
                }
                else
                {
                    return list;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 根据角色查询所有权限
        /// </summary>
        /// <param name="roleID">角色标识</param>
        /// <returns>权限集合</returns>
        public List<SysModuleModel> GetPermissionByRoleID(string roleID)
        {
            List<SysModuleModel> list = new List<SysModuleModel>();
            try
            {
                string sql = string.Empty;

                sql = string.Format("select Modules.*,Permission.Enable from Permission,Modules where Permission.ModuleID=Modules.ModuleIDKey and RoleID='{0}'",
                    roleID);

                DataTable dt = new DataTable();
                dt = iDBHelper.ExcuteQueryDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRowView item in dt.DefaultView)
                    {
                        list.Add(new SysModuleModel(
                            item["ModuleID"].ToString(),
                            item["ModuleIDKey"].ToString(),
                            item["ParentID"].ToString(),
                            item["Description"].ToString(),
                            int.Parse(item["LangID"].ToString()),
                            item["DllName"].ToString(),
                            item["NameSpace"].ToString(),
                            int.Parse(item["ShowOrder"].ToString()),
                            item["IconCommon"].ToString(),
                            (bool)item["Enable"],
                            (bool)item["IsMenuShow"],
                             int.Parse(item["ModuleType"].ToString()),
                             (bool)item["DisplayEnable"]
                            ));
                    }
                    return list;
                }
                else
                {
                    return list;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 按用户获取权限列表
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns>权限集合</returns>
        public List<SysModuleModel> GetPermissionByUserName(string userName)
        {
            List<SysModuleModel> list = new List<SysModuleModel>();
            try
            {
                return GetPermissionByRoleID(GetRoleIDByUserName(userName));
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="roleID">角色标识</param>
        /// <param name="moduleIDKey">类名称  原模块标识 的资源编码</param>
        /// <param name="parentID">上级模块标识</param>
        /// <param name="enable">权限</param>
        /// <returns>成功返回ok，不存在权限时返回permissionNotExist，其他错误返回otherError</returns>
        public PermissionErrorCode ModifyPermission(string roleID, string moduleIDKey, string parentID, bool enable)
        {
            try
            {
                //权限表结构未变  ModuleID值改为业务表ModuleIDKey值  因ModuleID值改成不唯一了  Modified by kanxd 20181105
                string sql = string.Empty;
                sql = string.Format("select * from Permission where RoleID='{0}' and ModuleID='{1}' and ParentID='{2}'", roleID, moduleIDKey, parentID);
                DataTable ormDT = iDBHelper.ExcuteQueryDataTable(sql);
                if (ormDT != null && ormDT.Rows.Count > 0)
                {

                }
                else
                {
                    return PermissionErrorCode.PermissionNotExist;
                }
                sql = string.Format("update Permission set Enable='{0}' where RoleID='{1}' and ModuleID='{2}' and ParentID='{3}'", enable, roleID, moduleIDKey, parentID);
                int ormInt = iDBHelper.ExcuteNonQueryInt(sql);
                if (ormInt > 0)
                {
                    return PermissionErrorCode.OK;
                }
                else
                {
                    return PermissionErrorCode.OtherError;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        #endregion

        #region 用户登录
        /// <summary>
        /// 查询用户名密码是否正确
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="password">密码</param>
        /// <returns>是否成功</returns>
        public bool Login(string userName, string password)
        {
            string sql = string.Format("select * from Users where UserName='{0}' and Password='{1}'", userName, DataEncryptionHelper.EncryptDES(password));
            try
            {
                DataTable ormDT = iDBHelper.ExcuteQueryDataTable(sql);
                if (ormDT != null && ormDT.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error(e.Message, e);
                throw e;
            }
        }
        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<ModuleMenuItem> GetModuleMenuItemList(string userName)
        {
            List<ModuleMenuItem> listresult = new List<ModuleMenuItem>();
            List<SysModuleModel> listAll = GetPermissionByUserName(userName).Where(p => p.DllName != "" && p.Enable == true).OrderBy(p => p.ShowOrder).ToList();
            List<SysModuleModel> listTmp = listAll.Where(p => p.ParentID == "").OrderBy(p => p.ShowOrder).ToList();
            foreach (var item in listTmp)
            {
                ModuleMenuItem menuItem = new ModuleMenuItem();
                menuItem.Id = item.ModuleIDKey;
                menuItem.Name = SystemResources.Instance.LanguageArray[item.LangID];
                menuItem.Glyph = item.IconCommon;
                menuItem.Source = item.NameSpace + "." + item.ModuleID;
                menuItem.ModuleName = item.DllName;
                menuItem.IsMenuShow = item.IsMenuShow;
                menuItem.ModuleType = (ModuleType)item.ModuleType;
                menuItem.AddItems(FindChildrenMenu(listAll, item.ModuleIDKey));
                listresult.Add(menuItem);
            }
            return listresult;
        }
        /// <summary>
        /// 递归获取并填充子集
        /// </summary>
        /// <param name="listtmp"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        List<ModuleMenuItem> FindChildrenMenu(List<SysModuleModel> listtmp, string parentID)
        {
            List<ModuleMenuItem> listresult = new List<ModuleMenuItem>();
            List<SysModuleModel> listTmp = listtmp.Where(p => p.ParentID == parentID).ToList();
            if (listTmp.Count > 0)
            {
                foreach (var item in listTmp)
                {
                    ModuleMenuItem menuItem = new ModuleMenuItem();
                    menuItem.Name = SystemResources.Instance.LanguageArray[item.LangID];
                    menuItem.Id = item.ModuleID;
                    menuItem.Glyph = item.IconCommon;
                    menuItem.Source = item.NameSpace + "." + item.ModuleID;
                    menuItem.ModuleName = item.DllName;
                    menuItem.IsMenuShow = item.IsMenuShow;
                    menuItem.ModuleType = (ModuleType)item.ModuleType;
                    menuItem.AddItems(FindChildrenMenu(listtmp, item.ModuleIDKey));
                    listresult.Add(menuItem);
                }
            }
            return listresult;
        }
        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Dictionary<string, bool> GetModulePermissionDic(string userName)
        {
            Dictionary<string, bool> dicResult = new Dictionary<string, bool>();
            List<SysModuleModel> listTmp = GetPermissionByUserName(userName);
            foreach (var item in listTmp)
            {
                dicResult.Add(item.ModuleIDKey, item.Enable);
            }
            return dicResult;
        }
        #endregion
    }
}
