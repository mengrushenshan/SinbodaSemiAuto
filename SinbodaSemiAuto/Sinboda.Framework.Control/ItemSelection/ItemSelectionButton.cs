using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.Framework.Control.ItemSelection
{
    /// <summary>
    /// 自定义选中按钮
    /// </summary>
    public class ItemSelectionButton : System.Windows.Controls.CheckBox
    {
        #region ShowName
        /// <summary>
        /// 注册“显示名称”依赖属性
        /// </summary>
        public static readonly DependencyProperty ShowNameProperty = DependencyProperty.Register("ShowName", typeof(string), typeof(ItemSelectionButton), new PropertyMetadata(""));
        /// <summary>
        /// “显示名称”依赖属性
        /// </summary>
        public string ShowName
        {
            get { return (string)GetValue(ShowNameProperty); }
            set { SetValue(ShowNameProperty, value); }
        }
        #endregion

        #region IsDilution
        private static readonly DependencyProperty IsDilutionProperty = DependencyProperty.Register("IsDilution", typeof(bool), typeof(ItemSelectionButton), new PropertyMetadata(false));

        /// <summary>
        /// 是否稀释
        /// </summary>
        public bool IsDilution
        {
            get { return (bool)GetValue(IsDilutionProperty); }
            set { SetValue(IsDilutionProperty, value); }
        }
        #endregion

        #region IsIncreament
        /// <summary>
        /// 注册“增量、减量、正常量”依赖属性
        /// </summary>
        public static readonly DependencyProperty IsIncreamentProperty = DependencyProperty.Register("IsIncreament", typeof(SampleVolumeFlag), typeof(ItemSelectionButton), new PropertyMetadata(SampleVolumeFlag.Normal));
        /// <summary>
        /// “增量、减量、正常量”依赖属性
        /// </summary>
        public SampleVolumeFlag IsIncreament
        {
            get { return (SampleVolumeFlag)GetValue(IsIncreamentProperty); }
            set { SetValue(IsIncreamentProperty, value); }
        }
        #endregion

        #region Mark
        /// <summary>
        /// 标记
        /// </summary>
        public static readonly DependencyProperty MarkProperty = DependencyProperty.Register("Mark", typeof(MaskShowFlag), typeof(ItemSelectionButton), new PropertyMetadata(MaskShowFlag.None));
        /// <summary>
        /// “当前是为选中或取消”依赖属性
        /// </summary>
        public MaskShowFlag Mark
        {
            get { return (MaskShowFlag)GetValue(MarkProperty); }
            set { SetValue(MarkProperty, value); }
        }
        #endregion
    }
}
