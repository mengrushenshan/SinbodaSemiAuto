using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Sinboda.Framework.Control.Controls
{
    /// <summary>
    /// SinWindow
    /// </summary>
    [TemplatePart(Name = "ButtonListName", Type = typeof(ItemsControl))]
    public class SinWindow : Window
    {
        private const string ButtonListName = "PART_BottomItems";
        private ItemsControl _ButtonItemsControl;

        #region Dependency Propertys

        /// <summary>
        /// 标识 BackgroundContent 的依赖性属性
        /// </summary>
        public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register("BackgroundContent", typeof(object), typeof(SinWindow), new PropertyMetadata(null));
        /// <summary>
        /// 标识 TitleForeground 的依赖性属性
        /// </summary>
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(SinWindow), new UIPropertyMetadata(Brushes.White));
        /// <summary>
        /// 标识 TitleBackground 的依赖性属性
        /// </summary>
        public static readonly DependencyProperty TitleBackgroundProperty = DependencyProperty.Register("TitleBackground", typeof(Brush), typeof(SinWindow), new UIPropertyMetadata(Brushes.Green));
        /// <summary>
        /// 标识 TitleBorderBrush 的依赖性属性
        /// </summary>
        public static readonly DependencyProperty TitleBorderBrushProperty = DependencyProperty.Register("TitleBorderBrush", typeof(Brush), typeof(SinWindow), new UIPropertyMetadata(Brushes.Green));
        /// <summary>
        /// 标识 <seealso cref="GlyphIcon"/> 依赖性属性
        /// </summary>
        public static readonly DependencyProperty GlyphIconProperty = SinTextBox.GlyphIconProperty.AddOwner(typeof(SinWindow));
        /// <summary>
        /// 标识 <seealso cref="IsBottomPanel"/> 只读依赖性属性
        /// </summary>
        public static readonly DependencyPropertyKey IsBottomPanelPropertyKey = DependencyProperty.RegisterReadOnly("IsBottomPanel", typeof(bool), typeof(SinWindow), new PropertyMetadata(false));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsBottomPanelProperty = IsBottomPanelPropertyKey.DependencyProperty;

        /// <summary>
        /// 标识 <seealso cref="TitleHeight"/> 的依赖性属性
        /// </summary>
        public static readonly DependencyProperty TitleHeightProperty = DependencyProperty.Register("TitleHeight", typeof(double), typeof(SinWindow), new PropertyMetadata(50d));

        /// <summary>
        /// 获取或设置标题栏的高度
        /// </summary>
        public double TitleHeight
        {
            get { return (double)GetValue(TitleHeightProperty); }
            set { SetValue(TitleHeightProperty, value); }
        }

        /// <summary>
        /// 获取 <seealso cref="BottomPanel"/> 属性是否为空
        /// </summary>
        public bool IsBottomPanel
        {
            get { return (bool)GetValue(IsBottomPanelProperty); }
        }
        /// <summary>
        /// 获取或设置 <see cref="DrTextBox"/> 左侧矢量图标名称
        /// </summary>
        public string GlyphIcon
        {
            get { return (string)GetValue(GlyphIconProperty); }
            set { SetValue(GlyphIconProperty, value); }
        }

        /// <summary>
        /// 标题栏边框颜色 
        /// </summary>
        public Brush TitleBorderBrush
        {
            get { return (Brush)GetValue(TitleBorderBrushProperty); }
            set { SetValue(TitleBorderBrushProperty, value); }
        }

        /// <summary>
        /// 获取或设置标题区背景色
        /// </summary>
        public Brush TitleBackground
        {
            get { return (Brush)GetValue(TitleBackgroundProperty); }
            set { SetValue(TitleBackgroundProperty, value); }
        }
        /// <summary>
        /// 标题颜色
        /// </summary>
        public Brush TitleForeground
        {
            get { return (Brush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }


        /// <summary>
        /// 获取或设置窗口背景内容
        /// </summary>
        public object BackgroundContent
        {
            get { return (object)GetValue(BackgroundContentProperty); }
            set { SetValue(BackgroundContentProperty, value); }
        }
        #endregion

        /// <summary>
        /// 获取窗口的底部区域
        /// </summary>
        public ObservableCollection<object> BottomPanel { get; }

        static SinWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SinWindow), new FrameworkPropertyMetadata(typeof(SinWindow)));
        }

        /// <summary>
        /// 构造
        /// </summary>
        public SinWindow()
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
            Style = (Style)Application.Current.Resources[typeof(SinWindow)]; // 设置样式
            BottomPanel = new ObservableCollection<object>();
            BottomPanel.CollectionChanged += BottomPanel_CollectionChanged;

            //// 修复WindowChrome导致的窗口大小错误
            var sizeToContent = SizeToContent.Manual;
            Loaded += (ss, ee) =>
            {
                sizeToContent = SizeToContent;
            };
            ContentRendered += (ss, ee) =>
            {
                SizeToContent = SizeToContent.Manual;
                Width = ActualWidth;
                Height = ActualHeight;
                SizeToContent = sizeToContent;
            };

            ShowActivated = true;
            ShowInTaskbar = false;

            if (Application.Current != null && Application.Current.MainWindow != this)
                Owner = Application.Current.MainWindow;
        }

        private void BottomPanel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// 初始化时调用
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// 应用模版时调用
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _ButtonItemsControl = GetTemplateChild(ButtonListName) as ItemsControl;
            if (_ButtonItemsControl == null) return;
            UpdateView();
        }

        private void UpdateView()
        {
            SetIsBottomPanel(BottomPanel != null && BottomPanel.Count > 0);
            if (_ButtonItemsControl == null) return;

            foreach (var item in BottomPanel)
            {
                _ButtonItemsControl.Items.Add(item);
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        /// <summary>
        /// 最大化
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        /// <summary>
        /// 调整大小
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        /// <summary>
        /// 是否可调整大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        /// <summary>
        /// 是否可以最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        private void SetIsBottomPanel(bool value)
        {
            SetValue(IsBottomPanelPropertyKey, value);
        }
    }
}
