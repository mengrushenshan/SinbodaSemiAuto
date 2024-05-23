using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto
{
    /// <summary>
    /// 电机表
    /// </summary>
    [Serializable]
    public partial class Sin_Motor : EntityModelBase
    {
        /// <summary>
        /// 机械臂id
        /// </summary>
        public MotorId MotorId { get; set; }

        /// <summary>
        /// 方向 1:正向 2:反向
        /// </summary>
        public Direction Dir { get; set; }

        /// <summary>
        /// 是否使用高速 1:高速 2:慢速
        /// </summary>
        public Rate UseFastSpeed { get; set; }

        /// <summary>
        /// 步数
        /// </summary>
        public int Steps { get; set; }

        /// <summary>
        /// 目标位置
        /// </summary>
        public int TargetPos { get; set; }

    }
}
