using Sinboda.Framework.LIS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    /// <summary>
    /// 串口LIS客户端模式
    /// </summary>
    public class SerialPortClientLISProtocol : ClientLISProtocol
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SerialPortClientLISProtocol() : base()
        {

        }
        /// <summary>
        /// ASTM客户端对象
        /// </summary>
        protected ASTMClient _astmClient = new ASTMClient();
        /// <summary>
        /// 通讯参数
        /// </summary>
        /// <returns></returns>
        protected override LISProtocolParameter CreateParameterInstance()
        {
            return (new SerialPortParameter());
        }
        /// <summary>
        /// 创建LIS客户端对象
        /// </summary>
        /// <returns></returns>
        protected override Component CreateLISClientInstance()
        {
            return _astmClient;
        }

        protected ASTMMessage _sendASTMSource = new ASTMMessage();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Component CreateLISSendDataInstance()
        {
            return _sendASTMSource;
        }
        /// <summary>
        /// 通讯类别
        /// </summary>
        /// <returns></returns>
        protected override CommunicateType GetCommunicateType()
        {
            return CommunicateType.SerialPort;
        }
        /// <summary>
        /// 串口连接
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            try
            {
                _astmClient.Encoding = base.Encoding;
                _astmClient.Timeout = (Parameter as SerialPortParameter).SendTimeout;
                _astmClient.ReSendTime = (Parameter as SerialPortParameter).ReSendTime;
                _astmClient.FrameLength = (Parameter as SerialPortParameter).FrameLength;
                _astmClient.IsSplitSend = (Parameter as SerialPortParameter).IsSplitSend;
                _astmClient.Parameter = (Parameter as SerialPortParameter);
                return _astmClient.Connect();
            }
            catch (Exception ex)
            {
                //LogHelper.logSoftWare.Error("SerialPort Connect error", ex);
                return false;
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public override void Disconnect()
        {
            _astmClient.Disconnect();
        }
        public override void Disconnect(bool isThrow)
        {
            _astmClient.Disconnect();
        }
        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        protected override bool GetConnected()
        {
            return _astmClient.GetConnected();
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
                str = ((ASTMMessage)base.SendData).ASTMData.ToString();
            }
            return str;
        }
        /// <summary>
        /// 发送一个流数据,网口暂时不支持，有需要再支持
        /// </summary>
        /// <param name="stream">数据流</param>
        public override void SendStream(Stream stream)
        {
            
        }
        /// <summary>
        /// 发送ASTM Message数据
        /// </summary>
        public override void SendMessage()
        {
            _astmClient.SendASTMSource = (ASTMMessage)base.SendData;
            _astmClient.SendASTM();
        }
        /// <summary>
        /// 发送命令
        /// </summary>
        public override void SendCommand(string command)
        {
            _astmClient.SendCommand(command);
        }
        /// <summary>
        /// 接收到数据引发的事件
        /// </summary>
        public override event InceptEvent OnAccept
        {
            add { _astmClient.OnAccept += value; }
            remove { _astmClient.OnAccept -= value; }
        }
        /// <summary>
        /// 发生错误引发的事件
        /// </summary>
        public override event ErrorEvent OnError
        {
            add { _astmClient.OnError += value; }
            remove { _astmClient.OnError -= value; }
        }
        /// <summary>
        /// 连接事件
        /// </summary>
        public override event ConnectEvent OnConnect
        {
            add { _astmClient.OnConnect += value; }
            remove { _astmClient.OnConnect -= value; }
        }
        /// <summary>
        /// 断开连接事件
        /// </summary>
        public override event DisconnectEvent OnDisconnect
        {
            add { _astmClient.OnDisconnect += value; }
            remove { _astmClient.OnDisconnect -= value; }
        }
        /// <summary>
        ///  连接超时事件，串口没有连接超时
        /// </summary>
        public override event TimeoutEvent OnTimeout;
    }
}
