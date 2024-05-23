using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Core.AbstractClass;
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

namespace Sinboda.Framework.MainWindow.Blue.Views
{
    /// <summary>
    /// StateView.xaml 的交互逻辑
    /// </summary>
    public partial class StateView : UserControl
    {
        /// <summary>
        /// 初始化 <see cref="StateView"/> 类型的实例
        /// </summary>
        public StateView()
        {
            InitializeComponent();
            Messenger.Default.Register<AnalyzerCurrentStateInfo>(this, MessageNameBase.SetAnalyzerCurrentStateInfo, StartRemainTimer);
        }

        #region 仪器状态属性及处理
        /// <summary>
        /// 计时器
        /// </summary>
        private DispatcherTimer timer;
        /// <summary>
        /// 倒计时处理类
        /// </summary>
        RemainTimer remainTimerCountProcess;
        /// <summary>
        /// 倒计时委托
        /// </summary>
        /// <returns></returns>
        public delegate bool CountDownHandler();
        /// <summary>
        /// 处理事件
        /// </summary>
        public event CountDownHandler CountDown;
        /// <summary>
        /// 开启定时器
        /// </summary>
        /// <param name="info"></param>
        public void StartRemainTimer(AnalyzerCurrentStateInfo info)
        {
            if (timer != null)
            {
                timer.Stop();
                remainTime.Text = "";
            }

            if (info.RemainSeconds > 0)
            {
                //设置定时器 此为wpf定时器
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(10000000);   //时间间隔为一秒
                timer.Tick += new EventHandler(timer_Tick);
                //处理倒计时的类
                remainTimerCountProcess = new RemainTimer(info.RemainSeconds);
                CountDown += new CountDownHandler(remainTimerCountProcess.ProcessRemainTimerDown);
                //开启定时器
                timer.Start();
            }
        }
        /// <summary>
        /// Timer触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (OnCountDown())
            {
                remainTime.Text = remainTimerCountProcess.GetHour() + ":" + remainTimerCountProcess.GetMinute() + ":" + remainTimerCountProcess.GetSecond();
            }
            else
            {
                remainTime.Text = "";
                timer.Stop();
            }
        }
        /// <summary>
        /// 事件处理
        /// </summary>
        /// <returns></returns>
        public bool OnCountDown()
        {
            if (CountDown != null)
                return CountDown();
            return false;
        }
        #endregion
    }
}
