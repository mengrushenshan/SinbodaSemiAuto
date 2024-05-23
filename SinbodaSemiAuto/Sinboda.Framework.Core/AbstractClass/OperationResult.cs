using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.AbstractClass
{
    /// <summary>
    /// 操作返回值类
    /// </summary>
    public class OperationResult
    {
        /// <summary>
        /// 结果布尔值
        /// </summary>
        public bool ResultBool
        {
            get { return ResultEnum == OperationResultEnum.SUCCEED; }
        }
        private OperationResultEnum _resultEnum;
        /// <summary>
        /// 结果枚举值
        /// </summary>
        public OperationResultEnum ResultEnum
        {
            get { return _resultEnum; }
            set
            {
                _resultEnum = value;
            }
        }
        /// <summary>
        /// 操作返回信息
        /// </summary>
        public string Message;
    }

    /// <summary>
    /// 操作返回值类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OperationResult<T> : OperationResult
    {
        /// <summary>
        /// 结果
        /// </summary>
        public T Results;
    }

    /// <summary>
    /// 结果枚举
    /// </summary>
    public enum OperationResultEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        SUCCEED,
        /// <summary>
        /// 失败
        /// </summary>
        FAILED,
    }
}
