using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Control.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sinboda.Framework.Control.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class SinTabControl : TabControl
    {
        /// <summary>
        /// 重写 切换tab页时记录页面name及tab页签头
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (e.Source.GetType() == typeof(SinTabControl))
            {

                Visual parentVisual = (Visual)(this.Parent);
                while (VisualTreeHelper.GetParent(parentVisual) != null)
                {
                    try
                    {
                        parentVisual = (Visual)VisualTreeHelper.GetParent(parentVisual);
                        if (parentVisual.ToString().Contains("PageView") || parentVisual.ToString().Contains("HomeView"))//pageview为主窗体加载页面后缀  homeview为主窗体页面后缀
                        {
                            //LogHelper.logSoftWare.Debug("Operation   " + (!string.IsNullOrEmpty(((UserControl)parentVisual).Name) ? ((UserControl)parentVisual).Name : parentVisual.ToString()) + "  " + ((DrTabItem)e.AddedItems[0]).Header);
                            OperationLogBusiness.Instance.WriteOperationLogToDb((!string.IsNullOrEmpty(((UserControl)parentVisual).Name) ? ((UserControl)parentVisual).Name : parentVisual.ToString()) + "  " + ((SinTabItem)e.AddedItems[0]).Header, 1);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.logSoftWare.Error("Operation   " + parentVisual.ToString() + "   ERR" + ex.ToString());
                        break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class SinTabItem : TabItem
    {
    }
}
