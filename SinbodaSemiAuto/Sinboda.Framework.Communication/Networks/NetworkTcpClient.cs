using Sinboda.Framework.Common.CommonFunc;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Communication.DataPackages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication.Networks
{
    public class TcpData
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="bufferSize">缓存</param>
        /// <param name="WorkSocket">工作的插座</param>
        public TcpData(int bufferSize, Socket workSocket)
        {
            m_Buffer = new byte[bufferSize];
            m_WorkSocket = workSocket;
        }
        /// <summary>
        /// 缓存
        /// </summary>
        public byte[] m_Buffer = null;
        /// <summary>
        /// 工作插座
        /// </summary>
        public Socket m_WorkSocket = null;
    }

    /// <summary>
    /// Tcp客户端
    /// 采用异步回调的方式接收数据
    /// </summary>
    public class NetworkTcpClient
    {
        #region 属性
        /// <summary>
        /// 连接的服务端IP
        /// </summary>
        public IPAddress IP
        {
            get
            {
                if (null == m_Param)
                {
                    return null;
                }

                IPAddress ip;
                if (!IPAddress.TryParse(m_Param.RemoteAddress, out ip))
                {
                    return null;
                }

                return ip;
            }
        }
        /// <summary>
        /// 连接的服务端的端口号
        /// </summary>
        public int Port
        {
            get
            {
                if (null == m_Param)
                {
                    return int.MaxValue;
                }

                return m_Param.RemotePort;
            }
        }
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool m_Connected
        {
            get
            {
                if (null != m_TcpSocket && m_TcpSocket.Connected)
                {
                    return true;
                }

                return false;
            }
        }
        /// <summary>
        /// 客户端
        /// </summary>
        public Socket m_TcpSocket { get; set; }
        /// <summary>
        /// 网络连接参数
        /// </summary>
        public NetworkParameter m_Param { get; set; }
        /// <summary>
        /// 发送队列
        /// </summary>
        public ConcurrentQueue<List<byte>> m_SendQueue = new ConcurrentQueue<List<byte>>();
        /// <summary>
        /// 接收队列
        /// </summary>
        public List<byte> m_RecieveList = new List<byte>();
        /// <summary>
        /// 心跳包信息
        /// </summary>
        public PackageInfo m_KeepAlivePackege;
        private Timer KeepTimer;
        private int keepMark = 0;   // -1 第一次 0 未回复 1 已回复
        #endregion

        #region 变量
        /// <summary>
        /// 接收消息模块号
        /// </summary>
        private int m_ReceiveModule = int.MaxValue;
        /// <summary>
        /// 一整包信息（分包数据拼接）
        /// </summary>
        private List<List<byte>> m_PacakgeList = new List<List<byte>>();
        /// <summary>
        /// 一条信息共多少包
        /// </summary>
        private int m_PackageNums = 0;

        /// <summary>
        /// 连接锁
        /// </summary>
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        /// <summary>
        /// 发送锁
        /// </summary>
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        #endregion

        #region 事务
        public delegate DataPackage AnalysisEvent(ref List<byte> recieveList, string ipAddress);
        public delegate void ReceiveEvent(object sender);
        public delegate void ErrorEvent(object sender);
        public delegate bool SendDataEvent(DataPackage package);

        public event AnalysisEvent DoAnalysis;
        public event ReceiveEvent OnReceive;
        public event ErrorEvent ReceiveError;
        public event SendDataEvent SendData;
        #endregion

        public NetworkTcpClient(NetworkParameter param, PackageInfo keepAlivePkg)
        {
            m_Param = param;
            m_Param.Timeout = 100;
            m_KeepAlivePackege = keepAlivePkg;
        }

        /// <summary>
        /// 连接服务端
        /// </summary>
        /// <returns></returns>
        public bool ConnectService()
        {
            if (null == m_Param ||
                null == IP ||
                int.MaxValue == Port)
            {
                return false;
            }

            CloseClient();


            int timeOut = m_Param.Timeout;
            int connectTime = m_Param.ReconnectionTime;

            bool ret = ConnectCore(IP, Port, timeOut, connectTime);
            if (!ret)
            {
                CloseClient();
            }

            if (null != m_KeepAlivePackege)
            {
                KeepTimer = new Timer(KeepAliveThreadHandler, null, m_Param.KeepAliveInterval, m_Param.KeepAliveInterval);
                LogHelper.logCommunication.Debug("心跳线程创建并启动");
            }

            return ret;
        }

        /// <summary>
        /// 关闭客户端
        /// </summary>
        /// <returns></returns>
        public bool CloseClient()
        {
            try
            {
                if (KeepTimer != null)
                {
                    KeepTimer.Dispose();
                    KeepTimer = null;
                    LogHelper.logCommunication.Debug("终止心跳计时器");
                }

                if (null != m_TcpSocket)
                {
                    LogHelper.logCommunication.InfoFormat("主动关闭客户端{0}", m_TcpSocket.RemoteEndPoint.ToString());
                    m_TcpSocket.Shutdown(SocketShutdown.Both);
                    m_TcpSocket.Close();
                    m_TcpSocket = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">数据内容</param>
        /// <returns></returns>
        public bool WriteBuffer(byte[] buffer)
        {
            if (null == m_TcpSocket || !m_TcpSocket.Connected)
            {
                return false;
            }

            sendDone.Reset();
            m_TcpSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), m_TcpSocket);

            if (!sendDone.WaitOne(m_Param.Timeout))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取一整条数据
        /// </summary>
        /// <returns></returns>
        public DataPackage GetDataPackage()
        {
            DataPackage pak = DoAnalysis(ref m_RecieveList, IP.ToString());

            if (null != pak && null != pak.PackageInfo)
            {
                if (m_ReceiveModule != pak.PackageInfo.Module)
                {
                    if (m_PackageNums > m_PacakgeList.Count)
                    {
                        // 模块变更，前一个模块数据没有接收完毕，输出日志
                        // 正常情况下，建立连接，服务端模块号不应该改变
                    }

                    m_PacakgeList.Clear();
                    m_ReceiveModule = pak.PackageInfo.Module;
                    m_PackageNums = pak.PackageInfo.PackNums;
                }

                // 当总包数和记录包数不一致，说明指令更换
                if (m_PackageNums != pak.PackageInfo.PackNums)
                {
                    m_PacakgeList.Clear();
                    m_PackageNums = pak.PackageInfo.PackNums;
                }

                if (pak.PackageInfo.PackNums > pak.PackageInfo.PackNo + 1)
                {
                    // 分包数据
                    if (null != pak.Data)
                    {
                        m_PacakgeList.Add(pak.Data.ToList());
                    }

                    return null;
                }
                else if (pak.PackageInfo.PackNums == pak.PackageInfo.PackNo + 1)
                {
                    // 当总包数和实际记录的包信息个数不一致，说明中途发生丢包
                    if (m_PackageNums != m_PacakgeList.Count + 1)
                    {
                        // 丢包，输出日志
                        return null;
                    }

                    // 全部数据
                    if (null != pak.Data)
                    {
                        m_PacakgeList.Add(pak.Data.ToList());

                        List<byte> tmp = new List<byte>();
                        foreach (var item in m_PacakgeList)
                        {
                            tmp.AddRange(item);
                        }

                        pak.Data = tmp.ToArray();
                    }

                    m_PacakgeList.Clear();

                    return pak;
                }
                else
                {
                    // 包号超过包数，错误，输出日志
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// 异步连接服务端
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        /// <param name="reCount"></param>
        /// <returns></returns>
        private bool ConnectCore(IPAddress ip, int port, int timeout, int reCount)
        {
            try
            {
                if (reCount <= 0)
                {
                    LogHelper.logCommunication.Error($"尝试 {m_Param.ReconnectionTime} 次连接仍无法成功，通讯创建失败");
                    return false;
                }

                LogHelper.logCommunication.Info($"第 {m_Param.ReconnectionTime - reCount + 1} 次连接");

                CloseClient();

                m_TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint remoteEP = new IPEndPoint(ip, port);

                connectDone.Reset();
                m_TcpSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), m_TcpSocket);
                if (!connectDone.WaitOne(timeout, false))
                {
                    LogHelper.logCommunication.Info($"IP:{ip.ToString()};Port:{port}第 {m_Param.ReconnectionTime - reCount + 1} 次通讯，连接超时");
                    return ConnectCore(ip, port, timeout, --reCount);
                }

                if (!m_TcpSocket.Connected)
                {
                    LogHelper.logCommunication.Info($"IP:{ip.ToString()};Port:{port}第 {m_Param.ReconnectionTime - reCount + 1} 次通讯，连接失败");
                    return ConnectCore(ip, port, timeout, --reCount);
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.logCommunication.Info($"第 {m_Param.ReconnectionTime - reCount + 1} 次通讯，连接异常");
                return ConnectCore(ip, port, timeout, --reCount);
            }
        }

        /// <summary>
        /// 心跳处理
        /// </summary>
        /// <param name="param"></param>
        private void KeepAliveThreadHandler(object param)
        {
            try
            {
                if (m_Connected)
                {
                    if (keepMark >= 3)
                    {
                        LogHelper.logCommunication.Debug($"[{DateTime.Now.ToLongTimeString()}] [心跳超时] 心跳计数={keepMark}");
                        LogHelper.logCommunication.Debug("出现心跳超时，断开连接");
                        KeepTimer.Dispose();
                        ReceiveError(CommunicateError.KeepAliveTimeout);
                    }
                    else
                    {
                        LogHelper.logCommunication.Debug($"[{DateTime.Now.ToLongTimeString()}] [发送心跳前] 心跳计数={keepMark}");
                        SendKeepAlive();
                        Interlocked.Increment(ref keepMark);
                        LogHelper.logCommunication.Debug($"[{DateTime.Now.ToLongTimeString()}] [发送心跳后] 心跳计数={keepMark}");
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.logCommunication.Error("心跳处理出现：" + e.GetType().Name + "异常", e);
                ReceiveError(CommunicateError.UnknownError);
            }
        }

        /// <summary>
        /// 发送心跳指令
        /// </summary>
        /// <returns></returns>
        private bool SendKeepAlive()
        {
            DataPackage package = new DataPackage();
            package.PackageInfo = m_KeepAlivePackege;

            return SendData(package);
        }

        /// <summary>
        /// 连接回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            try
            {
                client.EndConnect(ar);
                if (client.Connected)
                {
                    TcpData receiveData = new TcpData(m_Param.ReceiveBufferSize, client);
                    client.BeginReceive(receiveData.m_Buffer, 0, m_Param.ReceiveBufferSize, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback), receiveData);
                }
            }
            catch (Exception e)
            {
                LogHelper.logCommunication.InfoFormat("连接服务端{0}，ConnectCallback 异常：{1}", IP.ToString(), e.Message);
                CloseClient();
            }
            finally
            {
                connectDone.Set();
            }
        }
        /// <summary>
        /// 接收回调函数
        /// </summary>
        /// <param name="asycResult"></param>
        private void ReceiveCallback(IAsyncResult asycResult)
        {
            Socket client = null;

            int logIndex = 0;
            int byteLen = 0;
            byte[] bufferTmp = null;
            try
            {
                lock (asycResult)
                {
                    TcpData receiveData = (TcpData)asycResult.AsyncState;
                    client = receiveData.m_WorkSocket;
                    int bytesRead = client.EndReceive(asycResult);//结束该次接收，将数据保存到state.buffer，返回缓冲区里的数据以及数据大小

                    logIndex = 1;
                    byteLen = bytesRead;

                    if (bytesRead > 0)
                    {
                        if (m_KeepAlivePackege != null)
                        {
                            Interlocked.Exchange(ref keepMark, 0);
                        }

                        logIndex = 2;
                        bufferTmp = new byte[bytesRead];
                        Array.Copy(receiveData.m_Buffer, bufferTmp, bytesRead);

                        //LogHelper.logCommunication.DebugFormat("---------------------------数据长度{0},总数据{1}", bytesRead, BinaryUtilHelper.ByteToHex(bufferTmp).ToString());

                        logIndex = 3;

                        m_RecieveList.AddRange(bufferTmp);

                        logIndex = 4;

                        //int nCount = m_RecieveList.Count;
                        for (int i = 0; i < m_RecieveList.Count;)
                        {
                            if (m_RecieveList[i] == 0x0a)
                            {
                                DataPackage package = GetDataPackage();
                                if (null != package)
                                {
                                    i = 0;
                                    //nCount = m_RecieveList.Count;

                                    //LogHelper.logCommunication.DebugFormat("---------------------------数据长度{0}", m_RecieveList.Count);


                                    package.PackageInfo.ID = m_Param.ID;
                                    OnReceive(package);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                i++;
                            }
                        }

                        logIndex = 5;
                    }

                    if (client.Connected == true)
                    {
                        logIndex = 6;
                        receiveData.m_Buffer = new byte[m_Param.ReceiveBufferSize];
                        logIndex = 7;
                        client.BeginReceive(receiveData.m_Buffer, 0, m_Param.ReceiveBufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), receiveData);
                        logIndex = 8;
                    }
                }
            }
            catch (SocketException ex)
            {
                LogHelper.logCommunication.InfoFormat("接收{0}数据 Socket异常：{1}", IP.ToString(), ex.Message);
            }
            catch (Exception e)
            {
                LogHelper.logCommunication.InfoFormat("接收{0}数据 ReceiveCallback 异常：{1},执行至第{2}", IP.ToString(), e.Message, logIndex);

                if (null != bufferTmp && bufferTmp.Length > 0)
                {
                    LogHelper.logCommunication.InfoFormat("接收{0}数据 ReceiveCallback 异常 接收数据：{1},总数据{2}", IP.ToString(), byteLen, BinaryUtilHelper.ByteToHex(bufferTmp).ToString());
                }
            }
        }
        /// <summary>
        /// 发送回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            try
            {
                int bytesSent = client.EndSend(ar);
            }
            catch (Exception e)
            {
                LogHelper.logCommunication.InfoFormat("向{0}发送 数据SendCallback异常：{1}", IP.ToString(), e.Message);
            }
            finally
            {
                sendDone.Set();
            }
        }
    }
}
