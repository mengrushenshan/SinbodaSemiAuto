using Sinboda.Framework.Control.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sinboda.Framework.Control.Controls
{
    public class SinButton : Button
    {
        Timer _timer;

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(SinButton), new UIPropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ElementNameProperty =
            DependencyProperty.Register("ElementName", typeof(FrameworkElement), typeof(SinButton), new PropertyMetadata(null));


        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(SinButton), new PropertyMetadata(0));


        /// <summary>
        /// 延迟时间
        /// </summary>
        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public FrameworkElement ElementName
        {
            get { return (FrameworkElement)GetValue(ElementNameProperty); }
            set { SetValue(ElementNameProperty, value); }
        }

        protected override void OnClick()
        {
            base.OnClick();
            if (Interval > 0)
            {
                IsEnabled = false;
                _timer = new Timer(o =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        IsEnabled = true;
                        _timer.Dispose();
                        _timer = null;
                    });

                }, null, Interval, Timeout.Infinite);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            Visual parentVisual = (Visual)(this.Parent);
            while (VisualTreeHelper.GetParent(parentVisual) != null)
            {
                parentVisual = (Visual)VisualTreeHelper.GetParent(parentVisual);
                try
                {
                    if (parentVisual.ToString().Contains("PageView"))
                    {
                        #region
                        if (!string.IsNullOrEmpty(((UserControl)parentVisual).Name))
                        {
                            //LogHelper.logSoftWare.Debug("Operation   " + ((UserControl)parentVisual).Name);
                            OperationLogBusiness.Instance.WriteOperationLogToDb(((UserControl)parentVisual).Name, 1);
                        }
                        else
                        {
                            if (e.Source is SinButton)
                            {
                                //LogHelper.logSoftWare.Debug("Operation   " + parentVisual.ToString() + "  " + ((DrButton)e.Source).Content);
                                OperationLogBusiness.Instance.WriteOperationLogToDb(parentVisual.ToString() + "  " + ((SinButton)e.Source).Content, 1);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(((FrameworkElement)e.Source).Name))
                                {
                                    //LogHelper.logSoftWare.Debug("Operation   " + parentVisual.ToString() + "  " + ((FrameworkElement)e.Source).ToString());
                                    OperationLogBusiness.Instance.WriteOperationLogToDb(parentVisual.ToString() + "  " + ((FrameworkElement)e.Source).ToString(), 1);
                                }
                                else
                                {
                                    //LogHelper.logSoftWare.Debug("Operation   " + parentVisual.ToString() + "  " + ((FrameworkElement)e.Source).Name);
                                    OperationLogBusiness.Instance.WriteOperationLogToDb(parentVisual.ToString() + "  " + ((FrameworkElement)e.Source).Name, 1);
                                }
                            }
                        }
                        break;
                        #endregion
                    }
                    if (parentVisual.ToString().EndsWith("SetWin") || parentVisual.ToString().EndsWith("Window") || parentVisual is Window || parentVisual is SinWindow)
                    {
                        //LogHelper.logSoftWare.Debug("Operation   " + (!string.IsNullOrEmpty(((DrWindow)parentVisual).Title) ? ((DrWindow)parentVisual).Title : parentVisual.ToString()) + "  " + ((DrButton)e.Source).Content);
                        if (!string.IsNullOrEmpty(((Window)parentVisual).Title))
                        {
                            //LogHelper.logSoftWare.Debug("Operation   " + ((DrWindow)parentVisual).Title);
                            OperationLogBusiness.Instance.WriteOperationLogToDb(((Window)parentVisual).Title, 1);
                        }
                        else
                        {
                            if (e.Source is SinButton)
                            {
                                //LogHelper.logSoftWare.Debug("Operation   " + parentVisual.ToString() + "  " + ((DrButton)e.Source).Content);
                                OperationLogBusiness.Instance.WriteOperationLogToDb(parentVisual.ToString() + "  " + ((SinButton)e.Source).Content, 1);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(((FrameworkElement)e.Source).Name))
                                {
                                    //LogHelper.logSoftWare.Debug("Operation   " + parentVisual.ToString() + "  " + ((FrameworkElement)e.Source).ToString());
                                    OperationLogBusiness.Instance.WriteOperationLogToDb(parentVisual.ToString() + "  " + ((FrameworkElement)e.Source).ToString(), 1);
                                }
                                else
                                {
                                    //LogHelper.logSoftWare.Debug("Operation   " + parentVisual.ToString() + "  " + ((FrameworkElement)e.Source).Name);
                                    OperationLogBusiness.Instance.WriteOperationLogToDb(parentVisual.ToString() + "  " + ((FrameworkElement)e.Source).Name, 1);
                                }
                            }
                        }
                        break;
                    }
                }
                catch (Exception ex)
                {
                    //LogHelper.logSoftWare.Error("Operation   " + parentVisual.ToString() + "   ERR" + ex.ToString());
                    break;
                }
            }

            if (Source != null)
            {
                ((SinGrid)Source).ExcuteChildValidation(((SinGrid)Source).Name, this.Parent);
                if (!((SinGrid)Source).IsValidated)
                    e.Handled = true;
            }


        }
    }
}
