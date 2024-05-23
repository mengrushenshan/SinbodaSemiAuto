using Sinboda.Framework.Control.Controls.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Interface
{
    /// <summary>
    /// 表示一个导航服务功能
    /// </summary>
    public interface INavigationServiceEx
    {
        /// <summary>
        /// 获取是否可以后退
        /// </summary>
        bool CanGoBack { get; }
        /// <summary>
        /// 获取是否可以前进
        /// </summary>
        bool CanGoForward { get; }
        /// <summary>
        /// 当前项
        /// </summary>
        object Current { get; }
        /// <summary>
        /// 清空导航历史
        /// </summary>
        void ClearNavigationHistory();
        /// <summary>
        /// 清空导航缓存
        /// </summary>
        void ClearNavigationCache();
        /// <summary>
        /// 导航到指定页
        /// </summary>
        /// <param name="target"></param>
        void Navigate(NavigationItem target);
        /// <summary>
        /// 后退
        /// </summary>
        void GoBack();
        /// <summary>
        /// 前进
        /// </summary>
        void GoForward();
        /// <summary>
        /// 返回主页
        /// </summary>
        void GoHome();
        /// <summary>
        /// 返回到默认页面
        /// </summary>
        void GoDefaultView();
    }
}
