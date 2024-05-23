using Sinboda.Framework.Common;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Control.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Sinboda.Framework.Control.GridColumnSetting
{
    /// <summary>
    /// GridColumnSetting.xaml 的交互逻辑
    /// </summary>
    public partial class GridColumnSetting : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsColumnWidthEditProperty = DependencyProperty.RegisterAttached("IsColumnWidthEdit", typeof(bool), typeof(GridColumnSetting), new PropertyMetadata(true, ColumnWidthEdit));

        private static void ColumnWidthEdit(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridColumnSetting control = d as GridColumnSetting;
            if ((bool)e.NewValue)
            {
                control.widthTextBlock.Visibility = Visibility.Visible;
                control.columnWidthTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                control.widthTextBlock.Visibility = Visibility.Hidden;
                control.columnWidthTextBox.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 所有列（包括显示列和可选列）
        /// </summary>
        private List<ColumnSetting> columnsSrouce = new List<ColumnSetting>();
        /// <summary>
        /// 显示列
        /// </summary>
        private ObservableCollection<ColumnSetting> columnsShowItemSrouce = new ObservableCollection<ColumnSetting>();
        /// <summary>
        /// 可选列
        /// </summary>
        private ObservableCollection<ColumnSetting> columnsHideItemSrouce = new ObservableCollection<ColumnSetting>();
        /// <summary>
        /// 配置文件中Grid的名称
        /// </summary>
        private string strGridName = string.Empty;
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private string strConifgPath = string.Empty;

        private ColumnSetting SelectedItem
        {
            get
            {
                ColumnSetting item = ShowGridColumnNameList.SelectedItem as ColumnSetting;
                return item;
            }
        }
        /// <summary>
        /// 必填项
        /// </summary>
        public bool IsColumnWidthEdit
        {
            get { return (bool)GetValue(IsColumnWidthEditProperty); }
            set { SetValue(IsColumnWidthEditProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public GridColumnSetting()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strGridName">配置文件中Grid的名称</param>
        /// <param name="columns">所有列</param>
        /// <param name="configPath">配置文件路径</param>
        public GridColumnSetting(string strGridName, List<ColumnSetting> columns, string configPath)
        {
            InitializeComponent();
            InitDataSource(strGridName, columns, configPath);
        }
        /// <summary>
        /// 初始化数据源
        /// </summary>
        /// <param name="strGridName">配置文件中Grid的名称</param>
        /// <param name="columns">所有列</param>
        /// <param name="configPath">配置文件路径</param>
        public void InitDataSource(string strGridName, List<ColumnSetting> columns, string configPath)
        {
            this.strGridName = strGridName;
            this.columnsSrouce = columns;
            this.strConifgPath = configPath;
            List<ColumnSetting> showConfigList = columnsSrouce.Where(o => o.IsVisible == true).OrderBy(o => o.ColumnIndex).ToList();
            foreach (ColumnSetting config in showConfigList)
            {
                columnsShowItemSrouce.Add(config);
            }

            List<ColumnSetting> hideConfigList = columnsSrouce.Where(o => o.IsVisible == false).ToList();
            foreach (ColumnSetting config in hideConfigList)
            {
                columnsHideItemSrouce.Add(config);
            }
            this.ShowGridColumnNameList.ItemsSource = this.columnsShowItemSrouce;
            this.HiddenGridColumnNameList.ItemsSource = this.columnsHideItemSrouce;
        }

        private void moveLeftBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShowGridColumnNameList.SelectedItem != null)
            {
                ColumnSetting item = ShowGridColumnNameList.SelectedItem as ColumnSetting;
                UpdateDataSource(item, false);
            }
        }

        private void moveRightBtn_Click(object sender, RoutedEventArgs e)
        {
            if (HiddenGridColumnNameList.SelectedItem != null)
            {
                ColumnSetting item = HiddenGridColumnNameList.SelectedItem as ColumnSetting;
                UpdateDataSource(item, true);
            }
        }

        private void moveUptBtn_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = this.ShowGridColumnNameList.SelectedIndex;
            if (currentIndex > 0)
            {
                this.columnsShowItemSrouce[currentIndex].ColumnIndex = currentIndex - 1;
                this.columnsShowItemSrouce[currentIndex - 1].ColumnIndex = currentIndex;
                ColumnSetting tempConfig = this.columnsShowItemSrouce[currentIndex - 1];
                this.columnsShowItemSrouce.Move(currentIndex, currentIndex - 1);
                this.ShowGridColumnNameList.SelectedIndex = currentIndex - 1;
            }
        }

        private void moveDownBtn_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = this.ShowGridColumnNameList.SelectedIndex;
            if (currentIndex < 0)
            {
                return;
            }
            if (currentIndex < this.columnsShowItemSrouce.Count - 1)
            {
                this.columnsShowItemSrouce[currentIndex].ColumnIndex = currentIndex + 1;
                this.columnsShowItemSrouce[currentIndex + 1].ColumnIndex = currentIndex;
                ColumnSetting tempConfig = this.columnsShowItemSrouce[currentIndex + 1];
                this.columnsShowItemSrouce.Move(currentIndex, currentIndex + 1);
                this.ShowGridColumnNameList.SelectedIndex = currentIndex + 1;
            }
        }
        private void HiddenGridColumnNameList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (HiddenGridColumnNameList.SelectedItem != null)
            {
                ColumnSetting item = HiddenGridColumnNameList.SelectedItem as ColumnSetting;
                UpdateDataSource(item, true);
                this.ShowGridColumnNameList.SelectedIndex = item.ColumnIndex;
            }
        }

        private void ShowGridColumnNameList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ShowGridColumnNameList.SelectedItem != null)
            {
                ColumnSetting item = ShowGridColumnNameList.SelectedItem as ColumnSetting;
                UpdateDataSource(item, false);
                this.HiddenGridColumnNameList.SelectedIndex = item.ColumnIndex;
            }
        }


        /// <summary>
        /// 更新界面ListBox数据源
        /// </summary>
        /// <param name="updateConfigItem">选中的item</param>
        /// <param name="itemVisible">是否可见</param>
        private void UpdateDataSource(ColumnSetting updateConfigItem, bool itemVisible)
        {
            try
            {
                updateConfigItem.IsVisible = itemVisible;
                if (itemVisible)
                {
                    for (int i = 0; i < this.columnsHideItemSrouce.Count; i++)
                    {
                        if (i > updateConfigItem.ColumnIndex)
                        {
                            this.columnsHideItemSrouce[i].ColumnIndex -= 1;
                        }
                    }
                    this.columnsHideItemSrouce.Remove(updateConfigItem);
                    updateConfigItem.ColumnIndex = this.columnsShowItemSrouce.Count;
                    this.columnsShowItemSrouce.Add(updateConfigItem);
                }
                else
                {
                    for (int i = 0; i < this.columnsShowItemSrouce.Count; i++)
                    {

                        if (i > updateConfigItem.ColumnIndex)
                        {
                            this.columnsShowItemSrouce[i].ColumnIndex -= 1;
                        }
                    }
                    this.columnsShowItemSrouce.Remove(updateConfigItem);
                    updateConfigItem.ColumnIndex = this.columnsHideItemSrouce.Count;
                    this.columnsHideItemSrouce.Add(updateConfigItem);
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, MessageBoxButton.OK, SinMessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 保存设置
        /// </summary>
        public void SaveGridColumnSetting()
        {
            GridConfigManager configManager = new GridConfigManager();
            try
            {
                configManager.SetCurrentConfig(strConifgPath, this.strGridName, this.columnsSrouce);
                ShowMessage(StringResourceExtension.GetLanguage(7390, "设置成功"), MessageBoxButton.OK, SinMessageBoxImage.Completed);
            }
            catch (Exception ex)
            {
                ShowMessage(StringResourceExtension.GetLanguage(369, "设置失败"), MessageBoxButton.OK, SinMessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 宽度赋值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowGridColumnNameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShowGridColumnNameList.SelectedItem != null)
            {
                ColumnSetting item = ShowGridColumnNameList.SelectedItem as ColumnSetting;
                this.columnWidthTextBox.DataContext = item;
            }
        }
        private int columnWidthInt = 0;
        /// <summary>
        /// 失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void columnWidthTextBox_LostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //if (ShowGridColumnNameList.SelectedItem != null)
            //{
            Button bt = e.NewFocus as Button;
            if (bt != null && bt.Tag != null && bt.Tag.ToString() == "save")
            {
                if (string.IsNullOrEmpty(columnWidthTextBox.Text))
                {
                    ShowMessage(StringResourceExtension.GetLanguage(50, "列宽度不能为空，请输入数字"), MessageBoxButton.OK, SinMessageBoxImage.Error);
                    columnWidthTextBox.Focus();
                }
                else if (!int.TryParse(columnWidthTextBox.Text, out columnWidthInt) || int.Parse(columnWidthTextBox.Text) < 5 || int.Parse(columnWidthTextBox.Text) > 999)
                {
                    ShowMessage(StringResourceExtension.GetLanguage(61, "请输入大于等于5的整数"), MessageBoxButton.OK, SinMessageBoxImage.Error);
                    columnWidthTextBox.Focus();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(columnWidthTextBox.Text))
                {
                    ColumnSetting item = ShowGridColumnNameList.SelectedItem as ColumnSetting;
                    if (item != null)
                    {
                        columnWidthTextBox.Text = item.ColumnWidth;
                    }
                }
            }
            //}
        }
        /// <summary>
        /// 限制输入，只能输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void columnWidthTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);

        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="messageBoxText">内容</param>
        /// <param name="button">按钮</param>
        /// <param name="icon">图标</param>
        /// <returns></returns>
        public MessageBoxResult ShowMessage(string messageBoxText, MessageBoxButton button, SinMessageBoxImage icon)
        {
            return ShowMessage(GetCaption(icon), messageBoxText, button, icon, true);
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="caption">标题</param>
        /// <param name="messageBoxText">内容</param>
        /// <param name="button">按钮</param>
        /// <param name="icon">图标</param>
        /// <returns></returns>
        public MessageBoxResult ShowMessage(string caption, string messageBoxText, MessageBoxButton button, SinMessageBoxImage icon, bool autoclose = false)
        {
            object result = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                int autoclosetime = 0;
                if (autoclose)
                {
                    XmlDocument xd = new XmlDocument();
                    xd.Load(MapPath.XmlPath + "MESSAGE_CONFIG.xml");
                    XmlNodeList NodeList1 = xd.DocumentElement.ChildNodes;
                    autoclosetime = int.Parse(NodeList1[0].InnerText.ToString());
                }
                SinMessageBox box = new SinMessageBox(caption, messageBoxText, icon, autoclosetime, StringResourceExtension.GetLanguage(6577, "后自动关闭"));
                box.AddButtons(GetButtons(box, button));
                box.ShowDialog();
                result = box.Result;
            });

            // 为 Null 暂时返回‘取消’
            if (result == null)
                return MessageBoxResult.Cancel;

            LogHelper.logSoftWare.Debug("caption：" + caption + "；  msgtxt：" + messageBoxText + "；  result：" + result.ToString());
            return (MessageBoxResult)result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        private IEnumerable<Button> GetButtons(SinMessageBox box, MessageBoxButton button)
        {
            // TODO：GetButtons 未翻译
            if (button == MessageBoxButton.OK)
            {
                yield return box.CreateButton(StringResourceExtension.GetLanguage(142, "确定"), true, false, MessageBoxResult.OK);
            }
            else if (button == MessageBoxButton.OKCancel)
            {
                yield return box.CreateButton(StringResourceExtension.GetLanguage(142, "确定"), true, false, MessageBoxResult.OK);
                yield return box.CreateButton(StringResourceExtension.GetLanguage(143, "取消"), false, true, MessageBoxResult.Cancel);
            }
            else if (button == MessageBoxButton.YesNo)
            {
                yield return box.CreateButton(StringResourceExtension.GetLanguage(5, "是"), true, false, MessageBoxResult.Yes);
                yield return box.CreateButton(StringResourceExtension.GetLanguage(6, "否"), false, true, MessageBoxResult.No);
            }
            else if (button == MessageBoxButton.YesNoCancel)
            {
                yield return box.CreateButton(StringResourceExtension.GetLanguage(5, "是"), true, false, MessageBoxResult.Yes);
                yield return box.CreateButton(StringResourceExtension.GetLanguage(6, "否"), false, true, MessageBoxResult.No);
                yield return box.CreateButton(StringResourceExtension.GetLanguage(143, "取消"), false, true, MessageBoxResult.Cancel);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        private string GetCaption(SinMessageBoxImage icon)
        {
            // TODO：GetCaption 未翻译
            switch (icon)
            {
                case SinMessageBoxImage.None:
                    return string.Empty;
                case SinMessageBoxImage.Completed:
                    return StringResourceExtension.GetLanguage(283, "完成");
                case SinMessageBoxImage.Error:
                    return StringResourceExtension.GetLanguage(816, "错误");
                case SinMessageBoxImage.Information:
                    return StringResourceExtension.GetLanguage(6468, "提示信息");
                case SinMessageBoxImage.Question:
                    return StringResourceExtension.GetLanguage(2197, "询问");
                case SinMessageBoxImage.Stop:
                    return StringResourceExtension.GetLanguage(814, "停止");
                case SinMessageBoxImage.Warning:
                    return StringResourceExtension.GetLanguage(815, "警告");
                default:
                    return string.Empty;
            }
        }
    }
}
