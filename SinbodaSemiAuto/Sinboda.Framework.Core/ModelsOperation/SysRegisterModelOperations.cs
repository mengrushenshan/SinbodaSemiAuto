using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.ModelsOperation
{
    public class SysRegisterModelOperations : EFDataOperationBase<SysRegisterModelOperations, SysRegisterModel, DBContextBase>
    {
        /// <summary>
        /// 查询注册信息
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns>注册模型</returns>
        public List<SysRegisterModel> QueryRegister()
        {
            return base.Query(o => true);
        }
        /// <summary>
        /// 插入注册信息
        /// </summary>
        /// <param name="entity"></param>
        public void InsertRegister(SysRegisterModel entity)
        {
            base.Insert(entity);
        }
        /// <summary>
        /// 更新注册信息
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateRegister(SysRegisterModel entity)
        {
            base.Update(entity);
        }
    }
}
