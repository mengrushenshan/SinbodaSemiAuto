using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Sinboda.Framework.Control.DateTimePickers
{
    /// <summary>
    /// DateTimePicker.xaml 的交互逻辑
    /// </summary>
    public partial class DateTimePicker : UserControl
    {
        public static readonly RoutedEvent SelectedDateChangedEvent = DatePicker.SelectedDateChangedEvent.AddOwner(typeof(DateTimePicker));

        public event RoutedEventHandler SelectedDateChanged
        {
            add { AddHandler(SelectedDateChangedEvent, value); }
            remove { RemoveHandler(SelectedDateChangedEvent, value); }
        }

        public DateTimePicker()
        {
            InitializeComponent();
        }
        #region 事件
        /// <summary>
        /// DateTimePicker 窗体登录事件
        /// 外包要求初始值可以为空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    DateTime = DateTime.Now;
        //}

        private void datePicker_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (popChioce.IsOpen == true)
            {
                popChioce.IsOpen = false;
            }

            TDateTimeView dtView = new TDateTimeView(datePicker.Text);
            dtView.DateTimeOK += (dateTimeStr) =>
            {
                if (!string.IsNullOrEmpty(dateTimeStr))
                {
                    var oldDateTime = DateTime;
                    DateTime = Convert.ToDateTime(dateTimeStr, CultureInfo.CurrentCulture);
                    // 触发事件
                    RoutedEventArgs args = new RoutedEventArgs(SelectedDateChangedEvent, DateTime);
                    RaiseEvent(args);
                }
                popChioce.IsOpen = false;
            };

            popChioce.Child = dtView;
            popChioce.IsOpen = true;
        }
        #endregion

        #region 属性
        private static readonly DependencyProperty DateTimeProperty = DependencyProperty.Register("DateTime", typeof(DateTime?), typeof(DateTimePicker), new PropertyMetadata(new PropertyChangedCallback(SetValueCallBack)));

        private static readonly DependencyProperty IsDataRequireProperty = DependencyProperty.Register("IsDataRequire", typeof(bool), typeof(DateTimePicker), new PropertyMetadata(new PropertyChangedCallback(SetDataRequireValueCallBack)));



        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void SetValueCallBack(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimePicker control = (DateTimePicker)obj;
            control.DateTime = (DateTime?)args.NewValue;
            control.datePicker.Text = control.DateTime == null ? "" : control.DateTime.ToString();
        }
        /// <summary>
        /// 日期时间
        /// </summary>
        public DateTime? DateTime
        {
            get { return (DateTime?)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        /// <summary>
        /// 必填项
        /// </summary>
        public bool IsDataRequire
        {
            get { return datePicker.IsDataRequire; }
            set { datePicker.IsDataRequire = value; }
        }
        /// <summary>
        /// 设置必填项
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void SetDataRequireValueCallBack(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimePicker control = (DateTimePicker)obj;
            control.datePicker.IsDataRequire = (bool)args.NewValue;
        }

        /// <summary>
        /// 校验函数
        /// </summary>
        public void ValidateInput()
        {
            datePicker.ValidateInput();
        }
        #endregion
    }
}
