using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Sinboda.SemiAuto.Core.Models;

namespace Sinboda.SemiAuto.View.Converter
{
    public class InfoTypeConverter : IValueConverter
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
            var tag = (VerificationLevel)value;
            ImageSource key = null;

            switch (tag)
            {
                case VerificationLevel.Error:
                case VerificationLevel.Fusing:
                    key = new BitmapImage(new Uri("/Sinboda.Theme.Blue;component/images/system/Error.png", UriKind.RelativeOrAbsolute));
                    break;
                case VerificationLevel.Warning:
                    key = new BitmapImage(new Uri("/Sinboda.Theme.Blue;component/images/system/warning.png", UriKind.RelativeOrAbsolute));
                    break;
                default:
                    key = null;
                    break;
            }
            return key;
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
}
