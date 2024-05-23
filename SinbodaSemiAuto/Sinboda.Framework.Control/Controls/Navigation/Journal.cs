using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows;
using Sinboda.Framework.Control.Utils;

namespace Sinboda.Framework.Control.Controls.Navigation
{
    #region CacheQueue 导航缓存

    /// <summary>
    /// 导航缓存
    /// </summary>
    internal class CacheQueue : DependencyObject
    {
        private class CacheEntry
        {
            public object Page { get; private set; }
            public NavigationCacheMode CacheMode { get; private set; }
            public Action OnDrop { get; set; }
            public CacheEntry(object page, NavigationCacheMode cacheMode)
            {
                Page = page;
                CacheMode = cacheMode;
            }
        }

        private ConcurrentDictionary<object, CacheEntry> pages = new ConcurrentDictionary<object, CacheEntry>();

        /// <summary>
        /// 确定是否包含指定 key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(object key)
        {
            return key != null && pages.ContainsKey(key);
        }

        /// <summary>
        /// 根据指定 key 返回 page
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetPage(object key)
        {
            return pages[key].Page;
        }

        /// <summary>
        /// 添加page
        /// </summary>
        /// <param name="key"></param>
        /// <param name="page"></param>
        /// <param name="navigator"></param>
        public void InsertPage(object key, object page, INavigationFrame navigator)
        {
            if (!pages.ContainsKey(key))
            {
                // 是否缓存导航页
                NavigationCacheMode navigationCacheMode = GetVirtualCacheMode(page, navigator);
                if (navigationCacheMode == NavigationCacheMode.Enabled)
                {
                    pages[key] = new CacheEntry(page, navigationCacheMode);
                }
            }
        }

        /// <summary>
        /// 删除 page
        /// </summary>
        /// <param name="key"></param>
        public bool RemovePage(object key)
        {
            if (pages.ContainsKey(key))
                return false;

            CacheEntry outCacheEntry;
            return pages.TryRemove(key, out outCacheEntry);
        }

        /// <summary>
        /// 获取导航页的缓存模式
        /// </summary>
        /// <param name="page"></param>
        /// <param name="navigator"></param>
        /// <returns></returns>
        private NavigationCacheMode GetVirtualCacheMode(object page, INavigationFrame navigator)
        {
            DependencyObject dependencyObject = page as DependencyObject;
            if (dependencyObject == null)
            {
                return navigator.NavigationCacheMode;
            }
            // 如果导航页设置可附加属性 NavigationPage.NavigationCacheModeProperty，则使用页面的缓存模式
            object obj = dependencyObject.ReadLocalValue(NavigationPage.NavigationCacheModeProperty);
            if (obj != DependencyProperty.UnsetValue)
            {
                return (NavigationCacheMode)obj;
            }
            return navigator.NavigationCacheMode;
        }

        public void Clear()
        {
            pages.Clear();
        }
    }
    #endregion

    #region NavigationOperation  导航操作数据

    /// <summary>
    /// 导航操作数据
    /// </summary>
    internal class NavigationOperation
    {
        public IAsyncResult AsyncResult { get; set; }

        public JournalEntry TargetEntry { get; set; }

        public NavigationMode NavigationMode { get; set; }

        public object NavigationState { get; set; }
    }
    #endregion

    /// <summary>
    /// <see cref="IJournal"/>的默认实现
    /// </summary>
    public class Journal : IJournal, INotifyPropertyChanged
    {
        private INavigationContentProvider _NavigationContentProvider;
        private NavigationOperation _CurrentNavigation;
        internal JournalEntryStack<JournalEntry> _BackStack;      // 后退列表
        internal JournalEntryStack<JournalEntry> _ForwardStack;   // 前进列表
        internal CacheQueue _Cache;
        /// <summary>
        /// 导航是否正在进行
        /// </summary>
        protected bool NavigationInProgress { get; private set; }
        /// <summary>
        /// 后退事件
        /// </summary>
        public event EventHandler CanGoBackChanged;
        /// <summary>
        /// 前进事件
        /// </summary>
        public event EventHandler CanGoForwardChanged;

        /// <summary>
        /// 初始化 <seealso cref="Journal"/> 类的新实例
        /// </summary>
        public Journal() : this(null) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="contentProvider"></param>
        public Journal(INavigationContentProvider contentProvider)
        {
            NavigationContentProvider = contentProvider;
            _BackStack = new JournalEntryStack<JournalEntry>();
            //_BackStack.CountChanged += _BackStack_CountChanged;
            _ForwardStack = new JournalEntryStack<JournalEntry>();
            //_ForwardStack.CountChanged += _ForwardStack_CountChanged;
            _Cache = new CacheQueue();
        }

        /// <summary>
        /// 前进列表变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ForwardStack_CountChanged(object sender, JournalEntryStackCountChangedEventArgs e)
        {
            if ((e.OldValue == 0 && _BackStack.Count == 1) || (e.OldValue == 1 && _BackStack.Count == 0))
            {
                OnCanGoForwardChanged();
            }
        }

        /// <summary>
        /// 可以前进
        /// </summary>
        protected virtual void OnCanGoForwardChanged()
        {
            if (CanGoForwardChanged != null)
            {
                CanGoForwardChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 后退列表变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _BackStack_CountChanged(object sender, JournalEntryStackCountChangedEventArgs e)
        {
            if ((e.OldValue == 1 && _BackStack.Count == 2) || (e.OldValue == 2 && _BackStack.Count == 1))
            {
                OnCanGoBackChanged();
            }
        }

        /// <summary>
        /// 可以后退
        /// </summary>
        protected virtual void OnCanGoBackChanged()
        {
            if (CanGoBackChanged != null)
            {
                CanGoBackChanged(this, EventArgs.Empty);
            }
        }

        #region 虚函数

        /// <summary>
        /// 决定使用的<see cref="INavigationContentProvider"/>，可以在派生类中重新此方法
        /// </summary>
        /// <returns></returns>
        protected virtual INavigationContentProvider ResolveContentProvider()
        {
            return new NavigationContentProvider();
        }

        /// <summary>
        /// 清空导航历史
        /// </summary>
        protected virtual void ClearNavigationHistoryOverride()
        {
            _ForwardStack.Clear();
            _BackStack.Clear();
            //if (Current != null)
            //{
            //    _BackStack.Push(Current);
            //}
            UpdateProperties();
        }

        /// <summary>
        /// 通知 CanGoBack，CanGoForward 属性变化
        /// </summary>
        protected virtual void UpdateProperties()
        {
            RaisePropertyChanged("CanGoBack");
            RaisePropertyChanged("CanGoForward");
        }

        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        protected virtual bool NavigateCore(JournalEntry entry, NavigationMode mode)
        {
            // 进入导航
            NavigationInProgress = true;
            bool result;
            try
            {
                result = NavigateCoreOverride(entry, mode);
            }
            finally
            {
                // 结束导航
                NavigationInProgress = false;
            }
            return result;
        }

        /// <summary>
        /// 导航逻辑
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        protected virtual bool NavigateCoreOverride(JournalEntry entry, NavigationMode mode)
        {
            _CurrentNavigation = new NavigationOperation
            {
                TargetEntry = entry,
                NavigationMode = mode,
                NavigationState = entry.NavigationParameter
            };

            try
            {
                bool result = true;
                // 如果ViewModel实现了IViewModelNavigation接口则调用OnNavigatedFrom方法
                if (Current != null && Current.Content != null)
                {
                    object viewContent = ViewHelper.GetViewModelFromView(Current.Content);
                    IViewModelNavigation viewModelNavigation = viewContent as IViewModelNavigation;
                    if (viewModelNavigation != null)
                        result = viewModelNavigation.OnNavigatedFrom(entry.Source, mode, _CurrentNavigation.NavigationState);
                    if (entry.Source != null && entry.Source.ToString().Contains("NavigationItem"))
                        OperationLogBusiness.Instance.WriteOperationLogToDb(((NavigationItem)entry.Source).Name, 1);
                    //LogHelper.logSoftWare.Debug("Operation   " + ((NavigationItem)entry.Source).Name);
                }

                // 先调用 INavigationFrame.Navigating 返回 true 则继续
                if (result && Navigator.Navigating(entry.Source, mode, _CurrentNavigation.NavigationState))
                {
                    // 先查找缓存
                    if (_Cache.Contains(entry.Source))
                    {
                        ApplyContent(_Cache.GetPage(entry.Source));
                    }
                    else if (entry.Source != null)
                    {
                        // 如果未保存在缓存中，则调用INavigationContentProvider的BeginLoad方法加载page
                        // 加载时异步的BeginLoad完成后必须调用EndLoad（异步编程模型APM）
                        _CurrentNavigation.AsyncResult = NavigationContentProvider.BeginLoad(entry.Source, OnContentLoaded, _CurrentNavigation);
                    }
                    else
                    {
                        ApplyContent(null);
                    }
                    return true;
                }

                // 如果INavigationFrame.Navigating返回false，则调用如果INavigationFrame.NavigationStopped
                Navigator.NavigationStopped(entry.Source, mode, entry.NavigationParameter);
            }
            catch (NavigationException e)
            {
                LogHelper.logSoftWare.Error("NavigationCore NacigationException error", e);
                // 发生异常时调用INavigationFrame.NavigationFailed
                Navigator.NavigationFailed(entry.Source, e);
            }
            catch (Exception innerException)
            {
                LogHelper.logSoftWare.Error("NavigationCore Exception error", innerException);
                Navigator.NavigationFailed(entry.Source, new NavigationException(entry, innerException));
            }
            return false;
        }
        #endregion

        /// <summary>
        /// 将导航内容应用到 <see cref="Current"/>
        /// </summary>
        /// <param name="content"></param>
        private void ApplyContent(object content)
        {
            switch (_CurrentNavigation.NavigationMode)
            {
                case NavigationMode.New:
                    // 新页面时，将新页面添加到后退列表，清空前进页面
                    _BackStack.Push(_CurrentNavigation.TargetEntry);
                    _ForwardStack.Clear();
                    break;
                case NavigationMode.Back:
                    // 把当前元素添加到前进列表
                    if (Current.Content != null)
                        _ForwardStack.Push(Current);
                    break;
                case NavigationMode.Forward:
                    // 取出前进页面最后一页添加到后退列表
                    if (Current.Content != null)
                        _BackStack.Push(_ForwardStack.Pop());
                    break;
                case NavigationMode.Home:
                    // 清空前进和后退列表
                    _BackStack.Clear();
                    _ForwardStack.Clear();
                    _BackStack.Push(_CurrentNavigation.TargetEntry);
                    break;
            }

            // 是否清空当前导航页
            if (Current != null && !Current.KeepAlive)
                Current.ClearContent();

            Current = _BackStack.Peek();
            Current.SetContent(content);
            UpdateProperties();
            // 调用 NavigationComplete，表示导航完成
            Navigator.NavigationComplete(Current.Source, Current.Content, _CurrentNavigation.NavigationState);

            IViewModelNavigation supportParameter = ViewHelper.GetViewModelFromView(content) as IViewModelNavigation;
            if (supportParameter != null)
            {
                supportParameter.CurrentNavigationItem = _CurrentNavigation.TargetEntry.Source as NavigationItem;
                supportParameter.Parameter = _CurrentNavigation.NavigationState;
            }
        }

        /// <summary>
        /// 异步加载Page完成时
        /// </summary>
        /// <param name="result"></param>
        private void OnContentLoaded(IAsyncResult result)
        {
            try
            {
                LoadResult loadResult = NavigationContentProvider.EndLoad(result);
                // 如果返回的结果为空
                if (loadResult == null)
                    throw new NavigationException(_CurrentNavigation.TargetEntry.Source, null);

                // 添加到缓存中
                _Cache.InsertPage(_CurrentNavigation.TargetEntry.Source, loadResult.LoadedContent, Navigator);
                ApplyContent(loadResult.LoadedContent);
                //ClearNavigationHistoryOverride();
            }
            catch (NavigationException e)
            {
                LogHelper.logSoftWare.Error("OnContentLoaded NavigationException error", e);
                Navigator.NavigationFailed(Current?.Source, e);
            }
            catch (Exception innerException)
            {
                LogHelper.logSoftWare.Error("OnContentLoaded Exception error", innerException);
                object source = (Current != null) ? Current.Source : _CurrentNavigation.TargetEntry.Source;
                Navigator.NavigationFailed(source, new NavigationException(source, innerException));
            }
            finally
            {
                //this.clearHistoryLocker.Unlock();
            }
        }

        /// <summary>
        /// 创建<see cref="JournalEntry"/>实例
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected virtual JournalEntry CreateJournalEntry(object source, object parameter)
        {
            NavigationItem item = source as NavigationItem;
            if (item != null)
            {
                parameter = item.NavigationParameter;
            }

            // NavigationCacheMode.Disabled 则不缓存
            return new JournalEntry(source, parameter)
            {
                KeepAlive = Navigator.NavigationCacheMode != NavigationCacheMode.Disabled
            };
        }

        #region IJournal


        /// <summary>
        /// 当前的<see cref="JournalEntry"/>
        /// </summary>
        public JournalEntry Current { get; protected set; }
        /// <summary>
        /// 后退列表
        /// </summary>
        public IEnumerable<JournalEntry> BackStack { get { return _BackStack; } }
        /// <summary>
        /// 前进列表
        /// </summary>
        public IEnumerable<JournalEntry> ForwardStack { get { { return _ForwardStack; } } }
        /// <summary>
        /// 是否可以后退
        /// </summary>
        public bool CanGoBack { get { return _BackStack.Count > 1; } }
        /// <summary>
        /// 是否可以前进
        /// </summary>
        public bool CanGoForward { get { return _ForwardStack.Count > 0; } }

        /// <summary>
        /// 
        /// </summary>
        public INavigationContentProvider NavigationContentProvider
        {
            get
            {
                if (_NavigationContentProvider == null)
                    _NavigationContentProvider = ResolveContentProvider();

                return _NavigationContentProvider;
            }
            set { _NavigationContentProvider = value; }
        }

        /// <summary>
        /// 获取关联的<see cref="INavigationFrame"/>
        /// </summary>
        public INavigationFrame Navigator { get; set; }

        /// <summary>
        /// 清空导航缓存
        /// </summary>
        public void ClearNavigationCache()
        {
            _Cache.Clear();
        }
        /// <summary>
        /// 清空导航历史
        /// </summary>
        public void ClearNavigationHistory()
        {
            // 异步加锁
            //this.clearHistoryLocker.Reset();
            //if (this.Navigator.Status == NavigationStatus.Executing)
            //{
            //    this.clearHistoryLocker.Lock();
            //    return;
            //}
            //this.ClearNavigationHistoryOverride();

            // 不考虑异步
            if (Navigator.Status == NavigationStatus.Executing)
                return;

            ClearNavigationHistoryOverride();
        }

        /// <summary>
        /// 后退
        /// </summary>
        public void GoBack()
        {
            GoBack(null);
        }

        /// <summary>
        /// 后退，并传递参数
        /// </summary>
        /// <param name="param"></param>
        public void GoBack(object param)
        {
            if (_BackStack.Count <= 1)
                return;

            JournalEntry item = _BackStack.Pop();   // 取出当前项
            JournalEntry entry = _BackStack.Peek(); // 取出要导航的项
            entry.SetParameter(param);
            if (!NavigateCore(entry, NavigationMode.Back))
            {
                // 如果导航失败将原来的导航项添回去
                _BackStack.Push(item);
            }
        }

        /// <summary>
        /// 前进
        /// </summary>
        public void GoForward()
        {
            GoForward(null);
        }

        internal void GoForward(object param)
        {
            if (_ForwardStack.Count <= 0)
                return;

            JournalEntry entry = _ForwardStack.Peek();
            entry.SetParameter(param);
            NavigateCore(entry, NavigationMode.Forward);
        }

        /// <summary>
        /// 导航至并传递参数
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameter"></param>
        public void Navigate(object source, object parameter = null)
        {
            JournalEntry entry = CreateJournalEntry(source, parameter);
            NavigateCore(entry, NavigationMode.New);
        }
        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// 属性变更事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 事件处理方法
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
