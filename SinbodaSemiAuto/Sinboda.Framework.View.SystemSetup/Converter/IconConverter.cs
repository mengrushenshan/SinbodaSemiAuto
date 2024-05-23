using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Sinboda.Framework.View.SystemSetup.Converter
{
    /// <summary>
    /// 
    /// </summary>
    public class IconConverter : IValueConverter
    {
        /// <summary>
        /// 角色转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "";

            // TODO：暂时只处理矢量图标
            if (string.IsNullOrEmpty(value.ToString()))
                return string.Empty;

            Geometry data;
            var controlStyles = Application.Current.Resources.MergedDictionaries[1].MergedDictionaries[1];
            var glyphicons = controlStyles.MergedDictionaries[2];
            data = glyphicons[value.ToString()] as Geometry;
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameters"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameters, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
