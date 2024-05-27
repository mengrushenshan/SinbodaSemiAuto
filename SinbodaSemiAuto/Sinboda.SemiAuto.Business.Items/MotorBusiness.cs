using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.Model.DataOperation.SemiAuto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Business.Items
{
    public class MotorBusiness : BusinessBase<MotorBusiness>
    {
        /// <summary>
        /// 获取全部电机列表
        /// </summary>
        /// <returns>电机列表</returns>
        public List<Sin_Motor> GetMotorList()
        {
            List<Sin_Motor> motorList = Sin_Motor_DataOperation.Instance.Query(o => true).OrderBy(p => p.MotorId).ToList();
            if (motorList == null || motorList.Count == 0)
            {
                return null;
            }

            return motorList;
        }

        /// <summary>
        /// 保存电机参数
        /// </summary>
        /// <returns></returns>
        public bool SaveMotorItem(Sin_Motor motorItem)
        {
            bool result = false;
            try
            {
                if (motorItem != null)
                {
                    var tempMotor = Sin_Motor_DataOperation.Instance.Find(motorItem.Id);
                    if (tempMotor != null)
                    {
                        Sin_Motor_DataOperation.Instance.Update(motorItem);
                        result = true;
                    }
                    else
                    {
                        LogHelper.logSoftWare.Error($"SaveMotorItem error: Sin_Motor don't have motorItem Id={motorItem.Id} ");
                    }
                }
                else
                {
                    LogHelper.logSoftWare.Error($"SaveMotorItem error: Sin_Motor is null");
                }

                return result;
            }
            catch (Exception ex) 
            {
                LogHelper.logSoftWare.Error($"SaveMotorItem error: {ex.Message}");
                return result;
            }
        }
    }
}
