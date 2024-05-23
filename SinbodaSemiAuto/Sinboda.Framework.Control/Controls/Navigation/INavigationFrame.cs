using Sinboda.Framework.Common.ResourceExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Control.Controls.Navigation
{
    /// <summary>
    /// 导航框架控件接口
    /// </summary>
    public interface INavigationFrame : INavigationContainer
    {
        /// <summary>
        /// 获取关联的<see cref="IJournal"/>
        /// </summary>
        IJournal Journal { get; }
        /// <summary>
        /// 导航状态
        /// </summary>
        NavigationStatus Status { get; }
        /// <summary>
        /// 导航缓存模式
        /// </summary>
        NavigationCacheMode NavigationCacheMode { get; }
        /// <summary>
        /// 当开始时导航调用
        /// </summary>
        /// <param name="source"></param>
        /// <param name="mode"></param>
        /// <param name="navigationState"></param>
        /// <returns></returns>
        bool Navigating(object source, NavigationMode mode, object navigationState);
        /// <summary>
        /// 当导航完成时调用
        /// </summary>
        /// <param name="source"></param>
        /// <param name="content"></param>
        /// <param name="navigationState"></param>
        void NavigationComplete(object source, object content, object navigationState);

        /// <summary>
        /// 导航过程中发生异常
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void NavigationFailed(object source, NavigationException e);

        /// <summary>
        /// Navigating返回fasle时调用
        /// </summary>
        /// <param name="source"></param>
        /// <param name="mode"></param>
        /// <param name="navigationState"></param>
        void NavigationStopped(object source, NavigationMode mode, object navigationState);
    }

    /// <summary>
    /// 导航的缓存模式
    /// </summary>
    public enum NavigationCacheMode
    {
        /// <summary>
        /// 缓存
        /// </summary>
        Enabled,
        /// <summary>
        /// 不缓存
        /// </summary>
        Disabled
    }

    /// <summary>
    /// 导航方式
    /// </summary>
    public enum NavigationMode
    {
        /// <summary>
        /// 新页面
        /// </summary>
        New,
        /// <summary>
        /// 后退
        /// </summary>
        Back,
        /// <summary>
        /// 前进
        /// </summary>
        Forward,
        /// <summary>
        /// 主页
        /// </summary>
        Home
    }

    /// <summary>
    /// 导航状态
    /// </summary>
    public enum NavigationStatus
    {
        /// <summary>
        /// 未开始
        /// </summary>
        NotStarted,
        /// <summary>
        /// 执行
        /// </summary>
        Executing,
        /// <summary>
        /// 完成
        /// </summary>
        Completed,
        /// <summary>
        /// 中止
        /// </summary>
        Aborted,
        /// <summary>
        /// 失败
        /// </summary>
        Failed
    }

    /// <summary>
    /// 表示导航异常
    /// </summary>
    public class NavigationException : Exception
    {
        /// <summary>
        /// 初始化 <seealso cref="NavigationException"/> 类的新实例。
        /// </summary>
        /// <param name="message">异常信息</param>
        public NavigationException(string message) : base(message)
        { }

        /// <summary>
        /// 初始化 <seealso cref="NavigationException"/> 类的新实例。
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">异常</param>
        public NavigationException(string message, Exception innerException) : base(message, innerException)
        { }

        /// <summary>
        /// 初始化 <seealso cref="NavigationException"/> 类的新实例。
        /// </summary>
        /// <param name="source">异常信息</param>
        /// <param name="innerException">异常</param>
        public NavigationException(object source, Exception innerException = null)
            : base(StringResourceExtension.GetLanguage(40, "导航到 {0} 时发生错误", source), innerException) //TODO 翻译
        { }
    }
}
