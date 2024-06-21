using Sinboda.Framework.Core.AbstractClass;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Helpers
{
    internal class PrintHelper : TBaseSingleton<PrintHelper>
    {

        public void Print()
        {
            //新建打印对象
            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
            //打印机名字
            pd.PrinterSettings.PrinterName = "FUJIFILM Apeos C2560";
            //打印文档显示的名字
            pd.DocumentName = "订单";
            //
            //pd.PrinterSettings.MaximumCopies.ToString();

            //打印的格式
            pd.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.Menu);

            //
            pd.PrintController = new System.Drawing.Printing.StandardPrintController();

            //开始打印
            pd.Print();
        }

        //绘图需要使用的数组 后期可以套用变量list
        public string[] menu = { "菜一", "菜二", "中级菜单" };

        /// <summary>
        /// 打印的格式:菜单
        /// </summary>
        /// <param name="sender">自定义报表（原理是绘图）</param>
        /// <param name="e"></param>
        private void Menu(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            //绘制（输出，文字格式（字体，大小），颜色，位置起始位置x，y轴坐标）                 
            e.Graphics.DrawString("店名", new Font(new FontFamily("黑体"), 11), System.Drawing.Brushes.Black, 138, 10);

            //打印两个点的坐标（颜色，坐标1，坐标2）
            e.Graphics.DrawLine(Pens.Black, 8, 30, 292, 30);


            e.Graphics.DrawString("订单号：（）", new Font(new FontFamily("黑体"), 11), System.Drawing.Brushes.Black, 10, 35);

            e.Graphics.DrawString("编号", new Font(new FontFamily("黑体"), 11), System.Drawing.Brushes.Black, 10, 55);
            e.Graphics.DrawString("菜名", new Font(new FontFamily("黑体"), 11), System.Drawing.Brushes.Black, 75, 55);
            e.Graphics.DrawString("单价", new Font(new FontFamily("黑体"), 11), System.Drawing.Brushes.Black, 150, 55);
            e.Graphics.DrawString("数量", new Font(new FontFamily("黑体"), 11), System.Drawing.Brushes.Black, 200, 55);

            int i = 0;
            //循环输出变量
            foreach (string element in menu)
            {

                i = i + 20;
                e.Graphics.DrawString(element, new Font(new FontFamily("黑体"), 11), System.Drawing.Brushes.Black, 75, 80 + i);

            }


            e.Graphics.DrawLine(Pens.Black, 8, 200, 292, 200);

        }
    }
}
