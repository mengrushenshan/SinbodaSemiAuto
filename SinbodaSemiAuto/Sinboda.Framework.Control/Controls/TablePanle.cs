using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Sinboda.Framework.Control.Controls
{
    /// <summary>
    /// 带分割线的水平自适应布局
    /// <para>必须设置 <seealso cref="ItemWidth"/> 和 <seealso cref="ItemHeight"/> 属性</para>
    /// </summary>
    public class TablePanel : Panel
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(TablePanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(TablePanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LineWidthProperty = DependencyProperty.Register("LineWidth", typeof(double), typeof(TablePanel), new PropertyMetadata(1d));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LineBrushProperty = DependencyProperty.Register("LineBrush", typeof(Brush), typeof(TablePanel), new FrameworkPropertyMetadata(Brushes.Gray));
        private int row = 0;    // 行数 
        private int column = 0; // 列数

        /// <summary>
        /// 
        /// </summary>
        public double LineWidth
        {
            get { return (double)GetValue(LineWidthProperty); }
            set { SetValue(LineWidthProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public TablePanel()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            foreach (UIElement uielement in InternalChildren)
            {
                if (uielement != null)
                {
                    uielement.Measure(new Size(ItemWidth, ItemHeight));
                }
            }

            column = Convert.ToInt32(Math.Truncate((constraint.Width + LineWidth) / (ItemWidth + LineWidth)));
            row = InternalChildren.Count % column > 0 ? InternalChildren.Count / column + 1 : Convert.ToInt32(InternalChildren.Count / column);

            return new Size((ItemWidth + LineWidth) * column, (ItemHeight + LineWidth) * row);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement uielement = InternalChildren[i];

                int r = i / column;
                int c = i - r * column;

                double x = c * (ItemWidth + LineWidth);
                double y = r * (ItemHeight + LineWidth);
                uielement.Arrange(new Rect(x, y, ItemWidth, ItemHeight));
            }


            //Size rowSize = new Size();
            //double x = 0, y = 0;
            //int num = 0;
            //for (int i = 0; i < InternalChildren.Count; i++)
            //{
            //    UIElement uielement = InternalChildren[i];
            //    if ((rowSize.Width + ItemWidth) >= finalSize.Width)
            //    {
            //        for (int j = num; j < i; j++)
            //        {
            //            UIElement ui = InternalChildren[j];
            //            ui.Arrange(new Rect(x, y, ItemWidth, rowSize.Height));
            //            x += ItemWidth;
            //            num++;
            //        }
            //        y += rowSize.Height;
            //        rowSize = new Size(ItemWidth, ItemHeight);
            //        x = 0;
            //    }
            //    else
            //    {
            //        rowSize.Width += ItemWidth;
            //        rowSize.Height = ItemHeight;
            //    }
            //}

            //if (num < InternalChildren.Count)
            //{
            //    for (int i = num; i < InternalChildren.Count; i++)
            //    {
            //        UIElement ui = InternalChildren[i];
            //        ui.Arrange(new Rect(x, y, ItemWidth, ItemHeight));
            //        x += ItemWidth;
            //    }
            //}

            return base.ArrangeOverride(finalSize);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        protected override void OnRender(DrawingContext dc)
        {
            Pen pen = new Pen(LineBrush, LineWidth);

            double y = ItemHeight + LineWidth / 2;
            double x = ItemWidth + LineWidth / 2;
            double width = (ItemWidth + LineWidth) * column;
            double heighe = (ItemHeight + LineWidth) * row;
            for (int i = 0; i < row; i++)
            {
                dc.DrawLine(pen, new Point(0, y + (ItemHeight + LineWidth) * i), new Point(width, y + (ItemHeight + LineWidth) * i));
            }

            for (int i = 0; i < column; i++)
            {
                dc.DrawLine(pen, new Point(x + (ItemWidth + LineWidth) * i, 0), new Point(x + (ItemWidth + LineWidth) * i, ActualHeight));
            }

            base.OnRender(dc);
        }
    }
}
