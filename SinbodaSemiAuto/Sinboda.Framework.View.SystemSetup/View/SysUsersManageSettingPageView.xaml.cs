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

namespace Sinboda.Framework.View.SystemSetup.View
{
    /// <summary>
    /// SysUsersManageSettingPageView.xaml 的交互逻辑
    /// </summary>
    public partial class SysUsersManageSettingPageView : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public SysUsersManageSettingPageView()
        {
            InitializeComponent();
        }
        private void Validation()
        {

        }
        private void DrTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SinTextBox drText = sender as SinTextBox;
                if (drText != null && drText.MaxLength > 0)
                {
                    string inputText = drText.Text;
                    double _Count = 0;
                    int index = inputText.Length;
                    for (int i = 0; i != inputText.Length; i++)
                    {
                        if (inputText[i] > 255)
                            _Count += 2;
                        else
                            _Count++;
                        if (_Count == drText.MaxLength)
                        {
                            index = i + 1;
                            break;
                        }
                        else if (_Count > drText.MaxLength)
                        {
                            index = i;
                            break;
                        }
                        index = i + 1;
                    }
                    inputText = inputText.Substring(0, index);

                    if (drText.Text.Equals(inputText))
                    {
                        drText.Text = inputText;
                        drText.SelectionStart = drText.MaxLength;//把光标定位到输入字符最后
                    }

                    //int len = System.Text.Encoding.Default.GetByteCount(inputText);
                    //if (len > drText.MaxLength)
                    //{
                    //    //获取输入字符串的二进制数组
                    //    byte[] b = System.Text.Encoding.Default.GetBytes(inputText);
                    //    //把截取的字节数组转成字符串
                    //    string str = System.Text.Encoding.Default.GetString(b, 0, drText.MaxLength);
                    //    int lastIndex = drText.MaxLength;
                    //    //有?说明是汉字截取产生的乱码
                    //    if (str.EndsWith("?"))
                    //    {
                    //        str = str.Substring(0, str.Length - 1);
                    //    }
                    //    drText.Text = str;
                    //    drText.SelectionStart = drText.MaxLength;//把光标定位到输入字符最后
                    //}
                }
            }
            catch { }
        }

        private void userManage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.SysUsersManageSettingViewModel model = this.DataContext as ViewModel.SysUsersManageSettingViewModel;
            if (model != null)
            {
                model.InitUserAndRole();
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
