using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.Framework.Control.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class ViewHelper
    {
        /// <summary>
        /// 获取页面的 ViewModel
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static object GetViewModelFromView(object view)
        {
            return view.With((object x) => x as FrameworkElement).With((FrameworkElement x) => x.DataContext);
        }
    }
}
