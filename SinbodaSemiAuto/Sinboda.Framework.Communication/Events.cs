using Sinboda.Framework.Communication.DataPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication
{
    /// <summary>
    /// 接收事件
    /// </summary>
    public class ReceivedEventArgs : EventArgs
    {
        private DataPackage dataPackage;
        /// <summary>
        /// 数据包
        /// </summary>
        public DataPackage DataPackage
        {
            get
            {
                return dataPackage;
            }
        }

        internal ReceivedEventArgs(DataPackage package)
        {
            dataPackage = package;
        }
    }

    /// <summary>
    /// 通讯错误事件
    /// </summary>
    public class CommunicateErrorEventArgs : EventArgs
    {
        private CommunicateError communicateError;
        private Exception exception;
        /// <summary>
        /// 错误类型
        /// </summary>
        public CommunicateError ErrorType
        {
            get
            {
                return communicateError;
            }
        }

        internal CommunicateErrorEventArgs(CommunicateError error, Exception e)
        {
            communicateError = error;
            exception = e;
        }
    }
}
