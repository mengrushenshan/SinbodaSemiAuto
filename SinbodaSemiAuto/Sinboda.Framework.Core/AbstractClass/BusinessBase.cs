using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.Framework.Core.AbstractClass
{
    /// <summary>
    /// 业务层基类
    /// </summary>
    public abstract class BusinessBase<TBusiness> : TBaseSingleton<TBusiness> where TBusiness : new()
    {
        /// <summary>
        /// 返回一个状态为 <see cref="OperationResultEnum.FAILED"/>
        /// 消息为 <see cref="Exception.Message"/> 的 <see cref="OperationResult"/> 实例
        /// </summary>
        /// <param name="e">异常</param>
        /// <returns></returns>
        public OperationResult Result(Exception e)
        {
            WriteLog(e);
            return Result(OperationResultEnum.FAILED, e != null ? e.Message : string.Empty);
        }

        /// <summary>
        /// 返回成功及失败
        /// </summary>
        /// <param name="resultEnum"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public OperationResult Result(OperationResultEnum resultEnum, string message)
        {
            return new OperationResult { ResultEnum = resultEnum, Message = message };
        }
        /// <summary>
        /// 返回成功及失败
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public OperationResult Result(bool result, string message)
        {
            return Result(result ? OperationResultEnum.SUCCEED : OperationResultEnum.FAILED, message);
        }
        /// <summary>
        /// 返回成功及失败
        /// </summary>
        /// <param name="resultEnum"></param>
        /// <returns></returns>
        public OperationResult Result(OperationResultEnum resultEnum)
        {
            return Result(resultEnum, "");
        }
        /// <summary>
        /// 返回成功及失败
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public OperationResult Result(bool result)
        {
            return Result(result ? OperationResultEnum.SUCCEED : OperationResultEnum.FAILED, "");
        }

        /// <summary>
        /// 返回正常时结果信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resultEnum"></param>
        /// <param name="results"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public OperationResult<T> Result<T>(OperationResultEnum resultEnum, T results = default(T), string message = "")
        {
            return new OperationResult<T> { ResultEnum = resultEnum, Results = results, Message = message };
        }

        /// <summary>
        /// 返回正常时结果信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="results"></param>
        /// <returns></returns>
        public OperationResult<T> Result<T>(T results)
        {
            return Result(OperationResultEnum.SUCCEED, results);
        }

        /// <summary>
        /// 返回异常时错误信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public OperationResult<T> Result<T>(Exception e)
        {
            WriteLog(e);
            return Result(OperationResultEnum.FAILED, default(T), e != null ? e.Message : string.Empty);
        }

        private void WriteLog(Exception e)
        {
            if (e == null) return;

#if DEBUG
            NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[6467], e.ToStringEx(), MessageBoxButton.OK, SinMessageBoxImage.Error); //TODO 翻译
#endif
            LogHelper.logSoftWare.Error("错误信息", e); //TODO 翻译
        }
    }
}
