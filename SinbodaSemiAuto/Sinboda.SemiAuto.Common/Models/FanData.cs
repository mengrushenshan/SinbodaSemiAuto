using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Models
{
    public class FanData : ObservableObject
    {
        /// <summary>
        /// 风扇编号
        /// </summary>
        public int Id { get; set; } = -1;

        /// <summary>
        /// 0=关 1=开
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 频率
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// 占空比
        /// </summary>
        public int DutyRatio { get; set; }
    }
}
