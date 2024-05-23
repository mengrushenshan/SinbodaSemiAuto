using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Reports.UserDesigner;
using DevExpress.XtraReports.UI;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
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

namespace Sinboda.Framework.Print
{
    /// <summary>
    /// Designer.xaml 的交互逻辑
    /// </summary>
    public partial class Designer : Window
    {
        /// <summary>
        /// 数据源
        /// </summary>
        object myDataSource;
        /// <summary>
        /// 报表文件名
        /// </summary>
        string FileName;
        /// <summary>
        /// 传入参数
        /// </summary>
        object[] Parameters;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="fileName"></param>
        /// <param name="parameters"></param>
        public Designer(object dataSource, string fileName, object[] parameters = null)
        {
            InitializeComponent();
            myDataSource = dataSource;
            FileName = fileName;
            Parameters = parameters;
        }
        /// <summary>
        /// 设计报表按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void designer_Click(object sender, RoutedEventArgs e)
        {
            if (!validate(FileName)) return;
            XtraReport report = new XtraReport();
            report.LoadLayout(MapPath.PrintTempletPath + FileName);
            report.DataSource = myDataSource;
            TranseTitle(report);
            ReportDesigner designer = new ReportDesigner();
            designer.DocumentSource = report;
            designer.ShowWindow(this);
        }
        /// <summary>
        /// 预览报表按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void show_Click(object sender, RoutedEventArgs e)
        {
            if (!validate(FileName)) return;
            XtraReport report = new XtraReport();
            report.LoadLayout(MapPath.PrintTempletPath + FileName);
            report.DataSource = myDataSource;
            TranseTitle(report);
            report.Parameters[0].Value = SystemResources.Instance.CurrentUserName;
            if (report.Parameters != null && Parameters != null)
                for (int i = 0; i < Parameters.Length && i < report.Parameters.Count - 1; i++)
                    report.Parameters[i + 1].Value = Parameters[i];
            DocumentPreviewWindow win = new DocumentPreviewWindow();
            win.PreviewControl.DocumentSource = report;
            report.CreateDocument();
            win.Owner = this;
            win.ShowDialog();
        }
        /// <summary>
        /// 翻译报表标题
        /// </summary>
        /// <param name="report"></param>
        void TranseTitle(XtraReport report)
        {
            //报表每一个带区
            for (int i = 0; i < report.Bands.Count; i++)
            {
                if (report.Bands[i] == null)
                    continue;
                //每个带区中的每一个控件
                Translate(report.Bands[i]);
            }
        }
        private void Translate(XRControl control)
        {
            string langID, langValue;
            foreach (XRControl label in control.Controls)
            {
                if (label.GetType().BaseType == typeof(Band))
                {
                    Translate(label);
                }
                else
                {
                    langID = label.Tag.ToString();
                    if (langID == "") continue;

                    langValue = SystemResources.Instance.LanguageArray[int.Parse(langID)];
                    if (langValue == "") continue;
                    if (langID == "2162")
                    {
                        label.Text = "";
                    }
                    else
                    {
                        label.Text = langValue;
                    }
                }
            }
        }
        /// <summary>
        /// 校验配置文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool validate(string path)
        {
            string fullpath = MapPath.PrintTempletPath + path;

            if (new System.IO.FileInfo(fullpath).Exists == false)
            {
                NotificationService.Instance.ShowError(SystemResources.Instance.LanguageArray[2692]);//文件不存在
                return false;
            }
            return true;
        }
    }
}
