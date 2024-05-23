using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Sinboda.Framework.Control.Controls.Navigation
{
    /// <summary>
    /// 定义了导航日志应具备的功能
    /// </summary>
    public interface IJournal : INotifyPropertyChanged, INavigationContainer
    {
        /// <summary>
        /// 获取当前的 <see cref="JournalEntry"/> 类型实例
        /// </summary>
        JournalEntry Current { get; }

        /// <summary>
        /// 获取后退列表
        /// </summary>
        IEnumerable<JournalEntry> BackStack { get; }

        /// <summary>
        /// 获取前进列表
        /// </summary>
        IEnumerable<JournalEntry> ForwardStack { get; }

        /// <summary>
        /// 获取或设置关联的<see cref="INavigationContentProvider"/>
        /// </summary>
        INavigationContentProvider NavigationContentProvider { get; set; }

        /// <summary>
        /// 关联的导航框架
        /// </summary>
        INavigationFrame Navigator { get; set; }

        /// <summary>
        /// 清空导航历史
        /// </summary>
        void ClearNavigationHistory();

        /// <summary>
        /// 清空导航缓存
        /// </summary>
        void ClearNavigationCache();

    }
}
