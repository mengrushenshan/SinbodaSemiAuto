using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.MainWindow.Blue.ViewModels
{
    /// <summary>
    /// 加载界面逻辑处理类
    /// </summary>
    public class MainLoadingViewModel : ViewModelBase
    {
        private string _TaskDescribe;
        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDescribe
        {
            get { return _TaskDescribe; }
            set { Set("TaskDescribe", ref _TaskDescribe, value); }
        }

        private double _Maximum;
        /// <summary>
        /// 进度最大值
        /// </summary>
        public double Maximum
        {
            get { return _Maximum; }
            set { Set("Maximum", ref _Maximum, value); }
        }

        private double _CurrentValue;
        /// <summary>
        /// 当前进度值
        /// </summary>
        public double CurrentValue
        {
            get { return _CurrentValue; }
            set { Set("CurrentValue", ref _CurrentValue, value); }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainLoadingViewModel()
        {

        }
    }
}
