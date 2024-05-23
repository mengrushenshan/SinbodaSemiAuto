using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sinboda.SemiAuto.Core.Models
{
    public class MotorData : ObservableObject
    {
        /// <summary>
        /// 机械臂id
        /// </summary>
        public int Id { get; set; } = -1;

        /// <summary>
        /// 方向 1:正向 2:反向
        /// </summary>
        public int Dir { get; set; }

        /// <summary>
        /// 是否使用高速 1:高速 2:慢速
        /// </summary>
        public int UseFastSpeed { get; set; }

        /// <summary>
        /// 步数
        /// </summary>
        public int Steps { get; set; }

        /// <summary>
        /// 目标位置
        /// </summary>
        public int TargetPos { get; set; }

        /// <summary>
        /// 工作原点
        /// </summary>
        public int OriginPoint { get; set; }
    }
}
