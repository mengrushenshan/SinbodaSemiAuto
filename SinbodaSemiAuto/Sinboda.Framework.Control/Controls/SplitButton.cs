using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows;

namespace Sinboda.Framework.Control.Controls
{
    /// <summary>
    /// 下拉菜单按钮
    /// </summary>
    [TemplatePart(Name = partMenu)]
    [TemplatePart(Name = partDropDown)]
    [ContentProperty("Items")]
    [DefaultProperty("Items")]
    public class SplitButton : ButtonBase
    {
        const string partMenu = "PART_Menu";
        const string partDropDown = "PART_DropDown";

        private ContextMenu contextMenu;
        private System.Windows.Controls.Control dropDownBtn;

        #region DependencyProperty

        /// <summary>
        /// 标识 <see cref="HorizontalOffset"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty = ContextMenuService.HorizontalOffsetProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata(0.0));
        /// <summary>
        /// 标识 <see cref="IsContextMenuOpen"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty IsContextMenuOpenProperty = DependencyProperty.Register("IsContextMenuOpen", typeof(bool), typeof(SplitButton), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// 标识 <see cref="Mode"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(SplitButtonMode), typeof(SplitButton), new FrameworkPropertyMetadata(SplitButtonMode.Dropdown));
        /// <summary>
        /// 标识 <see cref="Placement"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty PlacementProperty = ContextMenuService.PlacementProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata(PlacementMode.Bottom));
        /// <summary>
        /// 标识 <see cref="PlacementRectangle"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty PlacementRectangleProperty = ContextMenuService.PlacementRectangleProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata(Rect.Empty));
        /// <summary>
        /// 标识 <see cref="VerticalOffset"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty = ContextMenuService.VerticalOffsetProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata(0.0));
        /// <summary>
        /// 标识 <see cref="DropDownContentTemplate"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty DropDownContentTemplateProperty = DependencyProperty.Register("DropDownContentTemplate", typeof(DataTemplate), typeof(SplitButton), new UIPropertyMetadata(null));
        /// <summary>
        /// 标识 <see cref="Image"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(object), typeof(SplitButton), new UIPropertyMetadata(null));
        /// <summary>
        /// 标识 <see cref="IsImageVisible"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty IsImageVisibleProperty = DependencyProperty.Register("IsImageVisible", typeof(bool), typeof(SplitButton), new UIPropertyMetadata(true));
        /// <summary>
        /// 标识 <see cref="SplitButtonItemsSource"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty SplitButtonItemsSourceProperty = DependencyProperty.Register("SplitButtonItemsSource", typeof(ObservableCollection<object>), typeof(SplitButton), new PropertyMetadata(new ObservableCollection<object>(), OnSplitButtonItemsSourceChanged));
        /// <summary>
        /// 标识 <see cref="MenuStyle"/> 的依赖项属性
        /// </summary>
        public static readonly DependencyProperty MenuStyleProperty = DependencyProperty.Register("MenuStyle", typeof(Style), typeof(SplitButton), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 获取或设置菜单样式
        /// </summary>
        public Style MenuStyle
        {
            get { return (Style)GetValue(MenuStyleProperty); }
            set { SetValue(MenuStyleProperty, value); }
        }

        /// <summary>
        /// 获取或设置图片是否显示
        /// </summary>
        public bool IsImageVisible
        {
            get { return (bool)GetValue(IsImageVisibleProperty); }
            set { SetValue(IsImageVisibleProperty, value); }
        }
        /// <summary>
        /// 获取或设置图片资源
        /// </summary>
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        /// <summary>
        /// 获取或设置DropDownContent的数据模板
        /// </summary>
        public DataTemplate DropDownContentTemplate
        {
            get { return (DataTemplate)GetValue(DropDownContentTemplateProperty); }
            set { SetValue(DropDownContentTemplateProperty, value); }
        }
        /// <summary>
        /// 获取或设置SplitButtonItemsSource
        /// </summary>
        public ObservableCollection<object> SplitButtonItemsSource
        {
            get { return (ObservableCollection<object>)GetValue(SplitButtonItemsSourceProperty); }
            set { SetValue(SplitButtonItemsSourceProperty, value); }
        }
        /// <summary>
        /// 获取或设置菜单是否打开
        /// </summary>
        public bool IsContextMenuOpen
        {
            get { return (bool)GetValue(IsContextMenuOpenProperty); }
            set { SetValue(IsContextMenuOpenProperty, value); }
        }

        /// <summary>
        /// 获取或设置菜单在屏幕上的显示位置
        /// </summary>
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        /// <summary>
        /// 取或设置当菜单打开时该控件相对于其放置的矩形
        /// </summary>
        public Rect PlacementRectangle
        {
            get { return (Rect)GetValue(PlacementRectangleProperty); }
            set { SetValue(PlacementRectangleProperty, value); }
        }

        /// <summary>
        /// 菜单的水平方向偏移量
        /// </summary>
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        /// <summary>
        /// 菜单的垂直方向偏移量
        /// </summary>
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }
        /// <summary>
        /// 按钮模式
        /// </summary>
        public SplitButtonMode Mode
        {
            get { return (SplitButtonMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<object> Items { get; }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public SplitButton()
        {
            Items = new List<object>();
        }

        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            dropDownBtn = GetTemplateChild(partDropDown) as System.Windows.Controls.Control;
            contextMenu = GetTemplateChild(partMenu) as ContextMenu;
            contextMenu.PlacementTarget = this;
            if (dropDownBtn != null)
            {
                dropDownBtn.MouseDown += DropDownBtn_MouseDown;
            }
            if (contextMenu != null)
            {
                contextMenu.Opened += ContextMenu_Opened;
                contextMenu.Closed += ContextMenu_Closed;
            }

            contextMenu.ItemsSource = Items;
            base.OnApplyTemplate();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {

        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {

        }

        private void DropDownBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            OpenMenu();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnClick()
        {
            switch (Mode)
            {
                case SplitButtonMode.Dropdown:
                    OpenMenu();
                    break;

                default:
                    base.OnClick();
                    break;
            }
        }


        private void OpenMenu()
        {
            if (IsContextMenuOpen)
                return;

            IsContextMenuOpen = true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        protected static void OnSplitButtonItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SplitButtonMode
    {
        /// <summary>
        /// 
        /// </summary>
        Split,
        /// <summary>
        /// 
        /// </summary>
        Dropdown
    }
}
