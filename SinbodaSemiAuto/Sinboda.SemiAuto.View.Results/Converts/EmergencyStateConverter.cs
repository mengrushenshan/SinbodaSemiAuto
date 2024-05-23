using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace Sinboda.SemiAuto.View.Results.Converts
{
    public static class DisplayElement
    {
        /// <summary>
        /// 创建急症显示元素
        /// </summary>
        /// <param name="isEmergency"></param>
        /// <returns></returns>
        public static FrameworkElement CreateEmergencyElement(bool isEmergency)
        {
            if (!isEmergency)
                return new ContentControl();

            return CreatePate("Glyphicon-Ok", "#FF4A88CD");
        }

        /// <summary>
        /// 创建打印显示元素
        /// </summary>
        /// <param name="isPrint"></param>
        /// <returns></returns>
        public static FrameworkElement CreatePrintElement(bool isPrint)
        {
            if (!isPrint)
                return new ContentControl();

            return CreatePate("Glyphicon-Ok", "#FF4A88CD");
        }

        /// <summary>
        /// 创建审核元素
        /// </summary>
        /// <param name="isAudit"></param>
        /// <returns></returns>
        public static FrameworkElement CreateAuditElement(bool isAudit)
        {
            if (!isAudit)
                return new ContentControl();

            return CreatePate("Glyphicon-AutoAuditSetting", "#FF5FC948", 20);
        }

        public static FrameworkElement CreateLisElement(bool isLis)
        {
            if (!isLis)
                return new ContentControl();

            return CreatePate("Glyphicon-Ok", "#FF4A88CD");
        }

        public static BitmapImage CreateImageSource(string imgName)
        {
            return new BitmapImage(new Uri($"/Sinboda.SemiAuto.View.Results;component/Images/{imgName}.png", UriKind.RelativeOrAbsolute));
        }

        private static Path CreatePate(string name, string color, double size = 15)
        {
            return new Path
            {
                Data = Application.Current.Resources[name] as Geometry,
                Stretch = Stretch.Uniform,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)),
                Height = size
            };
        }
    }
}
