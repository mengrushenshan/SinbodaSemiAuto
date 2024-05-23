using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication
{
    /// <summary>
    /// 通讯类别
    /// </summary>
    public enum CommunicateType
    {
        /// <summary>
        /// 串口
        /// </summary>
        SerialPort,
        /// <summary>
        /// 网口
        /// </summary>
        Network,
    }
    /// <summary>
    /// 通讯错误类别
    /// </summary>
    public enum CommunicateError
    {
        /// <summary>
        /// 空
        /// </summary>
        None,
        /// <summary>
        /// 连接失败
        /// </summary>
        ConnectFailed,
        /// <summary>
        /// 连接超时
        /// </summary>
        ConnectTimeout,
        /// <summary>
        /// 应答超时
        /// </summary>
        ReplyTimeout,
        /// <summary>
        /// 接收数据超时
        /// </summary>
        ReceiveTimeout,
        /// <summary>
        /// 心跳检查超时
        /// </summary>
        KeepAliveTimeout,
        /// <summary>
        /// 校验数据错误
        /// </summary>
        CheckDataError,
        /// <summary>
        /// 接收失败
        /// </summary>
        ReceiveFailed,
        /// <summary>
        /// 发送失败
        /// </summary>
        SendFailed,
        /// <summary>
        /// 服务器停止
        /// </summary>
        ServerStop,
        /// <summary>
        /// 通讯异常
        /// </summary>
        CommunicationException,
        /// <summary>
        /// 未知错误
        /// </summary>
        UnknownError,
    }
}
