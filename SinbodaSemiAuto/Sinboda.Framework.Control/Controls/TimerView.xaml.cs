using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Sinboda.Framework.Control.Controls
{
    /// <summary>
    /// TimerView.xaml 的交互逻辑
    /// </summary>
    public partial class TimerView : UserControl
    {
        /// <summary>
        /// 标识 <see cref="TimeFontSize"/> 的依赖性属性
        /// </summary>
        public static readonly DependencyProperty TimeFontSizeProperty = DependencyProperty.Register("TimeFontSize", typeof(double), typeof(TimerView), new PropertyMetadata(20d));

        /// <summary>
        /// 字体大小
        /// </summary>
        public double TimeFontSize
        {
            get { return (double)GetValue(TimeFontSizeProperty); }
            set { SetValue(TimeFontSizeProperty, value); }
        }


        private Timer timer;
        /// <summary>
        /// 构造函数
        /// </summary>
        public TimerView()
        {
            InitializeComponent();
            timer = new Timer(RefTime, null, 0, 1000);
        }

        private void RefTime(object s)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                text.Text = DateTime.Now.ToString();
            }));
        }
    }
}
