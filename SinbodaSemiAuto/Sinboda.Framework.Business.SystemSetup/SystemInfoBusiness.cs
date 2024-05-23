using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.ModelsOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Business.SystemSetup
{
    /// <summary>
    /// 信息管理业务实现类
    /// </summary>
    public class SystemInfoBusiness : BusinessBase<SystemInfoBusiness>
    {
        SystemManagementModelOperations operations = new SystemManagementModelOperations();

        /// <summary>
        /// 获得基础信息类型
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<DataDictionaryTypeModel>> GetDataDictionaryTypeList()
        {
            try
            {
                List<DataDictionaryTypeModel> result = operations.GetDataDictionaryTypeList();
                return Result(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetDataDictionaryTypeList", e);
                return Result<List<DataDictionaryTypeModel>>(e);
            }
        }
        /// <summary>
        /// 获得基础信息信息
        /// </summary>
        /// <returns></returns>
        public OperationResult<List<DataDictionaryInfoModel>> GetDataDictionaryInfoList(Guid codeGroup)
        {
            try
            {
                List<DataDictionaryInfoModel> result = operations.GetDataDictionaryInfoList(codeGroup);
                return Result(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetDataDictionaryInfoList", e);
                return Result<List<DataDictionaryInfoModel>>(e);
            }
        }
        /// <summary>
        /// 获取子节点信息
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public OperationResult<List<DataDictionaryInfoModel>> GetChildDataDictionaryInfoList(Guid parentId)
        {
            try
            {
                List<DataDictionaryInfoModel> result = operations.GetChildDataDictionaryInfoList(parentId);
                return Result(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetChildDataDictionaryInfoList", e);
                return Result<List<DataDictionaryInfoModel>>(e);
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
                bool result = operations.OperateT<T>(t, enums);
                if (t.GetType() == typeof(DataDictionaryTypeModel) && enums == OperationEnum.Delete)
                {
                    DataDictionaryTypeModel model = t as DataDictionaryTypeModel;
                    operations.OperateDataDicTypeInfos(operations.GetDataDictionaryInfoList(model.Id));
                }
                if (result)
                    return Result(OperationResultEnum.SUCCEED);
                return Result(OperationResultEnum.FAILED);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("SystemInfoBusiness OperateT", e);
                return Result(OperationResultEnum.FAILED, e);
            }

        }
        /// <summary>
        /// 交换顺序（向上、向下）
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public OperationResult ExchangeOrder(DataDictionaryInfoModel one, DataDictionaryInfoModel two)
        {
            try
            {
                operations.ExchangeOrder(one, two);
                return Result(true);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("SystemInfoBusiness ExchangeOrder", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="one"></param>
        /// <returns></returns>
        public OperationResult ExchangeOrderTop(DataDictionaryInfoModel one)
        {
            try
            {
                operations.ExchangeOrderTop(one);
                return Result(true);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("SystemInfoBusiness ExchangeOrder", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 置底
        /// </summary>
        /// <param name="one"></param>
        /// <returns></returns>
        public OperationResult ExchangeOrderBottom(DataDictionaryInfoModel one)
        {
            try
            {
                operations.ExchangeOrderBottom(one);
                return Result(true);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("SystemInfoBusiness ExchangeOrder", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
        /// <summary>
        /// 删除后修改其他项顺序
        /// </summary>
        /// <param name="showOrder"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public OperationResult ExchangeDeleteOrder(int showOrder, List<DataDictionaryInfoModel> others)
        {
            try
            {
                operations.ExchangeDeleteOrder(showOrder, others);
                return Result(true);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("SystemInfoBusiness ExchangeDeleteOrder", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
    }
}
