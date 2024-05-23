using System;
using System.IO;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Sinboda.Framework.Common.Log;

namespace Sinboda.Framework.LIS.SinHL7
{
    /// <summary>
    /// 提供在Net TCP_IP 协议上基于HL7消息的客户端 
    /// </summary>
    public class HL7Client : HL7Socket
    {
        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="container">父控件</param>
        public HL7Client(System.ComponentModel.IContainer container) : this() { container.Add(this); }

        /// <summary>
        /// 构造
        /// </summary>
        public HL7Client() : base() { }

        #endregion

        #region 私有方法
        private void ConnectCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            try
            {
                LogHelper.logLisComm.Info("【LIS底层】进入ConnectCallback");
                client.EndConnect(ar);
                if (client.Connected)
                {
                    if (OnConnect != null)
                    {
                        OnConnect(this);
                    }
                    //设置keep alive
                    SetIOControl(client);
                    StateObj state = new StateObj(_bufferSize, client);
                    client.BeginReceive(state.buffer, 0, _bufferSize, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback), state);
                    LogHelper.logLisComm.Info("【LIS底层】ConnectCallback 连接成功");
                }
                else
                {
                    LogHelper.logLisComm.Info("【LIS底层】ConnectCallback 连接失败");
                }
            }
            catch (Exception e)
            {
                LogHelper.logLisComm.Info("【LIS底层】ConnectCallback 异常：" + e.Message);
                Close(client, false);
                OnErrorEvent(new ErrorEventArgs(e, client));
            }
            finally
            {
                connectDone.Set();
            }
        }

        /// <summary>
        /// 析构
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            //this.Close();
        }



        #endregion

        #region 公开方法
        /// <summary>
        /// 连接服务器
        /// </summary>
        public void Conn()
        {
            try
            {
                //对socket再进行关闭，防止存在没关闭成功的链接
                FinallyClose(_socket);
                _socket = null;
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse(_ipAddress);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, _port);
                connectDone.Reset();
                _socket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), _socket);
                if (!connectDone.WaitOne(_timeout, false))
                {
                    LogHelper.logLisComm.Info("【LIS底层】连接超时");
                    if (OnTimeout != null)
                    {
                        OnTimeout(this);
                    }
                }
            }
            catch (Exception e)
            {
                OnErrorEvent(new ErrorEventArgs(e, _socket));
            }
        }
        public void Close(bool isThrowException)
        {
            Close(_socket, isThrowException);
            _socket = null;
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        private void Close(Socket socket, bool isThrowException)
        {
            try
            {
                if (socket != null)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    //_socket.Disconnect(true);
                    socket.Close();
                    LogHelper.logLisComm.Info("【LIS底层】Client Close");
                    OnDisconnectEvent(null);
                }
            }
            catch (Exception e)
            {
                LogHelper.logLisComm.Info("【LIS底层】Client Close异常：" + e.ToString());
                if (isThrowException)
                    OnErrorEvent(new ErrorEventArgs(e, _socket));
            }
        }
        /// <summary>
        /// 断开连接,不对外触发事件
        /// </summary>
        private void FinallyClose(Socket socket)
        {
            try
            {
                if (socket != null)
                {
                    //socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    LogHelper.logLisComm.Info("【LIS底层】Client FinallyClose");
                }
            }
            catch (Exception e)
            {
                LogHelper.logLisComm.Info("【LIS底层】Client FinallyClose异常：" + e.ToString());
            }
            //finally
            //{
            //    socket = null;
            //}
        }
        /// <summary>
        /// 发送一个流数据
        /// </summary>
        /// <param name="Astream">数据流</param>
        public void Send(Stream Astream)
        {
            base.Send(_socket, Astream);
        }

        /// <summary>
        /// 发送HL7Message流数据
        /// </summary>
        public void SendHL7()
        {
            base.SendHL7(_socket);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 连接超时
        /// </summary>
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }

        }
        /// <summary>
        /// 开始第一次 send 心跳包,毫秒
        /// </summary>
        public uint KeepAliveFlag
        {
            get
            {
                return keepAliveFlag;
            }
            set
            {
                keepAliveFlag = value;
            }

        }
        /// <summary>
        /// 开始第一次 send 心跳包,毫秒
        /// </summary>
        public int KeepAliveTime
        {
            get
            {
                return keepAliveTime;
            }
            set
            {
                keepAliveTime = value;
            }

        }
        /// <summary>
        /// 每隔多长时间(ms) send 一个心跳包
        /// </summary>
        public int KeepAliveInterval
        {
            get
            {
                return keepAliveInterval;
            }
            set
            {
                keepAliveInterval = value;
            }

        }
        /// <summary>
        /// 要连接的服务器IP地址
        /// </summary>
        public string ServerIP
        {
            get
            {
                return _ipAddress;
            }
            set
            {
                _ipAddress = value;
            }
        }

        protected override void OnDisconnectEvent(Socket WorkSocket)
        {
            autoResetEvent.Set();
            if (OnDisconnect != null)
            {
                OnDisconnect(this);
            }
        }

        public void SetAutoReset(ManualResetEvent resetEvent)
        {
            autoResetEvent = resetEvent;
        }
        #endregion

        #region 事件
        public event ConnectEvent OnConnect;
        public event DisconnectEvent OnDisconnect;
        public event TimeoutEvent OnTimeout;

        #endregion
    }
}
