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
using System.Windows.Threading;

namespace Sinboda.Framework.Control.Controls
{
    /// <summary>
    /// 指定 <see cref="DrMessageBox"/> 所显示的图标。
    /// </summary>
    public enum SinMessageBoxImage
    {
        /// <summary>
        /// 不显示图标
        /// </summary>
        None,
        /// <summary>
        /// 停止
        /// </summary>
        Stop,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// 询问
        /// </summary>
        Question,
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        /// <summary>
        /// 信息
        /// </summary>
        Information,
        /// <summary>
        /// 完成
        /// </summary>
        Completed
    }

    /// <summary>
    /// SinMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class SinMessageBox : SinWindow
    {
        /// <summary>
        /// 窗口返回结果
        /// </summary>
        public object Result { get; private set; }

        /// <summary>
        /// 倒计时秒
        /// </summary>
        private int countSecond = 0;
        /// <summary>
        /// 显示的倒计时文言
        /// </summary>
        private string messageautoclosetxt = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption">标题</param>
        /// <param name="messageBoxText">文本显示</param>
        /// <param name="icon">图标</param>
        /// <param name="autoclosetime">倒计时关闭</param>
        /// <param name="messageautoclose">倒计时文言</param>
        public SinMessageBox(string caption, string messageBoxText, SinMessageBoxImage icon, int autoclosetime = 0, string messageautoclose = "", bool btnAutoClose = false)
        {
            InitializeComponent();

            Title = caption ?? string.Empty;
            txtMessage.Text = messageBoxText;
            SetIcon(icon);
            if (autoclosetime != 0)
            {
                countSecond = autoclosetime;
                messageautoclosetxt = messageautoclose;
                txt.Text = countSecond.ToString() + "s" + messageautoclosetxt;
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += new EventHandler(Timer_Tick); //每一秒执行的方法
                timer.Start();
            }

            if (btnAutoClose)
            {

            }
        }
        /// <summary>
        /// 时间事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (countSecond == 1)
            {
                this.Close();
            }
            else
            {
                //判断txt是否处于UI线程上
                if (txt.Dispatcher.CheckAccess())
                {
                    txt.Text = (countSecond - 1).ToString() + "s" + messageautoclosetxt;
                }
                else
                {
                    txt.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        txt.Text = (countSecond - 1).ToString() + "s" + messageautoclosetxt;
                    }));
                }
                countSecond--;
            }
        }

        /// <summary>
        /// 创建按钮
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="isDefault">是否为默认按钮</param>
        /// <param name="isCancel">是否为取消按钮</param>
        /// <param name="result">返回值</param>
        public Button CreateButton(string content, bool isDefault, bool isCancel, MessageBoxResult result, bool btnAutoClose = false, int btnAutoCloseTime = 60)
        {
            Button btn = btnAutoClose ? new CloseButton(btnAutoCloseTime) : new Button();
            btn.Content = content;
            btn.IsDefault = isDefault;
            btn.IsCancel = isCancel;
            btn.Tag = result;
            return btn;
        }

        /// <summary>
        /// 批量添加按钮
        /// </summary>
        /// <param name="btns"></param>
        public void AddButtons(IEnumerable<Button> btns)
        {
            foreach (var btn in btns)
            {
                btn.Height = 35;
                btn.Margin = new Thickness(10, 0, 0, 0);
                btn.Click += Btn_Click;
                BottomPanel.Add(btn);
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsVisible)
                return;

            Button btn = sender as Button;
            MessageBoxResult? result = btn.Tag as MessageBoxResult?;
            if (result.HasValue)
            {
                if (result.Value == MessageBoxResult.OK || result.Value == MessageBoxResult.Yes)
                    DialogResult = true;
                else if (result.Value == MessageBoxResult.Cancel || result.Value == MessageBoxResult.No)
                    DialogResult = false;
                else
                    DialogResult = null;
            }
            Result = btn.Tag;
            Close();
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
    }

    public class CloseButton : Button
    {
        private int _Count = 60;
        private string _OldText = string.Empty;
        private DispatcherTimer timer = new DispatcherTimer();

        public CloseButton(int count)
        {
            _Count = count;
            SetResourceReference(StyleProperty, typeof(Button));
            Loaded += CloseButton_Loaded;
        }

        private void CloseButton_Loaded(object sender, RoutedEventArgs e)
        {

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            _OldText = Content.ToString();
        }

        protected override void OnClick()
        {
            timer.Stop();
            timer = null;
            base.OnClick();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_Count == 0)
            {
                timer.Stop();
                timer = null;
                RaiseEvent(new RoutedEventArgs(ClickEvent));
                return;
            }
            Content = $"{_OldText}({_Count}s)";
            _Count--;
        }
    }
}
