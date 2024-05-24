using Sinboda.Framework.Core.AbstractClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto
{
    /// <summary>
    /// 点位表
    /// </summary>
    [Serializable]
    public partial class Sin_Cell : EntityModelBase
    {
        /// <summary>
        /// 三个点位序列 1：原点 2、3：计算辅助点
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// X坐标
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Z坐标
        /// </summary>
        public int Z { get; set; }
    }
}
