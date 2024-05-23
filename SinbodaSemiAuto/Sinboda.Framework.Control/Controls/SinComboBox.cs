using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common.ResourceExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;

namespace Sinboda.Framework.Control.Controls
{
    public class SinComboBox : ComboBox
    {
        private SinTextBox searchTbx = null;
        private ListCollectionView listView = null;

        /// <summary>
        /// 标识 <seealso cref="NullText"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty NullTextProperty = SinTextBox.NullTextProperty.AddOwner(typeof(SinComboBox));
        /// <summary>
        /// 长度
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty = SinTextBox.MaxLengthProperty.AddOwner(typeof(SinComboBox));
        /// <summary>
        /// 正则表达式
        /// </summary>
        public static readonly DependencyProperty RegexTextProperty = SinTextBox.RegexTextProperty.AddOwner(typeof(SinComboBox));
        /// <summary>
        /// 正则表达式校验后出现的错误信息
        /// </summary>
        public static readonly DependencyProperty RegexTextErrorMsgProperty = SinTextBox.RegexTextErrorMsgProperty.AddOwner(typeof(SinComboBox));
        /// <summary>
        /// 正则表达式所代表的格式
        /// </summary>
        public static readonly DependencyProperty RegexTextPatternProperty = SinTextBox.RegexTextPatternProperty.AddOwner(typeof(SinComboBox));
        /// <summary>
        /// 必填项
        /// </summary>
        private static readonly DependencyProperty IsDataRequireProperty = DependencyProperty.Register("IsDataRequire", typeof(bool), typeof(SinComboBox), new PropertyMetadata(false));
        /// <summary>
        /// 是否包含错误
        /// </summary>
        public static readonly DependencyProperty IsDataErrorProperty = DependencyProperty.Register("IsDataError", typeof(bool), typeof(SinComboBox), new PropertyMetadata(false, new PropertyChangedCallback(OnValidateErrorOccur)));

        public static readonly DependencyProperty HotKeyPathProperty = DependencyProperty.Register("HotKeyPath", typeof(string), typeof(SinComboBox), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 标识 <seealso cref="IsDataErrorCannotInput"/> 依赖性属性
        /// </summary>
        public static readonly DependencyProperty IsDataErrorCannotInputProperty = DependencyProperty.Register("IsDataErrorCannotInput", typeof(bool), typeof(SinComboBox), new PropertyMetadata(false));

        public static readonly DependencyProperty AllowSearchProperty =
        DependencyProperty.Register("AllowSearch", typeof(Visibility), typeof(SinComboBox), new FrameworkPropertyMetadata(Visibility.Collapsed));


        public Visibility AllowSearch
        {
            get { return (Visibility)GetValue(AllowSearchProperty); }
            set { SetValue(AllowSearchProperty, value); }
        }

        public string HotKeyPath
        {
            get { return (string)GetValue(HotKeyPathProperty); }
            set { SetValue(HotKeyPathProperty, value); }
        }

        /// <summary>
        /// 获取或设置 SelectedItem 属性为NULL显示的信息
        /// </summary>
        public string NullText
        {
            get { return (string)GetValue(NullTextProperty); }
            set { SetValue(NullTextProperty, value); }
        }
        /// <summary>
        /// 长度
        /// </summary>
        public int Maxlenth
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        /// <summary>
        /// 是否错误不可输入
        /// </summary>
        public bool IsDataErrorCannotInput
        {
            get { return (bool)GetValue(IsDataErrorCannotInputProperty); }
            set { SetValue(IsDataErrorCannotInputProperty, value); }
        }


        /// <summary>
        /// 正则表达式
        /// </summary>
        public string RegexText
        {
            get { return (string)GetValue(RegexTextProperty); }
            set { SetValue(RegexTextProperty, value); }
        }
        /// <summary>
        /// 正则表达式校验后出现的错误信息
        /// </summary>
        public string RegexTextErrorMsg
        {
            get { return (string)GetValue(RegexTextErrorMsgProperty); }
            set { SetValue(RegexTextErrorMsgProperty, value); }
        }
        /// <summary>
        /// 正则表达式所代表的格式
        /// </summary>
        public string RegexTextPattern
        {
            get { return (string)GetValue(RegexTextPatternProperty); }
            set { SetValue(RegexTextPatternProperty, value); }
        }
        /// <summary>
        /// 必填项
        /// </summary>
        public bool IsDataRequire
        {
            get { return (bool)GetValue(IsDataRequireProperty); }
            set { SetValue(IsDataRequireProperty, value); }
        }
        /// <summary>
        /// 是否错误
        /// </summary>
        public bool IsDataError
        {
            get { return (bool)GetValue(IsDataErrorProperty); }
            set { SetValue(IsDataErrorProperty, value); }
        }
        /// <summary>
        /// 产生校验错误事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValidateErrorOccur(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SinComboBox control = (SinComboBox)obj;
            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>((bool)args.OldValue,
                (bool)args.NewValue, ValidateErrorOccurEvent);
            control.OnValidateErrorOccur(e);
        }
        /// <summary>
        /// 产生校验错误事件
        /// </summary>
        public static readonly RoutedEvent ValidateErrorOccurEvent = EventManager.RegisterRoutedEvent("ValidateErrorOccur", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<bool>), typeof(SinComboBox));
        /// <summary>
        /// 产生校验错误事件
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> ValidateErrorOccur
        {
            add { AddHandler(ValidateErrorOccurEvent, value); }
            remove { RemoveHandler(ValidateErrorOccurEvent, value); }
        }
        /// <summary>
        /// 产生校验错误事件
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnValidateErrorOccur(RoutedPropertyChangedEventArgs<bool> args)
        {
            RaiseEvent(args);
        }
        /// <summary>
        /// 获取焦点后进行提示
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            ValidateInput();
        }


        /// <summary>
        /// 选中变更以后
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            //键盘有焦点，进行判断
            if (this.IsKeyboardFocusWithin)
            {
                ValidateInput();
            }
            else
            {
                //键盘没有焦点，只判断有值的
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    ValidateInput();
                }
            }
        }

        /// <summary>
        /// 应用模版时调用
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Popup popup = GetTemplateChild("PART_Popup") as Popup;
            if (popup != null)
            {
                popup.Opened += Popup_Opened;
                popup.Closed += Popup_Closed;
            }

            TextBox tb = GetTemplateChild("PART_EditableTextBox") as TextBox;
            if (tb != null)
                tb.TextChanged += OnTextChanged;

            searchTbx = GetTemplateChild("PART_Search") as SinTextBox;
            if (searchTbx != null)
                searchTbx.TextChanged += Search_TextChanged;
        }

        private bool ItemSourceFilter(object obj)
        {
            if (searchTbx == null || string.IsNullOrEmpty(searchTbx.Text))
                return true;

            if (obj == null)
                return false;

            string value = string.Empty;
            if (obj is string)
            {
                value = obj.ToString();
            }
            else
            {
                var type = obj.GetType();
                var valueProperty = type.GetProperty(DisplayMemberPath);
                var tem = valueProperty.GetValue(obj);
                if (tem == null)
                    return false;

                value = tem.ToString();
            }

            return value.ToString().ToLower().Contains(searchTbx.Text.ToLower());
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            try
            {
                if (searchTbx != null)
                {
                    searchTbx.Text = string.Empty;
                    listView.Refresh();
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Debug($"SinComboBox Popup_Closed 异常{ex.Message}");
            }
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            try
            {
                if (listView == null)
                {
                    listView = (ListCollectionView)CollectionViewSource.GetDefaultView(ItemsSource);
                    listView.Filter = ItemSourceFilter;
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Debug($"SinComboBox Popup_Opened 异常{ex.Message}");
            }
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            listView.Refresh();
        }

        /// <summary>
        /// 内容更改时再次验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateLength(sender);
            ValidateInput();
        }

        private void ValidateLength(object sender)
        {
            //try
            //{
            //TextBox tb = sender as TextBox;
            //if (tb != null && Maxlenth > 0)
            //{
            //    string inputText = tb.Text;

            //    double _Count = 0;
            //    int index = inputText.Length;
            //    for (int i = 0; i != inputText.Length; i++)
            //    {
            //        if (inputText[i] > 255)
            //            _Count += 2;
            //        else
            //            _Count++;
            //        if (_Count == Maxlenth)
            //        {
            //            index = i + 1;
            //            break;
            //        }
            //        else if (_Count > Maxlenth)
            //        {
            //            index = i;
            //            break;
            //        }
            //        index = i + 1;
            //    }
            //    inputText = inputText.Substring(0, index);
            //    if(!tb.Text.Equals(inputText))
            //    {
            //        tb.Text = inputText;
            //        tb.SelectionStart = Maxlenth;//把光标定位到输入字符最后
            //    }
            //int len = System.Text.Encoding.Default.GetByteCount(inputText);
            //if (len > Maxlenth)
            //{
            //    //获取输入字符串的二进制数组
            //    byte[] b = System.Text.Encoding.Default.GetBytes(inputText);
            //    //把截取的字节数组转成字符串
            //    string str = System.Text.Encoding.Default.GetString(b, 0, Maxlenth);
            //    //有?说明是汉字截取产生的乱码
            //    if (str.EndsWith("?"))
            //    {
            //        str = str.Substring(0, str.Length - 1);
            //    }
            //    tb.Text = str;
            //    tb.SelectionStart = Maxlenth;//把光标定位到输入字符最后
            //}
            //    }
            //}
            //catch { }
        }
        /// <summary>
        /// 定时器
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer();
        /// <summary>
        /// 提示信息
        /// </summary>
        private ToolTip _toolTip = new ToolTip();
        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="message"></param>
        private void ShowToolTip(string message)
        {
            timer.Interval = TimeSpan.FromSeconds(1.5);
            _toolTip.StaysOpen = true;
            _toolTip.PlacementTarget = this;
            _toolTip.Placement = PlacementMode.Right;
            _toolTip.Content = message;
            _toolTip.IsOpen = true;
            timer.Tick += (sender, args) =>
            {
                _toolTip.IsOpen = false;
                timer.Stop();
            };
            timer.Start();
        }

        /// <summary>
        /// 校验函数
        /// </summary>
        public void ValidateInput()
        {
            if (string.IsNullOrEmpty(Text))
            {
                if (IsDataRequire)
                {
                    ToolTip = StringResourceExtension.GetLanguage(25, "必填项");
                    IsDataError = true;
                }
                else
                {
                    ToolTip = null;
                    IsDataError = false;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(RegexText))
                {
                    if (!Regex.IsMatch(Text, RegexText))
                    {
                        ToolTip = RegexTextErrorMsg;
                        IsDataError = true;
                    }
                    else
                    {
                        ToolTip = null;
                        IsDataError = false;
                    }
                }
                else
                {
                    ToolTip = null;
                    IsDataError = false;
                }
            }
        }
        static SinComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SinComboBox), new FrameworkPropertyMetadata(typeof(SinComboBox)));
        }

        /// <summary>
        /// 初始化 <see cref="SinComboBox"/> 类的新实例
        /// </summary>
        public SinComboBox()
        {
            LostKeyboardFocus += SinComboBox_LostKeyboardFocus;
        }

        private void SinComboBox_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (!IsEditable || string.IsNullOrEmpty(HotKeyPath))
                return;

            if (Items == null || Items.Count <= 0)
                return;

            var tem = Items[0];
            var type = tem.GetType();
            var hotKeyProperty = type.GetProperty(HotKeyPath);
            var valuesProperty = type.GetProperty(SelectedValuePath);
            if (hotKeyProperty == null || valuesProperty == null)
                return;

            foreach (var item in Items)
            {
                var hotkey = hotKeyProperty.GetValue(item);
                if (hotkey == null)
                    continue;

                if (hotkey.Equals(Text))
                {
                    SelectedValue = valuesProperty.GetValue(item);
                }
            }
        }
    }
}
