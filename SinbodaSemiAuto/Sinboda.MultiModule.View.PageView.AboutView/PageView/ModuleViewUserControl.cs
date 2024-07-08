using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Sinboda.SemiAuto.View.PageView
{
    public class ModuleViewUserControl : UserControl
    {
        public static readonly DependencyProperty ModuleContextProperty =
            DependencyProperty.Register("ModuleContext", typeof(object), typeof(ModuleViewUserControl), new PropertyMetadata(PropertyChangedCallback));

        public object ModuleContext
        {
            get { return GetValue(ModuleContextProperty); }
            set { SetValue(ModuleContextProperty, value); }
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ModuleViewUserControl cmModule = d as ModuleViewUserControl;
            cmModule.SetModuleContext(e.NewValue);
        }

        protected virtual void SetModuleContext(object newValue)
        { }
    }
}
