using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Sinboda.Framework.Control.ItemSelection;

namespace Sinboda.Framework.Control.Converts
{
    /// <summary>
    /// 增量减量转换
    /// </summary>
    public class IncrementIconConverter : IValueConverter
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
            var tag = (SampleVolumeFlag)value;
            ImageSource key = null;

            switch (tag)
            {
                case SampleVolumeFlag.Normal:
                    key = null;
                    break;
                case SampleVolumeFlag.Decrement:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/down.png", UriKind.RelativeOrAbsolute));
                    break;
                case SampleVolumeFlag.InCrement:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/up.png", UriKind.RelativeOrAbsolute));
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
    /// <summary>
    /// 按钮面板图片标识
    /// </summary>
    public class MarkIconConverter : IValueConverter
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
            var tag = (MaskShowFlag)value;
            ImageSource key = null;

            switch (tag)
            {
                case MaskShowFlag.None:
                    key = null;
                    break;
                case MaskShowFlag.X:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/X.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.R:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/R.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.S:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/S.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.C:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/C.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.XR:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/XR.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.XC:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/XC.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.RC:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/CR.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.SC:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/CS.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.RS:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/RS.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.XS:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/XS.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.XRC:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/XCR.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.RSC:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/CRS.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.XSC:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/XCS.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.XRS:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/XRS.png", UriKind.RelativeOrAbsolute));
                    break;
                case MaskShowFlag.XRSC:
                    key = new BitmapImage(new Uri("/Sinboda.Framework.Control;component/Images/XCRS.png", UriKind.RelativeOrAbsolute));
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
