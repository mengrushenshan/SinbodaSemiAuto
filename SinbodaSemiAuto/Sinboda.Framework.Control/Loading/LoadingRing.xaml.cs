using System;
using System.Windows;


namespace Sinboda.Framework.Control.Loading
{
    /// <summary>
    /// LoadingRing.xaml 的交互逻辑
    /// </summary>

    [TemplateVisualState(GroupName = GroupActiveStates, Name = StateInactive)]
    [TemplateVisualState(GroupName = GroupActiveStates, Name = StateActive)]
    public partial class LoadingRing : System.Windows.Controls.Control
    {
        private const string GroupActiveStates = "ActiveStates";
        private const string StateInactive = "Inactive";
        private const string StateActive = "Active";
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(LoadingRing), new PropertyMetadata(false, OnIsActiveChanged));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register("DisplayText", typeof(string), typeof(LoadingRing), new PropertyMetadata(string.Empty));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CurrentValueProperty = DependencyProperty.Register("CurrentValue", typeof(double), typeof(LoadingRing), new PropertyMetadata(0d, OnCurrentValueChanged));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(LoadingRing), new PropertyMetadata(100d));

        #region 依赖性属性
        /// <summary>
        /// 
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        /// <summary>
        /// 显示值
        /// </summary>
        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        /// <summary>
        /// 当前值
        /// </summary>
        public double CurrentValue
        {
            get { return (double)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }
        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        public LoadingRing()
        {
            DefaultStyleKey = typeof(LoadingRing);
        }

        private void GotoCurrentState(bool animate)
        {
            var state = IsActive ? StateActive : StateInactive;
            VisualStateManager.GoToState(this, state, animate);
        }
        /// <summary>
        /// 应用模版触发
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GotoCurrentState(false);
        }
        /// <summary>
        /// 显示进度
        /// </summary>
        public void ShowValue()
        {
            DisplayText = string.Format("{0}%", Math.Round((CurrentValue / MaxValue) * 100));
        }

        private static void OnIsActiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((LoadingRing)o).GotoCurrentState(true);
        }

        private static void OnCurrentValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LoadingRing c = o as LoadingRing;
            if (c == null) return;

            c.ShowValue();
        }
    }
}
