using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sinboda.Framework.Control.Controls
{
    public class GlyphButton : Button
    {
        /// <summary>
        /// 标识 Glyph 的依赖性属性
        /// </summary>
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(GlyphButton), new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 矢量图标
        /// </summary>
        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public GlyphButton()
        {

        }
    }
}
