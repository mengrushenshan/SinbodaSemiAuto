using Sinboda.Framework.Control.Controls;
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

namespace Sinboda.Framework.Control.GridColumnSetting
{
    /// <summary>
    /// GridColumnSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GridColumnSettingWindow : SinWindow
    {
        /// <summary>
        /// 保存成功后要对Grid重新初始化
        /// </summary>
        public Action InitColumns;

        /// <summary>
        /// 
        /// </summary>
        public GridColumnSettingWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化数据源
        /// </summary>
        /// <param name="configPath"></param>
        /// <param name="strGridName"></param>
        /// <param name="columns"></param>
        public void InitDataSource(string configPath, string strGridName, List<ColumnSetting> columns)
        {
            gridColumnSetting.InitDataSource(strGridName, columns, configPath);
        }
        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            gridColumnSetting.SaveGridColumnSetting();
            //设置完成保存之后需要再次调用InitGridColumn
            if (InitColumns != null)
                InitColumns();
            this.Close();
        }
        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
