using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Sinboda.Framework.Core.PeriodTimer.OperationFactory
{
    public delegate bool StandyStatusNotifyEvent();

    public class PeriodTaskLogoutFactory : PeriodTaskFactory
    {
        #region 属性
        private int nAlreadyStandySecond = 0;
        #endregion

        #region 事件
        public override event StandyStatusNotifyEvent GetStandyStatus;
        #endregion

        #region 函数
        public PeriodTaskLogoutFactory()
        {
            periodTimer = new Timer();
            periodTimer.Interval = 1000 * 10;
            periodTimer.Elapsed += new ElapsedEventHandler(LogoutElapsed);
        }

        public override void StartTimer()
        {
            nAlreadyStandySecond = 0;

            if (periodTimer != null)
            {
                periodTimer.Start();
            }
        }

        public override void StopTimer()
        {
            nAlreadyStandySecond = 0;

            if (periodTimer != null)
            {
                periodTimer.Stop();
            }
        }

        private void LogoutElapsed(object sender, ElapsedEventArgs e)
        {
            DoPeriodTask();
        }

        public override void DoPeriodTask()
        {
            bool isNeedAddTime = false;
            try
            {
                isNeedAddTime = GetStandyStatus();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (isNeedAddTime)
                {
                    nAlreadyStandySecond += 10;
                }
                else
                {
                    nAlreadyStandySecond = 0;
                }

                if (nAlreadyStandySecond >= SystemResources.Instance.Logout4StandyByTime * 60)
                {
                    // 提示用户要注销
                    BootStrapper.Current.ChangeUser(true);
                    nAlreadyStandySecond = 0;
                }
            }
        }
        #endregion
    }
}
