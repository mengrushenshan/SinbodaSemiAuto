using System.ComponentModel;

namespace Sinboda.Framework.LIS.SinHL7
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TComponents : TSourceCollectionBase
    {
        public TComponents()
            : base()
        {

        }


        /// <summary>
        /// 元素指针
        /// </summary>
        public TComponent this[int index]
        {
            get
            {
                return (TComponent)this.List[index];
            }
            set
            {
                this.List[index] = (TComponent)value;
            }
        }
        /// <summary>
        /// 向 TParameters 中添加项。
        /// </summary>  
        public void Add(TComponent component)
        {
            this.List.Add(component);
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
