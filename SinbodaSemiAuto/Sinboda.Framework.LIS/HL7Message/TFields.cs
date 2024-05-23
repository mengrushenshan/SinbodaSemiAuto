using System.ComponentModel;

namespace Sinboda.Framework.LIS.SinHL7
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TFields : TSourceCollectionBase
    {

        public TFields()
            : base()
        {

        }


        /// <summary>
        /// 元素指针
        /// </summary>
        public TField  this[int index]
        {
            get
            {
                return (TField)this.List[index];
            }
            set
            {
                this.List[index] = (TField)value;
            }
        }
        /// <summary>
        /// 向 TParameters 中添加项。
        /// </summary>  
        public void Add(TField field)
        {            
            this.List.Add(field);
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

