using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.View.Samples.ViewModel;
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
    /// SampleRoundMonitorControl.xaml 的交互逻辑
    /// </summary>
    public partial class SampleRoundMonitorControl : UserControl
    {
        SamplesRegisterPageViewModel viewModel;
        public SampleRoundMonitorControl()
        {
            InitializeComponent();
        }

        public OrderEventHandler orderEventHandler;

        /// <summary>
        /// 遍历每一个试剂，进行数据赋值
        /// </summary>
        public void SetData()
        {
            //遍历每一个试剂，进行数据赋值,Tag里存放的是架号-位置
            foreach (var item in canvasControl.Children)
            {
                if (item is SampleControl)
                {
                    SampleControl temp = item as SampleControl;
                    if (temp.Tag != null)
                    {
                        Sin_Sample sample = viewModel.GetSinSample(temp.Tag.ToString());
                        temp.InitData(sample);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SampleControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            orderEventHandler?.Invoke((sender as SampleControl).Tag.ToString());
        }
    }

    /// <summary>
    /// 事件
    /// </summary>
    /// <param name="Tag"></param>
    public delegate void OrderEventHandler(string Tag);
}
