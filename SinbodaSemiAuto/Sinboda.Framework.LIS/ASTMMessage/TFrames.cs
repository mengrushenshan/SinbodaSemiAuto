using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    public class TFrames : TSourceCollectionBase
    {
        /// <summary>
        /// 初始化 DAF.ADODB.TParameters 类的新实例。
        /// </summary> 
        public TFrames()
            : base()
        {
        }

        /// <summary>
        /// 元素指针
        /// </summary>
        public TFrame this[int index]
        {
            get
            {
                return (TFrame)this.List[index];
            }
            set
            {
                this.List[index] = (TFrame)value;
            }
        }

        /// <summary>
        /// 元素指针
        /// </summary>
        public TFrame this[string Name]
        {
            get
            {
                foreach (TFrame fI in this)
                {
                    if (fI.Name == Name)
                    {
                        return fI;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 元素指针
        /// </summary>
        public TFrame this[string Name, int Tag]
        {
            get
            {
                foreach (TFrame fI in this)
                {
                    if (fI.Name == Name && fI.Tag == Tag)
                    {
                        return fI;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 向 TParameters 中添加项。
        /// </summary>  
        public void Add(TFrame segment)
        {
            this.List.Add(segment);
        }

        /// <summary>
        /// 移除指定索引处的 TParameters 项。
        /// </summary> 
        public void Remove(int index)
        {
            if (index < this.Count - 1 && index > 0)
            {
                this.List.RemoveAt(index);
            }
        }
    }
}
