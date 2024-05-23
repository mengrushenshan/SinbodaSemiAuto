
using System;
using System.IO;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sinboda.Framework.Common.Log;

namespace Sinboda.Framework.LIS.SinHL7
{
    /// <summary>
    /// 提供在Net TCP_IP 协议上基于HL7消息的服务端
    /// </summary>
    public class HL7Server : HL7Socket
    {
        #region 变量
        private Thread _workthread = null;
        private Dictionary<string, Socket> _listeners = new Dictionary<string, Socket>();
        private int _backlog = 10;
        #endregion

        #region 属性

        /// <summary>
        /// 服务器IP
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

        /// <summary>
        /// 挂起连接队列的最大长度
        /// </summary>
        public int Backlog
        {
            set
            {
                _backlog = value;
            }
            get
            {
                return _backlog;
            }
        }
        #endregion

        #region 事件
        public event ConnectEvent OnConnect;
        public event DisconnectEvent OnDisconnect;
        #endregion

        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="container">父控件</param>
        public HL7Server(System.ComponentModel.IContainer container) : this() { container.Add(this); }

        /// <summary>
        /// 构造
        /// </summary>
        public HL7Server() : base() { _ipAddress = ""; }
        #endregion

        #region 方法 公开
        /// <summary>
        /// 开始监听访问
        /// </summary>
        public void Listening()
        {
            //StartListening();
            _workthread = new Thread(new ThreadStart(StartListening));
            _workthread.IsBackground = true;
            _workthread.Name = "HL7Server.Listening";
            _workthread.Start();
        }
        /// <summary>
        /// 异常中止服务
        /// </summary>
        public void Abort()
        {
            if (_workthread != null)
            {
                foreach(KeyValuePair<string,Socket> pair in _listeners)
                {
                    BaseClose(pair.Value);
                }
                _listeners.Clear();
                _workthread.Abort();
                _socket.Close();
            }
        }

        public void SendHL7ToClient(string clientIP, HL7Message sendMsg)
        {
            if (!_listeners.ContainsKey(clientIP))
            {
                LogHelper.logLisComm.Info("【LIS底层】未连接该客户端:" + clientIP);
                return;
            }

            Stream stream = null;

            if (sendMsg != null)
            {
                LogHelper.logLisComm.Info("【LIS底层】发送到LIS的原始数据:" + sendMsg.HL7.ToString());
                stream = sendMsg.HL7.ToStream();
            }

            if (null == stream)
            {
                LogHelper.logLisComm.Info("【LIS底层】发送到LIS的原始数据为空");
                return;
            }

            base.Send(_listeners[clientIP], stream);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        protected override void BaseClose(Socket clientSocket)
        {
            try
            {
                if (clientSocket != null)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch (Exception e)
            {
                LogHelper.logCommunication.Info("【LIS底层 Server】BaseClose Exception：" + e.Message);
            }
        }

        #endregion

        #region 方法 私有
        private void StartListening()
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                IPAddress ipAddress;
                if (_ipAddress.Trim() == "")
                {
                    ipAddress = IPAddress.Any;
                }
                else
                {
                    ipAddress = IPAddress.Parse(ServerIP);
                }
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _port);

                _socket.Bind(localEndPoint);
                _socket.Listen(_backlog);
                while (true)
                {
                    connectDone.Reset();
                    _socket.BeginAccept(new AsyncCallback(AcceptCallback), _socket);
                    connectDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                OnErrorEvent(new ErrorEventArgs(e, _socket));
            }
        }


        private void AcceptCallback(IAsyncResult ar)
        {
            Socket handler = null;
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                handler = listener.EndAccept(ar);

                string clientIP = handler.RemoteEndPoint.ToString();

                if (!_listeners.ContainsKey(clientIP))
                {
                    _listeners.Add(clientIP, handler);

                    if (OnConnect != null)
                    {
                        OnConnect(handler);
                    }
                }

                StateObj state = new StateObj(_bufferSize, handler);
                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, _bufferSize, SocketFlags.None,
                    new AsyncCallback(ServerReceiveCallback), state);
            }
            catch (Exception e)
            {
                OnErrorEvent(new ErrorEventArgs(e, handler));
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
            Abort();
        }

        protected override void OnDisconnectEvent(Socket WorkSocket)
        {
            string clientIP = WorkSocket.RemoteEndPoint.ToString();

            BaseClose(WorkSocket);

            if (_listeners.ContainsKey(clientIP))
            {
                _listeners.Remove(clientIP);

                if (OnDisconnect != null)
                {
                    OnDisconnect(WorkSocket);
                }
            }
        }

        protected override void OnErrorEvent(SinHL7.ErrorEventArgs e)
        {
            BaseClose(e.WorkSocket);
        }
        #endregion
    }

}
