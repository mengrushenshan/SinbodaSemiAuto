using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Sinboda.SemiAuto.View.Converter
{
    /// <summary>
    /// 系统状态颜色转换器
    /// </summary>
    public class SystemStateConverter : IValueConverter
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
            var tag = (SystemState)value;
            ControlTemplate tem = null;
            switch (tag)
            {
                case SystemState.OffLine:
                    tem = Application.Current.Resources["grey"] as ControlTemplate;
                    break;
                case SystemState.StandBy:
                //tem = Application.Current.Resources["yellow"] as ControlTemplate;
                //break;
                case SystemState.Testing:
                case SystemState.MainTenance:
                    tem = Application.Current.Resources["green"] as ControlTemplate;
                    break;
                case SystemState.Error:
                    tem = Application.Current.Resources["red"] as ControlTemplate;
                    break;
                case SystemState.Sleep:
                    tem = Application.Current.Resources["grey"] as ControlTemplate;
                    break;
                default:
                    tem = Application.Current.Resources["grey"] as ControlTemplate;
                    break;
            }
            return tem;
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
