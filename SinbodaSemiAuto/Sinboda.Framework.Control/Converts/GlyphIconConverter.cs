using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace Sinboda.Framework.Control.Converts
{
    /// <summary>
    /// 
    /// </summary>
    public static class SinConverts
    {
        private static GlyphIconConverter _GlyphIconConverter = null;
        /// <summary>
        /// 
        /// </summary>
        public static GlyphIconConverter GlyphIconConverter
        {
            get
            {
                if (_GlyphIconConverter == null)
                    _GlyphIconConverter = new GlyphIconConverter();
                return _GlyphIconConverter;
            }
        }

        private static DoubleToGridLenghtConverter _DoubleToGridLenghtConverter = null;
        /// <summary>
        /// 
        /// </summary>
        public static DoubleToGridLenghtConverter DoubleToGridLenghtConverter
        {
            get
            {
                if (_DoubleToGridLenghtConverter == null)
                    _DoubleToGridLenghtConverter = new DoubleToGridLenghtConverter();
                return _DoubleToGridLenghtConverter;
            }
        }
    }


    /// <summary>
    /// 字符串与矢量图标的转换
    /// </summary>
    public class GlyphIconConverter : IValueConverter
    {
        /// <summary>
        /// 字符串转换为矢量图标
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Geometry pathGeometry = Application.Current.Resources[value.ToString()] as Geometry;
            return pathGeometry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleToGridLenghtConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double v = System.Convert.ToDouble(value);
                if (double.IsNaN(v))
                    return GridLength.Auto;

                return new GridLength(v);
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("DoubleToGridLengthConvert error", e);
                return GridLength.Auto;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GridLength gridLength = (GridLength)value;
            return gridLength.Value;
        }
    }
}
