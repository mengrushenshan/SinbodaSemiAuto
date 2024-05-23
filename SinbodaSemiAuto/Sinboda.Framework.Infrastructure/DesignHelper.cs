using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 设计器帮助类
    /// </summary>
    public static class DesignHelper
    {
        /// <summary>
        /// 是否为设计模式
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement));
                bool? isInDesignMode = new bool?((bool)dependencyPropertyDescriptor.Metadata.DefaultValue);
                return isInDesignMode.Value;
            }
        }
    }
}
