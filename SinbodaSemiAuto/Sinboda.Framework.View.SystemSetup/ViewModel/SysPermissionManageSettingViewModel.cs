using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Business.SystemManagement;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.View.SystemSetup.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class SysPermissionManageSettingViewModel : NavigationViewModelBase
    {
        /// <summary>
        /// 用户管理业务逻辑接口
        /// </summary>
        PermissionManagerBusiness pBusiness = new PermissionManagerBusiness();

        /// <summary>
        /// 构造函数
        /// </summary>   
        public SysPermissionManageSettingViewModel()
        {
            PageTitle = SystemResources.Instance.LanguageArray[1955];
            //GetRoleList();
            InitializePermissionCommand();
        }
        #region 用户权限
        #region 属性
        /// <summary>
        /// 角色列表中的选中项
        /// </summary>
        SysRoleModel _SelectPermissionRole;
        /// <summary>
        /// 角色列表中的选中项属性
        /// </summary>
        public SysRoleModel SelectPermissionRole
        {
            get { return _SelectPermissionRole; }
            set
            {
                Set(ref _SelectPermissionRole, value);
                GetPermissionTree();
            }
        }
        /// <summary>
        /// 权限树列表
        /// </summary>
        ObservableCollection<SysModuleModel> _PermissionTreeList = new ObservableCollection<SysModuleModel>();
        /// <summary>
        /// 权限树列表属性
        /// </summary>
        public ObservableCollection<SysModuleModel> PermissionTreeList
        {
            get { return _PermissionTreeList; }
            set { Set(ref _PermissionTreeList, value); }
        }
        /// <summary>
        /// 权限列表中的选中项
        /// </summary>
        SysModuleModel _SelectPermission;
        /// <summary>
        /// 权限列表中的选中项属性
        /// </summary>
        public SysModuleModel SelectPermission
        {
            get { return _SelectPermission; }
            set { Set(ref _SelectPermission, value); }
        }
        /// <summary>
        /// 角色列表
        /// </summary>
        ObservableCollection<SysRoleModel> _RoleList = new ObservableCollection<SysRoleModel>();
        /// <summary>
        /// 角色列表属性
        /// </summary>
        public ObservableCollection<SysRoleModel> RoleList
        {
            get { return _RoleList; }
            set { Set(ref _RoleList, value); }
        }
        #endregion

        #region 命令
        private void InitializePermissionCommand()
        {
            SavePermissionCommand = new RelayCommand(SavePermissionMethod);
        }
        /// <summary>
        /// 保存权限命令
        /// </summary>
        public RelayCommand SavePermissionCommand { get; set; }
        /// <summary>
        /// 保存权限按钮的响应函数
        /// </summary>
        private void SavePermissionMethod()
        {
            if (SelectPermissionRole == null)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6458]); //"请选择角色后再进行权限保存！");// 3202:只能修改低级别角色权限
                return;
            }
            string id = string.Empty;
            OperationResult<string> operationResult = pBusiness.GetRoleIDByUserID(SystemResources.Instance.CurrentUserName);
            if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                id = operationResult.Results;
            else
            {
                ShowMessageError(operationResult.Message);
                return;
            }
            //判断当前用户不可更改本身权限
            if (!string.IsNullOrEmpty(id) && id == SelectPermissionRole.RoleID)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[3202]);// 3202:只能修改低级别角色权限
                return;
            }
            foreach (SysModuleModel item in PermissionTreeList)
            {
                pBusiness.ModifyPermission(SelectPermissionRole.RoleID, item.ModuleIDKey, item.ParentID, item.Enable);
            }
            ShowMessageComplete(SystemResources.Instance.LanguageArray[609]);//29提示609保存成功
            WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[45], SystemResources.Instance.LanguageArray[1955], SystemResources.Instance.LanguageArray[SelectPermissionRole.LangID]));//29提示609保存成功
        }
        #endregion

        #region 方法
        /// <summary>
        /// 更换角色后创建权限树
        /// </summary>
        private void GetPermissionTree()
        {
            if (SelectPermissionRole != null)
            {
                List<SysModuleModel> list = new List<SysModuleModel>();
                OperationResult<List<SysModuleModel>> operationResult = pBusiness.GetPermissionByRole(SelectPermissionRole.RoleID);
                if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    //list = operationResult.Results;
                    List<SysModuleModel> listParentID = operationResult.Results.Where(p => p.ParentID == "").OrderBy(p => p.ShowOrder).ToList();
                    foreach (var itemP in listParentID)
                    {
                        if (itemP.IsDisplayEnable || SelectPermissionRole.Level == 1)
                        {
                            list.Add(itemP);
                        }

                        if (string.IsNullOrEmpty(itemP.ModuleID))
                            continue;
                        foreach (var item in operationResult.Results.Where(p => p.ParentID == itemP.ModuleID).OrderBy(p => p.ShowOrder).ToList())
                        {
                            if (item.IsDisplayEnable || SelectPermissionRole.Level == 1)
                            {
                                list.Add(item);
                            }
                        }
                    }
                }
                else
                    ShowMessageError(operationResult.Message);
                PermissionTreeList = new ObservableCollection<SysModuleModel>(list);
                foreach (var item in PermissionTreeList)
                {
                    if (item.LangID >= 0)
                        item.Description = SystemResources.Instance.LanguageArray[item.LangID];
                }
            }
        }
        /// <summary>
        /// 获取角色列表数据
        /// </summary>
        public void GetRoleList()
        {
            List<SysRoleModel> list = new List<SysRoleModel>();
            OperationResult<List<SysRoleModel>> operationResult = pBusiness.GetRoleList();
            if (operationResult.ResultEnum == OperationResultEnum.SUCCEED)
                list = operationResult.Results;
            else
                ShowMessageError(operationResult.Message);//3903角色设置
            RoleList = new ObservableCollection<SysRoleModel>(list);
        }
        #endregion
        #endregion
    }
}
