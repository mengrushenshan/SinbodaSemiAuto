using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows;
using Sinboda.Framework.Common.ResourceExtensions;

namespace Sinboda.Framework.Control.Controls
{
    /// <summary>
    /// SinTextBox
    /// </summary>
    [ContentProperty("Buttons")]
    [TemplatePart(Name = "ButtonListName", Type = typeof(ItemsControl))]
    public class SinTextBox : TextBox
    {
        private const string ButtonListName = "PART_ButtonList";
        private ItemsControl _ButtonItemsControl;
        private bool validateInputTem = false;
        private int validateInputOffset = 0;

        /// <summary>
        /// 标识 <seealso cref="NullText"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty NullTextProperty = DependencyProperty.Register("NullText", typeof(string), typeof(SinTextBox), new PropertyMetadata(string.Empty));
        /// <summary>
        /// 标识 <seealso cref="PrefixText"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty PrefixTextProperty = DependencyProperty.Register("PrefixText", typeof(string), typeof(SinTextBox), new PropertyMetadata(string.Empty, PrefixTextChanged));
        /// <summary>
        /// 标识 <seealso cref="SuffixText"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty SuffixTextProperty = DependencyProperty.Register("SuffixText", typeof(string), typeof(SinTextBox), new PropertyMetadata(string.Empty, SuffixTextChanged));
        /// <summary>
        /// 标识 <seealso cref="IsPrefixText"/> 只读依赖性属性
        /// </summary>
        public static readonly DependencyPropertyKey IsPrefixTextPropertyKey = DependencyProperty.RegisterReadOnly("IsPrefixText", typeof(bool), typeof(SinTextBox), new PropertyMetadata(false));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsPrefixTextProperty = IsPrefixTextPropertyKey.DependencyProperty;
        /// <summary>
        /// 标识 <seealso cref="IsSuffixText"/> 只读依赖性属性
        /// </summary>
        public static readonly DependencyPropertyKey IsSuffixTextPropertyKey = DependencyProperty.RegisterReadOnly("IsSuffixText", typeof(bool), typeof(SinTextBox), new PropertyMetadata(false));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsSuffixTextProperty = IsSuffixTextPropertyKey.DependencyProperty;
        /// <summary>
        /// 标识 <seealso cref="GlyphIcon"/> 依赖性属性
        /// </summary>
        public static readonly DependencyProperty GlyphIconProperty = DependencyProperty.Register("GlyphIcon", typeof(string), typeof(SinTextBox), new PropertyMetadata(string.Empty));
        /// <summary>
        /// 标识 <seealso cref="IsDataRequire"/> 依赖性属性
        /// </summary>
        public static readonly DependencyProperty IsDataRequireProperty = DependencyProperty.Register("IsDataRequire", typeof(bool), typeof(SinTextBox), new PropertyMetadata(false));
        /// <summary>
        /// 标识 <seealso cref="RegexText"/> 依赖性属性
        /// </summary>
        public static readonly DependencyProperty RegexTextProperty = DependencyProperty.Register("RegexText", typeof(string), typeof(SinTextBox), new PropertyMetadata(string.Empty));
        /// <summary>
        /// 标识 <seealso cref="RegexTextErrorMsg"/> 依赖性属性
        /// </summary>
        public static readonly DependencyProperty RegexTextErrorMsgProperty = DependencyProperty.Register("RegexTextErrorMsg", typeof(string), typeof(SinTextBox), new PropertyMetadata(string.Empty));
        /// <summary>
        /// 标识 <seealso cref="RegexTextPattern"/> 依赖性属性
        /// </summary>
        public static readonly DependencyProperty RegexTextPatternProperty = DependencyProperty.Register("RegexTextPattern", typeof(string), typeof(SinTextBox), new PropertyMetadata(string.Empty));
        /// <summary>
        /// 标识 <seealso cref="IsDataError"/> 依赖性属性
        /// </summary>
        public static readonly DependencyProperty IsDataErrorProperty = DependencyProperty.Register("IsDataError", typeof(bool), typeof(SinTextBox), new PropertyMetadata(false, new PropertyChangedCallback(OnValidateErrorOccur)));
        /// <summary>
        /// 标识 <seealso cref="IsDataErrorCannotInput"/> 依赖性属性
        /// </summary>
        public static readonly DependencyProperty IsDataErrorCannotInputProperty = DependencyProperty.Register("IsDataErrorCannotInput", typeof(bool), typeof(SinTextBox), new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置 <see cref="SinTextBox"/> 左侧矢量图标名称
        /// </summary>
        public string GlyphIcon
        {
            get { return (string)GetValue(GlyphIconProperty); }
            set { SetValue(GlyphIconProperty, value); }
        }

        /// <summary>
        /// 获取 <seealso cref="SuffixText"/> 属性是否为空
        /// </summary>
        public bool IsSuffixText
        {
            get { return (bool)GetValue(IsSuffixTextProperty); }
        }

        /// <summary>
        /// 获取 <seealso cref="PrefixText"/> 属性是否为空
        /// </summary>
        public bool IsPrefixText
        {
            get { return (bool)GetValue(IsPrefixTextProperty); }
        }

        /// <summary>
        /// 获取或设置 <see cref="SinTextBox"/> 的右侧功能按键
        /// </summary>
        public ObservableCollection<object> Buttons { get; }

        /// <summary>
        /// 获取或设置 <seealso cref="TextBox.Text"/> 属性为NULL显示的信息
        /// </summary>
        public string NullText
        {
            get { return (string)GetValue(NullTextProperty); }
            set { SetValue(NullTextProperty, value); }
        }
        /// <summary>
        /// 获取或设置<see cref="SinTextBox"/>的前缀文字
        /// </summary>
        public string PrefixText
        {
            get { return (string)GetValue(PrefixTextProperty); }
            set { SetValue(PrefixTextProperty, value); }
        }
        /// <summary>
        /// 获取或设置<see cref="SinTextBox"/>的后缀文字
        /// </summary>
        public string SuffixText
        {
            get { return (string)GetValue(SuffixTextProperty); }
            set { SetValue(SuffixTextProperty, value); }
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
        /// 是否错误
        /// </summary>
        public bool IsDataError
        {
            get { return (bool)GetValue(IsDataErrorProperty); }
            set { SetValue(IsDataErrorProperty, value); }
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
        /// 产生校验错误事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValidateErrorOccur(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SinTextBox control = (SinTextBox)obj;
            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>((bool)args.OldValue,
                (bool)args.NewValue, ValidateErrorOccurEvent);
            control.OnValidateErrorOccur(e);
        }
        /// <summary>
        /// 产生校验错误事件
        /// </summary>
        public static readonly RoutedEvent ValidateErrorOccurEvent = EventManager.RegisterRoutedEvent("ValidateErrorOccur", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<bool>), typeof(SinTextBox));
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

        static SinTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SinTextBox), new FrameworkPropertyMetadata(typeof(SinTextBox)));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SinTextBox()
        {
            Buttons = new ObservableCollection<object>();
            Buttons.CollectionChanged += Items_CollectionChanged;
            ToolTipOpening += SinTextBox_ToolTipOpening;
        }

        private void SinTextBox_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (ToolTip == null || string.IsNullOrEmpty(ToolTip.ToString()))
                e.Handled = true;
        }


        /// <summary>
        /// 应用模版时调用
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _ButtonItemsControl = GetTemplateChild(ButtonListName) as ItemsControl;
            if (_ButtonItemsControl == null) return;
            UpdateView();
        }

        private void UpdateView()
        {
            if (_ButtonItemsControl == null) return;

            foreach (var item in Buttons)
            {
                _ButtonItemsControl.Items.Add(item);
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private static void ButtonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private void SetIsPrefixText(bool value)
        {
            SetValue(IsPrefixTextPropertyKey, value);
        }

        private void SetIsSuffixText(bool value)
        {
            SetValue(IsSuffixTextPropertyKey, value);
        }

        private static void SuffixTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SinTextBox tx = d as SinTextBox;
            if (tx == null) return;
            tx.SetIsSuffixText(e.NewValue != null);
        }

        private static void PrefixTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SinTextBox tx = d as SinTextBox;
            if (tx == null) return;
            tx.SetIsPrefixText(e.NewValue != null);
        }

        /// <summary>
        /// 获取焦点后进行提示
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (string.IsNullOrEmpty(Text))
            {
                if (IsDataRequire)
                {
                    if (string.IsNullOrEmpty(RegexTextPattern))
                        ShowToolTip(StringResourceExtension.GetLanguage(25, "必填项"));
                    else
                        ShowToolTip(StringResourceExtension.GetLanguage(25, "必填项") + "，" + StringResourceExtension.GetLanguage(64, "格式为") + " : " + RegexTextPattern);
                }
                else
                {
                    if (!string.IsNullOrEmpty(RegexTextPattern))
                        ShowToolTip(StringResourceExtension.GetLanguage(64, "格式为") + " : " + RegexTextPattern);
                }
            }
            else
                ValidateInput();
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
            if (IsReadOnly || !IsEnabled)
                return;

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
        /// 内容更改时再次验证
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (validateInputTem)
            {
                TextChange tc = e.Changes.LastOrDefault();
                if (tc != null)
                {
                    SelectionStart = validateInputOffset;
                }
                validateInputOffset = 0;
                validateInputTem = false;
            }

            //键盘有焦点，进行判断
            if (this.IsKeyboardFocusWithin)
            {
                ValidateInput(e.Changes);
                base.OnTextChanged(e);
            }
            else
            {
                //键盘没有焦点，只判断有值的
                if (!string.IsNullOrEmpty(Text))
                {
                    ValidateInput(e.Changes);
                    base.OnTextChanged(e);
                }
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {

        }

        /// <summary>
        /// 校验函数
        /// </summary>
        public void ValidateInput(ICollection<TextChange> textChanges = null)
        {
            if (IsReadOnly || !IsEnabled)
                return;

            if (string.IsNullOrEmpty(Text))
            {
                if (IsDataRequire)
                {
                    this.ToolTip = StringResourceExtension.GetLanguage(25, "必填项");
                    IsDataError = true;
                }
                else
                {
                    this.ToolTip = null;
                    IsDataError = false;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(RegexText))
                {
                    if (!Regex.IsMatch(Text, RegexText))
                    {
                        this.ToolTip = RegexTextErrorMsg;
                        IsDataError = true;
                        if (IsDataErrorCannotInput && !string.IsNullOrEmpty(Text) && textChanges != null)
                        {
                            foreach (var tc in textChanges)
                            {
                                validateInputTem = true;
                                validateInputOffset = tc.Offset;
                                Text = Text.Remove(tc.Offset, tc.AddedLength);
                            }
                        }
                    }
                    else
                    {
                        this.ToolTip = null;
                        IsDataError = false;
                    }
                }
                else
                {
                    this.ToolTip = null;
                    IsDataError = false;
                }
            }
        }
    }
}
