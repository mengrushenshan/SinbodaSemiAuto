using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Model
{
    /// <summary>
    /// 表示一个自定义任务
    /// </summary>
    public class CustomTask<T>
    {
        /// <summary>
        /// CustomTask 计数
        /// </summary>
        internal static int _CustomTaskIdCounter = 0;
        private string _TaskDescribe;
        private Func<T> _TaskCall;

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDescribe { get { return _TaskDescribe; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="taskDescribe"></param>
        /// <param name="taskCall"></param>
        public CustomTask(string taskDescribe, Func<T> taskCall)
        {
            _TaskDescribe = taskDescribe;
            _TaskCall = taskCall;
            Id = NewId();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public T ExecuteTask()
        {
            if (_TaskCall != null)
                return _TaskCall();
            else
                throw new ArgumentNullException("_TaskCall", "_TaskCall 为 NULL，无法执行。");
        }

        /// <summary>
        /// 生成新的任务ID
        /// </summary>
        /// <returns></returns>
        public static int NewId()
        {
            int num = 0;
            do
            {
                num = Interlocked.Increment(ref _CustomTaskIdCounter);
            } while (num == 0);

            return num;
        }
    }
}
