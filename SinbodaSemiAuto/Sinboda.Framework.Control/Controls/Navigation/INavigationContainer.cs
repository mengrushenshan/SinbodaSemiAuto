using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Control.Controls.Navigation
{
    /// <summary>
    /// 定义了导航容器应具备的功能
    /// </summary>
    public interface INavigationContainer
    {
        /// <summary>
        /// 是否可以执行后退操作
        /// </summary>
        bool CanGoBack { get; }
        /// <summary>
        /// 是否可以执行前进操作
        /// </summary>
        bool CanGoForward { get; }
        /// <summary>
        /// 导航到指定源
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="param">需要传递给源的参数</param>
        void Navigate(object source, object param);
        /// <summary>
        /// 执行后退操作
        /// </summary>
        void GoBack();
        /// <summary>
        /// 执行后退操作
        /// </summary>
        /// <param name="param">后退时传递的参数</param>
        void GoBack(object param);
        /// <summary>
        /// 执行前进操作
        /// </summary>
        void GoForward();
    }
}
