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
    public partial class SampleControl : UserControl
    {
        Dictionary<string, ReagentMonitorColor> colorDic = new Dictionary<string, ReagentMonitorColor>();

        public SampleControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化颜色
        /// </summary>
        private void InitColor()
        {
            ReagentMonitorColor color = new ReagentMonitorColor();
            //未使用
            color = new ReagentMonitorColor
            {
                RoundColor = Color.FromRgb(185, 185, 185),
                TopRangeColor = Color.FromRgb(255, 255, 255),
                MiddleRangeColor = Color.FromRgb(230, 230, 230),
                BottomRangeColor = Color.FromRgb(132, 132, 132)
            };
            colorDic.Add("1", color);
            //试剂正常
            color = new ReagentMonitorColor
            {
                RoundColor = Color.FromRgb(47, 132, 208),
                TopRangeColor = Color.FromRgb(246, 251, 255),
                MiddleRangeColor = Color.FromRgb(207, 229, 249),
                BottomRangeColor = Color.FromRgb(65, 151, 231)
            };
            colorDic.Add("2", color);
            //试剂过期
            color = new ReagentMonitorColor
            {
                RoundColor = Color.FromRgb(218, 60, 183),
                TopRangeColor = Color.FromRgb(255, 245, 253),
                MiddleRangeColor = Color.FromRgb(247, 176, 232),
                BottomRangeColor = Color.FromRgb(218, 62, 184)
            };
            colorDic.Add("3", color);
            //试剂不足
            color = new ReagentMonitorColor
            {
                RoundColor = Color.FromRgb(192, 157, 0),
                TopRangeColor = Color.FromRgb(255, 252, 238),
                MiddleRangeColor = Color.FromRgb(255, 234, 138),
                BottomRangeColor = Color.FromRgb(229, 188, 8)
            };
            colorDic.Add("4", color);
        }

        public void SetColor(string index, bool isCover)
        {
            if (isCover)
            {
                cover_Border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 216, 235));
            }
            else
            {
                cover_Border.BorderBrush = new SolidColorBrush(colorDic[index].RoundColor);
            }
            
            middle_border.Background = new SolidColorBrush(colorDic[index].MiddleRangeColor);
        }
    }

    public class ReagentMonitorColor
    {
        /// <summary>
        /// 描边
        /// </summary>
        public Color RoundColor { get; set; }
        /// <summary>
        /// 上
        /// </summary>
        public Color TopRangeColor { get; set; }
        /// <summary>
        /// 中
        /// </summary>
        public Color MiddleRangeColor { get; set; }
        /// <summary>
        /// 下
        /// </summary>
        public Color BottomRangeColor { get; set; }
    }
}
