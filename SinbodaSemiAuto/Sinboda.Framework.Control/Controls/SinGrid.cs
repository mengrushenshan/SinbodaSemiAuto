using Sinboda.Framework.Control.DateTimePickers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sinboda.Framework.Control.Controls
{
    public class SinGrid : Grid
    {
        private List<ValidateErrorInfoOfControl> errorList = new List<ValidateErrorInfoOfControl>();
        /// <summary>
        /// 构造函数，用于接收控件出现错误校验使用
        /// </summary>
        public SinGrid()
        {
            this.AddHandler(SinTextBox.ValidateErrorOccurEvent, new RoutedEventHandler(this.MyGrid_ValidateErrorOccur));
            this.AddHandler(SinNumricTextBox.ValidateErrorOccurEvent, new RoutedEventHandler(this.MyGrid_ValidateErrorOccur));
            this.AddHandler(SinComboBox.ValidateErrorOccurEvent, new RoutedEventHandler(this.MyGrid_ValidateErrorOccur));
            this.AddHandler(SinDatePicker.ValidateErrorOccurEvent, new RoutedEventHandler(this.MyGrid_ValidateErrorOccur));
            this.AddHandler(SinDateTimePicker.ValidateErrorOccurEvent, new RoutedEventHandler(this.MyGrid_ValidateErrorOccur));
        }
        /// <summary>
        /// 是否校验通过
        /// </summary>
        public static readonly DependencyProperty IsValidatedProperty = DependencyProperty.Register("IsValidated", typeof(bool), typeof(SinGrid), new PropertyMetadata(true));
        /// <summary>
        /// 是否校验通过
        /// </summary>
        public bool IsValidated
        {
            get { return (bool)GetValue(IsValidatedProperty); }
            internal set { SetValue(IsValidatedProperty, value); }
        }
        /// <summary>
        /// 表单内部控件出现校验错误处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyGrid_ValidateErrorOccur(object sender, RoutedEventArgs e)
        {
            if (e != null)
            {
                ValidateErrorInfoOfControl info = new ValidateErrorInfoOfControl();
                if (e.OriginalSource.GetType() == typeof(SinTextBox))
                {
                    info.Name = ((SinTextBox)e.OriginalSource).Name;
                    info.ValidateResult = ((SinTextBox)e.OriginalSource).IsDataError;
                    info.ErrorInfo = ((SinTextBox)e.OriginalSource).RegexTextErrorMsg;
                }
                if (e.OriginalSource.GetType() == typeof(SinNumricTextBox))
                {
                    info.Name = ((SinNumricTextBox)e.OriginalSource).Name;
                    info.ValidateResult = ((SinNumricTextBox)e.OriginalSource).IsDataError;
                }
                if (e.OriginalSource.GetType() == typeof(SinComboBox))
                {
                    info.Name = ((SinComboBox)e.OriginalSource).Name;
                    info.ValidateResult = ((SinComboBox)e.OriginalSource).IsDataError;
                }
                if (e.OriginalSource.GetType() == typeof(SinDatePicker))
                {
                    info.Name = ((SinDatePicker)e.OriginalSource).Name;
                    info.ValidateResult = ((SinDatePicker)e.OriginalSource).IsDataError;
                }
                if (e.OriginalSource.GetType() == typeof(SinDateTimePicker))
                {
                    info.Name = ((SinDateTimePicker)e.OriginalSource).Name;
                    info.ValidateResult = ((SinDateTimePicker)e.OriginalSource).IsDataError;
                }
                if (errorList.Where(o => o.Name == info.Name).Count() == 0)
                {
                    errorList.Add(info);
                }
                else
                {
                    errorList.Where(o => o.Name == info.Name).FirstOrDefault().ValidateResult = info.ValidateResult;
                }

                if (errorList.Where(o => o.ValidateResult == true).Count() == 0)
                    IsValidated = true;
                else
                    IsValidated = false;
            }
        }
        /// <summary>
        /// 用于记录表单名称
        /// </summary>
        string _name;
        /// <summary>
        /// 表单进行提交后校验内部所有控件
        /// </summary>
        public void ExcuteChildValidation(string name, DependencyObject ddo)
        {
            _name = name;

            Visual parentVisual = (Visual)ddo;
            while (VisualTreeHelper.GetParent(parentVisual) != null)
            {
                parentVisual = (Visual)VisualTreeHelper.GetParent(parentVisual);
            }

            EnumVisual(parentVisual);
        }
        /// <summary>
        /// 获取到当前需要校验的表单
        /// </summary>
        /// <param name="myVisual"></param>
        public void EnumVisual(Visual myVisual)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);

                if (childVisual.GetType() == typeof(SinGrid) && ((SinGrid)childVisual).Name == _name)
                    FindChildAndExcuteValidation(childVisual);
                else
                    EnumVisual(childVisual);
            }
        }

        /// <summary>
        /// 对表单内的输入控件进行校验
        /// </summary>
        /// <param name="Visual"></param>
        private void FindChildAndExcuteValidation(Visual Visual)
        {
            Panel panel = Visual as Panel;

            foreach (UIElement ui in panel.Children)
            {
                if (ui.GetType().BaseType == typeof(Panel))
                {
                    FindChildAndExcuteValidation(ui);
                }
                else
                {
                    if (ui.GetType() == typeof(SinTextBox))
                    {
                        ((SinTextBox)ui).IsDataError = false;
                        ((SinTextBox)ui).ValidateInput();
                    }
                    if (ui.GetType() == typeof(SinNumricTextBox))
                    {
                        ((SinNumricTextBox)ui).IsDataError = false;
                        ((SinNumricTextBox)ui).ValidateInput();
                    }
                    if (ui.GetType() == typeof(SinComboBox))
                    {
                        ((SinComboBox)ui).IsDataError = false;
                        ((SinComboBox)ui).ValidateInput();
                    }
                    if (ui.GetType() == typeof(SinDatePicker))
                    {
                        ((SinDatePicker)ui).IsDataError = false;
                        ((SinDatePicker)ui).ValidateInput();
                    }
                    if (ui.GetType() == typeof(DateTimePicker))
                    {
                        ((DateTimePicker)ui).ValidateInput();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 校验出错的类信息
    /// </summary>
    public class ValidateErrorInfoOfControl
    {
        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 校验结果
        /// </summary>
        public bool ValidateResult { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorInfo { get; set; }
    }
}
