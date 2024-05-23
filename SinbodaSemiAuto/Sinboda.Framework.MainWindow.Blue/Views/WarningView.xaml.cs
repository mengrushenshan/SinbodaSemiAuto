using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Infrastructure;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sinboda.Framework.MainWindow.Blue.Views
{
    /// <summary>
    /// WarningView.xaml 的交互逻辑
    /// </summary>
    public partial class WarningView : UserControl
    {
        private Storyboard beginAnimation;
        private AlarmLevel currentLevel = AlarmLevel.None;

        /// <summary>
        /// 标识 Level 的依赖性属性
        /// </summary>
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level", typeof(AlarmLevel), typeof(WarningView), new PropertyMetadata(AlarmLevel.None, LevelChanged));

        /// <summary>
        /// 报警级别
        /// </summary>
        public AlarmLevel Level
        {
            get { return (AlarmLevel)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        /// <summary>
        /// 当前的报警级别
        /// </summary>
        public AlarmLevel CurrentLevel
        {
            get { return currentLevel; }
            private set { currentLevel = value; }
        }

        /// <summary>
        /// 当前图标
        /// </summary>
        protected Image CurrentImage
        {
            get
            {
                switch (currentLevel)
                {
                    case AlarmLevel.Stop:
                        return img_error;
                    case AlarmLevel.Warning:
                        return img_warning;
                    default:
                        return img_none;
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WarningView()
        {
            InitializeComponent();
            beginAnimation = Resources["BeginAnimation"] as Storyboard;
            beginAnimation.Completed += BeginAnimation_Completed;
        }

        private void BeginAnimation_Completed(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 切换报警状态
        /// </summary>
        /// <param name="alarmLevel"></param>
        public void Alarm(AlarmLevel alarmLevel)
        {
            if (DesignHelper.IsInDesignMode)
                return;

            if (alarmLevel == CurrentLevel)
                return;

            if (alarmLevel == AlarmLevel.Warning && CurrentLevel == AlarmLevel.Stop)
                return;

            StopAnimation(CurrentLevel);
            CurrentLevel = alarmLevel;
            BeginAnimation(CurrentLevel);
        }

        /// <summary>
        /// <seealso cref="Level"/> 属性值发生变化时触发
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void LevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var WarningView = d as WarningView;
            if (d == null) return;

            WarningView.Alarm((AlarmLevel)e.NewValue);
        }

        /// <summary>
        /// 开始动画
        /// </summary>
        /// <param name="alarmLevel"></param>
        private void BeginAnimation(AlarmLevel alarmLevel)
        {
            if (alarmLevel == AlarmLevel.None)
            {
                img_none.Opacity = 1;
            }
            else
            {
                img_none.Opacity = 0;
                Storyboard.SetTargetName(beginAnimation, CurrentImage.Name);
                beginAnimation.Begin();
            }
        }

        /// <summary>
        /// 停止动画
        /// </summary>
        private void StopAnimation(AlarmLevel alarmLevel)
        {
            if (alarmLevel != AlarmLevel.None)
                CurrentImage.BeginAnimation(OpacityProperty, null);
        }

        /// <summary>
        /// 双击时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 停止动画重置报警级别,并跳转到报警页面
            StopAnimation(CurrentLevel);
            CurrentLevel = AlarmLevel.None;
            BeginAnimation(CurrentLevel);
            var item = NavigationHelper.Cuurrent.GetNavigationItem("AlarmsInfoPageView");
            if (item != null)
            {
                NavigationServiceExBase.CurrentService.Navigate(NavigationHelper.Cuurrent.GetNavigationItem("AlarmsInfoPageView"));
                Messenger.Default.Send<bool>(true, "ClearTopMenuSelectedStatus");
                Messenger.Default.Send<string>("StopBuzzerFromPlatFormByEvent", "StopBuzzerFromPlatFormByEvent");
            }
        }
    }
}
