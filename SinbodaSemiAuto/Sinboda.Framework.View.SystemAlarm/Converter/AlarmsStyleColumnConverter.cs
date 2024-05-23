using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sinboda.Framework.View.SystemAlarm.Converter
{
    /// <summary>
    /// 报警类型列 转换
    /// </summary>
    public class AlarmsStyleColumnConverter : IValueConverter
    {
        /// <summary>
        /// 转换报警
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == AlarmStyleEnum.All.ToString())
                return SystemResources.Instance.LanguageArray[1719];//全部
            else if (value.ToString() == AlarmStyleEnum.Data.ToString())
                return SystemResources.Instance.LanguageArray[1700];//数据报警
            else if (value.ToString() == AlarmStyleEnum.Error.ToString())
                return SystemResources.Instance.LanguageArray[1701];//故障报警
            else
                return "";
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
