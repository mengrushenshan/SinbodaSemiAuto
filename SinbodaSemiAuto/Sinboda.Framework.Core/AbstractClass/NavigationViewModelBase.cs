using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Sinboda.Framework.Control.Controls.Navigation;
using Sinboda.Framework.Core.Interface;
using Sinboda.Framework.Control.Controls;

namespace Sinboda.Framework.Core.AbstractClass
{
    public class NavigationViewModelBase : ViewModelBase, IViewModelNavigation
    {
        private object paramter = new object();
        private INavigationServiceEx m_navigationService;

        /// <summary>
        /// 导航参数
        /// </summary>
        public object Parameter
        {
            get { return paramter; }
            set
            {
                paramter = value;
                OnParameterChanged(paramter);
            }
        }

        /// <summary>
        /// 返回关联的 <see cref="INavigationServiceEx"/> 实例
        /// </summary>
        protected INavigationServiceEx NavigationService
        {
            get
            {
                if (m_navigationService == null)
                {
                    m_navigationService = NavigationServiceExBase.CurrentService;
                }
                return m_navigationService;
            }
        }

        /// <summary>
        /// 对当前视图模型进行导航时执行此方法（暂时未实现）
        /// </summary>
        public virtual void OnNavigatedTo()
        { }

        /// <summary>
        /// 当执行来着当前视图模型的导航时执行此方法
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="mode">导航方式</param>
        /// <param name="navigationState">导航参数</param>
        /// <returns></returns>
        protected virtual bool NavigatedFrom(object source, NavigationMode mode, object navigationState)
        {
            return true;
        }

        /// <summary>
        /// 当 <seealso cref="Parameter"/> 属性发生变化时触发
        /// </summary>
        /// <param name="parameter"></param>
        protected virtual void OnParameterChanged(object parameter)
        { }

        /// <summary>
        /// 
        /// </summary>
        public void Navigate(string target, string title = "", object param = null)
        {
            var navItem = NavigationHelper.Cuurrent.GetNavigationItem(target);
            if (navItem == null)
            {
                // 如果不在缓存中，说明是自定义子页面
                var viewModelNavigation = this as IViewModelNavigation;
                var item = viewModelNavigation.CurrentNavigationItem == null ? NavigationHelper.Cuurrent.RootItem : viewModelNavigation.CurrentNavigationItem;
                navItem = new NavigationItem()
                {
                    Name = title,
                    Source = target,
                    NavigationParameter = param,
                    ParentItem = item
                };
            }
            navItem.NavigationParameter = param;
            Navigate(navItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navItem"></param>
        public void Navigate(NavigationItem navItem)
        {
            NavigationService.Navigate(navItem);
        }

        #region IViewModelNavigation

        NavigationItem IViewModelNavigation.CurrentNavigationItem { get; set; }

        bool IViewModelNavigation.OnNavigatedFrom(object source, NavigationMode mode, object navigationState)
        {
            return NavigatedFrom(source, mode, navigationState);
        }
        #endregion

        #region 消息显示并存入操作日志
        /// <summary>
        /// 当前窗口标题（在构造函数内进行赋值）
        /// </summary>
        public string PageTitle { get; set; }
        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessageInfo(string message)
        {
            NotificationService.Instance.ShowMessage(message, MessageBoxButton.OK, SinMessageBoxImage.Information);
        }
        /// <summary>
        /// 显示完成信息
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessageComplete(string message)
        {
            NotificationService.Instance.ShowMessage(message, MessageBoxButton.OK, SinMessageBoxImage.Completed);
        }
        /// <summary>
        /// 显示警告信息
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessageWarning(string message)
        {
            NotificationService.Instance.ShowMessage(message, MessageBoxButton.OK, SinMessageBoxImage.Warning);
        }
        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessageError(string message)
        {
            NotificationService.Instance.ShowMessage(message, MessageBoxButton.OK, SinMessageBoxImage.Error);
        }
        /// <summary>
        /// 显示疑问信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="msb"></param>
        public void ShowMessageQuestion(string message, MessageBoxButton msb)
        {
            NotificationService.Instance.ShowMessage(message, msb, SinMessageBoxImage.Question);
        }
        /// <summary>
        /// 显示完成信息并记录操作日志（不带有TabPage页）
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessageCompleteAndLog(string message)
        {
            SystemResources.Instance.SysLogInstance.WriteLogDb(message, SysLogType.Operater);
            NotificationService.Instance.ShowMessage(message, MessageBoxButton.OK, SinMessageBoxImage.Completed);
        }
        /// <summary>
        /// 显示完成信息并记录操作日志（带有TabPage页）
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tabPageTitle"></param>
        public void ShowMessageCompleteAndLog(string message, string tabPageTitle)
        {
            SystemResources.Instance.SysLogInstance.WriteLogDb(message, SysLogType.Operater);
            NotificationService.Instance.ShowMessage(message, MessageBoxButton.OK, SinMessageBoxImage.Completed);
        }
        /// <summary>
        /// 显示完成信息并记录操作日志（不带有TabPage页）
        /// </summary>
        /// <param name="message"></param>
        public void WriteOperateLog(string message)
        {
            SystemResources.Instance.SysLogInstance.WriteLogDb(message, SysLogType.Operater);
        }
        /// <summary>
        /// 显示完成信息并记录操作日志（带有TabPage页）
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tabPageTitle"></param>
        public void WriteOperateLog(string message, string tabPageTitle)
        {
            SystemResources.Instance.SysLogInstance.WriteLogDb(message, SysLogType.Operater);
        }
        #endregion
    }
}
