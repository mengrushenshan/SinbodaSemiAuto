using Sinboda.Framework.Common.FileOperateHelper;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Common;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Control.Controls;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Reports.UserDesigner;
using Sinboda.Framework.Control.Loading;
using Sinboda.Framework.Core.Enums;

namespace Sinboda.Framework.Print
{
    /// <summary>
    /// ReportSetting.xaml 的交互逻辑
    /// </summary>
    public partial class ReportSetting : UserControl
    {
        /// <summary>
        /// 打印设置配置文件存储
        /// </summary>
        string printConfigPath = MapPath.XmlPath + @"REPORT_CONFIG.xml";
        /// <summary>
        /// 打印模版配置文件存储
        /// </summary>
        string reportConfigPath = MapPath.XmlPath + @"REPORT_MASTERPLATE.xml";
        /// <summary>
        /// 打印模版选择备份位置
        /// </summary>
        string reportSelectPath = MapPath.XmlPath + @"REPORT_DEFAULT_SELECT.xml";
        /// <summary>
        /// 默认模版
        /// </summary>
        string defaultReport = string.Empty;
        /// <summary>
        /// 报告单数据源
        /// </summary>
        private List<PropertyNodeItem> itemList;
        /// <summary>
        /// 基础接口声明
        /// </summary>
        FileHelper _helper = new FileHelper();
        /// <summary>
        /// 
        /// </summary>
        public ReportSetting()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Reload();
                AutoPrint_Click(null, null);
            };
        }

        /// <summary>
        /// 选择模版勾选事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void check_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            PropertyNodeItem checkName = (PropertyNodeItem)checkBox.DataContext;

            foreach (PropertyNodeItem item1 in itemList)
            {
                if (item1.NodeCode == checkName.ParentName)
                {
                    foreach (PropertyNodeItem item2 in item1.Children)
                    {
                        if (item2.NodeCode == checkName.NodeCode)
                            item2.IsChecked = true;
                        else
                            item2.IsChecked = false;
                    }
                }
            }
            tree.ItemsSource = null;
            tree.ItemsSource = itemList;
        }
        /// <summary>
        /// 从配置文件加载路径及构建界面显示
        /// </summary>
        private void XmlToTreeView()
        {
            itemList = new List<PropertyNodeItem>();
            //加载XML文件进来
            // 判断语言类型,获取对应语言的报告单
            string languageType = SystemResources.Instance.CurrentLanguage;

            XmlNode nodeValue = XMLHelper.GetNodeByByAttr(reportConfigPath, languageType, null, XMLHelper.ATTR_CODE);
            if (null == nodeValue || false == nodeValue.HasChildNodes)
            {
                tree.ItemsSource = itemList;
                return;
            }

            XmlNodeList nodeList = nodeValue.ChildNodes;
            if (null == nodeList)
            {
                tree.ItemsSource = itemList;
                return;
            }

            // 创建界面显示
            foreach (XmlNode item in nodeList)
            {
                // 填充一级节点
                if (item.Attributes.Count <= 0)
                {
                    continue;
                }

                if (null == item.Attributes.GetNamedItem(XMLHelper.ATTR_CODE) ||
                    null == item.Attributes.GetNamedItem(XMLHelper.ATTR_NAME) ||
                    null == item.Attributes.GetNamedItem(XMLHelper.ATTR_DIR) ||
                    null == item.Attributes.GetNamedItem(XMLHelper.ATTR_ISSHOW))
                {
                    continue;
                }

                if (item.Attributes.GetNamedItem(XMLHelper.ATTR_ISSHOW).Value != "true")
                {
                    continue;
                }

                string strName = item.Attributes.GetNamedItem(XMLHelper.ATTR_NAME).Value;
                int nDisplayCode = 0;
                if (int.TryParse(strName, out nDisplayCode))
                {
                    strName = SystemResources.Instance.LanguageArray[nDisplayCode];
                }

                PropertyNodeItem firstNode = new PropertyNodeItem()
                {
                    DisplayName = strName,
                    NodeCode = item.Attributes.GetNamedItem(XMLHelper.ATTR_CODE).Value,
                    FileName = string.Empty,
                    FileDir = item.Attributes.GetNamedItem(XMLHelper.ATTR_DIR).Value,
                    IsVisibility = Visibility.Collapsed
                };

                //填充二级节点
                if (false == item.HasChildNodes)
                {
                    continue;
                }

                //加载所有目录下的文件
                XmlNodeList secNodeList = item.ChildNodes;
                foreach (XmlNode childItem in secNodeList)
                {
                    if (childItem.Attributes.Count <= 0)
                    {
                        continue;
                    }

                    if (null == childItem.Attributes.GetNamedItem(XMLHelper.ATTR_CODE) ||
                        null == childItem.Attributes.GetNamedItem(XMLHelper.ATTR_NAME) ||
                        null == childItem.Attributes.GetNamedItem(XMLHelper.ATTR_FILE) ||
                        null == childItem.Attributes.GetNamedItem(XMLHelper.ATTR_SEL))
                    {
                        continue;
                    }

                    PropertyNodeItem childNode = new PropertyNodeItem();
                    childNode.NodeCode = childItem.Attributes.GetNamedItem(XMLHelper.ATTR_CODE).Value;

                    strName = childItem.Attributes.GetNamedItem(XMLHelper.ATTR_NAME).Value;
                    if (int.TryParse(strName, out nDisplayCode))
                    {
                        strName = SystemResources.Instance.LanguageArray[nDisplayCode];
                    }

                    childNode.DisplayName = strName;
                    childNode.FileName = childItem.Attributes.GetNamedItem(XMLHelper.ATTR_FILE).Value;
                    childNode.FileDir = item.Attributes.GetNamedItem(XMLHelper.ATTR_DIR).Value;
                    childNode.ParentName = firstNode.NodeCode;

                    childNode.IsVisibility = Visibility.Visible;
                    if ("true" == childItem.Attributes.GetNamedItem(XMLHelper.ATTR_SEL).Value)
                    { childNode.IsChecked = true; }
                    else
                    { childNode.IsChecked = false; }
                    firstNode.Children.Add(childNode);
                }

                itemList.Add(firstNode);
            }

            tree.ItemsSource = itemList;
        }
        /// <summary>
        /// 比较本地目录和xml配置文件，如不一致，按照本地目录实际情况修正配置文件
        /// </summary>
        private void DiffDirAndXmlReportInfo()
        {
            string languageType = SystemResources.Instance.CurrentLanguage;

            // 获取xml配置信息
            XmlNode nodeValue = XMLHelper.GetNodeByByAttr(reportConfigPath, languageType, null, XMLHelper.ATTR_CODE);
            if (null == nodeValue || false == nodeValue.HasChildNodes)
            {
                return;
            }

            XmlNodeList nodeList = nodeValue.ChildNodes;
            if (null == nodeList)
            {
                return;
            }

            foreach (XmlNode item in nodeList)
            {
                if (item.Attributes.Count <= 0)
                {
                    continue;
                }

                if (null == item.Attributes.GetNamedItem(XMLHelper.ATTR_DIR) ||
                    null == item.Attributes.GetNamedItem(XMLHelper.ATTR_CODE))
                {
                    continue;
                }

                // 获取本地目录文件
                string path = MapPath.PrintTempletPath + item.Attributes.GetNamedItem(XMLHelper.ATTR_DIR).Value;
                Dictionary<string, Operate_Type> dirFileList = GetFileListFromDir(path);

                // 比较xml文件和本地目录是否一致，不一致需处理一致
                if (true == item.HasChildNodes)
                {
                    // 判断文件是否在XML中存在节点
                    foreach (XmlNode childItem in item.ChildNodes)
                    {
                        if (childItem.Attributes.Count <= 0)
                        {
                            continue;
                        }

                        if (null == childItem.Attributes.GetNamedItem(XMLHelper.ATTR_FILE) ||
                            null == childItem.Attributes.GetNamedItem(XMLHelper.ATTR_CODE))
                        {
                            continue;
                        }

                        string nodFile = childItem.Attributes.GetNamedItem(XMLHelper.ATTR_FILE).Value;
                        if (dirFileList.ContainsKey(nodFile))
                        {
                            // 配置文件有，本地目录也有报告单，不需要处理
                            dirFileList[nodFile] = Operate_Type.Operate_Type_None;
                        }
                        else
                        {
                            // 配置文件有配置，但是本地目录没有报告单，需要将配置文件配置信息删除
                            dirFileList[nodFile] = Operate_Type.Operate_Type_Delete;
                            XMLHelper.DeleteNodeByAttr(reportConfigPath, languageType, childItem.Attributes.GetNamedItem(XMLHelper.ATTR_CODE).Value, XMLHelper.ATTR_CODE);
                        }
                    }
                }

                InsertReportNodeToXml(languageType, item.Attributes.GetNamedItem(XMLHelper.ATTR_CODE).Value, dirFileList);
            }
        }
        /// <summary>
        /// 插入节点信息到配置文件
        /// </summary>
        private void InsertReportNodeToXml(string languageType, string nodeCode, Dictionary<string, Operate_Type> dirFileList)
        {
            foreach (var fileItem in dirFileList)
            {
                if (fileItem.Value == Operate_Type.Operate_Type_Insert)
                {
                    // 插入节点到XML
                    string[] tmpStr = fileItem.Key.Split('.');
                    if (tmpStr == null || tmpStr.Length == 0)
                    {
                        continue;
                    }

                    Dictionary<string, string> addAttr = new Dictionary<string, string>();
                    addAttr.Add(XMLHelper.ATTR_CODE, tmpStr[0]);
                    addAttr.Add(XMLHelper.ATTR_NAME, tmpStr[0]);
                    addAttr.Add(XMLHelper.ATTR_FILE, fileItem.Key);
                    addAttr.Add(XMLHelper.ATTR_SEL, "false");
                    XMLHelper.InsertNode2XmlByAttr(reportConfigPath, languageType, nodeCode, XMLHelper.ATTR_CODE, "Report", addAttr);
                }
            }
        }

        /// <summary>
        /// 获取本地目录打印报告的文件信息
        /// </summary>
        private Dictionary<string, Operate_Type> GetFileListFromDir(string path)
        {
            Dictionary<string, Operate_Type> fileList = new Dictionary<string, Operate_Type>();

            if (!Directory.Exists(path))
            {
                return fileList;
            }

            string[] fileArray = Directory.GetFiles(path);

            if (null == fileArray || fileArray.Length <= 0)
            {
                return fileList;
            }

            for (int i = 0; i < fileArray.Length; i++)
            {
                string[] tmpStrArray = fileArray[i].Split('\\');
                if (tmpStrArray != null && 0 < tmpStrArray.Length)
                {
                    fileList.Add(tmpStrArray[tmpStrArray.Length - 1], Operate_Type.Operate_Type_Insert);
                }
            }

            return fileList;
        }

        /// <summary>
        /// 重新加载数据
        /// </summary>
        public void Reload()
        {
            // 检测实际目录文件是否存储于配置文件，如果未存储，需要写入配置文件
            DiffDirAndXmlReportInfo();
            // 将XMl文件映射到界面显示
            XmlToTreeView();
            //用户设置
            if (!File.Exists(printConfigPath))
            {
                CreateXmlConfig();
                ReadDataFromXml();
            }
            else
            {
                ReadDataFromXml();
            }
        }
        /// <summary>
        /// 初始化加载配置文件中信息，若不存在则使用创建默认信息
        /// </summary>
        private void ReadDataFromXml()
        {
            if (File.Exists(printConfigPath))
            {
                ReportSettingModel _model = null;
                try
                {
                    _model = _helper.ReadXML<ReportSettingModel>(printConfigPath);
                    int returnValue = 0;
                    _model.FirstName = int.TryParse(_model.FirstName, out returnValue) == true ? StringResourceExtension.GetLanguage(6364, "××医院") : _model.FirstName;
                    _model.SecondName = int.TryParse(_model.SecondName, out returnValue) == true ? StringResourceExtension.GetLanguage(6365, "××检验科") : _model.SecondName;
                    _model.Endnotes1 = int.TryParse(_model.Endnotes1, out returnValue) == true ? StringResourceExtension.GetLanguage(6366, "本报告单仅对该样本有效") : _model.Endnotes1;
                    _model.Endnotes2 = int.TryParse(_model.Endnotes2, out returnValue) == true ? StringResourceExtension.GetLanguage(6367, "联系地址: ××联系电话 : ××") : _model.Endnotes2;
                }
                catch //和原有模板序列化方式不一致时初始化模板
                {
                    CreateXmlConfig();
                    _model = _helper.ReadXML<ReportSettingModel>(printConfigPath);
                    int returnValue = 0;
                    _model.FirstName = int.TryParse(_model.FirstName, out returnValue) == true ? StringResourceExtension.GetLanguage(6364, "××医院") : _model.FirstName;
                    _model.SecondName = int.TryParse(_model.SecondName, out returnValue) == true ? StringResourceExtension.GetLanguage(6365, "××检验科") : _model.SecondName;
                    _model.Endnotes1 = int.TryParse(_model.Endnotes1, out returnValue) == true ? StringResourceExtension.GetLanguage(6366, "本报告单仅对该样本有效") : _model.Endnotes1;
                    _model.Endnotes2 = int.TryParse(_model.Endnotes2, out returnValue) == true ? StringResourceExtension.GetLanguage(6367, "联系地址 : ××联系电话 : ××") : _model.Endnotes2;
                    LogHelper.logSoftWare.Error("ReadDataFromXML CreatXML And Read again");
                }
                txtName1.Text = _model.FirstName;
                txtName2.Text = _model.SecondName;
                chkEndNote.IsChecked = _model.UseEndnotes == "1";
                txtEndNote1.Text = _model.Endnotes1;
                txtEndNote2.Text = _model.Endnotes2;
                chkAutoPrint.IsChecked = _model.AutoPrint == "1";
                chkAudit.IsChecked = _model.PrintAudit == "1";
                chkPatient.IsChecked = _model.PrintPatient == "1";

                SystemResources.Instance.IsAutoPrint = _model.AutoPrint == "1";
                SystemResources.Instance.IsAutoPrintByAudit = _model.PrintAudit == "1";
                SystemResources.Instance.IsAutoPrintByName = _model.PrintPatient == "1";

                //结果标识是否显示
                bool visible = int.Parse(_model.ResultFlag) != -1;
                if (visible)
                {
                    ResultFlagGrid.Visibility = Visibility.Visible;
                    Grid.SetRow(ResultFlagGrid, 9);
                    Grid.SetRow(CurrentPrintTemplate, 10);
                    List<InitDataGrid> ResultFlagList = new List<InitDataGrid>();

                    string[] markHighFlags = _model.HighFlag.Split('|');
                    string[] markLowFlags = _model.LowFlag.Split('|');
                    int loop = markHighFlags.Length < markLowFlags.Length ? markHighFlags.Length : markLowFlags.Length;
                    for (int i = 0; i < loop; i++)
                    {
                        InitDataGrid LookupItem1 = new InitDataGrid();
                        LookupItem1.ShowName = markHighFlags[i] + "/" + markLowFlags[i];
                        LookupItem1.ID = i.ToString();
                        ResultFlagList.Add(LookupItem1);
                    }

                    lueResultFlag.ItemsSource = ResultFlagList;
                    lueResultFlag.SelectedIndex = int.Parse(_model.ResultFlag);

                    if (ResultFlagList.Count > lueResultFlag.SelectedIndex)
                    {
                        InitDataGrid info = ResultFlagList[lueResultFlag.SelectedIndex];
                        SystemResources.Instance.PrintTemplateHighMark = info.ShowName.Split('/')[0];
                        SystemResources.Instance.PrintTemplateLowMark = info.ShowName.Split('/')[1];
                    }

                    FlagContentValue.Content = SystemResources.Instance.LanguageArray[3981];// "结果阳性标识";
                }
                else
                {
                    FlagContentValue.Content = SystemResources.Instance.LanguageArray[1980];// "结果阳性/阴性标识";

                    ResultFlagGrid.Visibility = Visibility.Collapsed;
                    Grid.SetRow(CurrentPrintTemplate, 9);
                    lueResultFlag.SelectedIndex = -1;
                }
            }
        }

        /// <summary>
        /// 创建默认配置文件信息
        /// </summary>
        private void CreateXmlConfig()
        {
            ReportSettingModel _writeModel = new ReportSettingModel()
            {
                FirstName = "6364",//StringResourceExtension.GetLanguage(6364, "××医院"),
                SecondName = "6365",// StringResourceExtension.GetLanguage(208, "××检验科"),
                UseEndnotes = "0",
                Endnotes1 = "6366",//StringResourceExtension.GetLanguage(6366, "本报告单仅对该样本有效"),
                Endnotes2 = "6367",//StringResourceExtension.GetLanguage(6367, "联系地址 : ××联系电话 : ××"),
                AutoPrint = "1",
                PrintAudit = "1",
                PrintPatient = "1",
                ResultFlag = "0",
                HighFlag = "↑|H",
                LowFlag = "↓|L",
            };
            _helper.SaveXML<ReportSettingModel>(_writeModel, printConfigPath);
        }
        /// <summary>
        /// 写数据到xml文件
        /// </summary>
        private bool WriteDataToXml()
        {
            string curLanguage = SystemResources.Instance.CurrentLanguage;
            bool isSucc = false;
            if (File.Exists(printConfigPath))
            {
                ReportSettingModel _model = _helper.ReadXML<ReportSettingModel>(printConfigPath);
                _model.FirstName = txtName1.Text == "××医院" ? "6364" : txtName1.Text;
                _model.SecondName = txtName2.Text == "××检验科" ? "6365" : txtName2.Text;
                _model.UseEndnotes = chkEndNote.IsChecked == true ? "1" : "0";
                _model.Endnotes1 = txtEndNote1.Text == "本报告单仅对该样本有效" ? "6366" : txtEndNote1.Text;
                _model.Endnotes2 = txtEndNote2.Text == "联系地址 : ××联系电话 : ××" ? "6367" : txtEndNote2.Text;
                _model.AutoPrint = chkAutoPrint.IsChecked == true ? "1" : "0";
                _model.PrintAudit = chkAudit.IsChecked == true ? "1" : "0";
                _model.PrintPatient = chkPatient.IsChecked == true ? "1" : "0";

                SystemResources.Instance.IsAutoPrint = _model.AutoPrint == "1";
                SystemResources.Instance.IsAutoPrintByAudit = _model.PrintAudit == "1";
                SystemResources.Instance.IsAutoPrintByName = _model.PrintPatient == "1";

                _model.ResultFlag = lueResultFlag.SelectedIndex.ToString();

                if (lueResultFlag != null)
                {
                    var ResultFlagList = lueResultFlag.ItemsSource as List<InitDataGrid>;
                    if (ResultFlagList != null)
                    {
                        if (ResultFlagList.Count > lueResultFlag.SelectedIndex)
                        {
                            InitDataGrid info = ResultFlagList[lueResultFlag.SelectedIndex];
                            SystemResources.Instance.PrintTemplateHighMark = info.ShowName.Split('/')[0];
                            SystemResources.Instance.PrintTemplateLowMark = info.ShowName.Split('/')[1];
                        }
                    }
                }

                if (_helper.SaveXML<ReportSettingModel>(_model, printConfigPath))
                    isSucc = true;
                else isSucc = false;
            }
            if (File.Exists(reportConfigPath))
            {
                foreach (PropertyNodeItem it1 in itemList)
                {
                    foreach (PropertyNodeItem it2 in it1.Children)
                    {
                        if (it2.IsChecked == true)
                        {
                            if (XMLHelper.UpdateXmlAttributeByPath(reportConfigPath, curLanguage.ToUpper(), it2.NodeCode, XMLHelper.ATTR_CODE, XMLHelper.ATTR_SEL, "true"))
                            {
                                XMLHelper.UpdateXmlNodeByXPath(reportSelectPath, "", it1.NodeCode, it2.NodeCode);
                                isSucc = true;
                            }
                            else
                            {
                                isSucc = false;
                            }
                        }
                        else
                        {
                            if (XMLHelper.UpdateXmlAttributeByPath(reportConfigPath, curLanguage.ToUpper(), it2.NodeCode, XMLHelper.ATTR_CODE, XMLHelper.ATTR_SEL, "false"))
                            {
                                isSucc = true;
                            }
                            else
                            {
                                isSucc = false;
                            }
                        }
                    }
                }
            }
            return isSucc;
        }
        /// <summary>
        /// 保存按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInput())
            {
                if (WriteDataToXml())
                {
                    tree_SelectedItemChanged(this, null);
                    SystemResources.Instance.SysLogInstance.WriteLogDb(string.Format("{0} : {1}", SystemResources.Instance.LanguageArray[15], SystemResources.Instance.LanguageArray[609]), SysLogType.Operater);//29提示609保存成功 //TODO 翻译
                    NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[609], MessageBoxButton.OK, SinMessageBoxImage.Completed);//29提示609保存成功！
                }
                else
                {
                    NotificationService.Instance.ShowError(SystemResources.Instance.LanguageArray[610]);//29提示610保存失败！
                }
            }
            else
            {

            }
        }

        private bool CheckInput()
        {
            try
            {
                Convert.ToDouble(txtName1.Text);
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(13184, "标题不能输入纯数字"));
                return false;
            }
            catch
            { }
            try
            {
                Convert.ToDouble(txtName2.Text);
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(13184, "标题不能输入纯数字"));
                return false;
            }
            catch
            { }
            try
            {
                Convert.ToDouble(txtEndNote1.Text);
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(13185, "尾注不能输入纯数字"));
                return false;
            }
            catch
            { }
            try
            {
                Convert.ToDouble(txtEndNote2.Text);
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(13185, "尾注不能输入纯数字"));
                return false;
            }
            catch
            { }
            return true;
        }

        /// <summary>
        /// 编辑模版按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            XtraReport report = new XtraReport();
            if ((PropertyNodeItem)tree.SelectedItem != null)
            {
                PropertyNodeItem item = (PropertyNodeItem)tree.SelectedItem;
                if (item.FileName.ToLower().Contains(".repx"))
                {
                    report.LoadLayout(MapPath.PrintTempletPath + item.FileDir + @"\" + item.FileName);
                }
                TranseTitle(report);
                ReportDesigner designer = new ReportDesigner();
                designer.DocumentSource = report;
                designer.ShowWindow(this);
            }
            else
            {
                NotificationService.Instance.ShowMessage(StringResourceExtension.GetLanguage(29, "提示"), StringResourceExtension.GetLanguage(6368, "请先选择模版再进行编辑"), MessageBoxButton.OK, SinMessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// 初始化dev打印模版
        /// </summary>
        XtraReport fXtraReport = new XtraReport();
        /// <summary>
        /// 显示模版信息
        /// </summary>
        /// <param name="path"></param>
        void show(string path)
        {
            if (File.Exists(printConfigPath))
            {
                var _reportModel = _helper.ReadXML<ReportSettingModel>(printConfigPath);
                int returnValue = 0;
                _reportModel.FirstName = int.TryParse(_reportModel.FirstName, out returnValue) == true ? StringResourceExtension.GetLanguage(6364, "××医院") : _reportModel.FirstName;
                _reportModel.SecondName = int.TryParse(_reportModel.SecondName, out returnValue) == true ? StringResourceExtension.GetLanguage(6365, "××检验科") : _reportModel.SecondName;
                _reportModel.Endnotes1 = int.TryParse(_reportModel.Endnotes1, out returnValue) == true ? StringResourceExtension.GetLanguage(6366, "本报告单仅对该样本有效") : _reportModel.Endnotes1;
                _reportModel.Endnotes2 = int.TryParse(_reportModel.Endnotes2, out returnValue) == true ? StringResourceExtension.GetLanguage(6367, "联系地址 : ××联系电话 : ××") : _reportModel.Endnotes2;
                fXtraReport.LoadLayout(path);
                XRControl title11 = fXtraReport.FindControl("title11", false);
                if (title11 != null)
                {
                    fXtraReport.Parameters["FirstTitle"].Value = _reportModel.FirstName;
                }
                XRControl title22 = fXtraReport.FindControl("title22", false);
                if (title22 != null)
                {
                    fXtraReport.Parameters["SecTitle"].Value = _reportModel.SecondName;
                }

                bool isFootVisible = _reportModel.UseEndnotes == "1";

                XRControl foot11 = fXtraReport.FindControl("foot11", false);
                if (foot11 != null && isFootVisible)
                {
                    fXtraReport.Parameters["Foot1"].Value = _reportModel.Endnotes1;
                    foot11.Visible = true;
                }
                else if (foot11 != null && !isFootVisible)
                {
                    foot11.Visible = false;
                }
                XRControl foot22 = fXtraReport.FindControl("foot22", false);
                if (foot22 != null && isFootVisible)
                {
                    fXtraReport.Parameters["Foot2"].Value = _reportModel.Endnotes2;
                    foot22.Visible = true;
                }
                else if (foot22 != null && !isFootVisible)
                {
                    foot22.Visible = false;
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    fXtraReport.SaveLayout(path);
                    TranseTitle(fXtraReport);
                    fXtraReport.CreateDocument();
                    host.DocumentSource = fXtraReport;
                });
            }
            else
            {
                NotificationService.Instance.ShowMessage(StringResourceExtension.GetLanguage(3120, "获取{0}配置文件异常，请查看后修复", "report_config.xml"));//29提示 3120 配置文件异常
            }
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
        /// 刷新按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        /// <summary>
        /// 自动打印按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoPrint_Click(object sender, RoutedEventArgs e)
        {
            if (chkAutoPrint.IsChecked == true)
            {
                rowPatient.MaxHeight = 30;
                rowAudit.MaxHeight = 30;
            }
            else
            {
                rowPatient.MaxHeight = 0;
                rowAudit.MaxHeight = 0;
            }
        }
        private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            PropertyNodeItem item = (PropertyNodeItem)tree.SelectedItem;
            if (item != null)
            {
                if (item.FileName.ToLower().Contains(".repx"))
                {
                    defaultReport = item.FileName;
                    LoadingHelper.Instance.ShowLoadingWindow(ancBegin =>
                    {
                        ancBegin.Title = "...";
                        show(MapPath.PrintTempletPath + item.FileDir + @"\" + item.FileName);
                    }, 0, ancBegin =>
                    {
                    });
                }
            }
        }
    }
    public class PropertyNodeItem
    {
        /// <summary>
        /// 父节点名称
        /// </summary>
        public string ParentName { get; set; }
        /// <summary>
        /// 节点ID
        /// </summary>
        public string NodeCode { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileDir { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded { get; set; }
        /// <summary>
        /// 是否显示CheckBox
        /// </summary>
        public Visibility IsVisibility { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PropertyNodeItem> Children { get; set; }
        public PropertyNodeItem()
        {
            Children = new List<PropertyNodeItem>();
        }
    }

    /// <summary>
    /// LookUpEdit下拉框
    /// </summary>
    public class InitDataGrid
    {
        /// <summary>
        /// 语言ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string ShowName { get; set; }
    }

    public enum Operate_Type
    {
        /// <summary>
        /// 不处理
        /// </summary>
        Operate_Type_None = 0,
        /// <summary>
        /// 插入
        /// </summary>
        Operate_Type_Insert,
        /// <summary>
        /// 删除
        /// </summary>
        Operate_Type_Delete,
    }
}
