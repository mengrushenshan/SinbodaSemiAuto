using Sinboda.Framework.Communication.DataPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication
{
    /// <summary>
    /// 客户端模式设备操作接口
    /// </summary>
    public interface IMultiDeviceProtocol
    {
        /// <summary>
        /// 连接或打开
        /// </summary>
        /// <returns></returns>
        Dictionary<string, bool> Connect(string[] id = null);
        /// <summary>
        /// 断开或关闭
        /// </summary>
        void Disconnect(string[] id = null);
        /// <summary>
        /// 判断是否联机
        /// </summary>
        bool GetConnected(string id);
        /// <summary>
        /// 设备通讯方式
        /// </summary>
        CommunicateType CommunicateType { get; }
        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="package">数据包</param>
        /// <returns>发送是否成功</returns>
        bool SendPackage(DataPackage package);
        /// <summary>
        /// 接收数据包事件
        /// </summary>
        event ClientDataReceivedEventHandler DataReceivedEvent;
        /// <summary>
        /// 错误事件
        /// </summary>
        event CommunicateErrorEventHandler CommunicateErrorEvent;
    }
}
