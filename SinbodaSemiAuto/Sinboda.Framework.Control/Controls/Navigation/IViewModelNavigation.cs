using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Sinboda.Framework.Control.Controls.Navigation
{
    public interface IViewModelNavigation
    {
        /// <summary>
        /// 参数
        /// </summary>
        object Parameter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        NavigationItem CurrentNavigationItem { get; set; }
        /// <summary>
        /// 当执行来着当前视图模型的导航时执行此方法
        /// </summary>
        bool OnNavigatedFrom(object source, NavigationMode mode, object navigationState);
        /// <summary>
        /// 对当前视图模型进行导航时执行此方法
        /// </summary>
        void OnNavigatedTo();
    }
}
