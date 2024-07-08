using GalaSoft.MvvmLight;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Models
{
    public class PilotInfo : ObservableObject
    {
        private PilotStatus status;
        /// <summary>
        /// 灯的颜色
        /// </summary>
        public PilotStatus Status
        {
            get { return status; }
            set { Set(ref status, value); }
        }

        private string massage;
        /// <summary>
        /// 提示消息
        /// </summary>
        public string Massage
        {
            get { return massage; }
            set { Set(ref massage, value); }
        }

        /// <summary>
        /// 参数
        /// </summary>
        public object Tag { get; set; }
    }
}
