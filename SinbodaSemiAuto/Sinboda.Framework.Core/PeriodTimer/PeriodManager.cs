using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.PeriodTimer.OperationFactory;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.PeriodTimer
{
    public class PeriodManager : BusinessBase<PeriodManager>
    {
        #region 事件
        /// <summary>
        /// 连接事件
        /// </summary>
        public event StandyStatusNotifyEvent GetStandyStatus
        {
            add
            {
                if (null != logoutTask)
                {
                    logoutTask.GetStandyStatus += value;
                }
            }
            remove
            {
                if (null != logoutTask)
                {
                    logoutTask.GetStandyStatus -= value;
                }
            }
        }
        #endregion

        #region 成员
        private PeriodTaskFactory logoutTask = null;
        #endregion

        #region 函数
        public PeriodManager()
        {
        }

        ~PeriodManager()
        {
        }

        public void StartLogoutPeriodTask()
        {
            if (logoutTask == null)
            {
                logoutTask = new PeriodTaskLogoutFactory();

                if (SystemResources.Instance.Logout4StandyEnable)
                {
                    StartLogoutPeroidTimer();
                }
            }
        }

        public void StopLogoutPeriodTask()
        {
            if (null != logoutTask)
            {
                StopLogoutPeriodTimer();
                logoutTask = null;
            }
        }

        public void StartLogoutPeroidTimer()
        {
            if (logoutTask != null)
            {
                logoutTask.StartTimer();
            }
        }

        public void StopLogoutPeriodTimer()
        {
            if (logoutTask != null)
            {
                logoutTask.StopTimer();
            }
        }
        #endregion
    }
}
