using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using Sinboda.Framework.Common.FileOperateHelper;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing.Printing;
using System.IO;
using System.Management;

namespace Sinboda.Framework.Print
{
    /// <summary>
    /// 打印实现类
    /// </summary>
    public class SinReport
    {
        /// <summary>
        /// 打印模版初始化
        /// </summary>
        private XtraReport report = new XtraReport();
        /// <summary>
        /// 
        /// </summary>
        public PrintAction Action { get; private set; }
        /// <summary>
        /// 调用打印接口
        /// </summary>
        /// <param name="pageTitle"></param>
        /// <param name="DataSource"></param>
        /// <param name="FileName"></param>
        /// <param name="parameters"></param>
        /// <param name="autoPrint"></param>
        /// <param name="isPrintPreview">是否预览</param>
        /// <param name="bOnlyPrintResult">是否只打印结果</param>
        public SinReport(string pageTitle, object DataSource, string FileName, object[] parameters = null,
            bool autoPrint = false, bool isPrintPreview = false, bool bOnlyPrintResult = true)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                //获取权限，当为研发权限时进行调用设计与效果接口，当为普通权限时进行调用打印接口
                if (string.IsNullOrEmpty(SystemResources.Instance.CurrentUserName))
                {
                    if (isPrintPreview)
                        PrintPreview(bOnlyPrintResult, DataSource, FileName, parameters, autoPrint);
                    else
                        Report(bOnlyPrintResult, DataSource, FileName, parameters, autoPrint);
                }
                else
                {
                    //此为研发权限
                    if (SystemResources.Instance.CurrentUserName == "dryf")
                    {
                        Designer d = new Designer(DataSource, FileName, null);
                        d.ShowInTaskbar = true;
                        d.ShowDialog();
                    }
                    else
                    {
                        if (isPrintPreview)
                            PrintPreview(bOnlyPrintResult, DataSource, FileName, parameters, autoPrint);
                        else
                            Report(bOnlyPrintResult, DataSource, FileName, parameters, autoPrint);
                    }
                }
            }
        }

        /// <summary>
        /// 打印预览
        /// </summary>
        /// <param name="bOnlyPrintResult">纸打印结果</param>
        /// <param name="DataSource"></param>
        /// <param name="FileName"></param>
        /// <param name="parameters"></param>
        /// <param name="autoPrint"></param>
        private void PrintPreview(bool bOnlyPrintResult, object DataSource, string FileName, object[] parameters = null, bool autoPrint = false)
        {
            PubPrint(bOnlyPrintResult, DataSource, FileName, parameters, autoPrint);
            DocumentPreviewWindow win = new DocumentPreviewWindow();
            win.ShowInTaskbar = true;
            win.PreviewControl.DocumentSource = report;
            report.CreateDocument();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.ShowDialog();
        }
        /// <summary>
        /// 调用打印
        /// </summary>
        /// <param name="bOnlyPrintResult">只打印结果</param>
        /// <param name="DataSource"></param>
        /// <param name="FileName"></param>
        /// <param name="parameters"></param>
        /// <param name="autoPrint"></param>
        private void Report(bool bOnlyPrintResult, object DataSource, string FileName, object[] parameters = null, bool autoPrint = false)
        {
            PubPrint(bOnlyPrintResult, DataSource, FileName, parameters, autoPrint);
            if (autoPrint)
            {
                report.Print();
            }
            else
            {
                DocumentPreviewWindow win = new DocumentPreviewWindow();
                win.ShowInTaskbar = true;
                win.PreviewControl.DocumentSource = report;
                report.CreateDocument();
                win.Owner = System.Windows.Application.Current.MainWindow;
                win.ShowDialog();
            }
        }
        /// <summary>
        /// 公共打印部分提取
        /// </summary>
        /// <param name="bOnlyPrintResult">只打印结果，不打印标题和注脚</param>
        /// <param name="DataSource"></param>
        /// <param name="FileName"></param>
        /// <param name="parameters"></param>
        /// <param name="autoPrint"></param>
        private void PubPrint(bool bOnlyPrintResult, object DataSource, string FileName, object[] parameters = null, bool autoPrint = false)
        {
            if (!Validate(ref FileName))
            {
                NotificationService.Instance.ShowMessage(string.Format(SystemResources.Instance.LanguageArray[3120], FileName));//29提示 3120 配置文件异常
                return;
            }
            Action = PrintAction.PrintToPreview;
            report.PrintProgress += Report_PrintProgress;
            report.LoadLayout(MapPath.PrintTempletPath + FileName);
            report.DataSource = DataSource;
            if (!bOnlyPrintResult || (MapPath.PrintTempletPath + FileName).ToLower().Contains("result"))
            {
                string path = MapPath.XmlPath + "REPORT_CONFIG.xml";
                FileHelper _help = new FileHelper();
                if (!File.Exists(path))
                {
                    ReportSettingModel _writeModel = new ReportSettingModel()
                    {
                        FirstName = "6364",//StringResourceExtension.GetLanguage(6364, "××医院"),
                        SecondName = "6365",// StringResourceExtension.GetLanguage(208, "××检验科"),
                        UseEndnotes = "0",
                        Endnotes1 = "6366",//StringResourceExtension.GetLanguage(6366, "本报告单仅对该样本有效"),
                        Endnotes2 = "6367",//StringResourceExtension.GetLanguage(6367, "联系地址：××联系电话：××"),
                        AutoPrint = "1",
                        PrintAudit = "1",
                        PrintPatient = "1",
                        ResultFlag = "-1",
                        HighFlag = "↑|H",
                        LowFlag = "↓|L",
                    };
                    _help.SaveXML<ReportSettingModel>(_writeModel, path);
                }
                else
                {
                    var _reportModel = _help.ReadXML<ReportSettingModel>(path);
                    int returnValue = 0;
                    _reportModel.FirstName = int.TryParse(_reportModel.FirstName, out returnValue) == true ? StringResourceExtension.GetLanguage(6364, "××医院") : _reportModel.FirstName;
                    _reportModel.SecondName = int.TryParse(_reportModel.SecondName, out returnValue) == true ? StringResourceExtension.GetLanguage(6365, "××检验科") : _reportModel.SecondName;
                    _reportModel.Endnotes1 = int.TryParse(_reportModel.Endnotes1, out returnValue) == true ? StringResourceExtension.GetLanguage(6366, "本报告单仅对该样本有效") : _reportModel.Endnotes1;
                    _reportModel.Endnotes2 = int.TryParse(_reportModel.Endnotes2, out returnValue) == true ? StringResourceExtension.GetLanguage(6367, "联系地址 : ××联系电话 : ××") : _reportModel.Endnotes2;
                    XRControl title11 = report.FindControl("title11", false);
                    if (title11 != null)
                    {
                        report.Parameters["FirstTitle"].Value = _reportModel.FirstName;
                    }
                    XRControl title22 = report.FindControl("title22", false);
                    if (title22 != null)
                    {
                        report.Parameters["SecTitle"].Value = _reportModel.SecondName;
                    }

                    bool isFootVisible = _reportModel.UseEndnotes == "1";

                    XRControl foot11 = report.FindControl("foot11", false);
                    if (foot11 != null && isFootVisible)
                    {
                        report.Parameters["Foot1"].Value = _reportModel.Endnotes1;
                        foot11.Visible = true;
                    }
                    if (foot11 != null && !isFootVisible)
                    {
                        foot11.Visible = false;
                    }
                    XRControl foot22 = report.FindControl("foot22", false);
                    if (foot22 != null && isFootVisible)
                    {
                        report.Parameters["Foot2"].Value = _reportModel.Endnotes2;
                        foot22.Visible = true;
                    }
                    if (foot22 != null && !isFootVisible)
                    {
                        foot22.Visible = false;
                    }
                }
            }
            TranseTitle(report, 0);
            if (report.Parameters.Count != 0)
                report.Parameters[0].Value = SystemResources.Instance.CurrentUserName;
            if (report.Parameters != null && parameters != null)
                for (int i = 0; i < parameters.Length && i < report.Parameters.Count - 1; i++)
                    report.Parameters[i + 1].Value = parameters[i];

            //if (IsReg == 1 || IsReg == 2) // 注册或试用期内不显示水印
            //{
            //    report.Watermark.Text = "";
            //}
            //else
            //{
                report.Watermark.ShowBehind = false;
                report.Watermark.Text = SystemResources.Instance.LanguageArray[2162];//"非正式报告单";
            //}
        }
        /// <summary>
        /// 批量打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Report_PrintProgress(object sender, DevExpress.XtraPrinting.PrintProgressEventArgs e)
        {
            Action = e.PrintAction;
        }
        /// <summary>
        /// 翻译报表标题
        /// </summary>
        /// <param name="report"></param>
        /// <param name="IsRegist"></param>
        private void TranseTitle(XtraReport report, Int16 IsRegist)
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
        private bool Validate(ref string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                FileHelper _helper = new FileHelper();
                string xmlpath = MapPath.XmlPath + "REPORT_CONFIG.XML";
                if (!File.Exists(xmlpath))
                {
                    return false;
                }
                ReportSettingModel _model = _helper.ReadXML<ReportSettingModel>(xmlpath);
            }
            return true;
        }

        /// <summary>
        /// 获得当前默认选中的模版路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string GetPrintTemplatePath(string path, string language = "")
        {
            string strLanguage = !string.IsNullOrEmpty(language) ? language : SystemResources.Instance.CurrentLanguage;

            string xmlPath = MapPath.XmlPath + "REPORT_MASTERPLATE.xml";
            //加载XML文件进来
            string rtpath = string.Empty;

            // 获取节点
            XmlNode xmlNode = XMLHelper.GetNodeByByAttr(xmlPath, strLanguage, path, XMLHelper.ATTR_CODE);

            if (null != xmlNode && xmlNode.Attributes.Count > 0 && null != xmlNode.Attributes.GetNamedItem(XMLHelper.ATTR_DIR))
            {
                rtpath = xmlNode.Attributes.GetNamedItem(XMLHelper.ATTR_DIR).Value + @"\";
            }

            if (xmlNode != null)
            {
                foreach (XmlNode item in xmlNode.ChildNodes)
                {
                    if (0 < item.Attributes.Count)
                    {
                        if (item.Attributes.GetNamedItem(XMLHelper.ATTR_SEL).Value == "true")
                        {
                            rtpath = rtpath + item.Attributes.GetNamedItem(XMLHelper.ATTR_FILE).Value;
                            break;
                        }
                    }
                }
            }
            return rtpath;
        }

        /// <summary>  
        /// 获取CpuID  
        /// </summary>  
        /// <returns>CpuID</returns>  
        public static string GetCpuID()
        {
            try
            {
                string strCpuID = string.Empty;
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    strCpuID = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return strCpuID;
            }
            catch
            {
                return "unknown";
            }
        }
    }
}
