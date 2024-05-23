using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Sinboda.Framework.Control.Chromes
{
    /// <summary>
    /// 
    /// </summary>
    public class ButtonChrome : ContentControl
    {
        #region Dependency Propertys

        /// <summary>
        ///  标识 <seealso cref="CornerRadius"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ButtonChrome), new PropertyMetadata(new CornerRadius(4)));
        /// <summary>
        /// 标识 <seealso cref="RenderPressed"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty RenderPressedProperty = DependencyProperty.Register("RenderPressed", typeof(bool), typeof(ButtonChrome), new UIPropertyMetadata(false));
        /// <summary>
        /// 标识 <seealso cref="RenderMouseOver"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty RenderMouseOverProperty = DependencyProperty.Register("RenderMouseOver", typeof(bool), typeof(ButtonChrome), new UIPropertyMetadata(false));
        /// <summary>
        /// 标识 <seealso cref="MouseOverBackground"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(ButtonChrome), new UIPropertyMetadata(null));
        /// <summary>
        /// 标识 <seealso cref="MousePressedBackground"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty MousePressedBackgroundProperty = DependencyProperty.Register("MousePressedBackground", typeof(Brush), typeof(ButtonChrome), new UIPropertyMetadata(null));
        /// <summary>
        /// 标识 <seealso cref="RenderFocused"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty RenderFocusedProperty = DependencyProperty.Register("RenderFocused", typeof(bool), typeof(ButtonChrome), new UIPropertyMetadata(false));
        /// <summary>
        /// 获取或设置是否获得焦点
        /// </summary>
        public bool RenderFocused
        {
            get { return (bool)GetValue(RenderFocusedProperty); }
            set { SetValue(RenderFocusedProperty, value); }
        }

        /// <summary>
        /// 获取或设置圆角
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// 获取或设置是否按下
        /// </summary>
        public bool RenderPressed
        {
            get { return (bool)GetValue(RenderPressedProperty); }
            set { SetValue(RenderPressedProperty, value); }
        }

        /// <summary>
        /// 获取或设置鼠标是否在元素上
        /// </summary>
        public bool RenderMouseOver
        {
            get { return (bool)GetValue(RenderMouseOverProperty); }
            set { SetValue(RenderMouseOverProperty, value); }
        }

        /// <summary>
        /// 获取或设置鼠标在元素上时的背景画刷
        /// </summary>
        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        /// <summary>
        /// 获取或设置鼠标按下时的背景画刷
        /// </summary>
        public Brush MousePressedBackground
        {
            get { return (Brush)GetValue(MousePressedBackgroundProperty); }
            set { SetValue(MousePressedBackgroundProperty, value); }
        }
        #endregion

        static ButtonChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonChrome), new FrameworkPropertyMetadata(typeof(ButtonChrome)));
        }

        public ButtonChrome()
        {
            Focusable = false;
        }
    }
}
