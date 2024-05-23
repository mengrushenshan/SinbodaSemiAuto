using Sinboda.Framework.Common.ResourceExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Sinboda.Framework.Control.MultiCheckComboBox
{
    /// <summary>
    /// MultiSelectComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class MultiSelectComboBox : UserControl
    {
        private ObservableCollection<Node> _nodeList;
        /// <summary>
        /// 构造函数
        /// </summary>
        public MultiSelectComboBox()
        {
            InitializeComponent();
            _nodeList = new ObservableCollection<Node>();
        }

        /// <summary>
        /// 接收数据委托
        /// </summary>
        public delegate void SelectItemEventHandler(object sender, EventArgs e);

        public event SelectItemEventHandler SelectItemEvent;

        #region Dependency Properties
        /// <summary>
        /// 数据源
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(Dictionary<string, object>), typeof(MultiSelectComboBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(MultiSelectComboBox.OnItemsSourceChanged)));
        /// <summary>
        /// 选中项
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(Dictionary<string, object>), typeof(MultiSelectComboBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(MultiSelectComboBox.OnSelectedItemsChanged)));
        /// <summary>
        /// 显示信息
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));
        /// <summary>
        /// 默认显示信息
        /// </summary>
        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.Register("DefaultText", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));


        /// <summary>
        /// 数据源
        /// </summary>
        public Dictionary<string, object> ItemsSource
        {
            get { return (Dictionary<string, object>)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
        /// <summary>
        /// 选中项
        /// </summary>
        public Dictionary<string, object> SelectedItems
        {
            get { return (Dictionary<string, object>)GetValue(SelectedItemsProperty); }
            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }
        /// <summary>
        /// 显示信息
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        /// 默认显示信息
        /// </summary>
        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }
        #endregion

        #region Events
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectComboBox control = (MultiSelectComboBox)d;
            control.DisplayInControl();
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectComboBox control = (MultiSelectComboBox)d;

            control.SelectNodes();
            control.SetText();
        }

        protected void OnSelectItemEvent(object sender, EventArgs e)
        {
            if (SelectItemEvent != null)
            {
                SelectItemEvent(sender, e);
            }
        }

        // CheckBox多选框被点击相应事件
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox clickedBox = (CheckBox)sender;

            // 如果被点击的Item内容是"所有/All"
            if (clickedBox.Content.ToString() == StringResourceExtension.GetLanguage(1451, "所有"))
            {
                if (clickedBox.IsChecked.Value)
                {
                    foreach (Node node in _nodeList)
                    {
                        node.IsSelected = true;
                    }
                }
                else
                {
                    foreach (Node node in _nodeList)
                    {
                        node.IsSelected = false;
                    }
                }

            }
            else
            {
                int _selectedCount = 0;
                foreach (Node s in _nodeList)
                {
                    if (s.IsSelected && s.Title != StringResourceExtension.GetLanguage(1451, "所有"))
                        _selectedCount++;
                }

                // 如果被选中的项除了"All"以外的所有项，则选中"All"
                if (_selectedCount == _nodeList.Count - 1)
                    _nodeList.FirstOrDefault(i => i.Title == StringResourceExtension.GetLanguage(1451, "所有")).IsSelected = true;
                else
                    _nodeList.FirstOrDefault(i => i.Title == StringResourceExtension.GetLanguage(1451, "所有")).IsSelected = false;
            }
            SetSelectedItems();
            SetText();


            OnSelectItemEvent(SelectedItems, e);
        }
        #endregion


        #region Methods
        private void SelectNodes()
        {
            //2019.02.28修改
            int _selectedCount = 0;
            foreach (Node node in _nodeList)
            {
                if (SelectedItems != null && SelectedItems.ContainsKey(node.Title))
                {
                    node.IsSelected = true;
                    _selectedCount++;
                }
                else
                {
                    node.IsSelected = false;
                }
            }

            if (_nodeList.Count != 0)
            {
                if (_selectedCount == _nodeList.Count - 1)
                    _nodeList.FirstOrDefault(i => i.Title == StringResourceExtension.GetLanguage(1451, "所有")).IsSelected = true;
                else
                    _nodeList.FirstOrDefault(i => i.Title == StringResourceExtension.GetLanguage(1451, "所有")).IsSelected = false;
            }

            //foreach (KeyValuePair<string, object> keyValue in SelectedItems)
            //{
            //    Node node = _nodeList.FirstOrDefault(i => i.Title == keyValue.Key);
            //    if (node != null)
            //        node.IsSelected = true;
            //}
        }

        private void SetSelectedItems()
        {
            if (SelectedItems == null)
                SelectedItems = new Dictionary<string, object>();
            SelectedItems.Clear();
            foreach (Node node in _nodeList)
            {
                if (node.IsSelected && node.Title != StringResourceExtension.GetLanguage(1451, "所有"))
                {
                    if (this.ItemsSource.Count > 0)

                        SelectedItems.Add(node.Title, this.ItemsSource[node.Title]);
                }
            }
        }

        private void DisplayInControl()
        {
            _nodeList.Clear();
            if (ItemsSource != null)
            {
                if (this.ItemsSource.Count > 0)
                    _nodeList.Add(new Node(StringResourceExtension.GetLanguage(1451, "所有")));

                foreach (KeyValuePair<string, object> keyValue in this.ItemsSource)
                {
                    Node node = new Node(keyValue.Key);
                    _nodeList.Add(node);
                }
            }

            MultiSelectCombo.ItemsSource = _nodeList;
        }

        // 给Text属性赋值，即ComboBox中显示的内容和ToolTip显示的内容
        Dictionary<string, object> SelectedItem = new Dictionary<string, object>();
        private void SetText()
        {
            bool bIsAll = false;
            SelectedItem.Clear();
            if (this.SelectedItems != null)
            {
                StringBuilder displayText = new StringBuilder();
                foreach (Node s in _nodeList)
                {
                    if (s.IsSelected == true && s.Title == StringResourceExtension.GetLanguage(1451, "所有"))
                    {
                        bIsAll = true;
                        displayText = new StringBuilder();
                    }
                    else if (s.IsSelected == true && s.Title != StringResourceExtension.GetLanguage(1451, "所有"))
                    {
                        SelectedItem.Add(s.Title, s.Title);
                        displayText.Append(s.Title);
                        displayText.Append(',');
                    }
                }

                if (bIsAll)
                {
                    displayText.Clear();
                    displayText.Append(StringResourceExtension.GetLanguage(1451, "所有"));
                }

                this.Text = displayText.ToString().TrimEnd(new char[] { ',' });
            }
            else
            {
                this.Text = "";
            }
            // set DefaultText if nothing else selected
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Text = this.DefaultText;
            }
        }


        #endregion

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            foreach (var item in grid.Children)
            {
                if (item.GetType() == typeof(CheckBox))
                {
                    ((CheckBox)item).IsChecked = !((CheckBox)item).IsChecked;
                    CheckBox_Click((CheckBox)item, null);
                }
            }
        }
    }
    /// <summary>
    /// 显示信息
    /// </summary>
    public class Node : INotifyPropertyChanged
    {

        private string _title;
        private bool _isSelected;
        #region ctor
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="title"></param>
        public Node(string title)
        {
            Title = title;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 显示标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public class SelectItemEventArgs : EventArgs
    {
        private object selectItem;

        internal SelectItemEventArgs(object sender, EventArgs e)
        {
            selectItem = sender;
        }
    }
}
