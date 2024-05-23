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

namespace Sinboda.Framework.Control.DateTimePickers
{
    /// <summary>
    /// IconButton.xaml 的交互逻辑
    /// </summary>
    public partial class IconButton : UserControl
    {
        public IconButton()
        {
            InitializeComponent();

            this.button.Click += delegate
            {
                RoutedEventArgs newEvent = new RoutedEventArgs(IconButton.ClickEvent, this);
                this.RaiseEvent(newEvent);
            };
        }

        #region 命令
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(IconButton), new PropertyMetadata(null, OnSelectCommandChanged));
        /// <summary>
        /// 
        /// </summary>
        public ICommand Command
        {
            set { SetValue(CommandProperty, value); }
            get { return (ICommand)GetValue(CommandProperty); }
        }
        private static void OnSelectCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            IconButton btn = obj as IconButton;
            if (btn == null)
            {
                return;
            }
            btn.button.Command = (ICommand)args.NewValue;
        }

        #endregion

        #region 点击事件
        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedEvent ClickEvent =
           EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(IconButton));

        /// <summary>
        /// 
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { base.AddHandler(ClickEvent, value); }
            remove { base.RemoveHandler(ClickEvent, value); }
        }
        #endregion
    }
}
