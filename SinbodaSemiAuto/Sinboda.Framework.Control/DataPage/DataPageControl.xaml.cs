using Sinboda.Framework.Common.ResourceExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sinboda.Framework.Control.DataPage
{
    /// <summary>
    /// DataPageControl.xaml 的交互逻辑
    /// </summary>
    public partial class DataPageControl : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DataPageControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 页面页码改变事件
        /// </summary>
        public event EventHandler<PageControlTestEventHandler> OnPageIndexChanged;
        /// <summary>
        /// 页码改变
        /// </summary>
        /// <param name="e"></param>
        private void PageIndexChanged(PageControlTestEventHandler e)
        {
            if (OnPageIndexChanged != null)
                OnPageIndexChanged(this, e);
        }

        private static readonly DependencyProperty _TotalCount = DependencyProperty.Register("TotalCount", typeof(int), typeof(DataPageControl), new PropertyMetadata(1, new PropertyChangedCallback(OnTotalCountChange)));
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount
        {
            get
            {
                return (int)GetValue(_TotalCount);
            }
            set
            {
                SetValue(_TotalCount, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnTotalCountChange(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataPageControl control = (DataPageControl)obj;
            RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>
                ((int)args.OldValue, (int)args.NewValue, TotalCountChangeEvent);
            control.OnTotalCountChange(e);
        }
        /// <summary>
        /// 总记录数变更事件
        /// </summary>
        public static readonly RoutedEvent TotalCountChangeEvent = EventManager.RegisterRoutedEvent("TotalCountChange", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<int>), typeof(DataPageControl));
        /// <summary>
        /// 总记录数变更事件属性
        /// </summary>
        public event RoutedPropertyChangedEventHandler<int> TotalCountChange
        {
            add { AddHandler(TotalCountChangeEvent, value); }
            remove { RemoveHandler(TotalCountChangeEvent, value); }
        }
        /// <summary>
        /// 总记录数变更处理
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnTotalCountChange(RoutedPropertyChangedEventArgs<int> args)
        {
            RaiseEvent(args);

            totalCount.Content = TotalCount.ToString();

            if (PageRecordCount > 0)
            {
                if (TotalCount % PageRecordCount > 0)
                    TotalPageCount = TotalCount / PageRecordCount + 1;
                else
                    TotalPageCount = TotalCount / PageRecordCount;

                totalPageCount.Content = "/" + TotalPageCount.ToString() + StringResourceExtension.GetLanguage(148, "页");
                CurrentPageIndex = 1;
            }
        }

        private static readonly DependencyProperty _PageRecordCount = DependencyProperty.Register("PageRecordCount", typeof(int), typeof(DataPageControl), new PropertyMetadata(100));
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageRecordCount
        {
            get { return (int)GetValue(_PageRecordCount); }
            set { SetValue(_PageRecordCount, value); }
        }

        private static readonly DependencyProperty _TotalPageCount = DependencyProperty.Register("TotalPageCount", typeof(int), typeof(DataPageControl), new PropertyMetadata(1));
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPageCount
        {
            get
            {
                return (int)GetValue(_TotalPageCount);
            }
            set
            {
                //如果页面数小于1，默认为1
                if (value < 1)
                {
                    value = 1;
                }
                SetValue(_TotalPageCount, value);
            }
        }


        private int _CurrentPageIndex = 1;
        /// <summary>
        /// 当前页索引(从1开始)
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                return _CurrentPageIndex;
            }
            set
            {
                _CurrentPageIndex = value;
                PageControlTestEventHandler pceh = new PageControlTestEventHandler(PageRecordCount * (value - 1));
                PageIndexChanged(pceh);
                IndexTB.Text = CurrentPageIndex.ToString();
                FlashButtonEnable();
            }
        }
        /// <summary>
        /// 控制按钮状态
        /// </summary>
        private void FlashButtonEnable()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                //总页数为1
                if (TotalPageCount == 1 && CurrentPageIndex == 1)
                {
                    FirstBtn.IsEnabled = false;
                    UpPageBtn.IsEnabled = false;
                    NextPageBtn.IsEnabled = false;
                    LastPageBtn.IsEnabled = false;
                }
                //当前为末页
                else if (CurrentPageIndex < TotalPageCount && CurrentPageIndex == 1)
                {
                    FirstBtn.IsEnabled = false;
                    UpPageBtn.IsEnabled = false;
                    NextPageBtn.IsEnabled = true;
                    LastPageBtn.IsEnabled = true;
                }
                //当前为中间页
                else if (CurrentPageIndex > 1 && CurrentPageIndex < TotalPageCount)
                {
                    FirstBtn.IsEnabled = true;
                    UpPageBtn.IsEnabled = true;
                    NextPageBtn.IsEnabled = true;
                    LastPageBtn.IsEnabled = true;
                }
                //当前为末页
                else if (TotalPageCount == CurrentPageIndex && CurrentPageIndex > 1)
                {
                    FirstBtn.IsEnabled = true;
                    UpPageBtn.IsEnabled = true;
                    NextPageBtn.IsEnabled = false;
                    LastPageBtn.IsEnabled = false;
                }
                string pageContent = IndexTB.Text.Trim();
                pageContent = pageContent.Contains(".") ? pageContent.Split('.')[0] : pageContent;
                if (int.Parse(pageContent) < 1 || int.Parse(pageContent) > TotalPageCount)
                {
                    GoToPageBtn.IsEnabled = false;
                }
                else
                {
                    GoToPageBtn.IsEnabled = true;
                    GoToPageBtn.Tag = int.Parse(pageContent);
                }

                totalPageCount.Content = "/" + TotalPageCount.ToString() + StringResourceExtension.GetLanguage(148, "页");
                totalPageCount.Tag = TotalPageCount;
            }));

        }
        /// <summary>
        /// 跳转页输入框回车事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IndexTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(IndexTB.Text)) return;
                string pageContent = IndexTB.Text.Trim();
                pageContent = pageContent.Contains(".") ? pageContent.Split('.')[0] : pageContent;
                if (string.IsNullOrEmpty(pageContent)) return;
                int skipPageCount = int.Parse(pageContent);
                if (skipPageCount <= TotalPageCount)
                    CurrentPageIndex = skipPageCount;
                FlashButtonEnable();
            }
        }
        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirstBtn_Click(object sender, RoutedEventArgs e)
        {
            IndexTB.Text = "1";
            CurrentPageIndex = 1;
        }
        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageIndex <= TotalPageCount && CurrentPageIndex > 1)
            {
                CurrentPageIndex = CurrentPageIndex - 1;
                FlashButtonEnable();
                IndexTB.Text = CurrentPageIndex.ToString();
            }
        }
        /// <summary>
        /// 跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(IndexTB.Text)) return;
            string pageContent = IndexTB.Text.Trim();
            pageContent = pageContent.Contains(".") ? pageContent.Split('.')[0] : pageContent;
            if (string.IsNullOrEmpty(pageContent)) return;
            int skipPageCount = int.Parse(pageContent);
            if (skipPageCount <= TotalPageCount)
                CurrentPageIndex = skipPageCount;
            FlashButtonEnable();
        }
        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageIndex < TotalPageCount)
            {
                CurrentPageIndex = CurrentPageIndex + 1;
                FlashButtonEnable();
                IndexTB.Text = CurrentPageIndex.ToString();
            }
        }
        /// <summary>
        /// 尾页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LastPageBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentPageIndex = TotalPageCount;
            IndexTB.Text = TotalPageCount.ToString();
        }
    }
    /// <summary>
    /// 跳转事件
    /// </summary>
    public class PageControlTestEventHandler : EventArgs
    {
        /// <summary> 
        /// 页码 
        /// </summary> 
        public int SkipCount { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="skipCount"></param>
        public PageControlTestEventHandler(int skipCount)
        {
            SkipCount = skipCount;
        }
    }
}
