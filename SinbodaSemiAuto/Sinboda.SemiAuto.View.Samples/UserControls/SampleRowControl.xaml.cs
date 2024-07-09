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
        public Action<string, bool> ShowRowBoard;
        private bool isSelected = false;

        public SampleRowControl()
        {
            InitializeComponent();
        }       

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ShowRowBoard != null)
            {
                isSelected = !isSelected;
                ShowRowBoard(row_TextBlock.Text, isSelected);
            }
        }

        public void SetIsSelect(bool flag)
        {
            isSelected = flag;
        }
    }

}
