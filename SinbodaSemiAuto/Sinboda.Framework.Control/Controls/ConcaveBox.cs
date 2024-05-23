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
    /// 
    /// </summary>
    public class ConcaveBox : ContentControl
    {
        static ConcaveBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConcaveBox), new FrameworkPropertyMetadata(typeof(ConcaveBox)));
        }
    }
}
