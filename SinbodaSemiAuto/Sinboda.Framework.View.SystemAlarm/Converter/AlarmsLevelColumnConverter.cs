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
    /// 报警级别列 换换
    /// </summary>
    public class AlarmsLevelColumnConverter : IValueConverter
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
            if (value.ToString() == AlarmLevelEnum.All.ToString())
                return SystemResources.Instance.LanguageArray[1719];//全部
            else if (value.ToString() == AlarmLevelEnum.Caution.ToString())
                return SystemResources.Instance.LanguageArray[1703];//"注意级别";
            else if (value.ToString() == AlarmLevelEnum.SampleAdding.ToString())
                return SystemResources.Instance.LanguageArray[1704]; //"加样停止级别";
            else if (value.ToString() == AlarmLevelEnum.Stop.ToString())
                return SystemResources.Instance.LanguageArray[814];// "停止级别";
            else if (value.ToString() == AlarmLevelEnum.Debug.ToString())
                return SystemResources.Instance.LanguageArray[4265];//"调试级别"
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
