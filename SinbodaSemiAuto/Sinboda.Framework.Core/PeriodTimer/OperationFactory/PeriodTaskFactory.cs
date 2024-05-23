using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.PeriodTimer.OperationFactory
{
    public abstract class PeriodTaskFactory
    {
        protected System.Timers.Timer periodTimer = null;

        public abstract event StandyStatusNotifyEvent GetStandyStatus;

        public abstract void DoPeriodTask();

        public abstract void StartTimer();

        public abstract void StopTimer();
    }
}
