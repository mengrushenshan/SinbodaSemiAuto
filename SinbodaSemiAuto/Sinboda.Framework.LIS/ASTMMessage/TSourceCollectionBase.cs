using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    /// <summary>
    /// 强类型集合在编译时和运行时提供自动类型验证，并且可避免那些对性能有副作用的进程，如装箱、取消装箱和转换
    /// </summary>
    public class TSourceCollectionBase : CollectionBase
    {
        public TSourceCollectionBase()
            : base()
        {
        }

        [Browsable(false)]
        public new int Capacity
        {
            get { return base.Capacity; }
            set { base.Capacity = value; }
        }

        [Browsable(false)]
        public new int Count
        {
            get { return base.Count; }
        }
    }
}
