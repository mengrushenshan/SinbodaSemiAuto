using Sinboda.Framework.Common.ResourceExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace Sinboda.Framework.Control.Controls
{
    public class SinNumricTextBox : TextBox
    {
        #region 依赖性属性

        private static readonly DependencyProperty IsDataRequireProperty = DependencyProperty.Register("IsDataRequire", typeof(bool), typeof(SinNumricTextBox), new FrameworkPropertyMetadata(false));

        private static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(SinNumricTextBox), new FrameworkPropertyMetadata(9999999999d));

        private static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(SinNumricTextBox), new FrameworkPropertyMetadata(-9999999999d));

        private static readonly DependencyProperty DecimalDigtsNumProperty = DependencyProperty.Register("DecimalDigtsNum", typeof(int), typeof(SinNumricTextBox), new FrameworkPropertyMetadata(0));


        /// <summary>
        /// 标识 <seealso cref="PrefixText"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty PrefixTextProperty = DependencyProperty.Register("PrefixText", typeof(string), typeof(SinNumricTextBox), new PropertyMetadata(string.Empty, PrefixTextChanged));
        /// <summary>
        /// 标识 <seealso cref="SuffixText"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty SuffixTextProperty = DependencyProperty.Register("SuffixText", typeof(string), typeof(SinNumricTextBox), new PropertyMetadata(string.Empty, SuffixTextChanged));
        /// <summary>
        /// 标识 <seealso cref="IsPrefixText"/> 只读依赖性属性
        /// </summary>
        public static readonly DependencyPropertyKey IsPrefixTextPropertyKey = DependencyProperty.RegisterReadOnly("IsPrefixText", typeof(bool), typeof(SinNumricTextBox), new PropertyMetadata(false));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsPrefixTextProperty = IsPrefixTextPropertyKey.DependencyProperty;
        /// <summary>
        /// 标识 <seealso cref="IsSuffixText"/> 只读依赖性属性
        /// </summary>
        public static readonly DependencyPropertyKey IsSuffixTextPropertyKey = DependencyProperty.RegisterReadOnly("IsSuffixText", typeof(bool), typeof(SinNumricTextBox), new PropertyMetadata(false));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsSuffixTextProperty = IsSuffixTextPropertyKey.DependencyProperty;


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsDataErrorProperty = DependencyProperty.Register("IsDataError", typeof(bool), typeof(SinNumricTextBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnValidateErrorOccur)));
        public static readonly DependencyProperty IsAllowNullProperty = DependencyProperty.Register("IsAllowNull", typeof(bool), typeof(SinNumricTextBox), new FrameworkPropertyMetadata(true));


        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool IsAllowNull
        {
            get { return (bool)GetValue(IsAllowNullProperty); }
            set { SetValue(IsAllowNullProperty, value); }
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
        /// 最大值
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        /// <summary>
        /// 小数位数
        /// </summary>
        public int DecimalDigtsNum
        {
            get { return (int)GetValue(DecimalDigtsNumProperty); }
            set { SetValue(DecimalDigtsNumProperty, value); }
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
        /// 获取或设置<see cref="DrTextBox"/>的前缀文字
        /// </summary>
        public string PrefixText
        {
            get { return (string)GetValue(PrefixTextProperty); }
            set { SetValue(PrefixTextProperty, value); }
        }
        /// <summary>
        /// 获取或设置<see cref="DrTextBox"/>的后缀文字
        /// </summary>
        public string SuffixText
        {
            get { return (string)GetValue(SuffixTextProperty); }
            set { SetValue(SuffixTextProperty, value); }
        }

        /// <summary>
        /// 产生校验错误事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValidateErrorOccur(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SinNumricTextBox control = (SinNumricTextBox)obj;
            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>((bool)args.OldValue,
                (bool)args.NewValue, ValidateErrorOccurEvent);
            control.OnValidateErrorOccur(e);
        }
        /// <summary>
        /// 产生校验错误事件
        /// </summary>
        public static readonly RoutedEvent ValidateErrorOccurEvent = EventManager.RegisterRoutedEvent("ValidateErrorOccur", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<bool>), typeof(SinNumricTextBox));
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
        #endregion

        /// <summary>
        /// 关联数字是负值的字符串
        /// </summary>
        private string negativeSign
        {
            get { return CultureInfo.CurrentCulture.NumberFormat.NegativeSign; }
        }
        /// <summary>
        /// 小数点分隔符
        /// </summary>
        private string numberDecimalDigits
        {
            get { return CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator; }
        }

        static SinNumricTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SinNumricTextBox), new FrameworkPropertyMetadata(typeof(SinNumricTextBox)));
        }

        /// <summary>
        /// 
        /// </summary>
        public SinNumricTextBox()
        {
            InputMethod.SetIsInputMethodEnabled(this, false);
        }

        /// <summary>
        /// 应用模版时调用
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
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
                    if ((double.IsNaN(MaxValue) && double.IsNaN(MinValue)) || MaxValue == MinValue)
                        ShowToolTip(StringResourceExtension.GetLanguage(25, "必填项"));
                    else
                        ShowToolTip(string.Format("{0}，{1} : {2}-{3}", StringResourceExtension.GetLanguage(25, "必填项"), StringResourceExtension.GetLanguage(3332, "范围"), MinValue, MaxValue));
                }
                else
                {
                    if ((!double.IsNaN(MaxValue) || !double.IsNaN(MinValue)) && MaxValue != MinValue)
                        ShowToolTip(string.Format("{0}: {1}-{2}", StringResourceExtension.GetLanguage(3332, "范围"), MinValue, MaxValue));
                }
            }
            else
                ValidateInput();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            ValidateInput();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            TrimZeroStart();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            int inputIndex = SelectionStart;
            if (e.Text == numberDecimalDigits)
            {
                int dIndex = Text.IndexOf(numberDecimalDigits);
                if (dIndex == -1)
                {
                    string integerText = Text.Substring(0, inputIndex);
                    string decimalText = Text.Substring(inputIndex);
                    // 新的小数部分长度如果大于允许输入的小数位数时忽略输入
                    if (decimalText.Length > DecimalDigtsNum)
                        e.Handled = true;
                }
                else
                {
                    SelectionStart = dIndex + 1;
                    e.Handled = true;
                }
            }
            else if (e.Text == negativeSign)
            {
                int mIndex = Text.IndexOf(negativeSign);
                if (mIndex == -1 && inputIndex == 0)
                {
                    if (!string.IsNullOrEmpty(Text))
                        e.Handled = !Checking(Text.Remove(inputIndex, SelectionLength).Insert(inputIndex, e.Text));
                    else
                        e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
            else if (e.Text.Length == 1 && (e.Text[0] >= '0' || e.Text[0] <= '9'))
            {
                string newValue = Text.Remove(inputIndex, SelectionLength).Insert(inputIndex, e.Text);
                string[] vs = newValue.Split(new string[] { numberDecimalDigits }, StringSplitOptions.RemoveEmptyEntries);
                int length = Math.Max(MaxValue.ToString().Length, MinValue.ToString().Length);
                if (vs[0].Replace(negativeSign, "").Length > length)
                {
                    e.Handled = true;
                    return;
                }

                if (vs.Length == 2 && vs[1].Length > DecimalDigtsNum)
                {
                    e.Handled = true;
                    return;
                }

                //int dIndex = newValue.IndexOf(numberDecimalDigits);
                //if (dIndex != -1 && newValue.Substring(dIndex + 1).Length > DecimalDigtsNum)
                //{
                //    e.Handled = true;
                //    return;
                //}



                e.Handled = !Checking(newValue);
            }
            else
                e.Handled = true;
        }

        /// <summary>
        /// 过滤按键
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            //数字键
            if ((KeyboardHelper.IsDot(e.Key) && DecimalDigtsNum <= 0)
                || KeyboardHelper.IsMinus(e.Key) && MinValue >= 0
                || KeyboardHelper.IsCharacter(e.Key) || e.Key == Key.Space)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }

        }

        /// <summary>
        /// 检查是否符合输入范围
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool Checking(string text)
        {
            double value = 0;
            if (!double.TryParse(text, out value))
            {
                return false;
            }
            if (text.IndexOf(numberDecimalDigits) == -1)
            {
                if (text.IndexOf(",") != -1)
                {
                    return false;
                }
            }

            if (value > MaxValue)
            {
                ShowToolTip(string.Format("{0} : {1}-{2}", StringResourceExtension.GetLanguage(3332, "范围"), MinValue, MaxValue));
                return false;
            }
            else if (value < MinValue)
            {
                ShowToolTip(string.Format("{0} : {1}-{2}", StringResourceExtension.GetLanguage(3332, "范围"), MinValue, MaxValue));
                return MinValue < 0 ? false : true;
            }
            return true;
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
        public void ShowToolTip(string message)
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
        /// 去掉开头部分多余的0
        /// </summary>
        private void TrimZeroStart()
        {
            string resultText = this.Text;
            // 如果为小数点开始，补0
            int index = resultText.IndexOf(numberDecimalDigits);
            if (index == 0)
            {
                resultText = "0" + resultText;
            }

            double v = 0;
            if (double.TryParse(resultText, out v))
            {
                Text = v.ToString();
            }
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
                if (DecimalDigtsNum == 0)
                {
                    if (Text.Contains('.'))
                        if (Text.Split('.')[0].Length == 0)
                        {
                            this.ToolTip = StringResourceExtension.GetLanguage(3135, "请输入数字"); //请输入有效数字
                            IsDataError = true;
                            return;
                        }

                    if (Text.Contains('-'))
                        if (Text.Split('-')[1].Length == 0)
                        {
                            this.ToolTip = StringResourceExtension.GetLanguage(3135, "请输入数字"); //请输入有效数字
                            IsDataError = true;
                            return;
                        }


                    double value = double.Parse(Text);
                    if (!double.IsNaN(MaxValue) && !double.IsNaN(MinValue))
                    {
                        if (value > MaxValue)
                        {
                            this.ToolTip = StringResourceExtension.GetLanguage(42, "已超过最大值") + MaxValue; //已超过最大值
                            IsDataError = true;
                        }
                        else if (value < MinValue)
                        {
                            this.ToolTip = StringResourceExtension.GetLanguage(43, "已超过最小值") + MinValue; //已超过最小值
                            IsDataError = true;
                        }
                        else
                        {
                            this.ToolTip = null;
                            IsDataError = false;
                        }
                    }
                    else if (double.IsNaN(MaxValue) && !double.IsNaN(MinValue))
                    {
                        if (value < MinValue)
                        {
                            this.ToolTip = StringResourceExtension.GetLanguage(43, "已超过最小值") + MinValue; //已超过最小值
                            IsDataError = true;
                        }
                        else
                        {
                            this.ToolTip = null;
                            IsDataError = false;
                        }
                    }
                    else if (!double.IsNaN(MaxValue) && double.IsNaN(MinValue))
                    {
                        if (value > MaxValue)
                        {
                            this.ToolTip = StringResourceExtension.GetLanguage(42, "已超过最大值") + MaxValue; //已超过最大值
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
                        this.ToolTip = null;
                        IsDataError = false;
                    }
                }
                else
                {
                    if (Text.Contains('.'))
                        if (Text.Split('.')[0].Length == 0)
                        {
                            this.ToolTip = StringResourceExtension.GetLanguage(49, "请输入有效小数"); //请输入有效小数
                            IsDataError = true;
                            return;
                        }

                    if (Text.Contains('-'))
                        if (Text.Split('-')[1].Length == 0)
                        {
                            this.ToolTip = StringResourceExtension.GetLanguage(3135, "请输入数字"); //请输入有效数字
                            IsDataError = true;
                            return;
                        }

                    double value = double.Parse(Text);
                    if (!double.IsNaN(MaxValue) && !double.IsNaN(MinValue))
                    {
                        if (value > MaxValue)
                        {
                            this.ToolTip = StringResourceExtension.GetLanguage(42, "已超过最大值") + MaxValue; //已超过最大值
                            IsDataError = true;
                        }
                        else if (value < MinValue)
                        {
                            this.ToolTip = StringResourceExtension.GetLanguage(43, "已超过最小值") + MinValue; //已超过最小值
                            IsDataError = true;
                        }
                        else
                        {
                            this.ToolTip = null;
                            IsDataError = false;
                        }
                    }
                    else if (double.IsNaN(MaxValue) && !double.IsNaN(MinValue))
                    {
                        if (value < MinValue)
                        {
                            this.ToolTip = StringResourceExtension.GetLanguage(43, "已超过最小值") + MinValue; //已超过最小值
                            IsDataError = true;
                        }
                        else
                        {
                            this.ToolTip = null;
                            IsDataError = false;
                        }
                    }
                    else if (!double.IsNaN(MaxValue) && double.IsNaN(MinValue))
                    {
                        if (value > MaxValue)
                        {
                            this.ToolTip = StringResourceExtension.GetLanguage(42, "已超过最大值") + MaxValue; //已超过最大值
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
                        this.ToolTip = null;
                        IsDataError = false;
                    }
                }
            }
        }

        private static void SuffixTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SinNumricTextBox tx = d as SinNumricTextBox;
            if (tx == null) return;
            tx.SetIsSuffixText(e.NewValue != null);
        }

        private static void PrefixTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SinNumricTextBox tx = d as SinNumricTextBox;
            if (tx == null) return;
            tx.SetIsPrefixText(e.NewValue != null);
        }

        private void SetIsPrefixText(bool value)
        {
            SetValue(IsPrefixTextPropertyKey, value);
        }

        private void SetIsSuffixText(bool value)
        {
            SetValue(IsSuffixTextPropertyKey, value);
        }
    }
    /// <summary>
    /// TextBox筛选行为，过滤不需要的按键
    /// </summary>
    public class TextBoxFilterBehavior : Behavior<SinNumricTextBox>
    {
        private string _prevText = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public TextBoxFilterBehavior()
        { }


        #region Dependency Properties
        /// <summary>
        /// TextBox筛选选项，这里选择的为过滤后剩下的按键
        /// 控制键不参与筛选，可以多选组合
        /// </summary>
        public TextBoxFilterOptions TextBoxFilterOptions
        {
            get { return (TextBoxFilterOptions)GetValue(TextBoxFilterOptionsProperty); }
            set { SetValue(TextBoxFilterOptionsProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for TextBoxFilterOptions.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextBoxFilterOptionsProperty =
            DependencyProperty.Register("TextBoxFilterOptions", typeof(TextBoxFilterOptions), typeof(TextBoxFilterBehavior), new PropertyMetadata(TextBoxFilterOptions.None));
        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewKeyDown += new KeyEventHandler(AssociatedObject_KeyDown);
            this.AssociatedObject.PreviewTextInput += new TextCompositionEventHandler(AssociatedObject_PreviewTextInput);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewKeyDown -= new KeyEventHandler(AssociatedObject_KeyDown);
            this.AssociatedObject.PreviewTextInput -= new TextCompositionEventHandler(AssociatedObject_PreviewTextInput);
        }

        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AssociatedObject_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int inputIndex = AssociatedObject.SelectionStart;
            if (e.Text == ".")
            {
                int dIndex = AssociatedObject.Text.IndexOf('.');
                if (dIndex == -1)
                {
                    string integerText = AssociatedObject.Text.Substring(0, inputIndex);
                    string decimalText = AssociatedObject.Text.Substring(inputIndex);
                    // 新的小数部分长度如果大于允许输入的小数位数时忽略输入
                    if (decimalText.Length > AssociatedObject.DecimalDigtsNum)
                        e.Handled = true;
                }
                else
                {
                    AssociatedObject.SelectionStart = dIndex + 1;
                    e.Handled = true;
                }
            }
            else if (e.Text == "-")
            {
                int mIndex = AssociatedObject.Text.IndexOf('-');
                if (mIndex == -1 && inputIndex == 0)
                {
                    if (!string.IsNullOrEmpty(AssociatedObject.Text))
                        e.Handled = !Checking(AssociatedObject.Text.Insert(inputIndex, e.Text));
                    else
                        e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
            else
            {
                string newValue = AssociatedObject.Text.Insert(inputIndex, e.Text);
                int dIndex = newValue.IndexOf(".");
                if (dIndex != -1 && newValue.Substring(dIndex + 1).Length > AssociatedObject.DecimalDigtsNum)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = !Checking(AssociatedObject.Text.Insert(inputIndex, e.Text));
            }
        }

        private bool Checking(string text)
        {

            double value = 0;
            if (!double.TryParse(text, out value))
            {
                return false;
            }

            if (value > AssociatedObject.MaxValue)
            {
                AssociatedObject.ShowToolTip(string.Format("{0} : {1}-{2}", StringResourceExtension.GetLanguage(3332, "范围"), AssociatedObject.MinValue, AssociatedObject.MaxValue)); //
                return false;
            }
            else if (value < AssociatedObject.MinValue)
            {
                AssociatedObject.ShowToolTip(string.Format("{0} : {1}-{2}", StringResourceExtension.GetLanguage(3332, "范围"), AssociatedObject.MinValue, AssociatedObject.MaxValue)); //
                return false;
            }
            return true;
        }

        /// <summary>
        /// 处理按键产生的输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            bool handled = true;
            //不进行过滤
            if (TextBoxFilterOptions == TextBoxFilterOptions.None || KeyboardHelper.IsControlKeys(e.Key))
            {
                handled = false;
            }
            //数字键
            if (handled && TextBoxFilterOptions.ContainsOption(TextBoxFilterOptions.Numeric))
            {
                handled = !KeyboardHelper.IsDigit(e.Key);
            }
            //小数点
            if (handled && TextBoxFilterOptions.ContainsOption(TextBoxFilterOptions.Dot) && AssociatedObject.DecimalDigtsNum > 0)
            {
                handled = !KeyboardHelper.IsDot(e.Key);
            }
            //负号
            if (handled && TextBoxFilterOptions.ContainsOption(TextBoxFilterOptions.Minus) && AssociatedObject.MinValue < 0)
            {
                handled = !KeyboardHelper.IsMinus(e.Key);
            }
            e.Handled = handled;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// 判断是否符合规则
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsValidChar(char c)
        {
            if (TextBoxFilterOptions == TextBoxFilterOptions.None)
            {
                return true;
            }
            else if (TextBoxFilterOptions.ContainsOption(TextBoxFilterOptions.Numeric) &&
                '0' <= c && c <= '9')
            {
                return true;
            }
            else if (TextBoxFilterOptions.ContainsOption(TextBoxFilterOptions.Dot) &&
                c == '.')
            {
                return true;
            }
            else if (TextBoxFilterOptions.ContainsOption(TextBoxFilterOptions.Minus) && c == '-')
            {
                return true;
            }
            else if (TextBoxFilterOptions.ContainsOption(TextBoxFilterOptions.Character))
            {
                if (('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z'))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断文本是否符合规则
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool IsValidText(string text)
        {
            //只能有一个小数点
            if (text.IndexOf('.') != text.LastIndexOf('.'))
            {
                return false;
            }
            //只能有一个负号
            if (text.IndexOf('-') != text.LastIndexOf('-'))
            {
                return false;
            }
            foreach (char c in text)
            {
                if (!IsValidChar(c))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion


    }
    /// <summary>
    /// TextBox筛选选项
    /// </summary>
    [Flags]
    public enum TextBoxFilterOptions
    {
        /// <summary>
        /// 不采用任何筛选
        /// </summary>
        None = 0,
        /// <summary>
        /// 数字类型不参与筛选
        /// </summary>
        Numeric = 1,
        /// <summary>
        /// 字母类型不参与筛选
        /// </summary>
        Character = 2,
        /// <summary>
        /// 小数点不参与筛选
        /// </summary>
        Dot = 4,
        /// <summary>
        /// 负号不参与筛选
        /// </summary>
        Minus = 8,
        /// <summary>
        /// 其它类型不参与筛选
        /// </summary>
        Other = 16
    }
    /// <summary>
    /// TextBox筛选选项枚举扩展方法
    /// </summary>
    public static class TextBoxFilterOptionsExtension
    {
        /// <summary>
        /// 在全部的选项中是否包含指定的选项
        /// </summary>
        /// <param name="allOptions">所有的选项</param>
        /// <param name="option">指定的选项</param>
        /// <returns></returns>
        public static bool ContainsOption(this TextBoxFilterOptions allOptions, TextBoxFilterOptions option)
        {
            return (allOptions & option) == option;
        }
    }
    /// <summary>
    /// 键盘操作帮助类
    /// </summary>
    public class KeyboardHelper
    {
        /// <summary>
        /// 键盘上的句号键
        /// </summary>
        public const int OemPeriod = 190;

        #region Fileds

        /// <summary>
        /// 控制键
        /// </summary>
        private static readonly List<Key> _controlKeys = new List<Key>
                                                             {
                                                                 Key.Back,
                                                                 Key.CapsLock,
                                                                 //Key.Ctrl,
                                                                 Key.Down,
                                                                 Key.End,
                                                                 Key.Enter,
                                                                 Key.Escape,
                                                                 Key.Home,
                                                                 Key.Insert,
                                                                 Key.Left,
                                                                 Key.PageDown,
                                                                 Key.PageUp,
                                                                 Key.Right,
                                                                 //Key.Shift,
                                                                 Key.Tab,
                                                                 Key.Up
                                                             };

        #endregion

        /// <summary>
        /// 是否是数字键
        /// </summary>
        /// <param name="key">按键</param>
        /// <returns></returns>
        public static bool IsDigit(Key key)
        {
            bool shiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
            bool retVal;
            //按住shift键后，数字键并不是数字键
            if (key >= Key.D0 && key <= Key.D9 && !shiftKey)
            {
                retVal = true;
            }
            else
            {
                retVal = key >= Key.NumPad0 && key <= Key.NumPad9;
            }
            return retVal;
        }

        /// <summary>
        /// 是否是控制键
        /// </summary>
        /// <param name="key">按键</param>
        /// <returns></returns>
        public static bool IsControlKeys(Key key)
        {
            return _controlKeys.Contains(key);
        }

        /// <summary>
        /// 是否是小数点
        /// Silverlight中无法识别问号左边的那个小数点键
        /// 只能识别小键盘中的小数点
        /// </summary>
        /// <param name="key">按键</param>
        /// <returns></returns>
        public static bool IsDot(Key key)
        {
            bool shiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
            bool flag = false;
            if (key == Key.Decimal)
            {
                flag = true;
            }
            if (key == Key.OemPeriod && !shiftKey)
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 是否是小数点
        /// </summary>
        /// <param name="key">按键</param>
        /// <param name="keyCode">平台相关的按键代码</param>
        /// <returns></returns>
        public static bool IsDot(Key key, int keyCode)
        {

            //return IsDot(key) || (key == Key.Unknown && keyCode == OemPeriod);
            return IsDot(key) || (keyCode == OemPeriod);
        }

        /// <summary>
        /// 是否是字母键
        /// </summary>
        /// <param name="key">按键</param>
        /// <returns></returns>
        public static bool IsCharacter(Key key)
        {
            return key >= Key.A && key <= Key.Z;
        }

        /// <summary>
        /// 是否为负号
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsMinus(Key key)
        {
            bool shiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
            bool flag = false;
            if (key == Key.Subtract)
            {
                flag = true;
            }
            if (key == Key.OemMinus && !shiftKey)
            {
                flag = true;
            }
            return flag;
        }

    }
}
