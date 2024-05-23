using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Sinboda.Framework.View.SystemSetup.Converter
{
    /// <summary>
    /// 
    /// </summary>
    public class VisibilityConverter : IValueConverter
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
                return "Visible";

            string visibility = string.Empty;

            switch ((bool)value)
            {
                case true:
                    visibility = "Visible";
                    break;
                case false:
                    visibility = "Hidden";
                    break;
            }

            return visibility;
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

    /// <summary>
    /// 
    /// </summary>
    public class VisibleConverter : IValueConverter
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
                return true;

            bool visible = true;

            switch ((Visibility)value)
            {
                case Visibility.Visible:
                    visible = true;
                    break;
                case Visibility.Hidden:
                    visible = false;
                    break;
                case Visibility.Collapsed:
                    visible = false;
                    break;
            }

            return visible;
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
