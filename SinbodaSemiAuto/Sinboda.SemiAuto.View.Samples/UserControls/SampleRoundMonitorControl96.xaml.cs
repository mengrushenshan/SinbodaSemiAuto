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
    public partial class SampleRoundMonitorControl96 : UserControl
    {
        public Action<Sin_BoardTemplate> GetBoardTemplate;
        public Action<Sin_Board> GetBoard;
        SamplesRegisterPageViewModel boardViewModel;
        SampleRegisterBoardViewModel TempLateViewModel;
        public SampleRoundMonitorControl96()
        {
            InitializeComponent();
        }

        public void InitBoardData()
        {
            boardViewModel = this.DataContext as SamplesRegisterPageViewModel;
            foreach (var item in canvasControl.Children)
            {
                if (item is SampleControl)
                {
                    SampleControl temp = item as SampleControl;
                    temp.GetBoard = GetBoard;
                }

            }
        }

        public void InitTemplateData()
        {
            TempLateViewModel = this.DataContext as SampleRegisterBoardViewModel;
            foreach (var item in canvasControl.Children)
            {
                if (item is SampleControl)
                {
                    SampleControl temp = item as SampleControl;
                    temp.GetBoardTemplate = GetBoardTemplate;
                }
               
            }

        }
        /// <summary>
        /// 遍历每一个试剂，进行数据赋值
        /// </summary>
        public void SetBoardData()
        {
            if (boardViewModel == null)
            {
                return;
            }
            //遍历每一个试剂，进行数据赋值,Tag里存放的是架号-位置
            foreach (var item in canvasControl.Children)
            {
                if (item is SampleControl)
                {
                    SampleControl temp = item as SampleControl;
                    if (temp.Tag != null)
                    {
                        Sin_Board board = boardViewModel.GetSinBoard(temp.Tag.ToString());
                        temp.InitBoardData(board);
                    }
                }
            }
        }

        /// <summary>
        /// 遍历每一个试剂，进行数据赋值
        /// </summary>
        public void SetTemplateData()
        {
            if (TempLateViewModel == null)
            {
                return;
            }    
            //遍历每一个试剂，进行数据赋值,Tag里存放的是架号-位置
            foreach (var item in canvasControl.Children)
            {
                if (item is SampleControl)
                {
                    SampleControl temp = item as SampleControl;
                    if (temp.Tag != null)
                    {
                        Sin_BoardTemplate sample = TempLateViewModel.GetSinBoardTemplate(temp.Tag.ToString());
                        temp.InitTemplateData(sample);
                    }
                }
            }
        }

        /// <summary>
        /// 加载页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetBoardData();
            SetTemplateData();
        }
    }
}
