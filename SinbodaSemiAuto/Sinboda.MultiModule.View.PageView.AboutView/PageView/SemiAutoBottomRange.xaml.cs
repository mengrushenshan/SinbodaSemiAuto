using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using log4net;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Core.Manager;
using Sinboda.SemiAuto.Core.Resources;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.TestFlow.Manager;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sinboda.SemiAuto.View.PageView
{
    /// <summary>
    /// SemiAutoBottomRange.xaml 的交互逻辑
    /// </summary>
    public partial class SemiAutoBottomRange : UserControl
    {
        private Timer timer;
        public SemiAutoBottomRange()
        {
            InitializeComponent();
            Loaded += MultiModuleBottomRange_Loaded;
            Unloaded += MultiModuleBottomRange_Unloaded;

            itemsControl.ItemTemplateSelector = new ModuleDataTemplateSelector();
            itemsControl.SetBinding(ItemsControl.ItemsSourceProperty, new Binding()
            {
                Source = MultiModuleManager.Instance.MuduleList,
                NotifyOnSourceUpdated = true,
                NotifyOnTargetUpdated = true
            });

            if (timer != null)
            {
                LogHelper.logSoftWare.Debug($"重置定时器 {DateTime.Now.ToString()}");
                timer.Dispose();
                timer = null;
            }
            timer = new Timer(RefTime, null, 0, 1000);

            Messenger.Default.Register<object>(this, "NotifyAlarmRefresh", (messenger) =>
            {
                AlarmHistoryInfoModel info = null;
                if (null != messenger && messenger is AlarmHistoryInfoModel)
                {
                    info = messenger as AlarmHistoryInfoModel;
                }

                RefreshAlarmInfo(info);
            });
        }

        private void MultiModuleBottomRange_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 注销后关闭定时器
                if (timer != null)
                    timer.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Debug("图标刷新注销消息：" + ex);
            }
        }

        private void MultiModuleBottomRange_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Debug("图标刷新注册消息：" + ex);
            }
        }

        private void RefTime(object s)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DateTime newDate = DateTime.Now;
                txtTime.Text = newDate.ToLongTimeString();
                txtDate.Text = newDate.ToShortDateString();
            }));
        }

        /// <summary>
        /// 仪器连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SystemResources.Instance.AnalyzerConnectionState)
            {
               
            }
            else
            {
                
            }
        }
        /// <summary>
        /// LIS连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
           

            if (SystemResources.Instance.LISConnectionState)
            {
                //确定要断开LIS连接吗？
                //if (NotificationService.Instance.ShowQuestion(SystemResources.Instance.LanguageArray[6186]) == MessageBoxResult.Yes)
                //{
                //    if (LISManager.Instance.DisConnect())
                //    {
                //        SystemResources.Instance.LISConnectionState = false;
                //    }
                //}

               
            }
            else
            {
                
            }
        }

        /// <summary>
        /// 刷新报警跑马灯信息
        /// </summary>
        /// <param name="alarmInfo"></param>
        public void RefreshAlarmInfo(AlarmHistoryInfoModel alarmInfo)
        {
            if (null != alarmInfo)
            {
                string strInfo = string.Empty;

                if (!string.IsNullOrEmpty(alarmInfo.Info))
                {
                    strInfo += alarmInfo.Info;
                }

                Task.Run(() => DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    if (!string.IsNullOrEmpty(strInfo))
                    {
                        text_FireMachineStatue.Text = strInfo;
                    }
                }));
            }
            else
            {
                Task.Run(() => DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    text_FireMachineStatue.Text = string.Empty;
                }));

            }
        }

        private void WarningView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            text_FireMachineStatue.Text = string.Empty;
        }
    }

    /// <summary>
    /// 连接状态转换类
    /// </summary>
    public class ConnectStatusConverter : IValueConverter
    {
        /// <summary>
        /// 状态转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tag = (bool)value;
            return Application.Current.Resources[tag ? "green" : "grey"] as ControlTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ModuleDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ModuleInfoModel minfo = item as ModuleInfoModel;
            var fe = container as FrameworkElement;

            if (minfo.ModuleTypeCode == (int)ProductType.Sinboda001)
                return (DataTemplate)fe.FindResource("SemiAutoDataTemplate");
            

            return base.SelectTemplate(item, container);
        }
    }
}
