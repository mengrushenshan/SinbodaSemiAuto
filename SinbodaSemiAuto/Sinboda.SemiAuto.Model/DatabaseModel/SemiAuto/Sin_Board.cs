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
    /// 实验板
    /// </summary>
    [Serializable]
    public class Sin_Board : EntityModelBase
    {
        /// <summary>
        /// 实验板号
        /// </summary>
        public int BoardId { get; set; }

        /// <summary>
        /// 架号
        /// </summary>
        public int Rack { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// 点位类型
        /// </summary>
        public TestType TestType { get; set; }

        /// <summary>
        /// 测试项目
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 位置是否启用
        /// </summary>
        public bool IsEnable { get; set; }

    }
}
