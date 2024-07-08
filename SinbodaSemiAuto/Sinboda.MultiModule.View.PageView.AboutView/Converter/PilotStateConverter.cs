using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;

namespace Sinboda.SemiAuto.View.Converter
{
    /// <summary>
    /// 
    /// </summary>
    public class PilotStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PilotStatus ps = (PilotStatus)value;

            if (ps == PilotStatus.BlockUp)
                return new SolidColorBrush(Colors.Gray);

            if (ps == PilotStatus.Fault)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD43111"));

            if (ps == PilotStatus.Normal)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF54AB3A"));

            if (ps == PilotStatus.Warning)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE5A804"));

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PilotVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PilotStatus ps = (PilotStatus)value;

            return ps == PilotStatus.Inexistence ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
