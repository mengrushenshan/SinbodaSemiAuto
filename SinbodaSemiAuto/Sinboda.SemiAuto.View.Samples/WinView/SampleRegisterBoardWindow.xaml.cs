using Sinboda.Framework.Control.Controls;
using Sinboda.SemiAuto.View.Samples.UserControls;
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

namespace Sinboda.SemiAuto.View.Samples.WinView
{
    /// <summary>
    /// SampleRegisterBoardWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SampleRegisterBoardWindow : SinWindow
    {
        SampleRegisterBoardViewModel viewModel;
        SampleRoundMonitorControl sampleRoundMonitorControl;
        public SampleRegisterBoardWindow()
        {
            InitializeComponent();
            DataContext = viewModel = new SampleRegisterBoardViewModel();
            sampleRoundMonitorControl = new SampleRoundMonitorControl();
            sampleRoundMonitorControl.DataContext = viewModel;
            sampleRoundMonitorControl.orderEventHandler += viewModel.HoleIndexChange;
            sampleRoundMonitorControl.GetBoardTemplate = viewModel.ShowTemplateInfo;
            sampleRoundMonitorControl.InitTemplateData();
            TempLateList.Children.Add(sampleRoundMonitorControl);
            viewModel.RefTemplateBoard = sampleRoundMonitorControl.SetTemplateData;


        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
