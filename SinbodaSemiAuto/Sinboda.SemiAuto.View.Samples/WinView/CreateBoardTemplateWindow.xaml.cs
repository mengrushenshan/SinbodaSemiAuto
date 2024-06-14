using Sinboda.Framework.Control.Controls;
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

namespace Sinboda.SemiAuto.View.Samples.WinView
{
    /// <summary>
    /// CreateBoardTemplateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreateBoardTemplateWindow : SinWindow
    {
        public CreateBoardTemplateWindow(List<Sin_BoardTemplate> boardTemplateList)
        {
            DataContext = new CreateBoardTemplateViewModel(boardTemplateList);
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
