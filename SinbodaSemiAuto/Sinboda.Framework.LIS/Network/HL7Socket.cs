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
using Sinboda.Framework.LIS.Common;

namespace Sinboda.Framework.LIS.SinHL7
{
    /// <summary>
    /// 提供在Net TCP_IP 协议上基于HL7消息的客户端、服务端基类 
    /// </summary>
    public abstract class HL7Socket : System.ComponentModel.Component
    {
        #region 变量
        protected int _bufferSize = 2048;
        //protected string _ipAddress = "127.0.0.1";
        //protected int _port = 1024;
        public string _ipAddress
        { get; set; }
        public int _port
        { get; set; }
        protected Socket _socket = null;
        /// <summary>
        /// 连接超时
        /// </summary>
        protected int _timeout = 100;
        /// <summary>
        /// 是否开启 keepalive ,默认开启
        /// </summary>
        protected uint keepAliveFlag = 1;
        /// <summary>
        /// 开始第一次 send 心跳包,单位毫秒，默认6秒
        /// </summary>
        protected int keepAliveTime = 6000;
        /// <summary>
        /// 每隔多长时间send 一个心跳包，单位毫秒，默认10秒
        /// </summary>
        protected int keepAliveInterval = 10000;

        protected ManualResetEvent connectDone = new ManualResetEvent(false);
        ManualResetEvent sendDone = new ManualResetEvent(false);

        /// <summary>
        /// 自动重连信号
        /// </summary>
        protected ManualResetEvent autoResetEvent;
        #endregion

        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        public HL7Socket()
        {
        }
        #endregion

        #region 方法
        /// <summary>
        /// 设置keep alive
        /// </summary>
        /// <param name="socket"></param>
        protected void SetIOControl(Socket socket)
        {
            if (keepAliveFlag == 1)
            {
                uint dummy = 0;
                byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
                BitConverter.GetBytes((uint)keepAliveFlag).CopyTo(inOptionValues, 0);//是否开启 keepalive 
                BitConverter.GetBytes((uint)keepAliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));//多长时间( ms )没有数据就开始第一次 send 心跳包 
                BitConverter.GetBytes((uint)keepAliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2); //每隔多长时间(ms) send 一个心跳包
                //发5次 (2000 XP 2003 默认，可以修改注册表 ), 10 次 (Vista 后系统默认,不允许修改) 
                socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
            }
        }

        byte[] receive = new byte[0];
        private void mergeArray(byte[] buffer, int offset, int count)
        {
            List<byte> bb = receive.ToList();
            for (int i = offset; i < count; i++)
            {
                bb.Add(buffer[i]);
            }
            receive = bb.ToArray();
        }
        private int FindFinishIndex(byte[] array)
        {
            try
            {
                //0x1c0x0D是包尾，先查找0x1c
                int idx = Array.IndexOf(array, (byte)0x1c);
                while (idx != -1)
                {
                    //查找0x0D
                    //单字节
                    if (array[idx + 1] == 0x0d)
                    {
                        return idx;
                    }
                    //双字节 00 1c 00 0d 
                    else if (array[idx + 2] == 0x0d)
                    {
                        if (array[idx - 1] == 0x00 && array[idx + 1] == 0x00)
                            return idx;
                    }
                    idx = Array.IndexOf(array, (byte)0x1c, idx + 1);
                }
                return idx;
            }
            catch (Exception ex)
            {
                LogHelper.logLisComm.Info("【LIS底层】ReceiveCallback 查找包尾异常：" + ex.Message);
                return -1;
            }
        }

        protected void ReceiveCallback(IAsyncResult ar)
        {
            Socket client = null;
            try
            {
                lock (ar)
                {
                    StateObj state = (StateObj)ar.AsyncState;
                    client = state.workSocket;
                    int bytesRead = client.EndReceive(ar);//结束该次接收，将数据保存到state.buffer，返回缓冲区里的数据以及数据大小
                    LogHelper.logLisComm.Info("【LIS底层】ReceiveCallback bytesRead字节数=" + bytesRead);
                    if (bytesRead > 0)
                    {
                        //state.Datastream.Write(state.buffer, 0, bytesRead);暂时不用Datastream，所以注释掉
                        mergeArray(state.buffer, 0, bytesRead);
                        //0x1c是包尾
                        int idx = FindFinishIndex(receive);
                        while (idx != -1)
                        {
                            //查找包头0x0b
                            int bdx = Array.IndexOf(receive, (byte)0x0b);
                            //没找到包头，数据有问题
                            if (bdx == -1)
                            {
                                LogHelper.logLisComm.Info("【LIS底层】ReceiveCallback 缺少包头,数据丢弃");
                                receive = new byte[0];
                                break;
                            }
                            byte[] newdata = new byte[idx - bdx];// 0x1c 0x0D <EB><CR>
                            Array.Copy(receive, bdx, newdata, 0, newdata.Length);
                            Stream temp = Common.Convert.BytesToStream(newdata);
                            Stream sm = _encoding != Encoding.Default ?
                                                     Common.Convert.BytesToStream(Common.Convert.ToEncoding(_encoding, Encoding.Default, Common.Convert.StreamToBytes(temp))) :
                                                     temp;
                            SinHL7.HL7Message hl7 = new HL7Message();
                            hl7.HL7.CreateData = sm;
                            LogHelper.logLisComm.Info("【LIS底层】接收到LIS传来的原始数据:" + hl7.HL7.ToString());
                            OnInceptEvent(new InceptEventArgs(hl7, sm, client));
                            //拆包后，继续查找剩下的数据，如果没找到包尾，将剩下的数据缓存，等待下一包数据，再将数据组包
                            int remainLength = receive.Length - idx - 1;//双字节
                            LogHelper.logLisComm.Info("【LIS底层】ReceiveCallback RemainLength=" + remainLength);
                            newdata = new byte[remainLength];
                            Array.Copy(receive, idx + 1, newdata, 0, remainLength);
                            receive = newdata;
                            idx = Array.IndexOf(receive, (byte)0x1c);
                        }
                        if (client.Connected == true)
                        {
                            state.buffer = new byte[_bufferSize];
                            client.BeginReceive(state.buffer, 0, _bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                        }
                    }
                    else
                    {
                        LogHelper.logLisComm.Info("【LIS底层】ReceiveCallback 关闭client, bytesRead字节数=" + bytesRead);
                        receive = new byte[0];
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                        OnDisconnectEvent(client);
                    }
                }
            }
            catch (SocketException ex)
            {
                receive = new byte[0];
                BaseClose(client);
                LogHelper.logLisComm.Info("【LIS底层】ReceiveCallback Socket异常：" + ex.Message);
                OnErrorEvent(new ErrorEventArgs(ex, client, ex.ErrorCode.ToString()));
            }
            catch (Exception e)
            {
                receive = new byte[0];
                BaseClose(client);
                LogHelper.logLisComm.Info("【LIS底层】ReceiveCallback 异常：" + e.Message);
                OnErrorEvent(new ErrorEventArgs(e, client));
            }
        }
        /// <summary>
        /// 服务器接收回调函数
        /// </summary>
        /// <param name="ar"></param>
        protected void ServerReceiveCallback(IAsyncResult ar)
        {
            Socket client = null;
            try
            {
                lock (ar)
                {
                    StateObj state = (StateObj)ar.AsyncState;
                    client = state.workSocket;
                    int bytesRead = client.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        state.Datastream.Write(state.buffer, 0, bytesRead);
                        int idx = Array.IndexOf(state.buffer, (byte)0x1c);
                        if (idx != -1)
                        {
                            Stream sm = _encoding != Encoding.Default ?
                                                     Common.Convert.BytesToStream(Common.Convert.ToEncoding(_encoding, Encoding.Default, Common.Convert.StreamToBytes(state.Datastream))) :
                                                     state.Datastream;

                            SinHL7.HL7Message hl7 = new HL7Message();
                            hl7.HL7.CreateData = sm;
                            LogHelper.logLisComm.Info("【Service】 ServerReceiveCallback 接收到LIS传来的原始数据:" + hl7.HL7.ToString());
                            OnInceptEvent(new InceptEventArgs(hl7, sm, client));
                            state.Datastream.SetLength(0);
                            state.Datastream.Position = 0;
                        }
                        if (client.Connected == true)
                        {
                            state.buffer = new byte[_bufferSize];
                            client.BeginReceive(state.buffer, 0, _bufferSize, SocketFlags.None, new AsyncCallback(ServerReceiveCallback), state);
                        }
                    }
                    else
                    {
                        LogHelper.logLisComm.Info("【Service】ServerReceiveCallback 关闭client, bytesRead字节数=" + bytesRead);
                        OnDisconnectEvent(client);
                    }
                }
            }
            catch (SocketException ex)
            {
                LogHelper.logLisComm.Info("【Service】ServerReceiveCallback Socket异常：" + ex.Message);
                OnErrorEvent(new ErrorEventArgs(ex, client, ex.ErrorCode.ToString()));
            }
            catch (Exception e)
            {
                LogHelper.logLisComm.Info("【Service】ServerReceiveCallback 异常：" + e.Message);
                OnErrorEvent(new ErrorEventArgs(e, client));
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        protected virtual void BaseClose(Socket clientSocket)
        {
            try
            {
                if (clientSocket != null)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    //_socket.Disconnect(true);
                    clientSocket.Close();
                }
            }
            catch (Exception e)
            {
                LogHelper.logLisComm.Info("【LIS底层】BaseClose Exception：" + e.Message);
            }
        }
        /// <summary>
        /// 发送一个流数据
        /// </summary>
        /// <param name="Astream">数据流</param>
        protected virtual void Send(Socket ClientSocket, Stream Astream)
        {
            try
            {
                Astream.Position = 0;
                int packetCount = (int)Math.Ceiling((decimal)Astream.Length / _bufferSize);

                //最后一个包的大小
                int lastPacketData = (int)(Astream.Length - (_bufferSize * (packetCount - 1)));

                for (int i = 0; i < packetCount; i++)
                {
                    byte[] byteData = i == packetCount - 1 ? new byte[lastPacketData] : new byte[_bufferSize];

                    Astream.Read(byteData, 0, byteData.Length);
                    byte[] data = _encoding != Encoding.Default ? Common.Convert.ToEncoding(Encoding.Default, _encoding, byteData) : byteData;

                    sendDone.Reset();
                    ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), ClientSocket);
                    sendDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                BaseClose(ClientSocket);
                LogHelper.logLisComm.Info("【LIS底层】发送 Exception：" + e.Message);
                OnErrorEvent(new ErrorEventArgs(e, ClientSocket));
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            try
            {
                int bytesSent = client.EndSend(ar);
            }
            catch (Exception e)
            {
                BaseClose(client);
                LogHelper.logLisComm.Info("【LIS底层】发送 SendCallback异常：" + e.Message);
                OnErrorEvent(new ErrorEventArgs(e, client));
            }
            finally
            {
                sendDone.Set();
            }
        }


        /// <summary>
        /// 发送HL7Message流数据
        /// </summary>
        protected virtual void SendHL7(Socket ClientSocket)
        {
            Stream stream = null;

            if (_sendHL7Source != null)
            {
                LogHelper.logLisComm.Info("【LIS底层】发送到LIS的原始数据:" + _sendHL7Source.HL7.ToString());
                stream = _sendHL7Source.HL7.ToStream();
            }

            this.Send(ClientSocket, stream);
        }


        /// <summary>
        /// 引发接收数据事件
        /// </summary>
        /// <param name="e">接收数据</param>
        protected virtual void OnInceptEvent(InceptEventArgs e)
        {
            if (OnAccept != null)
            {
                OnAccept(this, e);
            }
        }

        /// <summary>
        /// 引发错误事件
        /// </summary>
        /// <param name="e">错误数据</param>
        protected virtual void OnErrorEvent(SinHL7.ErrorEventArgs e)
        {
            autoResetEvent.Set();
            if (OnError != null)
            {
                OnError(this, e);
            }
        }

        /// <summary>
        /// 接收到数据引发的事件
        /// </summary>
        public event InceptEvent OnAccept;

        /// <summary>
        /// 发生错误引发的事件
        /// </summary>
        public event ErrorEvent OnError;

        /// <summary>
        /// 断开连接事件
        /// </summary>
        /// <param name="WorkSocket"></param>
        protected virtual void OnDisconnectEvent(Socket WorkSocket) { }

        #endregion

        #region 属性
        HL7Message _sendHL7Source = null;
        /// <summary>
        /// 接收HL7数据包后存储源
        /// </summary>
        public HL7Message SendHL7Source
        {
            get
            {
                return _sendHL7Source;
            }
            set
            {
                _sendHL7Source = value;
            }
        }



        Encoding _encoding = Encoding.Default;
        /// <summary>
        /// 字符编码
        /// </summary>
        public Encoding Encoding
        {
            set
            {
                _encoding = value;
            }
            get
            {
                return _encoding;
            }
        }

        /// <summary>
        /// 连接的活动状态
        /// </summary>
        [Browsable(false)]
        public bool Active
        {
            get
            {
                if (_socket == null)
                {
                    return false;
                }

                return _socket.Connected;
            }
        }

        /// <summary>
        /// 要连接的服务器所使用的端口
        /// </summary>
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }

        /// <summary>
        /// 缓冲器大小
        /// </summary>
        public int BufferSize
        {
            get
            {
                return _bufferSize;
            }
            set
            {
                _bufferSize = value;
            }
        }
        #endregion
    }
}
