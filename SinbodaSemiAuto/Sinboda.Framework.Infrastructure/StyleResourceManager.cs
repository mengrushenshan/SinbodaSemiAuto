using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 资源样式管理
    /// </summary>
    public static class StyleResourceManager
    {
        /// <summary>
        /// 缓存当前主题名称
        /// </summary>
        public static string currentThemeName;
        /// <summary>
        /// 缓存当前字体资源
        /// </summary>
        private static ResourceDictionary currentFontResource;
        /// <summary>
        /// 缓存当前主题资源
        /// </summary>
        private static ResourceDictionary currentThemeResouce;
        /// <summary>
        /// 设置皮肤
        /// </summary>
        /// <param name="themeName">皮肤名称</param>
        /// <param name="fontSize">字体大小</param>
        /// <returns></returns>
        public static bool SetTheme(string themeName, string fontSize)
        {
            currentThemeName = themeName;

            SetFontSize(fontSize);

            ResourceDictionary resourceDictionary = new ResourceDictionary() { Source = GetThemePath(themeName) };
            if (currentThemeResouce == null)
            {
                currentThemeResouce = resourceDictionary;
                Application.Current.Resources.MergedDictionaries.Add(currentThemeResouce);
                return true;
            }
            else
                return true;

        }

        /// <summary>
        /// 设置字体大小
        /// </summary>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public static bool SetFontSize(string fontSize)
        {
            //字体资源处理
            ResourceDictionary tmp = new ResourceDictionary() { Source = GetThemePath(currentThemeName, fontSize) };
            if (currentFontResource == null)
            {
                currentFontResource = tmp;
                Application.Current.Resources.MergedDictionaries.Insert(0, currentFontResource);
            }
            if (currentFontResource != null && tmp != null && currentFontResource.Source != tmp.Source)
            {
                if (Application.Current.Resources.MergedDictionaries.Contains(currentFontResource))
                {
                    Application.Current.Resources.MergedDictionaries.Remove(currentFontResource);
                }
                currentFontResource = tmp;
                Application.Current.Resources.MergedDictionaries.Insert(0, currentFontResource);

                return true;
            }
            else
                return true;
        }
        /// <summary>
        /// 获取除字体以外样式资源
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        private static Uri GetThemePath(string themeName)
        {
            return new Uri(string.Format("/Sinboda.Theme.{0};component/Themes/Generic.xaml", themeName), UriKind.RelativeOrAbsolute);
        }
        /// <summary>
        /// 获取字体样式资源
        /// </summary>
        /// <param name="themeName"></param>
        /// <param name="fileNamePre"></param>
        /// <returns></returns>
        private static Uri GetThemePath(string themeName, string fileNamePre)
        {
            return new Uri(string.Format("/Sinboda.Theme.{0};component/Themes/Fonts/{1}Font.xaml", themeName, fileNamePre), UriKind.RelativeOrAbsolute);
        }
    }
}
