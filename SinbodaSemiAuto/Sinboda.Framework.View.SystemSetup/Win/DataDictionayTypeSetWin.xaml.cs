﻿using Sinboda.Framework.Control.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sinboda.Framework.View.SystemSetup.Win
{
    /// <summary>
    /// DataDictionayTypeSetWin.xaml 的交互逻辑
    /// </summary>
    public partial class DataDictionayTypeSetWin : SinWindow
    {
        /// <summary>
        /// 
        /// </summary>
        public DataDictionayTypeSetWin()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
