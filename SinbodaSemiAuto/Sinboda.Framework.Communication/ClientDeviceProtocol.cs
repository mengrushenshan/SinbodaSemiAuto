using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Communication.DataPackages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Sinboda.Framework.Communication.Utils.WriteLogs;

namespace Sinboda.Framework.Communication
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ClientDeviceProtocol : IDeviceProtocol
    {
        private readonly object obj = new object();

        /// <summary>
        /// 通讯参数
        /// </summary>
        protected ProtocolParameter _parameter = null;
        /// <summary>
        /// 封包处理
        /// </summary>
        protected IPackageHandle _packageHandle = null;
        /// <summary>
        /// 发送线程
        /// </summary>
        protected Thread _sendThread = null;
        /// <summary>
        /// 接收线程
        /// </summary>
        protected Thread _receiveThread = null;
        /// <summary>
        /// 发送信号
        /// </summary>
        protected AutoResetEvent _sendEvent = new AutoResetEvent(false);
        /// <summary>
        /// 发送队列
        /// </summary>
        protected ConcurrentQueue<byte[]> _sendQueue = new ConcurrentQueue<byte[]>();
        /// <summary>
        /// 接收集合
        /// </summary>
        protected List<byte> _recieveList = new List<byte>();
        /// <summary>
        /// 创建通讯参数
        /// </summary>
        /// <returns></returns>
        protected abstract ProtocolParameter CreateParameterInstance();
        /// <summary>
        /// 参数信息
        /// </summary>
        public ProtocolParameter Parameter
        {
            get
            {
                if (null == _parameter)
                    _parameter = CreateParameterInstance();
                return _parameter;
            }
            set { _parameter = value; }
        }
        /// <summary>
        /// 创建不同通讯模式对应的封包处理
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public void PackageHandler(CommunicateType type)
        {
            switch (type)
            {
                case CommunicateType.Network:
                    _packageHandle = new NetworkPackageHeadHandle();
                    LogHelper.logCommunication.Debug("创建网口解析方式");
                    break;
                case CommunicateType.SerialPort:
                    _packageHandle = new SerialPortPackageHeadHandle();
                    LogHelper.logCommunication.Debug("创建串口解析方式");
                    break;
                default:
                    _packageHandle = null;
                    break;
            }
        }
        /// <summary>
        /// 心跳包信息
        /// </summary>
        private PackageInfo _keepAlivePackageInfo;
        /// <summary>
        /// 心跳包信息
        /// </summary>
        public PackageInfo KeepAlivePackegeInfo
        {
            get { return _keepAlivePackageInfo; }
            set { _keepAlivePackageInfo = value; }
        }
        /// <summary>
        /// 设备通讯方式
        /// </summary>
        public CommunicateType CommunicateType
        {
            get { return GetCommunicateType(); }
        }
        /// <summary>
        /// 获取通讯方式
        /// </summary>
        /// <returns></returns>
        protected abstract CommunicateType GetCommunicateType();
        /// <summary>
        /// 联机
        /// </summary>
        public abstract bool Connect();
        /// <summary>
        /// 脱机
        /// </summary>
        public abstract void Disconnect();

        private void DisconnectCore()
        {
            modulePakCache.Clear();
            Disconnect();
        }

        /// <summary>
        /// 判断是否联机
        /// </summary>
        public bool Connected
        {
            get { return GetConnected(); }
        }
        /// <summary>
        /// 是否联机
        /// </summary>
        /// <returns></returns>
        protected abstract bool GetConnected();

        private Timer KeepTimer;
        private int keepMark = 0;   // -1 第一次 0 未回复 1 已回复
        /// <summary>
        /// 开始线程
        /// </summary>
        protected void StartThreads()
        {
            LogHelper.logCommunication.Info("******************************** StartThreads ********************************");

            _sendThread = new Thread(new ParameterizedThreadStart(SendThreadHandler));
            _sendThread.IsBackground = true;
            _sendThread.Start(this);
            LogHelper.logCommunication.Debug("发送线程创建并启动");
            _receiveThread = new Thread(new ParameterizedThreadStart(ReceiveThreadHandler));
            _receiveThread.IsBackground = true;
            _receiveThread.Start(this);
            LogHelper.logCommunication.Debug("接收线程创建并启动");
            if (_parameter.KeepAliveInterval > 0 && KeepAlivePackegeInfo != null)
            {
                KeepTimer = new Timer(KeepAliveThreadHandler, null, _parameter.KeepAliveInterval, _parameter.KeepAliveInterval);
                LogHelper.logCommunication.Debug("心跳线程创建并启动");
            }
        }
        /// <summary>
        /// 结束线程
        /// </summary>
        protected void StopThreads()
        {
            lock (obj)
            {
                _sendEvent.Reset();
                try
                {
                    if (_sendThread != null)
                    {
                        _sendThread.Abort();
                        _sendThread = null;
                        LogHelper.logCommunication.Debug("中止发送线程");
                    }
                    if (_receiveThread != null)
                    {
                        _receiveThread.Abort();
                        _receiveThread = null;
                        LogHelper.logCommunication.Debug("中止接收线程");
                    }
                    if (KeepTimer != null)
                    {
                        KeepTimer.Dispose();
                        KeepTimer = null;
                        LogHelper.logCommunication.Debug("终止心跳计时器");
                    }
                }
                catch (ThreadAbortException ex)
                {
                    //LogHelper.logCommunication.Error("Stop Thread", ex);
                }
            }

            LogHelper.logCommunication.Info("******************************** StopThreads ********************************");
        }

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="package">数据包</param>
        /// <returns>发送是否成功</returns>
        public bool SendPackage(DataPackage package)
        {
            if (CommunicateType == CommunicateType.Network)
            {
                byte[] orinalData = package.Data;
                int loop = 1;
                if (package.Data != null)
                {
                    if (package.Data.Length > 1024)
                    {
                        loop = (int)Math.Ceiling((double)package.Data.Length / (double)1024);
                    }
                    for (int i = 0; i < loop; i++)
                    {
                        package.PackageInfo.PackNums = loop;
                        package.PackageInfo.PackNo = i;
                        byte[] dataTmp = null;
                        if (i != loop - 1)
                        {
                            dataTmp = new byte[1024];
                            Array.Copy(orinalData, i * 1024, dataTmp, 0, 1024);
                        }
                        else
                        {
                            dataTmp = new byte[orinalData.Length - i * 1024];
                            Array.Copy(orinalData, i * 1024, dataTmp, 0, orinalData.Length - i * 1024);
                        }
                        package.Data = dataTmp;
                        byte[] data = null;
                        if (_packageHandle.SetPackage(package, ref data))
                        {
                            if (!PostSend(data, 0, data.Length))
                            {
                                return false;
                            }
                            if (i == loop - 1)
                                return true;
                            else continue;
                        }
                    }
                    return false;
                }
                else
                {
                    package.PackageInfo.PackNums = 1;
                    package.PackageInfo.PackNo = 0;
                    byte[] data = null;
                    if (_packageHandle.SetPackage(package, ref data))
                    {
                        if (!PostSend(data, 0, data.Length))
                        {
                            return false;
                        }
                        return true;
                    }
                    return false;
                }
            }
            else
            {
                byte[] data = null;
                if (_packageHandle.SetPackage(package, ref data))
                {
                    if (!PostSend(data, 0, data.Length))
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 将发送数据添加到发送队列
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected bool PostSend(byte[] buffer, int offset, int count)
        {
            _sendQueue.Enqueue(buffer);
            _sendEvent.Set();
            return true;
        }
        /// <summary>
        /// 发送线程处理方法
        /// </summary>
        /// <param name="param"></param>
        protected void SendThreadHandler(object param)
        {
            while (Connected)
            {
                try
                {
                    _sendEvent.WaitOne();
                    if (!DoSendWait())
                    {
                        if (Connected)
                        {
                            LogHelper.logCommunication.Debug("出现发送错误");
                            OnCommunicateError(CommunicateError.SendFailed, new Exception("Sending data failed."));
                            DisconnectCore();
                        }
                        break;
                    }
                }
                catch (ThreadAbortException abortexception)
                {
                    DisconnectCore();
                }
                catch (SocketException socketexception)
                {
                    LogHelper.logCommunication.Error("SendThread SocketException error", socketexception);
                    var errorCode = socketexception.ErrorCode;
                    var socketErrorCode = socketexception.SocketErrorCode;
                    LogHelper.logCommunication.Error("socket异常：ErrorCode " + errorCode + "，socketErrorCode " + socketErrorCode);
                    OnCommunicateError(CommunicateError.CommunicationException, socketexception);
                    DisconnectCore();
                }
                catch (Exception e)
                {
                    LogHelper.logCommunication.Error("发送处理出现：" + e.GetType().Name + "异常", e);
                    if (e.InnerException != null && e.InnerException.GetType() == typeof(SocketException))
                    {
                        var errorCode = (e.InnerException as SocketException).ErrorCode;
                        var socketErrorCode = (e.InnerException as SocketException).SocketErrorCode;
                        LogHelper.logCommunication.Error("内部异常为socket异常：ErrorCode " + errorCode + "，socketErrorCode " + socketErrorCode);
                    }
                    OnCommunicateError(CommunicateError.UnknownError, e);
                }
            }
        }
        /// <summary>
        /// 发送队列数据进行封包发送
        /// </summary>
        /// <returns></returns>
        protected bool DoSendWait()
        {
            byte[] data;
            while ((Connected) && _sendQueue.TryDequeue(out data))
            {
                if (!WriteBuffer(data, 0, data.Length))
                {
                    LogHelper.logCommunication.Debug("发送数据失败");
                    return false;
                }
                else
                {
                    WriteLog(data, data.Length, communacationWay.SEND, GetCommunicateType());
                }
            }
            return true;
        }

        /// <summary>
        /// 接收线程处理方法
        /// </summary>
        /// <param name="param"></param>
        protected void ReceiveThreadHandler(object param)
        {
            while (Connected)
            {
                try
                {
                    if (!DoReceiveWait())
                    {
                        if (Connected)
                        {
                            LogHelper.logCommunication.Debug("出现接收错误");
                            OnCommunicateError(CommunicateError.ReceiveFailed, new Exception("Receiving data failed."));
                            DisconnectCore();
                        }
                        break;
                    }
                }
                catch (ThreadAbortException abortexcepion)
                {
                    DisconnectCore();
                }
                catch (SocketException socketexception)
                {
                    LogHelper.logCommunication.Error("ReceiveThread SocketException error", socketexception);
                    var errorCode = socketexception.ErrorCode;
                    var socketErrorCode = socketexception.SocketErrorCode;
                    LogHelper.logCommunication.Error("socket异常：ErrorCode " + errorCode + "，socketErrorCode " + socketErrorCode);
                    OnCommunicateError(CommunicateError.CommunicationException, socketexception);
                    DisconnectCore();
                }
                catch (Exception e)
                {
                    LogHelper.logCommunication.Error("接收处理出现：" + e.GetType().Name + "异常", e);
                    if (e.InnerException != null && e.InnerException.GetType() == typeof(SocketException))
                    {
                        var errorCode = (e.InnerException as SocketException).ErrorCode;
                        var socketErrorCode = (e.InnerException as SocketException).SocketErrorCode;
                        LogHelper.logCommunication.Error("内部异常为socket异常：ErrorCode " + errorCode + "，socketErrorCode " + socketErrorCode);
                    }
                    OnCommunicateError(CommunicateError.UnknownError, e);
                }
            }
        }
        /// <summary>
        /// 循环读取数据，读取成功后返回
        /// </summary>
        /// <returns></returns>
        protected bool DoReceiveWait()
        {
            byte[] data = new byte[Parameter.ReceiveBufferSize];
            int dataLen = ReadBuffer(data, 0, data.Length);
            if (dataLen < 0)
                return false;
            byte[] bufferTmp = new byte[dataLen];
            Array.Copy(data, bufferTmp, dataLen);

            if (dataLen > 0)
            {
                LogHelper.logCommunication.Debug($"--------read begin---------------");
                WriteDebugLog("网络底层数据 ", bufferTmp.ToArray());
                LogHelper.logCommunication.Debug($"---------read end--------------");
            }

            _recieveList.AddRange(bufferTmp);
            DoDataReceived(data, dataLen);
            return true;
        }

        ConcurrentDictionary<int, List<byte>> modulePakCache = new ConcurrentDictionary<int, List<byte>>();
        /// <summary>
        /// 数据接收处理
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataLen"></param>
        protected void DoDataReceived(byte[] data, int dataLen)
        {
            if (CommunicateType == CommunicateType.Network)
            {
                DataPackage pak = null;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (_packageHandle.GetPackage(_recieveList, ref pak, GetCommunicateType()))
                {
                    if (KeepAlivePackegeInfo != null)
                    {
                        // LogHelper.logCommunication.Debug($"[{DateTime.Now.ToLongTimeString()}] [接收心跳前] 心跳计数={keepMark}");
                        // LogHelper.logCommunication.Debug($"收到心跳包处理");
                        Interlocked.Exchange(ref keepMark, 0);
                        // LogHelper.logCommunication.Debug($"[{DateTime.Now.ToLongTimeString()}] [接收心跳后] 心跳计数={keepMark}");
                    }

                    List<byte> ls;
                    if (!modulePakCache.TryGetValue(pak.PackageInfo.Module, out ls))
                    {
                        modulePakCache[pak.PackageInfo.Module] = ls = new List<byte>();
                        LogHelper.logCommunication.Debug($"创建{pak.PackageInfo.Module}模块缓存");
                    }

                    if (pak.PackageInfo.PackNums != pak.PackageInfo.PackNo + 1)
                    {
                        if (pak.Data != null)
                            ls.AddRange(pak.Data);

                        LogHelper.logCommunication.Debug($"组合{pak.PackageInfo.Module}模块数据包，总{pak.PackageInfo.PackNums},当前{pak.PackageInfo.PackNo + 1}");
                        continue;
                    }
                    else
                    {
                        if (pak.Data != null)
                        {
                            ls.AddRange(pak.Data);
                            pak.Data = ls.ToArray();
                        }
                        ls.Clear();
                        LogHelper.logCommunication.Debug($"清空{pak.PackageInfo.Module}模块缓存");
                    }
                    OnDataReceived(pak);
                }
                sw.Stop();
                if (sw.ElapsedMilliseconds != 0)
                    LogHelper.logCommunication.Debug("当前触发一次解包处理时间为：" + sw.ElapsedMilliseconds);
            }
            else
            {
                DataPackage pak = null;
                while (_packageHandle.GetPackage(_recieveList, ref pak, GetCommunicateType()))
                {
                    if (KeepAlivePackegeInfo != null && pak != null)
                    {
                        if (pak.PackageInfo.Command == KeepAlivePackegeInfo.Command)
                        {
                            LogHelper.logCommunication.Debug($"[{DateTime.Now.ToLongTimeString()}] [接收心跳前] 心跳计数={keepMark}");
                            LogHelper.logCommunication.Debug($"收到心跳包处理");
                            Interlocked.Exchange(ref keepMark, 0);
                            LogHelper.logCommunication.Debug($"[{DateTime.Now.ToLongTimeString()}] [接收心跳后] 心跳计数={keepMark}");
                        }
                    }
                    OnDataReceived(pak);
                }
            }
        }
        /// <summary>
        /// 接收数据处理
        /// </summary>
        /// <param name="package"></param>
        protected void OnDataReceived(DataPackage package)
        {
            if (DataReceivedEvent != null)
            {
                ReceivedEventArgs e = new ReceivedEventArgs(package);
                LogHelper.logCommunication.Debug($"发送数据给上位机软件开始");
                DataReceivedEvent(this, e);
                LogHelper.logCommunication.Debug($"发送数据给上位机软件结束");
            }
        }
        /// <summary>
        /// 接收数据事件
        /// </summary>
        public event ClientDataReceivedEventHandler DataReceivedEvent;
        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="error"></param>
        /// <param name="e"></param>
        protected void OnCommunicateError(CommunicateError error, Exception e)
        {
            if (CommunicateErrorEvent != null)
            {
                CommunicateErrorEventArgs args = new CommunicateErrorEventArgs(error, e);
                CommunicateErrorEvent(this, args);
            }
        }
        /// <summary>
        /// 错误事件
        /// </summary>
        public event CommunicateErrorEventHandler CommunicateErrorEvent;

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected abstract bool WriteBuffer(byte[] buffer, int offset, int count);
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected abstract int ReadBuffer(byte[] buffer, int offset, int count);

        /// <summary>
        /// 心跳重试次数
        /// </summary>
        public int RetryTimes = 0;
        /// <summary>
        /// 心跳处理
        /// </summary>
        /// <param name="param"></param>
        protected void KeepAliveThreadHandler(object param)
        {
            try
            {
                if (Connected)
                {
                    if (keepMark >= 3)
                    {
                        LogHelper.logCommunication.Debug($"[{DateTime.Now.ToLongTimeString()}] [心跳超时] 心跳计数={keepMark}");
                        LogHelper.logCommunication.Debug("出现心跳超时，断开连接");
                        KeepTimer.Dispose();
                        OnCommunicateError(CommunicateError.KeepAliveTimeout, null);
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
                OnCommunicateError(CommunicateError.UnknownError, e);
            }
        }
        /// <summary>
        /// 是否心跳正常
        /// </summary>
        /// <returns></returns>
        protected bool SendKeepAlive()
        {
            DataPackage package = new DataPackage();
            package.PackageInfo = KeepAlivePackegeInfo;
            SendPackage(package);
            return true;
        }
    }
}
