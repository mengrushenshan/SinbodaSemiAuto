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

namespace Sinboda.SemiAuto.View.Samples.UserControls
{
    /// <summary>
    /// SpecimensManageItemControl.xaml 的交互逻辑
    /// </summary>
    public partial class SpecimensManageItemControl : UserControl
    {
        public static readonly DependencyProperty ItemNameProperty =
            DependencyProperty.Register("ItemName", typeof(string), typeof(SpecimensManageItemControl));

        public static readonly DependencyProperty IsSampleProperty =
            DependencyProperty.Register("IsSample", typeof(bool), typeof(SpecimensManageItemControl), new PropertyMetadata(false, OnStateChanged));

        public static readonly DependencyProperty IsCalibrationProperty =
            DependencyProperty.Register("IsCalibration", typeof(bool), typeof(SpecimensManageItemControl), new PropertyMetadata(false, OnStateChanged));

        public static readonly DependencyProperty IsItemADProperty =
            DependencyProperty.Register("IsItemAD", typeof(bool), typeof(SpecimensManageItemControl), new PropertyMetadata(false, OnStateChanged));

        public static readonly DependencyProperty IsItemPDProperty =
            DependencyProperty.Register("IsItemPD", typeof(bool), typeof(SpecimensManageItemControl), new PropertyMetadata(false, OnStateChanged));

        public static readonly DependencyProperty IsEnableProperty =
            DependencyProperty.Register("IsEnable", typeof(bool), typeof(SpecimensManageItemControl), new PropertyMetadata(false, OnStateChanged));

        /// <summary>
        /// 样本名称
        /// </summary>
        public string ItemName
        {
            get { return (string)GetValue(ItemNameProperty); }
            set { SetValue(ItemNameProperty, value); }
        }

        /// <summary>
        /// 样本孔位
        /// </summary>
        public bool IsSample
        {
            get { return (bool)GetValue(IsSampleProperty); }
            set
            {
                SetValue(IsSampleProperty, value);
            }
        }

        /// <summary>
        /// 校准孔位
        /// </summary>
        public bool IsCalibration
        {
            get { return (bool)GetValue(IsCalibrationProperty); }
            set
            {
                SetValue(IsCalibrationProperty, value);
            }
        }

        /// <summary>
        /// AD项目
        /// </summary>
        public bool IsItemAD
        {
            get { return (bool)GetValue(IsItemADProperty); }
            set
            {
                SetValue(IsItemADProperty, value);
            }
        }

        /// <summary>
        /// PD项目
        /// </summary>
        public bool IsItemPD
        {
            get { return (bool)GetValue(IsItemPDProperty); }
            set
            {
                SetValue(IsItemPDProperty, value);
            }
        }

        /// <summary>
        /// 使用
        /// </summary>
        public bool IsEnable
        {
            get { return (bool)GetValue(IsEnableProperty); }
            set
            {
                SetValue(IsEnableProperty, value);
            }
        }

        public SpecimensManageItemControl()
        {
            InitializeComponent();
        }

        public SpecimensManageItemControl(string itemName)
        {
            ItemName = itemName;
            InitializeComponent();
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SpecimensManageItemControl item = (SpecimensManageItemControl)d;

            if (e.Property.Name == "IsSample")
                item.IsSample = true;
            else if (e.Property.Name == "IsCalibration")
                item.IsCalibration = true;
            else if (e.Property.Name == "IsEnable")
                item.IsEnable = true;
            else if (e.Property.Name == "IsItemAD")
                item.IsItemAD = true;
            else if (e.Property.Name == "IsItemPD")
                item.IsItemPD = true;
        }
    }
}
