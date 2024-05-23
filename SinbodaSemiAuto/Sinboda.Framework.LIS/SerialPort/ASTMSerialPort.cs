using Sinboda.Framework.Common.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    public abstract class ASTMSerialPort : Component
    {
        #region  变量
        private const byte LF = 0x0A;      // 换行
        private const byte CR = 0x0D;      // 回车
        private const byte STX = 0x02;     // 文本开始
        private const byte ETX = 0x03;     // 文本结束帧结尾字符
        private const byte EOT = 0x04;     // 传输结束
        private const byte ENQ = 0x05;     // 查询
        private const byte ACK = 0x06;     // 应答
        private const byte NAK = 0x15;     // 无应答
        private const byte ETB = 0x17;     // 文本中间帧结尾字符

        /// <summary>
        /// 串口通讯参数
        /// </summary>
        private SerialPortParameter parameter = new SerialPortParameter();
        /// <summary>
        /// 发送线程
        /// </summary>
        private Thread _sendThread = null;
        /// <summary>
        /// 发送数据帧（字符串）队列
        /// </summary>
        protected ConcurrentQueue<string> sendMessageQueue = new ConcurrentQueue<string>();
        /// <summary>
        /// 发送数据帧（字节）队列
        /// </summary>
        protected ConcurrentQueue<byte[]> sendQueue = new ConcurrentQueue<byte[]>();
        /// <summary>
        /// 接收数据帧（字节）队列
        /// </summary>
        protected ConcurrentQueue<byte[]> receiveQueue = new ConcurrentQueue<byte[]>();
        /// <summary>
        /// 一帧数据（字节）队列
        /// </summary>
        protected ConcurrentQueue<byte[]> aFrameQueue = new ConcurrentQueue<byte[]>();
        /// <summary>
        /// 串口通讯对象
        /// </summary>
        protected SerialPort _serialPort = new SerialPort();
        /// <summary>
        /// 超时的信号
        /// </summary>
        protected ManualResetEvent sendTimeOutResetEvent = new ManualResetEvent(false);
        /// <summary>
        /// 收到NAK的信号
        /// </summary>
        protected ManualResetEvent sendFailedResetEvent = new ManualResetEvent(false);
        /// <summary>
        /// 发送信号
        /// </summary>
        protected ManualResetEvent sendResetEvent = new ManualResetEvent(false);
        /// <summary>
        /// 发送超时默认3000毫秒
        /// </summary>
        protected int timeout = 3000;
        /// <summary>
        /// 发送次数默认是3
        /// </summary>
        protected int reSendTime = 3;
        /// <summary>
        /// 帧长度
        /// </summary>
        private int frameLength = 200;
        /// <summary>
        /// 接收到数据引发的事件
        /// </summary>
        public event InceptEvent OnAccept;

        /// <summary>
        /// 发生错误引发的事件
        /// </summary>
        public event ErrorEvent OnError;
        /// <summary>
        /// 线程锁
        /// </summary>
        private Object thisLock = new Object();
        #endregion

        #region  属性
        Common.Encoding _encoding = Common.Encoding.Default;
        /// <summary>
        /// 字符编码
        /// </summary>
        public Common.Encoding Encoding
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
        ASTMMessage _sendASTMSource = null;
        /// <summary>
        /// 接收ASTM数据包后存储源
        /// </summary>
        public ASTMMessage SendASTMSource
        {
            get
            {
                return _sendASTMSource;
            }
            set
            {
                _sendASTMSource = value;
            }
        }

        private bool isSplitSend = true;

        /// <summary>
        /// 是否拆分发送，默认是true
        /// </summary>
        public bool IsSplitSend
        {
            get
            {
                return isSplitSend;
            }
            set
            {
                isSplitSend = value;
            }
        }

        /// <summary>
        /// 默认200,每一帧最多包含207个字符（包含帧开头和结尾字符），
        /// 超过200个字符的消息被分成两帧或多帧
        /// </summary>
        public int FrameLength
        {
            get
            {
                return frameLength;
            }
            set
            {
                frameLength = value;
            }
        }
        /// <summary>
        /// 串口通讯参数
        /// </summary>
        public SerialPortParameter Parameter
        {
            get
            {
                return parameter;
            }

            set
            {
                parameter = value;
            }
        }
        #endregion


        #region  方法

        public ASTMSerialPort()
        {
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            LogHelper.logLisComm.Info("【LIS底层】进入Connect");
            if (_serialPort.IsOpen)
            {
                this.Disconnect();
            }
            _serialPort.ReadBufferSize = Parameter.ReceiveBufferSize;
            _serialPort.WriteBufferSize = Parameter.SendBufferSize;
            _serialPort.PortName = Parameter.PortName;
            _serialPort.BaudRate = Parameter.BaudRate;
            _serialPort.DataBits = Parameter.DataBits;
            _serialPort.Parity = Parameter.Parity;
            try
            {
                _serialPort.StopBits = Parameter.StopBits;
                _serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceivedHandler);
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                _serialPort.Open();
                if (OnConnect != null)
                {
                    OnConnect(this);
                }

                StartThreads();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.logLisComm.Info("【LIS底层】Connect 异常：" + ex.Message);
                OnErrorEvent(new ErrorEventArgs(ex));
                OnDisconnectEvent(null);
                return false;
            }
        }
        /// <summary>
        /// 断开
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(ErrorReceivedHandler);
                    _serialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                    _serialPort.Close();
                }
                LogHelper.logLisComm.Info("【LIS底层】Disconnect");
                ClearSendQueue();
                ClearReceiveQueue();
                StopThreads();
            }
            catch (Exception e)
            {
                OnErrorEvent(new ErrorEventArgs(e));
            }
            OnDisconnectEvent(null);
        }
        /// <summary>
        /// 清空发送队列
        /// </summary>
        private void ClearSendQueue()
        {
            string str;
            while (sendMessageQueue.TryDequeue(out str))
            {

            }
            byte[] butter;
            while (sendQueue.TryDequeue(out butter))
            {

            }
        }
        /// <summary>
        /// 清空接收队列
        /// </summary>
        private void ClearReceiveQueue()
        {
            byte[] butter;
            while (receiveQueue.TryDequeue(out butter))
            {

            }

            ClearAFrameQueue();
        }

        private void ClearAFrameQueue()
        {
            byte[] butter;
            while (aFrameQueue.TryDequeue(out butter))
            {

            }
        }
        /// <summary>
        /// 开始线程
        /// </summary>
        protected void StartThreads()
        {
            sendResetEvent.Reset();
            _sendThread = new Thread(new ParameterizedThreadStart(SendThreadHandler));
            _sendThread.IsBackground = true;
            _sendThread.Start(this);
            //_receiveThread = new Thread(new ParameterizedThreadStart(ReceiveThreadHandler));
            //_receiveThread.IsBackground = true;
            //_receiveThread.Start(this);
        }
        /// <summary>
        /// 结束线程
        /// </summary>
        protected void StopThreads()
        {
            sendResetEvent.Reset();
            sendFailedResetEvent.Reset();
            sendTimeOutResetEvent.Reset();
            try
            {
                if (_sendThread != null)
                {
                    _sendThread.Abort();
                    _sendThread = null;
                }
            }
            catch (ThreadAbortException ex)
            {
                //LogHelper.logSoftWare.Error("Stop Thread", ex);
            }
        }
        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        public bool GetConnected()
        {
            return _serialPort.IsOpen;
        }
        /// <summary>
        /// 发送线程
        /// </summary>
        /// <param name="obj"></param>
        private void SendThreadHandler(object obj)
        {
            try
            {
                while (GetConnected())
                {
                    sendResetEvent.WaitOne();
                    if (!DoSendWait())
                    {
                        //LogHelper.logSoftWare.Debug("出现发送错误，断开连接");
                        Disconnect();
                        break;
                    }
                    sendResetEvent.Reset();
                }
            }
            catch (ThreadAbortException abortexception)
            {
                //LogHelper.logSoftWare.Error("SendThread Abort Exception error", abortexception);
                Disconnect();
            }
            catch (Exception e)
            {
                //LogHelper.logSoftWare.Error("发送处理出现：" + e.GetType().Name + "异常，断开连接", e);
                OnErrorEvent(new ErrorEventArgs(e));
                Disconnect();
            }
        }
        protected bool DoSendWait()
        {
            byte[] buffer;
            while (sendQueue.TryDequeue(out buffer))
            {
                if (!SendData(buffer))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">字节</param>
        /// <returns></returns>
        private bool SendData(byte[] buffer)
        {
            for (int i = 0; i < reSendTime; i++)
            {
                sendTimeOutResetEvent.Reset();
                sendFailedResetEvent.Reset();
                if (!WriteBuffer(buffer, 0, buffer.Length))
                {
                    //发送失败
                    return false;
                }

                LogHelper.logLisComm.InfoFormat(">>>>>> S >>>>>> {0}", Bytes2Str(buffer));

                //EOT结束命令发一次就行
                if (buffer[0] == EOT)
                {
                    OnInceptEvent(new InceptEventArgs(null, AnswerType.SendEOT));
                    break;
                }
                if (sendTimeOutResetEvent.WaitOne(timeout))
                {
                    //正常回复
                    if (sendFailedResetEvent.WaitOne(timeout))
                    {
                        break;
                    }
                    else
                    {
                        if (i == reSendTime - 1)
                        {
                            //发送失败
                            OnInceptEvent(new InceptEventArgs(null, AnswerType.NAK));
                            return false;
                        }
                        //收到NAK，重发
                        continue;
                    }
                }
                else
                {
                    if (i == reSendTime - 1)
                    {
                        //发送超时
                        OnInceptEvent(new InceptEventArgs(null, AnswerType.TimeOut));
                        return false;
                    }
                    //超时无回复，重发
                    continue;
                }
            }
            return true;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">ASTMMessage</param>
        public void SendASTM(ASTMMessage message)
        {
            SendASTMSource = message;
            SendASTM();
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">ASTMMessage</param>
        public void SendASTM()
        {
            if (!GetConnected())
            {
                throw new Exception("LIS is disconnect!");
            }
            if (SendASTMSource != null)
            {
                LogHelper.logLisComm.Info("【LIS底层】发送到LIS的原始数据:" + SendASTMSource.ASTMData.ToString());
                if (IsSplitSend)
                {
                    HandleSplitSendFrame();
                }
                else
                {
                    HandleNoSplitSendFrame();
                }
                Send();
            }
        }
        /// <summary>
        /// 从发送队列中获取数据帧进行发送
        /// </summary>
        private void Send()
        {
            string str = string.Empty;
            while (GetConnected() && sendMessageQueue.TryDequeue(out str))
            {
                byte[] buffer = _encoding != Common.Encoding.Default ?
                    Common.Convert.ToEncoding(Common.Encoding.Default, _encoding, System.Text.Encoding.Default.GetBytes(str)) : System.Text.Encoding.Default.GetBytes(str);
                sendQueue.Enqueue(buffer);
            }
            sendResetEvent.Set();
        }
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command)
        {
            //sendMessageQueue.Enqueue(command);
            //Send();
            byte[] buffer = _encoding != Common.Encoding.Default ?
                                         Common.Convert.ToEncoding(Common.Encoding.Default, _encoding, System.Text.Encoding.Default.GetBytes(command)) : System.Text.Encoding.Default.GetBytes(command);
            if (!WriteBuffer(buffer, 0, buffer.Length))
            {
                //发送失败
            }

            LogHelper.logLisComm.InfoFormat(">>>>>> S >>>>>>{0}", Bytes2Str(buffer));
        }
        /// <summary>
        /// 处理发送的数据帧，以拆分的方式发送，对数据帧进行拆分，封装
        /// </summary>
        private void HandleSplitSendFrame()
        {
            if (SendASTMSource.Frames == null || SendASTMSource.Frames.Count == 0) return;
            try
            {
                //ENQ
                sendMessageQueue.Enqueue(Common.Convert.GetASIIString(ASTMCommand.ENQBlockChar));
                int num = 1;
                foreach (TFrame frame in SendASTMSource.Frames)
                {
                    string frameStr = frame.Value;
                    if (string.IsNullOrEmpty(frame.Value))
                        continue;
                    int frameCount = frameStr.Length / frameLength;
                    if (frameCount * frameLength < frameStr.Length)
                    {
                        frameCount++;
                    }
                    for (int i = 0; i < frameCount; i++)
                    {
                        string temp = string.Empty;
                        string endStr = string.Empty;
                        num = num % 8;
                        if (i == frameCount - 1)
                        {
                            temp = frameStr.Substring(i * frameLength);
                            endStr = Common.Convert.GetASIIString(ASTMCommand.EndBlockChar) + Common.Convert.GetASIIString(ASTMCommand.EtxBlockChar);
                        }
                        else
                        {
                            temp = frameStr.Substring(i * frameLength, frameLength);
                            endStr = Common.Convert.GetASIIString(ASTMCommand.EtbBlockChar);
                        }
                        byte[] buffer = System.Text.Encoding.Default.GetBytes(char.Parse(num.ToString()) + temp + endStr);
                        int sum = 0;
                        for (int j = 0; j < buffer.Length; j++)
                        {
                            sum = sum + buffer[j];
                        }
                        int mod = sum % 256;

                        LogHelper.logLisComm.InfoFormat("发送之前 计算校验码{0}, 计算值{1}", mod.ToString("X2"), mod);
                        string str = Common.Convert.GetASIIString(ASTMCommand.StartBlockChar) + char.Parse(num.ToString()) + temp + endStr + mod.ToString("X2") + Common.Convert.GetASIIString(ASTMCommand.EndBlockChar) + Common.Convert.GetASIIString(ASTMCommand.LFBlockChar);
                        sendMessageQueue.Enqueue(str);
                        num++;
                    }
                }
                //EOT
                sendMessageQueue.Enqueue(Common.Convert.GetASIIString(ASTMCommand.EOTBlockChar));
            }
            catch (Exception e)
            {
                OnErrorEvent(new ErrorEventArgs(new Exception("SendFailed:" + e.Message)));
            }
        }
        /// <summary>
        /// 处理发送的数据帧，不拆分，一个数据包直接发送
        /// </summary>
        private void HandleNoSplitSendFrame()
        {
            if (SendASTMSource.Frames == null || SendASTMSource.Frames.Count == 0) return;
            try
            {
                //ENQ
                sendMessageQueue.Enqueue(Common.Convert.GetASIIString(ASTMCommand.ENQBlockChar));
                int num = 1;
                string frameStr = string.Empty;
                //int frameCount = 0;
                foreach (TFrame frame in SendASTMSource.Frames)
                {
                    if (string.IsNullOrEmpty(frame.Value))
                        continue;
                    frameStr = frameStr + frame.Value + Common.Convert.GetASIIString(ASTMCommand.EndBlockChar);
                }
                num = num % 8;
                byte[] buffer = System.Text.Encoding.Default.GetBytes(char.Parse(num.ToString()) + frameStr + Common.Convert.GetASIIString(ASTMCommand.EtxBlockChar));
                int sum = 0;
                for (int j = 0; j < buffer.Length; j++)
                {
                    sum = sum + buffer[j];
                }
                int mod = sum % 256;

                LogHelper.logLisComm.InfoFormat("发送之前 计算校验码{0}, 计算值{1}", mod.ToString("X2"), mod);
                string str = Common.Convert.GetASIIString(ASTMCommand.StartBlockChar) + char.Parse(num.ToString()) + frameStr + Common.Convert.GetASIIString(ASTMCommand.EtxBlockChar) + mod.ToString("X2") + Common.Convert.GetASIIString(ASTMCommand.EndBlockChar) + Common.Convert.GetASIIString(ASTMCommand.LFBlockChar);
                sendMessageQueue.Enqueue(str);
                //EOT
                sendMessageQueue.Enqueue(Common.Convert.GetASIIString(ASTMCommand.EOTBlockChar));
            }
            catch (Exception e)
            {
                OnErrorEvent(new ErrorEventArgs(new Exception("SendFailed:" + e.Message)));
            }
        }
        /// <summary>
        /// 处理接收的数据帧，将队列中的byte[]转成String
        /// 拼接到一个字符串中，再转成ASTMMessage
        /// </summary>
        /// <returns></returns>
        private ASTMMessage HandleReceiveFrame()
        {
            ASTMMessage receASTMMessage = new ASTMMessage();
            string recevieData = string.Empty;
            byte[] data;
            while (receiveQueue.TryDequeue(out data))
            {
                switch(Parameter.Encoding)
                {
                    case Common.Encoding.UTF8:
                        recevieData = recevieData + System.Text.Encoding.UTF8.GetString(data);
                        break;
                    case Common.Encoding.Unicode:
                        recevieData = recevieData + System.Text.Encoding.Unicode.GetString(data);
                        break;
                    case Common.Encoding.ASCII:
                        recevieData = recevieData + System.Text.Encoding.ASCII.GetString(data);
                        break;
                    case Common.Encoding.BigEndianUnicode:
                        recevieData = recevieData + System.Text.Encoding.BigEndianUnicode.GetString(data);
                        break;
                    case Common.Encoding.UTF7:
                        recevieData = recevieData + System.Text.Encoding.UTF7.GetString(data);
                        break;
                    case Common.Encoding.UTF32:
                        recevieData = recevieData + System.Text.Encoding.UTF32.GetString(data);
                        break;
                    default:
                        recevieData = recevieData + System.Text.Encoding.Default.GetString(data);
                        break;
                }
            }
            LogHelper.logLisComm.Info("【LIS底层】接收到LIS传来的原始数据:" + recevieData);
            receASTMMessage.ASTMData.CreateData = recevieData;
            return receASTMMessage;
        }
        /// <summary>
        /// 串口数据接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[Parameter.ReceiveBufferSize * 10];
            int dataLen = ReadBuffer(data, 0, data.Length);
            if (dataLen <= 0)
            {
                return;
            }
            byte[] bufferTmp = new byte[dataLen];
            Array.Copy(data, bufferTmp, dataLen);

            LogHelper.logLisComm.InfoFormat("<<<<<< R <<<<<< {0}", Bytes2Str(bufferTmp));

            //byte[] buffer = _encoding != Common.Encoding.Default ? Common.Convert.ToEncoding(_encoding, Common.Encoding.Default, bufferTmp) : bufferTmp;

            //LogHelper.logLisComm.InfoFormat("<< 转码{0}后接收数据{1}", _encoding.ToString(), Bytes2Str(buffer));
            AnalyMessage(bufferTmp);
        }

        /// <summary>
        /// 数据解析方法
        /// </summary>
        private void AnalyMessage(byte[] RecvData)
        {
            byte command = RecvData[0];
            byte[] oneFrameData = null;
            switch (command)
            {
                //ACK
                case ACK:
                    sendTimeOutResetEvent.Set();
                    sendFailedResetEvent.Set();
                    break;
                //NAK
                case NAK:
                    //重发刚才的数据，重发2次
                    sendTimeOutResetEvent.Set();
                    ClearReceiveQueue();
                    break;
                //ENQ
                case ENQ:
                    //回复ACK或者NAK
                    if (!sendQueue.IsEmpty)
                    {
                        //如果此时发送队列里有数据，要给lis回NAK
                        SendCommand(Common.Convert.GetASIIString(ASTMCommand.NAKBlockChar));
                    }
                    else
                    {
                        ClearReceiveQueue();
                        //OnInceptEvent(new InceptEventArgs(null, AnswerType.ENQ));//通知调用者ENQ，准备接收数据，接收队列清空;
                        SendCommand(Common.Convert.GetASIIString(ASTMCommand.ACKBlockChar));
                    }
                    break;
                //EOT
                case EOT:
                    sendTimeOutResetEvent.Set();
                    sendFailedResetEvent.Set();
                    //接收完毕，不回复Lis

                    OnInceptEvent(new InceptEventArgs(HandleReceiveFrame(), AnswerType.ReceEOT));//将接收的数据返回给调用者

                    break;
                //STX
                case STX:
                default:
                    oneFrameData = GetOneFrameData(RecvData);
                    if (null != oneFrameData)
                    {
                        sendTimeOutResetEvent.Set();
                        sendFailedResetEvent.Set();

                        //开始接收LIS端发送的数据,接收的一帧数据要做CS校验，如果错误返回NAK
                        if (EnqueueOneFrameDataToRecevieQueue(oneFrameData))
                        {
                            SendCommand(Common.Convert.GetASIIString(ASTMCommand.ACKBlockChar));
                        }
                        else
                        {
                            SendCommand(Common.Convert.GetASIIString(ASTMCommand.NAKBlockChar));
                            //ClearReceiveQueue();
                            ClearAFrameQueue();
                        }
                    }
                    break;
            }

        }
        /// <summary>
        /// 获取一帧数据
        /// </summary>
        /// <param name="recvData"></param>
        /// <returns></returns>
        private byte[] GetOneFrameData(byte[] recvData)
        {
            try
            {
                lock (thisLock)
                {
                    aFrameQueue.Enqueue(recvData);

                    // 判断是否有包尾
                    bool isFrameEnd = IsEndForFrameData(recvData);

                    if (isFrameEnd)
                    {
                        // 拼接接收数据
                        int dataLength = CalcOneFrameDataLength();
                        if (dataLength == 0)
                        {
                            return null;
                        }
                        byte[] frameData = new byte[dataLength];
                        byte[] tmpData;
                        int nIndex = 0;
                        while (aFrameQueue.TryDequeue(out tmpData))
                        {
                            Array.Copy(tmpData, 0, frameData, nIndex, tmpData.Length);
                            nIndex += tmpData.Length;
                        }

                        // 截取有效的数据 第一个含包头和包尾的数据
                        return CutValidFrameData(frameData);
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 将一帧数据，添加到接收队列，等待解析
        /// </summary>
        /// <param name="oneFrameData"></param>
        /// <returns></returns>
        private bool EnqueueOneFrameDataToRecevieQueue(byte[] oneFrameData)
        {
            try
            {
                lock (thisLock)
                {
                    // 校验
                    if (CheckFrameDataValid(oneFrameData))
                    {
                        receiveQueue.Enqueue(oneFrameData);
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        /// <summary>
        /// 判断是否是一帧数据的结尾
        /// </summary>
        /// <param name="recvData"></param>
        /// <returns></returns>
        private bool IsEndForFrameData(byte[] recvData)
        {
            for (int i = 0; i < recvData.Length; i++)
            {
                if (recvData[i] == LF)
                {
                    if (i == 0)
                    {
                        if (null != aFrameQueue)
                        {
                            var recvArray = aFrameQueue.ToArray();

                            // 此处是长度减2的原因，是由于已经将最新数据添加到了队列，必须找插入新数据之前最后一条数据
                            if (recvArray.Length > 1)
                            {
                                byte[] lastRecvData = recvArray[recvArray.Length - 2];
                                if (lastRecvData[lastRecvData.Length - 1] == CR)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (recvData[i - 1] == CR)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 计算一条消息报文长度
        /// </summary>
        /// <returns></returns>
        private int CalcOneFrameDataLength()
        {
            int length = 0;
            if (null == aFrameQueue)
            {
                return length;
            }

            var recevieArray = aFrameQueue.ToArray();
            if (null == recevieArray)
            {
                return length;
            }

            foreach(byte[] receiveData in recevieArray)
            {
                if (null == receiveData)
                {
                    continue;
                }

                length += receiveData.Length;
            }

            return length;
        }

        /// <summary>
        /// 一帧数据校验
        /// </summary>
        /// <param name="dataBuffer"></param>
        /// <returns></returns>
        private bool CheckFrameDataValid(byte[] dataBuffer)
        {
            if (null == dataBuffer)
            {
                return false;
            }

            if (dataBuffer.Length <= 7)
            {
                return false;
            }

            int etInx = Array.IndexOf(dataBuffer, ETB);
            if (etInx == -1)
            {
                etInx = Array.IndexOf(dataBuffer, ETX);
            }

            if (etInx != -1)
            {
                int sum = 0;
                for (int i = 1; i <= etInx; i++)
                {
                    sum = sum + dataBuffer[i];
                }
                int mod = sum % 256;
                string cs = System.Convert.ToString((char)dataBuffer[etInx + 1]);
                string cr = System.Convert.ToString((char)dataBuffer[etInx + 2]);

                LogHelper.logLisComm.InfoFormat("收到校验码{0}, 计算校验码{1}, 计算值{2}", cs + cr, mod.ToString("X2"), mod);
                if ((cs + cr).Equals(mod.ToString("X2"), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// 截取有效的数据
        /// </summary>
        /// <param name="receiveData"></param>
        /// <returns></returns>
        private byte[] CutValidFrameData(byte[] receiveData)
        {
            int startPos = FindStartPos(receiveData);
            int endPos = FindEndPos(receiveData);

            if (endPos <= startPos)
            {
                return null;
            }

            int validLength = endPos - startPos + 1;

            if (validLength != receiveData.Length)
            {
                byte[] frameData = new byte[validLength];

                Array.Copy(receiveData, startPos, frameData, 0, validLength);

                LogHelper.logLisComm.InfoFormat("切包后数据 {0}", Bytes2Str(frameData));
                return frameData;
            }

            return receiveData;
        }

        private int FindStartPos(byte[] recvData)
        {
            for (int i = 0; i < recvData.Length; i++)
            {
                if (recvData[i] == STX)
                {
                    return i;
                }
            }

            return recvData.Length;
        }

        private int FindEndPos(byte[] recvData)
        {
            for (int i = 0; i < recvData.Length; i++)
            {
                if (recvData[i] == LF)
                {
                    if (i > 1 && recvData[i - 1] == CR)
                    {
                        return i;
                    }
                }
            }

            return 0;
        }
        /// <summary>
        /// 串口接收异常的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialError.RXParity:
                case SerialError.Frame:
                    OnErrorEvent(new ErrorEventArgs(new Exception("CheckDataError")));
                    break;
                case SerialError.Overrun:
                    OnErrorEvent(new ErrorEventArgs(new Exception("UnknownError")));
                    break;
                case SerialError.RXOver:
                    OnErrorEvent(new ErrorEventArgs(new Exception("ReceiveFailed")));
                    break;
                case SerialError.TXFull:
                    OnErrorEvent(new ErrorEventArgs(new Exception("SendFailed")));
                    break;
                default:
                    break;
            }
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
        protected virtual void OnErrorEvent(ErrorEventArgs e)
        {
            if (OnError != null)
            {
                OnError(this, e);
            }
        }
        protected virtual void OnDisconnectEvent(SerialPort workPort)
        {
            if (OnDisconnect != null)
            {
                OnDisconnect(workPort);
            }
        }
        /// <summary>
        /// 发数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected bool WriteBuffer(byte[] buffer, int offset, int count)
        {
            try
            {
                _serialPort.Write(buffer, offset, count);
                return true;
            }
            catch (Exception ex)
            {
                OnErrorEvent(new ErrorEventArgs(ex));
                return false;
            }
        }
        /// <summary>
        /// 收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected int ReadBuffer(byte[] buffer, int offset, int count)
        {
            try
            {
                return _serialPort.Read(buffer, offset, count);
            }
            catch (Exception ex)
            {
                OnErrorEvent(new ErrorEventArgs(ex));
                return -1;
            }
        }

        public string Bytes2Str(byte[] bs)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var b in bs)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                sb.Append(" ");
            }

            return sb.ToString();
        }

        #endregion


        #region 事件
        public event ConnectEvent OnConnect;
        public event DisconnectEvent OnDisconnect;

        //public event TimeoutEvent OnTimeout;
        //public delegate void ConnectEvent(object sender);
        //public delegate void DisconnectEvent(object sender);
        //public delegate void TimeoutEvent(object sender);
        #endregion
    }
}
