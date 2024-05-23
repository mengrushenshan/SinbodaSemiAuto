using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Sinboda.Framework.MainWindow.Blue.Converters
{
    /// <summary>
    /// 连接状态转换类
    /// </summary>
    public class ConnectStatusConverter : IValueConverter
    {
        /// <summary>
        /// 状态转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var tag = (bool)value;
            return Application.Current.Resources[tag ? "green" : "red"] as ControlTemplate;

            //var tag = (bool)value;
            //string key = string.Empty;

            //if (parameter.ToString().Equals("analyzer"))
            //    key = tag ? "Instrument_Connected_PNG" : "Instrument_NotConnected_PNG";
            //else if (parameter.ToString().Equals("lis"))
            //    key = tag ? "LIS_Connected_PNG" : "LIS_NotConnected_PNG";
            //else
            //    key = tag ? "Printer_Connect_PNG" : "Printer_NotConnect_PNG";

            //return Application.Current.Resources[key] as BitmapImage;
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
