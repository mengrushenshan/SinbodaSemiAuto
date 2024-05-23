using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace Sinboda.Framework.LIS.SinHL7
{

    /// <summary>
    /// 错误事件
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        private readonly Exception error;
        private readonly Socket workSocket;
        private readonly string errorCode;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Error">错误信息对象</param>
        public ErrorEventArgs(Exception Error, Socket WorkSocket, string errCode = "")
        {
            error = Error;
            workSocket = WorkSocket;
            errorCode = errCode;
        }
        /// <summary>
        /// 错误信息对象
        /// </summary>
        public Exception Error
        {
            get { return error; }
        } 
        /// <summary>
        /// 问题插座
        /// </summary>
        public Socket WorkSocket
        {
            get { return workSocket; }
        }
        /// <summary>
        /// 问题插座
        /// </summary>
        public string ErrorCode
        {
            get { return errorCode; }
        }
    }

}
