using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Model
{
    /// <summary>
    /// 
    /// </summary>
    public enum ResourceType
    {
        /// <summary>
        /// 
        /// </summary>
        Font,
        /// <summary>
        /// 
        /// </summary>
        Theme,
    }
    /// <summary>
    /// 
    /// </summary>
    public class StyleResource
    {
        /// <summary>
        /// 
        /// </summary>
        public ResourceType ResourceTypes { get; private set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; private set; }
        /// <summary>
        /// 完整名称
        /// </summary>
        public string AssemblyFullName { get; private set; }
        /// <summary>
        /// 主题小图标
        /// </summary>
        public Uri SmallGlyph { get; private set; }
        /// <summary>
        /// 主题大图标
        /// </summary>
        public Uri LargeGlyph { get; private set; }
        /// <summary>
        /// 皮肤资源路径
        /// </summary>
        public Uri StyleSourcePath { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="themeName"></param>
        /// <param name="assemblyFullName"></param>
        public StyleResource(string themeName, string assemblyFullName)
        {
            DisplayName = themeName;
            AssemblyFullName = assemblyFullName;
            SmallGlyph = GetThemeGlyphUri(DisplayName, false);
            LargeGlyph = GetThemeGlyphUri(DisplayName, true);
            StyleSourcePath = GetThemePath(DisplayName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="themeName"></param>
        /// <param name="assemblyFullName"></param>
        /// <param name="fileNamePre"></param>
        public StyleResource(string themeName, string assemblyFullName, string fileNamePre)
        {
            DisplayName = themeName;
            AssemblyFullName = assemblyFullName;
            SmallGlyph = GetThemeGlyphUri(DisplayName, false);
            LargeGlyph = GetThemeGlyphUri(DisplayName, true);
            StyleSourcePath = GetThemePath(DisplayName, fileNamePre);
        }

        private Uri GetThemeGlyphUri(string themeName, bool isLarge)
        {
            string small = "_16x16.png";
            string large = "_48x48.png";
            return new Uri(string.Format("/Sinboda.Theme.{0};component/Themes/Images/{0}{1}", themeName, isLarge ? large : small), UriKind.Relative);
        }

        private Uri GetThemePath(string themeName)
        {
            return new Uri(string.Format("/Sinboda.Theme.{0};component/Themes/Generic.xaml", themeName), UriKind.RelativeOrAbsolute);
        }

        private Uri GetThemePath(string themeName, string fileNamePre)
        {
            return new Uri(string.Format("/Sinboda.Theme.{0};component/Themes/Fonts/{1}Font.xaml", themeName, fileNamePre), UriKind.RelativeOrAbsolute);
        }
    }
}
