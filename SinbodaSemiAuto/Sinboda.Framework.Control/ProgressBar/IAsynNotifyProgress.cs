using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Control.ProgressBar
{
    /// <summary>
    /// 异步通知消息
    /// </summary>
    public interface IAsynNotifyProgress
    {
        /// <summary>
        /// 总任务刻度
        /// </summary>
        double Total { get; }

        /// <summary>
        /// 已完成数量
        /// </summary>
        double Completed { get; }

        /// <summary>
        /// 是否完成
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// 完成百分比
        /// </summary>
        double Percent { get; }

        /// <summary>
        /// 是否执行成功，默认执行成功。若失败则Message会输出错误消息
        /// </summary>
        bool IsSuccess { get; set; }

        #region Methods

        /// <summary>
        /// 设置实际进度
        /// 直接设置进度
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        void SetProgress(double currProgress, string message = "");

        /// <summary>
        /// 完成（手动或自动调用）
        /// </summary>
        void Complete();

        /// <summary>
        /// 取消操作，若已完成则不处理。
        /// </summary>
        void Cancel();
        #endregion

        #region Events

        /// <summary>
        /// 完成时的通知操作
        /// </summary>
        Action OnCompleted { get; set; }

        #endregion
    }
}
