using Sinboda.Framework.Common.ResourceExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sinboda.Framework.Control.Controls
{
    public class SinDateTimePicker : DatePicker
    {
        /// <summary>
        /// 标识 <seealso cref="NullText"/> 依赖项属性
        /// </summary>
        public static readonly DependencyProperty NullTextProperty = DependencyProperty.Register("NullText", typeof(string), typeof(SinDateTimePicker), new PropertyMetadata(string.Empty));
        /// <summary>
        /// 显示文本
        /// </summary>
        public new static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SinDateTimePicker), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextValueChanged)));

        private static readonly DependencyProperty IsDataRequireProperty = DependencyProperty.Register("IsDataRequire", typeof(bool), typeof(SinDateTimePicker), new PropertyMetadata(false));
        /// <summary>
        /// 是否包含错误
        /// </summary>
        public static readonly DependencyProperty IsDataErrorProperty = DependencyProperty.Register("IsDataError", typeof(bool), typeof(SinDateTimePicker), new PropertyMetadata(false, new PropertyChangedCallback(OnValidateErrorOccur)));
        /// <summary>
        /// 获取或设置 <seealso cref="TextBox.Text"/> 属性为NULL显示的信息
        /// </summary>
        public string NullText
        {
            get { return (string)GetValue(NullTextProperty); }
            set { SetValue(NullTextProperty, value); }
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
        /// 显示文本属性
        /// </summary>
        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        /// 显示文本
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnTextValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SinDateTimePicker control = (SinDateTimePicker)obj;
            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>((string)args.OldValue,
                (string)args.NewValue, TextValueChangedEvent);
            control.OnTextValueChanged(e);
        }
        /// <summary>
        /// 显示文本事件
        /// </summary>
        public static readonly RoutedEvent TextValueChangedEvent = EventManager.RegisterRoutedEvent("TextValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<string>), typeof(SinDateTimePicker));
        /// <summary>
        /// 显示文本事件
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> TextValueChanged
        {
            add { AddHandler(TextValueChangedEvent, value); }
            remove { RemoveHandler(TextValueChangedEvent, value); }
        }
        /// <summary>
        /// 显示文本事件
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnTextValueChanged(RoutedPropertyChangedEventArgs<string> args)
        {
            RaiseEvent(args);

            TextBox tb = GetTemplateChild("PART_TextBox") as TextBox;
            if (tb != null)
            {
                tb.Text = Text;
            }
        }

        /// <summary>
        /// 产生校验错误事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValidateErrorOccur(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SinDateTimePicker control = (SinDateTimePicker)obj;
            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>((bool)args.OldValue,
                (bool)args.NewValue, ValidateErrorOccurEvent);
            control.OnValidateErrorOccur(e);
        }
        /// <summary>
        /// 产生校验错误事件
        /// </summary>
        public static readonly RoutedEvent ValidateErrorOccurEvent = EventManager.RegisterRoutedEvent("ValidateErrorOccur", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<bool>), typeof(SinDateTimePicker));
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
        /// 应用模版时调用
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextBox tb = GetTemplateChild("PART_TextBox") as TextBox;
            if (tb != null)
            {
                tb.TextChanged += DatePickerTextChanged;
                tb.Text = Text;
                tb.IsReadOnly = true;
            }

            GlyphButton gb = GetTemplateChild("PART_Button") as GlyphButton;
            if (gb != null)
                gb.Click += DatePickerButtonClicked;
        }
        /// <summary>
        /// 文本变更事件
        /// </summary>
        public event TextChangedEventHandler TextChanged;
        /// <summary>
        /// 内容更改时再次验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void DatePickerTextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
            if (TextChanged != null)
            {
                TextChanged(sender, e);
            }
        }
        /// <summary>
        /// 图标按钮点击事件
        /// </summary>
        public event RoutedEventHandler ButtonClicked;
        /// <summary>
        /// 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void DatePickerButtonClicked(object sender, RoutedEventArgs e)
        {
            if (ButtonClicked != null)
            {
                ButtonClicked(sender, e);
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
                    this.ToolTip = StringResourceExtension.GetLanguage(25, "必填项");//必填项
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

        static SinDateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SinDateTimePicker), new FrameworkPropertyMetadata(typeof(SinDateTimePicker)));
        }

        /// <summary>
        /// 初始化 <see cref="DrDateTimePicker"/> 类的新实例
        /// </summary>
        public SinDateTimePicker()
        {

        }
    }
}
