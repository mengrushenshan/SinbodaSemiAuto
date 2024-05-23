using DevExpress.Xpf.Grid;
using Sinboda.Framework.Common;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Control.GridColumnSetting;
using Sinboda.SemiAuto.View.Results.ViewModel;
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

namespace Sinboda.SemiAuto.View.Results.PageView
{
    /// <summary>
    /// ResultQueryPageView.xaml 的交互逻辑
    /// </summary>
    public partial class ResultQueryPageView : UserControl
    {
        ResultQueryPageViewModel viewModel;
        public ResultQueryPageView()
        {
            InitializeComponent();
            DataContext = viewModel = new ResultQueryPageViewModel();
        }

        /// <summary>
        /// 右键弹出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void columnResultSettingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            btnResultColumnSetting_Click(sender, e);
        }

        /// <summary>
        /// 调用设置界面之前请先调用InitGridColumn（参照此方法中的逻辑）进行初始化 
        /// 设置完成保存之后需要再次调用InitGridColumn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResultColumnSetting_Click(object sender, RoutedEventArgs e)
        {
            List<ColumnSetting> columnSettings = new List<ColumnSetting>();
            foreach (GridColumn column in columnResultSettingGridControl.Columns)
            {
                ColumnSetting item = new ColumnSetting();
                item.ColumnHeader = column.Header.ToString();
                item.ColumnWidth = column.Width.Value.ToString();
                item.IsVisible = column.Visible;
                item.ColumnField = column.FieldName;
                item.ColumnIndex = column.VisibleIndex;
                columnSettings.Add(item);
            }
            string strGridName = "ResultInfoGrid";
            string configPath = Path.Combine(MapPath.XmlPath, @"GridColumnConfig.xml");
            GridColumnSettingWindow settingWin = new GridColumnSettingWindow();
            settingWin.InitDataSource(configPath, strGridName, columnSettings);
            settingWin.ShowDialog();
            //设置完成保存之后需要再次调用InitGridColumn
            InitResultGridColumn();
        }

        /// <summary>
        /// GridColumn进行初始化
        /// </summary>
        private void InitResultGridColumn()
        {
            try
            {
                GridConfigManager configManager = new GridConfigManager();
                string strGridName = "ResultInfoGrid";
                string configPath = Path.Combine(MapPath.XmlPath, @"GridColumnConfig.xml");
                List<ColumnSetting> columnSettings = configManager.GetCurrentSetting(configPath, strGridName);
                //先看配置文件有没有配置
                if (columnSettings == null || columnSettings.Count == 0)
                {
                    //没有配置根据当前Grid的column生成配置
                    columnSettings = new List<ColumnSetting>();
                    foreach (GridColumn column in columnResultSettingGridControl.Columns)
                    {
                        ColumnSetting item = new ColumnSetting();
                        item.ColumnHeader = column.Header.ToString();
                        item.ColumnWidth = column.Width.Value.ToString();
                        item.IsVisible = column.Visible;
                        item.ColumnField = column.FieldName;
                        item.ColumnIndex = column.VisibleIndex;
                        columnSettings.Add(item);
                    }
                }
                //根据列索引属性排序
                columnSettings.Sort((x, y) => { return x.ColumnIndex - y.ColumnIndex; });
                //根据列可见属性设置是否显示
                foreach (ColumnSetting config in columnSettings)
                {
                    var tem = columnResultSettingGridControl.Columns.FirstOrDefault(o => o.FieldName == config.ColumnField);
                    if (tem == null)
                        continue;

                    this.columnResultSettingGridControl.Columns[config.ColumnField].Visible = config.IsVisible;
                    this.columnResultSettingGridControl.Columns[config.ColumnField].VisibleIndex = config.ColumnIndex;
                    this.columnResultSettingGridControl.Columns[config.ColumnField].Width = new GridColumnWidth(double.Parse(config.ColumnWidth));
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error("InitResultGridColumn:", ex);
            }
        }
    }
}
