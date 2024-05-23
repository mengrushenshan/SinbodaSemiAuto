using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.AbstractClass
{
    /// <summary>
    /// 业务实体基类
    /// </summary>
    [Serializable]
    public class EntityModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("ID")]
        public Guid Id { get; set; }

        /// <summary>
        /// 数据创建时间
        /// </summary>
        [Column("CREATE_USER")]
        [DataType(DataType.Text)]
        [MaxLength(200)]
        public string Create_user { get; set; }

        /// <summary>
        /// 数据创建人
        /// </summary>
        [Column("CREATE_TIME")]
        public DateTime Create_time { get; set; }

        #region INotifyPropertyChanged
        /// <summary>
        /// 属性触发变更事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性触发变更
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            return Set(propertyName, ref field, newValue);
        }
        /// <summary>
        /// 属性触发变更
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        protected bool Set<T>(string propertyName, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            field = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// 实现接口
        /// </summary>
        /// <param name="propertyName"></param>
        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
