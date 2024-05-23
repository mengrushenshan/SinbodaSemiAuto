using Sinboda.Framework.Control.Controls.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace Sinboda.Framework.Control.Controls
{
    /// <summary>
    /// 面包屑导航
    /// </summary>
    [TemplatePart(Name = partListBox)]
    public class BreadcrumbBar : System.Windows.Controls.Control
    {
        const string partListBox = "PART_ListBox";
        private ListBox listBox;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<BreadcrumbBarItem> BreadcrumItemSelected;    // 选中事件

        #region Dependency Properties

        #endregion

        private ObservableCollection<ButtonBase> buttons = new ObservableCollection<ButtonBase>();
        /// <summary>
        /// 右侧功能按钮组
        /// </summary>
        public ObservableCollection<ButtonBase> Buttons
        {
            get { return buttons; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            listBox = GetTemplateChild(partListBox) as ListBox;
            if (listBox != null)
                listBox.SelectionChanged += ListBox_SelectionChanged;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BreadcrumItemSelected != null)
                BreadcrumItemSelected(this, (BreadcrumbBarItem)listBox.SelectedItem);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void SetNavigationItem(NavigationItem item)
        {
            Stack<NavigationItem> stack = new Stack<NavigationItem>();
            stack.Push(item);
            while (item.ParentItem != null)
            {
                stack.Push(item.ParentItem);
                item = item.ParentItem;
            }

            if (listBox != null)
            {
                listBox.Items.Clear();
                listBox.Items.Add(new BreadcrumbBarItem(stack.Pop()));
                foreach (var ni in stack)
                {
                    listBox.Items.Add(new BreadcrumbBarItem(ni));
                }
            ((BreadcrumbBarItem)listBox.Items[listBox.Items.Count - 1]).IsLastItem = true;
            }
        }
    }

    /// <summary>
    /// 导航项
    /// </summary>
    public class BreadcrumbBarItem : ContentControl
    {
        #region DependencyProperties
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsLastItemProperty = DependencyProperty.Register("IsLastItem", typeof(bool), typeof(BreadcrumbBarItem), new PropertyMetadata(false));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsRootProperty = DependencyProperty.Register("IsRootItem", typeof(bool), typeof(BreadcrumbBarItem), new PropertyMetadata(false));
        /// <summary>
        /// 
        /// </summary>
        public bool IsRootItem
        {
            get { return (bool)GetValue(IsRootProperty); }
            set { SetValue(IsRootProperty, value); }
        }


        /// <summary>
        /// 是否为末尾项
        /// </summary>
        public bool IsLastItem
        {
            get { return (bool)GetValue(IsLastItemProperty); }
            set { SetValue(IsLastItemProperty, value); }
        }
        #endregion

        /// <summary>
        /// 获取或设置导航源
        /// </summary>
        public object Source { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public NavigationItem NavigationItem { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public BreadcrumbBarItem()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public BreadcrumbBarItem(NavigationItem item)
        {
            NavigationItem = item;
            IsRootItem = item.ParentItem == null;
            Content = item.Name;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class BorderGapMaskConverter : IMultiValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = System.Convert.ToDouble(values[0]) + 50;
            double height = System.Convert.ToDouble(values[1]);
            double x = 15;
            double y = 20;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.IsClosed = true;
            pathFigure.StartPoint = new Point(0, 0);
            pathFigure.Segments.Add(new LineSegment() { Point = new Point(x, y) });
            pathFigure.Segments.Add(new LineSegment() { Point = new Point(0, y * 2) });
            pathFigure.Segments.Add(new LineSegment() { Point = new Point(width, y * 2) });
            pathFigure.Segments.Add(new LineSegment() { Point = new Point(width + x, y) });
            pathFigure.Segments.Add(new LineSegment() { Point = new Point(width, 0) });
            pathGeometry.Figures.Add(pathFigure);
            return pathGeometry;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[]
            {
                Binding.DoNothing
            };
        }
    }
}
