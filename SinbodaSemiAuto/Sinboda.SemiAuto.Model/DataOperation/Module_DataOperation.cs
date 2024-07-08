using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Services;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DataOperation
{
    /// <summary>
    /// 模块业务
    /// </summary>
    public class Module_DataOperation : TBaseSingleton<Module_DataOperation>
    {
        /// <summary>
        /// 按模块类型查询模块信息
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public List<ModuleInfoModel> QueryModuleInfo(ProductType productType) => DataDictionaryService.Instance.ModuleInfoList.Where(o => o.ModuleTypeCode == (int)productType).ToList();

        /// <summary>
        /// 查询模块信息
        /// </summary>
        /// <param name="module_id">模块编码</param>
        /// <returns></returns>
        public ModuleInfoModel QueryModuleInfo(int module_id) => DataDictionaryService.Instance.ModuleInfoList.Find(o => o.ModuleID == module_id);

        /// <summary>
        /// 查询所有模块信息
        /// </summary>
        /// <returns></returns>
        public List<ModuleInfoModel> QueryModuleInfo()
        {

            return DataDictionaryService.Instance.ModuleInfoList;
        }

        /// <summary>
        /// 查询模块类型信息
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public List<ModuleTypeModel> QueryModuleType(ProductType productType) => new Sin_DbContext().ModuleTypeModel.AsNoTracking().ToList().FindAll(o => o.ModuleTypeCode == (int)productType);

        /// <summary>
        /// 查询所有模块类型信息
        /// </summary>
        /// <returns></returns>
        public List<ModuleTypeModel> QueryModuleType() => new Sin_DbContext().ModuleTypeModel.AsNoTracking().ToList();

        /// <summary>
        /// 保存模块信息
        /// </summary>
        /// <param name="mims"></param>
        public bool InsertModuleInfos(List<ModuleInfoModel> mims) => new DataDictionaryService().AddModuleInfoDictionary(mims);

        /// <summary>
        /// 删除所有模块信息
        /// </summary>
        /// <returns></returns>
        public bool DeleteAllModuleInfos() => new DataDictionaryService().ClearModuleInfoDictionary();


    }
}
