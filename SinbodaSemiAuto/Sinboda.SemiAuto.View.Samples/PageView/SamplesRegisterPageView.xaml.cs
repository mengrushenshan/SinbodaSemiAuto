using Sinboda.SemiAuto.View.Samples.UserControls;
using Sinboda.SemiAuto.View.Samples.ViewModel;
using System.Windows.Controls;


namespace Sinboda.SemiAuto.View.Samples.PageView
{
    /// <summary>
    /// SamplesRegisterPageView.xaml 的交互逻辑
    /// </summary>
    public partial class SamplesRegisterPageView : UserControl
    {
        SamplesRegisterPageViewModel viewModel;
        SampleRoundMonitorControl sampleRoundMonitorControl;
        public SamplesRegisterPageView()
        {
            InitializeComponent();
            DataContext = viewModel = new SamplesRegisterPageViewModel();
            sampleRoundMonitorControl = new SampleRoundMonitorControl();
            sampleRoundMonitorControl.DataContext = viewModel;
            SampleGrid.Children.Add(sampleRoundMonitorControl);
        }
    }
}
