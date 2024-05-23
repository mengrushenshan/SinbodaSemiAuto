using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Communication.DataPackages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication.SerialPorts
{
    public class SerialPortClient
    {
        #region 属性
        /// <summary>
        /// 串口
        /// </summary>
        private SerialPort m_SerialPort;
        /// <summary>
        /// 参数
        /// </summary>
        private SerialPortParameter m_Param;
        /// <summary>
        /// 心跳包信息
        /// </summary>
        public PackageInfo m_KeepAlivePackege;
        /// <summary>
        /// 发送队列
        /// </summary>
        public ConcurrentQueue<List<byte>> m_SendQueue = new ConcurrentQueue<List<byte>>();
        /// <summary>
        /// 接收队列
        /// </summary>
        public ConcurrentQueue<byte> m_RecieveQueue = new ConcurrentQueue<byte>();
        /// <summary>
        /// 串口通讯id密
        /// </summary>
        private AutoEventThread m_SecretParam;
        /// <summary>
        /// 心跳线程
        /// </summary>
        protected Thread m_KeepAliveThread = null;
        /// <summary>
        /// 心跳信号
        /// </summary>
        protected AutoResetEvent m_KeepAliveEvent = new AutoResetEvent(false);
        #endregion

        #region
        public delegate void ReceiveEvent(object sender);
        public delegate void ErrorEvent(object sender);
        public delegate bool SendDataEvent(DataPackage package);

        public event ReceiveEvent OnReceive;
        public event ErrorEvent ReceiveError;
        public event SendDataEvent SendData;
        #endregion

        #region 对外接口
        public SerialPortClient(SerialPortParameter serialParam, PackageInfo keepAlivePkg)
        {
            m_Param = serialParam;

            m_SerialPort = new SerialPort();
            m_SerialPort.ReadBufferSize = m_Param.ReceiveBufferSize;
            m_SerialPort.WriteBufferSize = m_Param.SendBufferSize;
            m_SerialPort.PortName = m_Param.PortName;
            m_SerialPort.BaudRate = m_Param.BaudRate;
            m_SerialPort.DataBits = m_Param.DataBits;
            m_SerialPort.Parity = m_Param.Parity;
            m_SerialPort.StopBits = m_Param.StopBits;

            m_KeepAlivePackege = keepAlivePkg;
        }

        /// <summary>
        /// 获取串口名称
        /// </summary>
        /// <returns></returns>
        public string GetPortName()
        {
            if (null == m_Param || null == m_SerialPort)
            {
                return string.Empty;
            }

            return m_Param.PortName;
        }

        /// <summary>
        /// 获取串口Id
        /// </summary>
        /// <returns></returns>
        public string GetPortId()
        {
            if (null == m_Param || null == m_SerialPort)
            {
                return string.Empty;
            }

            return m_Param.ID;
        }

        /// <summary>
        /// 串口是否打开
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            if (null == m_SerialPort)
            {
                return false;
            }

            return m_SerialPort.IsOpen;
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                if (null == m_SerialPort)
                {
                    return false;
                }

                Disconnect();

                m_SerialPort.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceivedHandler);
                m_SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                m_SerialPort.Open();

                if (null != m_KeepAlivePackege)
                {
                    m_KeepAliveThread = new Thread(new ParameterizedThreadStart(KeepAliveThreadHandler));
                    m_KeepAliveThread.IsBackground = true;
                    m_KeepAliveThread.Start(this);
                    LogHelper.logCommunication.Debug("心跳线程创建并启动");
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (null != m_SerialPort && m_SerialPort.IsOpen)
                {
                    m_SerialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(ErrorReceivedHandler);
                    m_SerialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);

                    m_SerialPort.Close();

                    m_KeepAliveEvent.Reset();
                    if (m_KeepAliveThread != null)
                    {
                        m_KeepAliveThread.Abort();
                        m_KeepAliveThread = null;
                        LogHelper.logCommunication.Debug("中止心跳线程");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.logCommunication.Error("SerialPort Disconnect error", ex);
            }
        }

        /// <summary>
        /// 串口写入数据
        /// </summary>
        public bool WriteBuffer(byte[] buffer, int offset, int count)
        {
            try
            {
                if (null != m_SerialPort && m_SerialPort.IsOpen)
                {
                    m_SerialPort.Write(buffer, offset, count);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.logCommunication.Error("SerialPort WriteBuffer error", ex);
                return false;
            }
        }

        public void SetSecretParam(ref AutoEventThread secretParam)
        {
            m_SecretParam = secretParam;
        }

        public void DoShakeHandSecret()
        {
            if (null == m_SecretParam)
            {
                return;
            }

            m_SecretParam.MyThread.Start();
        }

        public bool SecretSuccess()
        {
            if (null == m_SecretParam)
            {
                return false;
            }

            return m_SecretParam.ExcuteIsSucceed;
        }

        public byte[] GetStandardKey()
        {
            if (null == m_Param)
            {
                return new byte[8];
            }

            return m_Param.StandardKey;
        }

        public bool IsUsingNewKey()
        {
            if (null == m_SecretParam)
            {
                return false;
            }

            if (m_SecretParam.NewKeyInfo[4] != 0 &&
                m_SecretParam.NewKeyInfo[5] != 0 &&
                m_SecretParam.NewKeyInfo[6] != 0 &&
                m_SecretParam.NewKeyInfo[7] != 0)
            {
                return true;
            }

            return false;
        }

        public byte[] GetOrinalKey()
        {
            if (null == m_SecretParam)
            {
                return new byte[8];
            }

            return m_SecretParam.OrignalKeyInfo;
        }

        public byte[] GetRealKey()
        {
            if (null == m_SecretParam)
            {
                return new byte[8];
            }

            return m_SecretParam.NewKeyInfo;
        }

        public ushort GetShakeHandCommand()
        {
            if (null == m_SecretParam)
            {
                return 0;
            }

            return m_SecretParam.ShankeHandCommnad;
        }

        public ushort GetCommunicationEnsureCommand()
        {
            if (null == m_SecretParam)
            {
                return 0;
            }

            return m_SecretParam.CommunicationEnsureCommand;
        }

        public bool SetInfoForCom(byte[] inputInfo)
        {
            if (null == m_SecretParam)
            {
                return false;
            }

            m_SecretParam.AnalyzerReturnInfo = inputInfo;
            m_SecretParam.MyResetEvent.Set();
            return true;
        }

        public void AnalysisKeepAlivePkg(PackageInfo package)
        {
            if (null != package && null != m_KeepAlivePackege)
            {
                if (package.Command == m_KeepAlivePackege.Command)
                {
                    m_KeepAliveEvent.Set();
                }
            }
        }
        #endregion

        #region 私有接口
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            LogHelper.logCommunication.DebugFormat("****** DataReceivedHandler 端口名：{0} 平台从串口收到数据", sp.PortName);

            if (sp.PortName != GetPortName())
            {
                LogHelper.logCommunication.InfoFormat("****** DataReceivedHandler 端口名：{0} 和当前端口{1}不一致", sp.PortName, m_Param.PortName);
                return;
            }

            var length = sp.BytesToRead;
            List<byte> list = new List<byte>();
            while (length > 0)
            {
                byte[] data = new byte[length];
                int readCount = sp.Read(data, 0, length);
                list.AddRange(data);
                length = sp.BytesToRead;
            }
            StringBuilder sbPortData = new StringBuilder();
            LogHelper.logCommunication.DebugFormat("****** DataReceivedHandler 端口名：{0} 开始接收", sp.PortName);

            int i = 0;
            foreach (var item in list)
            {
                m_RecieveQueue.Enqueue(item);

                sbPortData.Append(Convert.ToString(item, 16).PadLeft(2, '0'));
                sbPortData.Append(" ");

                if (item == 0x0a)
                {
                    // 调用回调接口
                    OnReceive(m_Param);
                    LogHelper.logCommunication.DebugFormat("****** DataReceivedHandler 端口名：{0} 接收到第{1}包串口数据", sp.PortName, ++i);
                }
            }

            LogHelper.logCommunication.DebugFormat("****** DataReceivedHandler 端口名：{0} 串口数据：{1}", sp.PortName, sbPortData.ToString());
        }

        protected void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            // 回调
            ReceiveError(e.EventType);
        }

        /// <summary>
        /// 心跳处理
        /// </summary>
        /// <param name="param"></param>
        private void KeepAliveThreadHandler(object param)
        {
            while (IsOpen())
            {
                try
                {
                    if (IsOpen())
                    {
                        SendKeepAlive();
                    }

                    if (m_KeepAliveEvent.WaitOne(m_Param.KeepAliveInterval, true))
                    {
                        m_KeepAliveEvent.Reset();
                        continue;
                    }
                    else
                    {
                        LogHelper.logCommunication.Debug("出现心跳超时，断开连接");
                        ReceiveError(CommunicateError.KeepAliveTimeout);
                        Disconnect();
                        break;
                    }
                }
                catch (ThreadAbortException abortexception)
                {
                    Disconnect();
                }
                catch (Exception e)
                {
                    LogHelper.logCommunication.Error("心跳处理出现：" + e.GetType().Name + "异常", e);
                    ReceiveError(CommunicateError.UnknownError);
                }
            }
        }

        private bool SendKeepAlive()
        {
            DataPackage package = new DataPackage();
            package.PackageInfo = m_KeepAlivePackege;

            return SendData(package);
        }
        #endregion
    }
}
