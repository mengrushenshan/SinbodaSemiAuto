using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Control.Controls.Navigation
{
    /// <summary>
    /// 表示一个导航日志项信息
    /// </summary>
    public class JournalEntry
    {
        /// <summary>
        /// 获取关联的源
        /// </summary>
        public object Source { get; private set; }
        /// <summary>
        /// 获取关联的源内容
        /// </summary>
        public object Content { get; internal set; }
        /// <summary>
        /// 获取导航时传递的参数
        /// </summary>
        public object NavigationParameter { get; internal set; }
        /// <summary>
        /// 获取或设置日志项是否保持活动状态
        /// </summary>
        /// <remarks>
        /// 活动状态的项被添加到前进或后退列表中，通过调用 <seealso cref="INavigationContainer.GoBack()"/> 等方法可以导航到活动项。
        /// </remarks>
        public bool KeepAlive { get; set; }
        /// <summary>
        /// 创建一个<see cref="JournalEntry"/>实例
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="parameter">参数</param>
        public JournalEntry(object source, object parameter = null)
        {
            Source = source;
            NavigationParameter = parameter;
        }

        /// <summary>
        /// 创建一个<see cref="JournalEntry"/>实例
        /// </summary>
        public JournalEntry()
        { }

        /// <summary>
        /// 清空<seealso cref="Content"/>保存的导航内容
        /// </summary>
        internal void ClearContent()
        {
            if (Content != null && !KeepAlive)
            {
                SetContent(null);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        protected internal virtual void SetContent(object content)
        {
            Content = content;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        protected internal virtual void SetParameter(object parameter)
        {
            if (parameter != null)
                NavigationParameter = parameter;
        }
    }
}
