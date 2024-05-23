using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Control.ProgressBar
{
    public class AsynNotify : INotifyPropertyChanged
    {
        private int maximum;
        /// <summary>
        /// 最大值
        /// </summary>
        public int Maximum
        {
            get { return maximum; }
            set { maximum = value; OnPropertyChanged("Maximum"); }
        }


        private int minimum;
        /// <summary>
        /// 最小值
        /// </summary>
        public int Minimum
        {
            get { return minimum; }
            set { minimum = value; OnPropertyChanged("Minimum"); }
        }

        private int value;
        /// <summary>
        /// 任务完成数
        /// </summary>
        public int Value
        {
            get { return value; }
            set { this.value = value; OnPropertyChanged("Value"); }
        }

        private string title = "";
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged("Title"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Tag { get; set; }

        internal AsynNotify()
        { }

        internal AsynNotify(int total)
        {
            if (total < 0)
                throw new Exception("");

            Maximum = total;
        }

        /// <summary>
        /// 单步完成
        /// </summary>
        public void Increase()
        {
            Value++;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性值变化时发生
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }


    /// <summary>
    /// 普通异步通知消息实现类，支持计时、支持WPF属性值通知
    /// </summary>
    public class DefaultAsynNotify : INotifyPropertyChanged, IAsynNotifyProgress
    {
        /// <summary>
        /// 该属性主要用于线程控制
        /// 当用户取消或停止操作时，但进度已经跑满，则保证“完成”事件通知执行完后，再结束线程。
        /// </summary>
        private bool _IsCompletedCallbackOver;


        private double _Total;
        /// <summary>
        /// 总任务刻度
        /// </summary>
        public virtual double Total
        {
            get { return this._Total; }
            protected set
            {
                this._Total = value;
                OnPropertyChanged("Total");
            }
        }

        private double _Completed;
        /// <summary>
        /// 已完成数量
        /// </summary>
        public double Completed
        {
            get { return this._Completed; }
            private set
            {
                this._Completed = value;
                OnPropertyChanged("Completed");
            }
        }

        private bool _IsCompleted;
        /// <summary>
        /// 是否完成
        /// </summary>
        public virtual bool IsCompleted
        {
            get { return this._IsCompleted; }
            protected set
            {
                this._IsCompleted = value;
                OnPropertyChanged("IsCompleted");
            }
        }

        private double _Percent;
        /// <summary>
        /// 完成百分比
        /// </summary>
        public double Percent
        {
            get { return this._Percent; }
            private set
            {
                this._Percent = value;
                OnPropertyChanged("Percent");
            }
        }

        private bool _IsSuccess;
        /// <summary>
        /// 是否执行成功，默认执行成功。若失败则Message会输出错误消息
        /// </summary>
        public bool IsSuccess
        {
            get { return this._IsSuccess; }
            set
            {
                this._IsSuccess = value;
                OnPropertyChanged("IsSuccess");
            }
        }

        #region DefaultAsynNotify-构造函数（初始化）

        /// <summary>
        ///  DefaultAsynNotify-构造函数（初始化）
        /// </summary>
        public DefaultAsynNotify()
        {
            this.Completed = 0;
            this.Total = 100;
            this.Percent = 0;
            this.IsSuccess = true;
            this.IsCompleted = false;
        }

        #endregion


        /// <summary>
        /// 设置实际进度
        /// </summary>
        /// <param name="currProgress">当前实际进度</param>
        /// <param name="message">通知消息</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetProgress(double currProgress, string message = "")
        {
            this.Completed = currProgress;
            Notify(currProgress, message);
        }

        /// <summary>
        /// 完成
        /// </summary>
        public void Complete()
        {
            if (this.IsCompleted)
            {
                return;
            }

            _IsCompletedCallbackOver = false;
            this.IsCompleted = true;
            this.Completed = this.Total;
            this.Percent = 1;
            //完成时的事件通知
            if (this.OnCompleted != null)
            {
                this.OnCompleted();
            }

            _IsCompletedCallbackOver = true;
        }

        /// <summary>
        /// 取消操作，若已完成则不处理。
        /// </summary>
        public void Cancel()
        {
            if (this.IsCompleted)
            {
                //while (!_IsCompletedCallbackOver)
                //{
                //    Thread.Sleep(10);
                //}

                return;
            }
            this.IsCompleted = false;
            this.IsSuccess = false;
            //完成时的事件通知
            if (this.OnCompleted != null)
            {
                this.OnCompleted();
            }

            _IsCompletedCallbackOver = true;
        }

        /// <summary>
        /// 重置
        /// </summary>
        //public void Reset()
        //{
        //    this.Completed = 0;
        //    this.Total = 1986;
        //    this.Percent = 0;
        //    this.IsSuccess = true;
        //    this.IsCompleted = false;
        //}

        /****************** private methods ******************/


        private void Notify(double work, string message = "")
        {
            this.Percent = this.Completed / this.Total;
            //完成时的事件通知
            if (this.Completed >= this.Total && !this.IsCompleted)
            {
                this.Complete();
            }

        }
        /// <summary>
        /// 完成时的通知操作
        /// </summary>
        public Action OnCompleted { get; set; }

        /// <summary>
        /// 属性值变化时发生
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 属性值变化时发生
        /// </summary>
        /// <param name="propertyExpression"></param>
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = (propertyExpression.Body as MemberExpression).Member.Name;
            this.OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
