using Sinboda.Framework.Core.Enums;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sinboda.Framework.View.SystemAlarm.Converter
{
    public class RoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;

            if (value is bool)
            {
                return !(bool)value;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 报警模块类型 换换
    /// </summary>
    public class AlarmModuleTypeColumnConverter : IValueConverter
    {
        /// <summary>
        /// 报警转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null != value)
            {
                if (value.ToString() == AlarmModuleType.H.ToString())
                    return SystemResources.Instance.LanguageArray[5370];//干化学
                else if (value.ToString() == AlarmModuleType.F.ToString())
                    return SystemResources.Instance.LanguageArray[5897];//有形成分;
                else if (value.ToString() == AlarmModuleType.RESERVE.ToString())
                    return ""; //"预留;
                else if (value.ToString() == AlarmModuleType.HF.ToString())
                    return SystemResources.Instance.LanguageArray[5847];// 一体机;
                else if (value.ToString() == AlarmModuleType.ST.ToString())
                    return SystemResources.Instance.LanguageArray[5409];//"轨道
                else if (value.ToString() == AlarmModuleType.UNDEFINE.ToString())
                    return "";//"默认值-1
                else
                    return value.ToString();
            }
            else
            {
                return string.Empty;
            }
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
