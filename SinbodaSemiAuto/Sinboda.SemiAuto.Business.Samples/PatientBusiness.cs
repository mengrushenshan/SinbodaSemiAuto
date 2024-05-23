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
    public class PatientBusiness : BusinessBase<PatientBusiness>
    {
        /// <summary>
        /// 创建患者信息
        /// </summary>
        /// <returns></returns>
        public Guid CreatePatient()
        {
            try
            {
                Sin_Patient patient = new Sin_Patient()
                {
                    Id = Guid.NewGuid(),
                    Sex = Sex.Unknow,
                    Age_unit = AgeUnit.Annum,
                    Regist_date = DateTime.Now.Date,
                    Regist_datetime = DateTime.Now
                };
                Sin_Patient_DataOperation.Instance.Insert(patient);

                return patient.Id;
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("CreatePatient error:" + ex.Message);
                return Guid.Empty;
            }

        }
    }
}
