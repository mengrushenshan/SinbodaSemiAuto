using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DataOperation.SemiAuto
{
    public class Sin_Sample_DataOperation : EFDataOperationBase<Sin_Sample_DataOperation, Sin_Sample, Sin_DbContext>
    {
        /// <summary>
        /// 获取当天可用样本列表
        /// </summary>
        /// <returns></returns>
        public List<Sin_Sample> QueryTodaySampleList()
        {
            DateTime dateToday = DateTime.Now.Date;
            DateTime dateTomorrow = DateTime.Now.AddDays(1).Date;

            List<Sin_Sample> sampleList = Sin_Sample_DataOperation.Instance.Query(o => o.Sample_date >= dateToday && o.Sample_date < dateTomorrow && o.Delete_flag == false);
            if (sampleList.Count != 0)
            {
                return sampleList;
            }
            return null;
        }
    }
}
