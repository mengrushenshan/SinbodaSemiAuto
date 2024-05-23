using Sinboda.Framework.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Infrastructure.Interface
{
    /// <summary>
    /// 自定义任务管理
    /// </summary>
    public interface ICustomTaskManager<T>
    {
        /// <summary>
        /// 开始执行前触发
        /// </summary>
        event EventHandler<CustomTaskEventArgs<T>> BeginExecution;
        /// <summary>
        /// 执行结果后触发
        /// </summary>
        event EventHandler<CustomTaskEventArgs<T>> EndExecution;
        /// <summary>
        /// 全部任务执行完成后触发
        /// </summary>
        event EventHandler<CustomTaskCollectionEventArgs<T>> AllCustomTaskExecution;
        /// <summary>
        /// 获取包含的自定义任务数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 获取已添加的所有任务列表
        /// </summary>
        List<CustomTask<T>> InitTaskSource { get; }
        /// <summary>
        /// 创建自定义任务
        /// </summary>
        /// <param name="customTask">任务描述</param>
        /// <returns>任务ID</returns>
        int AddInitTask(CustomTask<T> customTask);
        /// <summary>
        /// 创建任务并插入到指定位置
        /// </summary>
        /// <param name="customTask">任务描述</param>
        /// <param name="index">位置</param>
        /// <returns>任务ID</returns>
        int AddInitTask(CustomTask<T> customTask, int index);
        /// <summary>
        /// 根据任务ID查询指定任务
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns>未找到返回NULL</returns>
        CustomTask<T> Find(int id);
        /// <summary>
        /// 执行指定ID的任务
        /// </summary>
        void Execute(int id);
        /// <summary>
        /// 按顺序执行全部任务
        /// </summary>
        void Execute();
        /// <summary>
        /// 取消执行
        /// </summary>
        void Cancel();
    }

    /// <summary>
    /// 自定义任务事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomTaskEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 自定义任务
        /// </summary>
        public CustomTask<T> CustomTask { get; }
        /// <summary>
        /// 异常
        /// </summary>
        public Exception Error { get; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public T Result { get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="customTask"></param>
        /// <param name="result"></param>
        /// <param name="error"></param>
        public CustomTaskEventArgs(CustomTask<T> customTask, T result = default(T), Exception error = null)
        {
            CustomTask = customTask;
            Error = error;
            Result = result;
        }
    }
    /// <summary>
    /// 自定义任务集合事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomTaskCollectionEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 任务状态
        /// </summary>
        public CustomTaskStatus Status { get; internal set; }
        /// <summary>
        /// 任务异常
        /// </summary>
        public Exception Error { get; internal set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="status"></param>
        /// <param name="error"></param>
        public CustomTaskCollectionEventArgs(CustomTaskStatus status, Exception error)
        {
            Status = status;
            Error = error;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomTaskCollectionEventArgs()
        { }
    }
    /// <summary>
    /// 任务状态枚举
    /// </summary>
    public enum CustomTaskStatus
    {
        /// <summary>
        /// 通过调用 <seealso cref="ICustomTaskManager{T}.Cancel"/> 方法完成的任务
        /// </summary>
        Canceled,
        /// <summary>
        /// 由于未处理异常的原因而完成的任务。
        /// </summary>
        Faulted,
        /// <summary>
        /// 已成功完成执行的任务。
        /// </summary>
        RanToCompletion
    }
}
