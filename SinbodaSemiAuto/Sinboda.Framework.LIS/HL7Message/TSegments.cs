using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections ;
using System.ComponentModel ;

namespace Sinboda.Framework.LIS.SinHL7
{  
    public class TSegments : TSourceCollectionBase
    {
         /// <summary>
        /// 初始化 DAF.ADODB.TParameters 类的新实例。
        /// </summary> 
        public TSegments()
            : base()
        {
        }

        /// <summary>
        /// 元素指针
        /// </summary>
        public   TSegment this[int index]
        {
            get
            {
                return (TSegment)this.List[index];
            }
            set
            {
                this.List[index] = (TSegment)value;
            }
        }

        /// <summary>
        /// 元素指针
        /// </summary>
        public TSegment this[string Name]
        {
            get
            {
                foreach (TSegment fI in this)
                {
                    if (fI.Name  == Name)
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
        public TSegment this[string Name, int Tag]
        {
            get
            {
                foreach (TSegment fI in this)
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
        public void Add(TSegment segment)
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
