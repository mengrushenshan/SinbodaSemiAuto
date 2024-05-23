using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    /// <summary>
    /// 错处事件
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        private readonly Exception error;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Error">错误信息对象</param>
        public ErrorEventArgs(Exception Error)
        {
            error = Error;
        }
        /// <summary>
        /// 错误信息对象
        /// </summary>
        public Exception Error
        {
            get { return error; }
        }
    }
}
