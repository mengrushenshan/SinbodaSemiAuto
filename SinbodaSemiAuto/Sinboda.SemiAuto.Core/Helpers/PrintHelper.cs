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
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name">打印机名称</param>
        public void Init(string name)
        {
            printerName = name;
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="img"></param>
        public void Print(Image img)
        {
            image = img;

            //新建打印对象
            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
            //打印机名字
            pd.PrinterSettings.PrinterName = printerName;
            //打印文档显示的名字
            pd.DocumentName = "报告单";

            //打印的格式
            pd.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.ImagePrint);

            //
            pd.PrintController = new System.Drawing.Printing.StandardPrintController();

            //开始打印
            pd.Print();
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="body">字符串 带格式</param>
        public void Print(string title, string body)
        {
            strBody = body;
            strTitle = title;
            //新建打印对象
            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
            //打印机名字
            pd.PrinterSettings.PrinterName = printerName;
            //打印文档显示的名字
            pd.DocumentName = "报告单";
            //
            //pd.PrinterSettings.MaximumCopies.ToString();

            //打印的格式
            pd.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.StringPrint);

            //
            pd.PrintController = new System.Drawing.Printing.StandardPrintController();

            //开始打印
            pd.Print();
        }

        /// <summary>
        /// 要打印的文字内容
        /// </summary>
        private string strBody;
        private string strTitle;
        private string printerName;

        private Image image;

        /// <summary>
        /// 打印的格式:图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagePrint(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //直接调用图片对象绘制
            e.Graphics.DrawImage(image, 0, 0);
        }

        /// <summary>
        /// 打印的格式 TODO:需要根据打印得格式调整
        /// </summary>
        /// <param name="sender">自定义报表（原理是绘图）</param>
        /// <param name="e"></param>
        private void StringPrint(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            //绘制（输出，文字格式（字体，大小），颜色，位置起始位置x，y轴坐标）                 
            e.Graphics.DrawString(strTitle, new Font(new FontFamily("黑体"), 24), System.Drawing.Brushes.Black, 138, 10);

            //打印两个点的坐标（颜色，坐标1，坐标2）
            e.Graphics.DrawLine(Pens.Black, 8, 30, 292, 30);

            e.Graphics.DrawString(strBody, new Font(new FontFamily("黑体"), 11), System.Drawing.Brushes.Black, 10, 35);

            e.Graphics.DrawLine(Pens.Black, 8, 200, 292, 200);

        }
    }
}
