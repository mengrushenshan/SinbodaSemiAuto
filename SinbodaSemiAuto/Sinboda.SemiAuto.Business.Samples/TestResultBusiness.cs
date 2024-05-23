using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.Model.DataOperation.SemiAuto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Business.Samples
{
    public class TestResultBusiness : BusinessBase<TestResultBusiness>
    {
        public bool CreateTestResult(Guid sampleId, string itemName)
        {
            try
            {
                Sin_Test_Result testResult = new Sin_Test_Result()
                {
                    Id = Guid.NewGuid(),
                    Sample_id = sampleId,
                    Item_test_name = itemName,
                    Item_type = ItemType.SingleMolecule,
                    Sample_send_date = DateTime.Now,
                    Test_state = TestState.Untested,
                    Using_flag = true,
                    Recheck_flag = false,
                    ResultErrorMark = ResultErrorMark.None,
                    ResultRangeMark = ResultRangeMark.None,
                    Test_result_type = TestResultType.Normal,
                    Result_update_flag = false,
                    Digits = 2,
                    NonEditable = false
                };

                Sin_Test_Result_DataOperation.Instance.Insert(testResult);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("CreateTestResult error:" + ex.Message);
                return false;
            }

        }

        public List<Sin_Test_Result> GetTestResultListBySampleId(Guid sampleId)
        {
            List<Sin_Test_Result> testResult = Sin_Test_Result_DataOperation.Instance.Query(o => o.Sample_id == sampleId);
            if (testResult != null && testResult.Count != 0)
            {
                return testResult;
            }
            return null;
        }
    }
}
