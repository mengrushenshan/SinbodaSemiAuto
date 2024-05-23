using Sinboda.Framework.Common;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure.Model;
using Sinboda.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Sinboda.Framework.Core.AbstractClass;
using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Infrastructure.Interface;
using Microsoft.Practices.ServiceLocation;
using Sinboda.Framework.Control.Controls.Navigation;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.MainWindow.Blue.Views;

namespace Sinboda.Framework.MainWindow.Blue.ViewModels
{
    public class AppWindowViewModel : NavigationViewModelBase
    {
        private IPermission permission = new PermissionOperation();
        private ModuleMenuItem menuItemSelected;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AppWindowViewModel()
        {
            if (IsInDesignMode) return;

            ContentProvider = new ModuleNavigationContentProvider();
            ChangeUserCommand = new RelayCommand(ChangeUser);
            AboutInfoCommand = new RelayCommand(AboutInfo);
            HelpCommand = new RelayCommand(Help);
            CloseCommand = new RelayCommand(Close);
            MinimalityCommand = new RelayCommand(Minimality);
            Initialize();

            MinimalityVisibility = SystemResources.Instance.CurrentUserName == "dryf" ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            BootStrapper.Current.WindowCreated();
        }

        /// <summary>
        /// 获取关联的 <see cref="IMenuItemLoader"/>
        /// </summary>
        public IMenuItemLoader MenuItemLoader
        {
            get { return ServiceLocator.Current.GetInstance<IMenuItemLoader>(); }
        }

        private ImageSource imagesource;
        /// <summary>
        /// 公司log图片
        /// </summary>
        public ImageSource ImageSource
        {
            get { return imagesource; }
            set
            {
                imagesource = value;
                RaisePropertyChanged("ImageSource");
            }
        }

        private Visibility _MinimalityVisibility = Visibility.Collapsed;

        public Visibility MinimalityVisibility
        {
            get { return _MinimalityVisibility; }
            set { Set(ref _MinimalityVisibility, value); }
        }


        #region 数据源

        /// <summary>
        /// 获取关联的 <see cref="INavigationContentProvider"/>
        /// </summary>
        public INavigationContentProvider ContentProvider { get; private set; }
        /// <summary>
        /// 系统菜单
        /// </summary>
        public ObservableCollection<ModuleMenuItem> ModuleMenuItemSource { get; set; }

        /// <summary>
        /// 系统菜单选中
        /// </summary>
        public ModuleMenuItem MenuItemSelected
        {
            get { return menuItemSelected; }
            set
            {
                Set(ref menuItemSelected, value);
                if (value != null)
                    Navigate(MenuItemSelected.Id);
            }
        }

        /// <summary>
        /// 通过F1快捷键打开帮助文档
        /// </summary>
        public void ShowHelpByKeyF1()
        {
            Help();
        }

        #endregion

        #region 命令
        /// <summary>
        /// 切换用户命令
        /// </summary>
        public RelayCommand ChangeUserCommand { get; set; }
        /// <summary>
        /// 关于信息命令
        /// </summary>
        public RelayCommand AboutInfoCommand { get; set; }
        /// <summary>
        /// 最小化应用程序命令
        /// </summary>
        public RelayCommand MinimalityCommand { get; set; }
        /// <summary>
        /// 关闭应用程序命令
        /// </summary>
        public RelayCommand CloseCommand { get; set; }
        /// <summary>
        /// 帮助命令
        /// </summary>
        public RelayCommand HelpCommand { get; set; }
        /// <summary>
        /// 切换用户
        /// </summary>
        private void ChangeUser()
        {
            BootStrapper.Current.ChangeUser();
        }

        private void AboutInfo()
        {
            AboutView aboutView = new AboutView();
            aboutView.ShowDialog();
        }
        /// <summary>
        /// 调用Help文件
        /// </summary>
        private void Help()
        {
            string helpfile = string.Empty;
            //if (SystemResources.Instance.CurrentLanguage.ToLower().ToString() == "cn")
            //{
            //    helpfile = MapPath.HelpPath + "help_cn.chm";
            //}
            //else
            //{
            //    helpfile = MapPath.HelpPath + "help_en.chm";
            //}

            helpfile = MapPath.HelpPath + "help_" + SystemResources.Instance.CurrentLanguage.ToLower().ToString() + ".chm";

            try
            {
                System.Diagnostics.Process.Start(helpfile);
            }
            catch (System.Exception e)
            {
                NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[816], SystemResources.Instance.LanguageArray[2692], MessageBoxButton.OK, SinMessageBoxImage.Error); //816 错误 2692 文件不存在
            }
        }
        /// <summary>
        /// 关闭程序
        /// </summary>
        private void Close()
        {
            BootStrapper.Current.Shutdown();

        }


        private void Minimality()
        {
            BootStrapper.Current.CurrentApp.MainWindow.WindowState = WindowState.Minimized;
        }

        #endregion
    }
}
