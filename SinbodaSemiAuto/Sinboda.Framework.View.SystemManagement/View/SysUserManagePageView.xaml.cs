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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sinboda.Framework.View.SystemManagement.View
{
    /// <summary>
    /// 矢量图
    /// </summary>
    internal class FontIconInfo
    {
        /// <summary>
        /// 图标
        /// </summary>
        public Geometry Icon { get; set; }
        /// <summary>
        /// 文字
        /// </summary>
        public string Text { get; set; }
    }

    /// <summary>
    /// SysUserManagePageView.xaml 的交互逻辑
    /// </summary>
    public partial class SysUserManagePageView : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SysUserManagePageView()
        {
            InitializeComponent();
            ResourceDictionary resourceDictionary = new ResourceDictionary()
            {
                Source = new Uri(string.Format("/Sinboda.Theme.{0};component/Themes/Glyphicons.xaml", StyleResourceManager.currentThemeName), UriKind.RelativeOrAbsolute)
            };
            //寻找资源文件中为Glyphicons.xaml的文件
            var glyphicons = resourceDictionary;

            List<FontIconInfo> source = new List<FontIconInfo>();
            foreach (var key in glyphicons.Keys.AsQueryable())
            {
                FontIconInfo info = new FontIconInfo();
                info.Text = key.ToString();
                info.Icon = (Geometry)glyphicons[key];
                source.Add(info);
            }

            txtIconCommon.ItemsSource = source.OrderBy(o => o.Text);
        }
    }
}
