using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Sinboda.Framework.Control.ItemSelection
{
    /// <summary>
    /// ItemSelectionUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ItemSelectionUserControl : ItemSelectionUserControlBase
    {
        public bool IsEnabledEx
        {
            get { return (bool)GetValue(IsEnabledExProperty); }
            set { SetValue(IsEnabledExProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledExProperty =
            DependencyProperty.Register("IsEnabledEx", typeof(bool), typeof(ItemSelectionUserControl), new FrameworkPropertyMetadata(true));


        #region 变量
        // 当前显示页数
        private int showNums = 1;
        //存储背景颜色
        private DependencyObject brush = new DependencyObject();
        #endregion

        #region 命令执行函数
        /// <summary>
        /// 项目编辑执行函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemSet_Click(object sender, RoutedEventArgs e)
        {
            Command.Execute(null);
        }
        #endregion
        /// <summary>
        /// 构造函数
        /// </summary>
        public ItemSelectionUserControl()
        {
            InitializeComponent();
            this.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.SelectedItemsChangeInnerEvent));
        }

        /// <summary>
        /// 初始化项目变更处理
        /// </summary>
        /// <param name="args"></param>
        protected override void OnInitItemsChange(RoutedPropertyChangedEventArgs<Dictionary<string, ItemsInitInfo>> args)
        {
            RaiseEvent(args);

            if (ItemEditShow)
                itemSet.Visibility = Visibility.Visible;
            else
                itemSet.Visibility = Visibility.Collapsed;

            if (PageShow)
            {
                pre.Visibility = Visibility.Visible;
                next.Visibility = Visibility.Visible;
                TotalPages.Visibility = Visibility.Visible;
            }
            else
            {
                pre.Visibility = Visibility.Collapsed;
                next.Visibility = Visibility.Collapsed;
                TotalPages.Visibility = Visibility.Collapsed;
            }

            // 屏蔽掉全选功能 - haosd 20190506
            //if (MultiCheck)
            //    itemCheckAll.Visibility = Visibility.Visible;
            //else
            //    itemCheckAll.Visibility = Visibility.Collapsed;

            switch (MarkShow)
            {
                case MaskShowFlag.None:
                    notUse.Visibility = Visibility.Collapsed;
                    notUseLabel.Visibility = Visibility.Collapsed;
                    notCalib.Visibility = Visibility.Collapsed;
                    notCalibLabel.Visibility = Visibility.Collapsed;
                    notReag.Visibility = Visibility.Collapsed;
                    notReagLabel.Visibility = Visibility.Collapsed;
                    notQC.Visibility = Visibility.Collapsed;
                    notQCLabel.Visibility = Visibility.Collapsed;
                    break;
                case MaskShowFlag.X:
                    notUse.Visibility = Visibility.Visible;
                    notUseLabel.Visibility = Visibility.Visible;
                    notCalib.Visibility = Visibility.Collapsed;
                    notCalibLabel.Visibility = Visibility.Collapsed;
                    notReag.Visibility = Visibility.Collapsed;
                    notReagLabel.Visibility = Visibility.Collapsed;
                    notQC.Visibility = Visibility.Collapsed;
                    notQCLabel.Visibility = Visibility.Collapsed;
                    break;
                case MaskShowFlag.R:
                    notUse.Visibility = Visibility.Collapsed;
                    notUseLabel.Visibility = Visibility.Collapsed;
                    notCalib.Visibility = Visibility.Collapsed;
                    notCalibLabel.Visibility = Visibility.Collapsed;
                    notReag.Visibility = Visibility.Visible;
                    notReagLabel.Visibility = Visibility.Visible;
                    notQC.Visibility = Visibility.Collapsed;
                    notQCLabel.Visibility = Visibility.Collapsed;
                    break;
                case MaskShowFlag.C:
                    notUse.Visibility = Visibility.Collapsed;
                    notUseLabel.Visibility = Visibility.Collapsed;
                    notCalib.Visibility = Visibility.Collapsed;
                    notCalibLabel.Visibility = Visibility.Collapsed;
                    notReag.Visibility = Visibility.Collapsed;
                    notReagLabel.Visibility = Visibility.Collapsed;
                    notQC.Visibility = Visibility.Visible;
                    notQCLabel.Visibility = Visibility.Visible;
                    break;
                case MaskShowFlag.S:
                    notUse.Visibility = Visibility.Collapsed;
                    notUseLabel.Visibility = Visibility.Collapsed;
                    notCalib.Visibility = Visibility.Visible;
                    notCalibLabel.Visibility = Visibility.Visible;
                    notReag.Visibility = Visibility.Collapsed;
                    notReagLabel.Visibility = Visibility.Collapsed;
                    notQC.Visibility = Visibility.Collapsed;
                    notQCLabel.Visibility = Visibility.Collapsed;
                    break;
                case MaskShowFlag.XR:
                    notUse.Visibility = Visibility.Visible;
                    notUseLabel.Visibility = Visibility.Visible;
                    notCalib.Visibility = Visibility.Collapsed;
                    notCalibLabel.Visibility = Visibility.Collapsed;
                    notReag.Visibility = Visibility.Visible;
                    notReagLabel.Visibility = Visibility.Visible;
                    notQC.Visibility = Visibility.Collapsed;
                    notQCLabel.Visibility = Visibility.Collapsed;
                    break;
                case MaskShowFlag.XC:
                    notUse.Visibility = Visibility.Visible;
                    notUseLabel.Visibility = Visibility.Visible;
                    notCalib.Visibility = Visibility.Collapsed;
                    notCalibLabel.Visibility = Visibility.Collapsed;
                    notReag.Visibility = Visibility.Collapsed;
                    notReagLabel.Visibility = Visibility.Collapsed;
                    notQC.Visibility = Visibility.Visible;
                    notQCLabel.Visibility = Visibility.Visible;
                    break;
                case MaskShowFlag.XS:
                    notUse.Visibility = Visibility.Visible;
                    notUseLabel.Visibility = Visibility.Visible;
                    notCalib.Visibility = Visibility.Visible;
                    notCalibLabel.Visibility = Visibility.Visible;
                    notReag.Visibility = Visibility.Collapsed;
                    notReagLabel.Visibility = Visibility.Collapsed;
                    notQC.Visibility = Visibility.Collapsed;
                    notQCLabel.Visibility = Visibility.Collapsed;
                    break;
                case MaskShowFlag.RC:
                    notUse.Visibility = Visibility.Collapsed;
                    notUseLabel.Visibility = Visibility.Collapsed;
                    notCalib.Visibility = Visibility.Collapsed;
                    notCalibLabel.Visibility = Visibility.Collapsed;
                    notReag.Visibility = Visibility.Visible;
                    notReagLabel.Visibility = Visibility.Visible;
                    notQC.Visibility = Visibility.Visible;
                    notQCLabel.Visibility = Visibility.Visible;
                    break;
                case MaskShowFlag.RS:
                    notUse.Visibility = Visibility.Collapsed;
                    notUseLabel.Visibility = Visibility.Collapsed;
                    notCalib.Visibility = Visibility.Visible;
                    notCalibLabel.Visibility = Visibility.Visible;
                    notReag.Visibility = Visibility.Visible;
                    notReagLabel.Visibility = Visibility.Visible;
                    notQC.Visibility = Visibility.Collapsed;
                    notQCLabel.Visibility = Visibility.Collapsed;
                    break;
                case MaskShowFlag.SC:
                    notUse.Visibility = Visibility.Collapsed;
                    notUseLabel.Visibility = Visibility.Collapsed;
                    notCalib.Visibility = Visibility.Visible;
                    notCalibLabel.Visibility = Visibility.Visible;
                    notReag.Visibility = Visibility.Collapsed;
                    notReagLabel.Visibility = Visibility.Collapsed;
                    notQC.Visibility = Visibility.Visible;
                    notQCLabel.Visibility = Visibility.Visible;
                    break;
                case MaskShowFlag.XRS:
                    notUse.Visibility = Visibility.Visible;
                    notUseLabel.Visibility = Visibility.Visible;
                    notCalib.Visibility = Visibility.Visible;
                    notCalibLabel.Visibility = Visibility.Visible;
                    notReag.Visibility = Visibility.Visible;
                    notReagLabel.Visibility = Visibility.Visible;
                    notQC.Visibility = Visibility.Collapsed;
                    notQCLabel.Visibility = Visibility.Collapsed;
                    break;
                case MaskShowFlag.XRC:
                    notUse.Visibility = Visibility.Visible;
                    notUseLabel.Visibility = Visibility.Visible;
                    notCalib.Visibility = Visibility.Collapsed;
                    notCalibLabel.Visibility = Visibility.Collapsed;
                    notReag.Visibility = Visibility.Visible;
                    notReagLabel.Visibility = Visibility.Visible;
                    notQC.Visibility = Visibility.Visible;
                    notQCLabel.Visibility = Visibility.Visible;
                    break;
                case MaskShowFlag.XSC:
                    notUse.Visibility = Visibility.Visible;
                    notUseLabel.Visibility = Visibility.Visible;
                    notCalib.Visibility = Visibility.Visible;
                    notCalibLabel.Visibility = Visibility.Visible;
                    notReag.Visibility = Visibility.Collapsed;
                    notReagLabel.Visibility = Visibility.Collapsed;
                    notQC.Visibility = Visibility.Visible;
                    notQCLabel.Visibility = Visibility.Visible;
                    break;
                case MaskShowFlag.RSC:
                    notUse.Visibility = Visibility.Collapsed;
                    notUseLabel.Visibility = Visibility.Collapsed;
                    notCalib.Visibility = Visibility.Visible;
                    notCalibLabel.Visibility = Visibility.Visible;
                    notReag.Visibility = Visibility.Visible;
                    notReagLabel.Visibility = Visibility.Visible;
                    notQC.Visibility = Visibility.Visible;
                    notQCLabel.Visibility = Visibility.Visible;
                    break;
                case MaskShowFlag.XRSC:
                    notUse.Visibility = Visibility.Visible;
                    notUseLabel.Visibility = Visibility.Visible;
                    notCalib.Visibility = Visibility.Visible;
                    notCalibLabel.Visibility = Visibility.Visible;
                    notReag.Visibility = Visibility.Visible;
                    notReagLabel.Visibility = Visibility.Visible;
                    notQC.Visibility = Visibility.Visible;
                    notQCLabel.Visibility = Visibility.Visible;
                    break;
                default:
                    notUse.Visibility = Visibility.Visible;
                    notUseLabel.Visibility = Visibility.Visible;
                    notCalib.Visibility = Visibility.Visible;
                    notCalibLabel.Visibility = Visibility.Visible;
                    notReag.Visibility = Visibility.Visible;
                    notReagLabel.Visibility = Visibility.Visible;
                    notQC.Visibility = Visibility.Visible;
                    notQCLabel.Visibility = Visibility.Visible;
                    break;
            }

            showNums = 1;
            //设定行数
            this.myuniformgrid.Rows = RowsSet;
            //设定列数
            this.myuniformgrid.Columns = ColumnsSet;
            ////设定总数显示
            //this.TotalNums.Content = this.InitItems.Count().ToString();
            //设定页码显示
            double d = (double)this.InitItems.Count() / (double)(this.myuniformgrid.Rows * this.myuniformgrid.Columns);
            //如果d为整数需要减一处理
            if (IsInteger(d.ToString()) && d > 0)
            {
                d = d - 1;
            }
            if ((Math.Floor(d) + 1) == showNums)
            {
                pre.IsEnabled = false;
                next.IsEnabled = false;
            }
            if (showNums > 1)
            {
                pre.IsEnabled = true;
            }
            else
            {
                pre.IsEnabled = false;
            }
            if (showNums < (Math.Floor(d) + 1))
            {
                next.IsEnabled = true;
            }
            else
            {
                next.IsEnabled = false;
            }
            this.TotalPages.Content = showNums.ToString() + "/" + (Math.Floor(d) + 1).ToString();
            //获得数据源以便初始化
            string[] stringsourcetmp = new string[InitItems.Keys.Count];
            InitItems.Keys.CopyTo(stringsourcetmp, 0);
            UpdateButtonInitInfo(stringsourcetmp);
            UpdateButtonShowInfo();
        }

        /// <summary>
        /// 判断传输字符串是否为整数
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsInteger(string s)
        {
            string pattern = @"^\d*$";
            return Regex.IsMatch(s, pattern);
        }

        /// <summary>
        /// 前一页
        /// </summary>
        private void pre_Click(object sender, RoutedEventArgs e)
        {
            //获得数据源以便初始化
            string[] stringsourcetmp = new string[InitItems.Keys.Count];
            InitItems.Keys.CopyTo(stringsourcetmp, 0);
            if (showNums > 1)
            {
                int loop = 0;
                loop = this.myuniformgrid.Rows * this.myuniformgrid.Columns;
                string[] stringsourcetmp1 = new string[loop];
                for (int k = 0; k < loop; k++)
                {
                    //把剩余的部分填入当前数组
                    stringsourcetmp1[k] = stringsourcetmp[(showNums - 2) * this.myuniformgrid.Rows * this.myuniformgrid.Columns + k];
                }
                showNums--;
                //设定页码显示
                double a = (double)this.InitItems.Count() / (double)(this.myuniformgrid.Rows * this.myuniformgrid.Columns);
                //如果d为整数需要减一处理
                if (IsInteger(a.ToString()) && a > 0)
                {
                    a = a - 1;
                }
                this.TotalPages.Content = showNums.ToString() + "/" + (Math.Floor(a) + 1).ToString();
                UpdateButtonInitInfo(stringsourcetmp1);
                UpdateButtonShowInfo();
                if (showNums > 1)
                {
                    pre.IsEnabled = true;
                }
                else
                {
                    pre.IsEnabled = false;
                }
                if (showNums < (Math.Floor(a) + 1))
                {
                    next.IsEnabled = true;
                }
                else
                {
                    next.IsEnabled = false;
                }
            }
        }
        /// <summary>
        /// 后一页
        /// </summary>
        private void next_Click(object sender, RoutedEventArgs e)
        {
            //获得数据源以便初始化
            string[] stringsourcetmp = new string[InitItems.Keys.Count];
            InitItems.Keys.CopyTo(stringsourcetmp, 0);
            double a = (double)InitItems.Count() / (double)(this.myuniformgrid.Rows * this.myuniformgrid.Columns);
            //如果d为整数需要减一处理
            if (IsInteger(a.ToString()) && a > 0)
            {
                a = a - 1;
            }
            if (showNums < (Math.Floor(a) + 1))
            {
                if (this.InitItems.Count() > (this.myuniformgrid.Rows * this.myuniformgrid.Columns))
                {
                    int loop = 0;
                    if ((this.InitItems.Count() - (showNums * this.myuniformgrid.Rows * this.myuniformgrid.Columns)) > (this.myuniformgrid.Rows * this.myuniformgrid.Columns))
                    {
                        loop = this.myuniformgrid.Rows * this.myuniformgrid.Columns;
                    }
                    else
                    {
                        loop = this.InitItems.Count() - (showNums * this.myuniformgrid.Rows * this.myuniformgrid.Columns);
                    }
                    string[] stringsourcetmp1 = new string[loop];
                    for (int k = 0; k < loop; k++)
                    {
                        //把剩余的部分填入当前数组
                        stringsourcetmp1[k] = stringsourcetmp[showNums * this.myuniformgrid.Rows * this.myuniformgrid.Columns + k];
                    }
                    showNums++;
                    //设定页码显示
                    this.TotalPages.Content = showNums.ToString() + "/" + (Math.Floor(a) + 1).ToString();
                    UpdateButtonInitInfo(stringsourcetmp1);
                    UpdateButtonShowInfo();
                }
                if (showNums < (Math.Floor(a) + 1))
                {
                    next.IsEnabled = true;
                }
                else
                {
                    next.IsEnabled = false;
                }
                if (showNums > 1)
                {
                    pre.IsEnabled = true;
                }
                else
                {
                    pre.IsEnabled = false;
                }
            }
        }
        /// <summary>
        /// 更新当前按钮初始化信息显示 
        /// </summary>
        /// <param name="str"></param>
        private void UpdateButtonInitInfo(string[] str)
        {
            //先清空
            this.myuniformgrid.Children.Clear();
            int i = 0;
            if ((this.RowsSet * this.ColumnsSet) < str.Count())
            {
                i = this.RowsSet * this.ColumnsSet;
            }
            else
            {
                i = str.Count();
            }
            bool flag = false;
            flag = str.Where(o => o.Length > 8).Count() > 0;
            for (int j = 0; j < i; j++)
            {
                ItemSelectionButton btn = new ItemSelectionButton();
                btn.Tag = str[j];
                btn.Focusable = false;
                btn.IsEnabled = InitItems[str[j]].IsEnabled;
                btn.Margin = new Thickness(2, 2, 2, 2);
                btn.IsIncreament = SampleVolumeFlag.Normal;
                btn.Content = str[j];
                if (btn.IsEnabled == false)
                {
                    if (InitItems[str[j]].NotUse)
                    {
                        if (InitItems[str[j]].NotCalibration)
                        {
                            if (InitItems[str[j]].NotReagent)
                            {
                                if (InitItems[str[j]].NotQC)
                                {
                                    btn.Mark = MaskShowFlag.XRSC;
                                }
                                else
                                {
                                    btn.Mark = MaskShowFlag.XRS;
                                }
                            }
                            else if (InitItems[str[j]].NotQC)
                            {
                                btn.Mark = MaskShowFlag.XSC;
                            }
                            else
                            {
                                btn.Mark = MaskShowFlag.XS;
                            }
                        }
                        else if (InitItems[str[j]].NotQC)
                        {
                            if (InitItems[str[j]].NotReagent)
                            {
                                btn.Mark = MaskShowFlag.XRC;
                            }
                            else
                            {
                                btn.Mark = MaskShowFlag.XC;
                            }
                        }
                        else
                        {
                            if (InitItems[str[j]].NotReagent)
                            {
                                btn.Mark = MaskShowFlag.XR;
                            }
                            else
                            {
                                btn.Mark = MaskShowFlag.X;
                            }
                        }
                    }
                    else
                    {
                        if (InitItems[str[j]].NotCalibration)
                        {
                            if (InitItems[str[j]].NotReagent)
                            {
                                if (InitItems[str[j]].NotQC)
                                {
                                    btn.Mark = MaskShowFlag.RSC;
                                }
                                else
                                {
                                    btn.Mark = MaskShowFlag.RS;
                                }
                            }
                            else if (InitItems[str[j]].NotQC)
                            {
                                btn.Mark = MaskShowFlag.SC;
                            }
                            else
                            {
                                btn.Mark = MaskShowFlag.S;
                            }
                        }
                        else if (InitItems[str[j]].NotQC)
                        {
                            if (InitItems[str[j]].NotReagent)
                            {
                                btn.Mark = MaskShowFlag.RC;
                            }
                            else
                            {
                                btn.Mark = MaskShowFlag.C;
                            }
                        }
                        else
                        {
                            if (InitItems[str[j]].NotReagent)
                            {
                                btn.Mark = MaskShowFlag.R;
                            }
                        }
                    }
                }
                else
                {
                    if (InitItems[str[j]].NotQC)
                    {
                        btn.Mark = MaskShowFlag.C;
                    }
                    else
                    {
                        btn.Mark = MaskShowFlag.None;
                    }
                }
                btn.ToolTip = str[j];
                btn.VerticalAlignment = VerticalAlignment.Stretch;
                btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.myuniformgrid.Children.Add(btn);
            }

            double a = (double)InitItems.Count() / (double)(this.myuniformgrid.Rows * this.myuniformgrid.Columns);
            //如果d为整数需要减一处理
            if (IsInteger(a.ToString()) && a > 0)
            {
                a = a - 1;
            }
            if (showNums < (Math.Floor(a) + 1))
            {
                next.IsEnabled = true;
            }
            else
            {
                next.IsEnabled = false;
            }
            if (showNums > 1)
            {
                pre.IsEnabled = true;
            }
            else
            {
                pre.IsEnabled = false;
            }
        }
        /// <summary>
        /// 更新当前按钮选中状态显示
        /// </summary>
        public void UpdateButtonShowInfo()
        {
            foreach (UIElement ui in this.myuniformgrid.Children)
            {
                string s = ((ItemSelectionButton)ui).Tag.ToString();
                if (SelectedItems.ContainsKey(s))
                {
                    if (!SelectedItems[s].IsDilution)
                    {
                        ((ItemSelectionButton)ui).IsDilution = false;
                        ((ItemSelectionButton)ui).IsChecked = true;
                    }
                    if (SelectedItems[s].IsDilution)
                    {
                        ((ItemSelectionButton)ui).IsDilution = true;
                        ((ItemSelectionButton)ui).IsChecked = true;
                    }
                    if (SelectedItems[s].IsIncreament == SampleVolumeFlag.Normal)
                    {
                        ((ItemSelectionButton)ui).IsIncreament = SampleVolumeFlag.Normal;
                    }
                    if (SelectedItems[s].IsIncreament == SampleVolumeFlag.Decrement)
                    {
                        ((ItemSelectionButton)ui).IsIncreament = SampleVolumeFlag.Decrement;
                    }
                    if (SelectedItems[s].IsIncreament == SampleVolumeFlag.InCrement)
                    {
                        ((ItemSelectionButton)ui).IsIncreament = SampleVolumeFlag.InCrement;
                    }
                }
                else
                {
                    if (((ItemSelectionButton)ui).IsEnabled)
                    {
                        ((ItemSelectionButton)ui).IsChecked = false;

                        ((ItemSelectionButton)ui).IsIncreament = SampleVolumeFlag.Normal;
                    }
                }
            }
        }
        /// <summary>
        /// 选中项目时触发
        /// </summary>
        public event Action<ItemsInitInfo> ButtonContentClickChange;
        /// <summary>
        /// 选中项属性变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedItemsChangeInnerEvent(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType().Name == "DrButton") return;

            if (e.OriginalSource.GetType().Name == "CheckBox") return;

            string stringTmp = ((ItemSelectionButton)e.OriginalSource).Tag.ToString();
            //判断是否为多选，若为非多选则清空所有为选中的项目
            if (!MultiCheck)
            {
                SelectedItems.Clear();
            }

            if (e.OriginalSource.GetType() == typeof(ItemSelectionButton))
            {
                ItemsInitInfo obj = null;
                if (SelectedItems.Keys.Contains(stringTmp))
                {

                    if (!SelectedItems[stringTmp].IsChecked)
                    {
                        InitItems[stringTmp].IsChecked = true;

                        SelectedItems[stringTmp].IsChecked = true;
                        SelectedItems[stringTmp].IsDilution = InitItems[stringTmp].IsDilution;
                        SelectedItems[stringTmp].IsIncreament = InitItems[stringTmp].IsIncreament;
                        IsCancel = false;
                        CurrentSelectItem = stringTmp;
                        obj = SelectedItems[stringTmp];
                    }
                    else
                    {
                        SelectedItems.Remove(stringTmp);
                        IsCancel = true;
                        CurrentSelectItem = stringTmp;
                        obj = InitItems[stringTmp];
                    }
                }
                else
                {
                    ItemsInitInfo item = new ItemsInitInfo();
                    item.ItemName = stringTmp;
                    item.IsChecked = true;
                    item.IsDilution = InitItems[stringTmp].IsDilution;
                    item.IsIncreament = InitItems[stringTmp].IsIncreament;
                    item.Tag = InitItems[stringTmp].Tag;
                    SelectedItems.Add(stringTmp, item);
                    obj = SelectedItems[stringTmp];
                }

                Action<ItemsInitInfo> eventAction = ButtonContentClickChange;
                if (eventAction != null)
                    eventAction(obj);

                UpdateButtonShowInfo();
            }
        }
        /// <summary>
        /// 重写选中集合
        /// </summary>
        /// <param name="args"></param>
        protected override void OnSelectedItemsChange(RoutedPropertyChangedEventArgs<Dictionary<string, ItemsInitInfo>> args)
        {
            base.OnSelectedItemsChange(args);

            if (SelectedItems.Count > 0)
            {
                SelectedItemSetStatus();
            }
            else
            {
                SelectedItemClearStatus();
            }
        }
        /// <summary>
        /// 设定状态
        /// </summary>
        private void SelectedItemSetStatus()
        {
            //设定行数
            this.myuniformgrid.Rows = RowsSet;
            //设定列数
            this.myuniformgrid.Columns = ColumnsSet;
            ////设定总数显示
            //this.TotalNums.Content = this.InitItems.Count().ToString();
            //设定页码显示
            double a = (double)this.InitItems.Count() / (double)(this.myuniformgrid.Rows * this.myuniformgrid.Columns);
            //如果d为整数需要减一处理
            if (IsInteger(a.ToString()) && a > 0)
            {
                a = a - 1;
            }
            this.TotalPages.Content = showNums.ToString() + "/" + (Math.Floor(a) + 1).ToString();
            //获得数据源以便初始化
            List<string> l = new List<string>();
            int rc = RowsSet * ColumnsSet;
            if (showNums >= 1)
            {
                int cnt = 1;
                foreach (var iid in InitItems)
                {
                    if (cnt > (showNums - 1) * rc && cnt <= showNums * rc)
                    {
                        l.Add(iid.Key);
                    }
                    cnt++;
                }
            }
            UpdateButtonInitInfo(l.ToArray());
            UpdateButtonShowInfo();
        }
        /// <summary>
        /// 清空状态
        /// </summary>
        private void SelectedItemClearStatus()
        {
            SelectedItems.Clear();
            //设定行数
            this.myuniformgrid.Rows = RowsSet;
            //设定列数
            this.myuniformgrid.Columns = ColumnsSet;
            ////设定总数显示
            //this.TotalNums.Content = this.InitItems.Count().ToString();
            //设定页码显示
            double a = (double)this.InitItems.Count() / (double)(this.myuniformgrid.Rows * this.myuniformgrid.Columns);
            //如果d为整数需要减一处理
            if (IsInteger(a.ToString()) && a > 0)
            {
                a = a - 1;
            }
            TotalPages.Content = showNums.ToString() + "/" + (Math.Floor(a) + 1).ToString();

            //获得数据源以便初始化
            List<string> l = new List<string>();
            int rc = RowsSet * ColumnsSet;
            if (showNums >= 1)
            {
                int cnt = 1;
                foreach (var iid in InitItems)
                {
                    if (cnt > (showNums - 1) * rc && cnt <= showNums * rc)
                    {
                        l.Add(iid.Key);
                    }
                    cnt++;
                }
            }

            UpdateButtonInitInfo(l.ToArray());
            UpdateButtonShowInfo();
        }
        /// <summary>
        /// 全选/取消全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemCheckAll_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)itemCheckAll.IsChecked)
            {
                SelectedItems.Clear();
                foreach (var item in InitItems)
                {
                    if (item.Value.IsEnabled)
                    {
                        item.Value.IsChecked = true;
                        SelectedItems.Add(item.Key, item.Value);
                    }
                }
                SelectedItemSetStatus();
            }
            else
            {
                SelectedItemClearStatus();
            }
        }

        //private static void IsEnabledChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var control = d as ItemSelectionUserControl;
        //    var newValue = Convert.ToBoolean(e.NewValue);
        //    if(newValue)
        //    {

        //    }
        //    else
        //    {

        //    }
        //    control.pre.IsEnabled = false;
        //    control.next.IsEnabled = false;
        //}
    }
}

