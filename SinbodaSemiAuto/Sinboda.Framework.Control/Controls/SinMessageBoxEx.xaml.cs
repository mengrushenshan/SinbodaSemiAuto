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

namespace Sinboda.Framework.Control.Controls
{
    /// <summary>
    /// SinMessageBoxEx.xaml 的交互逻辑
    /// </summary>
    public partial class SinMessageBoxEx : SinWindow
    {
        public SinMessageBoxEx(string caption, string messageBoxText, SinMessageBoxImage icon, IEnumerable<string> describes)
        {
            InitializeComponent();
            Title = caption;
            txtMessage.Text = messageBoxText;
            itemDescribe.ItemsSource = describes;
            SetIcon(icon);
        }

        private void SetIcon(SinMessageBoxImage image)
        {
            switch (image)
            {
                case SinMessageBoxImage.None:
                    icon.Visibility = Visibility.Collapsed;
                    break;
                case SinMessageBoxImage.Error:
                    icon.Data = Application.Current.Resources["Glyphicon-remove_sign"] as Geometry;
                    icon.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE64B4B"));
                    break;
                case SinMessageBoxImage.Information:
                    icon.Data = Application.Current.Resources["Glyphicon-info_sign"] as Geometry;
                    icon.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC"));
                    break;
                case SinMessageBoxImage.Question:
                    icon.Data = Application.Current.Resources["Glyphicon-question_sign"] as Geometry;
                    icon.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5BC0DE"));
                    break;
                case SinMessageBoxImage.Stop:
                    icon.Data = Application.Current.Resources["Glyphicon-ban_circle"] as Geometry;
                    icon.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE64B4B"));
                    break;
                case SinMessageBoxImage.Warning:
                    icon.Data = Application.Current.Resources["Glyphicon-exclamation_sign"] as Geometry;
                    icon.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF0AD4E"));
                    break;
                case SinMessageBoxImage.Completed:
                    icon.Data = Application.Current.Resources["Glyphicon-ok_sign"] as Geometry;
                    icon.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5FC948"));
                    break;
                default:
                    break;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
