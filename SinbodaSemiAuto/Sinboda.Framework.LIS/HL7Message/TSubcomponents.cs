using System.ComponentModel;

namespace Sinboda.Framework.LIS.SinHL7
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TSubcomponents : TSourceCollectionBase
    {
        public TSubcomponents()
            : base()
        {

        }


        /// <summary>
        /// 元素指针
        /// </summary>
        public TSubcomponent this[int index]
        {
            get
            {
                return (TSubcomponent)this.List[index];
            }
            set
            {
                this.List[index] = (TSubcomponent)value; 
            }
        }
        /// <summary>
        /// 向 TParameters 中添加项。
        /// </summary>  
        public void Add(TSubcomponent subcomponent)
        {
            this.List.Add(subcomponent);
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
