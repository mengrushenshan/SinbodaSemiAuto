using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.ModelsOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Business.SystemManagement
{
    /// <summary>
    /// 
    /// </summary>
    public class ManagementBusiness : BusinessBase<PermissionManagerBusiness>
    {
        SystemManagementModelOperations operation = new SystemManagementModelOperations();

        #region 基础信息处理、产品线、机型处理

        /// <summary>
        /// 获得基础信息类型
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<SystemTypeModel>> GetSysDataDictionaryTypeList()
        {
            try
            {
                List<SystemTypeModel> result = operation.GetSysDataDictionaryTypeList();
                return Result<List<SystemTypeModel>>(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetSysDataDictionaryTypeList error", e);
                return Result<List<SystemTypeModel>>(e);
            }
        }
        /// <summary>
        /// 获得基础信息信息
        /// </summary>
        /// <param name="codeGroup"></param>
        /// <returns></returns>
        public OperationResult<List<SystemTypeValueModel>> GetSysDataDictionaryInfoList(Guid codeGroup)
        {
            try
            {
                List<SystemTypeValueModel> result = operation.GetSysDataDictionaryInfoList(codeGroup);
                return Result<List<SystemTypeValueModel>>(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetSysDataDictionaryInfoList error", e);
                return Result<List<SystemTypeValueModel>>(e);
            }
        }

        /// <summary>
        /// 获得产品线类型
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<ModuleTypeModel>> GetModuleTypeList()
        {
            try
            {
                List<ModuleTypeModel> result = operation.GetModuleTypeList();
                return Result<List<ModuleTypeModel>>(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetModuleTypeList error", e);
                return Result<List<ModuleTypeModel>>(e);
            }
        }
        /// <summary>
        /// 获得产品线机型
        /// </summary>
        /// <param name="moduleTypeID"></param>
        /// <returns></returns>
        public OperationResult<List<ModuleInfoModel>> GetModuleInfoList(ModuleTypeModel moduleTypeID)
        {
            try
            {
                List<ModuleInfoModel> result = operation.GetModuleInfoList(moduleTypeID);
                return Result<List<ModuleInfoModel>>(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetModuleInfoList error", e);
                return Result<List<ModuleInfoModel>>(e);
            }
        }
        /// <summary>
        /// 基础信息、产品线与机型增删改查
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="enums"></param>
        /// <returns></returns>
        public OperationResult OperateT<T>(T t, OperationEnum enums) where T : EntityModelBase
        {
            try
            {
                bool result = operation.OperateT<T>(t, enums);

                if (t.GetType() == typeof(SystemTypeModel) && enums == OperationEnum.Delete)
                {
                    SystemTypeModel model = t as SystemTypeModel;
                    operation.OperateSysDataDicTypeInfos(operation.GetSysDataDictionaryInfoList(model.Id));
                }
                if (result)
                    return Result(OperationResultEnum.SUCCEED);
                return Result(OperationResultEnum.FAILED);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("Management OperateT", e);
                return Result(OperationResultEnum.FAILED, e);
            }

        }

        public OperationResult SaveSystemType<T>(T t, OperationEnum enums) where T : EntityModelBase
        {
            try
            {
                bool result = operation.OperateT_Sqlite(t, enums);

                if (t.GetType() == typeof(SystemTypeModel) && enums == OperationEnum.Delete)
                {
                    operation.OperateSysDataDicTypeInfos_Sqlite(operation.GetSysDataDictionaryInfoList(t.Id));
                }
                if (result)
                    return Result(OperationResultEnum.SUCCEED);
                return Result(OperationResultEnum.FAILED);
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("Management SaveSystemType", ex);
                return Result(OperationResultEnum.FAILED, ex);
            }
        }
        #endregion
    }
}
