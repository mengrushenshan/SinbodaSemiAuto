using Sinboda.Framework.Communication.DataPackages;
using Sinboda.Framework.Communication.Networks;
using Sinboda.Framework.Communication.SerialPorts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication
{
    /// <summary>
    /// 通讯处理工厂类
    /// </summary>
    public class ProtocolFactoryHandle
    {
        /// <summary>
        /// 根据协议参数,实例化客户端模式设备协议
        /// </summary>
        /// <param name="p_paramter">协议参数</param>
        /// <param name="packageInfo">心跳信息</param>
        /// <returns></returns>
        public static ClientDeviceProtocol GetClientInstanceProtocol(ProtocolParameter p_paramter, PackageInfo packageInfo = null)
        {
            ClientDeviceProtocol _protocol = null;
            if (p_paramter is SerialPortParameter)
            {
                _protocol = new SerialPortClientProtocol();
                _protocol.Parameter = p_paramter;
                _protocol.PackageHandler(CommunicateType.SerialPort);
                _protocol.KeepAlivePackegeInfo = packageInfo;
            }
            else
            {
                _protocol = new NetworkClientProtocol();
                _protocol.Parameter = p_paramter;
                _protocol.PackageHandler(CommunicateType.Network);
                _protocol.KeepAlivePackegeInfo = packageInfo;
            }
            return _protocol;
        }
        /// <summary>
        /// 根据协议参数,实例化服务器模式设备协议
        /// </summary>
        /// <param name="communicateType">通讯类型</param>
        /// <param name="packageInfo">心跳信息</param>
        /// <returns></returns>
        public static MultiClientDeviceProtocol GetMultiClientInstanceProtocol(CommunicateType communicateType, PackageInfo packageInfo = null, EncryptMode isNeedEncrypt = EncryptMode.None)
        {
            MultiClientDeviceProtocol _protocol = null;
            if (communicateType == CommunicateType.SerialPort)
            {
                _protocol = new SerialPortMultiClientProtocol();
                _protocol.PackageHandler(CommunicateType.SerialPort);
                //_protocol.KeepAlivePackegeInfo = packageInfo;
                _protocol.ISNeedEncrypt = isNeedEncrypt;
            }
            else if (communicateType == CommunicateType.Network)
            {
                _protocol = new NetworkMutltiClientProtocol();
                _protocol.PackageHandler(CommunicateType.Network);
                //_protocol.KeepAlivePackegeInfo = packageInfo;
                _protocol.ISNeedEncrypt = isNeedEncrypt;
            }

            return _protocol;
        }
    }

    public enum EncryptMode
    {
        /// <summary>
        /// 无加密
        /// </summary>
        None,
        /// <summary>
        /// 固定加密
        /// </summary>
        Standard,
        /// <summary>
        /// 握手加密
        /// </summary>
        ShakeHand
    }
}
