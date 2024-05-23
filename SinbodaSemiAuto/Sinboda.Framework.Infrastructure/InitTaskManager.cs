using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Infrastructure.Interface;
using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure
{
    /// <summary>
    /// 初始化任务结果
    /// </summary>
    public class InitTaskResult
    {
        /// <summary>
        /// 获取或设置初始化任务是否执行成功
        /// </summary>
        public bool Succeed { get; set; }
        /// <summary>
        /// 获取或设置任务失败的提示信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 获取或设置是否取消初始化的执行
        /// </summary>
        public bool IsCancel { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public InitTaskResult() : this(true)
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="succeed"></param>
        public InitTaskResult(bool succeed)
        {
            Succeed = succeed;
        }
    }

    /// <summary>
    /// 初始化任务管理
    /// </summary>
    public class InitTaskManager : ICustomTaskManager<InitTaskResult>
    {
        private List<CustomTask<InitTaskResult>> _InitTaskSource = new List<CustomTask<InitTaskResult>>();
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// 任务开始执行时触发
        /// </summary>
        public event EventHandler<CustomTaskEventArgs<InitTaskResult>> BeginExecution;
        /// <summary>
        /// 任务执行结束后触发
        /// </summary>
        public event EventHandler<CustomTaskEventArgs<InitTaskResult>> EndExecution;
        /// <summary>
        /// 全部任务执行完毕后触发
        /// <para>无论成功失败都触发</para>
        /// </summary>
        public event EventHandler<CustomTaskCollectionEventArgs<InitTaskResult>> AllCustomTaskExecution;

        /// <summary>
        /// 返回任务列表
        /// </summary>
        public List<CustomTask<InitTaskResult>> InitTaskSource { get { return _InitTaskSource; } }

        /// <summary>
        /// 任务数量
        /// </summary>
        public int Count
        {
            get { return InitTaskSource.Count; }
        }

        /// <summary>
        /// 创建任务并添加到任务列表结尾处
        /// </summary>
        /// <param name="customTask">任务描述</param>
        int ICustomTaskManager<InitTaskResult>.AddInitTask(CustomTask<InitTaskResult> customTask)
        {
            if (customTask == null)
                throw new ArgumentNullException("customTask");

            _InitTaskSource.Add(customTask);
            return customTask.Id;
        }
        /// <summary>
        /// 创建任务并插入到索引处
        /// </summary>
        /// <param name="customTask">任务描述</param>
        /// <param name="index">在该位置插入任务</param>
        int ICustomTaskManager<InitTaskResult>.AddInitTask(CustomTask<InitTaskResult> customTask, int index)
        {
            if (customTask == null)
                throw new ArgumentNullException("customTask");

            _InitTaskSource.Insert(index, customTask);
            return customTask.Id;
        }

        /// <summary>
        /// 查找任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CustomTask<InitTaskResult> Find(int id)
        {
            return _InitTaskSource.FirstOrDefault(o => o.Id == id);
        }

        /// <summary>
        /// 执行指定 <seealso cref="CustomTask{T}.Id"/> 的任务
        /// </summary>
        /// <param name="id"></param>
        public void Execute(int id)
        {
            var customTask = Find(id);
            if (customTask == null)
                return;

            Task task = new Task(() => ExecuteCustomTask(customTask, cts.Token));
            task.Start();
        }

        /// <summary>
        /// 执行全部任务
        /// </summary>
        public void Execute()
        {
            try
            {
                Task task = new Task(() =>
                {
                    foreach (var initTask in InitTaskSource)
                    {
                        ExecuteCustomTask(initTask, cts.Token);
                    }
                }, cts.Token, TaskCreationOptions.LongRunning);
                task.Start();
                task.ContinueWith((o) =>
                {
                    RaiseAllCustomTaskExecution(o.Status, o.Exception);
                });
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("Execute ", e);
                throw e;
            }
        }

        private CustomTaskEventArgs<InitTaskResult> ExecuteCustomTask(CustomTask<InitTaskResult> customTask, CancellationToken token)
        {
            RaiseBeginExecution(new CustomTaskEventArgs<InitTaskResult>(customTask));
            try
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();   // 抛出 OperationCanceledException 表示任务执行已取消

                InitTaskResult customTaskResult = customTask.ExecuteTask();
                var eventarg = new CustomTaskEventArgs<InitTaskResult>(customTask, customTaskResult);
                RaiseEndExecution(new CustomTaskEventArgs<InitTaskResult>(customTask, customTaskResult));
                return eventarg;
            }
            catch (OperationCanceledException ex)
            {
                LogHelper.logSoftWare.Error("CustomTask", ex);
                throw ex;
            }
            //catch (Exception ex)
            //{
            //    InitTaskResult result = new InitTaskResult { IsCancel = true, Message = ex.Message, Succeed = false };
            //    var eventarg = new CustomTaskEventArgs<InitTaskResult>(customTask, result, ex);
            //    RaiseEndExecution(new CustomTaskEventArgs<InitTaskResult>(customTask, result, ex));
            //    return eventarg;
            //}
        }


        /// <summary>
        /// 触发 BeginExecution 事件
        /// </summary>
        /// <param name="e"></param>
        private void RaiseBeginExecution(CustomTaskEventArgs<InitTaskResult> e)
        {
            var beginExecution = BeginExecution;
            if (beginExecution != null)
                beginExecution(this, e);
        }

        /// <summary>
        /// 触发 EndExecution 事件
        /// </summary>
        /// <param name="e"></param>
        private void RaiseEndExecution(CustomTaskEventArgs<InitTaskResult> e)
        {
            var endExecution = EndExecution;
            if (endExecution != null)
                endExecution(this, e);
        }
        /// <summary>
        /// 触发 AllCustomTaskExecution 事件
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ex"></param>
        public void RaiseAllCustomTaskExecution(TaskStatus status, AggregateException ex)
        {
            CustomTaskCollectionEventArgs<InitTaskResult> customTaskArgs = new CustomTaskCollectionEventArgs<InitTaskResult>();

            if (status == TaskStatus.RanToCompletion)
                customTaskArgs.Status = CustomTaskStatus.RanToCompletion;
            else if (status == TaskStatus.Canceled)
                customTaskArgs.Status = CustomTaskStatus.Canceled;
            else if (status == TaskStatus.Faulted)
                customTaskArgs.Status = CustomTaskStatus.Faulted;

            if (ex != null)
                customTaskArgs.Error = ex.GetBaseException();

            var allCustomTaskExecution = AllCustomTaskExecution;
            if (allCustomTaskExecution != null)
                allCustomTaskExecution(this, customTaskArgs);
        }

        /// <summary>
        /// 任务取消
        /// </summary>
        public void Cancel()
        {
            cts.Cancel();
        }
    }
}
