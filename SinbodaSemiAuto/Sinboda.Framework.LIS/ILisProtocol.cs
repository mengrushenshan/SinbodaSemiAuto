using Sinboda.Framework.LIS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS
{
    /// <summary>
    /// 定义接收委托
    /// </summary>
    public delegate void InceptEvent(object sender, EventArgs e);
    /// <summary>
    /// 错误委托
    /// </summary>
    public delegate void ErrorEvent(object sender, EventArgs e);
    public delegate void ConnectEvent(object sender);
    public delegate void DisconnectEvent(object sender);
    public delegate void TimeoutEvent(object sender);

    /// <summary>
    /// LIS接口
    /// </summary>
    public interface ILISProtocol
    {
        /// <summary>
        /// 连接或打开
        /// </summary>
        /// <returns></returns>
        bool Connect();
        /// <summary>
        /// 断开或关闭
        /// </summary>
        void Disconnect();
        /// <summary>
        /// 判断是否联机
        /// </summary>
        bool Connected { get; }
        /// <summary>
        /// 设备通讯方式
        /// </summary>
        CommunicateType CommunicateType { get; }

        /// <summary>
        /// 通讯参数
        /// </summary>
        LISProtocolParameter Parameter { get; set; }
        ///// <summary>
        ///// LIS客户端
        ///// </summary>
        //Component LisClient { get; set; }
        /// <summary>
        /// 发送Message数据
        /// </summary>
        void SendMessage();
        /// <summary>
        /// 服务端 发送数据
        /// </summary>
        /// <param name="socketIP">连接客户端IP</param>
        /// <param name="sendData">发送数据</param>
        void SendMessage(string socketIP, Component sendData);
        /// <summary>
        /// 发送一个流数据，目前网口使用
        /// </summary>
        /// <param name="stream"></param>
        void SendStream(Stream stream);
        /// <summary>
        /// 发送lis绑定数据
        /// </summary>
        Component SendData { get; set; }
        /// <summary>
        /// 串口使用：是否拆分发送，默认是true
        /// </summary>
        bool IsSplitSend { get; set; }
        /// <summary>
        /// 字符编码
        /// </summary>
        Common.Encoding Encoding { get; set; }
        /// <summary>
        /// 串口使用：默认200,每一帧最多包含207个字符（包含帧开头和结尾字符），
        /// 超过200个字符的消息被分成两帧或多帧
        /// </summary>
        int FrameLength { get; set; }
        /// <summary>
        /// 串口使用：超时重发次数
        /// </summary>
        int ReSendTime { get; set; }
        /// <summary>
        /// 发送超时
        /// </summary>
        int Timeout { get; set; }
    }
}
