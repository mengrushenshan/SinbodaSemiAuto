using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sinboda.Framework.Control.Controls.Navigation
{
    /// <summary>
    /// 导航页
    /// </summary>
    public class NavigationPage : ContentControl
    {
        /// <summary>
        /// 标识 <see cref="NavigationPage"/> 的附加属性
        /// </summary>
        public static readonly DependencyProperty NavigationCacheModeProperty = DependencyProperty.RegisterAttached("NavigationCacheMode", typeof(NavigationCacheMode), typeof(NavigationPage), new PropertyMetadata(NavigationCacheMode.Disabled));  // 缓存模式

        /// <summary>
        /// 返回<see cref="DependencyObject"/>实例上的缓存模式
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static NavigationCacheMode GetNavigationCacheMode(DependencyObject obj)
        {
            return (NavigationCacheMode)obj.GetValue(NavigationCacheModeProperty);
        }

        /// <summary>
        /// 设置<see cref="DependencyObject"/>实例的缓存模式
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetNavigationCacheMode(DependencyObject obj, NavigationCacheMode value)
        {
            obj.SetValue(NavigationCacheModeProperty, value);
        }
    }
}
