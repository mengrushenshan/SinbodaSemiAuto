using Sinboda.Framework.Common.Log;
using Sinboda.Framework.LIS.Common;
using Sinboda.Framework.LIS.SinHL7;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.Network
{
    public class NetworkServiceLISProtocol : ServiceLISProtocol
    {
        /// <summary>
        /// HL7客户端对象
        /// </summary>
        protected HL7Server _hl7Service = new HL7Server();

        public NetworkServiceLISProtocol() : base()
        {
        }

        protected override CommunicateType GetCommunicateType()
        {
            return CommunicateType.Network;
        }

        protected override LISProtocolParameter CreateParameterInstance()
        {
            return (new NetworkParameter());
        }

        public override bool Connect()
        {
            lock (lockObj)
            {
                try
                {
                    _hl7Service.Encoding = base.Encoding;
                    //_hl7Service.Timeout = (Parameter as NetworkParameter).Timeout;
                    _hl7Service.BufferSize = (Parameter as NetworkParameter).SendBufferSize;
                    _hl7Service.ServerIP = (Parameter as NetworkParameter).RemoteAddress;
                    _hl7Service.Port = (Parameter as NetworkParameter).RemotePort;
                    LogHelper.logCommunication.Info(string.Format("【LIS底层】NetworkParameter, IP:{0}, Port:{1}，BufferSize:{2}", 
                        _hl7Service.ServerIP, _hl7Service.Port, _hl7Service.BufferSize));
                    _hl7Service.Listening();
                    return true;
                }
                catch (Exception ex)
                {
                    LogHelper.logLisComm.Info("【LIS底层】NetworkProtocol:" + ex.ToString());
                    return false;
                }
            }
        }

        public override void Disconnect()
        {
            _hl7Service.Abort();
        }

        public override string MessageToString()
        {
            return string.Empty;
        }

        public override void SendMessage(string socketIP, Component sendData)
        {
            HL7Message msg = (HL7Message)sendData;
            _hl7Service.SendHL7ToClient(socketIP, msg);
        }

        /// <summary>
        /// 接收到数据引发的事件
        /// </summary>
        public override event InceptEvent OnAccept
        {
            add { _hl7Service.OnAccept += value; }
            remove { _hl7Service.OnAccept -= value; }
        }
        /// <summary>
        /// 发生错误引发的事件
        /// </summary>
        public override event ErrorEvent OnError
        {
            add { _hl7Service.OnError += value; }
            remove { _hl7Service.OnError -= value; }
        }
        /// <summary>
        /// 连接事件
        /// </summary>
        public override event ConnectEvent OnConnect
        {
            add { _hl7Service.OnConnect += value; }
            remove { _hl7Service.OnConnect -= value; }
        }
        /// <summary>
        /// 断开连接事件
        /// </summary>
        public override event DisconnectEvent OnDisconnect
        {
            add { _hl7Service.OnDisconnect += value; }
            remove { _hl7Service.OnDisconnect -= value; }
        }
    }
}
