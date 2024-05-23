using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.CommonModels;
using Sinboda.Framework.Core.Interface;
using Sinboda.Framework.Core.ModelsOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Business.SystemSetup
{
    /// <summary>
    /// 
    /// </summary>
    public class AnalyzerInfoSettingBusiness : BusinessBase<AnalyzerInfoSettingBusiness>
    {
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        public OperationResult<CurrentModuleInfo> GetAnalyzerInfo()
        {
            try
            {
                IAnalyzerInfo info = new AnalyzerInfoOperations();
                CurrentModuleInfo result = info.GetAnalyzerInfo();
                return Result(OperationResultEnum.SUCCEED, result);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("GetAnalyzerInfo", e);
                return Result<CurrentModuleInfo>(e);
            }
        }

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public OperationResult SetAnalyzerInfo(CurrentModuleInfo infos)
        {
            try
            {
                IAnalyzerInfo info = new AnalyzerInfoOperations();
                bool result = info.SetAnalyzerInfo(infos);
                if (result)
                    return Result(OperationResultEnum.SUCCEED);
                return Result(OperationResultEnum.FAILED);
            }
            catch (System.Exception e)
            {
                LogHelper.logSoftWare.Error("SetAnalyzerInfo", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
    }
}
