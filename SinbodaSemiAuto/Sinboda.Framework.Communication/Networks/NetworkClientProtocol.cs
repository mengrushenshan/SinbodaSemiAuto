using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication.Networks
{
    /// <summary>
    /// 网口客户端模式
    /// </summary>
    public class NetworkClientProtocol : ClientDeviceProtocol
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NetworkClientProtocol() : base()
        {

        }
        /// <summary>
        /// 网络连接
        /// </summary>
        protected TcpClient _tcpClient = new TcpClient();
        /// <summary>
        /// 通讯参数
        /// </summary>
        /// <returns></returns>
        protected override ProtocolParameter CreateParameterInstance()
        {
            return (new NetworkParameter());
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            IPAddress ip;
            if (!IPAddress.TryParse((Parameter as NetworkParameter).RemoteAddress, out ip))
            {
                ip = IPAddress.Any;
            }
            int port = (Parameter as NetworkParameter).RemotePort;
            int rsiez = (Parameter as NetworkParameter).ReceiveBufferSize;
            int ssize = (Parameter as NetworkParameter).SendBufferSize;

            var tem = ConnectCore(ip, port, rsiez, ssize, Parameter.Timeout, Parameter.ReconnectionTime);

            if (!tem || !_tcpClient.Connected)
                return false;

            StartThreads();
            return tem;
        }


        private bool ConnectCore(IPAddress ip, int port, int receiveBufferSize, int sendBufferSize, int timeout, int reCount)
        {
            try
            {
                if (reCount <= 0)
                {
                    LogHelper.logCommunication.Error($"尝试 {Parameter.ReconnectionTime} 次连接仍无法成功，通讯创建失败");
                    return false;
                }

                LogHelper.logCommunication.Info($"第 {Parameter.ReconnectionTime - reCount + 1} 次连接");

                _tcpClient = new TcpClient();
                _tcpClient.ReceiveBufferSize = receiveBufferSize;
                _tcpClient.SendBufferSize = sendBufferSize;
                var task = _tcpClient.ConnectAsync(ip, port);
                if (!task.Wait(timeout))
                {
                    _tcpClient.Close();
                    LogHelper.logCommunication.Info($"第 {Parameter.ReconnectionTime - reCount + 1} 次通讯，连接超时");
                    return ConnectCore(ip, port, receiveBufferSize, sendBufferSize, timeout, --reCount);
                }
                else
                {
                    return true;
                }
            }
            catch (AggregateException ex)
            {

                _tcpClient.Close();
                foreach (var es in ex.InnerExceptions)
                {
                    Exception e = es;
                    while (es.InnerException != null)
                        e = es.InnerException;

                    if (e is SocketException)
                        AddNetSocketExceptionLog(e as SocketException);
                    else
                        AddNetExceptionLog(e);
                }

                return ConnectCore(ip, port, receiveBufferSize, sendBufferSize, timeout, --reCount);
            }
            catch (SocketException ex)
            {
                _tcpClient.Close();
                AddNetSocketExceptionLog(ex);
                ConnectCore(ip, port, receiveBufferSize, sendBufferSize, timeout, --reCount);
                return false;
            }
            catch (Exception ex)
            {
                _tcpClient.Close();
                AddNetExceptionLog(ex);
                return ConnectCore(ip, port, receiveBufferSize, sendBufferSize, timeout, --reCount);
            }
        }

        private void AddNetSocketExceptionLog(SocketException ex)
        {
            LogHelper.logCommunication.Error("NetworkClient Connect error", ex);
            var errorCode = ex.ErrorCode;
            var socketErrorCode = ex.SocketErrorCode;
            LogHelper.logCommunication.Error("SocketException：ErrorCode " + errorCode + "，socketErrorCode " + socketErrorCode);
        }

        private void AddNetExceptionLog(Exception ex)
        {
            LogHelper.logCommunication.Error("NetworkClient Connect error", ex);
        }

        /// <summary>
        /// 断开
        /// </summary>
        public override void Disconnect()
        {
            if (GetConnected())
            {
                StopThreads();
                _tcpClient.Close();
            }
        }
        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        protected override bool GetConnected()
        {
            return _tcpClient.Client == null ? false : _tcpClient.Connected;
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
        /// 发数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override bool WriteBuffer(byte[] buffer, int offset, int count)
        {
            //try
            //{
            _tcpClient.GetStream().Write(buffer, offset, count);
            return true;
            //}
            //catch(Exception ex)
            //{
            //    LogHelper.logCommunication.Error("NetworkClient WriteBuffer error", ex);
            //    return false;
            //}
        }
        /// <summary>
        /// 收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override int ReadBuffer(byte[] buffer, int offset, int count)
        {
            //try
            //{
            return _tcpClient.GetStream().Read(buffer, offset, count);
            //}
            //catch(Exception ex)
            //{
            //    LogHelper.logCommunication.Error("NetworkClient ReadBuffer error",ex);
            //    return -1;
            //}
        }
    }
}
