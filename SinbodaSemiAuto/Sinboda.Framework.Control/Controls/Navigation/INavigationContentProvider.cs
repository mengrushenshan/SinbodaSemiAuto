using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Control.Controls.Navigation
{
    /// <summary>
    /// 表示导航项内容加载提供者应具备的功能
    /// </summary>
    public interface INavigationContentProvider
    {
        /// <summary>
        /// 同步方式加载源
        /// </summary>
        /// <param name="source">源</param>
        /// <returns></returns>
        object Load(object source);
        /// <summary>
        /// 异步方式加载源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userCallback"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        IAsyncResult BeginLoad(object source, AsyncCallback userCallback, object asyncState);
        /// <summary>
        /// 取消加载
        /// </summary>
        /// <param name="asyncResult"></param>
        void CancelLoad(IAsyncResult asyncResult);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        LoadResult EndLoad(IAsyncResult asyncResult);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool CanLoad(object source);
    }

    /// <summary>
    /// 
    /// </summary>
    public class LoadResult
    {
        /// <summary>
        /// 
        /// </summary>
        public object LoadedContent { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadedContent"></param>
        public LoadResult(object loadedContent)
        {
            LoadedContent = loadedContent;
        }
    }
}
