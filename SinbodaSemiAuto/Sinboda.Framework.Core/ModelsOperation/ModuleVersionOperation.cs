using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.ModelsOperation
{
    public class ModuleVersionOperation : EFDataOperationBase<ModuleVersionOperation, ModuleVersionModel, DBContextBase>
    {
        /// <summary>
        /// 初始化所有模块版本信息
        /// </summary>
        public List<ModuleVersionModel> InitializeModuleVersionInfo()
        {
            List<ModuleVersionModel> list = new List<ModuleVersionModel>();
            using (DBContextBase db = new DBContextBase())
            {
                list = db.ModuleVersionModel.ToList();
            }
            return list;
        }

        /// <summary>
        /// 添加模块版本信息
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool AddModuleInfoDictionary(List<ModuleInfoModel> models)
        {
            int result = 0;
            using (DBContextBase db = new DBContextBase())
            {
                foreach (var model in models)
                {

                    model.Id = Guid.NewGuid();
                    db.Set<ModuleInfoModel>().Add(model);
                }
                result = db.SaveChanges();
            }
            InitializeModuleVersionInfo();
            return result > 0;
        }
        /// <summary>
        /// 清空模块版本信息
        /// </summary>
        /// <returns></returns>
        public bool ClearModuleInfoDictionary()
        {
            int result = 0;
            using (DBContextBase db = new DBContextBase())
            {
                db.ModuleInfoModel.RemoveRange(db.ModuleInfoModel.ToList());
                result = db.SaveChanges();
            }
            InitializeModuleVersionInfo();
            return result > 0;
        }
    }
}
