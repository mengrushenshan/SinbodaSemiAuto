using Sinboda.Framework.Common.Log;
using Sinboda.Framework.LIS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinHL7
{
    /// <summary>
    /// 
    /// </summary>
    public class NetworkClientLISProtocol : ClientLISProtocol
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NetworkClientLISProtocol(ManualResetEvent resetEvent) : base()
        {
            //autoResetEvent = resetEvent;
            _hl7Client.SetAutoReset(resetEvent);
        }

        /// <summary>
        /// 通讯参数
        /// </summary>
        /// <returns></returns>
        protected override LISProtocolParameter CreateParameterInstance()
        {
            return (new NetworkParameter());
        }
        /// <summary>
        /// 通讯类别
        /// </summary>
        /// <returns></returns>
        protected override CommunicateType GetCommunicateType()
        {
            return CommunicateType.Network;
        }
        /// <summary>
        /// HL7客户端对象
        /// </summary>
        protected HL7Client _hl7Client = new HL7Client();
        /// <summary>
        /// 创建LIS客户端对象
        /// </summary>
        /// <returns></returns>
        protected override Component CreateLISClientInstance()
        {
            return _hl7Client;
        }

        protected HL7Message _sendHL7Source = new HL7Message();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Component CreateLISSendDataInstance()
        {
            return _sendHL7Source;
        }
        /// <summary>
        /// 网口连接
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            lock (lockObj)
            {
                try
                {
                    _hl7Client.Encoding = base.Encoding;
                    _hl7Client.Timeout = (Parameter as NetworkParameter).Timeout;
                    _hl7Client.BufferSize = (Parameter as NetworkParameter).SendBufferSize;
                    _hl7Client.ServerIP = (Parameter as NetworkParameter).RemoteAddress;
                    _hl7Client.Port = (Parameter as NetworkParameter).RemotePort;
                    LogHelper.logLisComm.Info(string.Format("【LIS底层】NetworkParameter, IP:{0}, Port:{1}, Timeout:{2} ms", _hl7Client.ServerIP, _hl7Client.Port, _hl7Client.Timeout));
                    _hl7Client.Conn();
                    return _hl7Client.Active;
                }
                catch (Exception ex)
                {
                    LogHelper.logLisComm.Info("【LIS底层】NetworkProtocol:" + ex.ToString());
                    return false;
                }
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public override void Disconnect()
        {
            _hl7Client.Close(true);
        }
        public override void Disconnect(bool isThrow)
        {
            _hl7Client.Close(isThrow);
        }
        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        protected override bool GetConnected()
        {
            return _hl7Client.Active;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string MessageToString()
        {
            string str = string.Empty;
            if (base.SendData != null)
            {
                str = ((HL7Message)base.SendData).HL7.ToString();
            }
            return str;
        }
        /// <summary>
        /// 发送一个流数据
        /// </summary>
        /// <param name="stream">数据流</param>
        public override void SendStream(Stream stream)
        {
            _hl7Client.Send(stream);
        }
        /// <summary>
        /// 发送HL7Message流数据
        /// </summary>
        public override void SendMessage()
        {
            _hl7Client.SendHL7Source = (HL7Message)base.SendData;
            _hl7Client.SendHL7();
        }

        public override void SendMessage(string socketIP, Component sendData)
        {
            base.SendData = sendData;
            SendMessage();
        }
        ///// <summary>
        ///// 发送HL7Message流数据
        ///// </summary>
        //public override void SendMessage(Component message)
        //{
        //    SendData = message;
        //    SendMessage();
        //}

        /// <summary>
        /// 接收到数据引发的事件
        /// </summary>
        public override event InceptEvent OnAccept
        {
            add { _hl7Client.OnAccept += value; }
            remove { _hl7Client.OnAccept -= value; }
        }
        /// <summary>
        /// 发生错误引发的事件
        /// </summary>
        public override event ErrorEvent OnError
        {
            add { _hl7Client.OnError += value; }
            remove { _hl7Client.OnError -= value; }
        }
        /// <summary>
        /// 连接事件
        /// </summary>
        public override event ConnectEvent OnConnect
        {
            add { _hl7Client.OnConnect += value; }
            remove { _hl7Client.OnConnect -= value; }
        }
        /// <summary>
        /// 断开连接事件
        /// </summary>
        public override event DisconnectEvent OnDisconnect
        {
            add { _hl7Client.OnDisconnect += value; }
            remove { _hl7Client.OnDisconnect -= value; }
        }
        /// <summary>
        ///  连接超时事件
        /// </summary>
        public override event TimeoutEvent OnTimeout
        {
            add { _hl7Client.OnTimeout += value; }
            remove { _hl7Client.OnTimeout -= value; }
        }
    }
}
