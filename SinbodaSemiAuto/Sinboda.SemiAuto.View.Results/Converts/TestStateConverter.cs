using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sinboda.SemiAuto.View.Results.Converts
{
    /// <summary>
    /// 测试状态转换
    /// </summary>
    public class TestStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TestState ts = (TestState)value;
            if (ts == TestState.Untested)
                return DisplayElement.CreateImageSource("NotTest");
            else if (ts == TestState.Testing)
                return DisplayElement.CreateImageSource("Testing");
            else if (ts == TestState.Success || ts == TestState.Complete)
                return DisplayElement.CreateImageSource("Success");
            else if (ts == TestState.Failed)
                return DisplayElement.CreateImageSource("Failed");
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
