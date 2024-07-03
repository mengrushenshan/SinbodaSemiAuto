using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
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

namespace Sinboda.SemiAuto.View.Samples.UserControls
{
    /// <summary>
    /// SampleControl.xaml 的交互逻辑
    /// </summary>
    public partial class SampleRowControl : UserControl
    {
        public Action<string> ShowRowBoard;
        Dictionary<string, ReagentMonitorColor> colorDic = new Dictionary<string, ReagentMonitorColor>();

        public SampleRowControl()
        {
            InitializeComponent();
            InitColor();
        }       

        /// <summary>
        /// 初始化颜色
        /// </summary>
        private void InitColor()
        {
            ReagentMonitorColor color = new ReagentMonitorColor();
            //样本
            color = new ReagentMonitorColor
            {
                //RoundColor = Color.FromRgb(137, 200, 123),
                MiddleRangeColor = Color.FromRgb(127, 191, 113)
            };
            colorDic.Add("1", color);
            //聚焦
            color = new ReagentMonitorColor
            {
                //RoundColor = Color.FromRgb(59, 208, 196),
                MiddleRangeColor = Color.FromRgb(59, 208, 196)
            };
            colorDic.Add("2", color);
            //质控
            color = new ReagentMonitorColor
            {
                //RoundColor = Color.FromRgb(218, 60, 183),
                MiddleRangeColor = Color.FromRgb(247, 176, 232)
            };
            colorDic.Add("3", color);
            //校准
            color = new ReagentMonitorColor
            {
                //RoundColor = Color.FromRgb(192, 157, 0),
                MiddleRangeColor = Color.FromRgb(255, 234, 138)
            };
            colorDic.Add("4", color);
            //选中
            color = new ReagentMonitorColor
            {
                RoundColor = Color.FromRgb(84, 84, 155),
            };
            colorDic.Add("5", color);
            color = new ReagentMonitorColor
            {
                RoundColor = Color.FromRgb(216, 216, 216),
                MiddleRangeColor = Color.FromRgb(255, 255, 255)
            };
            colorDic.Add("6", color);
        }

        public void SetColor(string index, bool isCover)
        {
            if (isCover)
            {
                middle_border.Background = new SolidColorBrush(colorDic[index].MiddleRangeColor);
            }

            cover_Border.BorderBrush = new SolidColorBrush(colorDic[index].RoundColor);
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ShowRowBoard != null)
            {
                ShowRowBoard(row_TextBlock.Text);
            }

            SetColor("5", false);
        }
    }

}
