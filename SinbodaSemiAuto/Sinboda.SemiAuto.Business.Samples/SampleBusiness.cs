using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.Model.DataOperation.SemiAuto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Business.Samples
{
    public class SampleBusiness : BusinessBase<SampleBusiness>
    {
        //最小样本号
        private const int MIN_SAMPLECODE = 1;
        //最大样本号
        private const int MAX_SAMPLECODE = 99999999;

        /// <summary>
        /// 取出当天登记的最大样本
        /// </summary>
        /// <returns></returns>
        public int GetMaxSampleCode()
        {
            List<Sin_Sample> sampleList = Sin_Sample_DataOperation.Instance.QueryTodaySampleList();
            if (sampleList != null)
            {
                int maxCode = sampleList.Max(o => o.SampleCode);
                if (maxCode < MIN_SAMPLECODE || maxCode > MAX_SAMPLECODE)
                {
                    return MIN_SAMPLECODE;
                }
                return maxCode + 1;
            }

            return MIN_SAMPLECODE;
        }

        /// <summary>
        /// 架号位置是否有样本
        /// </summary>
        /// <param name="rack"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool RackPosHaveSample(int rack, int pos)
        {
            List<Sin_Sample> sampleList = Sin_Sample_DataOperation.Instance.QueryTodaySampleList();
            
            if (sampleList != null)
            {
                var sampleTemp = sampleList.Where(o => o.RackDish == rack && o.Position == pos).ToList();
                if (sampleTemp.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 样本号是否有样本
        /// </summary>
        /// <param name="sampleCode"></param>
        /// <returns></returns>
        public Sin_Sample SampleCodeHaveSample(int sampleCode)
        {
            List<Sin_Sample> sampleList = Sin_Sample_DataOperation.Instance.QueryTodaySampleList();

            if (sampleList != null)
            {
                var sampleTemp = sampleList.Where(o => o.SampleCode == sampleCode).ToList();
                if (sampleTemp.Count > 0)
                {
                    return sampleTemp.FirstOrDefault();
                }
            }
            return null;
        }

        /// <summary>
        /// 删除指定时间指定样本号范围的样本和结果信息
        /// </summary>
        /// <param name="beginCode">开始样本号</param>
        /// <param name="endCode">结束样本号</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public OperationResult<List<string>> DeleteSampleAndResult(int beginCode, int endCode, DateTime beginTime, DateTime endTime)
        {

            var msgs = new List<string>();
            var sampleList = Sin_Sample_DataOperation.Instance.Query(e => e.Sample_date == beginTime && e.SampleCode >= beginCode && e.SampleCode <= endCode);
            if (sampleList.Count <= 0)
            {
                //return Result(false, SystemResources.Instance.GetLanguage(7799, "没有可删除的样本"));
                msgs.Add(SystemResources.Instance.GetLanguage(7799, "没有可删除的样本"));
                return Result(OperationResultEnum.FAILED, msgs);
            }

            List<int> deleteSampleCode = new List<int>();

            sampleList.Select(o => o.SampleCode).ToList().ForEach(o => deleteSampleCode.Add(o));

            try
            {
                var tc = sampleList.Where(o => o.Test_state == TestState.Testing);
                if (tc != null && tc.Count() > 0)
                {
                    msgs.Add(SystemResources.Instance.GetLanguage(7798, "删除的样本中包含已在测试状态的样本，不能删除样本，请重新输入"));
                    removeSampleCode(deleteSampleCode, tc.Select(e => e.SampleCode).ToList());
                    sampleList.RemoveAll(o => o.Test_state == TestState.Testing);
                }

                

                // 删除
                using (var sin = Sin_Sample_DataOperation.Instance.CreateContextScope())
                {

                    if (sampleList.Count <= 0)
                    {
                        return Result(OperationResultEnum.FAILED, msgs);
                    }

                    foreach (var item in sampleList)
                    {
                        sin.Context.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction,
                            "DELETE FROM sin_test_result WHERE SAMPLE_ID = @p0", item.Id);

                        sin.Context.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction,
                            "DELETE FROM SIN_PATIENT WHERE ID = @p0", item.Patient_id);

                    }

                    //cs.Context.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction,
                    //    "DELETE FROM BCA_SAMPLE WHERE AUDIT_FLAG!=1 AND TEST_STATE!=2 AND SAMPLE_DATE = @p0 AND SAMPLE_CODE >= @p1 AND SAMPLE_CODE <= @p2",
                    //    beginTime, beginCode, endCode);
                    Sin_Sample_DataOperation.Instance.Delete(e => e.Sample_date == beginTime && deleteSampleCode.Contains(e.SampleCode));
                    sin.Complete();
                }
            }
            catch (Exception ex)
            {
                return Result<List<string>>(OperationResultEnum.FAILED, new List<string>(), ex.Message);
            }
            return Result(msgs);
        }

        /// <summary>
        /// 移除样本号
        /// </summary>
        /// <param name="deleteSampleCode"></param>
        /// <param name="predicateList"></param>
        private void removeSampleCode(List<int> deleteSampleCode, List<int> predicateList)
        {
            Predicate<int> predicate = o => predicateList.Contains(o);
            deleteSampleCode.RemoveAll(predicate);
        }

        /// <summary>
        /// 登记样本信息
        /// </summary>
        /// <param name="sampleCode"></param>
        /// <param name="rack"></param>
        /// <param name="pos"></param>
        /// <param name="barcode"></param>
        /// <param name="count"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public OperationResult CreateSample(int sampleCode, int rack, int pos, string barcode, int count, string itemName)
        {
            
            OperationResult or = null;
            try
            {
                Sin_Sample sample = null;
                for (int i = 0; i < count; i++)
                {
                    Guid patientId = PatientBusiness.Instance.CreatePatient();
                    if (patientId == Guid.Empty)
                    {
                        return or = new OperationResult() { ResultEnum = OperationResultEnum.FAILED, Message = SystemResources.Instance.GetLanguage(3034, "登记失败，请重新登记") };
                    }

                    sample = new Sin_Sample()
                    {
                        Id = Guid.NewGuid(),
                        Patient_id = patientId,
                        SampleCode = sampleCode,
                        RackDish = rack,
                        Position = pos,
                        Barcode = barcode,
                        Sample_date = DateTime.Now,
                        Test_state = TestState.Untested,
                        Delete_flag = false,
                        Send_time = DateTime.Now,
                        Sampling_time = DateTime.Now
                    };

                    if (!TestResultBusiness.Instance.CreateTestResult(sample.Id, itemName))
                    {
                        return or = new OperationResult() { ResultEnum = OperationResultEnum.FAILED, Message = SystemResources.Instance.GetLanguage(3034, "登记失败，请重新登记") };
                    }

                    Sin_Sample_DataOperation.Instance.Insert(sample);
                }
                or = new OperationResult() { ResultEnum = OperationResultEnum.SUCCEED };
                return or;
            }
            catch (Exception ex) 
            {
                return new OperationResult() { ResultEnum = OperationResultEnum.FAILED, Message = ex.Message };
            }
        }

        /// <summary>
        /// 根据条件查询样本
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<Sin_Sample> GetSampleListByPredicate(Expression<Func<Sin_Sample, bool>> predicate)
        {
            List<Sin_Sample> sampleList = Sin_Sample_DataOperation.Instance.Query(predicate);
            return sampleList;
        }

        /// <summary>
        /// 根据条件查询样本
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<Sin_Sample> GetSampleListToday()
        {
            List<Sin_Sample> sampleList = Sin_Sample_DataOperation.Instance.QueryTodaySampleList();
            return sampleList;
        }
    }
}
