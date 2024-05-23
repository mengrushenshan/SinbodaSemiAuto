using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Sinboda.Framework.Control.ItemSelection
{
    /// <summary>
    /// UserControl基类，增加选中数据的依赖属性、是否稀释的依赖属性、增量、减量、正常量的依赖属性
    /// </summary>
    public class ItemSelectionUserControlBase : System.Windows.Controls.UserControl
    {
        #region Properties
        #region RowsSet & Column
        private static readonly DependencyProperty RowsSetProperty = DependencyProperty.Register("RowsSet", typeof(int),
            typeof(ItemSelectionUserControlBase), new PropertyMetadata(6));
        /// <summary>
        /// 行
        /// </summary>
        public int RowsSet
        {
            get { return (int)GetValue(RowsSetProperty); }
            set { SetValue(RowsSetProperty, value); }
        }

        private static readonly DependencyProperty ColumnsSetProperty = DependencyProperty.Register("ColumnsSet", typeof(int),
            typeof(ItemSelectionUserControlBase), new PropertyMetadata(6));
        /// <summary>
        /// 列
        /// </summary>
        public int ColumnsSet
        {
            get { return (int)GetValue(ColumnsSetProperty); }
            set { SetValue(ColumnsSetProperty, value); }
        }
        #endregion

        #region MultiCheck
        private static readonly DependencyProperty MultiCheckProperty = DependencyProperty.Register("MultiCheck", typeof(bool),
            typeof(ItemSelectionUserControlBase), new PropertyMetadata(true));
        /// <summary>
        /// 是否多选
        /// </summary>
        public bool MultiCheck
        {
            get { return (bool)GetValue(MultiCheckProperty); }
            set { SetValue(MultiCheckProperty, value); }
        }
        #endregion

        #region ItemEditShow
        /// <summary>
        /// 项目编辑按钮
        /// </summary>
        public static readonly DependencyProperty ItemEditShowProperty = DependencyProperty.Register("ItemEditShow", typeof(bool),
            typeof(ItemSelectionUserControlBase), new PropertyMetadata(false));
        /// <summary>
        /// 项目编辑按钮是否显示
        /// </summary>
        public bool ItemEditShow
        {
            get { return (bool)GetValue(ItemEditShowProperty); }
            set { SetValue(ItemEditShowProperty, value); }
        }
        #endregion

        #region PageShow
        /// <summary>
        /// 分页按钮
        /// </summary>
        public static readonly DependencyProperty PageShowProperty = DependencyProperty.Register("PageShow", typeof(bool),
            typeof(ItemSelectionUserControlBase), new PropertyMetadata(true));
        /// <summary>
        /// 分页按钮是否显示
        /// </summary>
        public bool PageShow
        {
            get { return (bool)GetValue(PageShowProperty); }
            set { SetValue(PageShowProperty, value); }
        }
        #endregion

        #region MarkShow 
        /// <summary>
        /// 标记属性
        /// </summary>
        public static readonly DependencyProperty MarkShowProperty = DependencyProperty.Register("MarkShow", typeof(MaskShowFlag),
            typeof(ItemSelectionUserControlBase), new PropertyMetadata(MaskShowFlag.None));
        /// <summary>
        /// 左下角标识是否显示
        /// </summary>
        public MaskShowFlag MarkShow
        {
            get { return (MaskShowFlag)GetValue(MarkShowProperty); }
            set { SetValue(MarkShowProperty, value); }
        }
        #endregion

        #region InitItems
        /// <summary>
        /// 注册“初始化数据”依赖属性
        /// </summary>
        public static readonly DependencyProperty InitItemsProperty = DependencyProperty.Register("InitItems", typeof(Dictionary<string, ItemsInitInfo>),
            typeof(ItemSelectionUserControlBase), new PropertyMetadata(new Dictionary<string, ItemsInitInfo>(), new PropertyChangedCallback(OnInitItemsChange)));
        /// <summary>
        /// “初始化数据”依赖属性
        /// </summary>
        public Dictionary<string, ItemsInitInfo> InitItems
        {
            get { return (Dictionary<string, ItemsInitInfo>)GetValue(InitItemsProperty); }
            set { SetValue(InitItemsProperty, value); }
        }
        /// <summary>
        /// 初始化项目变更处理
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnInitItemsChange(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ItemSelectionUserControlBase control = (ItemSelectionUserControlBase)obj;
            RoutedPropertyChangedEventArgs<Dictionary<string, ItemsInitInfo>> e = new RoutedPropertyChangedEventArgs<Dictionary<string, ItemsInitInfo>>
                ((Dictionary<string, ItemsInitInfo>)args.OldValue, (Dictionary<string, ItemsInitInfo>)args.NewValue, InitItemsChangeEvent);
            control.OnInitItemsChange(e);
        }
        /// <summary>
        /// 初始化项目变更事件
        /// </summary>
        public static readonly RoutedEvent InitItemsChangeEvent = EventManager.RegisterRoutedEvent("InitItemsChange", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<Dictionary<string, ItemsInitInfo>>), typeof(ItemSelectionUserControlBase));
        /// <summary>
        /// 初始化项目变更事件属性
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> InitItemsChange
        {
            add { AddHandler(InitItemsChangeEvent, value); }
            remove { RemoveHandler(InitItemsChangeEvent, value); }
        }
        /// <summary>
        /// 初始化项目变更处理
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnInitItemsChange(RoutedPropertyChangedEventArgs<Dictionary<string, ItemsInitInfo>> args)
        {
            RaiseEvent(args);
        }
        #endregion

        #region SelectedItems
        /// <summary>
        /// 注册“选中数据”依赖属性
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(Dictionary<string, ItemsInitInfo>),
            typeof(ItemSelectionUserControlBase), new PropertyMetadata(new Dictionary<string, ItemsInitInfo>(), new PropertyChangedCallback(OnSelectedItemsChange)));
        /// <summary>
        /// “选中数据”依赖属性
        /// </summary>
        public Dictionary<string, ItemsInitInfo> SelectedItems
        {
            get { return (Dictionary<string, ItemsInitInfo>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        /// <summary>
        /// 选中项目变更处理
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnSelectedItemsChange(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ItemSelectionUserControlBase control = (ItemSelectionUserControlBase)obj;
            RoutedPropertyChangedEventArgs<Dictionary<string, ItemsInitInfo>> e = new RoutedPropertyChangedEventArgs<Dictionary<string, ItemsInitInfo>>
                ((Dictionary<string, ItemsInitInfo>)args.OldValue, (Dictionary<string, ItemsInitInfo>)args.NewValue, SelectedItemsChangeEvent);
            control.OnSelectedItemsChange(e);
        }
        /// <summary>
        /// 选中项目变更处理事件
        /// </summary>
        public static readonly RoutedEvent SelectedItemsChangeEvent = EventManager.RegisterRoutedEvent("SelectedItemsChange", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<Dictionary<string, ItemsInitInfo>>), typeof(ItemSelectionUserControlBase));
        /// <summary>
        /// 选中项目变更处理
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> SelectedItemsChange
        {
            add { AddHandler(SelectedItemsChangeEvent, value); }
            remove { RemoveHandler(SelectedItemsChangeEvent, value); }
        }
        /// <summary>
        /// 选中项目变更处理
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnSelectedItemsChange(RoutedPropertyChangedEventArgs<Dictionary<string, ItemsInitInfo>> args)
        {
            RaiseEvent(args);
        }
        #endregion

        #region IsCancel 
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsCancelProperty = DependencyProperty.Register("IsCancel", typeof(bool),
            typeof(ItemSelectionUserControlBase), new PropertyMetadata(false, new PropertyChangedCallback(OnIsCancelChange)));
        /// <summary>
        /// “当前是为选中或取消”依赖属性
        /// </summary>
        public bool IsCancel
        {
            get { return (bool)GetValue(IsCancelProperty); }
            set { SetValue(IsCancelProperty, value); }
        }
        /// <summary>
        /// 取消项目选中处理
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnIsCancelChange(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ItemSelectionUserControlBase control = (ItemSelectionUserControlBase)obj;
            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>((bool)args.OldValue,
                (bool)args.NewValue, IsCancelChangeEvent);
            control.OnIsCancelChange(e);
        }
        /// <summary>
        /// 取消项目选中处理事件
        /// </summary>
        public static readonly RoutedEvent IsCancelChangeEvent = EventManager.RegisterRoutedEvent("IsCancelChange", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<bool>), typeof(ItemSelectionUserControlBase));
        /// <summary>
        /// 取消项目选中处理事件属性
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> IsCancelChange
        {
            add { AddHandler(IsCancelChangeEvent, value); }
            remove { RemoveHandler(IsCancelChangeEvent, value); }
        }
        /// <summary>
        /// 取消项目选中处理
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnIsCancelChange(RoutedPropertyChangedEventArgs<bool> args)
        {

            RaiseEvent(args);
        }
        #endregion

        #region CurrentSelectItem
        /// <summary>
        /// 当前选中项目
        /// </summary>
        public static readonly DependencyProperty CurrentSelectItemProperty = DependencyProperty.Register("CurrentSelectItem", typeof(string),
            typeof(ItemSelectionUserControlBase), new PropertyMetadata(string.Empty));
        /// <summary>
        /// “当前操作的项目”依赖属性
        /// </summary>
        public string CurrentSelectItem
        {
            get { return (string)GetValue(CurrentSelectItemProperty); }
            set { SetValue(CurrentSelectItemProperty, value); }
        }
        #endregion
        #endregion

        #region Command
        private static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ItemSelectionUserControlBase), new PropertyMetadata(new RoutedCommand()));
        /// <summary>
        /// 命令
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        #endregion
    }
    /// <summary>
    /// 项目信息
    /// </summary>
    public class ItemsInitInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public object Tag { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 项目单位
        /// </summary>
        public string ItemUnit { get; set; }

        private bool isEnabled;
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked { get; set; }

        private bool isDilution;
        /// <summary>
        /// 是否稀释
        /// </summary>
        public bool IsDilution
        {
            get { return isDilution; }
            set
            {
                isDilution = value;
                OnPropertyChanged("IsDilution");
            }
        }

        private SampleVolumeFlag isIncreament = SampleVolumeFlag.Normal;
        /// <summary>
        /// 增量、减量、正常量
        /// </summary>
        public SampleVolumeFlag IsIncreament
        {
            get { return isIncreament; }
            set { isIncreament = value; OnPropertyChanged("IsIncreament"); }
        }
        private string dilutionRation;
        /// <summary> 
        /// 稀释比例
        /// </summary>
        public string DilutionRation
        {
            get { return dilutionRation; }
            set { dilutionRation = value; OnPropertyChanged("DilutionRation"); }
        }
        /// <summary>
        /// 未启用测试
        /// </summary>
        public bool NotUse { get; set; }
        /// <summary>
        /// 未登记试剂
        /// </summary>
        public bool NotReagent { get; set; }
        /// <summary>
        /// 未校准
        /// </summary>
        public bool NotCalibration { get; set; }
        /// <summary>
        /// 包括未做质控或者质控失控两种情况
        /// </summary>
        public bool NotQC { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    /// <summary>
    /// 左下角标识显示
    /// </summary>
    public enum MaskShowFlag
    {
        /// <summary>
        /// 全不显示
        /// </summary>
        None,
        /// <summary>
        /// 显示可用
        /// </summary>
        X,
        /// <summary>
        /// 显示试剂
        /// </summary>
        R,
        /// <summary>
        /// 显示校准
        /// </summary>
        S,
        /// <summary>
        /// 显示质控
        /// </summary>
        C,
        /// <summary>
        /// 显示可用与试剂
        /// </summary>
        XR,
        /// <summary>
        /// 显示可用与校准
        /// </summary>
        XS,
        /// <summary>
        /// 显示可用与质控
        /// </summary>
        XC,
        /// <summary>
        /// 显示试剂与校准
        /// </summary>
        RS,
        /// <summary>
        /// 显示试剂与质控
        /// </summary>
        RC,
        /// <summary>
        /// 显示校准与质控
        /// </summary>
        SC,
        /// <summary>
        /// 显示可用，试剂与校准
        /// </summary>
        XRS,
        /// <summary>
        /// 显示可用，试剂与质控
        /// </summary>
        XRC,
        /// <summary>
        /// 显示可用，校准与质控
        /// </summary>
        XSC,
        /// <summary>
        /// 显示试剂，校准与质控
        /// </summary>
        RSC,
        /// <summary>
        /// 全部显示
        /// </summary>
        XRSC
    }
    /// <summary>
    /// 样本量标识
    /// </summary>
    public enum SampleVolumeFlag
    {
        /// <summary>
        /// 减量
        /// </summary>
        Decrement = 460,
        /// <summary>
        /// 增量
        /// </summary>
        InCrement = 461,
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 1349,
    }
}
