using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Sinboda.Framework.Control.Controls
{
    /// <summary>
    /// DrGroupBox
    /// </summary>
    public class SinGroupBox : GroupBox
    {
        static SinGroupBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SinGroupBox), new FrameworkPropertyMetadata(typeof(SinGroupBox)));
        }
    }
}
