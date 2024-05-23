using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;

namespace Sinboda.Framework.Control.Controls.Navigation
{
    /// <summary>
    /// 导航控件
    /// </summary>
    [TemplatePart(Name = partBar)]
    public class NavigationFrame : ContentControl, INavigationFrame
    {
        const string partBar = "PART_Bar";
        const string partBackBtn = "PART_BackBtn";
        const string partForwardBtn = "PART_ForwardBtn";

        private NavigationStatus _NavigationStatus;  // 保存导航状态
        private BreadcrumbBar _BreadcrumbBar;        // 
        private Button _BackBtn;                     // 后退按钮
        private Button _ForwardBtn;                  // 前进按钮

        /// <summary>
        /// 导航过程中触发，可取消导航
        /// </summary>
        public event EventHandler<NavigatingEventArgs> Navigating;
        /// <summary>
        /// 导航完成后触发
        /// </summary>
        public event EventHandler<NavigatedEventArgs> Navigated;
        /// <summary>
        /// 导航发生异常时触发
        /// </summary>
        public event EventHandler<NavigationFailedEventArgs> NavigationFailed;

        #region DependencyPropertes

        /// <summary>
        /// 标识 <see cref="Journal"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty JournalProperty = DependencyProperty.Register("Journal", typeof(IJournal), typeof(NavigationFrame), new PropertyMetadata(null, OnJournalChanged));
        /// <summary>
        /// 标识 <see cref="CanGoForward"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty CanGoForwardProperty = DependencyProperty.Register("CanGoForward", typeof(bool), typeof(NavigationFrame), new UIPropertyMetadata(false));
        /// <summary>
        /// 标识 <see cref="CanGoBack"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register("CanGoBack", typeof(bool), typeof(NavigationFrame), new UIPropertyMetadata(false));
        /// <summary>
        /// 标识 <see cref="Source"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(NavigationFrame), new PropertyMetadata(null, OnSourceChanged));
        /// <summary>
        /// 标识 <see cref="ContentProvider"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty ContentProviderProperty = DependencyProperty.Register("ContentProvider", typeof(INavigationContentProvider), typeof(NavigationFrame), new PropertyMetadata(null, OnContentProviderChanged));


        public static readonly DependencyProperty NavBarVisbilityProperty = DependencyProperty.Register("NavBarVisbility", typeof(Visibility), typeof(NavigationFrame), new PropertyMetadata(Visibility.Visible));

        public Visibility NavBarVisbility
        {
            get { return (Visibility)GetValue(NavBarVisbilityProperty); }
            set { SetValue(NavBarVisbilityProperty, value); }
        }

        /// <summary>
        /// 获取或设置 <see cref="NavigationFrame"/> 关联的 <see cref="IJournal"/>
        /// </summary>
        public IJournal Journal
        {
            get { return (IJournal)GetValue(JournalProperty); }
            set { SetValue(JournalProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        /// <summary>
        /// 返回否可以执行后退操作
        /// </summary>
        public bool CanGoBack
        {
            get { return (bool)GetValue(CanGoBackProperty); }
            protected set { SetValue(CanGoBackProperty, value); }
        }
        /// <summary>
        /// 返回是否可以执行前进操作
        /// </summary>
        public bool CanGoForward
        {
            get { return (bool)GetValue(CanGoForwardProperty); }
            protected set { SetValue(CanGoForwardProperty, value); }
        }

        /// <summary>
        /// 获取或设置 <see cref="NavigationFrame"/> 关联的 <see cref="INavigationContentProvider"/>
        /// </summary>
        public INavigationContentProvider ContentProvider
        {
            get { return (INavigationContentProvider)GetValue(ContentProviderProperty); }
            set { SetValue(ContentProviderProperty, value); }
        }

        #endregion

        /// <summary>
        /// 返回是否正在执行导航操作
        /// </summary>
        public bool NavigationInProgress { get; private set; }
        /// <summary>
        /// 返回导航状态
        /// </summary>
        public NavigationStatus Status
        {
            get { return _NavigationStatus; }
            private set { _NavigationStatus = value; }
        }

        static NavigationFrame()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationFrame), new FrameworkPropertyMetadata(typeof(NavigationFrame)));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            _BreadcrumbBar = GetTemplateChild(partBar) as BreadcrumbBar;
            _BackBtn = GetTemplateChild(partBackBtn) as Button;
            _ForwardBtn = GetTemplateChild(partForwardBtn) as Button;

            if (_BreadcrumbBar != null)
                _BreadcrumbBar.BreadcrumItemSelected += _BreadcrumbBar_BreadcrumItemSelected;

            if (_BackBtn != null)
                _BackBtn.Click += _BackBtn_Click;

            if (_ForwardBtn != null)
                _ForwardBtn.Click += _ForwardBtn_Click;

            base.OnApplyTemplate();
        }

        /// <summary>
        /// 前进按钮 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            GoForward();
        }

        /// <summary>
        /// 后退按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _BackBtn_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }


        /// <summary>
        /// <see cref="BreadcrumbBar"/> 点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _BreadcrumbBar_BreadcrumItemSelected(object sender, BreadcrumbBarItem e)
        {
            if (e != null)
                Navigate(e.NavigationItem);
        }

        /// <summary>
        /// 初始化 <seealso cref="NavigationFrame"/> 类的新实例
        /// </summary>
        public NavigationFrame()
        {
            // 创建时默认创建 Journal 实例作为导航日志的实现
            Journal = new Journal() { Navigator = this };
        }

        /// <summary>
        /// 当 <seealso cref="Journal"/> 属性变化时调用
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        protected static void OnJournalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// 当 <seealso cref="Source"/> 属性变化时调用
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private static void OnSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NavigationFrame navigationFrame = o as NavigationFrame;
            if (navigationFrame != null)
            {
                navigationFrame.OnSourceChanged(e.OldValue, e.NewValue);
            }
        }

        /// <summary>
        /// 当 <seealso cref="ContentProvider"/> 属性变化时调用
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private static void OnContentProviderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NavigationFrame navigationFrame = o as NavigationFrame;
            if (navigationFrame != null)
            {
                navigationFrame.Journal.NavigationContentProvider = (INavigationContentProvider)e.NewValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnSourceChanged(object oldValue, object newValue)
        {
            Navigate(newValue);
        }

        /// <summary>
        /// 跟新<seealso cref="CanGoBack"/> 和 <seealso cref="CanGoForward"/> 状态
        /// </summary>
        internal void UpdateJournalProperties()
        {
            //CanGoBack = (ActualJournal != null && ActualJournal.CanGoBack);
            //CanGoForward = (ActualJournal != null && ActualJournal.CanGoForward);
        }

        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="source">源</param>
        public void Navigate(object source)
        {
            Navigate(source, null);
        }
        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="param">参数</param>
        public void Navigate(object source, object param)
        {
            Status = NavigationStatus.Executing;
            Dispatcher.BeginInvoke(new Action(delegate
            {
                Navigate(NavigationMode.New, source, param);
            }));
        }

        /// <summary>
        /// 执行导航操作
        /// </summary>
        /// <param name="mode">导航模式</param>
        /// <param name="source">源</param>
        /// <param name="param">参数</param>
        private void Navigate(NavigationMode mode, object source, object param)
        {
            switch (mode)
            {
                case NavigationMode.New:
                    if (!NavigationInProgress)
                    {
                        NavigationInProgress = true;
                        Journal.Navigate(source, param);
                    }
                    break;
                case NavigationMode.Back:
                    if (Journal.CanGoBack)
                    {
                        Journal.GoBack(param);
                    }
                    break;
                case NavigationMode.Forward:
                    if (Journal.CanGoForward)
                    {
                        Journal.GoForward();
                    }
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        public void GoBack(object param = null)
        {
            Status = NavigationStatus.Executing;
            Navigate(NavigationMode.Back, null, param);
        }

        /// <summary>
        /// 
        /// </summary>
        public void GoForward()
        {
            Status = NavigationStatus.Executing;
            Navigate(NavigationMode.Forward, null, null);
        }

        #region INavigationFrame

        /// <summary>
        /// 获取或设置导航的缓存模式
        /// </summary>
        public NavigationCacheMode NavigationCacheMode { get; set; }

        /// <summary>
        /// 开始导航时调用
        /// </summary>
        /// <param name="source"></param>
        /// <param name="mode"></param>
        /// <param name="navigationState"></param>
        /// <returns></returns>
        bool INavigationFrame.Navigating(object source, NavigationMode mode, object navigationState)
        {
            // TODO：删除DEV中设置动画的部分

            NavigatingEventArgs navigatingEventArgs = new NavigatingEventArgs(source, mode, navigationState);
            if (Navigating != null)
            {
                Navigating(this, navigatingEventArgs);
            }

            if (!navigatingEventArgs.Cancel && Journal.Current != null)
            {
                //object content = Journal.Current.Content;
                #region DEV 
                //INavigationAware navigationAware = content as INavigationAware;
                //FrameworkElement frameworkElement = content as FrameworkElement;
                //if (navigationAware != null)
                //{
                //    navigationAware.NavigatingFrom(navigatingEventArgs);
                //}
                //if (frameworkElement != null)
                //{
                //    // 如果 DataContext（ViewModel）实现了INavigationAware接口，则会调用VM中NavigatingFrom方法
                //    INavigationAware navigationAware2 = frameworkElement.DataContext as INavigationAware;
                //    if (navigationAware2 != null && navigationAware2 != frameworkElement)
                //    {
                //        navigationAware2.NavigatingFrom(navigatingEventArgs);
                //    }
                //}
                #endregion
            }
            // 这里应该是调用等待条
            //if (!navigatingEventArgs.Cancel)
            //{
            //    this.LockContent();
            //}
            return !navigatingEventArgs.Cancel;
        }

        /// <summary>
        /// 导航完成
        /// </summary>
        /// <param name="source"></param>
        /// <param name="content"></param>
        /// <param name="navigationState"></param>
        void INavigationFrame.NavigationComplete(object source, object content, object navigationState)
        {
            // 重点！！界面显示
            if (content is Window)
            {
                ((Window)content).ShowDialog();
            }
            else
            {
                Content = content;
            }


            if (!(content is UIElement) && ContentTemplateSelector != null)
            {
                ContentTemplate = ContentTemplateSelector.SelectTemplate(source, content as DependencyObject);
            }

            NavigationItem navItem = source as NavigationItem;
            if (navItem != null)
            {
                _BreadcrumbBar.SetNavigationItem(navItem);
            }

            // TODO：导航完成后关闭加载界面
            NavigationInProgress = false;
            Status = NavigationStatus.Completed;
            // 触发 Navigated 事件
            NavigatedEventArgs navigationEventArgs = new NavigatedEventArgs(source, content, navigationState);
            if (Navigated != null)
                Navigated(this, navigationEventArgs);
        }

        /// <summary>
        /// 导航过程中发生错误
        /// </summary>
        /// <param name="source"></param>
        /// <param name="ex"></param>
        void INavigationFrame.NavigationFailed(object source, NavigationException ex)
        {
            NavigationInProgress = false;
            Status = NavigationStatus.Failed;

            if (NavigationFailed != null)
            {
                NavigationFailedEventArgs e = new NavigationFailedEventArgs(source, ex);
                NavigationFailed(this, e);
            }
        }

        void INavigationFrame.NavigationStopped(object source, NavigationMode mode, object navigationState)
        {
            NavigationInProgress = false;
            Status = NavigationStatus.Aborted;
        }
        /// <summary>
        /// 
        /// </summary>
        public void GoBack()
        {
            GoBack(null);
        }

        #endregion
    }

    #region 自定义 EventArgs
    /// <summary>
    /// 
    /// </summary>
    public abstract class NavigationBaseEventArgs : EventArgs
    {
        /// <summary>
        /// 源
        /// </summary>
        public object Source
        {
            get;
            private set;
        }

        /// <summary>
        /// 参数
        /// </summary>
        public object Parameter
        {
            get;
            private set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameter"></param>
        public NavigationBaseEventArgs(object source, object parameter)
        {
            Source = source;
            Parameter = parameter;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class NavigatedEventArgs : NavigationBaseEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public object Content
        {
            get;
            private set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="content"></param>
        /// <param name="parameter"></param>
        public NavigatedEventArgs(object source, object content, object parameter) : base(source, parameter)
        {
            Content = content;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class NavigationFailedEventArgs : NavigationBaseEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="ex"></param>
        public NavigationFailedEventArgs(object source, Exception ex) : base(source, null)
        {
            Exception = ex;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class NavigatingEventArgs : NavigationBaseEventArgs
    {
        /// <summary>
        /// 导航模式
        /// </summary>
        public NavigationMode NavigationMode
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否取消
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="mode"></param>
        /// <param name="parameter"></param>
        public NavigatingEventArgs(object source, NavigationMode mode, object parameter) : base(source, parameter)
        {
            NavigationMode = mode;
        }
    }
    #endregion
}
